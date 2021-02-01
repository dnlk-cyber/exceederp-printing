using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.SubLedger.ViewModel;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Finance.SubLedger.DataSource
{
   
    [DataObject]
    public class GlJournalReconcileOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public Organization GlJournalReconcileHeader()
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            Organization headerDs = new Organization();

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (headerDs.Address == null)
                        {
                            headerDs.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            headerDs.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return headerDs;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<GlJournalreconcileSubLedgerViewModel> GlJournalReconcileLines(string category, string source, string status, string dateFrom, string dateTo)
        {
            List<GlJournalreconcileSubLedgerViewModel> lines = new List<GlJournalreconcileSubLedgerViewModel>();

            var journals = _db.GLRecordJournals
                .Select(x => new { x.GlRecordJournalId,x.EffectiveDate,x.CategoryName,x.Status,x.SourcesName,x.PeriodName,x.JournalNumber,x.GLRecordJournalDetail, x.Remark })
                .AsQueryable();
           
            #region journal

            if (!String.IsNullOrEmpty(dateFrom) && !String.IsNullOrEmpty(dateTo))
            {
                DateTime from = DateTime.Parse(dateFrom);
                DateTime to = DateTime.Parse(dateTo);
                journals = journals.Where(x => x.EffectiveDate >= from && x.EffectiveDate <= to).AsQueryable();
                if (!String.IsNullOrEmpty(category))
                {
                    journals = journals.Where(x => x.CategoryName == category);
                }

                if (!String.IsNullOrEmpty(status))
                {
                    journals = journals.Where(x => x.Status == status);
                }

                var records = journals.ToList();
                foreach (var record in records)
                {
                    var line = new GlJournalreconcileSubLedgerViewModel
                    {
                        JournalNumber = record.GlRecordJournalId.ToString(),
                        Source = record.SourcesName,
                        Period = record.PeriodName,
                        JournalCategory = record.CategoryName,
                        Reference = record.JournalNumber,
                        EffectiveDate = record.EffectiveDate,
                        Amount = record.GLRecordJournalDetail.Sum(x => x.Debit),
                        Status = record.Status
                    };
                    lines.Add(line);

                }
            }

           

            #endregion

            
            return lines;
        }
    }
}
