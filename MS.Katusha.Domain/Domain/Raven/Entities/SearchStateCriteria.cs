using System;
using System.Linq.Expressions;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class SearchStateCriteria : BaseSearchCriteria, ISearchCriteria
    {
        public new bool CanSearch { get { return base.CanSearch; } }

        public new Expression GetFilter(ParameterExpression argParam)
        {
            var expression = base.GetFilter(argParam);
            if(argParam.Type != typeof(StateFacet)) {
                var onlineInterval = DateTime.Now.AddMinutes(-5.0);
                var pe = Expression.GreaterThan(Expression.Property(argParam, "LastOnline"), Expression.Constant(onlineInterval));
                expression = Expression.AndAlso(expression, pe);
            }
            expression = ExpressionHelper.GetExpressionLocation(Location, argParam, expression);
            //Expression.Property(argParam, "CityCode")
            return expression;
        }
    }
}