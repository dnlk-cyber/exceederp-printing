using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ProjectEstimationRawDataController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var lookups = db.Lookups.AsQueryable().ToList();
            ViewData["UOM"] = lookups.Where(x => x.LookupType == LookUpTypes.UOM).AsQueryable();
            ViewData["RowDataCategory"] = lookups.Where(x => x.LookupType == LookUpTypes.ROW_DATA).AsQueryable();
            return View();
        }

        public ActionResult ProjectEstimationRawData_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectEstimationRawData> projectestimationrawdata = db.ProjectEstimationRawData;
            DataSourceResult result = projectestimationrawdata.ToDataSourceResult(request, projectEstimationRawData => new {
                ProjectEstimationRawDataId = projectEstimationRawData.ProjectEstimationRawDataId,
                ItemNo = projectEstimationRawData.ItemNo,
                Description = projectEstimationRawData.Description,
                Amount = projectEstimationRawData.Amount,
            Category=    projectEstimationRawData.Category ,
                LookupId = projectEstimationRawData.LookupId,
                Remark = projectEstimationRawData.Remark,
                OrgBranchName = projectEstimationRawData.OrgBranchName,
                DateCreated = projectEstimationRawData.DateCreated,
                LastModified = projectEstimationRawData.LastModified,
                CreatedBy = projectEstimationRawData.CreatedBy,
                ModifiedBy = projectEstimationRawData.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationRawData_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationRawData projectEstimationRawData)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationRawData
                {
                    ItemNo = projectEstimationRawData.ItemNo,
                    Description = projectEstimationRawData.Description,
                    Amount = projectEstimationRawData.Amount,
                     Category=    projectEstimationRawData.Category ,
                LookupId = projectEstimationRawData.LookupId,
                Remark = projectEstimationRawData.Remark,
                    OrgBranchName = projectEstimationRawData.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectEstimationRawData.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = projectEstimationRawData.ModifiedBy 
                };

                db.ProjectEstimationRawData.Add(entity);
                db.SaveChanges();
                projectEstimationRawData.ProjectEstimationRawDataId = entity.ProjectEstimationRawDataId;
            }

            return Json(new[] { projectEstimationRawData }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationRawData_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationRawData projectEstimationRawData)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationRawData
                {
                    ProjectEstimationRawDataId = projectEstimationRawData.ProjectEstimationRawDataId,
                    ItemNo = projectEstimationRawData.ItemNo,
                    Description = projectEstimationRawData.Description,
                    Amount = projectEstimationRawData.Amount,
                    Category = projectEstimationRawData.Category,
                    LookupId = projectEstimationRawData.LookupId,

                    Remark = projectEstimationRawData.Remark,
                    OrgBranchName = projectEstimationRawData.OrgBranchName,
                    DateCreated = projectEstimationRawData.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectEstimationRawData.CreatedBy,
                    ModifiedBy =User.Identity .Name  
                };

                db.ProjectEstimationRawData.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationRawData }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationRawData_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationRawData projectEstimationRawData)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationRawData
                {
                    ProjectEstimationRawDataId = projectEstimationRawData.ProjectEstimationRawDataId,
                    ItemNo = projectEstimationRawData.ItemNo,
                    Description = projectEstimationRawData.Description,
                    Amount = projectEstimationRawData.Amount,
                    Category = projectEstimationRawData.Category,
                    LookupId = projectEstimationRawData.LookupId,
                    Remark = projectEstimationRawData.Remark,
                    OrgBranchName = projectEstimationRawData.OrgBranchName,
                    DateCreated = projectEstimationRawData.DateCreated,
                    LastModified = projectEstimationRawData.LastModified,
                    CreatedBy = projectEstimationRawData.CreatedBy,
                    ModifiedBy = projectEstimationRawData.ModifiedBy 
                };

                db.ProjectEstimationRawData.Attach(entity);
                db.ProjectEstimationRawData.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationRawData }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
