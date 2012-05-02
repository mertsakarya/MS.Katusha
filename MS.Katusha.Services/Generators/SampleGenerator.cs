using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services.Generators
{
    public class SampleGenerator
    {
        private readonly IProfileService _profileService;
        private readonly IUserService _userService;
        private readonly IPhotosService _photosService;
        private readonly IConversationService _conversationService;
        private readonly IVisitService _visitService;
        private readonly IResourceService _resourceService;

        public SampleGenerator(IProfileService profileService, IUserService userService, IPhotosService photosService, IConversationService conversationService, IVisitService visitService, IResourceService resourceService)
        {
            _profileService = profileService;
            _userService = userService;
            _photosService = photosService;
            _conversationService = conversationService;
            _visitService = visitService;
            _resourceService = resourceService;
        }

        public void CreateSamples(int count, int extra = 0)
        {
            IGenerator<Profile> generator = new ProfileGenerator(_profileService, _userService, _photosService, _resourceService);
            for (var i = 0; i < count; i++) {
                generator.Generate(extra);
            }
        }

        public void CreateConversations(int count, int extra = 0)
        {
            IGenerator<MS.Katusha.Domain.Raven.Entities.Conversation> generator = new ConversationGenerator(_profileService, _userService, _conversationService);
            for (var i = 0; i < count; i++) {
                generator.Generate(extra);
            }
        }

        public void CreateVisit(int count, int extra = 0)
        {
            IGenerator<Visit> generator = new VisitGenerator(_profileService, _visitService);
            for (var i = 0; i < count; i++) {
                generator.Generate(extra);
            }
        }
    }
}