using System;
using System.Configuration;
using Autofac;
using MS.Katusha.Domain;
using MS.Katusha.FileSystems;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.DB.Base;
using MS.Katusha.Repositories.RavenDB;
using MS.Katusha.Services;
using MS.Katusha.Services.Encryption;

//using ServiceStack.Redis;

namespace MS.Katusha.DependencyManagement
{
    public static class DependencyRegistrar
    {
        public static IContainer Container;

        public static IContainer BuildContainer()
        {
            var builder = new ContainerBuilder();
            DependencyRegistrar.Build(builder);
            Container = builder.Build();
            return Container;
        }

        public static void Build(ContainerBuilder builder, bool noHttpContext = false)
        {
            // InstancePerHttpRequest
            // SingleInstance

            builder.RegisterType<EncryptionService>().As<IEncryptionService>().SingleInstance();
            builder.RegisterType<TicketService>().As<ITicketService>().SingleInstance();

            builder.RegisterType<KatushaRavenStore>().As<IKatushaRavenStore>().SingleInstance();
            builder.RegisterType<KatushaDbContext>().As<IKatushaDbContext>().SingleInstance();
            builder.RegisterType<S3FileSystem>().As<IKatushaFileSystem>().SingleInstance();

            builder.RegisterType<CountryCityCountRepositoryRavenDB>().As<ICountryCityCountRepositoryRavenDB>().SingleInstance();
            builder.RegisterType<ResourceService>().As<IResourceService>().SingleInstance();

            builder.RegisterType<PaypalService>().As<IPaypalService>().SingleInstance();
            builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
            builder.RegisterType<ProfileService>().As<IProfileService>().SingleInstance();
            builder.RegisterType<SearchService>().As<ISearchService>().SingleInstance();
            builder.RegisterType<ConversationService>().As<IConversationService>().SingleInstance();
            builder.RegisterType<PhotosService>().As<IPhotosService>().SingleInstance();
            builder.RegisterType<S3PhotoBackupService>().As<IPhotoBackupService>().SingleInstance();
            builder.RegisterType<VisitService>().As<IVisitService>().SingleInstance();
            builder.RegisterType<StateService>().As<IStateService>().SingleInstance();
            builder.RegisterType<SamplesService>().As<ISamplesService>().SingleInstance();
            builder.RegisterType<UtilityService>().As<IUtilityService>().SingleInstance();
            builder.RegisterType<NotificationService>().As<INotificationService>().SingleInstance();
            builder.RegisterType<ProductService>().As<IProductService>().SingleInstance();
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
            builder.RegisterType<ProfileRepositoryRavenDB>().As<IProfileRepositoryRavenDB>().SingleInstance();
            builder.RegisterType<VisitRepositoryRavenDB>().As<IVisitRepositoryRavenDB>().SingleInstance();
            builder.RegisterType<ConversationRepositoryRavenDB>().As<IConversationRepositoryRavenDB>().SingleInstance();
            builder.RegisterType<StateRepositoryRavenDB>().As<IStateRepositoryRavenDB>().SingleInstance();

            builder.RegisterType<ConversationRepositoryDB>().As<IConversationRepositoryDB>().SingleInstance();
            builder.RegisterType<UserRepositoryDB>().As<IUserRepositoryDB>().SingleInstance();
            builder.RegisterType<ProfileRepositoryDB>().As<IProfileRepositoryDB>().SingleInstance();
            builder.RegisterType<CountriesToVisitRepositoryDB>().As<ICountriesToVisitRepositoryDB>().SingleInstance();
            builder.RegisterType<SearchingForRepositoryDB>().As<ISearchingForRepositoryDB>().SingleInstance();
            builder.RegisterType<LanguagesSpokenRepositoryDB>().As<ILanguagesSpokenRepositoryDB>().SingleInstance();
            builder.RegisterType<PhotoBackupRepositoryDB>().As<IPhotoBackupRepositoryDB>().SingleInstance();
            builder.RegisterType<PhotoRepositoryDB>().As<IPhotoRepositoryDB>().SingleInstance();
            builder.RegisterType<VisitRepositoryDB>().As<IVisitRepositoryDB>().SingleInstance();
            builder.RegisterType<StateRepositoryDB>().As<IStateRepositoryDB>().SingleInstance();

            builder.RegisterGeneric(typeof(GridService<>)).As(typeof(IGridService<>)).SingleInstance();
            builder.RegisterGeneric(typeof(RepositoryDB<>)).As(typeof(IRepository<>)).SingleInstance();
        }
    }
}
