using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Entities
{
    public class ExtendedProfile
    {
        public ExtendedProfile()
        {
            Photos = new Dictionary<Guid, byte[]>();
        }
        public Profile Profile { get; set; }
        public IDictionary<Guid, byte[]> Photos { get; set; }

        public string ToJson()
        {
            var serializer = JsonSerializer.Create(new JsonSerializerSettings());
            var sb = new StringBuilder();
            using (TextWriter writer = new StringWriter(sb)) {
                serializer.Serialize(writer, this);
            }
            return sb.ToString();
        }
    }
}