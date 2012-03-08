using System;

using MS.Katusha.Domain.Entities;
using MS.Katusha.IRepositories.Interfaces;

namespace MS.Katusha.MembershipService
{
    public class KatushaMembershipService : IKatushaMembershipService
    {
        private readonly IUserRepositoryDB _repository;

        public KatushaMembershipService(IUserRepositoryDB repository)
        {
            _repository = repository;
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
            Mailer.Mailer.SendMail(email, "Welcome! You need one more step to open a new world!", "MailConfirm.cshtml", user);
            status = KatushaMembershipCreateStatus.Success;
            return user;
        }

        public User GetUser(string userName, bool userIsOnline = false)
        {
            var user = _repository.Single(u => u.UserName == userName);
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
    }
}
