using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;

namespace MS.Katusha.Attributes
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KatushaRegularExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        IResourceManager _resourceManager;
        private string _errorMessage;
        private string _patternResourceName;

        public KatushaRegularExpressionAttribute(string patternResourceName)
            : base(patternResourceName)
        {
            _resourceManager = new ResourceManager();
            _patternResourceName = patternResourceName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || String.IsNullOrEmpty(value.ToString())) return null;

            ValidationResult validationResult = null;
            
            if (!Regex.IsMatch(value.ToString(), Pattern))
            {
                validationResult = new ValidationResult(ErrorMessage);
            }

            return validationResult;
        }

        public new string ErrorMessage
        {
            get
            {
                string errorMessage = string.Empty;
                errorMessage = _resourceManager.GetValueFromCode(ErrorMessageResourceName, (byte)Language);

                if(String.IsNullOrEmpty(errorMessage)){
                    errorMessage = _errorMessage;
                }

                return errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }
        
        public new string Pattern
        {
            get
            {
                string pattern = string.Empty;
                pattern = _resourceManager.GetValueFromCode(_patternResourceName, (byte)Language);

                if (String.IsNullOrEmpty(pattern))
                {
                    pattern = _errorMessage;
                }

                return pattern;
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
            yield return new ModelClientValidationRegexRule(ErrorMessage, Pattern)
            {
                ValidationType = "regex"
            };
        }
    }
}
