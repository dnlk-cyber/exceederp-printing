//using ExceedERP.DataAccess.Context;
//using ExceedERP.Reporting.JobCosting.ViewModel;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Telerik.Reporting;

//namespace ExceedERP.Reporting.JobCosting.DataSource
//{

//    [DataObject]

//    public class JobOrderCardOds
//    {
//        ExceedDbContext db = new ExceedDbContext();

//        [DataObjectMethod(DataObjectMethodType.Select)]
//        public JobOrderCardVM JobOrderCar(string id)
//        {
//            if (!int.TryParse(id, out int jobOrderId))
//            {
//                return null;
//            }

//            var jobOrder = db.Jobs.FirstOrDefault(x => x.JobId == jobOrderId);

//            if (jobOrder == null)
//            {
//                return null;
//            }

//            var jobOrders = new JobOrderCardVM
//            {
//                ReceivedDate = jobOrders.ReceivedDate,
//                Quantity = jobCost.Quantity,
//                Year = jobCost.Year,
//                Customer = jobCost.Customer,
//                ReceiptNo = jobCost.ReceiptNo,
//                OrderSize = jobCost.OrderSize,
//                SalesPrice = jobCost.SalesPrice
//            };

//            return jobOrder;
//        }
//    }


//}
//}
