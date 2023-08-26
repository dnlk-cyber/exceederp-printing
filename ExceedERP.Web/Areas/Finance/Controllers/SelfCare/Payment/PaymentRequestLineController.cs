using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare
{
    public class PaymentRequestLineController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Finance/PaymentRequestLine
     
        public ActionResult PaymentRequestLine_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            IQueryable<PaymentRequestLine> paymentRequestLines = db.PaymentRequestLines.Where(x=>x.PaymentRequestId == id);


            DataSourceResult result = paymentRequestLines.ToDataSourceResult(request, paymentRequest => new
            {
                PaymentRequestId = paymentRequest.PaymentRequestId,
                PaymentRequestLineId = paymentRequest.PaymentRequestLineId,
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
            var tax = db.GLTaxRates.Find(paymentRequest.GLTaxRateID);

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

                    db.PaymentRequestLines.Add(entity);
                    db.SaveChanges();
                    paymentRequest.PaymentRequestLineId = entity.PaymentRequestLineId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request item", "Success", User.Identity.Name, $"created payment request item {entity.Description} under record {entity.PaymentRequestId} ", Modules.Finance);
                    #endregion
                }
            }



            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentRequestLine_Update([DataSourceRequest]DataSourceRequest request, PaymentRequestLine paymentRequest)
        {
            var tax = db.GLTaxRates.Find(paymentRequest.GLTaxRateID);

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


                    db.PaymentRequestLines.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Self care payment request item", "Success", User.Identity.Name, $"updated payment request item {entity.Description} under record {entity.PaymentRequestId} ", Modules.Finance);
                    #endregion
                }

              

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
                #region delete log
                //UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Payment request of employee code  " + paymentRequest.EmployeeCode);
                #endregion
                db.PaymentRequestLines.Attach(entity);
                db.PaymentRequestLines.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { paymentRequest }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
