using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.DataSource
{


    [DataObject]
    public class DisposalDetailObjectDataSource : List<DisposalDetailViewModel>
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public DisposalHeaderViewModel DisposalHeader(string periodFrom, string periodTo)
        {
           
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            DisposalHeaderViewModel headerDs = new DisposalHeaderViewModel();
            if (!String.IsNullOrWhiteSpace(periodFrom) && !String.IsNullOrWhiteSpace(periodTo))
            {
                headerDs.From = _db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == periodFrom).DateFrom;
                headerDs.To = _db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == periodTo).DateTo;
            }

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
        public List<DisposalDetailViewModel> GetDesposal(string periodFrom, string periodTo, string branchid)
        {
            List<DisposalDetailViewModel> disposalLists = new List<DisposalDetailViewModel>();

            if (!String.IsNullOrWhiteSpace(periodFrom) && !String.IsNullOrWhiteSpace(periodTo))
            {
                var perFrom = _db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == periodFrom)?.DateFrom;
                var perTo = _db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == periodTo)?.DateTo;


           
                string branchName = "";
                if (!String.IsNullOrWhiteSpace(branchid))
                {
                    var branch = _db.Branch.Find(branchid);
                    if (branch != null) branchName = branch.Name;
                }

                var disposals = _db.DisposalItems
                    .Where(x => x.AssetDisposal.Date >= perFrom && x.AssetDisposal.Date <= perTo).AsQueryable();

                foreach (var disposalItem in disposals)
                {
                    var assetGroup = _db.FixedAssetGroups.Find(disposalItem.FixedAssetCategory.FixedAssetGroupID);

                    var dispos = new DisposalDetailViewModel
                    {
                        Branch = branchName
                    };
                    if (assetGroup!=null)
                        dispos.Group = assetGroup.Name;
                    dispos.Category = disposalItem.FixedAssetCategory.CategoryName;
                    var deprece = _db.AnnualDepreciationItems
                        .Where(x => x.AssetRegistrationId == disposalItem.AssetRegistrationId).ToList();
                    if (deprece.Any())
                        dispos.AccumulatedDep = deprece.Sum(x => x.DepreciationAmount);

                    var asset = _db.AssetRegistrations.Find(disposalItem.AssetRegistrationId);
                    if (asset != null)
                    {
                        dispos.BookValue = disposalItem.BookValue = asset.BookValue;                        
                        dispos.SerialNo = asset.SerialNo;
                        dispos.Tag = asset.Tag;
                        dispos.BarCode = asset.BarCodeNo;
                        dispos.Category = asset.FixedAssetCategory.CategoryName;
                        dispos.SubCategory = asset.FixedAssetSubCategory.SubCategoryName;
                        dispos.Name = asset.Name;
                        dispos.BookValue = asset.BookValue;
                        dispos.PurchasePrice = asset.PurchasePrice;
                        dispos.Size = asset.Size;
                        dispos.Location = asset.Location;
                        dispos.SivDate = "";
                        dispos.SivNo = asset.SIV;
                        dispos.GrnNo = asset.GRN;
                        dispos.GrnDate = asset.GRNDate.ToString();
                        dispos.SerialNo = asset.SerialNo;
                        dispos.ChasisNo = asset.ChassisNo;
                        dispos.EngineNo = asset.EngineNo;
                        dispos.Model = asset.ModelNo;
                        dispos.PlateNo = asset.PlateNo;
                        dispos.Condition = asset.Condition.Name;
                        dispos.Supplier = asset.Supplier;
                        dispos.Manufacturer = asset.Manufacturer;
                        dispos.ManufacturingYear = asset.ManufacturDate.ToString();
                        dispos.Quantity = asset.Quanity;
                        dispos.TotalPrice = asset.Quanity * asset.PurchasePrice;
                        dispos.Remark = "";
                    }



                     disposalLists.Add(dispos);

                   
                }
            }
            return disposalLists;
        }
    }
}