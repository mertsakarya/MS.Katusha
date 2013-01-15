using System;
using System.Configuration;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using MS.Katusha.FileSystems;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Interfaces.Services.Helpers;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.DB.Base;
using MS.Katusha.Repositories.DB.Context;
using MS.Katusha.Repositories.RavenDB;
using MS.Katusha.Services;
using Raven.Client.Document;
using Raven.Client;

//using ServiceStack.Redis;

namespace MS.Katusha.DependencyManagement
{
    public static class DependencyRegistrar
    {
        public static IContainer Container;

        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            Build(builder);
            Container = builder.Build();
            return Container;
        }

        public static void Build(ContainerBuilder builder, bool noHttpContext = false)
        {
            // InstancePerHttpRequest
            // SingleInstance
            builder.RegisterType<KatushaDbContext>().As<IKatushaDbContext>().InstancePerDependency();

            builder.RegisterType<ResourceService>().As<IResourceService>().SingleInstance();
            
            builder.RegisterType<EncryptionService>().As<IEncryptionService>().SingleInstance();
            builder.RegisterType<TicketService>().As<ITicketService>().SingleInstance();
            builder.RegisterType<S3FileSystem>().As<IKatushaFileSystem>().SingleInstance();

            builder.RegisterType<KatushaRavenStore>().As<IDocumentStore>().SingleInstance();
            builder.RegisterType<ProfileRepositoryRavenDB>().As<IProfileRepositoryRavenDB>().SingleInstance();
            builder.RegisterType<VisitRepositoryRavenDB>().As<IVisitRepositoryRavenDB>().SingleInstance();
            builder.RegisterType<ConversationRepositoryRavenDB>().As<IConversationRepositoryRavenDB>().SingleInstance();
            builder.RegisterType<StateRepositoryRavenDB>().As<IStateRepositoryRavenDB>().SingleInstance();
            builder.RegisterType<CountryCityCountRepositoryRavenDB>().As<ICountryCityCountRepositoryRavenDB>().SingleInstance();

            var cacheProviderText = ConfigurationManager.AppSettings["CacheProvider"];
            if (!String.IsNullOrWhiteSpace(cacheProviderText)) {
                switch (cacheProviderText.ToLowerInvariant()) {
                    case "ravendb":
                        builder.RegisterType<KatushaGlobalRavenCacheContext>().WithParameter(new TypedParameter(typeof(IKatushaGlobalCacheContext), null)).As<IKatushaGlobalCacheContext>().SingleInstance();
                        break;
                    default:
                        builder.RegisterType<KatushaGlobalMemoryCacheContext>().WithParameter(new TypedParameter(typeof(IKatushaGlobalCacheContext), null)).As<IKatushaGlobalCacheContext>().SingleInstance();
                        break;
                }
            } else builder.RegisterType<KatushaGlobalMemoryCacheContext>().As<IKatushaGlobalCacheContext>().SingleInstance();
            builder.RegisterType<CacheObjectRepositoryRavenDB>().As<IRepository<CacheObject>>().SingleInstance();

            builder.RegisterType<UserService>().As<IUserService>().InstancePerHttpRequest();
            builder.RegisterType<ProfileService>().As<IProfileService>().InstancePerHttpRequest();
            builder.RegisterType<SearchService>().As<ISearchService>().InstancePerHttpRequest();
            builder.RegisterType<ConversationService>().As<IConversationService>().InstancePerHttpRequest();
            builder.RegisterType<PhotosService>().As<IPhotosService>().InstancePerHttpRequest();
            builder.RegisterType<S3PhotoBackupService>().As<IPhotoBackupService>().InstancePerHttpRequest();
            builder.RegisterType<VisitService>().As<IVisitService>().InstancePerHttpRequest();
            builder.RegisterType<StateService>().As<IStateService>().InstancePerHttpRequest();
            builder.RegisterType<SamplesService>().As<ISamplesService>().InstancePerHttpRequest();
            builder.RegisterType<UtilityService>().As<IUtilityService>().InstancePerHttpRequest();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerHttpRequest();

            builder.RegisterType<PaypalService>().As<IPaypalService>().InstancePerHttpRequest();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerHttpRequest();

            builder.RegisterType<ConversationRepositoryDB>().As<IConversationRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<UserRepositoryDB>().As<IUserRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<ProfileRepositoryDB>().As<IProfileRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<CountriesToVisitRepositoryDB>().As<ICountriesToVisitRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<SearchingForRepositoryDB>().As<ISearchingForRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<LanguagesSpokenRepositoryDB>().As<ILanguagesSpokenRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<PhotoBackupRepositoryDB>().As<IPhotoBackupRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<PhotoRepositoryDB>().As<IPhotoRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<VisitRepositoryDB>().As<IVisitRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<StateRepositoryDB>().As<IStateRepositoryDB>().InstancePerHttpRequest();

            builder.RegisterGeneric(typeof(GridService<>)).As(typeof(IGridService<>)).InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(RepositoryDB<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();

            MapperHelper.HandleMappings();
        }
        public static object RegisterGlimpse()
        {
            var store = DependencyResolver.Current.GetService<IDocumentStore>() as DocumentStore;
            if (store == null) return null;
            Glimpse.RavenDb.Profiler.AttachTo(store);
            Glimpse.RavenDb.Profiler.HideFields("PasswordHash", "PasswordSalt");
            return store;
        }
    }
}
