using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Enumerations;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;

namespace MS.Katusha.Repositories.DB
{
    public class LanguagesSpokenRepositoryDB : BaseRepositoryDB<LanguagesSpoken>, ILanguagesSpokenRepositoryDB
    {
        public LanguagesSpokenRepositoryDB(IKatushaDbContext dbContext) : base(dbContext) { }

        public void DeleteByProfileId(long profileId, Language language)
        {
            var entity = Single(p => p.ProfileId == profileId && p.Language == (byte) language);
            if(entity != null)
                Delete(entity);
        }

        public void AddByProfileId(long profileId, Language language)
        {
            if (Single(p => p.ProfileId == profileId && p.Language == (byte)language) == null) ;
                Add(new LanguagesSpoken() {Language = (byte) language, ProfileId = profileId});
        }
    }
}