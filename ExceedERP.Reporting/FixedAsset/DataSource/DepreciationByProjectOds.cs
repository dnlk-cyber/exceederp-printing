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
    public class DepreciationByProjectOds : List<DepreciationViewModel>
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public DepreciationHeaderViewModel DepreciationByProjectHeader(string periodFrom, string periodTo)
        {

            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

            DepreciationHeaderViewModel headerDs = new DepreciationHeaderViewModel();
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
        public List<DepreciationViewModel> GetDepreciation(string periodFrom, string periodTo, string branchid)
        {
            List<DepreciationViewModel> deprciationLists = new List<DepreciationViewModel>();

            if (!String.IsNullOrWhiteSpace(periodFrom) && !String.IsNullOrWhiteSpace(periodTo))
            {
                var perFrom = _db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == periodFrom)?.DateFrom;
                var perTo = _db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == periodTo)?.DateTo;

                var periodlist = _db.GLPeriods.Where(x => x.DateFrom >= perFrom)
                    .Where(x => x.DateTo <= perTo).Select(x => x.GLPeriodId).ToList();

                var catagories = _db.FixedAssetCategories.ToList();
                var groups = _db.FixedAssetGroups.ToList();
                var branches = _db.Branch.ToList();
                if (!String.IsNullOrWhiteSpace(branchid))
                    branches = branches.Where(x => x.BranchId.ToString() == branchid).ToList();

                var deprciations = _db.DepreciationForCosts.Where(x => periodlist.Contains(x.GLPeriodId)).ToList();


                foreach (var group in groups)
                {
                    var groupCategories = catagories.Where(x => x.FixedAssetGroupID == group.FixedAssetGroupID).ToList();

                    foreach (var catagory in groupCategories)
                    {
                        var catdep = deprciations.Where(x => x.AssetRegistration.FixedAssetCategoryID == catagory.FixedAssetCategoryID)
                            .AsQueryable();

                        foreach (var branch in branches)
                        {
                            var dispos = new DepreciationViewModel
                            {
                                Group = group.Name,
                                Category = catagory.CategoryName,
                                Branch = branch.Name,
                            };

                            var branchcatdep = catdep.Where(x => x.AssetRegistration.BranchId == branch.BranchId)
                                .AsQueryable();
                           
                            if(branchcatdep.Any())
                            dispos.DepreciationAmount = branchcatdep.Sum(x=>x.DepreciationAmount);
                            deprciationLists.Add(dispos);

                        }

                    }


                }
            }

            return deprciationLists;
        }
    }
}
