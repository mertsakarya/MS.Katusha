using MS.Katusha.Domain.Entities;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Interfaces.Services
{
    public interface INotificationService
    {
        void UserRegistered(User user);
        void ProfileCreated(Profile profile);
        void PhotoAdded(Photo photo);
        void MessageSent(Conversation conversation);
        void MessageRead(Conversation conversation);
        void Purchase(User user, Product product);
    }
}