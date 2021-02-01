using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;
using XAPI;

namespace ExceedERP.Reporting.Finance.DataSource.AR
{
    [DataObject]
    public class ArAgingByInvoiceDataSource : List<ArAgingbyInvoiceViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ArAgingbyInvoiceViewModel> ArAgingByInvoice(string agetype, string branch, string customer, string periodFrom, string periodTo)
        {
            List<ArAgingbyInvoiceViewModel> list = new List<ArAgingbyInvoiceViewModel> { };

            if (!String.IsNullOrEmpty(periodFrom) && !String.IsNullOrEmpty(periodTo))
            {
                if (String.IsNullOrEmpty(agetype))
                {
                    agetype = "Invoice Date";
                }

                list = ArAgingByInvoiceData(agetype, branch, customer, periodTo);
            }


            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> AgingReportParamenters()
        {

            List<string> list = new List<string> {"Invoice Date", "Due Date"};
            return list;
        }
        private List<ArAgingbyInvoiceViewModel> ArAgingByInvoiceData(string agetype, string branch, string customer, string periodTo)
        {
            List<ArAgingbyInvoiceViewModel> list = new List<ArAgingbyInvoiceViewModel> { };
            var invoiceList = _db.ARInvoices
                //.Where(x => x.InvoiceStatus == InvoiceStatus.Open)
                                           .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                           .Select(x=> new {x.Reference,x.BranchId,x.CustomerId,x.CustomerName,x.Date,x.ArInvoiceId,x.AmountDue,x.DueDate})
                                           //.Where(x => x.AmountDue != 0)
                                           .ToList();

            if (!String.IsNullOrEmpty(branch))
            {
                invoiceList = invoiceList.Where(x => x.BranchId.ToString() == branch).ToList();
            }
            if (!String.IsNullOrEmpty(customer))
            {
                invoiceList = invoiceList.Where(x => x.CustomerId == customer).ToList();
            }
            if (!String.IsNullOrEmpty(periodTo))
            {
                int periodId = int.Parse(periodTo);

                DateTime startDate = _db.GLPeriods.Find(periodId).DateFrom.Date;
                DateTime asOfDate = _db.GLPeriods.Find(periodId).DateTo.Date;
                invoiceList = invoiceList
                    //.Where(x => x.Date >= from)
                    .Where(x => x.Date <= asOfDate)
                    .ToList();


                DateTime today = DateTime.Today;
                if (String.IsNullOrEmpty(agetype))
                {
                    agetype = "Invoice Date";
                }

                var periodSetup = _db.AgingPeriodSetups.FirstOrDefault(x => x.Type == AgingType.AccountReceivable);

                if (periodSetup != null)
                {
                    if (asOfDate >= today)
                    {
                        #region today or in the future

                        foreach (var invoice in invoiceList)
                        {
                            var record = new ArAgingbyInvoiceViewModel
                            {
                                CustomerName = invoice.CustomerName,
                                CustomerId = invoice.CustomerId,
                                InvoiceNo = invoice.ArInvoiceId.ToString(),
                                OutstandingAmount = invoice.AmountDue,
                                BranchId = invoice.BranchId,
                                Reference = invoice.Reference,
                            };
                            var date1 = agetype == "Invoice Date"
                                ? invoice.Date
                                : invoice.DueDate; //invoice date or due date

                            record.ActualAge = (int)Math.Round(XAPI.Utilities.getDateDifferenceinDays(date1, asOfDate), 0);

                            if (Utilities.getDateDifferenceinDays(date1, asOfDate) < 1)
                                record.CurrentAmount = record.OutstandingAmount;
                            else if (Utilities.getDateDifferenceinDays(date1, asOfDate) < periodSetup.Period1)
                                record.Aging1Amount = record.OutstandingAmount;
                            else if (Utilities.getDateDifferenceinDays(date1, asOfDate) < periodSetup.Period2)
                                record.Aging2Amount = record.OutstandingAmount;
                            else if (Utilities.getDateDifferenceinDays(date1, asOfDate) < periodSetup.Period3)
                                record.Aging3Amount = record.OutstandingAmount;
                            else
                                record.Aging4Amount = record.OutstandingAmount;
                            list.Add(record);
                        }

                        #endregion
                    }
                    else if (asOfDate < today)
                    {


                        foreach (var invoice in invoiceList)
                        {
                            string invoiceNo = $"INVOICE_{ invoice.ArInvoiceId.ToString()}_{ invoice.Reference}";
                            #region in the past

                            decimal totalcollected = 0;

                            #region get collections of the invoice after as of date

                            var collections = _db.ARStandardCollectionLines
                                .Where(x => x.ArStandardCollection.ReceiptDate > asOfDate)
                                .Where(x => x.ArStandardCollection.IsOnlineApproved)
                                .Where(x => x.ArStandardCollection.IsVoid == false)
                                .Where(x => x.Source == InvoiceSource.Invoice)
                                .Where(x => x.InvoiceNumber == invoiceNo).ToList();

                            if (collections.Any())
                                totalcollected = collections.Sum(x => x.ApplyAmount);

                            #endregion

                            var record = new ArAgingbyInvoiceViewModel
                            {
                                CustomerName = invoice.CustomerName,
                                CustomerId = invoice.CustomerId,
                                InvoiceNo = invoice.ArInvoiceId.ToString(),
                                OutstandingAmount = invoice.AmountDue + totalcollected,
                                BranchId = invoice.BranchId,
                                Reference = invoice.Reference,
                            };
                            var date1 = agetype == "Invoice Date"
                                ? invoice.Date
                                : invoice.DueDate; //invoice date or due date

                            DateTime collectionDate1 = date1.AddDays(1);
                            DateTime collectionDateP1 = date1.AddDays(periodSetup.Period1);
                            DateTime collectionDateP2 = date1.AddDays(periodSetup.Period2);
                            DateTime collectionDateP3 = date1.AddDays(periodSetup.Period3);

                            record.ActualAge = (int)Math.Round(XAPI.Utilities.getDateDifferenceinDays(date1, asOfDate), 0);

                            if (Utilities.getDateDifferenceinDays(date1, asOfDate) < 1)
                            {
                                record.CurrentAmount = invoice.AmountDue;


                                var subcollection = collections
                                    .Where(x => x.ArStandardCollection.ReceiptDate < collectionDate1).ToList();
                                if (subcollection.Any())
                                    record.CurrentAmount = record.CurrentAmount + subcollection.Sum(x => x.ApplyAmount);
                            }

                            else if (Utilities.getDateDifferenceinDays(date1, asOfDate) < periodSetup.Period1)
                            {
                                record.Aging1Amount = invoice.AmountDue;


                                var subcollection = collections
                                    .Where(x => x.ArStandardCollection.ReceiptDate >= collectionDate1)
                                    .Where(x => x.ArStandardCollection.ReceiptDate < collectionDateP1).ToList();
                                if (subcollection.Any())
                                    record.Aging1Amount = record.Aging1Amount + subcollection.Sum(x => x.ApplyAmount);
                            }

                            else if (Utilities.getDateDifferenceinDays(date1, asOfDate) < periodSetup.Period2)
                            {
                                record.Aging2Amount = invoice.AmountDue;


                                var subcollection = collections
                                    .Where(x => x.ArStandardCollection.ReceiptDate >= collectionDateP1)
                                    .Where(x => x.ArStandardCollection.ReceiptDate < collectionDateP2).ToList();
                                if (subcollection.Any())
                                    record.Aging2Amount = record.Aging2Amount + subcollection.Sum(x => x.ApplyAmount);
                            }

                            else if (Utilities.getDateDifferenceinDays(date1, asOfDate) < periodSetup.Period3)
                            {
                                record.Aging3Amount = invoice.AmountDue;


                                var subcollection = collections
                                    .Where(x => x.ArStandardCollection.ReceiptDate >= collectionDateP2)
                                    .Where(x => x.ArStandardCollection.ReceiptDate < collectionDateP3).ToList();
                                if (subcollection.Any())
                                    record.Aging3Amount = record.Aging3Amount + subcollection.Sum(x => x.ApplyAmount);
                            }

                            else
                            {
                                record.Aging4Amount = invoice.AmountDue;

                                var subcollection = collections
                                    .Where(x => x.ArStandardCollection.ReceiptDate >= collectionDateP3).ToList();
                                if (subcollection.Any())
                                    record.Aging4Amount = record.Aging4Amount + subcollection.Sum(x => x.ApplyAmount);
                            }

                            list.Add(record);
                            #endregion
                        }


                    }
                }
            }


            return list;
        }

