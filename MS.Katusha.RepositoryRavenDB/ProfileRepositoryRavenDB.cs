using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;
using Raven.Abstractions.Data;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Linq;


namespace MS.Katusha.Repositories.RavenDB
{

    public class ProfileRepositoryRavenDB : BaseFriendlyNameRepositoryRavenDB<Profile>, IProfileRepositoryRavenDB
    {
        public ProfileRepositoryRavenDB(IDocumentStore documentStore): base(documentStore)
        { }


        public IDictionary<string, IEnumerable<FacetValue>> FacetSearch(Expression<Func<Profile, bool>> filter)
        {
            using (var session = DocumentStore.OpenSession()) {
                return session.Query<Profile>("ProfileFacetsIndex").Where(filter).ToFacets("facets/ProfileFacets");
            }
        }

        public IList<Profile> Search(Expression<Func<Profile, bool>> filter, int pageNo, int pageSize, out int total)
        {
            using (var session = DocumentStore.OpenSession()) {
                RavenQueryStatistics stats;
                var query = session.Query<Profile>().Statistics(out stats).Where(filter).Skip((pageNo - 1)*pageSize).Take(pageSize).ToList();
                total = stats.TotalResults;
                return query;
            }
        }

        public void SetFaceting()
        {
            const string indexName = "ProfileFacetsIndex";
                var facets = new List<Facet> {
                                                 new Facet {Name = "From"},
                                                 new Facet {Name = "City"},
                                                 new Facet {Name = "BodyBuild"},
                                                 new Facet {Name = "HairColor"},
                                                 new Facet {Name = "Smokes"},
                                                 new Facet {Name = "Alcohol"},
                                                 new Facet {Name = "Religion"},
                                                 new Facet {Name = "DickSize"},
                                                 new Facet {Name = "DickThickness"},
                                                 new Facet {Name = "Alcohol"},
                                                 new Facet {Name = "BreastSize"},
                                                 new Facet {Name = "Searches"},
                                                 new Facet {Name = "LanguagesSpoken"},
                                                 new Facet {Name = "CountriesToVisit"},

                                                 new Facet {
                                                               Name = "BirthYear",
                                                               Mode = FacetMode.Ranges,
                                                               Ranges = {
                                                                            "[NULL TO Dx1940.0]",
                                                                            "[Dx1940.0 TO Dx1960.0]",
                                                                            "[Dx1960.0 TO Dx1965.0]",
                                                                            "[Dx1965.0 TO Dx1970.0]",
                                                                            "[Dx1970.0 TO Dx1975.0]",
                                                                            "[Dx1975.0 TO Dx1980.0]",
                                                                            "[Dx1980.0 TO Dx1985.0]",
                                                                            "[Dx1985.0 TO Dx1990.0]",
                                                                            "[Dx1990.0 TO Dx1995.0]",
                                                                            "[Dx1995.0 TO NULL]",
                                                                        }
                                                           },
                                                 new Facet {
                                                               Name = "Height",
                                                               Mode = FacetMode.Ranges,
                                                               Ranges = {
                                                                            "[NULL TO Dx160.0]",
                                                                            "[Dx160.0 TO Dx170.0]",
                                                                            "[Dx170.0 TO Dx180.0]",
                                                                            "[Dx180.0 TO Dx190.0]",
                                                                            "[Dx190.0 TO Dx200.0]",
                                                                            "[Dx200.0 TO Dx210.0]",
                                                                            "[Dx210.0 TO NULL]",
                                                                        }
                                                           }
                                             };
                using (var session = DocumentStore.OpenSession()) {
                    var facet = new FacetSetup {Id = "facets/ProfileFacets", Facets = facets};
                    session.Store(facet);
                    session.SaveChanges();
                }

                if (DocumentStore.DatabaseCommands.GetIndex(indexName) == null) {
                    DocumentStore.DatabaseCommands.PutIndex(indexName,
                                                        new IndexDefinition {
                                                                                Map = @"from profile in docs.Profiles 
                                    select new 
                                    { 
                                        profile.From,
                                        profile.City,
                                        profile.BodyBuild,
                                        profile.HairColor,
                                        profile.Smokes,
                                        profile.Alcohol,
                                        profile.Religion,
                                        profile.DickSize,
                                        profile.DickThickness,
                                        profile.BreastSize,
                                        profile.Height,
                                        profile.BirthYear,
                                        profile.Searches,
                                        profile.LanguagesSpoken,
                                        profile.CountriesToVisit
                                    }"
                                                                            });
                //var facetResults = s.Query<Profile>("ProfileFacetsIndex") 
                //.Where(x => x.Cost >= 100 && x.Cost <= 300 ) 
                //.ToFacets("facets/CameraFacets");

            }
        }
    }
}
