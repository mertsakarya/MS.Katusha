using System;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Services
{

    public interface IPhotoService
    {
        void DeletePhoto(Guid guid);
        void AddPhoto(Photo photo);
        Photo GetPhotoByGuid(Guid guid);
    }
}