        //private List<ArAgingbyInvoiceViewModel> ArAgingByInvoiceData1(string agetype, string branch, string customer, string period)
        //{
        //    List<ArAgingbyInvoiceViewModel> list = new List<ArAgingbyInvoiceViewModel> { };
        //    var invoiceList = _db.ARInvoices.Where(x => x.InvoiceStatus == InvoiceStatus.Open)
        //                                   .Where(x => x.IsOnlineApproved && x.IsVoid == false).ToList();
        //    if (!String.IsNullOrEmpty(branch))
        //    {
        //        invoiceList = invoiceList.Where(x => x.BranchId.ToString() == branch).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(customer))
        //    {
        //        invoiceList = invoiceList.Where(x => x.CustomerId == customer).ToList();
        //    }
        //    if (!String.IsNullOrEmpty(period))
        //    {
        //        int periodId = int.Parse(period);

        //        DateTime from = _db.GLPeriods.Find(periodId).DateFrom.Date;
        //        DateTime to = _db.GLPeriods.Find(periodId).DateTo.Date;
        //        invoiceList = invoiceList
        //            //.Where(x => x.Date >=from)
        //            .Where(x => x.Date <= to)
        //            .ToList();
        //    }
        //    DateTime today = DateTime.Today;
        //    if (String.IsNullOrEmpty(agetype))
        //    {
        //        agetype = "Invoice Date";
        //    }

