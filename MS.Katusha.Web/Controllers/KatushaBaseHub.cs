using System;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Web.Models;
using MS.Katusha.Web.Models.Entities;
using SignalR.Hubs;
using Profile = MS.Katusha.Domain.Entities.Profile;

namespace MS.Katusha.Web.Controllers
{
    public class KatushaBaseHub : Hub, IKatushaBase, IDisconnect, IConnected
    {
        private IResourceService _resourceService;
        private IUserService _userService;
        private IProfileService _profileService;
        private IStateService _stateService;
        private IConversationService _conversationService;
        private const int ProfileCount = 8;

        protected KatushaBaseHub()
        {
            ResourceService = DependencyResolver.Current.GetService<IResourceService>();
            UserService = DependencyResolver.Current.GetService<IUserService>();
            ProfileService = DependencyResolver.Current.GetService<IProfileService>();
            StateService = DependencyResolver.Current.GetService<IStateService>();
            ConversationService = DependencyResolver.Current.GetService<IConversationService>();
            var user = HttpContext.Current.User;
            KatushaUser = (user.Identity.IsAuthenticated) ? UserService.GetUser(user.Identity.Name) : null;
            if (KatushaUser != null) {
                KatushaProfile = (KatushaUser.Gender > 0) ? UserService.GetProfile(KatushaUser.Guid) : null;
            }
        }


        public Task Disconnect() { return Clients.All.leave(Context.ConnectionId, DateTime.Now.ToString(CultureInfo.InvariantCulture)); }

        public Task Connect() { return Clients.All.joined(Context.ConnectionId, DateTime.Now.ToString(CultureInfo.InvariantCulture)); }

        public Task Reconnect(IEnumerable<string> groups) { return Clients.All.rejoined(Context.ConnectionId, DateTime.Now.ToString(CultureInfo.InvariantCulture)); }


        protected bool IsKeyForProfile(string key)
        {
            if (KatushaUser == null) return false;
            Guid guid;
            if (Guid.TryParse(key, out guid))
                return KatushaUser.Guid == guid;
            if (KatushaProfile == null) return false;
            return (KatushaProfile.FriendlyName == key);
        }

        public User KatushaUser { get; set; }
        public Profile KatushaProfile { get; set; }
        public IUserService UserService { get; private set; }
        public IProfileService ProfileService { get; private set; }
        public IStateService StateService { get; private set; }
        public IResourceService ResourceService { get; set; }
        public IConversationService ConversationService { get; set; }
    }
}
