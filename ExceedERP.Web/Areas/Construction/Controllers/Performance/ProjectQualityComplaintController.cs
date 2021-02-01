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
    public class ProjectQualityComplaintController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QualityComplaint_Read([DataSourceRequest]DataSourceRequest request, int iRead)
        {
            IQueryable<MonthlyProjectQualityComplaint> complaints = db.MonthlyProjectQualityComplaints.Where(x => x.MonthlyProjectQualityId == iRead);
            DataSourceResult result = complaints.ToDataSourceResult(request, complaint => new
            {
                MonthlyProjectQualityComplaintId = complaint.MonthlyProjectQualityComplaintId,
                MonthlyProjectQualityId = complaint.MonthlyProjectQualityId,
                Source = complaint.Source,
                Remark = complaint.Remark,
                NumOfComplaints = complaint.NumOfComplaints,

                OrgBranchName = complaint.OrgBranchName,
                DateCreated = complaint.DateCreated,
                LastModified = complaint.LastModified,
                CreatedBy = complaint.CreatedBy,
                ModifiedBy = complaint.ModifiedBy

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityComplaint_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectQualityComplaint complaint, int iCreate)
        {
            if (ModelState.IsValid)
            {
                //if (CheckExistance(Block.Name, iddTwo))
                //{
                //    ModelState.AddModelError("", "Duplicate names are not allowed!!");
                //}
                //else
                //{
                var entity = new MonthlyProjectQualityComplaint
                {
                    MonthlyProjectQualityId = iCreate,
                    Source = complaint.Source,
                    Remark = complaint.Remark,
                    NumOfComplaints = complaint.NumOfComplaints,

                    OrgBranchName = complaint.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = complaint.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = complaint.ModifiedBy
                };

                db.MonthlyProjectQualityComplaints.Add(entity);
                db.SaveChanges();
                complaint.MonthlyProjectQualityComplaintId = entity.MonthlyProjectQualityComplaintId;

                PerformanceEvnHelper.UpdateMonthlyProjectQualityScore(entity.MonthlyProjectQualityId);
            }

            return Json(new[] { complaint }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityComplaint_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectQualityComplaint complaint)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectQualityComplaint
                {
                    MonthlyProjectQualityComplaintId = complaint.MonthlyProjectQualityComplaintId,
                    MonthlyProjectQualityId = complaint.MonthlyProjectQualityId,
                    Source = complaint.Source,
                    Remark = complaint.Remark,
                    NumOfComplaints = complaint.NumOfComplaints,

                    OrgBranchName = complaint.OrgBranchName,
                    DateCreated = complaint.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = complaint.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.MonthlyProjectQualityComplaints.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectQualityScore(entity.MonthlyProjectQualityId);

            }

            return Json(new[] { complaint }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityComplaint_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectQualityComplaint complaint)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectQualityComplaint
                {
                    MonthlyProjectQualityComplaintId = complaint.MonthlyProjectQualityComplaintId,
                    MonthlyProjectQualityId = complaint.MonthlyProjectQualityId,
                    Source = complaint.Source,
                    Remark = complaint.Remark,
                    NumOfComplaints = complaint.NumOfComplaints,

                    OrgBranchName = complaint.OrgBranchName,
                    DateCreated = complaint.DateCreated,
                    LastModified = complaint.LastModified,
                    CreatedBy = complaint.CreatedBy,
                    ModifiedBy = complaint.ModifiedBy
                };

                db.MonthlyProjectQualityComplaints.Attach(entity);
                db.MonthlyProjectQualityComplaints.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectQualityScore(entity.MonthlyProjectQualityId);

            }

            return Json(new[] { complaint }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}