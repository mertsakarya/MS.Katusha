using System.Collections.Generic;
using MS.Katusha.Enumerations;
using MS.Katusha.Repositories.RavenDB.Indexes;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;

namespace MS.Katusha.Web.Helpers
{
    public static class RavenHelper
    {
        public static DocumentStore RavenStore;

        public static void RegisterRaven()
        {
            //RavenStore = new EmbeddableDocumentStore { DataDirectory = ConfigurationManager.AppSettings["Root_Folder"] + @"App_Data\MS.Katusha.RavenDB", UseEmbeddedHttpServer = true };
            //RavenStore = new EmbeddableDocumentStore { DataDirectory = Environment.GetEnvironmentVariable("MS.KATUSHA_HOME") + @"App_Data\MS.Katusha.RavenDB", UseEmbeddedHttpServer = true };
            RavenStore = new EmbeddableDocumentStore { DataDirectory = DependencyHelper.RootFolder + @"App_Data\MS.Katusha.RavenDB", UseEmbeddedHttpServer = true };
            RavenStore.Initialize();
            CreateIndexes();
            CreateFacets();
        }

        private static void CreateIndexes()
        {
            IndexCreation.CreateIndexes(typeof (ProfileFacetsIndex).Assembly, RavenStore);
            IndexCreation.CreateIndexes(typeof (ProfileSearchFacetIndex).Assembly, RavenStore);
            IndexCreation.CreateIndexes(typeof (ProfileLanguageFacetIndex).Assembly, RavenStore);
            IndexCreation.CreateIndexes(typeof (ProfileCountryFacetIndex).Assembly, RavenStore);
            IndexCreation.CreateIndexes(typeof (ConversationIndex).Assembly, RavenStore);
        }

        private static void CreateFacets()
        {
            using (var session = RavenStore.OpenSession()) {
                session.Store(new FacetSetup {
                                                 Id = "facets/ProfileFacets",
                                                 Facets = new List<Facet> {
                                                                              new Facet {Name = "From"},
                                                                              new Facet {Name = "City"},
                                                                              new Facet {Name = "Gender"},
                                                                              new Facet {Name = "BodyBuild"},
                                                                              new Facet {Name = "HairColor"},
                                                                              new Facet {Name = "EyeColor"},
                                                                              new Facet {Name = "Smokes"},
                                                                              new Facet {Name = "Alcohol"},
                                                                              new Facet {Name = "Religion"},
                                                                              new Facet {Name = "DickSize"},
                                                                              new Facet {Name = "DickThickness"},
                                                                              new Facet {Name = "BreastSize"},
                                                                              new Facet {Name = "BirthYear", Mode = FacetMode.Ranges, Ranges = AgeHelper.Ranges,},
                                                                              new Facet {Name = "Height", Mode = FacetMode.Ranges, Ranges = HeightHelper.Ranges}
                                                                          }
                                             });
                session.Store(new FacetSetup { Id = "facets/ProfileCountryFacet", Facets = new List<Facet> { new Facet { Name = "Country" } } });
                session.Store(new FacetSetup { Id = "facets/ProfileSearchFacet", Facets = new List<Facet> { new Facet { Name = "Search" } } });
                session.Store(new FacetSetup { Id = "facets/ProfileLanguageFacet", Facets = new List<Facet> { new Facet { Name = "Language" } } });
                session.SaveChanges();
            }
        }
    }
}