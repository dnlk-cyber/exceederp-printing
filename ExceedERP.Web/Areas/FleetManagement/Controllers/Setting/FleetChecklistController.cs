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
    public class FleetChecklistController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["EquipmentIdentity"] = db.EquipmentIdentity.Select(x => new { x.EquipmentIdentityID, x.EquipmentCategoryName }).ToList();
            ViewData["EquipmentType"] = db.EquipmentType.Select(x => new { x.EquipmentTypeID, x.EquipmentTypeName }).ToList();
            return View();
        }
        public ActionResult FleetCheckLists_Read([DataSourceRequest] DataSourceRequest request)
        {
            var fleetCheckLists = db.FleetCheckLists;
            DataSourceResult result = fleetCheckLists.ToDataSourceResult(request, fleetCheckList => new
            {
                FleetCheckListId = fleetCheckList.FleetCheckListId,
                EquipmentIdentityID = fleetCheckList.EquipmentIdentityID,
                EquipmentTypeID = fleetCheckList.EquipmentTypeID,
                Type = fleetCheckList.Type,
                Name = fleetCheckList.Name,
                IsMaintenace = fleetCheckList.IsMaintenace,
                Code = fleetCheckList.Code,
                Description = fleetCheckList.Description,
                Remark = fleetCheckList.Remark,
                Selected = fleetCheckList.Selected
            });
            return Json(result);
        }
        public ActionResult FleetCheckLists_Create([DataSourceRequest] DataSourceRequest request, FleetCheckList fleetCheckList)
        {
            if (ModelState.IsValid)
            {
                var count = db.FleetCheckLists.Count();
                var entity = new FleetCheckList
                {
                    FleetCheckListId = fleetCheckList.FleetCheckListId,
                    EquipmentIdentityID = fleetCheckList.EquipmentIdentityID,
                    EquipmentTypeID = fleetCheckList.EquipmentTypeID,
                    Type = fleetCheckList.Type,
                    Name = fleetCheckList.Name,
                    IsMaintenace = fleetCheckList.IsMaintenace,
                    Code = (count + 1).ToString(),
                    Description = fleetCheckList.Description,
                    Selected = fleetCheckList.Selected

                };
                db.FleetCheckLists.Add(entity);
                db.SaveChanges();
                fleetCheckList.FleetCheckListId = entity.FleetCheckListId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Fleet checklist registration", "Success", User.Identity.Name, $"created checklist registration {entity.FleetCheckListId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { fleetCheckList }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetCheckLists_Update([DataSourceRequest] DataSourceRequest request, FleetCheckList fleetCheckList)
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
                    Description = fleetCheckList.Description,
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
        public ActionResult FleetCheckLists_Destroy([DataSourceRequest] DataSourceRequest request, FleetCheckList fleetCheckList)
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
                    Description = fleetCheckList.Description,
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