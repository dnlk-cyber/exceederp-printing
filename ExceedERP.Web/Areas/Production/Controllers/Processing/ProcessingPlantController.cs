using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.Processing
{
    public class ProcessingPlantController : Controller
    {
        ExceedDbContext db;

        public ProcessingPlantController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            //var stages = db.ProductionSettings.Where(s => s.Type == ProductionSettingType.SeedProductionStage || s.Type == ProductionSettingType.NaturalGumProductionStage).ToList();
            //ViewData["LandStatus"] = stages;

            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name}).ToList();

            return View();
        }
       
        public ActionResult ProductionPlants_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProcessingPlant> plants = db.ProcessingPlants;
            DataSourceResult result = plants.ToDataSourceResult(request, plant => new
            {
                BranchId = plant.BranchId,
                ProcessingPlantId = plant.ProcessingPlantId,
                PlantType = plant.PlantType,
                Name = plant.Name,

                OrgBranchName = plant.OrgBranchName,
                DateCreated = plant.DateCreated,
                LastModified = plant.LastModified,
                CreatedBy = plant.CreatedBy,
                ModifiedBy = plant.ModifiedBy,


            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionPlant_Create([DataSourceRequest]DataSourceRequest request, ProcessingPlant plant)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProcessingPlant
                {
                    BranchId = plant.BranchId,
                    ProcessingPlantId = plant.ProcessingPlantId,
                    PlantType = plant.PlantType,
                    Name = plant.Name,

                    OrgBranchName = plant.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = plant.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = plant.ModifiedBy,
                };
                //entity.Area = entity.Width * entity.Length;

                db.ProcessingPlants.Add(entity);
                db.SaveChanges();
                plant.ProcessingPlantId = entity.ProcessingPlantId;
            }

            return Json(new[] { plant }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionPlant_Update([DataSourceRequest]DataSourceRequest request, ProcessingPlant plant)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProcessingPlant
                {
                    BranchId = plant.BranchId,
                    ProcessingPlantId = plant.ProcessingPlantId,
                    PlantType = plant.PlantType,
                    Name = plant.Name,

                    OrgBranchName = plant.OrgBranchName,
                    DateCreated = plant.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = plant.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };
                //entity.Area = entity.Width * entity.Length;


                db.ProcessingPlants.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { plant }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionPlant_Destroy([DataSourceRequest]DataSourceRequest request, ProcessingPlant plant)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProcessingPlant
                {
                    BranchId = plant.BranchId,
                    ProcessingPlantId = plant.ProcessingPlantId,
                    PlantType = plant.PlantType,
                    Name = plant.Name,

                    OrgBranchName = plant.OrgBranchName,
                    DateCreated = plant.DateCreated,
                    LastModified = plant.LastModified,
                    CreatedBy = plant.CreatedBy,
                    ModifiedBy = plant.ModifiedBy,
                };

                db.ProcessingPlants.Attach(entity);
                db.ProcessingPlants.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { plant }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}