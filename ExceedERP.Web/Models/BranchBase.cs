using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Models
{
   

    public class BranchVm
    {
        public string BranchId { get; set;  } 
        public string BranchName { get; set;  }
        public string Remark { get; set;  }
        public bool IsAllowed { get; set; }
        public bool IsDefault { get; set; }
    }
    public class BranchBase
    {
          public DbContext Context
        {
            get;
            private set;
        }


        public DbSet<Branch> DbEntitySet
        {
            get;
            private set;
        }


        public IQueryable<Branch> EntitySet
        {
            get
            {
                return this.DbEntitySet;
            }
        }


        public BranchBase(DbContext context)
        {
            this.Context = context;
            this.DbEntitySet = context.Set<Branch>();
        }


        

        


        public virtual Task<Branch> GetByIdAsync(object id)
        {
            return this.DbEntitySet.FindAsync(new object[] { id });
        }


        public virtual Branch GetById(int id)
        {
             
            return this.DbEntitySet.Find(id);
        }


       
    }
}