using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class AdditionalCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdditionalCosts_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AdditionalCost> AdditionalCosts = db.AdditionalCosts;
            DataSourceResult result = AdditionalCosts.ToDataSourceResult(request, AdditionalCost => new AdditionalCost
            {
                AdditionalCostId = AdditionalCost.AdditionalCostId,
                CreatedBy = AdditionalCost.CreatedBy,
                Name = AdditionalCost.Name,
                Type = AdditionalCost.Type,
                SalesAccountCode = AdditionalCost.SalesAccountCode,

                DateCreated = AdditionalCost.DateCreated,
                LastModified = AdditionalCost.LastModified,
                ModifiedBy = AdditionalCost.ModifiedBy,
                OrgBranchName = AdditionalCost.OrgBranchName,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdditionalCosts_Create([DataSourceRequest]DataSourceRequest request, AdditionalCost AdditionalCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new AdditionalCost
                {

                    AdditionalCostId = AdditionalCost.AdditionalCostId,
                    SalesAccountCode = AdditionalCost.SalesAccountCode,
                    Type = AdditionalCost.Type,
                    Name = AdditionalCost.Name,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    
                };

                db.AdditionalCosts.Add(entity);
                db.SaveChanges();
                AdditionalCost.AdditionalCostId = entity.AdditionalCostId;
            }

            return Json(new[] { AdditionalCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdditionalCosts_Update([DataSourceRequest]DataSourceRequest request, AdditionalCost AdditionalCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new AdditionalCost
                {
                    AdditionalCostId = AdditionalCost.AdditionalCostId,
                    SalesAccountCode = AdditionalCost.SalesAccountCode,
                    Type = AdditionalCost.Type,
                    Name = AdditionalCost.Name,
                };

                db.AdditionalCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { AdditionalCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdditionalCosts_Destroy([DataSourceRequest]DataSourceRequest request, AdditionalCost AdditionalCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new AdditionalCost
                {
                    AdditionalCostId = AdditionalCost.AdditionalCostId,

                };

                db.AdditionalCosts.Attach(entity);
                db.AdditionalCosts.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Additional Cost number " + AdditionalCost.AdditionalCostId);

            }

            return Json(new[] { AdditionalCost }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}