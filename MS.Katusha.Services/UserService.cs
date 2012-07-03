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
        private readonly INotificationService _notificationService;
        private readonly IUserRepositoryDB _repository;
        private readonly IKatushaGlobalCacheContext _katushaGlobalCache;
        private IProfileRepositoryRavenDB _profileRepositoryRaven;

        public UserService(INotificationService notificationService, IUserRepositoryDB repository, IProfileRepositoryRavenDB profileRepositoryRaven, IKatushaGlobalCacheContext globalCacheContext)
        {
            _notificationService = notificationService;
            _repository = repository;
            _profileRepositoryRaven = profileRepositoryRaven;
            _katushaGlobalCache = globalCacheContext; // new KatushaRavenCacheContext(new CacheObjectRepositoryRavenDB());
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
            var user = new User {Email = email, Password = password, UserName = userName, Expires = DateTime.Now.AddYears(100), EmailValidated = isApproved};
            _repository.Add(user);
            _repository.Save();
            _notificationService.UserRegistered(user);
            status = KatushaMembershipCreateStatus.Success;
            return user;
        }

        public void UpdateUser(User user) { _repository.FullUpdate(user); }

        public User GetUser(long id) { return _repository.Single(u => u.Id == id); }
        public User GetUser(Guid guid) { return _repository.Single(u => u.Guid == guid); }

        public User GetUser(string userName, bool userIsOnline = false)
        {
            var user = _katushaGlobalCache.Get<User>("U:"+userName);
            if (user == null) {
                user = _repository.Single(u => u.UserName == userName);
                _katushaGlobalCache.Add("U:" + userName, user);
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
            _katushaGlobalCache.Add("U:" + user.UserName, user);
            return user;
        }

        public Profile GetProfile(Guid guid)
        {
            var strGuid = guid.ToString();
            var profile = _katushaGlobalCache.Get<Profile>("P:" + strGuid);
            if (profile == null) {
                profile = _profileRepositoryRaven.GetByGuid(guid, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.Photos, p=> p.User, p=> p.Photos);
                _katushaGlobalCache.Add("P:" + strGuid, profile);
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
