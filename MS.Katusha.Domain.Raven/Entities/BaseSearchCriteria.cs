using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Domain.Raven.Entities
{
    public abstract class BaseSearchCriteria : BaseModel 
    {
        public Sex Gender { get; set; }
        public Location Location { get; set; }
        public IList<BodyBuild?> BodyBuild { get; set; }
        public IList<EyeColor?> EyeColor { get; set; }
        public IList<HairColor?> HairColor { get; set; }

        public IList<Age?> Age { get; set; }
        public IList<Height?> Height { get; set; }

        public IList<LookingFor> LookingFor { get; set; }
        public IList<string> CountryToGo { get; set; }
        public IList<string> Language { get; set; }

        protected bool CanSearch
        {
            get
            {
                if (!(Gender == Sex.Male || Gender == Sex.Female)) return false;
                var result = (String.IsNullOrWhiteSpace(Location.CountryCode) && BodyBuild.Count == 0 && EyeColor.Count == 0 && HairColor.Count == 0
                              && LookingFor.Count == 0 && CountryToGo.Count == 0 && Language.Count == 0 && Age.Count == 0 && Height.Count == 0);
                return !result;
            }
        }

        protected virtual Expression GetFilter(ParameterExpression argParam)
        {
            var expression = ExpressionHelper.GetExpression(new[] {(Gender)}, Expression.Property(argParam, "Gender"));

            expression = ExpressionHelper.GetExpressionMinMax(HeightHelper.GetArrayItems(Height), Expression.Property(argParam, "Height"), expression);
            expression = ExpressionHelper.GetExpressionMinMax(AgeHelper.GetArrayItems(Age), Expression.Property(argParam, "BirthYear"), expression);

            expression = ExpressionHelper.GetExpression(BodyBuild, Expression.Property(argParam, "BodyBuild"), expression);
            expression = ExpressionHelper.GetExpression(EyeColor, Expression.Property(argParam, "EyeColor"), expression);
            expression = ExpressionHelper.GetExpression(HairColor, Expression.Property(argParam, "HairColor"), expression);
            return expression;
        }
    }
}