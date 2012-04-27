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

        public void DeleteByProfileId(long profileId, string country)
        {
            var entity = SingleAttached(p => p.ProfileId == profileId && p.Country == country);
            if(entity != null)
                Delete(entity);
        }

        public void AddByProfileId(long profileId, string country)
        {
            var exist = SingleAttached(p => p.ProfileId == profileId && p.Country == country);
            if(exist == null)
                Add(new CountriesToVisit() { Country = country, ProfileId = profileId });
        }
    }
}