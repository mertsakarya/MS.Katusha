using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Infrastructure.Attributes
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KatushaStringLengthAttribute : StringLengthAttribute, IClientValidatable
    {
        private static readonly IResourceService ResourceService = DependencyResolver.Current.GetService<IResourceService>();

        private const string ErrorMessageKeyName = "StringLengthErrorMessage";
        private const string MaximumLengthName = "MaximumLength";
        private const string MinimumLengthName = "MinimumLength";

        public KatushaStringLengthAttribute(string propertyName)
            : base(0)
        {
            PropertyName = propertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return null;
            var length = value.ToString().Length;
            if (length < MinimumLength || length > MaximumLength)
                return new ValidationResult(ErrorMessage);
            return null;
        }

        public string PropertyName { get; private set; }

        public new int MaximumLength
        {
            get
            {
                int i;
                return int.TryParse(ResourceService.ConfigurationValue(PropertyName, MaximumLengthName), out i) ? i : int.MaxValue;
            }
        }

        public new int MinimumLength
        {
            get
            {
                int i;
                return int.TryParse(ResourceService.ConfigurationValue(PropertyName, MinimumLengthName), out i) ? i : int.MinValue;
            }
        }

        public new string ErrorMessage
        {
            get {
                return String.Format(ResourceService.ResourceValue(PropertyName + "." + ErrorMessageKeyName) ?? "({0} - {1})", MinimumLength, MaximumLength);
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationStringLengthRule(ErrorMessage, MinimumLength, MaximumLength);
        }
    }
}
