using System;
using System.IO;
using ImageResizer;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class DBPhotoBackupService : IPhotoBackupService
    {
        private readonly IPhotoBackupRepositoryDB _photoBackupRepository;
        private readonly IKatushaFileSystem _fileSystem;

        public DBPhotoBackupService(IKatushaFileSystem filesystem, IPhotoBackupRepositoryDB photoBackupRepository)
        {
            _fileSystem = filesystem;
            _photoBackupRepository = photoBackupRepository;
        }

        public void AddPhoto(PhotoBackup photoBackup)
        {
            var photo = _photoBackupRepository.SingleAttached(p => p.Guid == photoBackup.Guid);
            if (photo == null)
                _photoBackupRepository.Add(photoBackup);
            else {
                photo.Data = photoBackup.Data;
                _photoBackupRepository.FullUpdate(photo);
            }
        }

        public void DeleteBackupPhoto(Guid guid) {
            var photo = _photoBackupRepository.SingleAttached(p => p.Guid == guid);
            if (photo == null) return;
            _photoBackupRepository.Delete(photo);
        }

        public PhotoBackup GetPhoto(Guid guid)
        {
            return _photoBackupRepository.Single(p => p.Guid == guid);
        }

        public bool GeneratePhoto(Guid guid, PhotoType photoType)
        {
            var file = String.Format("{0}/{1}-{2}.jpg", PhotoFolders.Photos, (byte)photoType, guid);
            if (_fileSystem.FileExists(file)) return true;
            var photo = GetPhoto(guid);
            return photo != null && GeneratePhoto2(photo, photoType);
        }

        private bool GeneratePhoto2(PhotoBackup photo, PhotoType photoType)
        {
            foreach (var suffix in PhotoTypes.Versions.Keys) {
                if ((byte)photoType != suffix) continue;
                byte[] bytes;
                using (var outputStream = new MemoryStream()) {
                    using (var stream = new MemoryStream(photo.Data)) {
                        ImageBuilder.Current.Build(stream, outputStream, new ResizeSettings(PhotoTypes.Versions[suffix]), false, true);
                        bytes = outputStream.ToArray();
                    }
                }
                using (var stream = new MemoryStream(bytes)) {
                    _fileSystem.Add(String.Format("{0}/{1}-{2}.jpg", PhotoFolders.Photos, (byte)photoType, photo.Guid), stream);
                }
            }
            return true;
        }

        public byte[] GetPhotoData(Guid guid)
        {
            var photo = GetPhoto(guid);
            return photo != null ? photo.Data : null;
        }

    }
}
