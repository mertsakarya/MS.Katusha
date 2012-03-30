using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using Raven.Abstractions.Data;

namespace MS.Katusha.Services
{
    public class SearchService : ISearchService
    {

        private readonly IProfileRepositoryRavenDB _profileRepositoryRaven;

        public SearchService(IProfileRepositoryRavenDB profileRepositoryRaven)
        {
            _profileRepositoryRaven = profileRepositoryRaven;
        }

        public IList<Profile> Search(Expression<Func<Profile, bool>> filter, int pageNo, int pageSize, out int total) { return _profileRepositoryRaven.Search(filter, pageNo, pageSize, out total); }
        public IDictionary<string, IEnumerable<FacetValue>> FacetSearch(Expression<Func<Profile, bool>> filter) { return _profileRepositoryRaven.FacetSearch(filter); }

        public SearchResult Search(NameValueCollection qs, int pageNo, int pageSize, out int total)
        {
            if (qs.HasKeys()) {
                Expression expression = null;
                Expression ex = null;
                var argParam = Expression.Parameter(typeof (Profile), "p");
                var list = new NameValueCollection();
                foreach (var key in qs.AllKeys) {
                    var values = qs.GetValues(key);
                    if (values == null) continue;
                    if (key == "From") {
                        ex = GetExpression(list, values, key, argParam);
                    } else if (key == "City") {
                        ex = GetExpression(list, values, key, argParam);
                    } else if (key == "BodyBuild") {
                        ex = GetExpression(list, values, key, argParam);
                    } else if (key == "HairColor") {
                        ex = GetExpression(list, values, key, argParam);
                    } else if (key == "Smokes") {
                        ex = GetExpression(list, values, key, argParam);
                    } else if (key == "Alcohol") {
                        ex = GetExpression(list, values, key, argParam);
                    } else if (key == "Religion") {
                        ex = GetExpression(list, values, key, argParam);
                    } else if (key == "DickSize") {
                        ex = GetExpression(list, values, key, argParam);
                    } else if (key == "DickThickness") {
                        ex = GetExpression(list, values, key, argParam);
                    } else if (key == "BreastSize") {
                        ex = GetExpression(list, values, key, argParam);
                    }
                    //} else if (key == "Height") { } else if (key == "BirthYear") { } else if (key == "Searches") { } else if (key == "LanguagesSpoken") { } else if (key == "CountriesToVisit") { }
                    expression = (expression == null) ? ex : Expression.AndAlso(expression, ex);
                }
                if (expression != null) {
                    var filter = Expression.Lambda<Func<Profile, bool>>(expression, argParam);
                    var searchResult = Search(filter, pageNo, pageSize, out total);
                    var searchFacet = FacetSearch(filter);
                    return new SearchResult {Profiles = searchResult, FacetValues = searchFacet, Filters = qs, Total = total};
                }
            }
            total = -1;
            return new SearchResult {Profiles = null, FacetValues = null, Filters = null, Total = total};
        }

        private static Expression GetExpression(NameValueCollection list, IEnumerable<string> values, string key, ParameterExpression argParam)
        {
            IList<Expression> expressions = new List<Expression>();
            foreach (var value in values) {
                var left = Expression.Property(argParam, key);
                var constant = GetConstantValue(key, value);
                Expression right = Expression.Constant(constant);
                var expression = Expression.Equal(left, right);
                expressions.Add(expression);
                list.Add(key, value);
            }

            Expression pe;
            if (expressions.Count > 0) {
                pe = expressions[0];
                for (var i = 1; i < expressions.Count; i++) {
                    pe = Expression.OrElse(pe, expressions[i]);
                }
            } else {
                pe = expressions[0];
            }

            return pe;
        }

        private static object GetConstantValue(string key, string value)
        {
            if(key == "City") {
                return value;
            }
            byte constant;
            return !Byte.TryParse(value, out constant) ? (byte)0 : constant;
        }
    }
}
