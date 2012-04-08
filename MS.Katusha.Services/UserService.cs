using System;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepositoryDB _repository;
        private readonly IProfileRepositoryDB _profileRepository;
        private readonly IKatushaCacheContext _katushaCache;

        public UserService(IUserRepositoryDB repository, IProfileRepositoryDB profileRepository, IKatushaCacheContext cacheContext)
        {
            _repository = repository;
            _profileRepository = profileRepository;
            _katushaCache = cacheContext; // new KatushaRavenCacheContext(new CacheObjectRepositoryRavenDB());
        }

        public bool ValidateUser(string userName, string password)
        {
            var user = _repository.Single(u => u.UserName == userName);
            if (user == null) return false;
            if (user.Password != password) return false;
            return true;
        }

        public User CreateUser(string userName, string password, string email, object passwordQuestion, object passwordAnswer, bool isApproved, object providerUserKey, out KatushaMembershipCreateStatus status)
        {
            var existingUser = GetUser(userName);
            if (existingUser != null) { 
                status = KatushaMembershipCreateStatus.DuplicateUserName;
                return null;
            }
            //existingUser = _repository.Single(p => p.Email == email);
            //if (existingUser != null)
            //{
            //    status = KatushaMembershipCreateStatus.DuplicateEmail;
            //    return null;
            //}
            var user = new User {Email = email, Password = password, UserName = userName, Expires = DateTime.Now.AddYears(100).ToUniversalTime(), EmailValidated = isApproved};
            _repository.Add(user);
            _repository.Save();
            SendConfirmationMail(user);
            status = KatushaMembershipCreateStatus.Success;
            return user;
        }

        public void SendConfirmationMail(User user)
        {
            Mailer.Mailer.SendMail(user.Email, "Welcome! You need one more step to open a new world!", "MailConfirm_en.cshtml", user);
        }

        public User GetUser(string userName, bool userIsOnline = false)
        {
            var user = _katushaCache.Get<User>("U:"+userName);
            if (user == null) {
                user = _repository.Single(u => u.UserName == userName);
                _katushaCache.Add("U:" + userName, user);
            }
            return user;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            var user = _repository.Single(u => u.UserName == userName);
            if (user == null) return false;
            if (user.Password != oldPassword) return false;
            user.Password = newPassword;
            _repository.FullUpdate(user);
            _repository.Save();
            return true;
        }

        public User ConfirmEMailAddresByGuid(Guid guid)
        {
            var user = _repository.GetByGuid(guid);
            user.EmailValidated = true;
            _repository.FullUpdate(user);
            _repository.Save();
            _katushaCache.Add("U:" + user.UserName, user);
            return user;
        }

        public Profile GetProfile(Guid guid)
        {
            var strGuid = guid.ToString();
            var profile = _katushaCache.Get<Profile>("P:" + strGuid);
            if (profile == null) {
                profile = _profileRepository.GetByGuid(guid, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.Photos, p=> p.User, p=> p.State);
                _katushaCache.Add("P:" + strGuid, profile);
            }
            return profile;
        }

        public User GetUserByFacebookUId(string uid)
        {
            var user = _repository.Single(u => u.FacebookUid == uid);
            return user;
        }

    }
}
