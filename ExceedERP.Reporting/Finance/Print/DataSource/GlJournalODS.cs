using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Reporting.Finance.ViewModel.Print;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Finance.DataSource.Print
{
    [DataObject]
    public class GlJournalOds
    {
        readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public GlJournalViewModel JournalHeader(int id)
        {
            var journal = _db.GLRecordJournals.Find(id);
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            GlJournalViewModel headerDs = new GlJournalViewModel();

            if (journal != null)
            {
                headerDs.VoucherId = journal.GlRecordJournalId.ToString();
                headerDs.JournalName = journal.JournalName;
                headerDs.JournalNumber = journal.JournalNumber;
                headerDs.Description = journal.Description;
                headerDs.FiscalYear = journal.FiscalYear;
                headerDs.PeriodName = journal.PeriodName;
                headerDs.CategoryName = journal.CategoryName;
                headerDs.SourcesName = journal.SourcesName;
                headerDs.EffectiveDate = journal.EffectiveDate;
                headerDs.PreparedBy = journal.CreatedBy;
                headerDs.ApprovedBy = journal.OnlineApprovedBy;

                try
                {

                    string type = journal.JournalNumber.Split('.')[0];
                    string recordid = journal.JournalNumber.Split('.')[1];

                    if (type == "Transfer Send")
                    {
                        int did = int.Parse(recordid);
                        var transferSend = _db.ItemTransfers.Find(did);
                        headerDs.TransferFrom =
                            _db.Branch.Find(InventoryHelper.GetStoreBranch(transferSend?.FromStore))?.Name;
                        headerDs.TransferTo =
                            _db.Branch.Find(InventoryHelper.GetStoreBranch(transferSend?.ToStore))?.Name;
                    }

                }
                catch (Exception e)
                {
                    ;
                }

            }

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
        public List<GlJournalLinesViewModel> JournalLines(int id)
        {
            List<GlJournalLinesViewModel> lines = new List<GlJournalLinesViewModel>();
            var journalLines = _db.GLRecordJournalDetails.Where(x => x.GlRecordJournalId == id).ToList();
            foreach (var line in journalLines)
            {
                GlJournalLinesViewModel record = new GlJournalLinesViewModel
                {
                    AccountCode = line.AccountCode,
                    AccountDesc = line.AccountDesc,
                    Debit = line.Debit,
                    Credit = line.Credit,
                    AdditionalDescription= line.AdditionalDescription
                };

                lines.Add(record);

            }
            return lines;
        }
    }
}
