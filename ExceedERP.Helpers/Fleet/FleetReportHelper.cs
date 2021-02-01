using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Helpers.Common;

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
