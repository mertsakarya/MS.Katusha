using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Domain.Raven.Entities
{
    public class SearchProfileCriteria : BaseSearchCriteria, ISearchCriteria
    {
        public string Name { get; set; }
        public IList<Smokes?> Smokes { get; set; }
        public IList<Alcohol?> Alcohol { get; set; }
        public IList<Religion?> Religion { get; set; }
        public IList<DickSize?> DickSize { get; set; }
        public IList<DickThickness?> DickThickness { get; set; }
        public IList<BreastSize?> BreastSize { get; set; }

        public new bool CanSearch
        {
            get
            {
                if (base.CanSearch) {
                    return (String.IsNullOrWhiteSpace(Name) && Smokes.Count == 0 && Alcohol.Count == 0 && Religion.Count == 0 && DickSize.Count == 0
                            && DickThickness.Count == 0 && BreastSize.Count == 0);
                }
                return false;
            }
        }

        public new Expression GetFilter(ParameterExpression argParam)
        {
            var expression = base.GetFilter(argParam);
            //expression = GetExpressionString(new[] {(Name)}, Expression.Property(argParam, "Name"), expression);
            if(argParam.Type == typeof(Profile)) {
                expression = ExpressionHelper.GetExpressionLocation(Location, Expression.Property(argParam, "Location"), expression);
            } else {
                expression = ExpressionHelper.GetExpressionLocation(Location, argParam, expression);
            }
            expression = ExpressionHelper.GetExpression(BreastSize, Expression.Property(argParam, "BreastSize"), expression);
            expression = ExpressionHelper.GetExpression(DickThickness, Expression.Property(argParam, "DickThickness"), expression);
            expression = ExpressionHelper.GetExpression(DickSize, Expression.Property(argParam, "DickSize"), expression);
            expression = ExpressionHelper.GetExpression(Religion, Expression.Property(argParam, "Religion"), expression);
            expression = ExpressionHelper.GetExpression(Alcohol, Expression.Property(argParam, "Alcohol"), expression);
            expression = ExpressionHelper.GetExpression(Smokes, Expression.Property(argParam, "Smokes"), expression);
            return expression;
        }
    }
}