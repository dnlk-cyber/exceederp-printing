using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using Kendo.Mvc.UI;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
     //[AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class CreditSettlement1Controller : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /CRM/CreditSettlement/
        public ActionResult Index()
        {

            ViewData["PaymentMethod"] = db.PaymentMethods.ToList();
            ViewData["OrganizationCustomer"] = db.OrganizationCustomers.ToList();


            return View();
        }


        public ActionResult CreditSettlements_Read([DataSourceRequest]DataSourceRequest request)
        {

            List<CreditSettlement> voulist = new List<CreditSettlement>();

            try
            {

                  decimal GrandTotal = decimal.Zero;
                decimal RemainingAmount = decimal.Zero;
                List<CreditSettlement> CreditSettlements = new List<CreditSettlement>();
                CreditSettlements = db.CreditSettlement.ToList();
         
                try
                {
                    foreach (var vou in CreditSettlements)
                {
                    var CreditSales = db.CashSalesInvoices.Where(x => x.OrganizationCustomerId == vou.Customer && x.PaymentMethod == CRMConstants.CreditPaymentMethod).ToList();
                      GrandTotal = decimal.Zero;
                      RemainingAmount = decimal.Zero;
                    foreach (var CreditSale in CreditSales)
                    {
                        var SettlementLine = db.LineCreditSettlements.Where(x =>   x.CreditSettlementId == vou.CreditSettlementId).ToList();

                        var lines =
                            db.LineItems.Where(
                                x => x.Voucher  == CreditSale.CashSalesInvoiceID.ToString() && x.VoucherDefinitionID == "2").ToList();
                        RemainingAmount = RemainingAmount +( lines.Sum(x => x.TotalAmount) - SettlementLine.Sum(x => x.SettelmentAmount));
                        GrandTotal = GrandTotal+ lines.Sum(x => x.TotalAmount);
                    } 
                    
               
                    

                    string SalesStatus = string.Empty;


                    voulist.Add(new CreditSettlement { CreditSettlementId = vou.CreditSettlementId, Customer = vou.Customer, Date = vou.Date, GrandTotal = GrandTotal, Status = RemainingAmount.ToString(), Remark = vou.Remark, RecieveMethod = vou.RecieveMethod, ModifiedBy = vou.ModifiedBy, LastModified = vou.LastModified, DateCreated = vou.DateCreated, CreatedBy = vou.CreatedBy, IsOnlineApproved = vou.IsOnlineApproved, IsOnlineTransferred = vou.IsOnlineTransferred, IsOnlineTransferredBy = vou.IsOnlineTransferredBy, OnlineApprovedBy = vou.OnlineApprovedBy, OnlineApprovedTime = vou.OnlineApprovedTime, OnlineTransferredTime = vou.OnlineTransferredTime, OrgBranchName = vou.OrgBranchName, SendForApprovalBy = vou.SendForApprovalBy, SendForApprovalTime = vou.SendForApprovalTime });
                }
            }
                catch (Exception e)
                    { 
                       
 }

            }
            catch (Exception e)
            {

              
            }
            return Json(voulist.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);


        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreditSettlements_Create([DataSourceRequest]DataSourceRequest request, CreditSettlement CreditSettlement)
        {
            List<CreditSettlement> lit = db.CreditSettlement.Where(li => li.Customer == CreditSettlement.Customer  ).ToList();


            if (lit.Any())
            {
                ModelState.AddModelError("CreditSettlementExist", " CreditSettlement Already Exist ! ");

            }
            else
            {
                if (ModelState.IsValid)
                {

                    var entity = new CreditSettlement
                    {

                        Customer = CreditSettlement.Customer,
                        Date = CreditSettlement.Date,
                        IsVoid = CreditSettlement.IsVoid,
                        GrandTotal = CreditSettlement.GrandTotal,

                        RecieveMethod = CreditSettlement.RecieveMethod,
                        Remark = CreditSettlement.Remark,

                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name
                    };

                    db.CreditSettlement.Add(entity);
                    db.SaveChanges();
                    CreditSettlement.CreditSettlementId = entity.CreditSettlementId;
                }
            }
            return Json(new[] { CreditSettlement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CreditSettlements_Destroy([DataSourceRequest]DataSourceRequest request, CreditSettlement CreditSettlement)
        {
            List<LineCreditSettlement> line = db.LineCreditSettlements.Where(li => li.CreditSettlementId == CreditSettlement.CreditSettlementId).ToList();


            foreach (var item in line)
            {
                CashSalesInvoice invoice = db.CashSalesInvoices.Where(x => x.CashSalesInvoiceID  == item.InvoiceId).FirstOrDefault();
                invoice.PaymentMethod = "CASH";

                db.CashSalesInvoices.Attach(invoice);
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();

            }
            CreditSettlement.IsOnlineApproved = true;
            CreditSettlement.OnlineApprovedBy = User.Identity.Name;
            CreditSettlement.OnlineApprovedTime = DateTime.Now;

            db.CreditSettlement.Attach(CreditSettlement);
            db.Entry(CreditSettlement).State = EntityState.Modified;
            db.SaveChanges();


            return Json(new[] { CreditSettlement }.ToDataSourceResult(request));
        }


    }
}