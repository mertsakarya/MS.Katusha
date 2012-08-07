using System;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUserRepositoryDB _userRepository;

        private const string AdminMailAddress = "mskatusha.info@gmail.com";

        private const string MailConfirm = "MailConfirm_en.cshtml";
        private const string MailConfirmAdmin = "MailConfirm_en.cshtml";

        private const string MailMessageSent = "MailMessageSent_en.cshtml";
        private const string MailMessageSentAdmin = "MailMessageSent_en.cshtml";

        private const string MailMessageRead = "MailMessageRead_en.cshtml";
        private const string MailMessageReadAdmin = "MailMessageRead_en.cshtml";

        private const string ProfileCreatedAdmin = "ProfileCreated_en.cshtml";

        private const string PhotoAddedAdmin = "PhotoAdded_en.cshtml";

        private const string PurchaseMade = "PurchaseMade_en.cshtml";

        public NotificationService(IUserRepositoryDB userRepository ) {
            _userRepository = userRepository;
        }

        public void UserRegistered(User user)
        {
            try {
                Mailer.Mailer.SendMail(user.Email, "Katusha says:Welcome! You need one more step to open a new world!", MailConfirm, user);
                Mailer.Mailer.SendMail(AdminMailAddress, "[USER REGISTERED] " + user.UserName, MailConfirmAdmin, user);
            } catch(Exception) {}
        }

        public void MessageSent(Conversation conversation)
        {
            try {
                var toUser = _userRepository.GetById(conversation.ToId);
                Mailer.Mailer.SendMail(toUser.Email, String.Format("Katusha says: {0} sent you a message.", conversation.FromName), MailMessageSent, conversation);
                Mailer.Mailer.SendMail(AdminMailAddress, String.Format("[NEW MESSAGE] From: {0} To: {1}", conversation.FromName, conversation.ToName), MailMessageSentAdmin, conversation);
            } catch(Exception) {}
        }

        public void MessageRead(Conversation conversation) {
            try {
                var fromUser = _userRepository.GetById(conversation.FromId);
                Mailer.Mailer.SendMail(fromUser.Email, String.Format("Katusha says: {0} read your message.", conversation.ToName), MailMessageRead, conversation);
                Mailer.Mailer.SendMail(AdminMailAddress, String.Format("[MESSAGE READ] From: {0} To: {1}", conversation.FromName, conversation.ToName), MailMessageReadAdmin, conversation);
            } catch(Exception) {}
        }

        public void Purchase(User user, Product product) {
            try {
                Mailer.Mailer.SendMail(user.Email, String.Format("Katusha says: {0} enjoy your membership. ({1}) ", user.UserName, product.Name), PurchaseMade, user);
                Mailer.Mailer.SendMail(AdminMailAddress, String.Format("[PURCHASE] for {0}. ({1}) ", user.UserName, product.Name), PurchaseMade, user);
            } catch (Exception) { }
        }

        public void ProfileCreated(Profile profile)
        {
            try {
                Mailer.Mailer.SendMail(AdminMailAddress, String.Format("[PROFILE CREATED] " + profile.User.UserName), ProfileCreatedAdmin, profile);
            } catch(Exception) {}
        }

        public void PhotoAdded(Photo photo)
        {
            try {
                Mailer.Mailer.SendMail(AdminMailAddress, String.Format("[PHOTO ADDED] " + photo.FileName), PhotoAddedAdmin, photo);
            } catch(Exception) {}
        }

    }

}
