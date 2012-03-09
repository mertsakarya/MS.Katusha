using System;
using System.Collections.Generic;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Services
{
    public interface IProfileService<T>
    {
        IEnumerable<T> GetNewProfiles<T>(int pageNo = 1, int pageSize = 20);
        IEnumerable<T> GetMostVisitedProfiles<T>(int pageNo = 1, int pageSize = 20); 
        T GetProfile<T>(int id);
        T GetProfile<T>(Guid guid);
        void CreateProfile<T>(T profile);
        void DeleteProfile<T>(T profile);
        void UpdateProfile<T>(T profile);
    }

        //    private IGirlRepository _repository;

        //public ProfileService(IGirlRepository repository)
        //{
        //    _repository = repository;
        //}

    public interface IGirlService : IProfileService<Girl>
    {

    }
}
