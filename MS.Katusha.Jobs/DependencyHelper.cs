using System.Reflection;
using System.Web;
using Autofac;
using MS.Katusha.Domain;
using MS.Katusha.Infrastructure;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.RavenDB;
using MS.Katusha.Services;
using Quartz;
using Raven.Client;

namespace MS.Katusha.Jobs
{
    public static class DependencyHelper
    {
        public static IContainer Container;

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ResourceService>().As<IResourceService>().SingleInstance();
            builder.RegisterType<KatushaRavenStore>().As<IKatushaRavenStore>().InstancePerLifetimeScope();
            builder.RegisterType<KatushaDbContext>().As<IKatushaDbContext>().InstancePerLifetimeScope();


            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileService>().As<IProfileService>().InstancePerLifetimeScope();
            builder.RegisterType<SearchService>().As<ISearchService>().InstancePerLifetimeScope();
            builder.RegisterType<ConversationService>().As<IConversationService>().InstancePerLifetimeScope();
            builder.RegisterType<PhotosService>().As<IPhotosService>().InstancePerLifetimeScope();
            builder.RegisterType<VisitService>().As<IVisitService>().InstancePerLifetimeScope();
            builder.RegisterType<StateService>().As<IStateService>().InstancePerLifetimeScope();
            builder.RegisterType<SamplesService>().As<ISamplesService>().InstancePerLifetimeScope();

            builder.RegisterType<KatushaGlobalMemoryCacheContext>().As<IKatushaGlobalCacheContext>().InstancePerLifetimeScope();
            //builder.RegisterType<KatushaRavenCacheContext>().As<IKatushaCacheContext>().InstancePerLifetimeScope();
            builder.RegisterType<CacheObjectRepositoryRavenDB>().As<IRepository<CacheObject>>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileRepositoryRavenDB>().As<IProfileRepositoryRavenDB>().InstancePerLifetimeScope();
            builder.RegisterType<VisitRepositoryRavenDB>().As<IVisitRepositoryRavenDB>().InstancePerLifetimeScope();
            builder.RegisterType<ConversationRepositoryRavenDB>().As<IConversationRepositoryRavenDB>().InstancePerLifetimeScope();
            builder.RegisterType<StateRepositoryRavenDB>().As<IStateRepositoryRavenDB>().InstancePerLifetimeScope();
            builder.RegisterType<CountryCityCountRepositoryRavenDB>().As<ICountryCityCountRepositoryRavenDB>().InstancePerLifetimeScope();
            
            builder.RegisterType<ConversationRepositoryDB>().As<IConversationRepositoryDB>().InstancePerLifetimeScope();
            builder.RegisterType<UserRepositoryDB>().As<IUserRepositoryDB>().InstancePerLifetimeScope();
            builder.RegisterType<ProfileRepositoryDB>().As<IProfileRepositoryDB>().InstancePerLifetimeScope();
            builder.RegisterType<CountriesToVisitRepositoryDB>().As<ICountriesToVisitRepositoryDB>().InstancePerLifetimeScope();
            builder.RegisterType<SearchingForRepositoryDB>().As<ISearchingForRepositoryDB>().InstancePerLifetimeScope();
            builder.RegisterType<LanguagesSpokenRepositoryDB>().As<ILanguagesSpokenRepositoryDB>().InstancePerLifetimeScope();
            builder.RegisterType<PhotoRepositoryDB>().As<IPhotoRepositoryDB>().InstancePerLifetimeScope();
            builder.RegisterType<VisitRepositoryDB>().As<IVisitRepositoryDB>().InstancePerLifetimeScope();
            builder.RegisterType<StateRepositoryDB>().As<IStateRepositoryDB>().InstancePerLifetimeScope();

            var dataAccess = Assembly.GetExecutingAssembly();
            
            //builder.RegisterAssemblyTypes(dataAccess).Where(t => t.Name.EndsWith("Job")).SingleInstance();
            builder.RegisterAssemblyTypes(dataAccess).Where(t => typeof(IJob).IsAssignableFrom(t)).SingleInstance();
                //.AsImplementedInterfaces();
            Container = builder.Build();
        }
    }
}