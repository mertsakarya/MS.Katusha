using System;
using System.Security.Principal;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IUserService
    {
        bool ValidateUser(string userName, string password);
        User CreateUser(string userName, string password, string email, object passwordQuestion, object passwordAnswer, bool isApproved, object providerUserKey, out KatushaMembershipCreateStatus status);
        void UpdateUser(User user);
        User GetUser(string userName, bool userIsOnline = false);
        User GetUser(long id);
        User GetUser(Guid guid);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        User ConfirmEMailAddresByGuid(Guid guid);

        Profile GetProfile(Guid guid);
        User GetUserByFacebookUId(string uid);
        void Purchase(User user, ProductNames productName, string PayerId);
    }
}
