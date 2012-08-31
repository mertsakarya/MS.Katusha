using MS.Katusha.Domain.Entities;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Interfaces.Services
{
    public interface INotificationService
    {
        string UserRegistered(User user);
        string ProfileCreated(Profile profile);
        string PhotoAdded(Photo photo);
        string MessageSent(Conversation conversation);
        string MessageRead(Conversation conversation);
        string Purchase(User user, Product product);
        string SiteDeployed(User model);
        string TestMail();
    }
}