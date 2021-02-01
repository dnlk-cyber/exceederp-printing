using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Finance.GL;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class HolidayController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string DateAmahic { get; set; }
        public static DateTime Date { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {

            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateHoAmhaic")
            {
                DateAmahic = value;
                var splitDate = DateAmahic.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }


        }
        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
        public ActionResult Holiday_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Holiday> holidays = db.Holidays;
            DataSourceResult result = holidays.ToDataSourceResult(request, holiday => new
            {
                HolidayId = holiday.HolidayId,
                Date = holiday.Date,
                DateHoAmhaic = holiday.DateHoAmhaic,
                GlFiscalYearId = holiday.GlFiscalYearId,
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

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Holiday_Create([DataSourceRequest]DataSourceRequest request, Holiday holiday)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmahic != null)
                {
                    var checkexistance =
                        db.Holidays.Where(x => x.GlFiscalYearId == holiday.GlFiscalYearId && x.Date == Date)
                            .FirstOrDefault();
                    if (checkexistance != null)
                    {
                        ModelState.AddModelError("", "This Holiday date is already registered!!");
                    }

                    else
                    {



                        var entity = new Holiday
                        {
                            Date = Date,
                            DateHoAmhaic = DateAmahic,
                            Name = holiday.Name,
                            Remark = holiday.Remark,
                            GlFiscalYearId = holiday.GlFiscalYearId,
                            OrgBranchName = holiday.OrgBranchName,
                            DateCreated = holiday.DateCreated,
                            LastModified = holiday.LastModified,
                            CreatedBy = holiday.CreatedBy,
                            ModifiedBy = holiday.ModifiedBy
                        };

                        db.Holidays.Add(entity);
                        db.SaveChanges();
                        holiday.HolidayId = entity.HolidayId;

                        #region operation log

                        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                        
                            UserHelper.OperationLog(Request.UserHostAddress, "Holiday ", "Success", User.Identity.Name, User.Identity.Name + " Add new Holiday in holidays list " + entity.Name,Modules.HRM);

                        

                        #endregion
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Date value is mandatory!!");
                }
            }

            return Json(new[] { holiday }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Holiday_Update([DataSourceRequest]DataSourceRequest request, Holiday holiday)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (DateAmahic == null)
                {
                    DateAmahic = holiday.DateHoAmhaic;
                    var splitDate = DateAmahic.Split('/');
                    Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                    var entity = new Holiday
                    {
                        HolidayId = holiday.HolidayId,
                        Date = Date,
                        DateHoAmhaic = holiday.DateHoAmhaic,
                        Name = holiday.Name,
                        GlFiscalYearId = holiday.GlFiscalYearId,
                        Remark = holiday.Remark,
                        OrgBranchName = holiday.OrgBranchName,
                        DateCreated = holiday.DateCreated,
                        LastModified = holiday.LastModified,
                        CreatedBy = holiday.CreatedBy,
                        ModifiedBy = holiday.ModifiedBy
                    };

                    db.Holidays.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
              
                    UserHelper.OperationLog(Request.UserHostAddress, "Holiday ", "Success", User.Identity.Name, User.Identity.Name + " Updated Holiday Info of " + entity.Name,Modules.HRM);

                #endregion

            }

            return Json(new[] { holiday }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Holiday_Destroy([DataSourceRequest]DataSourceRequest request, Holiday holiday)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Holiday
                {
                    HolidayId = holiday.HolidayId,
                    Date = holiday.Date,
                    DateHoAmhaic = holiday.DateHoAmhaic,
                    GlFiscalYearId = holiday.GlFiscalYearId,
                    Name = holiday.Name,
                    Remark = holiday.Remark,
                    OrgBranchName = holiday.OrgBranchName,
                    DateCreated = holiday.DateCreated,
                    LastModified = holiday.LastModified,
                    CreatedBy = holiday.CreatedBy,
                    ModifiedBy = holiday.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Holiday Setting " + holiday.Name);
                #endregion
                db.Holidays.Attach(entity);
                db.Holidays.Remove(entity);
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
