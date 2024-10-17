using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.FixedAsset.Print.ViewModel;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.Print.DataSource
{
    [DataObject]
    public class FixedAssetTransferOds
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetTransferHeader GetAssetTransfer(int id)
        {
            var assetTransfer = db.AssetTransfers.Find(id);
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            FixedAssetTransferHeader headerDs = new FixedAssetTransferHeader();


            if (assetTransfer != null)
            {
                headerDs.StoreRequistionNo = assetTransfer.FixedAssetRequisitionId.ToString();
                headerDs.TransferDescription = assetTransfer.TransferDesc;
                
                headerDs.TransferNumber = InventoryPrintHelper.GetId(assetTransfer.AssetTransferId);
                headerDs.TransferType = assetTransfer.Type.ToString();
                headerDs.FromStore = assetTransfer.FromStoreCode+"-"+InventoryPrintHelper.StoreName(assetTransfer.FromStoreCode);
                headerDs.ToStore = assetTransfer.ToStoreCode + "-" + InventoryPrintHelper.StoreName(assetTransfer.ToStoreCode);
                if(assetTransfer.CustodianBeforeTransfer!=null)
                 headerDs.FromEmployee = EmployeeHelper.GetEmployeeEnglishName((int)assetTransfer.CustodianBeforeTransfer);
                if (assetTransfer.TransferTo != null)
                    headerDs.ToEmployee = EmployeeHelper.GetEmployeeEnglishName((int)assetTransfer.TransferTo);
                headerDs.TransferDate = assetTransfer.TransferDate.ToShortDateString();
                
                headerDs.StoreKeeperName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetTransfer.CreatedBy));

                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetTransfer.CreatedBy));
                headerDs.PreparedDate = assetTransfer.DateCreated != null ? assetTransfer.DateCreated : null;
                headerDs.PreparedBySignature = SignatureHelper.GetUserSignature(assetTransfer.CreatedBy);

                headerDs.Approver = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetTransfer.OnlineApprovedBy));
                headerDs.ApprovedDate = assetTransfer.OnlineApprovedTime != null ? assetTransfer.OnlineApprovedTime : null;
                headerDs.ApproverSignature = SignatureHelper.GetUserSignature(assetTransfer.OnlineApprovedBy);

                headerDs.DelivererPlate = assetTransfer.PlateNo;

                if (assetTransfer.FromBranchId != null)
                    headerDs.Project = InventoryPrintHelper.BranchName((int)assetTransfer.FromBranchId);

                headerDs.Remark = assetTransfer.Remark;

                if (assetTransfer.DriverType == AssetTransferDelivererType.Internal)
                {
                    if (!String.IsNullOrEmpty(assetTransfer.DeliveredBy))
                        headerDs.Deliverer = EmployeeHelper.GetEmployeeEnglishName(int.Parse(assetTransfer.DeliveredBy));
                }
                else
                {
                    headerDs.Deliverer = assetTransfer.SupplierName;
                }

                if (assetTransfer.Type == TransferType.StoreToStore)
                {
                    int empid = UserHelper.GetUserEmployeeId(assetTransfer.OnlineApprovedBy);                   
                    headerDs.Department = EmployeeHelper.GetEmployeeDepartmentNameByEmpId(empid);

                    if (assetTransfer.RecipientType == AssetTransferRecipientType.Employee)
                    {
                        if (!String.IsNullOrEmpty(assetTransfer.DeliveredBy))
                            headerDs.Recipient = EmployeeHelper.GetEmployeeEnglishName(int.Parse(assetTransfer.RecipientEmployee));
                    }
                    else
                    {
                        headerDs.Deliverer = assetTransfer.RecipientSupplier;
                    }
                }
                else if (assetTransfer.Type == TransferType.PersonToPerson)
                {
                    if (assetTransfer.TransferTo != null)
                    {
                        headerDs.Recipient = EmployeeHelper.GetEmployeeEnglishName((int)assetTransfer.TransferTo);
                        var employee = db.Person_Employee.Find(assetTransfer.TransferTo);
                        if (employee != null)
                            headerDs.Department = EmployeeHelper.GetEmployeeDepartmentNameByEmpId(employee.EmployeeId);
                    }
                }

            }

            if (organization != null)
            {
                headerDs.Name = organization.TradeName;
                headerDs.AmharicName = organization.AmharicName;
                var address =
                    db.Addresses.Where(
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
                    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                                              a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return headerDs;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FixedAssetTransferItemVm> TransferItems(int id)
        {
            List<FixedAssetTransferItemVm> list = new List<FixedAssetTransferItemVm>();

            var assetTransferItems = db.AssetTransferItems.Where(x => x.AssetTransferId == id).ToList();

            foreach (var item in assetTransferItems)
            {
                var asset = db.AssetRegistrations.Find(item.AssetRegistrationId);

                if (asset != null)
                {
                    var issueitem = new FixedAssetTransferItemVm
                    {
                        Category = asset.FixedAssetCategory.CategoryName,
                        SubCategory = asset.FixedAssetSubCategory.SubCategoryName,
                        TagNo = asset.Tag,
                        SerialNo = asset.SerialNo,
                        EngineNo = asset.EngineNo,
                        ChassisNo = asset.ChassisNo,
                        Manufacturer = asset.Manufacturer,
                        Condition = asset.Condition.Name,
                        Unit = item.Unit,
                        UnitPrice = asset.BookValue,
                        Total = asset.BookValue*item.Quantity,
                        Model = item.ModelNo,
                        Name=item.Name,
                        GrnNo = item.GrnNo,
                        Size=item.Size,
                        PlateNo = asset.PlateNo,
                        Quantity=item.Quantity,
                        Remark=item.Remark

                    };

                    list.Add(issueitem);
                }
            }
           return list;
        }
    }
}