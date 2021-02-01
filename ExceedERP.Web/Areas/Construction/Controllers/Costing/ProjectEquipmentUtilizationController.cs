using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ProjectEquipmentUtilizationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ConstructionInputs inputs = new ConstructionInputs();
            ViewData["Equipment"] = inputs.GetInternalExternalEquipmentViewModel();
            return View();
        }

        public ActionResult EquipmentUtilization_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            IQueryable<ProjectEquipmentUtilization> equipmentutilization = db.EquipmentUtilization.Where( x=> x.ProjectInformationId.ToString() ==id);
            DataSourceResult result = equipmentutilization.ToDataSourceResult(request, projectEquipmentUtilization => new {
                ProjectEquipmentUtilizationId = projectEquipmentUtilization.ProjectEquipmentUtilizationId,
                Remark = projectEquipmentUtilization.Remark,
                ProjectEquipmentId = projectEquipmentUtilization.ProjectEquipmentId,
                ProjectInformationId = projectEquipmentUtilization.ProjectInformationId,
                OrgBranchName = projectEquipmentUtilization.OrgBranchName,
                DateCreated = projectEquipmentUtilization.DateCreated,
                LastModified = projectEquipmentUtilization.LastModified,
                CreatedBy = projectEquipmentUtilization.CreatedBy,
                ModifiedBy = projectEquipmentUtilization.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentUtilization_Create([DataSourceRequest]DataSourceRequest request, ProjectEquipmentUtilization projectEquipmentUtilization, string idTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentUtilization
                {
                    ProjectEquipmentId=projectEquipmentUtilization.ProjectEquipmentId,
                    Remark = projectEquipmentUtilization.Remark,
                    ProjectInformationId =Convert .ToInt32( idTwo),
                    OrgBranchName = projectEquipmentUtilization.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectEquipmentUtilization.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = projectEquipmentUtilization.ModifiedBy
                };

                db.EquipmentUtilization.Add(entity);
                db.SaveChanges();
                projectEquipmentUtilization.ProjectEquipmentUtilizationId = entity.ProjectEquipmentUtilizationId;
            }

            return Json(new[] { projectEquipmentUtilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentUtilization_Update([DataSourceRequest]DataSourceRequest request, ProjectEquipmentUtilization projectEquipmentUtilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentUtilization
                {
                    ProjectEquipmentUtilizationId = projectEquipmentUtilization.ProjectEquipmentUtilizationId,
                    Remark = projectEquipmentUtilization.Remark,
                    ProjectEquipmentId = projectEquipmentUtilization.ProjectEquipmentId,
                    ProjectInformationId = projectEquipmentUtilization.ProjectInformationId,
                    OrgBranchName = projectEquipmentUtilization.OrgBranchName,
                    DateCreated = projectEquipmentUtilization.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectEquipmentUtilization.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };

                db.EquipmentUtilization.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEquipmentUtilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentUtilization_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEquipmentUtilization projectEquipmentUtilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipmentUtilization
                {
                    ProjectEquipmentUtilizationId = projectEquipmentUtilization.ProjectEquipmentUtilizationId,
                    Remark = projectEquipmentUtilization.Remark,
                    ProjectEquipmentId = projectEquipmentUtilization.ProjectEquipmentId,
                    ProjectInformationId = projectEquipmentUtilization.ProjectInformationId,
                    OrgBranchName = projectEquipmentUtilization.OrgBranchName,
                    DateCreated = projectEquipmentUtilization.DateCreated,
                    LastModified = projectEquipmentUtilization.LastModified,
                    CreatedBy = projectEquipmentUtilization.CreatedBy,
                    ModifiedBy = projectEquipmentUtilization.ModifiedBy
                };

                db.EquipmentUtilization.Attach(entity);
                db.EquipmentUtilization.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEquipmentUtilization }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
