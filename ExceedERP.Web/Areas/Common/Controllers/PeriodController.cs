﻿using System;
using System.Collections.Generic;
﻿using System.Data.Entity;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.HRM.Payroll;
﻿using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.Common.Controllers
{
    [Authorize(Roles = HR_Admin)]
   
    public class PeriodController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Admin = "HR_Admin";
        public static DateTime  StartDate { get; set; }
        public static DateTime EndDate { get; set; } 
        public static string AmharicStartDate { get; set; }
        public static string AmharicEndDate { get; set; }
        public ActionResult Index()
        {
            var periods = db.Periods.Where(x => x.PeriodType != PeriodType.PAYROLL_PERIOD);
            ViewData["PeriodList"] = periods;
            return View();
            
        }
        public  void SetDate(string name, string value)
        {
            if (name =="AmharicStartDate")
            {
                AmharicStartDate = value ;
                var splitDate= AmharicStartDate.Split ('/');
                StartDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32 (splitDate[0]), Convert.ToInt32 (splitDate[1]), Convert.ToInt32 (splitDate[2]));
            }
            else if (name =="AmharicEndDate")
            {
                AmharicEndDate = value ;
                var splitDate = AmharicEndDate.Split('/');
                EndDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
        }
        public ActionResult Periods_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Period> periods = db.Periods.Where(x => x.Remark != "Payroll" && x.PeriodType != PeriodType.PAYROLL_PERIOD); 
            DataSourceResult result = periods.ToDataSourceResult(request, period => new {
                PeriodId = period.PeriodId,
                ParentId = period.ParentId,
                PeriodName = period.PeriodName,
                PeriodType = period.PeriodType,
                AmharicStartDate = period.AmharicStartDate,
                AmharicEndDate = period.AmharicEndDate,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                Index = period.Index,
                Remark = period.Remark,
                DateCreated = period.DateCreated,
                LastModified = period.LastModified,
                CreatedBy = period.CreatedBy,
                ModifiedBy = period.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Periods_Create([DataSourceRequest]DataSourceRequest request, Period period)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Period
                {
                    PeriodName = period.PeriodName,
                    ParentId = period.ParentId,
                    PeriodType = period.PeriodType,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    AmharicStartDate = AmharicStartDate ,
                    AmharicEndDate = AmharicEndDate,
                    Index = period.Index,
                    Remark = period.Remark,
                    DateCreated = DateTime .Now ,
                    LastModified = period.LastModified,
                    CreatedBy = User .Identity .Name ,
                    ModifiedBy = period.ModifiedBy
                };

                db.Periods.Add(entity);
                db.SaveChanges();
                period.PeriodId = entity.PeriodId;;
               
            }
            //RedirectToAction("Index");
            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Periods_Update([DataSourceRequest]DataSourceRequest request, Period period)
        {
            if (ModelState.IsValid)
            {
                if (AmharicStartDate == null)
                {
                    AmharicStartDate = period .AmharicStartDate;
                    var splitDate = AmharicStartDate.Split('/');
                    StartDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                else if (AmharicEndDate==null)
                {
                    AmharicEndDate = period.AmharicEndDate;
                    var splitDate = AmharicEndDate.Split('/');
                    EndDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                var entity = new Period
                {
                    PeriodId = period.PeriodId,
                    PeriodName = period.PeriodName,
                      ParentId = period.ParentId,
                    PeriodType = period.PeriodType,
                    StartDate = StartDate,
                    EndDate = EndDate,
                    AmharicStartDate = AmharicStartDate,
                    AmharicEndDate = AmharicEndDate,
                    Index = period.Index,
                    Remark = period.Remark,
                    DateCreated = period.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = period.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };

                db.Periods.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Periods_Destroy([DataSourceRequest]DataSourceRequest request, Period period)
        {
            if (ModelState.IsValid)
            {
                var entity = new Period
                {
                    PeriodId = period.PeriodId,
                      ParentId = period.ParentId,
                    PeriodName = period.PeriodName,
                    PeriodType = period.PeriodType,
                    StartDate = period.StartDate,
                    EndDate = period.EndDate,
                    AmharicStartDate = period.AmharicStartDate,
                    AmharicEndDate = period.AmharicEndDate,
                    Index = period.Index,
                    Remark = period.Remark,
                    DateCreated = period.DateCreated,
                    LastModified = period.LastModified,
                    CreatedBy = period.CreatedBy,
                    ModifiedBy = period.ModifiedBy
                };

                db.Periods.Attach(entity);
                db.Periods.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

       

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
