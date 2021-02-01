using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Models
{
    public class ApplicationBranchStore : IDisposable
    {
         private bool _disposed;
        private BranchBase _branchBase;


        public ApplicationBranchStore(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            Context = context;
            _branchBase = new BranchBase(context);
        }


        public IQueryable<Branch> Branches
        {
            get
            {
                return _branchBase.EntitySet;
            }
        }
        
        public DbContext Context
        {
            get;
            private set;
        }


         

        public Task<Branch> FindByIdAsync(string id)
        {
            ThrowIfDisposed();
            return _branchBase.GetByIdAsync(id);
        }


        public Branch FindById(string id)
        {
            ThrowIfDisposed();
            int branchId = int.Parse(id);
            return _branchBase.GetById(branchId);
        }

        public Task<Branch> FindByNameAsync(string branchName)
        {
            ThrowIfDisposed();
            return _branchBase.EntitySet
                .FirstOrDefaultAsync(u => String.Equals(u.Name, branchName, StringComparison.CurrentCultureIgnoreCase));
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
            GC.SuppressFinalize(this);
        }
    }
}