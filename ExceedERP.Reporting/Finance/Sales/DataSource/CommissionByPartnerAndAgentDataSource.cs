using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.Sales.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;

namespace ExceedERP.Reporting.Finance.Sales.DataSource
{

    [DataObject]
    public class CommissionByPartnerAndAgentDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public IOrderedEnumerable<CommissionByPartnerAndAgentViewModel> GetCommistion(string fiscalYear, string periodFrom, string periodTo, string orderType, string partner,string account)
        {
            List<CommissionByPartnerAndAgentViewModel> commisionList = new List<CommissionByPartnerAndAgentViewModel>();

            if (!String.IsNullOrEmpty(periodFrom) && !String.IsNullOrEmpty(periodTo))
            {
                int from = int.Parse(periodFrom);
                int to = int.Parse(periodTo);
                DateTime dt11 = _db.GLPeriods.Find(from).DateFrom;
                DateTime dt22 = _db.GLPeriods.Find(to).DateTo;

                var arInvoiceDistributions = _db.ARInvoiceDistributions
                    .Where(x => x.ArInvoice.Date >= dt11 && x.ArInvoice.Date <= dt22)
                    .Where(x => x.ArInvoice.ArSalesOrderId != 0)
                    .Where(x => x.ArInvoice.IsVoid == false)
                    .Where(x => x.ArInvoice.IsOnlineApproved).AsQueryable();

                var arMiscelaneousCollectionDistributions = _db.ARMiscelaneousCollectionDistributions
                    .Where(x => x.ARMiscelaneousCollection.ReceiptDate >= dt11 && x.ARMiscelaneousCollection.ReceiptDate <= dt22)
                    .Where(x => x.ARMiscelaneousCollection.IsVoid == false)
                    .Where(x => x.ARMiscelaneousCollection.IsOnlineApproved).AsQueryable();

                var standardcollections = _db.ARStandardCollectionLines
                    .Where(x => x.ArStandardCollection.ReceiptDate >= dt11 && x.ArStandardCollection.ReceiptDate <= dt22)
                    .Where(x => x.ArStandardCollection.IsVoid == false)
                    .Where(x => x.ArStandardCollection.IsOnlineApproved)
                    //.Where(x => x.InvoiceNumber.StartsWith("SALESORDER_")) //to include all
                    .AsQueryable();

                var apmiscDistribution = _db.APMiscellaneousPaymentDistributions
                    .Where(x => x.ApMiscellaneousPayment.PaymentDate >= dt11 && x.ApMiscellaneousPayment.PaymentDate <= dt22)
                    .Where(x => x.ApMiscellaneousPayment.IsVoid == false)
                    .Where(x => x.ApMiscellaneousPayment.IsOnlineApproved).AsQueryable();

                List<int> standardcollectionIds = new List<int>();
                var salesOrders = _db.ArSalesOrders.AsQueryable();
               

                var salesOrderIds = salesOrders.Select(x => x.ArSalesOrderId).ToList();

                var arInvoiceIds = _db.ARInvoices.Where(x => salesOrderIds.Contains(x.ArSalesOrderId)).Select(x => x.ArInvoiceId).ToList();
                arInvoiceDistributions = arInvoiceDistributions.Where(x => arInvoiceIds.Contains(x.ArInvoiceId)).AsQueryable();

                foreach (var orderid in salesOrderIds)
                {
                    var collectionId = standardcollections.FirstOrDefault(x => x.InvoiceNumber == "SALESORDER_" + orderid || x.InvoiceNumber.StartsWith("SALESORDER_" + orderid + "_"));
                    if (collectionId != null)
                        standardcollectionIds.Add(collectionId.ArStandardCollectionId);
                }

                var arStandardCollectionDistributions = _db.ARStandardCollectionDistributions
                    .Where(x => standardcollectionIds.Contains(x.ArStandardCollectionId)).AsQueryable();



                if (!String.IsNullOrEmpty(account))
                {
                    string str = account + "-";
                    arInvoiceDistributions = arInvoiceDistributions.Where(x => x.AccountCode.StartsWith(str)).AsQueryable();
                    arStandardCollectionDistributions = arStandardCollectionDistributions.Where(x => x.AccountCode.StartsWith(str)).AsQueryable();
                    arMiscelaneousCollectionDistributions = arMiscelaneousCollectionDistributions.Where(x => x.AccountCode.StartsWith(str)).AsQueryable();
                    apmiscDistribution = apmiscDistribution.Where(x => x.AccountCode.StartsWith(str)).AsQueryable();                   
                }

                if (orderType != "Cash")
                {
                    #region invoice

                    foreach (var distribution in arInvoiceDistributions)
                    {
                        var commission = new CommissionByPartnerAndAgentViewModel();
                        commission.TransactId = "INV-" + distribution.ArInvoiceId;
                        var salesorder = _db.ArSalesOrders.Find(distribution.ArInvoice.ArSalesOrderId);
                        if (salesorder != null)
                        {
                            if (!String.IsNullOrEmpty(partner) && salesorder.PartnerId.ToString() != partner)
                                continue;

                            commission.SaleOrderNo = salesorder.ArSalesOrderId;
                            commission.OrderType = salesorder.OrderType.ToString();
                            var partnerObj = _db.Partners.FirstOrDefault(x => x.PartnerId == salesorder.PartnerId);
                            if (partnerObj != null)
                            {
                                var partnerType =
                                    _db.PartnerTypes.FirstOrDefault(x => x.PartnerTypeId == partnerObj.PartnerTypeId);
                                if (partnerType != null)
                                {
                                    commission.PartnerName = partnerObj.Name;
                                    commission.PartnerType = partnerType.TypeName;
                                }
                            }

                            var employee =
                                _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == salesorder.SalesPersonId);
                            if (employee != null)
                            {
                                commission.SalesPerson = employee.FullNameaEnglish;
                            }
                        }

                        commission.Customer = distribution.ArInvoice.CustomerName;
                        commission.Date = distribution.ArInvoice.Date;
                        commission.ReceiptNo = "";
                        commission.AccountCode = distribution.AccountCode;
                        commission.DebitAmount = distribution.Debit;
                        commission.CreditAmount = distribution.Credit;
                        commission.Balance = 0; // saleOrder.ba;
                        commission.TransactionRef = distribution.ArInvoice.Reference;
                        commission.InvoiceStatus = distribution.ArInvoice.InvoiceStatus.ToString();
                        commisionList.Add(commission);
                    }

                    #endregion
                }

