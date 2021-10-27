using System;
using System.Collections.Generic;
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
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlPostJournalController : Controller
    {

        private readonly ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        readonly FinanceHelper _financeHelper = new FinanceHelper();

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser, FinanceConstants.GeneralLedgerApprover, FinanceConstants.GeneralLedgerView, FinanceConstants.GeneralLedgerPostUser, FinanceConstants.GeneralLedgerAdmin)]
        public ActionResult Index()
        {
            PopulateData();

            return View();
        }

        

        private void PopulateData()
        {
            

            List<string> list = new List<string>();

            var categories = _db.GLRecordJournals
                .Where(x => x.IsOnlineApproved==true)
                .Where(x=>x.isPosted==false).OrderBy(x => x.CategoryName)
                .Select(x => x.CategoryName).AsNoTracking().Distinct().ToList();

            list.AddRange(categories);

            ViewData["CategoryList"] = list
                   .Select(category => new
                   {
                       Category = category
                   });


            var users = _db.GLRecordJournals
                            .Where(x => x.IsOnlineApproved && !x.isVoid && !x.isPosted).AsNoTracking().OrderBy(x => x.CreatedBy).Select(x => x.CreatedBy).Distinct().ToList();

            ViewData["UserList"] = users
                    .Select(user => new
                    {
                        User = user
                    })
                ;


        }
        [AuthorizeRoles(FinanceConstants.GeneralLedgerPostUser,FinanceConstants.GeneralLedgerAdmin)]
        public JsonResult BatchPost(string id,string category,string user)
        {
            //if (FinanceGlobalStaticValues.BATCH_POST_RUNNING)
            //{
            //    var formattedData = new
            //    {
            //        Message = "Another Batch Processing is running already.",
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}

            //try
            //{
                //FinanceGlobalStaticValues.BATCH_POST_RUNNING = true;
                int did = int.Parse(id);
                var period = _db.GLPeriods.Find(did);
                if (period != null)
                {
                var unposted = _db.GLRecordJournals
                    .Where(x => x.IsOnlineApproved)
                    .Where(x => x.PeriodName == period.Name)
                    .Where(x => x.isVoid == false)
                    .Where(x => x.isPosted == false)
                    //.ToList();
                        .AsQueryable();

                if (!string.IsNullOrEmpty(category))
                    unposted = unposted.Where(x => x.CategoryName == category).AsQueryable();

                if (!string.IsNullOrEmpty(user))
                    unposted = unposted.Where(x => x.CreatedBy == user).AsQueryable();



                var ledgerIds = unposted.Select(x => x.GlRecordJournalId).ToList();
                    if (!ledgerIds.Any())
                    {
                        var formattedData1 = new
                        {
                            Message = "Data not found for posting.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData1, JsonRequestBehavior.AllowGet);
                    }

                    foreach (var journalId in ledgerIds)
                    {
                        var glRecordJournalDetails =
                            _db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == journalId).ToList();
                        string validForPostError = _accountHelper.IsValidJournal(glRecordJournalDetails);
                        if (!string.IsNullOrEmpty(validForPostError))
                            continue;
                        _financeHelper.PostSingleToLedger(journalId, User.Identity.Name);
                    }

                #region check and remove duplicate lines
                var jobId = BackgroundJob.Enqueue(() => _financeHelper.DetectAndRemoveDuplicateLedgerLines());
                #endregion

                //#region split ledger job

                ////var jobId = BackgroundJob.Enqueue(() => _accountHelper.SplitLedgerAccountCombination());
                //#endregion
                #region update daily balance job

                //var jobUpdateStandard = BackgroundJob.Enqueue(() => _financeHelper.UpdateAllStandardBalancesMonthlyFastP(period.GLPeriodId, User.Identity.Name));

                // RecurringJob.AddOrUpdate(() => _financeHelper.UpdateAllStandardBalancesMonthlyFastP(period.GLPeriodId, User.Identity.Name), "0 19 * * *");//7pm
                #endregion

                // FinanceGlobalStaticValues.BATCH_POST_RUNNING = false;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL batch post", "Success", User.Identity.Name, $"posted journal number {id} category {category}", Modules.Finance);
                #endregion
                var formattedData = new
                    {
                        Message = $"Operation Completed. {ledgerIds.Count} records processed.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //FinanceGlobalStaticValues.BATCH_POST_RUNNING = false;
                    var formattedData = new
                    {
                        Message = "Please choose a period",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            //}
            //catch (Exception e)
            //{
            //    FinanceGlobalStaticValues.BATCH_POST_RUNNING = false;
            //    var formattedData = new
            //    {
            //        Message = "Posting not done."+e.Message,
            //        Status = "NOT_OK"
            //    };
            //    return Json(formattedData, JsonRequestBehavior.AllowGet);
            //}

        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerPostUser, FinanceConstants.GeneralLedgerAdmin)]
        public JsonResult PostLedger(string id)
        {
            int myid = int.Parse(id);
            var gLRecordJournal = _db.GLRecordJournals.Find(myid);
            string validForPostError = _accountHelper.IsValidJournal(gLRecordJournal?.GLRecordJournalDetail);
            if (!string.IsNullOrEmpty(validForPostError))
            {
                var formattedData = new
                {
                    Message = validForPostError,
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var glperiod = _db.GLPeriods.FirstOrDefault(x => x.Name == gLRecordJournal.PeriodName);
            if (glperiod != null && glperiod.Status != FiscalYearStatus.Open)
            {
                var formattedData = new
                {
                    Message = $"The period { glperiod.Name } is not open ,hence can't post.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (glperiod == null)
            {
                var formattedData = new
                {
                    Message = $"The period { gLRecordJournal?.PeriodName } is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (gLRecordJournal != null && glperiod.Status == FiscalYearStatus.Open && !gLRecordJournal.isPosted)
            {
                bool result=_financeHelper.PostSingleToLedger(gLRecordJournal.GlRecordJournalId, User.Identity.Name);
                if (result)
                {
                    //#region split ledger job

                    ////var jobId = BackgroundJob.Enqueue(() => _accountHelper.SplitLedgerAccountCombination());
                    //#endregion

                    #region check and remove duplicate lines
                    var jobId = BackgroundJob.Enqueue(() => _financeHelper.DetectAndRemoveDuplicateLedgerLines());
                    #endregion
                    #region update daily balance joob

                    // var jobUpdateStandard = BackgroundJob.Enqueue(() => );

                    RecurringJob.AddOrUpdate(() => _financeHelper.UpdateAllStandardBalancesMonthlyFastP(glperiod.GLPeriodId, User.Identity.Name), "0 19 * * *");//7pm

                    #endregion

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL post", "Success", User.Identity.Name, $"posted journal number {id} ", Modules.Finance);
                    #endregion

                    var formattedData = new
                    {
                        Message = "Operation Successful.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "Try Again.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerUnPostUser)]
        public JsonResult UnPostLedger(string id)
        {
            int myid = int.Parse(id);
            var gLRecordJournal = _db.GLRecordJournals.Find(myid);

            if (gLRecordJournal != null && gLRecordJournal.isPosted)
            {
                bool result = _financeHelper.Unpost(gLRecordJournal.GlRecordJournalId, User.Identity.Name);
                if (result)
                {
                    #region update daily balance joob

                    var glperiod = _db.GLPeriods.FirstOrDefault(x => x.Name == gLRecordJournal.PeriodName);
                    // var jobUpdateStandard = BackgroundJob.Enqueue(() => );

                    RecurringJob.AddOrUpdate(() => _financeHelper.UpdateAllStandardBalancesMonthlyFastP(glperiod.GLPeriodId, User.Identity.Name), "0 19 * * *");//7pm

                    #endregion

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "GL unpost", "Success", User.Identity.Name, $"unposted journal number {id} ", Modules.Finance);
                    #endregion

                    var formattedData = new
                    {
                        Message = "Operation Successful.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "Try Again.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            }

            var formattedData3 = new
            {
                Message = "It is not possible to unpost the record.",
                Status = "NOT_OK"
            };
            return Json(formattedData3, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeRoles(FinanceConstants.GeneralLedgerUnPostUser)]
        public JsonResult BatchUnPost(string category)
        {
            var journals = _db.GLRecordJournals.Where(x => x.CategoryName == category).AsNoTracking().ToList();
            string userName = User.Identity.Name;

            foreach (var journal in journals)
            {

                if (journal != null && journal.isPosted)
                {
                    bool result = _financeHelper.Unpost(journal.GlRecordJournalId, userName);
                    if (result)
                    {
                        #region update daily balance joob

                        var glperiod = _db.GLPeriods.FirstOrDefault(x => x.Name == journal.PeriodName);
                        // var jobUpdateStandard = BackgroundJob.Enqueue(() => );

                        //RecurringJob.AddOrUpdate(() => _financeHelper.UpdateAllStandardBalancesMonthlyFastP(glperiod.GLPeriodId, User.Identity.Name), "0 19 * * *");//7pm

                        #endregion

                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "GL unpost", "Success", User.Identity.Name, $"unposted journal number {journal.GlRecordJournalId} ", Modules.Finance);
                        #endregion
                        
                    }
                    else
                    {
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "GL unpost", "Failed", User.Identity.Name, $"unposted journal number {journal.GlRecordJournalId} ", Modules.Finance);
                        #endregion
                    }
                }
            }
            var formattedData = new
            {
                Message = "Operation Complete.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        [AuthorizeRoles(FinanceConstants.GeneralLedgerUser, FinanceConstants.GeneralLedgerApprover, FinanceConstants.GeneralLedgerView, FinanceConstants.GeneralLedgerPostUser, FinanceConstants.GeneralLedgerAdmin)]
        public ActionResult GlPostJournals_Read([DataSourceRequest]DataSourceRequest request)
       {
           List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
           IQueryable<GlRecordJournal> glRecordJournals = _db.GLRecordJournals.Where(x => x.IsOnlineApproved)
               .Where(x => x.isPosted == false && x.IsVoidDoc == false);

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
                BranchId = glRecordJournal.BranchId,
                JournalName = glRecordJournal.JournalName,
                Description = glRecordJournal.Description,
                FiscalYear = glRecordJournal.FiscalYear,
                PeriodName = glRecordJournal.PeriodName,
                CategoryName = glRecordJournal.CategoryName,
                SourcesName = glRecordJournal.SourcesName,
                JournalNumber = glRecordJournal.JournalNumber,
                EffectiveDate = glRecordJournal.EffectiveDate.Date,
                Posting = glRecordJournal.Posting,
                isReversed = glRecordJournal.isReversed,
                IsSendForApproval = glRecordJournal.IsSendForApproval,
                isPosted = glRecordJournal.isPosted,
                Status = glRecordJournal.Status,
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

       [AuthorizeRoles(FinanceConstants.GeneralLedgerUser, FinanceConstants.GeneralLedgerAdmin)]
       [AcceptVerbs(HttpVerbs.Post)]
       public ActionResult GLRecordJournalDetails_Update(int id, [DataSourceRequest]DataSourceRequest request, GLRecordJournalDetail gLRecordJournalDetail)
       {
           bool isvalid = true;
           #region validated record

           var parent = _db.GLRecordJournals.FirstOrDefault(x => x.GlRecordJournalId == gLRecordJournalDetail.GlRecordJournalId);
           if (parent != null && parent.isPosted)
           {
               ModelState.AddModelError("posted","You can not update the record because it is already posted.");
               isvalid = false;
           }

           #endregion
           if (ModelState.IsValid)
           {
              

               if (isvalid)
               {
                   var entity = new GLRecordJournalDetail
                   {
                       GlRecordJournalId = id,
                       GlRecordJournalDetailId = gLRecordJournalDetail.GlRecordJournalDetailId,
                       AccountCode = gLRecordJournalDetail.AccountCode,
                       AccountDesc = _accountHelper.GetAccountDesc(gLRecordJournalDetail.AccountCode),
                       Debit = gLRecordJournalDetail.Debit,
                       Credit = gLRecordJournalDetail.Credit,
                       AdditionalDescription = gLRecordJournalDetail.AdditionalDescription,
                       OrgBranchName = gLRecordJournalDetail.OrgBranchName,
                       DateCreated = gLRecordJournalDetail.DateCreated,
                       LastModified = DateTime.Now,
                       CreatedBy = gLRecordJournalDetail.CreatedBy,
                       ModifiedBy = User.Identity.Name
                   };

                   _db.GLRecordJournalDetails.Attach(entity);
                   _db.Entry(entity).State = EntityState.Modified;
                   _db.SaveChanges();
               }
           }

           return Json(new[] { gLRecordJournalDetail }.ToDataSourceResult(request, ModelState));
       }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
