using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.Repository
{
    public interface IGuidRepository<T> where T : BaseFriendlyModel
    {
    }
}
