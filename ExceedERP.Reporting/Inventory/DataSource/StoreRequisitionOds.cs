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
    public class StoreRequisitionOds : List<StoreRequisitionVm>
    {
        private ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<StoreRequisitionVm> StoreRequisitionByDateInterval(DateTime from, DateTime to,string store,string workunit, string branch, string category,string item)
        {
            List<StoreRequisitionVm> storeRequisitionVms = new List<StoreRequisitionVm> { };
            try
            {

               var reportData = _db.StoreRequisitionItems.Where(x => x.StoreRequisition.IsOnlineApproved)
                   .Where(x => x.StoreRequisition.IsVoid == false)
                    .Where(x => x.StoreRequisition.RequestDate >= from)
                    .Where(x => x.StoreRequisition.RequestDate <= to).ToList();

                if (!String.IsNullOrEmpty(store))
                {
                    reportData = reportData
                        .Where(x => x.StoreRequisition.StoreCode == store).ToList();
                }

                if (!String.IsNullOrEmpty(branch))
                {
                    reportData = reportData
                        .Where(x => x.StoreRequisition.BranchId.ToString() == branch).ToList();
                }

                if (!String.IsNullOrEmpty(workunit))
                {
                    reportData = reportData
                        .Where(x => x.StoreRequisition.OrgStructureId.ToString() == workunit).ToList();
                }

                if (!String.IsNullOrEmpty(category))
                {
                    reportData = reportData.Where(x => x.ItemCategoryCode == category).ToList();
                }


                if (!String.IsNullOrEmpty(item))
                {
                    reportData = reportData.Where(x => x.ItemCode == item).ToList();
                }
                var uniqueCate = reportData.DistinctByField(x => x.ItemCategoryCode);
                int branchid = 0;
                foreach (var cat in uniqueCate)
                {
                    var requisitionVm = new StoreRequisitionVm
                    {
                        CategoryCode = cat.ItemCategoryCode,
                        CategoryName = cat.ItemCategory.Name,
                        CategoryTotal = 0,
                        StoreRequisitionItemList = new List<StoreRequisitionItemViewModel>()
                    };

                    foreach (var requisitionItem in reportData.Where(x => x.ItemCategoryCode == cat.ItemCategoryCode))
                    {
                        if (requisitionItem.StoreRequisition.IsTransfer)
                            branchid = InventoryHelper.GetStoreBranch(requisitionItem.StoreRequisition.ToStoreCode);
                        if (!requisitionItem.StoreRequisition.IsTransfer)
                            branchid = requisitionItem.StoreRequisition.IsForOther
                                ? (int)requisitionItem.StoreRequisition.ForBranchId
                                : requisitionItem.StoreRequisition.BranchId;

                        var requisitionItemViewModel = new StoreRequisitionItemViewModel
                        {
                            Id = requisitionItem.StoreRequisitionID.ToString(),
                            Reference = requisitionItem.StoreRequisition.Reference,
                            RequisitionDate = requisitionItem.StoreRequisition.RequestDate,
                            InventoryItem = requisitionItem.InventoryItem.Name,
                            RequestedQuantity = requisitionItem.RequestedQuantity,
                            ApprovedQuantity = requisitionItem.ApprovedQuantity,
                            RequestingDepartment = InventoryPrintHelper.WorkUnitName(requisitionItem.StoreRequisition.OrgStructureId),
                            ForProject = InventoryPrintHelper.BranchName(branchid),
                        };

                        requisitionVm.StoreRequisitionItemList.Add(requisitionItemViewModel);
                    }
                    requisitionVm.CategoryTotal = requisitionVm.StoreRequisitionItemList.Sum(x => x.ApprovedQuantity);
                    storeRequisitionVms.Add(requisitionVm);
                }
               
                return storeRequisitionVms;
            }
            catch (Exception)
            {
                return storeRequisitionVms;
            }
        }
    }
}