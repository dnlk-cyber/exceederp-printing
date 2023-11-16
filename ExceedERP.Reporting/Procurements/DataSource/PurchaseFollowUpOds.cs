using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Procurements.ViewModel;

namespace ExceedERP.Reporting.Procurements.DataSource
{

    [DataObject]
    public class PurchaseFollowUpOds
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PurchaseFollowUpReportViewModel> GetPurchases(DateTime from, DateTime to,string purchaser, string branchid, string purchaseType, string businessUnitId)
        {
            string purchasetype = "All";

            List<PurchaseFollowUpReportViewModel> list = new List<PurchaseFollowUpReportViewModel>();

            var branch = db.Branch.ToList();

            var purchaseRequisitions = db.PurchaseRequisitions
                .Where(x => x.IsSecondOnlineApproved && x.IsVoid == false)
                .Where(x => x.RequestDate >= from && x.RequestDate <= to)
                .Include(x => x.PurchaseRequisitionAssignments)
                .AsQueryable();

            if (!String.IsNullOrWhiteSpace(purchaser))
            {
                int purchaserD = int.Parse(purchaser);
                var prassignments = db.PurchaseRequisitionAssignments
                    .Where(x => x.ProcurementOfficer== purchaserD).Select(x => x.PurchaseRequisitionID)
                    .ToList();
                purchaseRequisitions = purchaseRequisitions.Where(x => prassignments.Contains(x.PurchaseRequisitionID))
                    .AsQueryable();
            }

            if (!String.IsNullOrWhiteSpace(branchid))
            {
                int did = int.Parse(branchid);
                purchaseRequisitions = purchaseRequisitions
                    .Where(x => x.BranchId == did).AsQueryable();
            }

            if (!String.IsNullOrEmpty(businessUnitId))
            {
                int bdid = int.Parse(businessUnitId);
                purchaseRequisitions = purchaseRequisitions.Where(x => x.BusinessUnitId == bdid).AsQueryable();
            }


            foreach (var purchaseRequisition in purchaseRequisitions)
            {
                
                var assignments =
                    purchaseRequisition.PurchaseRequisitionAssignments.Where(x =>
                        x.IsOnlineApproved && x.IsVoid == false);

                if (!String.IsNullOrWhiteSpace(purchaser))
                {
                    int purchaserD = int.Parse(purchaser);
                    assignments = assignments.Where(x => x.ProcurementOfficer == purchaserD).ToList();
                }
                if (assignments.Any())
                {
                    foreach (var assignment in assignments)
                    {
                        var requisition = new PurchaseFollowUpReportViewModel
                        {
                            PurchaseRequistionId = purchaseRequisition.PurchaseRequisitionID,
                            ApplicationDate = purchaseRequisition.RequestDate,
                            ProcurementMethod = assignment.ProcurementMethod.ToString(),

                        };

                        if (assignment.OnlineApprovedTime != null)
                            requisition.StartingDate = (DateTime)assignment.OnlineApprovedTime;

                        var branchName = branch.FirstOrDefault(x => x.BranchId == purchaseRequisition.BranchId);
                        if (branchName != null)
                        {
                            requisition.Branch = branchName.Name;
                        }
                        if ((assignment.ProcurementMethod == ProcurementMethod.DirectPurchase) && (purchasetype == "Direct Purchase" || purchasetype == "All"))
                        {

                            var poPr = db.PoPurchaseRequisitions
                                .FirstOrDefault(x => x.PurchaseRequisitionID == assignment.PurchaseRequisitionID);
                            if (poPr != null)
                            {
                                var po = db.PurchaseOrders.Find(poPr.PurchaseOrderID);

                                if (po != null)
                                {
                                    requisition.TechnicalOpen = po.OrderDate;
                                    requisition.FinancialOpen = po.OrderDate;
                                }
                            }
                            list.Add(requisition);
                        }

                        else if ((assignment.ProcurementMethod == ProcurementMethod.Proforma) && (purchasetype == "Proforma" || purchasetype == "All"))
                        {
                            #region proforma

                            var proformaPr = db.ProformaPurchaseRequisitions
                                .FirstOrDefault(x => x.PurchaseRequisitionID == assignment.PurchaseRequisitionID);

                            if (proformaPr != null)
                            {
                                var proforma = db.ProformaPurchases.Find(proformaPr.ProformaPurchaseID);

                                if (proforma != null)
                                {
                                    requisition.TechnicalOpen = proforma.EvaluationDate;
                                    requisition.FinancialOpen = proforma.EvaluationDate;
                                }
                            }

                            list.Add(requisition);
                            #endregion
                        }
                        else if ((assignment.ProcurementMethod == ProcurementMethod.Tender) && (purchasetype == "Tender" || purchasetype == "All"))
                        {
                            #region tender

                            var lotPurchaseRequisition = db.LotPurchaseRequisitions
                                .FirstOrDefault(x => x.PurchaseRequisitionID == assignment.PurchaseRequisitionID);

                            if (lotPurchaseRequisition != null)
                            {
                                var tender = db.Tenders.Find(lotPurchaseRequisition.Lot.TenderID);
                                if (tender != null)
                                {
                                    if (tender.TechnicalOpenigDate != null)
                                        requisition.TechnicalOpen = (DateTime)tender.TechnicalOpenigDate;

                                    if (tender.FinancialOpenigDate != null)
                                        requisition.FinancialOpen = (DateTime)tender.FinancialOpenigDate;
                                }
                            }

                            list.Add(requisition);

                            #endregion
                        }

                    }
                }
                else
                {
                    var requisition = new PurchaseFollowUpReportViewModel
                    {
                        PurchaseRequistionId = purchaseRequisition.PurchaseRequisitionID,
                        ApplicationDate = purchaseRequisition.RequestDate,
                        TechnicalOpen = null,
                        FinancialOpen=null

                    };

                    list.Add(requisition);
                }
                
            }
            return list;
        }
    }
}
