using System;
using System.Collections.Generic;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Repositories.DB;


namespace MS.Katusha.Services
{

    public class GirlService : IGirlService
    {
        private IGirlRepositoryDB _girlRepository;

        public GirlService(IGirlRepositoryDB girlRepository)
        {
            _girlRepository = girlRepository;
        }
        public IEnumerable<T> GetNewProfiles<T>(int pageNo = 1, int pageSize = 20)
        {
            var a = _girlRepository.GetAll(pageNo, pageSize);
            var b = a as IEnumerable<T>;
            return b;
        }

        public IEnumerable<T> GetMostVisitedProfiles<T>(int pageNo = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public T GetProfile<T>(int id)
        {
            throw new NotImplementedException();
        }

        public T GetProfile<T>(Guid guid)
        {
            throw new NotImplementedException();
        }

        public void CreateProfile<T>(T profile)
        {
            throw new NotImplementedException();
        }

        public void DeleteProfile<T>(T profile)
        {
            throw new NotImplementedException();
        }

        public void UpdateProfile<T>(T profile)
        {
            throw new NotImplementedException();
        }
    }
}
