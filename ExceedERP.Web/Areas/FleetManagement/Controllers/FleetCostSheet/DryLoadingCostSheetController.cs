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
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Core.Domain.Finance.AP.Travel;
//using ExceedERP.Reporting.Fleet.ViewModel;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    [AuthorizeRoles(FleetManagementRoles.FleetCostSheetUser, FleetManagementRoles.FleetCostSheetApprover)]
    public class DryLoadingCostSheetController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        ExceedDbContext db = new ExceedDbContext();       
        public ActionResult Index()
        {
            ViewData["RepairTypes"] = db.RepairType.Select(s => new
            {
                Code = s.RepairTypeID,
                Name = s.Name
            }).AsQueryable();
            //ViewData["category"] = db.EquipmentIdentity.Select(s => new
            //{
            //    Text = s.EquipmentCategoryName,
            //    Value = s.EquipmentIdentityID
            //}).ToList();


            //ViewData["type"] = db.EquipmentType.Select(s => new
            //{
            //    Text = s.EquipmentTypeName,
            //    Value = s.EquipmentTypeID
            //}).ToList();
            //ViewData["name"] = db.EquipmentName.Select(s => new
            //{
            //    Text = s.Name,
            //    Value = s.EquipmentNameID
            //}).ToList();
            var fetchSetting = db.RequestToInspections.FirstOrDefault();
            string isECWC = (fetchSetting != null && fetchSetting.Remark == "ECWC") ? "ECWC" : "Not";
            ViewBag.setting = (fetchSetting != null) ? fetchSetting.Remark : "";
            // Store in ViewBag (or use a strongly typed ViewModel if preferred)
            ViewData["IsECWC"] = isECWC;
            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(
       i => new
       {
           Text = i.Name,
           Value = i.Id
       }).AsQueryable();
            ViewData["type"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["name"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
           // ViewData["fleet"] = db.Fleets.Select(x => new { x.FleetsID, x.PlateNumber }).AsQueryable();
            ViewData["fleet"] = CacheData.GetFleetsSecondCache().Select(x => new { x.FleetsID, x.PlateNumber }).AsQueryable();
            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name }).AsQueryable();

          
            ViewData["operator"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.FullName
            }).AsQueryable();
            ViewData["branch"] = CacheData.GetBranchCaches().Select(
        i => new
        {
            Name = i.Name,
            BranchId = i.Id
        }).AsQueryable();
            ViewData["businessunit"] = CacheData.GetBusinessUnitCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();


            return View();
        }
        public ActionResult DryLoadingCostSheet_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var dryLoadingTimeSheets = db.DryLoadingTimeSheets.Where(x => x.TimeSheetApproved)
                .Where(x => mybraches.Contains(x.BranchId));
            DataSourceResult result = dryLoadingTimeSheets.ToDataSourceResult(request, dryLoadingTimeSheet => new DryLoadingTimeSheet
            {
                DryLoadingTimeSheetId = dryLoadingTimeSheet.DryLoadingTimeSheetId,
                ProcurementSupplierID = dryLoadingTimeSheet.ProcurementSupplierID,
                FleetsID = dryLoadingTimeSheet.FleetsID,
                WorkItem = dryLoadingTimeSheet.WorkItem,
                WorkingHour = dryLoadingTimeSheet.WorkingHour,
                TripStartPlace = dryLoadingTimeSheet.TripStartPlace,
                TriplArrivalPlace = dryLoadingTimeSheet.TriplArrivalPlace,
                TripStartDate = dryLoadingTimeSheet.TripStartDate,
                TripArrivalDate = dryLoadingTimeSheet.TripArrivalDate,
                Asphalt = dryLoadingTimeSheet.Asphalt,
                Gravel = dryLoadingTimeSheet.Gravel,
                TotalKmTravelled = dryLoadingTimeSheet.TotalKmTravelled,
                FuelInLiter = dryLoadingTimeSheet.FuelInLiter,
                PerDiem = dryLoadingTimeSheet.PerDiem,
                IdleHour = dryLoadingTimeSheet.IdleHour,
                IdleRate = dryLoadingTimeSheet.IdleRate,
                ReasonForIdle = dryLoadingTimeSheet.ReasonForIdle,
                DateCreated = dryLoadingTimeSheet.DateCreated,
                CreatedBy = dryLoadingTimeSheet.CreatedBy,
                ModifiedBy = dryLoadingTimeSheet.ModifiedBy,
                LastModified = dryLoadingTimeSheet.LastModified,
                OilConsumption = dryLoadingTimeSheet.OilConsumption,
                AsphaltRate = dryLoadingTimeSheet.AsphaltRate,
                ExternalOperatorName = dryLoadingTimeSheet.ExternalOperatorName,
                FuelRate = dryLoadingTimeSheet.FuelRate,
                GravelRate = dryLoadingTimeSheet.GravelRate,
                LoadedType = dryLoadingTimeSheet.LoadedType,
                LoadingUnloadingCost = dryLoadingTimeSheet.LoadingUnloadingCost,
                LubricantCost = dryLoadingTimeSheet.LubricantCost,
                MaintenanceCost = dryLoadingTimeSheet.MaintenanceCost,
                NetworkingHour = dryLoadingTimeSheet.NetworkingHour,
                ReasonForDown = dryLoadingTimeSheet.ReasonForDown,
                IdleReason = dryLoadingTimeSheet.IdleReason,
                NoOfTripe = dryLoadingTimeSheet.NoOfTripe,
                Perdium = dryLoadingTimeSheet.Perdium,
                LabourCost = dryLoadingTimeSheet.LabourCost,
                BatteryCost = dryLoadingTimeSheet.BatteryCost,
                SparePartCost = dryLoadingTimeSheet.SparePartCost,
                OverTime = dryLoadingTimeSheet.OverTime,
                BasicSalary = dryLoadingTimeSheet.BasicSalary,
                Salary = dryLoadingTimeSheet.Salary,
                TotalTime = dryLoadingTimeSheet.TotalTime,
                TireCost = dryLoadingTimeSheet.TireCost,
                BranchId = dryLoadingTimeSheet.BranchId,
                BusinessUnitId = dryLoadingTimeSheet.BusinessUnitId,
                Category = dryLoadingTimeSheet.Category,
                CostSheetApproved = dryLoadingTimeSheet.CostSheetApproved,
                CostSheetApprovedBy = dryLoadingTimeSheet.CostSheetApprovedBy,
                CostSheetApprovedDate = dryLoadingTimeSheet.CostSheetApprovedDate,
                CostSheetSendForAproval = dryLoadingTimeSheet.CostSheetSendForAproval,
                CostSheetStatus =  dryLoadingTimeSheet.CostSheetStatus,
                AsphaltWithOut =  dryLoadingTimeSheet.AsphaltWithOut,
                GravelWithOut =  dryLoadingTimeSheet.GravelWithOut,                
                ProcurementSupplier = dryLoadingTimeSheet.ProcurementSupplier,
                TimeSheetApproved = dryLoadingTimeSheet.TimeSheetApproved,
                TimeSheetApprovedBy = dryLoadingTimeSheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = dryLoadingTimeSheet.TimeSheetApprovedDate,
                CostSheetSendForApprovalBy = dryLoadingTimeSheet.CostSheetSendForApprovalBy,
                CostSheetSendForApprovalTime = dryLoadingTimeSheet.CostSheetSendForApprovalTime,
                TimeSheetSendForApprovalBy = dryLoadingTimeSheet.TimeSheetSendForApprovalBy,
                TimeSheetSendForApprovalTime = dryLoadingTimeSheet.TimeSheetSendForApprovalTime,
                TimeSheetSendForAproval = dryLoadingTimeSheet.TimeSheetSendForAproval,
                TimeSheetStatus = dryLoadingTimeSheet.TimeSheetStatus,
                RentalRate = dryLoadingTimeSheet.RentalRate,
                OperatorId = dryLoadingTimeSheet.OperatorId,
                ExternalOperator = dryLoadingTimeSheet.ExternalOperator,
                EquipmentCategoryId = dryLoadingTimeSheet.EquipmentCategoryId,
                EquipmentNameId = dryLoadingTimeSheet.EquipmentNameId,
                EquipmentTypeId = dryLoadingTimeSheet.EquipmentTypeId,
                IsECWC=checkIsECWC()
                
                
            });
            return Json(result);
        }
        private bool checkIsECWC()
        {
            bool x = false;
            var fetchSetting = db.RequestToInspections.FirstOrDefault();
            if (fetchSetting!=null&&fetchSetting.Remark == "ECWC")
            {
                x = true;
            }
            return (x);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DryLoadingCostSheet_Update([DataSourceRequest] DataSourceRequest request, DryLoadingTimeSheet dryLoadingTimeSheet)
        {
            dryLoadingTimeSheet.TotalTime = 0;
            var fetchSetting = db.RequestToInspections.FirstOrDefault();

            decimal MCost = 0;
            if (fetchSetting != null && fetchSetting.Remark != "ECWC")
            {
                if (dryLoadingTimeSheet.MaintenanceCost == 0)
                {
                    var MaintenanceJob = db.JobOrder.FirstOrDefault(x => x.FleetsID == dryLoadingTimeSheet.FleetsID && x.SecondOnlineApprovedTime == DateTime.Now && x.Status == MaintenanceConst.Delivered);
                    if (MaintenanceJob != null)
                    {
                        var WorkShopJobs = db.ServiceCenterJob.Where(x => x.JobOrderID == MaintenanceJob.JobOrderID).ToList();
                        foreach (var WJ in WorkShopJobs)
                        {
                            var StoreReq = db.StoreRequisitions.FirstOrDefault(x => x.ServiceJobOrderNo == WJ.ServiceCenterJobId.ToString());
                            if (StoreReq != null)
                            {
                                var Issue = db.Issues.Where(x => x.StoreRequisitionID == StoreReq.StoreRequisitionID).ToList();
                                foreach (var issue in Issue)
                                {
                                    var issueItems = db.IssueItems.Where(x => x.IssueID == issue.IssueID);
                                    foreach (var item in issueItems)
                                    {
                                        MCost += item.IssuedQuantity * item.UnitPrice;
                                    }
                                }
                            }
                            var HumanCost = db.ServiceCrewWorkLogs.Where(x => x.JobId == WJ.ServiceCenterJobId).ToList();
                            foreach (var HC in HumanCost)
                            {
                                MCost += HC.TotalLabourCost;
                            }
                        }
                    }

                }
                if (dryLoadingTimeSheet.PerDiem == 0&&dryLoadingTimeSheet.FuelInLiter==0)
                {
                    var fetchFleet = db.Fleets
                        .AsNoTracking()
                        .FirstOrDefault(x => x.FleetsID == dryLoadingTimeSheet.FleetsID);

                    var fetchSelfCareIssue = db.StoreRequisitions
                        .Where(x => x.PlateNo == fetchFleet.PlateNumber && x.ServiceJobOrderNo == null && x.IsFuel == true&&x.IsOnlineApproved&&DbFunctions.TruncateTime(x.OnlineApprovedTime)==DbFunctions.TruncateTime(DateTime.Now))
                        .Select(x => x.StoreRequisitionID)
                        .ToList();
                    if (fetchSelfCareIssue != null)
                    {
                        var fetchIssueIds = db.Issues.Where(x => fetchSelfCareIssue.Contains(x.StoreRequisitionID) && x.IsFuel == true).Select(x => x.IssueID).ToList();
                        if (fetchIssueIds.Any())
                        {
                            var fetchItems = db.IssueItems.Where(x => fetchIssueIds.Contains(x.IssueID)).ToList();

                            if (fetchItems.Any())
                            {
                                dryLoadingTimeSheet.FuelInLiter += fetchItems.Sum(x => x.Total);
                            }
                        }
                    }
                    var fetchRequestIds = db.AssignedEquipment.Where(x => x.FleetId == dryLoadingTimeSheet.FleetsID).Select(x => x.RequestedEquipmentId).ToList();
                if (fetchRequestIds.Any())
                {
                    var fetchRequestNumbers = db.RequestedEquipments.Where(x => fetchRequestIds.Contains(x.RequestedEquipmentId)).Select(x => x.RequestNumber).ToList();
                    if (fetchRequestNumbers.Any())
                    {
                        var fetchDistributionIds = db.VehicleDistribution.Where(x => fetchRequestNumbers.Contains(x.RequestID) && x.IsOnlineApproved && DbFunctions.TruncateTime(x.OnlineApprovedTime) == DbFunctions.TruncateTime(DateTime.Now)).Select(x => x.VehicleDistributionID).ToList();
                        if (fetchDistributionIds.Any())
                        {
                            var fetchTravelOrderIds = db.ApTravelOrders.Where(x => x.VehicleDistributionID.HasValue && fetchDistributionIds.Contains(x.VehicleDistributionID.Value)).Select(x => x.ApTravelOrderId).ToList();
                            if (fetchTravelOrderIds.Any())
                            {
                                var fetchAdvancePayment = db.ApTravelOrderAdvancePayments.Where(x => fetchTravelOrderIds.Contains(x.ApTravelOrderId)).ToList();
                                if (fetchAdvancePayment.Any())
                                {
                                    foreach (var adP in fetchAdvancePayment)
                                    {
                                        if (adP.Type == TravelPaymentType.Fuel)
                                        {
                                            dryLoadingTimeSheet.FuelInLiter += adP.Total;

                                        }
                                        else
                                        {
                                            dryLoadingTimeSheet.Perdium += adP.Total;

                                        }

                                    }
                                }
                            }
                        }
                    }
                }
            }
                if (dryLoadingTimeSheet.Salary == 0)
                {
                    var fetchOperator = db.Operator.FirstOrDefault(x => x.OperatorID == dryLoadingTimeSheet.OperatorId);
                    if (fetchOperator != null)
                    {
                        var Placement = db.Placements.FirstOrDefault(x => x.EmployeeId == fetchOperator.EmployeeId && x.isCurrent == true);
                        dryLoadingTimeSheet.Salary = Placement.Salary;
                    }
                }
              
                 

            }
            if(fetchSetting!=null&&fetchSetting.Remark=="MCG")
            {
                dryLoadingTimeSheet.TotalTime = dryLoadingTimeSheet.NetworkingHour + dryLoadingTimeSheet.IdleRate * dryLoadingTimeSheet.IdleHour * 0.01m;
            }

        
          if (ModelState.IsValid)
            {
                //var companySetting = db.RequestToInspections.FirstOrDefault();
                //if (companySetting != null && companySetting.Remark == "MCG")
                //{
                //    dryLoadingTimeSheet.IdleRate = dryLoadingTimeSheet.RentalRate * (dryLoadingTimeSheet.IdleRate / 100);
                //}
                dryLoadingTimeSheet.LastModified = DateTime.Now.Date;
                dryLoadingTimeSheet.ModifiedBy = User.Identity.Name;
                dryLoadingTimeSheet.TotalTime = dryLoadingTimeSheet.TotalTime;
                //db.DryLoadingTimeSheets.Attach(dryLoadingTimeSheet);
                db.Entry(dryLoadingTimeSheet).State = EntityState.Modified;
                db.SaveChanges();
                //#region operation log
                //UserHelper.OperationLog(Request.UserHostAddress, "Dry loading timesheet update", "Success", User.Identity.Name, $"updated dry loading timesheet {dryLoadingTimeSheet.DryLoadingTimeSheetId} ", Modules.FleetManagement);
                //#endregion
            }
            return Json(new[] { dryLoadingTimeSheet }.ToDataSourceResult(request, ModelState));
        }




        #region Approval
        public JsonResult CostSheetSendForApproval(int id)
        {

            var dryLoadingCostSheet = db.DryLoadingTimeSheets.Find(id);

            if (dryLoadingCostSheet != null)
            {
                dryLoadingCostSheet.CostSheetSendForAproval = true;
                dryLoadingCostSheet.CostSheetSendForApprovalTime = DateTime.Now;
                dryLoadingCostSheet.CostSheetSendForApprovalBy = User.Identity.Name;
                dryLoadingCostSheet.CostSheetStatus = MaintenanceConst.WaitingApproval;

                db.DryLoadingTimeSheets.Attach(dryLoadingCostSheet);
                db.Entry(dryLoadingCostSheet).State = EntityState.Modified;
                db.SaveChanges();
                var returnMessage = new
                {
                    Message = "Successfully Sent!",

                };
                return Json(returnMessage.Message, JsonRequestBehavior.AllowGet);

            }

            var jsonData = new
            {
                Message = "Error Sending!",

            };
            return Json(jsonData.Message, JsonRequestBehavior.AllowGet);

        }
        public JsonResult VoidCostSheet(int id)
        {

            var dryLoadingCostSheet = db.DryLoadingTimeSheets.Find(id);

            if (dryLoadingCostSheet != null)
            {
                dryLoadingCostSheet.CostSheetSendForAproval = false;
                dryLoadingCostSheet.CostSheetApproved = false;
                dryLoadingCostSheet.CostSheetApprovedDate = DateTime.Now;
                dryLoadingCostSheet.CostSheetApprovedBy = User.Identity.Name;
                dryLoadingCostSheet.CostSheetStatus = MaintenanceConst.Rejected;

                db.DryLoadingTimeSheets.Attach(dryLoadingCostSheet);
                db.Entry(dryLoadingCostSheet).State = EntityState.Modified;
                db.SaveChanges();
                var returnMessage = new
                {
                    Message = "Successfully Sent!",

                };
                return Json(returnMessage.Message, JsonRequestBehavior.AllowGet);

            }

            var jsonData = new
            {
                Message = "Error Sending!",

            };
            return Json(jsonData.Message, JsonRequestBehavior.AllowGet);

        }


        public ActionResult DryLoadingCostSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.CostSheetApprovals.Where(x => x.CostSheetId == id && x.TimeSheetType == TimeSheetType.DryLoading);
            DataSourceResult result = approval.ToDataSourceResult(request, model => new CostSheetApproval
            {
                CostSheetId = model.CostSheetId,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                IsApproved = model.IsApproved,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                Remark = model.Remark,
                Status = model.Status,
                TimeSheetType = model.TimeSheetType,
                CostSheetApprovalId = model.CostSheetApprovalId
            });
            return Json(result);
        }

        [AuthorizeRoles(FleetManagementRoles.FleetCostSheetApprover)]
        public ActionResult DryLoadingCostSheetApproval_Create([DataSourceRequest] DataSourceRequest request, CostSheetApproval model, int id)
        {
            if (ModelState.IsValid)
            {
                if (model.Status == MaintenanceConst.Approved)
                {
                    Approved(id);
                    model.IsApproved = true;

                }
                else
                {
                    Rejected(id);
                    model.IsApproved = false;
                }
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                model.CostSheetId = id;
                model.TimeSheetType = TimeSheetType.DryLoading;
                db.CostSheetApprovals.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var dryLoading = db.DryLoadingTimeSheets.Find(id);
            if (dryLoading != null)
            {
                dryLoading.CostSheetApproved = true;
                dryLoading.CostSheetStatus = MaintenanceConst.Approved;
                dryLoading.CostSheetApprovedBy = User.Identity.Name;
                dryLoading.CostSheetApprovedDate = DateTime.Now;
                db.Entry(dryLoading).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var dryLoading = db.DryLoadingTimeSheets.Find(id);
            if (dryLoading != null)
            {
                dryLoading.CostSheetApproved = false;
                dryLoading.CostSheetStatus = MaintenanceConst.Rejected;
                dryLoading.CostSheetApprovedBy = User.Identity.Name;
                dryLoading.CostSheetApprovedDate = DateTime.Now;
                db.Entry(dryLoading).State = EntityState.Modified;
                db.SaveChanges();

            }
        }

        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}