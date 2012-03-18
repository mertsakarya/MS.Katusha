using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class ResourceLookup : BaseModel
    {
        public string ResourceKey { get; set; }
        public string Value { get; set; }
        public byte Language { get; set; }
        public string LookupName { get; set; }
        public byte Order { get; set; }

        [NotMapped]
        public string Key
        {
            get { return LookupName + Language.ToString(); }
        }
        public override string ToString()
        {
            return string.Format("{0} | LookupName: {4} | ResourceKey: {1} | Value: {2} | Language: {3} | Order: {5} | Key: {6}", base.ToString(), ResourceKey, Value, Language, LookupName, Order, Key);
        }
    }
}
