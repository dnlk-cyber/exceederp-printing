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
    public class APsupplierBlacklistController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();


        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
        public ActionResult APsupplierBlacklists_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<APsupplierBlacklist> apsupplierblacklists = _db.APsupplierBlacklists;
            DataSourceResult result = apsupplierblacklists.ToDataSourceResult(request, aPsupplierBlacklist => new
            {
                APsupplierBlacklistID = aPsupplierBlacklist.APsupplierBlacklistID,
                Opertion = aPsupplierBlacklist.Opertion,
                Date = aPsupplierBlacklist.Date,
                Remark = aPsupplierBlacklist.Remark,
                OrgBranchName = aPsupplierBlacklist.OrgBranchName,
                DateCreated = aPsupplierBlacklist.DateCreated,
                LastModified = aPsupplierBlacklist.LastModified,
                CreatedBy = aPsupplierBlacklist.CreatedBy,
                ModifiedBy = aPsupplierBlacklist.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APsupplierBlacklists_Create([DataSourceRequest]DataSourceRequest request, APsupplierBlacklist aPsupplierBlacklist)
        {
            if (ModelState.IsValid)
            {
                var entity = new APsupplierBlacklist
                {
                    Opertion = aPsupplierBlacklist.Opertion,
                    Date = aPsupplierBlacklist.Date,
                    Remark = aPsupplierBlacklist.Remark,
                    OrgBranchName = aPsupplierBlacklist.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = aPsupplierBlacklist.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aPsupplierBlacklist.ModifiedBy
                };

                _db.APsupplierBlacklists.Add(entity);
                _db.SaveChanges();
                aPsupplierBlacklist.APsupplierBlacklistID = entity.APsupplierBlacklistID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "supplier black list", "Success", User.Identity.Name, $"created supplier black list {entity.APSupplierRegistrationID} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aPsupplierBlacklist }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APsupplierBlacklists_Update([DataSourceRequest]DataSourceRequest request, APsupplierBlacklist aPsupplierBlacklist)
        {
            if (ModelState.IsValid)
            {
                var entity = new APsupplierBlacklist
                {
                    APsupplierBlacklistID = aPsupplierBlacklist.APsupplierBlacklistID,
                    Opertion = aPsupplierBlacklist.Opertion,
                    Date = aPsupplierBlacklist.Date,
                    Remark = aPsupplierBlacklist.Remark,
                    OrgBranchName = aPsupplierBlacklist.OrgBranchName,
                    DateCreated = aPsupplierBlacklist.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aPsupplierBlacklist.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.APsupplierBlacklists.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "supplier black list", "Success", User.Identity.Name, $"updated supplier black list {entity.APSupplierRegistrationID} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aPsupplierBlacklist }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APsupplierBlacklists_Destroy([DataSourceRequest]DataSourceRequest request, APsupplierBlacklist aPsupplierBlacklist)
        {
            if (ModelState.IsValid)
            {
                var entity = new APsupplierBlacklist
                {
                    APsupplierBlacklistID = aPsupplierBlacklist.APsupplierBlacklistID,
                    Opertion = aPsupplierBlacklist.Opertion,
                    Date = aPsupplierBlacklist.Date,
                    Remark = aPsupplierBlacklist.Remark,
                    OrgBranchName = aPsupplierBlacklist.OrgBranchName,
                    DateCreated = aPsupplierBlacklist.DateCreated,
                    LastModified = aPsupplierBlacklist.LastModified,
                    CreatedBy = aPsupplierBlacklist.CreatedBy,
                    ModifiedBy = aPsupplierBlacklist.ModifiedBy
                };

                _db.APsupplierBlacklists.Attach(entity);
                _db.APsupplierBlacklists.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted AP supplier black list " + aPsupplierBlacklist.APsupplierBlacklistID + " " + aPsupplierBlacklist.APSupplierRegistration.Account, Modules.Finance);
                #endregion
            }

            return Json(new[] { aPsupplierBlacklist }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment , FinanceConstants.AccountPayableInvoice)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
