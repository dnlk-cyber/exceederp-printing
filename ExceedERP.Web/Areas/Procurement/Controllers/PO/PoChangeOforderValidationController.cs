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
using ExceedERP.Helpers.Procurement;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PO
{
    
    public class PoChangeOforderValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderUser, ProcurementRoles.ProcurementTenderApprover)]
        public ActionResult PoChangeOforderValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PoChangeOforderValidation> PoChangeOforderValidations = db.PoChangeOforderValidations.Where(p => p.PoChangeOforderId == id);
            DataSourceResult result = PoChangeOforderValidations.ToDataSourceResult(request, PoChangeOforderValidation => new {
                PoChangeOforderValidationId = PoChangeOforderValidation.PoChangeOforderValidationId,
                PoChangeOforderId = PoChangeOforderValidation.PoChangeOforderId,
                Status = PoChangeOforderValidation.Status,
                Remark = PoChangeOforderValidation.Remark,
                OrgBranchName = PoChangeOforderValidation.OrgBranchName,
                DateCreated = PoChangeOforderValidation.DateCreated,
                LastModified = PoChangeOforderValidation.LastModified,
                CreatedBy = PoChangeOforderValidation.CreatedBy,
                ModifiedBy = PoChangeOforderValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoChangeOforderValidations_Create(int id, [DataSourceRequest]DataSourceRequest request, PoChangeOforderValidation PoChangeOforderValidation)
        {
            var record = db.PoChangeOforders.Find(id);
            var items = db.PoChangeOfOrderItems.Where(x => x.PoChangeOforderId == id).ToList();

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

            var hasApproved = db.PoChangeOforderValidations
                    .Where(x=>x.PoChangeOforderId==id)
                .Where(x => x.CreatedBy == User.Identity.Name && x.Status == ApprovalStatuses.Approve).AsQueryable();
            if(hasApproved.Any())
                ModelState.AddModelError("hasapproved", "You can't approve again.You have already approved.");

            if (record != null && (PoChangeOforderValidation.Status == ApprovalStatuses.Approve))
            {
                var po = db.PurchaseOrders.Find(record.PurchaseOrderID);
                if (po != null && !po.IsManual)
                {
                    #region validate proforma

                    var proformaPurchase = db.ProformaPurchases
                        .FirstOrDefault(x => x.ProformaPurchaseID == po.ProformaPurchaseID);

                    if (proformaPurchase != null && proformaPurchase.IsVoid && !proformaPurchase.IsOnlineApproved)
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
            }

           

            if (ModelState.IsValid)
            {
                if (record != null)
                {

                    if (PoChangeOforderValidation.Status == ApprovalStatuses.Approve)
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
                                URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.POChangeOrder, record.PoChangeOforderId),
                                Message =
                                    $"Purchase order document {record.PoChangeOforderId} is approved by User: {record.ForwardedTo} at {DateTime.Now}"
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
                                URL = DocApprovalWorkflowHelper.GetDocUrl(ApprovalDocumentType.POChangeOrder, record.PoChangeOforderId),
                                Message =
                                    $"Purchase order document {record.PoChangeOforderId} is approved by User: {record.ForwardedTo} at {DateTime.Now} and forwarded to next user."
                            };

                            Notify.NotifyUser(notif);

                            #endregion

                            #region forward to next approver
                            decimal docamount = record.PoChangeOfOrderItems.Sum(x => x.Total);
                            int step = record.CurrentApprovalStep != null ? (int)record.CurrentApprovalStep : 0;
                            DocApprovalWorkflowHelper.ForwardToApprover(record.PoChangeOforderId, docamount, "",
                                record.ForwardedTo, ApprovalDocumentType.POChangeOrder, false, step);

                            #endregion

                        }


                    }

                    if (PoChangeOforderValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsOnlineApproved = false;
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                        //notification
                    }

                    var entity = new PoChangeOforderValidation
                    {
                        PoChangeOforderId = id,
                        Status = PoChangeOforderValidation.Status,
                        Remark = PoChangeOforderValidation.Remark,
                        OrgBranchName = PoChangeOforderValidation.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = PoChangeOforderValidation.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = PoChangeOforderValidation.ModifiedBy
                    };


                    db.PoChangeOforderValidations.Add(entity);
                    db.SaveChanges();
                    PoChangeOforderValidation.PoChangeOforderValidationId = entity.PoChangeOforderValidationId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "purchase order approval", "Success", User.Identity.Name, $"approved po {entity.PoChangeOforderId} ", Modules.Procurement);
                    #endregion
                }
            }

            return Json(new[] { PoChangeOforderValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoChangeOforderValidations_Update([DataSourceRequest]DataSourceRequest request, PoChangeOforderValidation PoChangeOforderValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PoChangeOforderValidation
                {
                    PoChangeOforderValidationId = PoChangeOforderValidation.PoChangeOforderValidationId,
                    PoChangeOforderId = PoChangeOforderValidation.PoChangeOforderId,
                    Status = PoChangeOforderValidation.Status,
                    Remark = PoChangeOforderValidation.Remark,
                    OrgBranchName = PoChangeOforderValidation.OrgBranchName,
                    DateCreated = PoChangeOforderValidation.DateCreated,
                    LastModified = PoChangeOforderValidation.LastModified,
                    CreatedBy = PoChangeOforderValidation.CreatedBy,
                    ModifiedBy = PoChangeOforderValidation.ModifiedBy
                };

                db.PoChangeOforderValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { PoChangeOforderValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementTenderApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PoChangeOforderValidations_Destroy([DataSourceRequest]DataSourceRequest request, PoChangeOforderValidation PoChangeOforderValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PoChangeOforderValidation
                {
                    PoChangeOforderValidationId = PoChangeOforderValidation.PoChangeOforderValidationId,
                    Status = PoChangeOforderValidation.Status,
                    Remark = PoChangeOforderValidation.Remark,
                    OrgBranchName = PoChangeOforderValidation.OrgBranchName,
                    DateCreated = PoChangeOforderValidation.DateCreated,
                    LastModified = PoChangeOforderValidation.LastModified,
                    CreatedBy = PoChangeOforderValidation.CreatedBy,
                    ModifiedBy = PoChangeOforderValidation.ModifiedBy
                };

                db.PoChangeOforderValidations.Attach(entity);
                db.PoChangeOforderValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { PoChangeOforderValidation }.ToDataSourceResult(request, ModelState));
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
