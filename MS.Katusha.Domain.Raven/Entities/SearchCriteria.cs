using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Domain.Raven.Entities
{
    public interface ISearchExpression
    {
        Expression GetFilter(ParameterExpression argParam);
    }

    public abstract class BaseSearchCriteria : BaseModel
    {
        public Sex Gender { get; set; }
        public IList<string> From { get; set; }
        public IList<string> City { get; set; }
        public IList<BodyBuild?> BodyBuild { get; set; }
        public IList<EyeColor?> EyeColor { get; set; }
        public IList<HairColor?> HairColor { get; set; }

        public IList<Age?> Age { get; set; }
        public IList<Height?> Height { get; set; }

        public IList<LookingFor> LookingFor { get; set; }
        public IList<string> Country { get; set; }
        public IList<string> Language { get; set; }

        protected bool CanSearch
        {
            get
            {
                if (!(Gender == Sex.Male || Gender == Sex.Female)) return false;
                var result = (From.Count == 0 && City.Count == 0 && BodyBuild.Count == 0 && EyeColor.Count == 0 && HairColor.Count == 0
                              && LookingFor.Count == 0 && Country.Count == 0 && Language.Count == 0 && Age.Count == 0 && Height.Count == 0);
                return !result;
            }
        }

        protected virtual Expression GetFilter(ParameterExpression argParam)
         {
             var expression = GetExpression(new[] { (Gender) }, Expression.Property(argParam, "Gender"));

             expression = GetExpressionLocation(From, Expression.Property(argParam, "From"), "Country", "", expression);
             expression = GetExpression(BodyBuild, Expression.Property(argParam, "BodyBuild"), expression);
             expression = GetExpression(EyeColor, Expression.Property(argParam, "EyeColor"), expression);
             expression = GetExpression(HairColor, Expression.Property(argParam, "HairColor"), expression);

             expression = GetExpressionString(City, Expression.Property(argParam, "City"), expression);
             return expression;
         }

         protected static Expression GetExpressionMinMax(IList<int[]> values, Expression left, Expression expression = null)
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

         protected static Expression GetExpressionLocation(ICollection<string> values, Expression left, string lookupName, string countryCode = "", Expression expression = null)
         {
             if (values.Count == 0) return expression;
             IList<Expression> expressions = new List<Expression>();
             foreach (var key in values) {
                 ConstantExpression right = Expression.Constant(key);
                 Expression expression1 = Expression.Equal(left, right);
                 expressions.Add(expression1);
             }
             if (expressions.Count <= 0) return expression;
             var pe = expressions[0];
             for (var i = 1; i < expressions.Count; i++) pe = Expression.OrElse(pe, expressions[i]);
             return expression == null ? pe : Expression.AndAlso(expression, pe);
         }

         protected static Expression GetExpression<TEnum>(ICollection<TEnum> values, Expression left, Expression expression = null)
         {
             if (values.Count == 0) return expression;
             IList<Expression> expressions = values.Where(p => Convert.ToByte(p) > 0).Select(value => Expression.Constant(Convert.ToByte(value))).Select(right => Expression.Equal(left, right)).Cast<Expression>().ToList();
             if (expressions.Count <= 0) return expression;
             var pe = expressions[0];
             for (var i = 1; i < expressions.Count; i++) pe = Expression.OrElse(pe, expressions[i]);
             return expression == null ? pe : Expression.AndAlso(expression, pe);
         }

         protected static Expression GetExpressionString(ICollection<string> values, Expression left, Expression expression = null)
         {
             if (values.Count == 0) return expression;
             IList<Expression> expressions = values.Where(p => !String.IsNullOrWhiteSpace(p)).Select(Expression.Constant).Select(right => Expression.Equal(left, right)).Cast<Expression>().ToList();
             if (expressions.Count <= 0) return expression;
             var pe = expressions[0];
             for (var i = 1; i < expressions.Count; i++) pe = Expression.OrElse(pe, expressions[i]);
             return expression == null ? pe : Expression.AndAlso(expression, pe);
         }

         protected static Expression GetExpressionObject(object value, Expression left, Expression expression = null)
         {
             if (value == null) return expression;
             var pe = Expression.Equal(left, Expression.Constant(value));
             return expression == null ? pe : Expression.AndAlso(expression, pe);
         }
    }

    public class SearchProfileCriteria : BaseSearchCriteria, ISearchExpression
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
            expression = GetExpression(BreastSize, Expression.Property(argParam, "BreastSize"), expression);
            expression = GetExpression(DickThickness, Expression.Property(argParam, "DickThickness"), expression);
            expression = GetExpression(DickSize, Expression.Property(argParam, "DickSize"), expression);
            expression = GetExpression(Religion, Expression.Property(argParam, "Religion"), expression);
            expression = GetExpression(Alcohol, Expression.Property(argParam, "Alcohol"), expression);
            expression = GetExpression(Smokes, Expression.Property(argParam, "Smokes"), expression);
            expression = GetExpressionMinMax(HeightHelper.GetArrayItems(Height), Expression.Property(argParam, "Height"), expression);
            expression = GetExpressionMinMax(AgeHelper.GetArrayItems(Age), Expression.Property(argParam, "BirthYear"), expression);
            return expression;
        }
    }


    public class SearchStateCriteria : BaseSearchCriteria, ISearchExpression
    {

        public new bool CanSearch
        {
            get { return base.CanSearch; }
        }

        public new Expression GetFilter(ParameterExpression argParam) {
            var expression = base.GetFilter(argParam);
            return expression;
        }
    }    
    
}