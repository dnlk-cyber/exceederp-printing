using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class PreCustomerViewController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PreCustomerViews_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<PreCustomer> precustomers = db.PreCustomers.ToList();
            List<PreCustomerView> precustomerViewList = precustomers.Select(p=>new PreCustomerView() { PreCustomerViewID=p.PreCustomerID,
                TradeName= p.TradeName,
                BrandName =p.BrandName,
                BusinessType =p.BusinessType,
                Category =p.Category,
                Status =p.Status,
                IsActive =p.IsActive

            } ).ToList();
            DataSourceResult result = precustomerViewList.ToDataSourceResult(request, preCustomer => new {
                PreCustomerViewID = preCustomer.PreCustomerViewID,
                TradeName = preCustomer.TradeName,
                BrandName = preCustomer.BrandName,
                BusinessType = preCustomer.BusinessType,
                Category = preCustomer.Category,
                Status = preCustomer.Status,
                IsActive = preCustomer.IsActive
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PreCustomerViews_Create([DataSourceRequest]DataSourceRequest request, PreCustomerView preCustomerView)
        {
            if (ModelState.IsValid)
            {
                var entity = new PreCustomerView
                {
                    TradeName = preCustomerView.TradeName,
                    BrandName = preCustomerView.BrandName,
                    BusinessType = preCustomerView.BusinessType,
                    Category = preCustomerView.Category,
                    Status = preCustomerView.Status,
                    IsActive = preCustomerView.IsActive
                };

                db.PreCustomerViews.Add(entity);
                db.SaveChanges();
                preCustomerView.PreCustomerViewID = entity.PreCustomerViewID;
            }

            return Json(new[] { preCustomerView }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PreCustomerViews_Update([DataSourceRequest]DataSourceRequest request, PreCustomerView preCustomerView)
        {
            if (ModelState.IsValid)
            {
                var entity = new PreCustomerView
                {
                    PreCustomerViewID = preCustomerView.PreCustomerViewID,
                    TradeName = preCustomerView.TradeName,
                    BrandName = preCustomerView.BrandName,
                    BusinessType = preCustomerView.BusinessType,
                    Category = preCustomerView.Category,
                    Status = preCustomerView.Status,
                    IsActive = preCustomerView.IsActive
                };

                db.PreCustomerViews.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { preCustomerView }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PreCustomerViews_Destroy([DataSourceRequest]DataSourceRequest request, PreCustomerView preCustomerView)
        {
            if (ModelState.IsValid)
            {
                var entity = new PreCustomerView
                {
                    PreCustomerViewID = preCustomerView.PreCustomerViewID,
                    TradeName = preCustomerView.TradeName,
                    BrandName = preCustomerView.BrandName,
                    BusinessType = preCustomerView.BusinessType,
                    Category = preCustomerView.Category,
                    Status = preCustomerView.Status,
                    IsActive = preCustomerView.IsActive
                };

                db.PreCustomerViews.Attach(entity);
                db.PreCustomerViews.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { preCustomerView }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
