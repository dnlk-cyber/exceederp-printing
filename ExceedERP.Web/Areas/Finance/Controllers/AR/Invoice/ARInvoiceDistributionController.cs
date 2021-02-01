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

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Invoice
{
    public class ArInvoiceDistributionController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();

        [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover, FinanceConstants.AccountReceivableInvoice)]
        public ActionResult Index()
        {
            return View();
        }

         [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover, FinanceConstants.AccountReceivableInvoice)]
        public ActionResult ARInvoiceDistributions_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArInvoiceDistribution> arinvoicedistributions = _db.ARInvoiceDistributions.Where(x=>x.ArInvoiceId==id);
            DataSourceResult result = arinvoicedistributions.ToDataSourceResult(request, aRInvoiceDistribution => new
            {
                ArInvoiceDistributionId = aRInvoiceDistribution.ArInvoiceDistributionId,
                ArInvoiceId = aRInvoiceDistribution.ArInvoiceId,
                AccountCode = aRInvoiceDistribution.AccountCode,
                AccountDesc = _accountHelper.GetAccountDesc(aRInvoiceDistribution.AccountCode),
                Debit = aRInvoiceDistribution.Debit,
                Credit = aRInvoiceDistribution.Credit,
                OrgBranchName = aRInvoiceDistribution.OrgBranchName,
                DateCreated = aRInvoiceDistribution.DateCreated,
                LastModified = aRInvoiceDistribution.LastModified,
                CreatedBy = aRInvoiceDistribution.CreatedBy,
                ModifiedBy = aRInvoiceDistribution.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles( FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoiceDistributions_Create(int id, [DataSourceRequest]DataSourceRequest request, ArInvoiceDistribution aRInvoiceDistribution)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ArInvoice parent =
                    _db.ARInvoices.FirstOrDefault(
                        x => x.ArInvoiceId == aRInvoiceDistribution.ArInvoiceId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ArInvoiceDistribution
                    {
                        ArInvoiceId = id,
                        AccountCode = aRInvoiceDistribution.AccountCode,
                        AccountDesc = aRInvoiceDistribution.AccountDesc,
                        Debit = aRInvoiceDistribution.Debit,
                        Credit = aRInvoiceDistribution.Credit,
                        OrgBranchName = aRInvoiceDistribution.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aRInvoiceDistribution.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aRInvoiceDistribution.ModifiedBy
                    };

                    _db.ARInvoiceDistributions.Add(entity);
                    _db.SaveChanges();
                    aRInvoiceDistribution.ArInvoiceDistributionId = entity.ArInvoiceDistributionId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AR invoice distribution", "Success", User.Identity.Name, $"created invoice distribution {entity.AccountCode} under record {entity.ArInvoiceId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { aRInvoiceDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles( FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoiceDistributions_Update(int id, [DataSourceRequest]DataSourceRequest request, ArInvoiceDistribution aRInvoiceDistribution)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ArInvoice parent =
                    _db.ARInvoices.FirstOrDefault(
                        x => x.ArInvoiceId == aRInvoiceDistribution.ArInvoiceId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ArInvoiceDistribution
                    {
                        ArInvoiceId = id,
                        ArInvoiceDistributionId = aRInvoiceDistribution.ArInvoiceDistributionId,
                        AccountCode = aRInvoiceDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(aRInvoiceDistribution.AccountCode),
                        Debit = aRInvoiceDistribution.Debit,
                        Credit = aRInvoiceDistribution.Credit,
                        OrgBranchName = aRInvoiceDistribution.OrgBranchName,
                        DateCreated = aRInvoiceDistribution.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aRInvoiceDistribution.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ARInvoiceDistributions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AR invoice distribution", "Success", User.Identity.Name, $"updated invoice distribution {entity.AccountCode} under record {entity.ArInvoiceId} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { aRInvoiceDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles( FinanceConstants.AccountReceivableInvoice)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARInvoiceDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ArInvoiceDistribution aRInvoiceDistribution)
        {
            bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ArInvoice parent =
                    _db.ARInvoices.FirstOrDefault(
                        x => x.ArInvoiceId == aRInvoiceDistribution.ArInvoiceId);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ArInvoiceDistribution
                    {
                        ArInvoiceDistributionId = aRInvoiceDistribution.ArInvoiceDistributionId,
                        AccountCode = aRInvoiceDistribution.AccountCode,
                        AccountDesc = aRInvoiceDistribution.AccountDesc,
                        Debit = aRInvoiceDistribution.Debit,
                        Credit = aRInvoiceDistribution.Credit,
                        OrgBranchName = aRInvoiceDistribution.OrgBranchName,
                        DateCreated = aRInvoiceDistribution.DateCreated,
                        LastModified = aRInvoiceDistribution.LastModified,
                        CreatedBy = aRInvoiceDistribution.CreatedBy,
                        ModifiedBy = aRInvoiceDistribution.ModifiedBy
                    };

                    _db.ARInvoiceDistributions.Attach(entity);
                    _db.ARInvoiceDistributions.Remove(entity);
                    _db.SaveChanges();
                }
            }

            return Json(new[] { aRInvoiceDistribution }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover, FinanceConstants.AccountReceivableInvoice)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

         [AuthorizeRoles(FinanceConstants.AccountReceivableView , FinanceConstants.AccountReceivableApprover, FinanceConstants.AccountReceivableInvoice)]
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