                if (orderType != "Credit")
                {
                    #region standard 

                    foreach (var distribution in arStandardCollectionDistributions)
                    {
                        var commission = new CommissionByPartnerAndAgentViewModel();
                        commission.TransactId = "STC-" + distribution.ArStandardCollectionId;
                        var collectionline = _db.ARStandardCollectionLines
                            .Where(x => x.ArStandardCollectionId == distribution.ArStandardCollectionId)
                            .FirstOrDefault(x => x.InvoiceNumber.StartsWith("SALESORDER_"));
                        if (collectionline != null)
                        {
                            int salesorderid = int.Parse(collectionline.InvoiceNumber.Split('_')[1]);
                            var salesorder = _db.ArSalesOrders.Find(salesorderid);
                            if (salesorder != null)
                            {
                                if (!String.IsNullOrEmpty(partner) && salesorder.PartnerId.ToString() != partner)
                                    continue;

                                commission.SaleOrderNo = salesorder.ArSalesOrderId;
                                commission.OrderType = salesorder.OrderType.ToString();
                                var partnerObj = _db.Partners.FirstOrDefault(x => x.PartnerId == salesorder.PartnerId);
                                if (partnerObj != null)
                                {
                                    var partnerType =
                                        _db.PartnerTypes.FirstOrDefault(
                                            x => x.PartnerTypeId == partnerObj.PartnerTypeId);
                                    if (partnerType != null)
                                    {
                                        commission.PartnerName = partnerObj.Name;
                                        commission.PartnerType = partnerType.TypeName;
                                    }
                                }

                                var employee =
                                    _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == salesorder.SalesPersonId);
                                if (employee != null)
                                {
                                    commission.SalesPerson = employee.FullNameaEnglish;
                                }
                            }
                        }

                        commission.Customer = distribution.ArStandardCollection.CustomerName;
                        commission.Date = distribution.ArStandardCollection.ReceiptDate;
                        commission.ReceiptNo = distribution.ArStandardCollection.ReceiptNumber;
                        commission.AccountCode = distribution.AccountCode;
                        commission.DebitAmount = distribution.Debit;
                        commission.CreditAmount = distribution.Credit;
                        commission.Balance = 0; // saleOrder.ba;
                        commission.TransactionRef = distribution.ArStandardCollection.Reference;
                        commission.InvoiceStatus = "";
                        commisionList.Add(commission);
                    }

                    #endregion
                }

