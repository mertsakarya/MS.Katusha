using System;
using System.Web.Mvc;

namespace MS.Katusha.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KatushaFieldAttribute : Attribute
    {
        private static readonly IResourceManager ResourceManager = DependencyResolver.Current.GetService<IResourceManager>();


        public KatushaFieldAttribute(string propertyName) { PropertyName = propertyName; }
        public string PropertyName { get; private set; }
        public string GetFromResource(string key, bool mustFind = false, string language = "")
        {
            return ResourceManager.ResourceValue(PropertyName, key, mustFind, language);
        }

        public string GetFromConfiguration(string key, bool mustFind = false)
        {
            return ResourceManager.ConfigurationValue(PropertyName, key, mustFind);
        }
    }
}
