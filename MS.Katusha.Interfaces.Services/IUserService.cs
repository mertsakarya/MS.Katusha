using System;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IUserService
    {
        bool ValidateUser(string userName, string password);
        User CreateUser(string userName, string password, string email, object passwordQuestion, object passwordAnswer, bool isApproved, object providerUserKey, out KatushaMembershipCreateStatus status);
        User GetUser(string userName, bool userIsOnline = false);
        User GetUserByGuid(Guid guid);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        void SendConfirmationMail(User user);
        User ConfirmEMailAddresByGuid(Guid guid);

        Profile GetProfile(Guid guid, Sex gender);
    }
}
