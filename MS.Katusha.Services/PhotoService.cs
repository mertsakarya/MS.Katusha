using System;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;

namespace MS.Katusha.Services
{
    public class PhotoService : IPhotoService
    {
        private readonly IPhotoRepositoryDB _repository;

        public PhotoService(IPhotoRepositoryDB repository)
        {
            _repository = repository;
        }

        public void DeletePhoto(Guid guid)
        {
            var entity = _repository.GetByGuid(guid);
            if (entity != null) _repository.Delete(entity);
        }

        public Photo GetPhotoByGuid(Guid guid)
        {
            return _repository.GetByGuid(guid);
        }
        public void AddPhoto(Photo photo)
        {
            _repository.Add(photo, photo.Guid);
            _repository.Save();
        }
    }
}
