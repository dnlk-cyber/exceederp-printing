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
    public class FixedAssetTransferReceiveOds
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetTransferHeader GetAssetTransferReceive(int id)
        {
            var assetTransferReceive = db.AssetTransferReceives.Find(id);
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            FixedAssetTransferHeader headerDs = new FixedAssetTransferHeader();
            if (assetTransferReceive != null)
            {
                var assetTransferSend = db.AssetTransfers.Find(assetTransferReceive.AssetTransferId);

                headerDs.StoreRequistionNo = assetTransferSend.RequestNumber;
                headerDs.TransferDescription = assetTransferSend.TransferDesc;
                headerDs.TransferReceiveNumber = InventoryPrintHelper.GetId(assetTransferReceive.AssetTransferReceiveId);
                headerDs.TransferNumber = InventoryPrintHelper.GetId(assetTransferReceive.AssetTransferId);
                headerDs.TransferType = assetTransferSend.Type.ToString();
                headerDs.FromStore = assetTransferSend.FromStoreCode+"-"+InventoryPrintHelper.StoreName(assetTransferSend.FromStoreCode);
                headerDs.ToStore = assetTransferSend.ToStoreCode + "-" + InventoryPrintHelper.StoreName(assetTransferSend.ToStoreCode);
                if(assetTransferSend.CustodianBeforeTransfer!=null)
                 headerDs.FromEmployee = EmployeeHelper.GetEmployeeEnglishName((int)assetTransferSend.CustodianBeforeTransfer);
                if (assetTransferSend.TransferTo != null)
                    headerDs.ToEmployee = EmployeeHelper.GetEmployeeEnglishName((int)assetTransferSend.TransferTo);
                headerDs.TransferDate = assetTransferReceive.ReceiveDate.ToShortDateString();
                
                headerDs.StoreKeeperName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetTransferReceive.CreatedBy));

                headerDs.ApprovedDate = assetTransferReceive.OnlineApprovedTime.GetValueOrDefault().ToShortDateString();
                headerDs.ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetTransferReceive.OnlineApprovedBy));
                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetTransferReceive.CreatedBy));
                headerDs.Datecreated = assetTransferReceive.DateCreated.GetValueOrDefault().ToShortDateString();

                headerDs.Remark = assetTransferReceive.Remark;

                if (assetTransferSend.FromBranchId != null)
                    headerDs.Project = InventoryPrintHelper.BranchName((int)assetTransferSend.FromBranchId);

                if (assetTransferSend.DriverType == AssetTransferDelivererType.Internal)
                {
                    if (!String.IsNullOrEmpty(assetTransferSend.DeliveredBy))
                        headerDs.Deliverer = EmployeeHelper.GetEmployeeEnglishName(int.Parse(assetTransferSend.DeliveredBy));
                }
                else
                {
                    headerDs.Deliverer = assetTransferSend.SupplierName;
                }

                if (assetTransferSend.Type == TransferType.StoreToStore)
                {
                    int empid = UserHelper.GetUserEmployeeId(assetTransferReceive.OnlineApprovedBy);                   
                    headerDs.Department = EmployeeHelper.GetEmployeeDepartmentNameByEmpId(empid);

                    if (assetTransferSend.RecipientType == AssetTransferRecipientType.Employee)
                    {
                        if (!String.IsNullOrEmpty(assetTransferSend.RecipientEmployee))
                            headerDs.Recipient = EmployeeHelper.GetEmployeeEnglishName(int.Parse(assetTransferSend.RecipientEmployee));
                    }
                    else
                    {
                        headerDs.Deliverer = assetTransferSend.RecipientSupplier;
                    }
                }
                else if (assetTransferSend.Type == TransferType.PersonToPerson)
                {
                    if (assetTransferSend.TransferTo != null)
                    {
                        headerDs.Recipient = EmployeeHelper.GetEmployeeEnglishName((int)assetTransferSend.TransferTo);
                        var employee = db.Person_Employee.Find(assetTransferSend.TransferTo);
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
        public List<FixedAssetTransferItemVm>TransferReceiveItems(int id)
        {
            List<FixedAssetTransferItemVm> list = new List<FixedAssetTransferItemVm>();

            var assetTransferItems = db.AssetTransferReceiveItems.Where(x => x.AssetTransferReceiveItemId == id).ToList();
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