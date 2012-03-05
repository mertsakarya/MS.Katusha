using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
 
namespace ConsoleApplication1
{
    public interface IRepository<T> where T : class
    {    
        #region    Methods
    
        T GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Query(Expression<Func<T, bool>> filter);        
        void Add(T entity);
        void Remove(T entity);   
        
        #endregion
    }
    
    public abstract class Repository<T> : IRepository<T>
                                  where T : class
    {
        #region Members
 
        protected IObjectSet<T> _objectSet;
 
        #endregion
 
        #region Ctor
 
        public Repository(ObjectContext context)
        {
              _objectSet = context.CreateObjectSet<T>();
        }
 
        #endregion
 
        #region IRepository<T> Members
 
        public IEnumerable<T> GetAll()
        {
              return _objectSet;
        }
 
        public abstract T GetById(int id);
 
        public IEnumerable<T> Query(Expression<Func<T, bool>> filter)
        {
              return _objectSet.Where(filter);
        }
 
        public void Add(T entity)
        {
              _objectSet.AddObject(entity);
        }
 
        public void Remove(T entity)
        {
              _objectSet.DeleteObject(entity);
        }
 
        #endregion
      }
 
    
    public partial class CourseRepository : Repository<Course>
    {
        #region Ctor
 
        public CourseRepository(ObjectContext context)
               : base(context)
        {
        }
 
        #endregion
 
        #region Methods
 
        public override Course GetById(int id)   
        {
            return _objectSet.SingleOrDefault(e => e.CourseID == id);
        }
 
        #endregion        
    }
    
    public partial class DepartmentRepository : Repository<Department>
    {
        #region Ctor
 
        public DepartmentRepository(ObjectContext context)
               : base(context)
        {
        }
 
        #endregion
 
        #region Methods
 
        public override Department GetById(int id)   
        {
            return _objectSet.SingleOrDefault(e => e.DepartmentID == id);
        }
 
        #endregion        
    }
    
    public partial class EnrollmentRepository : Repository<Enrollment>
    {
        #region Ctor
 
        public EnrollmentRepository(ObjectContext context)
               : base(context)
        {
        }
 
        #endregion
 
        #region Methods
 
        public override Enrollment GetById(int id)   
        {
            return _objectSet.SingleOrDefault(e => e.EnrollmentID == id);
        }
 
        #endregion        
    }
    
    public partial class PersonRepository : Repository<Person>
    {
        #region Ctor
 
        public PersonRepository(ObjectContext context)
               : base(context)
        {
        }
 
        #endregion
 
        #region Methods
 
        public override Person GetById(int id)   
        {
            return _objectSet.SingleOrDefault(e => e.PersonID == id);
        }
 
        #endregion        
    }
        
  public interface IUnitOfWork
  {
      #region    Methods
    
            IRepository<Course> Courses { get; }   
            IRepository<Department> Departments { get; }   
            IRepository<Enrollment> Enrollments { get; }   
            IRepository<Person> People { get; }   
        void Commit();
    
    #endregion
  }
 
  public partial class UnitOfWork
  {
    #region Members
 
    private readonly ObjectContext _context;
        private CourseRepository _courses;
        private DepartmentRepository _departments;
        private EnrollmentRepository _enrollments;
        private PersonRepository _people;
        
    #endregion
 
    #region Ctor
 
    public UnitOfWork(ObjectContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context wasn't supplied");
      }
 
      _context = context;
    }
 
    #endregion
 
    #region IUnitOfWork Members
 
        public IRepository<Course> Courses
    {
        get
        {
            if (_courses == null)
            {
                _courses = new CourseRepository(_context);
            }
            return _courses;
        }
    }
        public IRepository<Department> Departments
    {
        get
        {
            if (_departments == null)
            {
                _departments = new DepartmentRepository(_context);
            }
            return _departments;
        }
    }
        public IRepository<Enrollment> Enrollments
    {
        get
        {
            if (_enrollments == null)
            {
                _enrollments = new EnrollmentRepository(_context);
            }
            return _enrollments;
        }
    }
        public IRepository<Person> People
    {
        get
        {
            if (_people == null)
            {
                _people = new PersonRepository(_context);
            }
            return _people;
        }
    }
        
    
    public void Commit()
    {
      _context.SaveChanges();
    }
 
    #endregion
  }
}