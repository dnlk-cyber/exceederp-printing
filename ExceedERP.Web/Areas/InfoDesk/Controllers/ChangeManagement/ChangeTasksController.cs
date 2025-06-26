using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Models;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.InfoDesk.ChangeManagement;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    [Authorize]
    public class ChangeTasksController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        ApplicationDbContext udb = new ApplicationDbContext();
       
        
       
        public ActionResult ChangeTasks_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
           
            var changeTasks = db.ChangeTasks.Where(x=>x.ChangeRequestId==id).ToList();
            DataSourceResult result = changeTasks.ToDataSourceResult(request, changeTask => new ChangeTask
            {
                ChangeTaskId = changeTask.ChangeTaskId,
                ChangeRequestId = changeTask.ChangeRequestId,
                CompletionDate = changeTask.CompletionDate,
                CreatedBy = changeTask.CreatedBy,
                Description = changeTask.Description,
                OrgStructureId = changeTask.OrgStructureId,
                Responsiblity = changeTask.Responsiblity,
                EmployeeId = changeTask.EmployeeId,
                IsCompleted = changeTask.IsCompleted,
                DateCreated = changeTask.DateCreated,
                LastModified = changeTask.LastModified,
                ModifiedBy = changeTask.ModifiedBy,
               


            });

            return Json(result);
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeTasks_Create([DataSourceRequest]DataSourceRequest request, ChangeTask changeRequest,int id)
        {
    
            changeRequest.DateCreated = DateTime.Now;
            changeRequest.CreatedBy = User.Identity.Name;
            changeRequest.ChangeRequestId = id;
            changeRequest.EmployeeId = db.Placements
                                        .Where(x => x.OrgStructureId == changeRequest.OrgStructureId
                                                     && x.IsHead)
                                        .OrderByDescending(x => x.PlacementId)
                                        .FirstOrDefault()?.EmployeeId ?? 0;

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(changeRequest);
                db.ChangeTasks.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { changeRequest}.ToDataSourceResult(request));
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeTasks_Update([DataSourceRequest]DataSourceRequest request, ChangeTask changeRequest)
        {

            changeRequest.EmployeeId = db.Placements
                                .Where(x => x.OrgStructureId == changeRequest.OrgStructureId
                                             && x.IsHead)
                                .OrderByDescending(x => x.PlacementId)
                                .FirstOrDefault()?.EmployeeId ?? 0;

            if (ModelState.IsValid)
            {
                changeRequest.LastModified = DateTime.Now;
                changeRequest.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(changeRequest);
                db.ChangeTasks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { changeRequest }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ChangeTasks_Destroy([DataSourceRequest]DataSourceRequest request, ChangeTask changeTask)
        {
            if (ModelState.IsValid)
            {

                var entity = GetMappedObject(changeTask);
                db.ChangeTasks.Attach(entity);
                db.ChangeTasks.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted change Task " + changeTask.ChangeTaskId);

               
            }

            return Json(new[] { changeTask }.ToDataSourceResult(request));
        }
        
        private ChangeTask GetMappedObject(ChangeTask changeTask)
        {
            var entity = new ChangeTask
            {
                ChangeTaskId = changeTask.ChangeTaskId,
                ChangeRequestId = changeTask.ChangeRequestId,
                CompletionDate = changeTask.CompletionDate,
                CreatedBy = changeTask.CreatedBy,
                Description = changeTask.Description,
                OrgStructureId = changeTask.OrgStructureId,
                Responsiblity = changeTask.Responsiblity,
                EmployeeId = changeTask.EmployeeId,
                IsCompleted = changeTask.IsCompleted,
                DateCreated = changeTask.DateCreated,
                LastModified = changeTask.LastModified,
                ModifiedBy = changeTask.ModifiedBy,

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
