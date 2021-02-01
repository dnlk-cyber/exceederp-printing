using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.MachineTransferSend
{
    public class AccessoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Accessories_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Accessory> accessories = db.Accessories.Where(x=>x.MachineryAndVehicleTransferId==id);
            DataSourceResult result = accessories.ToDataSourceResult(request, accessory => new {
                MachineryAndVehicleTransferId = accessory.MachineryAndVehicleTransferId,
                AccessoryId = accessory.AccessoryId,
                Description = accessory.Description,
                Quantity = accessory.Quantity,
                SerialNo = accessory.SerialNo,
                DateCreated = accessory.DateCreated,
                LastModified = accessory.LastModified,
                CreatedBy = accessory.CreatedBy,
                ModifiedBy = accessory.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Accessories_Create(int id,[DataSourceRequest]DataSourceRequest request, Accessory accessory)
        {
            if (ModelState.IsValid)
            {
                var entity = new Accessory
                {
                    MachineryAndVehicleTransferId = id,
                    Description = accessory.Description,
                    Quantity = accessory.Quantity,
                    SerialNo = accessory.SerialNo,
                    DateCreated = accessory.DateCreated,
                    LastModified = accessory.LastModified,
                    CreatedBy = accessory.CreatedBy,
                    ModifiedBy = accessory.ModifiedBy
                };

                db.Accessories.Add(entity);
                db.SaveChanges();
                accessory.AccessoryId = entity.AccessoryId;
            }

            return Json(new[] { accessory }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Accessories_Update([DataSourceRequest]DataSourceRequest request, Accessory accessory)
        {
            if (ModelState.IsValid)
            {
                var entity = new Accessory
                {
                    MachineryAndVehicleTransferId = accessory.MachineryAndVehicleTransferId,
                    AccessoryId = accessory.AccessoryId,
                    Description = accessory.Description,
                    Quantity = accessory.Quantity,
                    SerialNo = accessory.SerialNo,
                    DateCreated = accessory.DateCreated,
                    LastModified = accessory.LastModified,
                    CreatedBy = accessory.CreatedBy,
                    ModifiedBy = accessory.ModifiedBy
                };

                db.Accessories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { accessory }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Accessories_Destroy([DataSourceRequest]DataSourceRequest request, Accessory accessory)
        {
            if (ModelState.IsValid)
            {
                var entity = new Accessory
                {
                    MachineryAndVehicleTransferId = accessory.MachineryAndVehicleTransferId,
                    AccessoryId = accessory.AccessoryId,
                    Description = accessory.Description,
                    Quantity = accessory.Quantity,
                    SerialNo = accessory.SerialNo,
                    DateCreated = accessory.DateCreated,
                    LastModified = accessory.LastModified,
                    CreatedBy = accessory.CreatedBy,
                    ModifiedBy = accessory.ModifiedBy
                };

                db.Accessories.Attach(entity);
                db.Accessories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { accessory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
