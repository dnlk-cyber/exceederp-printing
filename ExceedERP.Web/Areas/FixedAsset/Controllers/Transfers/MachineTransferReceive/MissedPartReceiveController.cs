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
    public class MissedPartReceiveController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser, FixedAssetConstants.FixedAssetApprover)]
        public ActionResult MissedPartReceives_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MissedPartReceive> missedPartReceives = _db.MissedPartReceives.Where(x=>x.MachineryAndVehicleTransferReceiveId==id);
            DataSourceResult result = missedPartReceives.ToDataSourceResult(request, missedPartReceive => new {
                MachineryAndVehicleTransferReceiveId = missedPartReceive.MachineryAndVehicleTransferReceiveId,
                MissedPartReceiveId = missedPartReceive.MissedPartReceiveId,
                MissingPart = missedPartReceive.MissingPart,
                DateCreated = missedPartReceive.DateCreated,
                LastModified = missedPartReceive.LastModified,
                CreatedBy = missedPartReceive.CreatedBy,
                ModifiedBy = missedPartReceive.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MissedPartReceives_Create(int id,[DataSourceRequest]DataSourceRequest request, MissedPartReceive missedPartReceive)
        {
            if (ModelState.IsValid)
            {
                var entity = new MissedPartReceive
                {
                    MachineryAndVehicleTransferReceiveId = id,
                    MissingPart = missedPartReceive.MissingPart,
                    DateCreated = missedPartReceive.DateCreated,
                    LastModified = missedPartReceive.LastModified,
                    CreatedBy = missedPartReceive.CreatedBy,
                    ModifiedBy = missedPartReceive.ModifiedBy
                };

                _db.MissedPartReceives.Add(entity);
                _db.SaveChanges();
                missedPartReceive.MissedPartReceiveId = entity.MissedPartReceiveId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "MissedPartReceive", "Success", User.Identity.Name, $"created MissedPartReceive {entity.MissedPartReceiveId} MissedPart {entity.MissedPartReceiveId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { missedPartReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MissedPartReceives_Update([DataSourceRequest]DataSourceRequest request, MissedPartReceive missedPartReceive)
        {
            if (ModelState.IsValid)
            {
                var entity = new MissedPartReceive
                {
                    MachineryAndVehicleTransferReceiveId = missedPartReceive.MachineryAndVehicleTransferReceiveId,
                    MissedPartReceiveId = missedPartReceive.MissedPartReceiveId,
                    MissingPart = missedPartReceive.MissingPart,
                    DateCreated = missedPartReceive.DateCreated,
                    LastModified = missedPartReceive.LastModified,
                    CreatedBy = missedPartReceive.CreatedBy,
                    ModifiedBy = missedPartReceive.ModifiedBy
                };

                _db.MissedPartReceives.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "MissedPartReceive", "Success", User.Identity.Name, $"Update MissedPartReceive {entity.MissedPartReceiveId} MissedPart {entity.MissedPartReceiveId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { missedPartReceive }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FixedAssetConstants.FixedAssetUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MissedPartReceives_Destroy([DataSourceRequest]DataSourceRequest request, MissedPartReceive missedPartReceive)
        {
            if (ModelState.IsValid)
            {
                var entity = new MissedPartReceive
                {
                    MachineryAndVehicleTransferReceiveId = missedPartReceive.MachineryAndVehicleTransferReceiveId,
                    MissedPartReceiveId = missedPartReceive.MissedPartReceiveId,
                    MissingPart = missedPartReceive.MissingPart,
                    DateCreated = missedPartReceive.DateCreated,
                    LastModified = missedPartReceive.LastModified,
                    CreatedBy = missedPartReceive.CreatedBy,
                    ModifiedBy = missedPartReceive.ModifiedBy
                };

                _db.MissedPartReceives.Attach(entity);
                _db.MissedPartReceives.Remove(entity);
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Delete MissedPartReceive {missedPartReceive.MissedPartReceiveId} MissedPart {missedPartReceive.MissedPartReceiveId}", Modules.FixedAsset);
                #endregion
            }

            return Json(new[] { missedPartReceive }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
