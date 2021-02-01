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
    public class SalesJournalOds : List<SalesJournalViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalesJournalViewModel> GetSaleJournal (string fiscalYear,string periodfrom,string periodto,string customer,string branchid,string account)
        {
            List<SalesJournalViewModel> saleJournalLists = new List<SalesJournalViewModel>();

            if (!String.IsNullOrEmpty(periodfrom) && !String.IsNullOrEmpty(periodto))
            {
                int from = int.Parse(periodfrom);
                int to = int.Parse(periodto);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom.Date;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo.Date;

                var arInvoiceDistributions = _db.ARInvoiceDistributions
                    .Where(x => x.ArInvoice.Date >= dt11 && x.ArInvoice.Date <= dt22)
                    .Where(x => x.ArInvoice.ArSalesOrderId != 0)
                    .Where(x => x.ArInvoice.IsVoid == false)
                    .Where(x => x.ArInvoice.IsOnlineApproved).ToList();

                var arStandardCollectionDistributions = _db.ARStandardCollectionDistributions
                    .Where(x => x.ArStandardCollection.ReceiptDate >= dt11 && x.ArStandardCollection.ReceiptDate <= dt22)
                    .Where(x => x.ArStandardCollection.IsVoid == false)
                    .Where(x => x.ArStandardCollection.IsOnlineApproved).ToList();

                var arMiscelaneousCollectionDistributions = _db.ARMiscelaneousCollectionDistributions
                    .Where(x => x.ARMiscelaneousCollection.ReceiptDate >= dt11 && x.ARMiscelaneousCollection.ReceiptDate <= dt22)
                    .Where(x => x.ARMiscelaneousCollection.IsVoid == false)
                    .Where(x => x.ARMiscelaneousCollection.IsOnlineApproved).ToList();

                //List<int> standardcollectionIds = new List<int>();
                var salesOrders = _db.ArSalesOrders.AsQueryable();

               

                if (!String.IsNullOrEmpty(branchid))
                {
                    //salesOrders = salesOrders.Where(x => x.BranchId.ToString() == branchid).AsQueryable();
                    arInvoiceDistributions = arInvoiceDistributions.Where(x => x.ArInvoice.BranchId.ToString()==branchid).ToList();
                    arMiscelaneousCollectionDistributions = arMiscelaneousCollectionDistributions.Where(x => x.ARMiscelaneousCollection.BranchId.ToString() == branchid).ToList();

                    //misc
                    //var branch = _db.Branch.Find(int.Parse(branchid));
                    //string str = "-" + branch?.LocationAccount + "-";
                    //arInvoiceDistributions = arInvoiceDistributions.Where(x => x.Description.Contains(str)).ToList();
                    //arStandardCollectionDistributions = arStandardCollectionDistributions.Where(x => x.Description.Contains(str)).ToList();
                    //arMiscelaneousCollectionDistributions = arMiscelaneousCollectionDistributions.Where(x => x.Description.Contains(str)).ToList();

                }

                var salesOrderIds = salesOrders.Select(x => x.ArSalesOrderId).ToList();

                var arInvoiceIds = _db.ARInvoices.Where(x => salesOrderIds.Contains(x.ArSalesOrderId)).Select(x => x.ArInvoiceId).ToList();
                arInvoiceDistributions = arInvoiceDistributions.Where(x => arInvoiceIds.Contains(x.ArInvoiceId)).ToList();

                //foreach (var orderid in salesOrderIds)
                //{
                //    var collectionId = standardcollections.FirstOrDefault(x => x.InvoiceNumber == "SALESORDER_" + orderid || x.InvoiceNumber.StartsWith("SALESORDER_" + orderid + "_"));
                //    if (collectionId != null)
                //        standardcollectionIds.Add(collectionId.ArStandardCollectionId);
                //}

               

                if (customer != null)
                {
                    arInvoiceDistributions = arInvoiceDistributions.Where(x => x.ArInvoice.CustomerName == customer).ToList();
                    arStandardCollectionDistributions= arStandardCollectionDistributions.Where(x => x.ArStandardCollection.CustomerName == customer).ToList();
                    arMiscelaneousCollectionDistributions = arMiscelaneousCollectionDistributions.Where(x => x.ARMiscelaneousCollection.CustomerName == customer).ToList();
                }

                if (!String.IsNullOrEmpty(account))
                {
                    string str = account + "-";
                    arInvoiceDistributions = arInvoiceDistributions.Where(x => x.AccountCode.StartsWith(str)).ToList();
                    arStandardCollectionDistributions = arStandardCollectionDistributions.Where(x => x.AccountCode.StartsWith(str)).ToList();
                    arMiscelaneousCollectionDistributions = arMiscelaneousCollectionDistributions.Where(x => x.AccountCode.StartsWith(str)).ToList();
                }

                #region invoice

                foreach (var distribution in arInvoiceDistributions)
                {
                    var sale = new SalesJournalViewModel
                    {
                        TransactId= "INV-" + distribution.ArInvoiceId,
                        AccountCode = distribution.AccountCode,
                        AcctDescription = distribution.AccountDesc
                    };
                    var salesOrder = _db.ArSalesOrders.Find(distribution.ArInvoice.ArSalesOrderId);
                    var partner = _db.Partners.FirstOrDefault(x => x.PartnerId == salesOrder.PartnerId);
                    if (partner != null)
                    {
                        var partnerType = _db.PartnerTypes.FirstOrDefault(x => x.PartnerTypeId == partner.PartnerTypeId);
                        if (partnerType != null)
                        {
                            sale.PartnerType = partnerType.TypeName;
                        }

                    }

                    var saleOrderLine = _db.ArSalesOrderLines.FirstOrDefault(x => x.ArSalesOrderId == salesOrder.ArSalesOrderId);

                    if (saleOrderLine != null)
                    {
                        var productItem = _db.ArProductService.Find(int.Parse(saleOrderLine.SubItem));
                        if (productItem != null) sale.ItemDescription = productItem.ItemName;
                    }


                    var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == salesOrder.SalesPersonId);
                    if (employee != null)
                    {
                        sale.SalesPerson = employee.FullNameaEnglish;
                    }

                    sale.Date = distribution.ArInvoice.Date;
                    sale.CustomerName = distribution.ArInvoice.CustomerName;
                    sale.SaleOrderNo = distribution.ArInvoice.ArSalesOrderId.ToString();
                    sale.ReceiptNo = "0"; // saleOrder.ArSalesOrderId;
                    sale.DebitAmount = distribution.Debit;
                    sale.CreditAmount = distribution.Credit;
                    sale.TransactionRef = distribution.ArInvoice.Reference;

                    saleJournalLists.Add(sale);
                }

                #endregion
                #region standard 

                foreach (var distribution in arStandardCollectionDistributions)
                {
                    var sale = new SalesJournalViewModel
                    {
                        TransactId = "STC-" + distribution.ArStandardCollectionId,
                        AccountCode = distribution.AccountCode,
                        AcctDescription = distribution.AccountDesc,
                        Date = distribution.ArStandardCollection.ReceiptDate,
                        CustomerName = distribution.ArStandardCollection.CustomerName,
                        SaleOrderNo = "",
                        ReceiptNo = distribution.ArStandardCollection.ReceiptNumber,
                        DebitAmount = distribution.Debit,
                        CreditAmount = distribution.Credit,
                        TransactionRef = distribution.ArStandardCollection.Reference
                    };

                    var collectionline = _db.ARStandardCollectionLines
                        .Where(x => x.ArStandardCollectionId == distribution.ArStandardCollectionId)
                        .FirstOrDefault(x => x.InvoiceNumber.StartsWith("SALESORDER_"));

                    if (collectionline != null)
                    {
                        int salesorderid = int.Parse(collectionline.InvoiceNumber.Split('_')[1]);
                        var salesOrder = _db.ArSalesOrders.Find(salesorderid);

                        if (salesOrder!=null)
                        {
                            sale.SaleOrderNo = salesOrder.ArSalesOrderId.ToString();
                            var partner = _db.Partners.FirstOrDefault(x => x.PartnerId == salesOrder.PartnerId);
                            if (partner != null)
                            {
                                var partnerType =
                                    _db.PartnerTypes.FirstOrDefault(x => x.PartnerTypeId == partner.PartnerTypeId);
                                if (partnerType != null)
                                {
                                    sale.PartnerType = partnerType.TypeName;
                                }

                            }

                            var saleOrderLine =
                                _db.ArSalesOrderLines.FirstOrDefault(x =>
                                    x.ArSalesOrderId == salesOrder.ArSalesOrderId);

                            if (saleOrderLine != null)
                            {
                                var productItem = _db.ArProductService.Find(int.Parse(saleOrderLine.SubItem));
                                if (productItem != null) sale.ItemDescription = productItem.ItemName;
                            }


                            var employee =
                                _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == salesOrder.SalesPersonId);
                            if (employee != null)
                            {
                                sale.SalesPerson = employee.FullNameaEnglish;
                            }

                            if (!String.IsNullOrEmpty(branchid))
                            {
                                if (salesOrder.BranchId.ToString() == branchid)
                                    saleJournalLists.Add(sale);
                            }
                            else
                            {
                                saleJournalLists.Add(sale);
                            }
                        }
                    }
                    else
                    {
                        var collectionlineFromInvoice = _db.ARStandardCollectionLines
                            .Where(x => x.ArStandardCollectionId == distribution.ArStandardCollectionId)
                            .FirstOrDefault(x => x.InvoiceNumber.StartsWith("INVOICE_"));

                        if (collectionlineFromInvoice != null)
                        {
                            int invoiceid = int.Parse(collectionlineFromInvoice.InvoiceNumber.Split('_')[1]);
                            var arInvoice = _db.ARInvoices.Find(invoiceid);

                            if (arInvoice != null)
                            {
                                var salesOrder = _db.ArSalesOrders.Find(arInvoice.ArSalesOrderId);
                                if (salesOrder != null)
                                {
                                    sale.SaleOrderNo = salesOrder?.ArSalesOrderId.ToString();
                                    var partner = _db.Partners.FirstOrDefault(x => x.PartnerId == salesOrder.PartnerId);
                                    if (partner != null)
                                    {
                                        var partnerType =
                                            _db.PartnerTypes.FirstOrDefault(x =>
                                                x.PartnerTypeId == partner.PartnerTypeId);
                                        if (partnerType != null)
                                        {
                                            sale.PartnerType = partnerType.TypeName;
                                        }

                                    }

                                    var saleOrderLine =
                                        _db.ArSalesOrderLines.FirstOrDefault(x =>
                                            x.ArSalesOrderId == salesOrder.ArSalesOrderId);

                                    if (saleOrderLine != null)
                                    {
                                        var productItem = _db.ArProductService.Find(int.Parse(saleOrderLine.SubItem));
                                        if (productItem != null) sale.ItemDescription = productItem.ItemName;
                                    }


                                    var employee =
                                        _db.Person_Employee.FirstOrDefault(
                                            x => x.EmployeeId == salesOrder.SalesPersonId);
                                    if (employee != null)
                                    {
                                        sale.SalesPerson = employee.FullNameaEnglish;
                                    }

                                    if (!String.IsNullOrEmpty(branchid))
                                    {
                                        if (salesOrder.BranchId.ToString() == branchid)
                                            saleJournalLists.Add(sale);
                                    }
                                    else
                                    {
                                        saleJournalLists.Add(sale);
                                    }
                                }

                               
                            }

                        }
                        
                    }

                   
                }

                #endregion
                #region misc 

                foreach (var distribution in arMiscelaneousCollectionDistributions)
                {
                    var sale = new SalesJournalViewModel
                    {
                        TransactId = "MISC-"+distribution.ARMiscelaneousCollectionID,
                        AccountCode = distribution.AccountCode,
                        AcctDescription = distribution.AccountDesc,
                        Date = distribution.ARMiscelaneousCollection.ReceiptDate,
                        CustomerName = distribution.ARMiscelaneousCollection.CustomerName,
                        SaleOrderNo = "",
                        ReceiptNo = distribution.ARMiscelaneousCollection.ReceiptNumber,
                        DebitAmount = distribution.Debit,
                        CreditAmount = distribution.Credit,
                        TransactionRef = distribution.ARMiscelaneousCollection.Remark
                    };
                    

                    saleJournalLists.Add(sale);
                }

                #endregion
            }

            return saleJournalLists;
        }

        public List<SalesJournalViewModel> GetSaleJournal1(string fiscalYear, string periodfrom, string periodto, string customer, string branchid, string account)
        {
            List<SalesJournalViewModel> saleJournalLists = new List<SalesJournalViewModel>();

            if (!String.IsNullOrEmpty(periodfrom) && !String.IsNullOrEmpty(periodto))
            {
                int from = int.Parse(periodfrom);
                int to = int.Parse(periodto);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom.Date;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo.Date;

                var arInvoiceDistributions = _db.ARInvoiceDistributions
                    .Where(x => x.ArInvoice.Date >= dt11 && x.ArInvoice.Date <= dt22)
                    .Where(x => x.ArInvoice.ArSalesOrderId != 0)
                    .Where(x => x.ArInvoice.IsVoid == false)
                    .Where(x => x.ArInvoice.IsOnlineApproved).ToList();

                var standardcollections = _db.ARStandardCollectionLines
                    .Where(x => x.ArStandardCollection.ReceiptDate >= dt11 && x.ArStandardCollection.ReceiptDate <= dt22)
                    .Where(x => x.ArStandardCollection.IsVoid == false)
                    .Where(x => x.ArStandardCollection.IsOnlineApproved)
                    .Where(x => x.InvoiceNumber.StartsWith("SALESORDER_")).ToList();

                var arMiscelaneousCollectionDistributions = _db.ARMiscelaneousCollectionDistributions
                    .Where(x => x.ARMiscelaneousCollection.ReceiptDate >= dt11 && x.ARMiscelaneousCollection.ReceiptDate <= dt22)
                    .Where(x => x.ARMiscelaneousCollection.IsVoid == false)
                    .Where(x => x.ARMiscelaneousCollection.IsOnlineApproved).ToList();

                List<int> standardcollectionIds = new List<int>();
                var salesOrders = _db.ArSalesOrders.AsQueryable();

                if (!String.IsNullOrEmpty(branchid))
                {
                    salesOrders = salesOrders.Where(x => x.BranchId.ToString() == branchid).AsQueryable();

                    //misc
                    var branch = _db.Branch.Find(int.Parse(branchid));
                    string str = "-" + branch?.LocationAccount + "-";
                    arMiscelaneousCollectionDistributions = arMiscelaneousCollectionDistributions.Where(x => x.AccountCode.Contains(str)).ToList();

                }

                var salesOrderIds = salesOrders.Select(x => x.ArSalesOrderId).ToList();

                var arInvoiceIds = _db.ARInvoices.Where(x => salesOrderIds.Contains(x.ArSalesOrderId)).Select(x => x.ArInvoiceId).ToList();
                arInvoiceDistributions = arInvoiceDistributions.Where(x => arInvoiceIds.Contains(x.ArInvoiceId)).ToList();

                foreach (var orderid in salesOrderIds)
                {
                    var collectionId = standardcollections.FirstOrDefault(x => x.InvoiceNumber == "SALESORDER_" + orderid || x.InvoiceNumber.StartsWith("SALESORDER_" + orderid + "_"));
                    if (collectionId != null)
                        standardcollectionIds.Add(collectionId.ArStandardCollectionId);
                }

                var arStandardCollectionDistributions = _db.ARStandardCollectionDistributions
                    .Where(x => standardcollectionIds.Contains(x.ArStandardCollectionId)).ToList();

                if (customer != null)
                {
                    arInvoiceDistributions = arInvoiceDistributions.Where(x => x.ArInvoice.CustomerName == customer).ToList();
                    arStandardCollectionDistributions = arStandardCollectionDistributions.Where(x => x.ArStandardCollection.CustomerName == customer).ToList();
                }

                if (!String.IsNullOrEmpty(account))
                {
                    string str = account + "-";
                    arInvoiceDistributions = arInvoiceDistributions.Where(x => x.AccountCode.StartsWith(str)).ToList();
                    arStandardCollectionDistributions = arStandardCollectionDistributions.Where(x => x.AccountCode.StartsWith(str)).ToList();
                    arMiscelaneousCollectionDistributions = arMiscelaneousCollectionDistributions.Where(x => x.AccountCode.StartsWith(str)).ToList();
                }

                #region invoice

                foreach (var distribution in arInvoiceDistributions)
                {
                    var sale = new SalesJournalViewModel
                    {
                        AccountCode = distribution.AccountCode,
                        AcctDescription = distribution.AccountDesc
                    };
                    var salesOrder = _db.ArSalesOrders.Find(distribution.ArInvoice.ArSalesOrderId);
                    var partner = _db.Partners.FirstOrDefault(x => x.PartnerId == salesOrder.PartnerId);
                    if (partner != null)
                    {
                        var partnerType = _db.PartnerTypes.FirstOrDefault(x => x.PartnerTypeId == partner.PartnerTypeId);
                        if (partnerType != null)
                        {
                            sale.PartnerType = partnerType.TypeName;
                        }

                    }

                    var saleOrderLine = _db.ArSalesOrderLines.FirstOrDefault(x => x.ArSalesOrderId == salesOrder.ArSalesOrderId);

                    if (saleOrderLine != null)
                    {
                        var productItem = _db.ArProductService.Find(int.Parse(saleOrderLine.SubItem));
                        if (productItem != null) sale.ItemDescription = productItem.ItemName;
                    }


                    var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == salesOrder.SalesPersonId);
                    if (employee != null)
                    {
                        sale.SalesPerson = employee.FullNameaEnglish;
                    }

                    sale.Date = distribution.ArInvoice.Date;
                    sale.CustomerName = distribution.ArInvoice.CustomerName;
                    sale.SaleOrderNo = distribution.ArInvoice.ArSalesOrderId.ToString();
                    sale.ReceiptNo = "0"; // saleOrder.ArSalesOrderId;
                    sale.DebitAmount = distribution.Debit;
                    sale.CreditAmount = distribution.Credit;
                    sale.TransactionRef = distribution.ArInvoice.Reference;


                    saleJournalLists.Add(sale);
                }

                #endregion
                #region standard 

                foreach (var distribution in arStandardCollectionDistributions)
                {
                    var sale = new SalesJournalViewModel
                    {
                        AccountCode = distribution.AccountCode,
                        AcctDescription = distribution.AccountDesc,
                        Date = distribution.ArStandardCollection.ReceiptDate,
                        CustomerName = distribution.ArStandardCollection.CustomerName,
                        SaleOrderNo = "",
                        ReceiptNo = distribution.ArStandardCollection.ReceiptNumber,
                        DebitAmount = distribution.Debit,
                        CreditAmount = distribution.Credit,
                        TransactionRef = distribution.ArStandardCollection.Reference
                    };
                    //var salesOrder = _db.ArSalesOrders.Find(distribution.ArInvoice.ArSalesOrderId);
                    //var partner = _db.Partners.FirstOrDefault(x => x.PartnerId == salesOrder.PartnerId);
                    //if (partner != null)
                    //{
                    //    var partnerType = _db.PartnerTypes.FirstOrDefault(x => x.PartnerTypeId == partner.PartnerTypeId);
                    //    if (partnerType != null)
                    //    {
                    //        sale.PartnerType = partnerType.TypeName;
                    //    }

                    //}

                    //var saleOrderLine = _db.ArSalesOrderLines.FirstOrDefault(x => x.ArSalesOrderId == salesOrder.ArSalesOrderId);

                    //if (saleOrderLine != null)
                    //{
                    //    var productItem = _db.ArProductService.Find(int.Parse(saleOrderLine.SubItem));
                    //    if (productItem != null) sale.ItemDescription = productItem.ItemName;
                    //}


                    //var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == salesOrder.SalesPersonId);
                    //if (employee != null)
                    //{
                    //    sale.SalesPerson = employee.FullNameaEnglish;
                    //}



                    saleJournalLists.Add(sale);
                }

                #endregion
                #region misc 

                foreach (var distribution in arMiscelaneousCollectionDistributions)
                {
                    var sale = new SalesJournalViewModel
                    {
                        AccountCode = distribution.AccountCode,
                        AcctDescription = distribution.AccountDesc,
                        Date = distribution.ARMiscelaneousCollection.ReceiptDate,
                        CustomerName = distribution.ARMiscelaneousCollection.CustomerName,
                        SaleOrderNo = "",
                        ReceiptNo = distribution.ARMiscelaneousCollection.ReceiptNumber,
                        DebitAmount = distribution.Debit,
                        CreditAmount = distribution.Credit,
                        TransactionRef = distribution.ARMiscelaneousCollection.Remark
                    };


                    saleJournalLists.Add(sale);
                }

                #endregion
            }

            return saleJournalLists;
        }
    }
}
