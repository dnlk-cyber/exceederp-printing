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
    public class CustomerTransactionHistoryDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CustomerTransactionHistoryViewModel> GetCustomerTransaction(string fiscalYear, string periodFrom, string periodTo, string customer,object[] account)
        {
            List<CustomerTransactionHistoryViewModel> customerTransactionList = new List<CustomerTransactionHistoryViewModel>();

            if (!String.IsNullOrEmpty(periodFrom)&& !String.IsNullOrEmpty(periodTo))
            {
                int from = int.Parse(periodFrom);
                int to = int.Parse(periodTo);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;

               

                #region ar invoice

                var invoices = _db.ARInvoices
                    .Where(x => x.IsVoid == false)
                    .Where(x => x.IsOnlineApproved)
                    .Where(x => x.Date >= dt11 && x.Date <= dt22)
                    .ToList();

                var distinctInvoiceCustomers = invoices.DistinctByField(x => x.CustomerName).ToList();

                if (!String.IsNullOrEmpty(customer))
                {
                    distinctInvoiceCustomers = distinctInvoiceCustomers.Where(x => x.CustomerName == customer).ToList();
                }

                foreach (var customerDistinct in distinctInvoiceCustomers)
                {
                    var inv = new CustomerTransactionHistoryViewModel
                    {
                        CustomerTransactionLists = new List<CustomerTransactionLists>(),
                        CustomerName = customerDistinct.CustomerName,
                        InvoiceNo = customerDistinct.ArInvoiceId
                    };

                    var invoiceDetails = invoices.Where(x => x.CustomerName == customerDistinct.CustomerName).ToList();

                    foreach (var invoiceDetail in invoiceDetails)
                    {
                        var transaction = new CustomerTransactionLists
                        {
                            InvoiceNo=invoiceDetail.ArInvoiceId.ToString(),
                            Transaction = "Invoice",
                            TransNo = invoiceDetail.ArInvoiceId,
                            Date = invoiceDetail.Date,
                            Amount = invoiceDetail.TotalAmount,
                            Reference=invoiceDetail.Reference
                        };

                        inv.CustomerTransactionLists.Add(transaction);
                    }

                    customerTransactionList.Add(inv);

                }

                #endregion
                #region standard collection

                var standardCollections = _db.ARStandardCollections
                    .Where(x => x.IsVoid == false)
                    .Where(x => x.IsOnlineApproved)
                    .Where(x => x.ReceiptDate >= dt11 && x.ReceiptDate <= dt22).ToList();

                var distinctCustomerReceipts = standardCollections.DistinctByField(x => x.CustomerName).ToList();

                if (!String.IsNullOrEmpty(customer))
                {
                    distinctCustomerReceipts = distinctCustomerReceipts.Where(x => x.CustomerName == customer).ToList();
                }
                foreach (var customerDistinct in distinctCustomerReceipts)
                {
                    var inv = new CustomerTransactionHistoryViewModel
                    {
                        CustomerTransactionLists = new List<CustomerTransactionLists>(),
                        CustomerName = customerDistinct.CustomerName,
                        InvoiceNo = customerDistinct.ArStandardCollectionId
                    };

                    var receiptDetails = standardCollections.Where(x => x.CustomerName == customerDistinct.CustomerName).ToList();
                    foreach (var receiptDetail in receiptDetails)
                    {
                        var transaction = new CustomerTransactionLists
                        {
                            
                            Transaction = "Receipt",
                            TransNo = receiptDetail.ArStandardCollectionId,
                            Date = receiptDetail.ReceiptDate,
                            Amount = receiptDetail.ARStandardCollectionLines.Sum(x=>x.NetAmount),
                            Reference = receiptDetail.Reference
                        };

                        inv.CustomerTransactionLists.Add(transaction);

                    }
                    customerTransactionList.Add(inv);

                }

                #endregion
                #region misc  collection

                var miscelaneousCollections = _db.ARMiscelaneousCollections
                    .Where(x => x.IsVoid == false)
                    .Where(x => x.IsOnlineApproved)
                    .Where(x => x.ReceiptDate >= dt11 && x.ReceiptDate <= dt22).ToList();

                var distinctCustomerMiscReceipts = miscelaneousCollections.DistinctByField(x => x.CustomerName).ToList();
                if (!String.IsNullOrEmpty(customer))
                {
                    distinctCustomerMiscReceipts = distinctCustomerMiscReceipts.Where(x => x.CustomerName == customer).ToList();
                }
                foreach (var miscelaneous in distinctCustomerMiscReceipts)
                {
                    var inv = new CustomerTransactionHistoryViewModel
                    {
                        CustomerTransactionLists = new List<CustomerTransactionLists>(),
                        CustomerName = miscelaneous.CustomerName,
                        InvoiceNo = miscelaneous.ARMiscelaneousCollectionID
                    };


                    var mescelaneousDetails = miscelaneousCollections.Where(x => x.CustomerId == miscelaneous.CustomerId).ToList();
                    foreach (var mescel in mescelaneousDetails)
                    {
                        var transaction = new CustomerTransactionLists
                        {
                            
                            Transaction = "Misc. Receipt",
                            TransNo = mescel.ARMiscelaneousCollectionID,
                            Date = mescel.ReceiptDate,
                            Amount = mescel.ArMiscelaneousCollectionLines.Sum(x=>x.NetReceipt),
                            Reference = mescel.Remark
                        };

                        inv.CustomerTransactionLists.Add(transaction);

                    }
                    customerTransactionList.Add(inv);

                }

                #endregion

            }

            return customerTransactionList;
        }
    }
}
