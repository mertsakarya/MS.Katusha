using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using NLog;

namespace MS.Katusha.Services.Generators
{
    public class UserGenerator : IGenerator<User>
    {
        private readonly IUserService _userService;
        private readonly static Logger Logger = LogManager.GetLogger("MS.Katusha.UserGenerator");

        public UserGenerator(IUserService userService) {
            _userService = userService;
        }

        public User Generate(int extra = 0) { 
            KatushaMembershipCreateStatus createStatus;
            var user = _userService.CreateUser(GeneratorHelper.RandomString(4, true), "123456", "mertsakarya@gmail.com", passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);
            
#if DEBUG
            Logger.Info("User Created: " + user.UserName);
#endif
            return user;
        }
    }
}
