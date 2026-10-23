using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.TravelRequests
{
    public class TravelExpenseRefundBudgetController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApTravelExpenseRefunds_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelExpenseRefund> aptravelexpenserefunds = _db.ApTravelExpenseRefunds.Where(x => x.ApTravelExpenseClaimId == id); ;
            DataSourceResult result = aptravelexpenserefunds.ToDataSourceResult(request, apTravelExpenseRefund => new {
                ApTravelExpenseClaimId = apTravelExpenseRefund.ApTravelExpenseClaimId,
                ApTravelExpenseRefundId = apTravelExpenseRefund.ApTravelExpenseRefundId,
                Description = apTravelExpenseRefund.Description,
                AccountCode = apTravelExpenseRefund.AccountCode,
                Amount = apTravelExpenseRefund.Amount,
                PaymentMethod = apTravelExpenseRefund.PaymentMethod,
                Reference = apTravelExpenseRefund.Reference,

                OrgBranchName = apTravelExpenseRefund.OrgBranchName,
                DateCreated = apTravelExpenseRefund.DateCreated,
                LastModified = apTravelExpenseRefund.LastModified,
                CreatedBy = apTravelExpenseRefund.CreatedBy,
                ModifiedBy = apTravelExpenseRefund.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseRefunds_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelExpenseRefund apTravelExpenseRefund)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseRefund
                {
                    ApTravelExpenseClaimId=id,
                    Description = apTravelExpenseRefund.Description,
                    AccountCode = apTravelExpenseRefund.AccountCode,
                    Amount = apTravelExpenseRefund.Amount,
                    PaymentMethod = apTravelExpenseRefund.PaymentMethod,
                    Reference = apTravelExpenseRefund.Reference,

                    OrgBranchName = apTravelExpenseRefund.OrgBranchName,
                    DateCreated = apTravelExpenseRefund.DateCreated,
                    LastModified = apTravelExpenseRefund.LastModified,
                    CreatedBy = apTravelExpenseRefund.CreatedBy,
                    ModifiedBy = apTravelExpenseRefund.ModifiedBy
                };

                _db.ApTravelExpenseRefunds.Add(entity);
                _db.SaveChanges();
                apTravelExpenseRefund.ApTravelExpenseRefundId = entity.ApTravelExpenseRefundId;


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP expense claim refund item", "Success", User.Identity.Name, $"created expense claim refund item {entity.Description} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpenseRefund }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseRefunds_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseRefund apTravelExpenseRefund)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseRefund
                {
                    ApTravelExpenseClaimId = apTravelExpenseRefund.ApTravelExpenseClaimId,
                    ApTravelExpenseRefundId = apTravelExpenseRefund.ApTravelExpenseRefundId,
                    Description = apTravelExpenseRefund.Description,
                    AccountCode = apTravelExpenseRefund.AccountCode,
                    Amount = apTravelExpenseRefund.Amount,
                    PaymentMethod = apTravelExpenseRefund.PaymentMethod,
                    Reference = apTravelExpenseRefund.Reference,

                    OrgBranchName = apTravelExpenseRefund.OrgBranchName,
                    DateCreated = apTravelExpenseRefund.DateCreated,
                    LastModified = apTravelExpenseRefund.LastModified,
                    CreatedBy = apTravelExpenseRefund.CreatedBy,
                    ModifiedBy = apTravelExpenseRefund.ModifiedBy
                };

                _db.ApTravelExpenseRefunds.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
              

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP expense claim refund item", "Success", User.Identity.Name, $"updated expense claim refund item {entity.Description} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpenseRefund }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseRefunds_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseRefund apTravelExpenseRefund)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseRefund
                {
                    ApTravelExpenseRefundId = apTravelExpenseRefund.ApTravelExpenseRefundId,
                    Description = apTravelExpenseRefund.Description,
                    AccountCode = apTravelExpenseRefund.AccountCode,
                    Amount = apTravelExpenseRefund.Amount,
                    PaymentMethod = apTravelExpenseRefund.PaymentMethod,
                   
                    OrgBranchName = apTravelExpenseRefund.OrgBranchName,
                    DateCreated = apTravelExpenseRefund.DateCreated,
                    LastModified = apTravelExpenseRefund.LastModified,
                    CreatedBy = apTravelExpenseRefund.CreatedBy,
                    ModifiedBy = apTravelExpenseRefund.ModifiedBy
                };

                _db.ApTravelExpenseRefunds.Attach(entity);
                _db.ApTravelExpenseRefunds.Remove(entity);
                _db.SaveChanges();
               

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted expense claim refund item " + apTravelExpenseRefund.ApTravelExpenseClaimId + " " + apTravelExpenseRefund.Description, Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpenseRefund }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
