﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Finance.GL;
﻿using ExceedERP.DataAccess.Context;
 using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Performance
{
  
    [AuthorizeRoles(HrmConstants.HRPerformanceAdmin, HrmConstants.HRAdmin)]
    public class EvaluationPeriodController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime To { get; set; }
        public static string ToAmharic { get; set; }
        public static DateTime From { get; set; }

        public static string FromAmharic { get; set; }
        public ActionResult Index()
        {
            GetPeriod();
            return View();
        }
        public JsonResult GetPeriod()
        {
            var periodList = db.GLFiscalYears;
            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new { GlFiscalYearId = p.GlFiscalYearId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateFromAmharic")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "DateToAmharic")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }


        }
        public ActionResult GLPeriods_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLPeriod> glperiods = db.GLPeriods;
            DataSourceResult result = glperiods.ToDataSourceResult(request, gLPeriod => new {
                GLPeriodId = gLPeriod.GLPeriodId,
                GlFiscalYearId = gLPeriod.GlFiscalYearId,
                Name = gLPeriod.Name,
                DateFrom = gLPeriod.DateFrom,
                DateTo = gLPeriod.DateTo,
                DateFromAmharic = gLPeriod.DateFromAmharic,
                DateToAmharic = gLPeriod.DateToAmharic,
                Status = gLPeriod.Status,
                IsPermanentlyClosed = gLPeriod.IsPermanentlyClosed,
                OrgBranchName = gLPeriod.OrgBranchName,
                DateCreated = gLPeriod.DateCreated,
                LastModified = gLPeriod.LastModified,
                CreatedBy = gLPeriod.CreatedBy,
                ModifiedBy = gLPeriod.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLPeriods_Create([DataSourceRequest]DataSourceRequest request, GLPeriod gLPeriod)
        {
            if (ModelState.IsValid)
            {
                if (FromAmharic == null || ToAmharic == null)
                {
                    ModelState.AddModelError("", "Date form and to values are mandatory!!");
                }
                else if (From.Date > To.Date)
                {
                    ModelState.AddModelError("", "Please Check the date range!!");
                }
                else
                {
                    var entity = new GLPeriod
                    {
                        GlFiscalYearId = gLPeriod.GlFiscalYearId,
                        Name = gLPeriod.Name,
                        DateFrom = From,
                        DateTo = To,
                        DateFromAmharic = FromAmharic,
                        DateToAmharic = ToAmharic,
                        Status = gLPeriod.Status,
                        IsPermanentlyClosed = gLPeriod.IsPermanentlyClosed,
                        OrgBranchName = gLPeriod.OrgBranchName,
                        DateCreated = gLPeriod.DateCreated,
                        LastModified = gLPeriod.LastModified,
                        CreatedBy = gLPeriod.CreatedBy,
                        ModifiedBy = gLPeriod.ModifiedBy
                    };

                    db.GLPeriods.Add(entity);
                    db.SaveChanges();
                    gLPeriod.GLPeriodId = entity.GLPeriodId;
                }
            }

            return Json(new[] { gLPeriod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLPeriods_Update([DataSourceRequest]DataSourceRequest request, GLPeriod gLPeriod)
        {
            if (ModelState.IsValid)
            { if (FromAmharic == null)
                {
                    FromAmharic = gLPeriod.DateFromAmharic;
                    var splitDate = FromAmharic.Split('/');
                    From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]),
                        Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (ToAmharic == null)
                {
                    ToAmharic = gLPeriod.DateToAmharic;
                    var splitDate = FromAmharic.Split('/');
                    To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]),
                        Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (From.Date > To.Date)
                {
                    ModelState.AddModelError("", "Please Check the date range!!");
                }
                else
                {
                    var entity = new GLPeriod
                    {
                        GlFiscalYearId = gLPeriod.GlFiscalYearId,
                        GLPeriodId = gLPeriod.GLPeriodId,
                        Name = gLPeriod.Name,
                        DateFrom =From,
                        DateTo = To,
                        DateFromAmharic = FromAmharic,
                        DateToAmharic = ToAmharic,
                        Status = gLPeriod.Status,
                        IsPermanentlyClosed = gLPeriod.IsPermanentlyClosed,
                        OrgBranchName = gLPeriod.OrgBranchName,
                        DateCreated = gLPeriod.DateCreated,
                        LastModified = gLPeriod.LastModified,
                        CreatedBy = gLPeriod.CreatedBy,
                        ModifiedBy = gLPeriod.ModifiedBy
                    };

                    db.GLPeriods.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            return Json(new[] { gLPeriod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLPeriods_Destroy([DataSourceRequest]DataSourceRequest request, GLPeriod gLPeriod)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLPeriod
                {
                    GlFiscalYearId = gLPeriod.GlFiscalYearId,
                    GLPeriodId = gLPeriod.GLPeriodId,
                    Name = gLPeriod.Name,
                    DateFrom = gLPeriod.DateFrom,
                    DateTo = gLPeriod.DateTo,
                    DateFromAmharic = gLPeriod.DateFromAmharic,
                    DateToAmharic = gLPeriod.DateToAmharic,
                    Status = gLPeriod.Status,
                    IsPermanentlyClosed = gLPeriod.IsPermanentlyClosed,
                    OrgBranchName = gLPeriod.OrgBranchName,
                    DateCreated = gLPeriod.DateCreated,
                    LastModified = gLPeriod.LastModified,
                    CreatedBy = gLPeriod.CreatedBy,
                    ModifiedBy = gLPeriod.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Period " + gLPeriod.Name);
                #endregion
                db.GLPeriods.Attach(entity);
                db.GLPeriods.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { gLPeriod }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
