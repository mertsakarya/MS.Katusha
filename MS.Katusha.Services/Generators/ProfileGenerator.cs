using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure;
using MS.Katusha.Interfaces.Services;
using NLog;

namespace MS.Katusha.Services.Generators
{
    public class ProfileGenerator : IGenerator<Profile> {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;
        private readonly IPhotosService _photosService;
        private readonly static Logger Logger = LogManager.GetLogger("MS.Katusha.ProfileGenerator");
        private readonly IResourceService _resourceService;

        public ProfileGenerator(IProfileService profileService, IUserService userService, IPhotosService photosService, IResourceService resourceService)
        {
            _profileService = profileService;
            _userService = userService;
            _photosService = photosService;
            _resourceService = resourceService;
        }

        public Profile Generate(int extra = 0)
        {
#if DEBUG
            Logger.Info("Profile START:");
#endif
            IGenerator<User> generator = new UserGenerator(_userService);
            var user = generator.Generate();

            var geoCountries = _resourceService.GetCountries();
            var countries = new List<string>(geoCountries.Count);
            countries.AddRange(geoCountries.Select(country => country.Key));

            var geoLanguages = _resourceService.GetLanguages();
            var languages = new List<string>(geoLanguages.Count);
            languages.AddRange(geoLanguages.Select(language => language.Key));
            var cn = GeneratorHelper.RND.Next(countries.Count) + 1;
            if (cn <= 0 || cn > countries.Count - 2) cn = 3;
            var co = countries[cn];
            var cl = _resourceService.GetCities(co);
            var ci = cl[GeneratorHelper.RND.Next(cl.Count)];
            var profile = new Profile {
                                          Name = GeneratorHelper.RandomString(10, true),
                                          Gender = (byte) (GeneratorHelper.RND.Next((int) Sex.MAX) + 1), 
                                          UserId = user.Id, 
                                          Guid = user.Guid,
                                          BodyBuild = (byte) (GeneratorHelper.RND.Next((int) BodyBuild.MAX) + 1), 
                                          EyeColor = (byte) (GeneratorHelper.RND.Next((int) EyeColor.MAX) + 1), 
                                          Smokes = (byte) (GeneratorHelper.RND.Next((int) Smokes.MAX) + 1), 
                                          From = co, 
                                          HairColor = (byte) (GeneratorHelper.RND.Next((int) HairColor.MAX) + 1), 
                                          Alcohol = (byte) (GeneratorHelper.RND.Next(1 + (int) Alcohol.MAX)),
                                          Religion = (byte) (GeneratorHelper.RND.Next((int)Religion.MAX) + 1),
                                          City = ci, 
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

            foreach (var language in languages.Where(language => GeneratorHelper.RND.Next(10) + 1 == 1))
                _profileService.AddLanguagesSpoken(profile.Id, language);
            
            for(byte i = 1; i <= (byte)LookingFor.MAX; i++)
                if (GeneratorHelper.RND.Next(2) + 1 == 1)
                    _profileService.AddSearches(profile.Id, (LookingFor) i);

            foreach (var country in countries.Where(country => GeneratorHelper.RND.Next(10) + 1 == 1))
                _profileService.AddCountriesToVisit(profile.Id, country); 

            if (extra == 0) {
                var photoCount = GeneratorHelper.RND.Next(4);
                if (photoCount > 0) {
                    var root = HttpContext.Current.Server.MapPath(@"~\");
                    var samples = root + "Images\\SamplePhotos\\" + ((Sex) profile.Gender).ToString() + "\\";
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