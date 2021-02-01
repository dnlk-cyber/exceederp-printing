﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
﻿using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
      [AuthorizeRoles(CRMRoles.SalesQuoteUser, CRMRoles.SalesQuoteApprover)] 
   
    public class SalesQuoteVoucherViewController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SalesQuoteVoucherViews_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SalesQuoteVoucher> salesquotevoucherviews = db.SalesQuoteVouchers.Where(sqv => sqv.LastObjectState == null);
            DataSourceResult result = salesquotevoucherviews.ToDataSourceResult(request, salesQuoteVoucherView => new {
                SalesQuoteVoucherID = salesQuoteVoucherView.SalesQuoteVoucherID,
                OrganizationCustomerId = salesQuoteVoucherView.OrganizationCustomerId,
                IssuedDate = salesQuoteVoucherView.IssuedDate,
                IsIssued = salesQuoteVoucherView.IsIssued,
                Date = salesQuoteVoucherView.Date,
                IsVoid = salesQuoteVoucherView.IsVoid,
                GrandTotal = salesQuoteVoucherView.GrandTotal,
                Period = salesQuoteVoucherView.Period,
                LastObjectState = salesQuoteVoucherView.LastObjectState,
                PaymentMethod = salesQuoteVoucherView.PaymentMethod,
                Remark = salesQuoteVoucherView.Remark,
               
            });

            return Json(result);
        }
        public ActionResult SalesQuoteVoucherViews_Forward([DataSourceRequest]DataSourceRequest request, string userRole, string salesQuoteVoucherViewID)
        {
            SalesQuoteVoucher salesQuoteVoucher = db.SalesQuoteVouchers.FirstOrDefault(r => r.SalesQuoteVoucherID.ToString() ==salesQuoteVoucherViewID);
            salesQuoteVoucher.LastObjectState = "1";
            db.SalesQuoteVouchers.Attach(salesQuoteVoucher);
            db.Entry(salesQuoteVoucher).State = EntityState.Modified;
            db.SaveChanges();
            IQueryable<SalesQuoteVoucher> salesquotevoucherviews = db.SalesQuoteVouchers.Where(sqv=>sqv.LastObjectState==null);

            DataSourceResult result = salesquotevoucherviews.ToDataSourceResult(request, salesQuoteVoucherView => new
            {
                SalesQuoteVoucherID = salesQuoteVoucherView.SalesQuoteVoucherID,
                OrganizationCustomerId = salesQuoteVoucherView.OrganizationCustomerId,
                IssuedDate = salesQuoteVoucherView.IssuedDate,
                IsIssued = salesQuoteVoucherView.IsIssued,
                Date = salesQuoteVoucherView.Date,
                IsVoid = salesQuoteVoucherView.IsVoid,
                GrandTotal = salesQuoteVoucherView.GrandTotal,
                Period = salesQuoteVoucherView.Period,
                LastObjectState = salesQuoteVoucherView.LastObjectState,
                PaymentMethod = salesQuoteVoucherView.PaymentMethod,
                Remark = salesQuoteVoucherView.Remark,
                
            });

            return Json(result);
        }
      
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
