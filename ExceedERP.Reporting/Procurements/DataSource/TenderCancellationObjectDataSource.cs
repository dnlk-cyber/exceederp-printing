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
    public class TenderCancellationObjectDataSource : List<TenderCancellationVm>
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TenderCancellationVm> GetCanceledTender(string id)
        {
            List<TenderCancellationVm> cancledList = new List<TenderCancellationVm>();
            TenderCancellationVm cancled = new TenderCancellationVm();
            Organization record = new Organization();
            
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            if (organization != null)
            {
                cancled.Name = organization.TradeName;
                cancled.AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
                        a =>
                            a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Count > 0)
                {
                    foreach (var a in address)
                    {

                        if (cancled.Address == null)
                        {
                            cancled.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            cancled.Address += ", " + a.Category + "-" + a.Value;
                        }

                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    cancled.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            
          
            var tenders = db.Tenders.FirstOrDefault(x=>x.TenderID.ToString()==id);
            if(tenders!=null)
            {
                cancled.TenderRefNo = tenders.TenderReference;
              
                var lot = db.Lots.FirstOrDefault(x=>x.TenderID==tenders.TenderID);
                if(lot!=null)
                {
                    var lotPurchaseRequisition = db.LotPurchaseRequisitions.FirstOrDefault(x=>x.LotID==lot.LotID);
                    if(lotPurchaseRequisition!=null)
                    {
                        var bidPurchaseSupplier = db.BidPurchasingSupplierses.FirstOrDefault(x=>x.LotID==lot.LotID);
                        cancled.SupplierAddress = bidPurchaseSupplier.Address;
                        var purchaseRequisition = db.PurchaseRequisitions.Where(x=>x.PurchaseRequisitionID==lotPurchaseRequisition.PurchaseRequisitionID).ToList();
                      
                        foreach (var purchase in purchaseRequisition)
                        {
                            var branch = db.Branch.FirstOrDefault(x=>x.BranchId== purchase.ForBranchId);
                            if(branch!=null)
                            {
                                
                                cancled.Branch = cancled.Branch + ", " + branch.Name;
                               
                            }
                        }
                    }
                  
                }
                cancledList.Add(cancled);
            }


            if (id != null)
            {
                int tenderId = int.Parse(id);

                
                    Lot lot = db.Lots.Find(tenderId);
                  
                    if (lot != null)
                    {
                   
                    ReportTemplate templateFormat = db.ReportTemplates.FirstOrDefault(x => x.Status && x.Type == TemplateType.Cancellation);

                    Tender tender = lot.Tender;
                        if (templateFormat != null)
                        {
                        cancled.Let1 = templateFormat.Temp2 + " ";
                            if (tender != null)
                            {
                            cancled.Let2 = " ለ " + tender.Description + " ";
                            }

                        cancled.Let3 = templateFormat.Temp3 + " ";
                        cancled.Let4 = " ከ ";
                        cancled.Let5 = templateFormat.Temp4 + " ";
                        cancled.Summary = templateFormat.Temp5 + " ";
                        cancled.Subject = templateFormat.Temp1;                          

                        }
                    }
                
            }


            return cancledList;
        }
    }

}
