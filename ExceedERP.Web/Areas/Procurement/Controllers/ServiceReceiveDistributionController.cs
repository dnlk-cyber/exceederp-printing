using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers
{

    public class ServiceReceiveDistributionController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private AccountHelper accountHelper = new AccountHelper();

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentApprover, ProcurementRoles.ProcurementUserDepartmentUser, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(ProcurementRoles.ProcurementUserDepartmentApprover, ProcurementRoles.ProcurementUserDepartmentUser, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult ServiceReceiveDistributions_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ServiceReceiveDistribution> serviceReceiveDistributions = _db.ServiceReceiveDistributions.Where(gr => gr.ServiceReceiveId == id);
            DataSourceResult result = serviceReceiveDistributions.ToDataSourceResult(request, serviceReceiveDistribution => new
            {
                ServiceReceiveDistributionId = serviceReceiveDistribution.ServiceReceiveDistributionId,
                AccountCode = serviceReceiveDistribution.AccountCode,
                AccountDesc = serviceReceiveDistribution.AccountDesc,
                Debit = serviceReceiveDistribution.Debit,
                Credit = serviceReceiveDistribution.Credit
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceReceiveDistributions_Create([DataSourceRequest]DataSourceRequest request, ServiceReceiveDistribution serviceReceiveDistribution, int id)
        {
            #region validation

            var record = _db.GoodReceives.Find(id);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            var exists = _db.ServiceReceiveDistributions.Where(x => x.AccountCode == serviceReceiveDistribution.AccountCode);
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The account code selected already exists.");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ServiceReceiveDistribution
                {
                    ServiceReceiveId = id,
                    AccountCode = serviceReceiveDistribution.AccountCode,
                    AccountDesc = accountHelper.GetAccountDesc(serviceReceiveDistribution.AccountCode),
                    Debit = serviceReceiveDistribution.Debit,
                    Credit = serviceReceiveDistribution.Credit
                };

                _db.ServiceReceiveDistributions.Add(entity);
                _db.SaveChanges();
                serviceReceiveDistribution.ServiceReceiveDistributionId = entity.ServiceReceiveDistributionId;
            }

            return Json(new[] { serviceReceiveDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult ServiceReceiveDistributions_Update([DataSourceRequest]DataSourceRequest request, ServiceReceiveDistribution serviceReceiveDistribution)
        {
            #region validation

            var record = _db.GoodReceives.Find(serviceReceiveDistribution.ServiceReceiveId);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            var exists = _db.ServiceReceiveDistributions.Where(x => x.ServiceReceiveDistributionId != serviceReceiveDistribution.ServiceReceiveDistributionId).Where(x => x.AccountCode == serviceReceiveDistribution.AccountCode);
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The account code selected already exists.");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ServiceReceiveDistribution
                {
                    ServiceReceiveId = serviceReceiveDistribution.ServiceReceiveId,
                    ServiceReceiveDistributionId = serviceReceiveDistribution.ServiceReceiveDistributionId,
                    AccountCode = serviceReceiveDistribution.AccountCode,
                    AccountDesc = accountHelper.GetAccountDesc(serviceReceiveDistribution.AccountCode),
                    Debit = serviceReceiveDistribution.Debit,
                    Credit = serviceReceiveDistribution.Credit,

                };

                _db.ServiceReceiveDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { serviceReceiveDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult ServiceReceiveDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ServiceReceiveDistribution serviceReceiveDistribution)
        {
            var record = _db.GoodReceives.Find(serviceReceiveDistribution.ServiceReceiveId);
            if (record != null && record.IsOnlineTransferred)
            {
                ModelState.AddModelError("exist", "The record is already transfered,hence cannot be manipulated.");
            }
            if (ModelState.IsValid)
            {
                var entity = new ServiceReceiveDistribution
                {
                    ServiceReceiveId = serviceReceiveDistribution.ServiceReceiveId,
                    ServiceReceiveDistributionId = serviceReceiveDistribution.ServiceReceiveDistributionId,
                    AccountCode = serviceReceiveDistribution.AccountCode,
                    AccountDesc = serviceReceiveDistribution.AccountDesc,
                    Debit = serviceReceiveDistribution.Debit,
                    Credit = serviceReceiveDistribution.Credit
                };

                _db.ServiceReceiveDistributions.Attach(entity);
                _db.ServiceReceiveDistributions.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted good receive distribution " + serviceReceiveDistribution.ServiceReceiveId + " " + serviceReceiveDistribution.AccountCode);
                #endregion
            }

            return Json(new[] { serviceReceiveDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
