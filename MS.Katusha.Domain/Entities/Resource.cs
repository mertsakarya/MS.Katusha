using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class Resource : BaseModel
    {
        public string ResourceKey { get; set; }
        public string Value { get; set; }
        [StringLength(2)]
        public string Language { get; set; }

        [NotMapped]
        public string Key
        {
            get { return ResourceKey + Language.ToString(CultureInfo.InvariantCulture); }
        }
    }
}
