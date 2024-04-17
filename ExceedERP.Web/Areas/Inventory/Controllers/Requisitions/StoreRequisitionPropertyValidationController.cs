using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.ApplicationServices.Inventory;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.CRM;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Requisitions
{
   
    public class StoreRequisitionPropertyValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly DistributionHelper _distributionHelper = new DistributionHelper();

        [AuthorizeRoles(InventoryConstants.PropertyUser, InventoryConstants.PropertyAuthrizer)]
        public ActionResult Index()
        {
            return View();
        }

          [AuthorizeRoles(InventoryConstants.PropertyUser, InventoryConstants.PropertyAuthrizer)]
        public ActionResult StoreRequisitionPropertyValidations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<StoreRequisitionPropertyValidation> storerequisitionpropertyvalidations = db.StoreRequisitionPropertyValidations.Where(x => x.StoreRequisitionID == id);
            DataSourceResult result = storerequisitionpropertyvalidations.ToDataSourceResult(request, storeRequisitionPropertyValidation => new
            {
                StoreRequisitionPropertyValidationID = storeRequisitionPropertyValidation.StoreRequisitionPropertyValidationID,
                StoreRequisitionID = storeRequisitionPropertyValidation.StoreRequisitionID,
                Status = storeRequisitionPropertyValidation.Status,
                Remark = storeRequisitionPropertyValidation.Remark,
            });

            return Json(result);
        }

          [AuthorizeRoles(InventoryConstants.PropertyAuthrizer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreRequisitionPropertyValidations_Create([DataSourceRequest]DataSourceRequest request, StoreRequisitionPropertyValidation storeRequisitionPropertyValidation, int id)
          {
            InventoryItemService inventoryItemService = new InventoryItemService();
            var storeRequisition = db.StoreRequisitions.Find(id);
              if (storeRequisition!=null)
              {
                  if (storeRequisition.IsSecondOnlineApproved)
                  {
                      ModelState.AddModelError(String.Empty, "You Can Not Manipulate this Record: It is Already Approved");
                  }
                  if (storeRequisitionPropertyValidation.Status == ApprovalStatuses.Approve)
                  {
                      foreach (var item in storeRequisition.StoreRequisitionItem)
                      {
                          var currentBalance = inventoryItemService.GetStoreItemBalance(item.ItemCode, item.StoreRequisition.StoreCode);
                          if (item.IssuedQuantity > currentBalance)
                          {
                            if(currentBalance>0)
                              ModelState.AddModelError("no balance", "There is no enough balance in store for item: " + item.InventoryItem.Name + " current balance is: " + currentBalance);
                            else
                                ModelState.AddModelError("no balance", $"There is no enough balance or the item {item.InventoryItem.Name} is not in store {item.StoreRequisition.StoreCode} ");
                          }

                      }
                  }
              }
              else
              {
                  ModelState.AddModelError("notfound", "Record not found."); 
              }
            

            if (ModelState.IsValid)
            {
                if (storeRequisition != null)
                {
                    if (storeRequisitionPropertyValidation.Status == ApprovalStatuses.Approve)
                    {

                        storeRequisition.Status = "Property Approved";
                        storeRequisition.IsSecondOnlineApproved = true;
                        storeRequisition.SecondOnlineApprovedBy = User.Identity.Name;
                        storeRequisition.SecondOnlineApprovedTime = DateTime.Now;
                        db.Entry(storeRequisition).State = EntityState.Modified;
                        db.SaveChanges();
                        if (storeRequisition.IsTransfer)
                        {
                            #region transfer to transfer send

                            TransferStoreRequisitionItemsToTransfer(storeRequisition);

                            #endregion
                            #region transfer to purchase

                            TransferToPurchaseRequisition(storeRequisition);

                            #endregion
                        }
                        else
                        {
                         
                                #region transfer to issue

                                TransferStoreRequisitionItemsToIssue(storeRequisition);

                                #endregion
                            

                            #region transfer to purchase

                            TransferToPurchaseRequisition(storeRequisition);

                            #endregion
                        }

                    }
                    else if (storeRequisitionPropertyValidation.Status == ApprovalStatuses.Reject)
                    {
                        storeRequisition.IsSecondSendForApproval = false;
                        storeRequisition.Status = "Rejected";
                        db.Entry(storeRequisition).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    var entity = new StoreRequisitionPropertyValidation
                    {
                        StoreRequisitionID = id,
                        Status = storeRequisitionPropertyValidation.Status,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = storeRequisitionPropertyValidation.Remark,
                    };



                    db.StoreRequisitionPropertyValidations.Add(entity);
                    db.SaveChanges();
                    storeRequisitionPropertyValidation.StoreRequisitionPropertyValidationID =
                        entity.StoreRequisitionPropertyValidationID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store requisition property approval", "Success", User.Identity.Name, $"approved store requisition {entity.StoreRequisitionID} ", Modules.Inventory);
                    #endregion
                }
            }


            return Json(new[] { storeRequisitionPropertyValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyAuthrizer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreRequisitionPropertyValidations_Update([DataSourceRequest]DataSourceRequest request, StoreRequisitionPropertyValidation storeRequisitionPropertyValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionPropertyValidation
                {
                    StoreRequisitionPropertyValidationID = storeRequisitionPropertyValidation.StoreRequisitionPropertyValidationID,
                    StoreRequisitionID = storeRequisitionPropertyValidation.StoreRequisitionID,
                    Status = storeRequisitionPropertyValidation.Status,
                    Remark = storeRequisitionPropertyValidation.Remark,
                };

                db.StoreRequisitionPropertyValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { storeRequisitionPropertyValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyAuthrizer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StoreRequisitionPropertyValidations_Destroy([DataSourceRequest]DataSourceRequest request, StoreRequisitionPropertyValidation storeRequisitionPropertyValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionPropertyValidation
                {
                    StoreRequisitionPropertyValidationID = storeRequisitionPropertyValidation.StoreRequisitionPropertyValidationID,
                    Status = storeRequisitionPropertyValidation.Status,
                    Remark = storeRequisitionPropertyValidation.Remark,
                    StoreRequisitionID = storeRequisitionPropertyValidation.StoreRequisitionID,
                };

                db.StoreRequisitionPropertyValidations.Attach(entity);
                db.StoreRequisitionPropertyValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { storeRequisitionPropertyValidation }.ToDataSourceResult(request, ModelState));
        }

        private void TransferStoreRequisitionItemsToTransfer(StoreRequisition storerequisition)
        {
            InventoryItemService inventoryItemService = new InventoryItemService();
            var exist = db.ItemTransfers.Where(x => x.StoreRequisitionID == storerequisition.StoreRequisitionID)
                .AsNoTracking().ToList();
            if (!exist.Any())
            {
                bool transferrable = false;

                #region check if there is anything to issue

                foreach (var requestedItem in storerequisition.StoreRequisitionItem)
                {
                    if (requestedItem.IssuedQuantity > 0)
                    {
                        transferrable = true;
                        break;
                    }
                }

                #endregion

                if (transferrable)
                {
                    #region transfer

                    #region create transfer

                    var itemTransfer = new ItemTransfer
                    {
                        StoreRequisitionID = storerequisition.StoreRequisitionID,
                        TransferDate = DateTime.Today,
                        FromStore = storerequisition.StoreCode,
                        BranchId = storerequisition.BranchId,
                        BusinessUnitId = storerequisition.BusinessUnitId,
                        OrgBranchName = inventoryItemService.GetStoreBranch(storerequisition.StoreCode).ToString(),
                        ToStore = storerequisition.ToStoreCode,
                        DeliveredBy = "",
                        PlateNo = "",
                        SupplierName = "",
                        DriverType = 0,
                        Status = "Editing",
                        Remark = storerequisition.Remark,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        LastModified = storerequisition.LastModified,
                        ModifiedBy = storerequisition.ModifiedBy,
                    };

                    db.ItemTransfers.Add(itemTransfer);
                    db.SaveChanges();

                    #endregion

                    foreach (var requestedItem in storerequisition.StoreRequisitionItem)
                    {
                        if (requestedItem.IssuedQuantity > 0)
                        {
                            decimal avgPrice = inventoryItemService.GetItemAveragePrice(requestedItem.ItemCode,
                                storerequisition.StoreCode);
                            var transferItem = new ItemTransferItem
                            {
                                ItemTransferID = itemTransfer.ItemTransferID,
                                ItemCategoryCode = requestedItem.ItemCategoryCode,
                                ItemCode = requestedItem.ItemCode,
                                RequestedQuantity = requestedItem.ApprovedQuantity,
                                UPrice = avgPrice,
                                PartOrCodeNumber = requestedItem.PartNo,
                                TPrice = requestedItem.ApprovedQuantity * avgPrice,
                                ShippedQuantity = requestedItem.IssuedQuantity,
                                ReceivedQuantity = 0,
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name
                            };

                            db.ItemTransferItems.Add(transferItem);
                            db.SaveChanges();
                        }

                    }

                    #endregion

                    #region distribution

                    _distributionHelper.UpdateTransferSendDistribution(itemTransfer);

                    #endregion

                }
            }

        }
        private void TransferStoreRequisitionItemsToIssue(StoreRequisition storerequisition)
        {
            InventoryItemService inventoryItemService = new InventoryItemService();
            var exist = db.Issues.Where(x => x.StoreRequisitionID == storerequisition.StoreRequisitionID)
                .AsNoTracking().ToList();
            if (!exist.Any())
            {
                bool transferrable = false;

                #region check if there is anything to issue

                foreach (var requestedItem in storerequisition.StoreRequisitionItem)
                {
                    if (requestedItem.IssuedQuantity > 0)
                    {
                        transferrable = true;
                        break;
                    }
                }

                #endregion

                if (transferrable)
                {
                    #region   issue

                    #region create issue

                    var issue = new Issue
                    {
                        OrgStructureId = storerequisition.OrgStructureId,
                        PlateNo = storerequisition.PlateNo,
                        EmployeeId = (int)storerequisition.EmployeeId,
                        BranchId =storerequisition.BranchId,
                        BusinessUnitId = storerequisition.BusinessUnitId,
                        ForBranchId = storerequisition.IsForOther? (int)storerequisition.ForBranchId: storerequisition.BranchId,
                        OrgBranchName = inventoryItemService.GetStoreBranch(storerequisition.StoreCode).ToString(),
                        StoreCode = storerequisition.StoreCode,
                        StoreRequisitionID = storerequisition.StoreRequisitionID,
                        Reference = storerequisition.Reference,
                        GLPeriodId = storerequisition.GLPeriodId,
                        IssueDate = storerequisition.RequestDate,
                        IsFuel = storerequisition.IsFuel,
                        SupplierId = storerequisition.SupplierId,
                        IsLoan = storerequisition.IsLoan,
                        Status = "Editing",
                        Remark = storerequisition.Remark,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        LastModified = storerequisition.LastModified,
                        ModifiedBy = storerequisition.ModifiedBy,
                    };

                    if (storerequisition.IsForOther)
                    {
                        if (storerequisition?.ForBranchId != null) issue.BranchId = (int) storerequisition?.ForBranchId;
                    }

                    db.Issues.Add(issue);
                    db.SaveChanges();

                    #endregion

                    foreach (var requestedItem in storerequisition.StoreRequisitionItem)
                    {
                        if (requestedItem.IssuedQuantity > 0)
                        {
                            #region items

                            decimal avgPrice = inventoryItemService.GetItemAveragePrice(requestedItem.ItemCode,
                                storerequisition.StoreCode);


                            var issueItem = new IssueItem
                            {

                                IssueID = issue.IssueID,
                                ItemCategoryCode = requestedItem.ItemCategoryCode,
                                ItemCode = requestedItem.ItemCode,
                                RequestedQuantity = requestedItem.RequestedQuantity,
                                ApprovedQuantity = requestedItem.ApprovedQuantity,
                                IssuedQuantity = requestedItem.IssuedQuantity,
                                UnitPrice = avgPrice,
                                Total = avgPrice * requestedItem.IssuedQuantity,
                                AgreementPrice = requestedItem.AgreementPrice,


                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Today,
                                ModifiedBy = User.Identity.Name,
                                LastModified = DateTime.Today
                            };
                            db.IssueItems.Add(issueItem);
                            db.SaveChanges();

                            #endregion
                        }

                    }

                    #endregion

                    #region distribution

                    _distributionHelper.UpdateIssueDistribution(issue);

                    #endregion

                }

            }
        }
        private void TransferToPurchaseRequisition(StoreRequisition storerequisition)
        {
            bool transferrable = false;
            var exist = db.PurchaseRequisitions.Where(x=>x.ItemType==PurchaseItemType.Stock).Where(x => x.StoreRequisitionID == storerequisition.StoreRequisitionID)
                .AsNoTracking().ToList();
            if (!exist.Any())
            {
                //&& requestedItem.GeneratePurchaseReq
                #region check if there is anything to purchase

                foreach (var requestedItem in storerequisition.StoreRequisitionItem)
                {
                    if (requestedItem.PurchaseQuantity > 0)
                    {
                        transferrable = true;
                        break;
                    }
                }

                #endregion

                if (transferrable)
                {
                    #region purchase

                    #region create purchase requisition

                    int forbranch = 0;
                    if (storerequisition.IsForOther && !storerequisition.IsTransfer)
                    {
                        if (storerequisition.ForBranchId != null)
                            forbranch = (int) storerequisition.ForBranchId;
                    }
                    else if (!storerequisition.IsForOther && !storerequisition.IsTransfer)
                    {
                        forbranch = storerequisition.BranchId;

                    }
                    else if(storerequisition.IsTransfer)
                    {
                        forbranch = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storerequisition.ToStoreCode);
                    }
                    var purchaseRequisition = new PurchaseRequisition
                    {
                        StoreRequisitionID = storerequisition.StoreRequisitionID,
                        BranchId = storerequisition.BranchId,
                        BusinessUnitId = storerequisition.BusinessUnitId,
                        ForBranchId = forbranch,
                        OrgBranchName = storerequisition.BranchId.ToString(),
                        OrgStructureId = storerequisition.OrgStructureId,
                        EmployeeId = (int)storerequisition.EmployeeId,
                        ReferenceNo = storerequisition.Reference,
                        ProcurementOfficer = 0,
                        RequestDate = storerequisition.RequestDate,
                        ItemType = PurchaseItemType.Stock,
                        PlateNo = storerequisition.PlateNo,
                        Status = "Editing",
                        ModelNo=storerequisition.ModelNo,
                        Remark = storerequisition.Remark,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        LastModified = storerequisition.LastModified,
                        ModifiedBy = storerequisition.ModifiedBy,
                    };

                    db.PurchaseRequisitions.Add(purchaseRequisition);
                    db.SaveChanges();

                    #endregion

                    foreach (var requestedItem in storerequisition.StoreRequisitionItem)
                    {

                        if (requestedItem.PurchaseQuantity > 0)
                        {
                            #region stock item

                            var item = db.InventoryItems.Where(x => x.ItemCode == requestedItem.ItemCode).FirstOrDefault();

                            var purchaseRequisitionItem = new PurchaseRequisitionItem
                            {
                                PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                                CatCode = requestedItem.ItemCategoryCode,
                                CategoryName = requestedItem.ItemCategory.Name,
                                ItmCode = requestedItem.ItemCode,
                                ItemName = item.Name,
                                PartNo = requestedItem.PartNo,
                                ModelNo = requestedItem.ModelNo,
                                ChasisNo=requestedItem.ChasisNo,
                                UoM = item.UoMName,
                                RequestedQuantity = requestedItem.PurchaseQuantity,
                                Quantity = requestedItem.PurchaseQuantity,
                                ItemSpecification = requestedItem.ItemSpecification,
                                ItemType = PurchaseItemType.Stock,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now
                            };
                            db.PurchaseRequisitionItems.Add(purchaseRequisitionItem);
                            db.SaveChanges();

                            #endregion
                        }

                    }

                    foreach (var attachement in storerequisition.StoreRequisitionAttachments)
                    {
                        #region attachment
                        
                        var prattachment = new PurchaseRequisitionAttachment
                        {
                            PurchaseRequisitionID = purchaseRequisition.PurchaseRequisitionID,
                            FileName = attachement.FileName,
                            FilePath = attachement.FilePath
                        };
                        db.PurchaseRequisitionAttachments.Add(prattachment);
                        db.SaveChanges();

                        #endregion

                    }

                    #endregion
                }
            }

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
