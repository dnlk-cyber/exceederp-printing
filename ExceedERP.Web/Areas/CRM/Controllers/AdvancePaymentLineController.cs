using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using ExceedERP.Helpers.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class AdvancePaymentLineController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult AdvancePaymentLines_Read([DataSourceRequest]DataSourceRequest request ,int Id)
        {
           var AdvancePaymentLines = db.AdvancePaymentLines.Where(x=>x.AdvancePaymentId == Id).Select( s=> new  {
               AdvancePaymentLineId = s.AdvancePaymentLineId,
                AdvancePaymentId = s.AdvancePaymentId,
                RecieveMethod = s.RecieveMethod,
                CreatedBy = s.CreatedBy,
                DateCreated = s.DateCreated,
                Value = s.Value,
                PaymentMethod = s.PaymentMethod
            });
            DataSourceResult result = AdvancePaymentLines.ToDataSourceResult(request);

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvancePaymentLines_Create([DataSourceRequest]DataSourceRequest request, AdvancePaymentLine AdvancePaymentLine, int Id)
        {
            CRMPaymentDistibutionHelper CRMPaymentDistibutionHelper = new CRMPaymentDistibutionHelper();
                if (ModelState.IsValid)
                {
                    var entity = new AdvancePaymentLine
                    {
                       
                        RecieveMethod = AdvancePaymentLine.RecieveMethod,
                        Value = AdvancePaymentLine.Value,
                        CreatedBy = User.Identity.Name,
                        AdvancePaymentId = Id,
                        DateCreated = DateTime.Now
                    };



                    db.AdvancePaymentLines.Add(entity);
                    db.SaveChanges();

                    var  PaymentLine = db.AdvancePayments.Where(x => x.AdvancePaymentId == entity.AdvancePaymentId).FirstOrDefault();
                    
                    var AccountMap = db.AccountMaps.ToList();
                    var SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeAdvancePayment(entity, PaymentLine.OrganizationCustomer, AccountMap);

                    if (SalesInvoicePaymentDistibutions.Any())
                    {
                        db.AdvancePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                        db.SaveChanges();
                    }
                    

            }
            return Json(new[] { AdvancePaymentLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvancePaymentLines_Update([DataSourceRequest]DataSourceRequest request, AdvancePaymentLine AdvancePaymentLine)
        {
             

                if (ModelState.IsValid)
                {
                    var entity = new AdvancePaymentLine
                    {
                        AdvancePaymentLineId = AdvancePaymentLine.AdvancePaymentLineId,
                        AdvancePaymentId = AdvancePaymentLine.AdvancePaymentId,
                        RecieveMethod = AdvancePaymentLine.RecieveMethod,
                        Value = AdvancePaymentLine.Value,
                        ModifiedBy = User.Identity.Name,
                        LastModified = DateTime.Now
                    };

                    db.AdvancePaymentLines.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }

          
            return Json(new[] { AdvancePaymentLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvancePaymentLines_Destroy([DataSourceRequest]DataSourceRequest request, AdvancePaymentLine AdvancePaymentLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new AdvancePaymentLine
                {
                    AdvancePaymentLineId = AdvancePaymentLine.AdvancePaymentLineId,
                    Value = AdvancePaymentLine.Value
                };

                db.AdvancePaymentLines.Attach(entity);
                db.AdvancePaymentLines.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted   Advance Payment number " + AdvancePaymentLine.AdvancePaymentLineId);

            }

            return Json(new[] { AdvancePaymentLine }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}