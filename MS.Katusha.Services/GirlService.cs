using System;
using MS.Katusha.Interfaces.Services;


namespace MS.Katusha.Services
{

    public class GirlService : IGirlService
    {
        public T[] GetNewProfiles<T>(int pageNo = 0, int pageSize = 0)
        {
            throw new NotImplementedException();
        }

        public T[] GetMostVisitedProfiles<T>(int pageNo = 0, int pageSize = 0)
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
