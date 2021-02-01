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
    public class ContempraryRecordController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ContempraryRecord_Read([DataSourceRequest]DataSourceRequest request, int cIdReadRecord)
        {
            IQueryable<ContempraryRecord> records = db.ContempraryRecord.Where(x => x.MonthlyProjectClaimId == cIdReadRecord);
            DataSourceResult result = records.ToDataSourceResult(request, record => new
            {
                ContempraryRecordId = record.ContempraryRecordId,
                MonthlyProjectClaimId = record.MonthlyProjectClaimId,
                Description = record.Description,
                MaxAllowablePoints = record.MaxAllowablePoints,
                Points = record.Points,

                OrgBranchName = record.OrgBranchName,
                DateCreated = record.DateCreated,
                LastModified = record.LastModified,
                CreatedBy = record.CreatedBy,
                ModifiedBy = record.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContempraryRecord_Create([DataSourceRequest]DataSourceRequest request, ContempraryRecord record, int cIdCreateRecord)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContempraryRecord
                {
                    ContempraryRecordId = record.ContempraryRecordId,
                    MonthlyProjectClaimId = cIdCreateRecord,
                    Description = record.Description,
                    MaxAllowablePoints = record.MaxAllowablePoints,
                    Points = record.MaxAllowablePoints != null &&
                             record.Points > record.MaxAllowablePoints ?
                             (decimal)record.MaxAllowablePoints : record.Points,

                    OrgBranchName = record.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = record.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = record.ModifiedBy
                };

                db.ContempraryRecord.Add(entity);
                db.SaveChanges();
                record.ContempraryRecordId = entity.ContempraryRecordId;

                PerformanceEvnHelper.UpdateMonthlyProjectClaimsScore(entity.MonthlyProjectClaimId);

            }
            return Json(new[] { record }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContempraryRecord_Update([DataSourceRequest]DataSourceRequest request, ContempraryRecord record)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContempraryRecord
                {
                    ContempraryRecordId = record.ContempraryRecordId,
                    MonthlyProjectClaimId = record.MonthlyProjectClaimId,
                    Description = record.Description,
                    MaxAllowablePoints = record.MaxAllowablePoints,
                    Points = record.MaxAllowablePoints != null &&
                             record.Points > record.MaxAllowablePoints ?
                             (decimal)record.MaxAllowablePoints : record.Points,

                    OrgBranchName = record.OrgBranchName,
                    DateCreated = record.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = record.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ContempraryRecord.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectClaimsScore(entity.MonthlyProjectClaimId);

            }

            return Json(new[] { record }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContempraryRecord_Destroy([DataSourceRequest]DataSourceRequest request, ContempraryRecord record)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContempraryRecord
                {
                    ContempraryRecordId = record.ContempraryRecordId,
                    MonthlyProjectClaimId = record.MonthlyProjectClaimId,
                    Description = record.Description,
                    MaxAllowablePoints = record.MaxAllowablePoints,
                    Points = record.Points,


                    OrgBranchName = record.OrgBranchName,
                    DateCreated = record.DateCreated,
                    LastModified = record.LastModified,
                    CreatedBy = record.CreatedBy,
                    ModifiedBy = record.ModifiedBy
                };

                db.ContempraryRecord.Attach(entity);
                db.ContempraryRecord.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectClaimsScore(entity.MonthlyProjectClaimId);

            }

            return Json(new[] { record }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}