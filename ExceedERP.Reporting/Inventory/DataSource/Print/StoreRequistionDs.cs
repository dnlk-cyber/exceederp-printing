using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Inventory.DataSource.Print
{
    [DataObject]
    public class StoreRequistionDs
    {
        ExceedDbContext _db = new ExceedDbContext();
        StoreRequistionHeader headerDs = new StoreRequistionHeader();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public StoreRequistionHeader GetStoreRequistionHeader(int id)
        {
            var record = _db.StoreRequisitions.Find(id);
            if (record != null)
            {
                
                var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
                int branchid = record.BranchId;
                if (record.IsForOther&&!record.IsTransfer)
                    if (record.ForBranchId != null)
                        branchid = (int) record.ForBranchId;
                if (record.IsTransfer)
                        branchid =InventoryHelper.GetStoreBranch(record.ToStoreCode);

                headerDs = new StoreRequistionHeader
                {
                    StoreRequsitionNo = InventoryPrintHelper.GetId(record.StoreRequisitionID),
                    WorkUnit = HrHelper.GetOrganizatioName(record.OrgStructureId),
                    Model =record.ModelNo,
                    RequestedDate = record.RequestDate.ToShortDateString(),
                    ApprovedDate = record.OnlineApprovedTime.GetValueOrDefault().ToShortDateString(),
                    PlateNo = record.PlateNo,
                    BranchName = InventoryPrintHelper.BranchName(branchid),
                    Requestor = InventoryPrintHelper.EmployeeName(record.EmployeeId),
                    StoreName = InventoryPrintHelper.StoreName(record.StoreCode),
                    CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.CreatedBy)),
                    Reviewer = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.SecondSendForApprovalBy)),
                    AuthorizedBy =EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.SecondOnlineApprovedBy)),
                    ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(record.OnlineApprovedBy))
                };

                headerDs.NoOfCopy = InventoryHelper.GetDocumentCopy(PrintCopyAttributes.STORE_REQUISITION);
                headerDs.CopyMembers = InventoryHelper.GetDocumentCopyList(PrintCopyAttributes.STORE_REQUISITION);

                if (organization != null)
                {
                    headerDs.Name = organization.TradeName;
                    headerDs.AmharicName = organization.AmharicName;
                    var address =
                        _db.Addresses.Where(
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
                        _db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                                                  a.ReferenceType == AttachmentType.LOGO);
                    if (logo != null)
                    {
                        headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }
              
            }
            return headerDs;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<StoreRequisitionItem> StoreRequisitionItems(int id)
        {
            List<StoreRequisitionItem> storeRequisitionItems = _db.StoreRequisitionItems.Where(x => x.StoreRequisitionID == id).ToList();
            return storeRequisitionItems;
        }
    }
}