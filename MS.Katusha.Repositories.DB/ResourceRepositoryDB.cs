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
