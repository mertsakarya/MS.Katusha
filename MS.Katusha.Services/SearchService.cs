using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
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

        public SearchResult Search(NameValueCollection qs, Sex gender, int pageNo, int pageSize, out int total)
        {
            if (qs.HasKeys()) {
                var searchProfile = new Profile();
                Expression expression;
                Expression ex = null;
                var argParam = Expression.Parameter(typeof (Profile), "p");
                var list = new NameValueCollection();
                searchProfile.Gender = (byte)GetExpression(null, new[] { ((byte)gender).ToString() }, "Gender", typeof(Sex), argParam, out expression);
                foreach (var key in qs.AllKeys) {
                    var values = qs.GetValues(key);
                    if (values == null) continue;
                    switch (key) {
                        case "From":
                            searchProfile.From = (byte)GetExpression(list, values, key, typeof(Country), argParam, out ex);
                            break;
                        case "City":
                            searchProfile.City = (string) GetExpression(list, values, key, null, argParam, out ex);
                            break;

                        case "BodyBuild":
                            searchProfile.BodyBuild = (byte)GetExpression(list, values, key, typeof(BodyBuild), argParam, out ex);
                            break;
                        case "EyeColor":
                            searchProfile.EyeColor = (byte)GetExpression(list, values, key, typeof(EyeColor), argParam, out ex);
                            break;
                        case "HairColor":
                            searchProfile.HairColor = (byte)GetExpression(list, values, key, typeof(HairColor), argParam, out ex);
                            break;
                        case "Smokes":
                            searchProfile.Smokes = (byte)GetExpression(list, values, key, typeof(Smokes), argParam, out ex);
                            break;
                        case "Alcohol":
                            searchProfile.Alcohol = (byte)GetExpression(list, values, key, typeof(Alcohol), argParam, out ex);
                            break;
                        case "Religion":
                            searchProfile.Religion = (byte)GetExpression(list, values, key, typeof(Religion), argParam, out ex);
                            break;
                        case "DickSize":
                            searchProfile.DickSize = (byte)GetExpression(list, values, key, typeof(DickSize), argParam, out ex);
                            break;
                        case "DickThickness":
                            searchProfile.DickThickness = (byte)GetExpression(list, values, key, typeof(DickThickness), argParam, out ex);
                            break;
                        case "BreastSize":
                            searchProfile.BreastSize = (byte)GetExpression(list, values, key, typeof(BreastSize), argParam, out ex);
                            break;
                    }
                    //} else if (key == "Height") { } else if (key == "BirthYear") { } else if (key == "Searches") { } else if (key == "LanguagesSpoken") { } else if (key == "CountriesToVisit") { }
                    if(ex != null) expression = (expression == null) ? ex : Expression.AndAlso(expression, ex);
                }
                if (expression != null) {
                    var filter = Expression.Lambda<Func<Profile, bool>>(expression, argParam);
                    var searchResult = Search(filter, pageNo, pageSize, out total);
                    var searchFacet = FacetSearch(filter);
                    return new SearchResult {Profiles = searchResult, FacetValues = searchFacet, Filters = qs, SearchProfile = searchProfile, Total = total};
                }
            }
            total = -1;
            return new SearchResult {Profiles = null, FacetValues = null, Filters = null, Total = total};
        }

        private static object GetExpression(NameValueCollection list, IEnumerable<string> values, string key, Type enumType, ParameterExpression argParam, out Expression ex)
        {
            IList<Expression> expressions = new List<Expression>();
            object parsedValue = null;
            foreach (var value in values) {
                var left = Expression.Property(argParam, key);
                parsedValue = GetConstantValue(key, value, enumType);
                Expression right = Expression.Constant(parsedValue);
                var expression = Expression.Equal(left, right);
                expressions.Add(expression);
                if(list != null) list.Add(key, value);
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

            ex = pe;
            return parsedValue;
        }

        private static object GetConstantValue(string key, string value, Type enumType)
        {
            if(key == "City") {
                return value;
            }
            byte constant;
            if(!Byte.TryParse(value, out constant)) {
                try {
                    var val = Enum.Parse(enumType, value);
                    constant = (byte) val;
                } catch {
                    constant = 0;
                }
            }
            return constant;
        }
    }
}
