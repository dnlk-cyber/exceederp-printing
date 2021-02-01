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
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ResourceUtilizationEquipmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private decimal totalMaterialCost = 0;
        public string Type { get; set; }
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult UtilizationEquipment_Read([DataSourceRequest]DataSourceRequest request, int idfive)
        {
            IQueryable<ResourceUtilizationEquipment> projectequipment = db.ResourceUtilizationEquipment.Where(x => x.ResourceUtilizationWorkItemId == idfive);
            DataSourceResult result = projectequipment.ToDataSourceResult(request, projectEquipment => new {
                ResourceUtilizationEquipmentId = projectEquipment.ResourceUtilizationEquipmentId,
                FleetsID = projectEquipment.FleetsID,
                //EquipmentTypeID = projectEquipment.EquipmentTypeID,
                //EquipmentIdentityID = projectEquipment.EquipmentIdentityID,
                Unit = projectEquipment.Unit,
                Quantity = projectEquipment.Quantity,
                UnitFactory = projectEquipment.UnitFactory,
                RentalCost = projectEquipment.RentalCost,
                TotalCost = projectEquipment.TotalCost,
                EquipmentUnitCost = projectEquipment.EquipmentUnitCost,
                EquipmentType = projectEquipment.EquipmentType,
                OrgBranchName = projectEquipment.OrgBranchName,
                DateCreated = projectEquipment.DateCreated,
                LastModified = projectEquipment.LastModified,
                CreatedBy = projectEquipment.CreatedBy,
                ModifiedBy = projectEquipment.ModifiedBy,
                ResourceUtilizationWorkItemId = projectEquipment.ResourceUtilizationWorkItemId,
            });

            return Json(result);
        }

        private bool EquipmentExists(ResourceUtilizationEquipment equipment, int utilizationWorkItemId)
        {
            bool equipExists = false;
            var eqip = db.ResourceUtilizationEquipment.FirstOrDefault(
                e => e.ResourceUtilizationWorkItemId == utilizationWorkItemId && e.FleetsID == equipment.FleetsID);
            if (eqip != null)
            {
                equipExists = true;
            }
            return equipExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UtilizationEquipment_Create([DataSourceRequest]DataSourceRequest request, ResourceUtilizationEquipment projectEquipment, int idsix)
        {
            if (ModelState.IsValid)
            {
                if (EquipmentExists(projectEquipment, idsix))
                {
                    ModelState.AddModelError("", "Equipment Exists!");
                }
                else
                {
                    var entity = new ResourceUtilizationEquipment
                    {
                        FleetsID = projectEquipment.FleetsID,
                        Unit = projectEquipment.Unit,
                        Quantity = projectEquipment.Quantity,
                        UnitFactory = projectEquipment.UnitFactory,
                        RentalCost = ConstructionInputsHelper.GetRentalRate(projectEquipment.FleetsID),
                        //EquipmentTypeID = projectEquipment.EquipmentTypeID,
                        //EquipmentIdentityID = projectEquipment.EquipmentIdentityID,
                        TotalCost = (projectEquipment.Quantity * projectEquipment.RentalCost * projectEquipment.UnitFactory),
                        EquipmentUnitCost = GetEquipmentUnitCost(idsix),
                        EquipmentType = projectEquipment.EquipmentType,
                        OrgBranchName = projectEquipment.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = projectEquipment.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = projectEquipment.ModifiedBy,
                        ResourceUtilizationWorkItemId = idsix,

                    };
                    entity.TotalCost = entity.RentalCost * entity.Quantity * entity.UnitFactory;
                    db.ResourceUtilizationEquipment.Add(entity);
                    db.SaveChanges();
                    projectEquipment.ResourceUtilizationEquipmentId = entity.ResourceUtilizationEquipmentId;
                    //ProjectListController.UpdateProjectTotalCost(idsix);

                    ResourceUtilizationController controller = new ResourceUtilizationController();
                    controller.UpdateResourceUtilizationTotalDirectCostOfWorkItem(entity.ResourceUtilizationWorkItemId);

                }
            }

            return Json(new[] { projectEquipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UtilizationEquipment_Update([DataSourceRequest]DataSourceRequest request, ResourceUtilizationEquipment projectEquipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceUtilizationEquipment
                {
                    ResourceUtilizationEquipmentId = projectEquipment.ResourceUtilizationEquipmentId,
                    FleetsID = projectEquipment.FleetsID,
                    Unit = projectEquipment.Unit,
                    Quantity = projectEquipment.Quantity,
                    UnitFactory = projectEquipment.UnitFactory,
                    RentalCost = ConstructionInputsHelper.GetRentalRate(projectEquipment.FleetsID),
                    //EquipmentTypeID = projectEquipment.EquipmentTypeID,
                    //EquipmentIdentityID = projectEquipment.EquipmentIdentityID,
                    TotalCost = (projectEquipment.Quantity * projectEquipment.RentalCost * projectEquipment.UnitFactory),
                    EquipmentUnitCost = GetEquipmentUnitCost(projectEquipment.ResourceUtilizationWorkItemId),
                    EquipmentType = projectEquipment.EquipmentType,
                    OrgBranchName = projectEquipment.OrgBranchName,
                    DateCreated = projectEquipment.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectEquipment.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    ResourceUtilizationWorkItemId = projectEquipment.ResourceUtilizationWorkItemId,
                };
                db = new ExceedDbContext();
                entity.TotalCost = entity.RentalCost * entity.Quantity * entity.UnitFactory;

                db.ResourceUtilizationEquipment.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                //ProjectListController.UpdateProjectTotalCost(projectEquipment.WorkItemPerProjectId);
                ResourceUtilizationController controller = new ResourceUtilizationController();
                controller.UpdateResourceUtilizationTotalDirectCostOfWorkItem(entity.ResourceUtilizationWorkItemId);
            }

            return Json(new[] { projectEquipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UtilizationEquipment_Destroy([DataSourceRequest]DataSourceRequest request, ResourceUtilizationEquipment projectEquipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceUtilizationEquipment
                {
                    ResourceUtilizationEquipmentId = projectEquipment.ResourceUtilizationEquipmentId,
                    FleetsID = projectEquipment.FleetsID,
                    Unit = projectEquipment.Unit,
                    Quantity = projectEquipment.Quantity,
                    UnitFactory = projectEquipment.UnitFactory,
                    RentalCost = projectEquipment.RentalCost,
                    TotalCost = projectEquipment.TotalCost,
                    EquipmentUnitCost = projectEquipment.EquipmentUnitCost,
                    EquipmentType = projectEquipment.EquipmentType,
                    OrgBranchName = projectEquipment.OrgBranchName,
                    DateCreated = projectEquipment.DateCreated,
                    LastModified = projectEquipment.LastModified,
                    CreatedBy = projectEquipment.CreatedBy,
                    ModifiedBy = projectEquipment.ModifiedBy,
                    ResourceUtilizationWorkItemId = projectEquipment.ResourceUtilizationWorkItemId,
                    //EquipmentTypeID = projectEquipment.EquipmentTypeID,
                    //EquipmentIdentityID = projectEquipment.EquipmentIdentityID,
                };

                db.ResourceUtilizationEquipment.Attach(entity);
                db.ResourceUtilizationEquipment.Remove(entity);
                db.SaveChanges();
                //ProjectListController.UpdateProjectTotalCost(projectEquipment.WorkItemPerProjectId);

                ResourceUtilizationController controller = new ResourceUtilizationController();
                controller.UpdateResourceUtilizationTotalDirectCostOfWorkItem(entity.ResourceUtilizationWorkItemId);
            }

            return Json(new[] { projectEquipment }.ToDataSourceResult(request, ModelState));
        }
        #region Methods
        private decimal hourlyOutputRate;
        //private decimal GetRentalRate(Guid id)
        //{

        //    hourlyOutputRate = 0;
        //    //ConstructionInputs inputs = new ConstructionInputs();
        //    //var equipments = inputs.GetInternalExternalEquipmentViewModel();
        //    var allEquipments = db.Fleets;
        //    var fleet = db.Fleets.Where(x => x.FleetsID == id).FirstOrDefault();
        //    if (fleet != null)
        //    {
        //        if (fleet.IsExternal) {
        //            var equipment = db.MachinaryRent.Where(em => em.FleetsID == id).FirstOrDefault();
        //            if(equipment != null)
        //            {
        //                hourlyOutputRate = equipment.RentalWithFuel; 
        //            }
        //        }
        //        else
        //        {
        //            var equipment = db.InternalMachinaryRent.Where(im => im.FleetsID == id).FirstOrDefault();
        //            if(equipment != null)
        //            {
        //                hourlyOutputRate = equipment.HourlyRate;
        //            }
        //        }
        //        //  int fixedassetId = Convert.ToInt32(id);
        //        //var workItem = db.MachinaryRent.Where(x => x.RentOtherSuppliersID == id).FirstOrDefault();
        //        //if (workItem != null)
        //        //{
        //        //    hourlyOutputRate = workItem.RentalWithFuel;
        //        //    return hourlyOutputRate;
        //        //}
        //    }
        //    //else
        //    //{
        //    //    var workItemm = db.InternalMachinaryRent.Where(x => x.FleetsID == id).FirstOrDefault();
        //    //    if (workItemm != null)
        //    //    {
        //    //        hourlyOutputRate = workItemm.HourlyRate;
        //    //        return hourlyOutputRate;
        //    //    }
        //    //}
        //    return hourlyOutputRate;
        //}
        private decimal GetEquipmentUnitCost(int id)
        {
            decimal labourCost = 0;
            totalMaterialCost = 0;
            db = new ExceedDbContext();
            var workItem = db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();

            if (workItem != null)
            {
                var labouCosts =
                    db.ProjectEquipment.Where(x => x.WorkItemPerProjectId == id).ToList();
                if (labouCosts.Any())
                {
                    labourCost = labouCosts.Sum(x => x.TotalCost);
                }
                labourCost = labourCost / workItem.LabourHourlyOutPut;
            }
            totalMaterialCost = labourCost;
            return labourCost;
        }

        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}