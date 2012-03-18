using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Text;
using MS.Katusha.Domain;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Base;

namespace MS.Katusha.Repositories.DB
{
    public class ResourceLookupRepositoryDB : BaseRepositoryDB<ResourceLookup>, IResourceLookupRepository
    {

        public ResourceLookupRepositoryDB(IKatushaDbContext context) : base(context)
        {
        }

        public ResourceLookup[] GetActiveValues()
        {
            return DbContext.Set<ResourceLookup>().Where(r => !r.Deleted).OrderBy(r=>r.LookupName).OrderBy(r=>r.Language).ToArray();
        }
    }

}
