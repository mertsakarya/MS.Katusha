using System;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IPhotoBackupService
    {
        void AddPhoto(PhotoBackup photoBackup);
        void DeleteBackupPhoto(Guid guid);
        bool GeneratePhoto(Guid guid, PhotoType type);
        byte[] GetPhotoData(Guid guid);
        PhotoBackup GetPhoto(Guid guid);
    }
}