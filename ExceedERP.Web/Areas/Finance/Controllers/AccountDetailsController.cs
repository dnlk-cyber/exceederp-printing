using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;

namespace ExceedERP.Web.Areas.Finance.Controllers
{
    [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin, 
        FinanceConstants.GeneralLedgerUser, 
        FinanceConstants.GeneralLedgerView,
        FinanceConstants.GeneralLedgerPostUser)]
    public class AccountDetailsController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        FinanceHelper _financeHelper = new FinanceHelper();
        AccountHelper _accountHelper = new AccountHelper();
        FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        // GET: Finance/Description





        public ActionResult Index(int? id)
        {
            if(id== null)
                return View();

            var postLine = _db.GLLedgerPostingLines.Where(x => x.Type == LedgerPostTypes.Standard).Where(x => x.GLLedgerPostingLinesID == id).FirstOrDefault();
            if (postLine != null)
            {
                var journal = _db.GLRecordJournals.Find(postLine.JournalId);
                var record = new JournalTransactionVM();

                if (journal != null)
                {
                    var lines = _db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();

                    // Populate the JournalTransactionVM
                    record.Id = journal.GlRecordJournalId.ToString();
                    record.Description = journal.Description;
                    record.FiscalYear = journal.FiscalYear;
                    record.PeriodName = journal.PeriodName;
                    record.CategoryName = journal.CategoryName;
                    record.SourcesName = journal.SourcesName;
                    record.Reference = journal.JournalNumber;
                    record.Date = journal.EffectiveDate;

                    // Populate the JournalTransactionLineVms collection
                    foreach (var line in lines)
                    {
                        var lineVm = new JournalTransactionLineVm
                        {
                            LineId = line.GlRecordJournalDetailId.ToString(),
                            AccountCode = line.AccountCode,
                            AccountDesc = line.AccountDesc,
                            Debit = line.Debit,
                            Credit = line.Credit,
                            AdditionalDescription = line.AdditionalDescription
                        };

                        record.JournalTransactionLineVms.Add(lineVm);
                    }
                }

                return View(record);
            }
            return View();
        }



    }
}