using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Procurements.Print.Print
{
   
    [DataObject]
    public class InvitationLetterOds 
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public TemplateVM GetInvitationHeader(int id)
        {
            TemplateVM headerDs = new TemplateVM();
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
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

            ReportTemplate templateFormat = _db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.InvitationLetter);

            Lot lot = _db.Lots.Find(id);

           
            if (lot != null)
            {

                var tender = _db.Tenders.FirstOrDefault(x => x.TenderID == lot.TenderID);
                if (tender != null)
                {
                    headerDs.TenderRefNo = tender.TenderReference;
                    headerDs.Description = tender.Description + " ";
                    headerDs.TenderReference = tender.TenderReference + " ";
                }
                
            }
            
            if (templateFormat != null)
            {
                headerDs.Subject = templateFormat.Temp1;
                headerDs.Let1 = templateFormat.Temp2 + " ";
                headerDs.Let2 = templateFormat.Temp3 + " ";
                headerDs.Let3 = templateFormat.Temp4 + " ";
                headerDs.Let4 = templateFormat.Temp5 + " ";
                headerDs.Let5 = templateFormat.Temp6 + " ";
                headerDs.Let6 = templateFormat.Temp7 + " ";
                
               

            }

            return headerDs;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TranscriptViewModel> GetSelectedSupplierList(object[] transcript)
        {
            List<TranscriptViewModel> transcriptViewModelList = new List<TranscriptViewModel>();
            if (transcript != null)
            {
                if (transcript.Any())
                {
                    foreach (var supplierName in transcript)
                    {

                        var transcriptViewModel = new TranscriptViewModel
                        {
                            Name = supplierName.ToString()
                        };
                        transcriptViewModelList.Add(transcriptViewModel);
                    }


                }
            }

            return transcriptViewModelList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TranscriptViewModel> GetLetThemKnowList(object[] transcript)
        {
            List<TranscriptViewModel> transcriptViewModelList = new List<TranscriptViewModel>();
            if (transcript != null)
            {
                if (transcript.Any())
                {
                    foreach (var orgid in transcript)
                    {
                        var mainCompany = _db.MainCompanyStructures
                            .FirstOrDefault(x => x.OrgStructureId.ToString() == orgid.ToString());
                        var transcriptViewModel = new TranscriptViewModel
                        {
                            Name = mainCompany?.Name
                        };

                        transcriptViewModelList.Add(transcriptViewModel);
                    }


                }
            }

            return transcriptViewModelList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TranscriptViewModel> GetCcList(object[] transcript)
        {
            List<TranscriptViewModel> transcriptViewModelList = new List<TranscriptViewModel>();
            if (transcript != null)
            {
                if (transcript.Any())
                {
                    foreach (var ordid in transcript)
                    {
                        var mainCompany = _db.MainCompanyStructures
                            .FirstOrDefault(x => x.OrgStructureId.ToString() == ordid.ToString());
                        var transcriptViewModel = new TranscriptViewModel
                        {
                            Name = mainCompany?.Name
                        };

                        transcriptViewModelList.Add(transcriptViewModel);
                    }


                }
            }

            return transcriptViewModelList;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LotItems> GetLotItems(int lotid)
        {
            List<LotItems> list = new List<LotItems>();
            var lotitems = _db.LotItems.Where(x => x.LotID == lotid);
            foreach (var item in lotitems)
            {

                var transcriptViewModel = new LotItems
                {
                    ItmCode = item.ItmCode,
                    Name = item.ItemName+"-"+item.Name,
                    Quantity = item.Quantity,
                    UoM=item.UoM,
                };

                list.Add(transcriptViewModel);
            }

            return list;
        }
    }
}
