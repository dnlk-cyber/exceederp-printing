using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.PurchaseRequisitions
    {
     public class PurchaseRequsitionValidationUserController : Controller
        {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser, ProcurementConstants.ProcurementUserDepartmentApprover)]
        public ActionResult Index()
            {
            return View();
            }
         
        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentUser, ProcurementConstants.ProcurementUserDepartmentApprover)]
        public ActionResult PurchaseRequisitionUserDepartmentValidations_Read([DataSourceRequest] DataSourceRequest request, int id)
            {
            var purchaserequisitionuserdepartmentvalidations =
                db.PurchaseRequisitionUserDepartmentValidations.Where(pri => pri.PurchaseRequisitionID == id);
            var result = purchaserequisitionuserdepartmentvalidations.ToDataSourceResult(request,
                purchaseRequisitionUserDepartmentValidation => new
                    {
                        purchaseRequisitionUserDepartmentValidation.PurchaseRequisitionUserDepartmentValidationID,
                        purchaseRequisitionUserDepartmentValidation.Status,
                        
                        purchaseRequisitionUserDepartmentValidation.Remark
                    });

            return Json(result);
            }

        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionUserDepartmentValidations_Create([DataSourceRequest] DataSourceRequest request,PurchaseRequisitionUserDepartmentValidation purchaseRequisitionUserDepartmentValidation, int id)
        {
                var purchaseRequisiton =db.PurchaseRequisitions.Find(id);
                if (purchaseRequisiton != null && purchaseRequisiton.PurchaseRequisitionItems.Count == 0)
                {
                    ModelState.AddModelError("noItem","No Service Requisition Item found. Please Check purchase Requisition " +purchaseRequisiton.PurchaseRequisitionID);
                }
                 else if (purchaseRequisiton != null && purchaseRequisiton.IsOnlineApproved)
                {
                    ModelState.AddModelError("alreadyApproved","You Can not Manipulate This Record : It is Already Approved");
                }
           

            if (ModelState.IsValid)
                {
                    if (purchaseRequisitionUserDepartmentValidation.Status == ApprovalStatuses.Approve)
                    {
                        if (purchaseRequisiton != null)
                        {
                            purchaseRequisiton.Status = "User Approved";
                            purchaseRequisiton.IsOnlineApproved = true;
                            purchaseRequisiton.OnlineApprovedBy = User.Identity.Name;
                            purchaseRequisiton.OnlineApprovedTime = DateTime.Now.Date;
                            db.PurchaseRequisitions.Attach(purchaseRequisiton);
                            db.Entry(purchaseRequisiton).State = EntityState.Modified;
                            db.SaveChanges();
                            
                        }
                    }
                    else if (purchaseRequisitionUserDepartmentValidation.Status == ApprovalStatuses.Reject)
                    {
                        if (purchaseRequisiton != null)
                        {
                            purchaseRequisiton.IsSendForApproval = false;
                            purchaseRequisiton.Status = "Rejected";
                            db.PurchaseRequisitions.Attach(purchaseRequisiton);
                            db.Entry(purchaseRequisiton).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                    var entity = new PurchaseRequisitionUserDepartmentValidation
                        {
                            PurchaseRequisitionID = id,
                            Status = purchaseRequisitionUserDepartmentValidation.Status,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                            Remark = purchaseRequisitionUserDepartmentValidation.Remark
                        };

                    db.PurchaseRequisitionUserDepartmentValidations.Add(entity);
                    db.SaveChanges();
                    purchaseRequisitionUserDepartmentValidation.PurchaseRequisitionUserDepartmentValidationID =entity.PurchaseRequisitionUserDepartmentValidationID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "purchase requisition approval", "Success", User.Identity.Name, $"approved purchase requisition {entity.PurchaseRequisitionID} ", Modules.Procurement);
                #endregion

            }

            return Json(new[] {purchaseRequisitionUserDepartmentValidation}.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionUserDepartmentValidations_Update([DataSourceRequest] DataSourceRequest request,PurchaseRequisitionUserDepartmentValidation purchaseRequisitionUserDepartmentValidation)
            {
            if (ModelState.IsValid)
                {
                    var entity = new PurchaseRequisitionUserDepartmentValidation
                        {
                            PurchaseRequisitionUserDepartmentValidationID =
                                purchaseRequisitionUserDepartmentValidation
                                    .PurchaseRequisitionUserDepartmentValidationID,
                            Status = purchaseRequisitionUserDepartmentValidation.Status,
                            Remark = purchaseRequisitionUserDepartmentValidation.Remark
                        };

                    db.PurchaseRequisitionUserDepartmentValidations.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }

            return Json(new[] {purchaseRequisitionUserDepartmentValidation}.ToDataSourceResult(request, ModelState));
            }

        [AuthorizeRoles(ProcurementConstants.ProcurementUserDepartmentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseRequisitionUserDepartmentValidations_Destroy([DataSourceRequest] DataSourceRequest request,PurchaseRequisitionUserDepartmentValidation purchaseRequisitionUserDepartmentValidation)
            {
            if (ModelState.IsValid)
                {
                    var entity = new PurchaseRequisitionUserDepartmentValidation
                        {
                            PurchaseRequisitionUserDepartmentValidationID =
                                purchaseRequisitionUserDepartmentValidation
                                    .PurchaseRequisitionUserDepartmentValidationID,
                            Status = purchaseRequisitionUserDepartmentValidation.Status,
                            Remark = purchaseRequisitionUserDepartmentValidation.Remark
                        };

                    db.PurchaseRequisitionUserDepartmentValidations.Attach(entity);
                    db.PurchaseRequisitionUserDepartmentValidations.Remove(entity);
                    db.SaveChanges();
                }

            return Json(new[] {purchaseRequisitionUserDepartmentValidation}.ToDataSourceResult(request, ModelState));
            }

        protected override void Dispose(bool disposing)
            {
            db.Dispose();
            base.Dispose(disposing);
            }
        }
    }