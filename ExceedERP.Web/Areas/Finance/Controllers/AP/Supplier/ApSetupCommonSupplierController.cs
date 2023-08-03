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
    public class ApSetupCommonSupplierController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FinanceConstants.AccountPayableAdmin)]
        public ActionResult Index()
        {
            ViewBag.Count = _db.ApSetupCommonSuppliers.Count();
            return View();
        }

        public ActionResult ApSetupCommonSuppliers_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApSetupCommonSupplier> apsetupcommonsuppliers = _db.ApSetupCommonSuppliers;
            DataSourceResult result = apsetupcommonsuppliers.ToDataSourceResult(request, apSetupCommonSupplier => new {
                ApSetupCommonSupplierId = apSetupCommonSupplier.ApSetupCommonSupplierId,
                Description = apSetupCommonSupplier.Description,
                AccountCode = apSetupCommonSupplier.AccountCode,
                DateCreated = apSetupCommonSupplier.DateCreated,
                LastModified = apSetupCommonSupplier.LastModified,
                CreatedBy = apSetupCommonSupplier.CreatedBy,
                ModifiedBy = apSetupCommonSupplier.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApSetupCommonSuppliers_Create([DataSourceRequest]DataSourceRequest request, ApSetupCommonSupplier apSetupCommonSupplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApSetupCommonSupplier
                {
                    Description = apSetupCommonSupplier.Description,
                    AccountCode = apSetupCommonSupplier.AccountCode,
                    DateCreated = apSetupCommonSupplier.DateCreated,
                    LastModified = apSetupCommonSupplier.LastModified,
                    CreatedBy = apSetupCommonSupplier.CreatedBy,
                    ModifiedBy = apSetupCommonSupplier.ModifiedBy
                };

                _db.ApSetupCommonSuppliers.Add(entity);
                _db.SaveChanges();
                apSetupCommonSupplier.ApSetupCommonSupplierId = entity.ApSetupCommonSupplierId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Common supplier setup", "Success", User.Identity.Name, $"created  {apSetupCommonSupplier.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { apSetupCommonSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApSetupCommonSuppliers_Update([DataSourceRequest]DataSourceRequest request, ApSetupCommonSupplier apSetupCommonSupplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApSetupCommonSupplier
                {
                    ApSetupCommonSupplierId = apSetupCommonSupplier.ApSetupCommonSupplierId,
                    Description = apSetupCommonSupplier.Description,
                    AccountCode = apSetupCommonSupplier.AccountCode,
                    DateCreated = apSetupCommonSupplier.DateCreated,
                    LastModified = apSetupCommonSupplier.LastModified,
                    CreatedBy = apSetupCommonSupplier.CreatedBy,
                    ModifiedBy = apSetupCommonSupplier.ModifiedBy
                };

                _db.ApSetupCommonSuppliers.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Common supplier setup", "Success", User.Identity.Name, $"updated  {apSetupCommonSupplier.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { apSetupCommonSupplier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApSetupCommonSuppliers_Destroy([DataSourceRequest]DataSourceRequest request, ApSetupCommonSupplier apSetupCommonSupplier)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApSetupCommonSupplier
                {
                    ApSetupCommonSupplierId = apSetupCommonSupplier.ApSetupCommonSupplierId,
                    Description = apSetupCommonSupplier.Description,
                    AccountCode = apSetupCommonSupplier.AccountCode,
                    DateCreated = apSetupCommonSupplier.DateCreated,
                    LastModified = apSetupCommonSupplier.LastModified,
                    CreatedBy = apSetupCommonSupplier.CreatedBy,
                    ModifiedBy = apSetupCommonSupplier.ModifiedBy
                };

                _db.ApSetupCommonSuppliers.Attach(entity);
                _db.ApSetupCommonSuppliers.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted common supplier " + apSetupCommonSupplier.ApSetupCommonSupplierId + " " + apSetupCommonSupplier.AccountCode, Modules.Finance);
                #endregion
            }

            return Json(new[] { apSetupCommonSupplier }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