                if (orderType != "Credit" || String.IsNullOrEmpty(partner))
                {

                    #region misc 

                    foreach (var distribution in arMiscelaneousCollectionDistributions)
                    {
                        var commission = new CommissionByPartnerAndAgentViewModel
                        {
                            TransactId = "MISC-" + distribution.ARMiscelaneousCollectionID,
                            Customer = distribution.ARMiscelaneousCollection.CustomerName,
                            Date = distribution.ARMiscelaneousCollection.ReceiptDate,
                            ReceiptNo = distribution.ARMiscelaneousCollection.ReceiptNumber,
                            AccountCode = distribution.AccountCode,
                            DebitAmount = distribution.Debit,
                            CreditAmount = distribution.Credit,
                            Balance = 0,
                            TransactionRef = distribution.ARMiscelaneousCollection.Remark,
                            InvoiceStatus = ""
                        };


                        // saleOrder.ba;
                        commisionList.Add(commission);
                    }

                    #endregion
                }

                if (!String.IsNullOrEmpty(account))
                {
                    #region AP misc 

                    foreach (var distribution in apmiscDistribution)
                    {
                        var commission = new CommissionByPartnerAndAgentViewModel
                        {
                            TransactId = "MISP-" + distribution.ApMiscellaneousPaymentId,
                            Customer = distribution.ApMiscellaneousPayment.SupplierName,
                            Date = distribution.ApMiscellaneousPayment.PaymentDate,
                            ReceiptNo = distribution.ApMiscellaneousPayment.PaymentReference,
                            AccountCode = distribution.AccountCode,
                            DebitAmount = distribution.Debit,
                            CreditAmount = distribution.Credit,
                            Balance = 0,
                            TransactionRef = distribution.ApMiscellaneousPayment.Remark,
                            InvoiceStatus = ""
                        };


                        // saleOrder.ba;
                        commisionList.Add(commission);
                    }

                    #endregion
                }

            }

            return commisionList.OrderBy(x=>x.Date);
        }

        //public List<CommissionByPartnerAndAgentViewModel> GetCommistion1(string fiscalYear, string period, string orderType, string partner, string account)
        //{
        //    List<CommissionByPartnerAndAgentViewModel> commisionList = new List<CommissionByPartnerAndAgentViewModel>();

        //    if (!String.IsNullOrEmpty(period))
        //    {
        //        int per = int.Parse(period);
        //        DateTime dt11 = _db.GLPeriods.Find(per).DateFrom.Date;
        //        DateTime dt22 = _db.GLPeriods.Find(per).DateTo.Date;
        //        var saleOrders = _db.ArSalesOrders
        //            .Where(x => x.isVoid == false)
        //           .Where(x => x.IsOnlineApproved)
        //            .Where(x => x.OrderDate >= dt11 && x.OrderDate <= dt22);

        //        if (!String.IsNullOrEmpty(orderType))
        //        {
        //            if (orderType == "Cash")
        //                saleOrders = saleOrders.Where(x => x.OrderType == Core.Domain.Finance.AR.SalesOrder.OrderType.Cash);
        //            else
        //                saleOrders = saleOrders.Where(x => x.OrderType == Core.Domain.Finance.AR.SalesOrder.OrderType.Credit);
        //        }
        //        if (!String.IsNullOrEmpty(partner))
        //        {
        //            int pid = int.Parse(partner);
        //            saleOrders = saleOrders.Where(x => x.PartnerId == pid);
        //        }

        //        foreach (var saleOrder in saleOrders)
        //        {
        //            if (saleOrder.OrderType == OrderType.Cash)
        //            {
        //                #region cash

        //                var collectionline = _db.ARStandardCollectionLines
        //                    .Where(x => x.ArStandardCollection.IsOnlineApproved)
        //                    .Where(x => x.ArStandardCollection.IsVoid == false)
        //                    .FirstOrDefault(x => x.InvoiceNumber == "SALESORDER_" + saleOrder.ArSalesOrderId);

        //                if (collectionline != null)
        //                {
        //                    int standardcollectionId = collectionline.ArStandardCollectionId;
        //                    var distributions = _db.ARStandardCollectionDistributions
        //                        .Where(x => x.ArStandardCollectionId == standardcollectionId).ToList();

        //                    if (!String.IsNullOrEmpty(account))
        //                    {
        //                        string str = account + "-";
        //                        distributions = distributions.Where(x => x.Description.StartsWith(str)).ToList();
        //                    }

        //                    foreach (var distribution in distributions)
        //                    {
        //                        var commission = new CommissionByPartnerAndAgentViewModel();
        //                        var partnerObjs = _db.Partners.FirstOrDefault(x => x.PartnerId == saleOrder.PartnerId);
        //                        if (partnerObjs != null)
        //                        {
        //                            var partnerType = _db.PartnerTypes.FirstOrDefault(x => x.PartnerTypeId == partnerObjs.PartnerTypeId);
        //                            if (partnerType != null)
        //                            {
        //                                commission.PartnerName = partnerObjs.Name;
        //                                commission.PartnerType = partnerType.TypeName;
        //                            }
        //                        }
        //                        var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == saleOrder.SalesPersonId);
        //                        if (employee != null)
        //                        {
        //                            commission.SalesPerson = employee.FullNameaEnglish;
        //                        }

        //                        commission.Customer = saleOrder.CustomerName;
        //                        commission.Date = saleOrder.OrderDate;
        //                        commission.SaleOrderNo = saleOrder.ArSalesOrderId;
        //                        commission.ReceiptNo = distribution.ArStandardCollection.ReceiptNumber;
        //                        commission.Description = distribution.Description;
        //                        commission.DebitAmount = distribution.Debit;
        //                        commission.CreditAmount = distribution.Credit;
        //                        commission.Balance = 0;// saleOrder.ba;
        //                        commission.OrderType = saleOrder.OrderType.ToString();
        //                        commission.TransactionRef = saleOrder.OrderReference;
        //                        commisionList.Add(commission);
        //                    }
        //                }

        //                #endregion
        //            }

        //            else
        //            {
        //                #region credit

        //                var arinvoices = _db.ARInvoices
        //                    .Where(x => x.IsOnlineApproved)
        //                    .Where(x => x.IsVoid == false)
        //                    .Where(x => x.ArSalesOrderId == saleOrder.ArSalesOrderId).ToList();

        //                foreach (var arinvoice in arinvoices)
        //                {
        //                    var collectionline = _db.ARStandardCollectionLines
        //                        .Where(x => x.ArStandardCollection.IsOnlineApproved)
        //                        .Where(x => x.ArStandardCollection.IsVoid == false)
        //                        .FirstOrDefault(x => x.InvoiceNumber == "INVOICE_" + arinvoice.ArInvoiceId + "_" + arinvoice.Reference);

        //                    if (collectionline != null)
        //                    {
        //                        int standardcollectionId = collectionline.ArStandardCollectionId;
        //                        var distributions = _db.ARStandardCollectionDistributions
        //                            .Where(x => x.ArStandardCollectionId == standardcollectionId).ToList();

        //                        if (!String.IsNullOrEmpty(account))
        //                        {
        //                            string str = account + "-";
        //                            distributions = distributions.Where(x => x.Description.StartsWith(str)).ToList();
        //                        }

        //                        foreach (var distribution in distributions)
        //                        {
        //                            var commission = new CommissionByPartnerAndAgentViewModel();
        //                            var partnerObj = _db.Partners.FirstOrDefault(x => x.PartnerId == saleOrder.PartnerId);
        //                            if (partnerObj != null)
        //                            {
        //                                var partnerType = _db.PartnerTypes.FirstOrDefault(x => x.PartnerTypeId == partnerObj.PartnerTypeId);
        //                                if (partnerType != null)
        //                                {
        //                                    commission.PartnerName = partnerObj.Name;
        //                                    commission.PartnerType = partnerType.TypeName;
        //                                }
        //                            }
        //                            var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == saleOrder.SalesPersonId);
        //                            if (employee != null)
        //                            {
        //                                commission.SalesPerson = employee.FullNameaEnglish;
        //                            }

        //                            commission.Customer = saleOrder.CustomerName;
        //                            commission.Date = saleOrder.OrderDate;
        //                            commission.SaleOrderNo = saleOrder.ArSalesOrderId;
        //                            commission.ReceiptNo = distribution.ArStandardCollection.ReceiptNumber;
        //                            commission.Description = distribution.Description;
        //                            commission.DebitAmount = distribution.Debit;
        //                            commission.CreditAmount = distribution.Credit;
        //                            commission.Balance = 0;// saleOrder.ba;
        //                            commission.OrderType = saleOrder.OrderType.ToString();
        //                            commission.TransactionRef = saleOrder.OrderReference;
        //                            int maxid = arinvoices.Max(x => x.ArInvoiceId);
        //                            commission.InvoiceStatus = arinvoices.FirstOrDefault(x => x.ArInvoiceId == maxid)
        //                                ?.InvoiceStatus.ToString();
        //                            commisionList.Add(commission);
        //                        }
        //                    }
        //                }


        //                #endregion
        //            }

        //        }
        //    }

        //    return commisionList;
        //}
    }
}
