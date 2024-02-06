using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Operation
{

    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpRJobOrderUser)]
    public class ProductionStoreRequisitionValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View(); 
        }


        //[AuthorizeRoles(InventoryConstants.PropertyRequistion, InventoryConstants.UserDepartment)]
        public ActionResult ProductionStoreRequisitionValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<StoreRequisitionValidation> storerequisitionvalidations = db.StoreRequisitionValidations.Where(sr => sr.StoreRequisitionID == id);
            DataSourceResult result = storerequisitionvalidations.ToDataSourceResult(request, storeRequisitionValidation => new
            {
                StoreRequisitionValidationID = storeRequisitionValidation.StoreRequisitionValidationID,
                Status = storeRequisitionValidation.Status,
                Remark = storeRequisitionValidation.Remark,
            });

            return Json(result);
        }



        //[AuthorizeRoles(InventoryConstants.UserDepartment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionStoreRequisitionValidations_Create([DataSourceRequest]DataSourceRequest request, StoreRequisitionValidation storeRequisitionValidation, int id)
        {
            var storeRequisition = db.StoreRequisitions.Find(id);
            if (storeRequisition != null && storeRequisition.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate this Record: It is Already Approved");
            }
            if (storeRequisition != null && storeRequisition.StoreRequisitionItem.Count <= 0 && storeRequisitionValidation.Status == ApprovalStatuses.Approve)
            {
                ModelState.AddModelError("no items", "Please add at least one item to the requisition.");
            }
            if (storeRequisition != null && storeRequisition.IsTransfer && storeRequisitionValidation.Status == ApprovalStatuses.Approve)
            {
                foreach (var item in storeRequisition.StoreRequisitionItem)
                {
                    if (item.RequestedQuantity <= 0)
                    {
                        ModelState.AddModelError("", $"Requested quantity should be positive number. Item : {item.InventoryItem.Name}, Requested quantity: {item.RequestedQuantity} is invalid.");
                    }
                }
            }
            if (ModelState.IsValid)
            {
                if (storeRequisition != null)
                {
                    if (storeRequisitionValidation.Status == ApprovalStatuses.Approve)
                    {
                        foreach (var item in storeRequisition.StoreRequisitionItem)
                        {
                            var currentBalance =
                                ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(item.ItemCode, item.StoreRequisition.StoreCode);
                            if (item.IssuedQuantity > currentBalance)
                            {
                                ModelState.AddModelError("no balance",
                                    "There is no enough balance in store for item: " + item.InventoryItem.Name +
                                    " current balance is: " + currentBalance);
                            }
                        }
                    }
                    //// check again because other validation errors may have been added since last check
                    //if (ModelState.IsValid)
                    //{
                    if (storeRequisitionValidation.Status == ApprovalStatuses.Approve)
                    {
                        storeRequisition.Status = "User Approved";
                        storeRequisition.IsOnlineApproved = true;
                        storeRequisition.OnlineApprovedBy = User.Identity.Name;
                        storeRequisition.OnlineApprovedTime = DateTime.Now;
                        db.Entry(storeRequisition).State = EntityState.Modified;
                        var result = db.SaveChanges();
                       
                        //#region send notification

                        //List<UserVM> users = new List<UserVM>();
                        //users.AddRange(UserInformationHelper.GetUsersInRole("Property_User"));
                        //users.AddRange(UserInformationHelper.GetUsersInRole("Property_Authorizer"));
                        //var uniqueusers = users.DistinctByField(x => x.UserId);

                        //string message = "New store requisition generated.";
                        //foreach (var user in uniqueusers)
                        //{
                        //    NotifModels notif = new NotifModels
                        //    {
                        //        UserId = user.UserName,
                        //        Title = "store requisition",
                        //        Message = message,
                        //        URL = "~/Inventory/ProductionStoreRequisitionPropertyReview"
                        //    };
                        //    Notify.NotifyUser(notif);
                        //}

                        //#endregion
                    }
                    else if (storeRequisitionValidation.Status == ApprovalStatuses.Reject)
                    {
                        storeRequisition.IsSendForApproval = false;
                        storeRequisition.Status = Enum.GetName(typeof(OperationStatuses), 8);
                        db.Entry(storeRequisition).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var entity = new StoreRequisitionValidation
                    {
                        StoreRequisitionID = id,
                        Status = storeRequisitionValidation.Status,
                        Remark = storeRequisitionValidation.Remark,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Today,
                    };

                    db.StoreRequisitionValidations.Add(entity);
                    db.SaveChanges();
                    storeRequisitionValidation.StoreRequisitionValidationID = entity.StoreRequisitionValidationID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store requisition approval", "Success", User.Identity.Name, $"approved store requisition {entity.StoreRequisitionID} ", Modules.Inventory);
                    #endregion
                    //}
                }
            }
            return Json(new[] { storeRequisitionValidation }.ToDataSourceResult(request, ModelState));

        }

        //[AuthorizeRoles(InventoryConstants.UserDepartment)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionStoreRequisitionValidations_Update([DataSourceRequest]DataSourceRequest request, StoreRequisitionValidation storeRequisitionValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionValidation
                {
                    StoreRequisitionValidationID = storeRequisitionValidation.StoreRequisitionValidationID,
                    Status = storeRequisitionValidation.Status,
                    Remark = storeRequisitionValidation.Remark,
                };

                db.StoreRequisitionValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { storeRequisitionValidation }.ToDataSourceResult(request, ModelState));
        }

      //  [AuthorizeRoles(InventoryConstants.UserDepartment)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionStoreRequisitionValidations_Destroy([DataSourceRequest]DataSourceRequest request, StoreRequisitionValidation storeRequisitionValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionValidation
                {
                    StoreRequisitionValidationID = storeRequisitionValidation.StoreRequisitionValidationID,
                    Status = storeRequisitionValidation.Status,
                    Remark = storeRequisitionValidation.Remark,
                };

                db.StoreRequisitionValidations.Attach(entity);
                db.StoreRequisitionValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { storeRequisitionValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
