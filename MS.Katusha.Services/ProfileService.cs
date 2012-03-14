using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Interfaces.Services;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Services
{
    public abstract class ProfileService<T> : IProfileService<T> where T : BaseFriendlyModel
    {

        protected readonly IFriendlyNameRepository<T> Repository;
        private readonly IUserRepositoryDB _userRepository;

        protected ProfileService(IFriendlyNameRepository<T> repository, IUserRepositoryDB userRepository)
        {
            Repository = repository;
            _userRepository = userRepository;
        }

        public IEnumerable<T> GetNewProfiles(int pageNo = 1, int pageSize = 20)
        {
            var items = Repository.GetAll(pageNo, pageSize);
            return items;
        }

        public IEnumerable<T> GetMostVisitedProfiles(int pageNo = 1, int pageSize = 20)
        {
            throw new NotImplementedException();
        }

        public virtual T GetProfile(int id, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            throw new NotImplementedException();
        }

        public virtual T GetProfile(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            var item = Repository.GetByGuid(guid, includeExpressionParams);
            return item;
        }

        public void CreateProfile(T profile)
        {
            Repository.Add(profile);
            Repository.Save();
            var user = _userRepository.GetByGuid(profile.Guid);
            user.Gender = (profile is Boy) ? (byte)Sex.Male : (profile is Girl) ? (byte)Sex.Female : (byte)0;
            _userRepository.FullUpdate(user);
        }

        public void DeleteProfile(Guid guid)
        {
            var profile = Repository.GetByGuid(guid);
            if (profile != null)
            {
                Repository.Delete(profile);
                Repository.Save();
                var user = _userRepository.GetByGuid(profile.Guid);
                user.Gender = (byte) 0;
                _userRepository.FullUpdate(user);
            }
        }

        public abstract void UpdateProfile(T profile);

        protected void SetData(Profile dataProfile, Profile profile)
        {
            dataProfile.Alcohol = profile.Alcohol;
            dataProfile.BirthYear = profile.BirthYear;
            dataProfile.BodyBuild = profile.BodyBuild;
            dataProfile.City = profile.City;
            dataProfile.Description = profile.Description;
            dataProfile.EyeColor = profile.EyeColor;
            dataProfile.From = profile.From;
            dataProfile.HairColor = profile.HairColor;
            dataProfile.Height = profile.Height;
            dataProfile.Name = profile.Name;
            dataProfile.Religion = profile.Religion;
            dataProfile.Smokes = profile.Smokes;
        }


    }
}

