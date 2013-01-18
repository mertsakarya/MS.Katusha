using System;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IRavenRepository<T> : IRepository<T> where T : BaseModel
    {
        /// <summary>
        /// BECAREFUL!!!
        /// <code>
        /// Patch("blogposts/1234",
        ///    new[]{
        ///        new PatchRequest
        ///            {
        ///                Type = PatchCommandType.Add,
        ///                Name = "Comments",
        ///                Value = RavenJObject.FromObject(comment)
        ///            }
        ///    });
        /// </code>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchRequests"></param>
        //void Patch(long id, PatchRequest[] patchRequests);

        T Add(T entity, DateTime expireTimeUtc);

    }
}
