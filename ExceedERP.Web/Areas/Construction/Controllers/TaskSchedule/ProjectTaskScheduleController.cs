using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;

namespace ExceedERP.Web.Areas.Construction.Controllers.TaskSchedule
{
    [AuthorizeRoles(ConstructionConstants.SchedulingUser)]
    public class ProjectTaskScheduleController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var periods = db.ConstructionPeriod;
            ViewData["Periods"] = periods.ToList();

            var subPeriods = db.ConstructionSubPeriod.ToList();
            ViewData["SubPeriods"] = subPeriods;

            var taskList = db.Tasks.ToList().Select(task => new TaskViewModel
            {
                TaskID = task.ID,
                Title = "["+ task.ProjectSpecificId + " "+ "] "           
            });
            //var taskList = db.Tasks.ToList();
            ViewData["tasks"] = taskList;

            var tasks = db.Tasks.ToList();
            ViewData["AllTasks"] = tasks;

            //var workDuration = db.WorkItemDuration.ToList();
            //ViewData["WorkItemDurations"] = workDuration;

            return View();
        }
        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {
            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation.Where(p => !p.IsDeleted && ids.Contains(p.ProjectInformationId));//.Where (x=> x.ProjectStatus == ProjectStatus .STARTED );
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new {
                ProjectInformationId = projectInformation.ProjectInformationId,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                Category = projectInformation.Category,
                ProjectType = projectInformation.ProjectType,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,
                ProjectStatus = projectInformation.ProjectStatus,

                OrgBranchName = projectInformation.OrgBranchName,
                DateCreated = projectInformation.DateCreated,
                LastModified = projectInformation.LastModified,
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy,
            });

            return Json(result);
        }
        public ActionResult ProjectTaskSchedules_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProjectTaskSchedule> projecttaskschedules = db.ProjectTaskSchedules.Where( x=> x.ProjectInformationId ==id);
            DataSourceResult result = projecttaskschedules.ToDataSourceResult(request, projectTaskSchedule => new {
                ProjectTaskScheduleId = projectTaskSchedule.ProjectTaskScheduleId,
                TaskName = projectTaskSchedule.TaskName,
                Duration = projectTaskSchedule.Duration,
                StartDate = projectTaskSchedule.StartDate,
                EndDate = projectTaskSchedule.EndDate,
                CompletePercentage = projectTaskSchedule.CompletePercentage,
                ProjectInformationId = projectTaskSchedule.ProjectInformationId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTaskSchedules_Create([DataSourceRequest]DataSourceRequest request, ProjectTaskSchedule projectTaskSchedule, int idTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTaskSchedule
                {
                    TaskName = projectTaskSchedule.TaskName,
                    Duration = projectTaskSchedule.Duration,
                    StartDate = projectTaskSchedule.StartDate,
                    EndDate = projectTaskSchedule.EndDate,
                    CompletePercentage = projectTaskSchedule.CompletePercentage,
                    ProjectInformationId = idTwo,
                };

                db.ProjectTaskSchedules.Add(entity);
                db.SaveChanges();
                projectTaskSchedule.ProjectTaskScheduleId = entity.ProjectTaskScheduleId;
            }

            return Json(new[] { projectTaskSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTaskSchedules_Update([DataSourceRequest]DataSourceRequest request, ProjectTaskSchedule projectTaskSchedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTaskSchedule
                {
                    ProjectTaskScheduleId = projectTaskSchedule.ProjectTaskScheduleId,
                    TaskName = projectTaskSchedule.TaskName,
                    Duration = projectTaskSchedule.Duration,
                    StartDate = projectTaskSchedule.StartDate,
                    EndDate = projectTaskSchedule.EndDate,
                    CompletePercentage = projectTaskSchedule.CompletePercentage,
                    ProjectInformationId = projectTaskSchedule.ProjectInformationId
                };

                db.ProjectTaskSchedules.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectTaskSchedule }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectTaskSchedules_Destroy([DataSourceRequest]DataSourceRequest request, ProjectTaskSchedule projectTaskSchedule)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectTaskSchedule
                {
                    ProjectTaskScheduleId = projectTaskSchedule.ProjectTaskScheduleId,
                    TaskName = projectTaskSchedule.TaskName,
                    Duration = projectTaskSchedule.Duration,
                    StartDate = projectTaskSchedule.StartDate,
                    EndDate = projectTaskSchedule.EndDate,
                    CompletePercentage = projectTaskSchedule.CompletePercentage,
                    ProjectInformationId = projectTaskSchedule.ProjectInformationId
                };

                db.ProjectTaskSchedules.Attach(entity);
                db.ProjectTaskSchedules.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectTaskSchedule }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
