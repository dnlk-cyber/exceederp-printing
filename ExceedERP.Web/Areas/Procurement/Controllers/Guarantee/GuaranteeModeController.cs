using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Guarantee;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Guarantee
{
    [AuthorizeRoles(ProcurementRoles.ProcurementAdmin)]
    public class GuaranteeModeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GuaranteeModes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GuaranteeMode> guaranteemodes = db.GuaranteeModes;
            DataSourceResult result = guaranteemodes.ToDataSourceResult(request, guaranteeMode => new {
                GuaranteeModeId = guaranteeMode.GuaranteeModeId,
                Mode = guaranteeMode.Mode,
                Remark = guaranteeMode.Remark,
                DateCreated = guaranteeMode.DateCreated,
                LastModified = guaranteeMode.LastModified,
                CreatedBy = guaranteeMode.CreatedBy,
                ModifiedBy = guaranteeMode.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeModes_Create([DataSourceRequest]DataSourceRequest request, GuaranteeMode guaranteeMode)
        {
            #region validation

            var exists = db.GuaranteeModes.Where(x => x.Mode == guaranteeMode.Mode).ToList();
            if (exists.Any())
            {
                ModelState.AddModelError("exists","The guarantee mode "+guaranteeMode.Mode+" already exists in the system.");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeMode
                {
                    Mode = guaranteeMode.Mode,
                    Remark = guaranteeMode.Remark,
                    DateCreated = guaranteeMode.DateCreated,
                    LastModified = guaranteeMode.LastModified,
                    CreatedBy = guaranteeMode.CreatedBy,
                    ModifiedBy = guaranteeMode.ModifiedBy
                };

                db.GuaranteeModes.Add(entity);
                db.SaveChanges();
                guaranteeMode.GuaranteeModeId = entity.GuaranteeModeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Guarantee mode setup", "Success", User.Identity.Name, $"created guarantee mode {entity.Mode}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { guaranteeMode }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeModes_Update([DataSourceRequest]DataSourceRequest request, GuaranteeMode guaranteeMode)
        {
            #region validation

            var exists = db.GuaranteeModes.Where(x=>x.GuaranteeModeId!=guaranteeMode.GuaranteeModeId).Where(x => x.Mode == guaranteeMode.Mode).ToList();
            if (exists.Any())
            {
                ModelState.AddModelError("exists", "The guarantee mode " + guaranteeMode.Mode + " already exists in the system.");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeMode
                {
                    GuaranteeModeId = guaranteeMode.GuaranteeModeId,
                    Mode = guaranteeMode.Mode,
                    Remark = guaranteeMode.Remark,
                    DateCreated = guaranteeMode.DateCreated,
                    LastModified = guaranteeMode.LastModified,
                    CreatedBy = guaranteeMode.CreatedBy,
                    ModifiedBy = guaranteeMode.ModifiedBy
                };

                db.GuaranteeModes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Guarantee mode setup", "Success", User.Identity.Name, $"updated guarantee mode {entity.Mode}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { guaranteeMode }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GuaranteeModes_Destroy([DataSourceRequest]DataSourceRequest request, GuaranteeMode guaranteeMode)
        {
            if (ModelState.IsValid)
            {
                var entity = new GuaranteeMode
                {
                    GuaranteeModeId = guaranteeMode.GuaranteeModeId,
                    Mode = guaranteeMode.Mode,
                    Remark = guaranteeMode.Remark,
                    DateCreated = guaranteeMode.DateCreated,
                    LastModified = guaranteeMode.LastModified,
                    CreatedBy = guaranteeMode.CreatedBy,
                    ModifiedBy = guaranteeMode.ModifiedBy
                };

                db.GuaranteeModes.Attach(entity);
                db.GuaranteeModes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { guaranteeMode }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
