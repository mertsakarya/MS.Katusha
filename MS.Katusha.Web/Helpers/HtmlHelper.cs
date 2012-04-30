using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using MS.Katusha.Infrastructure;
using MS.Katusha.Enumerations;
using MS.Katusha.Web.Models.Entities;
using MS.Katusha.Web.Models.Entities.BaseEntities;

namespace MS.Katusha.Web.Helpers
{
    public static class HtmlHelper
    {
        private static readonly IResourceManager resourceManager =  ResourceManager.GetInstance(); //DependencyResolver.Current.GetService<IResourceManager>();

        private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        {
            Type realModelType = modelMetadata.ModelType;
            Type underlyingType = Nullable.GetUnderlyingType(realModelType);
            if (underlyingType != null)
                realModelType = underlyingType;
            return realModelType;
        }

        public static string GetUrlFriendlyDateTime<TModel>(this HtmlHelper<TModel> htmlHelper, DateTime dateTime)
        {
            var year = dateTime.Year;
            var month = dateTime.Month;
            var day = dateTime.Day;
            var hour = dateTime.Hour;
            var minute = dateTime.Minute;
            var second = dateTime.Second;
            return String.Format("{0}{1}{2}{3}{4}{5}",
                                 year,
                                 (month < 10) ? "0" + month: month.ToString(CultureInfo.InvariantCulture),
                                 (day < 10) ? "0" + day: day.ToString(CultureInfo.InvariantCulture),
                                 (hour < 10) ? "0" + hour: hour.ToString(CultureInfo.InvariantCulture),
                                 (minute < 10) ? "0" + minute: minute.ToString(CultureInfo.InvariantCulture),
                                 (second < 10) ? "0" + second:second.ToString(CultureInfo.InvariantCulture)
                                 );
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, bool optional = false) { return EnumDropDownListFor(htmlHelper, expression, optional, null); }

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, bool optional, object htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var type = GetNonNullableModelType(metadata);
            var dict = resourceManager.GetLookup(type.Name);
            var list = new List<SelectListItem>();
            if (optional || metadata.IsNullableValueType)
                list.Add(new SelectListItem { Text = htmlHelper.ResourceValue("EmptyText"), Value = "0" });
            foreach (var item in dict) {
                var sli = new SelectListItem { Text = item.Value, Value = item.Key };
                try {
                    if (metadata.Model != null && item.Key == metadata.Model.ToString())
                        sli.Selected = true;
                } catch { }
                list.Add(sli);
            }
            return htmlHelper.DropDownListFor(expression, list, htmlAttributes);
        }

        public static string KeyFor<TModel>(this HtmlHelper<TModel> htmlHelper, BaseFriendlyModel model) { return (String.IsNullOrEmpty(model.FriendlyName)) ? model.Guid.ToString() : model.FriendlyName; }
        public static string KeyFor<TModel>(this HtmlHelper<TModel> htmlHelper, Domain.Entities.BaseEntities.BaseFriendlyModel model) { return (String.IsNullOrEmpty(model.FriendlyName)) ? model.Guid.ToString() : model.FriendlyName; }

        private static string ResourceValue<TModel>(this HtmlHelper<TModel> htmlHelper, string resourceName, string language = "")
        {
            return resourceManager.ResourceValue(resourceName, language);
        }

        private static string LookupText<TModel>(this HtmlHelper<TModel> htmlHelper, string lookupName, string name, string language = "")
        {
            return resourceManager.GetLookupText(lookupName, name,  language);
        }

        private static string LookupText<TModel>(this HtmlHelper<TModel> htmlHelper, string lookupName, byte value, string language = "")
        {
            var key = resourceManager.GetLookupEnumKey(lookupName, value, language);
            return key;
        }

        public static string LocationText<TModel>(this HtmlHelper<TModel> htmlHelper, string lookupName, string key, string countryCode = "")
        {
            return resourceManager.GetLookupText(lookupName, key, countryCode);
        }

