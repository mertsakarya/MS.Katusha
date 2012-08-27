using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Repositories.DB.Base
{
    public class RepositoryDB<T> : BaseRepositoryDB<T> where T : BaseModel
    {
        public RepositoryDB(IKatushaDbContext context) : base(context)
        {
                
        }
    }
}