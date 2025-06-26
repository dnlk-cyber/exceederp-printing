using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetTimesheet
{
    public class TimeSheetSettingController : Controller
    {
        // GET: FleetManagement/TimeSheetSetting
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["branch"] = db.Branch.Select(s => new
            {
                Value = s.BranchId,
                Text = s.Name
            }).ToList();
            return View();
        }
        public ActionResult TimeSheetSetting_Read([DataSourceRequest]DataSourceRequest request)
        {
           IQueryable<TimeSheetSetting> fetchTimeSheetSetting = db.TimeSheetSetting;
            var Result = fetchTimeSheetSetting.ToDataSourceResult(request, x => new {

                DateCreated = x.DateCreated,
                CreatedBy = x.CreatedBy,
                LastModified = x.LastModified,
                ModifiedBy = x.ModifiedBy,
                TimeSheetSettingId = x.TimeSheetSettingId,
                SettedHours = x.SettedHours,
                IsSet = x.IsSet


            });
            return Json(Result);
        }
        public ActionResult TimeSheetSetting_Create([DataSourceRequest]DataSourceRequest request,TimeSheetSetting timeSheetSetting)
        {
            var fetchSetting = db.TimeSheetSetting.ToList();
            if(fetchSetting.Any())
            {
                ModelState.AddModelError("There is recorded","Record is already available");
            }
            if(ModelState.IsValid)
            {
                var entity = new TimeSheetSetting();
                entity.TimeSheetSettingId = entity.TimeSheetSettingId;
                entity.CreatedBy = User.Identity.Name;
                entity.DateCreated = DateTime.Now;
                entity.IsSet = true;
                entity.SettedHours = timeSheetSetting.SettedHours;
                db.TimeSheetSetting.Add(entity);
                db.SaveChanges();
            }
            return Json(new[] { timeSheetSetting }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult TimeSheetSetting_Update([DataSourceRequest]DataSourceRequest request, TimeSheetSetting timeSheetSetting)
        {
            var fetchSetting = db.TimeSheetSetting.FirstOrDefault(x=>x.TimeSheetSettingId==timeSheetSetting.TimeSheetSettingId);
            if (fetchSetting==null)
            {
                ModelState.AddModelError("There is no recorded", "No record found");
            }
            if (ModelState.IsValid)
            {
                fetchSetting.SettedHours = timeSheetSetting.SettedHours;
                fetchSetting.IsSet = timeSheetSetting.IsSet;
                fetchSetting.LastModified = DateTime.Now;
                fetchSetting.ModifiedBy = User.Identity.Name;
                db.Entry(fetchSetting).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
               
            }
            return Json(new[] { timeSheetSetting }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult TimeSheetSetting_Destroy([DataSourceRequest]DataSourceRequest request, TimeSheetSetting timeSheetSetting)
        {
            var fetchSetting = db.TimeSheetSetting.FirstOrDefault(x=>x.TimeSheetSettingId==timeSheetSetting.TimeSheetSettingId);
            if (fetchSetting==null)
            {
                ModelState.AddModelError("There is no recorded", "No record found");
            }
            if (ModelState.IsValid)
            {
                db.TimeSheetSetting.Remove(fetchSetting);
                db.SaveChanges();
               
            }
            return Json(new[] { timeSheetSetting }.ToDataSourceResult(request, ModelState));

        }
    }
}