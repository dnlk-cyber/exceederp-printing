using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.Helpers.Common;  
using System.Data.Entity;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class CheckListLogController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        // GET: /FleetManagement/CheckListLog/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DriverTransferCheckList_Read(  string FromFleetId, string ToFleetId, string Id)
        {
            List<CheckList> list = db.CheckLists.ToList();
            Fleets fleet = db.Fleets.Where(x => x.FleetsID.ToString() == FromFleetId || x.FleetsID.ToString() == ToFleetId).FirstOrDefault();
            if (fleet != null)
            {
                var CheckListlog = db.CheckListLogs.Where(x => x.AnnualEquipmentInventoryInspectionId == Id && (x.FleetId == FromFleetId || x.FleetId ==  ToFleetId)&& x.IsActive == true).ToList();

                if (CheckListlog.Any())
                {
                    return Json(CheckListlog, JsonRequestBehavior.AllowGet);
                }
                else
                { 
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DriverTransferCheckList_Read(  string Id)
        {
            List<CheckList> list = db.CheckLists.ToList();
            
                var CheckListlog = db.CheckListLogs.Where(x => x.AnnualEquipmentInventoryInspectionId == Id  && x.IsActive == true).ToList();

                if (CheckListlog.Any())
                {
                    return Json(CheckListlog, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(string.Empty, JsonRequestBehavior.AllowGet);
                }
           

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckListLog_Read([DataSourceRequest] DataSourceRequest request, string PlateNumber, string Id)
        {
            List<CheckList> list = db.CheckLists.ToList();
            Fleets fleet = db.Fleets.Where(x => x.PlateNumber == PlateNumber).FirstOrDefault();
            if (fleet != null)
            {
                var CheckListlog = db.CheckListLogs.Where(x => x.AnnualEquipmentInventoryInspectionId == Id && x.FleetId == fleet.FleetsID.ToString() && x.IsActive == true).ToList();

                if (CheckListlog.Any())
                {
                    return Json(CheckListlog.ToDataSourceResult(request));
                }
                else
                {
                    CheckListlog = db.CheckListLogs.Where(x => x.FleetId == fleet.FleetsID.ToString()).ToList();

                    return Json(CheckListlog, JsonRequestBehavior.AllowGet);
                }
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MaintenaceCheckList_Read([DataSourceRequest] DataSourceRequest request, string PlateNumber, string Id)
        {
            List<CheckList> list = db.CheckLists.Where(x=>x.IsMaintenace ==true ).ToList();
        
                List<CheckListViewModel> cline = new List<CheckListViewModel>();
                List<CheckList> checkLists = db.CheckLists.Where(x => x.IsMaintenace == true && x.EquipmentTypeId.ToString() == Id).OrderBy(x => x.Code).ToList();


                foreach (var checkList in checkLists)
                {
                    List<LineCheckList> lines = db.LineCheckLists.Where(x => x.CheckListId == checkList.CheckListId).ToList();

                    foreach (var line in lines)
                    {
                        cline.Add(new CheckListViewModel { LineId = line.LineCheckListId, CategoryName = checkList.Code + " ( " + checkList.Type.ToString() + " ) ", LineName= line.Name, IsActive = "false", IsActiveInspection = false, LineCode = line.Code });
                    }

                }
                  return Json(cline, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MaintenaceCheckListByOrder_Read([DataSourceRequest] DataSourceRequest request , string Id)
        {
            List<MaintenaceInspectionLog> list = db.MaintenaceInspectionLog.Where(x => x.JobId.ToString() == Id && x.IsAccessory == false).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MaintenaceCheckAccessory_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            List<MaintenaceInspectionLog> list = db.MaintenaceInspectionLog.Where(x => x.JobId.ToString() == Id && x.IsAccessory == true).ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EquipmentCheckListLog_Read([DataSourceRequest] DataSourceRequest request, string PlateNumber, string Id)
        {
            List<CheckList> list = db.CheckLists.ToList();
            Fleets fleet = db.Fleets.Where(x => x.PlateNumber == PlateNumber).FirstOrDefault();
            if (fleet != null)
            {
                var CheckListlog = db.CheckListLogs.Where(x => x.FleetId == fleet.FleetsID.ToString()).ToList();

                if (CheckListlog.Any())
                {
                    return Json(CheckListlog, JsonRequestBehavior.AllowGet);
                }

            }

            return Json(string.Empty, JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckListLogs_Read([DataSourceRequest] DataSourceRequest request, string PlateNumber, string Id)
        {
            List<CheckList> list = db.CheckLists.ToList();
            Fleets fleet = db.Fleets.Where(x => x.PlateNumber == PlateNumber).FirstOrDefault();
            if (fleet != null)
            {

                List<CheckListViewModel> cline = new List<CheckListViewModel>();
                List<CheckList> checkLists = db.CheckLists.OrderBy(x => x.Code).ToList();


                foreach (var checkList in checkLists)
                {
                    List<LineCheckList> lines = db.LineCheckLists.Where(x => x.CheckListId == checkList.CheckListId).ToList();

                    foreach (var line in lines)
                    {
                        cline.Add(new CheckListViewModel { LineId = line.LineCheckListId, CategoryName = checkList.Code + " ( " + checkList.Name + " ) ", IsActive = "false", IsActiveInspection = false, LineCode = line.Code });
                    }

                }
                return Json(cline.ToDataSourceResult(request));
            }


            return Json(list.ToDataSourceResult(request));
        }






        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckListLog_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<CheckListLog> CheckListLogs, string Id)
        {

            foreach (var CheckListLog in CheckListLogs)
            {
                if (CheckListLog != null)
                {
                    var entity = new CheckListLog
                    {
                        AnnualEquipmentInventoryInspectionId = Id,
                        //     CheckListId = CheckListLog.CheckListId,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };
                    db.CheckListLogs.Add(CheckListLog);
                    db.SaveChanges();
                    CheckListLog.CheckListLogId = entity.CheckListLogId;
                }
            }

            return Json(new[] { CheckListLogs }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckListLog_Update([DataSourceRequest] DataSourceRequest request, CheckListLog CheckListLog)
        {
            if (CheckListLog != null)
            {
                CheckListLog.LastModified = DateTime.Now;
                CheckListLog.ModifiedBy = User.Identity.Name;

                db.CheckListLogs.Attach(CheckListLog);
                db.Entry(CheckListLog).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { CheckListLog }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CheckListLog_Destroy([DataSourceRequest] DataSourceRequest request, CheckListLog CheckListLog)
        {
            var CheckListLogObj = db.CheckListLogs.Where(x => x.CheckListLogId == CheckListLog.CheckListLogId).FirstOrDefault();
            if (CheckListLog != null)
            {

                db.CheckListLogs.Attach(CheckListLogObj);
                db.CheckListLogs.Remove(CheckListLogObj);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted CheckListLog number " + CheckListLog.CheckListLogId);

            }

            return Json(new[] { CheckListLog }.ToDataSourceResult(request));
        }

        [AuthorizeRoles(FleetManagementRoles.ServiceUser)]
        public ActionResult MaintenaceInspectionCheckList(CheckListLogViewModel model)
        {
            try
            {
                string RefId = model.CheckListViewModel.FirstOrDefault().InspectionId;
                Guid JobId = Guid.Parse(RefId);
                List<MaintenaceInspectionLog> loges = db.MaintenaceInspectionLog.Where(x => x.JobId.ToString() == RefId)
                    .ToList();

                foreach (var log in loges)
                {
                    db.MaintenaceInspectionLog.Attach(log);
                    db.MaintenaceInspectionLog.Remove(log);
                    db.SaveChanges();
                }
                if (model.CheckListViewModel.Any())
                {
                    foreach (var m in model.CheckListViewModel)
                    {
                        bool IsFound = false;

                        if (m.IsActive == "on")
                        {
                            IsFound = true;
                            
                        }
                      var entity = new MaintenaceInspectionLog
                            {
                                MaintenaceInspectionLogId = Guid.NewGuid(),
                                Name = m.LineName,
                                JobId =  JobId,
                                IsActive = IsFound,

                            };
                            db.MaintenaceInspectionLog.Add(entity);
                            db.SaveChanges();
                        
                    }

                }
            }
            catch (Exception e)
            {

            }

            return RedirectToActionPermanent("Index", "WorkShop", new { area = "FleetManagement" });
        }


        [AuthorizeRoles(FleetManagementRoles.ServiceUser)]
        public ActionResult MaintenaceInspectionAccessoryCheckList(CheckListLogViewModel model )
        {
            try
            {
                Guid JobId = Guid.Parse(model.CheckListViewModel.FirstOrDefault().InspectionId);
                List<MaintenaceInspectionLog> loges = db.MaintenaceInspectionLog.Where(x => x.JobId == JobId)
                    .ToList();

                foreach (var log in loges)
                {
                    db.MaintenaceInspectionLog.Attach(log);
                    db.MaintenaceInspectionLog.Remove(log);
                    db.SaveChanges();
                }
                if (model.CheckListViewModel.Any())
                {
                    foreach (var m in model.CheckListViewModel)
                    {
                        bool IsFound = false;

                        if (m.IsActive == "on")
                        {
                            IsFound = true;

                        }
                        var entity = new MaintenaceInspectionLog
                        {
                            MaintenaceInspectionLogId = Guid.NewGuid(),
                            Name = m.LineName,
                            JobId = JobId,
                            IsAccessory = true,
                            IsActive = IsFound,

                        };
                        db.MaintenaceInspectionLog.Add(entity);
                        db.SaveChanges();

                    }

                }
            }
            catch (Exception e)
            {

            }

            return RedirectToActionPermanent("Index", "WorkShop", new { area = "FleetManagement" });
        }
     
        
        
        
        
        [AuthorizeRoles(FleetManagementRoles.RegistrationUser)]
        public ActionResult FleetCheckList(CheckListLogViewModel model)
        {
            try
            {
                string RefId = model.CheckListViewModel.FirstOrDefault().FleetId;
                var logs = db.CheckListLogs.Where(x => x.FleetId == RefId).ToList();
                if (logs.Any())
                {
                    foreach (var log in logs)
                    {

                        db.CheckListLogs.Attach(log);
                        db.CheckListLogs.Remove(log);
                        db.SaveChanges();

                    }
                }

                if (model.CheckListViewModel.Any())
                {
                    foreach (var m in model.CheckListViewModel)
                    {
                        if (m.IsActive == "on")
                        {
                            var entity = new CheckListLog
                            {
                                FleetId = RefId,
                                Name = m.CategoryName,
                                LineCode = m.LineCode,
                                LineName = m.LineName,
                                IsActive = true,
                                CategoryName = m.CategoryName,
                                Remark = m.Remark,
                            };
                            db.CheckListLogs.Add(entity);
                            db.SaveChanges();
                        }
                    }
                    IFleetService _FleetService = new FleetService();
                    Fleets fleet = _FleetService.GetAll().Where(x => x.FleetsID.ToString() == RefId).FirstOrDefault();
                    fleet.LastModified =DateTime.Now;
                    _FleetService.Update(fleet);

                }
            }
            catch (Exception e)
            {

            }

            return RedirectToActionPermanent("Index", "EquipmentRegistration", new { area = "FleetManagement" });
        }
        [AuthorizeRoles(FleetManagementRoles.InspectionUser)]
        public ActionResult InspectionCheckList(CheckListLogViewModel model)
        {
            if (model.CheckListViewModel.Any())
            {
                foreach (var m in model.CheckListViewModel)
                {
                    InspectionLog log = new InspectionLog();

                    log.IsActive = false;
                    if (m.IsActive == "on")
                    {
                        log.IsActive = true;
                    }
                    log.CategoryName = m.CategoryName;
                    log.LineName = m.LineCode + " - " + m.LineName;
                    log.InspectionId = m.InspectionId;
                    log.DateCreated = DateTime.Now;
                    log.CreatedBy = User.Identity.Name;

                    db.InspectionLogs.Add(log);
                    db.SaveChanges();

                }
            }

            return RedirectToAction("Index", "AnnualEquipmentInventoryInspection", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.InspectionUser)]
        public ActionResult SiteTransferCheckList(CheckListLogViewModel model)
        {
            if (model.CheckListViewModel.Any())
            {
                foreach (var m in model.CheckListViewModel)
                {
                    InspectionLog log = new InspectionLog();

                    log.IsActive = false;
                    if (m.IsActive == "on")
                    {
                        log.IsActive = true;
                    }
                    log.CategoryName = m.CategoryName;
                    log.LineName = m.LineCode + " - " + m.LineName;
                    log.Remark = m.Remark;
                    log.InspectionId = m.InspectionId;
                    log.DateCreated = DateTime.Now;
                    log.CreatedBy = User.Identity.Name;

                    db.InspectionLogs.Add(log);
                    db.SaveChanges();

                }
            }

            return RedirectToAction("Index", "SitesVehicleTransfer", new { area = "FleetManagement" });
        }
        [AuthorizeRoles(FleetManagementRoles.InspectionUser)]
        public ActionResult DriverTransferCheckList(CheckListLogViewModel model)
        {
            if (model.CheckListViewModel.Any())
            {
                foreach (var m in model.CheckListViewModel)
                {
                    InspectionLog log = new InspectionLog();

                    log.IsActive = false;
                    if (m.IsActive == "on")
                    {
                        log.IsActive = true;
                    }
                    log.CategoryName = m.CategoryName;
                    log.LineName = m.LineCode + " - " + m.LineName;
                    log.InspectionId = m.InspectionId;
                    log.Remark = m.Remark;
                    log.DateCreated = DateTime.Now;
                    log.CreatedBy = User.Identity.Name;

                    db.InspectionLogs.Add(log);
                    db.SaveChanges();

                }
            }

            return RedirectToAction("Index", "DriverTransfer", new { area = "FleetManagement" });
        }
   

    }
}

