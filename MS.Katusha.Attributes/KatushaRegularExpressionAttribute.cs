﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using MS.Katusha.Infrastructure;

namespace MS.Katusha.Attributes
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KatushaRegularExpressionAttribute : RegularExpressionAttribute, IClientValidatable
    {
        private readonly IResourceManager _resourceManager;
        private const string ErrorMessageKeyName = "RegularExpressionErrorMessage";
        private const string PatternName = "RegularExpressionPattern";
        public KatushaRegularExpressionAttribute(string PropertyName) : base(PropertyName)
        {
            _resourceManager = new ResourceManager();
            this.PropertyName = PropertyName;
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

        public new string ErrorMessage { get { return _resourceManager._R(PropertyName, ErrorMessageKeyName) ?? "*"; } }

        public new string Pattern { get { return _resourceManager._C(PropertyName, PatternName); } }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context) { yield return new ModelClientValidationRegexRule(ErrorMessage, Pattern); }
    }
}