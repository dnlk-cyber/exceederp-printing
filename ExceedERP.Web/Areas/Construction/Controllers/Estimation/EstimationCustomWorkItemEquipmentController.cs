using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class EstimationCustomWorkItemEquipmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Equipment_Read([DataSourceRequest]DataSourceRequest request, int workItemId)
        {
            IQueryable<EstimationCustomWorkItemEquipment> equipments = db.EstimationCustomWorkItemEquipment.Where(x => x.ProjectEstimationCustomWorkItemId == workItemId);
            DataSourceResult result = equipments.ToDataSourceResult(request, equipment => new
            {
                EstimationCustomWorkItemEquipmentId = equipment.EstimationCustomWorkItemEquipmentId,
                FleetsID = equipment.FleetsID,
                
                Quantity = equipment.Quantity,
                UnitFactory = equipment.UnitFactory,
                RentalCost = equipment.RentalCost,
                TotalCost = equipment.TotalCost,
                ProjectEstimationCustomWorkItemId = equipment.ProjectEstimationCustomWorkItemId,
                //EquipmentUnitCost = equipment.EquipmentUnitCost,

                OrgBranchName = equipment.OrgBranchName,
                DateCreated = equipment.DateCreated,
                LastModified = equipment.LastModified,
                CreatedBy = equipment.CreatedBy,
                ModifiedBy = equipment.ModifiedBy
            });

            return Json(result);
        }

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
        public ActionResult Equipment_Create([DataSourceRequest]DataSourceRequest request, EstimationCustomWorkItemEquipment equipment, int workItemId)
        {
            if (ModelState.IsValid)
            {
                if (EquipmentExits(workItemId, equipment.FleetsID))
                {
                    ModelState.AddModelError("", "Equipment Exits already!");
                }
                else
                {
                    var entity = new EstimationCustomWorkItemEquipment
                    {
                        FleetsID = equipment.FleetsID,
                        Quantity = equipment.Quantity,
                        UnitFactory = equipment.UnitFactory,
                        RentalCost = ConstructionInputsHelper.GetRentalRate(equipment.FleetsID),
                        TotalCost = (equipment.Quantity * equipment.RentalCost),
                        ProjectEstimationCustomWorkItemId = workItemId,
                        //EquipmentUnitCost = GetEquipmentUnitCost(idsix),

                        OrgBranchName = equipment.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = equipment.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = equipment.ModifiedBy
                    };
                    entity.TotalCost = entity.RentalCost * entity.Quantity * entity.UnitFactory;
                    db.EstimationCustomWorkItemEquipment.Add(entity);
                    db.SaveChanges();
                    equipment.EstimationCustomWorkItemEquipmentId = entity.EstimationCustomWorkItemEquipmentId;

                    ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(workItemId);
                }


            }

            return Json(new[] { equipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Equipment_Update([DataSourceRequest]DataSourceRequest request, EstimationCustomWorkItemEquipment equipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationCustomWorkItemEquipment
                {
                    EstimationCustomWorkItemEquipmentId = equipment.EstimationCustomWorkItemEquipmentId,
                    FleetsID = equipment.FleetsID,
                    //Unit = GetUnitOfMeasurment(equipment.ItemCode),
                    Quantity = equipment.Quantity,
                    UnitFactory = equipment.UnitFactory,
                    RentalCost = ConstructionInputsHelper.GetRentalRate(equipment.FleetsID),
                    TotalCost = (equipment.Quantity * equipment.RentalCost),
                    ProjectEstimationCustomWorkItemId = equipment.ProjectEstimationCustomWorkItemId,
                    //EquipmentUnitCost = GetEquipmentUnitCost(equipment.ProjectEstimationWorkItemId),

                    OrgBranchName = equipment.OrgBranchName,
                    DateCreated = equipment.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = equipment.CreatedBy,
                    ModifiedBy = User.Identity.Name,

                    //EquipmentIdentityID = equipment.EquipmentIdentityID,
                    //EquipmentTypeID = equipment.EquipmentTypeID,
                };
                entity.TotalCost = entity.RentalCost * entity.Quantity * entity.UnitFactory;
                db.EstimationCustomWorkItemEquipment.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);
            }

            return Json(new[] { equipment }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Equipment_Destroy([DataSourceRequest]DataSourceRequest request, EstimationCustomWorkItemEquipment equipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new EstimationCustomWorkItemEquipment
                {
                    EstimationCustomWorkItemEquipmentId = equipment.EstimationCustomWorkItemEquipmentId,
                    FleetsID = equipment.FleetsID,
                    //EquipmentIdentityID = equipment.EquipmentIdentityID,
                    //EquipmentTypeID = equipment.EquipmentTypeID,
                    Quantity = equipment.Quantity,
                    UnitFactory = equipment.UnitFactory,
                    RentalCost = equipment.RentalCost,
                    TotalCost = equipment.TotalCost,
                    ProjectEstimationCustomWorkItemId = equipment.ProjectEstimationCustomWorkItemId,
                    //EquipmentUnitCost = equipment.EquipmentUnitCost,
                    OrgBranchName = equipment.OrgBranchName,
                    DateCreated = equipment.DateCreated,
                    LastModified = equipment.LastModified,
                    CreatedBy = equipment.CreatedBy,
                    ModifiedBy = equipment.ModifiedBy
                };

                db.EstimationCustomWorkItemEquipment.Attach(entity);
                db.EstimationCustomWorkItemEquipment.Remove(entity);
                db.SaveChanges();

                ProjectEstimationHelper.UpdateCustomWorkItemUnitPrice(entity.ProjectEstimationCustomWorkItemId);
            }

            return Json(new[] { equipment }.ToDataSourceResult(request, ModelState));
        }
        //private decimal GetEquipmentUnitCost(int id)
        //{
        //    decimal labourCost = 0;
        //    db = new ExceedDbContext();
        //    var workItem = db.ProjectEstimationWorkItem.Where(x => x.ProjectEstimationWorkItemId == id).FirstOrDefault();

        //    if (workItem != null)
        //    {
        //        var labouCosts =
        //            db.ProjectEstimationEquipment.Where(x => x.ProjectEstimationWorkItemId == id).ToList();
        //        if (labouCosts.Any())
        //        {

        //            foreach (var m in labouCosts)
        //            {
        //                labourCost += m.TotalCost;
        //            }
        //        }
        //        if (workItem.LabourHourlyOutPut > 0)
        //        {
        //            labourCost = labourCost / workItem.LabourHourlyOutPut;
        //        }
        //        else
        //        {
        //            labourCost = labourCost / 1;
        //        }
        //    }
        //    totalMaterialCost = labourCost;
        //    return labourCost;
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}