using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Inventory.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class PurchaseRequisitionOds : List<PurchaseRequisitionVm>
    {
        private ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PurchaseRequisitionVm> PurchaseRequisitionByDateInterval(DateTime from, DateTime to, string category,string item, string store, string branch)
        {
            List<PurchaseRequisitionVm> purchaseRequisitionVms = new List<PurchaseRequisitionVm> { };
            try
            {

               var reportData = _db.PurchaseRequisitionItems.Where(x => x.PurchaseRequisition.IsOnlineApproved)
                   .Where(x => x.PurchaseRequisition.IsVoid == false)
                    .Where(x => x.PurchaseRequisition.RequestDate >= from)
                    .Where(x => x.PurchaseRequisition.RequestDate <= to).ToList();

                //no store in PR

                //if (!String.IsNullOrEmpty(store))
                //{
                //    reportData = reportData
                //        .Where(x => x.PurchaseRequisition.StoreCode == store).ToList();
                //}

                if (!String.IsNullOrEmpty(branch))
                {
                    reportData = reportData
                        .Where(x => x.PurchaseRequisition.BranchId.ToString() == branch).ToList();
                }

                if (!String.IsNullOrEmpty(category))
                {
                    reportData = reportData.Where(x => x.CatCode == category).ToList();
                }


                if (!String.IsNullOrEmpty(item))
                {
                    reportData = reportData.Where(x => x.ItmCode == item).ToList();
                }
                var uniqueCate = reportData.DistinctByField(x => x.CatCode);
                int branchid = 0;
                foreach (var cat in uniqueCate)
                {
                    var requisitionVm = new PurchaseRequisitionVm
                    {
                        CategoryCode = cat.CatCode,
                        CategoryName = cat.CategoryName,
                        CategoryTotal = 0,
                        PurchaseRequisitionItemVms = new List<PurchaseRequisitionItemVm>()
                    };

                    foreach (var requisitionItem in reportData.Where(x => x.CatCode == cat.CatCode))
                    {
                        //if (requisitionItem.PurchaseRequisition.IsTransfer)
                        //    branchid = InventoryHelper.GetStoreBranch(requisitionItem.StoreRequisition.ToStoreCode);
                        //if (!requisitionItem.StoreRequisition.IsTransfer)
                        //    branchid = requisitionItem.StoreRequisition.IsForOther
                        //        ? (int)requisitionItem.StoreRequisition.ForBranchId
                        //        : requisitionItem.StoreRequisition.BranchId;

                        var requisitionItemViewModel = new PurchaseRequisitionItemVm
                        {
                            Id = requisitionItem.PurchaseRequisitionID.ToString(),
                            Reference = requisitionItem.PurchaseRequisition.ReferenceNo,
                            RequisitionDate = requisitionItem.PurchaseRequisition.RequestDate,
                            ItemName = requisitionItem.ItemName,
                            Quantity = requisitionItem.Quantity,
                            //ApprovedQuantity = requisitionItem.ApprovedQuantity,
                           // RequestingDepartment = InventoryPrintHelper.WorkUnitName(requisitionItem.StoreRequisition.OrgStructureId),
                           // ForProject = InventoryPrintHelper.BranchName(branchid),
                        };

                        requisitionVm.PurchaseRequisitionItemVms.Add(requisitionItemViewModel);
                    }
                    requisitionVm.CategoryTotal = requisitionVm.PurchaseRequisitionItemVms.Sum(x => x.Quantity);
                    purchaseRequisitionVms.Add(requisitionVm);
                }
               
                return purchaseRequisitionVms;
            }
            catch (Exception)
            {
                return purchaseRequisitionVms;
            }
        }
    }
}