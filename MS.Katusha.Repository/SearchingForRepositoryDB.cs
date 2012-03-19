using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;


namespace MS.Katusha.Repositories.DB
{
    public class SearchingForRepositoryDB : BaseRepositoryDB<SearchingFor>, ISearchingForRepositoryDB
    {
        public SearchingForRepositoryDB(IKatushaDbContext dbContext) : base(dbContext) { }

        public void DeleteByProfileId(long profileId, LookingFor lookingFor)
        {
            var entity = Single(p => p.ProfileId == profileId && p.Search == (byte) lookingFor);
            if(entity != null)
                Delete(entity);
        }

        public void AddByProfileId(long profileId, LookingFor lookingFor)
        {
            var exist = Single(p => p.ProfileId == profileId && p.Search == (byte)lookingFor);
            if (exist == null)
                Add(new SearchingFor {Search = (byte) lookingFor, ProfileId = profileId});

        }
    }

}