using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;


namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Assetregistration
{

    public class InactiveAssetRegistrationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            PopulateData();
            return View();
        }

        public void PopulateData()
        {
           
            var branches = _db.Branch;
            ViewData["Branches"] = branches;
            var fixedAssetCategories = _db.FixedAssetCategories;
            ViewData["fixedAssetCategories"] = fixedAssetCategories.ToList();

            var fixedAssetSubCategories = _db.FixedAssetSubCategories;
            ViewData["fixedAssetSubCategories"] = fixedAssetSubCategories.ToList();
            var fixedAssetSubCategoryNames = _db.FixedAssetSubCategoryNames;
            ViewData["fixedAssetSubCategoryNames"] = fixedAssetSubCategoryNames.Select(s=>new {
                FixedAssetSubCategoryNameId = s.FixedAssetSubCategoryNameId,
                Name = s.Name
            }).ToList();

            var fiscalYears = _db.GLFiscalYears;
            ViewData["fiscalYears"] = fiscalYears.ToList();

            var tax = _db.GLTaxRates;
            ViewData["tax"] = tax;

           
            ViewData["conditions"] = _db.FixedAssetConditions;

            var stores = _db.InventoryStores;
            ViewData["Stores"] = stores;

            var employee = _db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.EmployeeCode + "-" +i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var costCenter = _db.BranchCostCenter;

            var selectedcostCenter = costCenter.Select(
                i => new
                {
                    Name = i.MainCompanyStructure.Name,
                    Code = i.OrgStructureId
                }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Registrations_Read([DataSourceRequest]DataSourceRequest request,string subcategory, string searchkey)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            IQueryable<AssetRegistration> registrations = _db.AssetRegistrations
                                                            .Where(x => x.Disable)
                                                            .Where(x => mybraches.Contains(x.BranchId));

            if (!String.IsNullOrEmpty(subcategory))
            {
                int id = int.Parse(subcategory);
                registrations = registrations.Where(x => x.FixedAssetSubCategoryID == id);
            }
            if (!String.IsNullOrEmpty(searchkey))
            {
                registrations = registrations.Where(x => x.Tag == searchkey || x.Name == searchkey || x.SerialNo == searchkey);
            
            }
            DataSourceResult result = registrations.ToDataSourceResult(request, registration => new {
                AssetRegistrationId = registration.AssetRegistrationId,
                StoreCode = registration.StoreCode,
                BranchId = registration.BranchId,
                OrgStructureId = registration.OrgStructureId,
                EmployeeId = registration.EmployeeId,
                Name = registration.Name,
                FixedAssetConditionId = registration.FixedAssetConditionId,
                FixedAssetCategoryID = registration.FixedAssetCategoryID,
                FixedAssetSubCategoryID = registration.FixedAssetSubCategoryID,
                FixedAssetNameID = registration.FixedAssetNameID,
                Unit = registration.Unit,
                Size = registration.Size,
                Tag = registration.Tag,
                SIV = registration.SIV,
                SIVDate = registration.SIVDate,
                PurchaseDate = registration.PurchaseDate,
                PurchasePrice = registration.PurchasePrice,
                GRN = registration.GRN,
                GRNDate = registration.GRNDate,
                OriginalValue = registration.OriginalValue,
                BookValue = registration.BookValue,
                IsGroup = registration.IsGroup,
                GroupCode = registration.GroupCode,
                Quanity = registration.Quanity,
                Status = registration.Status,
                IsMachinery = registration.IsMachinery,
                Supplier = registration.Supplier,
                Manufacturer = registration.Manufacturer,
                SerialNo = registration.SerialNo,
                PlateNo = registration.PlateNo,
                PlateType = registration.PlateType,
                ChassisNo = registration.ChassisNo,
                MotorNo = registration.MotorNo,
                EngineNo = registration.EngineNo,
                ModelNo = registration.ModelNo,
                LoadCapacity = registration.LoadCapacity,
                FuelType = registration.FuelType,
                ManufacturDate = registration.ManufacturDate,
                NetWeight = registration.NetWeight,
                TotalWeight = registration.TotalWeight,
                Battery = registration.Battery,
                Tyre = registration.Tyre,
                IsBuilding = registration.IsBuilding,
                Location = registration.Location,
                Contractor = registration.Contractor,
                ConstructionStartDate = registration.ConstructionStartDate,
                ConstructionEndDate = registration.ConstructionEndDate,

                Condition = registration.Condition,
                Remark = registration.Remark,

                OrgBranchName = registration.OrgBranchName,
                DateCreated = registration.DateCreated,
                LastModified = registration.LastModified,
                CreatedBy = registration.CreatedBy,
                ModifiedBy = registration.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Depreciations_Read(int? id, [DataSourceRequest]DataSourceRequest request)
        {
            List<DepreciationViewModel> list = new List<DepreciationViewModel>();
            //can also be from monthly sum
            var depreciationList = _db.AnnualDepreciationItems.Where(x => x.AssetRegistrationId == id);
            foreach (var depreciationItem in depreciationList)
            {
                var view = new DepreciationViewModel
                {
                    DepreciationAmount = depreciationItem.DepreciationAmount,
                    GlFiscalYearId = depreciationItem.AnnualDepreciation.GlFiscalYearId
                };
                list.Add(view);
            }
           
            DataSourceResult result = list.ToDataSourceResult(request, depreciation => new {
                DepreciationAmount = depreciation.DepreciationAmount,
                BookValue = depreciation.BookValue,
                GlFiscalYearId = depreciation.GlFiscalYearId
            });

            return Json(result);
        }


        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Registrations_Create([DataSourceRequest]DataSourceRequest request, AssetRegistration assetRegistration)
        {
            ModelState.Clear();// problem with created date validation
            #region validation
            if (assetRegistration.FixedAssetCategoryID == 0)
                ModelState.AddModelError("cat", "Category is required");

            if (assetRegistration.FixedAssetSubCategoryID == 0)
                ModelState.AddModelError("sub", "Sub Category is required");

            if (String.IsNullOrEmpty(assetRegistration.Tag)&&!assetRegistration.IsGroup)
                ModelState.AddModelError("tag", "Tag Number is required");

            if (String.IsNullOrEmpty(assetRegistration.GroupCode) && assetRegistration.IsGroup)
                ModelState.AddModelError("group", "Group Code is required");
            //added
            if(string.IsNullOrEmpty(assetRegistration.Unit))
                ModelState.AddModelError("unit", "Unit is required");

            //if (String.IsNullOrEmpty(assetRegistration.Name))
            //    ModelState.AddModelError("name", "Name is required");

            if (!String.IsNullOrEmpty(assetRegistration.Tag))
            {
                var exist = _db.AssetRegistrations.Where(x => x.Tag == assetRegistration.Tag).AsQueryable();
                if (exist.Any())
                {
                    ModelState.AddModelError("exist", "The Tag Number already exists in the system");
                }
            }

            if (!String.IsNullOrEmpty(assetRegistration.SerialNo))
            {
                var exist = _db.AssetRegistrations.Where(x => x.SerialNo == assetRegistration.SerialNo).AsQueryable();
                if (exist.Any())
                {
                    ModelState.AddModelError("exist", "The Serial Number already exists in the system");
                }
            }
            if (!String.IsNullOrEmpty(assetRegistration.PlateNo))
            {
                var exist = _db.AssetRegistrations.Where(x => x.PlateNo == assetRegistration.PlateNo).AsQueryable();
                if (exist.Any())
                {
                    ModelState.AddModelError("exist", "The Plate Number already exists in the system");
                }
            }
            if (!String.IsNullOrEmpty(assetRegistration.ChassisNo))
            {
                var exist = _db.AssetRegistrations.Where(x => x.ChassisNo == assetRegistration.ChassisNo).AsQueryable();
                if (exist.Any())
                {
                    ModelState.AddModelError("exist", "The Chassis Number already exists in the system");
                }
            }
            if (!String.IsNullOrEmpty(assetRegistration.EngineNo))
            {
                var exist = _db.AssetRegistrations.Where(x => x.EngineNo == assetRegistration.EngineNo).AsQueryable();
                if (exist.Any())
                {
                    ModelState.AddModelError("exist", "The Chassis Number already exists in the system");
                }
            }

            if (assetRegistration.IsMachinery)
            {
                //if (String.IsNullOrEmpty(assetRegistration.SerialNo))
                //{
                //    ModelState.AddModelError("fill", "Please fill Serial Number.");
                //}
                //if (String.IsNullOrEmpty(assetRegistration.PlateNo))
                //{
                //    ModelState.AddModelError("fill", "Please fill Plate Number.");
                //}
                if (String.IsNullOrEmpty(assetRegistration.ChassisNo))
                {
                    ModelState.AddModelError("fill", "Please fill Chassis Number.");
                }
                if (String.IsNullOrEmpty(assetRegistration.EngineNo))
                {
                    ModelState.AddModelError("fill", "Please fill Engine Number.");
                }
            }
            if (assetRegistration.IsGroup)
            {
                if (assetRegistration.Quanity == 0)
                    ModelState.AddModelError("qty", "Please fill quantity.");
            }
            else
            {
                if (assetRegistration.Quanity > 1)
                    ModelState.AddModelError("qty", "Quanity for non group assets should be 1.");
                assetRegistration.Quanity = 1;
            }

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new AssetRegistration
                {
                    StoreCode = assetRegistration.StoreCode,
                    BranchId = assetRegistration.BranchId,
                    OrgStructureId = assetRegistration.OrgStructureId,
                    EmployeeId = assetRegistration.EmployeeId,
                    FixedAssetConditionId = assetRegistration.FixedAssetConditionId,
                    FixedAssetCategoryID = assetRegistration.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = assetRegistration.FixedAssetSubCategoryID,
                    FixedAssetNameID = assetRegistration.FixedAssetNameID,
                    Name = _db.FixedAssetSubCategoryNames.Find(assetRegistration.FixedAssetNameID)?.Name,
                    Unit = assetRegistration.Unit,
                    Size = assetRegistration.Size,
                    
                    Tag = assetRegistration.Tag,
                    SIV = assetRegistration.SIV,
                    SIVDate = assetRegistration.SIVDate,
                    PurchaseDate = assetRegistration.PurchaseDate,
                    PurchasePrice = assetRegistration.PurchasePrice,
                    GRN = assetRegistration.GRN,
                    GRNDate = assetRegistration.GRNDate,
                    OriginalValue = assetRegistration.OriginalValue,
                    BookValue = assetRegistration.BookValue,
                    IsGroup = assetRegistration.IsGroup,
                    GroupCode = assetRegistration.GroupCode,
                    Quanity = assetRegistration.Quanity,
                    Status = assetRegistration.Status,
                    IsMachinery = assetRegistration.IsMachinery,
                    Supplier = assetRegistration.Supplier,
                    Manufacturer = assetRegistration.Manufacturer,
                    SerialNo = assetRegistration.SerialNo,
                    PlateNo = assetRegistration.PlateNo,
                    PlateType = assetRegistration.PlateType,
                    ChassisNo = assetRegistration.ChassisNo,
                    MotorNo = assetRegistration.MotorNo,
                    EngineNo = assetRegistration.EngineNo,
                    ModelNo = assetRegistration.ModelNo,
                    LoadCapacity = assetRegistration.LoadCapacity,
                    FuelType = assetRegistration.FuelType,
                    ManufacturDate = assetRegistration.ManufacturDate,
                    NetWeight = assetRegistration.NetWeight,
                    TotalWeight = assetRegistration.TotalWeight,
                    Battery = assetRegistration.Battery,
                    Tyre = assetRegistration.Tyre,
                    Color = assetRegistration.Color,
                    IsBuilding = assetRegistration.IsBuilding,
                    Location = assetRegistration.Location,
                    Contractor = assetRegistration.Contractor,
                    ConstructionStartDate = assetRegistration.ConstructionStartDate,
                    ConstructionEndDate = assetRegistration.ConstructionEndDate,

                    Remark = assetRegistration.Remark,

                    OrgBranchName = assetRegistration.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = assetRegistration.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = assetRegistration.ModifiedBy
                };
               
                _db.AssetRegistrations.Add(entity);
                _db.SaveChanges();
                assetRegistration.AssetRegistrationId = entity.AssetRegistrationId;

                #region Update UserCard if Registered with Employee

                if (assetRegistration.EmployeeId != null)
                {
                    FixedAssetHelper.UpdateUserCardFromAssetRegistration(assetRegistration.AssetRegistrationId, User.Identity.Name);
                }

                #endregion

                #region Update LocationTracking

                FixedAssetHelper.UpdateAssetLocationTrackingForAssetRegistration(assetRegistration.AssetRegistrationId, User.Identity.Name);

                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Asset registration", "Success", User.Identity.Name, $"created asset reg {entity.AssetRegistrationId} Tag {entity.Tag}", Modules.FixedAsset);
                #endregion
            }
            return Json(new[] { assetRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Registrations_Update([DataSourceRequest] DataSourceRequest request, AssetRegistration assetRegistration)
        {

            ModelState.Clear();

            #region validation

            if (assetRegistration.FixedAssetCategoryID == 0)
                ModelState.AddModelError("cat", "Category is required");

            if (assetRegistration.FixedAssetSubCategoryID == 0)
                ModelState.AddModelError("sub", "Sub Category is required");

            if (String.IsNullOrEmpty(assetRegistration.Tag) && !assetRegistration.IsGroup)
                ModelState.AddModelError("tag", "Tag Number is required");

            if (String.IsNullOrEmpty(assetRegistration.GroupCode) && assetRegistration.IsGroup)
                ModelState.AddModelError("group", "Group Code is required");
            //added
            if (string.IsNullOrEmpty(assetRegistration.Unit))
                ModelState.AddModelError("unit", "Unit is required");
            //if (String.IsNullOrEmpty(assetRegistration.Name))
            //    ModelState.AddModelError("name", "Name is required");

            if (!String.IsNullOrEmpty(assetRegistration.Tag))
            {
                var exist = _db.AssetRegistrations
                    .Where(x => x.AssetRegistrationId != assetRegistration.AssetRegistrationId)
                    .Where(x => x.Tag == assetRegistration.Tag).AsQueryable();
            
                if (exist.Any())
                {
                    ModelState.AddModelError("exist", "The Tag Number already exists in the system");
                }
            }

            if (!String.IsNullOrEmpty(assetRegistration.SerialNo))
            {
                var exist = _db.AssetRegistrations.Where(x => x.AssetRegistrationId != assetRegistration.AssetRegistrationId).Where(x => x.SerialNo == assetRegistration.SerialNo).AsQueryable();
                if (exist.Any())
                {
                    ModelState.AddModelError("exist", "The Serial Number already exists in the system");
                }
            }
            if (!String.IsNullOrEmpty(assetRegistration.PlateNo) && assetRegistration.PlateNo != "0")
            {
                var exist = _db.AssetRegistrations.Where(x => x.AssetRegistrationId != assetRegistration.AssetRegistrationId).Where(x => x.PlateNo == assetRegistration.PlateNo).AsQueryable();
                if (exist.Any())
                {
                    ModelState.AddModelError("exist", "The Plate Number already exists in the system");
                }
            }
            if (!String.IsNullOrEmpty(assetRegistration.ChassisNo)&&assetRegistration.ChassisNo!="0")
            {
               var exist = _db.AssetRegistrations.Where(x => x.AssetRegistrationId != assetRegistration.AssetRegistrationId).Where(x => x.ChassisNo == assetRegistration.ChassisNo).AsQueryable();
                if (exist.Any())
                {
                    ModelState.AddModelError("exist", "The Chassis Number already exists in the system");
                }
            }
            if (!String.IsNullOrEmpty(assetRegistration.EngineNo) && assetRegistration.EngineNo != "0")
            {
                var exist = _db.AssetRegistrations.Where(x => x.AssetRegistrationId != assetRegistration.AssetRegistrationId).Where(x => x.EngineNo == assetRegistration.EngineNo).AsQueryable();
                if (exist.Any())
                {
                    ModelState.AddModelError("exist", "The Engine Number already exists in the system");
                }
            }
            if (assetRegistration.IsMachinery)
            {
                //if (String.IsNullOrEmpty(assetRegistration.SerialNo))
                //{
                //    ModelState.AddModelError("fill", "Please fill Serial Number.");
                //}
                if (String.IsNullOrEmpty(assetRegistration.PlateNo))
                {
                    ModelState.AddModelError("fill", "Please fill Plate Number.");
                }
                if (String.IsNullOrEmpty(assetRegistration.ChassisNo))
                {
                    ModelState.AddModelError("fill", "Please fill Chassis Number.");
                }
                if (String.IsNullOrEmpty(assetRegistration.EngineNo))
                {
                    ModelState.AddModelError("fill", "Please fill Engine Number.");
                }
            }
            if (assetRegistration.IsGroup)
            {
                if(assetRegistration.Quanity==0)
                    ModelState.AddModelError("qty", "Please fill quantity.");
            }
            else
            {
                if (assetRegistration.Quanity > 1)
                    ModelState.AddModelError("qty", "Quanity for non group assets should be 1.");
                assetRegistration.Quanity = 1;
            }
            #endregion           

            if (ModelState.IsValid)
            {
                var entity = new AssetRegistration
                {
                    StoreCode = assetRegistration.StoreCode,
                    BranchId = assetRegistration.BranchId,
                    OrgStructureId = assetRegistration.OrgStructureId,
                    EmployeeId = assetRegistration.EmployeeId,
                    FixedAssetConditionId = assetRegistration.FixedAssetConditionId,
                    AssetRegistrationId = assetRegistration.AssetRegistrationId,
                    FixedAssetCategoryID = assetRegistration.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = assetRegistration.FixedAssetSubCategoryID,
                    FixedAssetNameID = assetRegistration.FixedAssetNameID,
                    Unit = assetRegistration.Unit,
                    Size = assetRegistration.Size,                    
                    Name = _db.FixedAssetSubCategoryNames.Find(assetRegistration.FixedAssetNameID)?.Name,
                    Tag = assetRegistration.Tag,
                    SIV = assetRegistration.SIV,
                    SIVDate = assetRegistration.SIVDate,
                    PurchaseDate = assetRegistration.PurchaseDate,
                    PurchasePrice = assetRegistration.PurchasePrice,
                    GRN = assetRegistration.GRN,
                    GRNDate = assetRegistration.GRNDate,
                    OriginalValue = assetRegistration.OriginalValue,
                    BookValue = assetRegistration.BookValue,
                    Status = assetRegistration.Status,
                    IsGroup = assetRegistration.IsGroup,
                    GroupCode = assetRegistration.GroupCode,
                    Quanity = assetRegistration.Quanity,
                    IsMachinery = assetRegistration.IsMachinery,
                    Supplier = assetRegistration.Supplier,
                    Manufacturer = assetRegistration.Manufacturer,
                    SerialNo = assetRegistration.SerialNo,
                    PlateNo = assetRegistration.PlateNo,
                    PlateType = assetRegistration.PlateType,
                    ChassisNo = assetRegistration.ChassisNo,
                    MotorNo = assetRegistration.MotorNo,
                    ModelNo = assetRegistration.ModelNo,
                    EngineNo = assetRegistration.EngineNo,
                    LoadCapacity = assetRegistration.LoadCapacity,
                    FuelType = assetRegistration.FuelType,
                    ManufacturDate = assetRegistration.ManufacturDate,
                    NetWeight = assetRegistration.NetWeight,
                    TotalWeight = assetRegistration.TotalWeight,
                    Battery = assetRegistration.Battery,
                    Tyre = assetRegistration.Tyre,
                    Color = assetRegistration.Color,
                    IsBuilding = assetRegistration.IsBuilding,
                    Location = assetRegistration.Location,
                    Contractor = assetRegistration.Contractor,
                    ConstructionStartDate = assetRegistration.ConstructionStartDate,
                    ConstructionEndDate = assetRegistration.ConstructionEndDate,

                    Remark = assetRegistration.Remark,

                    OrgBranchName = assetRegistration.OrgBranchName,
                    DateCreated = assetRegistration.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = assetRegistration.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                //modify in db
                _db.AssetRegistrations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                UpdateFleet(assetRegistration.Tag,assetRegistration.PlateNo,assetRegistration.ChassisNo,assetRegistration.SerialNo);

                #region Update UserCard if Registered with Employee

                if (assetRegistration.EmployeeId != null)
                {
                    FixedAssetHelper.UpdateUserCardFromAssetRegistration(assetRegistration.AssetRegistrationId, User.Identity.Name);
                }

                #endregion

                #region Update LocationTracking

                FixedAssetHelper.UpdateAssetLocationTrackingForAssetRegistration(assetRegistration.AssetRegistrationId, User.Identity.Name);

                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Asset registration", "Success", User.Identity.Name, $"updated asset reg {entity.AssetRegistrationId} Tag {assetRegistration.Tag}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { assetRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Registrations_Destroy([DataSourceRequest]DataSourceRequest request, AssetRegistration assetRegistration)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssetRegistration
                {
                    StoreCode = assetRegistration.StoreCode,
                    BranchId = assetRegistration.BranchId,
                    FixedAssetConditionId = assetRegistration.FixedAssetConditionId,
                    AssetRegistrationId = assetRegistration.AssetRegistrationId,
                    FixedAssetCategoryID = assetRegistration.FixedAssetCategoryID,
                    FixedAssetSubCategoryID = assetRegistration.FixedAssetSubCategoryID,
                    FixedAssetNameID = assetRegistration.FixedAssetNameID,
                    Unit = assetRegistration.Unit,
                    Size = assetRegistration.Size,
                    
                    Name = assetRegistration.Name,
                    Tag = assetRegistration.Tag,
                    SIV = assetRegistration.SIV,
                    SIVDate = assetRegistration.SIVDate,
                    PurchaseDate = assetRegistration.PurchaseDate,
                    PurchasePrice = assetRegistration.PurchasePrice,
                    GRN = assetRegistration.GRN,
                    GRNDate = assetRegistration.GRNDate,
                    OriginalValue = assetRegistration.OriginalValue,
                    BookValue = assetRegistration.BookValue,
                    Status = assetRegistration.Status,
                    IsGroup = assetRegistration.IsGroup,
                    Quanity = assetRegistration.Quanity,
                    IsMachinery = assetRegistration.IsMachinery,
                    Supplier = assetRegistration.Supplier,
                    Manufacturer = assetRegistration.Manufacturer,
                    SerialNo = assetRegistration.SerialNo,
                    PlateNo = assetRegistration.PlateNo,
                    PlateType = assetRegistration.PlateType,
                    ChassisNo = assetRegistration.ChassisNo,
                    MotorNo = assetRegistration.MotorNo,
                    ModelNo = assetRegistration.ModelNo,
                    LoadCapacity = assetRegistration.LoadCapacity,
                    FuelType = assetRegistration.FuelType,
                    ManufacturDate = assetRegistration.ManufacturDate,
                    NetWeight = assetRegistration.NetWeight,
                    TotalWeight = assetRegistration.TotalWeight,
                    Battery = assetRegistration.Battery,
                    Tyre = assetRegistration.Tyre,
                    IsBuilding = assetRegistration.IsBuilding,
                    Location = assetRegistration.Location,
                    Contractor = assetRegistration.Contractor,
                    ConstructionStartDate = assetRegistration.ConstructionStartDate,
                    ConstructionEndDate = assetRegistration.ConstructionEndDate,
                    OrgBranchName = assetRegistration.OrgBranchName,
                    DateCreated = assetRegistration.DateCreated,
                    LastModified = assetRegistration.LastModified,
                    CreatedBy = assetRegistration.CreatedBy,
                    ModifiedBy = assetRegistration.ModifiedBy
                };

                _db.AssetRegistrations.Attach(entity);
                _db.AssetRegistrations.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted asset registration number {assetRegistration.AssetRegistrationId } and Tag {assetRegistration.Tag}");
                #endregion
            }

            return Json(new[] { assetRegistration }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        //[AuthorizeRoles(FixedAssetConstants.FixedAssetAdmin, FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetDepreciationApprover, FixedAssetConstants.FixedAssetDepreciationUser)]

        //public ActionResult Depreciations_Read(int assetRegistrationId, [DataSourceRequest]DataSourceRequest request)
        //{
        //    IQueryable<AnnualDepreciation> depreciations = _db.AnnualDepreciations.Where(x => x.AssetRegistrationId == assetRegistrationId);
        //    DataSourceResult result = depreciations.ToDataSourceResult(request, depreciation => new
        //    {
        //        AnnualDepreciationID = depreciation.AnnualDepreciationID,
        //        DepreciationAmount = depreciation.DepreciationAmount,
        //        BookValue = depreciation.BookValue,
        //        OrgBranchName = depreciation.OrgBranchName,
        //        DateCreated = depreciation.DateCreated,
        //        LastModified = depreciation.LastModified,
        //        CreatedBy = depreciation.CreatedBy,
        //        ModifiedBy = depreciation.ModifiedBy
        //    });

        //    return Json(result);
        //}

        [AuthorizeRoles(FixedAssetConstants.FixedAssetAdmin, FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetDepreciationApprover, FixedAssetConstants.FixedAssetDepreciationUser)]

        public ActionResult Revaluations_Read(int assetRegistrationId, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FixedAssetRevaluation> revaluations = _db.FixedAssetRevaluations.Where(x => x.AssetRegistrationId == assetRegistrationId);
            DataSourceResult result = revaluations.ToDataSourceResult(request, revaluation => new
            {
                FixedAssetRevaluationID = revaluation.FixedAssetRevaluationID,
                AddedValue = revaluation.AddedValue,
                Description = revaluation.Description,

                OrgBranchName = revaluation.OrgBranchName,
                DateCreated = revaluation.DateCreated,
                LastModified = revaluation.LastModified,
                CreatedBy = revaluation.CreatedBy,
                ModifiedBy = revaluation.ModifiedBy
            });

            return Json(result);
        }
        [AuthorizeRoles(FixedAssetConstants.FixedAssetAdmin, FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetDepreciationApprover, FixedAssetConstants.FixedAssetDepreciationUser)]

        public ActionResult Disposals_Read(int assetRegistrationId, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DisposalItem> disposals = _db.DisposalItems.Where(x => x.AssetRegistrationId == assetRegistrationId);
            DataSourceResult result = disposals.ToDataSourceResult(request, disposal => new
            {
                DisposalItemID = disposal.DisposalItemId,
                Description = disposal.AssetDisposal.Description,

                OrgBranchName = disposal.OrgBranchName,
                DateCreated = disposal.DateCreated,
                LastModified = disposal.LastModified,
                CreatedBy = disposal.CreatedBy,
                ModifiedBy = disposal.ModifiedBy
            });

            return Json(result);
        }
        [AuthorizeRoles(FixedAssetConstants.FixedAssetAdmin, FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover, FixedAssetConstants.FixedAssetDepreciationApprover, FixedAssetConstants.FixedAssetDepreciationUser)]

        public ActionResult Sales_Read(int assetRegistrationId, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Sales> sales = _db.Sales.Where(x => x.AssetRegistrationId == assetRegistrationId);
            DataSourceResult result = sales.ToDataSourceResult(request, sale => new
            {
                SalesID = sale.SalesID,
                Amount = sale.Amount,
                TaxAmount = sale.TaxAmount,
                TotalAmount = sale.TotalAmount,
                Description = sale.Description,
                OrgBranchName = sale.OrgBranchName,
                DateCreated = sale.DateCreated,
                LastModified = sale.LastModified,
                CreatedBy = sale.CreatedBy,
                ModifiedBy = sale.ModifiedBy
            });

            return Json(result);
        }

        public JsonResult ChangeStatus(string id)
        {
            if (id != null)
            {
                int did = int.Parse(id);
                var item = _db.AssetRegistrations.Find(did);

                if (item != null)
                {
                    item.Disable = false;
                    _db.AssetRegistrations.Attach(item);
                    _db.Entry(item).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region disable log
                    UserHelper.OperationLog(Request.UserHostAddress, "Disable", "Success", User.Identity.Name, "Enabled item  " + item.Tag, Modules.FixedAsset);
                    #endregion
                }

                CacheHelper.ResetCache();
            }

            var formattedData = new
            {
                Message = "Operation Complete.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }
        //Uncomment in production
        public void UpdateFleet(string tagNo, string plateNo, string chasisNo, string serialNo)
        {
            //IFleetService fleetService = new FleetService();
            //var fleet = fleetService.GetAll().FirstOrDefault(f => f.UnitNum == tagNo || f.PlateNumber == plateNo || f.SerialNo == serialNo || f.ChasisNumber == chasisNo);

            //if (fleet != null)
            //{
            //    fleet.PlateNumber = plateNo;
            //    fleet.UnitNum = tagNo;
            //    fleet.ChasisNumber = chasisNo;
            //    fleet.SerialNo = serialNo;
            //    fleetService.Update(fleet);
            //}
        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
