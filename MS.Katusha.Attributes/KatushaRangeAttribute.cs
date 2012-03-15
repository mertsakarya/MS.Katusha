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
    public class KatushaRangeAttribute : RangeAttribute, IClientValidatable
    {
        IResourceManager _resourceManager;
        private string _errorMessage;

        public KatushaRangeAttribute(double minimum, double maximum) 
            : base(minimum, maximum)
        {
            _resourceManager = new ResourceManager();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            //ValidationResult validationResult = base.IsValid(value, validationContext);

            ValidationResult validationResult = null;

            if (double.Parse(value.ToString()) < double.Parse(Minimum.ToString()) ||
                double.Parse(value.ToString()) > double.Parse(Maximum.ToString()))
            {
                validationResult = new ValidationResult(ErrorMessage);
            }

            //if (validationResult != null && !string.IsNullOrEmpty(validationResult.ErrorMessage))
            //{
            //    validationResult.ErrorMessage = ErrorMessageResourceName;
            //}

            return validationResult;
        }

        public new Language Language { get; set; }
        public new string ErrorMessage
        {
            get
            {
                string errorMessage = string.Empty;
                errorMessage = _resourceManager.GetValueFromCode(ErrorMessageResourceName, (byte)Language);

                if (String.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = _errorMessage;
                }

                return errorMessage;
            }
            set
            {
                _errorMessage = value;
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRangeRule(ErrorMessage, base.Minimum, base.Maximum)
            {
                ValidationType = "range"
            };
        }
    }
}
