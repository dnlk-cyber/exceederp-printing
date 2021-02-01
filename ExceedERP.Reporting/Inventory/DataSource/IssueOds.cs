using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Inventory.ViewModel;

namespace ExceedERP.Reporting.Inventory.DataSource
{
    [DataObject]
    public class IssueOds : List<IssueVm>
    {
        ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<IssueVm> GetIssueItemByDateInterval(DateTime from, DateTime to , string store,string category,string item,string plateNo,string workunit)
        {
            List<IssueVm> issueItemList = new List<IssueVm> { };
            try
            {
                var reportData = _db.IssueItems
                    .Where(x => x.Issue.IsOnlineApproved)
                    .Where(x => x.Issue.IsVoid==false)
                    .Where(x => x.Issue.IssueDate >= from)
                    .Where(x => x.Issue.IssueDate <= to).ToList();

                if (!String.IsNullOrEmpty(store))
                {
                    reportData = reportData
                        .Where(x => x.Issue.StoreCode== store).ToList();
                }

                if (!String.IsNullOrEmpty(category))
                {
                    reportData = reportData.Where(x => x.ItemCategoryCode==category).ToList();
                }

                if (!String.IsNullOrEmpty(item))
                {
                    reportData = reportData.Where(x => x.ItemCode == item).ToList();
                }

                if (!String.IsNullOrEmpty(plateNo))
                {
                    reportData = reportData
                        .Where(x => x.Issue.PlateNo == plateNo).ToList();
                }

                if (!String.IsNullOrEmpty(workunit))
                {
                    reportData = reportData
                        .Where(x => x.Issue.OrgStructureId.ToString() == workunit).ToList();
                }


                var uniqueCate = reportData.DistinctByField(x => x.ItemCategoryCode);
                foreach (var cat in uniqueCate)
                {
                    var issueVm = new IssueVm
                    {
                        CategoryCode =cat.ItemCategoryCode,
                        CategoryName = cat.ItemCategory.Name,
                        CategoryTotal = 0,
                        IssueItemList = new List<IssueItemViewModel>()
                    };

                    foreach (var issueItem in reportData.Where(x=>x.ItemCategoryCode==cat.ItemCategoryCode))
                    {
                        var issueItemRecord = new IssueItemViewModel
                        {
                            Id = issueItem.IssueID.ToString(),
                            ToDepartment = InventoryPrintHelper.WorkUnitName(issueItem.Issue.OrgStructureId),
                            Reference = issueItem.Issue.Reference,
                            IssueDate = issueItem.Issue.IssueDate,
                            InventoryItem = issueItem.ItemCode + "-" + issueItem.InventoryItem.Name,
                            Quantity = issueItem.IssuedQuantity,
                            UnitPrice = issueItem.UnitPrice,
                            Total = issueItem.Total,
                            PlateNo = issueItem.Issue.PlateNo,
                            StoreName = issueItem.Issue.StoreCode,
                            WorkItemCode = issueItem.WorkItemCode,
                            Branch= InventoryPrintHelper.BranchName(issueItem.Issue.BranchId),
                            ForBranch = InventoryPrintHelper.BranchName(issueItem.Issue.ForBranchId),
                        };
                        issueVm.IssueItemList.Add(issueItemRecord);
                    }
                    issueVm.CategoryTotal = issueVm.IssueItemList.Sum(x => x.Total);
                    issueItemList.Add(issueVm);
                }
               
                return issueItemList;
            }
            catch (Exception)
            {
                return issueItemList;
            }
        }
    }
}