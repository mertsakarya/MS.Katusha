using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using MS.Katusha.Domain.Entities;
using MS.Katusha.Interfaces.Repositories;
using MS.Katusha.Repositories.DB.Context;

namespace MS.Katusha.Repositories.DB
{
    public class GeoLanguageRepositoryDB : IGeoLanguageRepositoryDB
    {
        private readonly KatushaDbContext _dbContext;
        public GeoLanguageRepositoryDB(IKatushaDbContext dbContext) { _dbContext = dbContext as KatushaDbContext; }
        public IList<GeoLanguage> GetAll() { return _dbContext.Set<GeoLanguage>().AsQueryable().AsNoTracking().ToList(); }
    }
}