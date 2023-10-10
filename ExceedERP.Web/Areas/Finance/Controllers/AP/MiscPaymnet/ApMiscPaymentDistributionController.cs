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
    public class ApMiscPaymentDistributionController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();


        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        public ActionResult APMiscellaneousPaymentDistributions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApMiscellaneousPaymentDistribution> apmiscellaneouspaymentdistributions = _db.APMiscellaneousPaymentDistributions.Where(x=>x.ApMiscellaneousPaymentId==id);
            DataSourceResult result = apmiscellaneouspaymentdistributions.ToDataSourceResult(request, aPMiscellaneousPaymentDistribution => new {
                ApMiscellaneousPaymentDistributionId = aPMiscellaneousPaymentDistribution.ApMiscellaneousPaymentDistributionId,
                ApMiscellaneousPaymentId=id,
                AccountCode = aPMiscellaneousPaymentDistribution.AccountCode,
                 AccountDesc = _accountHelper.GetAccountDesc(aPMiscellaneousPaymentDistribution.AccountCode),
                AdditionalDescription = aPMiscellaneousPaymentDistribution.AdditionalDescription,
                Debit = aPMiscellaneousPaymentDistribution.Debit,
                Credit = aPMiscellaneousPaymentDistribution.Credit,
                OrgBranchName = aPMiscellaneousPaymentDistribution.OrgBranchName,
                DateCreated = aPMiscellaneousPaymentDistribution.DateCreated,
                LastModified = aPMiscellaneousPaymentDistribution.LastModified,
                CreatedBy = aPMiscellaneousPaymentDistribution.CreatedBy,
                ModifiedBy = aPMiscellaneousPaymentDistribution.ModifiedBy
            });

            return Json(result);
        }
        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APMiscellaneousPaymentDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ApMiscellaneousPaymentDistribution aPMiscellaneousPaymentDistribution)
        {
            #region validated record

            var parent =
                _db.APMiscellaneousPayments.FirstOrDefault(
                    x => x.ApMiscellaneousPaymentId ==id);
            if (parent != null && parent.IsOnlineTransferred)
                ModelState.AddModelError("approved", "You cannot manipulate this record, it is already transferred");


            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new ApMiscellaneousPaymentDistribution
                    {
                        ApMiscellaneousPaymentId = id,
                        AccountCode = aPMiscellaneousPaymentDistribution.AccountCode,
                         AccountDesc = _accountHelper.GetAccountDesc(aPMiscellaneousPaymentDistribution.AccountCode),
                        AdditionalDescription = aPMiscellaneousPaymentDistribution.AdditionalDescription,
                        Debit = aPMiscellaneousPaymentDistribution.Debit,
                        Credit = aPMiscellaneousPaymentDistribution.Credit,
                        OrgBranchName = aPMiscellaneousPaymentDistribution.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aPMiscellaneousPaymentDistribution.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aPMiscellaneousPaymentDistribution.ModifiedBy
                    };

                    _db.APMiscellaneousPaymentDistributions.Add(entity);
                    _db.SaveChanges();
                    aPMiscellaneousPaymentDistribution.ApMiscellaneousPaymentDistributionId =
                        entity.ApMiscellaneousPaymentDistributionId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP misc payment distribution", "Success", User.Identity.Name, $"updated AP misc payment distribution {entity.AccountCode} under record {entity.ApMiscellaneousPaymentId} ", Modules.Finance);
                    #endregion
                
            }
            return Json(new[] { aPMiscellaneousPaymentDistribution }.ToDataSourceResult(request, ModelState));
        }
         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APMiscellaneousPaymentDistributions_Update(int id,[DataSourceRequest]DataSourceRequest request, ApMiscellaneousPaymentDistribution aPMiscellaneousPaymentDistribution)
        {
            #region operation log
            var parent =
                _db.APMiscellaneousPayments.FirstOrDefault(
                    x => x.ApMiscellaneousPaymentId == aPMiscellaneousPaymentDistribution.ApMiscellaneousPaymentId);
            if (parent != null && parent.IsOnlineTransferred)
                ModelState.AddModelError("approved", "You cannot manipulate this record, it is already transferred");
            #endregion

            if (ModelState.IsValid)
            {
              
                    var entity = new ApMiscellaneousPaymentDistribution
                    {
                        ApMiscellaneousPaymentDistributionId =aPMiscellaneousPaymentDistribution.ApMiscellaneousPaymentDistributionId,
                        ApMiscellaneousPaymentId = id,
                        AccountCode = aPMiscellaneousPaymentDistribution.AccountCode,
                         AccountDesc = _accountHelper.GetAccountDesc(aPMiscellaneousPaymentDistribution.AccountCode),
                        AdditionalDescription = aPMiscellaneousPaymentDistribution.AdditionalDescription,
                        Debit = aPMiscellaneousPaymentDistribution.Debit,
                        Credit = aPMiscellaneousPaymentDistribution.Credit,
                        OrgBranchName = aPMiscellaneousPaymentDistribution.OrgBranchName,
                        DateCreated = aPMiscellaneousPaymentDistribution.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aPMiscellaneousPaymentDistribution.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.APMiscellaneousPaymentDistributions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                
            }

            return Json(new[] { aPMiscellaneousPaymentDistribution }.ToDataSourceResult(request, ModelState));
        }

          [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APMiscellaneousPaymentDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ApMiscellaneousPaymentDistribution aPMiscellaneousPaymentDistribution)
        {
             bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ApMiscellaneousPayment parent =
                    _db.APMiscellaneousPayments.FirstOrDefault(
                        x => x.ApMiscellaneousPaymentId == aPMiscellaneousPaymentDistribution.ApMiscellaneousPaymentId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ApMiscellaneousPaymentDistribution
                    {
                        ApMiscellaneousPaymentDistributionId =
                            aPMiscellaneousPaymentDistribution.ApMiscellaneousPaymentDistributionId,
                        ApMiscellaneousPaymentId = aPMiscellaneousPaymentDistribution.ApMiscellaneousPaymentId,
                        AccountCode = aPMiscellaneousPaymentDistribution.AccountCode,
                         AccountDesc = _accountHelper.GetAccountDesc(aPMiscellaneousPaymentDistribution.AccountCode),
                        Debit = aPMiscellaneousPaymentDistribution.Debit,
                        Credit = aPMiscellaneousPaymentDistribution.Credit,
                        OrgBranchName = aPMiscellaneousPaymentDistribution.OrgBranchName,
                        DateCreated = aPMiscellaneousPaymentDistribution.DateCreated,
                        LastModified = aPMiscellaneousPaymentDistribution.LastModified,
                        CreatedBy = aPMiscellaneousPaymentDistribution.CreatedBy,
                        ModifiedBy = aPMiscellaneousPaymentDistribution.ModifiedBy
                    };

                    _db.APMiscellaneousPaymentDistributions.Attach(entity);
                    _db.APMiscellaneousPaymentDistributions.Remove(entity);
                    _db.SaveChanges();
                }
            }
            return Json(new[] { aPMiscellaneousPaymentDistribution }.ToDataSourceResult(request, ModelState));
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
