using MS.Katusha.Domain.Entities;
namespace MS.Katusha.MembershipService
{
    public interface IKatushaMembershipService
    {
        bool ValidateUser(string userName, string password);
        User CreateUser(string userName, string password, string email, object passwordQuestion, object passwordAnswer, bool isApproved, object providerUserKey, out KatushaMembershipCreateStatus status);
        User GetUser(string userName, bool userIsOnline = false);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
    }
}
