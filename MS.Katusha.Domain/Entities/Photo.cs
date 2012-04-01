using System;
using System.ComponentModel.DataAnnotations;
using MS.Katusha.Domain.Entities.BaseEntities;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Entities
{
    public class Photo : BaseGuidModel
    {
        [JsonIgnore]
        public long ProfileId { get; set; }
        [JsonIgnore]
        public Profile Profile { get; set; }
        public string Description { get; set; }

        ////newFile.FileContents = System.IO.File.ReadAllBytes("TextFile1.txt");
        ////System.IO.File.WriteAllBytes(file.FileID + ".txt", file.FileContents);
        //[Column(TypeName = "varbinary(800000000)")]
        ////[Column(TypeName = "image")]
        //public byte[] FileContents { get; set; }
        //[Column(TypeName = "varbinary(800000000)")]
        ////[Column(TypeName = "image")]
        //public byte[] SmallFileContents { get; set; }

        public string ContentType { get; set; }

        public string FileName { get; set; }

        public override string ToString()
        {
            return base.ToString() + String.Format(" | ProfileId: {0} | Description: [\r\n{1}\r\n]", ProfileId, Description);
        }
    }
}