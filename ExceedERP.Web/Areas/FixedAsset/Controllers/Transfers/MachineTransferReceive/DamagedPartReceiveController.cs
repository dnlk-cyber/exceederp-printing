using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.MachineTransferReceive
{
    public class DamagedPartReceiveController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult DamagedPartReceives_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DamagedPartReceive> damagedPartReceives = _db.DamagedPartReceives.Where(x=>x.MachineryAndVehicleTransferReceiveId==id);
            DataSourceResult result = damagedPartReceives.ToDataSourceResult(request, damagedPartReceive => new {
                MachineryAndVehicleTransferReceiveId = damagedPartReceive.MachineryAndVehicleTransferReceiveId,
                DamagedPartReceiveId = damagedPartReceive.DamagedPartReceiveId,
                DamagePart = damagedPartReceive.DamagePart,
                DateCreated = damagedPartReceive.DateCreated,
                LastModified = damagedPartReceive.LastModified,
                CreatedBy = damagedPartReceive.CreatedBy,
                ModifiedBy = damagedPartReceive.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DamagedPartReceives_Create(int id,[DataSourceRequest]DataSourceRequest request, DamagedPartReceive damagedPartReceive)
        {
            if (ModelState.IsValid)
            {
                var entity = new DamagedPartReceive
                {
                    MachineryAndVehicleTransferReceiveId = id,
                    DamagePart = damagedPartReceive.DamagePart,
                    DateCreated = damagedPartReceive.DateCreated,
                    LastModified = damagedPartReceive.LastModified,
                    CreatedBy = damagedPartReceive.CreatedBy,
                    ModifiedBy = damagedPartReceive.ModifiedBy
                };

                _db.DamagedPartReceives.Add(entity);
                _db.SaveChanges();
                damagedPartReceive.DamagedPartReceiveId = entity.DamagedPartReceiveId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "DamagedPartReceive", "Success", User.Identity.Name, $"created DamagedPartReceive {entity.DamagedPartReceiveId} DamagePart {entity.DamagePart}", Modules.FixedAsset);
                #endregion

            }

            return Json(new[] { damagedPartReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DamagedPartReceives_Update([DataSourceRequest]DataSourceRequest request, DamagedPartReceive damagedPartReceive)
        {
            if (ModelState.IsValid)
            {
                var entity = new DamagedPartReceive
                {
                    MachineryAndVehicleTransferReceiveId = damagedPartReceive.MachineryAndVehicleTransferReceiveId,
                    DamagedPartReceiveId = damagedPartReceive.DamagedPartReceiveId,
                    DamagePart = damagedPartReceive.DamagePart,
                    DateCreated = damagedPartReceive.DateCreated,
                    LastModified = damagedPartReceive.LastModified,
                    CreatedBy = damagedPartReceive.CreatedBy,
                    ModifiedBy = damagedPartReceive.ModifiedBy
                };

                _db.DamagedPartReceives.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "DamagedPartReceive", "Success", User.Identity.Name, $"update DamagedPartReceive {entity.DamagedPartReceiveId} DamagePart {entity.DamagePart}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { damagedPartReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DamagedPartReceives_Destroy([DataSourceRequest]DataSourceRequest request, DamagedPartReceive damagedPartReceive)
        {
            if (ModelState.IsValid)
            {
                var entity = new DamagedPartReceive
                {
                    MachineryAndVehicleTransferReceiveId = damagedPartReceive.MachineryAndVehicleTransferReceiveId,
                    DamagedPartReceiveId = damagedPartReceive.DamagedPartReceiveId,
                    DamagePart = damagedPartReceive.DamagePart,
                    DateCreated = damagedPartReceive.DateCreated,
                    LastModified = damagedPartReceive.LastModified,
                    CreatedBy = damagedPartReceive.CreatedBy,
                    ModifiedBy = damagedPartReceive.ModifiedBy
                };

                _db.DamagedPartReceives.Attach(entity);
                _db.DamagedPartReceives.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete DamagedPartReceive {damagedPartReceive.DamagedPartReceiveId} DamagePart {damagedPartReceive.DamagePart}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { damagedPartReceive }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
