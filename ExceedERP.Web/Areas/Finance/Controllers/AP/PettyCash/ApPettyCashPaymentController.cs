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
    public class ApPettyCashPaymentController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApPettyCashPayments_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPettyCashPayment> appettycashpayments = _db.ApPettyCashPayments.Where(x => x.ApPettyCashId == id);
            DataSourceResult result = appettycashpayments.ToDataSourceResult(request, apPettyCashPayment => new {
                ApPettyCashId = apPettyCashPayment.ApPettyCashId,
                ApPettyCashPaymentId = apPettyCashPayment.ApPettyCashPaymentId,
                Description = apPettyCashPayment.Description,
                PaymentMethod = apPettyCashPayment.PaymentMethod,
                Amount = apPettyCashPayment.Amount,
                Reference = apPettyCashPayment.Reference,
                OrgBranchName = apPettyCashPayment.OrgBranchName,
                DateCreated = apPettyCashPayment.DateCreated,
                LastModified = apPettyCashPayment.LastModified,
                CreatedBy = apPettyCashPayment.CreatedBy,
                ModifiedBy = apPettyCashPayment.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCashPayments_Create(int id,[DataSourceRequest]DataSourceRequest request, ApPettyCashPayment apPettyCashPayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPettyCashPayment
                {
                    ApPettyCashId=id,
                    Description = apPettyCashPayment.Description,
                    PaymentMethod = apPettyCashPayment.PaymentMethod,
                    Amount = apPettyCashPayment.Amount,
                    Reference = apPettyCashPayment.Reference,
                    OrgBranchName = apPettyCashPayment.OrgBranchName,
                    DateCreated = apPettyCashPayment.DateCreated,
                    LastModified = apPettyCashPayment.LastModified,
                    CreatedBy = apPettyCashPayment.CreatedBy,
                    ModifiedBy = apPettyCashPayment.ModifiedBy
                };

                _db.ApPettyCashPayments.Add(entity);
                _db.SaveChanges();
                apPettyCashPayment.ApPettyCashPaymentId = entity.ApPettyCashPaymentId;
                _distributionHelper.UpdateApPettyCashDistribution(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP petty cash payment item", "Success", User.Identity.Name, $"created petty cash payment  item {entity.Description} under record {entity.ApPettyCashId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apPettyCashPayment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCashPayments_Update([DataSourceRequest]DataSourceRequest request, ApPettyCashPayment apPettyCashPayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPettyCashPayment
                {
                    ApPettyCashId = apPettyCashPayment.ApPettyCashId,
                    ApPettyCashPaymentId = apPettyCashPayment.ApPettyCashPaymentId,
                    Description = apPettyCashPayment.Description,
                    PaymentMethod = apPettyCashPayment.PaymentMethod,
                    Amount = apPettyCashPayment.Amount,
                    Reference = apPettyCashPayment.Reference,
                    OrgBranchName = apPettyCashPayment.OrgBranchName,
                    DateCreated = apPettyCashPayment.DateCreated,
                    LastModified = apPettyCashPayment.LastModified,
                    CreatedBy = apPettyCashPayment.CreatedBy,
                    ModifiedBy = apPettyCashPayment.ModifiedBy
                };

                _db.ApPettyCashPayments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                _distributionHelper.UpdateApPettyCashDistribution(entity.ApPettyCashId);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP petty cash payment item", "Success", User.Identity.Name, $"updated petty cash payment  item {entity.Description} under record {entity.ApPettyCashId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apPettyCashPayment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPettyCashPayments_Destroy([DataSourceRequest]DataSourceRequest request, ApPettyCashPayment apPettyCashPayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApPettyCashPayment
                {
                    ApPettyCashId = apPettyCashPayment.ApPettyCashId,
                    ApPettyCashPaymentId = apPettyCashPayment.ApPettyCashPaymentId,
                    Description = apPettyCashPayment.Description,
                    PaymentMethod = apPettyCashPayment.PaymentMethod,
                    Amount = apPettyCashPayment.Amount,
                    Reference = apPettyCashPayment.Reference,
                    OrgBranchName = apPettyCashPayment.OrgBranchName,
                    DateCreated = apPettyCashPayment.DateCreated,
                    LastModified = apPettyCashPayment.LastModified,
                    CreatedBy = apPettyCashPayment.CreatedBy,
                    ModifiedBy = apPettyCashPayment.ModifiedBy
                };

                _db.ApPettyCashPayments.Attach(entity);
                _db.ApPettyCashPayments.Remove(entity);
                _db.SaveChanges();
                _distributionHelper.UpdateApPettyCashDistribution(entity.ApPettyCashId);

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted petty cash payment item " + apPettyCashPayment.ApPettyCashId + " " + apPettyCashPayment.Description, Modules.Finance);
                #endregion
            }

            return Json(new[] { apPettyCashPayment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
