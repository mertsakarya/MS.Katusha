using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Domain.Entities
{
    public class PhotoBackup : BaseGuidModel
    {
        [Column(TypeName = "varbinary(max)")]
        public byte[] Data { get; set; }
    }
}
