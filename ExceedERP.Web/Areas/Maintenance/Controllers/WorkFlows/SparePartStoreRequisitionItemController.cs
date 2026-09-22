using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.Maintenance.SpareStoreRequisition;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.WorkFlows
{
    [AuthorizeRoles(MaintenanceRoles.MaintenanceOrderUser, MaintenanceRoles.MaintenanceWorkShopUser, MaintenanceRoles.ServiceUser)]

    public class SparePartStoreRequisitionItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        private bool IsSRReviewed(int id)
        {
            var sr = db.StoreRequisitions.Find(id);
            if (sr != null)
            {
                if (sr.IsSecondOnlineApproved)
                    return true;
            }
            return false;
        }
        private string GetStore(int id)
        {
            return db.StoreRequisitions.Find(id).StoreCode;
        }
        //[AuthorizeRoles(InventoryConstants.PropertyRequistion, InventoryConstants.UserDepartment)]
        public ActionResult SparePartStoreRequisitionItems_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<StoreRequisitionItem> storerequisitionItems = db.StoreRequisitionItems.Where(p => p.StoreRequisitionID == id);
            DataSourceResult result = storerequisitionItems.ToDataSourceResult(request, storeRequisitionItem => new 
            {
                
                //AgreementPrice = storeRequisitionItem.AgreementPrice,
                StoreRequisitionItemID = storeRequisitionItem.StoreRequisitionItemID,
                StoreRequisitionID = storeRequisitionItem.StoreRequisitionID,
                ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                ItemCode = storeRequisitionItem.ItemCode,
                CurrentAPrice = storeRequisitionItem.CurrentAPrice,
                PartNo = storeRequisitionItem.PartNo,
                ChasisNo = storeRequisitionItem.ChasisNo,
                SerialNo = storeRequisitionItem.SerialNo,
                ModelNo = storeRequisitionItem.ModelNo,
                //IsLog = storeRequisitionItem.IsLog,
                //IsBiologicalAsset = storeRequisitionItem.IsBiologicalAsset,
                //IsLumber = storeRequisitionItem.IsLumber,
                RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                //IssuedQuantity = GetIssued(storeRequisitionItem.StoreRequisitionID,storeRequisitionItem.ItemCategoryCode,storeRequisitionItem.ItemCode),
                IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                //AgreementPrice=GetIssuedPrice(storeRequisitionItem.StoreRequisitionID, storeRequisitionItem.ItemCategoryCode, storeRequisitionItem.ItemCode),
                PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                //AgreementPrice = storeRequisitionItem.AgreementPrice,
                //AgreementPrice = storeRequisitionItem.CurrentAPrice,
                IsApproved = storeRequisitionItem.IsApproved,
                ItemSpecification = storeRequisitionItem.ItemSpecification,
                Unit= ExceedERP.Helpers.Inventory.InventoryHelper.GetItemUom(storeRequisitionItem.ItemCode),
                CurrentBalance = storeRequisitionItem.CurrentBalance,
               // CurrentAPrice = storeRequisitionItem.CurrentAPrice,
               // CurrentBalance = IsSRReviewed(storeRequisitionItem.StoreRequisitionID) ? storeRequisitionItem.CurrentBalance : ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode, GetStore(storeRequisitionItem.StoreRequisitionID)),
            });

            return Json(result);
        }
        private decimal GetIssuedPrice(int SId, string ICcode, string ICode)
        {
            decimal Value = 0;
            var fetchIssue = db.Issues.FirstOrDefault(x => x.StoreRequisitionID == SId);
            if (fetchIssue != null)
            {
                var fetchItem = db.IssueItems.FirstOrDefault(x => x.ItemCategoryCode == ICcode && x.ItemCode == ICode);
                if (fetchItem != null)
                {
                    Value = fetchItem.IssuedQuantity*fetchItem.UnitPrice;
                }
            }
            return Value;
        }
        private decimal GetIssued(int SId,string ICcode,string ICode)
        {
            decimal Value = 0;
            var fetchIssue = db.Issues.FirstOrDefault(x => x.StoreRequisitionID == SId);
                if(fetchIssue!=null)
            {
                var fetchItem = db.IssueItems.FirstOrDefault(x => x.ItemCategoryCode == ICcode && x.ItemCode == ICode);
                  if(fetchItem!=null)
                {
                    Value = fetchItem.IssuedQuantity;
                }
            }
            return Value;
        }
        private decimal GetUnitPrice(string ItemCategoryCode, string ItemCode)
        {
            var GetAveragePrice = db.InventoryItems.FirstOrDefault(x => x.ItemCategoryCode == ItemCategoryCode && x.ItemCode == ItemCode).AvgPrice;
            return GetAveragePrice;
        }
       // [AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SparePartStoreRequisitionItems_Create([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem , int id)
        {
            var companySetting = db.RequestToInspections.FirstOrDefault();

            if (companySetting != null && companySetting.Remark == "MCG")
            {
                var fuelCategoryCodes = new HashSet<string>(db.FuelItemSettings.Select(x => x.ItemCategoryCode));
                var storeItems = db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == id).ToList();
                if (storeItems.Any())
                {
                    var categoryIds = storeItems.Select(x => x.ItemCategoryCode).ToList();
                    var count = 0;
                    foreach(var itemc in categoryIds)
                    {
                        if(fuelCategoryCodes.Contains(itemc))
                        {
                            count = count + 1;
                        }
                    }
                    if(count!=0&&!fuelCategoryCodes.Contains(storeRequisitionItem.ItemCategoryCode))
                    {
                        ModelState.AddModelError("", "You can not request non Fuel items with Fuel items together");
                    }

                    if(count==0&&fuelCategoryCodes.Contains(storeRequisitionItem.ItemCategoryCode))
                    {
                        ModelState.AddModelError("", "You can not request non Fuel items with Fuel items together");
                    }
                }

                if(fuelCategoryCodes.Contains(storeRequisitionItem.ItemCategoryCode))
                { 
                var sr = db.StoreRequisitions.Find(id);
                storeRequisitionItem.IssuedQuantity = 0;
                storeRequisitionItem.PurchaseQuantity = 0;
                if (storeRequisitionItem.ApprovedQuantity > storeRequisitionItem.RequestedQuantity)
                {
                    ModelState.AddModelError("above", "Approved quantity cannot be more than requested.");
                }
                var exist =
                    db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == id)
                        .Where(x => x.ItemCode == storeRequisitionItem.ItemCode)
                        .Where(x => x.ItemSpecification == storeRequisitionItem.ItemSpecification)//added
                        .AsQueryable();
                if (exist.Any())
                {
                    ModelState.AddModelError("exist", "Item " + storeRequisitionItem.ItemCode + " already exists");
                }
                ModelState.Remove("AgreementPrice");
                    if (ModelState.IsValid)
                    {
                        var entity = new StoreRequisitionItem
                        {
                            StoreRequisitionID = id,
                            ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                            ItemCode = storeRequisitionItem.ItemCode,
                            PartNo = storeRequisitionItem.PartNo,
                            ModelNo = storeRequisitionItem.ModelNo,
                            SerialNo = storeRequisitionItem.SerialNo,
                            
                            ChasisNo = storeRequisitionItem.ChasisNo,
                            RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                            ApprovedQuantity = storeRequisitionItem.RequestedQuantity,
                            IssuedQuantity = storeRequisitionItem.RequestedQuantity,
                            PurchaseQuantity = 0,//assuming all will be given
                            IsApproved = storeRequisitionItem.IsApproved,
                            ItemSpecification = storeRequisitionItem.ItemSpecification,
                            CurrentAPrice = GetUnitPrice(storeRequisitionItem.ItemCategoryCode, storeRequisitionItem.ItemCode),
                            CurrentBalance = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode, sr.StoreCode),

                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,

                        };

                        db.StoreRequisitionItems.Add(entity);
                        db.SaveChanges();
                        sr.IsFuel = true;
                        db.Entry(sr).State = EntityState.Modified;
                        db.SaveChanges();
                        storeRequisitionItem.StoreRequisitionItemID = entity.StoreRequisitionItemID;
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "store requisition item", "Success", User.Identity.Name, $"created store requisition item {entity.ItemCode} under record {entity.StoreRequisitionID} ", Modules.Inventory);
                        #endregion
                    }


                }
                if (!fuelCategoryCodes.Contains(storeRequisitionItem.ItemCategoryCode))
                {
                    var sr = db.StoreRequisitions.Find(id);
                    storeRequisitionItem.IssuedQuantity = 0;
                    storeRequisitionItem.PurchaseQuantity = 0;
                    if (storeRequisitionItem.ApprovedQuantity > storeRequisitionItem.RequestedQuantity)
                    {
                        ModelState.AddModelError("above", "Approved quantity cannot be more than requested.");
                    }
                    var exist =
                        db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == id)
                            .Where(x => x.ItemCode == storeRequisitionItem.ItemCode)
                            .Where(x => x.ItemSpecification == storeRequisitionItem.ItemSpecification)//added
                            .AsQueryable();
                    if (exist.Any())
                    {
                        ModelState.AddModelError("exist", "Item " + storeRequisitionItem.ItemCode + " already exists");
                    }
                    ModelState.Remove("AgreementPrice");
                    if (ModelState.IsValid)
                    {
                        var entity = new StoreRequisitionItem
                        {
                            StoreRequisitionID = id,
                            ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                            ItemCode = storeRequisitionItem.ItemCode,
                            PartNo = storeRequisitionItem.PartNo,
                            ModelNo = storeRequisitionItem.ModelNo,
                            SerialNo = storeRequisitionItem.SerialNo,
                            ChasisNo = storeRequisitionItem.ChasisNo,
                            RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                            ApprovedQuantity = storeRequisitionItem.RequestedQuantity,
                            IssuedQuantity = storeRequisitionItem.RequestedQuantity,
                            PurchaseQuantity = 0,//assuming all will be given
                            IsApproved = storeRequisitionItem.IsApproved,
                            ItemSpecification = storeRequisitionItem.ItemSpecification,
                            CurrentAPrice = GetUnitPrice(storeRequisitionItem.ItemCategoryCode, storeRequisitionItem.ItemCode),
                            CurrentBalance = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode, sr.StoreCode),

                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,

                        };

                        db.StoreRequisitionItems.Add(entity);
                        db.SaveChanges();
                        db.Entry(sr).State = EntityState.Modified;
                        db.SaveChanges();
                        storeRequisitionItem.StoreRequisitionItemID = entity.StoreRequisitionItemID;
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "store requisition item", "Success", User.Identity.Name, $"created store requisition item {entity.ItemCode} under record {entity.StoreRequisitionID} ", Modules.Inventory);
                        #endregion
                    }


                }
            }
            else
            {
                var sr = db.StoreRequisitions.Find(id);
                storeRequisitionItem.IssuedQuantity = 0;
                storeRequisitionItem.PurchaseQuantity = 0;
                if (storeRequisitionItem.ApprovedQuantity > storeRequisitionItem.RequestedQuantity)
                {
                    ModelState.AddModelError("above", "Approved quantity cannot be more than requested.");
                }
                var exist =
                    db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == id)
                        .Where(x => x.ItemCode == storeRequisitionItem.ItemCode)
                        .Where(x => x.ItemSpecification == storeRequisitionItem.ItemSpecification)//added
                        .AsQueryable();
                if (exist.Any())
                {
                    ModelState.AddModelError("exist", "Item " + storeRequisitionItem.ItemCode + " already exists");
                }
                ModelState.Remove("AgreementPrice");
                if (ModelState.IsValid)
                {
                    var entity = new StoreRequisitionItem
                    {
                        StoreRequisitionID = id,
                        ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                        ItemCode = storeRequisitionItem.ItemCode,
                        PartNo = storeRequisitionItem.PartNo,
                        ModelNo = storeRequisitionItem.ModelNo,
                        SerialNo = storeRequisitionItem.SerialNo,
                        ChasisNo = storeRequisitionItem.ChasisNo,
                        //IsLog = storeRequisitionItem.IsLog,
                        //IsBiologicalAsset = storeRequisitionItem.IsBiologicalAsset,
                        //IsLumber = storeRequisitionItem.IsLumber,
                        RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                        ApprovedQuantity = storeRequisitionItem.RequestedQuantity,
                        IssuedQuantity = storeRequisitionItem.RequestedQuantity,
                        PurchaseQuantity = 0,//assuming all will be given
                        IsApproved = storeRequisitionItem.IsApproved,
                        ItemSpecification = storeRequisitionItem.ItemSpecification,
                        CurrentAPrice = GetUnitPrice(storeRequisitionItem.ItemCategoryCode, storeRequisitionItem.ItemCode),
                        CurrentBalance = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode, sr.StoreCode),

                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,

                    };

                    db.StoreRequisitionItems.Add(entity);
                    db.SaveChanges();
                    storeRequisitionItem.StoreRequisitionItemID = entity.StoreRequisitionItemID;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "store requisition item", "Success", User.Identity.Name, $"created store requisition item {entity.ItemCode} under record {entity.StoreRequisitionID} ", Modules.Inventory);
                    #endregion
                }
            }
            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


       //[AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SparePartStoreRequisitionItems_Update([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem)
        {
            var sr = db.StoreRequisitions.Find(storeRequisitionItem.StoreRequisitionID);
            storeRequisitionItem.IssuedQuantity = 0;
            storeRequisitionItem.PurchaseQuantity = 0;
            if (storeRequisitionItem.ApprovedQuantity > storeRequisitionItem.RequestedQuantity)
            {
                ModelState.AddModelError("above", "Approved quantity cannot be more than requested.");
            }

            var exist = db.StoreRequisitionItems
                     .Where(x => x.StoreRequisitionItemID != storeRequisitionItem.StoreRequisitionItemID)
                     .Where(x => x.StoreRequisitionID == storeRequisitionItem.StoreRequisitionID)
                     .Where(x => x.ItemCode == storeRequisitionItem.ItemCode)
                     .Where(x => x.ItemSpecification == storeRequisitionItem.ItemSpecification)//added
                     .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + storeRequisitionItem.ItemCode + " already exists");
            }
            ModelState.Remove("AgreementPrice");
            
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionItem
                {

                    StoreRequisitionID = storeRequisitionItem.StoreRequisitionID,
                    StoreRequisitionItemID = storeRequisitionItem.StoreRequisitionItemID,
                    ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                    ItemCode = storeRequisitionItem.ItemCode,
                    PartNo = storeRequisitionItem.PartNo,
                    ModelNo = storeRequisitionItem.ModelNo,
                    SerialNo = storeRequisitionItem.SerialNo,
                    ChasisNo = storeRequisitionItem.ChasisNo,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                    IssuedQuantity = storeRequisitionItem.ApprovedQuantity,//default issue is equal to approved
                    PurchaseQuantity = 0,//assuming all will be given
                    ItemSpecification = storeRequisitionItem.ItemSpecification,
                    //IsLog = storeRequisitionItem.IsLog,
                    //IsBiologicalAsset = storeRequisitionItem.IsBiologicalAsset,
                    //IsLumber = storeRequisitionItem.IsLumber,
                    DateCreated = storeRequisitionItem.DateCreated,
                    CreatedBy = storeRequisitionItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    CurrentAPrice = GetUnitPrice(storeRequisitionItem.ItemCategoryCode, storeRequisitionItem.ItemCode),
                    CurrentBalance = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreItemBalance(storeRequisitionItem.ItemCode, sr.StoreCode),
                };

                db.StoreRequisitionItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "store requisition item", "Success", User.Identity.Name, $"updated store requisition item {entity.ItemCode} under record {entity.StoreRequisitionID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }


         //[AuthorizeRoles(InventoryConstants.PropertyRequistion)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SparePartStoreRequisitionItems_Destroy([DataSourceRequest]DataSourceRequest request, StoreRequisitionItem storeRequisitionItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new StoreRequisitionItem
                {
                    StoreRequisitionItemID = storeRequisitionItem.StoreRequisitionItemID,
                    ItemCategoryCode = storeRequisitionItem.ItemCategoryCode,
                    ItemCode = storeRequisitionItem.ItemCode,
                    PartNo = storeRequisitionItem.PartNo,
                    ModelNo = storeRequisitionItem.ModelNo,
                    SerialNo = storeRequisitionItem.SerialNo,
                    ChasisNo = storeRequisitionItem.ChasisNo,
                    RequestedQuantity = storeRequisitionItem.RequestedQuantity,
                    ApprovedQuantity = storeRequisitionItem.ApprovedQuantity,
                    IssuedQuantity = storeRequisitionItem.IssuedQuantity,
                    PurchaseQuantity = storeRequisitionItem.PurchaseQuantity,
                    IsApproved = storeRequisitionItem.IsApproved,
                    ItemSpecification = storeRequisitionItem.ItemSpecification,
                    CurrentAPrice = storeRequisitionItem.CurrentAPrice
                };
                db.StoreRequisitionItems.Attach(entity);
                db.StoreRequisitionItems.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted store requisition item " + storeRequisitionItem.StoreRequisitionID + " " + storeRequisitionItem.InventoryItem.Name);
                #endregion
            }

            return Json(new[] { storeRequisitionItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
