using MS.Katusha.Domain.Entities.BaseEntities;
using MS.Katusha.Repositories.DB.Context;

namespace MS.Katusha.Repositories.DB.Base
{
    public class RepositoryDB<T> : BaseRepositoryDB<T> where T : BaseModel
    {
        public RepositoryDB(IKatushaDbContext context) : base(context)
        {
                
        }
    }
}