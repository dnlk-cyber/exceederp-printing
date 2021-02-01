using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.MachineTransferReceive
{
    public class AccessoryReceiveController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult AccessoryReceives_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AccessoryReceive> accessoryReceives = _db.AccessoryReceives.Where(x=>x.MachineryAndVehicleTransferReceiveId==id);
            DataSourceResult result = accessoryReceives.ToDataSourceResult(request, accessoryReceive => new {
                MachineryAndVehicleTransferReceiveId = accessoryReceive.MachineryAndVehicleTransferReceiveId,
                AccessoryReceiveId = accessoryReceive.AccessoryReceiveId,
                Description = accessoryReceive.Description,
                Quantity = accessoryReceive.Quantity,
                SerialNo = accessoryReceive.SerialNo,
                DateCreated = accessoryReceive.DateCreated,
                LastModified = accessoryReceive.LastModified,
                CreatedBy = accessoryReceive.CreatedBy,
                ModifiedBy = accessoryReceive.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccessoryReceives_Create(int id,[DataSourceRequest]DataSourceRequest request, AccessoryReceive accessoryReceive)
        {
            if (ModelState.IsValid)
            {
                var entity = new AccessoryReceive
                {
                    MachineryAndVehicleTransferReceiveId = id,
                    Description = accessoryReceive.Description,
                    Quantity = accessoryReceive.Quantity,
                    SerialNo = accessoryReceive.SerialNo,
                    DateCreated = accessoryReceive.DateCreated,
                    LastModified = accessoryReceive.LastModified,
                    CreatedBy = accessoryReceive.CreatedBy,
                    ModifiedBy = accessoryReceive.ModifiedBy
                };

                _db.AccessoryReceives.Add(entity);
                _db.SaveChanges();
                accessoryReceive.AccessoryReceiveId = entity.AccessoryReceiveId;
            }

            return Json(new[] { accessoryReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccessoryReceives_Update([DataSourceRequest]DataSourceRequest request, AccessoryReceive accessoryReceive)
        {
            if (ModelState.IsValid)
            {
                var entity = new AccessoryReceive
                {
                    MachineryAndVehicleTransferReceiveId = accessoryReceive.MachineryAndVehicleTransferReceiveId,
                    AccessoryReceiveId = accessoryReceive.AccessoryReceiveId,
                    Description = accessoryReceive.Description,
                    Quantity = accessoryReceive.Quantity,
                    SerialNo = accessoryReceive.SerialNo,
                    DateCreated = accessoryReceive.DateCreated,
                    LastModified = accessoryReceive.LastModified,
                    CreatedBy = accessoryReceive.CreatedBy,
                    ModifiedBy = accessoryReceive.ModifiedBy
                };

                _db.AccessoryReceives.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { accessoryReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AccessoryReceives_Destroy([DataSourceRequest]DataSourceRequest request, AccessoryReceive accessoryReceive)
        {
            if (ModelState.IsValid)
            {
                var entity = new AccessoryReceive
                {
                    MachineryAndVehicleTransferReceiveId = accessoryReceive.MachineryAndVehicleTransferReceiveId,
                    AccessoryReceiveId = accessoryReceive.AccessoryReceiveId,
                    Description = accessoryReceive.Description,
                    Quantity = accessoryReceive.Quantity,
                    SerialNo = accessoryReceive.SerialNo,
                    DateCreated = accessoryReceive.DateCreated,
                    LastModified = accessoryReceive.LastModified,
                    CreatedBy = accessoryReceive.CreatedBy,
                    ModifiedBy = accessoryReceive.ModifiedBy
                };

                _db.AccessoryReceives.Attach(entity);
                _db.AccessoryReceives.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { accessoryReceive }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
