using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services.Generators
{
    public class SampleGenerator
    {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;
        private readonly IPhotosService _photosService;

        public SampleGenerator(IProfileService profileService, IUserService userService, IPhotosService photosService)
        {
            _profileService = profileService;
            _userService = userService;
            _photosService = photosService;
        }
        public void CreateSamples(int count)
        {
            IGenerator<Profile> generator = new ProfileGenerator(_profileService, _userService, _photosService);
            for(var i =0 ; i < count; i++) {
                generator.Generate();
            }
        }
    }
}