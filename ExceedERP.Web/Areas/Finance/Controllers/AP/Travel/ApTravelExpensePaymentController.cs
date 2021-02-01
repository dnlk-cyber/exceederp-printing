using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.Travel
{
    public class ApTravelExpensePaymentController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApTravelExpensePayments_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelExpensePayment> aptravelexpensepayments = _db.ApTravelExpensePayments.Where(x => x.ApTravelExpenseClaimId == id); ;
            DataSourceResult result = aptravelexpensepayments.ToDataSourceResult(request, apTravelExpensePayment => new {
                ApTravelExpenseClaimId = apTravelExpensePayment.ApTravelExpenseClaimId,
                ApTravelExpensePaymentId = apTravelExpensePayment.ApTravelExpensePaymentId,
                Description = apTravelExpensePayment.Description,
                AccountCode = apTravelExpensePayment.AccountCode,
                Amount = apTravelExpensePayment.Amount,
                PaymentMethod = apTravelExpensePayment.PaymentMethod,
                Reference = apTravelExpensePayment.Reference,

                OrgBranchName = apTravelExpensePayment.OrgBranchName,
                DateCreated = apTravelExpensePayment.DateCreated,
                LastModified = apTravelExpensePayment.LastModified,
                CreatedBy = apTravelExpensePayment.CreatedBy,
                ModifiedBy = apTravelExpensePayment.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpensePayments_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelExpensePayment apTravelExpensePayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpensePayment
                {
                    ApTravelExpenseClaimId=id,
                    Description = apTravelExpensePayment.Description,
                    AccountCode = apTravelExpensePayment.AccountCode,
                    Amount = apTravelExpensePayment.Amount,
                    PaymentMethod = apTravelExpensePayment.PaymentMethod,
                    Reference = apTravelExpensePayment.Reference,

                    OrgBranchName = apTravelExpensePayment.OrgBranchName,
                    DateCreated = apTravelExpensePayment.DateCreated,
                    LastModified = apTravelExpensePayment.LastModified,
                    CreatedBy = apTravelExpensePayment.CreatedBy,
                    ModifiedBy = apTravelExpensePayment.ModifiedBy
                };

                _db.ApTravelExpensePayments.Add(entity);
                _db.SaveChanges();
                apTravelExpensePayment.ApTravelExpensePaymentId = entity.ApTravelExpensePaymentId;
                _distributionHelper.UpdateApTravelOrdeExpenseClaimDistribution(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP expense claim payment item", "Success", User.Identity.Name, $"created expense claim payment item {entity.Description} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpensePayment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpensePayments_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpensePayment apTravelExpensePayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpensePayment
                {
                    ApTravelExpenseClaimId = apTravelExpensePayment.ApTravelExpenseClaimId,
                    ApTravelExpensePaymentId = apTravelExpensePayment.ApTravelExpensePaymentId,
                    Description = apTravelExpensePayment.Description,
                    AccountCode = apTravelExpensePayment.AccountCode,
                    Amount = apTravelExpensePayment.Amount,
                    PaymentMethod = apTravelExpensePayment.PaymentMethod,
                    Reference = apTravelExpensePayment.Reference,

                    OrgBranchName = apTravelExpensePayment.OrgBranchName,
                    DateCreated = apTravelExpensePayment.DateCreated,
                    LastModified = apTravelExpensePayment.LastModified,
                    CreatedBy = apTravelExpensePayment.CreatedBy,
                    ModifiedBy = apTravelExpensePayment.ModifiedBy
                };

                _db.ApTravelExpensePayments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                _distributionHelper.UpdateApTravelOrdeExpenseClaimDistribution(apTravelExpensePayment.ApTravelExpenseClaimId);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP expense claim payment item", "Success", User.Identity.Name, $"updated expense claim payment item {entity.Description} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpensePayment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpensePayments_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpensePayment apTravelExpensePayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpensePayment
                {
                    ApTravelExpensePaymentId = apTravelExpensePayment.ApTravelExpensePaymentId,
                    Description = apTravelExpensePayment.Description,
                    AccountCode = apTravelExpensePayment.AccountCode,
                    Amount = apTravelExpensePayment.Amount,
                    PaymentMethod = apTravelExpensePayment.PaymentMethod,
                    Reference = apTravelExpensePayment.Reference,

                    OrgBranchName = apTravelExpensePayment.OrgBranchName,
                    DateCreated = apTravelExpensePayment.DateCreated,
                    LastModified = apTravelExpensePayment.LastModified,
                    CreatedBy = apTravelExpensePayment.CreatedBy,
                    ModifiedBy = apTravelExpensePayment.ModifiedBy
                };

                _db.ApTravelExpensePayments.Attach(entity);
                _db.ApTravelExpensePayments.Remove(entity);
                _db.SaveChanges();
                _distributionHelper.UpdateApTravelOrdeExpenseClaimDistribution(apTravelExpensePayment.ApTravelExpenseClaimId);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted expense claim payment item " + apTravelExpensePayment.ApTravelExpenseClaimId + " " + apTravelExpensePayment.Description, Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpensePayment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
