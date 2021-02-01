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

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class GenericProjectEquipmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult ProjectEquipment_Read([DataSourceRequest]DataSourceRequest request, int workItemId)
        {
            IQueryable<GenericProjectEquipment> projectequipment = db.GenericProjectEquipment.Where(x => x.GenericWorkItemPerProjectId == workItemId);
            DataSourceResult result = projectequipment.ToDataSourceResult(request, projectEquipment => new
            {
                GenericProjectEquipmentId = projectEquipment.GenericProjectEquipmentId,
                GenericWorkItemPerProjectId = projectEquipment.GenericWorkItemPerProjectId,

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
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEquipment_Create([DataSourceRequest]DataSourceRequest request, GenericProjectEquipment projectEquipment, int workItemId)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectEquipment
                {
                    FleetsID = projectEquipment.FleetsID,
                    Unit = projectEquipment.Unit,
                    Quantity = projectEquipment.Quantity,
                    UnitFactory = projectEquipment.UnitFactory,
                    RentalCost = projectEquipment.RentalCost,
                    TotalCost = (projectEquipment.Quantity * projectEquipment.RentalCost * projectEquipment.UnitFactory),
                    //EquipmentUnitCost = GetEquipmentUnitCost(idsix),
                    EquipmentType = projectEquipment.EquipmentType,
                    OrgBranchName = projectEquipment.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectEquipment.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = projectEquipment.ModifiedBy,
                    GenericWorkItemPerProjectId = workItemId,

                };
                entity.TotalCost = entity.RentalCost * entity.Quantity * entity.UnitFactory;
                db.GenericProjectEquipment.Add(entity);
                db.SaveChanges();
                projectEquipment.GenericProjectEquipmentId = entity.GenericProjectEquipmentId;

                ProjectEstimationHelper.UpdateGenericWorkItem(workItemId);

            }

            return Json(new[] { projectEquipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEquipment_Update([DataSourceRequest]DataSourceRequest request, GenericProjectEquipment projectEquipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectEquipment
                {
                    GenericProjectEquipmentId = projectEquipment.GenericProjectEquipmentId,
                    FleetsID = projectEquipment.FleetsID,
                    Unit = projectEquipment.Unit,
                    Quantity = projectEquipment.Quantity,
                    UnitFactory = projectEquipment.UnitFactory,
                    RentalCost = projectEquipment.RentalCost,
                    TotalCost = (projectEquipment.Quantity * projectEquipment.RentalCost * projectEquipment.UnitFactory),
                    //EquipmentUnitCost = GetEquipmentUnitCost(projectEquipment.WorkItemPerProjectId),
                    EquipmentType = projectEquipment.EquipmentType,
                    OrgBranchName = projectEquipment.OrgBranchName,
                    DateCreated = projectEquipment.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectEquipment.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    GenericWorkItemPerProjectId = projectEquipment.GenericWorkItemPerProjectId,
                };
                db = new ExceedDbContext();
                entity.TotalCost = entity.RentalCost * entity.Quantity * entity.UnitFactory;

                db.GenericProjectEquipment.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ProjectEstimationHelper.UpdateGenericWorkItem(entity.GenericWorkItemPerProjectId);

            }

            return Json(new[] { projectEquipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEquipment_Destroy([DataSourceRequest]DataSourceRequest request, GenericProjectEquipment projectEquipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new GenericProjectEquipment
                {
                    GenericProjectEquipmentId = projectEquipment.GenericProjectEquipmentId,
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
                    GenericWorkItemPerProjectId = projectEquipment.GenericWorkItemPerProjectId,
                    //EquipmentTypeID = projectEquipment.EquipmentTypeID,
                    //EquipmentIdentityID = projectEquipment.EquipmentIdentityID,
                };

                db.GenericProjectEquipment.Attach(entity);
                db.GenericProjectEquipment.Remove(entity);
                db.SaveChanges();

                ProjectEstimationHelper.UpdateGenericWorkItem(entity.GenericWorkItemPerProjectId);

            }

            return Json(new[] { projectEquipment }.ToDataSourceResult(request, ModelState));
        }
        #region Methods
        //private decimal hourlyOutputRate;
        //private decimal GetRentalRate(Guid id)
        //{

        //    hourlyOutputRate = 0;
        //    //ConstructionInputs inputs = new ConstructionInputs();
        //    //var equipments = inputs.GetInternalExternalEquipmentViewModel();
        //    var fleet = db.Fleets.Where(x => x.FleetsID == id).FirstOrDefault();
        //    if (fleet != null)
        //    {
        //        if (fleet.IsExternal)
        //        {
        //            var equipment = db.MachinaryRent.Where(em => em.FleetsID == id).FirstOrDefault();
        //            if (equipment != null)
        //            {
        //                hourlyOutputRate = equipment.RentalWithFuel;
        //            }
        //        }
        //        else
        //        {
        //            var equipment = db.InternalMachinaryRent.Where(im => im.FleetsID == id).FirstOrDefault();
        //            if (equipment != null)
        //            {
        //                hourlyOutputRate = equipment.HourlyRate;
        //            }
        //        }
        //        #region MyRegion
        //        //  int fixedassetId = Convert.ToInt32(id);
        //        //var workItem = db.MachinaryRent.Where(x => x.RentOtherSuppliersID == id).FirstOrDefault();
        //        //if (workItem != null)
        //        //{
        //        //    hourlyOutputRate = workItem.RentalWithFuel;
        //        //    return hourlyOutputRate;
        //        //} 
        //        #endregion
        //    }
        //    #region MyRegion
        //    //else
        //    //{
        //    //    var workItemm = db.InternalMachinaryRent.Where(x => x.FleetsID == id).FirstOrDefault();
        //    //    if (workItemm != null)
        //    //    {
        //    //        hourlyOutputRate = workItemm.HourlyRate;
        //    //        return hourlyOutputRate;
        //    //    }
        //    //} 
        //    #endregion
        //    return hourlyOutputRate;
        //}
        //private decimal GetEquipmentUnitCost(int id)
        //{
        //    decimal labourCost = 0;
        //    totalMaterialCost = 0;
        //    db = new ExceedDbContext();
        //    var workItem = db.WorkItemPerProject.Where(x => x.WorkItemPerProjectId == id).FirstOrDefault();

        //    if (workItem != null)
        //    {
        //        var labouCosts =
        //            db.ProjectEquipment.Where(x => x.WorkItemPerProjectId == id).ToList();
        //        if (labouCosts.Any())
        //        {
        //            labourCost = labouCosts.Sum(x => x.TotalCost);
        //        }
        //        labourCost = labourCost / workItem.LabourHourlyOutPut;
        //    }
        //    totalMaterialCost = labourCost;
        //    return labourCost;
        //}
        public decimal GetRentalRate(Guid id)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal hourlyOutputRate = 0;
            var fleet = db.Fleets.Where(x => x.FleetsID == id).FirstOrDefault();
            if (fleet != null)
            {
                if (fleet.IsExternal)
                {
                    var equipment = db.MachinaryRent.Where(em => em.FleetsID == id).FirstOrDefault();
                    if (equipment != null)
                    {
                        hourlyOutputRate = equipment.ApprovedCost;
                    }
                }
                else
                {
                    var equipment = db.InternalMachinaryRent.Where(im => im.FleetsID == id).FirstOrDefault();
                    if (equipment != null)
                    {
                        hourlyOutputRate = equipment.HourlyRate;
                    }
                }
            }

            return hourlyOutputRate;
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}