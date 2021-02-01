using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class ProjectEquipmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private decimal totalMaterialCost = 0;
        public string Type { get; set; }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectEquipment_Read([DataSourceRequest]DataSourceRequest request, int idfive)
        {
            IQueryable<ProjectEquipment> projectequipment = db.ProjectEquipment.Where(x => x.WorkItemPerProjectId == idfive);
            DataSourceResult result = projectequipment.ToDataSourceResult(request, projectEquipment => new
            {
                ProjectEquipmentId = projectEquipment.ProjectEquipmentId,
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
                WorkItemPerProjectId = projectEquipment.WorkItemPerProjectId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEquipment_Create([DataSourceRequest]DataSourceRequest request, ProjectEquipment projectEquipment, int idsix)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipment
                {
                    FleetsID = projectEquipment.FleetsID,
                    Unit = projectEquipment.Unit,
                    Quantity = projectEquipment.Quantity,
                    UnitFactory = projectEquipment.UnitFactory,
                    RentalCost = projectEquipment.RentalCost,
                    //RentalCost = ConstructionInputsHelper.GetRentalRate(projectEquipment.FleetsID),
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
                    WorkItemPerProjectId = idsix,

                };
                entity.TotalCost = entity.RentalCost * entity.Quantity * entity.UnitFactory;
                db.ProjectEquipment.Add(entity);
                db.SaveChanges();
                projectEquipment.ProjectEquipmentId = entity.ProjectEquipmentId;
                ProjectListController.UpdateProjectTotalCost(idsix);

            }

            return Json(new[] { projectEquipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEquipment_Update([DataSourceRequest]DataSourceRequest request, ProjectEquipment projectEquipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipment
                {
                    ProjectEquipmentId = projectEquipment.ProjectEquipmentId,
                    FleetsID = projectEquipment.FleetsID,
                    Unit = projectEquipment.Unit,
                    Quantity = projectEquipment.Quantity,
                    UnitFactory = projectEquipment.UnitFactory,
                    RentalCost = projectEquipment.RentalCost,
                    //EquipmentTypeID = projectEquipment.EquipmentTypeID,
                    //EquipmentIdentityID = projectEquipment.EquipmentIdentityID,
                    TotalCost = (projectEquipment.Quantity * projectEquipment.RentalCost * projectEquipment.UnitFactory),
                    EquipmentUnitCost = GetEquipmentUnitCost(projectEquipment.WorkItemPerProjectId),
                    EquipmentType = projectEquipment.EquipmentType,
                    OrgBranchName = projectEquipment.OrgBranchName,
                    DateCreated = projectEquipment.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = projectEquipment.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    WorkItemPerProjectId = projectEquipment.WorkItemPerProjectId,
                };
                db = new ExceedDbContext();
                entity.TotalCost = entity.RentalCost * entity.Quantity * entity.UnitFactory;

                db.ProjectEquipment.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                ProjectListController.UpdateProjectTotalCost(projectEquipment.WorkItemPerProjectId);

            }

            return Json(new[] { projectEquipment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEquipment_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEquipment projectEquipment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEquipment
                {
                    ProjectEquipmentId = projectEquipment.ProjectEquipmentId,
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
                    WorkItemPerProjectId = projectEquipment.WorkItemPerProjectId,
                    //EquipmentTypeID = projectEquipment.EquipmentTypeID,
                    //EquipmentIdentityID = projectEquipment.EquipmentIdentityID,
                };

                db.ProjectEquipment.Attach(entity);
                db.ProjectEquipment.Remove(entity);
                db.SaveChanges();
                ProjectListController.UpdateProjectTotalCost(projectEquipment.WorkItemPerProjectId);

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
        public  decimal GetRentalRate(Guid id)
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
