using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Context;

namespace MS.Katusha.Repositories.DB
{
    public class GeoTimeZoneRepositoryDB : IGeoTimeZoneRepositoryDB
    {
        private readonly KatushaDbContext _dbContext;
        public GeoTimeZoneRepositoryDB(IKatushaDbContext dbContext) { _dbContext = dbContext as KatushaDbContext; }
        public IList<GeoTimeZone> GetAll() { return _dbContext.Set<GeoTimeZone>().AsQueryable().AsNoTracking().ToList(); }
    }
}