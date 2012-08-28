using System;
using System.IO;
using ImageResizer;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Service;
using MS.Katusha.Enumerations;
using MS.Katusha.FileSystems;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class S3PhotoBackupService : IPhotoBackupService
    {
        private readonly S3FileSystem _fileSystem ;

        public S3PhotoBackupService(string bucketName="")
        {
            _fileSystem = new S3FileSystem(bucketName);
        }

        public void AddPhoto(PhotoBackup photoBackup) {
            using (var stream = new MemoryStream(photoBackup.Data)) {
                _fileSystem.Add(String.Format("{0}/{1}.jpg", PhotoFolders.PhotoBackups, photoBackup.Guid), stream);
            } 
        }

        public void DeleteBackupPhoto(Guid guid) { _fileSystem.DeleteBackupPhoto(guid); }

        public PhotoBackup GetPhoto(Guid guid) {
            return new PhotoBackup { Guid = guid, Data = GetPhotoData(guid) };
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

        public bool GeneratePhoto(Guid guid, PhotoType photoType)
        {
            var file = String.Format("{0}/{1}-{2}.jpg", PhotoFolders.Photos, (byte)photoType, guid);
            if (_fileSystem.FileExists(file)) return true;
            var photo = GetPhoto(guid);
            return photo != null && GeneratePhoto2(photo, photoType);
        }

        public byte[] GetPhotoData(Guid guid)
        {
            var file = String.Format("{0}/{1}.jpg", PhotoFolders.PhotoBackups, guid);
            return _fileSystem.GetData(file);
        }
    }
}
