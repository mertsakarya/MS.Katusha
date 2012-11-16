using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using ImageResizer;
using ImageResizer.Plugins.Basic;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Service;
using MS.Katusha.Enumerations;
using MS.Katusha.FileSystems;
using MS.Katusha.Infrastructure.Cache;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class PhotoManager
    {
        public void AddPhoto(IKatushaFileSystem fileSystem, IPhotoBackupService photoBackupService, Photo photo, HttpPostedFileBase hpf, byte[] thumbnailBytes)
        {
            foreach (var suffix in PhotoTypes.Versions.Keys) {
                var bytes = (suffix == (byte) PhotoType.Thumbnail) ? thumbnailBytes : BuildImage(hpf, suffix);
                fileSystem.WritePhoto(photo, (PhotoType)suffix, bytes);
                if (suffix == (byte)PhotoType.Original)
                    photoBackupService.AddPhoto(new PhotoBackup { Guid = photo.Guid, Data = bytes });
            }
        }

        public static byte[] BuildImage(HttpPostedFileBase hpf, byte suffix)
        {
            byte[] bytes;
            using (var stream = new MemoryStream()) {
                try {
                    ImageBuilder.Current.Build(hpf, stream, new ResizeSettings(PhotoTypes.Versions[suffix].ToString()), false, true);
                } catch(SizeLimits.SizeLimitException) {
                    //ex
                }
                bytes = stream.ToArray();
            }
            return bytes;
        }
    }

    public delegate void AsyncAddPhotoCaller(IKatushaFileSystem fileSystem, IPhotoBackupService photoBackupService, Photo photo, HttpPostedFileBase hpf, byte[] thumbnailBytes);


    public class PhotosService : IPhotosService
    {
        private readonly IKatushaFileSystem _fileSystem;
        private readonly IProfileService _profileService;
        private readonly INotificationService _notificationService;
        private readonly IConversationService _conversationService;
        private readonly IProfileRepositoryDB _profileRepository;
        private readonly IPhotoRepositoryDB _photoRepository;
        private readonly IPhotoBackupService _photoBackupService;
        private readonly IKatushaGlobalCacheContext _cacheContext;

        public PhotosService(IKatushaGlobalCacheContext cacheContext, IKatushaFileSystem fileSystem, IProfileService profileService, INotificationService notificationService, IConversationService conversationService, IProfileRepositoryDB profileRepository, IPhotoRepositoryDB photoRepository, IPhotoBackupService photoBackupService)
        {
            _cacheContext = cacheContext;
            _fileSystem = fileSystem;
            _profileService = profileService;
            _notificationService = notificationService;
            _conversationService = conversationService;
            _profileRepository = profileRepository;
            _photoRepository = photoRepository;
            _photoBackupService = photoBackupService;
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

        public ViewDataUploadFilesResult AddPhoto(long profileId, string description, HttpPostedFileBase hpf)
        {
            if (hpf.ContentLength <= 0) return null;
            var profile = _profileRepository.SingleAttached(p => p.Id == profileId);
            if (profile == null) return null;
            var guid = Guid.NewGuid();
            var photo = new Photo { Description = description, ProfileId = profileId, Status = (byte)PhotoStatus.Uploading, FileName = hpf.FileName, ContentType = "image/jpeg", Guid = guid };
            _photoRepository.Add(photo, photo.Guid);
            var bytes = PhotoManager.BuildImage(hpf, (byte)PhotoType.Thumbnail);

            if (profile.ProfilePhotoGuid == Guid.Empty) { //set first photo as default photo
                profile.ProfilePhotoGuid = photo.Guid;
                _profileRepository.FullUpdate(profile);
            }
            _profileService.UpdateRavenProfile(profile.Id);
            _notificationService.PhotoAdded(photo);
            (new PhotoManager()).AddPhoto(_fileSystem, _photoBackupService, photo, hpf, bytes);
            //(new AsyncAddPhotoCaller((new PhotoManager()).AddPhoto)).BeginInvoke(_fileSystem, _photoBackupService, photo, hpf, bytes, null, null);

            var id = (String.IsNullOrEmpty(profile.FriendlyName)) ? profile.Guid.ToString() : profile.FriendlyName;
            return new ViewDataUploadFilesResult {
                name = hpf.FileName,
                size = hpf.ContentLength,
                type = hpf.ContentType,
                url = _fileSystem.GetPhotoUrl(guid, PhotoType.Large),
                delete_url = String.Format("/Profiles/DeletePhoto/{0}/{1}", id, guid),
                delete_type = "GET",
                thumbnail_url = @"data:image/jpg;base64," + Convert.ToBase64String(bytes) //fileSystem.GetPhotoUrl(guid, PhotoType.Thumbnail)
            };
        }

        public Photo AddSamplePhoto(long profileId, string description, string fileName, string filePath)
        {
            /////// INTERNAL USE /////
            //var profile = _profileRepository.SingleAttached(p => p.Id == profileId);
            //if (profile == null) return null;
            //var guid = Guid.NewGuid();
            //var photo = new Photo { Description = description, ProfileId = profileId, FileName = fileName, ContentType = "image/jpg", Guid = guid };

            //if (profile.ProfilePhotoGuid == Guid.Empty) { //set first photo as default photo
            //    profile.ProfilePhotoGuid = photo.Guid;
            //    _profileRepository.FullUpdate(profile);
            //}
            //_photoRepository.Add(photo, photo.Guid);

            //if (!Directory.Exists(pathToPhotos)) Directory.CreateDirectory(pathToPhotos);

            //foreach (var suffix in PhotoTypes.Versions.Keys) {
            //    var fn = Path.Combine(pathToPhotos, suffix.ToString(CultureInfo.InvariantCulture) + "-" + guid.ToString());
            //    ImageBuilder.Current.Build(filePath, fn, new ResizeSettings(PhotoTypes.Versions[suffix]), false, true);
            //    if (suffix == (byte)PhotoType.Original)
            //        _photoBackupService.AddPhoto(new PhotoBackup { Guid = guid, Data = File.ReadAllBytes(fn + ".jpg") });

            //}

            //_profileService.UpdateRavenProfile(profile.Id);
            //return _photoRepository.SingleAttached(p => p.Id == photo.Id);
            return null;
        }

        public Photo GetByGuid(Guid guid) { return _photoRepository.GetByGuid(guid); }

        public string GetPhotoUrl(Guid photoGuid, PhotoType photoType, bool encode = false)
        {
            if (_cacheContext == null || photoGuid == Guid.Empty) return _fileSystem.GetPhotoUrl(photoGuid, photoType);
            var key = "F:" + (byte) photoType + photoGuid;
            //if (photoType == PhotoType.Icon) encode = true;
            var encodingText = (encode) ? @"data:image/jpg;base64," : "";
            if (!encode) return encodingText + _fileSystem.GetPhotoUrl(photoGuid, photoType);
            var result = _cacheContext.Get<string>(key);
            if (String.IsNullOrWhiteSpace(result)) {
                result = encodingText + _fileSystem.GetPhotoUrl(photoGuid, photoType, true);
                _cacheContext.Add(key, result);
            }
            return result;
        }

        public string GetPhotoBaseUrl() { return _fileSystem.GetPhotoBaseUrl(); }
        public void ClearPhotos(bool clearBackups) { _fileSystem.ClearPhotos(clearBackups); }

        public IList<Photo> GetPhotosByTime(int pageNo, DateTime dateTime, out int total, int pageSize)
        {
            total = _photoRepository.Count(p => p.ModifiedDate > dateTime);
            return _photoRepository.Query(p => p.ModifiedDate > dateTime, null, false);
        }

        public IList<Guid> AllPhotos(out int total, string prefix = "", int pageNo = 1, int pageSize = 20)
        {
            IList<string> badFiles;
            var files = _fileSystem.GetPhotoNames(out badFiles, prefix);
            total = files.Count;
            var fileList = files.Skip((pageNo - 1)*pageSize).Take(pageSize).ToArray();
            var list = new List<Guid>(fileList.Count());
            list.AddRange(fileList.Select(file => file.Guid));
            return list;
        }

        public List<string> CheckPhotos()
        {
            var list = new List<string>();
            int total;
            var photos = _photoRepository.GetAll(out total).ToList();
            foreach(var photo in photos) {
                var profile = _profileRepository.GetById(photo.ProfileId);
                if(profile == null) {
                    list.Add("NOPROFILE\t" + photo.Guid.ToString());
                }
                foreach (var suffix in PhotoTypes.Versions.Keys) {
                    if(_photoBackupService.GeneratePhoto(photo.Guid, (PhotoType)suffix))
                        list.Add("CREATED\t" + photo.Guid);
                }
            }
            return list;
        }

        public List<string> CheckPhotoFiles()
        {
            var guids = new List<Guid>();
            var list = new List<string>();
            IList<string> badFiles;
            foreach (var file in _fileSystem.GetPhotoNames(out badFiles).Where(file => file.Guid != Guid.Empty).Where(file => !guids.Contains(file.Guid))) {
                guids.Add(file.Guid);
                list.AddRange(from suffix in PhotoTypes.Versions.Keys where _photoBackupService.GeneratePhoto(file.Guid, (PhotoType) suffix) select "CREATED\t" + file.Guid);
                var photo = _photoRepository.GetByGuid(file.Guid);
                if (photo == null) {
                    list.Add("NOPHOTO\t" + file.Guid.ToString());
                }
            }
            list.AddRange(badFiles.Select(item => "BADFILE\t" + item));
            return list;
        }

        public List<string> CheckProfilePhotos()
        {
            return (from profile in _profileRepository.Query(p => p.ProfilePhotoGuid != Guid.Empty, null, false).ToList() select _photoRepository.GetByGuid(profile.ProfilePhotoGuid) into photo from suffix in PhotoTypes.Versions.Keys where _photoBackupService.GeneratePhoto(photo.Guid, (PhotoType) suffix) select "CREATED\t" + photo.Guid).ToList();
        }

        public bool DeletePhoto(long profileId, Guid photoGuid)
        {
            var entity = _photoRepository.SingleAttached(p=>p.Guid == photoGuid);
            if (entity != null) {
                _photoRepository.Delete(entity);
                if (!_conversationService.HasPhotoGuid(photoGuid)) {
                    _fileSystem.DeletePhoto(photoGuid);
                    _photoBackupService.DeleteBackupPhoto(photoGuid);
                }
            }

            var isProfilePhoto = false;
            var profile = _profileRepository.SingleAttached(p => p.Id == profileId, p => p.Photos);
            if (profile != null) {
                if (profile.ProfilePhotoGuid == photoGuid) {
                    isProfilePhoto = true;
                    profile.ProfilePhotoGuid = Guid.Empty;
                    _profileRepository.FullUpdate(profile);
                }
                _profileService.UpdateRavenProfile(profileId);
            }
            return isProfilePhoto;
        }
    }
}
