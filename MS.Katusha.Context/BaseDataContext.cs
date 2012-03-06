using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace MS.Katusha.Context
{

    public interface IDataContext
    {
        int Save();
    }

    public abstract  class BaseDataContext : IDataContext
    {
        public abstract int Save();
    }

    public class DatabaseContext : BaseDataContext
    {
        private DbContext _context;

        public DatabaseContext(DbContext context)
        {
            _context = context;
        }

        public override int Save()
        {
            return _context.SaveChanges();
        }
    }
    public class RavenDbContext : BaseDataContext
    {
        private string _context;

        public RavenDbContext(string connectionString)
        {
            _context = connectionString;
        }

        public override int Save()
        {
            return _context.Length; //return _context.SaveChanges();
        }
    }

}
