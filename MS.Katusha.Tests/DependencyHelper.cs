using Autofac;
using MS.Katusha.Domain;
using MS.Katusha.Infrastructure;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.RavenDB;
using MS.Katusha.Services;
using Raven.Client;

namespace MS.Katusha.Tests
{

    public static class DependencyHelper
    {
        public const int GlobalPageSize = 40;
        public static IContainer Container;

        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<ResourceManager>().As<IResourceManager>().SingleInstance();

            builder.RegisterType<UserService>().As<IUserService>().SingleInstance();
            builder.RegisterType<ProfileService>().As<IProfileService>().SingleInstance();
            builder.RegisterType<SearchService>().As<ISearchService>().SingleInstance();
            builder.RegisterType<ConversationService>().As<IConversationService>().SingleInstance();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterType<PhotosService>().As<IPhotosService>().SingleInstance();
            builder.RegisterType<PhotoBackupService>().As<IPhotoBackupService>().SingleInstance();
            builder.RegisterType<VisitService>().As<IVisitService>().SingleInstance();
            builder.RegisterType<StateService>().As<IStateService>().SingleInstance();
            builder.RegisterType<SamplesService>().As<ISamplesService>().SingleInstance();
            builder.RegisterType<LocationService>().As<ILocationService>().SingleInstance();

            builder.RegisterType<KatushaGlobalMemoryCacheContext>().As<IKatushaGlobalCacheContext>().SingleInstance();
            //builder.RegisterType<KatushaRavenCacheContext>().As<IKatushaCacheContext>().SingleInstance();
            builder.RegisterType<CacheObjectRepositoryRavenDB>().WithParameter(new TypedParameter(typeof(IDocumentStore), RavenHelper.RavenStore)).As<IRepository<CacheObject>>().SingleInstance();
            builder.RegisterType<ProfileRepositoryRavenDB>().WithParameter(new TypedParameter(typeof(IDocumentStore), RavenHelper.RavenStore)).As<IProfileRepositoryRavenDB>().SingleInstance();
            builder.RegisterType<VisitRepositoryRavenDB>().WithParameter(new TypedParameter(typeof(IDocumentStore), RavenHelper.RavenStore)).As<IVisitRepositoryRavenDB>().SingleInstance();
            builder.RegisterType<ConversationRepositoryRavenDB>().WithParameter(new TypedParameter(typeof(IDocumentStore), RavenHelper.RavenStore)).As<IConversationRepositoryRavenDB>().SingleInstance();
            builder.RegisterType<StateRepositoryRavenDB>().WithParameter(new TypedParameter(typeof(IDocumentStore), RavenHelper.RavenStore)).As<IStateRepositoryRavenDB>().SingleInstance();

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

            builder.RegisterType<KatushaDbContext>().As<IKatushaDbContext>().SingleInstance();
            Container = builder.Build();
            
        }
    }
}