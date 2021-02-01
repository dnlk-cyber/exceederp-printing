using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.StandardCollection
{
    public class ArStandardCollectionDistributionController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();


        [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover,FinanceConstants.AccountReceivableCollection)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover,FinanceConstants.AccountReceivableCollection)]
        public ActionResult ARStandardCollectionDistributions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArStandardCollectionDistribution> arstandardcollectiondistributions = _db.ARStandardCollectionDistributions.Where(x=>x.ArStandardCollectionId==id);
            DataSourceResult result = arstandardcollectiondistributions.ToDataSourceResult(request, aRStandardCollectionDistribution => new
            {
                ArStandardCollectionDistributionId = aRStandardCollectionDistribution.ArStandardCollectionDistributionId,
                ArStandardCollectionId = id,
                AccountCode = aRStandardCollectionDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(aRStandardCollectionDistribution.AccountCode),
                AdditionalDescription = aRStandardCollectionDistribution.AdditionalDescription,
                Debit = aRStandardCollectionDistribution.Debit,
                Credit = aRStandardCollectionDistribution.Credit,
                OrgBranchName = aRStandardCollectionDistribution.OrgBranchName,
                DateCreated = aRStandardCollectionDistribution.DateCreated,
                LastModified = aRStandardCollectionDistribution.LastModified,
                CreatedBy = aRStandardCollectionDistribution.CreatedBy,
                ModifiedBy = aRStandardCollectionDistribution.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollectionDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ArStandardCollectionDistribution aRStandardCollectionDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArStandardCollectionDistribution
                {
                    ArStandardCollectionId = id,
                    AccountCode = aRStandardCollectionDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(aRStandardCollectionDistribution.AccountCode),
                    AdditionalDescription = aRStandardCollectionDistribution.AdditionalDescription,
                    Debit = aRStandardCollectionDistribution.Debit,
                    Credit = aRStandardCollectionDistribution.Credit,
                    OrgBranchName = aRStandardCollectionDistribution.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = aRStandardCollectionDistribution.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRStandardCollectionDistribution.ModifiedBy
                };

                _db.ARStandardCollectionDistributions.Add(entity);
                _db.SaveChanges();
                aRStandardCollectionDistribution.ArStandardCollectionDistributionId = entity.ArStandardCollectionDistributionId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR standard collection distribution", "Success", User.Identity.Name, $"created standard collection distribution {entity.AccountCode} under record {entity.ArStandardCollectionId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRStandardCollectionDistribution }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollectionDistributions_Update(int id,[DataSourceRequest]DataSourceRequest request, ArStandardCollectionDistribution aRStandardCollectionDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArStandardCollectionDistribution
                {
                    ArStandardCollectionId = id,
                    ArStandardCollectionDistributionId = aRStandardCollectionDistribution.ArStandardCollectionDistributionId,
                    AccountCode = aRStandardCollectionDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(aRStandardCollectionDistribution.AccountCode),
                    AdditionalDescription = aRStandardCollectionDistribution.AdditionalDescription,
                    Debit = aRStandardCollectionDistribution.Debit,
                    Credit = aRStandardCollectionDistribution.Credit,
                    OrgBranchName = aRStandardCollectionDistribution.OrgBranchName,
                    DateCreated = aRStandardCollectionDistribution.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aRStandardCollectionDistribution.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ARStandardCollectionDistributions.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR standard collection distribution", "Success", User.Identity.Name, $"updated standard collection distribution {entity.AccountCode} under record {entity.ArStandardCollectionId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRStandardCollectionDistribution }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollectionDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ArStandardCollectionDistribution aRStandardCollectionDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArStandardCollectionDistribution
                {
                    ArStandardCollectionDistributionId = aRStandardCollectionDistribution.ArStandardCollectionDistributionId,
                    AccountCode = aRStandardCollectionDistribution.AccountCode,
                    AccountDesc = _accountHelper.GetAccountDesc(aRStandardCollectionDistribution.AccountCode),
                    Debit = aRStandardCollectionDistribution.Debit,
                    Credit = aRStandardCollectionDistribution.Credit,
                    OrgBranchName = aRStandardCollectionDistribution.OrgBranchName,
                    DateCreated = aRStandardCollectionDistribution.DateCreated,
                    LastModified = aRStandardCollectionDistribution.LastModified,
                    CreatedBy = aRStandardCollectionDistribution.CreatedBy,
                    ModifiedBy = aRStandardCollectionDistribution.ModifiedBy
                };

                _db.ARStandardCollectionDistributions.Attach(entity);
                _db.ARStandardCollectionDistributions.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { aRStandardCollectionDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover,FinanceConstants.AccountReceivableCollection)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover,FinanceConstants.AccountReceivableCollection)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
