using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MS.Katusha.Infrastructure;
using MS.Katusha.Web.Models.Entities.BaseEntities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Web.Helpers
{
    public static class HtmlHelper
    {
        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;

            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
            {
                realModelType = underlyingType;
            }
            return realModelType;
        }

        private static readonly SelectListItem[] SingleEmptyItem = new[] { new SelectListItem { Text = CultureHelper._R("EmptyText"), Value = "0" } };

        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if ((attributes != null) && (attributes.Length > 0))
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, bool optional = false)
        {
            return EnumDropDownListFor(htmlHelper, expression, optional, null);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, bool optional, object htmlAttributes)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            Type enumType = GetNonNullableModelType(metadata);
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();
            
            IEnumerable<SelectListItem> items = from value in values
                                                select new SelectListItem
                                                {
                                                    Text = GetEnumDescription(value),
                                                    Value = value.ToString(),
                                                    Selected = value.Equals(metadata.Model)
                                                };

            // If the enum is nullable, add an 'empty' item to the collection
            if (optional || metadata.IsNullableValueType)
                items = SingleEmptyItem.Concat(items);

            return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
        }

        public static string KeyFor<TModel>(this HtmlHelper<TModel> htmlHelper, BaseFriendlyModel model)
        {   
            return (String.IsNullOrEmpty(model.FriendlyName)) ? model.Guid.ToString() : model.FriendlyName;
        }


        public static string _R<TModel>(this HtmlHelper<TModel> htmlHelper, string resourceName, Language language = 0)
        {
            IResourceManager rm = new ResourceManager();
            return rm._R(resourceName, (byte)language);
        }

        public static IHtmlString DisplayDetailFor<TModel, TProp>(this HtmlHelper<TModel> htmlHelper, bool condition, Expression<Func<TModel, TProp>> expression)
        {
            if (condition)
                return  htmlHelper.Raw(
                        String.Format("<div class=\"display-label\">{0}</div><div class=\"display-field\">{1}</div>",
                                     htmlHelper.DisplayNameFor(expression), htmlHelper.DisplayTextFor(expression)));
            return htmlHelper.Raw("");
        }
    }
}