using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
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
            var fleetCheckLists = db.FleetCheckLists.Where(x=>x.DriverVehicleTransferID==null);

            DataSourceResult result = fleetCheckLists.ToDataSourceResult(request, fleetCheckList => new
            {
                FleetCheckListId = fleetCheckList.FleetCheckListId,
                EquipmentIdentityID = fleetCheckList.EquipmentIdentityID,
                EquipmentTypeID = fleetCheckList.EquipmentTypeID,
                
                Description = fleetCheckList.Description,
                Remark = fleetCheckList.Remark,
                
            });
            return Json(result);
        }
        public ActionResult DriverFleetCheckLists_Read([DataSourceRequest] DataSourceRequest request,string id)
        {
            var fleetCheckLists = db.FleetCheckLists.Where(x=>x.DriverVehicleTransferID==id);
            DataSourceResult result = fleetCheckLists.ToDataSourceResult(request, fleetCheckList => new
            {
                FleetCheckListId = fleetCheckList.FleetCheckListId,
                EquipmentIdentityID = fleetCheckList.EquipmentIdentityID,
                EquipmentTypeID = fleetCheckList.EquipmentTypeID,
                
                Description = fleetCheckList.Description,
                Remark = fleetCheckList.Remark,
                
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
                    
                    Description = fleetCheckList.Description,
                   
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
        public ActionResult DriverFleetCheckLists_Create([DataSourceRequest] DataSourceRequest request, FleetCheckList fleetCheckList,string id)
        {
            var fetchDriverMachine = db.DriverVehicleTransfer.FirstOrDefault(x => x.DriverVehicleTransferID.ToString() == id);
              
            if (ModelState.IsValid)
            {
                var count = db.FleetCheckLists.Count();
                var entity = new FleetCheckList
                {
                    FleetCheckListId = fleetCheckList.FleetCheckListId,
                    EquipmentIdentityID = fetchDriverMachine.EquipmentCategoryId,
                    EquipmentTypeID = fetchDriverMachine.EquipmentTypeId,
                    DriverVehicleTransferID = id,
                    Description = fleetCheckList.Description,
                   
                };
                db.FleetCheckLists.Add(entity);
                db.SaveChanges();
                var masterCheckList = db.FleetCheckLists.FirstOrDefault(x => x.EquipmentIdentityID == entity.EquipmentIdentityID&&x.EquipmentTypeID==entity.EquipmentTypeID&& x.DriverVehicleTransferID==null);
                 if(masterCheckList!=null)
                {
                    var checkListLines = db.FleetLineCheckLists.Where(x => x.FleetCheckListId == masterCheckList.FleetCheckListId).ToList();
                    foreach (var fcl in checkListLines)
                    {
                        var Lineentity = new FleetLineCheckList
                        {
                            FleetCheckListId = entity.FleetCheckListId,
                            SystemName = fcl.SystemName,
                            Description = fcl.Description,                            
                            CheckList = fcl.CheckList,
                            CreatedBy = User.Identity.Name,
                        };
                        db.FleetLineCheckLists.Add(Lineentity);
                    }
                    db.SaveChanges();
                }
                
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
                    
                    Description = fleetCheckList.Description,
                    Remark = fleetCheckList.Remark,
                    
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
                    
                    Description = fleetCheckList.Description,
                    Remark = fleetCheckList.Remark,
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