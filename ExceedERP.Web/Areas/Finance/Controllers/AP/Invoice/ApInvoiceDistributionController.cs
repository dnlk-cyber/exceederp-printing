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
    public class ApInvoiceDistributionController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();


        [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableInvoice)]
        public ActionResult Index()
        {
            return View();
        }

       [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableInvoice)]
        public ActionResult APPaybleInvoiceDistributions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApPaybleInvoiceDistribution> appaybleinvoicedistributions = _db.APPaybleInvoiceDistributions.Where(x=>x.ApPaybleInvoicesId==id);
            DataSourceResult result = appaybleinvoicedistributions.ToDataSourceResult(request, aPPaybleInvoiceDistribution => new {
                ApPaybleInvoiceDistributionId = aPPaybleInvoiceDistribution.ApPaybleInvoiceDistributionId,
                ApPaybleInvoicesId=id,
                AccountCode = aPPaybleInvoiceDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(aPPaybleInvoiceDistribution.AccountCode),
                Debit = aPPaybleInvoiceDistribution.Debit,
                Credit = aPPaybleInvoiceDistribution.Credit,
                OrgBranchName = aPPaybleInvoiceDistribution.OrgBranchName,
                DateCreated = aPPaybleInvoiceDistribution.DateCreated,
                LastModified = aPPaybleInvoiceDistribution.LastModified,
                CreatedBy = aPPaybleInvoiceDistribution.CreatedBy,
                ModifiedBy = aPPaybleInvoiceDistribution.ModifiedBy
            });

            return Json(result);
        }

       [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoiceDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ApPaybleInvoiceDistribution aPPaybleInvoiceDistribution)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ApPaybleInvoices parent =
                    _db.APPaybleInvoices.FirstOrDefault(
                        x => x.ApPaybleInvoicesId == aPPaybleInvoiceDistribution.ApPaybleInvoicesId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ApPaybleInvoiceDistribution
                    {
                        ApPaybleInvoicesId = id,
                        AccountCode = aPPaybleInvoiceDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aPPaybleInvoiceDistribution.AccountCode),
                        Debit = aPPaybleInvoiceDistribution.Debit,
                        Credit = aPPaybleInvoiceDistribution.Credit,
                        OrgBranchName = aPPaybleInvoiceDistribution.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aPPaybleInvoiceDistribution.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aPPaybleInvoiceDistribution.ModifiedBy
                    };

                    _db.APPaybleInvoiceDistributions.Add(entity);
                    _db.SaveChanges();
                    aPPaybleInvoiceDistribution.ApPaybleInvoiceDistributionId = entity.ApPaybleInvoiceDistributionId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP invoice distribution", "Success", User.Identity.Name, $"updated AP invoice distribution {entity.AccountCode} under record {entity.ApPaybleInvoicesId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { aPPaybleInvoiceDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoiceDistributions_Update(int id,[DataSourceRequest]DataSourceRequest request, ApPaybleInvoiceDistribution aPPaybleInvoiceDistribution)
        {
           bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ApPaybleInvoices parent =
                    _db.APPaybleInvoices.FirstOrDefault(
                        x => x.ApPaybleInvoicesId == aPPaybleInvoiceDistribution.ApPaybleInvoicesId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ApPaybleInvoiceDistribution
                    {
                        ApPaybleInvoicesId = id,
                        ApPaybleInvoiceDistributionId = aPPaybleInvoiceDistribution.ApPaybleInvoiceDistributionId,
                        AccountCode = aPPaybleInvoiceDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aPPaybleInvoiceDistribution.AccountCode),
                        Debit = aPPaybleInvoiceDistribution.Debit,
                        Credit = aPPaybleInvoiceDistribution.Credit,
                        OrgBranchName = aPPaybleInvoiceDistribution.OrgBranchName,
                        DateCreated = aPPaybleInvoiceDistribution.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aPPaybleInvoiceDistribution.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.APPaybleInvoiceDistributions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AP invoice distribution", "Success", User.Identity.Name, $"updated AP invoice distribution {entity.AccountCode} under record {entity.ApPaybleInvoicesId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { aPPaybleInvoiceDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountPayableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult APPaybleInvoiceDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ApPaybleInvoiceDistribution aPPaybleInvoiceDistribution)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ApPaybleInvoices parent =
                    _db.APPaybleInvoices.FirstOrDefault(
                        x => x.ApPaybleInvoicesId == aPPaybleInvoiceDistribution.ApPaybleInvoicesId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ApPaybleInvoiceDistribution
                    {
                        ApPaybleInvoiceDistributionId = aPPaybleInvoiceDistribution.ApPaybleInvoiceDistributionId,
                        AccountCode = aPPaybleInvoiceDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aPPaybleInvoiceDistribution.AccountCode),
                        Debit = aPPaybleInvoiceDistribution.Debit,
                        Credit = aPPaybleInvoiceDistribution.Credit,
                        OrgBranchName = aPPaybleInvoiceDistribution.OrgBranchName,
                        DateCreated = aPPaybleInvoiceDistribution.DateCreated,
                        LastModified = aPPaybleInvoiceDistribution.LastModified,
                        CreatedBy = aPPaybleInvoiceDistribution.CreatedBy,
                        ModifiedBy = aPPaybleInvoiceDistribution.ModifiedBy
                    };

                    _db.APPaybleInvoiceDistributions.Attach(entity);
                    _db.APPaybleInvoiceDistributions.Remove(entity);
                    _db.SaveChanges();
                }
            }

            return Json(new[] { aPPaybleInvoiceDistribution }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableInvoice)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

       [AuthorizeRoles(FinanceConstants.AccountPayableView , FinanceConstants.AccountPayableApprover , FinanceConstants.AccountPayableInvoice)]
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
