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

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
    public class UomController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UoMs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<UoM> uoms = db.UoMs;
            DataSourceResult result = uoms.ToDataSourceResult(request, uoM => new
            {
                UoMName = uoM.UoMName,
                Description = uoM.Description,
                Default = uoM.Default,
                DateCreated = uoM.DateCreated,
                CreatedBy = uoM.CreatedBy,
                LastModified = uoM.LastModified,
                ModifiedBy = uoM.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UoMs_Create([DataSourceRequest]DataSourceRequest request, UoM uoM)
        {
            
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var exists =
                    db.UoMs.Where(x => x.UoMName == uoM.UoMName).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("UoM",
                        uoM.UoMName + "  already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new UoM
                    {
                        UoMName = uoM.UoMName,
                        Description = uoM.Description,
                        Default = uoM.Default,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now
                    };

                    db.UoMs.Add(entity);
                    db.SaveChanges();
                    uoM.UoMName = entity.UoMName;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "unit of measurement", "Success", User.Identity.Name, $"created UOM {entity.UoMName}  ", Modules.Inventory);
                    #endregion
                }
            }
          
            return Json(new[] { uoM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UoMs_Update([DataSourceRequest]DataSourceRequest request, UoM uoM)
        {
            
            if (ModelState.IsValid)
            {
               
                    var entity = new UoM
                    {
                        UoMName = uoM.UoMName,
                        Description = uoM.Description,
                        Default = uoM.Default,
                        CreatedBy = uoM.CreatedBy,
                        DateCreated= uoM.DateCreated,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now
                    };

                    db.UoMs.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "unit of measurement", "Success", User.Identity.Name, $"updated UOM {entity.UoMName}  ", Modules.Inventory);
                #endregion

            }

            return Json(new[] { uoM }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UoMs_Destroy([DataSourceRequest]DataSourceRequest request, UoM uoM)
        {
            if (ModelState.IsValid)
            {
                var entity = new UoM
                {
                    UoMName = uoM.UoMName,

                    Description = uoM.Description
                };

                db.UoMs.Attach(entity);
                db.UoMs.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted UoM " + uoM.UoMName );
                #endregion
            }

            return Json(new[] { uoM }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
