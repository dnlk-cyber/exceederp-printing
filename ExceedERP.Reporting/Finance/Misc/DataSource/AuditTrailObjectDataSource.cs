using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.Misc.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Reporting.Finance.Misc.DataSource
{

    [DataObject]
    public class AuditTrailObjectDataSource
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AuditTrailViewModel> GetAuditTrail(string user, string periodFrom, string periodTo)
        {
            List<AuditTrailViewModel> auditTrails = new List<AuditTrailViewModel>();
            
            if ( !string.IsNullOrWhiteSpace(periodFrom) && !string.IsNullOrWhiteSpace(periodTo))
            {
                int from = int.Parse(periodFrom);
                int to = int.Parse(periodTo);

                var fromObj = _db.GLPeriods.Find(from);
                var toObj = _db.GLPeriods.Find(to);

                var periods = _db.GLPeriods.Where(x => x.DateFrom >= fromObj.DateFrom)
                    .Where(x => x.DateTo <= toObj.DateTo).Select(x => x.GLPeriodId).ToList();

                var ledgerLines = _db.GLLedgerPostingLines.Where(x=>x.Type==LedgerPostTypes.Standard).Where(x=>x.CreatedBy==user).Where(x => periods.Contains(x.GLPeriodId))
                    .Select(x=>new {x.GLLedgerPosting,x.JournalName,x.Description,x.JournalNumber,x.JournalReference,x.CategoryName,x.SourcesName,x.EffectiveDate,x.AdditionalDescription,x.ReferenceDoc,x.Debit,x.Credit}).ToList();

                foreach (var line in ledgerLines)
                {
                    var postLine = new AuditTrailViewModel
                    {
                        AccountCode = line.GLLedgerPosting.AccountCode,
                        JournalName = line.JournalName,
                        Description = line.Description,
                        JournalNumber = line.JournalNumber,
                        JournalReference = line.JournalReference,
                        CategoryName = line.CategoryName,
                        SourcesName = line.SourcesName,
                        EffectiveDate = line.EffectiveDate,
                        Debit = line.Debit,
                        Credit = line.Credit,
                        AdditionalDescription=line.AdditionalDescription,
                        ReferenceDoc = line.ReferenceDoc
                    };

                    auditTrails.Add(postLine);
                }

                

            }

            return auditTrails;
        }
    }
}