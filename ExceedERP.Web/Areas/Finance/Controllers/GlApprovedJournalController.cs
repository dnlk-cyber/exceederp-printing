﻿using System;
 using System.Collections.Generic;
 using System.Data.Entity;
﻿using System.Linq;
﻿using System.Web.Mvc;
 using ExceedERP.Core.Domain.Finance;
 using ExceedERP.Core.Domain.Finance.Common;
﻿using ExceedERP.Core.Domain.Finance.GL;
 using ExceedERP.Core.Domain.Inventory;
 using ExceedERP.DataAccess.Context;
 using ExceedERP.Helpers.Inventory;
 using ExceedERP.Web.Helpers;
 using Kendo.Mvc.Extensions;
﻿using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers
{
    public class GlApprovedJournalController : Controller
    {

        private ExceedDbContext _db = new ExceedDbContext();
        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser, FinanceConstants.GeneralLedgerApprover, FinanceConstants.GeneralLedgerView, FinanceConstants.GeneralLedgerAdmin)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser, FinanceConstants.GeneralLedgerApprover, FinanceConstants.GeneralLedgerView, FinanceConstants.GeneralLedgerAdmin)]
        public ActionResult AccountsTransactions_Read([DataSourceRequest]DataSourceRequest request)
       {
           List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);

           IQueryable<GlRecordJournal> glRecordJournals = _db.GLRecordJournals.Where(x => x.IsOnlineApproved);

            //if (!((System.Web.HttpContext.Current.User.IsInRole(FinanceConstants.GeneralLedgerApprover))
            //      || (System.Web.HttpContext.Current.User.IsInRole(FinanceConstants.GeneralLedgerView))))
            //{
            //    glRecordJournals = glRecordJournals.Where(x => mybraches.Contains(x.OrgBranchName));
            //}

            //new
            if (System.Web.HttpContext.Current.User.IsInRole(FinanceConstants.GeneralLedgerAdmin))
                glRecordJournals = glRecordJournals.Where(x => mybraches.Contains(x.OrgBranchName) || x.OrgBranchName == null || x.OrgBranchName == "");
            else
                glRecordJournals = glRecordJournals.Where(x => mybraches.Contains(x.OrgBranchName));

            DataSourceResult result = glRecordJournals.ToDataSourceResult(request, glRecordJournal => new
            {
                GlRecordJournalId = glRecordJournal.GlRecordJournalId,
                JournalName = glRecordJournal.JournalName,
                Description = glRecordJournal.Description,
                FiscalYear = glRecordJournal.FiscalYear,
                PeriodName = glRecordJournal.PeriodName,
                CategoryName = glRecordJournal.CategoryName,
                SourcesName = glRecordJournal.SourcesName,
                JournalNumber = glRecordJournal.JournalNumber,
                EffectiveDate = glRecordJournal.EffectiveDate,
                Posting = glRecordJournal.Posting,
                isReversed = glRecordJournal.isReversed,
                IsSendForApproval = glRecordJournal.IsSendForApproval,
                isPosted = glRecordJournal.isPosted,
                Status = glRecordJournal.Status,
                PostedBy = glRecordJournal.PostedBy,
                PostedTime = glRecordJournal.PostedTime,
                Remark = glRecordJournal.Remark,
                ReferenceDoc = glRecordJournal.ReferenceDoc,
                PaymentReference = glRecordJournal.PaymentReference,
                InvoiceReferenceDoc = glRecordJournal.InvoiceReferenceDoc,
                IsOnlineApproved = glRecordJournal.IsOnlineApproved,
                OnlineApprovedBy = glRecordJournal.OnlineApprovedBy,
                OnlineApprovedTime = glRecordJournal.OnlineApprovedTime,
                isVoid = glRecordJournal.isVoid,
                VoidBy = glRecordJournal.VoidBy,
                VoidTime = glRecordJournal.VoidTime,
                OrgBranchName = glRecordJournal.OrgBranchName,
                DateCreated = glRecordJournal.DateCreated,
                LastModified = glRecordJournal.LastModified,
                CreatedBy = glRecordJournal.CreatedBy,
                ModifiedBy = glRecordJournal.ModifiedBy
            });

            return Json(result);
        }
                     
        public JsonResult CopyRecord(int id)
        {
            try
            {
                var glRecordJournal = _db.GLRecordJournals.Find(id);
                if (glRecordJournal != null)
                {
                    #region header
                    var entity = new GlRecordJournal
                    {
                        JournalName = glRecordJournal.JournalName,
                        Description = glRecordJournal.Description,
                        FiscalYear = glRecordJournal.FiscalYear,
                        PeriodName = glRecordJournal.PeriodName,
                        CategoryName = glRecordJournal.CategoryName,
                        SourcesName = glRecordJournal.SourcesName,
                        JournalNumber = glRecordJournal.JournalNumber,
                        EffectiveDate = glRecordJournal.EffectiveDate,
                        IsSendForApproval = glRecordJournal.IsSendForApproval,
                        IsPostedToSecondLedger = false,
                        Remark = glRecordJournal.Remark,
                        Status = "Editing",
                        IsOnlineApproved = false,
                        isVoid = false,
                        isPosted = false,
                        OrgBranchName = glRecordJournal.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = glRecordJournal.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = glRecordJournal.ModifiedBy
                    };

                    _db.GLRecordJournals.Add(entity);
                    _db.SaveChanges();
                    #endregion

                    var details = _db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == id).ToList();
                    foreach (var detail in details)
                    {
                        #region lines
                        var line = new GLRecordJournalDetail
                        {
                            GlRecordJournalId = entity.GlRecordJournalId,
                            AccountCode = detail.AccountCode,
                            AccountDesc = detail.AccountDesc,
                            Debit = detail.Debit,
                            Credit = detail.Credit,
                            AdditionalDescription = detail.AdditionalDescription,
                            AccountType = detail.AccountType,
                            AccountSegment = detail.AccountSegment,
                            BranchSegment = detail.BranchSegment,
                            CostCenterSegment = detail.CostCenterSegment,
                            SubAccountSegment = detail.SubAccountSegment,
                            ChannelSegment = detail.ChannelSegment,
                            OrgBranchName = detail.OrgBranchName,
                            DateCreated = DateTime.Now,
                            LastModified = detail.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = detail.ModifiedBy
                        };


                        _db.GLRecordJournalDetails.Add(line);
                        _db.SaveChanges();

                        #endregion

                    }

                }

                var formattedData = new
                {
                    Message = "Record Copied Successfuly.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                var formattedData = new
                {
                    Message = "Problem occured.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

           

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
