using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.RavenDB.Indexes;
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

        public IList<Profile> Search(Expression<Func<Profile, bool>> filter, int pageNo, int pageSize, out int total)
        {
            return _profileRepositoryRaven.Search(filter, pageNo, pageSize, out total);
        }

        public IDictionary<string, IEnumerable<FacetValue>> FacetSearch<T>(Expression<Func<T, bool>> filter, string facetName)
        {
            return _profileRepositoryRaven.FacetSearch<T>(filter, facetName);
        }

        public SearchResult Search(SearchCriteria searchCriteria, int pageNo = 1, int pageSize = 50)
        {
            if (searchCriteria.CanSearch) {
                int total;
                var filter = GetFilter<Profile>(searchCriteria);
                var facetFilter = GetFilter<ProfileFacet>(searchCriteria, true);
                var searchFilter = GetFilter<ProfileSearchFacet>(searchCriteria, true);
                var countryFilter = GetFilter<ProfileCountryFacet>(searchCriteria, true);
                var languageFilter = GetFilter<ProfileLanguageFacet>(searchCriteria, true);
                var facetSearch = FacetSearch(facetFilter, "ProfileFacets");
                facetSearch.Add("Search", FacetSearch(searchFilter, "ProfileSearchFacet")["Search"]);
                facetSearch.Add("Country", FacetSearch(countryFilter, "ProfileCountryFacet")["Country"]);
                facetSearch.Add("Language", FacetSearch(languageFilter, "ProfileLanguageFacet")["Language"]);
              
                return new SearchResult { 
                    Profiles = Search(filter, pageNo, pageSize, out total),
                    FacetValues = facetSearch, 
                    SearchCriteria = searchCriteria, 
                    Total = total 
                };
            }
            return new SearchResult { Profiles = null, FacetValues = null, SearchCriteria = searchCriteria, Total = -1 };
        }

        private static Expression<Func<T, bool>> GetFilter<T>(SearchCriteria searchCriteria, bool isFacet = false)
        {
            var type = typeof (T);
            var argParam = Expression.Parameter(type, "p");
            var expression = GetExpression(new[] {(searchCriteria.Gender)}, Expression.Property(argParam, "Gender"));

            expression = GetExpression(searchCriteria.From, Expression.Property(argParam, "From"), expression);
            expression = GetExpression(searchCriteria.BodyBuild, Expression.Property(argParam, "BodyBuild"), expression);
            expression = GetExpression(searchCriteria.EyeColor, Expression.Property(argParam, "EyeColor"), expression);
            expression = GetExpression(searchCriteria.BreastSize, Expression.Property(argParam, "BreastSize"), expression);
            expression = GetExpression(searchCriteria.DickThickness, Expression.Property(argParam, "DickThickness"), expression);
            expression = GetExpression(searchCriteria.DickSize, Expression.Property(argParam, "DickSize"), expression);
            expression = GetExpression(searchCriteria.Religion, Expression.Property(argParam, "Religion"), expression);
            expression = GetExpression(searchCriteria.Alcohol, Expression.Property(argParam, "Alcohol"), expression);
            expression = GetExpression(searchCriteria.Smokes, Expression.Property(argParam, "Smokes"), expression);
            expression = GetExpression(searchCriteria.HairColor, Expression.Property(argParam, "HairColor"), expression);

            expression = GetExpressionString(searchCriteria.City, Expression.Property(argParam, "City"), expression);
            //expression = GetExpressionString(new[] {(searchCriteria.Name)}, Expression.Property(argParam, "Name"), expression);

            expression = GetExpressionMinMax(HeightHelper.GetArrayItems(searchCriteria.Height), Expression.Property(argParam, "Height"), expression);
            expression = GetExpressionMinMax(AgeHelper.GetArrayItems(searchCriteria.Age), Expression.Property(argParam, "BirthYear"), expression);
            if (!isFacet) {
                expression = GetExpressionIn(searchCriteria.Language, "Language", Expression.PropertyOrField(argParam, "LanguagesSpoken"), expression);
                expression = GetExpressionIn(searchCriteria.LookingFor, "Search", Expression.Property(argParam, "Searches"), expression);
                expression = GetExpressionIn(searchCriteria.Country, "Country", Expression.Property(argParam, "CountriesToVisit"), expression);
            } else {
                if (type.Name == "ProfileLanguageFacet") expression = GetExpression(searchCriteria.Language, Expression.PropertyOrField(argParam, "Language"), expression);
                if (type.Name == "ProfileSearchFacet") expression = GetExpression(searchCriteria.LookingFor, Expression.Property(argParam, "Search"), expression);
                if (type.Name == "ProfileCountryFacet") expression = GetExpression(searchCriteria.Country, Expression.Property(argParam, "Country"), expression);
            }
            var filter = Expression.Lambda<Func<T, bool>>(expression, argParam);
            return filter;
        }

        private static Expression GetExpressionMinMax(IList<int[]> values, Expression left, Expression expression = null)
        {
            IList<Expression> expressions = new List<Expression>();
            foreach (var item in values) {
                var min = item[0];
                var max = item[1];
                if (min == 0 && max == 0) return expression;
                var minExpression = (min > 0) ? Expression.GreaterThanOrEqual(left, Expression.Constant(min)) : null;
                var maxExpression = (max > 0) ? Expression.LessThanOrEqual(left, Expression.Constant(max)) : null;
                var combinedExpression = (minExpression != null && maxExpression != null) ? Expression.AndAlso(minExpression, maxExpression) : null;
                expressions.Add(combinedExpression ?? (minExpression ?? maxExpression));
            }
            if (expressions.Count <= 0) return expression;
            var pe = expressions[0];
            for (var i = 1; i < expressions.Count; i++) pe = Expression.OrElse(pe, expressions[i]);
            return expression == null ? pe : Expression.AndAlso(expression, pe);
        }

        private static Expression GetExpression<TEnum>(ICollection<TEnum> values, Expression left, Expression expression = null)
        {
            if (values.Count == 0) return expression;
            IList<Expression> expressions = values.Where(p=> Convert.ToByte(p) > 0).Select(value => Expression.Constant(Convert.ToByte(value))).Select(right => Expression.Equal(left, right)).Cast<Expression>().ToList();
            if (expressions.Count <= 0) return expression;
            var pe = expressions[0];
            for (var i = 1; i < expressions.Count; i++) pe = Expression.OrElse(pe, expressions[i]);
            return expression == null ? pe : Expression.AndAlso(expression, pe);
        }

        private static Expression GetExpressionString(ICollection<string> values, Expression left, Expression expression = null)
        {
            if (values.Count == 0) return expression;
            IList<Expression> expressions = values.Where(p=>!String.IsNullOrWhiteSpace(p)).Select(Expression.Constant).Select(right => Expression.Equal(left, right)).Cast<Expression>().ToList();
            if (expressions.Count <= 0) return expression;
            var pe = expressions[0];
            for (var i = 1; i < expressions.Count; i++) pe = Expression.OrElse(pe, expressions[i]);
            return expression == null ? pe : Expression.AndAlso(expression, pe);
        }

        private static Expression GetExpressionIn<TEnum>(ICollection<TEnum> values, string itemName, Expression left, Expression expression = null)
        {
            if(values.Count == 0) return expression;
            IList<Expression> expressions = new List<Expression>();
            foreach (var value in values) {
                var val = Convert.ToByte(value);
                Expression<Func<Profile, bool>> ex = null;
                switch(itemName) {
                    case "Language":
                        ex = profile => profile.LanguagesSpoken.Any(p => p.Language == val);
                        break;
                    case "Search":
                        ex = profile => profile.Searches.Any(p => p.Search == val);
                        break;
                    case "Country":
                        ex = profile => profile.CountriesToVisit.Any(p => p.Country == val);
                        break;

                }
                if (ex != null) expressions.Add(ex.Body);
            }
            var pe = expressions[0];
            for (var i = 1; i < expressions.Count; i++) pe = Expression.OrElse(pe, expressions[i]);
            return expression == null ? pe : Expression.AndAlso(expression, pe);
        }
    }
}
