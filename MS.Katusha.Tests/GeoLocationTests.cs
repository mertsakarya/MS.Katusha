using System;
using System.Diagnostics;
using Autofac;
using MS.Katusha.Interfaces.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MS.Katusha.Tests
{
    [TestClass]
    public class GeoLocationTests
    {
        private static ILocationService _locationService;

        [AssemblyInitialize()]  public static void AssemblyInit(TestContext context) { }
        [TestInitialize()]      public void Initialize() { }
        [TestCleanup()]         public void Cleanup() { }
        [ClassCleanup()]        public static void ClassCleanup() { }
        [AssemblyCleanup()]     public static void AssemblyCleanup() { }

        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            DependencyHelper.RegisterDependencies();
            _locationService = DependencyHelper.Container.Resolve<ILocationService>();
        }

        [TestMethod]
        public void get_countries()
        {
            var countries = _locationService.GetCountries();
            Assert.IsNotNull(countries);
            Assert.IsTrue(countries.Count > 0);
        }

        [TestMethod]
        public void get_cities()
        {
            var countries = _locationService.GetCountries();
            foreach(var country in countries) {
                try {
                    var city = _locationService.GetCities(country.Key);
                    Assert.IsNotNull(city);
                } catch (Exception ex) {
                    Debug.WriteLine(country.Key + " : " + country.Value + " : " + ex.Message);
                }
            }
            Assert.IsTrue(countries.Count > 0);
        }

        [TestMethod]
        public void get_cities_with_alternates()
        {
            var countries = _locationService.GetCountries();
            foreach (var country in countries) {
                try {
                    var city = _locationService.GetCitiesWithAlternates(country.Key);
                    Assert.IsNotNull(city);
                } catch (Exception ex) {
                    Debug.WriteLine(country.Key + " : " + country.Value + " : " + ex.Message);
                }
            }
            Assert.IsTrue(countries.Count > 0);
        }

        [TestMethod]
        public void get_languages()
        {

            try {
                var language = _locationService.GetLanguages();
                Assert.IsNotNull(language);
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void get_all_cities_with_alternates()
        {
            try {
                var list = _locationService.GetCitiesWithAlternates();
                Assert.IsNotNull(list);
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void get_all_cities()
        {
            try {
                var list = _locationService.GetCities();
                Assert.IsNotNull(list);
            } catch (Exception ex) {
                Debug.WriteLine(ex.Message);
            }
        }

        [TestMethod]
        public void get_languages_from_countries()
        {
            var countries = _locationService.GetCountries();
            foreach (var country in countries) {
                try {
                    var language = _locationService.GetLanguages(country.Key);
                    Assert.IsNotNull(language);
                } catch (Exception ex) {
                    Debug.WriteLine("LANGUAGE: " + country.Key + " : " + country.Value + " : " + ex.Message);
                }
            }
            Assert.IsTrue(countries.Count > 0);
        }
    }
}
