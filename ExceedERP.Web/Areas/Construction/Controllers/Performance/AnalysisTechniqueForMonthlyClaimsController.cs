using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Performance
{
    public class AnalysisTechniqueForMonthlyClaimsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AnalysisTechniqueForMonthlyClaims_Read([DataSourceRequest]DataSourceRequest request, int cIdReadTechnique)
        {
            IQueryable<AnalysisTechniqueApplicationForMonthlyClaims> technics = db.AnalysisTechniqueApplicationForMonthlyClaims.Where(x => x.MonthlyProjectClaimId == cIdReadTechnique);
            DataSourceResult result = technics.ToDataSourceResult(request, technique => new
            {
                AnalysisTechniqueApplicationForMonthlyClaimsId = technique.AnalysisTechniqueApplicationForMonthlyClaimsId,
                MonthlyProjectClaimId = technique.MonthlyProjectClaimId,
                WasThereIssueToClaim = technique.WasThereIssueToClaim,
                AnalysisTechnique = technique.AnalysisTechnique,
                Points = technique.Points,

                OrgBranchName = technique.OrgBranchName,
                DateCreated = technique.DateCreated,
                LastModified = technique.LastModified,
                CreatedBy = technique.CreatedBy,
                ModifiedBy = technique.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnalysisTechniqueForMonthlyClaims_Create([DataSourceRequest]DataSourceRequest request, AnalysisTechniqueApplicationForMonthlyClaims technique, int cIdCreateTechnique)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnalysisTechniqueApplicationForMonthlyClaims
                {
                    //AnalysisTechniqueApplicationForMonthlyClaimsId = technique.AnalysisTechniqueApplicationForMonthlyClaimsId,
                    MonthlyProjectClaimId = cIdCreateTechnique,
                    WasThereIssueToClaim = technique.WasThereIssueToClaim,
                    AnalysisTechnique = technique.AnalysisTechnique,
                    Points = this.CalculateScore(technique),

                    OrgBranchName = technique.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = technique.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = technique.ModifiedBy
                };

                db.AnalysisTechniqueApplicationForMonthlyClaims.Add(entity);
                db.SaveChanges();
                technique.AnalysisTechniqueApplicationForMonthlyClaimsId = entity.AnalysisTechniqueApplicationForMonthlyClaimsId;

                PerformanceEvnHelper.UpdateMonthlyProjectClaimsScore(entity.MonthlyProjectClaimId);

            }
            return Json(new[] { technique }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnalysisTechniqueForMonthlyClaims_Update([DataSourceRequest]DataSourceRequest request, AnalysisTechniqueApplicationForMonthlyClaims technique)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnalysisTechniqueApplicationForMonthlyClaims
                {
                    AnalysisTechniqueApplicationForMonthlyClaimsId = technique.AnalysisTechniqueApplicationForMonthlyClaimsId,
                    MonthlyProjectClaimId = technique.MonthlyProjectClaimId,
                    WasThereIssueToClaim = technique.WasThereIssueToClaim,
                    AnalysisTechnique = technique.AnalysisTechnique,
                    Points = this.CalculateScore(technique),


                    OrgBranchName = technique.OrgBranchName,
                    DateCreated = technique.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = technique.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.AnalysisTechniqueApplicationForMonthlyClaims.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectClaimsScore(entity.MonthlyProjectClaimId);

            }

            return Json(new[] { technique }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnalysisTechniqueForMonthlyClaims_Destroy([DataSourceRequest]DataSourceRequest request, AnalysisTechniqueApplicationForMonthlyClaims technique)
        {
            if (ModelState.IsValid)
            {
                var entity = new AnalysisTechniqueApplicationForMonthlyClaims
                {
                    AnalysisTechniqueApplicationForMonthlyClaimsId = technique.AnalysisTechniqueApplicationForMonthlyClaimsId,
                    MonthlyProjectClaimId = technique.MonthlyProjectClaimId,
                    WasThereIssueToClaim = technique.WasThereIssueToClaim,
                    AnalysisTechnique = technique.AnalysisTechnique,
                    Points = technique.Points,

                    OrgBranchName = technique.OrgBranchName,
                    DateCreated = technique.DateCreated,
                    LastModified = technique.LastModified,
                    CreatedBy = technique.CreatedBy,
                    ModifiedBy = technique.ModifiedBy
                };

                db.AnalysisTechniqueApplicationForMonthlyClaims.Attach(entity);
                db.AnalysisTechniqueApplicationForMonthlyClaims.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectClaimsScore(entity.MonthlyProjectClaimId);

            }

            return Json(new[] { technique }.ToDataSourceResult(request, ModelState));
        }
        private decimal CalculateScore(AnalysisTechniqueApplicationForMonthlyClaims model) {
            decimal point = new decimal();
            if (model.WasThereIssueToClaim)
            {
                var tech = model.AnalysisTechnique;
                if( tech == AnalysisTechniqueForMonthlyClaims.Appropriate_Delay_Analysis_Technics)
                {
                    point = 2;
                }
                else if(tech == AnalysisTechniqueForMonthlyClaims.Delay_Analysis_Technic_Applied)
                {
                    point = 1;
                }
                else if(tech == AnalysisTechniqueForMonthlyClaims.No_Delay_Analysis_Technic_Applied)
                {
                    point = 0;
                }
            }
            return point;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}