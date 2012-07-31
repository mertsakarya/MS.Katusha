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
    public class PhotoBackupService : IPhotoBackupService
    {
        private readonly IPhotoBackupRepositoryDB _photoBackupRepository;

        private readonly IDictionary<byte, string> _versions = new Dictionary<byte, string> {
                {(byte)PhotoType.Thumbnail, "width=80&height=106&crop=auto&format=jpg&quality=90"}, 
                {(byte)PhotoType.Medium, "maxwidth=400&maxheight=530&format=jpg&quality=90"},
                {(byte)PhotoType.Large, "maxwidth=800&maxheight=1060&format=jpg&quality=90"},
                {(byte)PhotoType.Icon, "width=40&height=53&format=jpg&quality=90"},
                {(byte)PhotoType.Original, "format=jpg"}
            };

        public PhotoBackupService(IPhotoBackupRepositoryDB photoBackupRepository) {
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

        public void DeletePhoto(Guid guid) {
            var photo = _photoBackupRepository.SingleAttached(p => p.Guid == guid);
            if(photo != null)
                _photoBackupRepository.Delete(photo);
        }

        public PhotoBackup GetPhoto(Guid guid)
        {
            return _photoBackupRepository.Single(p => p.Guid == guid);
        }

        public void GeneratePhoto(Guid guid, string path, PhotoType photoType)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var photo = GetPhoto(guid);
            if (photo != null) {
                foreach (var suffix in _versions.Keys) {
                    if ((byte) photoType == suffix) {
                        var fileName = Path.Combine(path, suffix.ToString(CultureInfo.InvariantCulture) + "-" + guid.ToString());
                        using (var stream = new MemoryStream(photo.Data)) {
                            ImageBuilder.Current.Build(stream, fileName, new ResizeSettings(_versions[suffix]), false, true);
                        }
                    }
                }
            }
        }

        public void GeneratePhoto(Guid guid, string path)
        {
            foreach (var suffix in _versions.Keys) {
                GeneratePhoto(guid, path, (PhotoType) suffix);
            }
        }

        public byte[] GetPhotoData(Guid guid)
        {
            var photo = GetPhoto(guid);
            return photo != null ? photo.Data : null;
        }

    }
}
