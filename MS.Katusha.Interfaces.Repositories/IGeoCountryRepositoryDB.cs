using System.Collections.Generic;
using MS.Katusha.Domain.Entities;

namespace MS.Katusha.Interfaces.Repositories
{
    public interface IGeoCountryRepositoryDB
    {
        IList<GeoCountry> GetAll();
    }
}