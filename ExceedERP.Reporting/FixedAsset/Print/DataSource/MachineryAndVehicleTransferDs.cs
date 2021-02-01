using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.FixedAsset.Print.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.Print.DataSource
{
    [DataObject]
    public class MachineryAndVehicleTransferDs
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public MachineryAndVehicleTransferHeader GetMachineryAndVehicleTransferHeader(int id)
        {
            var transfer = db.MachineryAndVehicleTransfers.Find(id);
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            MachineryAndVehicleTransferHeader headerDs = new MachineryAndVehicleTransferHeader();
            if (transfer != null)
            {
                
                
                headerDs.RequestNumber = transfer.RequestNumber;
                headerDs.CategoryCode = transfer.AssetRegistration.FixedAssetCategory.CategoryName;
                headerDs.SubCategoryCode = transfer.AssetRegistration.FixedAssetSubCategory.SubCategoryName;
                headerDs.Tag = transfer.TagNo;

                headerDs.TransferDate = transfer.TransferDate;
                headerDs.UsageDays = transfer.UsageDays;
                headerDs.TransferDesc = transfer.TransferDesc;
                //headerDs.CustodianBeforeTransfer = EmployeeHelper.GetEmployeeFullName(transfer.CustodianBeforeTransfer);
                headerDs.LocationBeforeTransfer = transfer.LocationBeforeTransfer;
                //headerDs.TransferTo = EmployeeHelper.GetEmployeeFullName(transfer.TransferTo);
                headerDs.TransferReason = transfer.TransferReason;
                headerDs.TransferBy = transfer.TransferBy.ToString();
                headerDs.DriverName = transfer.DriverName;
                headerDs.TransferPlateNo = transfer.TransferPlateNo;

                headerDs.Manufacturer = transfer.Manufacturer;
                headerDs.ModelNo = transfer.ModelNo;
                headerDs.ChassisNo = transfer.ChassisNo;
                headerDs.SerialNo = transfer.SerialNo;
                headerDs.PlateNo = transfer.PlateNo;
                headerDs.EngineNo = transfer.EngineNo;
                headerDs.BarCodeNo = transfer.BarCodeNo;

                headerDs.KmHourReading = transfer.KmHourReading;
                headerDs.Condition = transfer.AssetRegistration.Condition.Name;
                headerDs.FuelVolume = transfer.FuelVolume;
                headerDs.HydraulicOilVolume = transfer.HydraulicOilVolume;
                headerDs.BatteryAmpere = transfer.BatteryAmpere;
                headerDs.BatteryStatus = transfer.BatteryStatus;
                headerDs.BatteryQuantity = transfer.BatteryQuantity;
                headerDs.DriverName = transfer.DriverName;

                if (transfer.TransferTo != null)
                {
                    headerDs.Recipient = EmployeeHelper.GetEmployeeEnglishName((int)transfer.TransferTo);
                    var employee = db.Person_Employee.Find(transfer.TransferTo);

                }

                headerDs.ApprovedDate = transfer.OnlineApprovedTime.GetValueOrDefault().ToShortDateString();
                headerDs.ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(transfer.OnlineApprovedBy));
                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(transfer.CreatedBy));
                headerDs.Datecreated = transfer.DateCreated.GetValueOrDefault().ToShortDateString();



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
        public MachineryAndVehicleTransferViewModel MachineTransfer(int id)
        {
            MachineryAndVehicleTransferViewModel transferData = new MachineryAndVehicleTransferViewModel
            {
                Accessories = db.Accessories.Where(x => x.MachineryAndVehicleTransferId == id).ToList(),
                DamagedParts = db.DamagedParts.Where(x => x.MachineryAndVehicleTransferId == id).ToList(),
                MissedParts = db.MissedParts.Where(x => x.MachineryAndVehicleTransferId == id).ToList()
            };

            return transferData;
        }
    }
}