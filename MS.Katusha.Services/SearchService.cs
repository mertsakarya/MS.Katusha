using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Raven.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using Raven.Abstractions.Data;

namespace MS.Katusha.Services
{
    public class SearchService : ISearchService
    {

        private readonly IProfileRepositoryRavenDB _profileRepositoryRaven;
        private readonly IStateRepositoryRavenDB _stateRepositoryRaven;

        public SearchService(IProfileRepositoryRavenDB profileRepositoryRaven, IStateRepositoryRavenDB stateRepositoryRaven)
        {
            _profileRepositoryRaven = profileRepositoryRaven;
            _stateRepositoryRaven = stateRepositoryRaven;
        }

        private IEnumerable<Profile> Search(Expression<Func<Profile, bool>> filter, int pageNo, int pageSize, out int total)
        {
            return _profileRepositoryRaven.Search(filter, pageNo, pageSize, out total);
        }

        private IDictionary<string, IEnumerable<FacetValue>> FacetSearch<T>(Expression<Func<T, bool>> filter, string facetName)
        {
            return _profileRepositoryRaven.FacetSearch(filter, facetName);
        }

        public SearchResult Search(SearchCriteria searchCriteria, int pageNo = 1, int pageSize = 50)
        {
            if (searchCriteria.CanSearch) {
                int total;
                var filter = GetFilter<Profile>(searchCriteria);
                var facetFilter = GetFilter<ProfileFacet>(searchCriteria);
                var facetSearch = FacetSearch(facetFilter, "ProfileFacets");

                return new SearchResult {
                    Profiles = Search(filter, pageNo, pageSize, out total),
                    FacetValues = facetSearch,
                    SearchCriteria = searchCriteria,
                    Total = total
                };
            }
            return new SearchResult { Profiles = null, FacetValues = null, SearchCriteria = searchCriteria, Total = -1 };
        }

        private static Expression<Func<T, bool>> GetFilter<T>(SearchCriteria searchCriteria)
        {
            var type = typeof (T);
            var argParam = Expression.Parameter(type, "p");
            var expression = GetExpression(new[] {(searchCriteria.Gender)}, Expression.Property(argParam, "Gender"));

            expression = GetExpressionLocation(searchCriteria.From, Expression.Property(argParam, "From"), "Country", "", expression);
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

        private static Expression GetExpressionLocation(ICollection<string> values, Expression left, string lookupName, string countryCode = "", Expression expression = null)
        {
            if (values.Count == 0) return expression;
            IList<Expression> expressions = new List<Expression>();
            var resourceManager = ResourceManager.GetInstance();
            foreach (var key in values) {
                if (resourceManager.ContainsKey(lookupName, key, countryCode)) {
                    ConstantExpression right = Expression.Constant(key);
                    Expression expression1 = Expression.Equal(left, right);
                    expressions.Add(expression1);
                }
            }
            if (expressions.Count <= 0) return expression;
            var pe = expressions[0];
            for (var i = 1; i < expressions.Count; i++) pe = Expression.OrElse(pe, expressions[i]);
            return expression == null ? pe : Expression.AndAlso(expression, pe);
        }

        private static Expression GetExpression<TEnum>(ICollection<TEnum> values, Expression left, Expression expression = null)
        {
            if (values.Count == 0) return expression;
            IList<Expression> expressions = values.Where(p => Convert.ToByte(p) > 0).Select(value => Expression.Constant(Convert.ToByte(value))).Select(right => Expression.Equal(left, right)).Cast<Expression>().ToList();
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
    }
}
