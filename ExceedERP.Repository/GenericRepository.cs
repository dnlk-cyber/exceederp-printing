using ExceedERP.DataAccess;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;

namespace ExceedERP.Repository
{
    public class GenericRepository<T> : IGenericRepository<T>
    where T : class
    {
        internal DbSet<T> dbSet;
        public GenericRepository(ExceedDbContext context)
        {
            _context = context;
        }
        private ExceedDbContext _context;
        public ExceedDbContext db
        {
            get { 
                if(_context == null)
            {
                _context = new ExceedDbContext();
                return _context;
            }
                return _context; }
            set { _context = value; }
        }

        public virtual List<T> GetAll()
        {
            try
            {
                IQueryable<T> query = _context.Set<T>();
                return query.ToList();
            }
            catch(Exception ee)
            {
                throw ee;
            }
        }

        public IQueryable<T> FindEnumerableBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(predicate);
                return query;
            }
            catch
            {
                throw;
            }
        }

        public List<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            try
            {
                IQueryable<T> query = _context.Set<T>().Where(predicate);
                return query.ToList();
            }
            catch
            {
                throw;
            }
        }


        public virtual bool Add(T entity)
        {
            try
            {
               
                _context.Set<T>().Add(entity);
                return true;
            }
            catch
            {
                throw;
            }
        }
        public virtual bool AddOrUpdate(T entity)
        {
            try
            {
                _context.Set<T>().AddOrUpdate(entity);
                return true;
            }
            catch
            {
                throw;
            }

             
        }

        public virtual bool Delete(T entity)
        {
            try
            {
                _context.Set<T>().Remove(entity);
                return true;
            }
            catch
            {
                throw;
            }
        }
        bool IGenericRepository<T>.DeleteById(Guid id)
        {
            T entityToDelete = _context.Set<T>().Find(id);
            return Delete(entityToDelete);
        }

        public virtual bool Edit(T entity)
        {
            try
            {
                _context.Entry(entity).State = EntityState.Modified;
                return true;
            }
            catch
            {
                throw;
            }
        }

        public virtual bool EditNoTracking(T entity)
        {
            try
            {
                var entry = _context.Entry<T>(entity);

                if (entry.State == EntityState.Detached)
                {
                    var set = _context.Set<T>();
                    T attachedEntity = set.Local.FirstOrDefault();

                    if (attachedEntity != null)
                    {
                        var attachedEntry = _context.Entry(attachedEntity);
                        attachedEntry.CurrentValues.SetValues(entity);
                    }
                    else
                    {
                        entry.State = EntityState.Modified;
                    }
                }
                return true;
            }
            catch
            {
                throw;
            }
        }


        public virtual T FindById(int id)
        {
            try
            {
                return _context.Set<T>().Find(id);
            }
            catch
            {
                throw;
            }
        }
        public virtual T FindById(Guid id)
        {
            try
            {
                return _context.Set<T>().Find(id);
            }
            catch
            {
                throw;
            }
        }

        public virtual IEnumerable<T> Get(
          Expression<Func<T, bool>> filter = null,
          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
          string includeProperties = "")
        {
            try
            {
                IQueryable<T> query = _context.Set<T>();

                if (filter != null)
                {
                    query = query.Where(filter);
                }
                if (includeProperties != null)
                    foreach (var includeProperty in includeProperties.Split
                        (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProperty);
                    }

                return query.ToList();
            }
            catch
            {
                throw;
            }
        }

        public void DeleteAll(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public virtual bool AddRange(IEnumerable<T> entities)
        {
            try
            {
                _context.Set<T>().AddRange(entities);
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
