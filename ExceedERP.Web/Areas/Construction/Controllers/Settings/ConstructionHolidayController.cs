using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class ConstructionHolidayController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Holidays_Read([DataSourceRequest]DataSourceRequest request, int projectId)
        {
            #region copy from general holday setting if none defined for the current project
            var generalHolidays = db.Holidays.ToList();
            var projectHolidays = db.ConstructionHolidays.Where(h => h.ProjectInformationId == projectId).ToList();
            if (generalHolidays.Any() && !projectHolidays.Any())
            {
                foreach (Holiday holiday in generalHolidays)
                {
                    var entity = new ConstructionHoliday
                    {
                        ProjectInformationId = projectId,
                        Date = holiday.Date,
                        DateHoAmhaic = holiday.DateHoAmhaic,
                        GlFiscalYearId = holiday.GlFiscalYearId,
                        HolidayId = holiday.HolidayId,
                        Name = holiday.Name,
                        Remark = holiday.Remark,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,

                    };
                    db.ConstructionHolidays.Add(entity);
                    db.SaveChanges();
                }
            }
            #endregion

            IQueryable<ConstructionHoliday> holidays = db.ConstructionHolidays.Where(h => h.ProjectInformationId == projectId);
            DataSourceResult result = holidays.ToDataSourceResult(request, holiday => new
            {
                ConstructionHolidayId = holiday.ConstructionHolidayId,
                HolidayId = holiday.HolidayId,
                ProjectInformationId = holiday.ProjectInformationId,
                GlFiscalYearId = holiday.GlFiscalYearId,

                Date = holiday.Date,
                DateHoAmhaic = holiday.DateHoAmhaic,
                Name = holiday.Name,
                Remark = holiday.Remark,

                OrgBranchName = holiday.OrgBranchName,
                DateCreated = holiday.DateCreated,
                LastModified = holiday.LastModified,
                CreatedBy = holiday.CreatedBy,
                ModifiedBy = holiday.ModifiedBy
            });

            return Json(result);
        }
        private bool DateIsInvalid(int yearId, DateTime time)
        {
            var isInvalid = false;
            var temp = db.GLFiscalYears.Find(yearId);
            if(temp != null)
            {
                if(time < temp.DateFrom || time > temp.DateTo)
                {
                    isInvalid = true;
                }
            }
            return isInvalid;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Holiday_Create([DataSourceRequest]DataSourceRequest request, ConstructionHoliday holiday, int projectId)
        {
            if (ModelState.IsValid)
            {
                if(DateIsInvalid(holiday.GlFiscalYearId, holiday.Date))
                {
                    ModelState.AddModelError("", "Date must be within the Year Range!");
                }
                else
                {
                    var entity = new ConstructionHoliday
                    {
                        //ConstructionHolidayId = holiday.ConstructionHolidayId,
                        HolidayId = holiday.HolidayId,
                        ProjectInformationId = projectId,
                        GlFiscalYearId = holiday.GlFiscalYearId,

                        Date = holiday.Date,
                        DateHoAmhaic = holiday.DateHoAmhaic,
                        Name = holiday.Name,
                        Remark = holiday.Remark,

                        OrgBranchName = holiday.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = holiday.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = holiday.ModifiedBy
                    };

                    db.ConstructionHolidays.Add(entity);
                    db.SaveChanges();
                    holiday.ConstructionHolidayId = entity.ConstructionHolidayId;


                }

            }
            return Json(new[] { holiday }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Holiday_Update([DataSourceRequest]DataSourceRequest request, ConstructionHoliday holiday)
        {
            if (ModelState.IsValid)
            {
                if (DateIsInvalid(holiday.GlFiscalYearId, holiday.Date))
                {
                    ModelState.AddModelError("", "Date must be within the Year Range!");
                }
                else
                {
                    var entity = new ConstructionHoliday
                    {
                        ConstructionHolidayId = holiday.ConstructionHolidayId,
                        HolidayId = holiday.HolidayId,
                        ProjectInformationId = holiday.ProjectInformationId,
                        GlFiscalYearId = holiday.GlFiscalYearId,

                        Date = holiday.Date,
                        DateHoAmhaic = holiday.DateHoAmhaic,
                        Name = holiday.Name,
                        Remark = holiday.Remark,

                        OrgBranchName = holiday.OrgBranchName,
                        DateCreated = holiday.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = holiday.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    db.ConstructionHolidays.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }

            }

            return Json(new[] { holiday }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Holiday_Destroy([DataSourceRequest]DataSourceRequest request, ConstructionHoliday holiday)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConstructionHoliday
                {
                    ConstructionHolidayId = holiday.ConstructionHolidayId,
                    HolidayId = holiday.HolidayId,
                    ProjectInformationId = holiday.ProjectInformationId,
                    GlFiscalYearId = holiday.GlFiscalYearId,

                    Date = holiday.Date,
                    DateHoAmhaic = holiday.DateHoAmhaic,
                    Name = holiday.Name,
                    Remark = holiday.Remark,

                    OrgBranchName = holiday.OrgBranchName,
                    DateCreated = holiday.DateCreated,
                    LastModified = holiday.LastModified,
                    CreatedBy = holiday.CreatedBy,
                    ModifiedBy = holiday.ModifiedBy
                };

                db.ConstructionHolidays.Attach(entity);
                db.ConstructionHolidays.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { holiday }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}