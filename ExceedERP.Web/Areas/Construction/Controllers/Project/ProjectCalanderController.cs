using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class ProjectCalanderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ProjectCalendarViewModel> products, int idTwo)
        {
            var results = new List<ProjectCalendar>();
            ExceedDbContext dbs = new ExceedDbContext();
            //var allCalander = db.ProjectCalander.Where(x => x.ProjectInformationId == idTwo).ToList();
            //db.ProjectCalander.RemoveRange(allCalander);
            //db.SaveChanges();
            db = new ExceedDbContext();
            if (products != null && ModelState.IsValid)
            {
                foreach (var projectCalendar in products)
                {
                    var existingModel = db.ProjectCalander.Find(projectCalendar.ProjectCalanderId);
                    if(existingModel != null)
                    {
                        existingModel.Selected = projectCalendar.Selected;

                        existingModel.LastModified = DateTime.Now;
                        existingModel.ModifiedBy = User.Identity.Name;

                        db.ProjectCalander.Attach(existingModel);
                        db.Entry(existingModel).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    //var entity = new ProjectCalendar
                    //{
                    //    ProjectCalanderId = projectCalendar.ProjectCalanderId,
                    //    ToHour = projectCalendar.ToHour,
                    //    Day = projectCalendar.Day,
                    //    FromHour = projectCalendar.FromHour,
                    //    Selected = projectCalendar.Selected,
                    //    ProjectInformationId = idTwo,
                    //    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    //    DateCreated = DateTime.Now,
                    //    LastModified = null,
                    //    CreatedBy = User.Identity.Name,
                    //    ModifiedBy = null
                    //};

                    //db.ProjectCalander.Attach(entity);
                    //db.Entry(entity).State = EntityState.Modified;
                    //db.SaveChanges();

                }
            }


            return Json(results.ToDataSourceResult(request, ModelState));
        }
        public ActionResult ProjectCalander_Read([DataSourceRequest]DataSourceRequest request, int idOne)
        {
            var projectCalendars = db.ProjectCalander.Where(x => x.ProjectInformationId == idOne).ToList();

            var calendars = db.SettingCalendar.ToList();
            db = new ExceedDbContext();
            //if (!projectcalander.Any())
            //{

            if (calendars.Any())
            {
                foreach (var calendar in calendars)
                {

                    var existingCalendar = projectCalendars
                        .FirstOrDefault(c => c.ProjectInformationId == idOne && c.SettingCalendarId == calendar.SettingCalendarId);
                    if (existingCalendar == null)
                    {
                        var entity = new ProjectCalendar
                        {
                            FromHour = calendar.FromHour,
                            ToHour = calendar.ToHour,
                            Day = calendar.Day,
                            SettingCalendarId = calendar.SettingCalendarId,
                            ProjectInformationId = idOne,
                            WorkingHourType = calendar.WorkingHourType,

                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                            DateCreated = DateTime.Now,
                            LastModified = null,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = null,
                            Selected = true
                        };

                        db.ProjectCalander.Add(entity);
                        db.SaveChanges();
                    }

                }
            }
            projectCalendars = db.ProjectCalander.Where(x => x.ProjectInformationId == idOne).ToList();

            DataSourceResult result = projectCalendars.ToDataSourceResult(request, projectCalander => new
            {
                ProjectCalanderId = projectCalander.ProjectCalanderId,

                Day = projectCalander.Day,
                FromHour = projectCalander.FromHour,
                ToHour = projectCalander.ToHour,
                ProjectInformationId = projectCalander.ProjectInformationId,
                Selected = projectCalander.Selected,
                SettingCalendarId = projectCalander.SettingCalendarId,
                WorkingHourType = projectCalander.WorkingHourType,
                
                OrgBranchName = projectCalander.OrgBranchName,
                DateCreated = projectCalander.DateCreated,
                LastModified = projectCalander.LastModified,
                CreatedBy = projectCalander.CreatedBy,
                ModifiedBy = projectCalander.ModifiedBy

            });

            return Json(result);
            //}
            //else if (projectcalander.Any())
            //{

            //}
            //else
            //{
            //    DataSourceResult result = projectcalander.ToDataSourceResult(request, projectCalander => new
            //    {
            //        ProjectCalanderId = projectCalander.ProjectCalanderId,

            //        Day = projectCalander.Day,
            //        FromHour = projectCalander.FromHour,
            //        ToHour = projectCalander.ToHour,
            //        Selected = projectCalander.Selected,
            //        ProjectInformationId = projectCalander.ProjectInformationId,

            //        OrgBranchName = projectCalander.OrgBranchName,
            //        DateCreated = projectCalander.DateCreated,
            //        LastModified = projectCalander.LastModified,
            //        CreatedBy = projectCalander.CreatedBy,
            //        ModifiedBy = projectCalander.ModifiedBy

            //    });

            //    return Json(result);

            //}
        }


       // [AcceptVerbs(HttpVerbs.Post)]


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
