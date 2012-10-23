using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MS.Katusha.Interfaces.Services
{
    public interface IRestore<T>
    {
        IList<string> RestoreFromDB(Expression<Func<T, bool>> filter, bool deleteIfExists = false);
    }
}