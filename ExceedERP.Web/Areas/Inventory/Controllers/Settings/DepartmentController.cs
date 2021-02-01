using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser)]
    public class DepartmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
           
            return View();
        }
   [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser)]

        public ActionResult DepartmentCostCenters_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DepartmentCostCenter> departmentcostcenters = db.DepartmentCostCenters;
            DataSourceResult result = departmentcostcenters.ToDataSourceResult(request, departmentCostCenter => new {
                DepartmentCostCenterID = departmentCostCenter.DepartmentCostCenterID,
                DepartmentName = departmentCostCenter.DepartmentName,
                LocationCode = departmentCostCenter.LocationCode
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentCostCenters_Create([DataSourceRequest]DataSourceRequest request, DepartmentCostCenter departmentCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new DepartmentCostCenter
                {
                    DepartmentName = departmentCostCenter.DepartmentName,
                    LocationCode = departmentCostCenter.LocationCode
                };

                db.DepartmentCostCenters.Add(entity);
                db.SaveChanges();
                departmentCostCenter.DepartmentCostCenterID = entity.DepartmentCostCenterID;
            }

            return Json(new[] { departmentCostCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentCostCenters_Update([DataSourceRequest]DataSourceRequest request, DepartmentCostCenter departmentCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new DepartmentCostCenter
                {
                    DepartmentCostCenterID = departmentCostCenter.DepartmentCostCenterID,
                    DepartmentName = departmentCostCenter.DepartmentName,
                    LocationCode = departmentCostCenter.LocationCode
                };

                db.DepartmentCostCenters.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { departmentCostCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentCostCenters_Destroy([DataSourceRequest]DataSourceRequest request, DepartmentCostCenter departmentCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new DepartmentCostCenter
                {
                    DepartmentCostCenterID = departmentCostCenter.DepartmentCostCenterID,
                    DepartmentName = departmentCostCenter.DepartmentName,
                    LocationCode = departmentCostCenter.LocationCode
                };

                db.DepartmentCostCenters.Attach(entity);
                db.DepartmentCostCenters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { departmentCostCenter }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
