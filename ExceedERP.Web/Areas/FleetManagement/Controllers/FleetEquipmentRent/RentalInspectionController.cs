using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;  
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Web.Areas.FleetManagement.Models;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    public class RentalInspectionController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult Index()
        {
            ViewData["EquipmentType"] = db.EquipmentType.Select(x => new { x.EquipmentTypeID, x.EquipmentTypeName }).ToList();
            ViewData["EquipmentName"] = db.EquipmentName.Select(x => new { x.EquipmentNameID, x.Name}).ToList();
            ViewData["BranchName"] = db.Branch.Select(x => new { x.BranchId, x.Name }).ToList();
            ViewData["RentalInspectionCriteria"] = db.RentalInspectionCriteria.Select(x => new { x.RentalInspectionCriteriaId, x.CriteriaCategory }).ToList();
            ViewData["RentalInspectionCriteriaDetail"] = db.RentalInspectionCriteriaDetails.Select(x => new { x.RentalInspectionCriteriaDetailId, x.CriteriaName }).ToList();
            return View();
        }
        private int GetValidationStaus(int id)
        {
            var validation = db.RentalInspectionValidations.Where(x => x.RentalInspectionId == id).FirstOrDefault();
            if(validation != null && validation.Status ==ApprovalStatuses.Approve)
            {
                return  0 ;

            }
            return 3;
        }
        public ActionResult RentalInspection_Read([DataSourceRequest] DataSourceRequest request)
        {
            var rentalInspections = db.RentalInspections;
            DataSourceResult result = rentalInspections.ToDataSourceResult(request, rentalInspection => new
            {
                CCKW = rentalInspection.CCKW,
                RentalInspectionId = rentalInspection.RentalInspectionId,
                EquipmentTypeID = rentalInspection.EquipmentTypeID,
                EquipmentNameID = rentalInspection.EquipmentNameID,
                Model = rentalInspection.Model,
                ChasisSerialNo = rentalInspection.ChasisSerialNo,
                EngineModel = rentalInspection.EngineModel,
                ManufacturingYear = rentalInspection.ManufacturingYear,
                Capacity = rentalInspection.Capacity,
                Hp = rentalInspection.Hp,
                OperatorLicenceNo = rentalInspection.OperatorLicenceNo,
                Lessor = rentalInspection.Lessor,
                PlateNo = rentalInspection.PlateNo,
                BranchId = rentalInspection.BranchId,
                EngineSerialNo=rentalInspection.EngineSerialNo,
                MeterReading = rentalInspection.MeterReading,
                IsSendForApproval = rentalInspection.IsSendForApproval,
                SendForApprovalBy = rentalInspection.SendForApprovalBy,
                SendForApprovalTime = rentalInspection.SendForApprovalTime,
                IsOnlineTransferred = rentalInspection.IsOnlineTransferred,
                IsOnlineTransferredBy = rentalInspection.IsOnlineTransferredBy,
                OnlineTransferredTime = rentalInspection.OnlineTransferredTime,
                validationStatus = GetValidationStaus(rentalInspection.RentalInspectionId)
            });
            return Json(result);
        }
 public ActionResult RentalInspectionValidation_Read([DataSourceRequest] DataSourceRequest request,int Id)
        {
            var rentalInspectionValidations = db.RentalInspectionValidations.Where(x=>x.RentalInspectionId==Id);
            

            DataSourceResult result = rentalInspectionValidations.ToDataSourceResult(request, rentalInspectionValidation => new
            {
                RentalInspectionValidationId = rentalInspectionValidation.RentalInspectionValidationId,
                RentalInspectionId = rentalInspectionValidation.RentalInspectionId,
                Status = rentalInspectionValidation.Status,
                Remark = rentalInspectionValidation.Remark,
            });
            return Json(result);
        }
         public ActionResult RentalMVInspections_Read([DataSourceRequest] DataSourceRequest request,int id)
        {
            //var rentalInspections = db.RentalMVInspections;
            var rentalInspections = db.RentalMVInspections.Where(x => x.RentalInspectionId == id);

            DataSourceResult result = rentalInspections.ToDataSourceResult(request, rentalInspection => new
            {
                RentalMVInspectionId = rentalInspection.RentalMVInspectionId,
                RentalInspectionCriteriaId = rentalInspection.RentalInspectionCriteriaId,
                RentalInspectionCriteriaDetailId = rentalInspection.RentalInspectionCriteriaDetailId,
                RentalInspectionId = rentalInspection.RentalInspectionId,
                Index = rentalInspection.Index,
                HeadOfficeGoodCondtion = rentalInspection.HeadOfficeGoodCondtion,
                HeadOfficePoorCondtion = rentalInspection.HeadOfficePoorCondtion,
                BranchOfficeGoodCondtion = rentalInspection.BranchOfficeGoodCondtion,
                BranchOfficePoorCondtion = rentalInspection.BranchOfficePoorCondtion,
                IsSendForApproval = rentalInspection.IsSendForApproval,
                SendForApprovalBy = rentalInspection.SendForApprovalBy,
                SendForApprovalTime = rentalInspection.SendForApprovalTime,
                IsOnlineTransferred = rentalInspection.IsOnlineTransferred,
                IsOnlineTransferredBy = rentalInspection.IsOnlineTransferredBy,
                OnlineTransferredTime = rentalInspection.OnlineTransferredTime,
                
                
            });
            return Json(result);
        }
        public ActionResult RentalInspection_Create([DataSourceRequest] DataSourceRequest request, RentalInspection rentalInspection, RentalMVInspection rentalMVInspection)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalInspection
                {
                    RentalInspectionId = rentalInspection.RentalInspectionId,
                    EquipmentTypeID = rentalInspection.EquipmentTypeID,
                    EquipmentNameID = rentalInspection.EquipmentNameID,
                    Model = rentalInspection.Model,
                    ChasisSerialNo = rentalInspection.ChasisSerialNo,
                    EngineModel = rentalInspection.EngineModel,
                    ManufacturingYear = rentalInspection.ManufacturingYear,
                    Capacity = rentalInspection.Capacity,
                    Hp = rentalInspection.Hp,
                    OperatorLicenceNo = rentalInspection.OperatorLicenceNo,
                    Lessor = rentalInspection.Lessor,
                    PlateNo = rentalInspection.PlateNo,
                    BranchId = rentalInspection.BranchId,
                    DateCreated = DateTime.Now.Date,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = rentalInspection.ModifiedBy,
                    LastModified = rentalInspection.LastModified,
                    EngineSerialNo = rentalInspection.EngineSerialNo,
                    MeterReading = rentalInspection.MeterReading,
                    IsSendForApproval = rentalInspection.IsSendForApproval,
                    SendForApprovalBy = rentalInspection.SendForApprovalBy,
                    SendForApprovalTime = rentalInspection.SendForApprovalTime,
                    IsOnlineTransferred = rentalInspection.IsOnlineTransferred,
                    IsOnlineTransferredBy = rentalInspection.IsOnlineTransferredBy,
                    OnlineTransferredTime = rentalInspection.OnlineTransferredTime,
                    CCKW = rentalInspection.CCKW
                };
                db.RentalInspections.Add(entity);
                db.SaveChanges();
                rentalInspection.RentalInspectionId = entity.RentalInspectionId;
                #region add_criteria
                var branchName = db.Branch.FirstOrDefault(x => x.BranchId == entity.BranchId).Name;
                var criterias = db.RentalInspectionCriteria.OrderBy(x => x.Index).ToList();
                foreach (var criteria in criterias)
                {
                    var criteriaDetails = db.RentalInspectionCriteriaDetails.Where(x => x.RentalInspectionCriteriaId == criteria.RentalInspectionCriteriaId).OrderBy(x => x.Index).ToList();
                    foreach (var criteriaDetail in criteriaDetails)
                    {
                        var detail = new RentalMVInspection
                        {
                            RentalMVInspectionId = rentalMVInspection.RentalMVInspectionId,
                            RentalInspectionCriteriaId = criteria.RentalInspectionCriteriaId,
                            RentalInspectionCriteriaDetailId = criteriaDetail.RentalInspectionCriteriaDetailId,
                            RentalInspectionId = entity.RentalInspectionId,
                            Index = criteriaDetail.Index,
                           
                        };
                        detail.HeadOfficeGoodCondtion = branchName == "Head Office" ? GoodCondition.Good : GoodCondition.NA;
                        detail.HeadOfficePoorCondtion = branchName == "Head Office" ? PoorCondition.NA : PoorCondition.NA;
                        detail.BranchOfficeGoodCondtion = branchName == "Head Office" ? GoodCondition.NA : GoodCondition.Good;
                        detail.BranchOfficePoorCondtion = branchName == "Head Office" ? PoorCondition.NA : PoorCondition.NA;

                        db.RentalMVInspections.Add(detail);
                        db.SaveChanges();
                    }

                }
            


                #endregion
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental inspection registration", "Success", User.Identity.Name, $"created rental inspection {entity.RentalInspectionId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalInspection }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentalInspection_Update([DataSourceRequest] DataSourceRequest request, RentalInspection rentalInspection)
        {
          if (ModelState.IsValid)
            {
                var entity = new RentalInspection
                {
                    RentalInspectionId = rentalInspection.RentalInspectionId,
                    EquipmentTypeID = rentalInspection.EquipmentTypeID,
                    EquipmentNameID = rentalInspection.EquipmentNameID,
                    Model = rentalInspection.Model,
                    ChasisSerialNo = rentalInspection.ChasisSerialNo,
                    EngineModel = rentalInspection.EngineModel,
                    ManufacturingYear = rentalInspection.ManufacturingYear,
                    Capacity = rentalInspection.Capacity,
                    Hp = rentalInspection.Hp,
                    OperatorLicenceNo = rentalInspection.OperatorLicenceNo,
                    Lessor = rentalInspection.Lessor,
                    PlateNo = rentalInspection.PlateNo,
                    BranchId = rentalInspection.BranchId,
                    DateCreated = rentalInspection.LastModified,
                    CreatedBy = rentalInspection.ModifiedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified =DateTime.Now.Date,
                    EngineSerialNo = rentalInspection.EngineSerialNo,
                    MeterReading = rentalInspection.MeterReading,
                    IsSendForApproval = rentalInspection.IsSendForApproval,
                    SendForApprovalBy = rentalInspection.SendForApprovalBy,
                    SendForApprovalTime = rentalInspection.SendForApprovalTime,
                    IsOnlineTransferred = rentalInspection.IsOnlineTransferred,
                    IsOnlineTransferredBy = rentalInspection.IsOnlineTransferredBy,
                    OnlineTransferredTime = rentalInspection.OnlineTransferredTime,
                    CCKW = rentalInspection.CCKW
                };
                db.RentalInspections.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental inspection ", "Success", User.Identity.Name, $"updated rental inspection {entity.RentalInspectionId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalInspection }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RentalInspectionValidation_Update([DataSourceRequest] DataSourceRequest request, RentalInspectionValidation rentalInspectionValidation)
        {
            
          if (ModelState.IsValid)
            {
                var entity = new RentalInspectionValidation
                {
                    RentalInspectionValidationId = rentalInspectionValidation.RentalInspectionValidationId,
                    RentalInspectionId = rentalInspectionValidation.RentalInspectionId,
                    Status = rentalInspectionValidation.Status,
                    Remark = rentalInspectionValidation.Remark
                };                
                db.RentalInspectionValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region Register Equipment
                if (rentalInspectionValidation.Status == ApprovalStatuses.Approve)
                {
                    var inspectedEquipment = db.RentalInspections.FirstOrDefault(x => x.RentalInspectionId == rentalInspectionValidation.RentalInspectionId);


                    //var equipmentName = db.EquipmentName.Where(x => x.EquipmentTypeId == inspectedEquipment.EquipmentTypeID).FirstOrDefault();
                    var equipmntCategory = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == inspectedEquipment.EquipmentTypeID);
                    var planner = db.Planner.Select(x => x.PlannerID).FirstOrDefault();
                    var equipment = new Fleets
                    {
                        FleetsID = Guid.NewGuid(),
                        EquipmentIdentityID = equipmntCategory.EquipmentIdentityID,
                        EquipmentTypeID = inspectedEquipment.EquipmentTypeID,
                        PlannerID = planner,
                        LastMaintained = DateTime.Now,
                        EquipmentNameID = (Guid)inspectedEquipment.EquipmentNameID, //equipmentName.EquipmentNameID,
                        SubType = "",
                        Type = "",
                        UnitNum = "",
                        Manufacturer = inspectedEquipment.Manufacturer,
                        ManufacturerYear = inspectedEquipment.ManufacturingYear,
                        Condition = "New",
                        ReadingType = ReadingType.HR,
                        EngineModel = inspectedEquipment.EngineModel,
                        OperatorName = "",
                        EngineManufacturer = "",
                        Location = inspectedEquipment.BranchId.ToString(),
                        HistoricalCost = "",
                        Model = inspectedEquipment.Model,
                        IsReturned = false,
                        Capacity = inspectedEquipment.Capacity,
                        IsExternal = true,
                        RentType = RentType.Daily,
                        RentId = Guid.NewGuid(),
                        Color = "",
                        TyreSize = "",
                        PlateNumber = inspectedEquipment.PlateNo,
                        EngineNumber = inspectedEquipment.EngineSerialNo,
                        IFRSCondition = "",
                        ChasisNumber = inspectedEquipment.ChasisSerialNo,
                        MinFuelStandard = 0,
                        MaxFuelStandard = 10,
                        Battery = "",
                        Tyre = "",
                        RentalRate = 1,
                        IdealRentalRate = 10,
                        WayOfSupplyenum = WayOfSupplyenum.Rental, /*WayOfSupplyenum.Donated,*/
                        manufaturerenum = manufaturerenum.SINO,
                        Colorenum = Colorenum.BLACK,
                        yearenum = yearenum.EthiopianCalendar,
                        WayOfSupply = "",
                        SupplierCompany = "",
                        PreviousOdoMeter = 0,
                        CurentOdoMeter = 0,
                        LastMaintenanceOdometer = 0,
                        PurchasingCost = 0,
                        AnnualDepreciationCost = 0,
                        DepreciationCost = 0,
                        BookValue = 100,
                        SerialNo = inspectedEquipment.ChasisSerialNo,
                        Libre = "",
                        TypeOfFuelenum = TypeOfFuelenum.Benzene,
                        TypeOfFuel = "",
                        StateOfOdoMeter = "",
                        StateOfOdoMeterenum = StateOfOdoMeterenum.Functional,
                        Attachment = "",
                        FleetPhoto = "",
                        PurchasedDate = DateTime.Now,
                        ApprovalStatus = "",
                        Status = ""



                    };
                    db.Fleets.Add(equipment);
                    db.SaveChanges();
                    inspectedEquipment.IsOnlineApproved = true;
                    
                    db.Entry(inspectedEquipment).State = EntityState.Modified;
                    db.SaveChanges();
                }
                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Rental inspection ", "Success", User.Identity.Name, $"updated rental inspection {entity.RentalInspectionId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { rentalInspectionValidation }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RentalInspection_Destroy([DataSourceRequest] DataSourceRequest request, RentalInspection rentalInspection)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalInspection
                {
                    RentalInspectionId = rentalInspection.RentalInspectionId,
                    EquipmentTypeID = rentalInspection.EquipmentTypeID,
                    EquipmentNameID = rentalInspection.EquipmentNameID,
                    Model = rentalInspection.Model,
                    ChasisSerialNo = rentalInspection.ChasisSerialNo,
                    EngineModel = rentalInspection.EngineModel,
                    ManufacturingYear = rentalInspection.ManufacturingYear,
                    Capacity = rentalInspection.Capacity,
                    Hp = rentalInspection.Hp,
                    OperatorLicenceNo = rentalInspection.OperatorLicenceNo,
                    Lessor = rentalInspection.Lessor,
                    PlateNo = rentalInspection.PlateNo,
                    BranchId = rentalInspection.BranchId,
                    EngineSerialNo = rentalInspection.EngineSerialNo,
                    MeterReading = rentalInspection.MeterReading,
                    IsSendForApproval = rentalInspection.IsSendForApproval,
                    SendForApprovalBy = rentalInspection.SendForApprovalBy,
                    SendForApprovalTime = rentalInspection.SendForApprovalTime,
                    IsOnlineTransferred = rentalInspection.IsOnlineTransferred,
                    IsOnlineTransferredBy = rentalInspection.IsOnlineTransferredBy,
                    OnlineTransferredTime = rentalInspection.OnlineTransferredTime,
                    CCKW = rentalInspection.CCKW
                };

                db.RentalInspections.Attach(entity);
                db.RentalInspections.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted rental inspection {entity.RentalInspectionId }");
                #endregion
            }
            return Json(new[] { rentalInspection }.ToDataSourceResult(request, ModelState));
        }public ActionResult RentalInspectionValidation_Destroy([DataSourceRequest] DataSourceRequest request, RentalInspectionValidation rentalInspectionValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new RentalInspectionValidation
                {
                    RentalInspectionValidationId = rentalInspectionValidation.RentalInspectionValidationId,
                    RentalInspectionId = rentalInspectionValidation.RentalInspectionId,
                    Status = rentalInspectionValidation.Status,
                    Remark = rentalInspectionValidation.Remark
                };

                db.RentalInspectionValidations.Attach(entity);
                db.RentalInspectionValidations.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted rental inspection {entity.RentalInspectionId }");
                #endregion
            }
            return Json(new[] { rentalInspectionValidation }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult SendForApproval(int id)
        {
            var record = db.RentalInspections.Find(id);
            if (record != null && !record.IsSendForApproval)
            {
                record.Status = Enum.GetName(typeof(ApprovalStatuses), 0);
                record.IsSendForApproval = true;
                record.IsVoid = false;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.RentalInspections.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var entity = new RentalInspectionValidation
                {
                    RentalInspectionId = id,
                    Status = ApprovalStatuses.Pending,
                    Remark = "",
                };
                db.RentalInspectionValidations.Add(entity);
                db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}