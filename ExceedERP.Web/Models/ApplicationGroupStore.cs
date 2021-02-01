using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace ExceedERP.Web.Models
{
    public class ApplicationGroupStore : IDisposable
    {
        private bool _disposed;
        private GroupStoreBase _groupStore;


        public ApplicationGroupStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
            _groupStore = new GroupStoreBase(context);
        }


        public IQueryable<ApplicationGroup> Groups
        {
            get
            {
                return _groupStore.EntitySet;
            }
        }
        
        public DbContext Context
        {
            get;
            private set;
        }


        public virtual void Create(ApplicationGroup group)
        {
            ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            _groupStore.Create(group);
            Context.SaveChanges();
        }


        public virtual async Task CreateAsync(ApplicationGroup group)
        {
            ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            _groupStore.Create(group);
            await Context.SaveChangesAsync();
        }


        public virtual async Task DeleteAsync(ApplicationGroup group)
        {
            ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            _groupStore.Delete(group);
            await Context.SaveChangesAsync();
        }


        public virtual void Delete(ApplicationGroup group)
        {
            ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            _groupStore.Delete(group);
            Context.SaveChanges();
        }


        public Task<ApplicationGroup> FindByIdAsync(string roleId)
        {
            ThrowIfDisposed();
            return _groupStore.GetByIdAsync(roleId);
        }


        public ApplicationGroup FindById(string roleId)
        {
            ThrowIfDisposed();
            return _groupStore.GetById(roleId);
        }

        public Task<ApplicationGroup> FindByNameAsync(string groupName)
        {
            ThrowIfDisposed();
            return _groupStore.EntitySet
                .FirstOrDefaultAsync(u => String.Equals(u.Name, groupName, StringComparison.CurrentCultureIgnoreCase));
        }


        public virtual async Task UpdateAsync(ApplicationGroup group)
        {
            ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            _groupStore.Update(group);
            await Context.SaveChangesAsync();
        }


        public virtual void Update(ApplicationGroup group)
        {
            ThrowIfDisposed();
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            _groupStore.Update(group);
            Context.SaveChanges();
        }


        // DISPOSE STUFF: ===============================================

        public bool DisposeContext
        {
            get;
            set;
        }


        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposing)
        {
            if (DisposeContext && disposing && Context != null)
            {
                Context.Dispose();
            }
            _disposed = true;
            Context = null;
            _groupStore = null;
        }
    }
}