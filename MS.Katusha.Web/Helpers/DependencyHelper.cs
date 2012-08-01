using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using MS.Katusha.DependencyManagement;
using MS.Katusha.Domain;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Redis;
using MS.Katusha.Repositories.DB;
using MS.Katusha.Repositories.DB.Base;
using MS.Katusha.Repositories.RavenDB;
using MS.Katusha.Services;
using MS.Katusha.Web.Helpers.Binders;
using MS.Katusha.Web.Models;
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
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            DependencyRegistrar.Build(builder);
            Container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));

            ModelBinders.Binders[typeof(SearchProfileCriteriaModel)] = new SearchCriteriaBinder();
            ModelBinders.Binders[typeof(SearchStateCriteriaModel)] = new SearchCriteriaBinder();
            ModelBinders.Binders[typeof(ProfileModel)] = new ProfileModelBinder();
            ModelBinders.Binders[typeof(RegisterModel)] = new ProfileModelBinder();
            ModelBinders.Binders[typeof(FacebookProfileModel)] = new FacebookProfileModelBinder();


#if DEBUG
            Logger.Info("Dependencies resolved");
#endif
        }

    }
}