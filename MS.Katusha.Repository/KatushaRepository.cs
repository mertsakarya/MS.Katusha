using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Enums;

namespace MS.Katusha.Repository
{
    public interface IKatushaRepository
    {
        IKatushaContext Context { get; }
        void GetPhotos(Profile profile);
        void GetSearchingFor(Profile profile);
        void GetLanguageSpoken(Profile profile);
        void GetCountriesToVisit(Profile profile);
        Profile GetProfile(User user);
        Profile GetProfile(long id, Sex gender);
    }
    
    public class KatushaRepository : IKatushaRepository
    {
        public KatushaRepository()
        {
            this.Context = new KatushaContext(); ;
        }

        public IKatushaContext Context { get; private set; }

        public void GetPhotos(Profile profile)
        {
            var items = (from item in Context.Photos where item.ProfileId == profile.Id select item);
            profile.Photos.Clear();
            foreach(var item in items)
                profile.Photos.Add(item);
        }

        public void GetLanguageSpoken(Profile profile)
        {
            var items = (from item in Context.LanguagesSpoken where item.ProfileId == profile.Id select item);
            profile.LanguagesSpoken.Clear();
            foreach (var item in items)
                profile.LanguagesSpoken.Add(item);
        }

        public void GetCountriesToVisit(Profile profile)
        {
            var items = (from item in Context.CountriesToVisit where item.ProfileId == profile.Id select item);
            profile.CountriesToVisit.Clear();
            foreach (var item in items)
                profile.CountriesToVisit.Add(item);
        }

        public Profile GetProfile(User user)
        {

            return GetProfile(user.Id, (Sex) user.Gender);
        }

        public Profile GetProfile(long id, Sex gender)
        {
            Profile profile;
            if (gender == Sex.Male)
            {
                profile = (from item in Context.Boys.Include("State") where item.Id == id select item).SingleOrDefault();
            }
            else
            {
                profile = (from item in Context.Girls.Include("State") where item.Id == id select item).SingleOrDefault();
            }

            return profile;
        }


        public void GetSearchingFor(Profile profile)
        {
            var items = (from item in Context.Searches where item.ProfileId == profile.Id select item);
            profile.Searches.Clear();
            foreach (var item in items)
                profile.Searches.Add(item);
        }
    }
}
