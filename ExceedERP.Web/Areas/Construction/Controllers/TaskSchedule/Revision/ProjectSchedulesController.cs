using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule.Revision
{
    [AuthorizeRoles(ConstructionConstants.SchedulingUser)]
    public class ProjectSchedulesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        private readonly TaskService _taskService;
        private readonly DependencyService _dependencyService;

        public ProjectSchedulesController()
        {
            db = new ExceedDbContext();
            _taskService = new TaskService();
            _dependencyService = new DependencyService();
        }
        protected override void Dispose(bool disposing)
        {
            _taskService.Dispose();
            _dependencyService.Dispose();

            base.Dispose(disposing);
        }
        public ActionResult Index()
        {

            this.PrepareFirstVersionOfSchedules();

            return View();
        }

        private void PrepareFirstVersionOfSchedules()
        {
            var tasks = db.Tasks.ToList();
            var allProjects = db.ProjectInformation.ToList();
            var projectIds = new HashSet<int>(tasks.Select(t => t.WorkItemDuration.ProjectInformationId)).Distinct();
            var projectsWithSchedule = allProjects.Where(proj => projectIds.Contains(proj.ProjectInformationId));

            if (projectsWithSchedule.Any())
            {
                foreach(ProjectInformation  project in projectsWithSchedule)
                {
                    var firstVersionOfSchedule = db.ScheduleRevisions.Where(sr => sr.ProjectInformationId == project.ProjectInformationId).FirstOrDefault();
                    if(firstVersionOfSchedule == null)
                    {
                        var firstVersion = new ScheduleRevision
                        {
                            ProjectInformationId = project.ProjectInformationId,
                            Version = 1,
                            ScheduleStatus = ProjectScheduleStatus.Editing,
                            OrgBranchName = null,
                            DateOfApproval = null,
                            CreatedBy = null,
                            ModifiedBy = null,
                            DateCreated = null,
                            LastModified = null,
                            
                        };
                        db.ScheduleRevisions.Add(firstVersion);
                        db.SaveChanges();
                    }
                }
            }
        }

        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(p => !p.IsDeleted && ids.Contains(p.ProjectInformationId));//.Where (x=> x.ProjectStatus == ProjectStatus .STARTED );
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new ScheduleRevisionProjectViewModel
            {
                ProjectInformationId = projectInformation.ProjectInformationId,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                ProjectType = projectInformation.ProjectType,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,
                ProjectStatus = projectInformation.ProjectStatus,
                PreviousVersionNotApproved = PreviousVersionNotApproved(projectInformation.ProjectInformationId)
            });

            return Json(result);
        }
        private bool PreviousVersionNotApproved(int projectId)
        {
            var previousNotApproved = false;
            var versions = db.ScheduleRevisions.Where(v => v.ProjectInformationId == projectId).ToList();
            if (versions.Any())
            {
                var lastVerion = versions.Max(v => v.Version);
                var latest = versions.FirstOrDefault(v => v.Version == lastVerion);
                if (latest.ScheduleStatus != ProjectScheduleStatus.Approved)
                {
                    previousNotApproved = true;
                }
            }
            return previousNotApproved;
        }
        // read tasks of a given version
        public virtual JsonResult ReadTasks([DataSourceRequest] DataSourceRequest request, int revisionId)
        {
            var currentRevion = db.ScheduleRevisions.Find(revisionId);
            int projectId = 0;
            if(currentRevion != null)
            {
                var project = db.ProjectInformation.Find(currentRevion.ProjectInformationId);
                if(project != null)
                {
                    projectId = project.ProjectInformationId;
                }

            }
            var tasksUnderThisProject = _taskService.GetAll().Where(t => t.ProjectInformationId == projectId && t.Version == currentRevion.Version);
            return Json(tasksUnderThisProject.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}