using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class CalendarController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Calander_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SettingCalendar> calendars = db.SettingCalendar;
            DataSourceResult result = calendars.ToDataSourceResult(request, calendar => new
            {
                SettingCalendarId = calendar.SettingCalendarId,
                Day = calendar.Day,
                FromHour = calendar.FromHour,
                ToHour = calendar.ToHour,
                WorkingHourType = calendar.WorkingHourType,

                OrgBranchName = calendar.OrgBranchName,
                DateCreated = calendar.DateCreated,
                LastModified = calendar.LastModified,
                CreatedBy = calendar.CreatedBy,
                ModifiedBy = calendar.ModifiedBy

            });

            return Json(result);
        }
        private bool WorkingHourExists(SettingCalendar record) {
            var valueExists = false;
            var temp = db.SettingCalendar
                .FirstOrDefault(c => c.Day == record.Day && 
                               c.WorkingHourType == record.WorkingHourType && 
                               record.SettingCalendarId != c.SettingCalendarId);
            if (temp != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Calander_Create([DataSourceRequest]DataSourceRequest request, SettingCalendar calendar)
        {
            if (ModelState.IsValid)
            {
                if (WorkingHourExists(calendar))
                {
                    ModelState.AddModelError("", "Value Exists");
                }
                else
                {
                    var entity = new SettingCalendar
                    {
                        Day = calendar.Day,
                        FromHour = calendar.WorkingHourType == WorkingHourType.Morning  ? new DateTime(2018, 01, 01, 8, 30, 00) : new DateTime(2018, 01, 01, 13, 30, 00),
                        ToHour = calendar.WorkingHourType == WorkingHourType.Morning ? new DateTime(2018, 01, 01, 12, 30, 00) : new DateTime(2018, 01, 01, 17, 30, 00),
                        WorkingHourType = calendar.WorkingHourType,

                        OrgBranchName = calendar.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = calendar.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = calendar.ModifiedBy
                    };

                    db.SettingCalendar.Add(entity);
                    db.SaveChanges();
                    calendar.SettingCalendarId = entity.SettingCalendarId;
                }
                
            }

            return Json(new[] { calendar }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Calander_Update([DataSourceRequest]DataSourceRequest request, SettingCalendar calendar)
        {
            if (ModelState.IsValid)
            {
                if (WorkingHourExists(calendar))
                {
                    ModelState.AddModelError("", "Value Exists");
                }
                else
                {
                    var entity = new SettingCalendar
                    {
                        SettingCalendarId = calendar.SettingCalendarId,
                        Day = calendar.Day,
                        FromHour = calendar.WorkingHourType == WorkingHourType.Morning ? new DateTime(2018, 01, 01, 8, 30, 00) : new DateTime(2018, 01, 01, 13, 30, 00),
                        ToHour = calendar.WorkingHourType == WorkingHourType.Morning ? new DateTime(2018, 01, 01, 12, 30, 00) : new DateTime(2018, 01, 01, 17, 30, 00),
                        WorkingHourType = calendar.WorkingHourType,

                        OrgBranchName = calendar.OrgBranchName,
                        DateCreated = calendar.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = calendar.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    db.SettingCalendar.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { calendar }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Calander_Destroy([DataSourceRequest]DataSourceRequest request, SettingCalendar calendar)
        {
            if (ModelState.IsValid)
            {
                var entity = new SettingCalendar
                {
                    SettingCalendarId = calendar.SettingCalendarId,
                    Day = calendar.Day,
                    FromHour = calendar.FromHour,
                    ToHour = calendar.ToHour,
                    WorkingHourType = calendar.WorkingHourType,

                    OrgBranchName = calendar.OrgBranchName,
                    DateCreated = calendar.DateCreated,
                    LastModified = calendar.LastModified,
                    CreatedBy = calendar.CreatedBy,
                    ModifiedBy = calendar.ModifiedBy
                };

                db.SettingCalendar.Attach(entity);
                db.SettingCalendar.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { calendar }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
