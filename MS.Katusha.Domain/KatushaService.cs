using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Domain
{
    public interface IKatushaService
    {
        long GetUserIdByGuid(Guid guid);
        long GetUserIdByUserName(string userName);

        User GetUserById(long id);
        Profile GetProfileById(long id);
        Girl GetGirlById(long id);
        Boy GetBoyById(long id);

        Profile GetConversations(Profile profile);
        Profile GetConversationBetween(Profile from, Profile to);

        void GetPhotos(Profile profile);
        void GetSearchingFor(Profile profile);
        void GetLanguageSpoken(Profile profile);
        void GetCountriesToVisit(Profile profile);

    }

    public class KatushaService : IKatushaService
    {
        private readonly IKatushaRepository _repository;
        private readonly IKatushaContext _context;

        public KatushaService(IKatushaRepository repository)
        {
            this._repository = repository;
            this._context = repository.Context;
        }


        public long GetUserIdByGuid(Guid guid)
        {
            var id = (from user in _context.Users where user.Guid == guid select user.Id).SingleOrDefault();
            return id;
        }

        public long GetUserIdByUserName(string userName)
        {
            var id = (from user in _context.Users where user.UserName == userName select user.Id).SingleOrDefault();
            return id;
        }

        public User GetUserById(long id)
        {
            var user = (from u in _context.Users where u.Id == id select u).SingleOrDefault();
            return user;
        }

        public Profile GetProfileById(long id)
        {
            var user = GetUserById(id);
            return GetProfileByUser(user);
        }

        public Profile GetProfileByUser(User user)
        {
            var profile = _repository.GetProfile(user);
            return profile;
        }

        public Girl GetGirlById(long id)
        {
            var girl = (from item in _context.Girls where item.Id == id select item).SingleOrDefault();
            return girl;
        }

        public Boy GetBoyById(long id)
        {
            var boy = (from item in _context.Boys where item.Id == id select item).SingleOrDefault();
            return boy;
        }


        public Profile GetConversations(Profile profile)
        {
            throw new NotImplementedException();
        }

        public Profile GetConversationBetween(Profile from, Profile to)
        {
            throw new NotImplementedException();
        }

        public void GetPhotos(Profile profile)
        {
            _repository.GetPhotos(profile);
        }

        public void GetSearchingFor(Profile profile)
        {
            _repository.GetSearchingFor(profile);
        }

        public void GetLanguageSpoken(Profile profile)
        {
            _repository.GetLanguageSpoken(profile);
        }

        public void GetCountriesToVisit(Profile profile)
        {
            _repository.GetCountriesToVisit(profile);
        }
    }
}
