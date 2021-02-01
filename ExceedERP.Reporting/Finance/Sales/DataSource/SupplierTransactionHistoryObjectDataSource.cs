using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
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
    public class SupplierTransactionHistoryObjectDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SupplierTransactionHistoryViewModel> GetSupplierTransaction(string fiscalYear, string periodFrom, string periodTo)
        {
            List<SupplierTransactionHistoryViewModel> supplierTransactionList = new List<SupplierTransactionHistoryViewModel>();


            if (!String.IsNullOrEmpty(periodFrom) && !String.IsNullOrEmpty(periodTo))
            {
                int from = int.Parse(periodFrom);
                int to = int.Parse(periodTo);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;

                #region supplier invoices

                var invoices = _db.APPaybleInvoices
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Where(x => x.InvoiceDate >= dt11 && x.InvoiceDate <= dt22).ToList();

                var distinctInvoiceCustomers = invoices.DistinctByField(x => x.SupplierName).ToList();
                foreach (var paybleInvoices in distinctInvoiceCustomers)
                {
                    var inv = new SupplierTransactionHistoryViewModel
                    {
                        SupplierTransactionLists = new List<SupplierTransactionListsViewModel>(),
                        SupplierName = paybleInvoices.SupplierName,
                        InvoiceNo = paybleInvoices.ApPaybleInvoicesId
                    };

                    var invoiceDetails = invoices.Where(x => x.SupplierName == paybleInvoices.SupplierName).ToList();

                    foreach (var invoiceDetail in invoiceDetails)
                    {
                        var transaction = new SupplierTransactionListsViewModel
                        {
                            InvoiceNo=invoiceDetail.InvoiceNumber,
                            Transaction = "Invoice",
                            TransNo = invoiceDetail.ApPaybleInvoicesId,
                            Date = invoiceDetail.InvoiceDate,
                            Amount = invoiceDetail.AmountDue,
                            Reference=invoiceDetail.Remark
                        };

                        inv.SupplierTransactionLists.Add(transaction);
                    }

                    supplierTransactionList.Add(inv);

                }

                #endregion
                #region payment

                var payments = _db.APPayments
                    .Where(x=>x.IsOnlineApproved&&x.IsVoid==false)
                    .Where(x => x.PaymentDate >= dt11 && x.PaymentDate <= dt22).ToList();

                var distinctSupplierPayment = payments.DistinctByField(x => x.SupplierName).ToList();
                foreach (var payment in distinctSupplierPayment)
                {
                    var inv = new SupplierTransactionHistoryViewModel
                    {
                        SupplierTransactionLists = new List<SupplierTransactionListsViewModel>(),
                        SupplierName = payment.SupplierName,
                        InvoiceNo = payment.ApPaymentId
                    };


                    var apPayments = payments.Where(x => x.SupplierName == payment.SupplierName).ToList();
                    foreach (var apPayment in apPayments)
                    {
                        var transaction = new SupplierTransactionListsViewModel
                        {
                            InvoiceNo=apPayment.InvoiceNumber,
                            Transaction = "Payment",
                            TransNo = apPayment.ApPaymentId,
                            Date = apPayment.PaymentDate,
                            Amount = apPayment.InvoiceAmount,
                            Reference=apPayment.PaymentReference
                        };

                        inv.SupplierTransactionLists.Add(transaction);

                    }
                    supplierTransactionList.Add(inv);

                }

                #endregion
                #region direct payment

                var directPayments = _db.ApDirectPayments
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Where(x => x.PaymentDate >= dt11 && x.PaymentDate <= dt22).ToList();

                var distinctDirectPayment = directPayments.DistinctByField(x => x.SupplierName).ToList();
                foreach (var payment in distinctDirectPayment)
                {
                    var inv = new SupplierTransactionHistoryViewModel
                    {
                        SupplierTransactionLists = new List<SupplierTransactionListsViewModel>(),
                        SupplierName = payment.SupplierName,
                        InvoiceNo = payment.ApDirectPaymentId
                    };


                    var apdirectPayments = directPayments.Where(x => x.SupplierName == payment.SupplierName).ToList();
                    foreach (var apPayment in apdirectPayments)
                    {
                        var transaction = new SupplierTransactionListsViewModel
                        {
                            InvoiceNo = "",
                            Transaction = "Direct Payment",
                            TransNo = apPayment.ApDirectPaymentId,
                            Date = apPayment.PaymentDate,
                            Amount = apPayment.ApDirectPaymentLines.Sum(x=>x.NetPayable),
                            Reference = apPayment.PaymentReference
                        };

                        inv.SupplierTransactionLists.Add(transaction);

                    }
                    supplierTransactionList.Add(inv);

                }

                #endregion
                #region misc payment

                var miscelaneousCollections = _db.APMiscellaneousPayments
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Where(x => x.PaymentDate >= dt11 && x.PaymentDate <= dt22).ToList();

                var distinctCustomerMiscPayments = miscelaneousCollections.DistinctByField(x => x.SupplierName).ToList();
                foreach (var miscelaneous in distinctCustomerMiscPayments)
                {
                    var inv = new SupplierTransactionHistoryViewModel
                    {
                        SupplierTransactionLists = new List<SupplierTransactionListsViewModel>(),
                        SupplierName = miscelaneous.SupplierName,
                        InvoiceNo = miscelaneous.ApMiscellaneousPaymentId
                    };


                    var mescelaneousDetails = miscelaneousCollections.Where(x => x.SupplierName == miscelaneous.SupplierName).ToList();
                    foreach (var mescel in mescelaneousDetails)
                    {
                        var transaction = new SupplierTransactionListsViewModel
                        {
                            Transaction = "Miscelaneous Payment",
                            TransNo = mescel.ApMiscellaneousPaymentId,
                            Date = mescel.PaymentDate,
                            Amount = mescel.ApMiscellaneousPaymentLines.Sum(x=>x.Amount),
                            Reference = mescel.PaymentReference
                        };

                        inv.SupplierTransactionLists.Add(transaction);

                    }
                    supplierTransactionList.Add(inv);

                }

                #endregion

            }



            return supplierTransactionList;
        }
    }
}
