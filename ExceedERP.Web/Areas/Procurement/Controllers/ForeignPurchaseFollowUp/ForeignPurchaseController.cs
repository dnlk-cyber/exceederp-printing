using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.ForeignPurchaseFollowUp
{
    public class ForeignPurchaseController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseUser, ProcurementRoles.ProcurementForeignPurchaseApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var tax = _db.GLTaxRates;
            ViewData["tax"] = tax;
            var costCenter = _db.BranchCostCenter;

            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.MainCompanyStructure.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;

        }

        [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseUser, ProcurementRoles.ProcurementForeignPurchaseApprover)]
        public ActionResult ForeignPurchases_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Core.Domain.Procurement.ForeignPurchase.ForeignPurchase> foreignpurchases = _db.ForeignPurchases;
            DataSourceResult result = foreignpurchases.ToDataSourceResult(request, foreignPurchase => new
            {
                ForeignPurchaseId = foreignPurchase.ForeignPurchaseId,
                TenderRef = foreignPurchase.TenderRef,
                PurchaseRef = foreignPurchase.PurchaseRef,
                Supplier = foreignPurchase.Supplier,
                ItemDesc = foreignPurchase.ItemDesc,
                OrgStructureId = foreignPurchase.OrgStructureId,
                Quantity = foreignPurchase.Quantity,
                ProcurementMethod = foreignPurchase.ProcurementMethod,
                FOBMethod = foreignPurchase.FOBMethod,
                ApprovalDate = foreignPurchase.ApprovalDate,
                OpenigDate = foreignPurchase.OpenigDate,
                TechnicalDocSendDate = foreignPurchase.TechnicalDocSendDate,
                FinancialOpenigDate = foreignPurchase.FinancialOpenigDate,
                MinuteApprovedDate = foreignPurchase.MinuteApprovedDate,
                AwardDate = foreignPurchase.AwardDate,
                ContractDate = foreignPurchase.ContractDate,
                PaymentMethod = foreignPurchase.PaymentMethod,
                LcTtCadRef = foreignPurchase.LcTtCadRef,
                Advance = foreignPurchase.Advance,
                LC = foreignPurchase.LC,
                Retention = foreignPurchase.Retention,
                PaymentOpenDate = foreignPurchase.PaymentOpenDate,
                PaymentExtensionDate = foreignPurchase.PaymentExtensionDate,
                DocReceivedFromBankDate = foreignPurchase.DocReceivedFromBankDate,
                ClearanceStartDate = foreignPurchase.ClearanceStartDate,
                TransitProcessDate = foreignPurchase.TransitProcessDate,
                DeliveryDate = foreignPurchase.DeliveryDate,
                DeliveryNo = foreignPurchase.DeliveryNo,
                UnitPrice = foreignPurchase.UnitPrice,
                ForeignCurrencyProcessId = foreignPurchase.ForeignCurrencyProcessId,

                Status =foreignPurchase.Status,
                IsVoid =foreignPurchase.IsVoid,
                VoidBy =foreignPurchase.VoidBy,
                VoidTime =foreignPurchase.VoidTime,
                IsOnlineApproved =foreignPurchase.IsOnlineApproved,
                OnlineApprovedBy =foreignPurchase.OnlineApprovedBy,
                OnlineApprovedTime =foreignPurchase.OnlineApprovedTime,
                IsSendForApproval =foreignPurchase.IsSendForApproval,
                SendForApprovalBy =foreignPurchase.SendForApprovalBy,
                SendForApprovalTime =foreignPurchase.SendForApprovalTime,
                IsOnlineTransferred =foreignPurchase.IsOnlineTransferred,
                IsOnlineTransferredBy =foreignPurchase.IsOnlineTransferredBy,
                OnlineTransferredTime =foreignPurchase.OnlineTransferredTime,


                DateCreated =foreignPurchase.DateCreated,
                LastModified =foreignPurchase.LastModified,
                CreatedBy =foreignPurchase.CreatedBy,
                ModifiedBy = foreignPurchase.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchases_Create([DataSourceRequest]DataSourceRequest request, Core.Domain.Procurement.ForeignPurchase.ForeignPurchase foreignPurchase)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Procurement.ForeignPurchase.ForeignPurchase
                {
                    TenderRef = foreignPurchase.TenderRef,
                    PurchaseRef = foreignPurchase.PurchaseRef,
                    Supplier = foreignPurchase.Supplier,
                    ItemDesc = foreignPurchase.ItemDesc,
                    OrgStructureId = foreignPurchase.OrgStructureId,
                    Quantity = foreignPurchase.Quantity,
                    ProcurementMethod = foreignPurchase.ProcurementMethod,
                    FOBMethod = foreignPurchase.FOBMethod,
                    ApprovalDate = foreignPurchase.ApprovalDate,
                    OpenigDate = foreignPurchase.OpenigDate,
                    TechnicalDocSendDate = foreignPurchase.TechnicalDocSendDate,
                    FinancialOpenigDate = foreignPurchase.FinancialOpenigDate,
                    MinuteApprovedDate = foreignPurchase.MinuteApprovedDate,
                    AwardDate = foreignPurchase.AwardDate,
                    ContractDate = foreignPurchase.ContractDate,
                    PaymentMethod = foreignPurchase.PaymentMethod,
                    LcTtCadRef = foreignPurchase.LcTtCadRef,
                    Advance = foreignPurchase.Advance,
                    PaymentOpenDate = foreignPurchase.PaymentOpenDate,
                    PaymentExtensionDate = foreignPurchase.PaymentExtensionDate,
                    DocReceivedFromBankDate = foreignPurchase.DocReceivedFromBankDate,
                    ClearanceStartDate = foreignPurchase.ClearanceStartDate,
                    TransitProcessDate = foreignPurchase.TransitProcessDate,
                    DeliveryDate = foreignPurchase.DeliveryDate,
                    DeliveryNo = foreignPurchase.DeliveryNo,
                    UnitPrice = foreignPurchase.UnitPrice,
                    ForeignCurrencyProcessId = foreignPurchase.ForeignCurrencyProcessId,
                    Remark = foreignPurchase.Remark,
                    OrgBranchName = foreignPurchase.OrgBranchName,

                    Status = foreignPurchase.Status,
                    IsVoid = foreignPurchase.IsVoid,
                    VoidBy = foreignPurchase.VoidBy,
                    VoidTime = foreignPurchase.VoidTime,
                    IsOnlineApproved = foreignPurchase.IsOnlineApproved,
                    OnlineApprovedBy = foreignPurchase.OnlineApprovedBy,
                    OnlineApprovedTime = foreignPurchase.OnlineApprovedTime,
                    IsSendForApproval = foreignPurchase.IsSendForApproval,
                    SendForApprovalBy = foreignPurchase.SendForApprovalBy,
                    SendForApprovalTime = foreignPurchase.SendForApprovalTime,
                    IsOnlineTransferred = foreignPurchase.IsOnlineTransferred,
                    IsOnlineTransferredBy = foreignPurchase.IsOnlineTransferredBy,
                    OnlineTransferredTime = foreignPurchase.OnlineTransferredTime,


                    DateCreated = DateTime.Now,
                    LastModified = foreignPurchase.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = foreignPurchase.ModifiedBy
                };

               _db.ForeignPurchases.Add(entity);
               _db.SaveChanges();
                foreignPurchase.ForeignPurchaseId = entity.ForeignPurchaseId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase ", "Success", User.Identity.Name, $"created  {entity.ForeignPurchaseId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchase }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchases_Update([DataSourceRequest]DataSourceRequest request, Core.Domain.Procurement.ForeignPurchase.ForeignPurchase foreignPurchase)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Procurement.ForeignPurchase.ForeignPurchase
                {
                    ForeignPurchaseId = foreignPurchase.ForeignPurchaseId,
                    TenderRef = foreignPurchase.TenderRef,
                    PurchaseRef = foreignPurchase.PurchaseRef,
                    Supplier = foreignPurchase.Supplier,
                    ItemDesc = foreignPurchase.ItemDesc,
                    OrgStructureId = foreignPurchase.OrgStructureId,
                    Quantity = foreignPurchase.Quantity,
                    ProcurementMethod = foreignPurchase.ProcurementMethod,
                    FOBMethod = foreignPurchase.FOBMethod,
                    ApprovalDate = foreignPurchase.ApprovalDate,
                    OpenigDate = foreignPurchase.OpenigDate,
                    TechnicalDocSendDate = foreignPurchase.TechnicalDocSendDate,
                    FinancialOpenigDate = foreignPurchase.FinancialOpenigDate,
                    MinuteApprovedDate = foreignPurchase.MinuteApprovedDate,
                    AwardDate = foreignPurchase.AwardDate,
                    ContractDate = foreignPurchase.ContractDate,
                    PaymentMethod = foreignPurchase.PaymentMethod,
                    LcTtCadRef = foreignPurchase.LcTtCadRef,
                    Advance = foreignPurchase.Advance,
                    LC = foreignPurchase.LC,
                    Retention = foreignPurchase.Retention,
                    PaymentOpenDate = foreignPurchase.PaymentOpenDate,
                    PaymentExtensionDate = foreignPurchase.PaymentExtensionDate,
                    DocReceivedFromBankDate = foreignPurchase.DocReceivedFromBankDate,
                    ClearanceStartDate = foreignPurchase.ClearanceStartDate,
                    TransitProcessDate = foreignPurchase.TransitProcessDate,
                    DeliveryDate = foreignPurchase.DeliveryDate,
                    DeliveryNo = foreignPurchase.DeliveryNo,
                    UnitPrice = foreignPurchase.UnitPrice,
                    ForeignCurrencyProcessId = foreignPurchase.ForeignCurrencyProcessId,
                    Remark = foreignPurchase.Remark,
                    OrgBranchName = foreignPurchase.OrgBranchName,


                    Status = foreignPurchase.Status,
                    IsVoid = foreignPurchase.IsVoid,
                    VoidBy = foreignPurchase.VoidBy,
                    VoidTime = foreignPurchase.VoidTime,
                    IsOnlineApproved = foreignPurchase.IsOnlineApproved,
                    OnlineApprovedBy = foreignPurchase.OnlineApprovedBy,
                    OnlineApprovedTime = foreignPurchase.OnlineApprovedTime,
                    IsSendForApproval = foreignPurchase.IsSendForApproval,
                    SendForApprovalBy = foreignPurchase.SendForApprovalBy,
                    SendForApprovalTime = foreignPurchase.SendForApprovalTime,
                    IsOnlineTransferred = foreignPurchase.IsOnlineTransferred,
                    IsOnlineTransferredBy = foreignPurchase.IsOnlineTransferredBy,
                    OnlineTransferredTime = foreignPurchase.OnlineTransferredTime,


                    DateCreated = foreignPurchase.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = foreignPurchase.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

               _db.ForeignPurchases.Attach(entity);
               _db.Entry(entity).State = EntityState.Modified;
               _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase ", "Success", User.Identity.Name, $"updated  {entity.ForeignPurchaseId}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchase }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchases_Destroy([DataSourceRequest]DataSourceRequest request, Core.Domain.Procurement.ForeignPurchase.ForeignPurchase foreignPurchase)
        {
            if (ModelState.IsValid)
            {
                var entity = new Core.Domain.Procurement.ForeignPurchase.ForeignPurchase
                {
                    ForeignPurchaseId = foreignPurchase.ForeignPurchaseId,
                    TenderRef = foreignPurchase.TenderRef,
                    PurchaseRef = foreignPurchase.PurchaseRef,
                    Supplier = foreignPurchase.Supplier,
                    ItemDesc = foreignPurchase.ItemDesc,
                    OrgStructureId = foreignPurchase.OrgStructureId,
                    Quantity = foreignPurchase.Quantity,
                    ProcurementMethod = foreignPurchase.ProcurementMethod,
                    FOBMethod = foreignPurchase.FOBMethod,
                    ApprovalDate = foreignPurchase.ApprovalDate,
                    OpenigDate = foreignPurchase.OpenigDate,
                    TechnicalDocSendDate = foreignPurchase.TechnicalDocSendDate,
                    FinancialOpenigDate = foreignPurchase.FinancialOpenigDate,
                    MinuteApprovedDate = foreignPurchase.MinuteApprovedDate,
                    AwardDate = foreignPurchase.AwardDate,
                    ContractDate = foreignPurchase.ContractDate,
                    PaymentMethod = foreignPurchase.PaymentMethod,
                    LcTtCadRef = foreignPurchase.LcTtCadRef,
                    Advance = foreignPurchase.Advance,
                    LC = foreignPurchase.LC,
                    Retention = foreignPurchase.Retention,
                    PaymentOpenDate = foreignPurchase.PaymentOpenDate,
                    PaymentExtensionDate = foreignPurchase.PaymentExtensionDate,
                    DocReceivedFromBankDate = foreignPurchase.DocReceivedFromBankDate,
                    ClearanceStartDate = foreignPurchase.ClearanceStartDate,
                    TransitProcessDate = foreignPurchase.TransitProcessDate,
                    DeliveryDate = foreignPurchase.DeliveryDate,
                    DeliveryNo = foreignPurchase.DeliveryNo,
                    UnitPrice = foreignPurchase.UnitPrice,
                    ForeignCurrencyProcessId = foreignPurchase.ForeignCurrencyProcessId,
                    Remark = foreignPurchase.Remark,
                    OrgBranchName = foreignPurchase.OrgBranchName,
                    DateCreated = foreignPurchase.DateCreated,
                    LastModified = foreignPurchase.LastModified,
                    CreatedBy = foreignPurchase.CreatedBy,
                    ModifiedBy = foreignPurchase.ModifiedBy
                };

               _db.ForeignPurchases.Attach(entity);
               _db.ForeignPurchases.Remove(entity);
               _db.SaveChanges();
            }

            return Json(new[] { foreignPurchase }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementForeignPurchaseUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.ForeignPurchases.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.ForeignPurchaseFollowUpItems.Count > 0)
            {
                #region validate price and tax

                foreach (var item in record.ForeignPurchaseFollowUpItems)
                {
                    if (item.AdditionalUnitCost <= 0)
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill additional unit cost allocation for all items.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                _db.ForeignPurchases.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
               
                var formattedData = new
                {
                    Message = "Successfully Sent.",
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

            if (record.ForeignPurchaseFollowUpItems.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
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
            return null;
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementVoid)]
        public JsonResult Void(int id)
        {
            var record = _db.ForeignPurchases.Find(id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid)
            {
                var poreceivetypes = _db.ReceiveTypes.Where(x => x.Type == GoodReceiveType.Tender)
                    .Select(x => x.ReceiveTypeId).ToList();

                #region validate grn

                var goodReceive = _db.GoodReceives
                    .Where(x => x.IsVoid == false)
                    .Where(x => poreceivetypes.Contains(x.ReceiveTypeId))
                    .FirstOrDefault(x => x.ReceiveRef == record.PurchaseRef.ToString());

                if (goodReceive != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This followup can not be void since it is referred in good receive {goodReceive.GoodReceiveId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region validate service grn

                var serviceReceive = _db.ServiceReceives
                    .Where(x => x.IsVoid == false)
                    .Where(x => poreceivetypes.Contains(x.ReceiveTypeId))
                    .FirstOrDefault(x => x.ReceiveRef == record.PurchaseRef.ToString());

                if (serviceReceive != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This followup can not be void since it is referred in service receive {serviceReceive.ServiceReceiveId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
                #region validate fixed grn

                var fixedAssetReceive = _db.FixedAssetReceives
                    .Where(x => x.IsVoid == false)
                    .Where(x => poreceivetypes.Contains(x.ReceiveTypeId))
                    .FirstOrDefault(x => x.ReceiveRef == record.PurchaseRef.ToString());

                if (fixedAssetReceive != null)
                {
                    var formattedData2 = new
                    {
                        Message = $"This followup can not be void since it is referred in fixed asset receive {fixedAssetReceive.FixedAssetReceiveId}.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }
                #endregion
                record.Status = Enum.GetName(typeof(OperationStatuses), 10);
                record.IsOnlineApproved = false;
                record.IsSendForApproval = false;
                record.IsVoid = true;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                _db.ForeignPurchases.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();

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
        protected override void Dispose(bool disposing)
        {
           _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
