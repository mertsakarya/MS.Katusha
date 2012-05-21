using System;
using System.Configuration;
using System.Globalization;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using MS.Katusha.Domain;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Redis;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.RavenDB;
using MS.Katusha.Services;
using MS.Katusha.Web.Helpers.Binders;
using MS.Katusha.Web.Models.Entities;
using NLog;
using ServiceStack.Redis;

namespace MS.Katusha.Web.Helpers
{
    public static class DependencyHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public const int GlobalPageSize = 40;
        public static IContainer Container;
        public static readonly string PhotosFolder = HttpContext.Current.Server.MapPath("/Photos");

        public static void RegisterDependencies()
        {
            const string redisUrlName = "REDISTOGO_URL";
            const string appHarborRedisToGo = "redis://redistogo-appharbor:";
            var redisUrl = ConfigurationManager.AppSettings.Get(redisUrlName);
            if(redisUrl.IndexOf(appHarborRedisToGo, System.StringComparison.Ordinal) >= 0) {
                redisUrl = redisUrl.Substring(appHarborRedisToGo.Length);
            }
            Uri redisUri;
            if(!Uri.TryCreate(redisUrl, UriKind.RelativeOrAbsolute,  out redisUri)) {
                throw new ArgumentException(redisUrl.ToString(CultureInfo.InvariantCulture));
            }
            var redisUrls = new string [] {redisUri.ToString()};
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<KatushaRavenStore>().As<IKatushaRavenStore>().SingleInstance();
            builder.RegisterType<PooledRedisClientManager>().As<IRedisClientsManager>().WithParameter("readWriteHosts", redisUrls).SingleInstance();
            builder.RegisterType<KatushaDbContext>().As<IKatushaDbContext>().InstancePerHttpRequest();

            builder.RegisterType<CountryCityCountRepositoryRavenDB>().As<ICountryCityCountRepositoryRavenDB>().InstancePerHttpRequest();
            builder.RegisterType<ResourceService>().As<IResourceService>().InstancePerHttpRequest();

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
            var cacheProviderText = ConfigurationManager.AppSettings["CacheProvider"];
            if (!String.IsNullOrWhiteSpace(cacheProviderText)) {
                switch(cacheProviderText.ToLowerInvariant()) {
                    case "redis":
                        builder.RegisterType<KatushaGlobalRedisCacheContext>().As<IKatushaGlobalCacheContext>().InstancePerHttpRequest();
                        break;
                    case "ravendb":
                        builder.RegisterType<KatushaGlobalRavenCacheContext>().As<IKatushaGlobalCacheContext>().InstancePerHttpRequest();
                        break;
                    default:
                        builder.RegisterType<KatushaGlobalMemoryCacheContext>().As<IKatushaGlobalCacheContext>().InstancePerHttpRequest();
                        break;
                }

            } else {
                builder.RegisterType<KatushaGlobalMemoryCacheContext>().As<IKatushaGlobalCacheContext>().InstancePerHttpRequest();
            }
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
            

            Container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));

            ModelBinders.Binders[typeof(SearchProfileCriteriaModel)] = new SearchCriteriaBinder();
            ModelBinders.Binders[typeof(SearchStateCriteriaModel)] = new SearchCriteriaBinder();
            ModelBinders.Binders[typeof(ProfileModel)] = new ProfileModelBinder();
            ModelBinders.Binders[typeof(FacebookProfileModel)] = new FacebookProfileModelBinder();


#if DEBUG
            Logger.Info("Dependencies resolved");
#endif
        }
    }
}