using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
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
    public class AwardOds
    {
       ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public TemplateVM GetAward(string id, string supplier)
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

            TemplateVM template = new TemplateVM();
            template.LogoFileName = record.LogoFileName;
            template.Name = record.Name;
            template.AmharicName = record.AmharicName;
            if (supplier != null)
            {
                int lotid = int.Parse(id);
                Lot lot = _db.Lots.Find(lotid);

                var tenders = _db.Tenders.FirstOrDefault(x => x.TenderID == lot.TenderID);
                if (tenders != null)
                {
                    template.TenderRefNo = tenders.TenderReference;
                }
                var winnerSupplier = _db.BidAwardedSuppliers.FirstOrDefault(x => x.LotID == lot.LotID);
                if (winnerSupplier != null)
                {
                    template.WinnerSupplier = winnerSupplier.SupplierName;
                }
                if (lot != null)
                {
                    var lotPurchaseRequisition = _db.LotPurchaseRequisitions.FirstOrDefault(x => x.LotID == lot.LotID);
                    if (lotPurchaseRequisition != null)
                    {
                        var bidPurchaseSupplier = _db.BidPurchasingSupplierses.FirstOrDefault(x => x.LotID == lot.LotID);

                        var purchaseRequisition = _db.PurchaseRequisitions.Where(x => x.PurchaseRequisitionID == lotPurchaseRequisition.PurchaseRequisitionID).ToList();

                        foreach (var purchase in purchaseRequisition)
                        {
                            var branch = _db.Branch.FirstOrDefault(x => x.BranchId == purchase.ForBranchId);
                            if (branch != null)
                            {

                                template.Branch = template.Branch + ", " + branch.Name;

                            }
                        }
                    }
                }
                BidPurchasingSuppliers bidPurchasing = _db.BidPurchasingSupplierses.FirstOrDefault(x => x.SupplierName == supplier);

                if (lot != null)
                {
                    List<TenderEvaluationResult> tenderEvaluation = new List<TenderEvaluationResult>();
                    var criteria = lot.EvaluationCriterias;

                    foreach (BidAwardedSupplier bidAwardedSupplier in lot.BidAwardedSupplier.Where(x => x.SupplierName == supplier))
                    {
                        var bidAwardedSupplierItem = _db.BidAwardedSupplierItems.FirstOrDefault(x => x.BidAwardedSupplierID == bidAwardedSupplier.BidAwardedSupplierID);
                        if (bidAwardedSupplierItem != null)
                        {
                            TenderEvaluationResult evaluationResult =
                                lot.EvaluationResults.FirstOrDefault(x => x.SupplierName == supplier);// && x.ItmCode == bidAwardedSupplierItem.ItmCode);
                            tenderEvaluation.Add(evaluationResult);
                            TenderEvaluationResult evaluationResultTec = lot.EvaluationResults.FirstOrDefault(x => x.SupplierName == supplier);
                            tenderEvaluation.Add(evaluationResultTec);
                        }

                    }
                    //template.To = supplier;

                    Tender tender = lot.Tender;
                    if (templateFormat != null)
                    {
                        template.Let1 = templateFormat.Temp2 + " ";
                        if (tender != null)
                        {

                            template.Let2 = templateFormat.Temp3 + " ";
                            template.Description = tender.Description + " ";
                            template.TenderReference = tender.TenderReference + " ";
                        }

                        template.Let3 = templateFormat.Temp4 + " ";
                        template.Let4 = templateFormat.Temp6 + " ";
                        //template.Let5 = templateFormat.Temp6 + " ";
                        template.Summary = templateFormat.Temp6 + " ";

                        if (criteria != null)
                        {
                            float weight = criteria.Sum(x => x.Weight);
                            template.Value3 = weight.ToString() + " ";
                        }
                        if (tenderEvaluation.Any())
                        {                            
                            //float result = tenderEvaluation.Average(x => x.Technical);//to be update
                            //template.Value2 = result.ToString() + " ";

                        }

                        template.Subject = templateFormat.Temp1;


                        if (bidPurchasing != null)
                        {
                            template.City = bidPurchasing.Address;
                            template.To = bidPurchasing.SupplierName;
                        }

                    }
                }
            }


            return template;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public TemplateVM GetAwardAmharic(string id, string supplier)
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


            ReportTemplate templateFormat = _db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.AwardAm);

            TemplateVM template = new TemplateVM();
            template.LogoFileName = record.LogoFileName;
            template.Name = record.Name;
            template.AmharicName = record.AmharicName;
            if (id != null)
            {
                int tenderId = int.Parse(id);

                if (supplier != null)
                {
                    Lot lot = _db.Lots.Find(tenderId);

                    var tenders = _db.Tenders.FirstOrDefault(x => x.TenderID == lot.TenderID);
                    if (tenders != null)
                    {
                        template.TenderRefNo = tenders.TenderReference;
                    }
                    var winnerSupplier = _db.BidAwardedSuppliers.FirstOrDefault(x => x.LotID == lot.LotID);
                    if (winnerSupplier != null)
                    {
                        template.WinnerSupplier = winnerSupplier.SupplierName;
                    }
                    if (lot != null)
                    {
                        var lotPurchaseRequisition = _db.LotPurchaseRequisitions.FirstOrDefault(x => x.LotID == lot.LotID);
                        if (lotPurchaseRequisition != null)
                        {
                            var bidPurchaseSupplier = _db.BidPurchasingSupplierses.FirstOrDefault(x => x.LotID == lot.LotID);

                            var purchaseRequisition = _db.PurchaseRequisitions.Where(x => x.PurchaseRequisitionID == lotPurchaseRequisition.PurchaseRequisitionID).ToList();

                            foreach (var purchase in purchaseRequisition)
                            {
                                var branch = _db.Branch.FirstOrDefault(x => x.BranchId == purchase.ForBranchId);
                                if (branch != null)
                                {

                                    template.Branch = template.Branch + ", " + branch.Name;

                                }
                            }
                        }
                    }

                    BidPurchasingSuppliers bidPurchasing = _db.BidPurchasingSupplierses.FirstOrDefault(x => x.SupplierName == supplier);

                    if (lot != null)
                    {
                        List<TenderEvaluationResult> tenderEvaluation = new List<TenderEvaluationResult>();
                        var criteria = lot.EvaluationCriterias;

                        foreach (BidAwardedSupplier bidAwardedSupplier in lot.BidAwardedSupplier.Where(x => x.SupplierName == supplier))
                        {
                            var bidAwardedSupplierItem = _db.BidAwardedSupplierItems.FirstOrDefault(x => x.BidAwardedSupplierID == bidAwardedSupplier.BidAwardedSupplierID);
                            if (bidAwardedSupplierItem != null)
                            {
                                TenderEvaluationResult evaluationResult =
                                    lot.EvaluationResults.FirstOrDefault(x => x.SupplierName == supplier);// && x.ItmCode == bidAwardedSupplierItem.ItmCode);
                                tenderEvaluation.Add(evaluationResult);
                                TenderEvaluationResult evaluationResultTec = lot.EvaluationResults.FirstOrDefault(x => x.SupplierName == supplier);
                                tenderEvaluation.Add(evaluationResultTec);
                            }

                        }


                        Tender tender = lot.Tender;
                        if (templateFormat != null)
                        {
                            template.Let1 = templateFormat.Temp2 + " ";
                            if (tender != null)
                            {
                                template.Let2 = templateFormat.Temp3 + " ";
                                template.Description = tender.Description + " ";
                                //template.Value1 = tender.TenderReference + " ";
                                template.TenderReference = tender.TenderReference + " ";
                            }

                            template.Let3 = templateFormat.Temp4 + " ";

                            template.Let4 = templateFormat.Temp5 + " ";
                            template.Let5 = "  ";
                            template.Summary = templateFormat.Temp6 + " ";

                            if (criteria != null)
                            {
                                float weight = criteria.Sum(x => x.Weight);
                                template.Value3 = weight.ToString() + " ";
                            }
                            if (tenderEvaluation.Count != 0)
                            {
                                //float result = tenderEvaluation.Average(x => x.Technical);//to be updated
                                //template.Value2 = result.ToString() + " ";

                            }

                            template.Subject = templateFormat.Temp1;

                            if (bidPurchasing != null)
                            {
                                template.City = bidPurchasing.Address;
                                template.To = bidPurchasing.SupplierName;
                            }

                        }
                    }
                }
            }

            return template;

        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AwardItems> GetAwardItems(string supplier, string id)
        {
            List<AwardItems> awardItems = new List<AwardItems>();
            if (supplier != null && id != null)
            {

                int lotid = int.Parse(id);

                var awardedSuppliers = _db.BidAwardedSuppliers.Where(x => x.LotID == lotid).Where(x => x.SupplierName == supplier).ToList();

                var award = new AwardItems();
                foreach (var bidAwardedSupplier in awardedSuppliers)
                {
                    var bidAwardedSupplierItems = _db.BidAwardedSupplierItems
                        .Where(x => x.BidAwardedSupplierID == bidAwardedSupplier.BidAwardedSupplierID);

                    foreach (var bidAwardedSupplierItem in bidAwardedSupplierItems)
                    {
                        var lotitem = _db.LotItems
                            .Where(x => x.LotID == lotid)
                            .Where(x => x.ItmCode == bidAwardedSupplierItem.ItmCode)
                            .FirstOrDefault(x => x.ItemSpecification == bidAwardedSupplierItem.ItemSpecification);

                        var result = _db.TenderEvaluationResults.Where(x => x.LotID == lotid).FirstOrDefault(x => x.SupplierName == supplier);//Where(x => x.ItmCode == bidAwardedSupplierItem.ItmCode).

                        award = new AwardItems
                        {
                            ItemName = bidAwardedSupplierItem.ItmCode + "_" + bidAwardedSupplierItem.ItemName + "_" + bidAwardedSupplierItem.Name + "_" + bidAwardedSupplierItem.ItemSpecification,
                            Spec=bidAwardedSupplierItem.ItemSpecification,
                            Quantity = bidAwardedSupplierItem.Quanity,
                            Unit = lotitem?.UoM,
                            Result = result?.Total ?? 0,
                            UnitPrice = bidAwardedSupplierItem.UPrice,
                            Discount = bidAwardedSupplierItem.Discount,
                            FinalUnitPrice = bidAwardedSupplierItem.FinalUPrice,
                           

                            VAT = bidAwardedSupplierItem.TaxAmount,
                            SubTotal = bidAwardedSupplierItem.Quanity * bidAwardedSupplierItem.UPrice,
                            FinalSubTotal = bidAwardedSupplierItem.Quanity * bidAwardedSupplierItem.FinalUPrice,
                            TotalPrice = (bidAwardedSupplierItem.Quanity*bidAwardedSupplierItem.FinalUPrice) +(bidAwardedSupplierItem.TaxAmount),
                            //SubTotal = bidAwardedSupplierItem.TPrice - bidAwardedSupplierItem.TaxAmount

                        };

                        awardItems.Add(award);
                    }

                }

            }
            return awardItems;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LotPurchaseRequisition> PrList(int id)
        {
            List<LotPurchaseRequisition> prList = _db.LotPurchaseRequisitions.Where(x => x.LotID == id).ToList();
            return prList;
        }
    }
}
