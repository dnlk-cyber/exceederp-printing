using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.FixedAsset.DataSource
{
    [DataObject]
    public class AssetMovementOds : List<AssetMovementVm>
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public Organization AssetMovementHeader()
        {

            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            Organization headerDs = new Organization();
           


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
            return headerDs;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AssetMovementVm> AssetMovementDetail(string  assetid)
        {
            List<AssetMovementVm> assetMovementVms = new List<AssetMovementVm> { };

            if (!String.IsNullOrEmpty(assetid))
            {
                int did = int.Parse(assetid);
                var reportData = _db.LocationTrackings
                    .Where(x => x.AssetRegistrationId == did).ToList();


                foreach (var item in reportData)
                {
                    var movementVm = new AssetMovementVm
                    {
                        LocationType = item.LocationType,
                        FromDate = item.StartDate,
                        ToDate = (DateTime) item.EndDateDate,
                        Description = item.Description,
                        Store = item.StoreCode,
                        Employee = EmployeeHelper.GetEmployeeEnglishName(int.Parse(item.EmployeeId)),
                        Branch = InventoryPrintHelper.BranchName(item.BranchId),
                        IsCurrent = item.IsCurrent ? "Yes" : "No"
                    };

                    assetMovementVms.Add(movementVm);
                }
            }


            return assetMovementVms;
        }
    }
}