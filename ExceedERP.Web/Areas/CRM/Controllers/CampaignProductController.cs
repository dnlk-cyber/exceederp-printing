using System;
using ExceedERP.Web.Helpers;
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
using ExceedERP.Web.Areas.FleetManagement.Models;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.MarketingUser)]
    public class CampaignProductController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CampaignProducts_Read([DataSourceRequest]DataSourceRequest request, string campaignID)
        {
            IQueryable<CampaignProduct> campaignproducts = db.CampaignProducts.Where(cp => cp.CampaignID.ToString() == campaignID);
            DataSourceResult result = campaignproducts.ToDataSourceResult(request, campaignProduct => new
            {
                CampaignProductID = campaignProduct.CampaignProductID,
                CampaignID = campaignProduct.CampaignID,
                CreatedBy = campaignProduct.CreatedBy,
                ProductID = campaignProduct.ProductID,
                Feedback = campaignProduct.Feedback,
                DateCreated = campaignProduct.DateCreated,
                LastModified = campaignProduct.LastModified,
                ModifiedBy = campaignProduct.ModifiedBy,
                OrgBranchName = campaignProduct.OrgBranchName,
              
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CampaignProducts_Create([DataSourceRequest]DataSourceRequest request, CampaignProduct campaignProduct, string Id)
        {
            if (ModelState.IsValid)
            {
                var entity = new CampaignProduct
                {
                    CampaignID = int.Parse(Id),
                    Feedback = campaignProduct.Feedback,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ProductID = campaignProduct.ProductID
                };

                db.CampaignProducts.Add(entity);
                db.SaveChanges();
                campaignProduct.CampaignProductID = entity.CampaignProductID;
            }

            return Json(new[] { campaignProduct }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CampaignProducts_Update([DataSourceRequest]DataSourceRequest request, CampaignProduct campaignProduct)
        {
            if (ModelState.IsValid)
            {
                var entity = new CampaignProduct
                {
                    CampaignProductID = campaignProduct.CampaignProductID,
                    CampaignID = campaignProduct.CampaignID,
                    ProductID = campaignProduct.ProductID
                };

                db.CampaignProducts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { campaignProduct }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CampaignProducts_Destroy([DataSourceRequest]DataSourceRequest request, CampaignProduct campaignProduct)
        {
            if (ModelState.IsValid)
            {
                var entity = new CampaignProduct
                {
                    CampaignProductID = campaignProduct.CampaignProductID,
                    CampaignID = campaignProduct.CampaignID,
                    ProductID = campaignProduct.ProductID
                };

                db.CampaignProducts.Attach(entity);
                db.CampaignProducts.Remove(entity);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Campaign Product number " + campaignProduct.CampaignID);


            }

            return Json(new[] { campaignProduct }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
