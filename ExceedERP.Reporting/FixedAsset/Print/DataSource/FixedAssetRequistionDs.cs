using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.FixedAsset.Print.ViewModel;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.Print.DataSource
{
    [DataObject]
    public class FixedAssetRequistionDs
    {
        ExceedDbContext db = new ExceedDbContext();
        private FixedAssetRequistionHeader _headerDs = new FixedAssetRequistionHeader();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetRequistionHeader GetFixedAssetRequistionHeader(int id)
        {
            var assetRequisition = db.FixedAssetRequisitions.Find(id);
            if (assetRequisition != null)
            {
         
                var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

                _headerDs = new FixedAssetRequistionHeader
                {
                    StoreRequsitionNo = InventoryPrintHelper.GetId(assetRequisition.FixedAssetRequisitionId),
                    RequestedDate = assetRequisition.RequestDate.ToShortDateString(),
                    ApprovedDate = assetRequisition.OnlineApprovedTime.GetValueOrDefault().ToShortDateString(),
                    Requestor = EmployeeHelper.GetEmployeeEnglishName(assetRequisition.EmployeeId),
                    StoreName = InventoryPrintHelper.StoreName(assetRequisition.StoreCode),
                    AuthorizedBy =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(assetRequisition.MiddleOnlineApprovedBy)),
                    ApprovedBy =
                        EmployeeHelper.GetEmployeeEnglishName(
                            UserHelper.GetUserEmployeeId(assetRequisition.OnlineApprovedBy)),
                    RequestingDepartment = InventoryPrintHelper.WorkUnitName(assetRequisition.OrgStructureId),
                    RequestingStore = InventoryPrintHelper.StoreName(assetRequisition.StoreCode),
                    ForProject = InventoryPrintHelper.BranchName(assetRequisition.BranchId),
                    Remark = assetRequisition.Remark
                };


                if (organization != null)
                {
                    _headerDs.Name = organization.TradeName;
                    _headerDs.AmharicName = organization.AmharicName;
                    var address =
                        db.Addresses.Where(
                            a =>
                                a.Reference == organization.MainCompanyId.ToString() &&
                                a.ReferenceType.ToLower() == "company").ToList();
                    if (address.Count > 0)
                    {
                        foreach (var a in address)
                        {

                            if (_headerDs.Address == null)
                            {
                                _headerDs.Address = a.Category + ":- " + a.Value;
                            }
                            else
                            {
                                _headerDs.Address += ", " + a.Category + "-" + a.Value;
                            }

                        }

                    }
                    var logo =
                        db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                                                  a.ReferenceType == AttachmentType.LOGO);
                    if (logo != null)
                    {
                        _headerDs.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    }
                }
              
            }
            return _headerDs;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FixedAssetRequisitionItem> StoreRequisitionItems(int id)
        {
            List<FixedAssetRequisitionItem> storeRequisitionItems = db.FixedAssetRequisitionItems.Where(x => x.FixedAssetRequisitionId == id).ToList();
            return storeRequisitionItems;
        }
    }
}