        public static IHtmlString Photo<TModel>(this HtmlHelper<TModel> htmlHelper, Guid photoGuid, PhotoType photoType = PhotoType.Original, string description = "", bool setId = false, bool encode = false)
        {
            var tb = new TagBuilder("img");
            if(setId) {
                tb.Attributes.Add("id", String.Format("ProfilePhoto"));
            }
            string str = GetPhotoPath(photoGuid, photoType);
            if (encode) {
                try {
                    var fileName = htmlHelper.ViewContext.HttpContext.Server.MapPath(str);
                    var bytes = ToBytes(fileName);
                    var encodedBytes = EncodeBytes(bytes);
                    str = @"data:image/jpg;base64," + encodedBytes;
                } catch {}
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

        public static IHtmlString DisplayDetailFor<TModel, TProp>(this HtmlHelper<TModel> htmlHelper, bool condition, Expression<Func<TModel, TProp>> expression, string countryCode = "")
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            if (condition) {
                var label = new TagBuilder("div");
                label.AddCssClass("display-label");
                label.InnerHtml = String.Format("<b>{0}</b>", htmlHelper.DisplayNameFor(expression).ToHtmlString());
                var text = new TagBuilder("div");
                text.AddCssClass("display-field");
                if(String.IsNullOrWhiteSpace(countryCode))
                    text.InnerHtml = htmlHelper.DisplayTextFor(expression).ToHtmlString();
                else
                    text.SetInnerText(htmlHelper.LocationText("Country", countryCode));
                var container = new TagBuilder("div");
                container.Attributes.Add("title", metadata.Description);
                container.InnerHtml = label + text.ToString();
                var result = htmlHelper.Raw(container.ToString());
                return result;
            }
            return htmlHelper.Raw("");
        }

        public static IHtmlString DisplayProfilePhoto<TModel>(this HtmlHelper<TModel> htmlHelper, ProfileModel profile, PhotoType photoType, string galleryName, bool encode = false)
        {
            var val = ((String.IsNullOrWhiteSpace(profile.FriendlyName)) ? profile.Guid.ToString() : profile.FriendlyName);
            var title = String.Format("{0} - {1} - {2}", profile.Name, (DateTime.Now.Year - profile.BirthYear), htmlHelper.LocationText("Country", profile.From));
            var url = "/Profiles/Show/" + val;
            var anchor = new TagBuilder("a");
            anchor.Attributes.Add("title", title);
            anchor.Attributes.Add("href", url);
            anchor.Attributes.Add("rel", galleryName);
            anchor.InnerHtml = htmlHelper.Photo(profile.ProfilePhotoGuid, photoType, "", false, encode).ToString();
            return htmlHelper.Raw(anchor.ToString());
        }

        private static string Append(string key, string value, string keyName = "key", string valueName = "value") { return String.Format("{{{0}:\"{1}\",{2}:\"{3}\"}}", keyName, key, valueName, value.Replace("\n", "\\n").Replace("\t", "\\t").Replace("\r", "\\r").Replace("'", "\\'").Replace("\"", "\\\"")); }

        private static IHtmlString ToJson<TModel>(this HtmlHelper<TModel> htmlHelper, string lookupName)
        {
            var keyValues = resourceManager.GetLookup(lookupName);
            var sb = new StringBuilder();
            sb.Append("[");
            var i = 0;
            if(keyValues != null)
                foreach (var item in keyValues) {
                    sb.Append(Append(item.Key, item.Value));
                    if (i != keyValues.Count - 1) sb.Append(",");
                    i++;
                }
            sb.Append("]");
            return htmlHelper.Raw(sb.ToString());
        }

        private static IHtmlString ToJson<TModel, TColl, TProp>(this HtmlHelper<TModel> htmlHelper, IEnumerable<TColl> list, Expression<Func<TColl, TProp>> expression)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            var compiledExpression = expression.Compile();
            var enumType = typeof(TProp);
            var firstTime = true;
            foreach (var item in list) {
                if (!firstTime) sb.Append(',');
                var v = Enum.GetName(enumType, compiledExpression.Invoke(item));
                sb.Append(Append(resourceManager.GetLookupText(enumType.Name, v), v, "title"));
                firstTime = false;
            }
            sb.Append("]");
            return htmlHelper.Raw(sb.ToString());
        }

