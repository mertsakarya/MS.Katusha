using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class ConfigurationData : BaseModel
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
