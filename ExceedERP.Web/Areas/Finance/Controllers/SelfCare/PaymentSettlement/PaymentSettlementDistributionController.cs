using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class PaymentSettlementDistributionController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();


        public ActionResult Index()
        {
            return View();
        }

     
        public ActionResult ApPaymentSettlementDistributions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPaymentSettlementDistribution> ApPaymentSettlementDistributions = _db.ApPaymentSettlementDistributions.Where(x=>x.ApPaymentSettlementId==id);
            DataSourceResult result = ApPaymentSettlementDistributions.ToDataSourceResult(request, ApPaymentSettlementDistribution => new {
                ApPaymentSettlementDistributionId = ApPaymentSettlementDistribution.ApPaymentSettlementDistributionId,
                ApPaymentSettlementId=id,
                AccountCode = ApPaymentSettlementDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(ApPaymentSettlementDistribution.AccountCode),
                AdditionalDescription = ApPaymentSettlementDistribution.AdditionalDescription,
                Debit = ApPaymentSettlementDistribution.Debit,
                Credit = ApPaymentSettlementDistribution.Credit,
                OrgBranchName = ApPaymentSettlementDistribution.OrgBranchName,
                DateCreated = ApPaymentSettlementDistribution.DateCreated,
                LastModified = ApPaymentSettlementDistribution.LastModified,
                CreatedBy = ApPaymentSettlementDistribution.CreatedBy,
                ModifiedBy = ApPaymentSettlementDistribution.ModifiedBy
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ApPaymentSettlementDistribution ApPaymentSettlementDistribution)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                var parent =
                    _db.ApPaymentSettlements.FirstOrDefault(
                        x => x.ApPaymentSettlementId == ApPaymentSettlementDistribution.ApPaymentSettlementId);
                if (parent != null && parent.IsOnlineTransferred)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ApPaymentSettlementDistribution
                    {
                        ApPaymentSettlementId = id,
                        AccountCode = ApPaymentSettlementDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(ApPaymentSettlementDistribution.AccountCode),
                        AdditionalDescription = ApPaymentSettlementDistribution.AdditionalDescription,
                        Debit = ApPaymentSettlementDistribution.Debit,
                        Credit = ApPaymentSettlementDistribution.Credit,
                        OrgBranchName = ApPaymentSettlementDistribution.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = ApPaymentSettlementDistribution.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = ApPaymentSettlementDistribution.ModifiedBy
                    };

                    _db.ApPaymentSettlementDistributions.Add(entity);
                    _db.SaveChanges();
                    ApPaymentSettlementDistribution.ApPaymentSettlementDistributionId =
                        entity.ApPaymentSettlementDistributionId;
                }
            }
            return Json(new[] { ApPaymentSettlementDistribution }.ToDataSourceResult(request, ModelState));
        }
         
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementDistributions_Update(int id,[DataSourceRequest]DataSourceRequest request, ApPaymentSettlementDistribution ApPaymentSettlementDistribution)
        {
             bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                var parent =
                    _db.ApPaymentSettlements.FirstOrDefault(
                        x => x.ApPaymentSettlementId == ApPaymentSettlementDistribution.ApPaymentSettlementId);
                if (parent != null && parent.IsOnlineTransferred)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ApPaymentSettlementDistribution
                    {
                        ApPaymentSettlementDistributionId =ApPaymentSettlementDistribution.ApPaymentSettlementDistributionId,
                        ApPaymentSettlementId = id,
                        AccountCode = ApPaymentSettlementDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(ApPaymentSettlementDistribution.AccountCode),
                        AdditionalDescription = ApPaymentSettlementDistribution.AdditionalDescription,
                        Debit = ApPaymentSettlementDistribution.Debit,
                        Credit = ApPaymentSettlementDistribution.Credit,
                        OrgBranchName = ApPaymentSettlementDistribution.OrgBranchName,
                        DateCreated = ApPaymentSettlementDistribution.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = ApPaymentSettlementDistribution.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ApPaymentSettlementDistributions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return Json(new[] { ApPaymentSettlementDistribution }.ToDataSourceResult(request, ModelState));
        }

       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApPaymentSettlementDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ApPaymentSettlementDistribution ApPaymentSettlementDistribution)
        {
             bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                var parent =
                    _db.ApPaymentSettlements.FirstOrDefault(
                        x => x.ApPaymentSettlementId == ApPaymentSettlementDistribution.ApPaymentSettlementId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ApPaymentSettlementDistribution
                    {
                        ApPaymentSettlementDistributionId =
                            ApPaymentSettlementDistribution.ApPaymentSettlementDistributionId,
                        ApPaymentSettlementId = ApPaymentSettlementDistribution.ApPaymentSettlementId,
                        AccountCode = ApPaymentSettlementDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(ApPaymentSettlementDistribution.AccountCode),
                        Debit = ApPaymentSettlementDistribution.Debit,
                        Credit = ApPaymentSettlementDistribution.Credit,
                        OrgBranchName = ApPaymentSettlementDistribution.OrgBranchName,
                        DateCreated = ApPaymentSettlementDistribution.DateCreated,
                        LastModified = ApPaymentSettlementDistribution.LastModified,
                        CreatedBy = ApPaymentSettlementDistribution.CreatedBy,
                        ModifiedBy = ApPaymentSettlementDistribution.ModifiedBy
                    };

                    _db.ApPaymentSettlementDistributions.Attach(entity);
                    _db.ApPaymentSettlementDistributions.Remove(entity);
                    _db.SaveChanges();
                }
            }
            return Json(new[] { ApPaymentSettlementDistribution }.ToDataSourceResult(request, ModelState));
        }

       
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableSettlement)]
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
