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
    public class TakeOffLinePerProjectController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            db.Configuration.ProxyCreationEnabled = false;
            return View();
        }

        public ActionResult TakeOffLinePerProject_Read([DataSourceRequest]DataSourceRequest request,int lineId)
        {
            IQueryable<TakeOffLinePerProject> takeofflineperproject = db.TakeOffLinePerProject.Where (x=> x.TakeOffPerProjectId ==lineId );
            DataSourceResult result = takeofflineperproject.ToDataSourceResult(request, takeOffLinePerProject => new {
                TakeOffLinePerProjectId = takeOffLinePerProject.TakeOffLinePerProjectId,
                No = takeOffLinePerProject.No,
                D = takeOffLinePerProject.D,
                Quantity = CalculateQuantity(takeOffLinePerProject),
                Description = takeOffLinePerProject.Description,
                UnitOfMeasurementId = takeOffLinePerProject.UnitOfMeasurementId,
                Length = takeOffLinePerProject.Length,
                Width = takeOffLinePerProject.Width,
                Depth = takeOffLinePerProject.Depth,
                IsDeductive = takeOffLinePerProject.IsDeductive,

                OrgBranchName = takeOffLinePerProject.OrgBranchName,
                DateCreated = takeOffLinePerProject.DateCreated,
                LastModified = takeOffLinePerProject.LastModified,
                CreatedBy = takeOffLinePerProject.CreatedBy,
                ModifiedBy = takeOffLinePerProject.ModifiedBy,
                TakeOffPerProjectId = lineId 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TakeOffLinePerProject_Create([DataSourceRequest]DataSourceRequest request, TakeOffLinePerProject takeOffLinePerProject, int lineIdTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new TakeOffLinePerProject
                {
                    No = takeOffLinePerProject.No,
                    D = takeOffLinePerProject.D,
                    Quantity = CalculateQuantity(takeOffLinePerProject),
                    UnitOfMeasurementId = this.GetTakeOffUnitOfMeasurement(lineIdTwo),
                    Description = takeOffLinePerProject.Description,
                    Length = takeOffLinePerProject.Length,
                    Width = takeOffLinePerProject.Width,
                    Depth = takeOffLinePerProject.Depth,
                    IsDeductive = takeOffLinePerProject.IsDeductive,

                    OrgBranchName = takeOffLinePerProject.OrgBranchName,
                    DateCreated = DateTime .Now ,
                    LastModified = takeOffLinePerProject.LastModified,
                    CreatedBy = User .Identity .Name ,
                    ModifiedBy = takeOffLinePerProject.ModifiedBy,
                    TakeOffPerProjectId = lineIdTwo
                };

                db.TakeOffLinePerProject.Add(entity);
                db.SaveChanges();
                takeOffLinePerProject.TakeOffLinePerProjectId = entity.TakeOffLinePerProjectId;

                ProjectEstimationHelper.UpdateWorkItemPerProjecQuantityOfWork(entity.TakeOffPerProjectId);
            }

            return Json(new[] { takeOffLinePerProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TakeOffLinePerProject_Update([DataSourceRequest]DataSourceRequest request, TakeOffLinePerProject takeOffLinePerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new TakeOffLinePerProject
                {
                    TakeOffLinePerProjectId = takeOffLinePerProject.TakeOffLinePerProjectId,
                    No = takeOffLinePerProject.No,
                    D = takeOffLinePerProject.D,
                    Quantity = CalculateQuantity(takeOffLinePerProject),
                    Description = takeOffLinePerProject.Description,
                    UnitOfMeasurementId = takeOffLinePerProject.UnitOfMeasurementId,
                    Length = takeOffLinePerProject.Length,
                    Width = takeOffLinePerProject.Width,
                    Depth = takeOffLinePerProject.Depth,
                    IsDeductive = takeOffLinePerProject.IsDeductive,

                    OrgBranchName = takeOffLinePerProject.OrgBranchName,
                    DateCreated = takeOffLinePerProject.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = takeOffLinePerProject.CreatedBy,
                    ModifiedBy = User .Identity .Name,
                    TakeOffPerProjectId = takeOffLinePerProject.TakeOffPerProjectId
                };

                db.TakeOffLinePerProject.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ProjectEstimationHelper.UpdateWorkItemPerProjecQuantityOfWork(entity.TakeOffPerProjectId);
            }

            return Json(new[] { takeOffLinePerProject }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TakeOffLinePerProject_Destroy([DataSourceRequest]DataSourceRequest request, TakeOffLinePerProject takeOffLinePerProject)
        {
            if (ModelState.IsValid)
            {
                var entity = new TakeOffLinePerProject
                {
                    TakeOffLinePerProjectId = takeOffLinePerProject.TakeOffLinePerProjectId,
                    No = takeOffLinePerProject.No,
                    D = takeOffLinePerProject.D,
                    Quantity = CalculateQuantity(takeOffLinePerProject),
                    Description = takeOffLinePerProject.Description,
                    UnitOfMeasurementId = takeOffLinePerProject.UnitOfMeasurementId,
                    Length = takeOffLinePerProject.Length,
                    Width = takeOffLinePerProject.Width,
                    Depth = takeOffLinePerProject.Depth,
                    IsDeductive = takeOffLinePerProject.IsDeductive,

                    OrgBranchName = takeOffLinePerProject.OrgBranchName,
                    DateCreated = takeOffLinePerProject.DateCreated,
                    LastModified = takeOffLinePerProject.LastModified,
                    CreatedBy = takeOffLinePerProject.CreatedBy,
                    ModifiedBy = takeOffLinePerProject.ModifiedBy,
                    TakeOffPerProjectId = takeOffLinePerProject.TakeOffPerProjectId
                };

                db.TakeOffLinePerProject.Attach(entity);
                db.TakeOffLinePerProject.Remove(entity);
                db.SaveChanges();

                ProjectEstimationHelper.UpdateWorkItemPerProjecQuantityOfWork(entity.TakeOffPerProjectId);
            }

            return Json(new[] { takeOffLinePerProject }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private decimal CalculateQuantity(TakeOffLinePerProject model) {
            decimal total = new decimal();
            if(model != null)
            {
                total = model.No * model.Length * model.Width * model.Depth;
                if (model.IsDeductive)
                {
                    total = -1 * total;
                }
            }
            return total;
        }
        private int GetTakeOffUnitOfMeasurement(int takeOffId) {
            WorkItemPerProject workItem = new WorkItemPerProject();
            var currentTakeOff = db.TakeOffPerProject.Find(takeOffId);
            if(currentTakeOff != null)
            {
                var workItemId = currentTakeOff.WorkItemPerProjectId;
                var temp = db.WorkItemPerProject.Find(workItemId);
                if(temp != null)
                {
                    workItem = temp;
                }
            }
            return workItem.UnitOfMeasurementId??0;
        }
    }
}
