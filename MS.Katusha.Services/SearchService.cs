using System;
using System.Collections.Generic;
using System.Linq;
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

        public SearchResult Search(SearchCriteria searchCriteria, int pageNo = 1, int pageSize = 50)
        {
            if (searchCriteria.CanSearch) {
                var argParam = Expression.Parameter(typeof(Profile), "p");
                var expression = GetExpression(new[] { (searchCriteria.Gender) }, Expression.Property(argParam, "Gender"));

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
                expression = GetExpressionString(new[] { (searchCriteria.Name) }, Expression.Property(argParam, "Name"), expression);

                expression = GetExpressionMinMax(searchCriteria.MinHeight, searchCriteria.MaxHeight, Expression.Property(argParam, "Height"), expression);
                var maxBirthyear = (searchCriteria.MinAge == 0) ? 0 : DateTime.Now.Year - searchCriteria.MinAge;
                var minBirthYear = (searchCriteria.MaxAge == 0) ? 0 : DateTime.Now.Year - searchCriteria.MaxAge;
                expression = GetExpressionMinMax(minBirthYear, maxBirthyear, Expression.Property(argParam, "BirthYear"), expression);
                expression = GetExpressionIn(searchCriteria.LanguagesSpoken, "Language", Expression.Property(argParam, "LanguagesSpoken"), expression);
                expression = GetExpressionIn(searchCriteria.Searches, "LookingFor", Expression.Property(argParam, "Searches"), expression);
                expression = GetExpressionIn(searchCriteria.CountriesToVisit, "Country", Expression.Property(argParam, "CountriesToVisit"), expression);

                int total;
                var filter = Expression.Lambda<Func<Profile, bool>>(expression, argParam);
                return new SearchResult { 
                    Profiles = Search(filter, pageNo, pageSize, out total), 
                    FacetValues = FacetSearch(filter), 
                    SearchCriteria = searchCriteria, 
                    Total = total 
                };
            }
            return new SearchResult { Profiles = null, FacetValues = null, SearchCriteria = null, Total = -1 };
        }

        private static Expression GetExpressionMinMax(int min, int max, Expression left, Expression expression = null)
        {
            if (min == 0 &&  max == 0) return expression;
            var minExpression = (min > 0) ? Expression.GreaterThanOrEqual(left, Expression.Constant(min)) : null;
            var maxExpression = (max > 0) ? Expression.LessThanOrEqual(left, Expression.Constant(max)) : null;
            var combinedExpression = (minExpression != null && maxExpression != null) ? Expression.AndAlso(minExpression, maxExpression) : null;
            var pe = combinedExpression ?? (minExpression ?? maxExpression);
            if(pe == null) return expression;
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
            var item = Expression.Property(left, itemName);
            foreach (var value in values) {
                var constant = Expression.Constant(Convert.ToByte(value));
                expressions.Add(Expression.Equal(item, constant));
            }
            var pe = expressions[0];
            for (var i = 1; i < expressions.Count; i++) pe = Expression.OrElse(pe, expressions[i]);
            return expression == null ? pe : Expression.AndAlso(expression, pe);
        }

    }
}
