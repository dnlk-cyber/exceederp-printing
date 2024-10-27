using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{

    public class PurchaseReturnValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult PurchaseReturnValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PurchaseReturnValidation> purchasereturnvalidations = db.PurchaseReturnValidations.Where(pr => pr.PurchaseReturnID == id);
            DataSourceResult result = purchasereturnvalidations.ToDataSourceResult(request, purchaseReturnValidation => new
            {
                PurchaseReturnValidationID = purchaseReturnValidation.PurchaseReturnValidationID,
                Status = purchaseReturnValidation.Status,
                Remark = purchaseReturnValidation.Remark,
                DateCreated = purchaseReturnValidation.DateCreated,
                LastModified = purchaseReturnValidation.LastModified,
                CreatedBy = purchaseReturnValidation.CreatedBy,
                ModifiedBy = purchaseReturnValidation.ModifiedBy
            });

            return Json(result);
        }


        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseReturnValidations_Create([DataSourceRequest]DataSourceRequest request, PurchaseReturnValidation purchaseReturnValidation, int id)
        {
            var purchaseReturn = db.PurchaseReturns.Find(id);
            var storeItemAssignment = db.StoreItemAssignments.Where(st => st.StoreCode == purchaseReturn.StoreCode);
            
                if ( purchaseReturn != null && purchaseReturn.IsOnlineApproved)
                {
                    ModelState.AddModelError("approved","The record is already approved. you can not approve again.");
                }
                if (ModelState.IsValid)
                    {
                        if (purchaseReturn != null)
                        {
                            if (purchaseReturnValidation.Status == ApprovalStatuses.Approve)
                            {
                                foreach (var item in purchaseReturn.PurchaseReturnItems)
                                {
                                    #region update store Balance
                                    var storeItem =storeItemAssignment.FirstOrDefault(it => it.ItemCode == item.ItemCode);
                                    if (storeItem != null)
                                    {
                                        storeItem.Balance = storeItem.Balance- item.Quantity;
                                        db.StoreItemAssignments.Attach(storeItem);
                                        db.Entry(storeItem).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    #endregion
                                    #region Item Balance

                                    var branchItemAssignment = db.BranchItemAssignments.FirstOrDefault(x => x.ItemCode==item.ItemCode);
                                    if (branchItemAssignment != null)
                                    {
                                        branchItemAssignment.Balance = branchItemAssignment.Balance - item.Quantity;
                                        db.BranchItemAssignments.Attach(branchItemAssignment);
                                        db.Entry(storeItem).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                    #endregion
                                    #region Item Balance

                                    var inventoryItem = db.InventoryItems.Find(item.ItemCode);
                                    if (inventoryItem != null)
                                    {
                                        inventoryItem.Balance = inventoryItem.Balance- item.Quantity;
                                        db.InventoryItems.Attach(inventoryItem);
                                        db.Entry(storeItem).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }

                                    #endregion
                                }
                                purchaseReturn.Status = "Approved";
                                purchaseReturn.IsOnlineApproved = true;
                                purchaseReturn.OnlineApprovedBy = User.Identity.Name;
                                purchaseReturn.OnlineApprovedTime = DateTime.Now;
                            }
                            else if (purchaseReturnValidation.Status == ApprovalStatuses.Reject)
                            {
                                purchaseReturn.IsSendForApproval = false;
                                purchaseReturn.Status = "Rejected";
                            }

                            db.PurchaseReturns.Attach(purchaseReturn);
                            db.Entry(purchaseReturn).State = EntityState.Modified;
                            db.SaveChanges();
                            var entity = new PurchaseReturnValidation
                            {
                                PurchaseReturnID = id,
                                Status = purchaseReturnValidation.Status,
                                Remark = purchaseReturnValidation.Remark,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now
                            };

                            db.PurchaseReturnValidations.Add(entity);
                            db.SaveChanges();
                            purchaseReturnValidation.PurchaseReturnValidationID = entity.PurchaseReturnValidationID;
                        }

                    }
            
            
            return Json(new[] { purchaseReturnValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseReturnValidations_Update([DataSourceRequest]DataSourceRequest request, PurchaseReturnValidation purchaseReturnValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseReturnValidation
                {
                    PurchaseReturnValidationID = purchaseReturnValidation.PurchaseReturnValidationID,
                    Status = purchaseReturnValidation.Status,
                    Remark = purchaseReturnValidation.Remark,
                    DateCreated = purchaseReturnValidation.DateCreated,
                    LastModified = purchaseReturnValidation.LastModified,
                    CreatedBy = purchaseReturnValidation.CreatedBy,
                    ModifiedBy = purchaseReturnValidation.ModifiedBy
                };

                db.PurchaseReturnValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { purchaseReturnValidation }.ToDataSourceResult(request, ModelState));
        }


         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PurchaseReturnValidations_Destroy([DataSourceRequest]DataSourceRequest request, PurchaseReturnValidation purchaseReturnValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new PurchaseReturnValidation
                {
                    PurchaseReturnValidationID = purchaseReturnValidation.PurchaseReturnValidationID,
                    Status = purchaseReturnValidation.Status,
                    Remark = purchaseReturnValidation.Remark,
                    DateCreated = purchaseReturnValidation.DateCreated,
                    LastModified = purchaseReturnValidation.LastModified,
                    CreatedBy = purchaseReturnValidation.CreatedBy,
                    ModifiedBy = purchaseReturnValidation.ModifiedBy
                };

                db.PurchaseReturnValidations.Attach(entity);
                db.PurchaseReturnValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { purchaseReturnValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
