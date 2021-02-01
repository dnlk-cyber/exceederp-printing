using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ProjectBudgetDefinationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int ProjectId { get; set; }
        public ActionResult Index()
        {
            var projects = db.ProjectInformation.ToList();
            ViewData["ProjectList"] = projects;
            var branches = db.Branch.ToList();
            ViewData["Branch"] = branches;
            return View();
        }
        public void SetProjectId(int value)
        {
            ProjectId = 0;
            ProjectId = value;
            // Category = category;
        }

        public ActionResult ProjectInformation_Read([DataSourceRequest] DataSourceRequest request)
        {
            //IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(x => x.ProjectStatus == ProjectStatus.STARTED || x.ProjectStatus == ProjectStatus.APPROVED);
            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation;
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new
            {
                ProjectInformationId = projectInformation.ProjectInformationId,
                ProjectType = projectInformation.ProjectType,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                Category = projectInformation.Category,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,
                OrgBranchName = projectInformation.OrgBranchName,
                DateCreated = projectInformation.DateCreated,
                LastModified = projectInformation.LastModified,
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy,
                ProjectStatus = projectInformation.ProjectStatus
            });

            return Json(result);
        }

        public ActionResult ProjectDefineBudgets_Read([DataSourceRequest]DataSourceRequest request, int idOne)
        {
            IQueryable<ProjectDefineBudget> projectdefinebudgets = db.ProjectDefineBudgets.AsQueryable();
            if (idOne != -1)
            {
                projectdefinebudgets = projectdefinebudgets.Where(x => x.ProjectInformationId == idOne);
            }
            else if (ProjectId != 0)
            {
                projectdefinebudgets = projectdefinebudgets.Where(x => x.ProjectInformationId == ProjectId);
            }
            DataSourceResult result = projectdefinebudgets.ToDataSourceResult(request, projectDefineBudget => new {
                ProjectDefineBudgetId = projectDefineBudget.ProjectDefineBudgetId,
                BudgetCode = projectDefineBudget.BudgetCode,
                TotalBudget = projectDefineBudget.TotalBudget,
                EstimatedAmount = projectDefineBudget.EstimatedAmount,
                Description = projectDefineBudget.Description,
                ProjectInformationId = projectDefineBudget.ProjectInformationId ,
                OrgBranchName = projectDefineBudget.OrgBranchName,
                DateCreated = projectDefineBudget.DateCreated,
                LastModified = projectDefineBudget.LastModified,
                CreatedBy = projectDefineBudget.CreatedBy,
                ModifiedBy = projectDefineBudget.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectDefineBudgets_Create([DataSourceRequest]DataSourceRequest request, ProjectDefineBudget projectDefineBudget, int idTwo)
        {
            if (ProjectId != 0)
            {
                idTwo = ProjectId;
            }
            if (ModelState.IsValid)
            {
                var entity = new ProjectDefineBudget
                {
                    BudgetCode = projectDefineBudget.BudgetCode,
                    TotalBudget = projectDefineBudget.TotalBudget,
                    EstimatedAmount = projectDefineBudget.EstimatedAmount,
                    Description = projectDefineBudget.Description,
                    ProjectInformationId = idTwo ,
                    OrgBranchName = projectDefineBudget.OrgBranchName,
                    DateCreated = projectDefineBudget.DateCreated,
                    LastModified = projectDefineBudget.LastModified,
                    CreatedBy = projectDefineBudget.CreatedBy,
                    ModifiedBy = projectDefineBudget.ModifiedBy
                };

                db.ProjectDefineBudgets.Add(entity);
                db.SaveChanges();
                projectDefineBudget.ProjectDefineBudgetId = entity.ProjectDefineBudgetId;
            }

            return Json(new[] { projectDefineBudget }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectDefineBudgets_Update([DataSourceRequest]DataSourceRequest request, ProjectDefineBudget projectDefineBudget)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectDefineBudget
                {
                    ProjectDefineBudgetId = projectDefineBudget.ProjectDefineBudgetId,
                    BudgetCode = projectDefineBudget.BudgetCode,
                    TotalBudget = projectDefineBudget.TotalBudget,
                    EstimatedAmount = projectDefineBudget.EstimatedAmount,
                    Description = projectDefineBudget.Description,
                    ProjectInformationId = projectDefineBudget.ProjectInformationId ,
                    OrgBranchName = projectDefineBudget.OrgBranchName,
                    DateCreated = projectDefineBudget.DateCreated,
                    LastModified = projectDefineBudget.LastModified,
                    CreatedBy = projectDefineBudget.CreatedBy,
                    ModifiedBy = projectDefineBudget.ModifiedBy
                };

                db.ProjectDefineBudgets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectDefineBudget }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectDefineBudgets_Destroy([DataSourceRequest]DataSourceRequest request, ProjectDefineBudget projectDefineBudget)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectDefineBudget
                {
                    ProjectDefineBudgetId = projectDefineBudget.ProjectDefineBudgetId,
                    BudgetCode = projectDefineBudget.BudgetCode,
                    TotalBudget = projectDefineBudget.TotalBudget,
                    EstimatedAmount = projectDefineBudget.EstimatedAmount,
                    Description = projectDefineBudget.Description,
                    ProjectInformationId = projectDefineBudget.ProjectInformationId ,
                    OrgBranchName = projectDefineBudget.OrgBranchName,
                    DateCreated = projectDefineBudget.DateCreated,
                    LastModified = projectDefineBudget.LastModified,
                    CreatedBy = projectDefineBudget.CreatedBy,
                    ModifiedBy = projectDefineBudget.ModifiedBy
                };

                db.ProjectDefineBudgets.Attach(entity);
                db.ProjectDefineBudgets.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectDefineBudget }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
