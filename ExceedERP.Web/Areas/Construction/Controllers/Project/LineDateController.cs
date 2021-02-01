using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class LineDateController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var periods = db.Periods.Where(x => x.PeriodType == PeriodType.FISCAL_YEAR).AsQueryable();
            ViewData["PeriodList"] = periods;
            return View();
        }
        public JsonResult GetPeriod()
        {
            var periodList = db.Periods.Where(x => x.PeriodType == PeriodType.LIABILITY_PERIOD);
            return Json(periodList.Select(p => new { PeriodId = p.PeriodId, PeriodName = p.PeriodName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult LineDate_Read([DataSourceRequest]DataSourceRequest request, int idOne=0)
        {
            IQueryable<LineDate> linedate = db.LineDate.Where(x => x.ProjectInformationId == idOne);
            DataSourceResult result = linedate.ToDataSourceResult(request, lineDate => new {
                LineDateId = lineDate.LineDateId,
                AwardDate = lineDate.AwardDate,
                ContractPeriod = lineDate.ContractPeriod,
                ContractSigningDate = lineDate.ContractSigningDate,
                SiteHandOverDate = lineDate.SiteHandOverDate,
                EstimatedStartDate = lineDate.EstimatedStartDate,
                EstimatedEndDate = lineDate.EstimatedEndDate,
                CompletionDate = lineDate.CompletionDate,
                PeriodId = lineDate.PeriodId,
                ProjectInformationId = lineDate.ProjectInformationId,
                OrgBranchName = lineDate.OrgBranchName,
                DateCreated = lineDate.DateCreated,
                LastModified = lineDate.LastModified,
                CreatedBy = lineDate.CreatedBy,
                ModifiedBy = lineDate.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineDate_Create([DataSourceRequest]DataSourceRequest request, LineDate lineDate, int idTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new LineDate
                {
                    AwardDate = lineDate.AwardDate,
                    ContractPeriod = lineDate.ContractPeriod,
                    ContractSigningDate = lineDate.ContractSigningDate,
                    SiteHandOverDate = lineDate.SiteHandOverDate,
                    EstimatedStartDate = lineDate.EstimatedStartDate,
                    EstimatedEndDate = lineDate.EstimatedEndDate,
                    CompletionDate = lineDate.CompletionDate,
                    PeriodId = lineDate.PeriodId,
                    ProjectInformationId = idTwo,
                    OrgBranchName = lineDate.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = lineDate.LastModified,
                    CreatedBy = User .Identity .Name ,
                    ModifiedBy = lineDate.ModifiedBy 
                };

                db.LineDate.Add(entity);
                db.SaveChanges();
                lineDate.LineDateId = entity.LineDateId;
            }

            return Json(new[] { lineDate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineDate_Update([DataSourceRequest]DataSourceRequest request, LineDate lineDate)
        {
            if (ModelState.IsValid)
            {
                var entity = new LineDate
                {
                    LineDateId = lineDate.LineDateId,
                    AwardDate = lineDate.AwardDate,
                    ContractPeriod = lineDate.ContractPeriod,
                    ContractSigningDate = lineDate.ContractSigningDate,
                    SiteHandOverDate = lineDate.SiteHandOverDate,
                    EstimatedStartDate = lineDate.EstimatedStartDate,
                    EstimatedEndDate = lineDate.EstimatedEndDate,
                    CompletionDate = lineDate.CompletionDate,
                    PeriodId = lineDate.PeriodId,
                    ProjectInformationId = lineDate.ProjectInformationId,
                    OrgBranchName = lineDate.OrgBranchName,
                    DateCreated = lineDate.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = lineDate.CreatedBy,
                    ModifiedBy =User.Identity .Name 
                };

                db.LineDate.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { lineDate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineDate_Destroy([DataSourceRequest]DataSourceRequest request, LineDate lineDate)
        {
            if (ModelState.IsValid)
            {
                var entity = new LineDate
                {
                    LineDateId = lineDate.LineDateId,
                    AwardDate = lineDate.AwardDate,
                    ContractPeriod = lineDate.ContractPeriod,
                    ContractSigningDate = lineDate.ContractSigningDate,
                    SiteHandOverDate = lineDate.SiteHandOverDate,
                    EstimatedStartDate = lineDate.EstimatedStartDate,
                    EstimatedEndDate = lineDate.EstimatedEndDate,
                    CompletionDate = lineDate.CompletionDate,
                    PeriodId = lineDate.PeriodId,
                    ProjectInformationId = lineDate.ProjectInformationId,
                    OrgBranchName = lineDate.OrgBranchName,
                    DateCreated = lineDate.DateCreated,
                    LastModified = lineDate.LastModified,
                    CreatedBy = lineDate.CreatedBy,
                    ModifiedBy = lineDate.ModifiedBy 
                };

                db.LineDate.Attach(entity);
                db.LineDate.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { lineDate }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
