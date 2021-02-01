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
    public class CommunicationWithConsultantController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CommunicationWithConsultant_Read([DataSourceRequest]DataSourceRequest request, int pIdReadComnC)
        {
            IQueryable<MonthlyProjectCommunicationWithConsultant> communications = db.MonthlyProjectCommunicationWithConsultant.Where(x => x.MonthlyProjectCommunicationId == pIdReadComnC);
            DataSourceResult result = communications.ToDataSourceResult(request, comn => new
            {
                MonthlyProjectCommunicationWithConsultantId = comn.MonthlyProjectCommunicationWithConsultantId,
                MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                CommunicationLevel = comn.CommunicationLevel,
                Description = comn.Description,
                Points = comn.Points,

                OrgBranchName = comn.OrgBranchName,
                DateCreated = comn.DateCreated,
                LastModified = comn.LastModified,
                CreatedBy = comn.CreatedBy,
                ModifiedBy = comn.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommunicationWithConsultant_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectCommunicationWithConsultant comn, int pIdCreateComnC)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCommunicationWithConsultant
                {
                    //MonthlyProjectCommunicationWithConsultantId = comn.MonthlyProjectCommunicationWithConsultantId,
                    MonthlyProjectCommunicationId = pIdCreateComnC,
                    CommunicationLevel = comn.CommunicationLevel,
                    Description = comn.Description,
                    Points = this.CalculateScoredPoints(comn),

                    OrgBranchName = comn.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = comn.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = comn.ModifiedBy
                };

                db.MonthlyProjectCommunicationWithConsultant.Add(entity);
                db.SaveChanges();
                comn.MonthlyProjectCommunicationWithConsultantId = entity.MonthlyProjectCommunicationWithConsultantId;

                PerformanceEvnHelper.UpdateMonthlyProjectComn(entity.MonthlyProjectCommunicationId);
            }
            return Json(new[] { comn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommunicationWithConsultant_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectCommunicationWithConsultant comn)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCommunicationWithConsultant
                {
                    MonthlyProjectCommunicationWithConsultantId = comn.MonthlyProjectCommunicationWithConsultantId,
                    MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                    CommunicationLevel = comn.CommunicationLevel,
                    Description = comn.Description,
                    Points = this.CalculateScoredPoints(comn),

                    OrgBranchName = comn.OrgBranchName,
                    DateCreated = comn.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = comn.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.MonthlyProjectCommunicationWithConsultant.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectComn(entity.MonthlyProjectCommunicationId);

            }

            return Json(new[] { comn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommunicationWithConsultant_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectCommunicationWithConsultant comn)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCommunicationWithConsultant
                {
                    MonthlyProjectCommunicationWithConsultantId = comn.MonthlyProjectCommunicationWithConsultantId,
                    MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                    CommunicationLevel = comn.CommunicationLevel,
                    Description = comn.Description,
                    Points = comn.Points,

                    OrgBranchName = comn.OrgBranchName,
                    DateCreated = comn.DateCreated,
                    LastModified = comn.LastModified,
                    CreatedBy = comn.CreatedBy,
                    ModifiedBy = comn.ModifiedBy
                };

                db.MonthlyProjectCommunicationWithConsultant.Attach(entity);
                db.MonthlyProjectCommunicationWithConsultant.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectComn(entity.MonthlyProjectCommunicationId);

            }

            return Json(new[] { comn }.ToDataSourceResult(request, ModelState));
        }
        private decimal CalculateScoredPoints(MonthlyProjectCommunicationWithConsultant model)
        {
            var comn = model.CommunicationLevel;
            var pt = new decimal();
            if (comn == MonthlyProjectCommunicationLevel.Very_Good)
            {
                pt = 2;
            }
            else if (comn == MonthlyProjectCommunicationLevel.Good)
            {
                pt = 1;
            }
            else if (comn == MonthlyProjectCommunicationLevel.Not_Good)
            {
                pt = 0;
            }
            else if (comn == MonthlyProjectCommunicationLevel.Poor)
            {
                pt = -1;
            }
            return pt;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}