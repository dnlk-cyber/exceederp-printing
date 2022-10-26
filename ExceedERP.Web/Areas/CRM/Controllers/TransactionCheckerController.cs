using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [Authorize(Users ="tester")] 
    public class TransactionCheckerController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        #region bank reconciliation
        //public JsonResult SendMissingInvoicesToBankReconciliation()
        //{
        //    int successfulCnt = 0;
        //    int skippedCnt = 0;
        //    var transferedInvoices = db.CashSalesInvoices.Where(i => i.IsOnlineTransferred).ToList();
        //    foreach(var invoice in transferedInvoices)
        //    {
        //       var result =  CRMSalesHelper.SendSalesToBankReconciliation(invoice, User.Identity.Name);
        //        if(result != null)
        //        {
        //            successfulCnt += result.SuccessfulOperations;
        //            skippedCnt += result.IgnoredOperations;
        //        }
        //    }
        //    return Json(new
        //    {
        //        Message = $"{successfulCnt} distributions sent to reconciliation",
        //        Status = "OK"
        //    });
        //}
        //public JsonResult SendMissingDepositToBankReconciliation()
        //{
        //    int successfulCnt = 0;
        //    int skippedCnt = 0;
        //    var transferedInvoices = db.CRMDeposit.Where(i => i.IsOnlineTransferred).ToList();
        //    foreach (var invoice in transferedInvoices)
        //    {
        //        var result = CRMSalesHelper.SendDepositToBankReconciliation(invoice, User.Identity.Name);
        //        if (result != null)
        //        {
        //            successfulCnt += result.SuccessfulOperations;
        //            skippedCnt += result.IgnoredOperations;
        //        }
        //    }
        //    return Json(new
        //    {
        //        Message = $"{successfulCnt} distributions sent to reconciliation",
        //        Status = "OK"
        //    });
        //}
        //public JsonResult SendMissingCreditSettlementToBankReconciliation()
        //{
        //    int successfulCnt = 0;
        //    int skippedCnt = 0;
        //    var transferedRecords = db.CreditSettlement.Where(i => i.IsOnlineTransferred).ToList();
        //    foreach (var invoice in transferedRecords)
        //    {
        //        var result = CRMSalesHelper.SendCreditSettlementToBankReconciliation(invoice, User.Identity.Name);
        //        if (result != null)
        //        {
        //            successfulCnt += result.SuccessfulOperations;
        //            skippedCnt += result.IgnoredOperations;
        //        }
        //    }
        //    return Json(new
        //    {
        //        Message = $"{successfulCnt} distributions sent to reconciliation",
        //        Status = "OK"
        //    });
        //}
        #endregion


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
