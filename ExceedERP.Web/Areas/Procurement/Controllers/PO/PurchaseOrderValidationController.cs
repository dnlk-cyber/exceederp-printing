using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PO
{
    
    public class PurchaseOrderValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderUser, ProcurementConstants.ProcurementTenderApprover)]
        public ActionResult PurchaseOrderValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PurchaseOrderValidation> purchaseordervalidations = db.PurchaseOrderValidations.Where(p => p.PurchaseOrderID == id);
            DataSourceResult result = purchaseordervalidations.ToDataSourceResult(request, purchaseOrderValidation => new {
                PurchaseOrderValidationID = purchaseOrderValidation.PurchaseOrderValidationID,
                PurchaseOrderID = purchaseOrderValidation.PurchaseOrderID,
                Status = purchaseOrderValidation.Status,
                Remark = purchaseOrderValidation.Remark,
                OrgBranchName = purchaseOrderValidation.OrgBranchName,
                DateCreated = purchaseOrderValidation.DateCreated,
                LastModified = purchaseOrderValidation.LastModified,
                CreatedBy = purchaseOrderValidation.CreatedBy,
                ModifiedBy = purchaseOrderValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseOrderValidations_Create(int id, [DataSourceRequest]DataSourceRequest request, PurchaseOrderValidation purchaseOrderValidation)
        {
            var record = db.PurchaseOrders.Find(id);
            var items = db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == id).ToList();

            foreach (var item in items)
            {
                if(item.Quantity<=0)
                    ModelState.AddModelError("qty", "Item Quantity should be more than 0 for all items.");
                if (item.UnitPrice <= 0)
                    ModelState.AddModelError("price", "Item Price should be more than 0 for all items.");
                if (item.GLTaxRateID == 0)
                    ModelState.AddModelError("tax", "Tax should be more than 0 for all items.");
            }

            if (record != null && record.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record,it is already approved");

            if (record != null && record.ForwardedTo != User.Identity.Name)
                ModelState.AddModelError("notcorrectuser", "You can't approve this record,it is forwarded to another user.");

            var hasApproved = db.PurchaseOrderValidations
                    .Where(x=>x.PurchaseOrderID==id)
                .Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
            if(hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");

            if (record != null && (purchaseOrderValidation.Status == ApprovalStatuses.Approve && !record.IsManual))
            {
                #region validate proforma

                var proformaPurchase = db.ProformaPurchases
                    .FirstOrDefault(x => x.ProformaPurchaseID == record.ProformaPurchaseID);
                
                if (proformaPurchase != null && proformaPurchase.IsVoid&& ! proformaPurchase.IsOnlineApproved)
                {
                    var formattedData2 = new
                    {
                        Message =
                        $"This proforma purchase {proformaPurchase.ProformaPurchaseID} related with this purchase order is either void or not approved.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData2, JsonRequestBehavior.AllowGet);
                }

                #endregion
            }

            if (ModelState.IsValid)
            {
                if (record != null)
                {

                    if (purchaseOrderValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (record.IsFinalApproval)
                        {
                            record.Status = "Approved";
                            record.IsOnlineApproved = true;
                            record.IsVoid = false;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();

                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                Message =
                                    $"Purchase order document {record.PurchaseOrderID} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
                            };

                            Notify.NotifyUser(notif);

                            #endregion
                        }
                        else
                        {
                            record.Status = "Partially Approved";
                            record.IsOnlineApproved = false;
                            record.IsVoid = false;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.OnlineApprovedTime = DateTime.Now;
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();

                            #region approval notification

                            NotifModels notif = new NotifModels
                            {
                                UserId = record.ForwardedFrom,
                                Message =
                                    $"Purchase order document {record.PurchaseOrderID} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
                            };

                            Notify.NotifyUser(notif);

                            #endregion

                            #region forward to next approver

                            decimal docamount = record.PurchaseOrderItem.Sum(x => x.Total);
                            DocApprovalWorkflowHelper.ForwardToApprover(record.PurchaseOrderID, docamount, "",
                                record.ForwardedTo, ApprovalDocumentType.PurchaseOrder, false);

                            #endregion

                        }


                    }

                    if (purchaseOrderValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        //notification
                    }

                    var entity = new PurchaseOrderValidation
                    {
                        PurchaseOrderID = id,
                        Status = purchaseOrderValidation.Status,
                        Remark = purchaseOrderValidation.Remark,
                        OrgBranchName = purchaseOrderValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = purchaseOrderValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = purchaseOrderValidation.ModifiedBy
                    };


                    db.PurchaseOrderValidations.Add(entity);
                    db.SaveChanges();
                    purchaseOrderValidation.PurchaseOrderValidationID = entity.PurchaseOrderValidationID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "purchase order approval", "Success", User.Identity.Name, $"approved po {entity.PurchaseOrderID} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { purchaseOrderValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseOrderValidations_Update([DataSourceRequest]DataSourceRequest request, PurchaseOrderValidation purchaseOrderValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseOrderValidation
                {
                    PurchaseOrderValidationID = purchaseOrderValidation.PurchaseOrderValidationID,
                    PurchaseOrderID = purchaseOrderValidation.PurchaseOrderID,
                    Status = purchaseOrderValidation.Status,
                    Remark = purchaseOrderValidation.Remark,
                    OrgBranchName = purchaseOrderValidation.OrgBranchName,
                    DateCreated = purchaseOrderValidation.DateCreated,
                    LastModified = purchaseOrderValidation.LastModified,
                    CreatedBy = purchaseOrderValidation.CreatedBy,
                    ModifiedBy = purchaseOrderValidation.ModifiedBy
                };

                db.PurchaseOrderValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { purchaseOrderValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementTenderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseOrderValidations_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseOrderValidation purchaseOrderValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseOrderValidation
                {
                    PurchaseOrderValidationID = purchaseOrderValidation.PurchaseOrderValidationID,
                    Status = purchaseOrderValidation.Status,
                    Remark = purchaseOrderValidation.Remark,
                    OrgBranchName = purchaseOrderValidation.OrgBranchName,
                    DateCreated = purchaseOrderValidation.DateCreated,
                    LastModified = purchaseOrderValidation.LastModified,
                    CreatedBy = purchaseOrderValidation.CreatedBy,
                    ModifiedBy = purchaseOrderValidation.ModifiedBy
                };

                db.PurchaseOrderValidations.Attach(entity);
                db.PurchaseOrderValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { purchaseOrderValidation }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
