using System;
using System.Collections.Generic;
using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Repositories.RavenDB.Indexes;
using Raven.Abstractions.Commands;
using Raven.Abstractions.Data;
using Raven.Client.Document;
using Raven.Client.Indexes;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Repositories.RavenDB
{
    public sealed class KatushaRavenStore : DocumentStore
    {
        public KatushaRavenStore(int i, string connectionString)
        {
            var parser = ConnectionStringParser<RavenConnectionStringOptions>.FromConnectionString(connectionString);
            parser.Parse();

            ApiKey = parser.ConnectionStringOptions.ApiKey;
            Url = parser.ConnectionStringOptions.Url;
            //RavenStore = new EmbeddableDocumentStore { DataDirectory = DependencyHelper.RootFolder + @"App_Data\MS.Katusha.RavenDB", UseEmbeddedHttpServer = true };
            Initialize();
        }

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
            DeleteAll<VideoRoom>();
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
            using (var session = OpenSession()) {
                foreach (var item in session.Query<T>()) {
                    session.Delete(item);
                }
                session.SaveChanges();
            }
        }

        public IEnumerable<ICommandData> DeleteProfile(long profileId, IEnumerable<Visit> visits, IEnumerable<Conversation> messages)
        {
            var list = new List<ICommandData> {
                new DeleteCommandData {Etag = null, Key = "profiles/" + profileId},
                new DeleteCommandData {Etag = null, Key = "states/" + profileId}
            };
            list.AddRange(visits.Select(item => new DeleteCommandData {Etag = null, Key = "visits/" + item.Id}));
            list.AddRange(messages.Select(item => new DeleteCommandData {Etag = null, Key = "conversations/" + item.Id}));
            return list;
        }

        public void Batch(List<ICommandData> list) { DatabaseCommands.Batch(list.ToArray()); }

        private void CreateIndexes()
        {
            IndexCreation.CreateIndexes(typeof(DialogIndex).Assembly, this);
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