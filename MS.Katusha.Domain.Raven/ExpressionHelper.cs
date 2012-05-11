using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Domain.Raven
{
    internal static class ExpressionHelper {
        internal static Expression GetExpressionMinMax(IList<int[]> values, Expression left, Expression expression = null)
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

        internal static Expression GetExpressionLocation(Location location, Expression argParam, Expression expression = null)
        {
            if (location == null || String.IsNullOrWhiteSpace(location.CountryCode)) return expression;
            var country = Expression.Equal(Expression.Property(argParam, "CountryCode"), Expression.Constant(location.CountryCode));
            var city = (location.CityCode == 0) ? null : Expression.Equal(Expression.Property(argParam, "CityCode"), Expression.Constant(location.CityCode));
            var exp = (city == null) ? country : Expression.AndAlso(country, city);
            return expression == null ? exp : Expression.AndAlso(expression, exp);
        }

        internal static Expression GetExpression<TEnum>(ICollection<TEnum> values, Expression left, Expression expression = null)
        {
            if (values.Count == 0) return expression;
            IList<Expression> expressions = values.Where(p => Convert.ToByte((object) p) > 0).Select(value => Expression.Constant(Convert.ToByte(value))).Select(right => Expression.Equal(left, right)).Cast<Expression>().ToList();
            if (expressions.Count <= 0) return expression;
            var pe = expressions[0];
            for (var i = 1; i < expressions.Count; i++) pe = Expression.OrElse(pe, expressions[i]);
            return expression == null ? pe : Expression.AndAlso(expression, pe);
        }

        internal static Expression GetExpressionString(ICollection<string> values, Expression left, Expression expression = null)
        {
            if (values.Count == 0) return expression;
            IList<Expression> expressions = values.Where(p => !String.IsNullOrWhiteSpace(p)).Select(Expression.Constant).Select(right => Expression.Equal(left, right)).Cast<Expression>().ToList();
            if (expressions.Count <= 0) return expression;
            var pe = expressions[0];
            for (var i = 1; i < expressions.Count; i++) pe = Expression.OrElse(pe, expressions[i]);
            return expression == null ? pe : Expression.AndAlso(expression, pe);
        }

        internal static Expression GetExpressionObject(object value, Expression left, Expression expression = null)
        {
            if (value == null) return expression;
            var pe = Expression.Equal(left, Expression.Constant(value));
            return expression == null ? pe : Expression.AndAlso(expression, pe);
        }
    }
}