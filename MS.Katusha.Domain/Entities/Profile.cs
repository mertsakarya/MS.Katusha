using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Domain.Enums;
using Newtonsoft.Json;

namespace MS.Katusha.Domain.Entities
{
    public class Profile : BaseFriendlyModel
    {
        public Profile()
        {
            Searches = new HashSet<SearchingFor>();
            Photos = new HashSet<Photo>();
            CountriesToVisit = new HashSet<CountriesToVisit>();
            LanguagesSpoken = new HashSet<LanguagesSpoken>();
            SentMessages = new HashSet<Conversation>();
            RecievedMessages = new HashSet<Conversation>();
            WhoVisited = new HashSet<Visit>();
            Visited = new HashSet<Visit>();
        }

        //[Required]
        //public long UserId { get; set; }
        
        public User User { get; set; }

        // public long StateId { get; set; }
        public State State { get; set; }

        public string Name { get; set; }

        public byte From { get; set; }

        public string City { get; set; }
        public byte BodyBuild { get; set; }
        public byte EyeColor { get; set; }
        public byte HairColor { get; set; }
        public byte Smokes { get; set; }
        public byte Alcohol { get; set; }
        public byte Religion { get; set; }

        public int Height { get; set; }
        public int BirthYear { get; set; }

        public string Description { get; set; }

        public HashSet<SearchingFor> Searches { get; set; }
        public HashSet<Photo> Photos { get; set; }
        public HashSet<CountriesToVisit> CountriesToVisit { get; set; }
        public HashSet<LanguagesSpoken> LanguagesSpoken { get; set; }

        [JsonIgnore]
        public HashSet<Conversation> SentMessages { get; set; }
        [JsonIgnore]
        public HashSet<Conversation> RecievedMessages { get; set; }

        [JsonIgnore]
        public HashSet<Visit> WhoVisited { get; set; }
        [JsonIgnore]
        public HashSet<Visit> Visited { get; set; }

        public override string ToString()
        {
            var str = base.ToString() +
                      String.Format(
                          " | UserId: {0} | State: {1} | Name: {2} | From: {3} | City: {4} | BodyBuild: {5} | EyeColor: {6} | HairColor: {7} | Smokes: {8} | Alcohol: {9} | Religion: {10} | Height: {11} | BirthYear: {12}",
                          (User == null) ? 0 : User.Id, State, Name,
                          Enum.GetName(typeof (Country), From),
                          City,
                          Enum.GetName(typeof (BodyBuild), BodyBuild),
                          Enum.GetName(typeof (EyeColor), EyeColor),
                          Enum.GetName(typeof (HairColor), HairColor),
                          Enum.GetName(typeof (Smokes), Smokes),
                          Enum.GetName(typeof (Alcohol), Alcohol),
                          Enum.GetName(typeof (Religion), Religion),
                          Height,
                          BirthYear
                          );

            if (Searches != null && Searches.Count > 0)
            {
                str += "\r\nSearches: [\r\n";
                foreach (var search in Searches)
                    str += "\t" + search.ToString() + "\r\n";
                str += "]";
            }

            if (Photos != null && Photos.Count > 0)
            {
                str += "\r\nPhotos: [\r\n";
                foreach (var photo in Photos)
                    str += "\t" + photo + "\r\n";
                str += "]";
            }

            if (CountriesToVisit != null && CountriesToVisit.Count > 0)
            {
                str += "\r\nCountriesToVisit: [\r\n";
                foreach (var countriesToVisit in CountriesToVisit)
                    str += "\t" + countriesToVisit + "\r\n";
                str += "]";
            }

            if (LanguagesSpoken != null && LanguagesSpoken.Count > 0)
            {
                str += "\r\nLanguagesSpoken: [\r\n";
                foreach (var languagesSpoken in LanguagesSpoken)
                    str += "\t" + languagesSpoken + "\r\n";
                str += "]";
            }
            return str;
        }
    }
}