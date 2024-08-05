using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExceedERP.Helpers.Fleet
{
    public   class FleetReportHelper 
    {
      
        
        
        public List<Issue> GetInventoryIssue(System.Linq.Expressions.Expression<Func<Issue, bool>> predicate)
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.Issues.AsNoTracking().Where(predicate).OrderByDescending(x => x.DateCreated).Take(300).ToList();


        }
    

        public List<InventoryItem> GetInventoryItems(System.Linq.Expressions.Expression<Func<InventoryItem, bool>> predicate)
        {
            ExceedDbContext db = new ExceedDbContext();
           
            return  db.InventoryItems.AsNoTracking().Where(predicate).OrderByDescending(x => x.DateCreated).ToList();
        }

        public  List<IssueItem> GetInventoryIssueItem(System.Linq.Expressions.Expression<Func<IssueItem, bool>> predicate)
        {

            ExceedDbContext db = new ExceedDbContext();
          
            return  db.IssueItems.AsNoTracking().Where(predicate).OrderByDescending(x => x.DateCreated).ToList();

        }

         
    
    }
}
