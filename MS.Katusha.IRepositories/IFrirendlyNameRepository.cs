﻿using System;
using System.Linq;
using System.Linq.Expressions;
using MS.Katusha.Domain.Entities.BaseEntities;

namespace MS.Katusha.IRepositories
{
    public interface IFriendlyNameRepository<T>: IGuidRepository<T> where T : BaseFriendlyModel
    {
        T GetByFriendlyName(string friendlyName, params Expression<Func<T, object>>[] includeExpressionParams);
        bool CheckIfFriendlyNameExsists(string friendlyName);
    }
}
