using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{


    public class EquipmentRegistrationRentalController : Controller
    {

        private readonly IFleetService _FleetsService;
        private readonly IAccessoriesService _AccessoriesService;
        private readonly IFleetHistoryService _FleetHistoryService;
        private ExceedDbContext db = new ExceedDbContext();
        private IJobOrderService _JobOrderService = new JobOrderService();
        private FleetCostHelper FleetCostHelper = new FleetCostHelper();


        public EquipmentRegistrationRentalController()
        {
            _FleetsService = new FleetService();
            _AccessoriesService = new AccessoriesService();
            _FleetHistoryService = new FleetHistoryService();
        }

        [AuthorizeRoles(FleetManagementRoles.RegistrationApprover)]
        public ActionResult ApproveEquipment()
        {

            return View();
        }
        public ActionResult Tabstrip()
        {

            return View();
        }

        public static Fleets GetAssetBranchandCondition(Fleets fleet, List<AssetRegistration> assetRegistrations, List<Branch> branches)
        {

            if (assetRegistrations != null)
            {


                if (assetRegistrations.Any())
                {

                    var Asset = assetRegistrations.Where(x => x.Tag == fleet.UnitNum).FirstOrDefault();

                    if (Asset != null)
                    {
                        if (Asset.Condition != null)
                        {
                            fleet.IFRSCondition = Asset.Condition.Name;
                        }

                        if (fleet.Location == "Not Assigned" || fleet.Location == "UnAssigned" || string.IsNullOrWhiteSpace(fleet.Location))
                        {
                            var branch = branches.Where(x => x.BranchId == Asset.BranchId).FirstOrDefault();
                            if (branch != null)
                            {
                                fleet.Location = branch.Name;
                            }
                        }
                        else
                        {
                            fleet.Location = fleet.Location;
                        }
                    }
                }
            }
            return fleet;
        }
        public static Fleets GetAssetLocation(Fleets fleet, List<AssetRegistration> assetRegistrations, List<Branch> branches)
        {

            //if (assetRegistrations != null)
            //{


            //    if (assetRegistrations.Any())
            //    {

            //        var Asset = assetRegistrations.Where(x => x.Tag == fleet.UnitNum).FirstOrDefault();

            //        if (Asset != null)
            //        {
            //            if (Asset.Condition != null)
            //            {
            //                fleet.IFRSCondition = Asset.Condition.Name;
            //            }

            //            if (fleet.Location == "Not Assigned" || fleet.Location == "UnAssigned" || string.IsNullOrWhiteSpace(fleet.Location))
            //            {
            //                var branch = branches.Where(x => x.BranchId == Asset.BranchId).FirstOrDefault();
            //                if (branch != null)
            //                {
            //                    fleet.Location = branch.Name;
            //                }
            //            }
            //            else
            //            {
            //                fleet.Location = fleet.Location;
            //            }
            //        }
            //    }
            //}
            return fleet;
        }
        public static Fleets GetAssetDependancies(Fleets fleet, List<AssetRegistration> assetRegistrations)
        {
            //if (assetRegistrations != null)
            //{


            //    if (assetRegistrations.Any())
            //    {

            //        var Asset = assetRegistrations.Where(x => x.Tag == fleet.UnitNum).FirstOrDefault();

            //        if (Asset != null)
            //        {



            //                fleet.SerialNo = Asset.SerialNo;

            //                fleet.ChasisNumber =  Asset.ChassisNo;


            //            if (fleet.Location == "Not Assigned" || string.IsNullOrWhiteSpace(fleet.Location))
            //            {
            //                fleet.Location = Asset.Location;
            //            }

            //                fleet.Model = Asset.ModelNo;

            //            fleet.EngineNumber = Asset.EngineNo;
            //            fleet.PlateNumber = Asset.PlateNo;
            //            fleet.EquipmentName = Asset.Name;
            //            if (Asset.Condition != null)
            //            {
            //                fleet.Condition = Asset.Condition.Name;
            //            }
            //        }
            //    }
            //}



            return fleet;
        }
        public static FleetViewModel GetAssetDependancies(FleetViewModel fleet)
        {
            //var Assets = System.Runtime.Caching.MemoryCache.Default["AssetList"] as List<AssetRegistration>;

            //if (Assets != null)
            //{

            //    if (Assets.Any())
            //    {

            //        var Asset = Assets.Where(x => x.Tag == fleet.UnitNum).FirstOrDefault();

            //        if (Asset != null)
            //        {


            //            fleet.PlateNumber = Asset.PlateNo;
            //            fleet.SerialNo = Asset.SerialNo;
            //            fleet.ChasisNumber = Asset.ChassisNo;
            //            fleet.EquipmentName = Asset.Name;
            //        }
            //    }
            //}
            //fleet.FleetsID = fleet.FleetsID;

            return fleet;
        }

        #region Approval

        public ActionResult EquipmentApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<EquipmentApproval> fleetApproval = db.EquipmentApproval.Where(x => x.FleetsId == Id).ToList();
            List<Fleets> Category = db.Fleets.Where(x => x.FleetsID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new EquipmentApproval()
                {
                    FleetsId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,

                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new EquipmentApproval()
            {
                EquipmentApprovalId = approval.EquipmentApprovalId,
                FleetsId = approval.FleetsId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        ////public ActionResult Equipment_SendForOnlineApproval(BindingModel model)
        ////{

        ////    List<CheckListLog> log = db.CheckListLogs.Where(x => x.FleetId == model.Id).ToList();
        ////    if (log.Any())
        ////    {

        ////        Guid Id = Guid.Parse(model.Id);
        ////        var fleet = db.Fleets.Where(x => x.FleetsID == Id).FirstOrDefault();
        ////        if (fleet != null)
        ////        {
        ////            fleet.IsSendForApproval = true;
        ////            fleet.LastModified = DateTime.Now;
        ////            fleet.ModifiedBy = User.Identity.Name;

        ////            db.Fleets.Attach(fleet);
        ////            db.Entry(fleet).State = EntityState.Modified;
        ////            db.SaveChanges();
        ////        }
        ////    }

        ////    return RedirectToActionPermanent("Index", "EquipmentRegistration", new { area = "FleetManagement" });
        ////}

        public JsonResult SendForApproval(Guid id)
        {
            var record = db.Fleets.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved)
            {
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.Fleets.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Send for approval", "Success", User.Identity.Name, $"created asset reg {record.FleetsID} Tag {record.EquipmentName}", Modules.FleetManagement);
                #endregion



                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            else if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            else if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        [AuthorizeRoles(FleetManagementRoles.RegistrationApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Equipment_Approve([DataSourceRequest]DataSourceRequest request, EquipmentApproval EquipmentApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (EquipmentApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.Fleets.Where(x => x.FleetsID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;

                    db.Fleets.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Equipment Approve", "Success", User.Identity.Name, $"created asset reg {entity.FleetsID} Tag {entity.EquipmentName}", Modules.FleetManagement);
                    #endregion

                    Notify.NotifyUser(new NotifModels { UserId = User.Identity.Name, DeadLine = DateTime.Now, Message = entity.PlateNumber + " - " + entity.UnitNum + " Approved Equipment Registration ", Title = " Approved Equipment Registration ", URL = Url.Action("Index", "EquipmentRegistration", new { area = "FleetManagment" }) });

                }
                if (EquipmentApproval.Status == MaintenanceConst.Rejected)
                {
                    var entity = db.Fleets.Where(x => x.FleetsID == Id).FirstOrDefault();

                    entity.IsSendForApproval = false;

                    db.Fleets.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Equipment Approve", "Success", User.Identity.Name, $"created asset reg {entity.FleetsID} Tag {entity.EquipmentName}", Modules.FleetManagement);
                    #endregion

                }
                var approval = new EquipmentApproval()
                {

                    Status = EquipmentApproval.Status,
                    FleetsId = Id,
                    Remark = EquipmentApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.EquipmentApproval.Add(approval);
                db.SaveChanges();
                EquipmentApproval.EquipmentApprovalId = approval.EquipmentApprovalId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Equipment Approve", "Success", User.Identity.Name, $"created asset reg {approval.EquipmentApprovalId} Tag {approval.EquipmentApprovalId}", Modules.FleetManagement);
                #endregion


            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { EquipmentApproval }.ToDataSourceResult(request));
        }


        #endregion
        //
        // GET: /FleetManagement/Fleets/
        [AuthorizeRoles(FleetManagementRoles.RegistrationUser, FleetManagementRoles.RegistrationApprover)]

        public ActionResult Index(string Reset)
        {


            if (!string.IsNullOrWhiteSpace(Reset) && Reset.IsCaseInsensitiveEqual("Reset"))
            {
                System.Runtime.Caching.MemoryCache.Default["FleetList"] = string.Empty;
                System.Runtime.Caching.MemoryCache.Default["AssetList"] = db.AssetRegistrations.ToList();
            }
            var EquipmentStatuss = new List<FleetBindingHelper.HelperObject>();
            var EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = "Operational",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = "Due to Non Avilablity of Construction Assignment",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = "Due To Other Machine non Avilablity",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = "Due to Lack of Fuel",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = "Due To lack of Operators / Drivers ",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            EquipmentStatus = new FleetBindingHelper.HelperObject
            {
                Name = "Due To Accident ",
                Value = "",
            };
            EquipmentStatuss.Add(EquipmentStatus);
            /*    EquipmentStatus = new BindingHelper.HelperObject
                {
                    Name = " Under Maintenace",
                    Value = "",
                };
                EquipmentStatuss.Add(EquipmentStatus);*/

            ViewBag.EquipmentStatus = EquipmentStatuss;

            var equitsCats = db.EquipmentIdentity.Select(s => new
            {
                Id = s.EquipmentIdentityID,
                Name = s.EquipmentCategoryName
            }).ToList();
            var equipSubCats = db.EquipmentType.Select(s => new
            {
                Id = s.EquipmentTypeID,
                Name = s.EquipmentTypeName
            }).ToList();
            var equipSubCatNames = db.EquipmentName.Select(s => new
            {
                Id = s.EquipmentNameID,
                Name = s.Name
            }).ToList();
            var assets = db.AssetRegistrations.Select(s => new
            {
                Id = s.AssetRegistrationId,
                Name = s.Name
            }).ToList();

            var branch = db.Branch.Select(s => new
            {
                Id = s.BranchId,
                Name = s.Name
            });
            ViewData["EquipmentCategorys"] = equitsCats;
            ViewData["EquipmentSubCategorys"] = equipSubCats;
            ViewData["EquipmentNames"] = equipSubCatNames;
            ViewData["FixedAssets"] = assets;
            ViewData["VehicleParts"] = db.VehiclePart.ToList();
            ViewData["VehiclePartDamageType"] = db.VehiclePartDamageType.ToList();
            ViewData["Branch"] = branch;
            return View();
        }


        public ActionResult UploadPhoto(BindingModel model, IEnumerable<HttpPostedFileBase> AttachmentFile)
        {



            try
            {
                foreach (var fileBase in AttachmentFile)
                {

                    string path = Server.MapPath("~/Content/Images/Fleet/Equipment/");
                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                    {
                        Directory.CreateDirectory(path);
                    }

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    fileBase.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/Fleet/Equipment/" + model.Id + ".jpg")));


                }
            }
            catch (Exception)
            {

            }
            return RedirectToAction("Index");
        }

        public ActionResult FleetCost(string Id)
        {
            //FleetCostHelper fleetCostHelper = new FleetCostHelper();
            var inventoryMapping = db.InventorySparePartMapping.ToList();


            var assetRegistrations = db.AssetRegistrations.ToList();


            var fleets = _FleetsService.GetAll().Where(x => x.FleetsID.ToString() == Id
                ).Select(f => new

                {
                    MinFuelStandard = f.MinFuelStandard,

                    MaxFuelStandard = f.MaxFuelStandard,
                    SubCategory = f.EquipmentType.EquipmentTypeName,
                    Category = f.EquipmentIdentity.EquipmentCategoryName,
                    Capacity = f.Capacity,
                    ChasisNumber = GetAssetDependancies(f, assetRegistrations).ChasisNumber,
                    Color = f.Color,
                    Remark = f.CurentOdoMeter + " " + Enum.GetName(typeof(ReadingType), f.ReadingType),
                    CurentOdoMeter = f.CurentOdoMeter + " " + Enum.GetName(typeof(ReadingType), f.ReadingType),
                    DepreciationCost = f.DepreciationCost,
                    Attachment = f.Attachment,
                    EngineNumber = f.EngineNumber,
                    EquipmentName = GetAssetDependancies(f, assetRegistrations).EquipmentNameID,
                    EquipmentIdentityID = f.EquipmentIdentityID,
                    EquipmentTypeID = f.EquipmentTypeID,
                    FleetPhoto = f.FleetPhoto,
                    FleetsID = f.FleetsID,
                    Location = GetAssetDependancies(f, assetRegistrations).Location,
                    LastMaintained = f.LastMaintained,
                    Libre = f.Libre,
                    Manufacturer = f.Manufacturer,
                    ManufacturerYear = f.ManufacturerYear,
                    Model = f.Model,
                    PlateNumber = GetAssetDependancies(f, assetRegistrations).PlateNumber,
                    PreviousOdoMeter = f.PreviousOdoMeter,
                    PlannerID = f.PlannerID,
                    PurchasedDate = f.PurchasedDate,
                    PurchasingCost = f.PurchasingCost,
                    RentalRate = f.RentalRate,
                    RentType = f.RentType,
                    RentTypeName = f.RentType.ToString(),
                    SerialNo = GetAssetDependancies(f, assetRegistrations).SerialNo,
                    StateOfOdoMeter = f.StateOfOdoMeter,
                    Status = f.Status,
                    HistoricalCost = f.HistoricalCost,
                    SupplierCompany = f.SupplierCompany,
                    Type = f.Type,
                    TypeOfFuel = f.TypeOfFuel,
                    TyreSize = f.TyreSize,
                    UnitNum = f.UnitNum,
                    WorkingCost = GetWorkingCost(f.FleetsID),
                    IdealCost = GetIdealCost(f.FleetsID),


                    //SpareRepairCost = fleetCostHelper.GetEquipmentSpareCost(f, inventoryMapping, SpareType.Spare),
                    //TyreRepairCost = fleetCostHelper.GetEquipmentSpareCost(f, inventoryMapping, SpareType.Tyre),
                    //OilRepairCost = fleetCostHelper.GetEquipmentSpareCost(f, inventoryMapping, SpareType.Oil_Lubricant),
                    //OtherRepairCost = fleetCostHelper.GetEquipmentSpareCost(f, inventoryMapping, SpareType.Other),
                    //SalvageCost = fleetCostHelper.GetSalvageSpareCost(f),
                    //BattreyCost = fleetCostHelper.GetEquipmentSpareCost(f, inventoryMapping, SpareType.Battery),
                    InsuranceCost = GetInsuranceCost(f.FleetsID),
                    BatteryRepairCost = GetBatterySpareCost(f.FleetsID),
                    FuelConsumptionLevel = GetFuelConsumptionLevel(f.FleetsID),
                    FuelConsumptionRate = GetFuelConsumptionRate(f.FleetsID),
                    ManPowerCost = EquipmentManPowerCost(f.FleetsID),
                    FuelIssue = GetFuelConsumption(f.FleetsID).ToString(),
                    OperatorName = f.OperatorName,
                    AnnualDepreciationCost = f.AnnualDepreciationCost,
                    WayOfSupply = f.WayOfSupply,
                    IdealRentalRate = f.IdealRentalRate,
                    EngineManufacturer = f.EngineManufacturer,
                    EngineModel = f.EngineModel,
                    IsSendForApproval = f.IsSendForApproval,
                    IsOnlineApproved = f.IsOnlineApproved,
                    DateCreated = f.DateCreated,
                    Condition = GetAssetDependancies(f, assetRegistrations).Condition

                }

                );


            return Json(fleets, JsonRequestBehavior.AllowGet);


        }

        public ActionResult SendForApprovalFleets_Read([DataSourceRequest] DataSourceRequest request)
        {

            FleetCostHelper FleetCostHelper = new FleetCostHelper();

            var fleets = _FleetsService.GetAll().Where(x => x.IsSendForApproval == true && x.IsOnlineApproved == false
                ).Select(f => new
                {
                    MinFuelStandard = f.MinFuelStandard,

                    MaxFuelStandard = f.MaxFuelStandard,
                    SubCategory = f.EquipmentType.EquipmentTypeName,
                    Category = f.EquipmentIdentity.EquipmentCategoryName,
                    Capacity = f.Capacity,
                    ChasisNumber = f.ChasisNumber,
                    Color = f.Color,
                    PettyCash = FleetCostHelper.GetPettyCashByFleet(f.FleetsID),
                    Remark = f.CurentOdoMeter + " " + Enum.GetName(typeof(ReadingType), f.ReadingType),
                    CurentOdoMeter = f.CurentOdoMeter + " " + Enum.GetName(typeof(ReadingType), f.ReadingType),
                    DepreciationCost = f.DepreciationCost,
                    Attachment = f.Attachment,
                    EngineNumber = f.EngineNumber,
                    EquipmentName = f.EquipmentNameID,
                    EquipmentIdentityID = f.EquipmentIdentityID,
                    EquipmentTypeID = f.EquipmentTypeID,
                    FleetPhoto = f.FleetPhoto,
                    FleetsID = f.FleetsID,
                    Location = f.Location,
                    LastMaintained = f.LastMaintained,
                    Libre = f.Libre,
                    Manufacturer = f.Manufacturer,
                    ManufacturerYear = f.ManufacturerYear,
                    Model = f.Model,
                    PlateNumber = f.PlateNumber,
                    PreviousOdoMeter = f.PreviousOdoMeter,
                    PlannerID = f.PlannerID,
                    PurchasedDate = f.PurchasedDate,
                    PurchasingCost = f.PurchasingCost,
                    RentalRate = f.RentalRate,
                    RentType = f.RentType,
                    RentTypeName = f.RentType.ToString(),
                    SerialNo = f.SerialNo,
                    StateOfOdoMeter = f.StateOfOdoMeter,
                    Status = f.Status,
                    HistoricalCost = f.HistoricalCost,
                    SupplierCompany = f.SupplierCompany,
                    Type = f.Type,
                    TypeOfFuel = f.TypeOfFuel,
                    TyreSize = f.TyreSize,
                    UnitNum = f.UnitNum,
                    //WorkingCost = GetWorkingCost(f.FleetsID),
                    //IdealCost = GetIdealCost(f.FleetsID),
                    //SpareRepairCost = GetSpareCost(f.FleetsID),
                    //TyreRepairCost = GetTyreSpareCost(f.FleetsID),
                    //OtherRepairCost = GetOtherSpareCost(f.FleetsID),
                    //OilRepairCost = GetOilSpareCost(f.FleetsID),
                    //InsuranceCost = GetInsuranceCost(f.FleetsID),
                    SalvageCost = 0,
                    //BatteryRepairCost = GetBatterySpareCost(f.FleetsID),
                    //FuelConsumptionLevel = GetFuelConsumptionLevel(f.FleetsID),
                    //FuelConsumptionRate = GetFuelConsumptionRate(f.FleetsID),
                    OperatorName = f.OperatorName,
                    AnnualDepreciationCost = f.AnnualDepreciationCost,
                    WayOfSupply = f.WayOfSupply,
                    IdealRentalRate = f.IdealRentalRate,
                    EngineManufacturer = f.EngineManufacturer,
                    EngineModel = f.EngineModel,
                    IsSendForApproval = f.IsSendForApproval,
                    IsOnlineApproved = false,
                    DateCreated = f.DateCreated
                }
                );
            return Json(fleets.ToDataSourceResult(request));
        }
        public async Task<bool> Fleets_DataUpgrade()
        {
            var assetRegistrations = db.AssetRegistrations.ToList();
            var branches = db.Branch.ToList();

            var fleets = _FleetsService.GetAll().Where(x => x.IsExternal == false
                ).ToList();
            foreach (var f in fleets)
            {
                var Asset = assetRegistrations.Where(x => x.Tag == f.UnitNum || x.AssetRegistrationId.ToString() == f.Attachment).FirstOrDefault();
                if (Asset != null)
                {
                    var fleet = GetAssetBranchandCondition(f, assetRegistrations, branches);
                    f.ChasisNumber = Asset.ChassisNo;
                    f.EngineNumber = Asset.EngineNo;
                    f.EquipmentNameID = (Guid)db.EquipmentName.Where(q => q.FixedAssetSubCategoryNameId == Asset.FixedAssetNameID).FirstOrDefault()?.EquipmentNameID;
                    f.Location = fleet.Location;
                    f.IFRSCondition = fleet.IFRSCondition;
                    f.Manufacturer = Asset.Manufacturer;
                    f.PurchasingCost = Asset.PurchasePrice;
                    f.Model = Asset.ModelNo;
                    f.PlateNumber = Asset.PlateNo;
                    f.SerialNo = Asset.SerialNo;
                    f.Attachment = Asset.AssetRegistrationId.ToString();
                    await _FleetsService.UpdateAsync(f);
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Fleets Upadet", "Success", User.Identity.Name, $"updated asset reg {f.FleetsID} Tag {f.EquipmentName}", Modules.FleetManagement);
                    #endregion

                }
            }
            return true;
        }
        public ActionResult EquipmentCheckList_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {

            var checkList = db.EquipmentCheckLists.Where(x => x.FleetsID == Id);

            DataSourceResult result = checkList.ToDataSourceResult(request, equipmentCheckList => new
            {
                FleetsID = equipmentCheckList.FleetsID,
                EquipmentCheckListId = equipmentCheckList.EquipmentCheckListId,
                EquipmentLineCheckListId = equipmentCheckList.EquipmentLineCheckListId,
                Code = equipmentCheckList.Code,
                Name = equipmentCheckList.Name,
                Quantity = equipmentCheckList.Quantity,
                Description = equipmentCheckList.Description,
            });
            return Json(result);
        }
        public ActionResult EquipmentCheckList_Update([DataSourceRequest] DataSourceRequest request, EquipmentCheckList equipmentCheckList)
        {
            if (ModelState.IsValid)
            {
                var entity = new EquipmentCheckList
                {
                    FleetsID = equipmentCheckList.FleetsID,
                    EquipmentCheckListId = equipmentCheckList.EquipmentCheckListId,
                    EquipmentLineCheckListId = equipmentCheckList.EquipmentLineCheckListId,
                    Name = equipmentCheckList.Name,
                    Description = equipmentCheckList.Description,
                    Code = equipmentCheckList.Code,
                    Quantity = equipmentCheckList.Quantity,
                };

                db.EquipmentCheckLists.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance update", "Success", User.Identity.Name, $"updated insurance registration number {entity.EquipmentCheckListId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { equipmentCheckList }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult RentalInspectionCriteriaDetail_Destroy([DataSourceRequest] DataSourceRequest request, EquipmentCheckList equipmentCheckList)
        {
            if (ModelState.IsValid)
            {
                var entity = new EquipmentCheckList
                {
                    FleetsID = equipmentCheckList.FleetsID,
                    EquipmentCheckListId = equipmentCheckList.EquipmentCheckListId,
                    EquipmentLineCheckListId = equipmentCheckList.EquipmentLineCheckListId,
                    Name = equipmentCheckList.Name,
                    Description = equipmentCheckList.Description,
                    Code = equipmentCheckList.Code,
                    Quantity = equipmentCheckList.Quantity,
                };

                db.EquipmentCheckLists.Attach(entity);
                db.EquipmentCheckLists.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted insurance registration number {entity.EquipmentCheckListId }");
                #endregion
            }

            return Json(new[] { equipmentCheckList }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RentalFleets_Read([DataSourceRequest] DataSourceRequest request)
        {
            FleetCostHelper FleetCostHelper = new FleetCostHelper();
            
            var branches = db.Branch.ToList();
            bool isHeadBranch = false;

            var currentBranch = branches.Where(x => x.BranchId.ToString() == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).FirstOrDefault();

            if (currentBranch != null)
            {
                if (currentBranch.BranchTypes == BranchTypes.HEAD_OFFICE)
                {
                    isHeadBranch = true;
                }
            }
            
            var fleets = db.Fleets.Where(x => x.IsExternal).ToList().Select(f => new
            {
                MinFuelStandard = f.MinFuelStandard,
                LastModified = f.LastModified,
                MaxFuelStandard = f.MaxFuelStandard,
                SubCategory = f.EquipmentType.EquipmentTypeName,
                Category = f.EquipmentIdentity.EquipmentCategoryName,
                Capacity = f.Capacity,
                ChasisNumber = f.SerialNo,
                Color = f.Color,
                PettyCash = 0,
                Remark = f.CurentOdoMeter + " " + Enum.GetName(typeof(ReadingType), f.ReadingType),
                CurentOdoMeter = f.CurentOdoMeter + " " + Enum.GetName(typeof(ReadingType), f.ReadingType),
                DepreciationCost = f.DepreciationCost,
                Attachment = f.Attachment,
                EngineNumber = f.EngineNumber,
                EquipmentNameID = f.EquipmentNameID,
                EquipmentIdentityID = f.EquipmentIdentityID,
                EquipmentTypeID = f.EquipmentTypeID,
                FleetPhoto = f.FleetPhoto,
                FleetsID = f.FleetsID,
                Location = f.Location,
                IFRSCondition = f.IFRSCondition,
                LastMaintained = f.LastMaintained,
                Libre = f.Libre,
                Manufacturer = f.Manufacturer,
                ManufacturerYear = f.ManufacturerYear,
                Model = f.Model,
                PlateNumber = f.PlateNumber,
                PreviousOdoMeter = f.PreviousOdoMeter,
                PlannerID = f.PlannerID,
                PurchasedDate = f.PurchasedDate,
                PurchasingCost = f.PurchasingCost,
                RentalRate = f.RentalRate,
                RentType = f.RentType,
                RentTypeName = f.RentType.ToString(),
                SerialNo = f.SerialNo,
                StateOfOdoMeter = f.StateOfOdoMeter,
                Status = f.Status,
                HistoricalCost = f.HistoricalCost,
                SupplierCompany = f.SupplierCompany,
                Type = f.Type,
                TypeOfFuel = f.TypeOfFuel,
                TyreSize = f.TyreSize,
                UnitNum = f.UnitNum,
                ApprovalStatus = f.GetApprovalStatus(),
                SalvageCost = 0,
                OperatorName = f.OperatorName.DefaultValue(),
                AnnualDepreciationCost = f.AnnualDepreciationCost,
                WayOfSupply = f.WayOfSupply,
                IdealRentalRate = f.IdealRentalRate,
                EngineManufacturer = f.EngineManufacturer,
                EngineModel = f.EngineModel,
                IsSendForApproval = f.IsSendForApproval,
                IsOnlineApproved = f.IsOnlineApproved,
                DateCreated = f.DateCreated,
                AssetId = f.AssetId,
                RentFromExternalNum = f.RentFromExternalNum

            }

             ).ToList();

            
            return Json(fleets.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            
        }
        public ActionResult UpdateStatus(Fleets model)
        {
            IFleetService _FleetService = new FleetService();
            var Fleet = _FleetService.GetById(model.FleetsID);
            if (Fleet != null)
            {
                Fleet.Status = model.Status;
                _FleetService.Update(Fleet);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Update Status", "Success", User.Identity.Name, $"updated status reg {Fleet.FleetsID} Tag {Fleet.EquipmentName}", Modules.FixedAsset);
                #endregion
            }
            return RedirectToAction("Index");
        }
        [AuthorizeRoles(FleetManagementRoles.RegistrationUser)]
        public ActionResult Fleets_Create([DataSourceRequest] DataSourceRequest request, Fleets fleets, EquipmentCheckList equipmentCheckList)
        {
            ModelState.Remove("FleetsID");
            ModelState.Clear();
            if (fleets.AssetId == null)
                ModelState.AddModelError("AssetNull", "Please select asset !");
            if (fleets.UnitNum == null)
                ModelState.AddModelError("AssetCodeNull", "Please select asset code !");
            IEquipmentNameService _equipmentNameService = new EquipmentNameService();
            IFleetService _FleetService = new FleetService();
            AssetRegistration Asset = db.AssetRegistrations.Where(x => x.AssetRegistrationId == fleets.AssetId || x.AssetRegistrationId.ToString() == fleets.SerialNo || x.AssetRegistrationId.ToString() == fleets.PlateNumber).FirstOrDefault();

            List<Fleets> fleetsPlate = _FleetService.GetAll().Where(f => f.UnitNum == Asset?.Tag).ToList();
            if (fleetsPlate.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingPlateNumber", " Equipment with Asset No. " + Asset.Tag + " Already Exists !");

                return Json(new[] { fleets }.ToDataSourceResult(request, ModelState));
            }

            else
            {


                if (fleets != null)
                {
                    fleets.AssetId = Asset.AssetRegistrationId;
                    fleets.FleetsID = Guid.NewGuid();
                    fleets.LastMaintained = DateTime.Now;
                    fleets.PreviousOdoMeter = fleets.CurentOdoMeter;
                    fleets.Status = MaintenanceConst.ACTIVE;
                    fleets.ModifiedBy = User.Identity.Name;
                    fleets.LastModified = DateTime.Now;
                    fleets.Battery = Asset.Battery?.DefaultValue();
                    fleets.ChasisNumber = Asset?.MotorNo.DefaultValue();
                    fleets.EngineNumber = Asset?.EngineNo.DefaultValue();
                    fleets.EngineModel = Asset?.MotorNo.DefaultValue();
                    fleets.Manufacturer = Asset?.Manufacturer.DefaultValue();
                    fleets.ManufacturerYear = Asset?.ManufacturDate.ToString();
                    fleets.Location = Asset?.Location.DefaultValue();
                    fleets.Capacity = Asset?.LoadCapacity.DefaultValue();
                    fleets.PlateNumber = Asset?.PlateNo.DefaultValue();
                    fleets.PurchasedDate = Asset?.PurchaseDate;
                    fleets.PurchasingCost = (decimal)Asset?.PurchasePrice;
                    fleets.Attachment = Asset?.AssetRegistrationId.ToString();
                    fleets.SerialNo = Asset?.SerialNo.DefaultValue();
                    fleets.TyreSize = fleets?.TyreSize.DefaultValue();
                    fleets.TypeOfFuel = Asset?.FuelType.DefaultValue();
                    fleets.Tyre = Asset?.Tyre.DefaultValue();
                    fleets.Model = Asset?.ModelNo.DefaultValue();
                    fleets.SupplierCompany = Asset?.Supplier.DefaultValue();
                    fleets.HistoricalCost = Asset?.PurchasePrice.ToString();
                    fleets.DepreciationCost = (int)(Asset?.PurchasePrice - Asset?.BookValue);
                    fleets.PurchasedDate = Asset?.PurchaseDate;
                    fleets.RentType = fleets.RentType;
                    fleets.IdealRentalRate = fleets.IdealRentalRate;
                    fleets.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    fleets.Condition = Asset.Condition.Name;
                    fleets.OperatorName = MaintenanceConst.UnAssigned;
                    fleets.UnitNum = Asset.Tag.DefaultValue();
                    _FleetsService.Insert(fleets);


                    if (!string.IsNullOrEmpty(fleets.Condition) || !string.IsNullOrEmpty(fleets.Location))
                    {
                        EquipmentLocation location = new EquipmentLocation
                        {
                            EquipmentLocationId = Guid.NewGuid(),
                            FleetId = fleets.FleetsID,
                            Name = fleets.Location,
                            Odometer = fleets.CurentOdoMeter,
                            Condition = fleets.Condition,
                            DateCreated = DateTime.Now

                        };
                        db.EquipmentLocations.Add(location);
                        db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Fleet registration", "Success", User.Identity.Name, $"created fleet {location.FleetId} Tag {location.Name}", Modules.FleetManagement);
                        #endregion
                        var fleetCheckListIds = db.FleetCheckLists.Where(x => x.EquipmentIdentityID == fleets.EquipmentIdentityID && x.EquipmentTypeID == fleets.EquipmentTypeID).Select(x => x.FleetCheckListId).ToList();

                        var fleetLineCheckLists = db.FleetLineCheckLists.Where(x => fleetCheckListIds.Contains(x.FleetCheckListId)).ToList();
                        foreach (var fleetLineCheckList in fleetLineCheckLists)
                        {
                            var checkList = new EquipmentCheckList
                            {
                                FleetsID = fleets.FleetsID,
                                EquipmentLineCheckListId = fleetLineCheckList.FleetLineCheckListId,
                                Name = fleetLineCheckList.SystemName,
                                Code = fleetLineCheckList.Code,
                                Description = fleetLineCheckList.Description,
                                Quantity = 0
                            };
                            db.EquipmentCheckLists.Add(checkList);
                            db.SaveChanges();
                        }



                    }

                    EquipmentName _equipmentName = _equipmentNameService.GetAll().Where(x => x.EquipmentNameID == fleets.EquipmentNameID).FirstOrDefault();
                    if (_equipmentName != null)
                    {
                        _equipmentName.IsAssigned = true;
                        _equipmentNameService.Update(_equipmentName);
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Fleet registration", "Success", User.Identity.Name, $"created fleet {_equipmentName.EquipmentNameID} Tag {_equipmentName.Name}", Modules.FixedAsset);
                        #endregion
                    }


                }
            }


            return Json(new[] { fleets }.ToDataSourceResult(request, ModelState));
        }


        //public void OnAssetDelete(this AssetRegistration Asset )
        //{
        //    IFleetService _FleetService = new FleetService();

        //    var fleet = _FleetService.GetAll().Where(f => f.UnitNum == Asset.Tag || f.PlateNumber == Asset.PlateNo || f.SerialNo == Asset.SerialNo || f.ChasisNumber == Asset.ChassisNo).FirstOrDefault();

        //    if (fleet != null)
        //    {
        //        _FleetsService.Delete(fleet);
        //    }
        //}

        public void UpdateFleet(string TagNo, string PlateNo, string ChasisNo, string SerialNo)
        {
            IFleetService _FleetService = new FleetService();


            var fleet = _FleetService.GetAll().Where(f => f.UnitNum == TagNo || f.PlateNumber == PlateNo || f.SerialNo == SerialNo || f.ChasisNumber == ChasisNo).FirstOrDefault();

            if (fleet != null)
            {
                fleet.PlateNumber = PlateNo;
                fleet.UnitNum = TagNo;
                fleet.ChasisNumber = ChasisNo;
                fleet.SerialNo = SerialNo;
                _FleetsService.Update(fleet);

            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Fleets_Update([DataSourceRequest] DataSourceRequest request, Fleets fleets)
        {
            IEquipmentNameService _equipmentNameService = new EquipmentNameService();
            IFleetService _FleetService = new FleetService();

            AssetRegistration Asset = db.AssetRegistrations.Where(x => x.AssetRegistrationId.ToString() == fleets.UnitNum || x.AssetRegistrationId.ToString() == fleets.SerialNo || x.AssetRegistrationId.ToString() == fleets.PlateNumber).FirstOrDefault();

            if (Asset != null)
            {
                List<Fleets> fleetsPlate = _FleetService.GetAll().Where(f => f.UnitNum == Asset.Tag && f.FleetsID != fleets.FleetsID).ToList();
                if (fleetsPlate.Any())
                {
                    ModelState.Clear();
                    ModelState.AddModelError("ExistingPlateNumber", " Equipment with Asset No. " + Asset.Tag + " Already Exists !");

                    return Json(new[] { fleets }.ToDataSourceResult(request, ModelState));
                }
                if (fleets.UnitNum == null)
                    ModelState.AddModelError("AssetCodeNull", "Please select asset code !");
                if (fleetsPlate.Any())
                {
                    ModelState.Clear();
                    ModelState.AddModelError("ExistingPlateNumber", " Equipment Plate Number Already Exists !");
                    return Json(new[] { fleets }.ToDataSourceResult(request, ModelState));
                }
                else
                {
                    FleetHistory fH = new FleetHistory
                    {
                        FleetHistoryID = Guid.NewGuid(),
                        FleetsID = fleets.FleetsID,
                        FuelStandard = fleets.MinFuelStandard.ToString(),
                        RentalRate = fleets.RentalRate,
                        AnnualDepreciationCost = fleets.AnnualDepreciationCost
                    };
                    if (fleets != null)
                    {
                        fleets.AssetId = Asset.AssetRegistrationId;
                        fleets.LastMaintained = DateTime.Now;
                        fleets.PreviousOdoMeter = fleets.CurentOdoMeter;
                        fleets.Status = MaintenanceConst.ACTIVE;
                        fleets.ModifiedBy = User.Identity.Name;
                        fleets.LastModified = DateTime.Now;
                        fleets.Battery = Asset.Battery;
                        fleets.ChasisNumber = Asset.MotorNo;
                        fleets.EngineNumber = Asset.EngineNo;
                        fleets.EngineModel = Asset.MotorNo;
                        fleets.Manufacturer = Asset.Manufacturer;
                        fleets.ManufacturerYear = Asset.ManufacturDate.ToString();
                        fleets.Location = Asset.Location;
                        fleets.Capacity = Asset.LoadCapacity;
                        fleets.PlateNumber = Asset.PlateNo;
                        fleets.PurchasedDate = DateTime.Now;
                        fleets.PurchasingCost = Asset.PurchasePrice;
                        fleets.SerialNo = Asset.SerialNo;
                        fleets.TyreSize = fleets.TyreSize;
                        fleets.TypeOfFuel = Asset.FuelType;
                        fleets.Tyre = Asset.Tyre;
                        fleets.Model = Asset.ModelNo;
                        fleets.SupplierCompany = Asset.Supplier;
                        fleets.HistoricalCost = Asset.PurchasePrice.ToString();
                        fleets.DepreciationCost = (int)(Asset.PurchasePrice - Asset.BookValue);
                        fleets.PurchasedDate = Asset.PurchaseDate;
                        fleets.RentType = fleets.RentType;
                        fleets.IdealRentalRate = fleets.IdealRentalRate;
                        fleets.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        fleets.Condition = Asset.Condition.Name;
                        fleets.OperatorName = MaintenanceConst.UnAssigned;
                        fleets.UnitNum = Asset.Tag;
                        fleets.ModifiedBy = User.Identity.Name;
                        fleets.LastModified = DateTime.Now;
                        _FleetsService.Update(fleets);
                        _FleetHistoryService.Insert(fH);

                        EquipmentName _equipmentName =
                            _equipmentNameService.GetAll()
                                .Where(x => x.EquipmentNameID == fleets.EquipmentNameID)
                                .FirstOrDefault();
                        if (_equipmentName != null)
                        {
                            _equipmentName.IsAssigned = true;
                            _equipmentNameService.Update(_equipmentName);
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "Fleets upadet", "Success", User.Identity.Name, $"updated fleet {fleets.FleetsID}", Modules.FleetManagement);
                            #endregion
                        }

                    }
                }
            }
            else
            {
                FleetHistory fH = new FleetHistory
                {
                    FleetHistoryID = Guid.NewGuid(),
                    FleetsID = fleets.FleetsID,
                    FuelStandard = fleets.MinFuelStandard.ToString(),
                    RentalRate = fleets.RentalRate,
                    AnnualDepreciationCost = fleets.AnnualDepreciationCost
                };
                if (fleets != null)
                {
                    fleets.LastMaintained = DateTime.Now;
                    fleets.PreviousOdoMeter = fleets.CurentOdoMeter;
                    fleets.Status = MaintenanceConst.ACTIVE;
                    fleets.ModifiedBy = User.Identity.Name;
                    fleets.LastModified = DateTime.Now;
                    fleets.Battery = fleets.Battery;
                    fleets.ChasisNumber = fleets.ChasisNumber;
                    fleets.EngineNumber = fleets.EngineNumber;
                    fleets.EngineModel = fleets.EngineModel;
                    fleets.Manufacturer = fleets.Manufacturer;
                    fleets.ManufacturerYear = fleets.ManufacturerYear;
                    fleets.Location = fleets.Location;
                    fleets.Capacity = fleets.Capacity;
                    fleets.PlateNumber = fleets.PlateNumber;
                    fleets.PurchasedDate = DateTime.Now;
                    fleets.PurchasingCost = fleets.PurchasingCost;
                    fleets.SerialNo = fleets.SerialNo;
                    fleets.TyreSize = fleets.TyreSize;
                    fleets.TypeOfFuel = fleets.TypeOfFuel;
                    fleets.Tyre = fleets.Tyre;
                    fleets.Model = fleets.Model;
                    fleets.SupplierCompany = fleets.SupplierCompany;
                    fleets.HistoricalCost = fleets.HistoricalCost;
                    fleets.DepreciationCost = fleets.DepreciationCost;
                    fleets.PurchasedDate = fleets.PurchasedDate;
                    fleets.RentType = fleets.RentType;
                    fleets.IdealRentalRate = fleets.IdealRentalRate;
                    fleets.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    fleets.Condition = fleets.Condition;
                    fleets.OperatorName = MaintenanceConst.UnAssigned;
                    fleets.UnitNum = fleets.UnitNum;
                    fleets.ModifiedBy = User.Identity.Name;
                    fleets.LastModified = DateTime.Now;
                    _FleetsService.Update(fleets);
                    _FleetHistoryService.Insert(fH);

                    EquipmentName _equipmentName =
                        _equipmentNameService.GetAll()
                            .Where(x => x.EquipmentNameID == fleets.EquipmentNameID)
                            .FirstOrDefault();
                    if (_equipmentName != null)
                    {
                        _equipmentName.IsAssigned = true;
                        _equipmentNameService.Update(_equipmentName);
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Fleets upadet", "Success", User.Identity.Name, $"updated fleet {fleets.FleetsID}", Modules.FleetManagement);
                        #endregion
                    }

                }
            }

            return Json(new[] { fleets }.ToDataSourceResult(request));
        }


        public ActionResult Fleets_Destroy([DataSourceRequest] DataSourceRequest request, Fleets fleets)
        {
            var fleet = _FleetsService.GetById(fleets.FleetsID);
            if (fleet != null)
            {

                _FleetsService.Delete(fleet);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment " + fleet.FleetsID);

            }

            return Json(new[] { fleets }.ToDataSourceResult(request));
        }

        public string GetFuelConsumptionRate(Guid Id)
        {
            try
            {
                IFleetService _FleetsService = new FleetService();
                ITimeSheetService _TimeSheetService = new TimeSheetService();
                decimal Rate = 0;
                decimal Issued = 0;
                var AlertLevels = db.FuelConsumptionAlertLevel.ToList();
                var Fleet = _FleetsService.GetById(Id);
                decimal TotalReading = decimal.Zero;
                if (Fleet != null)
                {
                    Issued =
                      db.IssueItems.Where(x => x.Issue.IsFuel == true && x.Issue.PlateNo == Fleet.PlateNumber).Sum(x => x.ApprovedQuantity);
                    var Sheet = _TimeSheetService.GetAll().Where(x => x.FleetsID == Fleet.FleetsID);
                    if (Fleet.ReadingType == ReadingType.HR)
                    {
                        foreach (var Work in Sheet)
                        {
                            TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) - DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                            TimeSpan AfterNoonDuration = DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());

                            TotalReading = TotalReading + (MorningDuration.Hours + AfterNoonDuration.Hours);

                        }
                    }
                    else
                    {
                        foreach (var Work in Sheet)
                        {

                            TotalReading = TotalReading + (Work.FinalKMHrReading - Work.KMHrReading);

                        }

                    }


                    if (Issued == 0)
                    {
                        Issued = 1;
                    }



                    Rate = (Issued * ((Fleet.MinFuelStandard + Fleet.MaxFuelStandard) / 2) / FleetGenericHelper.DivideByZero(TotalReading)) * 100;

                }

                var Level = AlertLevels.Where(x => x.FromValue > Rate && x.ToValue < Rate).FirstOrDefault();

                if (Level != null)
                {
                    return Rate + "_(" + Rate + "%) " + Level.Name;

                }

                return Rate + "_(" + Rate + "%) Alert Level Not Found ";
            }
            catch (Exception)
            {
                return string.Empty;
            }


        }
        public decimal GetFuelConsumption(Guid Id)
        {

            var Fleet = _FleetsService.GetById(Id);
            if (Fleet != null)
            {
                return db.IssueItems.Where(x => x.Issue.IsFuel == true && x.Issue.PlateNo == Fleet.PlateNumber).Select(x => x.Total).DefaultIfEmpty(0).Sum(x => x);

            }


            return 0;

        }


        public string GetFuelConsumptionLevel(Guid Id)
        {
            try
            {

                decimal Rate = 0;
                decimal Issued = 0;
                var AlertLevels = db.FuelConsumptionAlertLevel.ToList();
                var Fleet = _FleetsService.GetById(Id);
                if (Fleet != null)
                {
                    Issued =
                      db.IssueItems.Where(x => x.Issue.IsFuel == true && x.Issue.PlateNo == Fleet.PlateNumber).Sum(x => x.ApprovedQuantity);



                    if (Issued == 0)
                    {
                        Issued = 1;
                    }

                    if (string.IsNullOrEmpty(Fleet.CurentOdoMeter.ToString()))
                    {
                        Fleet.CurentOdoMeter = 0;
                    }

                    Rate = (Issued * ((Fleet.MinFuelStandard + Fleet.MaxFuelStandard) / 2) / (decimal.Parse(Fleet.CurentOdoMeter.ToString()))) * 100;

                }

                var Level = AlertLevels.Where(x => x.FromValue > Rate && x.ToValue < Rate).FirstOrDefault();

                if (Level != null)
                {
                    return Level.Name;
                }


                return " Alert Level Not Found ";
            }
            catch (Exception)
            {
                return string.Empty;
            }



        }
        public string GetInsuranceCost(Guid Id)
        {

            decimal cost = 0;
            try
            {


                cost = db.FleetInsurance.Where(x => x.FleetsID == Id).ToList().Sum(x => x.PremiumCost);

            }
            catch (Exception)
            {

                return string.Empty;
            }
            return cost.ToString();
        }
        public decimal GetWorkingCost(Guid Id)
        {
            decimal cost = 0;
            var Working = db.TimeSheet.Where(x => x.FleetsID == Id).ToList();
            foreach (var Work in Working)
            {
                TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) - DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                TimeSpan AfterNoonDuration = DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());

                cost = cost + ((MorningDuration.Hours + AfterNoonDuration.Hours) * decimal.Parse(Work.RentalRate.ToString()));
            }
            return cost;
        }

        public decimal GetIdealCost(Guid Id)
        {

            decimal cost = 0;
            try
            {


                var Working = db.TimeSheet.Where(x => x.FleetsID == Id).ToList();
                foreach (var Work in Working)
                {
                    int IdealTime = Work.DownTime + Work.IdleTime + Work.BreakDownTime;

                    cost = cost + (IdealTime * decimal.Parse(Work.IdealRentalRate.ToString()));
                }
            }
            catch (Exception)
            {

                return 0;
            }
            return cost;
        }

        public decimal GetJobCost(Guid Id)
        {
            decimal Cost = 0;
            IJobOrderService jobOrderService = new JobOrderService();
            FleetCostHelper fleetCostHelper = new FleetCostHelper();
            var JobOrder = jobOrderService.GetAll().Where(x => x.FleetsID == Id).ToList();


            var SpareMap = db.InventorySparePartMapping.ToList();

            Cost = fleetCostHelper.GetEquipmentTotalServiceCost(JobOrder, SpareMap);

            return Cost;

        }


        public decimal GetBatterySpareCost(Guid Id)
        {
            decimal Cost = 0;

            var JobOrder = _JobOrderService.GetAll().Where(x => x.FleetsID == Id).ToList();


            var SpareMap = db.InventorySparePartMapping.ToList();

            Cost = FleetCostHelper.GetEquipmentSpareCost(JobOrder, SpareMap, SpareType.Battery);

            return Cost;
        }


        public decimal GetOilSpareCost(Guid Id)
        {
            decimal Cost = 0;

            var JobOrder = _JobOrderService.GetAll().Where(x => x.FleetsID == Id).ToList();




            var SpareMap = db.InventorySparePartMapping.ToList();

            Cost = FleetCostHelper.GetEquipmentSpareCost(JobOrder, SpareMap, SpareType.Oil_Lubricant);

            return Cost;
        }





        
        public ActionResult FleetsDDBind()
        {
            var assetRegistrations = db.AssetRegistrations.ToList();
            var fleets = _FleetsService.GetAll().Where(x => !x.IsExternal && !x.IsReturned).Select(s => new FleetViewModel
            {
                PlateNumber = GetAssetDependancies(s, assetRegistrations).PlateNumber + " - " + s.UnitNum,
                FleetsID = s.FleetsID
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FleetsTyreReadingDDBind()
        {
            var returnBattery = db.EquipmentBattery.Select(x => new
            {
                x.EquipmentBatteryId,
                x.SerialNo
            }).ToList();
            //var batt = _FleetsService.GetAll().Where(x => !x.IsExternal && !x.IsReturned).Select(s => new 
            //{
            //    //PlateNumber = GetAssetDependancies(s, assetRegistrations).PlateNumber + " - " + s.UnitNum,
            //   s.AssetId,
            //   s.EquipmentName
            //});

            return Json(returnBattery, JsonRequestBehavior.AllowGet);
        }

        //
        public ActionResult EquipmentNameDDBind(string Search, string EquipmentTypeId)
        {
            var fleets = new List<FleetViewModel>();
            if (string.IsNullOrWhiteSpace(EquipmentTypeId))
            {
                IEquipmentNameService _EquipmentNameService = new EquipmentNameService();
                fleets = _EquipmentNameService.GetAll().Select(s => new FleetViewModel
                {
                    EquipmentName = s.Name

                }).ToList();
            }
            else
            {
                fleets = _FleetsService.GetAll().Where(x => x.EquipmentTypeID.ToString() == EquipmentTypeId).DistinctByField(x => x.EquipmentName).Select(s => new FleetViewModel
                {
                    EquipmentName = s.EquipmentName

                }).ToList();

            }


            if (!string.IsNullOrWhiteSpace(Search))
            {

                fleets = fleets.Where(x => x.EquipmentName.ToUpper().Contains(Search.ToUpper())).ToList();

            }

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        //
        //public ActionResult EquipmentNameFilterDDBind(string Search, string EquipmentTypeId)
        //{

        //    var fleets = _FleetsService.GetAll().Where(x => x.EquipmentTypeID.ToString() == EquipmentTypeId).DistinctByField(x => x.EquipmentName).Select(s => new FleetViewModel
        //    {
        //        EquipmentName = s.EquipmentName

        //    });
        //    if (!string.IsNullOrWhiteSpace(Search))
        //    {
        //        fleets = fleets.Where(x => x.EquipmentName.ToUpper().Contains(Search.ToUpper()));
        //    }
        //    return Json(fleets, JsonRequestBehavior.AllowGet);
        //}

        //
        //public ActionResult EquipmentFilterDDBind(string Search, string EquipmentTypeId)
        //{

        //    var fleets = _FleetsService.GetAll().Where(x => x.EquipmentTypeID.ToString() == EquipmentTypeId).DistinctByField(x => x.EquipmentName).Select(s => new FleetViewModel
        //    {
        //        EquipmentName = s.EquipmentName

        //    });
        //    if (!string.IsNullOrWhiteSpace(Search))
        //    {
        //        fleets = fleets.Where(x => x.EquipmentName.ToUpper().Contains(Search.ToUpper()));
        //    }
        //    return Json(fleets, JsonRequestBehavior.AllowGet);
        //}




        //
        //public ActionResult EquipmentDDBind(string EquipmentTypeId, string Search)
        //{

        //    var fleets = _FleetsService.GetAll().Where(x =>  x.EquipmentTypeID.ToString() == EquipmentTypeId).Select(s => new FleetViewModel
        //    {
        //        PlateNumber = s.EquipmentName + " _ "+s.PlateNumber + " _ " + s.UnitNum,
        //        FleetsID = s.FleetsID,
        //        UnitNum = s.UnitNum,
        //        EquipmentName = s.PlateNumber,
        //    });
        //    if (!string.IsNullOrWhiteSpace(Search))
        //    {
        //        fleets = fleets.Where(x => x.PlateNumber.ToUpper().Contains(Search.ToUpper())).ToList();
        //    }
        //    return Json(fleets, JsonRequestBehavior.AllowGet);
        //}
        //
        //public ActionResult AssignedEquipmentDDBind()
        //{

        //    var fleets = _FleetsService.GetAll().Where(x => x.OperatorName != MaintenanceConst.UnAssigned && x.IsOnlineApproved == true).Select(s => new FleetViewModel
        //    {
        //        PlateNumber = s.PlateNumber + " - " + s.UnitNum,

        //        FleetsID = s.FleetsID
        //    });

        //    foreach (var fleet in fleets)
        //    {
        //        fleet.PlateNumber = GetAssetDependancies(fleet).PlateNumber + " - " + fleet.UnitNum;
        //    }

        //    return Json(fleets, JsonRequestBehavior.AllowGet);
        //}



        //
        //public ActionResult FixedAssetPlateDDBind(string Id, string Search)
        //{

        //    var fleets = db.AssetRegistrations.Where(x => x.IsMachinery == true && x.FixedAssetSubCategoryID.ToString() == Id).Select(s => new FleetViewModel
        //    {
        //        EquipmentName = s.PlateNo,
        //        PlateNumber = s.AssetRegistrationId.ToString()
        //    }).ToList();
        //    if (!string.IsNullOrWhiteSpace(Search))
        //    {
        //        fleets = fleets.Where(x => x.EquipmentName.ToUpper().Contains(Search.ToUpper())).ToList();
        //    }
        //    return Json(fleets, JsonRequestBehavior.AllowGet);

        //}
        //
        //public ActionResult EquipmentByNameDDBind(string Id, string Search)
        //{

        //    var fleets = db.Fleets.Where(x => x.EquipmentName == Id && x.IsReturned == false && x.IsOnlineApproved).Select(s => new FleetViewModel
        //    {
        //        EquipmentName = s.UnitNum + " - " + s.PlateNumber,
        //        PlateNumber = s.FleetsID.ToString(),
        //        FleetsID = s.FleetsID
        //    }).ToList();

        //    foreach (var fleet in fleets)
        //    {
        //        fleet.PlateNumber = GetAssetDependancies(fleet).PlateNumber + " - " + fleet.UnitNum;
        //    }

        //    if (!string.IsNullOrWhiteSpace(Search))
        //    {
        //        fleets = fleets.Where(x => x.EquipmentName.ToUpper().Contains(Search.ToUpper())).ToList();
        //    }
        //    return Json(fleets, JsonRequestBehavior.AllowGet);
        //}


        //
        //public ActionResult FixedAssetPlateSearchDDBind(string Id, string search)
        //{

        //    var fleets = db.AssetRegistrations.Where(x => x.IsMachinery == true && x.FixedAssetSubCategoryID.ToString() == Id).Select(s => new FleetViewModel
        //    {
        //        EquipmentName = s.PlateNo,
        //        PlateNumber = s.AssetRegistrationId.ToString()
        //    }).ToList();
        //    fleets = fleets.Where(x => x.PlateNumber.ToUpper().Contains(search.ToUpper())).ToList();


        //    return Json(fleets, JsonRequestBehavior.AllowGet);

        //}
        //
        //public ActionResult FixedAssetChasisDDBind(string Id, string Search)
        //{

        //    var fleets = db.AssetRegistrations.Where(x => x.IsMachinery == true && x.FixedAssetSubCategoryID.ToString() == Id).Select(s => new FleetViewModel
        //    {
        //        EquipmentName = s.ChassisNo,
        //        PlateNumber = s.AssetRegistrationId.ToString()
        //    }).ToList();
        //    if (!string.IsNullOrWhiteSpace(Search))
        //    {
        //        fleets = fleets.Where(x => x.EquipmentName.ToUpper().Contains(Search.ToUpper())).ToList();
        //    }
        //    return Json(fleets, JsonRequestBehavior.AllowGet);

        //}


        //
        //public ActionResult FixedAssetSerialNoDDBind(string Id, string Search)
        //{
        //    var fleets = db.AssetRegistrations.Where(x => x.IsMachinery == true && x.FixedAssetSubCategoryID.ToString() == Id).Select(s => new FleetViewModel
        //    {
        //        EquipmentName = s.SerialNo,
        //        PlateNumber = s.AssetRegistrationId.ToString()
        //    }).ToList();
        //    if (!string.IsNullOrWhiteSpace(Search))
        //    {
        //        fleets = fleets.Where(x => x.EquipmentName.ToUpper().Contains(Search.ToUpper())).ToList();
        //    }
        //    return Json(fleets, JsonRequestBehavior.AllowGet);
        //}


        //
        //public ActionResult OutOfGarageFleetDDBind()
        //{

        //    var ActiveFleet = db.Fleets.Where(x => x.Status != MaintenanceConst.OnMaintenance && x.IsReturned == false).Select(s => new FleetViewModel
        //    {
        //        FleetsID = s.FleetsID,
        //        UnitNum = s.UnitNum,
        //        EquipmentName = s.PlateNumber + " - " + s.UnitNum,

        //        PlateNumber = s.PlateNumber + " - " + s.UnitNum
        //    }).ToList();


        //    return Json(ActiveFleet, JsonRequestBehavior.AllowGet);
        //}
        //
        //public ActionResult FixedNameBySubType(string Id, string search)
        //{

        //    var EquipmentType = db.EquipmentType.Where(x => x.EquipmentTypeID.ToString() == Id).FirstOrDefault();
        //    if (EquipmentType != null)
        //    {
        //        var FixedAssetCategory = db.FixedAssetCategories.Where(x => x.FixedAssetCategoryID == EquipmentType.FixedAssetCategoryID).FirstOrDefault();
        //        if (FixedAssetCategory != null)
        //        {
        //            var FixedAssets = db.FixedAssetSubCategories.Where(x => x.CategoryCode == FixedAssetCategory.CategoryCode).ToList();
        //            if (FixedAssets.Any())
        //            {
        //                if (!string.IsNullOrWhiteSpace(search))
        //                {
        //                    FixedAssets = FixedAssets.Where(x => x.SubCategoryName.ToUpper().Contains(search.ToUpper())).ToList();
        //                }
        //                return Json(FixedAssets.Select(s => new { Name = s.SubCategoryName, Tag = s.FixedAssetSubCategoryID }).ToList(), JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        return Json(string.Empty, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(string.Empty, JsonRequestBehavior.AllowGet);
        //}

        //
        //public ActionResult FixedNameBySubTypeByName(string Id, string Search)
        //{

        //    var EquipmentType = db.EquipmentType.Where(x => x.EquipmentTypeName == Id).FirstOrDefault();
        //    if (EquipmentType != null)
        //    {
        //        var FixedAssetCategory = db.FixedAssetCategories.Where(x => x.FixedAssetCategoryID == EquipmentType.FixedAssetCategoryID).FirstOrDefault();
        //        if (FixedAssetCategory != null)
        //        {
        //            var FixedAssets = db.FixedAssetSubCategories.Where(x => x.CategoryCode == FixedAssetCategory.CategoryCode).ToList();
        //            if (FixedAssets.Any())
        //            {
        //                if (!string.IsNullOrWhiteSpace(Search))
        //                {
        //                    FixedAssets = FixedAssets.Where(x => x.SubCategoryName.ToUpper().Contains(Search.ToUpper())).ToList();
        //                }
        //                return Json(FixedAssets.Select(s => new { Name = s.SubCategoryName, Tag = s.FixedAssetSubCategoryID }).ToList(), JsonRequestBehavior.AllowGet);
        //            }
        //        }
        //        return Json(string.Empty, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json(string.Empty, JsonRequestBehavior.AllowGet);
        //}


        //
        //public ActionResult UnAssignedFleetsDDBind()
        //{
        //    IOperatorService _operatorService = new OperatorService();
        //    List<FleetViewModel> UnAssignedFleet = new List<FleetViewModel>();
        //    var operators = _operatorService.GetAll();
        //    var assetRegistrations = db.AssetRegistrations.ToList();

        //    var fleets = _FleetsService.GetAll().Select(s => new FleetViewModel
        //    {
        //        PlateNumber = GetAssetDependancies(s, assetRegistrations).PlateNumber + " -- " + s.UnitNum,
        //        FleetsID = s.FleetsID
        //    });


        //    foreach (var Fleetoperator in fleets)
        //    {
        //        var Assigned = operators.Where(x => x.FleetsID == Fleetoperator.FleetsID).FirstOrDefault();
        //        if (Assigned == null)
        //        {
        //            UnAssignedFleet.Add(Fleetoperator);
        //        }

        //    }

        //    return Json(UnAssignedFleet, JsonRequestBehavior.AllowGet);
        //}

        //
        //public ActionResult YearDDBind()
        //{
        //    List<SelectListItem> Years = new List<SelectListItem>();
        //    for (int i = DateTime.Now.Year; i > 1980; i--)
        //    {
        //        Years.Add(new SelectListItem()
        //        {
        //            Text = i.ToString(),
        //            Value = i.ToString()
        //        });
        //    }
        //    return Json(Years, JsonRequestBehavior.AllowGet);
        //}
        //
        //public ActionResult ApprovalDDBind()
        //{
        //    var approvals = new List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject>();
        //    var approval = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
        //    {
        //        Name = MaintenanceConst.Approved,
        //        Id = 1,
        //    };
        //    approvals.Add(approval);

        //    var approval2 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
        //    {
        //        Name = MaintenanceConst.Pending,
        //        Id = 2,
        //    };
        //    approvals.Add(approval2);
        //    var approval3 = new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject()
        //    {
        //        Name = MaintenanceConst.Rejected,
        //        Id = 3,
        //    };
        //    approvals.Add(approval3);
        //    return Json(approvals, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult AssignByCategoryDDBind(string type)
        //{
        //    IOperatorService _OperatorService = new OperatorService();
        //    IFleetService _FleetsService = new FleetService();

        //    IAssignedEquipmentService _assignedEquipmentService = new AssignedEquipmentService();
        //    List<Fleets> fleets = new List<Fleets>();
        //    List<Fleets> EquipmentForAssign = new List<Fleets>();
        //    var Equipments = _FleetsService.GetAll();
        //    var Operators = _OperatorService.GetAll();

        //    foreach (var Equipment in Equipments)
        //    {

        //        Equipment.FleetPhoto = Equipment.OperatorName;

        //        fleets.Add(Equipment);
        //    }

        //    List<AssignedEquipment> AssignedEquipments = _assignedEquipmentService.GetAll();

        //    foreach (var Equipment in fleets)
        //    {
        //        List<AssignedEquipment> assignedEquipments =
        //            AssignedEquipments.Where(x => x.FleetId == Equipment.FleetsID && x.EndDate > DateTime.Now)
        //                .ToList();
        //        if (assignedEquipments.Count < 1)
        //        {
        //            EquipmentForAssign.Add(Equipment);

        //        }
        //    }
        //    var RequestEquipment = EquipmentForAssign.Select(s => new FleetViewModel
        //    {
        //        EquipmentName = s.PlateNumber + " -- " + s.UnitNum + " -- " + s.FleetPhoto,
        //        PlateNumber = s.PlateNumber,
        //        UnitNum = s.UnitNum
        //    });

        //    foreach (var fleet in RequestEquipment)
        //    {
        //        fleet.PlateNumber = GetAssetDependancies(fleet).PlateNumber + " - " + fleet.UnitNum;
        //    }
        //    return Json(RequestEquipment, JsonRequestBehavior.AllowGet);
        //}
        //
        //public ActionResult AssignDDBind(string Id, string Search)
        //{

        //    ExceedDbContext db = new ExceedDbContext();
        //    IOperatorService _OperatorService = new OperatorService();
        //    IFleetService _FleetsService = new FleetService();
        //    IAssignedEquipmentService _assignedEquipmentService = new AssignedEquipmentService();
        //    IEquipmentTypeService typeService = new EquipmentTypeService();

        //    List<Fleets> fleets = new List<Fleets>();
        //    List<Fleets> EquipmentForAssign = new List<Fleets>();

        //    var Request = db.RequestedEquipments.Where(x => x.RequestedEquipmentId.ToString() == Id).FirstOrDefault();
        //    var Etype = typeService.GetAll().Where(x => x.EquipmentTypeID == Request.EquipmentTypeId).FirstOrDefault();
        //    var Equipments = _FleetsService.GetAll().Where(x => x.EquipmentTypeID == Etype.EquipmentTypeID && x.IsExternal == false && (x.Status != MaintenanceConst.OnMaintenance || x.Status != MaintenanceConst.ONDUTY) && x.IsOnlineApproved == true && x.IsReturned == false).ToList();
        //    if (string.IsNullOrWhiteSpace(Request.EquipmentNameId.ToString()))
        //    {
        //        Equipments = Equipments.Where(x => x.EquipmentNameId == Request.EquipmentNameId).ToList();
        //    }
        //    var Operators = _OperatorService.GetAll();

        //    foreach (var Equipment in Equipments)
        //    {

        //        Equipment.FleetPhoto = Equipment.OperatorName;


        //        fleets.Add(Equipment);
        //    }
        //    /*
        //                List<AssignedEquipment> AssignedEquipments = _assignedEquipmentService.GetAll();

        //                foreach (var Equipment in fleets)
        //                {
        //                    List<AssignedEquipment> assignedEquipments =
        //                        AssignedEquipments.Where(x => x.EquipmentID == Equipment.FleetsID && x.IsAssigned == true)
        //                            .ToList();
        //                    if (assignedEquipments.Count < 1)
        //                    {
        //                        var Ftype = Etype.Where(x => x.EquipmentTypeID == Equipment.EquipmentIdentityID).FirstOrDefault();
        //                        if (Ftype != null)
        //                        {
        //                            Equipment.Model = Ftype.CategoryName + " -- " + Ftype.EquipmentTypeName;
        //                        }

        //                        EquipmentForAssign.Add(Equipment);

        //                    }
        //                }*/
        //    var RequestEquipment = fleets.Select(s => new FleetViewModel
        //    {
        //        EquipmentName = s.UnitNum + " -- " + s.EquipmentName,
        //        PlateNumber = s.PlateNumber,
        //        UnitNum = s.UnitNum
        //    });
        //    foreach (var fleet in RequestEquipment)
        //    {
        //        fleet.PlateNumber = GetAssetDependancies(fleet).PlateNumber + " - " + fleet.UnitNum;
        //    }
        //    if (!string.IsNullOrWhiteSpace(Search))
        //    {
        //        RequestEquipment = RequestEquipment.Where(x => x.EquipmentName.ToUpper().Contains(Search.ToUpper())).ToList();
        //    }
        //    return Json(RequestEquipment, JsonRequestBehavior.AllowGet);
        //}

        //
        //public ActionResult FleetsTyreDDBind(string PlateNumber)
        //{
        //    string FleetType = string.Empty;
        //    var fleet = _FleetsService.GetAll().Where(f => f.PlateNumber == PlateNumber).FirstOrDefault();
        //    if (fleet != null)
        //    {
        //        var Type = db.EquipmentType.Where(x => x.EquipmentTypeID == fleet.EquipmentTypeID).FirstOrDefault();
        //        if (Type != null)
        //        {
        //            FleetType = Type.EquipmentTypeName;
        //        }
        //    }
        //    return Json(fleet.MinFuelStandard + "_" + fleet.TypeOfFuel + "_" + fleet.RentalRate + "_" + fleet.CurentOdoMeter + "_" + fleet.UnitNum + "_" + fleet.Model + "_" + fleet.Location + "_" + fleet.OperatorName + "_" + FleetType, JsonRequestBehavior.AllowGet);
        //}


        //[AuthorizeRoles(FleetManagementRoles.RegistrationApprover)]
        //public ActionResult ApproveBatch(Crews model)
        //{
        //    IFleetService _FleetService = new FleetService();
        //    try
        //    {
        //        if (model.ServiceCrews.Any())
        //        {
        //            foreach (var m in model.ServiceCrews)
        //            {
        //                var Fleet = _FleetService.GetById(m.ServiceCrewId);

        //                if (Fleet != null)
        //                {
        //                    Fleet.IsOnlineApproved = true;
        //                    Fleet.OnlineApprovedBy = User.Identity.Name;
        //                    Fleet.OnlineApprovedTime = DateTime.Now;

        //                    _FleetService.Update(Fleet);

        //                    EquipmentApproval Approval = new EquipmentApproval
        //                    {
        //                        FleetsId = m.ServiceCrewId,
        //                        Status = ApprovalStatuses.Approve.ToString(),
        //                        DateCreated = DateTime.Now,
        //                        CreatedBy = User.Identity.Name
        //                    };

        //                    db.EquipmentApproval.Add(Approval);
        //                    db.SaveChanges();
        //                }


        //            }

        //        }


        //    }
        //    catch (Exception e)
        //    {
        //        string ee = e.Message;
        //    }

        //    return Json(model);
        //}


        //
        //public ActionResult FleetsJobRequestDDBind(string Id)
        //{
        //    string FleetType = string.Empty;
        //    var fleet = _FleetsService.GetAll().Where(f => f.FleetsID.ToString() == Id).FirstOrDefault();
        //    if (fleet != null)
        //    {
        //        var Type = db.EquipmentType.Where(x => x.EquipmentTypeID == fleet.EquipmentTypeID).FirstOrDefault();
        //        if (Type != null)
        //        {
        //            FleetType = Type.EquipmentTypeName;
        //        }
        //        var Asset = db.AssetRegistrations.Where(x => x.Tag == fleet.UnitNum).FirstOrDefault();
        //        if (Asset != null)
        //        {
        //            FleetType = Type.EquipmentTypeName;
        //            return Json(fleet.MinFuelStandard + "_" + fleet.TypeOfFuel + "_" + fleet.RentalRate + "_" + fleet.CurentOdoMeter + "_" + fleet.UnitNum + "_" + Asset.ModelNo + "_" + fleet.Location + "_" + fleet.OperatorName + "_" + FleetType + "_" + Asset.Name, JsonRequestBehavior.AllowGet);

        //        }
        //        return Json(fleet.MinFuelStandard + "_" + fleet.TypeOfFuel + "_" + fleet.RentalRate + "_" + fleet.CurentOdoMeter + "_" + fleet.UnitNum + "_" + fleet.Model + "_" + fleet.Location + "_" + fleet.OperatorName + "_" + FleetType + "_" + fleet.EquipmentName, JsonRequestBehavior.AllowGet);

        //    }
        //    return Json("");
        //}
        //
        //public ActionResult FleetsFuelAndRentDDBind(string FleetsID)
        //{

        //    var fleets = _FleetsService.GetAll().Where(f => f.FleetsID == Guid.Parse(FleetsID)).Select(s => new FleetViewModel
        //    {
        //        RentalRate = s.RentalRate.ToString(),
        //        EquipmentName = s.EquipmentName,
        //        FuelStandard = s.MinFuelStandard.ToString(),
        //        UnitNum = s.UnitNum,
        //        FuelType = s.TypeOfFuel
        //    });

        //    foreach (var fleet in fleets)
        //    {
        //        fleet.PlateNumber = GetAssetDependancies(fleet).PlateNumber + " - " + fleet.UnitNum;
        //    }
        //    return Json(fleets, JsonRequestBehavior.AllowGet);
        //}
        //
        //public ActionResult FleetsTODDBind(string PartNeededFleetsID)
        //{

        //    var fleets = _FleetsService.GetAll().Where(x => x.FleetsID != Guid.Parse(PartNeededFleetsID) && x.IsOnlineApproved == true).Select(s => new FleetViewModel
        //    {
        //        PlateNumber = s.PlateNumber + " - " + s.UnitNum,
        //        FleetsID = s.FleetsID,
        //        UnitNum = s.UnitNum

        //    });
        //    foreach (var fleet in fleets)
        //    {
        //        fleet.PlateNumber = GetAssetDependancies(fleet).PlateNumber + " - " + fleet.UnitNum;
        //    }
        //    return Json(fleets, JsonRequestBehavior.AllowGet);
        //}

        //
        //public ActionResult GetEquipmentByRent(Guid RentId)
        //{

        //    var fleets = _FleetsService.GetAll().Where(x => x.RentId == RentId).Select(s => new FleetViewModel
        //    {
        //        PlateNumber = s.EquipmentName + " - " + s.PlateNumber,
        //        FleetsID = s.FleetsID,
        //        UnitNum = s.UnitNum
        //    });

        //    foreach (var fleet in fleets)
        //    {
        //        fleet.PlateNumber = GetAssetDependancies(fleet).PlateNumber + " - " + fleet.UnitNum;
        //    }
        //    return Json(fleets, JsonRequestBehavior.AllowGet);
        //}


        //
        //public ActionResult OnDistributionFleetDDBind()
        //{
        //    IVehicleDistributionService _VehicleDistributionService = new VehicleDistributionService();
        //    List<Fleets> fleet = new List<Fleets>();
        //    var Distributions = _VehicleDistributionService.GetAll().ToList();
        //    var fleets = _FleetsService.GetAll().Where(x => x.IsOnlineApproved == true).Select(s => new Fleets
        //    {
        //        PlateNumber = s.PlateNumber + " - " + s.UnitNum,
        //        FleetsID = s.FleetsID
        //    }).ToList();
        //    foreach (var Distribution in Distributions)
        //    {


        //        foreach (var s in fleets)
        //        {
        //            fleet.Add(new Fleets
        //            {
        //                PlateNumber = s.PlateNumber + " - " + s.UnitNum,
        //                FleetsID = s.FleetsID
        //            });

        //        }
        //    }
        //    return Json(fleets, JsonRequestBehavior.AllowGet);
        //}






        public string GetSection(Guid ServiceId)
        {
            var _ServiceService = new ServiceService();
            var Service = _ServiceService.GetById(ServiceId);
            if (Service != null)
            {
                var Sections = db.WorkUnitSection.Where(x => x.WorkUnitSectionId == Service.SectionId).FirstOrDefault();
                if (Sections != null)
                {
                    return Sections.Name;
                }

            }
            return string.Empty;
        }
        public ActionResult EquipmentServiceCost_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            var ServiceList = new List<JobOrderDetail>();
            var _JobOrderService = new JobOrderService();
            var JobOrderDetailService = new JobOrderDetailService();
            decimal Cost = decimal.Zero;
            var JobOrder = _JobOrderService.GetAll()
                .Where(
                    x =>
                         x.FleetsID == Id).ToList();
            foreach (var job in JobOrder)
            {
                Cost = Cost + GetJobCost(job.JobOrderID);
            }
            ServiceList.Add(new JobOrderDetail { ServiceName = "Total", Efficency = Cost });

            return Json(ServiceList.ToDataSourceResult(request));
        }
        public ActionResult EquipmentItemIssue_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            var IssueItems = new List<IssueItem>();
            FleetReportHelper fleetReportHelper = new FleetReportHelper();
            var JobOrder = _JobOrderService.GetAll().Where(x => x.FleetsID == Id).ToList();
            foreach (var job in JobOrder)
            {
                var StoreRequestions = db.StoreRequisitions.Where(x => x.JobOrderNo == job.Num.ToString() || x.JobOrderNo == job.JobOrderID.ToString());
                foreach (var StoreRequestion in StoreRequestions)
                {
                    var Requistions = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.StoreRequisitionID == StoreRequestion.StoreRequisitionID).ToList();

                    IssueItems.AddRange(Requistions);

                }


            }
            return Json(IssueItems.Select(s => new { IssueItemID = s.IssueItemID, UnitPrice = s.UnitPrice, Name = s.InventoryItem.Name, DateCreated = s.DateCreated, CreatedBy = s.CreatedBy, IssuedQuantity = s.IssuedQuantity }).ToList().ToDataSourceResult(request));
        }

        public ActionResult EquipmentManPower_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            var Crews = new List<ServiceCrew>();
            var JobOrder = _JobOrderService.GetAll().Where(x => x.FleetsID == Id).ToList();
            FleetMaintenaceHelper fleetMaintenaceHelper = new FleetMaintenaceHelper();
            foreach (var job in JobOrder)
            {
                var MechanicLogs = db.ServiceCrewWorkLogs.Where(x => x.JobId == job.JobOrderID).ToList();
                IMechanicCrewService mechanicCrewService = new MechanicCrewService();



                foreach (var log in MechanicLogs.DistinctByField(x => x.MecanicId))
                {
                    var Crew = new ServiceCrew();
                    var Mechanic = mechanicCrewService.GetById(log.MecanicId);

                    if (Mechanic != null)
                    {
                        var MechanicLog = MechanicLogs.Where(x => x.MecanicId == log.MecanicId).ToList();

                        Crew.Type = Mechanic.Postion;
                        Crew.CreatedBy = Mechanic.FullName;
                        Crew.ServiceName = Mechanic.FullName;
                        Crew.LabourCost = decimal.Parse(fleetMaintenaceHelper.GetMechanicDuration(MechanicLog).ToString()) * Mechanic.HourlyWage;
                        Crew.HourlyWage = fleetMaintenaceHelper.GetMechanicDuration(MechanicLog);
                        Crews.Add(Crew);
                    }
                }


            }

            return Json(Crews.Select(s => new
            {
                ServiceCrewID = s.ServiceCrewID,
                CreatedBy = s.CreatedBy,
                ServiceName = s.ServiceName,
                Type = s.Type,
                IsActive = s.IsActive,
                ActivatedDate = s.ActivatedDate,
                HourlyWage = s.HourlyWage,
                LabourCost = s.LabourCost.ToString()

            }).ToDataSourceResult(request));
        }

        public string EquipmentManPowerCost(Guid Id)
        {
            decimal TotalCost = 0;
            var JobOrder = _JobOrderService.GetAll().Where(x => x.FleetsID == Id).ToList();
            FleetMaintenaceHelper fleetMaintenaceHelper = new FleetMaintenaceHelper();
            foreach (var job in JobOrder)
            {
                var MechanicLogs = db.ServiceCrewWorkLogs.Where(x => x.JobId == job.JobOrderID).ToList();
                IMechanicCrewService mechanicCrewService = new MechanicCrewService();



                foreach (var log in MechanicLogs.DistinctByField(x => x.MecanicId))
                {
                    var Crew = new ServiceCrew();
                    var Mechanic = mechanicCrewService.GetById(log.MecanicId);

                    if (Mechanic != null)
                    {
                        var MechanicLog = MechanicLogs.Where(x => x.MecanicId == log.MecanicId).ToList();

                        TotalCost += decimal.Parse(fleetMaintenaceHelper.GetMechanicDuration(MechanicLog).ToString()) * Mechanic.HourlyWage;

                    }
                }


            }

            return TotalCost.ToString();
        }
        public ActionResult EquipmentFuelIssue_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {

            var fleet = db.Fleets.Where(x => x.FleetsID == Id).FirstOrDefault();
            var IssueItems = db.IssueItems.Where(x => x.Issue.PlateNo == fleet.PlateNumber && x.Issue.IsFuel).Select(s => new { IssueItemID = s.IssueItemID, UnitPrice = s.UnitPrice, Name = s.InventoryItem.Name, DateCreated = s.DateCreated, CreatedBy = s.CreatedBy, IssuedQuantity = s.IssuedQuantity }).ToList();

            return Json(IssueItems.ToDataSourceResult(request));
        }

        public ActionResult EquipmentServiceHistory_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            FleetCostHelper fleetCostHelper = new FleetCostHelper();
            var ServiceList = new List<JobOrderDetail>();
            var _JobOrderService = new JobOrderService();
            var JobOrderDetailService = new JobOrderDetailService();
            var inventortySpareMapping = db.InventorySparePartMapping.ToList();
            var JobOrder = _JobOrderService.GetAll()
                .Where(
                    x =>
                        x.FleetsID == Id).ToList();
            foreach (var job in JobOrder)
            {
                var Service = JobOrderDetailService.GetAll().Where(x => x.JobOrderID == job.JobOrderID).ToList();

                if (Service.Any())
                {
                    var JobCost = fleetCostHelper.GetJobOrderSpareCost(job, inventortySpareMapping) / Service.Count;

                    ServiceList.AddRange(Service.Select(s => new JobOrderDetail
                    {
                        ServiceName = s.ServiceName,
                        Remark = job.Odometer.ToString(),
                        DateCreated = s.DateCreated,
                        Section = GetSection(s.ServiceID),
                        Efficency = JobCost

                    }));
                }
            }


            return Json(ServiceList.ToDataSourceResult(request));
        }


        #region ExternalEquipment


        public ActionResult ExternalFleets_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            var fleets = _FleetsService.GetAll().Where(x => x.IsExternal == true && x.RentId == Id && x.IsReturned == false).Select(f => new

            {
                MinFuelStandard = f.MinFuelStandard,
                MaxFuelStandard = f.MaxFuelStandard,
                Capacity = f.Capacity,
                ChasisNumber = f.ChasisNumber,
                EquipmentName = f.EquipmentNameID,
                Color = f.Color,
                CurentOdoMeter = f.CurentOdoMeter,
                DepreciationCost = f.DepreciationCost,
                Attachment = f.Attachment,
                EngineNumber = f.EngineNumber,
                EngineManufacturer = f.EngineManufacturer,
                EngineModel = f.EngineModel,
                EquipmentNameID = f.EquipmentNameID,
                EquipmentIdentityID = f.EquipmentIdentityID,
                UnitNum = f.UnitNum,
                IdealRentalRate = f.IdealRentalRate,

                EquipmentTypeID = f.EquipmentTypeID,
                FleetPhoto = f.FleetPhoto,
                FleetsID = f.FleetsID,
                Location = f.Location,
                LastMaintained = f.LastMaintained,
                Libre = f.Libre,
                Manufacturer = f.Manufacturer,
                RentType = f.RentType,
                ManufacturerYear = f.ManufacturerYear,
                Model = f.Model,
                PlateNumber = f.PlateNumber,
                PreviousOdoMeter = f.PreviousOdoMeter,
                PlannerID = f.PlannerID,
                PurchasedDate = f.PurchasedDate,
                PurchasingCost = f.PurchasingCost,
                RentalRate = f.RentalRate,
                SerialNo = f.SerialNo,
                StateOfOdoMeter = f.StateOfOdoMeter,
                Status = f.Status,
                SupplierCompany = f.SupplierCompany,
                Type = f.Type,
                TypeOfFuel = f.TypeOfFuel,
                TyreSize = f.TyreSize,
                // UnitNum = f.UnitNum,
                //WorkingCost = GetWorkingCost(f.FleetsID),
                //IdealCost = GetIdealCost(f.FleetsID),
                //SpareRepairCost = GetSpareCost(f.FleetsID),
                //TyreRepairCost = GetTyreSpareCost(f.FleetsID),
                //OtherRepairCost = GetOtherSpareCost(f.FleetsID),
                //OilRepairCost = GetOilSpareCost(f.FleetsID),
                //BatteryRepairCost = GetBatterySpareCost(f.FleetsID),
                RentTypeName = f.RentType,
                OperatorName = f.OperatorName,
                AnnualDepreciationCost = f.AnnualDepreciationCost,
                WayOfSupply = f.WayOfSupply,
                IsSendForApproval = f.IsSendForApproval,
                IsOnlineApproved = f.IsOnlineApproved,
                HorsePower = f.HorsePower,
            });

            return this.Json(fleets.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }



        [AuthorizeRoles(FleetManagementRoles.RentUser)]
        public ActionResult ExternalFleets_Create([DataSourceRequest] DataSourceRequest request, Fleets fleets, Guid Id)
        {
            ModelState.Remove("FleetsID");
            ModelState.Remove("RentId");
            IFleetService _FleetService = new FleetService();
            IRentFromExternalService _RentFromExternalService = new RentFromExternalService();
            var ExternalRent = _RentFromExternalService.GetById(Id);
            List<Fleets> AssignedEquipment =
                _FleetService.GetAll()
                    .Where(x => x.IsExternal == true && x.RentId == ExternalRent.RentFromExternalID)
                    .ToList();

            if (ExternalRent != null)
            {


                if (AssignedEquipment.Count >= ExternalRent.Quantity)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("EquipmentExceed", " Equipment Exceed !");

                }
                else if (AssignedEquipment.Count >= ExternalRent.Quantity)
                {
                    ModelState.Clear();
                    ModelState.AddModelError("ExistingPlateNumber", " Equipment Plate Number Already Exists !");

                }
                else
                {


                    List<Fleets> fleetsPlate =
                        _FleetService.GetAll()
                            .Where(f => f.RentId == Id && f.IsExternal == true && f.PlateNumber == fleets.PlateNumber)
                            .ToList();

                    if (fleetsPlate.Any())
                    {
                        ModelState.Clear();
                        ModelState.AddModelError("ExistingPlateNumber", " Equipment Plate Number Already Exists !");
                    }

                    else
                    {


                        if (fleets != null)
                        {
                            fleets.FleetsID = Guid.NewGuid();
                            fleets.LastMaintained = DateTime.Now;
                            fleets.PreviousOdoMeter = fleets.CurentOdoMeter;
                            fleets.Status = MaintenanceConst.ONDUTY;
                            fleets.IsExternal = true;
                            fleets.RentId = Id;
                            fleets.MinFuelStandard = fleets.MinFuelStandard;
                            fleets.MaxFuelStandard = fleets.MaxFuelStandard;
                            fleets.Location = ExternalRent.ProjectLocation;
                            fleets.CreatedBy = User.Identity.Name;
                            fleets.DateCreated = DateTime.Now;

                            _FleetsService.Insert(fleets);
                            if (ModelState.IsValid)
                            {
                                db.Fleets.Add(fleets);
                                db.SaveChanges();
                                #region operation log
                                UserHelper.OperationLog(Request.UserHostAddress, "External fleet registration", "Success", User.Identity.Name, $"created external fleet {fleets.FleetsID} Tag {fleets.EquipmentName}", Modules.FleetManagement);
                                #endregion

                            }





                        }
                    }
                }
            }


            return Json(new[] { fleets }.ToDataSourceResult(request));
        }
        //public async Task GetMaintainaceDue()
        //{
        //    IFleetService _FleetService = new FleetService();
        //    IServiceService _ServiceService = new ServiceService();
        //    IPlannerService _PlannerService = new PlannerService();
        //    List<MaintainceDueViewModel> FinalAssetsDues = new List<MaintainceDueViewModel>();
        //    Guid AssetsId = Guid.Empty;
        //    Guid AssetID = Guid.Empty;
        //    List<Fleets> AssetsDue = _FleetService.GetAll().Where(x => x.Status != MaintenanceConst.OnMaintenance).ToList();
        //    string AssetName = "";

        //    int CountDue = 0;
        //    string IsMileageOrDate = null;
        //    try
        //    {

        //        foreach (var AssetsDues in AssetsDue)
        //        {
        //            AssetID = Guid.Empty;
        //            AssetName = AssetsDues.EquipmentName + "--" + AssetsDues.PlateNumber;
        //            List<Service> AssetsDuess = _ServiceService.GetAll().Where(x => x.PlannerID == AssetsDues.PlannerID).ToList();// MDueMgt.getMaintenaceDueInforamation().Where(x => x.Status != "OnMaintenance" && x.AssetId == AssetsDues).ToList();
        //            int TotDues = AssetsDuess.Count;

        //            foreach (var Services in AssetsDuess)
        //            {
        //                CountDue = CountDue + 1;
        //                IsMileageOrDate = null;

        //                Service PMM = Services;

        //                if (PMM.IsPm == MaintenanceConst.Planned)
        //                {
        //                    if (PMM.Planner.TrackByDate)
        //                    {

        //                        if (Services.DateIntervalType == MaintenanceConst.Day)
        //                        {

        //                            DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddDays(double.Parse(Services.DateInterval.ToString()));
        //                            if (DateTime.Now > newDay)
        //                            {
        //                                IsMileageOrDate = IsMileageOrDate + " By Date";
        //                                Notify.NotifyUser(new NotifModels { UserId = User.Identity.Name, DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

        //                            }

        //                        }
        //                        if (Services.DateIntervalType == MaintenanceConst.Week)
        //                        {

        //                            DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddDays(double.Parse(Services.DateInterval.ToString()) * 7);
        //                            if (DateTime.Now > newDay)
        //                            {
        //                                IsMileageOrDate = IsMileageOrDate + " By Date";
        //                                Notify.NotifyUser(new NotifModels { UserId = User.Identity.Name, DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

        //                            }

        //                        }
        //                        else if (Services.DateIntervalType == MaintenanceConst.Month)
        //                        {
        //                            DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddMonths(int.Parse(Services.DateInterval.ToString()));
        //                            if (DateTime.Now > newDay)
        //                            {
        //                                IsMileageOrDate = IsMileageOrDate + " By Date";
        //                                Notify.NotifyUser(new NotifModels { UserId = User.Identity.Name, DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

        //                            }

        //                        }
        //                        else if (Services.DateIntervalType == MaintenanceConst.Year)
        //                        {
        //                            DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddYears(int.Parse(Services.DateInterval.ToString()));
        //                            if (DateTime.Now > newDay)
        //                            {
        //                                IsMileageOrDate = IsMileageOrDate + " By Date";
        //                                Notify.NotifyUser(new NotifModels { UserId = User.Identity.Name, DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Date at " + newDay.Day + "/" + newDay.Month + "/" + newDay.Year, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

        //                            }

        //                        }
        //                    }

        //                    if (PMM.Planner.TrackByMileage)
        //                    {

        //                        decimal OdoMeterMeasure = AssetsDues.CurentOdoMeter - AssetsDues.PreviousOdoMeter;
        //                        if (OdoMeterMeasure > Services.MileageInterval)
        //                        {
        //                            IsMileageOrDate = IsMileageOrDate + " By Mileage";
        //                            Notify.NotifyUser(new NotifModels { UserId = User.Identity.Name, DeadLine = DateTime.Now, Message = "Equipment with Asset No " + AssetsDues.UnitNum + " For Service " + Services.ServiceName + "  ", Title = " Equipment Due By Mileage " + OdoMeterMeasure + " KM", URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });


        //                        }

        //                    }
        //                }



        //            }


        //        }
        //    }
        //    catch (Exception)
        //    {

        //    }

        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalFleets_Update([DataSourceRequest] DataSourceRequest request, Fleets fleets)
        {
            IEquipmentNameService _equipmentNameService = new EquipmentNameService();
            IFleetService _FleetService = new FleetService();

            AssetRegistration Asset = db.AssetRegistrations.Where(x => x.AssetRegistrationId.ToString() == fleets.UnitNum || x.AssetRegistrationId.ToString() == fleets.SerialNo || x.AssetRegistrationId.ToString() == fleets.PlateNumber).FirstOrDefault();

            if (Asset != null)
            {
                List<Fleets> fleetsPlate = _FleetService.GetAll().Where(f => f.UnitNum == Asset.Tag && f.FleetsID != fleets.FleetsID).ToList();

                if (fleetsPlate.Any())
                {
                    ModelState.Clear();
                    ModelState.AddModelError("ExistingPlateNumber", " Equipment with Asset No. " + Asset.Tag + " Already Exists !");

                    return Json(new[] { fleets }.ToDataSourceResult(request, ModelState));
                }

                if (fleetsPlate.Any())
                {
                    ModelState.Clear();
                    ModelState.AddModelError("ExistingPlateNumber", " Equipment Plate Number Already Exists !");
                    return Json(new[] { fleets }.ToDataSourceResult(request, ModelState));
                }

                else
                {
                    FleetHistory fH = new FleetHistory
                    {
                        FleetHistoryID = Guid.NewGuid(),
                        FleetsID = fleets.FleetsID,
                        FuelStandard = fleets.MinFuelStandard.ToString(),
                        RentalRate = fleets.RentalRate,
                        AnnualDepreciationCost = fleets.AnnualDepreciationCost
                    };
                    if (fleets != null)
                    {
                        fleets.AssetId = Asset.AssetRegistrationId;
                        fleets.LastMaintained = DateTime.Now;
                        fleets.PreviousOdoMeter = fleets.CurentOdoMeter;
                        fleets.Status = MaintenanceConst.ACTIVE;
                        fleets.ModifiedBy = User.Identity.Name;
                        fleets.LastModified = DateTime.Now;
                        fleets.Battery = Asset.Battery;
                        fleets.ChasisNumber = Asset.MotorNo;
                        fleets.EngineNumber = Asset.EngineNo;
                        fleets.EngineModel = Asset.MotorNo;
                        fleets.Manufacturer = Asset.Manufacturer;
                        fleets.ManufacturerYear = Asset.ManufacturDate.ToString();
                        fleets.Location = Asset.Location;
                        fleets.Capacity = Asset.LoadCapacity;
                        fleets.PlateNumber = Asset.PlateNo;
                        fleets.PurchasedDate = DateTime.Now;
                        fleets.PurchasingCost = Asset.PurchasePrice;
                        fleets.SerialNo = Asset.SerialNo;
                        fleets.TyreSize = fleets.TyreSize;
                        fleets.TypeOfFuel = Asset.FuelType;
                        fleets.Tyre = Asset.Tyre;
                        fleets.Model = Asset.ModelNo;
                        fleets.SupplierCompany = Asset.Supplier;
                        fleets.HistoricalCost = Asset.PurchasePrice.ToString();
                        fleets.DepreciationCost = (int)(Asset.PurchasePrice - Asset.BookValue);
                        fleets.PurchasedDate = Asset.PurchaseDate;
                        fleets.RentType = fleets.RentType;
                        fleets.IdealRentalRate = fleets.IdealRentalRate;
                        fleets.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        fleets.Condition = Asset.Condition.Name;
                        fleets.OperatorName = MaintenanceConst.UnAssigned;
                        fleets.UnitNum = Asset.Tag;
                        fleets.ModifiedBy = User.Identity.Name;
                        fleets.LastModified = DateTime.Now;
                        _FleetsService.Update(fleets);
                        _FleetHistoryService.Insert(fH);

                        EquipmentName _equipmentName =
                            _equipmentNameService.GetAll()
                                .Where(x => x.EquipmentNameID == fleets.EquipmentNameID)
                                .FirstOrDefault();
                        if (_equipmentName != null)
                        {
                            _equipmentName.IsAssigned = true;
                            _equipmentNameService.Update(_equipmentName);
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "Fleets upadet", "Success", User.Identity.Name, $"updated fleet {fleets.FleetsID}", Modules.FleetManagement);
                            #endregion
                        }

                    }
                }

            }
            return Json(new[] { fleets }.ToDataSourceResult(request));
        }

        #endregion

    }



}
