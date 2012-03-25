using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MS.Katusha.Domain.Entities;
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
                realModelType = underlyingType;
            return realModelType;
        }

        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            var fi = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[]) fi.GetCustomAttributes(typeof (DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, bool optional = false) { return EnumDropDownListFor(htmlHelper, expression, optional, null); }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, bool optional, object htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var type = GetNonNullableModelType(metadata);
            IResourceManager rm = new ResourceManager();
            var dict = rm._L(type.Name);
            var list = new List<SelectListItem>();
            if (optional || metadata.IsNullableValueType)
                list.Add(new SelectListItem {Text = CultureHelper._R("EmptyText"), Value = "0"});
            list.AddRange(dict.Select(item => new SelectListItem {Text = item.Value, Value = item.Key, Selected = item.Key.Equals(metadata.Model)}));
            return htmlHelper.DropDownListFor(expression, list, htmlAttributes);
        }
 
        /// <summary>
        /// If there is a VirtualPath value in AppSettings it is used
        /// else url.Scheme :// url.Host + : url.Port / is returned
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static string Host<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["VirtualPath"])) return ConfigurationManager.AppSettings["VirtualPath"];
            var url = htmlHelper.ViewContext.RequestContext.HttpContext.Request.Url;
            var str = string.Format("{0}://{1}{2}/", url.Scheme, url.Host, ((url.Port == 80 || url.Port == 0) ? "" : ":" + url.Port));
            return str;
        }

        public static string KeyFor<TModel>(this HtmlHelper<TModel> htmlHelper, BaseFriendlyModel model)
        {
            return (String.IsNullOrEmpty(model.FriendlyName)) ? model.Guid.ToString() : model.FriendlyName;
        }

        public static string KeyFor<TModel>(this HtmlHelper<TModel> htmlHelper, Profile model)
        {
            return (String.IsNullOrEmpty(model.FriendlyName)) ? model.Guid.ToString() : model.FriendlyName;
        }


        public static string _R<TModel>(this HtmlHelper<TModel> htmlHelper, string resourceName, Language language = 0)
        {
            IResourceManager rm = new ResourceManager();
            return rm._R(resourceName, (byte)language);
        }

        public static string _LText<TModel>(this HtmlHelper<TModel> htmlHelper, string resourceName, string name, Language language = 0)
        {
            IResourceManager rm = new ResourceManager();
            return rm._LText(resourceName, name, (byte)language);
        }

        public static IDictionary<string, string> _L<TModel>(this HtmlHelper<TModel> htmlHelper, string resourceName, Language language = 0)
        {
            IResourceManager rm = new ResourceManager();
            return rm._L(resourceName, (byte)language);
        }

        public static IHtmlString Photo<TModel>(this HtmlHelper<TModel> htmlHelper, Guid photoGuid, Sex gender, PhotoType photoType = PhotoType.Original, string description = "")
        {
            var tb = new TagBuilder("img");
            var size = (photoType == PhotoType.Thumbnail) ? "small" : "";
            var sex = (gender == Sex.Male) ? "Boy" : "Girl";
            tb.Attributes.Add("src", (photoGuid == Guid.Empty) ? String.Format("/Images/{1}{0}.jpg", size, sex) : String.Format("/Profiles/Photo/{0}/{1}", photoGuid, size));
            if (!String.IsNullOrWhiteSpace(description))
                tb.Attributes.Add("title", description);
            return htmlHelper.Raw(tb.ToString());
        }

        public static string PhotoLink<TModel>(this HtmlHelper<TModel> htmlHelper, Guid photoGuid, Sex gender, PhotoType photoType = PhotoType.Original)
        {
            var size = (photoType == PhotoType.Thumbnail) ? "small" : "";
            var sex = (gender == Sex.Male) ? "Boy" : "Girl";
            var str = (photoGuid == Guid.Empty) ? String.Format("/Images/{1}{0}.jpg", size, sex) : String.Format("/Profiles/Photo/{0}/{1}", photoGuid, size);
            return str;
        }

        public static IHtmlString DisplayDetailFor<TModel, TProp>(this HtmlHelper<TModel> htmlHelper, bool condition, Expression<Func<TModel, TProp>> expression)
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            if (condition) {
                var label = new TagBuilder("div");
                label.AddCssClass("display-label");
                label.InnerHtml = String.Format("<b>{0}</b>", htmlHelper.DisplayNameFor(expression).ToHtmlString());
                var text = new TagBuilder("div");
                text.AddCssClass("display-field");
                text.InnerHtml = htmlHelper.DisplayTextFor(expression).ToHtmlString();
                var container = new TagBuilder("div");
                container.Attributes.Add("title", metadata.Description);
                container.InnerHtml = label + text.ToString();
                var result = htmlHelper.Raw(container.ToString());
                return result;
            }
            //return htmlHelper.Raw(
            //        String.Format("<div class=\"display-label\"><b>{0}</b></div><div class=\"display-field\">{1}</div><span title=\"{2}\">?</span><br />",
            //                      , htmlHelper.DisplayTextFor(expression), metadata.Description));
            return htmlHelper.Raw("");
        }

        public static IHtmlString ToJson<TModel>(this HtmlHelper<TModel> htmlHelper, Type enumType )
        {
            var keyValues = htmlHelper._L(enumType.Name);
            var sb = new StringBuilder();
            sb.Append("[");
            var i = 0;
            foreach (var item in keyValues) {
                sb.Append(Append(item.Key, item.Value));
                if (i != keyValues.Count - 1) sb.Append(",");
                i++;
            }
            sb.Append("]");
            return htmlHelper.Raw(sb.ToString());
        }

        private static string Append(string key, string value, string keyName = "key") { return String.Format("{{{0}:\"{1}\",value:\"{2}\"}}", keyName, key, value.Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r").Replace("'", "\\'").Replace("\"", "\\\"")); }

        public static IHtmlString ToJson<TModel, TColl, TProp>(this HtmlHelper<TModel> htmlHelper, IList<TColl> list,  Expression<Func<TColl, TProp>> expression)
        {
            var sb = new StringBuilder();
            sb.Append("[");

            IResourceManager rm = new ResourceManager();
            var compiledExpression = expression.Compile();
            var enumType = typeof(TProp);
            var firstTime = true;
            foreach (var item in list) {
                if (!firstTime) sb.Append(',');
                var v = Enum.GetName(enumType, compiledExpression.Invoke(item));
                sb.Append(Append(rm._LText(enumType.Name, v), v, "title"));
                firstTime = false;
            }
            sb.Append("]");
            return htmlHelper.Raw(sb.ToString());
        }

        public static IHtmlString FacebookListFor<TModel, TColl, TProp>(
            this HtmlHelper<TModel> htmlHelper, 
            string resourceName,
            TModel model,
            IList<TColl> collection,
            Type enumType, 
            Expression<Func<TModel, IList<TColl>>> modelPropertyExpression,
            Expression<Func<TColl, TProp>> propertyPropertyExpression 
            )
        {
            var name = typeof(TColl).Name;
            var label = new TagBuilder("div");
            var edit = new TagBuilder("div");
            var select = new TagBuilder("select");
            var script = new TagBuilder("script");

            label.AddCssClass("editor-label");
            label.InnerHtml = (htmlHelper.LabelFor(modelPropertyExpression).ToHtmlString());
            select.Attributes.Add("id", name + "Selection");
            select.Attributes.Add("name", name + "Selection[]");
            script.Attributes.Add("type", "text/javascript");
            var message = htmlHelper._R(resourceName + ".Message");
            var tmp = htmlHelper._R(resourceName + ".MaxItems");
            int maxItems;
            if(!int.TryParse(tmp, out maxItems)) maxItems = 0;
            var maxItemsText = (maxItems > 0) ? maxItems.ToString(CultureInfo.InvariantCulture) : String.Format("_FCBK{0}.length", name);
            script.InnerHtml = String.Format(
@"      var _FCBK{0}={1};
        var _FCBK{0}SelectedList={2};
        $('#{0}Selection').fcbkcomplete({{
            json_url:_FCBK{0},
            cache: true,
            json_cache:true,
            filter_case:false,
            filter_hide:false,
            firstselected:false,
            filter_selected:true,
            maxitems:{4},
            delay:10,
            complete_text:'{3}'
        }});
        for (var i = 0; i < _FCBK{0}SelectedList.length; i++) {{  
            $('#{0}Selection').trigger('addItem',_FCBK{0}SelectedList[i]);  
        }}
", name, htmlHelper.ToJson(enumType), htmlHelper.ToJson(collection, propertyPropertyExpression), message.Replace("\'", "\\'"), maxItemsText);
            edit.InnerHtml = select.ToString() + htmlHelper.ValidationMessageFor(modelPropertyExpression);  
            return htmlHelper.Raw( String.Format("{0}{1}{2}", label,edit,script));
        }


        public static IHtmlString DisplayDetailForEnum<TModel, TColl, TProp>(this HtmlHelper<TModel> htmlHelper, TModel model, Expression<Func<TModel, IList<TColl>>> expression,
            string lookupName, Type enumType, Expression<Func<TColl, TProp>> collectionPropertyExpression)
        {
            var compiledExpression = expression.Compile();
            var list = compiledExpression.Invoke(model);
            if (list.Count > 0) {
                var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
                var compiledPropertyExpression = collectionPropertyExpression.Compile();
                IResourceManager rm = new ResourceManager();
                var sb = new StringBuilder() ;
                var first = true;
                foreach (var item in list) {
                    var val = compiledPropertyExpression.Invoke(item);
                    if (!first) sb.Append(", "); else first = false;
                    sb.Append(rm._LText(lookupName, val.ToString()));
                }
                
                var label = new TagBuilder("div");
                label.AddCssClass("display-label");
                label.InnerHtml = String.Format("<b>{0}</b>", htmlHelper.DisplayNameFor(expression).ToHtmlString());
                var text = new TagBuilder("div");
                text.AddCssClass("display-field");
                text.InnerHtml = sb.ToString();
                var container = new TagBuilder("div");
                container.Attributes.Add("title", metadata.Description);
                container.InnerHtml = label + text.ToString();
                return htmlHelper.Raw(container.ToString());
            }

            return htmlHelper.Raw("");
        }    
    }
}
