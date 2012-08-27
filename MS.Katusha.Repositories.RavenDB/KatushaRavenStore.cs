using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Repositories.RavenDB.Indexes;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Raven.Json.Linq;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Repositories.RavenDB
{
    public class KatushaRavenStore : DocumentStore, IKatushaRavenStore
    {
        public KatushaRavenStore(string connectionName = "RavenDB")  {
            var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionStringName(connectionName);
            parser.Parse();

            ApiKey = parser.ConnectionStringOptions.ApiKey;
            Url = parser.ConnectionStringOptions.Url;
            //RavenStore = new EmbeddableDocumentStore { DataDirectory = DependencyHelper.RootFolder + @"App_Data\MS.Katusha.RavenDB", UseEmbeddedHttpServer = true };
            Initialize();
            try {
                Create();
            } catch { }
        }

        public void ClearRaven()
        {
            DeleteAll<Conversation>();
            DeleteAll<State>();
            DeleteAll<Visit>();
            DeleteAll<Profile>();
            DeleteAll<FacetSetup>();
            var indexes = DatabaseCommands.GetIndexNames(0, int.MaxValue);
            foreach (var index in indexes)
                if (!index.ToLowerInvariant().StartsWith("raven/"))
                    DatabaseCommands.DeleteIndex(index);
            Create();
        }

        public void Create()
        {
            CreateIndexes();
            CreateFacets();
        }

        public void DeleteAll<T>()
        {
            using (var session = this.OpenSession()) {
                foreach (var item in session.Query<T>()) {
                    session.Delete(item);
                }
                session.SaveChanges();
            }
        }


        public List<ICommandData> DeleteProfile(long profileId, Domain.Entities.Visit[] visits, Conversation[] messages)
        {
            var list = new List<ICommandData> {
                new DeleteCommandData {Etag = null, Key = "profiles/" + profileId},
                new DeleteCommandData {Etag = null, Key = "states/" + profileId}
            };
            foreach (var item in visits) 
                list.Add(new DeleteCommandData() { Etag = null, Key = "visits/" + item.Id });
            foreach (var item in messages) 
                list.Add(new DeleteCommandData() { Etag = null, Key = "conversations/" + item.Id });
            return list;
        }

        public void Batch(List<ICommandData> list) { DatabaseCommands.Batch(list.ToArray()); }

        private void CreateIndexes()
        {
            IndexCreation.CreateIndexes(typeof(CountryCityCountIndex).Assembly, this);
            IndexCreation.CreateIndexes(typeof(ProfileFacetsIndex).Assembly, this);
            IndexCreation.CreateIndexes(typeof(ConversationIndex).Assembly, this);
            IndexCreation.CreateIndexes(typeof(ConversationToCountIndex).Assembly, this);
            IndexCreation.CreateIndexes(typeof(ConversationFromCountIndex).Assembly, this);
            IndexCreation.CreateIndexes(typeof(UniqueVisitorsIndex).Assembly, this);
            IndexCreation.CreateIndexes(typeof(StateFacetsIndex).Assembly, this);
        }

        private void CreateFacets()
        {
            using (var session = OpenSession()) {

                session.Store(new FacetSetup {
                    Id = "facets/ProfileFacets",
                    Facets = new List<Facet> {
                        new Facet {Name = "CountryCode"},
                        new Facet {Name = "CityCode"},
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

                session.Store(new FacetSetup {
                    Id = "facets/StateFacets",
                    Facets = new List<Facet> {
                        new Facet {Name = "CountryCode"},
                        new Facet {Name = "CityCode"},
                        new Facet {Name = "Gender"},
                        new Facet {Name = "BodyBuild"},
                        new Facet {Name = "HairColor"},
                        new Facet {Name = "EyeColor"},
                        new Facet {Name = "BirthYear", Mode = FacetMode.Ranges, Ranges = AgeHelper.Ranges,},
                        new Facet {Name = "Height", Mode = FacetMode.Ranges, Ranges = HeightHelper.Ranges}
                    }
                });
                session.SaveChanges();
            }
        }
    }
}