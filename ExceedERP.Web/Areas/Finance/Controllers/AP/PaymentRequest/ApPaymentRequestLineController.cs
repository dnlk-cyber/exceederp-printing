using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP.PaymentRequest
{
    public class ApPaymentRequestLineController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        // GET: Finance/PaymentRequestLine
        public ActionResult Index()
        {
            PopulateViewData();

            return View();
        }
        private void PopulateViewData()
        {
            var employeName = _db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;

            var tax = _db.GLTaxRates.ToList();
            ViewData["GLTaxRate"] = tax;

            var payment = _db.PaymentRequests.ToList();
            ViewData["PaymentRequest"] = payment;
        }
      
        public ActionResult PaymentRequestLine_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            IQueryable<PaymentRequestLine> paymentRequestLines = _db.PaymentRequestLines.Where(x=>x.PaymentRequestId == id);


            DataSourceResult result = paymentRequestLines.ToDataSourceResult(request, paymentRequest => new
            {
                PaymentRequestLineId = paymentRequest.PaymentRequestLineId,
                PaymentRequestId = paymentRequest.PaymentRequestId,
                GLTaxRateID = paymentRequest.GLTaxRateID,
                ItemName = paymentRequest.ItemName,
                Description = paymentRequest.Description,
                Amount = paymentRequest.Amount,
                TaxAmount = paymentRequest.TaxAmount,
                TotalAmount = paymentRequest.TotalAmount,
                Remark = paymentRequest.Remark
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestLine_Create([DataSourceRequest]DataSourceRequest request, PaymentRequestLine paymentRequest, int id)
        {
           // ModelState.Clear();
            var tax = _db.GLTaxRates.Find(paymentRequest.GLTaxRateID);

            if (ModelState.IsValid)
            {
                if (tax != null)
                {
                    var entity = new PaymentRequestLine
                    {

                        GLTaxRateID = paymentRequest.GLTaxRateID,
                        PaymentRequestId = id,
                        ItemName = paymentRequest.ItemName,
                        Description = paymentRequest.Description,
                        Amount = paymentRequest.Amount,
                        TaxAmount = paymentRequest.Amount* tax.CalculatedRate,
                        TotalAmount = paymentRequest.Amount+ (paymentRequest.Amount * tax.CalculatedRate),
                        Remark = paymentRequest.Remark
                    };

                    _db.PaymentRequestLines.Add(entity);
                    _db.SaveChanges();
                    paymentRequest.PaymentRequestLineId = entity.PaymentRequestLineId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP payment request item", "Success", User.Identity.Name, $"created payment request item {entity.Description} under record {entity.PaymentRequestId} ", Modules.Finance);
                    #endregion
                }
            }



            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestLine_Update([DataSourceRequest]DataSourceRequest request, PaymentRequestLine paymentRequest)
        {
            var tax = _db.GLTaxRates.Find(paymentRequest.GLTaxRateID);

            if (ModelState.IsValid)
            {
                if (tax != null)
                {
                    var entity = new PaymentRequestLine
                    {

                        PaymentRequestLineId=paymentRequest.PaymentRequestLineId,
                        GLTaxRateID = paymentRequest.GLTaxRateID,
                        PaymentRequestId = paymentRequest.PaymentRequestId,
                        ItemName = paymentRequest.ItemName,
                        Description = paymentRequest.Description,
                        Amount = paymentRequest.Amount,
                        TaxAmount = paymentRequest.Amount * tax.CalculatedRate,
                        TotalAmount = paymentRequest.Amount + (paymentRequest.Amount * tax.CalculatedRate),
                        Remark = paymentRequest.Remark
                    };


                    _db.PaymentRequestLines.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP payment request item", "Success", User.Identity.Name, $"updated payment request item {entity.Description} under record {entity.PaymentRequestId} ", Modules.Finance);
                    #endregion
                }

                _db.SaveChanges();

            }

            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestLine_Destroy([DataSourceRequest]DataSourceRequest request, PaymentRequestLine paymentRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new PaymentRequestLine
                {
                    PaymentRequestLineId = paymentRequest.PaymentRequestLineId,
                    GLTaxRateID = paymentRequest.GLTaxRateID,
                    PaymentRequestId = paymentRequest.PaymentRequestId,
                    ItemName = paymentRequest.ItemName,
                    Description = paymentRequest.Description,
                    Amount = paymentRequest.Amount,
                    TaxAmount = paymentRequest.TaxAmount,
                    TotalAmount = paymentRequest.TotalAmount,
                    Remark = paymentRequest.Remark
                };
             
                _db.PaymentRequestLines.Attach(entity);
                _db.PaymentRequestLines.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted payment request item " + paymentRequest.PaymentRequestId + " " + paymentRequest.ItemName, Modules.Finance);
                #endregion
            }

            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
