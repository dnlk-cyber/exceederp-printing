using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Receives;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Receives
{
   
    public class FixedAssetReceiveItemManualController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly FixedAssetDistributionHelper _distributionHelper = new FixedAssetDistributionHelper();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetInspector)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetInspector)]
        public ActionResult FixedAssetReceiveItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            IQueryable<FixedAssetReceiveItem> fixedAssetReceiveItems = db.FixedAssetReceiveItems.Where(gi => gi.FixedAssetReceiveId == id);
            DataSourceResult result = fixedAssetReceiveItems.ToDataSourceResult(request, fixedAssetReceiveItem => new
            {
                FixedAssetReceiveItemID = fixedAssetReceiveItem.FixedAssetReceiveItemID,
                FixedAssetReceiveId = fixedAssetReceiveItem.FixedAssetReceiveId,
                Name = fixedAssetReceiveItem.Name,
                FixedAssetNameID = fixedAssetReceiveItem.FixedAssetNameID,
                TagNo = fixedAssetReceiveItem.TagNo,
                FixedAssetCategoryID = fixedAssetReceiveItem.FixedAssetCategoryID,
                FixedAssetSubCategoryID = fixedAssetReceiveItem.FixedAssetSubCategoryID,
                Quantity = fixedAssetReceiveItem.Quantity,
                IsGroup = fixedAssetReceiveItem.IsGroup,
                Remark = fixedAssetReceiveItem.Remark,
                Unit = fixedAssetReceiveItem.Unit,
                UnitPrice = fixedAssetReceiveItem.UnitPrice,
                SubTotal = fixedAssetReceiveItem.SubTotal,
                Total = fixedAssetReceiveItem.Total,
                GLTaxRateID = fixedAssetReceiveItem.GLTaxRateID,
                TaxAmount = fixedAssetReceiveItem.TaxAmount,
                IsExtra = fixedAssetReceiveItem.IsExtra,
                ExtraQuantity = fixedAssetReceiveItem.ExtraQuantity,
                ExtraAmount = fixedAssetReceiveItem.ExtraAmount,
                WarrantyInformation = fixedAssetReceiveItem.WarrantyInformation,
                Specification = fixedAssetReceiveItem.Specification,
                Expire = fixedAssetReceiveItem.Expire,
                ExpireDate = fixedAssetReceiveItem.ExpireDate,
                Balance = fixedAssetReceiveItem.Balance,
                IsMachinery = fixedAssetReceiveItem.IsMachinery,
                
                Manufacturer = fixedAssetReceiveItem.Manufacturer,
                ManufacturDate = fixedAssetReceiveItem.ManufacturDate,
                Size = fixedAssetReceiveItem.Size,
                BatchNo = fixedAssetReceiveItem.BatchNo,
                Model = fixedAssetReceiveItem.Model,
                SerialNo = fixedAssetReceiveItem.SerialNo,
                FixedAssetConditionId = fixedAssetReceiveItem.FixedAssetConditionId,
                PlateNo = fixedAssetReceiveItem.PlateNo,
                ChassisNo = fixedAssetReceiveItem.ChassisNo,
                EngineNo = fixedAssetReceiveItem.EngineNo,
                EngineModelNo = fixedAssetReceiveItem.EngineModelNo,
                EngineElectricalNo = fixedAssetReceiveItem.EngineElectricalNo,
                EngineElectricalModelNo = fixedAssetReceiveItem.EngineElectricalModelNo,
                //land
                IsBuilding = fixedAssetReceiveItem.IsBuilding,
                Location = fixedAssetReceiveItem.Location,
                Contractor = fixedAssetReceiveItem.Contractor,
                ConstructionStartDate = fixedAssetReceiveItem.ConstructionStartDate,
                ConstructionEndDate = fixedAssetReceiveItem.ConstructionEndDate,
                OrgBranchName = fixedAssetReceiveItem.OrgBranchName,
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceiveItems_Create([DataSourceRequest]DataSourceRequest request, FixedAssetReceiveItem fixedAssetReceiveItem, int id)
        {
            if (fixedAssetReceiveItem.Quantity < 1)
                ModelState.AddModelError("qty", "Quantity should be at least one");
            if (fixedAssetReceiveItem.UnitPrice == 0)
                ModelState.AddModelError("zero", "Unit Price can't be zero.");
            if (fixedAssetReceiveItem.GLTaxRateID == 0)
                ModelState.AddModelError("zero", "Please select tax.");
            if (fixedAssetReceiveItem.FixedAssetNameID == 0)
                ModelState.AddModelError("zero", "Please select Name.");

            var tagExisit = db.FixedAssetReceiveItems.Where(x => x.FixedAssetReceiveItemID != fixedAssetReceiveItem.FixedAssetReceiveItemID).Where(x => x.TagNo == fixedAssetReceiveItem.TagNo).FirstOrDefault();
            if (tagExisit != null)
                ModelState.AddModelError("exisit", "The tag number already taken");

            var tax = db.GLTaxRates.Find(fixedAssetReceiveItem.GLTaxRateID);

            
            if (ModelState.IsValid)
            {
                if (tax != null)
                {
                    var subCat = db.FixedAssetSubCategories.Find(fixedAssetReceiveItem.FixedAssetSubCategoryID);
                    if (subCat.IsGroup)
                    {
                        #region by group
                        var entity = new FixedAssetReceiveItem
                        {
                            FixedAssetReceiveId = id,
                            FixedAssetNameID = fixedAssetReceiveItem.FixedAssetNameID,
                            Name = db.FixedAssetSubCategoryNames.Find(fixedAssetReceiveItem.FixedAssetNameID)?.Name,
                            TagNo = fixedAssetReceiveItem.TagNo,
                            FixedAssetCategoryID = fixedAssetReceiveItem.FixedAssetCategoryID,
                            FixedAssetSubCategoryID = fixedAssetReceiveItem.FixedAssetSubCategoryID,
                            Quantity = fixedAssetReceiveItem.Quantity,
                            IsGroup = fixedAssetReceiveItem.IsGroup,
                            Remark = fixedAssetReceiveItem.Remark,
                            Unit = fixedAssetReceiveItem.Unit,
                            UnitPrice = fixedAssetReceiveItem.UnitPrice,
                            SubTotal = fixedAssetReceiveItem.UnitPrice * fixedAssetReceiveItem.Quantity,
                            GLTaxRateID = fixedAssetReceiveItem.GLTaxRateID,
                            TaxAmount = fixedAssetReceiveItem.UnitPrice * tax.CalculatedRate * fixedAssetReceiveItem.Quantity,
                            Total = fixedAssetReceiveItem.UnitPrice * fixedAssetReceiveItem.Quantity + (fixedAssetReceiveItem.UnitPrice * fixedAssetReceiveItem.Quantity * tax.CalculatedRate),
                            IsExtra = fixedAssetReceiveItem.IsExtra,
                            ExtraQuantity = fixedAssetReceiveItem.ExtraQuantity,
                            ExtraAmount = fixedAssetReceiveItem.ExtraAmount,
                            WarrantyInformation = fixedAssetReceiveItem.WarrantyInformation,
                            Specification = fixedAssetReceiveItem.Specification,
                            Expire = fixedAssetReceiveItem.Expire,
                            ExpireDate = fixedAssetReceiveItem.ExpireDate,
                            Balance = fixedAssetReceiveItem.Quantity,
                            IsMachinery = fixedAssetReceiveItem.IsMachinery,

                            Manufacturer = fixedAssetReceiveItem.Manufacturer,
                            ManufacturDate = fixedAssetReceiveItem.ManufacturDate,
                            Size = fixedAssetReceiveItem.Size,
                            BatchNo = fixedAssetReceiveItem.BatchNo,
                            Model = fixedAssetReceiveItem.Model,
                            SerialNo = fixedAssetReceiveItem.SerialNo,
                            FixedAssetConditionId = fixedAssetReceiveItem.FixedAssetConditionId,
                            PlateNo = fixedAssetReceiveItem.PlateNo,
                            ChassisNo = fixedAssetReceiveItem.ChassisNo,
                            EngineNo = fixedAssetReceiveItem.EngineNo,
                            EngineModelNo = fixedAssetReceiveItem.EngineModelNo,
                            EngineElectricalNo = fixedAssetReceiveItem.EngineElectricalNo,
                            EngineElectricalModelNo = fixedAssetReceiveItem.EngineElectricalModelNo,
                            //land
                            IsBuilding = fixedAssetReceiveItem.IsBuilding,
                            Location = fixedAssetReceiveItem.Location,
                            Contractor = fixedAssetReceiveItem.Contractor,
                            ConstructionStartDate = fixedAssetReceiveItem.ConstructionStartDate,
                            ConstructionEndDate = fixedAssetReceiveItem.ConstructionEndDate,
                            OrgBranchName = fixedAssetReceiveItem.OrgBranchName,
                            ModifiedBy = User.Identity.Name,
                            LastModified = DateTime.Today,
                        };
                        db.FixedAssetReceiveItems.Add(entity);
                        db.SaveChanges();
                        fixedAssetReceiveItem.FixedAssetReceiveItemID = entity.FixedAssetReceiveItemID;
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReceiveItem", "Success", User.Identity.Name, $"created FixedAssetReceiveItem {entity.FixedAssetReceiveItemID} Tag {entity.TagNo}", Modules.FixedAsset);
                        #endregion

                        #endregion
                    }
                    else
                    {

                        #region individually
                        for (int i = 0; i < fixedAssetReceiveItem.Quantity; i++)
                        {
                            var entity = new FixedAssetReceiveItem
                            {
                                FixedAssetReceiveId = id,
                                FixedAssetNameID = fixedAssetReceiveItem.FixedAssetNameID,
                                Name = db.FixedAssetSubCategoryNames.Find(fixedAssetReceiveItem.FixedAssetNameID)?.Name,
                                TagNo = fixedAssetReceiveItem.TagNo,
                                FixedAssetCategoryID = fixedAssetReceiveItem.FixedAssetCategoryID,
                                FixedAssetSubCategoryID = fixedAssetReceiveItem.FixedAssetSubCategoryID,
                                Quantity = 1,
                                IsGroup = fixedAssetReceiveItem.IsGroup,
                                Remark = fixedAssetReceiveItem.Remark,
                                Unit = fixedAssetReceiveItem.Unit,
                                UnitPrice = fixedAssetReceiveItem.UnitPrice,
                                SubTotal = fixedAssetReceiveItem.UnitPrice * 1,
                                GLTaxRateID = fixedAssetReceiveItem.GLTaxRateID,
                                TaxAmount = fixedAssetReceiveItem.UnitPrice * tax.CalculatedRate * 1,
                                Total = fixedAssetReceiveItem.UnitPrice * 1 + (fixedAssetReceiveItem.UnitPrice * 1 * tax.CalculatedRate),
                                IsExtra = fixedAssetReceiveItem.IsExtra,
                                ExtraQuantity = fixedAssetReceiveItem.ExtraQuantity,
                                ExtraAmount = fixedAssetReceiveItem.ExtraAmount,
                                WarrantyInformation = fixedAssetReceiveItem.WarrantyInformation,
                                Specification = fixedAssetReceiveItem.Specification,
                                Expire = fixedAssetReceiveItem.Expire,
                                ExpireDate = fixedAssetReceiveItem.ExpireDate,
                                Balance = 1,
                                IsMachinery = fixedAssetReceiveItem.IsMachinery,

                                Manufacturer = fixedAssetReceiveItem.Manufacturer,
                                ManufacturDate = fixedAssetReceiveItem.ManufacturDate,
                                Size = fixedAssetReceiveItem.Size,
                                BatchNo = fixedAssetReceiveItem.BatchNo,
                                Model = fixedAssetReceiveItem.Model,
                                SerialNo = fixedAssetReceiveItem.SerialNo,
                                FixedAssetConditionId = fixedAssetReceiveItem.FixedAssetConditionId,
                                PlateNo = fixedAssetReceiveItem.PlateNo,
                                ChassisNo = fixedAssetReceiveItem.ChassisNo,
                                EngineNo = fixedAssetReceiveItem.EngineNo,
                                EngineModelNo = fixedAssetReceiveItem.EngineModelNo,
                                EngineElectricalNo = fixedAssetReceiveItem.EngineElectricalNo,
                                EngineElectricalModelNo = fixedAssetReceiveItem.EngineElectricalModelNo,
                                //land
                                IsBuilding = fixedAssetReceiveItem.IsBuilding,
                                Location = fixedAssetReceiveItem.Location,
                                Contractor = fixedAssetReceiveItem.Contractor,
                                ConstructionStartDate = fixedAssetReceiveItem.ConstructionStartDate,
                                ConstructionEndDate = fixedAssetReceiveItem.ConstructionEndDate,
                                OrgBranchName = fixedAssetReceiveItem.OrgBranchName,
                                ModifiedBy = User.Identity.Name,
                                LastModified = DateTime.Today,
                            };
                            db.FixedAssetReceiveItems.Add(entity);
                            db.SaveChanges();
                            fixedAssetReceiveItem.FixedAssetReceiveItemID = entity.FixedAssetReceiveItemID;
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReceiveItem", "Success", User.Identity.Name, $"created FixedAssetReceiveItem {entity.FixedAssetReceiveItemID} Tag {entity.TagNo}", Modules.FixedAsset);
                            #endregion
                        }
                        #endregion

                    }

                    var record = db.FixedAssetReceives.Find(id);
                    if (record != null)
                        _distributionHelper.UpdateGoodReceiveDistribution(record);
                }
            }


            return Json(new[] { fixedAssetReceiveItem }.ToDataSourceResult(request, ModelState));

        }
        
        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceiveItems_Update([DataSourceRequest]DataSourceRequest request, FixedAssetReceiveItem fixedAssetReceiveItem, int id)
        {
            
            if (fixedAssetReceiveItem.Quantity < 1)
                ModelState.AddModelError("qty", "Quantity should be at least one");
            if (fixedAssetReceiveItem.UnitPrice == 0)
                ModelState.AddModelError("zero", "Unit Price can't be zero.");
            if (fixedAssetReceiveItem.GLTaxRateID == 0)
                ModelState.AddModelError("zero", "Please select tax.");
            if (fixedAssetReceiveItem.FixedAssetNameID == 0)
                ModelState.AddModelError("zero", "Please select Name.");

            var subCat = db.FixedAssetSubCategories.Find(fixedAssetReceiveItem.FixedAssetSubCategoryID);
            if (!subCat.IsGroup&& fixedAssetReceiveItem.Quantity > 1)
                ModelState.AddModelError("qty", "For non group items quantity should be one");

            var tagExisit = db.FixedAssetReceiveItems.Where(x => x.FixedAssetReceiveItemID != fixedAssetReceiveItem.FixedAssetReceiveItemID).Where(x => x.TagNo == fixedAssetReceiveItem.TagNo).FirstOrDefault();
            if (tagExisit != null)
                ModelState.AddModelError("exisit", "The tag number already taken");


            var tax = db.GLTaxRates.Find(fixedAssetReceiveItem.GLTaxRateID);

           
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReceiveItem
                {
                    FixedAssetReceiveId = fixedAssetReceiveItem.FixedAssetReceiveId,
                    FixedAssetReceiveItemID = fixedAssetReceiveItem.FixedAssetReceiveItemID,
                    FixedAssetNameID = fixedAssetReceiveItem.FixedAssetNameID,
                    Name = db.FixedAssetSubCategoryNames.Find(fixedAssetReceiveItem.FixedAssetNameID)?.Name,
                    TagNo = fixedAssetReceiveItem.TagNo,
                    FixedAssetCategoryID = fixedAssetReceiveItem.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = fixedAssetReceiveItem.FixedAssetSubCategoryID,
                    Quantity = fixedAssetReceiveItem.Quantity,
                    IsGroup = fixedAssetReceiveItem.IsGroup,
                    Remark = fixedAssetReceiveItem.Remark,
                    Unit = fixedAssetReceiveItem.Unit,
                    UnitPrice = fixedAssetReceiveItem.UnitPrice,
                    SubTotal = fixedAssetReceiveItem.UnitPrice * fixedAssetReceiveItem.Quantity,
                    GLTaxRateID = fixedAssetReceiveItem.GLTaxRateID
                };

                if (tax != null)
                    entity.TaxAmount = fixedAssetReceiveItem.UnitPrice * fixedAssetReceiveItem.Quantity * tax.CalculatedRate;
                entity.Total = fixedAssetReceiveItem.UnitPrice * fixedAssetReceiveItem.Quantity + fixedAssetReceiveItem.UnitPrice * fixedAssetReceiveItem.Quantity * tax.CalculatedRate;
                entity.IsExtra = fixedAssetReceiveItem.IsExtra;
                entity.WarrantyInformation = fixedAssetReceiveItem.WarrantyInformation;
                entity.Specification = fixedAssetReceiveItem.Specification;
                entity.Expire = fixedAssetReceiveItem.Expire;
                entity.ModifiedBy = User.Identity.Name;
                entity.LastModified = DateTime.Today;
                entity.IsMachinery = fixedAssetReceiveItem.IsMachinery;
              
                entity.Manufacturer = fixedAssetReceiveItem.Manufacturer;
                entity.ManufacturDate = fixedAssetReceiveItem.ManufacturDate;
                entity.Size = fixedAssetReceiveItem.Size;
                entity.BatchNo = fixedAssetReceiveItem.BatchNo;
                entity.Model = fixedAssetReceiveItem.Model;
                entity.SerialNo = fixedAssetReceiveItem.SerialNo;
                entity.FixedAssetConditionId = fixedAssetReceiveItem.FixedAssetConditionId;
                entity.PlateNo = fixedAssetReceiveItem.PlateNo;
                entity.ChassisNo = fixedAssetReceiveItem.ChassisNo;
                entity.EngineNo = fixedAssetReceiveItem.EngineNo;
                entity.EngineModelNo = fixedAssetReceiveItem.EngineModelNo;
                entity.EngineElectricalNo = fixedAssetReceiveItem.EngineElectricalNo;
                entity.EngineElectricalModelNo = fixedAssetReceiveItem.EngineElectricalModelNo;
                 //land
                entity.IsBuilding = fixedAssetReceiveItem.IsBuilding;
                entity.Location = fixedAssetReceiveItem.Location;
                entity.Contractor = fixedAssetReceiveItem.Contractor;
                entity.ConstructionStartDate = fixedAssetReceiveItem.ConstructionStartDate;
                entity.ConstructionEndDate = fixedAssetReceiveItem.ConstructionEndDate;
                db.FixedAssetReceiveItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //FixedAssetReceive record = db.FixedAssetReceives.FirstOrDefault(gi => gi.FixedAssetReceiveId == id);
                var record = db.FixedAssetReceives.Find(id);
                if (record != null)
                    _distributionHelper.UpdateGoodReceiveDistribution(record);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "FixedAssetReceiveItem", "Success", User.Identity.Name, $"updated FixedAssetReceiveItem {entity.FixedAssetReceiveItemID} Tag {entity.TagNo}", Modules.FixedAsset);
                #endregion

            }

            return Json(new[] { fixedAssetReceiveItem }.ToDataSourceResult(request, ModelState));
        }
        
        [AuthorizeRoles(FixedAssetConstants.FixedAssetStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FixedAssetReceiveItems_Destroy([DataSourceRequest]DataSourceRequest request, FixedAssetReceiveItem fixedAssetReceiveItem)
        {
            ModelState.Remove("FixedAssetReceiveId");
            if (ModelState.IsValid)
            {
                var entity = new FixedAssetReceiveItem
                {
                    FixedAssetReceiveItemID = fixedAssetReceiveItem.FixedAssetReceiveItemID,
                    Name = fixedAssetReceiveItem.Name,
                    FixedAssetNameID = fixedAssetReceiveItem.FixedAssetNameID,
                    FixedAssetCategoryID = fixedAssetReceiveItem.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = fixedAssetReceiveItem.FixedAssetSubCategoryID,
                    Quantity = fixedAssetReceiveItem.Quantity,
                    UnitPrice = fixedAssetReceiveItem.UnitPrice,
                    SubTotal = fixedAssetReceiveItem.SubTotal,
                    GLTaxRateID = fixedAssetReceiveItem.GLTaxRateID,
                    TaxAmount = fixedAssetReceiveItem.TaxAmount,
                    Total = fixedAssetReceiveItem.Total,
                    IsExtra = fixedAssetReceiveItem.IsExtra,
                    ExtraQuantity = fixedAssetReceiveItem.ExtraQuantity,
                    ExtraAmount = fixedAssetReceiveItem.ExtraAmount,
                    WarrantyInformation = fixedAssetReceiveItem.WarrantyInformation,
                    Specification = fixedAssetReceiveItem.Specification,
                    Expire = fixedAssetReceiveItem.Expire,
                    ExpireDate = fixedAssetReceiveItem.ExpireDate,
                    Balance = fixedAssetReceiveItem.Balance,
                    IsMachinery = fixedAssetReceiveItem.IsMachinery,
                    
                    Manufacturer = fixedAssetReceiveItem.Manufacturer,
                    ManufacturDate = fixedAssetReceiveItem.ManufacturDate,
                    Size = fixedAssetReceiveItem.Size,
                    BatchNo = fixedAssetReceiveItem.BatchNo,
                    Model = fixedAssetReceiveItem.Model,
                    SerialNo = fixedAssetReceiveItem.SerialNo,
                    FixedAssetConditionId = fixedAssetReceiveItem.FixedAssetConditionId,
                    PlateNo = fixedAssetReceiveItem.PlateNo,
                    ChassisNo = fixedAssetReceiveItem.ChassisNo,
                    EngineNo = fixedAssetReceiveItem.EngineNo,
                    EngineModelNo = fixedAssetReceiveItem.EngineModelNo,
                    EngineElectricalNo = fixedAssetReceiveItem.EngineElectricalNo,
                    EngineElectricalModelNo = fixedAssetReceiveItem.EngineElectricalModelNo,
                    //land
                    IsBuilding = fixedAssetReceiveItem.IsBuilding,
                    Location = fixedAssetReceiveItem.Location,
                    Contractor = fixedAssetReceiveItem.Contractor,
                    ConstructionStartDate = fixedAssetReceiveItem.ConstructionStartDate,
                    ConstructionEndDate = fixedAssetReceiveItem.ConstructionEndDate,
                    OrgBranchName = fixedAssetReceiveItem.OrgBranchName,
                };

                db.FixedAssetReceiveItems.Attach(entity);
                db.FixedAssetReceiveItems.Remove(entity);
                db.SaveChanges();

                var record = db.FixedAssetReceives.Find(fixedAssetReceiveItem.FixedAssetReceiveId);
                if (record != null)
                    _distributionHelper.UpdateGoodReceiveDistribution(record);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted FixedAssetReceiveItem {fixedAssetReceiveItem.FixedAssetReceiveItemID} Tag {fixedAssetReceiveItem.TagNo}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { fixedAssetReceiveItem }.ToDataSourceResult(request, ModelState));
        }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
