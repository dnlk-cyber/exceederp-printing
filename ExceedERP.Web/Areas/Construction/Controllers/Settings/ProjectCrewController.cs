using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class ProjectCrewController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/ProjectCrew
        public ActionResult Index()
        {
            var lu = db.Lookups.Where(l => l.LookupType == LookUpTypes.JOB_TITLE);

            var rawList = from p in db.Placements
                          join emp in db.Person_Employee
                          on p.EmployeeId equals emp.EmployeeId
                          join l in db.Lookups
                          on p.LookupId equals l.LookupId
                          where p.isCurrent && emp.IsActive
                          select (new { Id = emp.EmployeeId, FName = emp.FirstName, LName = emp.LastName, job = p.JobTitleName, salary = p.Salary, LookupId = l.LookupId });

            var jobTitles = db.ProjectJobTitles.ToList();
            var ids = new HashSet<int>(jobTitles.Select(s => s.LookupId));

            var listForTeam = rawList.Where(item => (ids.Contains(item.LookupId)));
            ViewData["JobTitles"] = listForTeam;

            ConstructionInputs inputs = new ConstructionInputs();
            ViewData["Equipments"] = inputs.GetInternalExternalEquipmentViewModel();

            //db.Configuration.ProxyCreationEnabled = false;
            //var items = db.InventoryItems.ToList();
            ////var list = items.Select(p => new { ItemCode = p.ItemCode, Name = p.Name });
            //ViewData["Equipments"] = items;

            var workCategories = db.WorkCategory.ToList();
            ViewData["WorkTypes"] = workCategories;
            return View();
        }

        public ActionResult ProjectCrew_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectCrew> crews = db.ProjectCrew;
            DataSourceResult result = crews.ToDataSourceResult(request, crew => new {
                ProjectCrewId = crew.ProjectCrewId,
                WorkCategoryId = crew.WorkCategoryId,
                CrewCode = crew.CrewCode,
                CrewLeader = crew.CrewLeader,
                AmountofWorkDone = crew.AmountofWorkDone,
                TotalCrewCost = crew.TotalCrewCost
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectCrew_Create([DataSourceRequest]DataSourceRequest request, ProjectCrew crew)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCrew
                {
                    ProjectCrewId = crew.ProjectCrewId,
                    WorkCategoryId = crew.WorkCategoryId,
                    CrewCode = crew.CrewCode,
                    CrewLeader = crew.CrewLeader,
                    AmountofWorkDone = crew.AmountofWorkDone,
                    TotalCrewCost = crew.TotalCrewCost
                };

                db.ProjectCrew.Add(entity);
                db.SaveChanges();
                crew.ProjectCrewId = entity.ProjectCrewId;
            }

            return Json(new[] { crew }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectCrew_Update([DataSourceRequest]DataSourceRequest request, ProjectCrew crew)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCrew
                {
                    ProjectCrewId = crew.ProjectCrewId,
                    WorkCategoryId = crew.WorkCategoryId,
                    CrewCode = crew.CrewCode,
                    CrewLeader = crew.CrewLeader,
                    AmountofWorkDone = crew.AmountofWorkDone,
                    TotalCrewCost = crew.TotalCrewCost
                };

                db.ProjectCrew.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { crew }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectCrew_Destroy([DataSourceRequest]DataSourceRequest request, ProjectCrew crew)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectCrew
                {
                    ProjectCrewId = crew.ProjectCrewId,
                    WorkCategoryId = crew.WorkCategoryId,
                    CrewCode = crew.CrewCode,
                    CrewLeader = crew.CrewLeader,
                    AmountofWorkDone = crew.AmountofWorkDone,
                    TotalCrewCost = crew.TotalCrewCost
                };

                db.ProjectCrew.Attach(entity);
                db.ProjectCrew.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { crew }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}