using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{

    [DataObject]
    public class TenderOpeningObjectDataSource : List<TenderOpening>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TenderOpening> GetFinancialOpenTender(string id)
        {
            List<TenderOpening> opendList = new List<TenderOpening>();
            TenderOpening opened = new TenderOpening();

            Organization record = new Organization();

            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            if (organization != null)
            {
                opened.Name = organization.TradeName;
                opened.AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (opened.Address == null)
                        {
                            opened.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            opened.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    opened.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            if (id != null)
            {
                var tenders = db.Tenders.FirstOrDefault(x => x.TenderID.ToString() == id);
                if (tenders != null)
                {
                    opened.TenderRefNo = tenders.TenderReference;
                    opened.FinancialOpeningDate = tenders.FinancialOpenigDate;
                    if (tenders.FinancialOpenigDate != null)
                        opened.OpeningHour = tenders.FinancialOpenigDate.Value.Hour;
                    var lot = db.Lots.FirstOrDefault(x => x.TenderID == tenders.TenderID);
                    if (lot != null)
                    {
                        var lotPurchaseRequisition = db.LotPurchaseRequisitions.FirstOrDefault(x => x.LotID == lot.LotID);
                        if (lotPurchaseRequisition != null)
                        {
                            var bidPurchaseSupplier = db.BidPurchasingSupplierses.FirstOrDefault(x => x.LotID == lotPurchaseRequisition.LotPurchaseRequisitionId);
                            if (bidPurchaseSupplier != null) opened.SupplierAddress = bidPurchaseSupplier.Address;
                            var purchaseRequisition = db.PurchaseRequisitions.Where(x => x.PurchaseRequisitionID == lotPurchaseRequisition.PurchaseRequisitionID).ToList();

                            foreach (var purchase in purchaseRequisition)
                            {
                                var branch = db.Branch.FirstOrDefault(x => x.BranchId == purchase.ForBranchId);
                                if (branch != null)
                                {
                                    opened.Branch = opened.Branch + ", " + branch.Name;

                                }
                            }
                        }
                    }
                    opendList.Add(opened);
                }
                if (id != null)
                {
                    int tenderId = int.Parse(id);


                    Lot lot = db.Lots.Find(tenderId);

                    if (lot != null)
                    {

                        ReportTemplate templateFormat = db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.FinancialOpen);

                        Tender tender = lot.Tender;
                        if (templateFormat != null)
                        {
                            opened.Let1 = templateFormat.Temp2 + " ";
                            if (tender != null)
                            {
                                opened.Let2 = " ለ " + tender.Description + " ";
                            }

                            opened.Let3 = templateFormat.Temp3 + " ";
                            opened.Let4 = " ከ ";
                            opened.Let5 = templateFormat.Temp4 + " ";
                            opened.Summary = templateFormat.Temp5 + " ";
                            opened.Subject = templateFormat.Temp1;

                        }
                    }

                }
            }
            return opendList;
        }

       
    }
    [DataObject]
    public class TechnicalTenderOpeningObjectDataSource : List<TenderOpening>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TenderOpening> GetTechnicalOpenTender(string id)
        {

            List<TenderOpening> opendList = new List<TenderOpening>();
            TenderOpening opened = new TenderOpening();

            Organization record = new Organization();

            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            if (organization != null)
            {
                opened.Name = organization.TradeName;
                opened.AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (opened.Address == null)
                        {
                            opened.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            opened.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    opened.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            if (id != null)
            {
                var tenders = db.Tenders.FirstOrDefault(x => x.TenderID.ToString() == id);
                if (tenders != null)
                {
                    opened.TenderRefNo = tenders.TenderReference;
                    opened.TechnicalOpeningDate = tenders.TechnicalOpenigDate;
                    if (tenders.TechnicalOpenigDate != null)
                        opened.OpeningHour = tenders.TechnicalOpenigDate.Value.Hour;
                    var lot = db.Lots.FirstOrDefault(x => x.TenderID == tenders.TenderID);
                    if (lot != null)
                    {
                        var lotPurchaseRequisition = db.LotPurchaseRequisitions.FirstOrDefault(x => x.LotID == lot.LotID);
                        if (lotPurchaseRequisition != null)
                        {
                            var bidPurchaseSupplier = db.BidPurchasingSupplierses.FirstOrDefault(x => x.LotID == lotPurchaseRequisition.LotPurchaseRequisitionId);
                            if (bidPurchaseSupplier != null)
                            {
                                opened.SupplierAddress = bidPurchaseSupplier.Address;
                            }
                            var purchaseRequisition = db.PurchaseRequisitions.Where(x => x.PurchaseRequisitionID == lotPurchaseRequisition.PurchaseRequisitionID).ToList();
                            foreach (var purchase in purchaseRequisition)
                            {
                                var branch = db.Branch.FirstOrDefault(x => x.BranchId == purchase.ForBranchId);
                                if (branch != null)
                                {
                                    opened.Branch = opened.Branch + ", " + branch.Name;

                                }
                            }
                        }
                    }
                    opendList.Add(opened);
                }

                if (id != null)
                {
                    int tenderId = int.Parse(id);


                    Lot lot = db.Lots.Find(tenderId);

                    if (lot != null)
                    {

                        ReportTemplate templateFormat = db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.TechnicalOpen);

                        Tender tender = lot.Tender;
                        if (templateFormat != null)
                        {
                            opened.Let1 = templateFormat.Temp2 + " ";
                            if (tender != null)
                            {
                                opened.Let2 = " ለ " + tender.Description + " ";
                            }

                            opened.Let3 = templateFormat.Temp3 + " ";
                            opened.Let4 = " ከ ";
                            opened.Let5 = templateFormat.Temp4 + " ";
                            opened.Summary = templateFormat.Temp5 + " ";
                            opened.Subject = templateFormat.Temp1;

                        }
                    }

                }
            }

            return opendList;

        }
    }
    
}