        private static IHtmlString ToJson<TModel, TColl>(this HtmlHelper<TModel> htmlHelper, IEnumerable<TColl> list, Expression<Func<TColl, string>> expression, string lookupName)
        {
            var sb = new StringBuilder();
            sb.Append("[");
            var compiledExpression = expression.Compile();
            var keyValues = resourceManager.GetLookup(lookupName);
            var firstTime = true;
            if (keyValues != null) {
                foreach (var item in list) {
                    if (!firstTime) sb.Append(',');
                    var key = compiledExpression.Invoke(item);
                    if (keyValues.ContainsKey(key)) {
                        var value = keyValues[key];
                        sb.Append(Append(value, key, "title"));
                        firstTime = false;
                    } else {
                        Debug.WriteLine(lookupName + " : " + key);
                    }
                }
            }
            sb.Append("]");
            return htmlHelper.Raw(sb.ToString());
        }

        public static IHtmlString FacebookListFor2<TModel, TColl>(
            this HtmlHelper<TModel> htmlHelper,
            string resourceName,
            IEnumerable<TColl> collection,
            string lookupName,
            Expression<Func<TModel, IList<TColl>>> modelPropertyExpression,
            Expression<Func<TColl, string>> propertyPropertyExpression
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
            var message = htmlHelper.ResourceValue(resourceName + ".Message");
            var tmp = htmlHelper.ResourceValue(resourceName + ".MaxItems");
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
", name, htmlHelper.ToJson(lookupName), htmlHelper.ToJson(collection, propertyPropertyExpression, lookupName), message.Replace("\'", "\\'"), maxItemsText);
            edit.InnerHtml = select.ToString() + htmlHelper.ValidationMessageFor(modelPropertyExpression);
            return htmlHelper.Raw(String.Format("{0}{1}{2}", label, edit, script));
        }


        public static IHtmlString FacebookListFor<TModel, TColl, TProp>(
            this HtmlHelper<TModel> htmlHelper,
            string resourceName,
            IEnumerable<TColl> collection,
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
            var message = htmlHelper.ResourceValue(resourceName + ".Message");
            var tmp = htmlHelper.ResourceValue(resourceName + ".MaxItems");
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
", name, htmlHelper.ToJson(enumType.Name), htmlHelper.ToJson(collection, propertyPropertyExpression), message.Replace("\'", "\\'"), maxItemsText);
            edit.InnerHtml = select.ToString() + htmlHelper.ValidationMessageFor(modelPropertyExpression);
            return htmlHelper.Raw(String.Format("{0}{1}{2}", label, edit, script));
        }

