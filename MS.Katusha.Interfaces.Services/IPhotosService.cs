using System;
using System.Web;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface IPhotosService
    {
        void MakeProfilePhoto(long profileId, Guid photoGuid);
        void DeletePhoto(long profileId, Guid guid, string pathToPhotos);
        ViewDataUploadFilesResult AddPhoto(long profileId, string description, string pathToPhotos, HttpPostedFileBase hpf);
        Photo AddSamplePhoto(long profileId, string description, string pathToPhotos, string fileName, string filePath);
    }
}