using System;
using System.Collections.Generic;
using ExceedERP.DataAccess.Context;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;


namespace ExceedERP.Web.Areas.Finance.Controllers
{
    public class FinanceDataProviderController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        AccountHelper _accountHelper = new AccountHelper();

        public JsonResult GetDirectPaymentForSettlement()
        {
            var invoices = _db.ApDirectPayments
                .Where(i => i.IsOnlineApproved)
                .Where(i => i.IsVoid==false)
                .AsQueryable();
            return Json(invoices.Select(i => new
            {
                Name = i.ApDirectPaymentId+"_"+i.SupplierName + "_" +i.PaymentReference,
                Code = i.ApDirectPaymentId

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriodDateRangeByName(string name)
        {

            var glPeriod = _db.GLPeriods.FirstOrDefault(x => x.Name == name);
            if (glPeriod != null)
            {
                var formattedData = new
                {
                    Min = glPeriod.DateFrom,
                    Max = glPeriod.DateTo
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);

            }

            return null;
        }
        public JsonResult GetSupplierInvoices(string supliercode)
        {
            var invoices = _db.APPaybleInvoices
                .Where(i => i.IsOnlineApproved)
                .Where(i => i.InvoiceStatus==InvoiceStatus.Open)
                .Where(i => i.SupplierId == supliercode)
                .AsQueryable();
            return Json(invoices.Select(i => new
            {
                Name = i.InvoiceNumber,
                Code = i.InvoiceNumber

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoAType()
        {
            return Json(new
            {
                Type = _accountHelper.IsDynamicCoA() ? "Yes":"No"

            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetOpenApInvoices(string text)
        {
            var invoices = _db.APPaybleInvoices
                .Where(i => i.IsOnlineApproved)
                .Where(i => i.IsVoid==false)
                .Where(i => i.InvoiceStatus == InvoiceStatus.Open)
                .AsQueryable();

            if (!string.IsNullOrEmpty(text))
                invoices = invoices.Where(x => x.ApPaybleInvoicesId.ToString().Contains(text)
                                               || x.InvoiceNumber.Contains(text)
                                               || x.SupplierName.Contains(text)).AsQueryable();
            return Json(invoices.Select(i => new
            {
                Name = i.ApPaybleInvoicesId + "-" + i.InvoiceNumber+"-"+i.SupplierName,
                Code = i.InvoiceNumber

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPartnerTypes()
        {
            var segmentSetups = _db.PartnerTypes
                .Where(x => x.Enable).AsQueryable();
            return Json(segmentSetups.Select(c => new
                {
                    Code = c.PartnerTypeId,
                    Name = c.TypeName
                }),
                JsonRequestBehavior.AllowGet);
        }

        class InvoiceVm
        {
            public string Name { get; set; }
            public string Code { get; set; }
        }

        public JsonResult GetArInvoiceForDebitCreditMemo(string customerid,string text)
        {
            var invoices = _db.ARInvoices
                .Where(x => x.CustomerId == customerid)
                .Where(x => x.IsOnlineApproved)
                .Where(x => x.IsVoid == false)
                .AsQueryable();

            if (!String.IsNullOrEmpty(text))
            {
                invoices = invoices.Where(x => x.Reference.Contains(text) ||
                                               x.ArInvoiceId.ToString() == text).AsQueryable();
            }
            var invoicesList = invoices.ToList();
            return Json(invoicesList.Select(c => new
            {
                Code = c.ArInvoiceId,
                Name = c.ArInvoiceId+"-"+ c.Reference
            }),JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOpenArInvoices(string id,string text)
        {
            int did = int.Parse(id);
            var collection = _db.ARStandardCollections.Find(did);

            List<InvoiceVm> list = new List<InvoiceVm>();

            if (collection != null && !collection.IsNewCustomer)
            {
                var invoices = _db.ARInvoices
                    .Where(x => x.CustomerId == collection.CustomerId)
                    .Where(x => x.IsOnlineApproved)
                    .Where(x => x.IsVoid==false)
                    .Where(x => x.TransactionClass !=TransactionClass.CreditMemo)
                    .Where(x => x.InvoiceStatus == InvoiceStatus.Open).ToList();

                if (!String.IsNullOrEmpty(text))
                    invoices = invoices.Where(x => x.ArInvoiceId.ToString().Contains(text)
                                                   || x.Reference.Contains(text)).ToList();
                foreach (var invoice in invoices)
                {
                    var record = new InvoiceVm
                    {
                        Code = "INVOICE_" + invoice.ArInvoiceId+"_"+invoice.Reference,
                        Name = "INVOICE_" + invoice.ArInvoiceId + "_" + invoice.Reference
                    };
                    list.Add(record);
                }

                var salesOrders = _db.ArSalesOrders
                    .Where(x => x.CustomerId == collection.CustomerId)
                    .Where(x => x.IsOnlineApproved)
                    .Where(x => x.isVoid == false)
                    .Where(x => x.OrderType == OrderType.Cash).ToList();

                if (!String.IsNullOrEmpty(text))
                    salesOrders = salesOrders.Where(x => x.ArSalesOrderId.ToString().Contains(text)
                                                   || x.OrderReference.Contains(text)).ToList();

                foreach (var salesOrder in salesOrders)
                {
                    var record = new InvoiceVm
                    {
                        Code = "SALESORDER_" + salesOrder.ArSalesOrderId + "_" + salesOrder.OrderReference,
                        Name = "SALESORDER_" + salesOrder.ArSalesOrderId + "_" + salesOrder.OrderReference
                    };
                    list.Add(record);
                }

            }

            else if (collection != null && collection.IsNewCustomer)
            {
                var salesOrders = _db.ArSalesOrders
                    .Where(x => x.CustomerName == collection.CustomerName)
                    .Where(x => x.IsOnlineApproved)
                    .Where(x => x.OrderType == OrderType.Cash).ToList();

                if (!String.IsNullOrEmpty(text))
                    salesOrders = salesOrders.Where(x => x.ArSalesOrderId.ToString().Contains(text)
                                                         || x.OrderReference.Contains(text)).ToList();
                foreach (var salesOrder in salesOrders)
                {
                    var record = new InvoiceVm
                    {
                        Code = "SALESORDER_" + salesOrder.ArSalesOrderId,
                        Name = "SALESORDER_" + salesOrder.ArSalesOrderId
                    };
                    list.Add(record);
                }
            }

            return Json(list.Select(c => new
                {
                    Code = c.Code,
                    Name = c.Name
              }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GeActiveCashiers()
        {
            var cashierSetups = _db.CashierSetups
                .Where(x => x.IsActive)
               .AsQueryable();
            return Json(cashierSetups.Select(c => new
                {
                    Code = c.ApCashierSetupId,
                    Name = c.Cashier
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSegmentTypes()
        {
            var glSegmentSetups = _db.GlSegmentSetups
                .Where(x => x.Enabled)
                .Where(x=>x.SegmentType!= DynamicSegmentTypes.NaturalAccount).AsQueryable();
            return Json(glSegmentSetups.Select(c => new
                {
                    Code = c.GLSegmentSetupId,
                    Name=c.Name
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBusinessUnitAccounts()
        {
            var glSegmentSetups = _db.GlSegmentSetups
                .Where(x => x.Enabled)
                .Where(x => x.SegmentType == DynamicSegmentTypes.BusinessUnit).FirstOrDefault();

            var businessUnits = _db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == glSegmentSetups.GLSegmentSetupId).AsQueryable();
            return Json(businessUnits.Select(c => new
            {
                Code = c.Values,
                Name = c.Values+"-"+c.Description
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomerTypes()
        {
            var customers = _db.ARCustomerTypes.Where(x=>x.Enable).AsQueryable();
            return Json(customers.Select(c => new
            {
                CustomerTypeName = c.CustomerTypeName
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGrnNumbers()
        {
            List<InvoiceVm> list = new List<InvoiceVm>();

            List<string> receiveTypes =
                _db.ReceiveTypes.Where(x => x.Type == GoodReceiveType.PurchaseOrder || x.Type == GoodReceiveType.Tender)
                    .Select(x => x.Name)
                    .ToList();

            //stock
            var appayableinvoiceStock = _db.APPaybleInvoices.Where(x=>x.Source==GrnSource.StockGrn).Where(x => x.IsVoid==false).Select(x=>x.Grn).AsQueryable();

            var stockgrns =_db.GoodReceives
                .Where(x => receiveTypes.Contains(x.ReceiveType.Name))
                .Where(x => x.IsOnlineApproved)
                .Where(x => x.IsVoid==false)
                .Where(x=>appayableinvoiceStock.Contains(x.GoodReceiveId.ToString())==false)
                .AsQueryable();

            foreach (var receive in stockgrns)
            {
                var record = new InvoiceVm
                {
                    Code = "STOCK_" + receive.GoodReceiveId,
                    Name = "STOCK_" + receive.GoodReceiveId + "_" + receive.ReceiveRef
                };
                list.Add(record);
            }

            //service
            var appayableinvoiceService = _db.APPaybleInvoices.Where(x => x.Source == GrnSource.ServiceGrn).Where(x => x.IsVoid == false).Select(x => x.Grn).AsQueryable();

            var servicegrns = _db.ServiceReceives
                .Where(x => receiveTypes.Contains(x.ReceiveType.Name))
                .Where(x => x.IsOnlineApproved)
                .Where(x => x.IsVoid == false)
                .Where(x => appayableinvoiceService.Contains(x.ServiceReceiveId.ToString()) == false)
                .AsQueryable();

            foreach (var receive in servicegrns)
            {
                var record = new InvoiceVm
                {
                    Code = "SERVICE_" + receive.ServiceReceiveId,
                    Name = "SERVICE_" + receive.ServiceReceiveId + "_" + receive.ReceiveRef
                };
                list.Add(record);
            }

            //fixed asset
            var appayableinvoiceFixedAsset = _db.APPaybleInvoices.Where(x => x.Source == GrnSource.FixedssetGrn).Where(x => x.IsVoid == false).Select(x => x.Grn).AsQueryable();

            var fixedassetgrns = _db.FixedAssetReceives
                .Where(x => receiveTypes.Contains(x.ReceiveType.Name))
                .Where(x => x.IsOnlineApproved)
                .Where(x => x.IsVoid == false)
                .Where(x => appayableinvoiceFixedAsset.Contains(x.FixedAssetReceiveId.ToString()) == false)
                .AsQueryable();

            foreach (var receive in fixedassetgrns)
            {
                var record = new InvoiceVm
                {
                    Code = "FIXEDASSET_" + receive.FixedAssetReceiveId,
                    Name = "FIXEDASSET_" + receive.FixedAssetReceiveId + "_" + receive.ReceiveRef
                };
                list.Add(record);
            }




            return Json(list.Select(c => new
                {
                    Code = c.Code,
                    Name = c.Name
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActiveSuppliers()
        {
            var customers = _db.ProcurementSuppliers.Where(x => x.IsActive).AsQueryable();
            return Json(customers.Select(c => new
            {
                Code = c.SupplierId,
                Name = c.SupplierId + "_" + c.Name
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLcAndOtherPayments()
        {
            List<SelectorVm> list = new List<SelectorVm>();

            #region foregin

            var foreignPurchaseCosts = _db.ForeignPurchaseCosts
               
                .Where(x => x.IsOnlineApproved && x.IsVoid == false && x.Paid == false).AsQueryable();
            foreach (var c in foreignPurchaseCosts)
            {
                var record = new SelectorVm
                {
                    Code = "LC_" + c.ForeignPurchaseCostId.ToString(),
                    Name = "LC_" + c.ForeignPurchase.TenderRef + "_" + c.CostType + "_" + c.Payee + "_" + c.ForeignPurchaseCostId
                };
                list.Add(record);
            }

            #endregion
            #region payment request

            var payments = _db.PaymentRequests
                .Where(x => x.IsOnlineApproved && x.IsVoid == false && x.Paid == false).AsQueryable();
            foreach (var c in payments)
            {
                var record = new SelectorVm
                {
                    Code = "PAYMENT-REQUEST_" + c.PaymentRequestId.ToString(),
                    Name = "PAYMENT-REQUEST_" + c.ReasonOfPayment + "_" + c.Supplier + "_" + c.PaymentRequestId
                };
                list.Add(record);
            }

            #endregion
            #region travel order

            var travelOrders = _db.ApTravelOrders
                .Where(x => x.IsOnlineApproved && x.IsVoid == false && x.Paid == false).AsQueryable();
            foreach (var c in travelOrders)
            {
                string employeeName = EmployeeHelper.GetEmployeeEnglishName(c.EmployeeId);
                var record = new SelectorVm
                {
                    Code = "TRAVEL-ORDER_" + c.ApTravelOrderId.ToString(),
                    Name = "TRAVEL-ORDER_" + employeeName + "_" + c.Reference + "_" + c.ApTravelOrderId
                };
                list.Add(record);
            }

            #endregion
            #region travel expense claim

            var expenseClaims = _db.ApTravelExpenseClaims
                .Where(x => x.IsOnlineApproved && x.IsVoid == false && x.Paid == false).AsQueryable();
            foreach (var c in expenseClaims)
            {
                string employeeName = EmployeeHelper.GetEmployeeEnglishName(c.EmployeeId);
                var record = new SelectorVm
                {
                    Code = "EXPENSE-CLAIM_" + c.ApTravelExpenseClaimId.ToString(),
                    Name = "EXPENSE-CLAIM_" + employeeName + "_" + c.Reference + "_" + c.ApTravelExpenseClaimId
                };
                list.Add(record);
            }

            #endregion

            return Json(list.Select(c => new
                {
                    Code = c.Code,
                    Name =c.Name
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentRequests()
        {
            var customers = _db.ContractPayments.Where(x => x.IsOnlineApproved && x.IsVoid == false && x.Paid == false).AsQueryable();
            return Json(customers.Select(c => new
                {
                    Code = c.ContractPaymentId,
                    Name = c.ContractManagement.ContractRefNo + "_" + c.ContractManagement.TenderRefNo + "_" + c.ContractManagement.SupplierName
            }),JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccounts()
        {
            var subAccounts = _db.GLChartOfAccountAccounts.Where(x => x.Enable && !x.Parent);
            return Json(subAccounts.Select(e => new
            {
                Description = e.Values + "-" + e.Description,
                Values = e.Values
            }),
                JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetSubAccounts()
        {
            if (_accountHelper.IsDynamicCoA())
            {
                var segmentSetup = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType== DynamicSegmentTypes.SubAccount);

                if (segmentSetup != null)
                {
                    var subAccounts = _db.GlChartOfAccountSegments.Where(x =>
                                                  x.Enable && !x.Parent && x.GLSegmentSetupId == segmentSetup.GLSegmentSetupId);
                    return Json(subAccounts.Select(e => new
                        {
                            Description = e.Values + "-" + e.Description,
                            Values = e.Values
                        }),
                        JsonRequestBehavior.AllowGet);
                }

                #region null;
                
                #endregion
            }
            else
            {
               var subAccounts = _db.GLChartOfAccountSubAccounts.Where(x => x.Enable && !x.Parent);
                return Json(subAccounts.Select(e => new
                    {
                        Description = e.Values + "-" + e.Description,
                        Values = e.Values
                    }),
                    JsonRequestBehavior.AllowGet);
            }

            return null;


        }

        public JsonResult GetProductsAndService()
        {
            var productorService = _db.ARSetupProductServiceAccounts.ToList();

            return Json(productorService.Select(e => new
            {
                Product = e.Product
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductsAndServiceForSalesOrder()
        {
            var productorService = _db.ARSetupProductServiceAccounts.ToList();

            return Json(productorService.Select(e => new
                {
                    Name = e.Product,
                    Code=e.ARSetupProductServiceAccountID
                }),
                JsonRequestBehavior.AllowGet);
        }

       

        public JsonResult GetProductsAndServiceSubItem(string productid,string text)
        {
            int did = int.Parse(productid);
            var productorService = _db.ArProductService.Where(x=>x.ARSetupProductServiceAccountID==did).ToList();

            if (!String.IsNullOrWhiteSpace(text))
                productorService =
                    productorService.Where(x => x.Description.Contains(text) || x.ItemName.Contains(text) || x.Code.Contains(text)).ToList();

            return Json(productorService.Select(e => new
                {
                    Name = e.Code +"_"+e.ItemName,
                    Code=e.ArProductServiceId
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductsAndServiceForSalesOrderPrint(int id)
        {
            var productorService = _db.ArSalesOrderLines.Where(x => x.ArSalesOrderId == id).ToList();

            return Json(productorService.Select(e => new
                {
                    Name = _db.ArProductService.Find(int.Parse(e.SubItem))?.Code + "_" + _db.ArProductService.Find(int.Parse(e.SubItem))?.ItemName,
                    Code = e.ArSalesOrderLineId
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalesOrdersForInvoice()
        {
            var arSalesOrders = _db.ArSalesOrders.Where(x => x.IsOnlineApproved && x.OrderType==OrderType.Credit && x.isVoid==false).AsQueryable();
            return Json(arSalesOrders.Select(c => new
                {
                    Code = c.ArSalesOrderId,
                    Name = c.ArSalesOrderId + "_"  + c.OrderReference
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTravelOrders()
        {
            var orders = _db.ApTravelOrders.Where(x => x.IsOnlineApproved  && x.IsVoid == false).AsQueryable();
            return Json(orders.Select(c => new
                {
                    Code = c.ApTravelOrderId,
                    Name = c.ApTravelOrderId + "_" + c.Reference
            }),
                JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetActiveCustomers()
        {
            var customers = _db.ARCustomerProfiles.Where(x => x.IsActive).AsQueryable();
            return Json(customers.Select(c => new
            {
                CustomerId = c.CustomerId,
                CompanyName = c.CustomerId + "_" + c.CompanyName
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalesOrderNewCustomers()
        {
            var customers = _db.ArSalesOrders.Where(x=>x.IsNewCustomer).Where(x => x.IsOnlineApproved&&x.isVoid==false).AsQueryable();
            return Json(customers.Select(c => new
                {
                    CustomerName = c.CustomerName
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetActivePartners()
        {
            var customers = _db.Partners.Where(x => x.IsActive).AsQueryable();
            return Json(customers.Select(c => new
                {
                    Code = c.PartnerId,
                    Name = c.Name 
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentTerms()
        {
            var terms = _db.PaymentTerms.AsQueryable();
            return Json(terms.Select(p => new
            {
                PaymentTermsDays = p.PaymentTermsDays
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetArPaymentMethods()
        {
            string mybrach = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

            var paymentMethods = _db.PaymentMethods.Where(x => x.ApplyToAr).Where(x => x.Enable)
                .Where(x => x.BranchId.ToString() == mybrach)
                .AsQueryable();
            return Json(paymentMethods.Select(p => new
            {
                Method = p.Method
            }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApPaymentMethods()
        {
             string mybrach = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

            var paymentMethods = _db.PaymentMethods.Where(x => x.ApplyToAp).Where(x => x.Enable)
                .Where(x =>x.BranchId.ToString()== mybrach)
                .AsQueryable();
            return Json(paymentMethods.Select(p => new
            {
                Method = p.Method
            }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApTax()
        {
            var tax = _db.GLTaxRates.Where(x => x.Enable).Where(x => x.ApplyToAp).AsQueryable();
            return Json(tax.Select(p => new
            {
                Name = p.TaxName,
                GLTaxRateID = p.GLTaxRateID
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetArTax()
        {
            var tax = _db.GLTaxRates.Where(x => x.Enable).Where(x => x.ApplyToAr).AsQueryable();
            return Json(tax.Select(p => new
            {
                TaxName = p.TaxName,
                GLTaxRateID = p.GLTaxRateID
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWithholdingTax()
        {
            var tax = _db.GLTaxRates.Where(x => x.Enable).Where(x => x.ApplyToWithholding).AsQueryable();
            return Json(tax.Select(p => new
            {
                Name = p.TaxName,
                GLTaxRateID = p.GLTaxRateID
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSalesOrderCustomer(string id)
        {
            var data = from a in _db.ArSalesOrders
                where (a.ArSalesOrderId.ToString().Equals(id))
                select a;
            var formattedData = data.Select(m => new
            {
                CustomerId = m.CustomerId,
                PaymentTerm = m.PaymentTerm,
                BranchId = m.BranchId
            });
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTravelorder(string id)
        {
            var data = from a in _db.ApTravelOrders
                where (a.ApTravelOrderId.ToString().Equals(id))
                select a;
            var formattedData = data.Select(m => new
            {
                Name = m.EmployeeId,
                PurposeOfTravel = m.PurposeOfTravel
            });
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductAccount(string id = "")
        {
          
            var data = from a in _db.ARSetupProductServiceAccounts
                       where (a.Product.Equals(id))
                       select a;
            var formattedData = data.Select(m => new
            {
                AccountCode = m.AccountCode,
                Description = m.Description
            });
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProductItemPrice(string id)
        {
            if (!String.IsNullOrWhiteSpace(id))
            {
                int did = int.Parse(id);


                var data = from a in _db.ArProductService
                    where (a.ArProductServiceId.Equals(did))
                    select a;
                var formattedData = data.Select(m => new
                {
                    Price = m.SalesPrice,
                    Unit = m.UoM
                });
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var formattedData1 =new 
            {
                Price = 0,
                Unit = ""
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }
    

        public JsonResult GetInvoiceDueAmount(string id = "")
        {
            string [] voucher = id.Split('_');
            int idInt = int.Parse(voucher[1]);
            if (voucher[0] == "INVOICE")
            {
                var data = from a in _db.ARInvoices
                    where (a.ArInvoiceId.Equals(idInt))
                    select a;
                var formattedData = data.Select(m => new
                {
                    AmountDue = m.AmountDue
                });
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (voucher[0] == "SALESORDER")
            {
                var data = from a in _db.ArSalesOrders
                    where (a.ArSalesOrderId.Equals(idInt))
                    select a;
                var formattedData = data.Select(m => new
                {
                    AmountDue = m.ArSalesOrderLines.Sum(x=>x.TotalAmount)
                });
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;
        }

        public JsonResult GetPayableInvoiceData(string id = "")
        {
            string supplierId = "";
            string supplierName = "";
            string amountdue = "";

            var invoice = _db.APPaybleInvoices.FirstOrDefault(a => a.InvoiceNumber == id);
            if (invoice != null)
            {
                supplierId = invoice.SupplierId;
                supplierName = invoice.SupplierName;
                amountdue = invoice.AmountDue.ToString();
            }

            var formattedData = new
            {
                AmountDue = amountdue,
                SupplierId = supplierId,
                SupplierName = supplierName
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
           
        }

        public JsonResult GetPurchaseAdvanceAmount(string id = "",string source = "")
        {
            decimal amount = 0;

            if (!String.IsNullOrWhiteSpace(id))
            {
                int did = int.Parse(id);
                if (source == "Purchase Order")
                {
                    amount = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == did).ToList()
                        .Sum(x => x.UnitPrice * x.Quantity);

                    #region deduct previous advance taken

                    var advanceTaken = _db.APPayments
                        .Where(x => x.PurchaseReference ==did.ToString())
                        .Where(x => x.PoType == "Purchase Order")
                        .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                        .ToList();
                    if (advanceTaken.Any())
                    {
                        decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount);
                        amount = amount - advanceTakenAmount;
                    }

                    #endregion
                }

                else if (source == "Tender Award")
                {
                    var contract = _db.ContractManagements.FirstOrDefault(x => x.ProcurementRefNo.ToString() == id);

                    if (contract != null)
                    {
                        var items = _db.ContractManagementItems.Where(x => x.ContractManagementID == contract.ContractManagementID).ToList();

                        var schedule = _db.ContractPaymentSchedules
                            .Where(x => x.ContractManagementID == contract.ContractManagementID)
                            .FirstOrDefault(x => x.PaymentType == PaymentType.Advance);

                        foreach (var item in items)
                        {
                            amount = amount + (item.Quantity + item.Addition + item.Omission) * item.UnitPrice;
                        }

                        if (schedule != null && schedule.Percentage > 0)
                            amount = amount * schedule.Percentage / 100;

                        #region deduct previous advance taken

                        var advanceTaken = _db.APPayments
                            .Where(x => x.PurchaseReference == did.ToString())
                            .Where(x => x.PoType == "Tender Award")
                            .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                            .ToList();
                        if (advanceTaken.Any())
                        {
                            decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount);
                            amount = amount - advanceTakenAmount;
                        }

                        #endregion
                    }
                }
            }

            var formattedData =  new
            {
                AdvanceAmount =amount
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGrnData(string id)
        {
            string supplierId = "";
            string supplierName = "";
            string purchaser = "";
            string prno = "";
            string pono = "";
            string invoivenumber = "";

            string grn = id.Split('_')[1];
            string grnType = id.Split('_')[0];
            int intId = int.Parse(grn);
            switch (grnType)
            {
                case "STOCK":
                    var goodReceive = _db.GoodReceives.Find(intId);
                    var exists = _db.APPaybleInvoices.Where(x => x.Source == GrnSource.StockGrn).Where(x => x.Grn == goodReceive.GoodReceiveId.ToString()).ToList();
                    if (exists.Any())
                        ModelState.AddModelError("exist", "The GRN Number already exists in Payable Invoice.");


                    #region get data
                    if (goodReceive != null)
                    {
                        pono = goodReceive.ReceiveRef;
                        invoivenumber = goodReceive.SupplierInvoiceNo;

                        ReceiveType type = _db.ReceiveTypes.Find(goodReceive.ReceiveTypeId);
                        if (type != null)
                        {
                            switch (type.Type)
                            {
                                case GoodReceiveType.PurchaseOrder:
                                    int pid = int.Parse(goodReceive.ReceiveRef);
                                    PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
                                    if (purchaseOrder != null)
                                    {
                                        prno = purchaseOrder.PurchaseRequisitionID.ToString();
                                        purchaser = purchaseOrder.ProcurementOfficer.ToString();
                                        supplierId = purchaseOrder.SupplierId;
                                        supplierName = purchaseOrder.SupplierName;
                                    }
                                    break;
                                case GoodReceiveType.Tender:
                                    int awardid = int.Parse(goodReceive.ReceiveRef);
                                    BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
                                    if (award != null)
                                    {
                                        //not possible to get pr since merged prs
                                        //prno = award.Lot.pu.ToString();
                                        var lot = _db.Lots.Find(award.LotID);
                                        if (lot != null)
                                        {
                                            var tender = _db.Tenders.Find(lot.TenderID);
                                            if (tender != null) purchaser= tender.ProcurementOfficer.ToString();
                                        }
                                        supplierId= award.SupplierId;
                                        supplierName = award.SupplierName;
                                    }
                                    break;
                            }
                        }
                    }
                    #endregion
                    break;

                case "SERVICE":
                    var serviceReceive = _db.ServiceReceives.Find(intId);
                    
                    #region get data
                    if (serviceReceive != null)
                    {
                        pono = serviceReceive.ReceiveRef.ToString();
                        invoivenumber = serviceReceive.SupplierInvoiceNo;

                        ReceiveType type = _db.ReceiveTypes.Find(serviceReceive.ReceiveTypeId);
                        if (type != null)
                        {
                            switch (type.Type)
                            {
                                case GoodReceiveType.PurchaseOrder:
                                    int pid = int.Parse(serviceReceive.ReceiveRef);
                                    PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
                                    if (purchaseOrder != null)
                                    {
                                        prno = purchaseOrder.PurchaseRequisitionID.ToString();
                                        purchaser = purchaseOrder.ProcurementOfficer.ToString();
                                        supplierId = purchaseOrder.SupplierId;
                                        supplierName = purchaseOrder.SupplierName;
                                    }
                                    break;
                                case GoodReceiveType.Tender:
                                    int awardid = int.Parse(serviceReceive.ReceiveRef);
                                    BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
                                    if (award != null)
                                    {
                                        //not possible to get pr since merged prs
                                        //prno = award.Lot.pu.ToString();
                                        var lot = _db.Lots.Find(award.LotID);
                                        if (lot != null)
                                        {
                                            var tender = _db.Tenders.Find(lot.TenderID);
                                            if (tender != null) purchaser = tender.ProcurementOfficer.ToString();
                                        }
                                        supplierId = award.SupplierId;
                                        supplierName = award.SupplierName;
                                    }
                                    break;
                            }
                        }
                    }
                    #endregion
                    break;

                case "FIXEDASSET":
                    var fixedAssetReceive = _db.FixedAssetReceives.Find(intId);
                   
                    #region get data
                    if (fixedAssetReceive != null)
                    {
                        pono = fixedAssetReceive.ReceiveRef.ToString();
                        invoivenumber = fixedAssetReceive.SupplierInvoiceNo;

                        ReceiveType type = _db.ReceiveTypes.Find(fixedAssetReceive.ReceiveTypeId);
                        if (type != null)
                        {
                            switch (type.Type)
                            {
                                case GoodReceiveType.PurchaseOrder:
                                    int pid = int.Parse(fixedAssetReceive.ReceiveRef);
                                    PurchaseOrder purchaseOrder = _db.PurchaseOrders.Find(pid);
                                    if (purchaseOrder != null)
                                    {
                                        prno = purchaseOrder.PurchaseRequisitionID.ToString();
                                        purchaser = purchaseOrder.ProcurementOfficer.ToString();
                                        supplierId = purchaseOrder.SupplierId;
                                        supplierName = purchaseOrder.SupplierName;

                                    }
                                    break;
                                case GoodReceiveType.Tender:
                                    int awardid = int.Parse(fixedAssetReceive.ReceiveRef);
                                    BidAwardedSupplier award = _db.BidAwardedSuppliers.Find(awardid);
                                    if (award != null)
                                    {
                                        //not possible to get pr since merged prs
                                        //prno = award.Lot.pu.ToString();
                                        var lot = _db.Lots.Find(award.LotID);
                                        if (lot != null)
                                        {
                                            var tender = _db.Tenders.Find(lot.TenderID);
                                            if (tender != null) purchaser = tender.ProcurementOfficer.ToString();
                                        }
                                        supplierId= award.SupplierId;
                                        supplierName= award.SupplierName;

                                      
                                    }
                                    break;
                            }
                        }
                    }
                    #endregion
                    break;

            }

            var formattedData = new
            {
                SupplierId = supplierId,
                SupplierName=supplierName,
                ProcurementOfficer=purchaser,
                PurchaseRequisitionID=prno,
                PONo=pono,
                InvoiceNumber=invoivenumber
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPaymentRequestData(string id)
        {
            string supplierId = "";
            string supplierName = "";
            string purchaser = "";
            string prno = "";
            string pono = "";
            string invoivenumber = "";
            decimal invoiceamount = 0;

            var contractPayment = _db.ContractPayments.FirstOrDefault(x => x.ContractPaymentId.ToString() == id);
            if (contractPayment != null)
            {
                var contract = _db.ContractManagements.Find(contractPayment.ContractManagementID);
                if (contract != null)
                {
                    pono = contract.ContractRefNo;
                    invoivenumber = contractPayment.SupplierInvoice;
                    prno = "";
                    purchaser = contract.ProcurementOfficer.ToString();
                    supplierId = contract.SupplierId;
                    supplierName = contract.SupplierName;
                    invoiceamount = contractPayment.NetPayable;
                }
            }

            var formattedData = new
            {
                SupplierId = supplierId,
                SupplierName = supplierName,
                ProcurementOfficer = purchaser,
                PurchaseRequisitionID = prno,
                PONo = pono,
                InvoiceNumber = invoivenumber,
                InvoiceAmount = invoiceamount
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSupplierBalance(string id)
        {
            SecondLedgerHelper secondLedgerHelper = new SecondLedgerHelper();
            decimal advanceBalance = secondLedgerHelper.GetSupplierAdvanceBalance(id);
            var formattedData =  new
            {
                OutstandingBalance =0,
                PrepaymentBalance= advanceBalance
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPayableItems()
        {
            var items = _db.ApSetupItems.ToList();
            var formattedData = items.Select(m => new
            {
                Code = m.ApSetupItemId,
                Name = m.Name+"-"+m.Description
            });
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPayableItemsAccount(string id = "")
        {
            var data = from a in _db.ApSetupItems
                       where (a.Name.Equals(id))
                       select a;
            var formattedData = data.Select(m => new
            {
                AccountCode = m.AccountCode,
                Description = m.Description
            });
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGlFiscalYear()
        {
            var fiscalYears = _db.GLFiscalYears.AsQueryable();
            return Json(fiscalYears.Select(p => new
                {
                    Name = p.Name,
                    Code = p.GlFiscalYearId
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGlPeriod(string year)
        {
            int did = int.Parse(year);
            var periods = _db.GLPeriods.Where(x=>x.GlFiscalYearId==did).AsQueryable();
            return Json(periods.Select(p => new
                {
                    Name = p.Name,
                    Code = p.GLPeriodId
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOpenGlPeriod()
        {
            var periods = _db.GLPeriods.Where(x => x.Status==FiscalYearStatus.Open).AsQueryable();
            return Json(periods.Select(p => new
                {
                    Name = p.Name,
                    Code = p.GLPeriodId
                }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseTypes()//apapyment
        {
            string[] types = {"Purchase Order", "Tender Award"};
            return Json(types.Select(p => new
                {
                    Name = p,
                    Code = p
                }),
                JsonRequestBehavior.AllowGet);
        }
    }
}