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
            Images = new Dictionary<string, string>();
            CountriesToVisit = new List<CountriesToVisit>();
            LanguagesSpoken = new List<LanguagesSpoken>();
            Searches = new List<SearchingFor>();
            Photos = new List<Photo>();
            Messages = new List<Conversation>();
            Visits = new List<Visit>();
            Visitors = new List<Visit>();
        }

        public Profile Profile { get; set; }
        public IDictionary<string, string> Images { get; set; }
        public IList<CountriesToVisit> CountriesToVisit { get; set; }
        public IList<LanguagesSpoken> LanguagesSpoken { get; set; }
        public IList<SearchingFor> Searches { get; set; }
        public IList<Photo> Photos { get; set; }
        public User User { get; set; }
        public IList<Conversation> Messages { get; set; }
        public IList<Visit> Visits { get; set; }
        public IList<Visit> Visitors { get; set; }

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