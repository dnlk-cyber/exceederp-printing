using System;
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

namespace ExceedERP.Web.Areas.Inventory.Controllers.Receives
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser)]
    public class TempGoodReceiveValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult TempGoodReceiveValidations_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<TempGoodReceiveValidation> tempgoodreceivevalidations = db.TempGoodReceiveValidations.Where(tgi => tgi.TempGoodReceiveID == id);
            DataSourceResult result = tempgoodreceivevalidations.ToDataSourceResult(request, tempGoodReceiveValidation => new {
                TempGoodReceiveValidationID = tempGoodReceiveValidation.TempGoodReceiveValidationID,
                Status = tempGoodReceiveValidation.Status,
                Remark = tempGoodReceiveValidation.Remark,
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TempGoodReceiveValidations_Create([DataSourceRequest]DataSourceRequest request, TempGoodReceiveValidation tempGoodReceiveValidation , int id )
            {

                var record = db.TempGoodReceives.Find(id);
                #region Checking the grn

                if (record != null)
                {
                    if (record.IsOnlineApproved)
                    {
                        string message = "You cannot manipulate this record,it  is already approved";
                        ModelState.AddModelError("approved", message);
                    }
                    if (record.TempGoodReceiveItem.Count == 0)
                    {
                        string message = "The Record Does not Contain Any Item";
                        ModelState.AddModelError(String.Empty, message);
                    }
                    if (tempGoodReceiveValidation.Status == ApprovalStatuses.Approve)
                    {
                        foreach (var i in record.TempGoodReceiveItem)
                        {
                            var item = db.InventoryItems.FirstOrDefault(ic => ic.ItemCode == i.ItemCode);
                            if (item == null)
                            {
                                string message = "Item " + i.ItemCode + " is not defined in the system.";
                                ModelState.AddModelError("stockItem", message);
                            }
                        }
                    }
                    if (tempGoodReceiveValidation.Status == ApprovalStatuses.Reject)
                    {
                        record.IsSendForApproval = false;
                        record.Status = "Rejected";
                    }
                }
                else
                {
                    ModelState.AddModelError("No Found", "GRN record not found");
                }


                #endregion

                if (ModelState.IsValid)
                {
                    if (record != null)
                    {
                        if (tempGoodReceiveValidation.Status == ApprovalStatuses.Approve)
                        {
                            record.Status = "Approved";
                            record.OnlineApprovedTime = DateTime.Now.Date;
                            record.OnlineApprovedBy = User.Identity.Name;
                            record.IsOnlineApproved = true;
                            db.TempGoodReceives.Attach(record);
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();
                          
                        }
                        else if (tempGoodReceiveValidation.Status == ApprovalStatuses.Reject)
                        {
                            record.IsSendForApproval = false;
                            record.Status = "Rejected";
                            db.TempGoodReceives.Attach(record);
                            db.Entry(record).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                     

                        var entity = new TempGoodReceiveValidation
                        {
                            TempGoodReceiveID = id,
                            Status = tempGoodReceiveValidation.Status,
                            Remark = tempGoodReceiveValidation.Remark,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now
                        };

                        db.TempGoodReceiveValidations.Add(entity);
                        db.SaveChanges();
                        tempGoodReceiveValidation.TempGoodReceiveValidationID = entity.TempGoodReceiveValidationID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "temp good receive approval", "Success", User.Identity.Name, $"approved temp good receive {entity.TempGoodReceiveID} ", Modules.Inventory);
                    #endregion
                }
            }
                

            return Json(new[] { tempGoodReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TempGoodReceiveValidations_Update([DataSourceRequest]DataSourceRequest request, TempGoodReceiveValidation tempGoodReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new TempGoodReceiveValidation
                {
                    TempGoodReceiveValidationID = tempGoodReceiveValidation.TempGoodReceiveValidationID,
                    Status = tempGoodReceiveValidation.Status,
                    Remark = tempGoodReceiveValidation.Remark,
                };

                db.TempGoodReceiveValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { tempGoodReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TempGoodReceiveValidations_Destroy([DataSourceRequest]DataSourceRequest request, TempGoodReceiveValidation tempGoodReceiveValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new TempGoodReceiveValidation
                {
                    TempGoodReceiveValidationID = tempGoodReceiveValidation.TempGoodReceiveValidationID,
                    Status = tempGoodReceiveValidation.Status,
                    Remark = tempGoodReceiveValidation.Remark,
                };

                db.TempGoodReceiveValidations.Attach(entity);
                db.TempGoodReceiveValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { tempGoodReceiveValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
