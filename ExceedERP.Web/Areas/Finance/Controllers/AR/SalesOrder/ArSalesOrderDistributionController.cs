using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.SalesOrder
{
    public class ArSalesOrderDistributionController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ArSalesOrderDistributions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArSalesOrderDistribution> arsalesorderdistributions = _db.ArSalesOrderDistributions;
            DataSourceResult result = arsalesorderdistributions.ToDataSourceResult(request, arSalesOrderDistribution => new {
                ArSalesOrderDistributionId = arSalesOrderDistribution.ArSalesOrderDistributionId,
                AccountCode = arSalesOrderDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(arSalesOrderDistribution.AccountCode),
                Debit = arSalesOrderDistribution.Debit,
                Credit = arSalesOrderDistribution.Credit,
                OrgBranchName = arSalesOrderDistribution.OrgBranchName,
                DateCreated = arSalesOrderDistribution.DateCreated,
                LastModified = arSalesOrderDistribution.LastModified,
                CreatedBy = arSalesOrderDistribution.CreatedBy,
                ModifiedBy = arSalesOrderDistribution.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderDistributions_Create([DataSourceRequest]DataSourceRequest request, ArSalesOrderDistribution arSalesOrderDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderDistribution
                {
                    AccountCode = arSalesOrderDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(arSalesOrderDistribution.AccountCode),
                    Debit = arSalesOrderDistribution.Debit,
                    Credit = arSalesOrderDistribution.Credit,
                    OrgBranchName = arSalesOrderDistribution.OrgBranchName,
                    DateCreated = arSalesOrderDistribution.DateCreated,
                    LastModified = arSalesOrderDistribution.LastModified,
                    CreatedBy = arSalesOrderDistribution.CreatedBy,
                    ModifiedBy = arSalesOrderDistribution.ModifiedBy
                };

                _db.ArSalesOrderDistributions.Add(entity);
                _db.SaveChanges();
                arSalesOrderDistribution.ArSalesOrderDistributionId = entity.ArSalesOrderDistributionId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order distribution", "Success", User.Identity.Name, $"created sales order distribution {entity.AccountCode} under record {entity.ArSalesOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrderDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderDistributions_Update([DataSourceRequest]DataSourceRequest request, ArSalesOrderDistribution arSalesOrderDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderDistribution
                {
                    ArSalesOrderDistributionId = arSalesOrderDistribution.ArSalesOrderDistributionId,
                    AccountCode = arSalesOrderDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(arSalesOrderDistribution.AccountCode),
                    Debit = arSalesOrderDistribution.Debit,
                    Credit = arSalesOrderDistribution.Credit,
                    OrgBranchName = arSalesOrderDistribution.OrgBranchName,
                    DateCreated = arSalesOrderDistribution.DateCreated,
                    LastModified = arSalesOrderDistribution.LastModified,
                    CreatedBy = arSalesOrderDistribution.CreatedBy,
                    ModifiedBy = arSalesOrderDistribution.ModifiedBy
                };

                _db.ArSalesOrderDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR sales order distribution", "Success", User.Identity.Name, $"updated sales order distribution {entity.AccountCode} under record {entity.ArSalesOrderId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { arSalesOrderDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ArSalesOrderDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ArSalesOrderDistribution arSalesOrderDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArSalesOrderDistribution
                {
                    ArSalesOrderDistributionId = arSalesOrderDistribution.ArSalesOrderDistributionId,
                    AccountCode = arSalesOrderDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(arSalesOrderDistribution.AccountCode),
                    Debit = arSalesOrderDistribution.Debit,
                    Credit = arSalesOrderDistribution.Credit,
                    OrgBranchName = arSalesOrderDistribution.OrgBranchName,
                    DateCreated = arSalesOrderDistribution.DateCreated,
                    LastModified = arSalesOrderDistribution.LastModified,
                    CreatedBy = arSalesOrderDistribution.CreatedBy,
                    ModifiedBy = arSalesOrderDistribution.ModifiedBy
                };

                _db.ArSalesOrderDistributions.Attach(entity);
                _db.ArSalesOrderDistributions.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { arSalesOrderDistribution }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
