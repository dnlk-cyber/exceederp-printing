using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers
{

    public class ServiceReceiveItemController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly DistributionHelper _distributionHelper = new DistributionHelper();

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser, ProcurementRoles.ProcurementUserDepartmentApprover)]
        public ActionResult ServiceReceiveItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            IQueryable<ServiceReceiveItem> serviceReceiveItems = _db.ServiceReceiveItems.Where(gi => gi.ServiceReceiveId == id);
            DataSourceResult result = serviceReceiveItems.ToDataSourceResult(request, serviceReceiveItem => new
            {

                ServiceReceiveItemId = serviceReceiveItem.ServiceReceiveItemId,
                ServiceReceiveId = serviceReceiveItem.ServiceReceiveId,
                ServiceCategoryCode = serviceReceiveItem.ServiceCategoryCode,
                ServiceCategoryName = serviceReceiveItem.ServiceCategoryName,
                ServiceItemCode = serviceReceiveItem.ServiceItemCode,
                ServiceItemName = serviceReceiveItem.ServiceItemName,
                Quantity = serviceReceiveItem.Quantity,
                UnitPrice = serviceReceiveItem.UnitPrice,
                SubTotal = serviceReceiveItem.SubTotal,
                Total = serviceReceiveItem.Total,
                GLTaxRateID = serviceReceiveItem.GLTaxRateID,
                TaxAmount = serviceReceiveItem.TaxAmount
            });

            return Json(result);
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceReceiveItems_Create([DataSourceRequest]DataSourceRequest request, ServiceReceiveItem serviceReceiveItem, int id)
        {
            var tax = _db.GLTaxRates.Find(serviceReceiveItem.GLTaxRateID);
            var exist = _db.ServiceReceiveItems.Where(x => x.ServiceReceiveId == id).Where(x => x.ServiceItemCode == serviceReceiveItem.ServiceItemCode).AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + serviceReceiveItem.ServiceItemCode + " already exists");
            }
            if (serviceReceiveItem.UnitPrice==0)
            {
                ModelState.AddModelError("zero", "Item " + serviceReceiveItem.ServiceItemCode + " price can't be zero");
            }
            if (ModelState.IsValid)
            {
                if (tax != null)
                {
                    var entity = new ServiceReceiveItem
                    {
                        ServiceReceiveId = id,
                        ServiceCategoryCode = serviceReceiveItem.ServiceCategoryCode,
                        ServiceCategoryName = serviceReceiveItem.ServiceCategoryName,
                        ServiceItemCode = serviceReceiveItem.ServiceItemCode,
                        ServiceItemName = serviceReceiveItem.ServiceItemName,
                        Quantity = serviceReceiveItem.Quantity,
                        UnitPrice = serviceReceiveItem.UnitPrice,
                        SubTotal = serviceReceiveItem.UnitPrice * serviceReceiveItem.Quantity,
                        GLTaxRateID = serviceReceiveItem.GLTaxRateID,
                        TaxAmount = serviceReceiveItem.UnitPrice * serviceReceiveItem.Quantity * tax.CalculatedRate,
                        Total = serviceReceiveItem.UnitPrice * serviceReceiveItem.Quantity + (serviceReceiveItem.UnitPrice * serviceReceiveItem.Quantity * tax.CalculatedRate),
                     
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                    };
                    _db.ServiceReceiveItems.Add(entity);
                    _db.SaveChanges();
                    serviceReceiveItem.ServiceReceiveItemId = entity.ServiceReceiveItemId;

                    var record = _db.GoodReceives.Find(id);
                    if (record != null)
                        _distributionHelper.UpdateGoodReceiveDistribution(record);
                }


            }
            return Json(new[] { serviceReceiveItem }.ToDataSourceResult(request, ModelState));
        }



        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceReceiveItems_Update([DataSourceRequest]DataSourceRequest request, ServiceReceiveItem serviceReceiveItem, int id)
        {
            var tax = _db.GLTaxRates.Find(serviceReceiveItem.GLTaxRateID);
            var exist = _db.ServiceReceiveItems.Where(x => x.ServiceReceiveItemId != serviceReceiveItem.ServiceReceiveItemId)
                       .Where(x => x.ServiceReceiveId == serviceReceiveItem.ServiceReceiveId)
                       .Where(x => x.ServiceItemCode == serviceReceiveItem.ServiceItemCode)
                       .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + serviceReceiveItem.ServiceItemCode + " already exists");
            }
            if (serviceReceiveItem.UnitPrice == 0)
            {
                ModelState.AddModelError("zero", "Item " + serviceReceiveItem.ServiceItemCode + " price can't be zero");
            }

            if (ModelState.IsValid)
            {
                var entity = new ServiceReceiveItem
                {
                    ServiceReceiveId = serviceReceiveItem.ServiceReceiveId,
                    ServiceReceiveItemId = serviceReceiveItem.ServiceReceiveItemId,
                    ServiceCategoryCode = serviceReceiveItem.ServiceCategoryCode,
                    ServiceCategoryName = serviceReceiveItem.ServiceCategoryName,
                    ServiceItemCode = serviceReceiveItem.ServiceItemCode,
                    ServiceItemName = serviceReceiveItem.ServiceItemName,
                    Quantity = serviceReceiveItem.Quantity,
                    UnitPrice = serviceReceiveItem.UnitPrice,
                    SubTotal = serviceReceiveItem.UnitPrice * serviceReceiveItem.Quantity,
                    GLTaxRateID = serviceReceiveItem.GLTaxRateID
                };
                if (tax != null)
                    entity.TaxAmount = serviceReceiveItem.UnitPrice * serviceReceiveItem.Quantity * tax.CalculatedRate;
                entity.Total = entity.SubTotal + entity.TaxAmount;
                entity.ModifiedBy = User.Identity.Name;
                entity.LastModified = DateTime.Now;

                _db.ServiceReceiveItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                var record = _db.GoodReceives.Find(id);
                if (record != null)
                    _distributionHelper.UpdateGoodReceiveDistribution(record);

            }

            return Json(new[] { serviceReceiveItem }.ToDataSourceResult(request, ModelState));
        }



        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceReceiveItems_Destroy([DataSourceRequest]DataSourceRequest request, ServiceReceiveItem serviceReceiveItem)
        {
            ModelState.Remove("ServiceReceiveId");
            if (ModelState.IsValid)
            {
                var entity = new ServiceReceiveItem
                {
                    ServiceReceiveItemId = serviceReceiveItem.ServiceReceiveItemId,
                    ServiceCategoryCode = serviceReceiveItem.ServiceCategoryCode,
                    ServiceCategoryName = serviceReceiveItem.ServiceCategoryName,
                    ServiceItemCode = serviceReceiveItem.ServiceItemCode,
                    ServiceItemName = serviceReceiveItem.ServiceItemName,
                    Quantity = serviceReceiveItem.Quantity,
                    UnitPrice = serviceReceiveItem.UnitPrice,
                    SubTotal = serviceReceiveItem.SubTotal,
                    GLTaxRateID = serviceReceiveItem.GLTaxRateID,
                    TaxAmount = serviceReceiveItem.TaxAmount,
                    Total = serviceReceiveItem.Total,
                };

                _db.ServiceReceiveItems.Attach(entity);
                _db.ServiceReceiveItems.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted good receive item " + serviceReceiveItem.ServiceReceiveId + " " + serviceReceiveItem.ServiceItemName);
                #endregion
                var record = _db.GoodReceives.Find(serviceReceiveItem.ServiceReceiveId);
                if (record != null)
                    _distributionHelper.UpdateGoodReceiveDistribution(record);
            }

            return Json(new[] { serviceReceiveItem }.ToDataSourceResult(request, ModelState));
        }



        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
