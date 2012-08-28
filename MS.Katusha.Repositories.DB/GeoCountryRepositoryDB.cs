using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Context;

namespace MS.Katusha.Repositories.DB
{
    public class GeoCountryRepositoryDB : IGeoCountryRepositoryDB
    {
        private readonly KatushaDbContext _dbContext;
        public GeoCountryRepositoryDB(IKatushaDbContext dbContext) { _dbContext = dbContext as KatushaDbContext; }
        public IList<GeoCountry> GetAll() { return _dbContext.Set<GeoCountry>().AsQueryable().AsNoTracking().ToList(); }
    }
}