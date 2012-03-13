using System;
using System.Web.Http;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.WebMailer.Controllers
{
    public class MailController : ApiController
    {
        public void Get(string guid, MailType mailType)
        {
            var model = new MailModel {Guid = Guid.Parse(guid), MailType = mailType};
            Post(model);
        }

        public void Post(MailModel model)
        {
            User user;
            if(model.Guid != Guid.Empty)
            {
            }
        }
    }
}
