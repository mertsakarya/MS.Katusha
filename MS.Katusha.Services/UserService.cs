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
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepositoryDB _repository;
        private readonly IKatushaGlobalCacheContext _katushaGlobalCache;
        private readonly ITokBoxService _tokBoxService;
        private readonly IProfileRepositoryRavenDB _profileRepositoryRaven;

        public UserService(IProductService productService, INotificationService notificationService, IUserRepositoryDB repository, IProfileRepositoryRavenDB profileRepositoryRaven, IKatushaGlobalCacheContext globalCacheContext, ITokBoxService tokBoxService)
        {
            _productService = productService;
            _notificationService = notificationService;
            _repository = repository;
            _profileRepositoryRaven = profileRepositoryRaven;
            _katushaGlobalCache = globalCacheContext; // new KatushaRavenCacheContext(new CacheObjectRepositoryRavenDB());
            _tokBoxService = tokBoxService;
        }

        public bool ValidateUser(string userName, string password)
        {
            var user = _repository.Single(u => u.UserName == userName);
            if (user == null) return false;
            if (user.Password != password) return false;
            var ip = Configuration.KatushaConfigurationManager.Instance.GetSettings().Ip;
            _tokBoxService.CreateSession(user.Guid, ip);
            return true;
        }

        public User CreateUser(string userName, string password, string email, object passwordQuestion, object passwordAnswer, bool isApproved, object providerUserKey, out KatushaMembershipCreateStatus status)
        {
            var existingUser = GetUser(userName);
            if (existingUser != null) { 
                status = KatushaMembershipCreateStatus.DuplicateUserName;
                return null;
            }
            //TODO: Uncomment when you want unique emails.
            //existingUser = _repository.Single(p => p.Email == email);
            //if (existingUser != null)
            //{
            //    status = KatushaMembershipCreateStatus.DuplicateEmail;
            //    return null;
            //}
            var user = new User {Email = email, Password = password, UserName = userName, Expires = DateTime.Now.AddDays(15.0), EmailValidated = isApproved, UserRole = 1};
            _repository.Add(user);
            _repository.Save();
            _notificationService.UserRegistered(user);
            status = KatushaMembershipCreateStatus.Success;
            return user;
        }

        public void UpdateUser(User user) { _repository.FullUpdate(user); }

        public void Purchase(User user, ProductNames productName, string payerId)
        {
            var userData = _repository.SingleAttached(u => u.Id == user.Id);
            var product = _productService.GetProductByName(productName);

            var data = product.GetProductExecutionData();
            userData.MembershipType = data.Membership;
            userData.PaypalPayerId = payerId;
            var date = (DateTime.Now > userData.Expires) ? DateTime.Now : userData.Expires;
            switch((TimeFrameType)data.TimeFrame) {
                case TimeFrameType.Year:
                    userData.Expires = date.AddYears(data.Value);
                    break;
                case TimeFrameType.Month:
                    userData.Expires = date.AddMonths(data.Value);
                    break;
                case TimeFrameType.Day:
                    userData.Expires = date.AddDays(data.Value);
                    break;
                case TimeFrameType.Hour:
                    userData.Expires = date.AddHours(data.Value);
                    break;
            }
            _repository.FullUpdate(userData);
            _repository.Save();
            _notificationService.Purchase(user, product);
        }

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
            var user = _repository.SingleAttached(u => u.UserName == userName);
            if (user == null) return false;
            if (user.Password != oldPassword) return false;
            user.Password = newPassword;
            _repository.FullUpdate(user);
            _repository.Save();
            return true;
        }

        public User ConfirmEMailAddresByGuid(Guid guid)
        {
            var user = _repository.SingleAttached(p=>p.Guid == guid);
            if (user == null) return null;
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
