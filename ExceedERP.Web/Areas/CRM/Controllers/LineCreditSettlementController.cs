using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using ExceedERP.Helpers.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
     [AuthorizeRoles( CRMRoles.ServiceSupportUser)] 
    public class LineCreditSettlementController : Controller
    {
        //
        // GET: /CRM/LineCreditSettlement/
        public ActionResult Index()
        {
            return View();
        }
        private ExceedDbContext db = new ExceedDbContext();



        public ActionResult LineItems_Read([DataSourceRequest]DataSourceRequest request, int voucherID)
        {
            CRMPaymentDistibutionHelper CRMPaymentDistibutionHelper = new CRMPaymentDistibutionHelper();
            List<LineCreditSettlement> LineCreditSettlements = db.LineCreditSettlements.Where(li => li.CreditSettlementId == voucherID).ToList();
            DataSourceResult result = LineCreditSettlements.ToDataSourceResult(request, lineItem => new
            {
                LineCreditSettlementId = lineItem.LineCreditSettlementId,
                InvoiceId = lineItem.InvoiceId,
                CurrentAmmount = lineItem.CurrentAmmount,
                RemainingAmount = lineItem.RemainingAmount,
                SettelmentAmount = lineItem.SettelmentAmount,
                CreditSettlementId = lineItem.CreditSettlementId,
                Status = lineItem.Status,
                CreatedBy = lineItem.CreatedBy,
                DateCreated = lineItem.DateCreated,
                LastModified = lineItem.LastModified,
                ModifiedBy = lineItem.ModifiedBy,
                OrgBranchName = lineItem.OrgBranchName,
                Remark = lineItem.Remark

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItems_Create([DataSourceRequest]DataSourceRequest request, LineCreditSettlement lineItem, int voucherID)
        {
            CRMPaymentDistibutionHelper CRMPaymentDistibutionHelper = new CRMPaymentDistibutionHelper();
            db = new ExceedDbContext();

            decimal CurrentAmount = decimal.Zero;
            decimal lineAmount = 0;
            decimal settlementAmount = 0;

            var lines =db.LineItems.Where(x => x.Voucher  == lineItem.InvoiceId.ToString() && x.VoucherDefinitionID == "2").ToList();
            var settelmentline = db.LineCreditSettlements.Where(x=>x.InvoiceId ==  lineItem.InvoiceId).ToList();
            if (lines.Any())
                lineAmount = lines.Sum(x => x.TotalAmount);
            if (settelmentline.Any())
                settlementAmount = settelmentline.Sum(x => x.SettelmentAmount);

            CurrentAmount = lineAmount - settlementAmount; 
            //List<LineCreditSettlement> lit = db.LineCreditSettlements.Where(li => li.InvoiceId == lineItem.InvoiceId  ).ToList();


            //if (lit.Any())
            //{
            //    ModelState.AddModelError("InvoiceExist", " Invoice Already Exist ! ");

            //}
            //else

            if (lineItem.SettelmentAmount > CurrentAmount)
            {
                ModelState.AddModelError("AmountExceed", " Amount Exceed Required ! ");

            }
            else
            {




                var entity = new LineCreditSettlement
                {
                    CreditSettlementId = voucherID,
                    InvoiceId = lineItem.InvoiceId,
                    CurrentAmmount = CurrentAmount.ToString(),
                    RemainingAmount = CurrentAmount - lineItem.SettelmentAmount,
                    SettelmentAmount = lineItem.SettelmentAmount,

                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,

                };


                try
                {
                    db.LineCreditSettlements.Add(entity);
                    int save = db.SaveChanges();
                    if (save == 1)
                    {

                       // lineItem = entity;
                        var LineCredit = db.CreditSettlement.Where(x => x.CreditSettlementId == entity.CreditSettlementId).FirstOrDefault();
                        var PaymentMethod = db.PaymentMethods.Where(x => x.PaymentMethodID == LineCredit.RecieveMethod).FirstOrDefault();
                        var AccountMap = db.AccountMaps.ToList();
                        var SalesInvoicePaymentDistibutions = CRMPaymentDistibutionHelper.DistributeCreditSettelment(entity, PaymentMethod,LineCredit.Customer, AccountMap);

                        if (SalesInvoicePaymentDistibutions.Any())
                        {
                            db.SalesInvoicePaymentDistibutions.AddRange(SalesInvoicePaymentDistibutions);
                            db.SaveChanges();
                        }
                    }
                }
                catch (Exception e)
                {
                    return Json(new[] { lineItem }.ToDataSourceResult(request));
                   
                }
            }


            return Json(new[] { lineItem }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItems_Update([DataSourceRequest]DataSourceRequest request, LineCreditSettlement lineItem, int voucherID)
        {
            db = new ExceedDbContext();

            decimal CurrentAmount = decimal.Zero;

            var lines =
                db.LineItems.Where(
                    x => x.Voucher == lineItem.InvoiceId.ToString() && x.VoucherDefinitionID == "2").ToList();

            if (lines.Any())
            {
                CurrentAmount = lines.Sum(x => x.TotalAmount);
            }

            List<LineCreditSettlement> lit = db.LineCreditSettlements.Where(li => li.InvoiceId == lineItem.InvoiceId && li.CreditSettlementId == voucherID && li.LineCreditSettlementId != lineItem.LineCreditSettlementId).ToList();


            if (lit.Any())
            {
                ModelState.AddModelError("InvoiceExist", " Invoice Already Exist ! ");

            }
            else if (lineItem.SettelmentAmount > CurrentAmount)
            {
                ModelState.AddModelError("AmountExceed", " Amount Exceed Required ! ");

            }
            else
            {




                var entity = new LineCreditSettlement
                {
                    LineCreditSettlementId = lineItem.LineCreditSettlementId,
                    CreditSettlementId = voucherID,
                    InvoiceId = lineItem.InvoiceId,
                    CurrentAmmount = CurrentAmount.ToString(),
                    RemainingAmount = CurrentAmount - lineItem.SettelmentAmount,
                    SettelmentAmount = lineItem.SettelmentAmount,

                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,

                };


                try
                {
                    db.LineCreditSettlements.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                catch (Exception e)
                {

                    String err = e.InnerException.Data.Values.ToString();
                }
            }


            return Json(new[] { lineItem }.ToDataSourceResult(request, ModelState));
        }


    }
}