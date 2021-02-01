using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ProjectEstimationMobilizationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectEstimationMobilization_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectEstimationMobilization> projectestimationmobilization = db.ProjectEstimationMobilization;
            DataSourceResult result = projectestimationmobilization.ToDataSourceResult(request, projectEstimationMobilization => new {
                ProjectEstimationMobilizationId = projectEstimationMobilization.ProjectEstimationMobilizationId,
                ItemNo = projectEstimationMobilization.ItemNo,
                TypeOfMachinery = projectEstimationMobilization.TypeOfMachinery,
                Quantity = projectEstimationMobilization.Quantity,
                Trip = projectEstimationMobilization.Trip,
                Distance = projectEstimationMobilization.Distance,
                Speed = projectEstimationMobilization.Speed,
                Time = projectEstimationMobilization.Time,
                RentalCost = projectEstimationMobilization.RentalCost,
                TotalCost = projectEstimationMobilization.TotalCost,
                StaffName = projectEstimationMobilization.StaffName,
                StaffQuantity = projectEstimationMobilization.StaffQuantity,
                StaffTime = projectEstimationMobilization.StaffTime,
                Perdiem = projectEstimationMobilization.Perdiem,
                StaffTotalCost = projectEstimationMobilization.StaffTotalCost,
                OrgBranchName = projectEstimationMobilization.OrgBranchName,
                DateCreated = projectEstimationMobilization.DateCreated,
                LastModified = projectEstimationMobilization.LastModified,
                CreatedBy = projectEstimationMobilization.CreatedBy,
                ModifiedBy = projectEstimationMobilization.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationMobilization_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationMobilization projectEstimationMobilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationMobilization
                {
                    ItemNo = projectEstimationMobilization.ItemNo,
                    TypeOfMachinery = projectEstimationMobilization.TypeOfMachinery,
                    Quantity = projectEstimationMobilization.Quantity,
                    Trip = projectEstimationMobilization.Trip,
                    Distance = projectEstimationMobilization.Distance,
                    Speed = projectEstimationMobilization.Speed,
                    Time = projectEstimationMobilization.Time,
                    RentalCost = projectEstimationMobilization.RentalCost,
                    TotalCost = projectEstimationMobilization.TotalCost,
                    StaffName = projectEstimationMobilization.StaffName,
                    StaffQuantity = projectEstimationMobilization.StaffQuantity,
                    StaffTime = projectEstimationMobilization.StaffTime,
                    Perdiem = projectEstimationMobilization.Perdiem,
                    StaffTotalCost = projectEstimationMobilization.StaffTotalCost,
                    OrgBranchName = projectEstimationMobilization.OrgBranchName,
                    DateCreated = DateTime .Now ,
                    LastModified = projectEstimationMobilization.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = projectEstimationMobilization.ModifiedBy 
                };

                db.ProjectEstimationMobilization.Add(entity);
                db.SaveChanges();
                projectEstimationMobilization.ProjectEstimationMobilizationId = entity.ProjectEstimationMobilizationId;
            }

            return Json(new[] { projectEstimationMobilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationMobilization_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationMobilization projectEstimationMobilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationMobilization
                {
                    ProjectEstimationMobilizationId = projectEstimationMobilization.ProjectEstimationMobilizationId,
                    ItemNo = projectEstimationMobilization.ItemNo,
                    TypeOfMachinery = projectEstimationMobilization.TypeOfMachinery,
                    Quantity = projectEstimationMobilization.Quantity,
                    Trip = projectEstimationMobilization.Trip,
                    Distance = projectEstimationMobilization.Distance,
                    Speed = projectEstimationMobilization.Speed,
                    Time = projectEstimationMobilization.Time,
                    RentalCost = projectEstimationMobilization.RentalCost,
                    TotalCost = projectEstimationMobilization.TotalCost,
                    StaffName = projectEstimationMobilization.StaffName,
                    StaffQuantity = projectEstimationMobilization.StaffQuantity,
                    StaffTime = projectEstimationMobilization.StaffTime,
                    Perdiem = projectEstimationMobilization.Perdiem,
                    StaffTotalCost = projectEstimationMobilization.StaffTotalCost,
                    OrgBranchName = projectEstimationMobilization.OrgBranchName,
                    DateCreated = projectEstimationMobilization.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectEstimationMobilization.CreatedBy,
                    ModifiedBy =User.Identity .Name 
                };

                db.ProjectEstimationMobilization.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationMobilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationMobilization_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationMobilization projectEstimationMobilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationMobilization
                {
                    ProjectEstimationMobilizationId = projectEstimationMobilization.ProjectEstimationMobilizationId,
                    ItemNo = projectEstimationMobilization.ItemNo,
                    TypeOfMachinery = projectEstimationMobilization.TypeOfMachinery,
                    Quantity = projectEstimationMobilization.Quantity,
                    Trip = projectEstimationMobilization.Trip,
                    Distance = projectEstimationMobilization.Distance,
                    Speed = projectEstimationMobilization.Speed,
                    Time = projectEstimationMobilization.Time,
                    RentalCost = projectEstimationMobilization.RentalCost,
                    TotalCost = projectEstimationMobilization.TotalCost,
                    StaffName = projectEstimationMobilization.StaffName,
                    StaffQuantity = projectEstimationMobilization.StaffQuantity,
                    StaffTime = projectEstimationMobilization.StaffTime,
                    Perdiem = projectEstimationMobilization.Perdiem,
                    StaffTotalCost = projectEstimationMobilization.StaffTotalCost,
                    OrgBranchName = projectEstimationMobilization.OrgBranchName,
                    DateCreated = projectEstimationMobilization.DateCreated,
                    LastModified = projectEstimationMobilization.LastModified,
                    CreatedBy = projectEstimationMobilization.CreatedBy,
                    ModifiedBy = projectEstimationMobilization.ModifiedBy 
                };

                db.ProjectEstimationMobilization.Attach(entity);
                db.ProjectEstimationMobilization.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationMobilization }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
