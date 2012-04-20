using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using MS.Katusha.Infrastructure.Attributes;

namespace MS.Katusha.Web.Helpers
{
    public class KatushaMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            var modelMetadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);
            var list = attributes.OfType<KatushaFieldAttribute>().ToList();
            foreach (var item in list) {
                bool val;
                modelMetadata.DisplayName = item.GetFromResource("DisplayName");
                modelMetadata.Description = item.GetFromResource("Description");
                modelMetadata.DisplayFormatString = item.GetFromResource("DisplayFormatString");
                modelMetadata.EditFormatString = item.GetFromResource("EditFormatString");
                if (bool.TryParse(item.GetFromConfiguration("HideSurroundingHtml"), out val)) modelMetadata.HideSurroundingHtml = val;
                modelMetadata.NullDisplayText = item.GetFromResource("NullDisplayText");
                modelMetadata.ShortDisplayName = item.GetFromResource("ShortDisplayName");
                if (bool.TryParse(item.GetFromConfiguration("ShowForDisplay"), out val)) modelMetadata.ShowForDisplay = val;
                if (bool.TryParse(item.GetFromConfiguration("ShowForEdit"), out val)) modelMetadata.ShowForEdit = val;
                modelMetadata.TemplateHint = item.GetFromResource("TemplateHint");
                modelMetadata.Watermark = item.GetFromResource("Watermark");
                var additionalValues = item.GetFromResource("AdditionalValues");
                if (!String.IsNullOrWhiteSpace(additionalValues)) {
                    modelMetadata.AdditionalValues.Clear();
                    SetAdditionalValues(modelMetadata, additionalValues);
                }
                additionalValues = item.GetFromConfiguration("AdditionalValues");
                if (!String.IsNullOrWhiteSpace(additionalValues)) {
                    SetAdditionalValues(modelMetadata, additionalValues);
                }
            }
            return modelMetadata;
        }

        private static void SetAdditionalValues(ModelMetadata modelMetadata, string additionalValues)
        {
            foreach (var line in additionalValues.Split('|')) {
                var pos = line.IndexOf('=');
                if (pos > 0 && pos < line.Length)
                    modelMetadata.AdditionalValues.Add(line.Substring(0, pos), line.Substring(pos + 1));
            }
        }
    }
}