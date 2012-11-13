using System;
using System.Text;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Configuration;
using MS.Katusha.Configuration.Data;
using Conversation = MS.Katusha.Domain.Raven.Entities.Conversation;

namespace MS.Katusha.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUserRepositoryDB _userRepository;
        private readonly IProfileRepository _profileRepository;

        private readonly string _adminMailAddress;
        private readonly SettingsData _settings;
        private readonly string _mailTemplatesFolder;

        private const string MailConfirm = "MailConfirm_en.cshtml";

        private const string MailMessageSent = "MailMessageSent_en.cshtml";
        private const string MailMessageSentAdmin = "MailMessageSent_en.cshtml";

        private const string MailMessageRead = "MailMessageRead_en.cshtml";
        private const string MailMessageReadAdmin = "MailMessageRead_en.cshtml";

        private const string ProfileCreatedAdmin = "ProfileCreated_en.cshtml";

        private const string PhotoAddedAdmin = "PhotoAdded_en.cshtml";

        private const string PurchaseMade = "PurchaseMade_en.cshtml";

        private const string SiteDeployedTestMail = "SiteDeployed_en.cshtml";


        public NotificationService(IUserRepositoryDB userRepository, IProfileRepositoryDB profileRepository ) {
            _userRepository = userRepository;
            _profileRepository = profileRepository;
            _settings = KatushaConfigurationManager.Instance.GetSettings();
            _mailTemplatesFolder = _settings.MailViewFolder + @"Views\___MailTemplates\";
            _adminMailAddress = _settings.AdministratorMailAddress;
        }

        public string UserRegistered(User user)
        {
            try {
                if (user.Email.ToLowerInvariant() == "mskatusha.info@gmail.com" || user.Email.ToLowerInvariant() == "mskatusha.user@gmail.com") return "";
                return Mailer.Mailer.SendMail(user.Email, "Katusha says:Welcome! You need one more step to open a new world!", MailConfirm, _mailTemplatesFolder, user);
            } catch (Exception ex) {
                return ex.Message;
            }
        }

        public string MessageSent(Conversation conversation)
        {
            try {
                var toUser = _userRepository.GetByGuid(conversation.ToGuid);
                //Mailer.Mailer.SendMail(_adminMailAddress, String.Format("[NEW MESSAGE] From: {0} To: {1}", conversation.FromName, conversation.ToName), MailMessageSentAdmin, _mailTemplatesFolder, conversation);
                return Mailer.Mailer.SendMail(toUser.Email, String.Format("Katusha says: {0} sent you a message.", conversation.FromName), MailMessageSent, _mailTemplatesFolder, conversation);
            } catch (Exception ex) {
                return ex.Message;
            }
        }

        public string MessageRead(Conversation conversation) {
            //try {
            //    var fromUser = _userRepository.GetByGuid(conversation.FromGuid);
            //    //Mailer.Mailer.SendMail(_adminMailAddress, String.Format("[MESSAGE READ] From: {0} To: {1}", conversation.FromName, conversation.ToName), MailMessageReadAdmin, _mailTemplatesFolder, conversation);
            //    return Mailer.Mailer.SendMail(fromUser.Email, String.Format("Katusha says: {0} read your message.", conversation.ToName), MailMessageRead, _mailTemplatesFolder, conversation);
            //} catch (Exception ex) {
            //    return ex.Message;
            //}
            return "";
        }

        public string Purchase(User user, Product product) {
            try {
                Mailer.Mailer.SendMail(_adminMailAddress, String.Format("[PURCHASE] for {0}. ({1}) ", user.UserName, product.Name), PurchaseMade, _mailTemplatesFolder, user);
                return Mailer.Mailer.SendMail(user.Email, String.Format("Katusha says: {0} enjoy your membership. ({1}) ", user.UserName, product.Name), PurchaseMade, _mailTemplatesFolder, user);
            } catch (Exception ex) {
                return ex.Message;
            }
        }

        public string SiteDeployed(User user)
        {
            return Mailer.Mailer.SendMail(user.Email, "Katusha says: Site deployed @" + DateTime.Now, "@model MS.Katusha.Domain.Entities.User\r\n<h1>@Model.UserName</h1>", _mailTemplatesFolder, user);
        }

        public string ProfileCreated(Profile profile)
        {
            try {
                if (profile.User.Email.ToLowerInvariant() == "mskatusha.info@gmail.com" || profile.User.Email.ToLowerInvariant() == "mskatusha.user@gmail.com") return "";
                return Mailer.Mailer.SendMail(_adminMailAddress, String.Format("[PROFILE CREATED] " + profile.User.UserName), ProfileCreatedAdmin, _mailTemplatesFolder, profile);
            } catch (Exception ex) {
                return ex.Message;
            }
        }

        public string PhotoAdded(Photo photo)
        {
            return "";
            //try {
            //    return Mailer.Mailer.SendMail(_adminMailAddress, String.Format("[PHOTO ADDED] " + photo.FileName), PhotoAddedAdmin, _mailTemplatesFolder, photo);
            //} catch (Exception ex) {
            //    return ex.Message;
            //}
        }

        public string TestMail()
        {
            var user = _userRepository.GetByUserName("mertiko");
            user.Email = "mertsakarya@hotmail.com";
            var profile = _profileRepository.GetByGuid(user.Guid);
            profile.User = user;
            var conversation = new Conversation {FromName = "FROM Mert Sakarya", FromGuid = user.Guid, ToName = "TO Mert Sakarya", ToGuid = user.Guid};
            var photo = new Photo {Guid = profile.ProfilePhotoGuid, FileName = "PhotoFileName.jpg"};
            var sb = new StringBuilder();
            sb.AppendFormat("UserRegistered\r\n{0}\r\n", UserRegistered(user));
            sb.AppendFormat("\r\nSiteDeployed\r\n{0}\r\n", SiteDeployed(user));
            sb.AppendFormat("\r\nMessageSent\r\n{0}\r\n", MessageSent(conversation));
            sb.AppendFormat("\r\nMessageRead\r\n{0}\r\n", MessageRead(conversation));
            sb.AppendFormat("\r\nPurchase\r\n{0}\r\n", Purchase(user, new Product {Name = "SAMPLE PRODUCT"}));
            sb.AppendFormat("\r\nProfileCreated\r\n{0}\r\n", ProfileCreated(profile));
            sb.AppendFormat("\r\nPhotoAdded\r\n{0}\r\n", PhotoAdded(photo));
            return sb.ToString();
        }

    }

}
