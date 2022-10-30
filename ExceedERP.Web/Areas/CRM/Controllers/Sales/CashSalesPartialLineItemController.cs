using System;


using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class CashSalesPartialLineItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CashSalesPartialLineItems_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {
            var cashsalespartiallineitems = db.CashSalesPartialLineItems.Where(x => x.CashSalesPartialItemId == Id).ToList();
            DataSourceResult result = cashsalespartiallineitems.ToDataSourceResult(request, cashSalesPartialLineItem => new
            {
                CashSalesPartialLineItemId = cashSalesPartialLineItem.CashSalesPartialLineItemId,
                CashSalesPartialItemId = cashSalesPartialLineItem.CashSalesPartialItemId,
                PartialPayedAmount = cashSalesPartialLineItem.PartialPayedAmount,
                IsPaymentProcessed = cashSalesPartialLineItem.CashSalesPartialItem.IsPaymentProcessed,
                LineItemID = cashSalesPartialLineItem.LineItemID,
                Store = cashSalesPartialLineItem.LineItem.SalesStore.Description,
                ItemUOM = cashSalesPartialLineItem.LineItem.Product.UOM.ToString(),
                Status = cashSalesPartialLineItem.Status,
                ItemName = cashSalesPartialLineItem.LineItem.Product.Name,
                Quantity = cashSalesPartialLineItem.LineItem.Quantity,
                UnitPrice = cashSalesPartialLineItem.LineItem.UnitAmount,
                IsVoid = cashSalesPartialLineItem.IsVoid,
                VoidBy = cashSalesPartialLineItem.VoidBy,
                VoidTime = cashSalesPartialLineItem.VoidTime,
                IsOnlineApproved = cashSalesPartialLineItem.IsOnlineApproved,
                OnlineApprovedBy = cashSalesPartialLineItem.OnlineApprovedBy,
                OnlineApprovedTime = cashSalesPartialLineItem.OnlineApprovedTime,
                IsOnlineTransferred = cashSalesPartialLineItem.IsOnlineTransferred,
                IsOnlineTransferredBy = cashSalesPartialLineItem.IsOnlineTransferredBy,
                OnlineTransferredTime = cashSalesPartialLineItem.OnlineTransferredTime,
                IsSendForApproval = cashSalesPartialLineItem.IsSendForApproval,
                OrgBranchName = cashSalesPartialLineItem.OrgBranchName,
                SendForApprovalBy = cashSalesPartialLineItem.SendForApprovalBy,
                SendForApprovalTime = cashSalesPartialLineItem.SendForApprovalTime,
                DateCreated = cashSalesPartialLineItem.DateCreated,
                LastModified = cashSalesPartialLineItem.LastModified,
                CreatedBy = cashSalesPartialLineItem.CreatedBy,
                ModifiedBy = cashSalesPartialLineItem.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashSalesPartialLineItems_Create([DataSourceRequest]DataSourceRequest request, CashSalesPartialLineItem cashSalesPartialLineItem, int Id)
        {

            var LineItem = db.LineItems.Where(x => x.LineItemID == cashSalesPartialLineItem.LineItemID).FirstOrDefault();
            if (LineItem.Quantity < cashSalesPartialLineItem.PartialPayedAmount)
            {
                ModelState.AddModelError("ExceedAmount", " Partial Quantity Exceed Orginal Quantity!");

                return Json(new[] { cashSalesPartialLineItem }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new CashSalesPartialLineItem
                {
                    CashSalesPartialItemId = Id,
                    PartialPayedAmount = cashSalesPartialLineItem.PartialPayedAmount,
                    Status = cashSalesPartialLineItem.Status,
                    Remark = cashSalesPartialLineItem.Remark,
                    IsVoid = cashSalesPartialLineItem.IsVoid,
                    VoidBy = cashSalesPartialLineItem.VoidBy,
                    VoidTime = cashSalesPartialLineItem.VoidTime,
                    IsOnlineApproved = cashSalesPartialLineItem.IsOnlineApproved,
                    OnlineApprovedBy = cashSalesPartialLineItem.OnlineApprovedBy,
                    OnlineApprovedTime = cashSalesPartialLineItem.OnlineApprovedTime,
                    IsOnlineTransferred = cashSalesPartialLineItem.IsOnlineTransferred,
                    IsOnlineTransferredBy = cashSalesPartialLineItem.IsOnlineTransferredBy,
                    OnlineTransferredTime = cashSalesPartialLineItem.OnlineTransferredTime,
                    IsSendForApproval = cashSalesPartialLineItem.IsSendForApproval,
                    OrgBranchName = cashSalesPartialLineItem.OrgBranchName,
                    SendForApprovalBy = cashSalesPartialLineItem.SendForApprovalBy,
                    SendForApprovalTime = cashSalesPartialLineItem.SendForApprovalTime,
                    DateCreated = DateTime.Now,
                    LastModified = cashSalesPartialLineItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = cashSalesPartialLineItem.ModifiedBy
                };

                db.CashSalesPartialLineItems.Add(entity);
                db.SaveChanges();
                cashSalesPartialLineItem.CashSalesPartialLineItemId = entity.CashSalesPartialLineItemId;
            }

            return Json(new[] { cashSalesPartialLineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashSalesPartialLineItems_Update([DataSourceRequest]DataSourceRequest request, CashSalesPartialLineItem cashSalesPartialLineItem)
        {
            CRMSalesHelper CRMSalesHelper = new CRMSalesHelper();
               var CashSalePartialLineItem = db.CashSalesPartialLineItems.ToList();
         var LineItem = db.LineItems.Where(x=>x.LineItemID == cashSalesPartialLineItem.LineItemID).FirstOrDefault();
         if (LineItem != null)
         {
            var IsValidQuantity = CRMSalesHelper.ValidatePartialLineAmount(LineItem, CashSalePartialLineItem, cashSalesPartialLineItem.CashSalesPartialLineItemId, cashSalesPartialLineItem.PartialPayedAmount);
             if (!IsValidQuantity)
                {
                    ModelState.AddModelError("ExceedAmount", " Partial Quantity Exceed Orginal Quantity!");

                    return Json(new[] { cashSalesPartialLineItem }.ToDataSourceResult(request, ModelState));
                }
         }
         db = new ExceedDbContext();
                var entity = new CashSalesPartialLineItem
                {
                    CashSalesPartialLineItemId = cashSalesPartialLineItem.CashSalesPartialLineItemId,
                    CashSalesPartialItemId = cashSalesPartialLineItem.CashSalesPartialItemId,
                    PartialPayedAmount = cashSalesPartialLineItem.PartialPayedAmount,
                    Status = cashSalesPartialLineItem.Status,
                    Remark = cashSalesPartialLineItem.Remark,
                    LineItemID = cashSalesPartialLineItem.LineItemID,
                    IsVoid = cashSalesPartialLineItem.IsVoid,
                    VoidBy = cashSalesPartialLineItem.VoidBy,
                    VoidTime = cashSalesPartialLineItem.VoidTime,
                    IsOnlineApproved = cashSalesPartialLineItem.IsOnlineApproved,
                    OnlineApprovedBy = cashSalesPartialLineItem.OnlineApprovedBy,
                    OnlineApprovedTime = cashSalesPartialLineItem.OnlineApprovedTime,
                    IsOnlineTransferred = cashSalesPartialLineItem.IsOnlineTransferred,
                    IsOnlineTransferredBy = cashSalesPartialLineItem.IsOnlineTransferredBy,
                    OnlineTransferredTime = cashSalesPartialLineItem.OnlineTransferredTime,
                    IsSendForApproval = cashSalesPartialLineItem.IsSendForApproval,
                    OrgBranchName = cashSalesPartialLineItem.OrgBranchName,
                    SendForApprovalBy = cashSalesPartialLineItem.SendForApprovalBy,
                    SendForApprovalTime = cashSalesPartialLineItem.SendForApprovalTime,
                    DateCreated = cashSalesPartialLineItem.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = cashSalesPartialLineItem.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.CashSalesPartialLineItems.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
        
            return Json(new[] { cashSalesPartialLineItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CashSalesPartialLineItems_Destroy([DataSourceRequest]DataSourceRequest request, CashSalesPartialLineItem cashSalesPartialLineItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashSalesPartialLineItem
                {
                    CashSalesPartialLineItemId = cashSalesPartialLineItem.CashSalesPartialLineItemId,
                    PartialPayedAmount = cashSalesPartialLineItem.PartialPayedAmount,
                    Status = cashSalesPartialLineItem.Status,
                    Remark = cashSalesPartialLineItem.Remark,
                    IsVoid = cashSalesPartialLineItem.IsVoid,
                    VoidBy = cashSalesPartialLineItem.VoidBy,
                    VoidTime = cashSalesPartialLineItem.VoidTime,
                    IsOnlineApproved = cashSalesPartialLineItem.IsOnlineApproved,
                    OnlineApprovedBy = cashSalesPartialLineItem.OnlineApprovedBy,
                    OnlineApprovedTime = cashSalesPartialLineItem.OnlineApprovedTime,
                    IsOnlineTransferred = cashSalesPartialLineItem.IsOnlineTransferred,
                    IsOnlineTransferredBy = cashSalesPartialLineItem.IsOnlineTransferredBy,
                    OnlineTransferredTime = cashSalesPartialLineItem.OnlineTransferredTime,
                    IsSendForApproval = cashSalesPartialLineItem.IsSendForApproval,
                    OrgBranchName = cashSalesPartialLineItem.OrgBranchName,
                    SendForApprovalBy = cashSalesPartialLineItem.SendForApprovalBy,
                    SendForApprovalTime = cashSalesPartialLineItem.SendForApprovalTime,
                    DateCreated = cashSalesPartialLineItem.DateCreated,
                    LastModified = cashSalesPartialLineItem.LastModified,
                    CreatedBy = cashSalesPartialLineItem.CreatedBy,
                    ModifiedBy = cashSalesPartialLineItem.ModifiedBy
                };

                db.CashSalesPartialLineItems.Attach(entity);
                db.CashSalesPartialLineItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { cashSalesPartialLineItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
