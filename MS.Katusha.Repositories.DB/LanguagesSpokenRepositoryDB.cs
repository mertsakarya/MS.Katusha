using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;

namespace MS.Katusha.Repositories.DB
{
    public class LanguagesSpokenRepositoryDB : BaseRepositoryDB<LanguagesSpoken>, ILanguagesSpokenRepositoryDB
    {
        public LanguagesSpokenRepositoryDB(IKatushaDbContext dbContext) : base(dbContext) { }

        public void DeleteByProfileId(long profileId, string language)
        {
            var entity = SingleAttached(p => p.ProfileId == profileId && p.Language == language);
            if(entity != null)
                Delete(entity);
        }

        public void AddByProfileId(long profileId, string language)
        {
            if (SingleAttached(p => p.ProfileId == profileId && p.Language == language) == null)
                Add(new LanguagesSpoken {Language = language, ProfileId = profileId});
        }
    }
}