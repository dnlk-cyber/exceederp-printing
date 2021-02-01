using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Receives
{
    [AuthorizeRoles(InventoryConstants.PropertyInspector, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
    public class ItemInspectionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }


        [AuthorizeRoles(InventoryConstants.PropertyInspector, InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult ItemInspections_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<ItemInspection> iteminspections = _db.ItemInspections.Where(i => i.GoodReceiveId == id);
            DataSourceResult result = iteminspections.ToDataSourceResult(request, itemInspection => new {
                ItemInspectionId = itemInspection.ItemInspectionId,
                GoodReceiveId = itemInspection.GoodReceiveId,
                InspectionDate = itemInspection.InspectionDate,
                EmployeeId = itemInspection.EmployeeId,
                Accepted = itemInspection.Accepted,
                Remark = itemInspection.Remark,

                CreatedBy = itemInspection.CreatedBy,
                DateCreated = itemInspection.DateCreated,
                ModifiedBy = itemInspection.ModifiedBy,
                LastModified = itemInspection.LastModified,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyInspector)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemInspections_Create([DataSourceRequest]DataSourceRequest request, ItemInspection itemInspection , int id)
            {
          
           
            if (ModelState.IsValid)
            {
                var entity = new ItemInspection
                {
                    InspectionDate = itemInspection.InspectionDate,
                    GoodReceiveId = id,
                    EmployeeId = itemInspection.EmployeeId,
                    Accepted = itemInspection.Accepted,
                    Remark = itemInspection.Remark,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = itemInspection.ModifiedBy,
                    LastModified = itemInspection.LastModified,
                };

                _db.ItemInspections.Add(entity);
                _db.SaveChanges();
                itemInspection.ItemInspectionId = entity.ItemInspectionId;


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good receive inspection", "Success", User.Identity.Name, $"created good receive inspection under record {entity.GoodReceiveId} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { itemInspection }.ToDataSourceResult(request, ModelState) );
        }

        [AuthorizeRoles(InventoryConstants.PropertyInspector)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemInspections_Update([DataSourceRequest]DataSourceRequest request, ItemInspection itemInspection)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemInspection
                {
                    GoodReceiveId = itemInspection.GoodReceiveId,
                    ItemInspectionId = itemInspection.ItemInspectionId,
                    InspectionDate = itemInspection.InspectionDate,
                    EmployeeId = itemInspection.EmployeeId,
                    Accepted = itemInspection.Accepted,
                    Remark = itemInspection.Remark,


                    CreatedBy = itemInspection.CreatedBy,
                    DateCreated = itemInspection.DateCreated,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now

                };

                _db.ItemInspections.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good receive inspection", "Success", User.Identity.Name, $"updated good receive inspection under record {entity.GoodReceiveId} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { itemInspection }.ToDataSourceResult(request, ModelState) );
        }

        [AuthorizeRoles(InventoryConstants.PropertyInspector)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemInspections_Destroy([DataSourceRequest]DataSourceRequest request, ItemInspection itemInspection)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemInspection
                {
                    ItemInspectionId = itemInspection.ItemInspectionId,
                    InspectionDate = itemInspection.InspectionDate,
                    EmployeeId = itemInspection.EmployeeId,
                    Accepted = itemInspection.Accepted,
                    Remark = itemInspection.Remark,
                };

                _db.ItemInspections.Attach(entity);
                _db.ItemInspections.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted item inspection " + itemInspection.GoodReceiveId);
                #endregion
            }

            return Json(new[] { itemInspection }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
