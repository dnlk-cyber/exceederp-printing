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
    public class ForeignPurchaseItemCostController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ForeignPurchaseItemCosts_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ForeignPurchaseItemCost> foreignPurchaseItemCosts = _db.ForeignPurchaseItemCosts.Where(x=>x.ForeignPurchaseFollowUpItemsID==id);
            DataSourceResult result = foreignPurchaseItemCosts.ToDataSourceResult(request, foreignPurchaseItemCost => new {
                ForeignPurchaseItemCostId = foreignPurchaseItemCost.ForeignPurchaseItemCostId,
                Date = foreignPurchaseItemCost.Date,
                Payee = foreignPurchaseItemCost.Payee,
                CostType = foreignPurchaseItemCost.CostType,
                Amount = foreignPurchaseItemCost.Amount,
                Currency = foreignPurchaseItemCost.Currency,
                ExchangeRate = foreignPurchaseItemCost.ExchangeRate,
                ForexAmount = foreignPurchaseItemCost.ForexAmount,
                Paid = foreignPurchaseItemCost.Paid,
                Remark = foreignPurchaseItemCost.Remark,
                OrgBranchName = foreignPurchaseItemCost.OrgBranchName,
                DateCreated = foreignPurchaseItemCost.DateCreated,
                LastModified = foreignPurchaseItemCost.LastModified,
                CreatedBy = foreignPurchaseItemCost.CreatedBy,
                ModifiedBy = foreignPurchaseItemCost.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseItemCosts_Create(int id,[DataSourceRequest]DataSourceRequest request, ForeignPurchaseItemCost foreignPurchaseItemCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseItemCost
                {
                    ForeignPurchaseFollowUpItemsID=id,
                    Date = foreignPurchaseItemCost.Date,
                    Payee = foreignPurchaseItemCost.Payee,
                    CostType = foreignPurchaseItemCost.CostType,
                    Amount = foreignPurchaseItemCost.ForexAmount* foreignPurchaseItemCost.ExchangeRate,
                    Currency = foreignPurchaseItemCost.Currency,
                    ExchangeRate = foreignPurchaseItemCost.ExchangeRate,
                    ForexAmount = foreignPurchaseItemCost.ForexAmount,
                    Paid = foreignPurchaseItemCost.Paid,
                    Remark = foreignPurchaseItemCost.Remark,
                    OrgBranchName = foreignPurchaseItemCost.OrgBranchName,
                    DateCreated = foreignPurchaseItemCost.DateCreated,
                    LastModified = foreignPurchaseItemCost.LastModified,
                    CreatedBy = foreignPurchaseItemCost.CreatedBy,
                    ModifiedBy = foreignPurchaseItemCost.ModifiedBy
                };

                _db.ForeignPurchaseItemCosts.Add(entity);
                _db.SaveChanges();
                foreignPurchaseItemCost.ForeignPurchaseItemCostId = entity.ForeignPurchaseItemCostId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase follow up item cost", "Success", User.Identity.Name, $"created cost {entity.CostType} under record {entity.ForeignPurchaseFollowUpItemsID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseItemCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseItemCosts_Update([DataSourceRequest]DataSourceRequest request, ForeignPurchaseItemCost foreignPurchaseItemCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseItemCost
                {
                    ForeignPurchaseFollowUpItemsID=foreignPurchaseItemCost.ForeignPurchaseFollowUpItemsID,
                    ForeignPurchaseItemCostId = foreignPurchaseItemCost.ForeignPurchaseItemCostId,
                    Date = foreignPurchaseItemCost.Date,
                    Payee = foreignPurchaseItemCost.Payee,
                    CostType = foreignPurchaseItemCost.CostType,
                    Amount = foreignPurchaseItemCost.ForexAmount * foreignPurchaseItemCost.ExchangeRate,
                    Currency = foreignPurchaseItemCost.Currency,
                    ExchangeRate = foreignPurchaseItemCost.ExchangeRate,
                    ForexAmount = foreignPurchaseItemCost.ForexAmount,
                    Paid = foreignPurchaseItemCost.Paid,
                    Remark = foreignPurchaseItemCost.Remark,
                    OrgBranchName = foreignPurchaseItemCost.OrgBranchName,
                    DateCreated = foreignPurchaseItemCost.DateCreated,
                    LastModified = foreignPurchaseItemCost.LastModified,
                    CreatedBy = foreignPurchaseItemCost.CreatedBy,
                    ModifiedBy = foreignPurchaseItemCost.ModifiedBy
                };

                _db.ForeignPurchaseItemCosts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Foreign purchase follow up item cost", "Success", User.Identity.Name, $"updated cost {entity.CostType} under record {entity.ForeignPurchaseFollowUpItemsID}", Modules.Procurement);
                #endregion
            }

            return Json(new[] { foreignPurchaseItemCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignPurchaseItemCosts_Destroy([DataSourceRequest]DataSourceRequest request, ForeignPurchaseItemCost foreignPurchaseItemCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignPurchaseItemCost
                {
                    ForeignPurchaseItemCostId = foreignPurchaseItemCost.ForeignPurchaseItemCostId,
                    Date = foreignPurchaseItemCost.Date,
                    Payee = foreignPurchaseItemCost.Payee,
                    CostType = foreignPurchaseItemCost.CostType,
                    Amount = foreignPurchaseItemCost.Amount,
                    Currency = foreignPurchaseItemCost.Currency,
                    ExchangeRate = foreignPurchaseItemCost.ExchangeRate,
                    ForexAmount = foreignPurchaseItemCost.ForexAmount,
                    Paid = foreignPurchaseItemCost.Paid,
                    Remark = foreignPurchaseItemCost.Remark,
                    OrgBranchName = foreignPurchaseItemCost.OrgBranchName,
                    DateCreated = foreignPurchaseItemCost.DateCreated,
                    LastModified = foreignPurchaseItemCost.LastModified,
                    CreatedBy = foreignPurchaseItemCost.CreatedBy,
                    ModifiedBy = foreignPurchaseItemCost.ModifiedBy
                };

                _db.ForeignPurchaseItemCosts.Attach(entity);
                _db.ForeignPurchaseItemCosts.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { foreignPurchaseItemCost }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
