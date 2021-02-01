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
    public class CrewEquipmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/CrewEquipment
        public ActionResult Index()
        {
            //var items = db.InventoryItems.ToList();
            //ViewData["Equipments"] = items.Select(p => new { ItemCode = p.ItemCode, Name = p.Name }); ;
            ConstructionInputs inputs = new ConstructionInputs();
            ViewData["Equipments"] = inputs.GetInternalExternalEquipmentViewModel();
            return View();
        }

        public ActionResult CrewEquipment_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CrewEquipment> equipments = db.CrewEquipments;
            DataSourceResult result = equipments.ToDataSourceResult(request, resource => new {
                CrewHumanResourceId = resource.CrewEquipmentId,
                ProjectCrewId = resource.ProjectCrewId,
                Name = resource.Name,
                Quantity = resource.Quantity,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CrewEquipment_Create([DataSourceRequest]DataSourceRequest request, CrewEquipment resource, int cId)
        {
            if (ModelState.IsValid)
            {
                var entity = new CrewEquipment
                {
                    CrewEquipmentId = resource.CrewEquipmentId,
                    ProjectCrewId = cId,
                    Name = resource.Name,
                    Quantity = resource.Quantity,
                };

                db.CrewEquipments.Add(entity);
                db.SaveChanges();
                resource.CrewEquipmentId = entity.CrewEquipmentId;
            }

            return Json(new[] { resource }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CrewEquipment_Update([DataSourceRequest]DataSourceRequest request, CrewEquipment resource)
        {
            if (ModelState.IsValid)
            {
                var entity = new CrewEquipment
                {
                    CrewEquipmentId = resource.CrewEquipmentId,
                    ProjectCrewId = resource.ProjectCrewId,
                    Name = resource.Name,
                    Quantity = resource.Quantity,
                    //JobTitle = resource.JobTitle
                };

                db.CrewEquipments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { resource }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CrewEquipment_Destroy([DataSourceRequest]DataSourceRequest request, CrewEquipment resource)
        {
            if (ModelState.IsValid)
            {
                var entity = new CrewEquipment
                {
                    CrewEquipmentId = resource.CrewEquipmentId,
                    ProjectCrewId = resource.ProjectCrewId,
                    Name = resource.Name,
                    Quantity = resource.Quantity,
                };

                db.CrewEquipments.Attach(entity);
                db.CrewEquipments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { resource }.ToDataSourceResult(request, ModelState));
        }
    }
}