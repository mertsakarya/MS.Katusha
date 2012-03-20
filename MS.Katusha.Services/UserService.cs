using System;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepositoryDB _repository;
        private readonly IGirlRepositoryDB _girlRepository;
        private readonly IBoyRepository _boyRepository;

        public UserService(IUserRepositoryDB repository, IGirlRepositoryDB girlRepository, IBoyRepositoryDB boyRepository)
        {
            _repository = repository;
            _girlRepository = girlRepository;
            _boyRepository = boyRepository;
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
            var user = new User {Email = email, Password = password, UserName = userName, Expires = DateTime.Now.AddYears(100).ToUniversalTime()};
            _repository.Add(user);
            _repository.Save();
            SendConfirmationMail(user);
            status = KatushaMembershipCreateStatus.Success;
            return user;
        }

        public void SendConfirmationMail(User user)
        {
            Mailer.Mailer.SendMail(user.Email, "Welcome! You need one more step to open a new world!", "MailConfirm.cshtml", user);
        }

        public User GetUser(string userName, bool userIsOnline = false)
        {
            var user = _repository.Single(u => u.UserName == userName);
            return user;
        }

        public User GetUserByGuid(Guid guid)
        {
            var user = _repository.Single(u => u.Guid == guid);
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
            return user;
        }

        public Profile GetProfile(Guid guid, Sex gender) { 
            if(gender == Sex.Male) {
                return _boyRepository.GetByGuid(guid, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.Photos);
            } else if(gender == Sex.Female)
                return _girlRepository.GetByGuid(guid, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.Photos);
            return null;
        }
    }
}
