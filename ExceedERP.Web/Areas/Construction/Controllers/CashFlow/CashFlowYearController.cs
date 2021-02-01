using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.CashFlow
{
    public class CashFlowYearController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();


        public ActionResult Years_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<CashFlowYear> years = db.CashFlowYear.Where(y => y.ProjectInformationId == id);
            DataSourceResult result = years.ToDataSourceResult(request, year => new {
                CashFlowYearId = year.CashFlowYearId,
                GlFiscalYearId = year.GlFiscalYearId,
                ProjectInformationId = year.ProjectInformationId,
                Remark = year.Remark,

                OrgBranchName = year.OrgBranchName,
                DateCreated = year.DateCreated,
                LastModified = year.LastModified,
                CreatedBy = year.CreatedBy,
                ModifiedBy = year.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Year_Create([DataSourceRequest]DataSourceRequest request, CashFlowYear year, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowYear
                {
                    //CashFlowYearId = year.CashFlowYearId,
                    GlFiscalYearId = year.GlFiscalYearId,
                    ProjectInformationId = id,
                    Remark = year.Remark,

                    OrgBranchName = year.OrgBranchName,
                    DateCreated = year.DateCreated,
                    LastModified = year.LastModified,
                    CreatedBy = year.CreatedBy,
                    ModifiedBy = year.ModifiedBy
                };

                db.CashFlowYear.Add(entity);
                db.SaveChanges();
                year.CashFlowYearId = entity.CashFlowYearId;
            }

            return Json(new[] { year }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Year_Update([DataSourceRequest]DataSourceRequest request, CashFlowYear year)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowYear
                {
                    CashFlowYearId = year.CashFlowYearId,
                    GlFiscalYearId = year.GlFiscalYearId,
                    ProjectInformationId = year.ProjectInformationId,
                    Remark = year.Remark,

                    OrgBranchName = year.OrgBranchName,
                    DateCreated = year.DateCreated,
                    LastModified = year.LastModified,
                    CreatedBy = year.CreatedBy,
                    ModifiedBy = year.ModifiedBy
                };

                db.CashFlowYear.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { year }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Year_Destroy([DataSourceRequest]DataSourceRequest request, CashFlowYear year)
        {
            if (ModelState.IsValid)
            {
                var entity = new CashFlowYear
                {
                    CashFlowYearId = year.CashFlowYearId,
                    GlFiscalYearId = year.GlFiscalYearId,
                    ProjectInformationId = year.ProjectInformationId,
                    Remark = year.Remark,

                    OrgBranchName = year.OrgBranchName,
                    DateCreated = year.DateCreated,
                    LastModified = year.LastModified,
                    CreatedBy = year.CreatedBy,
                    ModifiedBy = year.ModifiedBy
                };

                db.CashFlowYear.Attach(entity);
                db.CashFlowYear.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { year }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}