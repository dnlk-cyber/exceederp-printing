using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Printing.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Reporting.Printing.DataSource
{
    [DataObject]
    public class PrintEstimationMonthlyReportOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PrintEstimationMonthlyReportViewModel> GetMonthlyReport(DateTime from, DateTime to, string customerId)
        {
            var list = new List<PrintEstimationMonthlyReportViewModel>();
            
            var jobOrders = db.PrintingJobOrders
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

                var vm = new PrintEstimationMonthlyReportViewModel
                {
                    CustomerName = customer?.TradeName ?? "",
                    PrintJobVms = new List<PrintJobVm>()
                };

                var items = db.PrintingJobOrderItems
                              .Where(x => x.PrintingJobOrderId == jobOrder.PrintingJobOrderId)
                              .ToList();
                
                foreach (var item in items)
                {
                    vm.PrintJobVms.Add(new PrintJobVm
                    {
                        JobNo = item.PrintingJobOrderItemId.ToString(),
                        JobType = item.PrintJobType.ToString(),
                        Quantity = item.Quantity,
                        TotalCost = item.TotalPrice 
                    });
                }

                list.Add(vm);
            }

            return list.ToList();
        }
    }
}
