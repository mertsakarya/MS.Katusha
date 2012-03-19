using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Services;

namespace MS.Katusha.Web.Helpers
{
    public static class DependencyHelper
    {
        public static void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            builder.RegisterType<ResourceManager>().As<IResourceManager>().SingleInstance();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerHttpRequest();
            builder.RegisterType<GirlService>().As<IGirlService>().InstancePerHttpRequest();
            builder.RegisterType<BoyService>().As<IBoyService>().InstancePerHttpRequest();
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().InstancePerHttpRequest();

            builder.RegisterType<UserRepositoryDB>().As<IUserRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<GirlRepositoryDB>().As<IGirlRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<BoyRepositoryDB>().As<IBoyRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<CountriesToVisitRepositoryDB>().As<ICountriesToVisitRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<SearchingForRepositoryDB>().As<ISearchingForRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<LanguagesSpokenRepositoryDB>().As<ILanguagesSpokenRepositoryDB>().InstancePerHttpRequest();
            builder.RegisterType<PhotoRepositoryDB>().As<IPhotoRepositoryDB>().InstancePerHttpRequest();

            builder.RegisterType<KatushaDbContext>().As<IKatushaDbContext>().InstancePerHttpRequest();
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}