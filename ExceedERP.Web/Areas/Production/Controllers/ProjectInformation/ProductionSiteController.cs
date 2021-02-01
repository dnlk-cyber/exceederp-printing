using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.ProjectInformation
{
    public class ProductionSiteController : Controller
    {
        private ExceedDbContext db;
        public ProductionSiteController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name}).ToList();
            return View();
        }
        public ActionResult ProductionSites_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionSite> productionSites = db.ProductionSites;
            DataSourceResult result = productionSites.ToDataSourceResult(request, site => new
            {
                ProductionSiteId = site.ProductionSiteId,
                BranchId = site.BranchId,
                Name = site.Name,

                OrgBranchName = site.OrgBranchName,
                DateCreated = site.DateCreated,
                LastModified = site.LastModified,
                CreatedBy = site.CreatedBy,
                ModifiedBy = site.ModifiedBy,
            });

            return Json(result);
        }
        private bool SiteNameExists(ProductionSite model)
        {
            var siteExists = false;
            var temp = db.ProductionSites.FirstOrDefault(s => s.BranchId == model.BranchId && s.Name == model.Name && s.ProductionSiteId != model.ProductionSiteId);
            if (temp != null)
                siteExists = true;
            return siteExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionSite_Create([DataSourceRequest]DataSourceRequest request, ProductionSite site)
        {
            if (ModelState.IsValid)
            {
                if (SiteNameExists(site))
                {
                    ModelState.AddModelError("", "Such Site is Defined already!");
                }
                else
                {
                    var entity = new ProductionSite
                    {
                        ProductionSiteId = site.ProductionSiteId,
                        BranchId = site.BranchId,
                        Name = site.Name,

                        OrgBranchName = site.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = site.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = site.ModifiedBy,

                    };

                    db.ProductionSites.Add(entity);
                    db.SaveChanges();

                    site.ProductionSiteId = entity.ProductionSiteId;
                }

            }

            return Json(new[] { site }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionSite_Update([DataSourceRequest]DataSourceRequest request, ProductionSite site)
        {
            if (ModelState.IsValid)
            {
                if (SiteNameExists(site))
                {
                    ModelState.AddModelError("", "Such Site is Defined already!");
                }
                else
                {
                    var entity = new ProductionSite
                    {
                        ProductionSiteId = site.ProductionSiteId,
                        BranchId = site.BranchId,
                        Name = site.Name,

                        OrgBranchName = site.OrgBranchName,
                        DateCreated = site.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = site.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };

                    db.ProductionSites.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();


                }


            }

            return Json(new[] { site }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionSite_Destroy([DataSourceRequest]DataSourceRequest request, ProductionSite site)
        {
            if (ModelState.IsValid)
            {


                var entity = new ProductionSite
                {
                    ProductionSiteId = site.ProductionSiteId,
                    BranchId = site.BranchId,
                    Name = site.Name,

                    OrgBranchName = site.OrgBranchName,
                    DateCreated = site.DateCreated,
                    LastModified = site.LastModified,
                    CreatedBy = site.CreatedBy,
                    ModifiedBy = site.ModifiedBy,

                };

                db.ProductionSites.Attach(entity);
                db.ProductionSites.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { site }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}