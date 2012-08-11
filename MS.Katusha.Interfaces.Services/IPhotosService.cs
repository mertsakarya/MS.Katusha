using System;
using System.Collections.Generic;
using System.Web;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;

namespace MS.Katusha.Interfaces.Services
{
    public interface IPhotosService
    {
        void MakeProfilePhoto(long profileId, Guid photoGuid);
        bool DeletePhoto(long profileId, Guid guid);
        ViewDataUploadFilesResult AddPhoto(long profileId, string description, HttpPostedFileBase hpf);
        Photo AddSamplePhoto(long profileId, string description, string fileName, string filePath);
        IList<Guid> AllPhotos(out int total, string prefix = "", int pageNo = 1, int pageSize = 20);
        Photo GetByGuid(Guid guid);
        List<string> CheckPhotos();
        List<string> CheckPhotoFiles();
        List<string> CheckProfilePhotos();
        string GetPhotoUrl(Guid photoGuid, PhotoType photoType, bool encode = false);
        string GetPhotoBaseUrl();

    }
}