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
    public class DisposalObjectDataSource : List<DisposalViewModel>
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
        public List<DisposalViewModel> GetDesposal(string periodFrom, string periodTo, string branchid)
        {
            List<DisposalViewModel> disposalLists = new List<DisposalViewModel>();

            if (!String.IsNullOrWhiteSpace(periodFrom) && !String.IsNullOrWhiteSpace(periodTo))
            {
                var perFrom = _db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == periodFrom)?.DateFrom;
                var perTo = _db.GLPeriods.FirstOrDefault(x => x.GLPeriodId.ToString() == periodTo)?.DateTo;


                var catagories = _db.FixedAssetCategories.ToList();
                var groups = _db.FixedAssetGroups.ToList();
                var branches = _db.Branch.ToList();
                if (!String.IsNullOrWhiteSpace(branchid))
                    branches = branches.Where(x => x.BranchId.ToString() == branchid).ToList();

                var disposals = _db.DisposalItems
                    .Where(x => x.AssetDisposal.Date >= perFrom && x.AssetDisposal.Date <= perTo).AsQueryable();

                foreach (var group in groups)
                {
                    var groupCategories = catagories.Where(x => x.FixedAssetGroupID == group.FixedAssetGroupID).ToList();

                    foreach (var catagory in groupCategories)
                    {
                        var catdisposals = disposals.Where(x => x.FixedAssetCategoryID == catagory.FixedAssetCategoryID)
                            .AsQueryable();

                        foreach (var branch in branches)
                        {
                            decimal accum = 0;
                            decimal bookvalue = 0;
                            var dispos = new DisposalViewModel
                            {
                                Group =group.Name,
                                Category = catagory.CategoryName,
                                Branch=branch.Name,
                            };

                            foreach (var disposal in catdisposals)
                            {
                                var register = _db.AssetRegistrations.Where(x=>x.BranchId==branch.BranchId).FirstOrDefault(x =>
                                    x.AssetRegistrationId == disposal.AssetRegistrationId);

                                if (register != null)
                                {
                                    accum = accum + _db.AnnualDepreciationItems
                                                .Where(x => x.AssetRegistrationId == register.AssetRegistrationId)
                                                .Sum(x => x.DepreciationAmount);
                                    bookvalue = bookvalue+register.BookValue;
                                }
                            }

                            dispos.AccumulatedDep = accum;
                            dispos.BookValue = bookvalue;

                            disposalLists.Add(dispos);

                        }
                       
                    }

                   
                }
            }
            return disposalLists;
        }
    }
}