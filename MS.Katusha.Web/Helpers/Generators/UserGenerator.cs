using System.Configuration;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using NLog;

namespace MS.Katusha.Web.Helpers.Generators
{
    public interface IGenerator<T>
    {
        T Generate();
    }

    public class SampleGenerator
    {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;

        public SampleGenerator(IProfileService profileService, IUserService userService)
        {
            _profileService = profileService;
            _userService = userService;
        }
        public void CreateSamples(int count)
        {
            var generator = new ProfileGenerator(_profileService, _userService);
            for(var i =0 ; i < count; i++) {
                generator.Generate();
            }
        }
    }

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

    public class ProfileGenerator : IGenerator<Profile> {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;
        private readonly static Logger Logger = LogManager.GetLogger("ProfileGenerator");

        public ProfileGenerator(IProfileService profileService, IUserService userService)
        {
            _profileService = profileService;
            _userService = userService;
        }

        public User User { get; set; }
        
        public User GetRandomUser()
        {
            var generator = new UserGenerator(_userService);
            var user =  generator.Generate();
            return user;
        }

        public Profile Generate()
        {
            var user = GetRandomUser();
            var city = new[] { "Istanbul", "Ankara", "Kiev" };

            var profile = new Profile {
                Name = GeneratorHelper.RandomString(10, true),
                Gender = (byte) (GeneratorHelper.RND.Next((int) Sex.MAX) + 1), 
                UserId = user.Id, 
                Guid = user.Guid,
                BodyBuild = (byte) (GeneratorHelper.RND.Next((int) BodyBuild.MAX) + 1), 
                EyeColor = (byte) (GeneratorHelper.RND.Next((int) EyeColor.MAX) + 1), 
                Smokes = (byte) (GeneratorHelper.RND.Next((int) Smokes.MAX) + 1), 
                From = (byte) (GeneratorHelper.RND.Next((int) Country.MAX) + 1), 
                HairColor = (byte) (GeneratorHelper.RND.Next((int) HairColor.MAX) + 1), 
                Alcohol = (byte) (GeneratorHelper.RND.Next(1 + (int) Alcohol.MAX)),
                Religion = (byte) (GeneratorHelper.RND.Next((int)Religion.MAX) + 1),
                City = city[GeneratorHelper.RND.Next(3)], 
                Description = GeneratorHelper.RandomString(1000, false), 
                Height = GeneratorHelper.RandomNumber(150, 198), 
                BirthYear = GeneratorHelper.RandomNumber(1960, 1989)
            };

            if (profile.Gender == (byte)Sex.Male) {
                profile.DickSize = (byte) (GeneratorHelper.RND.Next((int) DickSize.MAX) + 1);
                profile.DickThickness = (byte) (GeneratorHelper.RND.Next((int) DickThickness.MAX) +1);
            } else {
                profile.BreastSize = (byte) (GeneratorHelper.RND.Next((int) BreastSize.MAX) + 1);
            }
            
            _profileService.CreateProfile(profile);

            for(byte i = 1; i <= (byte)Language.MAX; i++) 
                if(GeneratorHelper.RND.Next(2) + 1 == 1)
                    _profileService.AddLanguagesSpoken(profile.Id, (Language) i);
            
            for(byte i = 1; i <= (byte)LookingFor.MAX; i++)
                if (GeneratorHelper.RND.Next(2) + 1 == 1)
                    _profileService.AddSearches(profile.Id, (LookingFor) i);
            
            for(byte i = 1; i <= (byte)Country.MAX; i++)
                if (GeneratorHelper.RND.Next(2) + 1 == 1)
                    _profileService.AddCountriesToVisit(profile.Id, (Country) i);

            var photoCount = GeneratorHelper.RND.Next(4);
            if (photoCount > 0) {
                var root = ConfigurationManager.AppSettings["Root_Folder"];
                var samples = root + "..\\SamplePhotos\\"+ ((Sex)profile.Gender).ToString() + "\\";
                for (var i = 1; i <= photoCount; i++) {
                    var filename = "me" + i.ToString() + ".jpg";
                    var filepath = samples + filename;
                    _profileService.AddSamplePhoto(profile.Id, GeneratorHelper.RandomString(20, false), root + "Photos\\", filename, filepath);
                }
            }

            var id = _profileService.GetProfileId(profile.Guid);
            if (id > 0) {
                profile = _profileService.GetProfile(id, null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.Photos);
            }

#if DEBUG
            Logger.Info(profile);
#endif
            return profile;

        }

    }
}
