using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;
using MS.Katusha.Repositories.DB.Context;

namespace MS.Katusha.Repositories.DB
{
    public class ConfigurationDataRepositoryDB : BaseRepositoryDB<ConfigurationData>, IConfigurationDataRepository
    {

        public ConfigurationDataRepositoryDB(IKatushaDbContext context)
            : base(context)
        {
        }

        public ConfigurationData[] GetActiveValues()
        {
            return DbContext.Set<ConfigurationData>().Where(r => !r.Deleted).ToArray();
        }
    }

}
