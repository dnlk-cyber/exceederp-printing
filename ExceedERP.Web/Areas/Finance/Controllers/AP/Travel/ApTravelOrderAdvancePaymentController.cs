using System;
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
    public class ApTravelOrderAdvancePaymentController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApTravelOrderAdvancePayments_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelOrderAdvancePayment> aptravelorderadvancepayments = _db.ApTravelOrderAdvancePayments.Where(x=>x.ApTravelOrderId==id);
            DataSourceResult result = aptravelorderadvancepayments.ToDataSourceResult(request, apTravelOrderAdvancePayment => new {
                ApTravelOrderAdvancePaymentId = apTravelOrderAdvancePayment.ApTravelOrderAdvancePaymentId,
                ApTravelOrderId = apTravelOrderAdvancePayment.ApTravelOrderId,
                Type = apTravelOrderAdvancePayment.Type,
                Description = apTravelOrderAdvancePayment.Description,
                UoM = apTravelOrderAdvancePayment.UoM,
                UnitPrice = apTravelOrderAdvancePayment.UnitPrice,
                Quantity = apTravelOrderAdvancePayment.Quantity,
                Total = apTravelOrderAdvancePayment.Total,
                PaymentMethod = apTravelOrderAdvancePayment.PaymentMethod,
                OrgBranchName = apTravelOrderAdvancePayment.OrgBranchName,
                DateCreated = apTravelOrderAdvancePayment.DateCreated,
                LastModified = apTravelOrderAdvancePayment.LastModified,
                CreatedBy = apTravelOrderAdvancePayment.CreatedBy,
                ModifiedBy = apTravelOrderAdvancePayment.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderAdvancePayments_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelOrderAdvancePayment apTravelOrderAdvancePayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderAdvancePayment
                {
                    ApTravelOrderId = id,
                    Type = apTravelOrderAdvancePayment.Type,
                    Description = apTravelOrderAdvancePayment.Description,
                    UoM = apTravelOrderAdvancePayment.UoM,
                    UnitPrice = apTravelOrderAdvancePayment.UnitPrice,
                    Quantity = apTravelOrderAdvancePayment.Quantity,
                    Total = apTravelOrderAdvancePayment.UnitPrice* apTravelOrderAdvancePayment.Quantity,
                    PaymentMethod = apTravelOrderAdvancePayment.PaymentMethod,

                    OrgBranchName = apTravelOrderAdvancePayment.OrgBranchName,
                    DateCreated = apTravelOrderAdvancePayment.DateCreated,
                    LastModified = apTravelOrderAdvancePayment.LastModified,
                    CreatedBy = apTravelOrderAdvancePayment.CreatedBy,
                    ModifiedBy = apTravelOrderAdvancePayment.ModifiedBy
                };

                _db.ApTravelOrderAdvancePayments.Add(entity);
                _db.SaveChanges();
                apTravelOrderAdvancePayment.ApTravelOrderAdvancePaymentId = entity.ApTravelOrderAdvancePaymentId;

                UpdatetravelOrderBalance(id);
                _distributionHelper.UpdateApTravelOrderDistribution(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP travel order advance payment item", "Success", User.Identity.Name, $"created advance payment item {entity.Description} under record {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { apTravelOrderAdvancePayment }.ToDataSourceResult(request, ModelState));
        }

        

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderAdvancePayments_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrderAdvancePayment apTravelOrderAdvancePayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderAdvancePayment
                {
                    ApTravelOrderAdvancePaymentId = apTravelOrderAdvancePayment.ApTravelOrderAdvancePaymentId,
                    ApTravelOrderId=apTravelOrderAdvancePayment.ApTravelOrderId,
                    Type = apTravelOrderAdvancePayment.Type,
                    Description = apTravelOrderAdvancePayment.Description,
                    UoM = apTravelOrderAdvancePayment.UoM,
                    UnitPrice = apTravelOrderAdvancePayment.UnitPrice,
                    Quantity = apTravelOrderAdvancePayment.Quantity,
                    Total = apTravelOrderAdvancePayment.UnitPrice * apTravelOrderAdvancePayment.Quantity,
                    PaymentMethod = apTravelOrderAdvancePayment.PaymentMethod,

                    OrgBranchName = apTravelOrderAdvancePayment.OrgBranchName,
                    DateCreated = apTravelOrderAdvancePayment.DateCreated,
                    LastModified = apTravelOrderAdvancePayment.LastModified,
                    CreatedBy = apTravelOrderAdvancePayment.CreatedBy,
                    ModifiedBy = apTravelOrderAdvancePayment.ModifiedBy
                };

                _db.ApTravelOrderAdvancePayments.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                UpdatetravelOrderBalance(apTravelOrderAdvancePayment.ApTravelOrderId);
                _distributionHelper.UpdateApTravelOrderDistribution(apTravelOrderAdvancePayment.ApTravelOrderId);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AP travel order advance payment item", "Success", User.Identity.Name, $"updated advance payment item {entity.Description} under record {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrderAdvancePayment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelOrderAdvancePayments_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrderAdvancePayment apTravelOrderAdvancePayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderAdvancePayment
                {
                    ApTravelOrderAdvancePaymentId = apTravelOrderAdvancePayment.ApTravelOrderAdvancePaymentId,
                    Type = apTravelOrderAdvancePayment.Type,
                    Description = apTravelOrderAdvancePayment.Description,
                    UoM = apTravelOrderAdvancePayment.UoM,
                    UnitPrice = apTravelOrderAdvancePayment.UnitPrice,
                    Quantity = apTravelOrderAdvancePayment.Quantity,
                    Total = apTravelOrderAdvancePayment.Total,
                    PaymentMethod = apTravelOrderAdvancePayment.PaymentMethod,

                    OrgBranchName = apTravelOrderAdvancePayment.OrgBranchName,
                    DateCreated = apTravelOrderAdvancePayment.DateCreated,
                    LastModified = apTravelOrderAdvancePayment.LastModified,
                    CreatedBy = apTravelOrderAdvancePayment.CreatedBy,
                    ModifiedBy = apTravelOrderAdvancePayment.ModifiedBy
                };

                _db.ApTravelOrderAdvancePayments.Attach(entity);
                _db.ApTravelOrderAdvancePayments.Remove(entity);
                _db.SaveChanges();
                _distributionHelper.UpdateApTravelOrderDistribution(apTravelOrderAdvancePayment.ApTravelOrderId);

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted travel order advance item " + apTravelOrderAdvancePayment.ApTravelOrderId + " " + apTravelOrderAdvancePayment.Description, Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrderAdvancePayment }.ToDataSourceResult(request, ModelState));
        }


        private void UpdatetravelOrderBalance(int id)
        {
            var record = _db.ApTravelOrders.Find(id);
            decimal total = 0;
            if (record != null)
            {
                foreach (var advance in record.ApTravelOrderAdvancePayments)
                {
                    total = total + advance.UnitPrice * advance.Quantity;
                }

                record.AdvanceAmount = total;
                _db.Entry(record).State = EntityState.Modified;
            }

            _db.SaveChanges();

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
