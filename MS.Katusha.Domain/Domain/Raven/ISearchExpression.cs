using System.Linq.Expressions;

namespace MS.Katusha.Domain.Raven
{
    public interface ISearchCriteria
    {
        Expression GetFilter(ParameterExpression argParam);
        bool CanSearch { get; }
    }
}