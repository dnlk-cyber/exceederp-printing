using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Inventory.DataSource.Print
{
    [DataObject]
    public class IssueFuelDs
    {
        ExceedDbContext dbContext = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IssueHeader GetIssueFuel(int id)
        {
            var issue = dbContext.Issues.Find(id);
            var organization = dbContext.Companies.FirstOrDefault(x => x.Category == "0");

            IssueHeader headerDs = new IssueHeader();
            if (issue != null)
            {
                headerDs.IssueNumber = InventoryPrintHelper.GetId(issue.IssueID);
                headerDs.IssueDate = issue.IssueDate.ToShortDateString();
                // headerDs.IssueDate = issue.OnlineApprovedTime.GetValueOrDefault().ToShortDateString();
                headerDs.Requestor = EmployeeHelper.GetEmployeeEnglishName(issue.EmployeeId);
                headerDs.Recipient = EmployeeHelper.GetEmployeeEnglishName(issue.RecipientId);
                headerDs.StoreName = InventoryPrintHelper.StoreName(issue.StoreCode);
                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(issue.CreatedBy));
                headerDs.ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(issue.OnlineApprovedBy));
                headerDs.Project = InventoryPrintHelper.BranchName(issue.BranchId);
                headerDs.Department = InventoryPrintHelper.WorkUnitName(issue.OrgStructureId);
                headerDs.Datecreated = issue.DateCreated.GetValueOrDefault().ToShortDateString();
                if (issue.StoreRequisitionID != 0)
                {
                    var sr = dbContext.StoreRequisitions.Find(issue.StoreRequisitionID);
                    headerDs.IssueTo = sr?.OrgStructureId.ToString();
                    headerDs.ExternalDriver = sr?.ExternalDriver;
                }
               
                headerDs.ODOMeter = issue.ReadingKM;
                headerDs.PlateNo = issue.PlateNo; 
                headerDs.StoreRequistionNo = InventoryPrintHelper.GetId(issue.StoreRequisitionID);
            }

            headerDs.NoOfCopy = InventoryHelper.GetDocumentCopy(PrintCopyAttributes.STORE_ISSUE_VOUCHER);
            headerDs.CopyMembers = InventoryHelper.GetDocumentCopyList(PrintCopyAttributes.STORE_ISSUE_VOUCHER);

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    dbContext.Addresses.Where(
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
                    dbContext.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                                              a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return headerDs;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<IssueItem> IssueFuelItems(int id)
        {
            List<IssueItem> issueItems = dbContext.IssueItems.Where(x => x.IssueID == id).ToList();
            foreach (var issueitem in issueItems)
            {
                InventoryItem inventoryitem = dbContext.InventoryItems.Find(issueitem.ItemCode);
                if (inventoryitem != null) issueitem.ItemCode = inventoryitem.Name;
                if (InventoryConfigurationHelper.IsHidePriceOnPrintEnabled())
                {
                    issueitem.UnitPrice = 0;
                    issueitem.Total = 0;
                }
            }
            return issueItems;
        }    
    }
}