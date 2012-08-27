using System;
using System.Collections.Generic;
using System.IO;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Domain.Service;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IKatushaFileSystem
    {
        void Add(string path, Stream stream);
        bool FileExists(string path);
        void DeletePhoto(Guid photoGuid);
        void DeleteBackupPhoto(Guid guid);
        IList<PhotoFile> GetPhotoNames(out IList<string> unparseableFiles, string prefix = "");
        string GetPhotoUrl(Guid photoGuid, PhotoType photoType, bool encode = false);
        void WritePhoto(Photo photo, PhotoType photoType, byte[] bytes);
        string GetPhotoBaseUrl();
        byte[] GetData(string path);

        void ClearPhotos(bool clearBackups = false);
    }
}