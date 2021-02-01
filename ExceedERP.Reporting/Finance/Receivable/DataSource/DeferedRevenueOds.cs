using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.Receivable.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Receivable.DataSource
{
   
    [DataObject]
    public class DeferedRevenueOds : List<DeferedRevenueVm>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DeferedRevenueVm> GetDeferedRevenue(string from,string to ,string customer)
        {
            List<DeferedRevenueVm> deferedRevenueVms = new List<DeferedRevenueVm>();

            if (!String.IsNullOrEmpty(from)&& !String.IsNullOrEmpty(to))
            {
                
                DateTime dateFrom = DateTime.Parse(from).Date;
                DateTime dateTo = DateTime.Parse(to).Date;

                var arInvoices = _db.ARInvoices
                    .Where(x => x.Date >= dateFrom && x.Date<= dateTo)
                   .Where(x => x.IsVoid == false)
                   .Where(x => x.IsOnlineApproved).ToList();

                if (!String.IsNullOrEmpty(customer))
                {
                    arInvoices = arInvoices.Where(x => x.CustomerName==customer).ToList();
                }



                foreach (var invoice in arInvoices)
                {
                    var salesorder = _db.ArSalesOrders.Find(invoice.ArSalesOrderId);
                    var deferedRevenueVm = new DeferedRevenueVm
                    {
                        CustomerName = invoice.CustomerName,
                        CustomerId = invoice.CustomerId,
                        SalesOrderNo = invoice.ArSalesOrderId.ToString(),
                        InvoiceNo = invoice.ArInvoiceId.ToString(),
                        InvoiceDate = invoice.Date,
                        Status = invoice.InvoiceStatus.ToString()
                    };

                    if (salesorder != null)
                    {
                        deferedRevenueVm.SalesOrderAmount = invoice.AmountDue; //salesorder.AmountDue;
                        deferedRevenueVm.SalesOrderRef = salesorder.OrderReference;
                        deferedRevenueVm.TotalSpot = salesorder.ArSalesOrderBroadcastTime.Count;
                        deferedRevenueVm.EarnedRevenue = 0;
                        deferedRevenueVm.DeferedRevenue = 0;
                        deferedRevenueVm.TotalSpot = 0;
                    }


                    deferedRevenueVms.Add(deferedRevenueVm);

                }



            }

            return deferedRevenueVms;
        }
    }
}

