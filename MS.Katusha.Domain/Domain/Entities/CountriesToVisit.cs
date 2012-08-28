using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Entities
{
    public class CountriesToVisit : BaseModel
    {
        [JsonIgnore]
        public long ProfileId { get; set; }
        [JsonIgnore]
        public Profile Profile { get; set; }
        [StringLength(2)]
        public string Country { get; set; }


        public override string ToString()
        {
            return Country;
        }
    }
}