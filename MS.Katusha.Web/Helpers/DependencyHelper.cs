using System;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Features.OwnedInstances;
using Autofac.Integration.Mvc;
using MS.Katusha.Domain;
using MS.Katusha.Infrastructure;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.RavenDB;
using MS.Katusha.Services;
using MS.Katusha.Web.Models.Entities;
using NLog;
using Quartz;
using Raven.Client;

namespace MS.Katusha.Web.Helpers
{
    public static class DependencyHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public const int GlobalPageSize = 15;
        public static readonly string RootFolder = HttpContext.Current.Server.MapPath(@"~\");

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<ResourceManager>().As<IResourceManager>().SingleInstance();

            builder.RegisterType<UserService>().As<IUserService>().InstancePerHttpRequest();
            builder.RegisterType<ProfileService>().As<IProfileService>().InstancePerHttpRequest();
            builder.RegisterType<SearchService>().As<ISearchService>().InstancePerHttpRequest();
            builder.RegisterType<ConversationService>().As<IConversationService>().InstancePerHttpRequest();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().InstancePerHttpRequest();
            builder.RegisterType<PhotosService>().As<IPhotosService>().InstancePerHttpRequest();
            builder.RegisterType<VisitService>().As<IVisitService>().InstancePerHttpRequest();
            builder.RegisterType<StateService>().As<IStateService>().InstancePerHttpRequest();
            builder.RegisterType<SamplesService>().As<ISamplesService>().InstancePerHttpRequest();

            builder.RegisterType<KatushaGlobalMemoryCacheContext>().As<IKatushaGlobalCacheContext>().InstancePerHttpRequest();
            //builder.RegisterType<KatushaRavenCacheContext>().As<IKatushaCacheContext>().InstancePerHttpRequest();
            builder.RegisterType<CacheObjectRepositoryRavenDB>().WithParameter(new TypedParameter(typeof(IDocumentStore), RavenHelper.RavenStore)).As<IRepository<CacheObject>>().InstancePerHttpRequest();
            builder.RegisterType<ProfileRepositoryRavenDB>().WithParameter(new TypedParameter(typeof(IDocumentStore), RavenHelper.RavenStore)).As<IProfileRepositoryRavenDB>().InstancePerHttpRequest();
            builder.RegisterType<VisitRepositoryRavenDB>().WithParameter(new TypedParameter(typeof(IDocumentStore), RavenHelper.RavenStore)).As<IVisitRepositoryRavenDB>().InstancePerHttpRequest();
            builder.RegisterType<ConversationRepositoryRavenDB>().WithParameter(new TypedParameter(typeof(IDocumentStore), RavenHelper.RavenStore)).As<IConversationRepositoryRavenDB>().InstancePerHttpRequest();
            builder.RegisterType<StateRepositoryRavenDB>().WithParameter(new TypedParameter(typeof(IDocumentStore), RavenHelper.RavenStore)).As<IStateRepositoryRavenDB>().InstancePerHttpRequest();
            
            builder.RegisterType<ConversationRepositoryDB>().As<IConversationRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<UserRepositoryDB>().As<IUserRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<ProfileRepositoryDB>().As<IProfileRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<CountriesToVisitRepositoryDB>().As<ICountriesToVisitRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<SearchingForRepositoryDB>().As<ISearchingForRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<LanguagesSpokenRepositoryDB>().As<ILanguagesSpokenRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<PhotoRepositoryDB>().As<IPhotoRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<VisitRepositoryDB>().As<IVisitRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<StateRepositoryDB>().As<IStateRepositoryDB>().InstancePerHttpRequest();

            builder.RegisterType<KatushaDbContext>().As<IKatushaDbContext>().InstancePerHttpRequest();

            //builder.RegisterGeneric(typeof(JobWrapper<>));
            //builder.RegisterType<SomeJob>();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            ModelBinders.Binders[typeof(SearchCriteriaModel)] = new SearchCriteriaBinder();

            //var job = container.Resolve<JobWrapper<SomeJob>>();

#if DEBUG
            Logger.Info("Dependencies resolved");
#endif
        }
    }

    public class JobWrapper<T> : IJob where T : IJob
    {
        private readonly Func<Owned<T>> _jobFactory;

        public JobWrapper(Func<Owned<T>> jobFactory)
        {
            _jobFactory = jobFactory;
        }

        public void Execute(IJobExecutionContext context) { 
            using (var ownedJob = _jobFactory()) {
                var theJob = ownedJob.Value;
                theJob.Execute(context);
            }
        }
    }
}