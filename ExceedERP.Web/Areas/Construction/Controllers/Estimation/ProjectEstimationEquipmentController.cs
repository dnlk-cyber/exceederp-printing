using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ProjectEstimationEquipmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private decimal totalMaterialCost = 0;
        public string Type { get; set; }

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult ProjectEstimationEquipment_Read([DataSourceRequest]DataSourceRequest request, int idfive)
        {
            IQueryable<ProjectEstimationEquipment> projectestimationequipment = db.ProjectEstimationEquipment.Where(x => x.ProjectEstimationWorkItemId == idfive);
            DataSourceResult result = projectestimationequipment.ToDataSourceResult(request, projectEstimationEquipment => new
            {
                ProjectEstimationEquipmentId = projectEstimationEquipment.ProjectEstimationEquipmentId,
                FleetsID = projectEstimationEquipment.FleetsID,
                //EquipmentIdentityID = projectEstimationEquipment.EquipmentIdentityID,
                //EquipmentTypeID = projectEstimationEquipment.EquipmentTypeID,
                //Unit = GetUnitOfMeasurment(projectEstimationEquipment.ItemCode),
                Quantity = projectEstimationEquipment.Quantity,
                UnitFactory = projectEstimationEquipment.UnitFactory,
                RentalCost = projectEstimationEquipment.RentalCost,
                TotalCost = projectEstimationEquipment.TotalCost,
                ProjectEstimationWorkItemId = projectEstimationEquipment.ProjectEstimationWorkItemId,
                EquipmentUnitCost = projectEstimationEquipment.EquipmentUnitCost,
                OrgBranchName = projectEstimationEquipment.OrgBranchName,
                DateCreated = projectEstimationEquipment.DateCreated,
                LastModified = projectEstimationEquipment.LastModified,
                CreatedBy = projectEstimationEquipment.CreatedBy,
                ModifiedBy = projectEstimationEquipment.ModifiedBy
            });

            return Json(result);
        }

        private decimal hourlyOutputRate;
        private bool EquipmentExits(int workItemId, Guid equipmentId)
        {
            bool equipmentExits = false;
            var temp = db.ProjectEstimationEquipment.Where(e => e.ProjectEstimationWorkItemId == workItemId && e.FleetsID == equipmentId).FirstOrDefault();
            if (temp != null)
            {
                equipmentExits = true;
            }
            return equipmentExits;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationEquipment_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationEquipment projectEstimationEquipment, int idsix)
        {
            if (ModelState.IsValid)
            {
                if (EquipmentExits(idsix, projectEstimationEquipment.FleetsID))
                {
                    ModelState.AddModelError("", "Equipment Exits already!");
                }
                else
                {
                    var entity = new ProjectEstimationEquipment
                    {
                        FleetsID = projectEstimationEquipment.FleetsID,
                        //Unit = GetUnitOfMeasurment(projectEstimationEquipment.ItemCode),
                        Quantity = projectEstimationEquipment.Quantity,
                        UnitFactory = projectEstimationEquipment.UnitFactory,
                        RentalCost = ConstructionInputsHelper.GetRentalRate(projectEstimationEquipment.FleetsID),
                        //EquipmentIdentityID = projectEstimationEquipment.EquipmentIdentityID,
                        //EquipmentTypeID = projectEstimationEquipment.EquipmentTypeID,
                        TotalCost = (projectEstimationEquipment.Quantity * projectEstimationEquipment.RentalCost),
                        ProjectEstimationWorkItemId = idsix,
                        EquipmentUnitCost = GetEquipmentUnitCost(idsix),

                        OrgBranchName = projectEstimationEquipment.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = projectEstimationEquipment.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = projectEstimationEquipment.ModifiedBy
                    };
                    entity.TotalCost = entity.RentalCost * entity.Quantity * entity.UnitFactory;
                    db.ProjectEstimationEquipment.Add(entity);
                    db.SaveChanges();
                    projectEstimationEquipment.ProjectEstimationEquipmentId = entity.ProjectEstimationEquipmentId;

                    ProjectEstimationWorkItemController.UpdateWorkItemCost(idsix);
                    ProjectEstimationWorkItemController.UpdateCustomWorkItemAfterUpdatingTemplateWorkItem(idsix);
                }


            }

            return Json(new[] { projectEstimationEquipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationEquipment_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationEquipment projectEstimationEquipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationEquipment
                {
                    ProjectEstimationEquipmentId = projectEstimationEquipment.ProjectEstimationEquipmentId,
                    FleetsID = projectEstimationEquipment.FleetsID,
                    //Unit = GetUnitOfMeasurment(projectEstimationEquipment.ItemCode),
                    Quantity = projectEstimationEquipment.Quantity,
                    UnitFactory = projectEstimationEquipment.UnitFactory,
                    RentalCost = ConstructionInputsHelper.GetRentalRate(projectEstimationEquipment.FleetsID),
                    TotalCost = (projectEstimationEquipment.Quantity * projectEstimationEquipment.RentalCost),
                    ProjectEstimationWorkItemId = projectEstimationEquipment.ProjectEstimationWorkItemId,
                    EquipmentUnitCost = GetEquipmentUnitCost(projectEstimationEquipment.ProjectEstimationWorkItemId),

                    OrgBranchName = projectEstimationEquipment.OrgBranchName,
                    DateCreated = projectEstimationEquipment.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectEstimationEquipment.CreatedBy,
                    ModifiedBy = User.Identity.Name,

                    //EquipmentIdentityID = projectEstimationEquipment.EquipmentIdentityID,
                    //EquipmentTypeID = projectEstimationEquipment.EquipmentTypeID,
                };
                db = new ExceedDbContext();
                entity.TotalCost = entity.RentalCost * entity.Quantity * entity.UnitFactory;
                db.ProjectEstimationEquipment.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ProjectEstimationWorkItemController.UpdateWorkItemCost(projectEstimationEquipment.ProjectEstimationWorkItemId);
                ProjectEstimationWorkItemController.UpdateCustomWorkItemAfterUpdatingTemplateWorkItem(projectEstimationEquipment.ProjectEstimationWorkItemId);
            }

            return Json(new[] { projectEstimationEquipment }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationEquipment_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationEquipment projectEstimationEquipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationEquipment
                {
                    ProjectEstimationEquipmentId = projectEstimationEquipment.ProjectEstimationEquipmentId,
                    FleetsID = projectEstimationEquipment.FleetsID,
                    //EquipmentIdentityID = projectEstimationEquipment.EquipmentIdentityID,
                    //EquipmentTypeID = projectEstimationEquipment.EquipmentTypeID,
                    Quantity = projectEstimationEquipment.Quantity,
                    UnitFactory = projectEstimationEquipment.UnitFactory,
                    RentalCost = projectEstimationEquipment.RentalCost,
                    TotalCost = projectEstimationEquipment.TotalCost,
                    ProjectEstimationWorkItemId = projectEstimationEquipment.ProjectEstimationWorkItemId,
                    EquipmentUnitCost = projectEstimationEquipment.EquipmentUnitCost,
                    OrgBranchName = projectEstimationEquipment.OrgBranchName,
                    DateCreated = projectEstimationEquipment.DateCreated,
                    LastModified = projectEstimationEquipment.LastModified,
                    CreatedBy = projectEstimationEquipment.CreatedBy,
                    ModifiedBy = projectEstimationEquipment.ModifiedBy
                };

                db.ProjectEstimationEquipment.Attach(entity);
                db.ProjectEstimationEquipment.Remove(entity);
                db.SaveChanges();
                ProjectEstimationWorkItemController.UpdateWorkItemCost(projectEstimationEquipment.ProjectEstimationWorkItemId);
                ProjectEstimationWorkItemController.UpdateCustomWorkItemAfterUpdatingTemplateWorkItem(projectEstimationEquipment.ProjectEstimationWorkItemId);
                //db = new ExceedDbContext();

            }

            return Json(new[] { projectEstimationEquipment }.ToDataSourceResult(request, ModelState));
        }
        private decimal GetEquipmentUnitCost(int id)
        {
            decimal labourCost = 0;
            db = new ExceedDbContext();
            var workItem = db.ProjectEstimationWorkItem.Where(x => x.ProjectEstimationWorkItemId == id).FirstOrDefault();

            if (workItem != null)
            {
                var labouCosts =
                    db.ProjectEstimationEquipment.Where(x => x.ProjectEstimationWorkItemId == id).ToList();
                if (labouCosts.Any())
                {

                    foreach (var m in labouCosts)
                    {
                        labourCost += m.TotalCost;
                    }
                }
                if (workItem.LabourHourlyOutPut > 0)
                {
                    labourCost = labourCost / workItem.LabourHourlyOutPut;
                }
                else
                {
                    labourCost = labourCost / 1;
                }
            }
            totalMaterialCost = labourCost;
            return labourCost;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
