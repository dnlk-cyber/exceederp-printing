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
    public class DamagedPartController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult DamagedParts_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DamagedPart> damagedparts = db.DamagedParts.Where(x=>x.MachineryAndVehicleTransferId==id);
            DataSourceResult result = damagedparts.ToDataSourceResult(request, damagedPart => new {
                MachineryAndVehicleTransferId = damagedPart.MachineryAndVehicleTransferId,
                DamagedPartId = damagedPart.DamagedPartId,
                DamagePart = damagedPart.DamagePart,
                DateCreated = damagedPart.DateCreated,
                LastModified = damagedPart.LastModified,
                CreatedBy = damagedPart.CreatedBy,
                ModifiedBy = damagedPart.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DamagedParts_Create(int id,[DataSourceRequest]DataSourceRequest request, DamagedPart damagedPart)
        {
            if (ModelState.IsValid)
            {
                var entity = new DamagedPart
                {
                    MachineryAndVehicleTransferId = id,
                    DamagePart = damagedPart.DamagePart,
                    DateCreated = damagedPart.DateCreated,
                    LastModified = damagedPart.LastModified,
                    CreatedBy = damagedPart.CreatedBy,
                    ModifiedBy = damagedPart.ModifiedBy
                };

                db.DamagedParts.Add(entity);
                db.SaveChanges();
                damagedPart.DamagedPartId = entity.DamagedPartId;
            }

            return Json(new[] { damagedPart }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DamagedParts_Update([DataSourceRequest]DataSourceRequest request, DamagedPart damagedPart)
        {
            if (ModelState.IsValid)
            {
                var entity = new DamagedPart
                {
                    MachineryAndVehicleTransferId = damagedPart.MachineryAndVehicleTransferId,
                    DamagedPartId = damagedPart.DamagedPartId,
                    DamagePart = damagedPart.DamagePart,
                    DateCreated = damagedPart.DateCreated,
                    LastModified = damagedPart.LastModified,
                    CreatedBy = damagedPart.CreatedBy,
                    ModifiedBy = damagedPart.ModifiedBy
                };

                db.DamagedParts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { damagedPart }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DamagedParts_Destroy([DataSourceRequest]DataSourceRequest request, DamagedPart damagedPart)
        {
            if (ModelState.IsValid)
            {
                var entity = new DamagedPart
                {
                    MachineryAndVehicleTransferId = damagedPart.MachineryAndVehicleTransferId,
                    DamagedPartId = damagedPart.DamagedPartId,
                    DamagePart = damagedPart.DamagePart,
                    DateCreated = damagedPart.DateCreated,
                    LastModified = damagedPart.LastModified,
                    CreatedBy = damagedPart.CreatedBy,
                    ModifiedBy = damagedPart.ModifiedBy
                };

                db.DamagedParts.Attach(entity);
                db.DamagedParts.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { damagedPart }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
