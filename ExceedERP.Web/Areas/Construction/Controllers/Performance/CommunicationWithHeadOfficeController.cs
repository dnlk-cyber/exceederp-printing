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
    public class CommunicationWithHeadOfficeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CommunicationWithHeadOffice_Read([DataSourceRequest]DataSourceRequest request, int pIdReadComnHO)
        {
            IQueryable<MonthlyProjectCommunicationWithHeadOffice> communications = db.MonthlyProjectCommunicationWithHeadOffice.Where(x => x.MonthlyProjectCommunicationId == pIdReadComnHO);
            DataSourceResult result = communications.ToDataSourceResult(request, comn => new
            {
                MonthlyProjectCommunicationWithHeadOfficeId = comn.MonthlyProjectCommunicationWithHeadOfficeId,
                MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                CommunicationMethod = comn.CommunicationMethod,
                MaxAllowedPoints = comn.MaxAllowedPoints,
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
        public ActionResult CommunicationWithHeadOffice_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectCommunicationWithHeadOffice comn, int pIdCreateComnHO)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCommunicationWithHeadOffice
                {
                    MonthlyProjectCommunicationWithHeadOfficeId = comn.MonthlyProjectCommunicationWithHeadOfficeId,
                    MonthlyProjectCommunicationId = pIdCreateComnHO,
                    CommunicationMethod = comn.CommunicationMethod,
                    MaxAllowedPoints = comn.MaxAllowedPoints,
                    Points = comn.Points > comn.MaxAllowedPoints &&
                            comn.MaxAllowedPoints != null ?
                            (decimal)comn.MaxAllowedPoints : comn.Points,


                    OrgBranchName = comn.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = comn.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = comn.ModifiedBy
                };

                db.MonthlyProjectCommunicationWithHeadOffice.Add(entity);
                db.SaveChanges();
                comn.MonthlyProjectCommunicationWithHeadOfficeId = entity.MonthlyProjectCommunicationWithHeadOfficeId;

                PerformanceEvnHelper.UpdateMonthlyProjectComn(entity.MonthlyProjectCommunicationId);

            }
            return Json(new[] { comn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommunicationWithHeadOffice_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectCommunicationWithHeadOffice comn)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCommunicationWithHeadOffice
                {
                    MonthlyProjectCommunicationWithHeadOfficeId = comn.MonthlyProjectCommunicationWithHeadOfficeId,
                    MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                    CommunicationMethod = comn.CommunicationMethod,
                    MaxAllowedPoints = comn.MaxAllowedPoints,
                    Points = comn.Points > comn.MaxAllowedPoints &&
                            comn.MaxAllowedPoints != null ?
                            (decimal)comn.MaxAllowedPoints : comn.Points,

                    OrgBranchName = comn.OrgBranchName,
                    DateCreated = comn.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = comn.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.MonthlyProjectCommunicationWithHeadOffice.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectComn(entity.MonthlyProjectCommunicationId);

            }

            return Json(new[] { comn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommunicationWithHeadOffice_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectCommunicationWithHeadOffice comn)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCommunicationWithHeadOffice
                {
                    MonthlyProjectCommunicationWithHeadOfficeId = comn.MonthlyProjectCommunicationWithHeadOfficeId,
                    MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                    CommunicationMethod = comn.CommunicationMethod,
                    MaxAllowedPoints = comn.MaxAllowedPoints,
                    Points = comn.Points,

                    OrgBranchName = comn.OrgBranchName,
                    DateCreated = comn.DateCreated,
                    LastModified = comn.LastModified,
                    CreatedBy = comn.CreatedBy,
                    ModifiedBy = comn.ModifiedBy
                };

                db.MonthlyProjectCommunicationWithHeadOffice.Attach(entity);
                db.MonthlyProjectCommunicationWithHeadOffice.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectComn(entity.MonthlyProjectCommunicationId);

            }

            return Json(new[] { comn }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}