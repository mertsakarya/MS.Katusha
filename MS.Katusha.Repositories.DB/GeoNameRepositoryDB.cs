using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Context;

namespace MS.Katusha.Repositories.DB
{
    public class GeoNameRepositoryDB : IGeoNameRepositoryDB
    {
        private readonly KatushaDbContext _dbContext;
        public GeoNameRepositoryDB(IKatushaDbContext dbContext) { _dbContext = dbContext as KatushaDbContext; }
        public IList<GeoName> GetAll() { return _dbContext.Set<GeoName>().AsQueryable().AsNoTracking().ToList(); }
    }
}