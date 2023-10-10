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

namespace ExceedERP.Web.Areas.FixedAsset.Controllers.Transfers.MachineTransferSend
{
    public class MissedPartController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult MissedParts_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MissedPart> missedparts = db.MissedParts.Where(x=>x.MachineryAndVehicleTransferId==id);
            DataSourceResult result = missedparts.ToDataSourceResult(request, missedPart => new {
                MachineryAndVehicleTransferId = missedPart.MachineryAndVehicleTransferId,
                MissedPartId = missedPart.MissedPartId,
                MissingPart = missedPart.MissingPart,
                DateCreated = missedPart.DateCreated,
                LastModified = missedPart.LastModified,
                CreatedBy = missedPart.CreatedBy,
                ModifiedBy = missedPart.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MissedParts_Create(int id,[DataSourceRequest]DataSourceRequest request, MissedPart missedPart)
        {
            if (ModelState.IsValid)
            {
                var entity = new MissedPart
                {
                    MachineryAndVehicleTransferId = id,
                    MissingPart = missedPart.MissingPart,
                    DateCreated = missedPart.DateCreated,
                    LastModified = missedPart.LastModified,
                    CreatedBy = missedPart.CreatedBy,
                    ModifiedBy = missedPart.ModifiedBy
                };

                db.MissedParts.Add(entity);
                db.SaveChanges();
                missedPart.MissedPartId = entity.MissedPartId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "MissedPart", "Success", User.Identity.Name, $"created MissedPart  {entity.MissedPartId} MachineryAndVehicleTransferId {entity.MachineryAndVehicleTransferId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { missedPart }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MissedParts_Update([DataSourceRequest]DataSourceRequest request, MissedPart missedPart)
        {
            if (ModelState.IsValid)
            {
                var entity = new MissedPart
                {
                    MachineryAndVehicleTransferId = missedPart.MachineryAndVehicleTransferId,
                    MissedPartId = missedPart.MissedPartId,
                    MissingPart = missedPart.MissingPart,
                    DateCreated = missedPart.DateCreated,
                    LastModified = missedPart.LastModified,
                    CreatedBy = missedPart.CreatedBy,
                    ModifiedBy = missedPart.ModifiedBy
                };

                db.MissedParts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "MissedPart", "Success", User.Identity.Name, $"update MissedPart  {entity.MissedPartId} MachineryAndVehicleTransferId {entity.MachineryAndVehicleTransferId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { missedPart }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MissedParts_Destroy([DataSourceRequest]DataSourceRequest request, MissedPart missedPart)
        {
            if (ModelState.IsValid)
            {
                var entity = new MissedPart
                {
                    MachineryAndVehicleTransferId = missedPart.MachineryAndVehicleTransferId,
                    MissedPartId = missedPart.MissedPartId,
                    MissingPart = missedPart.MissingPart,
                    DateCreated = missedPart.DateCreated,
                    LastModified = missedPart.LastModified,
                    CreatedBy = missedPart.CreatedBy,
                    ModifiedBy = missedPart.ModifiedBy
                };

                db.MissedParts.Attach(entity);
                db.MissedParts.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete MissedPart  {missedPart.MissedPartId} MachineryAndVehicleTransferId {missedPart.MachineryAndVehicleTransferId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { missedPart }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
