using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlRecordJournalCurrLinesController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        AccountHelper _accountHelper = new AccountHelper();

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser , FinanceConstants.GeneralLedgerApprover , FinanceConstants.GeneralLedgerView)]
        public ActionResult GLRecordJournalDetails_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<GLRecordJournalDetail> glrecordjournaldetails = _db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == id);
            DataSourceResult result = glrecordjournaldetails.ToDataSourceResult(request, gLRecordJournalDetail => new {
                GlRecordJournalDetailId = gLRecordJournalDetail.GlRecordJournalDetailId,
                GlRecordJournalId = gLRecordJournalDetail.GlRecordJournalId,
                AccountCode = gLRecordJournalDetail.AccountCode,
                AccountDesc = gLRecordJournalDetail.AccountDesc,
                Debit = gLRecordJournalDetail.Debit,
                Credit = gLRecordJournalDetail.Credit,
                ExRate = gLRecordJournalDetail.ExRate,
                DebitNonFun = gLRecordJournalDetail.DebitNonFun,
                CreditNonFun = gLRecordJournalDetail.CreditNonFun,
                AdditionalDescription = gLRecordJournalDetail.AdditionalDescription,
                AccountType = gLRecordJournalDetail.AccountType,
                AccountSegment = gLRecordJournalDetail.AccountSegment,
                BranchSegment = gLRecordJournalDetail.BranchSegment,
                CostCenterSegment = gLRecordJournalDetail.CostCenterSegment,
                SubAccountSegment = gLRecordJournalDetail.SubAccountSegment,
                ChannelSegment = gLRecordJournalDetail.ChannelSegment,
                OrgBranchName = gLRecordJournalDetail.OrgBranchName,
                DateCreated = gLRecordJournalDetail.DateCreated,
                LastModified = gLRecordJournalDetail.LastModified,
                CreatedBy = gLRecordJournalDetail.CreatedBy,
                ModifiedBy = gLRecordJournalDetail.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLRecordJournalDetails_Create(int id, [DataSourceRequest]DataSourceRequest request, GLRecordJournalDetail gLRecordJournalDetail)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Journal First");
            }

            gLRecordJournalDetail.Debit = gLRecordJournalDetail.DebitNonFun * gLRecordJournalDetail.ExRate;
            gLRecordJournalDetail.Credit = gLRecordJournalDetail.CreditNonFun * gLRecordJournalDetail.ExRate;
            #region validated record

            GlRecordJournal parent =
                _db.GLRecordJournals.FirstOrDefault(x => x.GlRecordJournalId == gLRecordJournalDetail.GlRecordJournalId);

            if (parent != null && parent.IsOnlineApproved)
                ModelState.AddModelError("approved", "Record is already approved,hence you can not add a record.");
            if (gLRecordJournalDetail.Debit>0 && gLRecordJournalDetail.Credit>0)
                ModelState.AddModelError("same", "Both debit and credit can't have nvalue at the same time.");
            if (gLRecordJournalDetail.Debit == 0 && gLRecordJournalDetail.Credit == 0)
                ModelState.AddModelError("same", "Both debit and credit can't have zero value at the same time.");
            if (gLRecordJournalDetail.Debit < 0 || gLRecordJournalDetail.Credit < 0)
                ModelState.AddModelError("negative", "Negative values are not allowed.");

            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new GLRecordJournalDetail
                    {
                        GlRecordJournalId = id,
                        AccountCode = gLRecordJournalDetail.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(gLRecordJournalDetail.AccountCode),
                        Debit = gLRecordJournalDetail.Debit,
                        Credit = gLRecordJournalDetail.Credit,
                        ExRate = gLRecordJournalDetail.ExRate,
                        DebitNonFun = gLRecordJournalDetail.DebitNonFun,
                        CreditNonFun = gLRecordJournalDetail.CreditNonFun,
                        AdditionalDescription = gLRecordJournalDetail.AdditionalDescription,
                        AccountType = gLRecordJournalDetail.AccountType,
                        AccountSegment = gLRecordJournalDetail.AccountSegment,
                        BranchSegment = gLRecordJournalDetail.BranchSegment,
                        CostCenterSegment = gLRecordJournalDetail.CostCenterSegment,
                        SubAccountSegment = gLRecordJournalDetail.SubAccountSegment,
                        ChannelSegment = gLRecordJournalDetail.ChannelSegment,
                        OrgBranchName = gLRecordJournalDetail.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = gLRecordJournalDetail.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = gLRecordJournalDetail.ModifiedBy
                    };


                    _db.GLRecordJournalDetails.Add(entity);
                    _db.SaveChanges();
                    gLRecordJournalDetail.GlRecordJournalDetailId = entity.GlRecordJournalDetailId;
                #region update journal sum
                var journal = _db.GLRecordJournals.Find(id);
                var lines = _db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == id);
                journal.DebitSum = lines.Sum(x => x.Debit);
                _db.GLRecordJournals.Attach(journal);
                _db.Entry(journal).State = EntityState.Modified;
                _db.SaveChanges();
                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL record journal item", "Success", User.Identity.Name, $"created item {entity.AccountCode} under journal {entity.GlRecordJournalId}", Modules.Finance);
                #endregion

            }

            return Json(new[] { gLRecordJournalDetail }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLRecordJournalDetails_Update(int id,[DataSourceRequest]DataSourceRequest request, GLRecordJournalDetail gLRecordJournalDetail)
        {
            gLRecordJournalDetail.Debit = gLRecordJournalDetail.DebitNonFun * gLRecordJournalDetail.ExRate;
            gLRecordJournalDetail.Credit = gLRecordJournalDetail.CreditNonFun * gLRecordJournalDetail.ExRate;
            #region validated record

            GlRecordJournal parent =
                _db.GLRecordJournals.FirstOrDefault(x => x.GlRecordJournalId == gLRecordJournalDetail.GlRecordJournalId);

            if (parent != null && parent.IsOnlineApproved)
                ModelState.AddModelError("approved", "Record is already approved,hence you can not add a record.");
            if (gLRecordJournalDetail.Debit > 0 && gLRecordJournalDetail.Credit > 0)
                ModelState.AddModelError("same", "Both debit and credit can't have nvalue at the same time.");
            if (gLRecordJournalDetail.Debit == 0 && gLRecordJournalDetail.Credit == 0)
                ModelState.AddModelError("same", "Both debit and credit can't have zero value at the same time.");
            if (gLRecordJournalDetail.Debit < 0 || gLRecordJournalDetail.Credit < 0)
                ModelState.AddModelError("negative", "Negative values are not allowed.");

            #endregion
            if (ModelState.IsValid)
            { var entity = new GLRecordJournalDetail
                    {
                        GlRecordJournalId = id,
                        GlRecordJournalDetailId = gLRecordJournalDetail.GlRecordJournalDetailId,
                        AccountCode = gLRecordJournalDetail.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(gLRecordJournalDetail.AccountCode),
                        Debit = gLRecordJournalDetail.Debit,
                        Credit = gLRecordJournalDetail.Credit,
                        ExRate = gLRecordJournalDetail.ExRate,
                        DebitNonFun = gLRecordJournalDetail.DebitNonFun,
                        CreditNonFun = gLRecordJournalDetail.CreditNonFun,
                        AdditionalDescription = gLRecordJournalDetail.AdditionalDescription,
                        AccountType = gLRecordJournalDetail.AccountType,
                        AccountSegment = gLRecordJournalDetail.AccountSegment,
                        BranchSegment = gLRecordJournalDetail.BranchSegment,
                        CostCenterSegment = gLRecordJournalDetail.CostCenterSegment,
                        SubAccountSegment = gLRecordJournalDetail.SubAccountSegment,
                        ChannelSegment = gLRecordJournalDetail.ChannelSegment,
                        OrgBranchName = gLRecordJournalDetail.OrgBranchName,
                        DateCreated = gLRecordJournalDetail.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = gLRecordJournalDetail.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.GLRecordJournalDetails.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();
                #region update journal sum
                var journal = _db.GLRecordJournals.Find(id);
                var lines = _db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == id);
                journal.DebitSum = lines.Sum(x => x.Debit);
                _db.GLRecordJournals.Attach(journal);
                _db.Entry(journal).State = EntityState.Modified;
                _db.SaveChanges();
                #endregion

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL record journal item", "Success", User.Identity.Name, $"updated item {entity.AccountCode} under journal {entity.GlRecordJournalId}", Modules.Finance);
                #endregion

            }

            return Json(new[] { gLRecordJournalDetail }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLRecordJournalDetails_Destroy([DataSourceRequest]DataSourceRequest request, GLRecordJournalDetail gLRecordJournalDetail)
        {
            #region validated record

            GlRecordJournal parent =
                _db.GLRecordJournals.FirstOrDefault(x => x.GlRecordJournalId == gLRecordJournalDetail.GlRecordJournalId);

            if (parent != null && parent.IsOnlineApproved)
                ModelState.AddModelError("approved", "Record is already approved,hence you can not add a record.");

            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new GLRecordJournalDetail
                    {
                        GlRecordJournalDetailId = gLRecordJournalDetail.GlRecordJournalDetailId,
                        AccountCode = gLRecordJournalDetail.AccountCode,
                        AccountDesc = gLRecordJournalDetail.AccountDesc,
                        Debit = gLRecordJournalDetail.Debit,
                        Credit = gLRecordJournalDetail.Credit,
                        ExRate = gLRecordJournalDetail.ExRate,
                        DebitNonFun = gLRecordJournalDetail.DebitNonFun,
                        CreditNonFun = gLRecordJournalDetail.CreditNonFun,
                        AdditionalDescription = gLRecordJournalDetail.AdditionalDescription,
                        AccountType = gLRecordJournalDetail.AccountType,
                        AccountSegment = gLRecordJournalDetail.AccountSegment,
                        BranchSegment = gLRecordJournalDetail.BranchSegment,
                        CostCenterSegment = gLRecordJournalDetail.CostCenterSegment,
                        SubAccountSegment = gLRecordJournalDetail.SubAccountSegment,
                        ChannelSegment = gLRecordJournalDetail.ChannelSegment,
                        OrgBranchName = gLRecordJournalDetail.OrgBranchName,
                        DateCreated = gLRecordJournalDetail.DateCreated,
                        LastModified = gLRecordJournalDetail.LastModified,
                        CreatedBy = gLRecordJournalDetail.CreatedBy,
                        ModifiedBy = gLRecordJournalDetail.ModifiedBy
                    };

                    _db.GLRecordJournalDetails.Attach(entity);
                    _db.GLRecordJournalDetails.Remove(entity);
                    _db.SaveChanges();
                #region update journal sum
                var journal = _db.GLRecordJournals.Find(gLRecordJournalDetail.GlRecordJournalId);
                var lines = _db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == gLRecordJournalDetail.GlRecordJournalId);
                journal.DebitSum = lines.Sum(x => x.Debit);
                _db.GLRecordJournals.Attach(journal);
                _db.Entry(journal).State = EntityState.Modified;
                _db.SaveChanges();
                #endregion
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted journal line " + gLRecordJournalDetail.GlRecordJournalId + " " + gLRecordJournalDetail.AccountCode, Modules.Finance);
                #endregion

            }

            return Json(new[] { gLRecordJournalDetail }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser , FinanceConstants.GeneralLedgerApprover , FinanceConstants.GeneralLedgerView)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser , FinanceConstants.GeneralLedgerApprover , FinanceConstants.GeneralLedgerView)]
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
