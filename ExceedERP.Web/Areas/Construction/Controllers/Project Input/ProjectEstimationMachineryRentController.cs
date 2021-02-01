using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project_Input
{
    public class ProjectEstimationMachineryRentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var projects = db.ProjectEstimationName.ToList();
            ViewData["ProjectList"] = projects;
            return View();
        }
        public static int ProjectId { get; set; }
        public void SetProjectId(int value)
        {
            ProjectId = 0;
            ProjectId = value;
            RedirectToAction("ProjectEstimationMachineryRent_Read");
        }

        public ActionResult ProjectEstimationMachineryRent_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProjectMachineryRentHeader> projectestimationmachineryrent = db.ProjectMachineryRentHeader;
            if (ProjectId >0)
            {

                projectestimationmachineryrent =projectestimationmachineryrent.Where(x => x.ProjectEstimationNameId == ProjectId).AsQueryable() ;
            DataSourceResult result = projectestimationmachineryrent.ToDataSourceResult(request, projectEstimationMachineryRent => new {
                ProjectEstimationMachineryRentId = projectEstimationMachineryRent.ProjectEstimationMachineryRentId,
                FuelPurchase = projectEstimationMachineryRent.FuelPurchase,
                FuelCost = projectEstimationMachineryRent.FuelCost,
                FuelSupply = projectEstimationMachineryRent.FuelSupply,
                FuelDistribution = projectEstimationMachineryRent.FuelDistribution,
                ProjectEstimationNameId = projectEstimationMachineryRent.ProjectEstimationNameId,
              
            });

            return Json(result); 
            }
            return Json(null);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationMachineryRent_Create([DataSourceRequest]DataSourceRequest request, ProjectMachineryRentHeader projectMachineryRentHeader)
        {
           
                if (ModelState.IsValid)
                {
                    if (ProjectId == 0)
                    {

                        return new JavaScriptResult() { Script = "alert('" + "Please select the project first!!" + "');" };
                    }
                    var entity = new ProjectMachineryRentHeader
                    {
                        FuelPurchase = projectMachineryRentHeader.FuelPurchase,
                        FuelCost = projectMachineryRentHeader.FuelCost,
                        FuelSupply = GetFuelSupply(),
                        FuelDistribution = GetFuelDistribution(),
                        ProjectEstimationNameId = ProjectId

                    };

                    db.ProjectMachineryRentHeader.Add(entity);
                    db.SaveChanges();
                    projectMachineryRentHeader.ProjectEstimationMachineryRentId =
                        entity.ProjectEstimationMachineryRentId;
                }
           
            return Json(new[] { projectMachineryRentHeader }.ToDataSourceResult(request, ModelState));
        }

        private decimal GetFuelSupply()
        {
            var categories =
                db.Configurations.Where(x => x.Attribute == "TRANSPORTATION_COST_BIRR_LITTER" && x.Category == "Construction").FirstOrDefault();
            if (categories != null)
            {
                return Convert.ToDecimal( categories.CurrentValue);
            }
            return 0;
        }
        private decimal GetFuelDistribution()
        {
            var categories =
                db.Configurations.Where(x => x.Attribute == "DISTRIBUTION_COST_BIRR_LITTER" && x.Category == "Construction").FirstOrDefault();
            if (categories != null)
            {
                return Convert.ToDecimal(categories.CurrentValue);
            }
            return 0;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationMachineryRent_Update([DataSourceRequest]DataSourceRequest request, ProjectMachineryRentHeader projectMachineryRentHeader)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectMachineryRentHeader
                {
                    ProjectEstimationMachineryRentId = projectMachineryRentHeader.ProjectEstimationMachineryRentId,
                    FuelPurchase = projectMachineryRentHeader.FuelPurchase,
                    FuelCost = projectMachineryRentHeader.FuelCost,
                    FuelSupply = GetFuelSupply(),
                    FuelDistribution = GetFuelDistribution(),
                    ProjectEstimationNameId = projectMachineryRentHeader.ProjectEstimationNameId,
                   
                };

                db.ProjectMachineryRentHeader.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectMachineryRentHeader }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationMachineryRent_Destroy([DataSourceRequest]DataSourceRequest request, ProjectMachineryRentHeader projectMachineryRentHeader)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectMachineryRentHeader
                {
                    ProjectEstimationMachineryRentId = projectMachineryRentHeader.ProjectEstimationMachineryRentId,
                    FuelPurchase = projectMachineryRentHeader.FuelPurchase,
                    FuelCost = projectMachineryRentHeader.FuelCost,
                    FuelSupply = projectMachineryRentHeader.FuelSupply,
                    FuelDistribution = projectMachineryRentHeader.FuelDistribution,
                    ProjectEstimationNameId = projectMachineryRentHeader.ProjectEstimationNameId,
                 
                };

                db.ProjectMachineryRentHeader.Attach(entity);
                db.ProjectMachineryRentHeader.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectMachineryRentHeader }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
