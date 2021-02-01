using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.AP
{
    [DataObject]
    public class ApAgingByInvoiceDataSource : List<ApAgingbyInvoiceViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ApAgingbyInvoiceViewModel> ApAgingByInvoice(string agetype, string branch, string supplier, string periodFrom, string periodTo)
        {
            if (String.IsNullOrEmpty(agetype))
            {
                agetype = "Invoice Date";
            }

           var list = ApAgingByInvoiceData(agetype,branch, supplier, periodTo);
           

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
                .Select(x => new { x.AmountDue,x.SupplierName,x.SupplierId,x.InvoiceNumber,x.InvoiceDate,x.Remark })
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
                    //.Where(x => x.Date >=from)
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
                        Reference =invoice.Remark,
                        OutstandingAmount = invoice.AmountDue
                    };
                    var date1 = agetype == "Invoice Date" ? invoice.InvoiceDate : invoice.InvoiceDate;//invoice date or due date

                    record.ActualAge = (int)Math.Round(XAPI.Utilities.getDateDifferenceinDays(date1, today),0);

                    if (XAPI.Utilities.getDateDifferenceinDays(date1, today) < 1)
                        record.CurrentAmount = record.OutstandingAmount;
                    else if (XAPI.Utilities.getDateDifferenceinDays(date1, today) < periodSetup.Period1)
                        record.Aging1Amount = record.OutstandingAmount;
                    else if (XAPI.Utilities.getDateDifferenceinDays(date1, today) < periodSetup.Period2)
                        record.Aging2Amount = record.OutstandingAmount;
                    else if (XAPI.Utilities.getDateDifferenceinDays(date1, today) < periodSetup.Period3)
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
