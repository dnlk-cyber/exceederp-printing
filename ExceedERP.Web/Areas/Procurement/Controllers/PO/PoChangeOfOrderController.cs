using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Procurement;
using System;
using DocumentFormat.OpenXml.Office2010.Excel;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Migrations;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PO
{
    public class PoChangeOforderController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var items = _db.PurchaseOrderItems;

            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItmCode + "-" + i.ItemName,
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

            var pos = _db.PurchaseOrders;

            var selectedproformaPurchases = pos.Select(
              i => new
              {
                  Code = i.PurchaseOrderID,
                  Name = i.PurchaseOrderID + "-" + i.SupplierName,
              })
              .AsQueryable();

            ViewData["PurchaseOrders"] = selectedproformaPurchases;
           
            ViewData["tax"] = tax;

            

           

        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult PoChangeOforders_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PoChangeOforder> PoChangeOforders = _db.PoChangeOforders;
            DataSourceResult result = PoChangeOforders.ToDataSourceResult(request, poChangeOforder => new {
                PoChangeOforderId = poChangeOforder.PoChangeOforderId,
                PurchaseOrderID = poChangeOforder.PurchaseOrderID,
                BranchId = poChangeOforder.BranchId,
                BusinessUnitId = poChangeOforder.BusinessUnitId,

                ForwardedFrom = poChangeOforder.ForwardedFrom,
                ForwardedTo = poChangeOforder.ForwardedTo,
                IsFinalApproval = poChangeOforder.IsFinalApproval,
                CurrentApprovalStep = poChangeOforder.CurrentApprovalStep,

                IsSendForApproval = poChangeOforder.IsSendForApproval,
                SendForApprovalBy = poChangeOforder.SendForApprovalBy,
                SendForApprovalTime = poChangeOforder.SendForApprovalTime,

                IsOnlineApproved = poChangeOforder.IsOnlineApproved,
                OnlineApprovedBy = poChangeOforder.OnlineApprovedBy,
                OnlineApprovedTime = poChangeOforder.OnlineApprovedTime,

                IsVoid = poChangeOforder.IsVoid,
                VoidBy = poChangeOforder.VoidBy,
                VoidTime = poChangeOforder.VoidTime,

                OrgBranchName = poChangeOforder.OrgBranchName,
                DateCreated = poChangeOforder.DateCreated,
                LastModified = poChangeOforder.LastModified,
                CreatedBy = poChangeOforder.CreatedBy,
                ModifiedBy = poChangeOforder.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult PoChangeOfordersForPOLink_Read(int id,[DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<PoChangeOforder> PoChangeOforders = _db.PoChangeOforders.Where(x=>x.PurchaseOrderID==id);
            DataSourceResult result = PoChangeOforders.ToDataSourceResult(request, poChangeOforder => new {
                PoChangeOforderId = poChangeOforder.PoChangeOforderId,
                PurchaseOrderID = poChangeOforder.PurchaseOrderID,
                BranchId = poChangeOforder.BranchId,
                BusinessUnitId = poChangeOforder.BusinessUnitId,

                ForwardedFrom = poChangeOforder.ForwardedFrom,
                ForwardedTo = poChangeOforder.ForwardedTo,
                IsFinalApproval = poChangeOforder.IsFinalApproval,
                CurrentApprovalStep = poChangeOforder.CurrentApprovalStep,

                IsSendForApproval = poChangeOforder.IsSendForApproval,
                SendForApprovalBy = poChangeOforder.SendForApprovalBy,
                SendForApprovalTime = poChangeOforder.SendForApprovalTime,

                IsOnlineApproved = poChangeOforder.IsOnlineApproved,
                OnlineApprovedBy = poChangeOforder.OnlineApprovedBy,
                OnlineApprovedTime = poChangeOforder.OnlineApprovedTime,

                IsVoid = poChangeOforder.IsVoid,
                VoidBy = poChangeOforder.VoidBy,
                VoidTime = poChangeOforder.VoidTime,

                OrgBranchName = poChangeOforder.OrgBranchName,
                DateCreated = poChangeOforder.DateCreated,
                LastModified = poChangeOforder.LastModified,
                CreatedBy = poChangeOforder.CreatedBy,
                ModifiedBy = poChangeOforder.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoChangeOforders_Create([DataSourceRequest]DataSourceRequest request, PoChangeOforder poChangeOforder)
        {            
            //validate if po is received
            if (ModelState.IsValid)
            {
                var entity = new PoChangeOforder
                {
                    PurchaseOrderID = poChangeOforder.PurchaseOrderID,
                    BranchId = poChangeOforder.BranchId,
                    BusinessUnitId = poChangeOforder.BusinessUnitId,
                    ForwardedFrom = poChangeOforder.ForwardedFrom,
                    ForwardedTo = poChangeOforder.ForwardedTo,
                    IsFinalApproval = poChangeOforder.IsFinalApproval,
                    CurrentApprovalStep = poChangeOforder.CurrentApprovalStep,

                    IsSendForApproval = poChangeOforder.IsSendForApproval,
                    SendForApprovalBy = poChangeOforder.SendForApprovalBy,
                    SendForApprovalTime = poChangeOforder.SendForApprovalTime,

                    IsOnlineApproved = poChangeOforder.IsOnlineApproved,
                    OnlineApprovedBy = poChangeOforder.OnlineApprovedBy,
                    OnlineApprovedTime = poChangeOforder.OnlineApprovedTime,

                    IsVoid = poChangeOforder.IsVoid,
                    VoidBy = poChangeOforder.VoidBy,
                    VoidTime = poChangeOforder.VoidTime,
                    OrgBranchName = poChangeOforder.OrgBranchName,
                    DateCreated = poChangeOforder.DateCreated,
                    LastModified = poChangeOforder.LastModified,
                    CreatedBy = poChangeOforder.CreatedBy,
                    ModifiedBy = poChangeOforder.ModifiedBy
                };

                _db.PoChangeOforders.Add(entity);
                _db.SaveChanges();
                poChangeOforder.PoChangeOforderId = entity.PoChangeOforderId;

                AddPOItems(entity.PoChangeOforderId, poChangeOforder.PurchaseOrderID);              

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase order change of order", "Success", User.Identity.Name, $"created change of order under record {entity.PurchaseOrderID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { poChangeOforder }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoChangeOforders_Update([DataSourceRequest]DataSourceRequest request, PoChangeOforder poChangeOforder)
        {          

            if (ModelState.IsValid)
            {
                var entity = new PoChangeOforder
                {
                    PurchaseOrderID = poChangeOforder.PurchaseOrderID,
                    PoChangeOforderId = poChangeOforder.PoChangeOforderId,
                    BranchId = poChangeOforder.BranchId,
                    BusinessUnitId = poChangeOforder.BusinessUnitId,
                    ForwardedFrom = poChangeOforder.ForwardedFrom,
                    ForwardedTo = poChangeOforder.ForwardedTo,
                    IsFinalApproval = poChangeOforder.IsFinalApproval,
                    CurrentApprovalStep = poChangeOforder.CurrentApprovalStep,

                    IsSendForApproval = poChangeOforder.IsSendForApproval,
                    SendForApprovalBy = poChangeOforder.SendForApprovalBy,
                    SendForApprovalTime = poChangeOforder.SendForApprovalTime,

                    IsOnlineApproved = poChangeOforder.IsOnlineApproved,
                    OnlineApprovedBy = poChangeOforder.OnlineApprovedBy,
                    OnlineApprovedTime = poChangeOforder.OnlineApprovedTime,

                    IsVoid = poChangeOforder.IsVoid,
                    VoidBy = poChangeOforder.VoidBy,
                    VoidTime = poChangeOforder.VoidTime,
                    OrgBranchName = poChangeOforder.OrgBranchName,
                    DateCreated = poChangeOforder.DateCreated,
                    LastModified = poChangeOforder.LastModified,
                    CreatedBy = poChangeOforder.CreatedBy,
                    ModifiedBy = poChangeOforder.ModifiedBy
                };

                _db.PoChangeOforders.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
              

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase order change of order", "Success", User.Identity.Name, $"updated change of order {entity.PoChangeOforderId} under record {entity.PurchaseOrderID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { poChangeOforder }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoChangeOforders_Destroy([DataSourceRequest]DataSourceRequest request, PoChangeOforder poChangeOforder)
        {
            if (ModelState.IsValid)
            {
                var entity = new PoChangeOforder
                {
                    PurchaseOrderID = poChangeOforder.PurchaseOrderID,
                    PoChangeOforderId = poChangeOforder.PoChangeOforderId,
                    BranchId = poChangeOforder.BranchId,
                    BusinessUnitId = poChangeOforder.BusinessUnitId,
                    OrgBranchName = poChangeOforder.OrgBranchName,
                    DateCreated = poChangeOforder.DateCreated,
                    LastModified = poChangeOforder.LastModified,
                    CreatedBy = poChangeOforder.CreatedBy,
                    ModifiedBy = poChangeOforder.ModifiedBy
                };

                _db.PoChangeOforders.Attach(entity);
                _db.PoChangeOforders.Remove(entity);
                _db.SaveChanges();
               
            }

            return Json(new[] { poChangeOforder }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        private void AddPOItems(int changeid,int poid)
        {
            var items = _db.PurchaseOrderItems.Where(x=>x.PurchaseOrderID==poid).ToList();
            foreach (var item in items)
            {
                var entity = new PoChangeOfOrderItem
                {
                    PoChangeOforderId = changeid,
                    CatCode = item.CatCode,
                    CategoryName = item.CategoryName,
                    ItmCode = item.ItmCode,
                    ItemName = item.ItemName,
                    Name = item.Name,
                    PartNo = item.PartNo,
                    Quantity = item.Quantity,
                    IsApproved = item.IsApproved,
                    ItemSpecification = item.ItemSpecification,
                    UoM = item.UoM,
                    ItemType = item.ItemType,
                    UnitPrice = item.UnitPrice,
                    Discount = item.Discount,
                    GLTaxRateID = item.GLTaxRateID,
                    TaxAmount = item.TaxAmount,
                    SubTotal = item.SubTotal,
                    Total = item.Total         
                
                  
                };
           
                _db.PoChangeOfOrderItems.Add(entity);
                _db.SaveChanges();
            }
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser)]
        public JsonResult SendForApproval(int id)
        {
            var record = _db.PoChangeOforders.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.PoChangeOfOrderItems.Count > 0)
            {
                #region validate price and tax

                foreach (var item in record.PoChangeOfOrderItems)
                {
                    if (item.UnitPrice <= 0 || item.GLTaxRateID == 0)
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill unit price and tax information for all items.",
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
                _db.PoChangeOforders.Attach(record);
                _db.Entry(record).State = EntityState.Modified;
                _db.SaveChanges();
                #region approval workflow

                bool isNewRecord = true;
                var validations = record.PoChangeOforderValidations.Where(x => x.Status == ApprovalStatuses.Approve).AsQueryable();
                if (validations.Any())
                    isNewRecord = false;
                decimal docamount = record.PoChangeOfOrderItems.Sum(x => x.Total);
                int docid = record.PurchaseOrderID;
                int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                DocApprovalWorkflowHelper.ForwardToApprover(docid, docamount, "", record.ForwardedTo, ApprovalDocumentType.POChangeOrder, isNewRecord, step);
                #endregion
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

            if (record.PoChangeOfOrderItems.Count == 0)
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

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
