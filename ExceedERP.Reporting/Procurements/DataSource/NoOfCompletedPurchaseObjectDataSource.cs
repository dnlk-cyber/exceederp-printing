using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{
    [DataObject]
    public class NoOfCompletedPurchaseObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NoOfPurchasedRequisitionViewModel> GetNoOfCompletedPurchaseRequisition(DateTime from, DateTime to,string purchaser,string branchid)
        {
            List<NoOfPurchasedRequisitionViewModel> list =new List<NoOfPurchasedRequisitionViewModel>();
            var contract = db.ContractManagements.Where(x=>x.IsOnlineApproved && x.IsVoid==false).ToList();

            #region data

            var purchaseRequisitions = db.PurchaseRequisitions
                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Where(x => x.RequestDate >= from && x.RequestDate <= to)
                .AsQueryable();

            if (!String.IsNullOrWhiteSpace(branchid))
            {
                int did = int.Parse(branchid);
                purchaseRequisitions = purchaseRequisitions
                    .Where(x => x.BranchId ==did).AsQueryable();
            }

            if (!String.IsNullOrWhiteSpace(purchaser))
            {
                var prassignments = db.PurchaseRequisitionAssignments
                    .Where(x => x.ProcurementOfficer.ToString() == purchaser)
                    .Select(x => x.PurchaseRequisitionID)
                    .ToList();

                purchaseRequisitions = purchaseRequisitions.Where(x => prassignments.Contains(x.PurchaseRequisitionID))
                    .AsQueryable();
            }

            int proforma = 0;
            int openTender = 0;
            int selectedTender = 0;
            int directPurchase = 0;
            foreach (var purchaseRequisition in purchaseRequisitions)
            {
               
                var lotPurchaseRequest = db.LotPurchaseRequisitions.FirstOrDefault(x =>
                    x.PurchaseRequisitionID == purchaseRequisition.PurchaseRequisitionID);

                if (lotPurchaseRequest != null)
                {
                    var lot = db.Lots.FirstOrDefault(x => x.LotID == lotPurchaseRequest.LotID);
                    if (lot != null)
                    {
                        var tenderOpen = db.Tenders.Where(x => x.IsOnlineApproved)
                            .Where(x => x.IsVoid == false)
                            .FirstOrDefault(x => x.TenderID == lot.TenderID && x.TenderType == TenderType.Open);
                        if (tenderOpen != null)
                        {
                            var awards = db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID)
                                .Select(x => x.BidAwardedSupplierID).ToList();

                            var contractOpen = contract.Where(x => awards.Contains(x.ProcurementRefNo));
                            if(contractOpen.Any())
                              openTender++;
                        }
                        var tenderSelected = db.Tenders.Where(x => x.IsOnlineApproved)
                            .Where(x => x.IsVoid == false)
                            .FirstOrDefault(x => x.TenderID == lot.TenderID && x.TenderType == TenderType.Selected);
                        if (tenderSelected != null)
                        {
                            var awards = db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID)
                                .Select(x => x.BidAwardedSupplierID).ToList();

                            var contractSelected = contract.Where(x => awards.Contains(x.ProcurementRefNo));
                            if (contractSelected.Any())
                              selectedTender++;
                        }
                    }
                }
            }

            var noOfApprovedRequisitions = new NoOfPurchasedRequisitionViewModel
            {
                OpenTender = openTender,
                SelectedTender = selectedTender,
                Proforma = proforma,
                DirectPurchase = directPurchase
            };
            list.Add(noOfApprovedRequisitions);

            #endregion

            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PrViewModel> GetCompletedManualPoPrs(DateTime from, DateTime to, string purchaser,string branchid)
        {
            List<PrViewModel> list = new List<PrViewModel>();

            #region data

            var purchaseOrders = db.PurchaseOrders
                .Where(x => x.IsManual)
                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Where(x => x.OrderDate >= from && x.OrderDate <= to)
                .AsQueryable();

           

            if (!String.IsNullOrWhiteSpace(purchaser))
            {
                int did = int.Parse(purchaser);
                purchaseOrders = purchaseOrders
                    .Where(x => x.ProcurementOfficer==did)
                    .AsQueryable();
            }

            if (!String.IsNullOrWhiteSpace(branchid))
            {
                int did = int.Parse(branchid);
                purchaseOrders = purchaseOrders
                    .Where(x => x.BranchId == did)
                    .AsQueryable();
            }


           
            foreach (var purchaseOrder in purchaseOrders)
            {
                var prlist = purchaseOrder.PoPurchaseRequisitions.ToList();
                foreach (var requisition in prlist)
                {
                    var pr = new PrViewModel
                    {
                        PrNo = requisition.PurchaseRequisitionID.ToString(),
                        Remark = ""
                    };
                    list.Add(pr);
                }
               
                
            }

           

            #endregion

            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PrViewModel> GetCompletedProformaPoPrs(DateTime from, DateTime to, string purchaser, string branchid)
        {
            List<PrViewModel> list = new List<PrViewModel>();

            #region data

            var purchaseOrders = db.PurchaseOrders
                .Where(x => x.IsManual==false)
                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Where(x => x.OrderDate >= from && x.OrderDate <= to)
                .AsQueryable();

            if (!String.IsNullOrWhiteSpace(purchaser))
            {
                int did = int.Parse(purchaser);
                purchaseOrders = purchaseOrders.Where(x => x.ProcurementOfficer == did)
                    .AsQueryable();
            }

            if (!String.IsNullOrWhiteSpace(branchid))
            {
                int did = int.Parse(branchid);
                purchaseOrders = purchaseOrders.Where(x => x.BranchId == did)
                    .AsQueryable();
            }



            foreach (var purchaseOrder in purchaseOrders)
            {
                var prlist = purchaseOrder.PoPurchaseRequisitions.ToList();
                foreach (var requisition in prlist)
                {
                    var pr = new PrViewModel
                    {
                        PrNo = requisition.PurchaseRequisitionID.ToString(),
                        Remark = ""
                    };
                    list.Add(pr);
                }


            }



            #endregion

            return list;
        }
      

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PrViewModel> GetOpenTenderPrs(DateTime from, DateTime to, string purchaser, string branchid)
        {
            List<PrViewModel> list = new List<PrViewModel>();


            #region data

            var tenders = db.Tenders
                .Where(x => x.TenderType == TenderType.Open)
                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Where(x => x.FloatDate >= from && x.FloatDate <= to)
                .AsQueryable();

            if (!String.IsNullOrWhiteSpace(purchaser))
            {
                int did = int.Parse(purchaser);
                tenders = tenders.Where(x => x.ProcurementOfficer==did)
                    .AsQueryable();
            }

            if (!String.IsNullOrWhiteSpace(branchid))
            {
                int did = int.Parse(branchid);

                tenders = tenders.Where(x => x.BranchId == did)
                    .AsQueryable();
            }



            foreach (var tender in tenders)
            {
                var lots = tender.Lots.ToList();
                foreach (var lot in lots)
                {
                    var awards = db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID)
                        .Select(x => x.BidAwardedSupplierID).ToList();

                    var contractOpen = db.ContractManagements.Where(x => awards.Contains(x.ProcurementRefNo));
                    if (contractOpen.Any())
                    {
                        var prlist = lot.LotPurchaseRequisitions.ToList();
                        foreach (var requisition in prlist)
                        {
                            var pr = new PrViewModel
                            {
                                PrNo = requisition.PurchaseRequisitionID.ToString(),
                                Remark = ""
                            };
                            list.Add(pr);
                        }
                    }
                }


            }



            #endregion

            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PrViewModel> GetSelectedTenderPrs(DateTime from, DateTime to, string purchaser, string branchid)
        {
            List<PrViewModel> list = new List<PrViewModel>();


            #region data

            var tenders = db.Tenders
                .Where(x => x.TenderType == TenderType.Selected)
                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Where(x => x.FloatDate >= from && x.FloatDate <= to)
                .AsQueryable();

            if (!String.IsNullOrWhiteSpace(purchaser))
            {
                int did = int.Parse(purchaser);
                tenders = tenders.Where(x => x.ProcurementOfficer == did)
                    .AsQueryable();
            }

            if (!String.IsNullOrWhiteSpace(branchid))
            {
                int did = int.Parse(branchid);
                tenders = tenders.Where(x => x.BranchId == did)
                    .AsQueryable();
            }



            foreach (var tender in tenders)
            {
                var lots = tender.Lots.ToList();
                foreach (var lot in lots)
                {
                    var awards = db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID)
                        .Select(x => x.BidAwardedSupplierID).ToList();

                    var contractOpen = db.ContractManagements.Where(x => awards.Contains(x.ProcurementRefNo));
                    if (contractOpen.Any())
                    {
                        var prlist = lot.LotPurchaseRequisitions.ToList();
                        foreach (var requisition in prlist)
                        {
                            var pr = new PrViewModel
                            {
                                PrNo = requisition.PurchaseRequisitionID.ToString(),
                                Remark = ""
                            };
                            list.Add(pr);
                        }
                    }
                }


            }



            #endregion

            return list;
        }


    }
}