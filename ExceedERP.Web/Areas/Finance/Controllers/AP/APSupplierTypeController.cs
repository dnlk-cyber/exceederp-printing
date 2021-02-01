using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
   [AuthorizeRoles(FinanceConstants.AccountPayableAdmin)]
    public class ApSupplierTypeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
       
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult APSupplierTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<APSupplierType> apsuppliertypes = _db.APSupplierTypes;
            DataSourceResult result = apsuppliertypes.ToDataSourceResult(request, aPSupplierType => new {
                APSupplierTypeID = aPSupplierType.APSupplierTypeID,
                Supplier = aPSupplierType.Supplier,
                Description = aPSupplierType.Description,
                DateCreated = aPSupplierType.DateCreated,
                LastModified = aPSupplierType.LastModified,
                CreatedBy = aPSupplierType.CreatedBy,
                ModifiedBy = aPSupplierType.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APSupplierTypes_Create([DataSourceRequest]DataSourceRequest request, APSupplierType aPSupplierType)
        {
            if (ModelState.IsValid)
            {
                var entity = new APSupplierType
                {
                    Supplier = aPSupplierType.Supplier,
                    Description = aPSupplierType.Description,
                    DateCreated = DateTime.Now,
                    LastModified = aPSupplierType.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aPSupplierType.ModifiedBy
                };

                _db.APSupplierTypes.Add(entity);
                _db.SaveChanges();
                aPSupplierType.APSupplierTypeID = entity.APSupplierTypeID;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Supplier type", "Success", User.Identity.Name, $"created  {aPSupplierType.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aPSupplierType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APSupplierTypes_Update([DataSourceRequest]DataSourceRequest request, APSupplierType aPSupplierType)
        {
            if (ModelState.IsValid)
            {
                var entity = new APSupplierType
                {
                    APSupplierTypeID = aPSupplierType.APSupplierTypeID,
                    Supplier = aPSupplierType.Supplier,
                    Description = aPSupplierType.Description,
                    DateCreated = aPSupplierType.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aPSupplierType.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.APSupplierTypes.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Supplier type", "Success", User.Identity.Name, $"updated  {aPSupplierType.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aPSupplierType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APSupplierTypes_Destroy([DataSourceRequest]DataSourceRequest request, APSupplierType aPSupplierType)
        {
            if (ModelState.IsValid)
            {
                var entity = new APSupplierType
                {
                    APSupplierTypeID = aPSupplierType.APSupplierTypeID,
                    Supplier = aPSupplierType.Supplier,
                    Description = aPSupplierType.Description,
                    DateCreated = aPSupplierType.DateCreated,
                    LastModified = aPSupplierType.LastModified,
                    CreatedBy = aPSupplierType.CreatedBy,
                    ModifiedBy = aPSupplierType.ModifiedBy
                };

                _db.APSupplierTypes.Attach(entity);
                _db.APSupplierTypes.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted AP supplier type" + aPSupplierType.APSupplierTypeID + " " + aPSupplierType.Description, Modules.Finance);
                #endregion
            }

            return Json(new[] { aPSupplierType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
