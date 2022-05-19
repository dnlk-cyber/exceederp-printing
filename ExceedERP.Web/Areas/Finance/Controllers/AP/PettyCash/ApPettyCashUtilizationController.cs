using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.PettyCash;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.PettyCash
{
    public class ApPettyCashUtilizationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApPettyCashUtilizations_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPettyCashUtilization> appettycashutilizations = _db.ApPettyCashUtilizations.Where(x => x.ApPettyCashId == id);
            DataSourceResult result = appettycashutilizations.ToDataSourceResult(request, apPettyCashUtilization => new {
                ApPettyCashId = apPettyCashUtilization.ApPettyCashId,
                ApPettyCashUtilizationId = apPettyCashUtilization.ApPettyCashUtilizationId,
                Date = apPettyCashUtilization.Date,
                PaidTo = apPettyCashUtilization.PaidTo,
                Purpose = apPettyCashUtilization.Purpose,
                Amount = apPettyCashUtilization.Amount,
                AccountCode = apPettyCashUtilization.AccountCode,
                Reference = apPettyCashUtilization.Reference,
                OrgBranchName = apPettyCashUtilization.OrgBranchName,
                DateCreated = apPettyCashUtilization.DateCreated,
                LastModified = apPettyCashUtilization.LastModified,
                CreatedBy = apPettyCashUtilization.CreatedBy,
                ModifiedBy = apPettyCashUtilization.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCashUtilizations_Create(int id,[DataSourceRequest]DataSourceRequest request, ApPettyCashUtilization apPettyCashUtilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPettyCashUtilization
                {
                    ApPettyCashId=id,
                    Date = apPettyCashUtilization.Date,
                    PaidTo = apPettyCashUtilization.PaidTo,
                    Purpose = apPettyCashUtilization.Purpose,
                    Amount = apPettyCashUtilization.Amount,
                    AccountCode = apPettyCashUtilization.AccountCode,
                    Reference = apPettyCashUtilization.Reference,
                    OrgBranchName = apPettyCashUtilization.OrgBranchName,
                    DateCreated = apPettyCashUtilization.DateCreated,
                    LastModified = apPettyCashUtilization.LastModified,
                    CreatedBy = apPettyCashUtilization.CreatedBy,
                    ModifiedBy = apPettyCashUtilization.ModifiedBy
                };

                _db.ApPettyCashUtilizations.Add(entity);
                _db.SaveChanges();
                apPettyCashUtilization.ApPettyCashUtilizationId = entity.ApPettyCashUtilizationId;
                _distributionHelper.UpdateApPettyCashDistribution(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP petty cash utilization item", "Success", User.Identity.Name, $"created petty cash utilization item {entity.AccountCode} under record {entity.ApPettyCashId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apPettyCashUtilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCashUtilizations_Update([DataSourceRequest]DataSourceRequest request, ApPettyCashUtilization apPettyCashUtilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPettyCashUtilization
                {
                    ApPettyCashId = apPettyCashUtilization.ApPettyCashId,
                    ApPettyCashUtilizationId = apPettyCashUtilization.ApPettyCashUtilizationId,
                    Date = apPettyCashUtilization.Date,
                    PaidTo = apPettyCashUtilization.PaidTo,
                    Purpose = apPettyCashUtilization.Purpose,
                    Amount = apPettyCashUtilization.Amount,
                    AccountCode = apPettyCashUtilization.AccountCode,
                    Reference = apPettyCashUtilization.Reference,
                    OrgBranchName = apPettyCashUtilization.OrgBranchName,
                    DateCreated = apPettyCashUtilization.DateCreated,
                    LastModified = apPettyCashUtilization.LastModified,
                    CreatedBy = apPettyCashUtilization.CreatedBy,
                    ModifiedBy = apPettyCashUtilization.ModifiedBy
                };

                _db.ApPettyCashUtilizations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                _distributionHelper.UpdateApPettyCashDistribution(entity.ApPettyCashId);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP petty cash utilization item", "Success", User.Identity.Name, $"updated petty cash utilization item {entity.AccountCode} under record {entity.ApPettyCashId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apPettyCashUtilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCashUtilizations_Destroy([DataSourceRequest]DataSourceRequest request, ApPettyCashUtilization apPettyCashUtilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPettyCashUtilization
                {
                    ApPettyCashId = apPettyCashUtilization.ApPettyCashId,
                    ApPettyCashUtilizationId = apPettyCashUtilization.ApPettyCashUtilizationId,
                    Date = apPettyCashUtilization.Date,
                    PaidTo = apPettyCashUtilization.PaidTo,
                    Purpose = apPettyCashUtilization.Purpose,
                    Amount = apPettyCashUtilization.Amount,
                    AccountCode = apPettyCashUtilization.AccountCode,
                    Reference = apPettyCashUtilization.Reference,
                    OrgBranchName = apPettyCashUtilization.OrgBranchName,
                    DateCreated = apPettyCashUtilization.DateCreated,
                    LastModified = apPettyCashUtilization.LastModified,
                    CreatedBy = apPettyCashUtilization.CreatedBy,
                    ModifiedBy = apPettyCashUtilization.ModifiedBy
                };

                _db.ApPettyCashUtilizations.Attach(entity);
                _db.ApPettyCashUtilizations.Remove(entity);
                _db.SaveChanges();
                _distributionHelper.UpdateApPettyCashDistribution(entity.ApPettyCashId);

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted petty cash utilization item " + apPettyCashUtilization.ApPettyCashId + " " + apPettyCashUtilization.Amount,Modules.Finance);
                #endregion
            }

            return Json(new[] { apPettyCashUtilization }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
