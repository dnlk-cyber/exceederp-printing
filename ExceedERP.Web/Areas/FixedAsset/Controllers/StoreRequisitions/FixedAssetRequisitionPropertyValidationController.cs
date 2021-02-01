using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Issues;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.StoreRequisitions
{

    public class FixedAssetRequisitionPropertyValidationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult FixedAssetRequisitionPropertyValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FixedAssetRequisitionPropertyValidation> fixedAssetRequisitionpropertyvalidations = _db.FixedAssetRequisitionPropertyValidations.Where(x => x.FixedAssetRequisitionId == id);
            DataSourceResult result = fixedAssetRequisitionpropertyvalidations.ToDataSourceResult(request, fixedAssetRequisitionPropertyValidation => new
            {
                FixedAssetRequisitionPropertyValidationId = fixedAssetRequisitionPropertyValidation.FixedAssetRequisitionPropertyValidationId,
                FixedAssetRequisitionId = fixedAssetRequisitionPropertyValidation.FixedAssetRequisitionId,
                Status = fixedAssetRequisitionPropertyValidation.Status,
                Remark = fixedAssetRequisitionPropertyValidation.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionPropertyValidations_Create([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionPropertyValidation fixedAssetRequisitionPropertyValidation, int id)
        {
            FixedAssetRequisition fixedAssetRequisition = _db.FixedAssetRequisitions.Find(id);
            if (fixedAssetRequisition != null && fixedAssetRequisition.IsSecondOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate this Record: It is Already Approved");
            }

            if (ModelState.IsValid)
            {
                if (fixedAssetRequisitionPropertyValidation.Status == ApprovalStatuses.Approve)
                {
                    if (fixedAssetRequisition != null)
                    {
                        fixedAssetRequisition.Status = "Property Approved";
                        fixedAssetRequisition.IsSecondOnlineApproved = true;
                        fixedAssetRequisition.SecondOnlineApprovedBy = User.Identity.Name;
                        fixedAssetRequisition.SecondOnlineApprovedTime = DateTime.Now;
                        _db.Entry(fixedAssetRequisition).State = EntityState.Modified;
                        _db.SaveChanges();
                        if (fixedAssetRequisition.IsTransfer)
                        {
                            #region transfer to transfer send

                           // TransferfixedAssetRequisitionItemsToTransfer(fixedAssetRequisition);

                            #endregion
                            #region transfer to purchase

                            TransferToPurchaseRequisition(fixedAssetRequisition);

                            #endregion
                        }
                        else
                        {

                            #region transfer to issue

                            TransferRequisitionItemsToIssue(fixedAssetRequisition);

                            #endregion
                            #region transfer to purchase

                           TransferToPurchaseRequisition(fixedAssetRequisition);

                            #endregion
                        }
                    }
                }
                else if (fixedAssetRequisitionPropertyValidation.Status == ApprovalStatuses.Reject)
                {
                    if (fixedAssetRequisition != null)
                    {
                        fixedAssetRequisition.IsSendForApproval = false;
                        fixedAssetRequisition.Status = "Rejected";
                        _db.Entry(fixedAssetRequisition).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                }

                var entity = new FixedAssetRequisitionPropertyValidation
                {
                    FixedAssetRequisitionId = id,
                    Status = fixedAssetRequisitionPropertyValidation.Status,
                    Approver = User.Identity.Name,
                    Remark = fixedAssetRequisitionPropertyValidation.Remark,
                };

               _db.FixedAssetRequisitionPropertyValidations.Add(entity);
                _db.SaveChanges();
                fixedAssetRequisitionPropertyValidation.FixedAssetRequisitionPropertyValidationId = entity.FixedAssetRequisitionPropertyValidationId;
            }


            return Json(new[] { fixedAssetRequisitionPropertyValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionPropertyValidations_Update([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionPropertyValidation fixedAssetRequisitionPropertyValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisitionPropertyValidation
                {
                    FixedAssetRequisitionPropertyValidationId = fixedAssetRequisitionPropertyValidation.FixedAssetRequisitionPropertyValidationId,
                    FixedAssetRequisitionId = fixedAssetRequisitionPropertyValidation.FixedAssetRequisitionId,
                    Status = fixedAssetRequisitionPropertyValidation.Status,
                    Remark = fixedAssetRequisitionPropertyValidation.Remark,
                };

                _db.FixedAssetRequisitionPropertyValidations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { fixedAssetRequisitionPropertyValidation }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetRequisitionPropertyValidations_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetRequisitionPropertyValidation fixedAssetRequisitionPropertyValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetRequisitionPropertyValidation
                {
                    FixedAssetRequisitionPropertyValidationId = fixedAssetRequisitionPropertyValidation.FixedAssetRequisitionPropertyValidationId,
                    Status = fixedAssetRequisitionPropertyValidation.Status,
                    Remark = fixedAssetRequisitionPropertyValidation.Remark,
                    FixedAssetRequisitionId = fixedAssetRequisitionPropertyValidation.FixedAssetRequisitionId,
                };

                _db.FixedAssetRequisitionPropertyValidations.Attach(entity);
                _db.FixedAssetRequisitionPropertyValidations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { fixedAssetRequisitionPropertyValidation }.ToDataSourceResult(request, ModelState));
        }

        //private void TransferfixedAssetRequisitionItemsToTransfer(FixedAssetRequisition fixedAssetRequisition)
        //{
        //    bool transferrable = false;
        //    #region check if there is anything to issue

        //    foreach (var requestedItem in fixedAssetRequisition.FixedAssetRequisitionItem)
        //    {
        //        if (requestedItem.IssuedQuantity > 0)
        //        {
        //            transferrable = true;
        //        }
        //    }

        //    #endregion

        //    if (transferrable)
        //    {
        //        #region transfer
        //        #region create transfer
        //        var itemTransfer = new ItemTransfer
        //        {
        //            // FixedAssetRequisitionId = fixedAssetRequisition.FixedAssetRequisitionId,
        //            TransferDate = DateTime.Today,
        //            FromStore = fixedAssetRequisition.StoreCode,
        //            ToStore = fixedAssetRequisition.ToStoreCode,
        //            DeliveredBy = "",
        //            PlateNo = "",
        //            SupplierName = "",
        //            DriverType = 0,
        //            Status = "Editing",
        //            Remark = fixedAssetRequisition.Remark,

        //            DateCreated = DateTime.Now,
        //            CreatedBy = User.Identity.Name,
        //            LastModified = fixedAssetRequisition.LastModified,
        //            ModifiedBy = fixedAssetRequisition.ModifiedBy,
        //        };

        //        _db.ItemTransfers.Add(itemTransfer);
        //        _db.SaveChanges();
        //        #endregion

        //        foreach (var requestedItem in fixedAssetRequisition.FixedAssetRequisitionItem)
        //        {


        //            var transferItem = new ItemTransferItem
        //            {
        //                ItemTransferID = itemTransfer.ItemTransferID,
        //                ItemCategoryCode = requestedItem.FixedAssetCategory.CategoryCode,
        //                ItemCode = requestedItem.FixedAssetSubCategory.SubCategoryCode,
        //                RequestedQuantity = requestedItem.ApprovedQuantity,
        //                UPrice = 0,
        //                //PartOrCodeNumber = requestedItem.CategoryCode.ToString(),
        //                TPrice = 0,
        //                ShippedQuantity = requestedItem.IssuedQuantity,
        //                ReceivedQuantity = 0,
        //                DateCreated = DateTime.Now,
        //                CreatedBy = User.Identity.Name
        //            };

        //            _db.ItemTransferItems.Add(transferItem);
        //            _db.SaveChanges();

        //        }

        //        #endregion


        //    }

        //}
        private void TransferRequisitionItemsToIssue(FixedAssetRequisition fixedAssetRequisition)
        {
            bool transferrable = false;
            #region check if there is anything to issue

            foreach (var requestedItem in fixedAssetRequisition.FixedAssetRequisitionItem)
            {
                if (requestedItem.IssuedQuantity > 0)
                {
                    transferrable = true;
                    continue;
                }
            }

            #endregion

            if (transferrable)
            {
                #region transfer to issue
                #region create issue
                var fixedAssetIssue = new FixedAssetIssue
                {
                    OrgStructureId = fixedAssetRequisition.OrgStructureId,
                    BranchId = fixedAssetRequisition.BranchId,
                    StoreCode = fixedAssetRequisition.StoreCode,
                    FixedAssetRequisitionId = fixedAssetRequisition.FixedAssetRequisitionId,
                    Reference = fixedAssetRequisition.Reference,
                    IssueDate = DateTime.Now,
                    Status = "Editing",
                    Remark = fixedAssetRequisition.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = fixedAssetRequisition.LastModified,
                    ModifiedBy = fixedAssetRequisition.ModifiedBy,
                };

                _db.FixedAssetIssues.Add(fixedAssetIssue);
                _db.SaveChanges();
                #endregion

                foreach (var requestedItem in fixedAssetRequisition.FixedAssetRequisitionItem)
                {

                    for (int i = 0; i < requestedItem.IssuedQuantity; i++)
                    {
                        #region issue item

                        var issueItem = new FixedAssetIssueItem
                        {

                            FixedAssetIssueID = fixedAssetIssue.FixedAssetIssueID,
                            FixedAssetCategoryId = requestedItem.FixedAssetCategoryID,
                            FixedAssetSubCategoryId = requestedItem.FixedAssetSubCategoryID,
                            RequestedQuantity = 1,
                            ApprovedQuantity = 1,
                            IssuedQuantity = 1,
                            UnitPrice = 0,
                            Total = 0,
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,
                           
                        };
                        _db.FixedAssetIssueItems.Add(issueItem);
                        _db.SaveChanges();

                        #endregion
                    }

                }

                #endregion

               
            }

        }
       
        private void TransferToPurchaseRequisition(FixedAssetRequisition fixedAssetRequisition)
        {
            bool transferrable = false;
            #region check if there is anything to purchase

            foreach (var requestedItem in fixedAssetRequisition.FixedAssetRequisitionItem)
            {
                //&& requestedItem.GeneratePurchaseReq
                if (requestedItem.PurchaseQuantity > 0 )
                {
                    transferrable = true;
                    break;
                }
            }

            #endregion

            if (transferrable)
            {
                #region transfer
                #region create purchase requisition

                int forbranch = 0;
                if (fixedAssetRequisition.IsForOther && !fixedAssetRequisition.IsTransfer)
                {
                    if (fixedAssetRequisition.ForBranchId != null)
                        forbranch = (int)fixedAssetRequisition.ForBranchId;
                }
                else if (!fixedAssetRequisition.IsForOther && !fixedAssetRequisition.IsTransfer)
                {
                    forbranch = fixedAssetRequisition.BranchId;

                }
                else if (fixedAssetRequisition.IsTransfer)
                {
                    forbranch = InventoryHelper.GetStoreBranch(fixedAssetRequisition.ToStoreCode);
                }
                var purchaseRequisition = new PurchaseRequisition
                {
                    StoreRequisitionID = fixedAssetRequisition.FixedAssetRequisitionId,
                    BranchId = fixedAssetRequisition.BranchId,
                    ForBranchId = forbranch,
                    OrgBranchName = fixedAssetRequisition.BranchId.ToString(),
                    OrgStructureId = fixedAssetRequisition.OrgStructureId,
                    EmployeeId = fixedAssetRequisition.EmployeeId,
                    ReferenceNo = fixedAssetRequisition.Reference,
                    ProcurementOfficer = 0,
                    RequestDate = fixedAssetRequisition.RequestDate,
                    ItemType = PurchaseItemType.FixedAsset,
                    PlateNo = fixedAssetRequisition.PlateNo,
                    Status = "Editing",
                    Remark = fixedAssetRequisition.Remark +" Plate No.",

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    LastModified = fixedAssetRequisition.LastModified,
                    ModifiedBy = fixedAssetRequisition.ModifiedBy,
                };

                _db.PurchaseRequisitions.Add(purchaseRequisition);
                _db.SaveChanges();

                #endregion

                var requestedItems = _db.FixedAssetRequisitionItems
                    .Where(x => x.FixedAssetRequisitionId == fixedAssetRequisition.FixedAssetRequisitionId).ToList();
                foreach (var requestedItem in requestedItems)
                {

                    if (requestedItem.PurchaseQuantity > 0)
                    {
                        #region fixed asset items

                        var purchaseRequisitionItem = new PurchaseRequisitionItem
                        {
                            PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                            CatCode = requestedItem.FixedAssetCategoryID.ToString(),
                            CategoryName = requestedItem.FixedAssetCategory.CategoryName,
                            ItmCode = requestedItem.FixedAssetSubCategoryID.ToString(),
                            ItemName = requestedItem.FixedAssetSubCategory.SubCategoryName,
                            PartNo = "",
                            UoM = requestedItem.UoM,
                            RequestedQuantity = requestedItem.PurchaseQuantity,
                            Quantity = requestedItem.PurchaseQuantity,
                            ItemSpecification = requestedItem.ItemSpecification,
                            Name=requestedItem.Name,
                            ModelNo = requestedItem.ModelNo,
                            ItemType = PurchaseItemType.FixedAsset,
                            
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now
                        };
                        _db.PurchaseRequisitionItems.Add(purchaseRequisitionItem);
                        _db.SaveChanges();

                        #endregion
                    }

                }

                #endregion
            }

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
