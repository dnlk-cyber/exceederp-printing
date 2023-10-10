using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.FixedAsset.Print.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.Print.DataSource
{
    [DataObject]
    class FixedAssetDisposalOds
    {

        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetDisposalViewModel GetAssetDisposal(int id)
        {
            FixedAssetDisposalViewModel headerDs = new FixedAssetDisposalViewModel();

            var assetDisposal = db.AssetDisposals.Find(id);

            #region Company info
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
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
            #endregion
            

            var arCustomer = db.ARCustomerProfiles.FirstOrDefault(x => x.CustomerId == assetDisposal.CustomerId);

            if (assetDisposal != null)
            {
                headerDs.DisposalNumber = assetDisposal.AssetDisposalId;
                headerDs.Customer = arCustomer?.CustomerId + "-" + arCustomer?.CompanyName;
                headerDs.ExternalCustomer = assetDisposal.ExternalCustomer;
                headerDs.Date = assetDisposal.Date;
                headerDs.DisposalType = Enum.GetName(typeof(DisposalType), assetDisposal.DisposalType);
                headerDs.Discription = assetDisposal.Description;

                headerDs.StoreKeeperName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetDisposal.CreatedBy));

                headerDs.ApprovedDate = assetDisposal.OnlineApprovedTime.GetValueOrDefault().ToShortDateString();
                headerDs.ApproverName = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetDisposal.OnlineApprovedBy));
                headerDs.CreatedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(assetDisposal.CreatedBy));
                headerDs.Datecreated = assetDisposal.DateCreated.GetValueOrDefault().ToShortDateString();
            }

            return headerDs;
        }



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FixedAssetDisposalItemsViewModel> DisposalItems(int id)
        {
            List<FixedAssetDisposalItemsViewModel> fixedAssetDisposalItemsViewModels =
                new List<FixedAssetDisposalItemsViewModel>();

            var disposalItems = db.DisposalItems.Where(x => x.AssetDisposalId == id).ToList();

            foreach (var item in disposalItems)
            {
                var asset = db.AssetRegistrations.Find(item.AssetRegistrationId);

                if (asset != null)
                {
                    var disposalItem = new FixedAssetDisposalItemsViewModel()
                    {
                        Category = asset.FixedAssetCategory.CategoryName,
                        SubCategory = asset.FixedAssetSubCategory.SubCategoryName,
                        ItemName = asset.Name,
                        BookValue = item.BookValue,
                        SalesPrice = item.SalesPrice,
                        TaxAmount = item.TaxAmount,
                        Total = item.Total,
                        //TagNo = asset.Tag,
                        SerialNo = asset.SerialNo,
                        EngineNo = asset.EngineNo,
                        ChassisNo = asset.ChassisNo,
                        Manufacturer = asset.Manufacturer,
                        Condition = asset.Condition.Name,
                        
                        //UnitPrice = asset.BookValue,
                        //Total = asset.BookValue * item.Quantity,
                        
                    };

                    fixedAssetDisposalItemsViewModels.Add(disposalItem);
                }
            }

            return fixedAssetDisposalItemsViewModels;

        }

    }
}
