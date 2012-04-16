using System.Configuration;
using System.Globalization;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using NLog;

namespace MS.Katusha.Services.Generators
{
    public class ProfileGenerator : IGenerator<Profile> {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;
        private readonly IPhotosService _photosService;
        private readonly static Logger Logger = LogManager.GetLogger("MS.Katusha.ProfileGenerator");

        public ProfileGenerator(IProfileService profileService, IUserService userService, IPhotosService photosService)
        {
            _profileService = profileService;
            _userService = userService;
            _photosService = photosService;
        }

        public Profile Generate(int extra = 0)
        {
#if DEBUG
            Logger.Info("Profile START:");
#endif
            IGenerator<User> generator = new UserGenerator(_userService);
            var user = generator.Generate();
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
            if (extra == 0) {
                var photoCount = GeneratorHelper.RND.Next(4);
                if (photoCount > 0) {
                    var root = ConfigurationManager.AppSettings["Root_Folder"];
                    var samples = root + "..\\SamplePhotos\\" + ((Sex) profile.Gender).ToString() + "\\";
                    for (var i = 1; i <= photoCount; i++) {
                        var filename = "me" + i.ToString(CultureInfo.InvariantCulture) + ".jpg";
                        var filepath = samples + filename;
                        var photo = _photosService.AddSamplePhoto(profile.Id, GeneratorHelper.RandomString(20, false), root + "Photos\\", filename, filepath);
                        if (GeneratorHelper.RND.Next(3) + 1 == 1)
                            _photosService.MakeProfilePhoto(profile.Id, photo.Guid);
                    }
                }
            }
            var id = _profileService.GetProfileId(profile.Guid);
            if (id > 0) {
                profile = _profileService.GetProfile(id, null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.Photos);
            }

#if DEBUG
            Logger.Info("Profile END");
#endif
            return profile;

        }

    }
}