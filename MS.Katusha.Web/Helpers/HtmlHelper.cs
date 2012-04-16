using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Linq.Expressions;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Infrastructure;
using MS.Katusha.Enumerations;
using MS.Katusha.Web.Models.Entities;
using MS.Katusha.Web.Models.Entities.BaseEntities;

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
            foreach (var item in dict) {
                var sli = new SelectListItem() {Text = item.Value, Value = item.Key};
                try {
                    if (metadata.Model != null && item.Key == metadata.Model.ToString())
                        sli.Selected = true;
                } catch {}
                list.Add(sli);
            }
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

        public static string KeyFor<TModel>(this HtmlHelper<TModel> htmlHelper, BaseFriendlyModel model) { return (String.IsNullOrEmpty(model.FriendlyName)) ? model.Guid.ToString() : model.FriendlyName; }

        public static string KeyFor<TModel>(this HtmlHelper<TModel> htmlHelper, Profile model) { return (String.IsNullOrEmpty(model.FriendlyName)) ? model.Guid.ToString() : model.FriendlyName; }


        public static string _R<TModel>(this HtmlHelper<TModel> htmlHelper, string resourceName, Language language = 0)
        {
            var rm = DependencyResolver.Current.GetService<IResourceManager>();
            return rm._R(resourceName, (byte)language);
        }

        public static string _LText<TModel>(this HtmlHelper<TModel> htmlHelper, string lookupName, string name, Language language = 0)
        {
            var rm = DependencyResolver.Current.GetService<IResourceManager>();
            //IResourceManager rm = new ResourceManager();
            return rm._LText(lookupName, name, (byte) language);
        }

        public static string _LText<TModel>(this HtmlHelper<TModel> htmlHelper, string lookupName, byte value, Language language = 0)
        {
            var rm = DependencyResolver.Current.GetService<IResourceManager>();
            var key = rm._LKey(lookupName, value, (byte)language);
            return key;
        }

        public static IDictionary<string, string> _L<TModel>(this HtmlHelper<TModel> htmlHelper, string resourceName, Language language = 0)
        {
            var rm = DependencyResolver.Current.GetService<IResourceManager>();
            return rm._L(resourceName, (byte)language);
        }

        public static IHtmlString Photo<TModel>(this HtmlHelper<TModel> htmlHelper, Guid photoGuid, PhotoType photoType = PhotoType.Original, string description = "", bool setId = false, bool encode = false )
        {
            var tb = new TagBuilder("img");
            if(setId) {
                tb.Attributes.Add("id", String.Format("ProfilePhoto"));
            }
            string str = GetPhotoPath(photoGuid, photoType);
            if (encode) {
                var fileName = htmlHelper.ViewContext.HttpContext.Server.MapPath(str);
                var bytes = ToBytes(fileName);
                var encodedBytes = EncodeBytes(bytes);
                str = @"data:image/jpg;base64," + encodedBytes;
            }
            tb.Attributes.Add("src", str);
            if (!String.IsNullOrWhiteSpace(description))
                tb.Attributes.Add("title", description);
            return htmlHelper.Raw(tb.ToString());
        }

        public static string PhotoLink<TModel>(this HtmlHelper<TModel> htmlHelper, Guid photoGuid, PhotoType photoType = PhotoType.Large) { return GetPhotoPath(photoGuid, photoType); }

        private static string GetPhotoPath(Guid photoGuid, PhotoType photoType)
        {
            return String.Format("/{0}/{1}-{2}.jpg", ((photoGuid == Guid.Empty) ? "Images": "Photos"), (byte)photoType, photoGuid);
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

        public static IHtmlString DisplayProfilePhoto<TModel>(this HtmlHelper<TModel> htmlHelper, ProfileModel profile, PhotoType photoType, string galleryName, bool encode = false)
        {
            var val = ((String.IsNullOrWhiteSpace(profile.FriendlyName)) ? profile.Guid.ToString() : profile.FriendlyName);
            var title = String.Format("{0} - {1} - {2}", profile.Name, (DateTime.Now.Year - profile.BirthYear), htmlHelper._LText("Country", Enum.GetName(typeof (Country), profile.From ?? 0)));
            var url = "/Profiles/Show/" + val;
            var anchor = new TagBuilder("a");
            anchor.Attributes.Add("title", title);
            anchor.Attributes.Add("href", url);
            anchor.Attributes.Add("rel", galleryName);
            anchor.InnerHtml = htmlHelper.Photo(profile.ProfilePhotoGuid, photoType, "", false, encode).ToString();
            return htmlHelper.Raw(anchor.ToString());
        }

        public static IHtmlString ToJson<TModel>(this HtmlHelper<TModel> htmlHelper, Type enumType)
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

        public static IHtmlString ToJson<TModel, TColl, TProp>(this HtmlHelper<TModel> htmlHelper, IList<TColl> list, Expression<Func<TColl, TProp>> expression)
        {
            var sb = new StringBuilder();
            sb.Append("[");

            IResourceManager rm = new ResourceManager();
            var compiledExpression = expression.Compile();
            var enumType = typeof (TProp);
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
            var name = typeof (TColl).Name;
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
            if (!int.TryParse(tmp, out maxItems)) maxItems = 0;
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
            return htmlHelper.Raw(String.Format("{0}{1}{2}", label, edit, script));
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
                var sb = new StringBuilder();
                var first = true;
                foreach (var item in list) {
                    var val = compiledPropertyExpression.Invoke(item);
                    if (!first) sb.Append(", ");
                    else first = false;
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

        public static string SetFacet<TModel>(this HtmlHelper<TModel> htmlHelper, string key, string value)
        {
            var url = htmlHelper.ViewContext.RequestContext.HttpContext.Request.RawUrl;
            var qs = htmlHelper.ViewContext.RequestContext.HttpContext.Request.QueryString;
            var i = url.IndexOf('?');
            if (i >= 0)
                url = url.Substring(0, i + 1);
            foreach (var k in qs.AllKeys) {
                var v = qs.Get(k);
                if (!(String.IsNullOrWhiteSpace(v) || v == "0"))
                    url += k + "=" + v + "&";
            }
            return url + key + "=" + ((String.IsNullOrWhiteSpace(value)) ? "Empty" : value);
        }

        public static IHtmlString CriteriaItem<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, string key, IList<TEnum> values)
        {
            if (values.Count > 0) {
                var hasValue = false;
                var enumType = typeof (TEnum);
                var sb = new StringBuilder();
                sb.AppendFormat("<li>{0}<ul>", key);
                foreach (var value in values) {
                    var result = "";
                    var k = key;
                    if(enumType.IsEnum) {
                        switch (key) {
                            case "From":
                                k = "Country";
                                break;
                            case "BirthYear":
                                k = "Age";
                                break;
                        }
                        var val = Convert.ToByte(value);
                        if (val > 0) hasValue = true;
                        var lookupName = k;
                        result = htmlHelper._LText(lookupName, htmlHelper._LText(lookupName, val));
                    } else if (value is string) {
                        var val = Convert.ToString(value);
                        if (!String.IsNullOrWhiteSpace(val)) hasValue = true;
                        result = val;
                    }
                    sb.AppendFormat("<li>{0}<a class='removeFacet' href='#' onclick=\"RemoveSearchKey('{1}', '{2}');\">[X]</a></li>", result, k, value);
                }
                sb.Append("</ul></li>");
                return htmlHelper.Raw(hasValue ? sb.ToString() : "");
            }
            return htmlHelper.Raw("");
        }

        public static string SearchCriteriaValueText<TModel>(this HtmlHelper<TModel> htmlHelper, string key, string range, out string lookupKey)
        {
            var value = "";
            lookupKey = range;
            string k;
            switch (key) {
                case "City":
                    value = range;
                    break;
                case "From":
                case "BodyBuild":
                case "HairColor":
                case "EyeColor":
                case "Smokes":
                case "Alcohol":
                case "Religion":
                case "DickSize":
                case "DickThickness":
                case "BreastSize":
                    k = (key == "From") ? "Country" : key;
                    lookupKey = htmlHelper._LText(k, Convert.ToByte(range));
                    value = htmlHelper._LText(k, lookupKey);
                    break;
                case "Search":
                case "Language":
                case "Country":
                    k = (key == "Search") ? "LookingFor" : key;
                    lookupKey = htmlHelper._LText(k, Convert.ToByte(range));
                    value = htmlHelper._LText(k, lookupKey);
                    break;
                case "BirthYear":
                    k = "Age";
                    var age = (byte)AgeHelper.GetEnum(range);
                    lookupKey = htmlHelper._LText(k, age);
                    value = htmlHelper._LText(k, lookupKey);
                    break;
                case "Height":
                    var height = (byte)HeightHelper.GetEnum(range);
                    lookupKey = htmlHelper._LText(key, height);
                    value = htmlHelper._LText(key, lookupKey);
                    break;
            }
            value = (String.IsNullOrWhiteSpace(value)) ? htmlHelper._R("NotFilled") : value;
            return value;
        }

        public static string SearchCriteriaKeyText<TModel>(this HtmlHelper<TModel> htmlHelper, string key)
        {
            switch (key) {
                case "Search":
                    return htmlHelper._R("Profile.Searches.DisplayName");
                case "Language":
                    return htmlHelper._R("Profile.LanguagesSpoken.DisplayName");
                case "Country":
                    return htmlHelper._R("Profile.CountriesToVisit.DisplayName");
                case "BirthYear":
                    return htmlHelper._R("Profile.Age.DisplayName");
                default:
                    if (!(key == "From" || key == "City")) 
                        return htmlHelper._R(String.Format("Profile.{0}.DisplayName", key));
                    return key;
            }
        }

        private static byte[] ToBytes(string fileName)
        {
            return System.IO.File.ReadAllBytes(fileName);
        }

        private static string EncodeBytes(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }
    }
}
