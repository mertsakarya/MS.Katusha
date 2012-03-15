using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Exceptions.Services;
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

        public virtual T GetProfile(Guid guid, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            var item = Repository.GetByGuid(guid, includeExpressionParams);
            return item;
        }

        /// <summary>
        /// Tries to find friendlyName as Guid first, if unsuccessfull, looks for friendlyName
        /// </summary>
        /// <param name="friendlyName">This value can be Guid or a FriendlyName</param>
        /// <param name="includeExpressionParams"></param>
        /// <returns></returns>
        public T GetProfile(string friendlyName, params Expression<Func<T, object>>[] includeExpressionParams)
        {
            Guid guid;
            if (Guid.TryParse(friendlyName, out guid))
                return GetProfile(guid);
            var item = Repository.GetByFriendlyName(friendlyName, includeExpressionParams);
            if (item == null)
                throw new KatushaFriendlyNameNotFoundException(friendlyName);
            return item;
        }

        public void CreateProfile(T profile)
        {
            if (!String.IsNullOrEmpty(profile.FriendlyName))
                if (Repository.CheckIfFriendlyNameExists(profile.FriendlyName, profile.Id))
                    throw new KatushaFriendlyNameExistsException(profile as Profile);
            Repository.Add(profile);
            Repository.Save();
            var user = _userRepository.GetByGuid(profile.Guid);
            user.Gender = (profile is Boy) ? (byte) Sex.Male : (profile is Girl) ? (byte) Sex.Female : (byte) 0;
            _userRepository.FullUpdate(user);
        }

        public void DeleteProfile(Guid guid, bool force = false)
        {
            var profile = Repository.GetByGuid(guid);
            if (profile != null)
            {
                ExecuteDeleteProfile(profile, force);
            }
        }

        private void ExecuteDeleteProfile(T profile, bool softDelete)
        {
            if (!softDelete)
                Repository.Delete(profile);
            else
                Repository.SoftDelete(profile);

            Repository.Save();
            var user = _userRepository.GetByGuid(profile.Guid);
            user.Gender = (byte) 0;
            _userRepository.FullUpdate(user);
        }

        public void DeleteProfile(string friendlyName, bool softDelete = true)
        {
            var profile = Repository.GetByFriendlyName(friendlyName);
            if (profile != null)
            {
                ExecuteDeleteProfile(profile, softDelete);
            }
        }

        public virtual void UpdateProfile(T profile)
        {
            if(profile != null && !String.IsNullOrEmpty(profile.FriendlyName))
                if (Repository.CheckIfFriendlyNameExists(profile.FriendlyName, profile.Id))
                    throw new KatushaFriendlyNameExistsException(profile as Profile);
        }

        protected void SetData(Profile dataProfile, Profile profile)
        {
            dataProfile.FriendlyName = profile.FriendlyName;
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

