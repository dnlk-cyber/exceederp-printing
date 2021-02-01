using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.ForeignPurchase;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.ForeignPurchaseFollowUp
{
    public class ForeignPurchaseCostTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ForeignPurchaseCostTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ForeignPurchaseCostType> foreignpurchasecosttypes = db.ForeignPurchaseCostTypes;
            DataSourceResult result = foreignpurchasecosttypes.ToDataSourceResult(request, foreignPurchaseCostType => new {
                ForeignPurchaseCostTypeId = foreignPurchaseCostType.ForeignPurchaseCostTypeId,
                CostName = foreignPurchaseCostType.CostName,
                OrgBranchName = foreignPurchaseCostType.OrgBranchName,
                DateCreated = foreignPurchaseCostType.DateCreated,
                LastModified = foreignPurchaseCostType.LastModified,
                CreatedBy = foreignPurchaseCostType.CreatedBy,
                ModifiedBy = foreignPurchaseCostType.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseCostTypes_Create([DataSourceRequest]DataSourceRequest request, ForeignPurchaseCostType foreignPurchaseCostType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseCostType
                {
                    CostName = foreignPurchaseCostType.CostName,
                    OrgBranchName = foreignPurchaseCostType.OrgBranchName,
                    DateCreated = foreignPurchaseCostType.DateCreated,
                    LastModified = foreignPurchaseCostType.LastModified,
                    CreatedBy = foreignPurchaseCostType.CreatedBy,
                    ModifiedBy = foreignPurchaseCostType.ModifiedBy
                };

                db.ForeignPurchaseCostTypes.Add(entity);
                db.SaveChanges();
                foreignPurchaseCostType.ForeignPurchaseCostTypeId = entity.ForeignPurchaseCostTypeId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase cost type setup", "Success", User.Identity.Name, $"created type {entity.CostName}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseCostType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseCostTypes_Update([DataSourceRequest]DataSourceRequest request, ForeignPurchaseCostType foreignPurchaseCostType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseCostType
                {
                    ForeignPurchaseCostTypeId = foreignPurchaseCostType.ForeignPurchaseCostTypeId,
                    CostName = foreignPurchaseCostType.CostName,
                    OrgBranchName = foreignPurchaseCostType.OrgBranchName,
                    DateCreated = foreignPurchaseCostType.DateCreated,
                    LastModified = foreignPurchaseCostType.LastModified,
                    CreatedBy = foreignPurchaseCostType.CreatedBy,
                    ModifiedBy = foreignPurchaseCostType.ModifiedBy
                };

                db.ForeignPurchaseCostTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase cost type setup", "Success", User.Identity.Name, $"updated type {entity.CostName}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseCostType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseCostTypes_Destroy([DataSourceRequest]DataSourceRequest request, ForeignPurchaseCostType foreignPurchaseCostType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseCostType
                {
                    ForeignPurchaseCostTypeId = foreignPurchaseCostType.ForeignPurchaseCostTypeId,
                    CostName = foreignPurchaseCostType.CostName,
                    OrgBranchName = foreignPurchaseCostType.OrgBranchName,
                    DateCreated = foreignPurchaseCostType.DateCreated,
                    LastModified = foreignPurchaseCostType.LastModified,
                    CreatedBy = foreignPurchaseCostType.CreatedBy,
                    ModifiedBy = foreignPurchaseCostType.ModifiedBy
                };

                db.ForeignPurchaseCostTypes.Attach(entity);
                db.ForeignPurchaseCostTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { foreignPurchaseCostType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
