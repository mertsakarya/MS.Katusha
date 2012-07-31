using System;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IPhotoBackupService
    {
        void AddPhoto(PhotoBackup photoBackup);
        void DeletePhoto(Guid guid);
        PhotoBackup GetPhoto(Guid guid);
        void GeneratePhoto(Guid guid, string path);
        void GeneratePhoto(Guid guid, string path, PhotoType type);
        byte[] GetPhotoData(Guid guid);
    }
}