using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.Sales.DataSource
{

    [DataObject]
    public class InvoiceRegisterDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InvoiceRegisterViewModel> GetInvoiceRegister(string fiscalYear, string periodFrom, string periodTo, string branchid)
        {
            List<InvoiceRegisterViewModel> invoiceRegisterList = new List<InvoiceRegisterViewModel>();
            if (!String.IsNullOrEmpty(periodFrom) && !String.IsNullOrEmpty(periodTo))
            {
                int from = int.Parse(periodFrom);
                int to = int.Parse(periodTo);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;

                var arInvoices = _db.ARInvoices
                      .Where(x => x.IsVoid == false)
                      .Where(x => x.IsOnlineApproved)
                      .Where(x => x.Date >= dt11 && x.Date <= dt22).ToList();

                if (!String.IsNullOrEmpty(branchid))
                {
                    var salesOrderIds = _db.ArSalesOrders.Where(x => x.BranchId.ToString() == branchid)
                        .Select(x => x.ArSalesOrderId).ToList();
                    arInvoices = arInvoices.Where(x => salesOrderIds.Contains(x.ArSalesOrderId)).ToList();
                }

                foreach (var arInvoice in arInvoices)
                {
                    var invoice = new InvoiceRegisterViewModel
                    {
                        CustomoerName = arInvoice.CustomerName,
                        SaleOrderNo = arInvoice.ArSalesOrderId,
                        InvoiceNo = arInvoice.ArInvoiceId,
                        Reference = arInvoice.Reference,
                        InvoiceDate = arInvoice.Date,
                        Amount = arInvoice.AmountDue
                    };


                    invoiceRegisterList.Add(invoice);
                }
            }


            return invoiceRegisterList;
        }
    }
}
