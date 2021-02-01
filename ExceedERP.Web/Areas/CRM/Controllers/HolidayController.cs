using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{
           [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class HolidayController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["HolidayDefinitionList"] = db.HolidayDefinitions.ToList();
            ViewData["PeriodList"] = db.Periods.ToList();
        
          return View();
        }

        public ActionResult Holidays_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CRMHoliday> holidays = db.CRMHolidays;
            DataSourceResult result = holidays.ToDataSourceResult(request, holiday => new {
                CRMHolidayID = holiday.CRMHolidayID,
                HolidayDefinitionID = holiday.HolidayDefinitionID,
                PeriodId = holiday.PeriodID,
                CreatedBy = holiday.CreatedBy,
                DateCreated = holiday.DateCreated,
                LastModified = holiday.LastModified,
                ModifiedBy = holiday.ModifiedBy,
                OrgBranchName = holiday.OrgBranchName,
                Remark = holiday.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Holidays_Create([DataSourceRequest]DataSourceRequest request, CRMHoliday holiday)
        {
 
            if (ModelState.IsValid)
            {
                var entity = new CRMHoliday
                {
                    HolidayDefinitionID = holiday.HolidayDefinitionID,
                    PeriodID = holiday.PeriodID,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = holiday.Remark
                };

                db.CRMHolidays.Add(entity);
                db.SaveChanges();
                holiday.CRMHolidayID = entity.CRMHolidayID;
            }

            return Json(new[] { holiday }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Holidays_Update([DataSourceRequest]DataSourceRequest request, CRMHoliday holiday)
        {
            if (ModelState.IsValid)
            {
                var entity = new CRMHoliday
                {
                    CRMHolidayID = holiday.CRMHolidayID,
                    HolidayDefinitionID = holiday.HolidayDefinitionID,
                    PeriodID = holiday.PeriodID,
                    Remark = holiday.Remark
                };

                db.CRMHolidays.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { holiday }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Holidays_Destroy([DataSourceRequest]DataSourceRequest request, CRMHoliday holiday)
        {
            if (ModelState.IsValid)
            {
                var entity = new CRMHoliday
                {
                    CRMHolidayID = holiday.CRMHolidayID,
                    HolidayDefinitionID = holiday.HolidayDefinitionID,
                    PeriodID = holiday.PeriodID,
                    Remark = holiday.Remark
                };

                db.CRMHolidays.Attach(entity);
                db.CRMHolidays.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Holiday  number " + holiday.CRMHolidayID);
		
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