        public static IHtmlString AutoCompleteFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, string>> expression, string name, string lookupName, string urlToCall, string serverCountryCode = "", string clientCountryCode = "", string onselect = "")
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            var id = name.Replace('[', '_').Replace(']', '_');
            var key = "";
            try {
                key = (metadata.Model != null) ? metadata.Model.ToString() : "";
            } catch {
                if (name.IndexOf('[') < 0) throw;
            }
            var value = resourceManager.GetLookupText(lookupName, key, serverCountryCode);
            var hiddenTag = new TagBuilder("input");
            hiddenTag.Attributes.Add("id", id);
            hiddenTag.Attributes.Add("name", name);
            hiddenTag.Attributes.Add("type", "hidden");
            hiddenTag.Attributes.Add("value", key);


            var inputTag = new TagBuilder("input");
            inputTag.AddCssClass("text-box single-line");
            inputTag.Attributes.Add("id", id+"Lookup");
            inputTag.Attributes.Add("name", name + "Lookup");
            inputTag.Attributes.Add("type", "text");
            inputTag.Attributes.Add("value", value);
            var scriptTag = new TagBuilder("script") { InnerHtml = String.Format(@"
$(function() {{
    $('input#{0}Lookup').autocomplete({{
        select: function(event, ui) {{
            $('input#{0}')[0].value = ui.item.key;  
            $('input#{0}Lookup')[0].value = ui.item.value;  
{3}
        }},
        source: function (request, response) {{
            $.ajax({{
                url: '{1}',
                type: 'POST',
                dataType: 'json',
                data: {{ query: request.term{2} }},
                success: function (data) {{
                    response($.map(data, function(item) {{
                        return {{ label: item.Value, value: item.Value, key: item.Key }};
                    }}));
                }}
            }});
        }},
        minLength: 2 // require at least one character from the user
    }});
}});
setInterval(function() {{
    var fl=document.getElementById('{0}Lookup'); 
    if(fl != null && fl.value.length==0) document.getElementById('{0}').value = ''; 
}}, 7);
", id, urlToCall, (!String.IsNullOrWhiteSpace(clientCountryCode) ? ", countryCode:" + clientCountryCode : ""), onselect)
            };
            scriptTag.Attributes.Add("type", "text/javascript");
            return htmlHelper.Raw(String.Format("{0}{1}{2}", inputTag, hiddenTag, scriptTag));
        }

        public static IHtmlString DisplayDetailForEnum<TModel, TColl, TProp>(this HtmlHelper<TModel> htmlHelper, TModel model, Expression<Func<TModel, IList<TColl>>> expression,
                                                                             string lookupName, Expression<Func<TColl, TProp>> collectionPropertyExpression)
        {
            var compiledExpression = expression.Compile();
            var list = compiledExpression.Invoke(model);
            if (list.Count > 0) {
                var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
                var compiledPropertyExpression = collectionPropertyExpression.Compile();
                var sb = new StringBuilder();
                var first = true;
                var lookup = resourceManager.GetLookup(lookupName);
                foreach (var item in list) {
                    if (!first) sb.Append(", "); else first = false;
                    var key = Convert.ToString(compiledPropertyExpression.Invoke(item));
                    if(lookup.ContainsKey(key))
                        sb.Append(lookup[key]);
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
                        if(key == "BirthYear") k = "Age";
                        var val = Convert.ToByte(value);
                        if (val > 0) hasValue = true;
                        var lookupName = k;
                        result = htmlHelper.LookupText(lookupName, htmlHelper.LookupText(lookupName, val));
                    } else if (key == "From") {
                        var val = Convert.ToString(value);
                        if (!String.IsNullOrWhiteSpace(val)) hasValue = true;
                        result = htmlHelper.LocationText((key == "From") ? "Country" : key, val);
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
            switch (key) {
                case "City":
                    value = range;
                    break;
                case "BodyBuild":
                case "HairColor":
                case "EyeColor":
                case "Smokes":
                case "Alcohol":
                case "Religion":
                case "DickSize":
                case "DickThickness":
                case "BreastSize":
                    lookupKey = htmlHelper.LookupText(key, Convert.ToByte(range));
                    value = htmlHelper.LookupText(key, lookupKey);
                    break;
                case "Search":
                    lookupKey = htmlHelper.LookupText("LookingFor", Convert.ToByte(range));
                    value = htmlHelper.LookupText("LookingFor", lookupKey);
                    break;
                case "From":
                    if (key == "From") key = "Country";
                    lookupKey = range;
                    value = htmlHelper.LocationText(key, range);
                    break;
                case "BirthYear":
                    const string k = "Age";
                    var age = (byte)AgeHelper.GetEnum(range);
                    lookupKey = htmlHelper.LookupText(k, age);
                    value = htmlHelper.LookupText(k, lookupKey);
                    break;
                case "Height":
                    var height = (byte)HeightHelper.GetEnum(range);
                    lookupKey = htmlHelper.LookupText(key, height);
                    value = htmlHelper.LookupText(key, lookupKey);
                    break;
            }
            value = (String.IsNullOrWhiteSpace(value)) ? htmlHelper.ResourceValue("NotFilled") : value;
            return value;
        }

        public static string SearchCriteriaKeyText<TModel>(this HtmlHelper<TModel> htmlHelper, string key)
        {
            switch (key) {
                case "BirthYear":
                    return htmlHelper.ResourceValue("Profile.Age.DisplayName");
                default:
                    if (!(key == "From" || key == "City")) 
                        return htmlHelper.ResourceValue(String.Format("Profile.{0}.DisplayName", key));
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
