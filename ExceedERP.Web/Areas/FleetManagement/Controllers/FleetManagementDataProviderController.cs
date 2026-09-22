
using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Web.Areas.FleetManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class FleetManagementDataProviderController : Controller
    {

        // GET: FleetManagement/FleetManagementDataProvider
        ExceedDbContext db = new ExceedDbContext();
        private readonly IFleetService _FleetsService;

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetUoM()
        {
            var unitOfm = db.UoMs.AsQueryable();
            return Json(unitOfm.Select(i => new
            {
                UoMName = i.UoMName,
                Value = i.UoMName
            })
            , JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetTimeSheetData(int id)
        //{
        //    var fleet = db.Fleets.Find(id);
        //    var machinery = db.MachineryWorkingTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
        //    var dumpTruck = db.DumpTruckTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
        //    var dryLoading = db.DryLoadingTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
        //    var wagonDrill = db.WagonDrillTimesheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
        //    var lightVehicle = db.LightVehicleTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();

        //    if (machinery.Any())
        //    {

        //    }
        //    else if (dumpTruck.Any())
        //    {

        //    }
        //    else if (dryLoading.Any())
        //    {

        //    }
        //    else if (wagonDrill.Any())
        //    {

        //    }
        //    else if (lightVehicle.Any())
        //    {

        //    }
        //}
        public JsonResult FleetsTyreStoreSelector()
        {
            var storeList = db.InventoryStores;
            var result = storeList.Select(IS => new
            { StoreCode = IS.StoreCode,
              StoreName = IS.Name}).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGlPeriodForPlanAndEvaluation(int fiscalYearId)
        {

            var glPeriod = db.GLPeriods.Where(x => x.GlFiscalYearId == fiscalYearId);

            var result = glPeriod.Select(s => new
            {
                Value = s.GLPeriodId,
                Text = s.Name
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetTaxRate()
        {
            var tax = db.GLTaxRates;
            var result = tax.Select(s => new
            {
                Text = s.TaxName,
                Value = s.GLTaxRateID
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult getGLTaxRates()
        {
            var taxes = db.GLTaxRates.Select(x => new
            {
                Value = x.CalculatedRate,
                Text = x.TaxName
            }).AsQueryable();
            return Json(taxes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFleetRentalRequest2(int supplierId)
        {
            object result = new object();
            var rentFromExternal = db.RentFromExternal.FirstOrDefault(x => x.ProcurementSupplierID == supplierId);
            if (rentFromExternal != null)
            {
                var fleets = db.Fleets.Where(x => x.RentId == rentFromExternal.RentFromExternalID).ToList();
                result = fleets.Select(s => new
                {
                    Value = s.FleetsID,
                    Text = s.PlateNumber + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameID).Name
                });
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranchForRentFromExternal()
        {
            var branch = db.Branch;
            var result = branch.Select(s => new
            {
                Text = s.Name,
                Value = s.BranchId
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult StatusKeyDS()
        {
            var statusKeys = db.InspectionStatusKeys.Select(x => new
            {
                Value = x.InspectionStatusKeyId,
                Name = x.StatusKey

            }).ToList();
          
            return Json(statusKeys, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetEmployeeForTravelOrder()
        //{
        //    var emp = db.Person_Employee.ToList();
        //    var result = emp.Select(s => new
        //    {
        //        Value = s.EmployeeId,
        //        Text = s.FirstName + "-" + s.MiddleName + "-" + s.LastName
        //    }).ToList();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetEmployeeForTravelOrder(string text)
        {
            var emp = db.Person_Employee.AsQueryable();

            if (!string.IsNullOrWhiteSpace(text))
            {
                emp = emp.Where(e =>
                    e.EmployeeId.ToString().Contains(text) ||
                    e.FirstName.Contains(text) ||
                    e.MiddleName.Contains(text) ||
                    e.LastName.Contains(text));
            }

            var result = emp.Select(s => new
            {
                Value = s.EmployeeId,
                Text = s.EmployeeCode + " - " + s.FirstName + " " + s.MiddleName + " " + s.LastName
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeIdForTravelOrder(string id)
        {
            var requestEquipment = db.RequestedEquipments.FirstOrDefault(x => x.RequestNumber.ToString() == id);
            if (requestEquipment != null)
            {
                var assignedEquipment = db.AssignedEquipment.FirstOrDefault(x => x.VehicleDemandId == requestEquipment.VehicleDemandId);
                if (assignedEquipment != null)
                {
                    var op = db.Operator.FirstOrDefault(x => x.OperatorID == assignedEquipment.OperatorId);
                    if (op != null)
                    {
                        var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == op.EmployeeId);
                        var data = new
                        {
                            EmployeeId = employee.EmployeeId
                        };

                        return Json(data);
                    }


                }
            }
            return Json(null);

        }

        public JsonResult GetPlateNoForFuelRequisition(string id)
        {
            var requestEquipment = db.RequestedEquipments.FirstOrDefault(x => x.RequestNumber.ToString() == id);
            if (requestEquipment != null)
            {
                var assignedEquipment = db.AssignedEquipment.FirstOrDefault(x => x.VehicleDemandId == requestEquipment.VehicleDemandId);
                if (assignedEquipment != null)
                {
                    var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == assignedEquipment.FleetId);
                    if (fleet != null)
                    {
                        var data = new
                        {
                            PlateNo = fleet.PlateNumber
                        };

                        return Json(data);
                    }
                }
            }
            return Json(null);
        }
        public JsonResult GetPeriods()
        {
            var periods = db.GLPeriods.Select(x => new
            {
                Value = x.GLPeriodId,
                Text = x.Name
            }).AsQueryable();
            return Json(periods, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFleets(int id,string Text)
        {
            var glPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == id);
            List<Guid> fleetIds = new List<Guid>();
            var equipmentRentAgreementIds = db.RentFromExternal.Where(x => x.IsExpired == false && x.StartDate < glPeriod.DateTo).Select(x => x.RentFromExternalID).ToList();
            if(equipmentRentAgreementIds.Any())
            {
                var rentedEquipments = db.RentFromExternalEquipments.Where(x => equipmentRentAgreementIds.Contains(x.RentFromExternalId)).Select(x=>x.FleetId).ToList();
                if (rentedEquipments.Any())
                {                   
                    fleetIds = rentedEquipments;
                }
            }
            var rentedFleets = db.Fleets.Where(x => fleetIds.Contains(x.FleetsID)&&x.RentId!=null).ToList();
            if(!string.IsNullOrWhiteSpace(Text)&&rentedFleets.Any())
            {
                Text = Text.Trim();
                rentedFleets = rentedFleets
                    .Where(x =>
                        (x.PlateNumber != null && x.PlateNumber.Contains(Text)) ||
                        (x.SerialNo != null && x.SerialNo.Contains(Text)) ||
                        (x.Model != null && x.Model.Contains(Text)))
                    .ToList();
            }
            var result = rentedFleets.Select(x => new
            {
                Value = x.FleetsID,
                Text = x.PlateNumber + " " + x.SerialNo + " " + x.Model
            }).AsQueryable();
                return Json(result,JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFleetsForAdvancePayment(string Text)
        {
            //var glPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == id);
            List<Guid> fleetIds = new List<Guid>();
            var equipmentRentAgreementIds = db.RentFromExternal.Where(x => x.IsExpired == false ).Select(x => x.RentFromExternalID).ToList();
            if(equipmentRentAgreementIds.Any())
            {
                var rentedEquipments = db.RentFromExternalEquipments.Where(x => equipmentRentAgreementIds.Contains(x.RentFromExternalId)).Select(x=>x.FleetId).ToList();
                if (rentedEquipments.Any())
                {
                    fleetIds = rentedEquipments;
                }
            }
            var rentedFleets = db.Fleets.Where(x => fleetIds.Contains(x.FleetsID)&&x.RentId!=null).ToList();
            if(!string.IsNullOrWhiteSpace(Text))
            {
                rentedFleets = rentedFleets.Where(x => x.PlateNumber.ToLower().Contains(Text.ToLower()) || x.SerialNo.ToLower().Contains(Text.ToLower()) || x.Model.ToLower().Contains(Text.ToLower())).ToList();
            }
            var result = rentedFleets.Select(x => new
            {
                Value = x.FleetsID,
                Text = x.PlateNumber + " " + x.SerialNo + " " + x.Model
            }).AsQueryable();
                return Json(result,JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranches()
        {

            var branches = db.Branch;
            return Json(branches.Select(x => new
            {
                Name = x.Code + "-" + x.Name,
                Id = x.BranchId
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBusinessUnits()
        {

            var branches = db.BusinessUnits;
            return Json(branches.Select(x => new
            {
                Name = x.Name,
                Id = x.BusinessUnitId
            }), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetEmployees()
        {
            var employee = db.Person_Employee;
            return
                Json(
                    employee.Select(
                        i => new
                        {
                            Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                            Code = i.EmployeeId
                        }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEquipmentName()
        {
            var EquipmentName = db.EquipmentName;
            return
                Json(
                    EquipmentName.Select(
                        i => new
                        {
                            Name = i.Name,
                            Id = i.EquipmentNameID
                        }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult InternalEquipmentCategory(string CategoryId, string Text)
        {
            Guid categoryGuid;

            if (!Guid.TryParse(CategoryId, out categoryGuid))
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }

            var query = db.EquipmentType.AsQueryable();

            query = query.Where(x => x.EquipmentIdentityID == categoryGuid);

            if (!string.IsNullOrWhiteSpace(Text))
            {
                query = query.Where(x => x.EquipmentTypeName.ToLower().Contains(Text.ToLower()));
            }

            var result = query.Select(x => new
            {
                Value = x.EquipmentTypeID,
                Text = x.EquipmentTypeName
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEquipmentNameForInspection(Guid ETId)
        {
            var EquipmentName = db.EquipmentName.Where(x=>x.EquipmentTypeId==ETId);
            return
                Json(
                    EquipmentName.Select(
                        i => new
                        {
                            Name = i.Name,
                            Id = i.EquipmentNameID
                        }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTax()
        {
            var tax = db.GLTaxRates.AsQueryable();
            return Json(tax.Select(p => new
            {
                Name = p.TaxName,
                Id = p.GLTaxRateID
            }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEquipmentType()
        {
            var EquipmentType = db.EquipmentType;
            return
                Json(
                    EquipmentType.Select(
                        i => new
                        {
                            Name = i.EquipmentTypeName,
                            Id = i.EquipmentTypeID
                        }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult OperatorDDBind()
        {
            var Placment = new List<FleetBindingHelper.HelperObject>();
            var mapping = db.HRFleetMapping.Where(x => x.PlacmentType == FleetPlacmentType.OPERATOR || x.PlacmentType == FleetPlacmentType.DRIVER).ToList();
            foreach (var map in mapping)
            {
                var JobTitles = db.Lookups.Where(x => x.LookupId == map.JobTitleCode && x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE).ToList();
                foreach (var JobTitle in JobTitles)
                {
                    var sparePart = db.Placements.Where(x => x.LookupId == JobTitle.LookupId).ToList().Select(s => new FleetBindingHelper.HelperObject
                    {
                        Value = s.EmployeeId.ToString(),
                        Name = getEmployeeName(s.EmployeeId)
                    }).ToList();
                    Placment.AddRange(sparePart);
                }
            }
            return Json(Placment, JsonRequestBehavior.AllowGet);
        }
        //public JsonResult OperatorDDBindFoREmployee()
        //{
        //    var person = db.Person_Employee.Select(x => new
        //    {
        //        Name = x.FirstNameEnglish,
        //        Id = x.EmployeeId
        //    });
        //    return Json(person, JsonRequestBehavior.AllowGet);
        //}

        public JsonResult OperatorDDBindFoREmployee()
        {
            var Placment = new List<FleetBindingHelper.HelperObject>();

            var mapping = db.HRFleetMapping.Where(x => x.PlacmentType == FleetPlacmentType.OPERATOR || x.PlacmentType == FleetPlacmentType.DRIVER).ToList();
            foreach (var map in mapping)
            {
                var JobTitles = db.Lookups.Where(x => x.LookupId == map.JobTitleCode && x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE).ToList();
                foreach (var JobTitle in JobTitles)
                {
                    var sparePart = db.Placements.Where(x => x.LookupId == JobTitle.LookupId).ToList().Select(s => new FleetBindingHelper.HelperObject
                    {
                        Id = s.EmployeeId,
                        Name = getEmployeeName(s.EmployeeId)
                    }).ToList();
                    Placment.AddRange(sparePart);
                }
            }
            return Json(Placment, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OperatorDDBindFoRAccident()
        {
            var employee = db.Person_Employee
                .Select(x => new
                {
                    Text = x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish,
                    Value=x.EmployeeId
                }).AsQueryable();
            
            return Json(employee, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OperatorForDistribution()
        {
            var operatorForDistribution = db.Operator.Select(x => new { x.OperatorID, x.FullName }).ToList();
            return Json(operatorForDistribution, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OperatorForDistributionNew(string text)
        {
            var operatorForDistribution = db.Operator.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                operatorForDistribution = operatorForDistribution.Where(x => x.FullName.ToLower().Contains(text.ToLower()) || x.EmployeeCode.Contains(text)).AsQueryable();
            }
            var result = operatorForDistribution.Select(s => new
            {
                Text = s.EmployeeCode +" "+s.FullName,
                Value = s.OperatorID
            }).AsQueryable();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OperatorForDistributionFilterable(string text, string equipmentIdentityId)
        {
            var operators = db.Operator.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                operators = operators.Where(x => x.FullName.ToLower().Contains(text.ToLower())).AsQueryable();
            }
            operators.Select(x => new
            {
                value = (Guid?)x.OperatorID,
                text = x.FullName
            })
            .ToList();

            return Json(operators, JsonRequestBehavior.AllowGet);
        }

        //public JsonResult Operators(string text)
        //{
        //    var operators = db.Operator.ToList();
        //    if (!string.IsNullOrWhiteSpace(text))
        //    {
        //        operators = operators.Where(x => x.FullName.ToLower().Contains(text.ToLower())||x.EmployeeCode.ToLower().Contains(text.ToLower())).ToList();
        //    }
        //    var result = operators.Select(s => new
        //    {
        //        Value = s.OperatorID,
        //        Text = s.EmployeeCode+" "+s.FullName
        //    }).ToList();

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult Operators(string text)
        {
            // Join Operator with Person_Employee
            var query = from op in db.Operator
                        join pe in db.Person_Employee on op.EmployeeId equals pe.EmployeeId
                        select new { op.OperatorID, pe.EmployeeCode, pe.FirstNameEnglish,pe.MiddleNameEnglish,pe.LastNameEnglish };

            // Filter if text is provided
            if (!string.IsNullOrWhiteSpace(text))
            {
                text = text.ToLower();
                query = query.Where(x => x.FirstNameEnglish.ToLower().Contains(text) || x.MiddleNameEnglish.ToLower().Contains(text) || x.LastNameEnglish.ToLower().Contains(text) || x.EmployeeCode.ToLower().Contains(text));
            }

            // Project the result
            var result = query.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.EmployeeCode + " " + s.FirstNameEnglish+" "+s.MiddleNameEnglish+" "+s.LastNameEnglish
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        private string getEmployeeName(int employeeId)
        {
            var emp = db.Person_Employee.Find(employeeId);
            if (emp != null)
            {
                var fullName = emp.EmployeeCode + " " + emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish;
                return fullName;
            }
            return null;

        }

        public JsonResult GetAsset()
        {
            var assets = db.AssetRegistrations.Select(x => new { AssetId = x.AssetRegistrationId, x.Name }).ToList();
            return Json(assets, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFixedAssetCategories(string search)
        {
            var equipCats = db.FixedAssetCategories.Select(s => new
            {
                Id = s.FixedAssetCategoryID,
                Name = s.CategoryCode + "_" + s.CategoryName
            }).ToList();
            return Json(equipCats, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEquipmentIdentities(string search)
        {
            var equipmentIdentities = db.EquipmentIdentity.ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                equipmentIdentities = equipmentIdentities.Where(x => x.EquipmentCategoryName.ToLower().Contains(search)).ToList();
            }
            var equipIdentities = equipmentIdentities.Select(s => new
            {
                Id = s.EquipmentIdentityID,
                Name = s.EquipmentCategoryName
            }).ToList();

            return Json(equipIdentities, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEquipmentIdentitiesNullable(string search)
        {
            var equipmentIdentities = db.EquipmentIdentity.ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                equipmentIdentities = equipmentIdentities.Where(x => x.EquipmentCategoryName.ToLower().Contains(search)).ToList();
            }
            var equipIdentities = equipmentIdentities.Select(s => new
            {
                Id = (Guid?)s.EquipmentIdentityID,
                Name = s.EquipmentCategoryName
            }).ToList();

            return Json(equipIdentities, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBranchForSiteVehicleTransfer(string searchKey)
        {
            var branches = db.Branch.ToList();
            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                branches = branches.Where(x => x.Name.ToLower().Contains(searchKey.ToLower())).ToList();
            }
            var result = branches.Select(s => new
            {
                Value = s.BranchId,
                Text = s.Name
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocation(Guid PlateNumber)
        {
            //if (string.IsNullOrEmpty(PlateNumber))
            //    return Json(new { From = "" }, JsonRequestBehavior.AllowGet);

            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == PlateNumber);
            if (fleet == null)
                return Json(new { From = "" }, JsonRequestBehavior.AllowGet);

            var branch = db.Branch.FirstOrDefault(x => x.BranchId == fleet.BranchId);
            var branchName = branch?.Name ?? "";

            return Json(new { From = branchName }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlateNoForSiteVehicleTransfer(string searchKey, Guid equipmentNameID)
        {
            var fleets = db.Fleets.Where(x => x.EquipmentNameID == equipmentNameID).AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchKey))
            {
                fleets = fleets.Where(x => x.PlateNumber.ToLower().Contains(searchKey.ToLower()) || x.SerialNo.ToLower().Contains(searchKey.ToLower())).AsQueryable();
            }
            var result = fleets.Select(s => new
            {
                Text = s.PlateNumber + "/" + s.SerialNo,
                Value = s.FleetsID
            }).ToList();


            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetOperatorForVehicleTransfer()
        {
            var operators = db.Operator.ToList();
            return Json(operators.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.EmployeeCode+" "+s.FullName
            }).ToList(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult getUnitOfMeasurement(string search)
        {
            var UoMs = db.UoMs.Select(s => new
            {
                Name = s.UoMName
            }).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                UoMs = UoMs.Where(x => x.Name.Contains(search)).ToList();
            }
            return Json(UoMs, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEquipmentSubCategories(Guid equipmentIdentityId, string search)
        {
            var equipmentIdentity = db.EquipmentIdentity.Find(equipmentIdentityId);

            var assetCategory = db.FixedAssetCategories.Find(equipmentIdentity.FixedAssetCategoryId);

            var equipSubCats = db.FixedAssetSubCategories.Select(s => new
            {
                Id = s.FixedAssetSubCategoryID,
                s.CategoryCode,
                Name = s.SubCategoryCode + "_" + s.SubCategoryName
            }).ToList();

            if (equipmentIdentity != null)
            {
                equipSubCats = equipSubCats.Where(q => q.CategoryCode == assetCategory.CategoryCode).ToList();
            }

            return Json(equipSubCats, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEquipmentSubCategoryNames(Guid equipmentTypeId, string search)
        {
            var equipmentType = db.EquipmentType.Find(equipmentTypeId);
            var equipSubCatItems = db.FixedAssetSubCategoryNames.Select(s => new
            {
                Id = s.FixedAssetSubCategoryNameId,
                s.FixedAssetSubCategoryID,
                Name = s.Name
            }).ToList();
            if (equipmentType != null)
            {
                equipSubCatItems = equipSubCatItems.Where(q => q.FixedAssetSubCategoryID == equipmentType.FixedAssetSubCategoryId).ToList();
            }

            return Json(equipSubCatItems, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFixedAssetSubCategoryNameById(int assetSubCategoryNameId, string search)
        {
            var subcategoryName = db.FixedAssetSubCategoryNames.Find(assetSubCategoryNameId);
            return Json(new
            {
                Description = subcategoryName?.Name,
                Name = subcategoryName?.Name
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFixedAssetNames(string subCategoryCodeItemId, string search)
        {
            var equipSubCatItems = db.AssetRegistrations.Select(s => new
            {
                Id = s.AssetRegistrationId,
                s.FixedAssetSubCategoryID,
                Name = s.Name
            }).ToList();

            if (subCategoryCodeItemId != null)
            {
                equipSubCatItems = equipSubCatItems.Where(q => q.FixedAssetSubCategoryID.ToString() == subCategoryCodeItemId).ToList();
            }

            return Json(equipSubCatItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEquipmentTypes(Guid? equipmentIdentityId, string search)
        {
            var equipmentTypes = db.EquipmentType.Where(x => x.EquipmentIdentityID == equipmentIdentityId).ToList();
            if (!string.IsNullOrWhiteSpace(search))
            {
                equipmentTypes = equipmentTypes.Where(x => x.EquipmentTypeName.ToLower().Contains(search.ToLower())).ToList();
            }
            var equipTypes = equipmentTypes.Select(s => new
            {
                Id = s.EquipmentTypeID,
                s.EquipmentIdentityID,
                Name = s.EquipmentTypeName
            }).ToList();

            return Json(equipTypes, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetEquipmentNamesFromEquipmentType(Guid equipmentTypeId, string text)
        {

            var equipments = db.EquipmentName.Where(x => x.EquipmentTypeId == equipmentTypeId).ToList();
            if (!string.IsNullOrWhiteSpace(text))
            {
                equipments = equipments.Where(x => x.Name.ToLower().Contains(text.ToLower())).ToList();
            }

            var IdAndName = equipments.Select(s => new
            {
                Id = s.EquipmentNameID,
                Name = s.Name
            }).ToList();

            return Json(IdAndName, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult PlannerDDBind()
        {

            var fleets = db.Planner.ToList().Select(s => new Planner
            {
                PlannerName = s.PlannerName,
                PlannerID = s.PlannerID
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFixedAssetCategoryById(int assetCategoryId, string search)
        {
            var category = db.FixedAssetCategories.Find(assetCategoryId);

            return Json(new
            {
                Description = category?.CategoryCode + "_" + "_" + category?.Abbreviation + category?.CategoryName,
                Name = category?.CategoryName
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFixedAssetSubCategoryById(int assetSubCategoryId, string search)
        {
            var subcategory = db.FixedAssetSubCategories.Find(assetSubCategoryId);

            return Json(new
            {
                Description = subcategory?.SubCategoryCode + "_" + "_" + subcategory?.Abbreviation + subcategory?.SubCategoryName,
                Name = subcategory?.SubCategoryName
            }, JsonRequestBehavior.AllowGet);
        }
       
        public ActionResult GetFleetEquiments(string equipmentTypeId, string search)
        {
            var fleets = db.Fleets.Where(q => q.EquipmentTypeID.ToString() == equipmentTypeId).ToList()
                .Select(s => new
                {
                    Id = s.FleetsID.ToString(),
                    Name = s.PlateNumber + "_" + s.EquipmentName
                });
            if (!string.IsNullOrWhiteSpace(search))
            {
                fleets = fleets.Where(x => x.Name.Contains(search));
            }
            return Json(fleets, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFleetEquipmentById(Guid fleetId, string search)
        {
            var fleet = db.Fleets.Find(fleetId);

            return Json(new
            {
                fleet
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEquipmentNameByPlateNumber(Guid fleetId, string search)
        {
            var fleet = db.Fleets.Find(fleetId);

            return Json(new
            {
                fleet
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEquipmentNames(Guid? equipmentTypeId, string text)
        {
            var equipmentNames = db.EquipmentName.Where(x => x.EquipmentTypeId == equipmentTypeId).ToList();
            if (!string.IsNullOrWhiteSpace(text))
            {
                equipmentNames = equipmentNames.Where(x => x.Name.ToLower().Contains(text.ToLower())).ToList();
            }
            var result = equipmentNames.Select(s => new
            {
                Name = s.Name,
                Id = s.EquipmentNameID
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEquipmentNamesNullable(Guid? equipmentTypeId, string text)
        {
            var equipmentNames = db.EquipmentName.Where(x => x.EquipmentTypeId == equipmentTypeId).ToList();
            if (!string.IsNullOrWhiteSpace(text))
            {
                equipmentNames = equipmentNames.Where(x => x.Name.ToLower().Contains(text.ToLower())).ToList();
            }
            var result = equipmentNames.Select(s => new
            {
                Name = s.Name,
                s.EquipmentTypeId,
                Id = (Guid?)s.EquipmentNameID
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetExternalFleet(Guid? equipmentTypeId, string search)
        {
            var fleets = db.Fleets.Where(x => x.EquipmentTypeID == equipmentTypeId && x.WayOfSupplyenum == WayOfSupplyenum.Rental);
            if (!string.IsNullOrWhiteSpace(search))
            {
                fleets = fleets.Where(x => x.PlateNumber.Contains(search) || x.SerialNo.Contains(search));
            }
            var result = fleets.Select(s => new
            {
                Text = s.PlateNumber + " " + s.SerialNo,
                Value = s.SerialNo
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetExternalFleetForRentFromExternalEquipment(Guid? equipmentNameID, string search)
        {
            var fleets = db.Fleets.Where(x => x.EquipmentNameID == equipmentNameID && x.WayOfSupplyenum == WayOfSupplyenum.Rental);
            if (!string.IsNullOrWhiteSpace(search))
            {
                fleets = fleets.Where(x => x.PlateNumber.ToLower().Contains(search.ToLower()) || x.SerialNo.ToLower().Contains(search.ToLower())|| x.Model.ToLower().Contains(search.ToLower()));
            }
            var result = fleets.Select(s => new
            {
                Text = s.PlateNumber + " " + s.SerialNo,
                Value = s.FleetsID
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //public ActionResult FixedAssetDDBind(string Id, string Search)
        //{
        //    // Get EquipmentName by EquipmentNameId 
        //    var FetchFleets = db.Fleets.Where(x => x.EquipmentNameID.ToString() == Id).Select(p=>new {
        //        EquipmentName=p.PlateNumber,
        //        PlateNumber=p.PlateNumber
        //    }).ToList();
        //    return Json(FetchFleets, JsonRequestBehavior.AllowGet);

        //}
        public ActionResult FixedAssetDDBind(string Id, string Search)
        {
            var equipmentName = db.EquipmentName.Where(q => q.EquipmentNameID.ToString() == Id).FirstOrDefault();
            var fleets = new List<FleetViewModel>();
            if (equipmentName != null)
            {
                // Get FixedAssetsubCategoryNames by PrimeryKey equipmentName.FixedAssetSubCategoryNameId 
                var subCatName = db.FixedAssetSubCategoryNames.Find(equipmentName.FixedAssetSubCategoryNameId);
                if (subCatName != null)
                {
                    // Get AssetRegistration By FixedAssetCategoryName
                    fleets = db.AssetRegistrations.Where(x => x.FixedAssetNameID == subCatName.FixedAssetSubCategoryNameId || x.Name == subCatName.Name).Select(s => new FleetViewModel
                    {
                        EquipmentName = s.Tag + "_" + s.PlateNo + "_" + s.ChassisNo + "_" + s.Name,
                        PlateNumber = s.AssetRegistrationId.ToString()
                    }).ToList();
                }
            }


            var registeredFleets = db.Fleets.ToList();

            foreach (var fleet in fleets.ToList())
            {
                if (registeredFleets.Any(x => x.PlateNumber == fleet.PlateNumber))
                {
                    fleets.Remove(fleet);
                }
            }

            if (!string.IsNullOrWhiteSpace(Search))
            {
                fleets = fleets.Where(x => x.EquipmentName.ToUpper().Contains(Search.ToUpper())).ToList();
            }
            return Json(fleets.DistinctByField(x => x.EquipmentName), JsonRequestBehavior.AllowGet);
        }

        public ActionResult FixedAssetById(string Id)
        {
            var FixedAssets = db.AssetRegistrations.Where(x => x.AssetRegistrationId.ToString() == Id).Select(s => new AssetRegistrationViewModel
            {
                AssetRegistrationId = s.AssetRegistrationId,
                BarCodeNo = s.BarCodeNo,
                Battery = s.Battery,
                BookValue = s.BookValue,
                ChassisNo = s.ChassisNo,
                Condition = s.Condition.Name,
                EngineNo = s.EngineNo,
                FuelType = s.FuelType,
                LoadCapacity = s.LoadCapacity,
                Location = s.Location,
                ManufacturDate = s.ManufacturDate,
                Manufacturer = s.Manufacturer,
                ModelNo = s.ModelNo,
                PlateNo = s.PlateNo,
                PurchaseDate = s.PurchaseDate,
                PurchasePrice = s.OriginalValue - s.BookValue,
                OriginalValue = s.OriginalValue,
                Name = s.Name,
                SerialNo = s.SerialNo,
                Supplier = s.Supplier,
                Tag = s.Tag,
                Tyre = s.Tyre
            }).FirstOrDefault();


            if (FixedAssets != null)
            {
                var Fleet = db.Fleets.Where(x => x.UnitNum == FixedAssets.Tag).FirstOrDefault();
                if (Fleet != null)
                {
                    FixedAssets.IsRegistered = true;
                }

                return Json(FixedAssets, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FleetInformationByPlateNumber(string Id)
        {
            var fleetInformation = db.Fleets.Where(x => x.FleetsID.ToString() == Id).Select(s => new
            {
                PlateNumber = s.PlateNumber,
                Model = s.Model,
                Battery = s.Battery,
                BookValue = s.BookValue,
                Capacity = s.Capacity,
                RentalRate = s.RentalRate,
                SerialNo = s.SerialNo,
                ChasisNumber = s.ChasisNumber,
                EngineNumber = s.EngineNumber,
                Location = s.Location,
                Manufacturer = s.Manufacturer,
                Libre = s.Libre,
                ManufacturerYear = s.ManufacturerYear,
                PlannerID = s.PlannerID,
                IdealRentalRate = s.IdealRentalRate,
                RentType = s.RentType,
                MaxFuelStandard = s.MaxFuelStandard,
                MinFuelStandard = s.MinFuelStandard
            }).FirstOrDefault();


            if (fleetInformation != null)
            {
                var Fleet = db.Fleets.Where(x => x.PlateNumber == fleetInformation.PlateNumber).FirstOrDefault();
                return Json(fleetInformation, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }


        public ActionResult EquipmentTypeById(string Id)
        {
            var equipmentType = db.Fleets.Where(x => x.EquipmentTypeID.ToString() == Id).Select(s => new RentalInspectionViewModel
            {
                EquipmentTypeID = s.EquipmentTypeID,
                Model = s.Model,
                ChasisSerialNo = s.ChasisNumber,
                EngineModel = s.EngineModel,
                ManufacturingYear = s.ManufacturerYear,
                Capacity = s.Capacity,
                PlateNo = s.PlateNumber,
                EngineSerialNo = s.EngineNumber,



            }).FirstOrDefault();

            if (equipmentType != null)
            {
                var Fleet = db.Fleets.Where(x => x.EquipmentTypeID == equipmentType.EquipmentTypeID).FirstOrDefault();
                if (Fleet != null)
                {
                    equipmentType.IsRegistered = true;
                }

                return Json(equipmentType, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }





        
        public ActionResult FleetsDDBind(Guid? equipmentNameId)
        {
            var assetRegistrations = db.AssetRegistrations.ToList();
            var fleets = db.Fleets.ToList().Where(x => !x.IsExternal && !x.IsReturned).Select(s => new FleetViewModel
            {
                PlateNumber = GetAssetDependancies(s, assetRegistrations).PlateNumber + " - " + s.UnitNum,
                EquipmentNameID = s.EquipmentNameID,
                FleetsID = s.FleetsID
            });
            if (equipmentNameId != null)
            {
                fleets = fleets.Where(q => q.EquipmentNameID == equipmentNameId).ToList();
            }
            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFleetFromEquipmentName(Guid equipmentNameId, string text)
        {

            //var existingPlates = db.ExternalRentedEquipments.Where(x => x.EquipmentNameID == equipmentNameId).Select(x => x.FleetId).ToList();
            var existingPlates = db.AssignedEquipment.Where(x => x.EquipmentNameID == equipmentNameId&&x.IsAssigned==true).Select(x => x.FleetId).ToList();
            var fleets = db.Fleets.Where(x => x.EquipmentNameID == equipmentNameId &&!existingPlates.Contains(x.FleetsID) && (x.Status == MaintenanceConst.ACTIVE || x.Status == null || x.Status == ""));
            if (!string.IsNullOrWhiteSpace(text))
            {
                fleets = fleets.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()));
            }
            var data = fleets.Select(s => new
            {
                Id = s.FleetsID,
                Name = s.PlateNumber
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }


        public ActionResult GetFleetPlateNoForArrival(Guid equipmentNameId)
        {
            return Json(db.Fleets.Where(x => x.EquipmentNameID == equipmentNameId).Select(s => new
            {
                Id = s.FleetsID,
                Name = s.PlateNumber
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFleet()
        {
            var fleets = db.Fleets.ToList();
            var data = fleets.Select(s => new FleetViewModel
            {
                PlateNumber = s.PlateNumber + " - " + s.SerialNo,
                FleetsID = s.FleetsID
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFleetForEquipmentOperationHR(string text, Guid equipmentNameId)
        {
            var fleet = db.Fleets.Where(x => x.EquipmentNameID == equipmentNameId).AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                fleet = fleet.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()) || x.SerialNo.ToLower().Contains(text.ToLower())).AsQueryable();
            }
            var result = fleet.Select(s => new
            {
                Text = s.PlateNumber + " " + s.SerialNo,
                Value = s.FleetsID
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFleetForPlanAndEvaluation(string text)
        {
            var fleet = db.Fleets.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                fleet = fleet.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()) || x.SerialNo.ToLower().Contains(text.ToLower())).AsQueryable();
            }
            var result = fleet.Select(s => new
            {
                Text = s.PlateNumber + " " + s.SerialNo,
                Value = s.FleetsID
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFleetRentalRequest(int? supplierId, int? branchId,int glPeriodId, Guid equipmentNameId,string Text)
        {
            //if(supplierId == null)
            //{
            //    var internalFleet = db.Fleets.Where(x => x.EquipmentNameID == equipmentNameId).AsQueryable();

            //    var result = internalFleet.Select(s => new
            //    {
            //        Value = s.FleetsID,
            //        Text = s.PlateNumber + "-" + s.SerialNo
            //    });
            //    return Json(result, JsonRequestBehavior.AllowGet);
            //}
            var fleets = db.Fleets.Where(x => x.EquipmentNameID == equipmentNameId && x.IsExternal).Select(x => x.FleetsID).ToList(); ;
            List<Fleets> selectedFleets = new List<Fleets>();
            //var rentFromExternalIds = db.RentFromExternal
            //    .Where(x => x.ProcurementSupplierID == supplierId && x.BranchId == branchId)
            //    .Select(s => s.RentFromExternalID);
            //if (rentFromExternalIds != null)
            //{
            //    fleets = fleets.Where(x => rentFromExternalIds.Contains(x.RentId));
            //    var result = fleets.Select(s => new
            //    {
            //        Value = s.FleetsID,
            //        Text = s.PlateNumber + "-" + s.SerialNo
            //    });
            //    return Json(result, JsonRequestBehavior.AllowGet);
            //}
            var procurementSupplier = db.ProcurementSuppliers.FirstOrDefault(x => x.ProcurementSupplierID == supplierId).Name;
            var existingSummaries = db.RentalPaymentRequestLines.Select(x => x.CostSheetSummaryId).ToList();
            var paymentSummary = db.RentedEquipmentMonthlyPaymentSummary.Where(x => x.TimeSheetIsDGMApproved && x.CustomerName == procurementSupplier && x.BranchId == branchId && x.GlPeriodId == glPeriodId&&!existingSummaries.Contains(x.RentedEquipmentMonthlyPaymentSummaryId)).ToList();
            if (paymentSummary.Any())
            {
                var fleetIds = paymentSummary.Select(x => x.FleetId).ToList();

                selectedFleets = db.Fleets.Where(x => fleetIds.Contains(x.FleetsID.ToString()) && x.EquipmentNameID == equipmentNameId).ToList();
                if (!string.IsNullOrWhiteSpace(Text))
                { 
                    Text = Text.Trim();
                    selectedFleets = selectedFleets
                    .Where(x =>
                        (x.PlateNumber != null && x.PlateNumber.Contains(Text)) ||
                        (x.SerialNo != null && x.SerialNo.Contains(Text)) ||
                        (x.Model != null && x.Model.Contains(Text)))
                    .ToList();
            }
                if (selectedFleets.Any())
                {
                    var result = selectedFleets.Select(s => new
                    {
                        Value = s.FleetsID,
                        Text = s.PlateNumber + "-" + s.SerialNo + "-" + s.Model
                    });
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
                
            
            return Json(null, JsonRequestBehavior.AllowGet);
        }
       
        public JsonResult GetFleetTimesheet(TimeSheetType timeSheetEquipmentType, string text)
        {
            var equipmentTypeId = new HashSet<Guid>(db.EquipmentType.Where(x => x.TimeSheetType == timeSheetEquipmentType).Select(x => x.EquipmentTypeID));

            var fleets = db.Fleets.Where(x => equipmentTypeId.Contains(x.EquipmentTypeID)).AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                fleets = fleets.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()) || x.SerialNo.ToLower().Contains(text.ToLower())).AsQueryable();
            }
            var data = fleets.Select(s => new FleetViewModel
            {
                PlateNumber = s.PlateNumber + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameID).Name,
                FleetsID = s.FleetsID
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFleetIdTimesheetNew( string text,string timeSheetType)
        {
            var fleets = db.Fleets.AsQueryable();
            var externalIds = fleets.Where(x => x.IsExternal).Select(x=>x.FleetsID).ToList();
            //fleets = fleets.Where(x => !externalIds.Contains(x.FleetsID));
            var externalFleets = db.Fleets.Where(x => externalIds.Contains(x.FleetsID)&&x.RentId==null).Select(x=>x.FleetsID).ToList();
            fleets = fleets.Where(x => !externalFleets.Contains(x.FleetsID));
            if (timeSheetType=="LightVehicle")
            {
                var EquipmentTypes = db.EquipmentType.Where(x => x.TimeSheetType == TimeSheetType.LightVehicle).Select(x=>x.EquipmentTypeID).ToList();
                fleets = fleets.Where(x => EquipmentTypes.Contains(x.EquipmentTypeID)&&!x.IsContractTerminated).AsQueryable();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    fleets = fleets.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()) || x.SerialNo.ToLower().Contains(text.ToLower()) || x.Model.ToLower().Contains(text.ToLower())).AsQueryable();
                }
                var data = fleets.Select(s => new FleetViewModel
                {
                    PlateNumber = s.PlateNumber + " " + s.SerialNo + " " + s.Model,
                    FleetsID = s.FleetsID
                }).AsQueryable();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if (timeSheetType=="DumpTruck")
            {
                var EquipmentTypes = db.EquipmentType.Where(x => x.TimeSheetType == TimeSheetType.DumpTruck).Select(x => x.EquipmentTypeID).ToList();
                fleets = fleets.Where(x => EquipmentTypes.Contains(x.EquipmentTypeID)&&!x.IsContractTerminated).AsQueryable();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    fleets = fleets.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()) || x.SerialNo.ToLower().Contains(text.ToLower()) || x.Model.ToLower().Contains(text.ToLower())).AsQueryable();
                }
                var data = fleets.Select(s => new FleetViewModel
                {
                    PlateNumber = s.PlateNumber + " " + s.SerialNo + " " + s.Model,
                    FleetsID = s.FleetsID
                }).AsQueryable();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if (timeSheetType=="DryLoading")
            {
                var EquipmentTypes = db.EquipmentType.Where(x => x.TimeSheetType == TimeSheetType.DryLoading).Select(x => x.EquipmentTypeID).ToList();
                fleets = fleets.Where(x => EquipmentTypes.Contains(x.EquipmentTypeID)&&!x.IsContractTerminated).AsQueryable();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    fleets = fleets.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()) || x.SerialNo.ToLower().Contains(text.ToLower()) || x.Model.ToLower().Contains(text.ToLower())).AsQueryable();
                }
                var data = fleets.Select(s => new FleetViewModel
                {
                    PlateNumber = s.PlateNumber + " " + s.SerialNo + " " + s.Model,
                    FleetsID = s.FleetsID
                }).AsQueryable();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if (timeSheetType=="Machinery")
            {
                var EquipmentTypes = db.EquipmentType.Where(x => x.TimeSheetType == TimeSheetType.Machinery).Select(x => x.EquipmentTypeID).ToList();
                fleets = fleets.Where(x => EquipmentTypes.Contains(x.EquipmentTypeID)&&!x.IsContractTerminated).AsQueryable();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    fleets = fleets.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()) || x.SerialNo.ToLower().Contains(text.ToLower()) || x.Model.ToLower().Contains(text.ToLower())).AsQueryable();
                }
                var data = fleets.Select(s => new FleetViewModel
                {
                    PlateNumber = s.PlateNumber + " " + s.SerialNo + " " + s.Model,
                    FleetsID = s.FleetsID
                }).AsQueryable();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            if (timeSheetType=="WagonDrill")
            {
                var EquipmentTypes = db.EquipmentType.Where(x => x.TimeSheetType == TimeSheetType.WagonDrill).Select(x => x.EquipmentTypeID).ToList();
                fleets = fleets.Where(x => EquipmentTypes.Contains(x.EquipmentTypeID)&&!x.IsContractTerminated).AsQueryable();
                if (!string.IsNullOrWhiteSpace(text))
                {
                    fleets = fleets.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()) || x.SerialNo.ToLower().Contains(text.ToLower()) || x.Model.ToLower().Contains(text.ToLower())).AsQueryable();
                }
                var data = fleets.Select(s => new FleetViewModel
                {
                    PlateNumber = s.PlateNumber + " " + s.SerialNo + " " + s.Model,
                    FleetsID = s.FleetsID
                }).AsQueryable();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            var fleetss = db.Fleets.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                fleetss = fleetss.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()) || x.SerialNo.ToLower().Contains(text.ToLower())|| x.Model.ToLower().Contains(text.ToLower())).AsQueryable();
            }
            var dataa = fleets.Select(s => new FleetViewModel
            {
                PlateNumber = s.PlateNumber + " " + s.SerialNo+" "+s.Model,
                FleetsID = s.FleetsID
            });
            return Json(dataa, JsonRequestBehavior.AllowGet);
            }
            //public JsonResult GetFleetIdTimesheetNew(Guid equipmentNameId, string text)
        //{

        //    var fleets = db.Fleets.Where(x => x.EquipmentNameID == equipmentNameId).AsQueryable();
        //    if (!string.IsNullOrWhiteSpace(text))
        //    {
        //        fleets = fleets.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()) || x.SerialNo.ToLower().Contains(text.ToLower())).AsQueryable();
        //    }
        //    var data = fleets.Select(s => new FleetViewModel
        //    {
        //        PlateNumber = s.PlateNumber + " " + s.SerialNo,
        //        FleetsID = s.FleetsID
        //    });
        //    return Json(data, JsonRequestBehavior.AllowGet);
        //}
        public ActionResult GetEquipmentTypesTimeSheet(TimeSheetType? timeSheetEquipmentType, Guid? equipmentIdentityId, string search)
        {
            List<EquipmentType> equipmentTypes = new List<EquipmentType>();
            if (timeSheetEquipmentType == null)
            {
                equipmentTypes = db.EquipmentType
                .Where(x => x.EquipmentIdentityID == equipmentIdentityId)
                .ToList();
            }
            else
            {
                equipmentTypes = db.EquipmentType
               .Where(x => x.EquipmentIdentityID == equipmentIdentityId)
               .Where(x => x.TimeSheetType == timeSheetEquipmentType)
               .ToList();
            }
           
            if (!string.IsNullOrWhiteSpace(search))
            {
                equipmentTypes = equipmentTypes.Where(x => x.EquipmentTypeName.ToLower().Contains(search.ToLower())).ToList();
            }
            var equipTypes = equipmentTypes.Select(s => new
            {
                Id = (Guid?)s.EquipmentTypeID,
                s.EquipmentIdentityID,
                Name = s.EquipmentTypeName
            }).ToList();

            return Json(equipTypes, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetFleetIsExternal(string fleetId)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID.ToString() == fleetId);
            int? supplierID=null;
            var extEquipment = db.RentFromExternalEquipments.FirstOrDefault(x=>x.FleetId.ToString()==fleetId);
            if(extEquipment!=null)
            {
                supplierID = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == extEquipment.RentFromExternalId)?.ProcurementSupplierID;
            } 
            return Json(new
            {
                isExternal = fleet != null && fleet.IsExternal,
                EquipmentCategory=fleet.EquipmentIdentityID,
                EquipmentName=fleet.EquipmentNameID,
                EquipmentType=fleet.EquipmentTypeID,
                SupplierId= supplierID,
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFleetTimesheetSerialNumber(/*TimeSheetType timeSheetEquipmentType, */string text, Guid equipmentNameId)
        {
            //var equipmentTypeId = new HashSet<Guid>(db.EquipmentType.Where(x => x.TimeSheetType == timeSheetEquipmentType).Select(x => x.EquipmentTypeID));

            var fleets = db.Fleets.Where(x => x.EquipmentNameID == equipmentNameId).AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                fleets = fleets.Where(x => x.PlateNumber.ToLower().Contains(text.ToLower()) || x.SerialNo.ToLower().Contains(text.ToLower())).AsQueryable();
            }
            var data = fleets.Select(s => new FleetViewModel
            {
                PlateNumber = s.SerialNo + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameID).Name,
                FleetsID = s.FleetsID
            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEquipmentNameInsurance()
        {
            var equipmentName = db.EquipmentName;
            var result = equipmentName.Select(s => new
            {
                Text = s.Name,
                Value = s.EquipmentNameID
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFleetForInsurance(string text)
        {
            var fleets = db.Fleets.AsQueryable();
            if (!string.IsNullOrWhiteSpace(text))
            {
                fleets = fleets.Where(x => x.EquipmentName.Contains(text) || x.PlateNumber.Contains(text) || x.SerialNo.Contains(text)).AsQueryable();
            }
            var data = fleets.Select(s => new
            {
                Name = s.PlateNumber + "-" + db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameID).Name,
                Id = s.FleetsID
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBranch()
        {
            return Json(db.Branch.Select(x => new { x.BranchId, x.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPlateNumber(string equipmentNameID)
        {
            var fleets = db.Fleets
                .Where(x => x.EquipmentNameID.ToString() == equipmentNameID && x.WayOfSupplyenum == WayOfSupplyenum.Rental)
                .Select(x => new
                {
                    x.FleetsID,
                    x.PlateNumber
                }).ToList();
            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSuppliers()
        {
            return Json(db.ProcurementSuppliers.Select(x => new { Id = x.ProcurementSupplierID, Name = x.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSuppliersForRentFromExternal(string text)
        {
            var supplier = db.ProcurementSuppliers.ToList();
            if (!string.IsNullOrWhiteSpace(text))
            {
                supplier = supplier.Where(x => x.Name.ToLower().Contains(text.ToLower())).ToList();
            }
            var result = supplier.Select(x => new
            {
                Id = x.ProcurementSupplierID,
                Name = x.Name
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOpenYears()
        {
            return Json(db.ProcurementSuppliers.Select(x => new { x.SupplierId, x.Name }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartments()
        {
            return Json(db.MainCompanyStructures.Select(x => new { x.OrgStructureId, x.Name }), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult GetEquipmentType()
        //{
        //    return Json(db.EquipmentType.Select(x => new { x.EquipmentTypeID, x.EquipmentTypeName }), JsonRequestBehavior.AllowGet);

        //}
        public JsonResult GetInsuranceCompany()
        {
            return Json(db.InsuranceCompany.Select(x => new { x.InsuranceCompanyId, x.InsuranceCompanyName }), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetInsurance()
        //{
        //    return Json(db.FleetInsurance.Select(x => new { x.FleetInsuranceId, x.InsuranceCompanyId }), JsonRequestBehavior.AllowGet);
        //}
        public static Fleets GetAssetDependancies(Fleets fleet, List<AssetRegistration> assetRegistrations)
        {
            if (assetRegistrations != null)
            {
                if (assetRegistrations.Any())
                {
                    var Asset = assetRegistrations.Where(x => x.Tag == fleet.UnitNum).FirstOrDefault();

                    if (Asset != null)
                    {
                        fleet.SerialNo = Asset.SerialNo;

                        fleet.ChasisNumber = Asset.ChassisNo;
                        if (fleet.Location == "Not Assigned" || string.IsNullOrWhiteSpace(fleet.Location))
                        {
                            fleet.Location = Asset.Location;
                        }
                        fleet.Model = Asset.ModelNo;
                        fleet.EngineNumber = Asset.EngineNo;
                        fleet.PlateNumber = Asset.PlateNo;
                        fleet.EquipmentName = Asset.Name;
                        if (Asset.Condition != null)
                        {
                            fleet.Condition = Asset.Condition.Name;
                        }
                    }
                }
            }



            return fleet;
        }

        public ActionResult FleetConditionDDBind()
        {

            var fleets = db.FleetConditions.Select(s => new
            {
                Name = s.Name,
                FleetConditionId = s.FleetConditionId
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult OnRentFleetDDBind()
        {

            IRentFromExternalService _RentFromExternalService = new RentFromExternalService();
            List<Fleets> fleet = new List<Fleets>();

            var Now = DateTime.Now;

            var ExternalRentals = _RentFromExternalService.GetAll().Where(x => x.EndDate > Now).ToList();

            foreach (var ExternalRental in ExternalRentals)
            {
                var fleets = db.Fleets.ToList().Where(x => x.IsExternal == true && x.IsReturned == false && x.RentId == ExternalRental.RentFromExternalID).ToList();

                foreach (var s in fleets)
                {
                    if (fleet.Where(x => x.FleetsID == s.FleetsID).ToList().Count == 0)
                    {

                        fleet.Add(new Fleets
                        {
                            PlateNumber = s.PlateNumber + " - " + s.UnitNum,
                            FleetsID = s.FleetsID
                        });
                    }
                }
            }
            return Json(fleet, JsonRequestBehavior.AllowGet);
        }
        public JsonResult OnTimeSheetFleetDDBind(Guid fleetsId)
        {
            var fleet = db.Fleets.Where(x => x.FleetsID == fleetsId).ToList();
            var result = fleet.Select(s => new
            {
                PlateNumber = s.PlateNumber + " - " + s.UnitNum,
                FleetsID = s.FleetsID
            });
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LicenseLevelDDBind()
        {

            var fleets = db.LicenseLevels.Select(s => new
            {
                Name = s.Name,
                LicenseLevelId = s.LicenseLevelId
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult LicenseTypeDDBind()
        {

            var fleets = db.LicenseTypes.Select(s => new
            {
                Name = s.Name,
                LicenseTypeId = s.LicenseTypeId
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult FleetsTyreReadingDDBind(Guid? equipmentTypeId)
        {
            var fleets = db.Fleets.ToList();
            if (equipmentTypeId != null)
            {
                fleets = fleets.Where(q => q.EquipmentTypeID == equipmentTypeId).ToList();
            }
            var fleetSelecteds = fleets
                .Select(s => new FleetViewSelectedModel
                {
                    EquipmentName = s.PlateNumber + " -- " + s.UnitNum,
                    FleetsID = s.FleetsID,
                    PlateNumber = s.PlateNumber,
                    UnitNum = s.UnitNum
                });
            foreach (var fleet in fleetSelecteds)
            {
                fleet.PlateNumber = GetAssetDependancies(fleet).PlateNumber + " - " + fleet.UnitNum;
            }
            return Json(fleetSelecteds, JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult FleetsTyreIssuedDDBind()
        {
            var fleets = db.TireIssue.Where(q => q.IsOnlineApproved).ToList();

            var fleetSelecteds = fleets
                .Select(s => new FleetViewSelectedModel
                {
                    EquipmentName = s.PlateNumber,
                    FleetsID = (Guid)s.FleetId,
                    PlateNumber = s.PlateNumber,
                }).Distinct();
            return Json(fleetSelecteds, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FleetsTyreIssuedSerialsDDBind(Guid fleetId)
        {
            var fleets = db.TireIssue.Where(q => q.IsOnlineApproved && q.FleetId == fleetId).ToList();

            var fleetSelecteds = fleets
                .Select(s => new FleetViewSelectedModel
                {
                    SerialNo = s.SerialNo
                }).Distinct();
            return Json(fleetSelecteds, JsonRequestBehavior.AllowGet);
        }
        public static FleetViewSelectedModel GetAssetDependancies(FleetViewSelectedModel fleet)
        {
            var Assets = System.Runtime.Caching.MemoryCache.Default["AssetList"] as List<AssetRegistration>;

            if (Assets != null)
            {

                if (Assets.Any())
                {

                    var Asset = Assets.Where(x => x.Tag == fleet.UnitNum).FirstOrDefault();

                    if (Asset != null)
                    {


                        fleet.PlateNumber = Asset.PlateNo;
                        fleet.SerialNo = Asset.SerialNo;
                        fleet.ChasisNumber = Asset.ChassisNo;
                        fleet.EquipmentName = Asset.Name;
                    }
                }
            }
            fleet.FleetsID = fleet.FleetsID;

            return fleet;
        }

        public ActionResult OnDistributionTimeSheetDDBind()
        {

            IVehicleDistributionService _VehicleDistributionService = new VehicleDistributionService();
            List<Fleets> fleet = new List<Fleets>();
            //x.Type == ExceedERP.Core.Domain.FleetManagement.FleetResource.VehicleDistribution.RequestType.Company &&
            var Distributions =db.VehicleDistribution.Where(x => x.EquipmentStatus != MaintenanceConst.Finished && x.IsOnlineApproved == true).ToList();

            foreach (var Distribution in Distributions)
            {
                var fleets = db.Fleets.ToList().Where(x => x.Location == Distribution.FromBranchName /*&& x.Status == MaintenanceConst.ONDUTY*/ && x.IsExternal == false && x.IsReturned == false).ToList();

                foreach (var s in fleets)
                {


                    fleet.Add(new Fleets
                    {
                        PlateNumber = s.PlateNumber + " - " + s.UnitNum,
                        FleetsID = s.FleetsID
                    });

                }
            }


            return Json(fleet.DistinctByField(x => x.FleetsID), JsonRequestBehavior.AllowGet);
        }

        
        public ActionResult UnAssignedEquipmentDDBind(string equipmentNameId, string Search)
        {

            var fleets = db.Fleets.ToList().Where(x => x.OperatorName == MaintenanceConst.UnAssigned && x.EquipmentNameID.ToString() == equipmentNameId).Select(s => new FleetViewModel
            {
                PlateNumber = s?.PlateNumber + "-" + s?.SerialNo,
                FleetsID = s.FleetsID,
                UnitNum = s.UnitNum,
                EquipmentName = s.PlateNumber,
            });
            if (!string.IsNullOrWhiteSpace(Search))
            {
                fleets = fleets.Where(x => x.PlateNumber.ToUpper().Contains(Search.ToUpper()) || x.UnitNum.ToUpper().Contains(Search.ToUpper())).ToList();
            }
            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult FleetsDetailDDBind(Guid FleetsID)
        {

            var fleet = db.Fleets.Where(f => f.FleetsID == FleetsID).FirstOrDefault();


            return Json(fleet.MinFuelStandard + "_" + fleet.TypeOfFuel + "_" + fleet.RentalRate + "_" + fleet.CurentOdoMeter + "_" + fleet.Location + "_" + fleet.OperatorName + "_" + fleet.IdealRentalRate + "_" + fleet.EquipmentName + "_" + fleet.SupplierCompany, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JobOrderReferenceNumber(Guid id)
        {
            var serviceCenterJob = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId == id);
            var jobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == serviceCenterJob.JobOrderID);

            var result = new
            {
                ReferenceNo = serviceCenterJob.Num.ToString()
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}