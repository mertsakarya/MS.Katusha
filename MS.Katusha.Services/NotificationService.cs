using System;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Services;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Services
{
    public class NotificationService : INotificationService
    {
        public void UserRegistered(User user)
        {
            Mailer.Mailer.SendMail(user.Email, "Welcome! You need one more step to open a new world!", "MailConfirm_en.cshtml", user);
            Mailer.Mailer.SendMail("mertsakarya@gmail.com", "[USER REGISTERED] " + user.UserName, "MailConfirm_en.cshtml", user);
        }

        public void MessageSent(Conversation conversation) {
            //Mailer.Mailer.SendMail(conversation.ToId, String.Format("Katusha says: {0} sent you a message.", conversation.FromName), "MailMessageSent_en.cshtml", conversation);
            Mailer.Mailer.SendMail("mertsakarya@gmail.com", String.Format("[NEW MESSAGE] From: {0} To: {1}", conversation.FromName, conversation.ToName), "MailMessageSent_en.cshtml", conversation);
        }
    }

}
