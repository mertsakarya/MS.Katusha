using System.Globalization;
using System.Linq.Expressions;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class SearchStateCriteria : BaseSearchCriteria, ISearchCriteria
    {
        public new bool CanSearch { get { return base.CanSearch; } }

        public new Expression GetFilter(ParameterExpression argParam)
        {
            var expression = base.GetFilter(argParam);
            expression = ExpressionHelper.GetExpressionLocation(Location, argParam, expression);
            return expression;
        }
    }
}