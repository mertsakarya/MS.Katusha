using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using Raven.Abstractions.Data;

namespace MS.Katusha.Interfaces.Services
{
    public class SearchResult
    {
        public IList<Profile> Profiles { get; set; }
        public IDictionary<string, IEnumerable<FacetValue>> FacetValues { get; set; }
        public NameValueCollection Filters { get; set; }
        public Profile SearchProfile { get; set; }
        public int Total { get; set; }
    }

    public interface ISearchService
    {
        IList<Profile> Search(Expression<Func<Profile, bool>> filter, int pageNo, int pageSize, out int total);
        IDictionary<string, IEnumerable<FacetValue>> FacetSearch(Expression<Func<Profile, bool>> filter);
        SearchResult Search(NameValueCollection qs, Sex gender, int pageNo, int pageSize, out int total);
    }
}
