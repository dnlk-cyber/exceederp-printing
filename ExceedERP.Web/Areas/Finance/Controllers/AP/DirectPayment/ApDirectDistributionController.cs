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
    public class ApDirectPaymentDistributionController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();


        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        public ActionResult APDirectPaymentDistributions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApDirectPaymentDistribution> apDirectpaymentdistributions = _db.ApDirectPaymentDistributions.Where(x=>x.ApDirectPaymentId==id);
            DataSourceResult result = apDirectpaymentdistributions.ToDataSourceResult(request, aPDirectPaymentDistribution => new {
                ApDirectPaymentDistributionId = aPDirectPaymentDistribution.ApDirectPaymentDistributionId,
                ApDirectPaymentId=id,
                AccountCode = aPDirectPaymentDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(aPDirectPaymentDistribution.AccountCode),
                AdditionalDescription = aPDirectPaymentDistribution.AdditionalDescription,
                Debit = aPDirectPaymentDistribution.Debit,
                Credit = aPDirectPaymentDistribution.Credit,
                OrgBranchName = aPDirectPaymentDistribution.OrgBranchName,
                DateCreated = aPDirectPaymentDistribution.DateCreated,
                LastModified = aPDirectPaymentDistribution.LastModified,
                CreatedBy = aPDirectPaymentDistribution.CreatedBy,
                ModifiedBy = aPDirectPaymentDistribution.ModifiedBy
            });

            return Json(result);
        }
        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APDirectPaymentDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ApDirectPaymentDistribution aPDirectPaymentDistribution)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ApDirectPayment parent =
                    _db.ApDirectPayments.FirstOrDefault(
                        x => x.ApDirectPaymentId == aPDirectPaymentDistribution.ApDirectPaymentId);
                if (parent != null && parent.IsOnlineTransferred)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ApDirectPaymentDistribution
                    {
                        ApDirectPaymentId = id,
                        AccountCode = aPDirectPaymentDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aPDirectPaymentDistribution.AccountCode),
                        AdditionalDescription = aPDirectPaymentDistribution.AdditionalDescription,
                        Debit = aPDirectPaymentDistribution.Debit,
                        Credit = aPDirectPaymentDistribution.Credit,
                        OrgBranchName = aPDirectPaymentDistribution.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aPDirectPaymentDistribution.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aPDirectPaymentDistribution.ModifiedBy
                    };

                    _db.ApDirectPaymentDistributions.Add(entity);
                    _db.SaveChanges();
                    aPDirectPaymentDistribution.ApDirectPaymentDistributionId =
                        entity.ApDirectPaymentDistributionId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP direct payment distribution", "Success", User.Identity.Name, $"created AP direct payment distribution {entity.AccountCode} under record {entity.ApDirectPaymentId} ", Modules.Finance);
                    #endregion
                }
            }
            return Json(new[] { aPDirectPaymentDistribution }.ToDataSourceResult(request, ModelState));
        }
         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APDirectPaymentDistributions_Update(int id,[DataSourceRequest]DataSourceRequest request, ApDirectPaymentDistribution aPDirectPaymentDistribution)
        {
             bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ApDirectPayment parent =
                    _db.ApDirectPayments.FirstOrDefault(
                        x => x.ApDirectPaymentId == aPDirectPaymentDistribution.ApDirectPaymentId);
                if (parent != null && parent.IsOnlineTransferred)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ApDirectPaymentDistribution
                    {
                        ApDirectPaymentDistributionId =aPDirectPaymentDistribution.ApDirectPaymentDistributionId,
                        ApDirectPaymentId = id,
                        AccountCode = aPDirectPaymentDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aPDirectPaymentDistribution.AccountCode),
                        AdditionalDescription = aPDirectPaymentDistribution.AdditionalDescription,
                        Debit = aPDirectPaymentDistribution.Debit,
                        Credit = aPDirectPaymentDistribution.Credit,
                        OrgBranchName = aPDirectPaymentDistribution.OrgBranchName,
                        DateCreated = aPDirectPaymentDistribution.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aPDirectPaymentDistribution.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ApDirectPaymentDistributions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP direct payment distribution", "Success", User.Identity.Name, $"updated AP direct payment distribution {entity.AccountCode} under record {entity.ApDirectPaymentId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { aPDirectPaymentDistribution }.ToDataSourceResult(request, ModelState));
        }

          [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APDirectPaymentDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ApDirectPaymentDistribution aPDirectPaymentDistribution)
        {
             bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ApDirectPayment parent =
                    _db.ApDirectPayments.FirstOrDefault(
                        x => x.ApDirectPaymentId == aPDirectPaymentDistribution.ApDirectPaymentId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ApDirectPaymentDistribution
                    {
                        ApDirectPaymentDistributionId =
                            aPDirectPaymentDistribution.ApDirectPaymentDistributionId,
                        ApDirectPaymentId = aPDirectPaymentDistribution.ApDirectPaymentId,
                        AccountCode = aPDirectPaymentDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aPDirectPaymentDistribution.AccountCode),
                        Debit = aPDirectPaymentDistribution.Debit,
                        Credit = aPDirectPaymentDistribution.Credit,
                        OrgBranchName = aPDirectPaymentDistribution.OrgBranchName,
                        DateCreated = aPDirectPaymentDistribution.DateCreated,
                        LastModified = aPDirectPaymentDistribution.LastModified,
                        CreatedBy = aPDirectPaymentDistribution.CreatedBy,
                        ModifiedBy = aPDirectPaymentDistribution.ModifiedBy
                    };

                    _db.ApDirectPaymentDistributions.Attach(entity);
                    _db.ApDirectPaymentDistributions.Remove(entity);
                    _db.SaveChanges();
                }
            }
            return Json(new[] { aPDirectPaymentDistribution }.ToDataSourceResult(request, ModelState));
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
