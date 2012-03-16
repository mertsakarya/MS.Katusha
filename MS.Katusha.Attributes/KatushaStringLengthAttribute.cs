using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;

namespace MS.Katusha.Attributes
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KatushaStringLengthAttribute : StringLengthAttribute, IClientValidatable
    {
        private readonly IResourceManager _resourceManager;
        private string _errorMessage;

        public KatushaStringLengthAttribute(int maximumLength) 
            : base(maximumLength)
        {
            _resourceManager = new ResourceManager();
            
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var length = value.ToString().Length;
            if (length < MinimumLength || length > MaximumLength)
                return new ValidationResult(ErrorMessage);
            return null;
        }

        public Language Language { get; set; }

        public new string ErrorMessage
        {
            get
            {
                var errorMessage = _resourceManager._R(ErrorMessageResourceName, (byte)Language);
                if (String.IsNullOrEmpty(errorMessage))
                    errorMessage = _errorMessage;
                return errorMessage;
            }
            set { _errorMessage = value; }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRangeRule(ErrorMessage, base.MinimumLength, base.MaximumLength)
            {
                ValidationType = "range"
            };
        }
    }
}
