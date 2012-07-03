using MS.Katusha.Domain.Entities;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Interfaces.Services
{
    public interface INotificationService
    {
        void UserRegistered(User user);
        void MessageSent(Conversation conversation);
    }
}