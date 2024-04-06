using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin, HrmConstants.HRAdmin)]
    public class QuarterLengthController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            var fisaclYear = db.GLFiscalYears.ToList();
            ViewData["PeriodList"] = fisaclYear;
        }
        public JsonResult GetPeriod()
        {
            var periods = db.GLPeriods.ToList();

            return Json(periods.Select(p => new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult QuarterLength_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<QuarterLength> quarterLength = db.QuarterLengths;
            DataSourceResult result = quarterLength.ToDataSourceResult(request, QuarterLength => new
            {
                QuarterLengthId = QuarterLength.QuarterLengthId,
                Length = QuarterLength.Length,
                Remark = QuarterLength.Remark,
                GlFiscalYearId = QuarterLength.GlFiscalYearId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuarterLength_Create([DataSourceRequest]DataSourceRequest request, QuarterLength QuarterLength)
        {

            var checkExistance = db.QuarterLengths.FirstOrDefault(x => x.GlFiscalYearId == QuarterLength.GlFiscalYearId);
            if (checkExistance != null)
            {
                ModelState.AddModelError("", "This Fiscal Year is already registred!!");
            }
            if (QuarterLength.Length != 1 && QuarterLength.Length != 2 && QuarterLength.Length != 3 && QuarterLength.Length != 4 && QuarterLength.Length != 12)
            {
                ModelState.AddModelError("","Quarter length should eqluas one of the folowing 1/Yearly, 2/Half Year, 3/ 1/3 Year, 4/Quarterly ,12 Monthly");
            }
            if (ModelState.IsValid)
            {
                var entity = new QuarterLength
                {
                    //QuarterLengthId = QuarterLength.QuarterLengthId,
                    Length = QuarterLength.Length,
                    Remark = QuarterLength.Remark,
                    GlFiscalYearId = QuarterLength.GlFiscalYearId,
                };

                db.QuarterLengths.Add(entity);
                db.SaveChanges();
                QuarterLength.QuarterLengthId = entity.QuarterLengthId;

                GenerateQuarter(QuarterLength.Length, QuarterLength.QuarterLengthId);

                #region operation log
                var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == entity.GlFiscalYearId);
                if (year != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "QuarterLength", "Success", User.Identity.Name, User.Identity.Name + " add new Quarter length for fiscal year " + year.Name + " date " + DateTime.Today.ToShortDateString(), Modules.HRM);
                }
                #endregion
            }

            return Json(new[] { QuarterLength }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuarterLength_Update([DataSourceRequest]DataSourceRequest request, QuarterLength QuarterLength)
        {
            if (ModelState.IsValid)
            {
                var entity = new QuarterLength
                {
                    QuarterLengthId = QuarterLength.QuarterLengthId,
                    Length = QuarterLength.Length,
                    Remark = QuarterLength.Remark,
                    GlFiscalYearId = QuarterLength.GlFiscalYearId,

                };

                db.QuarterLengths.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == entity.GlFiscalYearId);
                if (year != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "QuarterLength", "Success", User.Identity.Name, User.Identity.Name + " update Quarter length of fiscal year " + year.Name + " date " + DateTime.Today.ToShortDateString(), Modules.HRM);
                }
                #endregion

            }

            return Json(new[] { QuarterLength }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuarterLength_Destroy([DataSourceRequest]DataSourceRequest request, QuarterLength QuarterLength)
        {
            var usedQurterList = db.Quarters.Where(x => x.QuarterLengthId == QuarterLength.QuarterLengthId)
                .Select(x => x.QuarterId);
            //var checkExistance = db.EmployeeMeasurementDefinition.Where(x => usedQurterList.Contains(x.QuarterId));
            //if (checkExistance.Any())
            //{
            //    ModelState.AddModelError("",
            //        "You can not delete this quarter length. \nUnder this qurter there is evaluated employees!!");
            //}
            if (ModelState.IsValid)
            {
                var entity = new QuarterLength
                {
                    QuarterLengthId = QuarterLength.QuarterLengthId,
                    Length = QuarterLength.Length,
                    Remark = QuarterLength.Remark,
                    GlFiscalYearId = QuarterLength.GlFiscalYearId,

                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Quarter length Id " + QuarterLength.QuarterLengthId);
                #endregion
                db.QuarterLengths.Attach(entity);
                db.QuarterLengths.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { QuarterLength }.ToDataSourceResult(request, ModelState));
        }

        private void GenerateQuarter(int length, int quarterLId)
        {
            string amname = "ኛ ሩብ ዓመት";
            string name = "Qurter ";

            var yearId = db.QuarterLengths.FirstOrDefault(x => x.QuarterLengthId == quarterLId);
            if (yearId != null)
            {
                var year = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == yearId.GlFiscalYearId);
                if (year != null)
                {

                    for (int i = 1; i <= length; i++)
                    {
                        var entity = new Quarter();

                        amname = "ኛ ሩብ ዓመት ";
                        name = "Qurter ";

                        entity.QuarterLengthId = quarterLId;

                        if (length == 1)
                        {
                            name = "Yearly ";
                            amname = "ዓመታዊ ";
                        }
                        else if (length == 2)
                        {
                            name = "Half Year " + i;
                            amname = i + "ኛ ግማሽ ዓመት ";
                        }
                        else if (length == 3)
                        {
                            name = "1/3 Year " + i;
                            amname = i + " ኛ ሲሶ ዓመት ";
                        }
                        else if (length == 4)
                        {
                            name = "Quarter " + i;
                            amname = i + "ኛ ሩብ አመት ";
                        }
                        else if (length == 12)
                        {
                            name = "Month " + i;
                            amname = i + "ኛ ወር ";
                        }
                        
                        entity.Name = name;
                        entity.AmharicName = amname;
                        entity.Sequence = i;
                        db.Quarters.Add(entity);
                        db.SaveChanges();

                    }
                }

            }
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
