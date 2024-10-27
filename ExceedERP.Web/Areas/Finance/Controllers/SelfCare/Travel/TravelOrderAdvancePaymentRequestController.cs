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

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare
{
    public class TravelOrderAdvancePaymentRequestController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TravelOrderAdvancePayments_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelOrderAdvancePayment> aptravelorderadvancepayments = _db.ApTravelOrderAdvancePayments.Where(x=>x.ApTravelOrderId==id);
            DataSourceResult result = aptravelorderadvancepayments.ToDataSourceResult(request, apTravelOrderAdvancePayment => new {
                ApTravelOrderAdvancePaymentId = apTravelOrderAdvancePayment.ApTravelOrderAdvancePaymentId,
                ApTravelOrderId = apTravelOrderAdvancePayment.ApTravelOrderId,
                Type = apTravelOrderAdvancePayment.Type,
                ItemName = apTravelOrderAdvancePayment.ItemName,
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
        public ActionResult TravelOrderAdvancePayments_Create(int id,[DataSourceRequest]DataSourceRequest request, ApTravelOrderAdvancePayment apTravelOrderAdvancePayment)
        {
            var exists=_db.ApTravelOrderAdvancePayments
                .Where(x=>x.ApTravelOrderId==id)
                .Where(x=>x.Type== apTravelOrderAdvancePayment.Type)
                .Where(x=>x.Type!= TravelPaymentType.Other)
                .FirstOrDefault();
            if(exists!=null)
                ModelState.AddModelError("exisit", "You have already added this payment type");
            #region check auto calculate
            if (FinanceConfigurationHelper.AutoCalculatetravelPerdiem() && apTravelOrderAdvancePayment.Type == TravelPaymentType.PerDiem)
            {
                var departureAndReturn=_db.ApTravelOrderDepartures.Where(x=>x.ApTravelOrderId==id).ToList();
                var departureDate= departureAndReturn.Where(x=>x.Type==DepartureType.Departure);
                var returnDate= departureAndReturn.Where(x => x.Type == DepartureType.Return);

                if (!departureDate.Any() || !returnDate.Any())
                    ModelState.AddModelError("dates", "Please fill both departure and return date");
                else
                {
                    apTravelOrderAdvancePayment.Quantity = (int)(returnDate.FirstOrDefault().ArrivalDate-departureDate.FirstOrDefault().Date).Days+1;
                }
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderAdvancePayment
                {
                    ApTravelOrderId = id,
                    Type = apTravelOrderAdvancePayment.Type,
                    ItemName = apTravelOrderAdvancePayment.ItemName,
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
                //_distributionHelper.UpdateApTravelOrderDistribution(id);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care advance payment item", "Success", User.Identity.Name, $"created advance payment item {entity.Description} under record {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { apTravelOrderAdvancePayment }.ToDataSourceResult(request, ModelState));
        }

        

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelOrderAdvancePayments_Update([DataSourceRequest]DataSourceRequest request, ApTravelOrderAdvancePayment apTravelOrderAdvancePayment)
        {
            var exists = _db.ApTravelOrderAdvancePayments
               .Where(x => x.ApTravelOrderAdvancePaymentId != apTravelOrderAdvancePayment.ApTravelOrderAdvancePaymentId)
               .Where(x => x.ApTravelOrderId == apTravelOrderAdvancePayment.ApTravelOrderId)
               .Where(x => x.Type == apTravelOrderAdvancePayment.Type)
               .Where(x => x.Type != TravelPaymentType.Other)
               .FirstOrDefault();
            if (exists != null)
                ModelState.AddModelError("exisit", "You have already added this payment type");
            #region check auto calculate
            if (FinanceConfigurationHelper.AutoCalculatetravelPerdiem() && apTravelOrderAdvancePayment.Type==TravelPaymentType.PerDiem)
            {
                var departureAndReturn = _db.ApTravelOrderDepartures.Where(x => x.ApTravelOrderId == apTravelOrderAdvancePayment.ApTravelOrderId).ToList();
                var departureDate = departureAndReturn.Where(x => x.Type == DepartureType.Departure);
                var returnDate = departureAndReturn.Where(x => x.Type == DepartureType.Return);

                if (!departureDate.Any() || !returnDate.Any())
                    ModelState.AddModelError("dates", "Please fill both departure and return date");
                else
                {
                    apTravelOrderAdvancePayment.Quantity = (int)(returnDate.FirstOrDefault().ArrivalDate - departureDate.FirstOrDefault().Date).Days + 1;
                }
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderAdvancePayment
                {
                    ApTravelOrderAdvancePaymentId = apTravelOrderAdvancePayment.ApTravelOrderAdvancePaymentId,
                    ApTravelOrderId=apTravelOrderAdvancePayment.ApTravelOrderId,
                    Type = apTravelOrderAdvancePayment.Type,
                    ItemName = apTravelOrderAdvancePayment.ItemName,
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
               // _distributionHelper.UpdateApTravelOrderDistribution(apTravelOrderAdvancePayment.ApTravelOrderId);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care advance payment item", "Success", User.Identity.Name, $"updated advance payment item {entity.Description} under record {entity.ApTravelOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelOrderAdvancePayment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelOrderAdvancePayments_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelOrderAdvancePayment apTravelOrderAdvancePayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelOrderAdvancePayment
                {
                    ApTravelOrderAdvancePaymentId = apTravelOrderAdvancePayment.ApTravelOrderAdvancePaymentId,
                    Type = apTravelOrderAdvancePayment.Type,
                    ItemName = apTravelOrderAdvancePayment.ItemName,
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
                UpdatetravelOrderBalance(apTravelOrderAdvancePayment.ApTravelOrderId);
                //_distributionHelper.UpdateApTravelOrderDistribution(apTravelOrderAdvancePayment.ApTravelOrderId);
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
