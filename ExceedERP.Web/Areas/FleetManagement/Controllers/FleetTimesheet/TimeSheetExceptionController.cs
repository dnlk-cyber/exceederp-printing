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
    public class TimeSheetExceptionController : Controller
    {
        // GET: FleetManagement/TimeSheetSetting
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult TimeSheetException_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
           IQueryable<TimeSheetException> fetchTimeSheetException= db.TimeSheetException.Where(x=>x.TimeSheetSettingId==id);
            var Result = fetchTimeSheetException.ToDataSourceResult(request, x => new {
                DateCreated = x.DateCreated,
                CreatedBy = x.CreatedBy,
                LastModified = x.LastModified,
                ModifiedBy = x.ModifiedBy,
                BranchId=x.BranchId,
                TimeSheetExceptionId = x.TimeSheetExceptionId,
                TimeSheetSettingId = x.TimeSheetSettingId,
                Reason = x.Reason,


            });
            return Json(Result);
        }
        public ActionResult TimesheetException_Create([DataSourceRequest]DataSourceRequest request, TimeSheetException timeSheetSetting,int id)
        {
            var fetchSetting = db.TimeSheetException.FirstOrDefault(x=>x.BranchId== timeSheetSetting.BranchId);
            if(fetchSetting!=null)
            {
                ModelState.AddModelError("There is recorded","Record is already available");
            }
            if(ModelState.IsValid)
            {
                var entity = new TimeSheetException();
                entity.TimeSheetSettingId = id;
                entity.TimeSheetExceptionId = entity.TimeSheetExceptionId;
                entity.CreatedBy = User.Identity.Name;
                entity.DateCreated = DateTime.Now;
                entity.BranchId = timeSheetSetting.BranchId;
                entity.Reason = timeSheetSetting.Reason;
                db.TimeSheetException.Add(entity);
                db.SaveChanges();
            }
            return Json(new[] { timeSheetSetting }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult TimesheetException_Update([DataSourceRequest]DataSourceRequest request, TimeSheetException timeSheetSetting)
        {
            var fetchSetting = db.TimeSheetException.FirstOrDefault(x=>x.TimeSheetExceptionId==timeSheetSetting.TimeSheetExceptionId);
            if (fetchSetting==null)
            {
                ModelState.AddModelError("There is no recorded", "No record found");
            }
            if (ModelState.IsValid)
            {
                fetchSetting.BranchId = timeSheetSetting.BranchId;
                fetchSetting.Reason = timeSheetSetting.Reason;
                fetchSetting.LastModified = DateTime.Now;
                fetchSetting.ModifiedBy = User.Identity.Name;
                db.Entry(fetchSetting).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
               
            }
            return Json(new[] { timeSheetSetting }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult TimeSheetSetting_Destroy([DataSourceRequest]DataSourceRequest request, TimeSheetException timeSheetSetting)
        {
            var fetchSetting = db.TimeSheetException.FirstOrDefault(x => x.TimeSheetExceptionId == timeSheetSetting.TimeSheetExceptionId);
            if (fetchSetting==null)
            {
                ModelState.AddModelError("There is no recorded", "No record found");
            }
            if (ModelState.IsValid)
            {
                db.TimeSheetException.Remove(fetchSetting);
                db.SaveChanges();
               
            }
            return Json(new[] { timeSheetSetting }.ToDataSourceResult(request, ModelState));

        }
    }
}