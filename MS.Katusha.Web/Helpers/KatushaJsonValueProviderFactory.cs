using System;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MS.Katusha.Web.Helpers
{
    public sealed class KatushaJsonValueProviderFactory : ValueProviderFactory
    {
        public override IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext");

            if (!controllerContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
                return null;

            var reader = new StreamReader(controllerContext.HttpContext.Request.InputStream);
            var bodyText = reader.ReadToEnd();

            var deserializeObject = JsonConvert.DeserializeObject<ExpandoObject>(bodyText, new ExpandoObjectConverter());
            var val = String.IsNullOrEmpty(bodyText) ? null : new DictionaryValueProvider<object>(deserializeObject, CultureInfo.CurrentCulture);
            return val;
        }
    }
}