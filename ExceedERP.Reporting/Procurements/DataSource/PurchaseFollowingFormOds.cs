using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{

    [DataObject]
    public class PurchaseFollowingFormOds
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PurchaseFollowingForm> GetPurchaseFollow(string supplier, DateTime from, DateTime to, string purchaser)
        {
            List<PurchaseFollowingForm> purchaseFollowingList = new List<PurchaseFollowingForm>();
            var purchaseFollowing = new PurchaseFollowingForm();
            var purchaseRequisition = db.PurchaseRequisitions.Where(x => x.RequestDate >= from && x.RequestDate <= to).ToList();

            if (!String.IsNullOrWhiteSpace(purchaser))
            {

                var prassignments = db.PurchaseRequisitionAssignments
                    .Where(x => x.ProcurementOfficer.ToString() == purchaser).Select(x => x.PurchaseRequisitionID)
                    .ToList();
                purchaseRequisition = purchaseRequisition.Where(x => prassignments.Contains(x.PurchaseRequisitionID))
                    .ToList();
            }

            var vat = db.PurchaseOrderItems.FirstOrDefault()?.TaxAmount;
            purchaseFollowing.Vat = Convert.ToInt32(vat);
            foreach (var requisition in purchaseRequisition)
            {
                purchaseFollowing = new PurchaseFollowingForm();
                var branch = db.Branch.FirstOrDefault(x => x.BranchId == requisition.BranchId);

                purchaseFollowing.Branch = branch?.Name;
                purchaseFollowing.RequisitionNo = requisition.ReferenceNo;

                var lotPurchaseRequisition = db.LotPurchaseRequisitions.FirstOrDefault(x =>
                    x.PurchaseRequisitionID == requisition.PurchaseRequisitionID);
                if (lotPurchaseRequisition != null)
                {
                    var lotRequisition = db.Lots.FirstOrDefault(x => x.LotID == lotPurchaseRequisition.LotID);
                    if (lotRequisition != null)
                    {
                        var tenders = db.Tenders.FirstOrDefault(x => x.TenderID == lotRequisition.TenderID);
                    }

                }



                purchaseFollowing.ItemType = requisition.ItemType;

                var purchaseOrder =
                    db.PurchaseOrders.FirstOrDefault(x => x.PurchaseRequisitionID == requisition.PurchaseRequisitionID);
                if (purchaseOrder != null)
                {
                    var purcaseOrderItem =
                    db.PurchaseOrderItems.FirstOrDefault(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID);


                    purchaseFollowing.DeliveryDate = purchaseOrder?.DeliveryDate;
                    purchaseFollowing.ItemAmount = purcaseOrderItem?.Quantity;
                    purchaseFollowing.UnitPrice = purcaseOrderItem?.UnitPrice;
                    purchaseFollowing.TotalPrice = purcaseOrderItem?.Total;
                    purchaseFollowing.TransportPaymentSendingDate = DateTime.Now;
                    purchaseFollowing.TransportPanymentDate = DateTime.Now;
                    purchaseFollowing.ItemPaymentSendingDate = DateTime.Now;
                    purchaseFollowing.ItemRecieptNo = purcaseOrderItem?.GLTaxRateID;
                    purchaseFollowing.NoOfItemRecieved = 0;
                    purchaseFollowing.ItemLeft = 0;
                    purchaseFollowing.Vat = purcaseOrderItem.TaxAmount;
                    purchaseFollowing.SubTotal = purcaseOrderItem.Total - purcaseOrderItem.TaxAmount;


                    purchaseFollowingList.Add(purchaseFollowing);
                }
            }


            return purchaseFollowingList;
        }
    }
}
