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
    public class KatushaLookupListAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly IResourceManager _resourceManager;
        private const string ErrorMessageKeyName = "RangeErrorMessage";
        private const string MaximumName = "Maximum";
        private const string MinimumName = "Minimum";

        public KatushaLookupListAttribute(string PropertyName)
            : base()
        {
            _resourceManager = new ResourceManager();
            this.PropertyName = PropertyName;
        }

        public new int Maximum
        {
            get
            {
                int i;
                return int.TryParse(_resourceManager._C(PropertyName, MaximumName), out i) ? i : int.MaxValue;
            }
        }

        public new int Minimum
        {
            get
            {
                int i;
                return int.TryParse(_resourceManager._C(PropertyName, MinimumName), out i) ? i : int.MinValue;
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            int i;
            return int.TryParse(value.ToString(), out i) ? (i >= Minimum && i <= Maximum ? null : new ValidationResult(ErrorMessage)) : new ValidationResult(ErrorMessage);
        }

        public string PropertyName { get; private set; }

        public new string ErrorMessage { get { return String.Format(_resourceManager._R(PropertyName, ErrorMessageKeyName) ?? "({0} - {1})", Minimum, Maximum); } }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRangeRule(ErrorMessage, Minimum, Maximum);
        }
    }
}
