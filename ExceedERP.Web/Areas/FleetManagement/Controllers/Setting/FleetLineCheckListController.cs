using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetInsurances;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
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
            DataSourceResult result = fleetLineCheckLists.ToDataSourceResult(request, fleetLineCheckList => new
            {
                FleetCheckListId = fleetLineCheckList.FleetCheckListId,
                FleetLineCheckListId = fleetLineCheckList.FleetLineCheckListId,
                Name = fleetLineCheckList.Name,
                Description = fleetLineCheckList.Description,
                Code = fleetLineCheckList.Code,
                IsActive = fleetLineCheckList.IsActive
            });
            return Json(result);
        }
        public ActionResult FleetLineCheckList_Create([DataSourceRequest] DataSourceRequest request, FleetLineCheckList fleetLineCheckList, int Id)
        {
            var checkList = db.FleetCheckLists.Where(x => x.FleetCheckListId == Id).FirstOrDefault();
            var countLineCheckList = db.FleetLineCheckLists.Where(x => x.FleetCheckListId == Id).Count();
            if (ModelState.IsValid)
            {
                var entity = new FleetLineCheckList
                {
                    FleetCheckListId = checkList.FleetCheckListId,
                    FleetLineCheckListId = fleetLineCheckList.FleetLineCheckListId,
                    Name = fleetLineCheckList.Name,
                    Description = fleetLineCheckList.Description,
                    Code = checkList.Code+"."+ countLineCheckList,
                    IsActive = fleetLineCheckList.IsActive

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
        public ActionResult FleetLineCheckList_Update([DataSourceRequest] DataSourceRequest request, FleetCheckList fleetCheckList)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetCheckList
                {
                    FleetCheckListId = fleetCheckList.FleetCheckListId,
                    EquipmentIdentityID = fleetCheckList.EquipmentIdentityID,
                    EquipmentTypeID = fleetCheckList.EquipmentTypeID,
                    Type = fleetCheckList.Type,
                    Name = fleetCheckList.Name,
                    IsMaintenace = fleetCheckList.IsMaintenace,
                    Code = fleetCheckList.Code,
                    Remark = fleetCheckList.Remark,
                    Selected = fleetCheckList.Selected
                };
                db.FleetCheckLists.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet insurance update", "Success", User.Identity.Name, $"updated insurance registration number {entity.FleetCheckListId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { fleetCheckList }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FleetLineCheckList_Destroy([DataSourceRequest] DataSourceRequest request, FleetCheckList fleetCheckList)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetCheckList
                {
                    FleetCheckListId = fleetCheckList.FleetCheckListId,
                    EquipmentIdentityID = fleetCheckList.EquipmentIdentityID,
                    EquipmentTypeID = fleetCheckList.EquipmentTypeID,
                    Type = fleetCheckList.Type,
                    Name = fleetCheckList.Name,
                    IsMaintenace = fleetCheckList.IsMaintenace,
                    Code = fleetCheckList.Code,
                    Remark = fleetCheckList.Remark,
                    Selected = fleetCheckList.Selected
                };
                db.FleetCheckLists.Attach(entity);
                db.FleetCheckLists.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted insurance registration number {entity.FleetCheckListId }");
                #endregion
            }
            return Json(new[] { fleetCheckList }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}