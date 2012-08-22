using System;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Services.Configuration;
using MS.Katusha.Services.Configuration.Data;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUserRepositoryDB _userRepository;

        private readonly string _adminMailAddress;
        private readonly SettingsData _settings;
        private readonly string _mailTemplatesFolder;

        private const string MailConfirm = "MailConfirm_en.cshtml";
        private const string MailConfirmAdmin = "MailConfirm_en.cshtml";

        private const string MailMessageSent = "MailMessageSent_en.cshtml";
        private const string MailMessageSentAdmin = "MailMessageSent_en.cshtml";

        private const string MailMessageRead = "MailMessageRead_en.cshtml";
        private const string MailMessageReadAdmin = "MailMessageRead_en.cshtml";

        private const string ProfileCreatedAdmin = "ProfileCreated_en.cshtml";

        private const string PhotoAddedAdmin = "PhotoAdded_en.cshtml";

        private const string PurchaseMade = "PurchaseMade_en.cshtml";

        private const string SiteDeployedTestMail = "SiteDeployed_en.cshtml";


        public NotificationService(IUserRepositoryDB userRepository ) {
            _userRepository = userRepository;
            _settings = KatushaConfigurationManager.Instance.GetSettings();
            _mailTemplatesFolder = _settings.MailViewFolder + @"Views\___MailTemplates\";
            _adminMailAddress = _settings.AdministratorMailAddress;
        }

        public void UserRegistered(User user)
        {
            try {
                Mailer.Mailer.SendMail(user.Email, "Katusha says:Welcome! You need one more step to open a new world!", _mailTemplatesFolder, MailConfirm, user);
                Mailer.Mailer.SendMail(_adminMailAddress, "[USER REGISTERED] " + user.UserName, _mailTemplatesFolder, MailConfirmAdmin, user);
            } catch(Exception) {}
        }

        public void MessageSent(Conversation conversation)
        {
            try {
                var toUser = _userRepository.GetByGuid(conversation.ToGuid);
                Mailer.Mailer.SendMail(toUser.Email, String.Format("Katusha says: {0} sent you a message.", conversation.FromName), MailMessageSent, _mailTemplatesFolder, conversation);
                Mailer.Mailer.SendMail(_adminMailAddress, String.Format("[NEW MESSAGE] From: {0} To: {1}", conversation.FromName, conversation.ToName), MailMessageSentAdmin, _mailTemplatesFolder, conversation);
            } catch(Exception) {}
        }

        public void MessageRead(Conversation conversation) {
            try {
                var fromUser = _userRepository.GetByGuid(conversation.FromGuid);
                Mailer.Mailer.SendMail(fromUser.Email, String.Format("Katusha says: {0} read your message.", conversation.ToName), MailMessageRead, _mailTemplatesFolder, conversation);
                Mailer.Mailer.SendMail(_adminMailAddress, String.Format("[MESSAGE READ] From: {0} To: {1}", conversation.FromName, conversation.ToName), MailMessageReadAdmin, _mailTemplatesFolder, conversation);
            } catch(Exception) {}
        }

        public void Purchase(User user, Product product) {
            try {
                Mailer.Mailer.SendMail(user.Email, String.Format("Katusha says: {0} enjoy your membership. ({1}) ", user.UserName, product.Name), PurchaseMade, _mailTemplatesFolder, user);
                Mailer.Mailer.SendMail(_adminMailAddress, String.Format("[PURCHASE] for {0}. ({1}) ", user.UserName, product.Name), PurchaseMade, _mailTemplatesFolder, user);
            } catch (Exception) { }
        }

        public string SiteDeployed(User user)
        {
            return Mailer.Mailer.SendMail(user.Email, "Katusha says: Site deployed @" + DateTime.Now, "@model MS.Katusha.Domain.Entities.User\r\n<h1>@Model.UserName</h1>", _mailTemplatesFolder, user);
        }

        public void ProfileCreated(Profile profile)
        {
            try {
                Mailer.Mailer.SendMail(_adminMailAddress, String.Format("[PROFILE CREATED] " + profile.User.UserName), ProfileCreatedAdmin, _mailTemplatesFolder, profile);
            } catch(Exception) {}
        }

        public void PhotoAdded(Photo photo)
        {
            try {
                Mailer.Mailer.SendMail(_adminMailAddress, String.Format("[PHOTO ADDED] " + photo.FileName), PhotoAddedAdmin, _mailTemplatesFolder, photo);
            } catch(Exception) {}
        }

    }

}
