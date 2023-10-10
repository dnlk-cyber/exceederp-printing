using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Procurements.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{

    [DataObject]
    public class LotSummaryOds
    {
        ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProformaAndLotSummaryViewModel> GetLotSummary(string id)
        {
            List<ProformaAndLotSummaryViewModel> summaryList = new List<ProformaAndLotSummaryViewModel>();

            int did = int.Parse(id);
            var supp = _db.BidPParticipatingSupplierses.Where(x => x.LotID == did).Select(x=>x.BidPParticipatingSuppliersID).ToList();

            var supplierItem = _db.BidParticipatingSupplierItems.Where(x => supp.Contains(x.BidPParticipatingSuppliersID));

            foreach (var item in supplierItem)
            {
                var supplier = _db.BidPParticipatingSupplierses.Where(x => x.LotID == did).Where(x => x.BidPParticipatingSuppliersID == item.BidPParticipatingSuppliersID).FirstOrDefault();
                var summary = new ProformaAndLotSummaryViewModel
                {
                    Description =  item.ItemName + "-" + item.Name + "-" + item.ModelNo+"-" +item.LotItemID ,
                    SupplierName = supplier?.SupplierName,
                    Quantity = item.Quantity,
                    Unit = item.UoM,
                    UPrice=item.UnitPrice.ToString(),// supplier.IsOutOfStock ? "Out of Stock" : (supplier.UPrice).ToString(),
                    //TPrice = supplier.IsOutOfStock ? "Out of Stock" : (supplier.Quantity * supplier.UPrice).ToString()
                };
                //sub total

                summaryList.Add(summary);
            }

            return summaryList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SupplierSummaryViewModel> GetLotSupplierSummary(string id)
        {
            List<SupplierSummaryViewModel> supplierSummaryList = new List<SupplierSummaryViewModel>();

          
            var winningSuppliers = _db.BidAwardedSuppliers.Where(x=>x.AwardGiven).Where(x => x.LotID.ToString() == id).ToList();

            foreach (var winner in winningSuppliers)
            {

                var supplierSummary = new SupplierSummaryViewModel
                {
                    WinningSupplier = new List<WinningSupplierDetailVM>(),
                    WinnerSupplierName = winner.SupplierName
                };



                var winnerSuppliers = _db.BidAwardedSupplierItems.Where(x=>x.BidAwardedSupplierID == winner.BidAwardedSupplierID).ToList();
                foreach (var winnerSupplier in winnerSuppliers)
                {
                    WinningSupplierDetailVM winnervm =
                        new WinningSupplierDetailVM
                        {
                            Description = winnerSupplier.ItemName + "-" + winnerSupplier.Name + "-" + winnerSupplier.ModelNo,
                            Unit = winnerSupplier.UoM,
                            Quantity = winnerSupplier.Quanity,
                            UnitPrice = winnerSupplier.FinalUPrice,
                            TotalPrice = winnerSupplier.TPrice,
                            SubTotal = winnerSupplier.FinalUPrice * winnerSupplier.Quanity,
                            Vat = winnerSupplier.TaxAmount
                        };

                    supplierSummary.WinningSupplier.Add(winnervm);
                }

                supplierSummaryList.Add(supplierSummary);
            
               
            }

            return supplierSummaryList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public TemplateVM GetLot(string id)
        {

            Organization record = new Organization();

            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            if (organization != null)
            {
                record.Name = organization.TradeName;
                record.AmharicName = organization.AmharicName;
                var address =
                    _db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (record.Address == null)
                        {
                            record.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            record.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    record.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            ReportTemplate templateFormat = _db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.AwardEn);

            TemplateVM template = new TemplateVM
            {
                LogoFileName = record.LogoFileName,
                Name = record.Name,
                AmharicName = record.AmharicName
            };

            if (id != null)
            {
                var winnerSupplier = _db.BidAwardedSuppliers.FirstOrDefault(x => x.LotID.ToString() == id);
                var lot = _db.Lots.FirstOrDefault(x => x.LotID.ToString() == id);
                if (lot != null)
                {
                    template.TenderReference = lot.Tender.TenderReference+" Lot Number "+lot.LotNumber;
                    template.EvaluationDate = null; /*proformaPurchase.Tender.TechnicalOpenigDate.ToShortDateString();*/
                    #region prepared checker approver

                    template.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(lot.CreatedBy));
                    if (lot.DateCreated != null)
                        template.PreparedDate = lot.DateCreated != null ? lot.DateCreated.ToString() : "";

                    var purchaseValidations = _db.TenderValidations
                        .Where(x => x.TenderID == lot.TenderID).ToList();

                    if (purchaseValidations.Any())
                    {
                        int maxid = purchaseValidations.Max(x => x.TenderID);
                        var approver =
                            purchaseValidations.FirstOrDefault(x => x.TenderID == maxid);
                        if (approver != null)
                        {
                            template.ApprovedBy =
                                EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(approver.CreatedBy));
                            template.ApprovedDate = approver.DateCreated != null ? approver.DateCreated.ToString() : "";

                            var checker = purchaseValidations.OrderByDescending(x => x.TenderID)
                                .FirstOrDefault(x => x.TenderID < maxid);

                            if (checker != null)
                            {
                                template.CheckedBy =
                                    EmployeeHelper.GetEmployeeEnglishName(
                                        UserHelper.GetUserEmployeeId(checker.CreatedBy));
                                template.CheckedDate = checker.DateCreated != null ? checker.DateCreated.ToString() : "";
                            }
                            else
                            {
                                template.CheckedBy =
                                    EmployeeHelper.GetEmployeeEnglishName(
                                        UserHelper.GetUserEmployeeId(lot.Tender.SendForApprovalBy));
                                template.CheckedDate = lot.Tender.SendForApprovalTime != null ? lot.Tender.SendForApprovalTime.ToString() : "";

                            }
                        }

                    }

                    #endregion
                }
                if (winnerSupplier != null)
                {
                    template.WinnerSupplier = winnerSupplier.SupplierName;

                }

            }

            return template;
        }

    }
}
