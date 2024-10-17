using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Issues;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
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

        //FixedAsset Requisition Review Validation 
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
                            //Transfer to PO
                            #region transfer to purchase

                            TransferToPurchaseRequisition(fixedAssetRequisition);

                            #endregion
                        }
                        else
                        {
                            //Transfer to issue
                            #region transfer to issue

                            TransferRequisitionItemsToIssue(fixedAssetRequisition);

                            #endregion
                            //Transfer to Purchase
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
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetRequisitionPropertyValidation", "Success", User.Identity.Name, $"created FixedAssetRequisitionPropertyValidation {entity.FixedAssetRequisitionPropertyValidationId} Tag {entity.FixedAssetRequisitionId}", Modules.FixedAsset);
                #endregion
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
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetRequisitionPropertyValidation", "Success", User.Identity.Name, $"Update FixedAssetRequisitionPropertyValidation {entity.FixedAssetRequisitionPropertyValidationId} Tag {entity.FixedAssetRequisitionId}", Modules.FixedAsset);
                #endregion
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
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"created FixedAssetRequisitionPropertyValidation {fixedAssetRequisitionPropertyValidation.FixedAssetRequisitionPropertyValidationId} Tag {fixedAssetRequisitionPropertyValidation.FixedAssetRequisitionId}", Modules.FixedAsset);
                #endregion
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

        /// <summary>
        /// This function will create FixedAssetIssue and FixedAssetIssueItem
        /// it uses FixedAssetRequisition and FixedAssetRequisition.FixedAssetRequisition for data
        /// </summary>
        /// <param name="fixedAssetRequisition"></param>
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
                var fixedAssetIssue = _db.FixedAssetIssues.Where(x => x.FixedAssetRequisitionId == fixedAssetRequisition.FixedAssetRequisitionId).FirstOrDefault();
                if (fixedAssetIssue == null)
                {
                    #region create issue
                    fixedAssetIssue = new FixedAssetIssue
                    {
                        OrgStructureId = fixedAssetRequisition.OrgStructureId,
                        BranchId = fixedAssetRequisition.BranchId,
                        StoreCode = fixedAssetRequisition.StoreCode,
                        FixedAssetRequisitionId = fixedAssetRequisition.FixedAssetRequisitionId,
                        Reference = fixedAssetRequisition.Reference,
                        RecipientId = fixedAssetRequisition.EmployeeId,
                        EmployeeId = fixedAssetRequisition.EmployeeId,
                        GLPeriodId = fixedAssetRequisition.GLPeriodId,
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

                    var items = _db.FixedAssetRequisitionItems.Where(x => x.FixedAssetRequisitionId == fixedAssetRequisition.FixedAssetRequisitionId).ToList();
                    if (items.Any())
                    {
                        foreach (var requestedItem in items)
                        {
                            if (requestedItem.IssuedQuantity > 1)
                            {

                                for (int i = 0; i < requestedItem.IssuedQuantity; i++)
                                {
                                    #region issueitem

                                    var issueItem = new FixedAssetIssueItem
                                    {

                                        FixedAssetIssueID = fixedAssetIssue.FixedAssetIssueID,
                                        FixedAssetCategoryId = requestedItem.FixedAssetCategoryID,
                                        FixedAssetSubCategoryId = requestedItem.FixedAssetSubCategoryID,
                                        FixedAssetNameID = requestedItem.FixedAssetNameID,
                                        AssetRegistrationId = requestedItem.AssetRegistrationId==null? 0:(int)requestedItem.AssetRegistrationId,
                                        Unit = requestedItem.UoM,
                                        Name = requestedItem.Name,
                                        Size = requestedItem.Size,
                                        Model = requestedItem.ModelNo,
                                        EngineNo = requestedItem.EngineNo,
                                        SerialNo = requestedItem.SerialNo,
                                        PlateNo = requestedItem.PlateNo,
                                        SendToUserCard = true,
                                        ChassisNo = requestedItem.ChasisNo,
                                        RequestedQuantity = 1,
                                        ApprovedQuantity = 1,
                                        IssuedQuantity = 1,
                                        UnitPrice = requestedItem.Price,
                                        Total = requestedItem.Price,
                                        CreatedBy = User.Identity.Name,
                                        DateCreated = DateTime.Now,

                                    };
                                    _db.FixedAssetIssueItems.Add(issueItem);
                                    _db.SaveChanges();

                                    #endregion
                                }
                            }
                            else
                            {
                                #region issueitem

                                var issueItem = new FixedAssetIssueItem
                                {

                                    FixedAssetIssueID = fixedAssetIssue.FixedAssetIssueID,
                                    FixedAssetCategoryId = requestedItem.FixedAssetCategoryID,
                                    FixedAssetSubCategoryId = requestedItem.FixedAssetSubCategoryID,
                                    FixedAssetNameID = requestedItem.FixedAssetNameID,
                                    AssetRegistrationId = requestedItem.AssetRegistrationId == null ? 0 : (int)requestedItem.AssetRegistrationId,
                                    Unit = requestedItem.UoM,
                                    Name = requestedItem.Name,
                                    Size = requestedItem.Size,
                                    Model = requestedItem.ModelNo,
                                    EngineNo = requestedItem.EngineNo,
                                    SerialNo = requestedItem.SerialNo,
                                    PlateNo = requestedItem.PlateNo,
                                    SendToUserCard = true,
                                    ChassisNo = requestedItem.ChasisNo,
                                    RequestedQuantity = requestedItem.RequestedQuantity,
                                    ApprovedQuantity = requestedItem.ApprovedQuantity,
                                    IssuedQuantity = requestedItem.IssuedQuantity,
                                    UnitPrice = requestedItem.Price,
                                    Total = requestedItem.Price,
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now,

                                };
                                _db.FixedAssetIssueItems.Add(issueItem);
                                _db.SaveChanges();

                                #endregion
                            }

                        }
                    }
                }

                else
                {
                    var issueitems=_db.FixedAssetIssueItems.Where(x=>x.FixedAssetIssueID== fixedAssetIssue.FixedAssetIssueID).ToList();
                    _db.FixedAssetIssueItems.RemoveRange(issueitems);

                    var items = _db.FixedAssetRequisitionItems.Where(x => x.FixedAssetRequisitionId == fixedAssetRequisition.FixedAssetRequisitionId).ToList();
                    if (items.Any())
                    {
                        foreach (var requestedItem in items)
                        {
                            if (requestedItem.IssuedQuantity > 1)
                            {

                                for (int i = 0; i < requestedItem.IssuedQuantity; i++)
                                {
                                    #region issueitem

                                    var issueItem = new FixedAssetIssueItem
                                    {

                                        FixedAssetIssueID = fixedAssetIssue.FixedAssetIssueID,
                                        FixedAssetCategoryId = requestedItem.FixedAssetCategoryID,
                                        FixedAssetSubCategoryId = requestedItem.FixedAssetSubCategoryID,
                                        FixedAssetNameID = requestedItem.FixedAssetNameID,
                                        AssetRegistrationId = requestedItem.AssetRegistrationId == null ? 0 : (int)requestedItem.AssetRegistrationId,
                                        Unit = requestedItem.UoM,
                                        Name = requestedItem.Name,
                                        Size = requestedItem.Size,
                                        Model = requestedItem.ModelNo,
                                        EngineNo = requestedItem.EngineNo,
                                        SerialNo = requestedItem.SerialNo,
                                        PlateNo = requestedItem.PlateNo,
                                        SendToUserCard = true,
                                        ChassisNo = requestedItem.ChasisNo,
                                        RequestedQuantity = 1,
                                        ApprovedQuantity = 1,
                                        IssuedQuantity = 1,
                                        UnitPrice = requestedItem.Price,
                                        Total = requestedItem.Price,
                                        CreatedBy = User.Identity.Name,
                                        DateCreated = DateTime.Now,

                                    };
                                    _db.FixedAssetIssueItems.Add(issueItem);
                                    _db.SaveChanges();

                                    #endregion
                                }
                            }
                            else
                            {
                                #region issueitem

                                var issueItem = new FixedAssetIssueItem
                                {

                                    FixedAssetIssueID = fixedAssetIssue.FixedAssetIssueID,
                                    FixedAssetCategoryId = requestedItem.FixedAssetCategoryID,
                                    FixedAssetSubCategoryId = requestedItem.FixedAssetSubCategoryID,
                                    FixedAssetNameID = requestedItem.FixedAssetNameID,
                                    AssetRegistrationId = requestedItem.AssetRegistrationId == null ? 0 : (int)requestedItem.AssetRegistrationId,
                                    Unit = requestedItem.UoM,
                                    Name = requestedItem.Name,
                                    Size = requestedItem.Size,
                                    Model = requestedItem.ModelNo,
                                    EngineNo = requestedItem.EngineNo,
                                    SerialNo = requestedItem.SerialNo,
                                    PlateNo = requestedItem.PlateNo,
                                    SendToUserCard = true,
                                    ChassisNo = requestedItem.ChasisNo,
                                    RequestedQuantity = requestedItem.RequestedQuantity,
                                    ApprovedQuantity = requestedItem.ApprovedQuantity,
                                    IssuedQuantity = requestedItem.IssuedQuantity,
                                    UnitPrice = requestedItem.Price,
                                    Total = requestedItem.Price,
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now,

                                };
                                _db.FixedAssetIssueItems.Add(issueItem);
                                _db.SaveChanges();

                                #endregion
                            }

                        }
                    }
                }
                #endregion

               
            }

        }
       

       
        /// <summary>
        /// This function will generate a PurchaseRequisition and PurchaseRequisitionItem. it take FixedAssetRequisition as a param and
        ///  will populate values for PurchaseRequisition and FixedAssetRequisition.FixedAssetRequisitionItem for PurchaseRequisitionItem
        /// </summary>
        /// <param name="fixedAssetRequisition"></param>
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
                    Remark = fixedAssetRequisition.Remark,

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
                            ModelNo = requestedItem.ModelNo,
                            ChasisNo =requestedItem.ChasisNo,
                            UoM = requestedItem.UoM,
                            RequestedQuantity = requestedItem.PurchaseQuantity,
                            Quantity = requestedItem.PurchaseQuantity,
                            ItemSpecification = requestedItem.ItemSpecification,
                            Name=requestedItem.Name,
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
