using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;


namespace MS.Katusha.Repositories.DB
{
    public class CountriesToVisitRepositoryDB : BaseRepositoryDB<CountriesToVisit>, ICountriesToVisitRepositoryDB
    {
        public CountriesToVisitRepositoryDB(IKatushaDbContext dbContext) : base(dbContext) { }

        public void DeleteByProfileId(long profileId, Country country)
        {
            var entity = Single(p => p.ProfileId == profileId && p.Country == (byte) country);
            Delete(entity);
        }

        public void AddByProfileId(long profileId, Country country)
        {
            var entity = new CountriesToVisit() {Country = (byte) country, ProfileId = profileId};
            Add(entity);
        }
    }
}