        //    var periodSetup = _db.AgingPeriodSetups.FirstOrDefault(x => x.Type == AgingType.AccountReceivable);

        //    if (periodSetup != null)
        //    {
        //        foreach (ArInvoice invoice in invoiceList)
        //        {
        //            var record = new ArAgingbyInvoiceViewModel
        //            {
        //                CustomerName = invoice.CustomerName,
        //                CustomerId = invoice.CustomerId,
        //                InvoiceNo = invoice.ArInvoiceId.ToString(),
        //                Reference = invoice.Reference,
        //                OutstandingAmount = invoice.AmountDue
        //            };
        //            var date1 = agetype == "Invoice Date" ? invoice.Date : invoice.DueDate;//invoice date or due date

        //            if (XAPI.Utilities.getDateDifferenceinDays(date1, today) < 1)
        //                record.CurrentAmount = record.OutstandingAmount;
        //            else if (XAPI.Utilities.getDateDifferenceinDays(date1, today) < periodSetup.Period1)
        //                record.Aging1Amount = record.OutstandingAmount;
        //            else if (XAPI.Utilities.getDateDifferenceinDays(date1, today) < periodSetup.Period2)
        //                record.Aging2Amount = record.OutstandingAmount;
        //            else if (XAPI.Utilities.getDateDifferenceinDays(date1, today) < periodSetup.Period3)
        //                record.Aging3Amount = record.OutstandingAmount;
        //            else
        //                record.Aging4Amount = record.OutstandingAmount;
        //            list.Add(record);
        //        }
        //    }

        //    return list;
        //}
    }
}
