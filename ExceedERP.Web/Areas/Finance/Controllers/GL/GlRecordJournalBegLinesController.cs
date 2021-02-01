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
    public class GlRecordJournalBegLinesController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        AccountHelper _accountHelper = new AccountHelper();

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser , FinanceConstants.GeneralLedgerApprover , FinanceConstants.GeneralLedgerView)]
        public ActionResult GLRecordJournalBegDistributions_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            //if (!_db.GLRecordJournalBegDistributions.Any())
            //{
            //    #region create beginning lines
            //    var begPeriod = _db.GlBeginningPeriods.FirstOrDefault();

            //    if (begPeriod != null)
            //    {
            //        var beginningLedgerLines = _db.GLLedgerPostingLines
            //            .Where(x=>x.GLPeriodId==begPeriod.GLPeriodId)
            //            .Where(x=>x.Type==LedgerPostTypes.Begining).ToList();

            //        foreach (var line in beginningLedgerLines)
            //        {
                        
            //            var entity = new GLRecordJournalBegDistribution
            //            {
            //                GLRecordJournalBegId = id,
            //                AccountCode = line.GLLedgerPosting.AccountCode,
            //                AccountDesc = line.GLLedgerPosting.AccountDesc,
            //                Debit = line.Debit,
            //                Credit = line.Credit,
            //                AdditionalDescription = line.AdditionalDescription,
            //                AccountType = line.GLLedgerPosting.AccountType,
            //                AccountSegment = line.GLLedgerPosting.AccountSegment,
            //                BranchSegment = line.GLLedgerPosting.BranchSegment,
            //                CostCenterSegment = line.GLLedgerPosting.CostCenterSegment,
            //                SubAccountSegment = line.GLLedgerPosting.SubAccountSegment,
            //                ChannelSegment = line.GLLedgerPosting.ChannelSegment,
            //                DateCreated = DateTime.Now,
            //                CreatedBy = User.Identity.Name
            //            };


            //            _db.GLRecordJournalBegDistributions.Add(entity);
            //            _db.SaveChanges();
            //        }
            //    }
            //    #endregion
            //}

            IQueryable<GLRecordJournalBegDistribution> GLRecordJournalBegDistributions = _db.GLRecordJournalBegDistributions.Where(x => x.GLRecordJournalBegId == id);
            DataSourceResult result = GLRecordJournalBegDistributions.ToDataSourceResult(request, GLRecordJournalBegDistribution => new {
                GLRecordJournalBegDistributionId = GLRecordJournalBegDistribution.GLRecordJournalBegDistributionId,
                GLRecordJournalBegId = GLRecordJournalBegDistribution.GLRecordJournalBegId,
                AccountCode = GLRecordJournalBegDistribution.AccountCode,
                AccountDesc = GLRecordJournalBegDistribution.AccountDesc,
                Debit = GLRecordJournalBegDistribution.Debit,
                Credit = GLRecordJournalBegDistribution.Credit,
                AdditionalDescription = GLRecordJournalBegDistribution.AdditionalDescription,
                AccountType = GLRecordJournalBegDistribution.AccountType,
                AccountSegment = GLRecordJournalBegDistribution.AccountSegment,
                BranchSegment = GLRecordJournalBegDistribution.BranchSegment,
                CostCenterSegment = GLRecordJournalBegDistribution.CostCenterSegment,
                SubAccountSegment = GLRecordJournalBegDistribution.SubAccountSegment,
                ChannelSegment = GLRecordJournalBegDistribution.ChannelSegment,
                OrgBranchName = GLRecordJournalBegDistribution.OrgBranchName,
                DateCreated = GLRecordJournalBegDistribution.DateCreated,
                LastModified = GLRecordJournalBegDistribution.LastModified,
                CreatedBy = GLRecordJournalBegDistribution.CreatedBy,
                ModifiedBy = GLRecordJournalBegDistribution.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLRecordJournalBegDistributions_Create(int id, [DataSourceRequest]DataSourceRequest request, GLRecordJournalBegDistribution GLRecordJournalBegDistribution)
        {
            if (id == 0)
            {
                ModelState.AddModelError("id", "Add Journal First");
            }
            #region validated record

            GLRecordJournalBeg parent =
                _db.GlRecordJournalBegs.FirstOrDefault(x => x.GLRecordJournalBegId == GLRecordJournalBegDistribution.GLRecordJournalBegId);

            if (parent != null && parent.IsOnlineApproved)
                ModelState.AddModelError("approved", "Record is already approved,hence you can not add a record.");
            if (GLRecordJournalBegDistribution.Debit>0 && GLRecordJournalBegDistribution.Credit>0)
                ModelState.AddModelError("same", "Both debit and credit can't have nvalue at the same time.");
            if (GLRecordJournalBegDistribution.Debit == 0 && GLRecordJournalBegDistribution.Credit == 0)
                ModelState.AddModelError("same", "Both debit and credit can't have zero value at the same time.");
            if (GLRecordJournalBegDistribution.Debit < 0 || GLRecordJournalBegDistribution.Credit < 0)
                ModelState.AddModelError("negative", "Negative values are not allowed.");

            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new GLRecordJournalBegDistribution
                    {
                        GLRecordJournalBegId = id,
                        AccountCode = GLRecordJournalBegDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(GLRecordJournalBegDistribution.AccountCode),
                        Debit = GLRecordJournalBegDistribution.Debit,
                        Credit = GLRecordJournalBegDistribution.Credit,
                        AdditionalDescription = GLRecordJournalBegDistribution.AdditionalDescription,
                        AccountType = GLRecordJournalBegDistribution.AccountType,
                        AccountSegment = GLRecordJournalBegDistribution.AccountSegment,
                        BranchSegment = GLRecordJournalBegDistribution.BranchSegment,
                        CostCenterSegment = GLRecordJournalBegDistribution.CostCenterSegment,
                        SubAccountSegment = GLRecordJournalBegDistribution.SubAccountSegment,
                        ChannelSegment = GLRecordJournalBegDistribution.ChannelSegment,
                        OrgBranchName = GLRecordJournalBegDistribution.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = GLRecordJournalBegDistribution.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = GLRecordJournalBegDistribution.ModifiedBy
                    };


                    _db.GLRecordJournalBegDistributions.Add(entity);
                    _db.SaveChanges();
                    GLRecordJournalBegDistribution.GLRecordJournalBegDistributionId = entity.GLRecordJournalBegDistributionId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL record beginning journal item", "Success", User.Identity.Name, $"created item {entity.AccountCode} under beginning journal {entity.GLRecordJournalBegId}", Modules.Finance);
                #endregion

            }

            return Json(new[] { GLRecordJournalBegDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLRecordJournalBegDistributions_Update(int id,[DataSourceRequest]DataSourceRequest request, GLRecordJournalBegDistribution GLRecordJournalBegDistribution)
        {
            #region validated record

            GLRecordJournalBeg parent =
                _db.GlRecordJournalBegs.FirstOrDefault(x => x.GLRecordJournalBegId == GLRecordJournalBegDistribution.GLRecordJournalBegId);

            if (parent != null && parent.IsOnlineApproved)
                ModelState.AddModelError("approved", "Record is already approved,hence you can not add a record.");
            if (GLRecordJournalBegDistribution.Debit > 0 && GLRecordJournalBegDistribution.Credit > 0)
                ModelState.AddModelError("same", "Both debit and credit can't have nvalue at the same time.");
            if (GLRecordJournalBegDistribution.Debit == 0 && GLRecordJournalBegDistribution.Credit == 0)
                ModelState.AddModelError("same", "Both debit and credit can't have zero value at the same time.");
            if (GLRecordJournalBegDistribution.Debit < 0 || GLRecordJournalBegDistribution.Credit < 0)
                ModelState.AddModelError("negative", "Negative values are not allowed.");

            #endregion
            if (ModelState.IsValid)
            { var entity = new GLRecordJournalBegDistribution
                    {
                        GLRecordJournalBegId = id,
                        GLRecordJournalBegDistributionId = GLRecordJournalBegDistribution.GLRecordJournalBegDistributionId,
                        AccountCode = GLRecordJournalBegDistribution.AccountCode,
                        AccountDesc = _accountHelper.GetAccountDesc(GLRecordJournalBegDistribution.AccountCode),
                        Debit = GLRecordJournalBegDistribution.Debit,
                        Credit = GLRecordJournalBegDistribution.Credit,
                        AdditionalDescription = GLRecordJournalBegDistribution.AdditionalDescription,
                        AccountType = GLRecordJournalBegDistribution.AccountType,
                        AccountSegment = GLRecordJournalBegDistribution.AccountSegment,
                        BranchSegment = GLRecordJournalBegDistribution.BranchSegment,
                        CostCenterSegment = GLRecordJournalBegDistribution.CostCenterSegment,
                        SubAccountSegment = GLRecordJournalBegDistribution.SubAccountSegment,
                        ChannelSegment = GLRecordJournalBegDistribution.ChannelSegment,
                        OrgBranchName = GLRecordJournalBegDistribution.OrgBranchName,
                        DateCreated = GLRecordJournalBegDistribution.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = GLRecordJournalBegDistribution.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.GLRecordJournalBegDistributions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL record beginning journal item", "Success", User.Identity.Name, $"updated item {entity.AccountCode} under beginning journal {entity.GLRecordJournalBegId}", Modules.Finance);
                #endregion

            }

            return Json(new[] { GLRecordJournalBegDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLRecordJournalBegDistributions_Destroy([DataSourceRequest]DataSourceRequest request, GLRecordJournalBegDistribution GLRecordJournalBegDistribution)
        {
            #region validated record

            GLRecordJournalBeg parent =
                _db.GlRecordJournalBegs.FirstOrDefault(x => x.GLRecordJournalBegId == GLRecordJournalBegDistribution.GLRecordJournalBegId);

            if (parent != null && parent.IsOnlineApproved)
                ModelState.AddModelError("approved", "Record is already approved,hence you can not add a record.");

            #endregion
            if (ModelState.IsValid)
            {
                    var entity = new GLRecordJournalBegDistribution
                    {
                        GLRecordJournalBegDistributionId = GLRecordJournalBegDistribution.GLRecordJournalBegDistributionId,
                        AccountCode = GLRecordJournalBegDistribution.AccountCode,
                        AccountDesc = GLRecordJournalBegDistribution.AccountDesc,
                        Debit = GLRecordJournalBegDistribution.Debit,
                        Credit = GLRecordJournalBegDistribution.Credit,
                        AdditionalDescription = GLRecordJournalBegDistribution.AdditionalDescription,
                        AccountType = GLRecordJournalBegDistribution.AccountType,
                        AccountSegment = GLRecordJournalBegDistribution.AccountSegment,
                        BranchSegment = GLRecordJournalBegDistribution.BranchSegment,
                        CostCenterSegment = GLRecordJournalBegDistribution.CostCenterSegment,
                        SubAccountSegment = GLRecordJournalBegDistribution.SubAccountSegment,
                        ChannelSegment = GLRecordJournalBegDistribution.ChannelSegment,
                        OrgBranchName = GLRecordJournalBegDistribution.OrgBranchName,
                        DateCreated = GLRecordJournalBegDistribution.DateCreated,
                        LastModified = GLRecordJournalBegDistribution.LastModified,
                        CreatedBy = GLRecordJournalBegDistribution.CreatedBy,
                        ModifiedBy = GLRecordJournalBegDistribution.ModifiedBy
                    };

                    _db.GLRecordJournalBegDistributions.Attach(entity);
                    _db.GLRecordJournalBegDistributions.Remove(entity);
                    _db.SaveChanges();
                
            }

            return Json(new[] { GLRecordJournalBegDistribution }.ToDataSourceResult(request, ModelState));
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
