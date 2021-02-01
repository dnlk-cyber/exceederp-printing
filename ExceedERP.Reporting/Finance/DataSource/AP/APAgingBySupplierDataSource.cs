using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;
using XAPI;

namespace ExceedERP.Reporting.Finance.DataSource.AP
{
    [DataObject]
    public class ApAgingBySupplierDataSource : List<ApAgingbySupplierViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ApAgingbySupplierViewModel> ApAgingBySupplier(string agetype,string branch, string supplier,string periodFrom,string periodTo)
        {
            List<ApAgingbySupplierViewModel> list = new List<ApAgingbySupplierViewModel> { };

            if (String.IsNullOrEmpty(agetype))
            {
                agetype = "Invoice Date";
            }
            var invoicelist = ApAgingByInvoiceData(agetype,branch, supplier, periodTo);

           

            List<string> uniquecustomerlist = invoicelist.Select(x => x.SupplierId).Distinct().ToList();
            foreach (string supplierid in uniquecustomerlist)
            {
                var record = new ApAgingbySupplierViewModel {SupplierId = supplier };
                var invoices = invoicelist.Where(x => x.SupplierId.ToUpper() == supplierid.ToUpper()).ToList();
                var supplierName = invoices.FirstOrDefault(x => x.SupplierId.ToUpper() == supplierid.ToString());
                if (supplierName != null) {record.SupplierName = supplierName.SupplierName;}
                record.OutstandingAmount = invoices.Sum(x => x.OutstandingAmount);
                record.CurrentAmount = invoices.Sum(x => x.CurrentAmount);
                record.Aging1Amount = invoices.Sum(x => x.Aging1Amount);
                record.Aging2Amount = invoices.Sum(x => x.Aging2Amount);
                record.Aging3Amount = invoices.Sum(x => x.Aging3Amount);
                record.Aging4Amount = invoices.Sum(x => x.Aging4Amount);
                list.Add(record);
            }

            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> AgingReportParamenters()
        {

            List<string> list = new List<string> {"Invoice Date", "Due Date"};
            return list;
        }

        private List<ApAgingbyInvoiceViewModel> ApAgingByInvoiceData(string agetype, string branch, string supplier, string period)
        {
            List<ApAgingbyInvoiceViewModel> list = new List<ApAgingbyInvoiceViewModel> { };
            var invoiceList = _db.APPaybleInvoices.Where(x => x.InvoiceStatus == InvoiceStatus.Open)
                                           .Where(x => x.IsOnlineApproved&&x.IsVoid==false)
                .Select(x => new { x.AmountDue, x.SupplierName, x.SupplierId, x.InvoiceNumber, x.InvoiceDate, x.Remark })
                                           .ToList();

            //if (!String.IsNullOrEmpty(branch))
            //{
            //    invoiceList = invoiceList.Where(x => x.BranchId.ToString() == branch).ToList();
            //}
            if (!String.IsNullOrEmpty(supplier))
            {
                invoiceList = invoiceList.Where(x => x.SupplierId == supplier).ToList();
            }
            if (!String.IsNullOrEmpty(period))
            {
                int periodId = int.Parse(period);
                
                DateTime from = _db.GLPeriods.Find(periodId).DateFrom.Date;
                DateTime to = _db.GLPeriods.Find(periodId).DateTo.Date;
                invoiceList = invoiceList
                    //.Where(x => x.Date >= from)
                    .Where(x => x.InvoiceDate <= to)
                    .ToList();
            }
            DateTime today = DateTime.Today;
            if (String.IsNullOrEmpty(agetype))
            {
                agetype = "Invoice Date";
            }

            var periodSetup = _db.AgingPeriodSetups.FirstOrDefault(x => x.Type == AgingType.AccountReceivable);

            if (periodSetup != null)
            {
                foreach (var invoice in invoiceList)
                {
                    var record = new ApAgingbyInvoiceViewModel
                    {
                        SupplierName = invoice.SupplierName,
                        SupplierId = invoice.SupplierId,
                        InvoiceNo = invoice.InvoiceNumber,
                        OutstandingAmount = invoice.AmountDue,
                       // BranchId= invoice.BranchId
                    };
                    var date1 = agetype == "Invoice Date" ? invoice.InvoiceDate : invoice.InvoiceDate;//invoice date or due date

                    if (Utilities.getDateDifferenceinDays(date1, today) < 1)
                        record.CurrentAmount = record.OutstandingAmount;
                    else if (Utilities.getDateDifferenceinDays(date1, today) < periodSetup.Period1)
                        record.Aging1Amount = record.OutstandingAmount;
                    else if (Utilities.getDateDifferenceinDays(date1, today) < periodSetup.Period2)
                        record.Aging2Amount = record.OutstandingAmount;
                    else if (Utilities.getDateDifferenceinDays(date1, today) < periodSetup.Period3)
                        record.Aging3Amount = record.OutstandingAmount;
                    else
                        record.Aging4Amount = record.OutstandingAmount;
                    list.Add(record);
                }
            }

            return list;
        }
    }
}
