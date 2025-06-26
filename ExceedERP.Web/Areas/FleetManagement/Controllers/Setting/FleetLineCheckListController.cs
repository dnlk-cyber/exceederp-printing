using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    //[AuthorizeRoles(FleetManagementRoles.FleetTimeSheet)]
    public class FleetLineCheckListController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["EquipmentIdentity"] = db.EquipmentIdentity.ToList();
            ViewData["EquipmentType"] = db.EquipmentType.ToList();
            return View();
        }
        public ActionResult FleetLineCheckList_Read([DataSourceRequest] DataSourceRequest request,int Id)
        {
            var fleetLineCheckLists = db.FleetLineCheckLists.Where(x=>x.FleetCheckListId==Id);
            DataSourceResult result = fleetLineCheckLists.ToDataSourceResult(request, fleetLineCheckList => new FleetLineCheckList
            {
                FleetCheckListId = fleetLineCheckList.FleetCheckListId,
                FleetLineCheckListId = fleetLineCheckList.FleetLineCheckListId,
                SystemName = fleetLineCheckList.SystemName,
                Description = fleetLineCheckList.Description,
                Code = fleetLineCheckList.Code,
                IsActive = fleetLineCheckList.IsActive,
                CheckList = fleetLineCheckList.CheckList,
                CreatedBy = fleetLineCheckList.CreatedBy,
                DateCreated = fleetLineCheckList.DateCreated,
                
            });
            return Json(result);
        }
        public ActionResult FleetLineCheckList_Create([DataSourceRequest] DataSourceRequest request, FleetLineCheckList fleetLineCheckList, int Id)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetLineCheckList
                {
                    FleetCheckListId = Id,
                    FleetLineCheckListId = fleetLineCheckList.FleetLineCheckListId,
                    SystemName = fleetLineCheckList.SystemName,
                    Description = fleetLineCheckList.Description,
                    IsActive = fleetLineCheckList.IsActive,
                    CheckList = fleetLineCheckList.CheckList,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today
                };
                db.FleetLineCheckLists.Add(entity);
                db.SaveChanges();
                fleetLineCheckList.FleetLineCheckListId = entity.FleetLineCheckListId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet line checklist registration", "Success", User.Identity.Name, $"created checklist registration {entity.FleetLineCheckListId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { fleetLineCheckList }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetLineCheckList_Update([DataSourceRequest] DataSourceRequest request, FleetLineCheckList model)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetLineCheckList
                {
                    FleetCheckListId = model.FleetCheckListId,
                    FleetLineCheckListId = model.FleetLineCheckListId,
                    SystemName = model.SystemName,
                    Description = model.Description,
                    IsActive = model.IsActive,
                    CheckList = model.CheckList,
                    DateCreated = model.DateCreated,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Today
                };
                db.FleetLineCheckLists.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                model.FleetLineCheckListId = entity.FleetLineCheckListId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance update", "Success", User.Identity.Name, $"updated insurance registration number {entity.FleetCheckListId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FleetLineCheckList_Destroy([DataSourceRequest] DataSourceRequest request, FleetLineCheckList model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.FleetLineCheckLists.Find(model.FleetLineCheckListId);
                db.FleetLineCheckLists.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted insurance registration number {entity.FleetCheckListId }");
                #endregion
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}