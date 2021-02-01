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
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{
       [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class PreCustomerController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PreCustomers_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PreCustomer> precustomers = db.PreCustomers;
            DataSourceResult result = precustomers.ToDataSourceResult(request, preCustomer => new {
                PreCustomerID = preCustomer.PreCustomerID,
                TradeName = preCustomer.TradeName,
                BrandName = preCustomer.BrandName,
                BusinessType = preCustomer.BusinessType,
                Category = preCustomer.Category,
                Status = preCustomer.Status,
                CreatedBy = preCustomer.CreatedBy,
                DateCreated = preCustomer.DateCreated,
                LastModified = preCustomer.LastModified,
                ModifiedBy = preCustomer.ModifiedBy,
                OrgBranchName = preCustomer.OrgBranchName,
                IsActive = preCustomer.IsActive
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PreCustomers_Create([DataSourceRequest]DataSourceRequest request, PreCustomer preCustomer)
        {
            if (ModelState.IsValid)
            {
                var entity = new PreCustomer
                {
                    TradeName = preCustomer.TradeName,
                    BrandName = preCustomer.BrandName,
                    BusinessType = preCustomer.BusinessType,
                    Category = preCustomer.Category,
                    Status = preCustomer.Status,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    IsActive = preCustomer.IsActive
                };

                db.PreCustomers.Add(entity);
                db.SaveChanges();
                preCustomer.PreCustomerID = entity.PreCustomerID;
            }

            return Json(new[] { preCustomer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PreCustomers_Update([DataSourceRequest]DataSourceRequest request, PreCustomer preCustomer)
        {
            if (ModelState.IsValid)
            {
                var entity = new PreCustomer
                {
                    PreCustomerID = preCustomer.PreCustomerID,
                    TradeName = preCustomer.TradeName,
                    BrandName = preCustomer.BrandName,
                    BusinessType = preCustomer.BusinessType,
                    Category = preCustomer.Category,
                    Status = preCustomer.Status,
                    IsActive = preCustomer.IsActive
                };

                db.PreCustomers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { preCustomer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PreCustomers_Destroy([DataSourceRequest]DataSourceRequest request, PreCustomer preCustomer)
        {
            if (ModelState.IsValid)
            {
                var entity = new PreCustomer
                {
                    PreCustomerID = preCustomer.PreCustomerID,
                    TradeName = preCustomer.TradeName,
                    BrandName = preCustomer.BrandName,
                    BusinessType = preCustomer.BusinessType,
                    Category = preCustomer.Category,
                    Status = preCustomer.Status,
                    IsActive = preCustomer.IsActive
                };

                db.PreCustomers.Attach(entity);
                db.PreCustomers.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted PreCustomer number " + preCustomer.PreCustomerID);
      
            }

            return Json(new[] { preCustomer }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
