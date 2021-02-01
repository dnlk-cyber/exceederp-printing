using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Issues;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Issues
{
    
    public class FixedAssetIssueItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult FixedAssetIssueItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FixedAssetIssueItem> fixedAssetIssueItems = db.FixedAssetIssueItems.Where(i => i.FixedAssetIssueID == id);
            DataSourceResult result = fixedAssetIssueItems.ToDataSourceResult(request, fixedAssetIssueItem => new {
                FixedAssetIssueItemID = fixedAssetIssueItem.FixedAssetIssueItemID,
                FixedAssetIssueID = fixedAssetIssueItem.FixedAssetIssueID,
                FixedAssetCategoryId = fixedAssetIssueItem.FixedAssetCategoryId,
                FixedAssetSubCategoryId = fixedAssetIssueItem.FixedAssetSubCategoryId,
                AssetRegistrationId = fixedAssetIssueItem.AssetRegistrationId,
                SendToUserCard = fixedAssetIssueItem.SendToUserCard,

                Unit = fixedAssetIssueItem.Unit,
                Name = fixedAssetIssueItem.Name,
                Size = fixedAssetIssueItem.Size,
                GrnNo = fixedAssetIssueItem.GrnNo,

                RequestedQuantity = fixedAssetIssueItem.RequestedQuantity,
                ApprovedQuantity = fixedAssetIssueItem.ApprovedQuantity,
                IssuedQuantity = fixedAssetIssueItem.IssuedQuantity,
                UnitPrice = fixedAssetIssueItem.UnitPrice,
                Total = fixedAssetIssueItem.Total,
                Remark = fixedAssetIssueItem.Remark,


                IsMachinery = fixedAssetIssueItem.IsMachinery,
                TagNo = fixedAssetIssueItem.TagNo,
                BatchNo = fixedAssetIssueItem.BatchNo,
                Model = fixedAssetIssueItem.Model,
                SerialNo = fixedAssetIssueItem.SerialNo,
                Condition = fixedAssetIssueItem.Condition,
                PlateNo = fixedAssetIssueItem.PlateNo,
                ChassisNo = fixedAssetIssueItem.ChassisNo,
                EngineNo = fixedAssetIssueItem.EngineNo,
                EngineModelNo = fixedAssetIssueItem.EngineModelNo,
                EngineElectricalNo = fixedAssetIssueItem.EngineElectricalNo,
                EngineElectricalModelNo = fixedAssetIssueItem.EngineElectricalModelNo,
                OrgBranchName = fixedAssetIssueItem.OrgBranchName,
                DateCreated = fixedAssetIssueItem.DateCreated,
                LastModified = fixedAssetIssueItem.LastModified,
                CreatedBy = fixedAssetIssueItem.CreatedBy,
                ModifiedBy = fixedAssetIssueItem.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetIssueItems_Create(int id,[DataSourceRequest]DataSourceRequest request, FixedAssetIssueItem issueItem)
        {
            var asset = db.AssetRegistrations.Find(issueItem.AssetRegistrationId);

            if(asset==null)
                ModelState.AddModelError("doesnotexist", "Please select an asset.");


            string storeCode = "";
            var fixedIssue = db.FixedAssetIssues.Find(id);
            storeCode = fixedIssue?.StoreCode;
            if (!FixedAssetHelper.IsAssetCurrentLocationStore(issueItem.AssetRegistrationId, storeCode))
            {
                string message = "Asset with tag number " + asset?.Tag + " does not exist in store " + storeCode;
                ModelState.AddModelError("doesnotexist", message);

            }
            if (ModelState.IsValid && asset!=null)
            {
                var entity = new FixedAssetIssueItem
                {
                    FixedAssetIssueID = id,
                    FixedAssetCategoryId = issueItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = issueItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = issueItem.AssetRegistrationId,
                    SendToUserCard = issueItem.SendToUserCard,

                    Unit = asset.Unit,
                    Name = asset.Name,
                    Size = asset.Size,
                    GrnNo = asset.GRN,

                    RequestedQuantity = issueItem.RequestedQuantity,
                    ApprovedQuantity = issueItem.ApprovedQuantity,
                    IssuedQuantity = issueItem.IssuedQuantity,
                    UnitPrice = asset.BookValue,
                   
                    Total = issueItem.IssuedQuantity * asset.BookValue,
                    Remark = issueItem.Remark,


                    IsMachinery = asset.IsMachinery,
                    TagNo = asset.Tag,
                    //BatchNo = asset.b,
                    Model = asset.ModelNo,
                    SerialNo = asset.SerialNo,
                    Condition = asset.Condition.Name,
                    PlateNo = asset.PlateNo,
                    ChassisNo = asset.ChassisNo,
                    EngineNo = asset.EngineNo,
                    //EngineModelNo = asset.EngineModelNo,
                   // EngineElectricalNo = asset.EngineElectricalNo,
                   // EngineElectricalModelNo = asset.EngineElectricalModelNo,
                    OrgBranchName = issueItem.OrgBranchName,
                    DateCreated = issueItem.DateCreated,
                    LastModified = issueItem.LastModified,
                    CreatedBy = issueItem.CreatedBy,
                    ModifiedBy = issueItem.ModifiedBy
                };

                db.FixedAssetIssueItems.Add(entity);
                db.SaveChanges();
                issueItem.FixedAssetIssueItemID = entity.FixedAssetIssueItemID;
            }

            return Json(new[] { issueItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetIssueItems_Update([DataSourceRequest]DataSourceRequest request, FixedAssetIssueItem issueItem, int id)
        {
            var asset = db.AssetRegistrations.Find(issueItem.AssetRegistrationId);

            if (asset == null)
                ModelState.AddModelError("doesnotexist", "Please select an asset.");

            string storeCode = "";
            var fixedIssue = db.FixedAssetIssues.Find(id);
            storeCode = fixedIssue?.StoreCode;
            if (!FixedAssetHelper.IsAssetCurrentLocationStore(issueItem.AssetRegistrationId, storeCode))
            {
                string message = "Asset with tag number " + asset?.Tag + " does not exist in store " + storeCode;
                ModelState.AddModelError("doesnotexist", message);

            }

            if (ModelState.IsValid &&asset!=null)
            {
                var entity = new FixedAssetIssueItem
                {
                    FixedAssetIssueItemID = issueItem.FixedAssetIssueItemID,
                    FixedAssetIssueID = issueItem.FixedAssetIssueID,
                    FixedAssetCategoryId = issueItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = issueItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = issueItem.AssetRegistrationId,
                    SendToUserCard = issueItem.SendToUserCard,

                    Unit = asset.Unit,
                    Name = asset.Name,
                    Size = asset.Size,
                    GrnNo = asset.GRN,

                    RequestedQuantity = issueItem.RequestedQuantity,
                    ApprovedQuantity = issueItem.ApprovedQuantity,
                    IssuedQuantity = issueItem.IssuedQuantity,
                    UnitPrice = asset.BookValue,

                    Total = issueItem.IssuedQuantity * asset.BookValue,
                    Remark = issueItem.Remark,


                    IsMachinery = asset.IsMachinery,
                    TagNo = asset.Tag,
                    //BatchNo = asset.b,
                    Model = asset.ModelNo,
                    SerialNo = asset.SerialNo,
                    Condition = asset.Condition.Name,
                    PlateNo = asset.PlateNo,
                    ChassisNo = asset.ChassisNo,
                    EngineNo = asset.EngineNo,
                    //EngineModelNo = asset.EngineModelNo,
                    // EngineElectricalNo = asset.EngineElectricalNo,
                    // EngineElectricalModelNo = asset.EngineElectricalModelNo,
                    OrgBranchName = issueItem.OrgBranchName,
                    DateCreated = issueItem.DateCreated,
                    LastModified = issueItem.LastModified,
                    CreatedBy = issueItem.CreatedBy,
                    ModifiedBy = issueItem.ModifiedBy
                };

                db.FixedAssetIssueItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                
            }
        
            return Json(new[] { issueItem }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetIssueItems_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetIssueItem issueItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetIssueItem
                {
                    FixedAssetIssueItemID = issueItem.FixedAssetIssueItemID,
                    FixedAssetCategoryId = issueItem.FixedAssetCategoryId,
                    FixedAssetSubCategoryId = issueItem.FixedAssetSubCategoryId,
                    AssetRegistrationId = issueItem.AssetRegistrationId,
                    SendToUserCard = issueItem.SendToUserCard,

                    RequestedQuantity = issueItem.RequestedQuantity,
                    ApprovedQuantity = issueItem.ApprovedQuantity,
                    IssuedQuantity = issueItem.IssuedQuantity,
                    UnitPrice = issueItem.UnitPrice,
                   
                    Total = issueItem.Total,
                    
                    IsMachinery = issueItem.IsMachinery,
                    TagNo = issueItem.TagNo,
                    BatchNo = issueItem.BatchNo,
                    Model = issueItem.Model,
                    SerialNo = issueItem.SerialNo,
                    Condition = issueItem.Condition,
                    PlateNo = issueItem.PlateNo,
                    ChassisNo = issueItem.ChassisNo,
                    EngineNo = issueItem.EngineNo,
                    EngineModelNo = issueItem.EngineModelNo,
                    EngineElectricalNo = issueItem.EngineElectricalNo,
                    EngineElectricalModelNo = issueItem.EngineElectricalModelNo,
                    OrgBranchName = issueItem.OrgBranchName,
                    DateCreated = issueItem.DateCreated,
                    LastModified = issueItem.LastModified,
                    CreatedBy = issueItem.CreatedBy,
                    ModifiedBy = issueItem.ModifiedBy
                };

                db.FixedAssetIssueItems.Attach(entity);
                db.FixedAssetIssueItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { issueItem }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
