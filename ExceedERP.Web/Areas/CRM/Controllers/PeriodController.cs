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
using ExceedERP.Web.Areas.CRM.Models;
 using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class PeriodController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var periods = db.Periods;
            ViewData["PeriodList"] = periods;
            return View();

        }

        public ActionResult Periods_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Period> periods = db.Periods;
            DataSourceResult result = periods.ToDataSourceResult(request, period => new
            {
                PeriodId = period.PeriodId,
                ParentId = period.ParentId,
                PeriodName = period.PeriodName,
                PeriodType = period.PeriodType,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                Index = period.Index,
                CreatedBy = period.CreatedBy,
                DateCreated = period.DateCreated,
                LastModified = period.LastModified,
                ModifiedBy = period.ModifiedBy,
                OrgBranchName = period.OrgBranchName,
                Remark = period.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Periods_Create([DataSourceRequest]DataSourceRequest request, Period period)
        {
            ModelState.Clear();

            List<Period> periodList = db.Periods.Where(ocl => ocl.Index == period.Index).ToList();
            List<Period> periodLists = db.Periods.Where(ocl => ocl.PeriodName == period.PeriodName && ocl.PeriodType == period.PeriodType).ToList();

            if (periodList.Count>0)
            {
                ModelState.AddModelError("Invalid Index Value", "Please readjust index value!");
            }
            else if (period.StartDate > period.EndDate)
            {
                ModelState.AddModelError("Warning", "Starting date should be greater than ending date!");
            }
            else if (periodLists.Count>0)
            {
                ModelState.AddModelError("Warning", "Period Name Already Exists !");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new Period
                    {
                        PeriodName = period.PeriodName,
                        ParentId = period.ParentId,
                        PeriodType = period.PeriodType,
                        StartDate = period.StartDate,
                        EndDate = period.EndDate,
                        Index = period.Index,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Remark = period.Remark
                    };

                    db.Periods.Add(entity);
                    db.SaveChanges();
                
                }
                RedirectToAction("Index");
            }
            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Periods_Update([DataSourceRequest]DataSourceRequest request, Period period)
        {
              List<Period> periodList = db.Periods.Where(ocl => ocl.Index == period.Index).ToList();
            List<Period> periodLists = db.Periods.Where(ocl => ocl.PeriodName == period.PeriodName).ToList();

            if (periodList.Count>0)
            {
                ModelState.AddModelError("Invalid Index Value", "Please readjust index value!");
            }
            else if (period.StartDate > period.EndDate)
            {
                ModelState.AddModelError("Warning", "Starting date should be greater than ending date!");
            }
            else if (periodLists.Count>0)
            {
                ModelState.AddModelError("Warning", "Period Name Already Exists !");
            }
                     
            else
            {

                if (ModelState.IsValid)
                {
                    var entity = new Period
                    {
                        PeriodId = period.PeriodId,
                        PeriodName = period.PeriodName,
                        ParentId = period.ParentId,
                        PeriodType = period.PeriodType,
                        StartDate = period.StartDate,
                        EndDate = period.EndDate,
                        Index = period.Index,
                        Remark = period.Remark
                    };

                    db.Periods.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
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
                    Index = period.Index,
                    Remark = period.Remark
                };

                db.Periods.Attach(entity);
                db.Periods.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Period  number " + period.PeriodId);
      
            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult PeriodDDBind()
        {

            var customers = db.Periods.Select(s => new PeriodViewModel
            {
                PeriodName = s.PeriodName,
                PeriodId = s.PeriodId
            });

            return Json(customers, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
