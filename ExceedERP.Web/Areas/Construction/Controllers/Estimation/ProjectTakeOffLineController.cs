using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ProjectTakeOffLineController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var unitWegight = db.UnitRate.ToList();
            ViewData["Diameters"] = unitWegight.Select(s => new { Text = s.Diameter.ToString(), Value = s.UnitRateId.ToString() });
            var uom = db.UnitOfMeasurement .ToList();
            ViewData["uom"] = uom.Select(s => new { Abbreviation = s.Abbreviation, UnitOfMeasurementId = s.UnitOfMeasurementId });
            return View();
        }

        public ActionResult ProjectTakeOffLine_Read([DataSourceRequest]DataSourceRequest request, int lineId)
        {
            IQueryable<ProjectTakeOffLine> projecttakeoffline = db.ProjectTakeOffLine.Where(x => x.ProjectTakeOffId == lineId);
            DataSourceResult result = projecttakeoffline.ToDataSourceResult(request, projectTakeOffLine => new {
                ProjectTakeOffLineId = projectTakeOffLine.ProjectTakeOffLineId,
                No = projectTakeOffLine.No,
                Quantity = projectTakeOffLine.Quantity,
                Description = projectTakeOffLine.Description,
                UnitOfMeasurementId = projectTakeOffLine.UnitOfMeasurementId,
                Length = projectTakeOffLine.Length,
                Width = projectTakeOffLine.Width,
                Depth = projectTakeOffLine.Depth,
                IsDeductive = projectTakeOffLine.IsDeductive,
                DateOfInformation = projectTakeOffLine.DateOfInformation,
                ProjectTakeOffId = projectTakeOffLine.ProjectTakeOffId,

                OrgBranchName = projectTakeOffLine.OrgBranchName,
                DateCreated = projectTakeOffLine.DateCreated,
                LastModified = projectTakeOffLine.LastModified,
                CreatedBy = projectTakeOffLine.CreatedBy,
                ModifiedBy = projectTakeOffLine.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTakeOffLine_Create([DataSourceRequest]DataSourceRequest request, ProjectTakeOffLine projectTakeOffLine, int lineIdTwo, string type)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTakeOffLine
                {
                    No = projectTakeOffLine.No,
                    Quantity = type == "road" ? projectTakeOffLine.Quantity : this.CalculateQuantity(projectTakeOffLine),
                    Description = projectTakeOffLine.Description,
                    UnitOfMeasurementId = this.GetTakeOffUnitOfMeasurement(lineIdTwo),
                    Length = projectTakeOffLine.Length,
                    Width = projectTakeOffLine.Width,
                    Depth = projectTakeOffLine.Depth,
                    IsDeductive = projectTakeOffLine.IsDeductive,
                    DateOfInformation = projectTakeOffLine.DateOfInformation,
                    ProjectTakeOffId = lineIdTwo,

                    OrgBranchName = projectTakeOffLine.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectTakeOffLine.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = projectTakeOffLine.ModifiedBy 
                };
                
                db.ProjectTakeOffLine.Add(entity);
                db.SaveChanges();
                projectTakeOffLine.ProjectTakeOffLineId = entity.ProjectTakeOffLineId;
                BillingHelper.UpdateWorkItemBoQAndScheduleOnTakeOffSheetContentChange(entity.ProjectTakeOffId);
            }

            return Json(new[] { projectTakeOffLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTakeOffLine_Update([DataSourceRequest]DataSourceRequest request, ProjectTakeOffLine projectTakeOffLine, string type)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTakeOffLine
                {
                    ProjectTakeOffLineId = projectTakeOffLine.ProjectTakeOffLineId,
                    No = projectTakeOffLine.No,
                    Quantity = type == "road" ? projectTakeOffLine.Quantity : this.CalculateQuantity(projectTakeOffLine),
                    Description = projectTakeOffLine.Description,
                    UnitOfMeasurementId = projectTakeOffLine.UnitOfMeasurementId,
                    Length = projectTakeOffLine.Length,
                    Width = projectTakeOffLine.Width,
                    Depth = projectTakeOffLine.Depth,
                    IsDeductive = projectTakeOffLine.IsDeductive,
                    DateOfInformation = projectTakeOffLine.DateOfInformation,
                    ProjectTakeOffId = projectTakeOffLine.ProjectTakeOffId,
                    
                    OrgBranchName = projectTakeOffLine.OrgBranchName,
                    DateCreated = projectTakeOffLine.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectTakeOffLine.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };
                
                db.ProjectTakeOffLine.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                BillingHelper.UpdateWorkItemBoQAndScheduleOnTakeOffSheetContentChange(entity.ProjectTakeOffId);
            }

            return Json(new[] { projectTakeOffLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTakeOffLine_Destroy([DataSourceRequest]DataSourceRequest request, ProjectTakeOffLine projectTakeOffLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTakeOffLine
                {
                    ProjectTakeOffLineId = projectTakeOffLine.ProjectTakeOffLineId,
                    No = projectTakeOffLine.No,
                    Quantity = projectTakeOffLine.Quantity,
                    Description = projectTakeOffLine.Description,
                    UnitOfMeasurementId = projectTakeOffLine.UnitOfMeasurementId,
                    Length = projectTakeOffLine.Length,
                    Width = projectTakeOffLine.Width,
                    Depth = projectTakeOffLine.Depth,
                    IsDeductive = projectTakeOffLine.IsDeductive,
                    DateOfInformation = projectTakeOffLine.DateOfInformation,
                    ProjectTakeOffId = projectTakeOffLine.ProjectTakeOffId,

                    OrgBranchName = projectTakeOffLine.OrgBranchName,
                    DateCreated = projectTakeOffLine.DateCreated,
                    LastModified = projectTakeOffLine.LastModified,
                    CreatedBy = projectTakeOffLine.CreatedBy,
                    ModifiedBy = projectTakeOffLine.ModifiedBy 
                };

                db.ProjectTakeOffLine.Attach(entity);
                db.ProjectTakeOffLine.Remove(entity);
                db.SaveChanges();

                BillingHelper.UpdateWorkItemBoQAndScheduleOnTakeOffSheetContentChange(entity.ProjectTakeOffId);
            }

            return Json(new[] { projectTakeOffLine }.ToDataSourceResult(request, ModelState));
        }
        private decimal CalculateQuantity(ProjectTakeOffLine model)
        {
            decimal total = new decimal();
            if (model != null)
            {
                total = model.No * model.Length * model.Width * model.Depth;
                if (model.IsDeductive)
                {
                    total = -1 * total;
                }
            }
            return total;
        }
        private int GetTakeOffUnitOfMeasurement(int takeOffId)
        {
            WorkItemPerProject workItem = new WorkItemPerProject();
            var actualQuantityTakeoff = db.ProjectTakeOff.Find(takeOffId);
            if (actualQuantityTakeoff != null)
            {
                var workItemId = actualQuantityTakeoff.WorkItemPerProjectId;
                var temp = db.WorkItemPerProject.Find(workItemId);
                if (temp != null)
                {
                    workItem = temp;
                }
            }
            return workItem.UnitOfMeasurementId ?? 0;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
