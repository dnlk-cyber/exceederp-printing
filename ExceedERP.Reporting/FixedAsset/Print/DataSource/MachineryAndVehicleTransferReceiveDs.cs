using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.FixedAsset.Print.ViewModel;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.Print.DataSource
{
    [DataObject]
    public class MachineryAndVehicleTransferReceiveDs
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public MachineryAndVehicleTransferHeader GetMachineryAndVehicleTransferReceiveHeader(int id)
        {
            var transferReceive = db.MachineryAndVehicleTransferReceives.Find(id);
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            MachineryAndVehicleTransferHeader headerDs = new MachineryAndVehicleTransferHeader();
            if (transferReceive != null)
            {
                var transferSend = db.MachineryAndVehicleTransfers.Find(transferReceive.MachineryAndVehicleTransferId);

               
                headerDs.TransferReceiveNumber = InventoryPrintHelper.GetId(transferReceive.MachineryAndVehicleTransferReceiveId);
                headerDs.TransferNumber = InventoryPrintHelper.GetId(transferReceive.MachineryAndVehicleTransferId);
                headerDs.RequestNumber = transferSend.RequestNumber;
                headerDs.CategoryCode = transferSend.AssetRegistration.FixedAssetCategory.CategoryName;
                headerDs.SubCategoryCode = transferSend.AssetRegistration.FixedAssetSubCategory.SubCategoryName;
                headerDs.Tag = transferSend.TagNo;

                headerDs.TransferDate = transferReceive.ReceiveDate;
                headerDs.UsageDays = transferSend.UsageDays;
                headerDs.TransferDesc = transferSend.TransferDesc;
                //headerDs.CustodianBeforeTransfer = EmployeeHelper.GetEmployeeFullName(transfer.CustodianBeforeTransfer);
                headerDs.LocationBeforeTransfer = transferSend.LocationBeforeTransfer;
                //headerDs.TransferTo = EmployeeHelper.GetEmployeeFullName(transfer.TransferTo);
                headerDs.TransferReason = transferSend.TransferReason;
                headerDs.TransferBy = transferSend.TransferBy.ToString();
                headerDs.DriverName = transferSend.DriverName;
                headerDs.TransferPlateNo = transferSend.TransferPlateNo;

                headerDs.Manufacturer = transferSend.Manufacturer;
                headerDs.ModelNo = transferSend.ModelNo;
                headerDs.ChassisNo = transferSend.ChassisNo;
                headerDs.SerialNo = transferSend.SerialNo;
                headerDs.PlateNo = transferSend.PlateNo;
                headerDs.EngineNo = transferSend.EngineNo;
                headerDs.BarCodeNo = transferSend.BarCodeNo;

                headerDs.KmHourReading = transferSend.KmHourReading;
                headerDs.Condition = transferSend.AssetRegistration.Condition.Name;
                headerDs.FuelVolume = transferSend.FuelVolume;
                headerDs.HydraulicOilVolume = transferSend.HydraulicOilVolume;
                headerDs.BatteryAmpere = transferSend.BatteryAmpere;
                headerDs.BatteryStatus = transferSend.BatteryStatus;
                headerDs.BatteryQuantity = transferSend.BatteryQuantity;

                if (transferSend.TransferTo != null)
                {
                    headerDs.Recipient = EmployeeHelper.GetEmployeeEnglishName((int)transferSend.TransferTo);
                    var employee = db.Person_Employee.Find(transferSend.TransferTo);
                    
                }

                headerDs.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(transferReceive.CreatedBy));
                headerDs.PreparedDate = transferReceive.DateCreated != null ? transferReceive.DateCreated : null;
                headerDs.PreparedBySignature = SignatureHelper.GetUserSignature(transferReceive.CreatedBy);

                headerDs.Approver = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(transferReceive.OnlineApprovedBy));
                headerDs.ApprovedDate = transferReceive.OnlineApprovedTime != null ? transferReceive.OnlineApprovedTime : null;
                headerDs.ApproverSignature = SignatureHelper.GetUserSignature(transferReceive.OnlineApprovedBy);

             
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
        public MachineryAndVehicleTransferReceiveVm MachineTransferReceive(int id)
        {
            MachineryAndVehicleTransferReceiveVm transferData = new MachineryAndVehicleTransferReceiveVm
            {
                Accessories = db.AccessoryReceives.Where(x => x.MachineryAndVehicleTransferReceiveId == id).ToList(),
                DamagedParts = db.DamagedPartReceives.Where(x => x.MachineryAndVehicleTransferReceiveId == id).ToList(),
                MissedParts = db.MissedPartReceives.Where(x => x.MachineryAndVehicleTransferReceiveId == id).ToList()
            };

            return transferData;
        }
    }
}