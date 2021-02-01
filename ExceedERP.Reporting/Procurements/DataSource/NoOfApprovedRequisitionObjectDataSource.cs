using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{

    [DataObject]
    public class NoOfApprovedRequisitionObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NoOfApprovedRequisitionViewModel> GetNoOfApprovedRequisition(DateTime from, DateTime to, string purchaser)
        {
            List<NoOfApprovedRequisitionViewModel> noOfApprovedRequisitionList = new List<NoOfApprovedRequisitionViewModel>();
            var purchase = db.PurchaseRequisitions.Where(x => x.RequestDate >= from && x.RequestDate <= to).ToList();
            var branch = db.Branch.ToList();

            if (!String.IsNullOrWhiteSpace(purchaser))
            {

                var prassignments = db.PurchaseRequisitionAssignments
                    .Where(x => x.ProcurementOfficer.ToString() == purchaser).Select(x => x.PurchaseRequisitionID)
                    .ToList();

                purchase = purchase
                    .Where(x => prassignments.Contains(x.PurchaseRequisitionID))
                    .ToList();
            }

            foreach (var purchaseRequisition in purchase)
            {

                var noOfApprovedRequisition = new NoOfApprovedRequisitionViewModel
                {
                    Branch = branch.FirstOrDefault(x => x.BranchId == purchaseRequisition.ForBranchId)?.Name,
                    ItemType = purchaseRequisition.ItemType
                };


                var lotPurchase = db.LotPurchaseRequisitions
                    .Where(x => x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID).ToList();
                foreach (var lotPurchaseRequisition in lotPurchase)
                {
                    var lot = db.Lots.Where(x => x.LotID == lotPurchaseRequisition.LotID).ToList();
                    foreach (var lot1 in lot)
                    {
                        var tenderOpen = db.Tenders.FirstOrDefault(x => x.TenderID == lot1.TenderID && x.TenderType == TenderType.Open);
                        var tenderSelected = db.Tenders.FirstOrDefault(x => x.TenderID == lot1.TenderID && x.TenderType == TenderType.Selected);

                        if (tenderOpen != null)
                        {
                            noOfApprovedRequisition.OpenTender = db.Lots.Where(x => x.TenderID == tenderOpen.TenderID)?.Count();
                        }

                        if (tenderSelected!=null)
                        {
                            noOfApprovedRequisition.SelectedTender = db.Lots.Where(x => x.TenderID == tenderSelected.TenderID)?.Count();
                        }
                    }
                }

               //noOfApprovedRequisition.OpenTender = db.Tenders.Where(x => x.OpenigDate >= from && x.ClosingDate <= to && x.TenderType == TenderType.Open)?.Count();
               // noOfApprovedRequisition.SelectedTender = db.Tenders.Where(x => x.OpenigDate >= from && x.ClosingDate <= to && x.TenderType == TenderType.Selected)?.Count();
                noOfApprovedRequisition.DirectPurchase = db.PurchaseOrders.Where(x =>
                    x.OrderDate >= from && x.DeliveryDate <= to &&
                    x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID)?.Count();

                var proforma = db.ProformaPurchases.Where(x =>
                    x.ClosingDate >= from && x.ClosingDate <= to &&
                    x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID);
                foreach (var proformaPurchase in proforma)
                {
                   noOfApprovedRequisition.Proforma =
                        db.ProformaItems.Where(x => x.ProformaPurchaseID == proformaPurchase.ProformaPurchaseID && x.IsApproved)?.Count();
                   
                }

            

                noOfApprovedRequisitionList.Add(noOfApprovedRequisition);
            }

            return noOfApprovedRequisitionList;
        }
    }
}
