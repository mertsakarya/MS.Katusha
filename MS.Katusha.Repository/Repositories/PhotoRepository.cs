using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Repository.Interfaces;

namespace MS.Katusha.Repository.Repositories
{
    public class PhotoRepository : BaseGuidRepository<Photo>, IPhotoRepository
    {
        public PhotoRepository(KatushaContext context) : base(context) { }
    }
}