using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class ProjectStatusController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/ProjectStatus
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ProjectStatusInformation_Read([DataSourceRequest]DataSourceRequest request, string reference)
        {
            IQueryable<ProjectInformation> projects = db.ProjectInformation.Where(p => !p.IsDeleted);
            List<ProjectStatusEditViewModel> listToReturn = new List<ProjectStatusEditViewModel>();
            foreach(var project in projects)
            {
                var vm = new ProjectStatusEditViewModel
                {
                    ProjectInformationId = project.ProjectInformationId,
                    ProjectName = project.Name,
                    Code=project.Code,
                    Type = project.ProjectType.ToString(),
                    ContractAmount = project.ContractAmount,
                    ProjectStatus = project.ProjectStatus
                };
                listToReturn.Add(vm);
            }
           

            return Json(listToReturn.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectStatusInformation_Update([DataSourceRequest]DataSourceRequest request, ProjectStatusEditViewModel projectStatusEditViewModel)
        {
            if (ModelState.IsValid)
            {
                var currentProject = db.ProjectInformation.Find(projectStatusEditViewModel.ProjectInformationId);
                if(currentProject != null)
                {
                    currentProject.ProjectStatus = projectStatusEditViewModel.ProjectStatus;

                    currentProject.ModifiedBy = User.Identity.Name;
                    currentProject.LastModified = DateTime.Now;

                    db.ProjectInformation.Attach(currentProject);
                    db.Entry(currentProject).State = EntityState.Modified;
                    db.SaveChanges();
                }
                

                
            }

            return Json(new[] { projectStatusEditViewModel }.ToDataSourceResult(request, ModelState));
        }
    }
}