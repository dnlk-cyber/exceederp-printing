using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.FundTransfer;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AP
{
    public class FundReceiveDistributionController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();


        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayablePayment)]
        public ActionResult FundReceiveDistributions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FundReceiveDistribution> FundReceiveDistributions = _db.FundReceiveDistributions.Where(x=>x.FundReceiveId==id);
            DataSourceResult result = FundReceiveDistributions.ToDataSourceResult(request, FundReceiveDistribution => new {
                FundReceiveDistributionId = FundReceiveDistribution.FundReceiveDistributionId,
                FundReceiveId=id,
                AccountCode = FundReceiveDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(FundReceiveDistribution.AccountCode),
                AdditionalDescription = FundReceiveDistribution.AdditionalDescription,
                Debit = FundReceiveDistribution.Debit,
                Credit = FundReceiveDistribution.Credit,
                OrgBranchName = FundReceiveDistribution.OrgBranchName,
                DateCreated = FundReceiveDistribution.DateCreated,
                LastModified = FundReceiveDistribution.LastModified,
                CreatedBy = FundReceiveDistribution.CreatedBy,
                ModifiedBy = FundReceiveDistribution.ModifiedBy
            });

            return Json(result);
        }
        [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundReceiveDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, FundReceiveDistribution FundReceiveDistribution)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                FundReceive parent =
                    _db.FundReceives.FirstOrDefault(
                        x => x.FundReceiveId == FundReceiveDistribution.FundReceiveId);
                if (parent != null && parent.IsOnlineTransferred)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new FundReceiveDistribution
                    {
                        FundReceiveId = id,
                        AccountCode = FundReceiveDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(FundReceiveDistribution.AccountCode),
                        AdditionalDescription = FundReceiveDistribution.AdditionalDescription,
                        Debit = FundReceiveDistribution.Debit,
                        Credit = FundReceiveDistribution.Credit,
                        OrgBranchName = FundReceiveDistribution.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = FundReceiveDistribution.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = FundReceiveDistribution.ModifiedBy
                    };

                    _db.FundReceiveDistributions.Add(entity);
                    _db.SaveChanges();
                    FundReceiveDistribution.FundReceiveDistributionId =
                        entity.FundReceiveDistributionId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP direct payment distribution", "Success", User.Identity.Name, $"created AP direct payment distribution {entity.AccountCode} under record {entity.FundReceiveId} ", Modules.Finance);
                    #endregion
                }
            }
            return Json(new[] { FundReceiveDistribution }.ToDataSourceResult(request, ModelState));
        }
         [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundReceiveDistributions_Update(int id,[DataSourceRequest]DataSourceRequest request, FundReceiveDistribution FundReceiveDistribution)
        {
             bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                FundReceive parent =
                    _db.FundReceives.FirstOrDefault(
                        x => x.FundReceiveId == FundReceiveDistribution.FundReceiveId);
                if (parent != null && parent.IsOnlineTransferred)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new FundReceiveDistribution
                    {
                        FundReceiveDistributionId =FundReceiveDistribution.FundReceiveDistributionId,
                        FundReceiveId = id,
                        AccountCode = FundReceiveDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(FundReceiveDistribution.AccountCode),
                        AdditionalDescription = FundReceiveDistribution.AdditionalDescription,
                        Debit = FundReceiveDistribution.Debit,
                        Credit = FundReceiveDistribution.Credit,
                        OrgBranchName = FundReceiveDistribution.OrgBranchName,
                        DateCreated = FundReceiveDistribution.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = FundReceiveDistribution.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.FundReceiveDistributions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP direct payment distribution", "Success", User.Identity.Name, $"updated AP direct payment distribution {entity.AccountCode} under record {entity.FundReceiveId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { FundReceiveDistribution }.ToDataSourceResult(request, ModelState));
        }

          [AuthorizeRoles(FinanceConstants.AccountPayablePayment)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FundReceiveDistributions_Destroy([DataSourceRequest]DataSourceRequest request, FundReceiveDistribution FundReceiveDistribution)
        {
             bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                FundReceive parent =
                    _db.FundReceives.FirstOrDefault(
                        x => x.FundReceiveId == FundReceiveDistribution.FundReceiveId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new FundReceiveDistribution
                    {
                        FundReceiveDistributionId =
                            FundReceiveDistribution.FundReceiveDistributionId,
                        FundReceiveId = FundReceiveDistribution.FundReceiveId,
                        AccountCode = FundReceiveDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(FundReceiveDistribution.AccountCode),
                        Debit = FundReceiveDistribution.Debit,
                        Credit = FundReceiveDistribution.Credit,
                        OrgBranchName = FundReceiveDistribution.OrgBranchName,
                        DateCreated = FundReceiveDistribution.DateCreated,
                        LastModified = FundReceiveDistribution.LastModified,
                        CreatedBy = FundReceiveDistribution.CreatedBy,
                        ModifiedBy = FundReceiveDistribution.ModifiedBy
                    };

                    _db.FundReceiveDistributions.Attach(entity);
                    _db.FundReceiveDistributions.Remove(entity);
                    _db.SaveChanges();
                }
            }
            return Json(new[] { FundReceiveDistribution }.ToDataSourceResult(request, ModelState));
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
