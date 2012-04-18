using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using ImageResizer;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class PhotosService : IPhotosService
    {
        private readonly IProfileService _profileService;
        private readonly IProfileRepositoryDB _profileRepository;
        private readonly IPhotoRepositoryDB _photoRepository;

        private readonly IDictionary<byte, string> _versions = new Dictionary<byte, string> {
                {(byte)PhotoType.Thumbnail, "width=80&height=106&crop=auto&format=jpg&quality=90"}, 
                {(byte)PhotoType.Medium, "maxwidth=400&maxheight=530&format=jpg&quality=90"},
                {(byte)PhotoType.Large, "maxwidth=800&maxheight=1060&format=jpg&quality=90"},
                {(byte)PhotoType.Icon, "width=40&height=53&format=jpg&quality=90"},
                {(byte)PhotoType.Original, "format=jpg"}
            };

        public PhotosService(IProfileService profileService, IProfileRepositoryDB profileRepository, IPhotoRepositoryDB photoRepository)
        {
            _profileService = profileService;
            _profileRepository = profileRepository;
            _photoRepository = photoRepository;
        }

        public void MakeProfilePhoto(long profileId, Guid photoGuid)
        {
            var profile = _profileRepository.SingleAttached(p => p.Id == profileId, p=> p.Photos);
            if (profile == null) return;
            if (!profile.Photos.Any(photo => photo.Guid == photoGuid))
                throw new HttpException(404, "Photo not found!");
            profile.ProfilePhotoGuid = photoGuid;
            _profileRepository.FullUpdate(profile);
            _profileService.UpdateRavenProfile(profile.Id);
        }

        public ViewDataUploadFilesResult AddPhoto(long profileId, string description, string pathToPhotos, HttpPostedFileBase hpf)
        {
            if (hpf.ContentLength <= 0) return null;

            var profile = _profileRepository.SingleAttached(p=> p.Id == profileId);
            if (profile == null) return null;
            var guid = Guid.NewGuid();
            var photo = new Photo { Description = description, ProfileId = profileId, FileName = hpf.FileName, ContentType = "image/jpeg", Guid = guid };

            if (profile.ProfilePhotoGuid == Guid.Empty) { //set first photo as default photo
                profile.ProfilePhotoGuid = photo.Guid;
                _profileRepository.FullUpdate(profile);
            }
            _photoRepository.Add(photo, photo.Guid);


            if (!Directory.Exists(pathToPhotos)) Directory.CreateDirectory(pathToPhotos);

            foreach (var suffix in _versions.Keys) {
                var fileName = Path.Combine(pathToPhotos, suffix.ToString(CultureInfo.InvariantCulture) + "-" + guid.ToString());
                ImageBuilder.Current.Build(hpf, fileName, new ResizeSettings(_versions[suffix]), false, true);
            }

            _profileService.UpdateRavenProfile(profile.Id);

            var id = (String.IsNullOrEmpty(profile.FriendlyName)) ? profile.Guid.ToString() : profile.FriendlyName;
            return new ViewDataUploadFilesResult {
                name = hpf.FileName,
                size = hpf.ContentLength,
                type = hpf.ContentType,
                url = String.Format("/Photos/{1}-{0}.jpg", guid, (byte)PhotoType.Large),
                delete_url = String.Format("/Profiles/DeletePhoto/{0}/{1}", id, guid),
                delete_type = "GET",
                thumbnail_url = String.Format("/Photos/{1}-{0}.jpg", guid, (byte)PhotoType.Thumbnail) //@"data:image/jpg;base64," + EncodeBytes(smallFileContents)
            };
        }

        public Photo AddSamplePhoto(long profileId, string description, string pathToPhotos, string fileName, string filePath)
        {
            ///// INTERNAL USE /////
            var profile = _profileRepository.SingleAttached(p=> p.Id == profileId);
            if (profile == null) return null;
            var guid = Guid.NewGuid();
            var photo = new Photo { Description = description, ProfileId = profileId, FileName = fileName, ContentType = "image/jpg", Guid = guid };

            if (profile.ProfilePhotoGuid == Guid.Empty) { //set first photo as default photo
                profile.ProfilePhotoGuid = photo.Guid;
                _profileRepository.FullUpdate(profile);
            }
            _photoRepository.Add(photo, photo.Guid);

            if (!Directory.Exists(pathToPhotos)) Directory.CreateDirectory(pathToPhotos);

            foreach (var suffix in _versions.Keys) {
                var fn = Path.Combine(pathToPhotos, suffix.ToString(CultureInfo.InvariantCulture) + "-" + guid.ToString());
                ImageBuilder.Current.Build(filePath, fn, new ResizeSettings(_versions[suffix]), false, true);
            }

            _profileService.UpdateRavenProfile(profile.Id);
            return _photoRepository.SingleAttached(p => p.Id == photo.Id);
        }

        public IList<Guid> Dir(string pathToPhotos, out int total, int pageNo = 1, int pageSize = 20)
        {
            var files = Directory.GetFiles(pathToPhotos, ((byte)PhotoType.Original).ToString(CultureInfo.InvariantCulture) + "-*.jpg");
            total = files.Length;
            var fileList = files.Skip((pageNo - 1)*pageSize).Take(pageSize);
            var list = new List<Guid>(fileList.Count());
            var start = 3 + pathToPhotos.Length;
            foreach(var fileName in fileList) {
                Guid guid;
                if (Guid.TryParse(fileName.Substring(start, 36), out guid)) {
                    list.Add(guid);
                }
            }
            return list;
        }

        public Photo GetByGuid(Guid guid) { return _photoRepository.GetByGuid(guid); }

        public void DeletePhoto(long profileId, Guid photoGuid, string pathToPhotos)
        {
            var profile = _profileRepository.GetById(profileId, p => p.Photos);
            if (profile == null) return;
            if (!profile.Photos.Any(photo => photo.Guid == photoGuid))
                throw new HttpException(404, "Photo not found!");
            var entity = _photoRepository.GetByGuid(photoGuid);
            if (entity != null) {
                _photoRepository.Delete(entity);
                for (byte i = 0; i < (byte)PhotoType.MAX; i++)
                    File.Delete(String.Format("{0}{1}-{2}.jpg", pathToPhotos, i, photoGuid));
            }
            if (profile.ProfilePhotoGuid == photoGuid) {
                profile.ProfilePhotoGuid = Guid.Empty;
                _profileRepository.FullUpdate(profile);
            }
            _profileService.UpdateRavenProfile(profile.Id);
        }
    }
}
