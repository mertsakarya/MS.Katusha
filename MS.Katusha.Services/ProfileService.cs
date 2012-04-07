using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using ImageResizer;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Exceptions.Services;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepositoryDB _profileRepository;
        private readonly IUserRepositoryDB _userRepository;
        private readonly ICountriesToVisitRepositoryDB _countriesToVisitRepository;
        private readonly IPhotoRepositoryDB _photoRepository;
        private readonly ILanguagesSpokenRepositoryDB _languagesSpokenRepository;
        private readonly ISearchingForRepositoryDB _searchingForRepository;
        private readonly IConversationRepositoryDB _conversationRepository;
        private readonly IVisitRepositoryDB _visitRepository;
        private readonly IProfileRepositoryRavenDB _profileRepositoryRaven;
        private readonly IVisitRepositoryRavenDB _visitRepositoryRaven;
        private readonly IConversationRepositoryRavenDB _conversationRepositoryRaven;

        private readonly IKatushaCacheContext _katushaCache
;

        public ProfileService(IProfileRepositoryDB profileRepository, IUserRepositoryDB userRepository, 
            ICountriesToVisitRepositoryDB countriesToVisitRepository, IPhotoRepositoryDB photoRepository,
            ILanguagesSpokenRepositoryDB languagesSpokenRepository, ISearchingForRepositoryDB searchingForRepository,
            IConversationRepositoryDB conversationRepository, IVisitRepositoryDB visitRepository,
            IProfileRepositoryRavenDB profileRepositoryRaven, IVisitRepositoryRavenDB visitRepositoryRaven, 
            IConversationRepositoryRavenDB conversationRepositoryRaven,
            IKatushaCacheContext cacheContext)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _countriesToVisitRepository = countriesToVisitRepository;
            _photoRepository = photoRepository;
            _languagesSpokenRepository = languagesSpokenRepository;
            _searchingForRepository = searchingForRepository;
            _conversationRepository = conversationRepository;
            _visitRepository = visitRepository;
            _profileRepositoryRaven = profileRepositoryRaven;
            _visitRepositoryRaven = visitRepositoryRaven;
            _conversationRepositoryRaven = conversationRepositoryRaven;
            _katushaCache = cacheContext; 
        }


        public IEnumerable<Profile> GetNewProfiles(Expression<Func<Profile, bool>> filter, out int total, int pageNo = 1, int pageSize = 20)
        {
            var items = _profileRepository.Query(filter, pageNo, pageSize, out total, o => o.Id, p => p.Photos);
            return items;
        }

        public long GetProfileId(string friendlyName)
        {
            return _profileRepository.GetProfileIdByFriendlyName(friendlyName);
        }

        public long GetProfileId(Guid guid)
        {
            return _profileRepository.GetProfileIdByGuid(guid);
        }

        public virtual Profile GetProfile(long profileId, Profile visitorProfile = null, params Expression<Func<Profile, object>>[] includeExpressionParams)
        {
            if (includeExpressionParams == null || includeExpressionParams.Length == 0) {
                includeExpressionParams = new Expression<Func<Profile, object>>[] {
                    p => p.CountriesToVisit, p => p.User, p => p.State,
                    p => p.LanguagesSpoken, p => p.LanguagesSpoken, p => p.Searches,
                    p => p.Photos
                };
            }
            var profile = _profileRepository.GetById(profileId, includeExpressionParams);
            Visit(visitorProfile, profile); 
            return profile;
        }

        private void Visit(Profile visitorProfile, Profile profile)
        {
            if (visitorProfile != null && profile.Id != visitorProfile.Id) {
                var visit = _visitRepository.SingleAttached(p => p.ProfileId == profile.Id);

                if (visit == null) {
                    _visitRepository.Add(new Visit { ProfileId = profile.Id, VisitorProfileId = visitorProfile.Id, VisitCount = 1 });
                } else {
                    visit.VisitCount++;
                    _visitRepository.FullUpdate(visit);
                }
                //TODO: Modify this with PATCH method of Raven Repository. http://ravendb.net/docs/client-api/partial-document-updates
                //_visitRepositoryRaven.Add(new Visit { ProfileId = profile.Id, VisitorProfileId = visitorProfile.Id, VisitCount = visit.VisitCount });
                var visitRaven = _visitRepositoryRaven.SingleAttached(p => p.ProfileId == profile.Id);
                if (visitRaven == null) {
                    _visitRepositoryRaven.Add(new Visit { ProfileId = profile.Id, VisitorProfileId = visitorProfile.Id, VisitCount = 1 });
                } else {
                    visitRaven.VisitCount++;
                    _visitRepositoryRaven.FullUpdate(visitRaven);
                }
            }
        }

        public virtual void CreateProfile(Profile profile)
        {
            if (!String.IsNullOrEmpty(profile.FriendlyName))
                if (_profileRepository.CheckIfFriendlyNameExists(profile.FriendlyName, profile.Id))
                    throw new KatushaFriendlyNameExistsException(profile);
            if (!(profile.Gender == (byte)Sex.Male || profile.Gender == (byte)Sex.Female))
                throw new KatushaGenderNotExistsException(profile);
            _profileRepository.Add(profile);
            _profileRepository.Save();
            
            var user = _userRepository.SingleAttached(p => p.Id == profile.UserId);
            user.Gender = profile.Gender;
            _userRepository.FullUpdate(user);
            _katushaCache.Delete("P:" + profile.Guid.ToString());
            _katushaCache.Delete("U:" + user.UserName);
            _profileRepositoryRaven.Add(GetProfile(profile.Id, null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User, p => p.State, p=> p.Photos));
        }

        public void DeleteProfile(long profileId, bool force = false)
        {
            var profile = _profileRepository.GetById(profileId);
            if (profile != null) {
                ExecuteDeleteProfile(profile, force);
            }
        }

        private void ExecuteDeleteProfile(Profile profile, bool softDelete)
        {
            if (!softDelete) {
                _profileRepository.Delete(profile);
            } else {
                _profileRepository.SoftDelete(profile);
            }
            var user = _userRepository.SingleAttached(p=> p.Guid == profile.Guid);
            user.Gender = 0;
            _userRepository.FullUpdate(user);
            _katushaCache.Delete("P:"+profile.Guid.ToString());
            _profileRepositoryRaven.Delete(profile);
        }

        public virtual void UpdateProfile(Profile profile)
        {
            if(profile == null)
                throw new ArgumentNullException("profile");
            if (!String.IsNullOrEmpty(profile.FriendlyName))
                if (_profileRepository.CheckIfFriendlyNameExists(profile.FriendlyName, profile.Id))
                    throw new KatushaFriendlyNameExistsException(profile);
            if (!(profile.Gender == (byte)Sex.Male || profile.Gender == (byte)Sex.Female))
                throw new KatushaGenderNotExistsException(profile);
            var dataProfile = GetProfile(profile.Id);
            dataProfile.FriendlyName = profile.FriendlyName;
            dataProfile.Name = profile.Name;
            dataProfile.Alcohol = profile.Alcohol;
            dataProfile.BirthYear = profile.BirthYear;
            dataProfile.BodyBuild = profile.BodyBuild;
            dataProfile.City = profile.City;
            dataProfile.Description = profile.Description;
            dataProfile.EyeColor = profile.EyeColor;
            dataProfile.From = profile.From;
            dataProfile.HairColor = profile.HairColor;
            dataProfile.Height = profile.Height;
            dataProfile.Religion = profile.Religion;
            dataProfile.Smokes = profile.Smokes;
            dataProfile.ProfilePhotoGuid = profile.ProfilePhotoGuid;
            dataProfile.Gender = profile.Gender;
            dataProfile.DickSize = profile.DickSize;
            dataProfile.DickThickness = profile.DickThickness;
            dataProfile.BreastSize = profile.BreastSize; 

            _profileRepository.FullUpdate(dataProfile);
            _katushaCache.Delete("P:" + dataProfile.Guid.ToString());
            _profileRepositoryRaven.FullUpdate(GetProfile(profile.Id, null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User, p => p.State, p => p.Photos));
        }

        public void DeleteCountriesToVisit(long profileId, Country country) { _countriesToVisitRepository.DeleteByProfileId(profileId, country); }

        public void AddCountriesToVisit(long profileId, Country country) { _countriesToVisitRepository.AddByProfileId(profileId, country); }

        public void DeleteLanguagesSpoken(long profileId, Language language) { _languagesSpokenRepository.DeleteByProfileId(profileId, language); }

        public void AddLanguagesSpoken(long profileId, Language language) { _languagesSpokenRepository.AddByProfileId(profileId, language); }

        public void DeleteSearches(long profileId, LookingFor lookingFor) { _searchingForRepository.DeleteByProfileId(profileId, lookingFor); }

        public void AddSearches(long profileId, LookingFor lookingFor) { _searchingForRepository.AddByProfileId(profileId, lookingFor); }

        public void MakeProfilePhoto(long profileId, Guid photoGuid)
        {
            var profile = _profileRepository.GetById(profileId, p => p.Photos);
            if (profile == null) return;
            if (!profile.Photos.Any(photo => photo.Guid == photoGuid))
                throw new HttpException(404, "Photo not found!");
            profile.ProfilePhotoGuid = photoGuid;
            _profileRepository.FullUpdate(profile);
            _katushaCache.Delete("P:" + profile.Guid.ToString());
            _profileRepositoryRaven.FullUpdate(GetProfile(profile.Id, null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User, p => p.State, p => p.Photos));
        }

        public ViewDataUploadFilesResult AddPhoto(long profileId, string description, string pathToPhotos, HttpPostedFileBase hpf)
        {
            if (hpf.ContentLength <= 0) return null;

            var profile = _profileRepository.SingleAttached(p=> p.Id == profileId);
            if (profile == null) return null;
            var guid = Guid.NewGuid();
            var photo = new Photo { Description = description, ProfileId = profileId, FileName = hpf.FileName, ContentType = "image/png", Guid = guid };

            if (profile.ProfilePhotoGuid == Guid.Empty) { //set first photo as default photo
                profile.ProfilePhotoGuid = photo.Guid;
                _profileRepository.FullUpdate(profile);
            }
            _photoRepository.Add(photo, photo.Guid);

            var versions = new Dictionary<byte, string> {
                {(byte)PhotoType.Thumbnail, "width=80&height=106&crop=auto&format=png"}, 
                {(byte)PhotoType.Medium, "maxwidth=400&maxheight=530&format=png"},
                {(byte)PhotoType.Large, "maxwidth=800&maxheight=1060&format=png"},
                {(byte)PhotoType.Original, "format=png"}
            };

            if (!Directory.Exists(pathToPhotos)) Directory.CreateDirectory(pathToPhotos);

            foreach (var suffix in versions.Keys) {
                var fileName = Path.Combine(pathToPhotos, suffix.ToString() + "-" + guid.ToString());
                ImageBuilder.Current.Build(hpf, fileName, new ResizeSettings(versions[suffix]), false, true);
            }

            _katushaCache.Delete("P:" + profile.Guid.ToString());
            _profileRepositoryRaven.FullUpdate(GetProfile(profile.Id, null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User, p => p.State, p => p.Photos));

            var id = (String.IsNullOrEmpty(profile.FriendlyName)) ? profile.Guid.ToString() : profile.FriendlyName;
            return new ViewDataUploadFilesResult {
                name = hpf.FileName,
                size = hpf.ContentLength,
                type = hpf.ContentType,
                url = String.Format("/Photos/{1}-{0}.png", guid, (byte)PhotoType.Large),
                delete_url = String.Format("/Profiles/DeletePhoto/{0}/{1}", id, guid),
                delete_type = "GET",
                thumbnail_url = String.Format("/Photos/{1}-{0}.png", guid, (byte)PhotoType.Thumbnail) //@"data:image/png;base64," + EncodeBytes(smallFileContents)
            };
        }

        public void AddSamplePhoto(long profileId, string description, string pathToPhotos, string fileName, string filePath)
        {
            ///// INTERNAL USE /////
            var profile = _profileRepository.SingleAttached(p=> p.Id == profileId);
            if (profile == null) return;
            var guid = Guid.NewGuid();
            var photo = new Photo { Description = description, ProfileId = profileId, FileName = fileName, ContentType = "image/png", Guid = guid };

            if (profile.ProfilePhotoGuid == Guid.Empty) { //set first photo as default photo
                profile.ProfilePhotoGuid = photo.Guid;
                _profileRepository.FullUpdate(profile);
            }
            _photoRepository.Add(photo, photo.Guid);

            var versions = new Dictionary<byte, string> {
                {(byte)PhotoType.Thumbnail, "width=80&height=106&crop=auto&format=png"}, 
                {(byte)PhotoType.Medium, "maxwidth=400&maxheight=530&format=png"},
                {(byte)PhotoType.Large, "maxwidth=800&maxheight=1060&format=png"},
                {(byte)PhotoType.Original, "format=png"}
            };

            if (!Directory.Exists(pathToPhotos)) Directory.CreateDirectory(pathToPhotos);

            foreach (var suffix in versions.Keys) {
                var fn = Path.Combine(pathToPhotos, suffix.ToString() + "-" + guid.ToString());
                ImageBuilder.Current.Build(filePath, fn, new ResizeSettings(versions[suffix]), false, true);
            }

            _katushaCache.Delete("P:" + profile.Guid.ToString());
            _profileRepositoryRaven.FullUpdate(GetProfile(profile.Id, null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User, p => p.State, p => p.Photos));
        }

        public void DeletePhoto(long profileId, Guid photoGuid, string pathToPhotos)
        {
            var profile = _profileRepository.GetById(profileId, p => p.Photos);
            if (profile == null) return;
            if (!profile.Photos.Any(photo => photo.Guid == photoGuid))
                throw new HttpException(404, "Photo not found!");
            var entity = _photoRepository.GetByGuid(photoGuid);
            if (entity != null) {
                _photoRepository.Delete(entity);
                for (byte i = 0; i < (byte)PhotoType.MaxPhotoType; i++)
                    File.Delete(String.Format("{0}{1}-{2}.png", pathToPhotos, i, photoGuid));
            }
            if (profile.ProfilePhotoGuid == photoGuid) {
                profile.ProfilePhotoGuid = Guid.Empty;
                _profileRepository.FullUpdate(profile);
            }
            _katushaCache.Delete("P:" + profile.Guid.ToString());
            _profileRepositoryRaven.FullUpdate(GetProfile(profile.Id, null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User, p => p.State, p => p.Photos));
        }

        public IEnumerable<Conversation> GetMessages(long profileId, out int total, int pageNo = 1, int pageSize = 20)
        {
            return _conversationRepository.Query(q => q.FromId == profileId || q.ToId == profileId, pageNo, pageSize, out total, o => o.CreationDate, p => p.From, p => p.To).ToList();
        }

        public void SendMessage(Conversation message)
        {
            _conversationRepository.Add(message);
            _conversationRepositoryRaven.Add(message);
        }

        public void ReadMessage(long profileId, Guid messageGuid)
        {
            var message = _conversationRepository.SingleAttached(p => p.Guid == messageGuid && p.ToId == profileId);
            if (message == null) return;
            message.ReadDate = DateTime.UtcNow;
            _conversationRepository.FullUpdate(message);
            var messageRaven = _conversationRepository.SingleAttached(p => p.Guid == messageGuid && p.ToId == profileId);
            if (messageRaven == null) return;
            messageRaven.ReadDate = DateTime.UtcNow;
            _conversationRepositoryRaven.FullUpdate(messageRaven);
        }

        public IEnumerable<Visit> GetVisitors(long profileId, out int total, int pageNo = 1, int pageSize = 20)
        {
            var items = _visitRepositoryRaven.Query(p => p.ProfileId == profileId, pageNo, pageSize, out total, o => o.Id, p => p.VisitorProfile);
            //var items = _visitRepository.Query(p => p.ProfileId == profileId, pageNo, pageSize, out total, o => o.Id, p => p.VisitorProfile);
            return items;
        }

    }
}
