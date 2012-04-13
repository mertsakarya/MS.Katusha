using System;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class KatushaFieldAttribute : Attribute
    {
        private static readonly IResourceManager ResourceManager = new ResourceManager();

        public KatushaFieldAttribute(string propertyName) { PropertyName = propertyName; }
        public string PropertyName { get; private set; }
        public string GetFromResource(string key, bool mustFind = false, Language language = 0)
        {
            return ResourceManager._R(PropertyName, key, mustFind, (byte)language);
        }

        public string GetFromConfiguration(string key, bool mustFind = false)
        {
            return ResourceManager._C(PropertyName, key, mustFind);
        }
    }
}
