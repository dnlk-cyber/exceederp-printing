//using ExceedERP.DataAccess.Context;
//using ExceedERP.Reporting.Printing.ViewModel;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data.Entity;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace ExceedERP.Reporting.Printing.DataSource
//{
//    [DataObject]
//    public class PrintTotalProductionCostReportOds
//    {
//        private readonly ExceedDbContext db = new ExceedDbContext();

//        [DataObjectMethod(DataObjectMethodType.Select)]
//        public List<PrintTotalProductionCostViewModel> GetTotalProductionCost(DateTime from, DateTime to)
//        {
//            List<PrintTotalProductionCostViewModel> list = new List<PrintTotalProductionCostViewModel>();
//            var jobOrders = db.PrintingJobOrders
//                              .Where(x => x.IsOnlineApproved)
//                              .Where(x => DbFunctions.TruncateTime(x.DateCreated) >= from.Date && DbFunctions.TruncateTime(x.DateCreated) <= to.Date)
//                              .ToList();
//            foreach (var jobOrder in jobOrders)
//            {
//                var customer = db.OrganizationCustomers
//                                .FirstOrDefault(x => x.OrganizationCustomerID == jobOrder.CustomerId);

//                var items = db.PrintingJobOrderItems
//                              .Where(x => x.PrintingJobOrderId == jobOrder.PrintingJobOrderId)
//                              .ToList();

//                foreach (var item in items)
//                {
                    
//                }
//            }
//    }
//}
