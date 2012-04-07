using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ImageResizer;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class PhotosService : IPhotosService
    {
        private readonly IProfileService _profileService;
        private readonly IProfileRepositoryDB _profileRepository;
        private readonly IPhotoRepositoryDB _photoRepository;
        private readonly IProfileRepositoryRavenDB _profileRepositoryRaven;

        private readonly IKatushaCacheContext _katushaCache
;

        public PhotosService(IProfileService profileService, IProfileRepositoryDB profileRepository, IPhotoRepositoryDB photoRepository,
            IProfileRepositoryRavenDB profileRepositoryRaven,
            IKatushaCacheContext cacheContext)
        {
            _profileService = profileService;
            _profileRepository = profileRepository;
            _photoRepository = photoRepository;
            _profileRepositoryRaven = profileRepositoryRaven;
            _katushaCache = cacheContext; 
        }

        public void MakeProfilePhoto(long profileId, Guid photoGuid)
        {
            var profile = _profileRepository.GetById(profileId, p => p.Photos);
            if (profile == null) return;
            if (!profile.Photos.Any(photo => photo.Guid == photoGuid))
                throw new HttpException(404, "Photo not found!");
            profile.ProfilePhotoGuid = photoGuid;
            _profileRepository.FullUpdate(profile);
            _katushaCache.Delete("P:" + profile.Guid.ToString());
            _profileRepositoryRaven.FullUpdate(_profileService.GetProfile(profile.Id, null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User, p => p.State, p => p.Photos));
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
            _profileRepositoryRaven.FullUpdate(_profileService.GetProfile(profile.Id, null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User, p => p.State, p => p.Photos));

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
            _profileRepositoryRaven.FullUpdate(_profileService.GetProfile(profile.Id, null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User, p => p.State, p => p.Photos));
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
            _profileRepositoryRaven.FullUpdate(_profileService.GetProfile(profile.Id, null, p => p.CountriesToVisit, p => p.LanguagesSpoken, p => p.Searches, p => p.User, p => p.State, p => p.Photos));
        }
    }
}
