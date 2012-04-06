using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.RavenDB.Base;
using MS.Katusha.Repositories.RavenDB.Indexes;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Indexes;
using Raven.Client.Linq;

namespace MS.Katusha.Repositories.RavenDB
{
    public class ProfileRepositoryRavenDB : BaseFriendlyNameRepositoryRavenDB<Profile>, IProfileRepositoryRavenDB
    {
        public ProfileRepositoryRavenDB(IDocumentStore documentStore): base(documentStore)
        { }

        public void CreateIndexes()
        {
            IndexCreation.CreateIndexes(typeof(ProfileFacetsIndex).Assembly, DocumentStore);
            IndexCreation.CreateIndexes(typeof(ProfileSearchFacetIndex).Assembly, DocumentStore);
            IndexCreation.CreateIndexes(typeof(ProfileLanguageFacetIndex).Assembly, DocumentStore);
            IndexCreation.CreateIndexes(typeof(ProfileCountryFacetIndex).Assembly, DocumentStore);
        }

        public IDictionary<string, IEnumerable<FacetValue>> FacetSearch<T>(Expression<Func<T, bool>> filter, string facetName)
        {
            using (var session = DocumentStore.OpenSession()) {
                return Queryable.Where(session.Query<T>(facetName + "Index"), filter).ToFacets("facets/" + facetName);
                //.AsProjection<Profile>()
            }
        }

        public IList<Profile> Search(Expression<Func<Profile, bool>> filter, int pageNo, int pageSize, out int total)
        {
            using (var session = DocumentStore.OpenSession()) {
                RavenQueryStatistics stats;
                var query = Queryable.Skip(session.Query<Profile>().Statistics(out stats).Where(filter), (pageNo - 1)*pageSize).Take(pageSize).ToList();
                total = stats.TotalResults;
                return query;
            }
        }

        public void CreateFacets()
        {
            var searchFacet = new List<Facet> { new Facet { Name = "Search" } };
            var languageFacet = new List<Facet> { new Facet { Name = "Language" } };
            var countryFacet = new List<Facet> { new Facet { Name = "Country" } };

            var profilefacet = new List<Facet> {
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
                new Facet {Name = "BirthYear", Mode = FacetMode.Ranges, Ranges = AgeHelper.Ranges, },
                new Facet {Name = "Height", Mode = FacetMode.Ranges, Ranges = HeightHelper.Ranges }
            };
            using (var session = DocumentStore.OpenSession()) {
                session.Store(new FacetSetup { Id = "facets/ProfileFacets", Facets = profilefacet });
                session.Store(new FacetSetup { Id = "facets/ProfileCountryFacet", Facets = countryFacet });
                session.Store(new FacetSetup { Id = "facets/ProfileSearchFacet", Facets = searchFacet });
                session.Store(new FacetSetup { Id = "facets/ProfileLanguageFacet", Facets = languageFacet });
                session.SaveChanges();
            }
        }

    }
}
