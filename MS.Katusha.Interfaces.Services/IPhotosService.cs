using System;
using System.Collections.Generic;
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
        IList<Guid> Dir(string pathToPhotos, out int total, int pageNo = 1, int pageSize = 20);
        Photo GetByGuid(Guid guid);
        List<string> CheckPhotos(string path);
        List<string> CheckPhotoFiles(string path);
        List<string> CheckProfilePhotos(string path);
    }
}