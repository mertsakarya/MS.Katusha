using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Infrastructure.Attributes
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KatushaRegularExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        private static readonly IResourceService ResourceService = DependencyResolver.Current.GetService<IResourceService>();

        private const string ErrorMessageKeyName = "RegularExpressionErrorMessage";
        private const string PatternName = "RegularExpressionPattern";
        public KatushaRegularExpressionAttribute(string propertyName) : base(propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || String.IsNullOrEmpty(value.ToString())) return null;
            ValidationResult validationResult = null;
            if (!Regex.IsMatch(value.ToString(), Pattern))
                validationResult = new ValidationResult(ErrorMessage);
            return validationResult;
        }

        public new string ErrorMessage { get { return ResourceService.ResourceValue(PropertyName + "." + ErrorMessageKeyName) ?? "*"; } }

        public new string Pattern { get { return ResourceService.ConfigurationValue(PropertyName, PatternName); } }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) { yield return new ModelClientValidationRegexRule(ErrorMessage, Pattern); }
    }
}
