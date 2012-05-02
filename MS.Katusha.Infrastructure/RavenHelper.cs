using System.Collections.Generic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Repositories.RavenDB.Indexes;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Infrastructure
{
    public static class RavenHelper
    {
        public static DocumentStore RavenStore;

        public static void RegisterRaven()
        {
            var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionStringName("RavenDB");
            parser.Parse();

            RavenStore = new DocumentStore { ApiKey = parser.ConnectionStringOptions.ApiKey, Url = parser.ConnectionStringOptions.Url };
            //RavenStore = new EmbeddableDocumentStore { DataDirectory = DependencyHelper.RootFolder + @"App_Data\MS.Katusha.RavenDB", UseEmbeddedHttpServer = true };
            
            RavenStore.Initialize();
            try {
                Create();
            } catch {}
        }

        public static void ClearRaven()
        {
            DeleteAll<Conversation>();
            DeleteAll<State>();
            DeleteAll<Visit>();
            DeleteAll<Profile>();
            DeleteAll<FacetSetup>();
            var indexes = RavenStore.DatabaseCommands.GetIndexNames(0, int.MaxValue);
            foreach(var index in indexes)
                if (!index.ToLowerInvariant().StartsWith("raven/"))
                    RavenStore.DatabaseCommands.DeleteIndex(index);
            Create();
        }

        private static void Create()
        {
            CreateIndexes();
            CreateFacets();
        }

        private static void DeleteAll<T>()
        {
            using (var session = RavenStore.OpenSession()) {
                foreach(var item in session.Query<T>()) {
                    session.Delete(item);
                }
                session.SaveChanges();
            }
        }

        public static void CreateIndexes()
        {
            IndexCreation.CreateIndexes(typeof(CountryCityCountIndex).Assembly, RavenStore);
            IndexCreation.CreateIndexes(typeof(ProfileFacetsIndex).Assembly, RavenStore);
            IndexCreation.CreateIndexes(typeof(ConversationIndex).Assembly, RavenStore);
            IndexCreation.CreateIndexes(typeof(ConversationCountIndex).Assembly, RavenStore);
            IndexCreation.CreateIndexes(typeof(UniqueVisitorsIndex).Assembly, RavenStore);
        }

        public static void CreateFacets()
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
                session.SaveChanges();
            }
        }
    }
}