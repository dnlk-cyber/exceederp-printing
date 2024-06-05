using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.MainTools
{
  //  [AuthorizeRoles(MaintenanceRoles.MaintenanceToolsStoreUser)]
    public class MaintenanceToolIssueAndReturnController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Tools_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MaintenanceToolIssueAndReturn> tools = db.MaintenanceToolIssueAndReturn;
            DataSourceResult result = tools.ToDataSourceResult(request, tool => new
            {
                MaintenanceToolIssueAndReturnId = tool.MaintenanceToolIssueAndReturnId,
                MechanicCrewID = tool.MechanicCrewID,
                MaintenanceToolID = tool.MaintenanceToolID,
                IssueDate = tool.IssueDate,
                IssuedQuantity = tool.IssuedQuantity,
                ReturnDate = tool.ReturnDate,
                Remark = tool.Remark,
                Status = tool.Status,

                DateCreated = tool.DateCreated,
                LastModified = tool.LastModified,
                CreatedBy = tool.CreatedBy,
                ModifiedBy = tool.ModifiedBy
            });

            return Json(result);
        }
        //private bool CategoryExists(MaintenanceToolIssueAndReturn model)
        //{
        //    return db.MaintenanceToolIssueAndReturn.FirstOrDefault(me => me.MaintenanceToolID == model.MaintenanceToolID && 
        //    me.MechanicCrewID == model.MechanicCrewID && me.SpareMaterialCategoryId != model.SpareMaterialCategoryId) != null;
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Tool_Create([DataSourceRequest]DataSourceRequest request, MaintenanceToolIssueAndReturn tool)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                
                var entity = new MaintenanceToolIssueAndReturn
                {
                    MaintenanceToolIssueAndReturnId = Guid.NewGuid(),
                    MechanicCrewID = tool.MechanicCrewID,
                    MaintenanceToolID = tool.MaintenanceToolID,
                    IssueDate = tool.IssueDate,
                    IssuedQuantity = tool.IssuedQuantity,
                    ReturnDate = tool.ReturnDate,
                    Remark = tool.Remark,
                    Status = MaintenanceToolStatus.Issued,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.MaintenanceToolIssueAndReturn.Add(entity);
                db.SaveChanges();
                tool.MaintenanceToolIssueAndReturnId = entity.MaintenanceToolIssueAndReturnId;

                //this.UpdateAllEquipmentMaintenanceIntervalOnIntervalCreate(entity);
                //}
            }

            return Json(new[] { tool }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Tool_Update([DataSourceRequest]DataSourceRequest request, MaintenanceToolIssueAndReturn tool)
        {
            if (ModelState.IsValid)
            {


                //if (CategoryExists(tool))
                //{
                //    ModelState.AddModelError("SpareMaterialCategoryId", "Duplicate tool not allowed!");
                //}
                //else
                //{
                var entity = new MaintenanceToolIssueAndReturn
                {

                    MaintenanceToolIssueAndReturnId = tool.MaintenanceToolIssueAndReturnId,
                    MechanicCrewID = tool.MechanicCrewID,
                    MaintenanceToolID = tool.MaintenanceToolID,
                    IssueDate = tool.IssueDate,
                    IssuedQuantity = tool.IssuedQuantity,
                    ReturnDate = tool.ReturnDate,
                    Remark = tool.Remark,
                    Status = tool.Status,

                    DateCreated = tool.DateCreated,
                    CreatedBy = tool.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };



                db.MaintenanceToolIssueAndReturn.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //}

            }

            return Json(new[] { tool }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Tool_Return([DataSourceRequest]DataSourceRequest request, MaintenanceToolIssueAndReturn tool)
        {
            if (ModelState.IsValid)
            {


                //if (CategoryExists(tool))
                //{
                //    ModelState.AddModelError("SpareMaterialCategoryId", "Duplicate tool not allowed!");
                //}
                //else
                //{
                var entity = new MaintenanceToolIssueAndReturn
                {

                    MaintenanceToolIssueAndReturnId = tool.MaintenanceToolIssueAndReturnId,
                    MechanicCrewID = tool.MechanicCrewID,
                    MaintenanceToolID = tool.MaintenanceToolID,
                    IssueDate = tool.IssueDate,
                    IssuedQuantity = tool.IssuedQuantity,
                    ReturnDate = tool.ReturnDate,
                    Remark = tool.Remark,
                    Status = MaintenanceToolStatus.Returned,

                    DateCreated = tool.DateCreated,
                    CreatedBy = tool.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };



                db.MaintenanceToolIssueAndReturn.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //}

            }

            return Json(new[] { tool }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Tool_Destroy([DataSourceRequest]DataSourceRequest request, MaintenanceToolIssueAndReturn tool)
        {
            if (ModelState.IsValid)
            {
                var entity = new MaintenanceToolIssueAndReturn
                {
                    MaintenanceToolIssueAndReturnId = tool.MaintenanceToolIssueAndReturnId,
                    MechanicCrewID = tool.MechanicCrewID,
                    MaintenanceToolID = tool.MaintenanceToolID,
                    IssueDate = tool.IssueDate,
                    IssuedQuantity = tool.IssuedQuantity,
                    ReturnDate = tool.ReturnDate,
                    Remark = tool.Remark,
                    Status = tool.Status,

                    DateCreated = tool.DateCreated,
                    LastModified = tool.LastModified,
                    CreatedBy = tool.CreatedBy,
                    ModifiedBy = tool.ModifiedBy
                };

                db.MaintenanceToolIssueAndReturn.Attach(entity);
                db.MaintenanceToolIssueAndReturn.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { tool }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}

