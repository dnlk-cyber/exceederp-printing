using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Areas.Finance.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{

    public class ApPaymentAdvanceController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayablePayment)]

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var branches = _db.Branch;
            ViewData["Branches"] = branches;

            var paymentMethod = _db.PaymentMethods.ToList();

            ViewData["methods"] = paymentMethod.Select(e => new PaymentMethod
            {
                Method = e.Method
            }).OrderBy(m => m.Method);

            var suppliers = _db.ProcurementSuppliers.ToList();

            ViewData["suppliers"] = suppliers.Select(c => new ProcurementSupplier
            {
                Name = c.Name,
                SupplierId = c.SupplierId
            });

            var invoices =
                _db.APPaybleInvoices.Where(x => x.IsOnlineApproved).Where(x => x.InvoiceStatus == InvoiceStatus.Open).ToList();

            ViewData["invoices"] = invoices
                .Select(e => new ApPaybleInvoices
                {
                    InvoiceNumber = e.InvoiceNumber
                })
                .OrderBy(e => e.InvoiceNumber);

            //procu
            var items = _db.PurchaseOrderItems;

            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemName,
                    ItmCode = i.ItmCode
                }).AsQueryable();

            ViewData["Items"] = selecteditems;

            var selectedcatedories = items.Select(
                i => new
                {
                    Name = i.CategoryName,
                    CatCode = i.CatCode
                }).AsQueryable();
            ViewData["Category"] = selectedcatedories;
            var tax = _db.GLTaxRates;
            ViewData["tax"] = tax;

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;
        }
        private bool IsPurchaseBelongToPurchaser(int empid,string type,string refe)
        {
            bool belongsToPurchaser = false;
            if (type == "Purchase Order")
            {
                var po = _db.PurchaseOrders.Find(int.Parse(refe));
                if (po.ProcurementOfficer == empid)
                    belongsToPurchaser = true;
            }

            else if (type == "Tender Award")
            {
                var award = _db.BidAwardedSuppliers.Find(int.Parse(refe));
                if (award != null)
                {
                    var lot = _db.Lots.Find(award.LotID);
                    if (lot != null)
                    {
                        var tender = _db.Tenders.Find(lot.TenderID);
                        if (tender != null && tender.ProcurementOfficer == empid)
                            belongsToPurchaser = true;
                    }
                }
            }

            return belongsToPurchaser;
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayablePayment)]
        public ActionResult APPayments_Read([DataSourceRequest]DataSourceRequest request, bool isNotTransffered)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<ApPayment> appayments = _db.APPayments.Where(x=>x.IsPrepayment)
                .Where(x => mybraches.Contains(x.OrgBranchName));

            if (isNotTransffered)
            {
                List<int> transfferableList = FinanceTransferHelper.GetApAdvancePaymentTranferableList();
                appayments =
                    appayments.Where(x => transfferableList.Contains(x.ApPaymentId));
            }
            DataSourceResult result = appayments.ToDataSourceResult(request, apPayment => new
            {
                ApPaymentId = apPayment.ApPaymentId,
                BranchId = apPayment.BranchId,
                BusinessUnitId = apPayment.BusinessUnitId,
                IsPrepayment = apPayment.IsPrepayment,
                SupplierName = apPayment.SupplierName,
                SupplierId = apPayment.SupplierId,
                InvoiceNumber = apPayment.InvoiceNumber,
                InvoiceAmount = apPayment.InvoiceAmount,
                PaymentAmount = apPayment.PaymentAmount,
                RemainingBalance = apPayment.RemainingBalance,
                HasPrepayment = apPayment.HasPrepayment,
                PrepaymentAmount = apPayment.PrepaymentAmount,
                NetAmount = apPayment.NetAmount,
                PaymentMethod = apPayment.PaymentMethod,
                PaymentReference = apPayment.PaymentReference,
                PaymentDate = apPayment.PaymentDate,
                PoTypeEnum = apPayment.PoTypeEnum,
                PoType = apPayment.PoType,
                PurchaseReference = apPayment.PurchaseReference,
                CollectedByPurchaser = apPayment.CollectedByPurchaser,
                EmployeeId = apPayment.EmployeeId,
                GLTaxRateID = apPayment.GLTaxRateID,
                TaxAmount = apPayment.TaxAmount,
                WithholdingType = apPayment.WithholdingType,
                WithholdingAmount = apPayment.WithholdingAmount,
                WithholdTax = apPayment.WithholdTax,
                WithholdTaxAmount = apPayment.WithholdTaxAmount,
                PrepaymentBalance = apPayment.PrepaymentBalance,
                OutstandingBalance = apPayment.OutstandingBalance,

                Remark = apPayment.Remark,
                IsSendForApproval = apPayment.IsSendForApproval,
                IsOnlineTransferred = apPayment.IsOnlineTransferred,
                IsOnlineTransferredBy = apPayment.IsOnlineTransferredBy,
                OnlineTransferredTime = apPayment.OnlineTransferredTime,
                Status = apPayment.Status,
                IsOnlineApproved = apPayment.IsOnlineApproved,
                OnlineApprovedBy = apPayment.OnlineApprovedBy,
                OnlineApprovedTime = apPayment.OnlineApprovedTime,
                IsVoid = apPayment.IsVoid,
                VoidBy = apPayment.VoidBy,
                VoidTime = apPayment.VoidTime,
                OrgBranchName = apPayment.OrgBranchName,
                DateCreated = apPayment.DateCreated,
                LastModified = apPayment.LastModified,
                CreatedBy = apPayment.CreatedBy,
                ModifiedBy = apPayment.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPayments_Create([DataSourceRequest]DataSourceRequest request, ApPayment apPayment)
        {

            #region validation

            if (apPayment.PrepaymentAmount <= 0)
            {
                ModelState.AddModelError("zero", "Please fill prepayment amount.");
            }
            //if (String.IsNullOrWhiteSpace(apPayment.PoType))
            //{
            //    ModelState.AddModelError("zero", "Please choose purchase type.");
            //}

            if (String.IsNullOrWhiteSpace(apPayment.PurchaseReference))
            {
                ModelState.AddModelError("zero", "Please choose  purchase reference.");
            }
            var exisit = _db.APPayments.FirstOrDefault(x => x.PaymentReference == apPayment.PaymentReference);
            if (exisit != null)
            {
                ModelState.AddModelError("exist", $"The reference number {apPayment.PaymentReference} already exisits in system.");
            }

            if (!String.IsNullOrEmpty(apPayment.PaymentReference))
            {
                var exists = _db.APPayments.Where(x => x.PaymentReference == apPayment.PaymentReference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Payment Reference", $"{apPayment.PaymentReference}  already exists in the system.");
            }

            //if (apPayment.EmployeeId != null && !String.IsNullOrWhiteSpace(apPayment.PoType) && !String.IsNullOrWhiteSpace(apPayment.PurchaseReference))
            //{
            //    if (!IsPurchaseBelongToPurchaser((int)apPayment.EmployeeId, apPayment.PoType, apPayment.PurchaseReference))
            //        ModelState.AddModelError("notown", "The selected purchase reference does not belong to the purchaser.");
            //}

            //apPayment.RemainingBalance = apPayment.InvoiceAmount - apPayment.PaymentAmount;
            #endregion
            #region data
            if(apPayment.CollectedByPurchaser)
            {
                if (!String.IsNullOrEmpty(apPayment.PurchaseReference))
                {
                    int did = int.Parse(apPayment.PurchaseReference);
                    var payreq= _db.PurchasePayments.Find(did);
                    apPayment.EmployeeId = payreq.ProcurementOfficer;
                }
            }
            
            

            //if (!String.IsNullOrWhiteSpace(apPayment.PoType)&& !String.IsNullOrWhiteSpace(apPayment.PurchaseReference))
            //{
            //    if (apPayment.PoType == "Purchase Order")
            //    {
            //        apPayment.PoTypeEnum = PurchaseType.PurchaseOrder;
            //        var po = _db.PurchaseOrders.Find(int.Parse(apPayment.PurchaseReference));
            //        if (po != null)
            //        {
            //            apPayment.SupplierId = po.SupplierId;
            //            apPayment.SupplierName = po.SupplierName;
            //            apPayment.EmployeeId = po.ProcurementOfficer;

            //            #region over payment validation
            //            var advanceTaken = _db.APPayments
            //                .Where(x => x.PurchaseReference == po.PurchaseOrderID.ToString())
            //                .Where(x => x.PoType == "Purchase Order")
            //                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
            //                .Select(x => new { x.PrepaymentAmount })
            //                .AsQueryable();
            //            if (advanceTaken.Any())
            //            {
            //                decimal totalAmount = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == po.PurchaseOrderID)                              
            //                    .Sum(x => x.Total);
            //                decimal advanceAllowedPerent = _db.PoPaymentSchedules
            //                    .Where(x => x.PurchaseOrderID == po.PurchaseOrderID)
            //                    .Where(x => x.PaymentType == PaymentType.Advance).Sum(x => x.Percentage);

            //                decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount)+ apPayment.PrepaymentAmount;
            //                decimal advancePossibleAmount = (totalAmount* advanceAllowedPerent)/100;
            //                if (advanceTakenAmount >= advancePossibleAmount)
            //                    ModelState.AddModelError("over","Overpaying the supplier is not possible");
            //            }
            //            #endregion
            //        }
            //    }

            //    if (apPayment.PoType == "Tender Award")
            //    {
            //        apPayment.PoTypeEnum = PurchaseType.TenderAward;
            //        var bidAwardedSupplier = _db.BidAwardedSuppliers.Find(int.Parse(apPayment.PurchaseReference));
            //        if (bidAwardedSupplier != null)
            //        {
            //            apPayment.SupplierId = bidAwardedSupplier.SupplierId;
            //            apPayment.SupplierName = bidAwardedSupplier.SupplierName;

            //            var lot = _db.Lots.Find(bidAwardedSupplier.LotID);
            //            if (lot != null)
            //            {
            //                var tender = _db.Tenders.Find(lot.TenderID);
            //                if (tender != null) apPayment.EmployeeId = tender.ProcurementOfficer;
            //            }

            //            #region check over advance taken

            //            var contract = _db.ContractManagements
            //                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
            //                .Where(x => x.ProcurementRefNo == int.Parse(apPayment.PurchaseReference)).FirstOrDefault();

            //            if (contract != null)
            //            {
            //                var advanceTaken = _db.APPayments
            //                    .Where(x => x.PurchaseReference == contract.ProcurementRefNo.ToString())
            //                    .Where(x => x.PoType == "Tender Award")
            //                    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
            //                    .ToList();
            //                if (advanceTaken.Any())
            //                {
            //                    decimal totalAmount = _db.ContractManagementItems.Where(x => x.ContractManagementID == contract.ContractManagementID).Sum(x=>x.Total);
            //                    decimal advanceAllowedPerent = _db.ContractPaymentSchedules
            //                        .Where(x => x.ContractManagementID == contract.ContractManagementID)
            //                        .Where(x => x.PaymentType == PaymentType.Advance).Sum(x => x.Percentage);

            //                    decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount) + apPayment.PrepaymentAmount;
            //                    decimal advancePossibleAmount = (totalAmount * advanceAllowedPerent) / 100;
            //                    if (advanceTakenAmount >= advancePossibleAmount)
            //                        ModelState.AddModelError("over", "Overpaying the supplier is not possible");
            //                }
            //            }
            //            else
            //            {
            //                ModelState.AddModelError("nocontract", "The award is not available in contract management.");
            //            }

            //            #endregion
                        
            //        }
            //    }
            //}
            //else
            //{
            //    ModelState.AddModelError("notselected", "Please select purchase source and purchase reference");
            //}
            #endregion
            #region net
            var tax = _db.GLTaxRates.Find(apPayment.GLTaxRateID);
            if (tax != null)
                apPayment.TaxAmount = apPayment.PrepaymentAmount * tax.CalculatedRate;

            if (!String.IsNullOrEmpty(apPayment.WithholdingType))
            {
                var withholdingtype = _db.GLTaxRates.Find(int.Parse(apPayment.WithholdingType));
                if (withholdingtype != null)
                    apPayment.WithholdingAmount = apPayment.PrepaymentAmount * withholdingtype.CalculatedRate;
            }

            if(apPayment.WithholdTax)
            {
                //get the percentage
                var varwitholding = _db.GLVatWithholdings.FirstOrDefault();
                if (varwitholding != null)
                {
                    apPayment.WithholdTaxAmount = apPayment.TaxAmount * (varwitholding.WithholdPercent / 100);
                }
                else
                {
                    apPayment.WithholdTaxAmount = apPayment.TaxAmount;
                }
            }

            apPayment.NetAmount = apPayment.PrepaymentAmount
                                  + apPayment.TaxAmount
                                  - apPayment.WithholdTaxAmount
                                  - apPayment.WithholdingAmount;

            #endregion

            if (apPayment.CollectedByPurchaser && apPayment.EmployeeId == 0)
                ModelState.AddModelError("zero", "System could not get purchaser information from purchase source.");

            if (ModelState.IsValid)
            {
                var entity = new ApPayment
                {
                    IsPrepayment = true,
                    BranchId = apPayment.BranchId,
                    BusinessUnitId = apPayment.BusinessUnitId,
                    SupplierName = apPayment.SupplierName,
                    SupplierId = apPayment.SupplierId,
                    InvoiceNumber = apPayment.InvoiceNumber,
                    InvoiceAmount = apPayment.InvoiceAmount,
                    PaymentAmount = apPayment.PaymentAmount,
                    RemainingBalance = apPayment.RemainingBalance,
                    HasPrepayment = apPayment.HasPrepayment,
                    PrepaymentAmount = apPayment.PrepaymentAmount,
                    NetAmount = apPayment.NetAmount,
                    PaymentMethod = apPayment.PaymentMethod,
                    PaymentReference = apPayment.PaymentReference,
                    PaymentDate = apPayment.PaymentDate,
                    PoTypeEnum = apPayment.PoTypeEnum,
                    PoType = apPayment.PoType,
                    PurchaseReference = apPayment.PurchaseReference,
                    CollectedByPurchaser = apPayment.CollectedByPurchaser,
                    EmployeeId = apPayment.EmployeeId,
                    GLTaxRateID = apPayment.GLTaxRateID,
                    TaxAmount = apPayment.TaxAmount,
                    WithholdingType = apPayment.WithholdingType,
                    WithholdingAmount = apPayment.WithholdingAmount,
                    WithholdTax = apPayment.WithholdTax,
                    WithholdTaxAmount = apPayment.WithholdTaxAmount,
                    PrepaymentBalance = apPayment.PrepaymentBalance,
                    OutstandingBalance = apPayment.OutstandingBalance,
                    ForBranchId = apPayment.ForBranchId,
                    Remark = apPayment.Remark,
                    IsSendForApproval = apPayment.IsSendForApproval,
                    IsOnlineTransferred = apPayment.IsOnlineTransferred,
                    IsOnlineTransferredBy = apPayment.IsOnlineTransferredBy,
                    OnlineTransferredTime = apPayment.OnlineTransferredTime,
                    Status = apPayment.Status,
                    IsOnlineApproved = apPayment.IsOnlineApproved,
                    OnlineApprovedBy = apPayment.OnlineApprovedBy,
                    OnlineApprovedTime = apPayment.OnlineApprovedTime,
                    IsVoid = apPayment.IsVoid,
                    VoidBy = apPayment.VoidBy,
                    VoidTime = apPayment.VoidTime,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = apPayment.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = apPayment.ModifiedBy
                };

                _db.APPayments.Add(entity);
                _db.SaveChanges();
                apPayment.ApPaymentId = entity.ApPaymentId;

                #region update and distribution

                UpdatePaymentTotals(entity.ApPaymentId);
                
                _distributionHelper.UpdateApAdvancePaymentDistribution(apPayment.ApPaymentId);

                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP advance payment", "Success", User.Identity.Name, $"created payment {entity.ApPaymentId} ", Modules.Finance);
                #endregion
            }
            return Json(new[] { apPayment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPayments_Update([DataSourceRequest]DataSourceRequest request, ApPayment apPayment)
        {

            #region validation

         

            if (apPayment.PrepaymentAmount <= 0)
            {
                ModelState.AddModelError("zero", "Please fill prepayment amount.");
            }
            //if (String.IsNullOrWhiteSpace(apPayment.PoType))
            //{
            //    ModelState.AddModelError("zero", "Please choose purchase type.");
            //}

            if (String.IsNullOrWhiteSpace(apPayment.PurchaseReference))
            {
                ModelState.AddModelError("zero", "Please choose  purchase reference.");
            }

            var exisit = _db.APPayments.Where(x => x.ApPaymentId != apPayment.ApPaymentId).FirstOrDefault(x => x.PaymentReference == apPayment.PaymentReference);
            if (exisit != null)
            {
                ModelState.AddModelError("exist", $"The reference number {apPayment.PaymentReference} already exisits in system.");
            }

            if (!String.IsNullOrEmpty(apPayment.PaymentReference))
            {
                var exists = _db.APPayments
                    .Where(x=>x.ApPaymentId!=apPayment.ApPaymentId).Where(x => x.PaymentReference == apPayment.PaymentReference && x.IsVoid == false).ToList();

                if (exists.Any())
                    ModelState.AddModelError("Payment Reference", $"{apPayment.PaymentReference}  already exists in the system.");
            }

            //if (apPayment.EmployeeId != null && !String.IsNullOrWhiteSpace(apPayment.PoType) && !String.IsNullOrWhiteSpace(apPayment.PurchaseReference))
            //{
            //    if (!IsPurchaseBelongToPurchaser((int)apPayment.EmployeeId, apPayment.PoType, apPayment.PurchaseReference))
            //        ModelState.AddModelError("notown", "The selected purchase reference does not belong to the purchaser.");
            //}

            //apPayment.RemainingBalance = apPayment.InvoiceAmount - apPayment.PaymentAmount;
            #endregion
            #region data

            if (apPayment.CollectedByPurchaser)
            {
                if (!String.IsNullOrEmpty(apPayment.PurchaseReference))
                {
                    int did = int.Parse(apPayment.PurchaseReference);
                    var payreq = _db.PurchasePayments.Find(did);
                    apPayment.EmployeeId = payreq.ProcurementOfficer;
                }
            }

            //if (!String.IsNullOrWhiteSpace(apPayment.PoType) && !String.IsNullOrWhiteSpace(apPayment.PurchaseReference))
            //{
            //    if (apPayment.PoType == "Purchase Order")
            //    {
            //        apPayment.PoTypeEnum = PurchaseType.PurchaseOrder;
            //        var po = _db.PurchaseOrders.Find(int.Parse(apPayment.PurchaseReference));
            //        if (po != null)
            //        {
            //            apPayment.SupplierId = po.SupplierId;
            //            apPayment.SupplierName = po.SupplierName;
            //            apPayment.EmployeeId = po.ProcurementOfficer;

            //            #region over payment validation
            //            var advanceTaken = _db.APPayments
            //                .Where(x => x.PurchaseReference == po.PurchaseOrderID.ToString())
            //                .Where(x => x.PoType == "Purchase Order")
            //                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
            //                .Select(x => new { x.PrepaymentAmount })
            //                .AsQueryable();
            //            if (advanceTaken.Any())
            //            {
            //                decimal totalAmount = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == po.PurchaseOrderID)
            //                    .Sum(x => x.Total);
            //                decimal advanceAllowedPerent = _db.PoPaymentSchedules
            //                    .Where(x => x.PurchaseOrderID == po.PurchaseOrderID)
            //                    .Where(x => x.PaymentType == PaymentType.Advance).Sum(x => x.Percentage);

            //                decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount) + apPayment.PrepaymentAmount;
            //                decimal advancePossibleAmount = (totalAmount * advanceAllowedPerent) / 100;
            //                if (advanceTakenAmount >= advancePossibleAmount)
            //                    ModelState.AddModelError("over", "Overpaying the supplier is not possible");
            //            }
            //            #endregion
            //        }
            //    }

            //    if (apPayment.PoType == "Tender Award")
            //    {
            //        apPayment.PoTypeEnum = PurchaseType.TenderAward;
            //        var bidAwardedSupplier = _db.BidAwardedSuppliers.Find(int.Parse(apPayment.PurchaseReference));
            //        if (bidAwardedSupplier != null)
            //        {
            //            apPayment.SupplierId = bidAwardedSupplier.SupplierId;
            //            apPayment.SupplierName = bidAwardedSupplier.SupplierName;

            //            var lot = _db.Lots.Find(bidAwardedSupplier.LotID);
            //            if (lot != null)
            //            {
            //                var tender = _db.Tenders.Find(lot.TenderID);
            //                if (tender != null) apPayment.EmployeeId = tender.ProcurementOfficer;
            //            }
            //            #region check over advance taken

            //            var contract = _db.ContractManagements
            //                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
            //                .Where(x => x.ProcurementRefNo == int.Parse(apPayment.PurchaseReference)).FirstOrDefault();

            //            if (contract != null)
            //            {
            //                var advanceTaken = _db.APPayments
            //                    .Where(x => x.PurchaseReference == contract.ProcurementRefNo.ToString())
            //                    .Where(x => x.PoType == "Tender Award")
            //                    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
            //                    .ToList();
            //                if (advanceTaken.Any())
            //                {
            //                    decimal totalAmount = _db.ContractManagementItems.Where(x => x.ContractManagementID == contract.ContractManagementID).Sum(x => x.Total);
            //                    decimal advanceAllowedPerent = _db.ContractPaymentSchedules
            //                        .Where(x => x.ContractManagementID == contract.ContractManagementID)
            //                        .Where(x => x.PaymentType == PaymentType.Advance).Sum(x => x.Percentage);

            //                    decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount) + apPayment.PrepaymentAmount;
            //                    decimal advancePossibleAmount = (totalAmount * advanceAllowedPerent) / 100;
            //                    if (advanceTakenAmount >= advancePossibleAmount)
            //                        ModelState.AddModelError("over", "Overpaying the supplier is not possible");
            //                }
            //            }
            //            else
            //            {
            //                ModelState.AddModelError("nocontract", "The award is not available in contract management.");
            //            }

            //            #endregion
            //        }
            //    }
            //}
            //else
            //{
            //    ModelState.AddModelError("notselected", "Please select purchase source and purchase reference");
            //}
            #endregion
            #region net
            var tax = _db.GLTaxRates.Find(apPayment.GLTaxRateID);
            if (tax != null)
                apPayment.TaxAmount = apPayment.PrepaymentAmount * tax.CalculatedRate;
            if (!String.IsNullOrEmpty(apPayment.WithholdingType))
            {
                var withholdingtype = _db.GLTaxRates.Find(int.Parse(apPayment.WithholdingType));
                if (withholdingtype != null)
                    apPayment.WithholdingAmount = apPayment.PrepaymentAmount * withholdingtype.CalculatedRate;
            }

            if (apPayment.WithholdTax)
            {
                //get the percentage
                var varwitholding = _db.GLVatWithholdings.FirstOrDefault();
                if (varwitholding != null)
                {
                    apPayment.WithholdTaxAmount = apPayment.TaxAmount*(varwitholding.WithholdPercent/100);
                }
                else
                {
                    apPayment.WithholdTaxAmount = apPayment.TaxAmount;
                }
            }
               

            apPayment.NetAmount = apPayment.PrepaymentAmount
                                  + apPayment.TaxAmount
                                  - apPayment.WithholdTaxAmount
                                  - apPayment.WithholdingAmount;

            #endregion
            #region validated record

            var parent = _db.APPayments.Find(apPayment.ApPaymentId);
            if (parent != null)
            {
                if (parent.IsOnlineApproved)
                    ModelState.AddModelError("approved", "You cannot manipulate this record");

                #region detach local version
                var local = _db.Set<ApPayment>()
                    .Local
                    .FirstOrDefault(f => f.ApPaymentId == parent.ApPaymentId);
                if (local != null)
                {
                    _db.Entry(local).State = EntityState.Detached;
                }
                #endregion

            }


            #endregion

            if (apPayment.CollectedByPurchaser && apPayment.EmployeeId == 0)
                ModelState.AddModelError("zero", "System could not get purchaser information from purchase source.");

            if (ModelState.IsValid)
            {
                    var entity = new ApPayment
                    {
                        ApPaymentId = apPayment.ApPaymentId,
                        BranchId = apPayment.BranchId,
                        BusinessUnitId = apPayment.BusinessUnitId,
                        IsPrepayment = apPayment.IsPrepayment,
                        SupplierName = apPayment.SupplierName,
                        SupplierId = apPayment.SupplierId,
                        InvoiceNumber = apPayment.InvoiceNumber,
                        InvoiceAmount = apPayment.InvoiceAmount,
                        PaymentAmount = apPayment.PaymentAmount,
                        RemainingBalance = apPayment.RemainingBalance,
                        HasPrepayment = apPayment.HasPrepayment,
                        PrepaymentAmount = apPayment.PrepaymentAmount,
                        NetAmount = apPayment.NetAmount,
                        PaymentMethod = apPayment.PaymentMethod,
                        PaymentReference = apPayment.PaymentReference,
                        PaymentDate = apPayment.PaymentDate,
                        PoTypeEnum = apPayment.PoTypeEnum,
                        PoType = apPayment.PoType,
                        PurchaseReference = apPayment.PurchaseReference,
                        CollectedByPurchaser = apPayment.CollectedByPurchaser,
                        EmployeeId = apPayment.EmployeeId,
                        GLTaxRateID = apPayment.GLTaxRateID,
                        TaxAmount = apPayment.TaxAmount,
                        WithholdingType = apPayment.WithholdingType,
                        WithholdingAmount = apPayment.WithholdingAmount,
                        WithholdTax = apPayment.WithholdTax,
                        WithholdTaxAmount = apPayment.WithholdTaxAmount,
                        PrepaymentBalance = apPayment.PrepaymentBalance,
                        OutstandingBalance = apPayment.OutstandingBalance,
                        ForBranchId = apPayment.ForBranchId,
                        Remark = apPayment.Remark,
                        IsSendForApproval = apPayment.IsSendForApproval,
                        IsOnlineTransferred = apPayment.IsOnlineTransferred,
                        IsOnlineTransferredBy = apPayment.IsOnlineTransferredBy,
                        OnlineTransferredTime = apPayment.OnlineTransferredTime,
                        Status = apPayment.Status,
                        IsOnlineApproved = apPayment.IsOnlineApproved,
                        OnlineApprovedBy = apPayment.OnlineApprovedBy,
                        OnlineApprovedTime = apPayment.OnlineApprovedTime,
                        IsVoid = apPayment.IsVoid,
                        VoidBy = apPayment.VoidBy,
                        VoidTime = apPayment.VoidTime,
                        OrgBranchName = apPayment.OrgBranchName,
                        DateCreated = apPayment.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = apPayment.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.APPayments.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();


                    #region update and distribution

                    UpdatePaymentTotals(entity.ApPaymentId);
                    _distributionHelper.UpdateApAdvancePaymentDistribution(apPayment.ApPaymentId);

                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP advance payment", "Success", User.Identity.Name, $"updated payment {entity.ApPaymentId} ", Modules.Finance);
                #endregion

            }
            return Json(new[] { apPayment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPayments_Destroy([DataSourceRequest]DataSourceRequest request, ApPayment apPayment)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                var parent = _db.APPayments.Find(apPayment.ApPaymentId);
                if (parent != null)
                {
                    if (parent.IsOnlineApproved)
                    {
                        isvalid = false;
                        //ViewBag.Note = "You cannot manipulate this record";
                        //return View("Error");
                    }

                    #region detach local version
                    var local = _db.Set<ApPayment>()
                        .Local
                        .FirstOrDefault(f => f.ApPaymentId == parent.ApPaymentId);
                    if (local != null)
                    {
                        _db.Entry(local).State = EntityState.Detached;
                    }
                    #endregion

                }


                #endregion

                if (isvalid)
                {
                    var entity = new ApPayment
                    {
                        ApPaymentId = apPayment.ApPaymentId,
                        BranchId = apPayment.BranchId,
                        BusinessUnitId = apPayment.BusinessUnitId,
                        IsPrepayment = apPayment.IsPrepayment,
                        SupplierName = apPayment.SupplierName,
                        SupplierId = apPayment.SupplierId,
                        InvoiceNumber = apPayment.InvoiceNumber,
                        InvoiceAmount = apPayment.InvoiceAmount,
                        PaymentAmount = apPayment.PaymentAmount,
                        RemainingBalance = apPayment.RemainingBalance,
                        HasPrepayment = apPayment.HasPrepayment,
                        PrepaymentAmount = apPayment.PrepaymentAmount,
                        NetAmount = apPayment.NetAmount,
                        PaymentMethod = apPayment.PaymentMethod,
                        PaymentReference = apPayment.PaymentReference,
                        PaymentDate = apPayment.PaymentDate,
                        PoTypeEnum = apPayment.PoTypeEnum,
                        PoType = apPayment.PoType,
                        PurchaseReference = apPayment.PurchaseReference,
                        CollectedByPurchaser = apPayment.CollectedByPurchaser,
                        EmployeeId = apPayment.EmployeeId,
                        GLTaxRateID = apPayment.GLTaxRateID,
                        TaxAmount = apPayment.TaxAmount,
                        ForBranchId = apPayment.ForBranchId,
                        Remark = apPayment.Remark,
                        IsSendForApproval = apPayment.IsSendForApproval,
                        IsOnlineTransferred = apPayment.IsOnlineTransferred,
                        IsOnlineTransferredBy = apPayment.IsOnlineTransferredBy,
                        OnlineTransferredTime = apPayment.OnlineTransferredTime,
                        Status = apPayment.Status,
                        IsOnlineApproved = apPayment.IsOnlineApproved,
                        OnlineApprovedBy = apPayment.OnlineApprovedBy,
                        OnlineApprovedTime = apPayment.OnlineApprovedTime,
                        IsVoid = apPayment.IsVoid,
                        VoidBy = apPayment.VoidBy,
                        VoidTime = apPayment.VoidTime,
                        OrgBranchName = apPayment.OrgBranchName,
                        DateCreated = apPayment.DateCreated,
                        LastModified = apPayment.LastModified,
                        CreatedBy = apPayment.CreatedBy,
                        ModifiedBy = apPayment.ModifiedBy
                    };

                    _db.APPayments.Attach(entity);
                    _db.APPayments.Remove(entity);
                    _db.SaveChanges();


                    #region update and distribution

                    UpdatePaymentTotals(entity.ApPaymentId);
                    _distributionHelper.UpdateApAdvancePaymentDistribution(apPayment.ApPaymentId);

                    #endregion
                }
            }
            return Json(new[] { apPayment }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView, FinanceConstants.AccountPayableApprover, FinanceConstants.AccountPayablePayment)]
       
        private void UpdatePaymentTotals(int? id)
        {
            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApPayment record = _db.APPayments.Find(id);
            if (record == null)
            {
                //return HttpNotFound();
            }

            else
            {
                #region update parent total

                #endregion
            }


        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.APPayments.Find(id);
            #region check segment length validation

            var distributions = _db.APPaymentDistributions.Where(x => x.ApPaymentId == id).ToList();
            AccountHelper accountHelper = new AccountHelper();
            foreach (var dist in distributions)
            {
                string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                if (!String.IsNullOrEmpty(message))
                {
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #region debit credit validation
                if (dist.Debit != 0 && dist.Credit != 0)
                {
                    message = $"{dist.AccountCode} Account has both debit and credit value.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit == 0 && dist.Credit == 0)
                {
                    message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit > 0 && dist.Credit > 0)
                {
                    message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                if (dist.Debit < 0 || dist.Credit < 0)
                {
                    message = $"{dist.AccountCode} Negative values are not allowed.";
                    var formattedData = new
                    {
                        Message = message,
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

            }


            #endregion
            decimal sumTotalDisDebit = 0;
            decimal sumTotalDisCredit = 0;
            
            try
            {
                sumTotalDisDebit = _db.APPaymentDistributions.Where(x => x.ApPaymentId == record.ApPaymentId).Sum(x => x.Debit);
                sumTotalDisCredit = _db.APPaymentDistributions.Where(x => x.ApPaymentId == record.ApPaymentId).Sum(x => x.Credit);
            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Please add distribution and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
           
            if (sumTotalDisDebit != sumTotalDisCredit)
            {
                var formattedData = new
                {
                    Message = "Total Debit and Total Credit is not equal.Please correct it and try again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            
            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (!record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.APPayments.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        public JsonResult TransferDistribution(string id, string description)
        {
            int myid = int.Parse(id);
            var record = _db.APPayments.Find(myid);
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsOnlineTransferred)
            {
                #region check if already transffered 
                var exist = _db.GLRecordJournals
                    .Where(x => x.Source == JournalSources.AP_PrePayment.ToString())
                    .Where(x => x.SourceId == record.ApPaymentId.ToString())
                    .FirstOrDefault();
                if (exist != null)
                {
                    var formattedData = new
                    {
                        Message = "The record is already transffered, it can't be retransferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.ApPaymentDistribution)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
                    {
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    #region debit credit validation
                    if (dist.Debit != 0 && dist.Credit != 0)
                    {
                        message = $"{dist.AccountCode} Account has both debit and credit value.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit == 0 && dist.Credit == 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit > 0 && dist.Credit > 0)
                    {
                        message = $"{dist.AccountCode} Both debit and credit can't have non zero value at the same time.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                    if (dist.Debit < 0 || dist.Credit < 0)
                    {
                        message = $"{dist.AccountCode} Negative values are not allowed.";
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    #endregion

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var distribution = record.ApPaymentDistribution.ToList();
                if (distribution.Any())
                {
                    debit = distribution.Sum(x => x.Debit);
                    credit = distribution.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is not balanced.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit == 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is zero.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region send to GL

                var year = _db.GLFiscalYears.FirstOrDefault();
                string glyear = year.Name;
                string glperiod = year.GLPeriod.FirstOrDefault().Name;
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.PaymentDate)
                    .Where(x => x.DateTo >= record.PaymentDate).ToList();
                if (period.Any())
                {

                    if (period.Count() == 1)
                    {
                        var myPeriod = period.SingleOrDefault();
                        glperiod = myPeriod.Name;
                        glyear = myPeriod.GlFiscalYear.Name.ToString();
                        if (myPeriod.Status == FiscalYearStatus.Close)
                        {
                            var formattedData = new
                            {
                                Message = "The period " + glperiod + " is closed,the record cannot be transfered to closed period.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (!period.Any())
                    {
                        var formattedData = new
                        {
                            Message = "There are no periods defined that contain the payment date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    else if (period.Any())
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the payment date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }

                #endregion
               
                if (record.ApPaymentDistribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "AP Payment",
                        Description = !String.IsNullOrEmpty(description) ? description : "AP Payment",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "AP Payment",
                        SourcesName = "AP",
                        JournalNumber = "APPAY." + record.ApPaymentId.ToString(),
                        SourceId = record.ApPaymentId.ToString(),
                        Source = JournalSources.AP_PrePayment.ToString(),
                        EffectiveDate = record.PaymentDate.Date,
                        OrgBranchName = record.OrgBranchName,
                        Status = "Approved",
                        Remark = record.Remark,
                        PaymentReference = record.PaymentReference,
                        ReferenceDoc = "",
                        InvoiceReferenceDoc = record.InvoiceNumber,
                        BranchId = record.BranchId,
                        BusinessUnitId=record.BusinessUnitId,
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in record.ApPaymentDistribution)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AdditionalDescription = i.AdditionalDescription,
                        };
                        _db.GLRecordJournalDetails.Add(journalDetail);
                        _db.SaveChanges();
                        #endregion
                    }

                    #region update record

                    record.IsOnlineTransferred = true;
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();

                    #endregion
                    #region split journal
                    var journaldetails = _db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion

                    var formattedData = new
                    {
                        Message = "Successfully transfered.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
                #endregion
               
            }
            else
            {
                var formattedData = new
                {
                    Message = "You cannot transfer this transaction, it is already transfered.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;
        }


        [AuthorizeRoles(FinanceConstants.AccountPayableVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.APPayments.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid && !record.IsOnlineTransferred)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.APPayments.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

                if (record.PoTypeEnum == PurchaseType.PurchaseOrder)
                {
                    #region purchase order
                    var purchaseOrder = _db.PurchaseOrders.Find(int.Parse(record.PurchaseReference));
                    if (purchaseOrder != null)
                    {
                        purchaseOrder.Status = "Advance Payment Void";
                        _db.PurchaseOrders.Attach(purchaseOrder);
                        _db.Entry(record).State = EntityState.Modified;
                        _db.SaveChanges();

                        if (!purchaseOrder.IsManual)
                        {
                            var proforma = _db.ProformaPurchases.Find(purchaseOrder.ProformaPurchaseID);
                            if (proforma != null)
                            {
                                proforma.Status = "Advance Payment Void";
                                _db.ProformaPurchases.Attach(proforma);
                            }

                            _db.Entry(record).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    #endregion 
                }

                else if (record.PoTypeEnum == PurchaseType.TenderAward)
                {
                    #region award

                    var bidAwardedSupplier = _db.BidAwardedSuppliers.Find(int.Parse(record.PurchaseReference));

                    if (bidAwardedSupplier != null)
                    {
                        var lot = _db.Lots.Find(bidAwardedSupplier.LotID);
                        if (lot != null)
                        {
                            var tender = _db.Tenders.Find(lot.TenderID);

                            if (tender != null)
                            {
                                tender.Status = "Advance Payment Void";
                                _db.Tenders.Attach(tender);
                                _db.Entry(record).State = EntityState.Modified;
                                _db.SaveChanges();
                            }
                        }
                    }
                    #endregion 
                }

                //copy
                //CopyRecord(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "advance payment", "Success", User.Identity.Name, $"void advance payment {id} ", Modules.Finance);
                #endregion

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            
            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void.You can not void again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsOnlineTransferred)
            {
                var formattedData = new
                {
                    Message = "The record is transferred to GL.You can not void .",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
       
        public ActionResult PurchaseHeader_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<PurchaseViewModel> purchases = new List<PurchaseViewModel>();
           
            var appayment = _db.APPayments.Find(id);

            if (!String.IsNullOrWhiteSpace(appayment?.PurchaseReference))
            {
                if (appayment.PoTypeEnum == PurchaseType.PurchaseOrder)
                {
                    #region purchase order

                    var purchaseOrder = _db.PurchaseOrders.Find(int.Parse(appayment.PurchaseReference));
                    if (purchaseOrder != null)
                    {
                        PurchaseViewModel record = new PurchaseViewModel
                        {
                            PurchaseId = purchaseOrder.PurchaseOrderID,
                            PoTypeEnum = appayment.PoTypeEnum,
                            ProcurementOfficer = purchaseOrder.ProcurementOfficer,
                            SupplierId = purchaseOrder.SupplierId,
                            IsNewSupplier = purchaseOrder.IsNewSupplier,
                            SupplierName = purchaseOrder.SupplierName,
                            ProcurementMethod = purchaseOrder.ProcurementMethod,
                            PurchaseRequisitionID = purchaseOrder.PurchaseRequisitionID,
                            ProformaPurchaseID = purchaseOrder.ProformaPurchaseID,
                            OrgStructureId = purchaseOrder.OrgStructureId,
                            OrderDate = purchaseOrder.OrderDate,
                            DeliveryDate = purchaseOrder.DeliveryDate,
                            DeliveryPlace = purchaseOrder.DeliveryPlace,
                            Status = purchaseOrder.Status,
                            Remark = purchaseOrder.Remark,

                            IsSendForApproval = purchaseOrder.IsSendForApproval,
                            SendForApprovalBy = purchaseOrder.SendForApprovalBy,
                            SendForApprovalTime = purchaseOrder.SendForApprovalTime,

                            IsOnlineApproved = purchaseOrder.IsOnlineApproved,
                            OnlineApprovedBy = purchaseOrder.OnlineApprovedBy,
                            OnlineApprovedTime = purchaseOrder.OnlineApprovedTime,

                            IsVoid = purchaseOrder.IsVoid,
                            VoidBy = purchaseOrder.VoidBy,
                            VoidTime = purchaseOrder.VoidTime,

                            DateCreated = purchaseOrder.DateCreated,
                            LastModified = purchaseOrder.LastModified,
                            CreatedBy = purchaseOrder.CreatedBy,
                            ModifiedBy = purchaseOrder.ModifiedBy
                        };
                        purchases.Add(record);
                    }

                    #endregion
                }
                else if (appayment.PoTypeEnum == PurchaseType.TenderAward)
                {
                    #region award

                    var bidAwardedSupplier = _db.BidAwardedSuppliers.Find(int.Parse(appayment.PurchaseReference));

                    if (bidAwardedSupplier != null)
                    {
                        var lot = _db.Lots.Find(bidAwardedSupplier.LotID);
                        if (lot != null)
                        {
                            var tender = _db.Tenders.Find(lot.TenderID);

                            if (tender != null)
                            {
                                PurchaseViewModel record = new PurchaseViewModel
                                {
                                    PurchaseId = bidAwardedSupplier.BidAwardedSupplierID,
                                    PoTypeEnum = appayment.PoTypeEnum,
                                    ProcurementOfficer = tender.ProcurementOfficer,
                                    TenderRef = tender.TenderReference,
                                    SupplierId = bidAwardedSupplier.SupplierId,
                                    IsNewSupplier = bidAwardedSupplier.IsNewSupplier,
                                    SupplierName = bidAwardedSupplier.SupplierName,
                                    ProcurementMethod = ProcurementMethod.Tender,
                                    OrderDate = bidAwardedSupplier.AwardDate,
                                    Status = tender.Status,
                                    Remark = bidAwardedSupplier.Remark,

                                    IsSendForApproval = tender.IsSendForApproval,
                                    SendForApprovalBy = tender.SendForApprovalBy,
                                    SendForApprovalTime = tender.SendForApprovalTime,

                                    IsOnlineApproved = tender.IsOnlineApproved,
                                    OnlineApprovedBy = tender.OnlineApprovedBy,
                                    OnlineApprovedTime = tender.OnlineApprovedTime,

                                    IsVoid = tender.IsVoid,
                                    VoidBy = tender.VoidBy,
                                    VoidTime = tender.VoidTime,

                                    DateCreated = bidAwardedSupplier.DateCreated,
                                    LastModified = bidAwardedSupplier.LastModified,
                                    CreatedBy = bidAwardedSupplier.CreatedBy,
                                    ModifiedBy = bidAwardedSupplier.ModifiedBy
                                };
                                purchases.Add(record);
                            }
                        }
                    }

                    #endregion
                }
            }

            IQueryable<PurchaseViewModel> purchaseViewModels = purchases.AsQueryable();
            DataSourceResult result = purchaseViewModels.ToDataSourceResult(request, purchaseViewModel => new {
                PurchaseId = purchaseViewModel.PurchaseId,
                TenderRef = purchaseViewModel.TenderRef,
                PoTypeEnum = purchaseViewModel.PoTypeEnum,
                ProcurementOfficer = purchaseViewModel.ProcurementOfficer,
                SupplierId = purchaseViewModel.SupplierId,
                IsNewSupplier = purchaseViewModel.IsNewSupplier,
                SupplierName = purchaseViewModel.SupplierName,
                ProcurementMethod = purchaseViewModel.ProcurementMethod,
                PurchaseRequisitionID = purchaseViewModel.PurchaseRequisitionID,
                ProformaPurchaseID = purchaseViewModel.ProformaPurchaseID,
                OrgStructureId = purchaseViewModel.OrgStructureId,
                OrderDate = purchaseViewModel.OrderDate,
                DeliveryDate = purchaseViewModel.DeliveryDate,
                DeliveryPlace = purchaseViewModel.DeliveryPlace,
                Status = purchaseViewModel.Status,
                Remark = purchaseViewModel.Remark,

                IsSendForApproval = purchaseViewModel.IsSendForApproval,
                SendForApprovalBy = purchaseViewModel.SendForApprovalBy,
                SendForApprovalTime = purchaseViewModel.SendForApprovalTime,

                IsOnlineApproved = purchaseViewModel.IsOnlineApproved,
                OnlineApprovedBy = purchaseViewModel.OnlineApprovedBy,
                OnlineApprovedTime = purchaseViewModel.OnlineApprovedTime,

                IsVoid = purchaseViewModel.IsVoid,
                VoidBy = purchaseViewModel.VoidBy,
                VoidTime = purchaseViewModel.VoidTime,

                DateCreated = purchaseViewModel.DateCreated,
                LastModified = purchaseViewModel.LastModified,
                CreatedBy = purchaseViewModel.CreatedBy,
                ModifiedBy = purchaseViewModel.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult PurchaseItem_Read([DataSourceRequest]DataSourceRequest request, int id, PurchaseType type)
        {
            List<PurchaseItemViewModel> purchases = new List<PurchaseItemViewModel>(); 

            if (type == PurchaseType.PurchaseOrder)
            {
                #region purchase order

                var purchaseOrder = _db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == id).ToList();
                foreach (var purchaseOrderItem in purchaseOrder)
                {

                    PurchaseItemViewModel record = new PurchaseItemViewModel
                    {
                        CatCode = purchaseOrderItem.CatCode,
                        CategoryName = purchaseOrderItem.CategoryName,
                        ItmCode = purchaseOrderItem.ItmCode,
                        ItemName = purchaseOrderItem.ItemName,
                        PartNo = purchaseOrderItem.PartNo,
                        Quantity = purchaseOrderItem.Quantity,
                        IsApproved = purchaseOrderItem.IsApproved,
                        ItemSpecification = purchaseOrderItem.ItemSpecification,
                        UoM = purchaseOrderItem.UoM,
                        ItemType = purchaseOrderItem.ItemType,
                        UnitPrice = purchaseOrderItem.UnitPrice,
                        GLTaxRateID = purchaseOrderItem.GLTaxRateID,
                        TaxAmount = purchaseOrderItem.TaxAmount,
                        SubTotal = purchaseOrderItem.SubTotal,
                        Total = purchaseOrderItem.Total,
                        Remark = purchaseOrderItem.Remark,
                        OrgBranchName = purchaseOrderItem.OrgBranchName,
                        DateCreated = purchaseOrderItem.DateCreated,
                        LastModified = purchaseOrderItem.LastModified,
                        CreatedBy = purchaseOrderItem.CreatedBy,
                        ModifiedBy = purchaseOrderItem.ModifiedBy
                    };
                    purchases.Add(record);
                }

                #endregion

            }
            else if (type == PurchaseType.TenderAward)
            {
                #region award

                var awardedSuppliers = _db.BidAwardedSupplierItems.Where(x => x.BidAwardedSupplierID == id).ToList();
                foreach (var items in awardedSuppliers)
                {

                    PurchaseItemViewModel record = new PurchaseItemViewModel
                    {
                        CatCode = items.CatCode,
                        CategoryName = items.CategoryName,
                        ItmCode = items.ItmCode,
                        ItemName = items.ItemName,
                        PartNo = items.PartNo,
                        Quantity = items.Quanity,
                        IsApproved = items.IsApproved,
                        ItemSpecification = items.ItemSpecification,
                        UoM = items.UoM,
                        ItemType = items.ItemType,
                        UnitPrice = items.FinalUPrice,
                        GLTaxRateID = items.GLTaxRateID,
                        TaxAmount = items.TaxAmount,
                        SubTotal = items.FinalUPrice* items.Quanity,
                        Total = items.TPrice,
                        Remark = items.BidAwardedSupplier.Remark,
                        OrgBranchName = items.OrgBranchName,
                        DateCreated = items.DateCreated,
                        LastModified = items.LastModified,
                        CreatedBy = items.CreatedBy,
                        ModifiedBy = items.ModifiedBy
                    };
                    purchases.Add(record);
                }

                #endregion

            }

            IQueryable<PurchaseItemViewModel> purchaseItemViewModels = purchases.AsQueryable();
            DataSourceResult result = purchaseItemViewModels.ToDataSourceResult(request, itemViewModels => new {
                CatCode = itemViewModels.CatCode + "-" + itemViewModels.CategoryName,
                CategoryName = itemViewModels.CategoryName,
                ItmCode = itemViewModels.ItmCode+"-"+ itemViewModels.ItemName,
                ItemName = itemViewModels.ItemName,
                PartNo = itemViewModels.PartNo,
                Quantity = itemViewModels.Quantity,
                IsApproved = itemViewModels.IsApproved,
                ItemSpecification = itemViewModels.ItemSpecification,
                UoM = itemViewModels.UoM,
                ItemType = itemViewModels.ItemType,
                UnitPrice = itemViewModels.UnitPrice,
                GLTaxRateID = itemViewModels.GLTaxRateID,
                TaxAmount = itemViewModels.TaxAmount,
                SubTotal = itemViewModels.SubTotal,
                Total = itemViewModels.Total,
                Remark = itemViewModels.Remark,
                OrgBranchName = itemViewModels.OrgBranchName,
                DateCreated = itemViewModels.DateCreated,
                LastModified = itemViewModels.LastModified,
                CreatedBy = itemViewModels.CreatedBy,
                ModifiedBy = itemViewModels.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult PurchasePaymentSchedule_Read([DataSourceRequest]DataSourceRequest request, int id, PurchaseType type)
        {
            List<PurchasePaymentScheduleViewModel> purchases = new List<PurchasePaymentScheduleViewModel>();

            if (type == PurchaseType.PurchaseOrder)
            {
                var paymentSchedules = _db.PoPaymentSchedules.Where(x => x.PurchaseOrderID == id).ToList();
                foreach (var paymentSchedule in paymentSchedules)
                {

                    PurchasePaymentScheduleViewModel record = new PurchasePaymentScheduleViewModel
                    {
                        PaymentType = paymentSchedule.PaymentType,
                        Percentage = paymentSchedule.Percentage,
                        Remark = paymentSchedule.Remark,
                        OrgBranchName = paymentSchedule.OrgBranchName,
                        DateCreated = paymentSchedule.DateCreated,
                        LastModified = paymentSchedule.LastModified,
                        CreatedBy = paymentSchedule.CreatedBy,
                        ModifiedBy = paymentSchedule.ModifiedBy
                    };
                    purchases.Add(record);
                }
            }

            IQueryable<PurchasePaymentScheduleViewModel> paymentScheduleViewModels = purchases.AsQueryable();
            DataSourceResult result = paymentScheduleViewModels.ToDataSourceResult(request, paymentScheduleViewModel => new {
                PaymentType = paymentScheduleViewModel.PaymentType,
                Percentage = paymentScheduleViewModel.Percentage,
                Remark = paymentScheduleViewModel.Remark,
                OrgBranchName = paymentScheduleViewModel.OrgBranchName,
                DateCreated = paymentScheduleViewModel.DateCreated,
                LastModified = paymentScheduleViewModel.LastModified,
                CreatedBy = paymentScheduleViewModel.CreatedBy,
                ModifiedBy = paymentScheduleViewModel.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult PurchaseValidation_Read([DataSourceRequest]DataSourceRequest request, int id, PurchaseType type)
        {
            List<PurchaseValidationViewModel> purchases = new List<PurchaseValidationViewModel>();

            if (type == PurchaseType.PurchaseOrder)
            {
                #region purchase order

                var orderValidations = _db.PurchaseOrderValidations.Where(x => x.PurchaseOrderID == id).ToList();
                foreach (var validation in orderValidations)
                {

                    PurchaseValidationViewModel record = new PurchaseValidationViewModel
                    {
                        Status = validation.Status,
                        Remark = validation.Remark,
                        OrgBranchName = validation.OrgBranchName,
                        DateCreated = validation.DateCreated,
                        LastModified = validation.LastModified,
                        CreatedBy = validation.CreatedBy,
                        ModifiedBy = validation.ModifiedBy
                    };
                    purchases.Add(record);
                }

                #endregion
            }
            if (type == PurchaseType.TenderAward)
            {
                #region tender award

                var bidAwardedSupplier = _db.BidAwardedSuppliers.Find(id);

                if (bidAwardedSupplier != null)
                {
                    var lot = _db.Lots.Find(bidAwardedSupplier.LotID);
                    if (lot != null)
                    {
                        var tender = _db.Tenders.Find(lot.TenderID);

                        var tenderValidations = _db.TenderValidations.Where(x => x.TenderID == tender.TenderID).ToList();
                        foreach (var validation in tenderValidations)
                        {

                            PurchaseValidationViewModel record = new PurchaseValidationViewModel
                            {
                                Status = validation.Status,
                                Remark = validation.Remark,
                                OrgBranchName = validation.OrgBranchName,
                                DateCreated = validation.DateCreated,
                                LastModified = validation.LastModified,
                                CreatedBy = validation.CreatedBy,
                                ModifiedBy = validation.ModifiedBy
                            };
                            purchases.Add(record);
                        }
                    }
                }

               

                #endregion
            }

            IQueryable<PurchaseValidationViewModel> paymentScheduleViewModels = purchases.AsQueryable();
            DataSourceResult result = paymentScheduleViewModels.ToDataSourceResult(request, paymentScheduleViewModel => new {
                Status = paymentScheduleViewModel.Status,
                Remark = paymentScheduleViewModel.Remark,
                OrgBranchName = paymentScheduleViewModel.OrgBranchName,
                DateCreated = paymentScheduleViewModel.DateCreated,
                LastModified = paymentScheduleViewModel.LastModified,
                CreatedBy = paymentScheduleViewModel.CreatedBy,
                ModifiedBy = paymentScheduleViewModel.ModifiedBy
            });

            return Json(result);
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
