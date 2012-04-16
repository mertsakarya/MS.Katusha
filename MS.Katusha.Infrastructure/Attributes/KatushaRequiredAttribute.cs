using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MS.Katusha.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KatushaRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        readonly IResourceManager _resourceManager;
        private const string ErrorMessageKeyName = "RequiredErrorMessage";

        public KatushaRequiredAttribute(string propertyName)
        {
            PropertyName = propertyName;
            _resourceManager = DependencyResolver.Current.GetService<IResourceManager>();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = base.IsValid(value, validationContext);
            if (validationResult != null && !string.IsNullOrEmpty(validationResult.ErrorMessage))
                validationResult.ErrorMessage = ErrorMessage;
            return validationResult;
        }

        protected string PropertyName { get; private set; }

        public new string ErrorMessage { get { return _resourceManager._R(PropertyName, ErrorMessageKeyName) ?? "*"; } }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRequiredRule(ErrorMessage);
        }
    }
}
