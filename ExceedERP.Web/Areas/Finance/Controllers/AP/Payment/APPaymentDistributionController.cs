using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class ApPaymentDistributionController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        public ActionResult Index()
        {
            return View();
        }

       [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        public ActionResult APPaymentDistributions_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPaymentDistribution> appaymentdistributions = _db.APPaymentDistributions.Where(x => x.ApPaymentId == id);
            DataSourceResult result = appaymentdistributions.ToDataSourceResult(request, aPPaymentDistribution => new
            {
                ApPaymentDistributionId = aPPaymentDistribution.ApPaymentDistributionId,
                ApPaymentId = id,
                AccountCode = aPPaymentDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(aPPaymentDistribution.AccountCode),
                AdditionalDescription = aPPaymentDistribution.AdditionalDescription,
                Debit = aPPaymentDistribution.Debit,
                Credit = aPPaymentDistribution.Credit,
                OrgBranchName = aPPaymentDistribution.OrgBranchName,
                DateCreated = aPPaymentDistribution.DateCreated,
                LastModified = aPPaymentDistribution.LastModified,
                CreatedBy = aPPaymentDistribution.CreatedBy,
                ModifiedBy = aPPaymentDistribution.ModifiedBy
            });

            return Json(result);
        }

       [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaymentDistributions_Create(int id, [DataSourceRequest]DataSourceRequest request, ApPaymentDistribution aPPaymentDistribution)
        {
            #region validated record

            ApPayment parent =
                _db.APPayments.FirstOrDefault(
                    x => x.ApPaymentId == aPPaymentDistribution.ApPaymentId);
            if (parent != null && parent.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record, it is already approved");


            #endregion
            if (ModelState.IsValid)
            {
               
                    var entity = new ApPaymentDistribution
                    {
                        ApPaymentId = id,
                        AccountCode = aPPaymentDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aPPaymentDistribution.AccountCode),
                        AdditionalDescription = aPPaymentDistribution.AdditionalDescription,
                        Debit = aPPaymentDistribution.Debit,
                        Credit = aPPaymentDistribution.Credit,
                        OrgBranchName = aPPaymentDistribution.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aPPaymentDistribution.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aPPaymentDistribution.ModifiedBy
                    };

                    _db.APPaymentDistributions.Add(entity);
                    _db.SaveChanges();
                    aPPaymentDistribution.ApPaymentDistributionId = entity.ApPaymentDistributionId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP payment distribution", "Success", User.Identity.Name, $"created AP payment distribution {entity.AccountCode} under record {entity.ApPaymentId} ", Modules.Finance);
                    #endregion
                
            }
            return Json(new[] { aPPaymentDistribution }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaymentDistributions_Update(int id, [DataSourceRequest]DataSourceRequest request, ApPaymentDistribution aPPaymentDistribution)
        {
            #region validated record

            ApPayment parent =
                _db.APPayments.FirstOrDefault(
                    x => x.ApPaymentId == aPPaymentDistribution.ApPaymentId);
            if (parent != null && parent.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record, it is already approved");
          

            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApPaymentDistribution
                    {
                        ApPaymentId = id,
                        ApPaymentDistributionId = aPPaymentDistribution.ApPaymentDistributionId,
                        AccountCode = aPPaymentDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aPPaymentDistribution.AccountCode),
                        AdditionalDescription = aPPaymentDistribution.AdditionalDescription,
                        Debit = aPPaymentDistribution.Debit,
                        Credit = aPPaymentDistribution.Credit,
                        OrgBranchName = aPPaymentDistribution.OrgBranchName,
                        DateCreated = aPPaymentDistribution.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aPPaymentDistribution.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.APPaymentDistributions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP payment distribution", "Success", User.Identity.Name, $"updated AP payment distribution {entity.AccountCode} under record {entity.ApPaymentId} ", Modules.Finance);
                    #endregion
                
            }

            return Json(new[] { aPPaymentDistribution }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaymentDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ApPaymentDistribution aPPaymentDistribution)
        {
            #region validated record

            ApPayment parent =
                _db.APPayments.FirstOrDefault(
                    x => x.ApPaymentId == aPPaymentDistribution.ApPaymentId);
            if (parent != null && parent.IsOnlineApproved)
                ModelState.AddModelError("approved", "You cannot manipulate this record, it is already approved");


            #endregion
            if (ModelState.IsValid)
            {
              
                    var entity = new ApPaymentDistribution
                    {
                        ApPaymentDistributionId = aPPaymentDistribution.ApPaymentDistributionId,
                        AccountCode = aPPaymentDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aPPaymentDistribution.AccountCode),
                        Debit = aPPaymentDistribution.Debit,
                        Credit = aPPaymentDistribution.Credit,
                        OrgBranchName = aPPaymentDistribution.OrgBranchName,
                        DateCreated = aPPaymentDistribution.DateCreated,
                        LastModified = aPPaymentDistribution.LastModified,
                        CreatedBy = aPPaymentDistribution.CreatedBy,
                        ModifiedBy = aPPaymentDistribution.ModifiedBy
                    };

                    _db.APPaymentDistributions.Attach(entity);
                    _db.APPaymentDistributions.Remove(entity);
                    _db.SaveChanges();
                
            }

            return Json(new[] { aPPaymentDistribution }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

       [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
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
