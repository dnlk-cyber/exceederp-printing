using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Furniture.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Reporting.Furniture.DataSource
{
    [DataObject]
    public class FurnitureMonthlyReportOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FurnitureMonthlyReportViewModel> GetMonthlyReport(DateTime from, DateTime to, string customerId)
        {
            var list = new List<FurnitureMonthlyReportViewModel>();
            
            var jobOrders = db.FurnitureJobOrderForms
                              .Where(x => x.IsOnlineApproved)
                              .Where(x => DbFunctions.TruncateTime(x.DateCreated) >= from.Date && DbFunctions.TruncateTime(x.DateCreated) <= to.Date)
                              .ToList();
            if (int.TryParse(customerId, out int id))
            {
                jobOrders = jobOrders.Where(x => x.CustomerId == id).ToList();
            }
           
            foreach (var jobOrder in jobOrders)
            {
                var customer = db.OrganizationCustomers
                                 .FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId);

                var vm = new FurnitureMonthlyReportViewModel
                {
                    CustomerName = customer?.TradeName ?? "",
                    FurnitureJobVms = new List<FurnitureJobVm>()
                };

                var items = db.FurnitureJobOrderItems
                              .Where(x => x.FurnitureJobOrderFormId == jobOrder.FurnitureJobOrderFormId)
                              .ToList();
                
                foreach (var item in items)
                {
                    var jobType = db.FurnitureStandardJobTypes.FirstOrDefault(x => x.FurnitureStandardJobTypeId == item.JobTypeId);
                    vm.FurnitureJobVms.Add(new FurnitureJobVm
                    {
                        JobType = jobType?.JobTypeName ?? "",
                        Quantity = item.Quantity,
                        TotalCost = item.UnitPriceWithVAT 
                    });
                }

                list.Add(vm);
            }

            return list.ToList();
        }
    }
}
