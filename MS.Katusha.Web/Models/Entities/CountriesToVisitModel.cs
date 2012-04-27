using System;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;
using Newtonsoft.Json;

namespace MS.Katusha.Web.Models.Entities
{
    public class CountriesToVisitModel : BaseModel
    {
        [JsonIgnore]
        public long ProfileId { get; set; }

        [JsonIgnore]
        public ProfileModel Profile { get; set; }
        
        [StringLength(2)]
        public string Country { get; set; }


        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | Country: {1}", ProfileId, Country);
        }
    }
}