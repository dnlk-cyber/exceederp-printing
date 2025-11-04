using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Procurement.Guarantee;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Procurements.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    [DataObject]
    public class ReportTemplateDS
    {
        ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InventoryItem> GetTendetTechnicals(int id)
        {
            Lot lot = _db.Lots.Find(id);
            List<InventoryItem> inventoryItems = new List<InventoryItem>();
            foreach (ParticipantsItem lotParticipantsItem in lot.ParticipantsItems)
            {
                //InventoryItem inventoryItem = _db.InventoryItems.Find(lotParticipantsItem.InventoryItem);
                //inventoryItems.Add(inventoryItem);

            }
            return inventoryItems;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BidAwardedSupplier> GetAwardedSuppliers(string id)
        {
            int lotid = int.Parse(id);
            var awardedSuppliers = _db.BidAwardedSuppliers.Where(x=>x.AwardGiven).Where(x => x.LotID == lotid).ToList();//.DistinctBy(x => x.SupplierName);
            return awardedSuppliers.ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BidPurchasingSuppliers> GetTechnicalSupplier(string id)
        {
            List<BidPurchasingSuppliers> bidPurchasingSuppliers = new List<BidPurchasingSuppliers>();

            if (id != null)
            {
                int lotid = int.Parse(id);
                Lot lot = _db.Lots.Find(lotid);
                List<TenderTechnical> technicals = lot.TenderTechnicals.ToList();
                foreach (var tenderTechnical in technicals)
                {
                    BidPurchasingSuppliers suppliers = _db.BidPurchasingSupplierses.Find(tenderTechnical.SupplierId);
                    bidPurchasingSuppliers.Add(suppliers);
                }
            }



            return bidPurchasingSuppliers;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BidPurchasingSuppliers> BidPurchasingSupplierses(int id, string itemCode)
        {
            Lot lot = _db.Lots.Find(id);
            List<BidPurchasingSuppliers> bidPurchasingSuppliers = new List<BidPurchasingSuppliers>();

            List<BidAwardedSupplier> tenderTechnicals = _db.BidAwardedSuppliers.Where(x => x.AwardGiven).Where(x => x.LotID == lot.LotID).ToList();
            foreach (var lotTenderTechnical in tenderTechnicals)
            {
                var bidAwardedSupplierItem = _db.BidAwardedSupplierItems.FirstOrDefault(x => x.BidAwardedSupplierID == lotTenderTechnical.BidAwardedSupplierID);
                if (bidAwardedSupplierItem != null)
                {

                    BidPurchasingSuppliers suppliers = _db.BidPurchasingSupplierses.Find(lotTenderTechnical.SupplierName);
                    bidPurchasingSuppliers.Add(suppliers);
                }

            }
            return bidPurchasingSuppliers;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public TemplateVM GetReportTemplate(string id, string supplier)
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
            TemplateVM template = new TemplateVM();
            ReportTemplate templateFormat = _db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.TechnicalEn);

            if (id != null && supplier != null)
            {
                int lotid = int.Parse(id);

                Lot lot = _db.Lots.Find(lotid);
                if (lot != null)
                {
                    BidPurchasingSuppliers bidPurchasing = _db.BidPurchasingSupplierses.FirstOrDefault(x => x.SupplierName == supplier);
                    Tender tender = lot.Tender;
                    var supTechnical = lot.TenderTechnicals.Where(x => x.SupplierName == supplier);
                    var technical = lot.EvaluationCriterias.FirstOrDefault(x => x.Type == CriteriaType.Technical);
                    var averagevalue = supTechnical.Average(x => x.Result);
                    if (templateFormat != null)
                    {
                        template.Let1 = templateFormat.Temp2 + " ";
                        if (tender != null)
                        {
                            template.Let2 = " for the " + tender.Description + " ";
                            template.Value1 = tender.TenderReference + " ";
                        }

                        template.Let3 = templateFormat.Temp3 + " ";
                        template.Let4 = " out of ";
                        template.Let5 = templateFormat.Temp4 + " ";
                        template.Summary = templateFormat.Temp5 + " ";

                        if (technical != null) template.Value3 = technical.Weight.ToString();

                        template.Value2 = averagevalue.ToString();


                        template.Subject = templateFormat.Temp1;
                        template.City = "Addis Ababa";
                        template.LogoFileName = record.LogoFileName;
                        template.Name = record.Name;
                        template.AmharicName = record.AmharicName;

                        if (bidPurchasing != null) template.To = bidPurchasing.SupplierName;
                    }
                }
            }



            return template;


        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public TemplateVM GetAmharicTechnical(string id, string supplier)
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
            TemplateVM template = new TemplateVM();
            ReportTemplate templateFormat = _db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.TechnicalAm);

            if (id != null && supplier != null)
            {
                int lotid = int.Parse(id);
                Lot lot = _db.Lots.Find(lotid);
                if (lot != null)
                {
                    BidPurchasingSuppliers bidPurchasing = _db.BidPurchasingSupplierses.FirstOrDefault(x => x.SupplierName == supplier);
                    Tender tender = lot.Tender;
                    var supTechnical = lot.TenderTechnicals.Where(x => x.SupplierName == supplier);
                    var technical = lot.EvaluationCriterias.FirstOrDefault(x => x.Type == CriteriaType.Technical);
                    var averagevalue = supTechnical.Average(x => x.Result);
                    if (templateFormat != null)
                    {
                        template.Let1 = templateFormat.Temp2 + " ";
                        if (tender != null)
                        {
                            template.Let2 = " ለ " + tender.Description + " ";
                            template.Value1 = tender.TenderReference + " ";
                        }

                        template.Let3 = templateFormat.Temp3 + " ";
                        template.Let4 = " ከ ";
                        template.Let5 = templateFormat.Temp4 + " ";
                        template.Summary = templateFormat.Temp5 + " ";

                        if (technical != null) template.Value3 = technical.Weight.ToString();

                        template.Value2 = averagevalue.ToString();


                        template.Subject = templateFormat.Temp1;
                        template.City = " አዲስ አበባ ";
                        template.LogoFileName = record.LogoFileName;
                        template.Name = record.Name;
                        template.AmharicName = record.AmharicName;

                        if (bidPurchasing != null) template.To = bidPurchasing.SupplierName;
                    }
                }
            }


            return template;
        }

       

        [DataObjectMethod(DataObjectMethodType.Select)]
        public TemplateVM GetProforma(string id)
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
                int did = int.Parse(id);
                var winnerSupplier = _db.WinningSuppliers.FirstOrDefault(x => x.ProformaPurchaseID == did);
                var proformaPurchase = _db.ProformaPurchases.FirstOrDefault(x => x.ProformaPurchaseID == did);
                var team = _db.ProformaTeams.FirstOrDefault(x => x.ProformaPurchaseID == did);
                if (team != null)
                {
                    template.ProformaNote = team.Description; //teamdecision?.Description;
                }
                if (proformaPurchase != null)
                {
                    template.ProformaId = proformaPurchase.ProformaPurchaseID;
                    template.EvaluationDate = proformaPurchase.EvaluationDate.ToShortDateString();
                    #region prepared checker approver

                    template.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(proformaPurchase.CreatedBy));
                    template.PreparedSignature = SignatureHelper.GetUserSignature(proformaPurchase.CreatedBy);
                    if (proformaPurchase.DateCreated != null)
                        template.PreparedDate = proformaPurchase.DateCreated;

                    var purchaseValidations = _db.PurchaseValidations
                        .Where(x => x.ProformaPurchaseID == proformaPurchase.ProformaPurchaseID).ToList();

                    if (purchaseValidations.Any())
                    {
                        int maxid = purchaseValidations.Max(x => x.PurchaseValidationID);
                        var approver =
                            purchaseValidations.FirstOrDefault(x => x.PurchaseValidationID == maxid);
                        if (approver != null)
                        {
                            template.ApprovedBy =
                                EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(approver.CreatedBy));
                            template.ApprovedDate = approver.DateCreated;
                            template.ApprovedSignature = SignatureHelper.GetUserSignature(approver.CreatedBy);

                            var checker = purchaseValidations.OrderByDescending(x => x.PurchaseValidationID)
                                .FirstOrDefault(x => x.PurchaseValidationID < maxid);

                            if (checker != null)
                            {
                                template.CheckedBy =
                                    EmployeeHelper.GetEmployeeEnglishName(
                                        UserHelper.GetUserEmployeeId(checker.CreatedBy));
                                template.CheckedDate = checker.DateCreated;
                                template.CheckerSignature = SignatureHelper.GetUserSignature(checker.CreatedBy);
                            }
                            else
                            {
                                template.CheckedBy =
                                    EmployeeHelper.GetEmployeeEnglishName(
                                        UserHelper.GetUserEmployeeId(proformaPurchase.SendForApprovalBy));
                                template.CheckedDate = proformaPurchase.SendForApprovalTime;
                                template.CheckerSignature = SignatureHelper.GetUserSignature(proformaPurchase.SendForApprovalBy);

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


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SupplierListViewModel> GetPartcipantSupplier(string id)
        {
            List<SupplierListViewModel> supplierList = new List<SupplierListViewModel>();
            if (id != null)
            {

                int lotid = int.Parse(id);
                var participants = _db.BidPParticipatingSupplierses.Where(x => x.LotID == lotid).ToList();
                foreach (var participant in participants)
                {
                    var supplier = new SupplierListViewModel
                    {
                        SupplierName = participant.SupplierName,
                        ID = participant.LotID.ToString()
                    };

                    supplierList.Add(supplier);

                }
                var winner = _db.BidAwardedSuppliers.FirstOrDefault(x => x.LotID == lotid);
                var winnerSupplier = new SupplierListViewModel();
                if (winner != null) winnerSupplier.SupplierName = winner.SupplierName;
                supplierList.Remove(winnerSupplier);
            }
            return supplierList;


        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AwardItems> GetTechnicalItems(string supplier, string id)
        {
            List<AwardItems> awardItems = new List<AwardItems>();
            if (supplier != null && id != null)
            {

                int lotid = int.Parse(id);
                Lot lot = _db.Lots.Find(lotid);
                if (lot != null)
                {
                    var supplierAwarded = lot.TenderTechnicals.Where(x => x.SupplierName == supplier);
                    foreach (TenderTechnical tenderTechnical in supplierAwarded)
                    {
                        LotItems lotItems = lot.LotItems.FirstOrDefault(x => x.ItmCode == tenderTechnical.ItmCode);

                        InventoryItem inventoryItem = _db.InventoryItems.Find(tenderTechnical.ItmCode);
                        TenderTechnical technical =
                            lot.TenderTechnicals.FirstOrDefault(
                                x => x.ItmCode == tenderTechnical.ItmCode && x.SupplierName == supplier);


                        if (technical != null)
                        {
                            float result = technical.Result;

                            AwardItems award = new AwardItems();
                            if (inventoryItem != null) award.ItemName = inventoryItem.Name;
                            if (lotItems != null)
                            {
                                award.Quantity = lotItems.Quantity;
                                award.Unit = lotItems.UoM;
                            }

                            award.Result = result;

                            awardItems.Add(award);
                        }
                    }
                }
            }
            return awardItems;


        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public TemplateVM GetGuaranteeExtension(string id)
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
            ReportTemplate templateFormat = _db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.GuaranteeExtension);

            TemplateVM template = new TemplateVM();
            template.LogoFileName = record.LogoFileName;
            template.Name = record.Name;
            template.AmharicName = record.AmharicName;

            int guaranteeId = int.Parse(id);
            Guarantee guarantee = _db.Guarantees.Find(guaranteeId);

            if (guarantee != null)
            {
                DateTime issuedate = guarantee.IssueDate;
                string reference = guarantee.GuaranteeRefNo;
                var renewals = _db.GuaranteeRenewals.Where(x => x.GuaranteeId == guarantee.GuaranteeId).ToList();
                if (renewals.Any())
                {
                    var maxDate = renewals.Max(x => x.Date);
                    var recent = renewals.FirstOrDefault(x => x.Date == maxDate);
                    issuedate = maxDate;
                    if (recent != null) reference = recent.GuaranteeRefNo;
                }

                template.TenderRefNo = guarantee.GuaranteeRefNo;
                template.City = guarantee.BankBranch;
                template.Address = guarantee.Bank;
                var date = XAPI.EthiopicDateTime.GetEthiopicDate(issuedate.Day, issuedate.Month, issuedate.Year);
                template.Date = date;




                List<TenderEvaluationResult> tenderEvaluation = new List<TenderEvaluationResult>();

              
                

                if (templateFormat != null)
                {
                    template.Let1 = templateFormat.Temp2 + " ";
                    template.Let2 = templateFormat.Temp3 + " ";
                    template.Description = guarantee.Purpose + " ግዥ አስመልክቶ ";

                    template.TenderReference = " ቁጥር "+reference + " በቀን ";
                    template.SupplierName = guarantee.SupplierName + " ";
                   

                    template.Let3 = templateFormat.Temp4 + " ";
                    template.Let4 = templateFormat.Temp5 + " ";
                    template.Let5 = templateFormat.Temp6 + " ";
                    template.SupplierName = guarantee.SupplierName+ " ";
                    //template.Summary = templateFormat.Temp6 + " ";


                  

                    template.Subject = templateFormat.Temp1;



                    //template.City = guarantee.Bank;
                    //template.To = guarantee.SupplierName;



                }
            }


            return template;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public TemplateVM GetGuaranteeForfeited(string id)
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
            ReportTemplate templateFormat = _db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.GuaranteForfeited);

            TemplateVM template = new TemplateVM();
            template.LogoFileName = record.LogoFileName;
            template.Name = record.Name;
            template.AmharicName = record.AmharicName;

            int guaranteeId = int.Parse(id);
            Guarantee guarantee = _db.Guarantees.Find(guaranteeId);
            if (guarantee != null)
            {
                DateTime issuedate = guarantee.IssueDate;
                string reference = guarantee.GuaranteeRefNo;
                var renewals = _db.GuaranteeRenewals.Where(x => x.GuaranteeId == guarantee.GuaranteeId).ToList();
                if (renewals.Any())
                {
                    var maxDate = renewals.Max(x => x.Date);
                    var recent = renewals.FirstOrDefault(x => x.Date == maxDate);
                    issuedate = maxDate;
                    if (recent != null) reference = recent.GuaranteeRefNo;
                }

                template.TenderRefNo = guarantee.GuaranteeRefNo;
                template.City = guarantee.BankBranch;
                template.Address = guarantee.Bank;
                var date = XAPI.EthiopicDateTime.GetEthiopicDate(issuedate.Day, issuedate.Month, issuedate.Year);
                template.Date = date;



                
                List<TenderEvaluationResult> tenderEvaluation = new List<TenderEvaluationResult>();

               

                if (templateFormat != null)
                {
                    template.Let1 = templateFormat.Temp2 + " ";
                    template.Let2 = templateFormat.Temp3 + " ";
                    template.Description = guarantee.Purpose + " ግዥ አስመልክቶ ";

                    template.TenderReference = " ቁጥር " + reference + " በቀን ";
                    template.SupplierName = guarantee.SupplierName + " ";


                    template.Let3 = templateFormat.Temp4 + " ";
                    template.Let4 = templateFormat.Temp5 + " ";
                    template.Let5 = templateFormat.Temp6 + " ";
                    template.SupplierName = guarantee.SupplierName + " ";
                    //template.Summary = templateFormat.Temp6 + " ";


                    template.Subject = templateFormat.Temp1;



                    //template.City = guarantee.Bank;
                    //template.To = guarantee.SupplierName;


                }

            }


            return template;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public TemplateVM GetGuaranteeRelease(string id)
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
            ReportTemplate templateFormat = _db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.GuaranteeRelease);

            TemplateVM template = new TemplateVM();
            template.LogoFileName = record.LogoFileName;
            template.Name = record.Name;
            template.AmharicName = record.AmharicName;

            int guaranteeId = int.Parse(id);
            Guarantee guarantee = _db.Guarantees.Find(guaranteeId);
            if (guarantee != null)
            {
                DateTime issuedate = guarantee.IssueDate;
                string reference = guarantee.GuaranteeRefNo;
                var renewals = _db.GuaranteeRenewals.Where(x => x.GuaranteeId == guarantee.GuaranteeId).ToList();
                if (renewals.Any())
                {
                    var maxDate = renewals.Max(x => x.Date);
                    var recent = renewals.FirstOrDefault(x => x.Date == maxDate);
                    issuedate = maxDate;
                    if (recent != null) reference = recent.GuaranteeRefNo;
                }

                template.TenderRefNo = guarantee.GuaranteeRefNo;
                template.City = guarantee.BankBranch;
                template.Address = guarantee.Bank;
                var date = XAPI.EthiopicDateTime.GetEthiopicDate(issuedate.Day, issuedate.Month, issuedate.Year);
                template.Date = date;




                List<TenderEvaluationResult> tenderEvaluation = new List<TenderEvaluationResult>();



                if (templateFormat != null)
                {
                    template.Let1 = templateFormat.Temp2 + " ";
                    template.Let2 = templateFormat.Temp3 + " ";
                    template.Description = guarantee.Purpose + " ግዥ አስመልክቶ ";

                    template.TenderReference = " ቁጥር " + reference + " በቀን ";
                    template.SupplierName = guarantee.SupplierName + " ";


                    template.Let3 = templateFormat.Temp4 + " ";
                    template.Let4 = templateFormat.Temp5 + " ";
                    template.Let5 = templateFormat.Temp6 + " ";
                    template.SupplierName = guarantee.SupplierName + " ";
                    //template.Summary = templateFormat.Temp6 + " ";


                    template.Subject = templateFormat.Temp1;



                    //template.City = guarantee.Bank;
                    //template.To = guarantee.SupplierName;


                }

            }


            return template;

        }
    }
}
