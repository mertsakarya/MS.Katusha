using System;
using System.Configuration;
using Autofac;
using Autofac.Integration.Mvc;
using MS.Katusha.Domain;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Redis;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.DB.Base;
using MS.Katusha.Repositories.RavenDB;
using MS.Katusha.Services;
using ServiceStack.Redis;

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
            const string redisUrlName = "REDISTOGO_URL";
            const string appHarborRedisToGo = "redis://redistogo-appharbor:";
            var redisUrl = ConfigurationManager.AppSettings.Get(redisUrlName);
            if (redisUrl.IndexOf(appHarborRedisToGo, StringComparison.Ordinal) >= 0) {
                // VERY BAD thing for appharbor
                redisUrl = "ab3740aa6f5b0b2d567f7279ae6e2159@lab.redistogo.com:9071"; //redisUrl.Substring(appHarborRedisToGo.Length);
            }
            Uri redisUri;
            if (!Uri.TryCreate(redisUrl, UriKind.RelativeOrAbsolute, out redisUri)) {
                throw new ArgumentException("WRONG REDIS STRING");
            }
            var redisUrls = new[] { redisUri.ToString() };


            builder.RegisterType<KatushaRavenStore>().As<IKatushaRavenStore>().InstancePerHttpRequest();
            builder.RegisterType<PooledRedisClientManager>().As<IRedisClientsManager>().WithParameter("readWriteHosts", redisUrls).InstancePerHttpRequest();
            //InstancePerHttpRequest
            builder.RegisterType<KatushaDbContext>().As<IKatushaDbContext>().InstancePerHttpRequest();

            builder.RegisterType<CountryCityCountRepositoryRavenDB>().As<ICountryCityCountRepositoryRavenDB>().InstancePerHttpRequest();
            builder.RegisterType<ResourceService>().As<IResourceService>().InstancePerHttpRequest();

            builder.RegisterType<PaypalServiceSOAP>().As<IPaypalService>().InstancePerHttpRequest();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerHttpRequest();
            builder.RegisterType<ProfileService>().As<IProfileService>().InstancePerHttpRequest();
            builder.RegisterType<SearchService>().As<ISearchService>().InstancePerHttpRequest();
            builder.RegisterType<ConversationService>().As<IConversationService>().InstancePerHttpRequest();
            builder.RegisterType<PhotosService>().As<IPhotosService>().InstancePerHttpRequest();
            builder.RegisterType<PhotoBackupService>().As<IPhotoBackupService>().InstancePerHttpRequest();
            builder.RegisterType<VisitService>().As<IVisitService>().InstancePerHttpRequest();
            builder.RegisterType<StateService>().As<IStateService>().InstancePerHttpRequest();
            builder.RegisterType<SamplesService>().As<ISamplesService>().InstancePerHttpRequest();
            builder.RegisterType<UtilityService>().As<IUtilityService>().InstancePerHttpRequest();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerHttpRequest();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerHttpRequest();
            var cacheProviderText = ConfigurationManager.AppSettings["CacheProvider"];
            if (!String.IsNullOrWhiteSpace(cacheProviderText)) {
                switch (cacheProviderText.ToLowerInvariant()) {
                    case "redis":
                        builder.RegisterType<KatushaGlobalRedisCacheContext>().WithParameter(new TypedParameter(typeof(IKatushaGlobalCacheContext), null)).As<IKatushaGlobalCacheContext>().InstancePerHttpRequest();
                        break;
                    case "ravendb":
                        builder.RegisterType<KatushaGlobalRavenCacheContext>().WithParameter(new TypedParameter(typeof(IKatushaGlobalCacheContext), null)).As<IKatushaGlobalCacheContext>().InstancePerHttpRequest();
                        break;
                    default:
                        builder.RegisterType<KatushaGlobalMemoryCacheContext>().WithParameter(new TypedParameter(typeof(IKatushaGlobalCacheContext), null)).As<IKatushaGlobalCacheContext>().InstancePerHttpRequest();
                        break;
                }
            } else builder.RegisterType<KatushaGlobalMemoryCacheContext>().As<IKatushaGlobalCacheContext>().InstancePerHttpRequest();
            builder.RegisterType<CacheObjectRepositoryRavenDB>().As<IRepository<CacheObject>>().InstancePerHttpRequest();
            builder.RegisterType<ProfileRepositoryRavenDB>().As<IProfileRepositoryRavenDB>().InstancePerHttpRequest();
            builder.RegisterType<VisitRepositoryRavenDB>().As<IVisitRepositoryRavenDB>().InstancePerHttpRequest();
            builder.RegisterType<ConversationRepositoryRavenDB>().As<IConversationRepositoryRavenDB>().InstancePerHttpRequest();
            builder.RegisterType<StateRepositoryRavenDB>().As<IStateRepositoryRavenDB>().InstancePerHttpRequest();

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

            //builder.RegisterType<GridService<User>>().As<IGridService<User>>().InstancePerHttpRequest();
            //builder.RegisterType<UserRepositoryDB>().As<IRepository<User>>().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(GridService<>)).As(typeof(IGridService<>)).InstancePerHttpRequest();
            builder.RegisterGeneric(typeof(RepositoryDB<>)).As(typeof(IRepository<>)).InstancePerHttpRequest();
        }

    }
}
