using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using NLog;

namespace MS.Katusha.Services.Generators
{
    public class UserGenerator : IGenerator<User>
    {
        private readonly IUserService _service;
        private readonly static Logger Logger = LogManager.GetLogger("UserGenerator");

        public UserGenerator(IUserService service) {
            _service = service;
        }

        public User Generate() { 
            KatushaMembershipCreateStatus createStatus;
            var user = _service.CreateUser(GeneratorHelper.RandomString(4, true), "123456", "mertsakarya@gmail.com", passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);
            
#if DEBUG
            Logger.Info(user);
#endif
            return user;
        }
    }
}
