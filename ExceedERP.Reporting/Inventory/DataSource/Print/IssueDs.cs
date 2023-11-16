using System;
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
    public class IssueDs
    {
        ExceedDbContext dbContext = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public IssueHeader GetIssue(int id)
        {
            Issue issue = dbContext.Issues.Find(id);
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
                headerDs.ApprovedDate = issue.OnlineApprovedTime;
                headerDs.Project = InventoryPrintHelper.BranchName(issue.BranchId);
                headerDs.Department = InventoryPrintHelper.WorkUnitName(issue.OrgStructureId);
                headerDs.Datecreated = issue.DateCreated.GetValueOrDefault().ToShortDateString();
                headerDs.IssueTo = InventoryPrintHelper.WorkUnitName(issue.OrgStructureId);

                headerDs.StoreRequistionNo = InventoryPrintHelper.GetId(issue.StoreRequisitionID);
                if(string.IsNullOrWhiteSpace(issue.SupplierId))
                {
                    var supplier = dbContext.ProcurementSuppliers.Where(x => x.SupplierId.ToUpper() == issue.SupplierId.ToUpper()).FirstOrDefault();
                    if (supplier != null)
                        headerDs.ExternalSupplier = supplier.SupplierId + "-" + supplier.Name;
                }
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
        public List<IssueItem> IssueItems(int id)
        {
            List<IssueItem> issueItems = dbContext.IssueItems.Where(x => x.IssueID == id).ToList();
            if (InventoryConfigurationHelper.IsHidePriceOnPrintEnabled())
            {
                foreach (var item in issueItems)
                {
                    item.UnitPrice = 0;
                    item.Total = 0;
                }
            }
            return issueItems;
        }
    }
}