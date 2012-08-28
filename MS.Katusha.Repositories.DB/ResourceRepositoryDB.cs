using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;
using MS.Katusha.Repositories.DB.Context;

namespace MS.Katusha.Repositories.DB
{
    public class ResourceRepositoryDB : BaseRepositoryDB<Resource>, IResourceRepository
    {

        public ResourceRepositoryDB(IKatushaDbContext context) : base(context)
        {
        }

        public Resource[] GetActiveValues()
        {
            return DbContext.Set<Resource>().Where(r => !r.Deleted).ToArray();
        }
    }

}
