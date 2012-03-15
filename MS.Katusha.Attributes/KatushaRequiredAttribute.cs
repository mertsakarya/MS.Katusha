using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;
using System.Web.Mvc;

namespace MS.Katusha.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KatushaRequiredAttribute : RequiredAttribute, IClientValidatable
    {
        readonly IResourceManager _resourceManager;
        private string _errorMessage;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult validationResult = base.IsValid(value, validationContext);

            if (validationResult != null && !string.IsNullOrEmpty(validationResult.ErrorMessage))
            {
                validationResult.ErrorMessage = ErrorMessage;
            }

            return validationResult;
        }

        public KatushaRequiredAttribute()
        {
            _resourceManager = new ResourceManager();
        }

        public new string ErrorMessage
        {
            get
            {
                _errorMessage = _resourceManager.GetValueFromCode(ErrorMessageResourceName, (byte)Language);
                return _errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        public new Language Language { get; set; }
        public new string ErrorMessageResourceName { get; set; }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "required"
            };
        }
    }
}
