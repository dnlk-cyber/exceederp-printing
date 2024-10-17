using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Inventory.DataSource.Print;
using ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;

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


                var catagories = _db.FixedAssetCategories.AsNoTracking().ToList();
                var groups = _db.FixedAssetGroups.AsNoTracking().ToList();
                var branches = _db.Branch.AsNoTracking().ToList();
                if (!String.IsNullOrWhiteSpace(branchid))
                {
                    int brid = int.Parse(branchid);
                    branches = branches.Where(x => x.BranchId == brid).ToList();
                }

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
                                var register = _db.AssetRegistrations
                                    .Where(x=>x.BranchId==branch.BranchId).FirstOrDefault(x =>
                                    x.AssetRegistrationId == disposal.AssetRegistrationId);

                                if (register != null)
								{
									accum += _db.AnnualDepreciationItems
	                                .Where(x => x.AssetRegistrationId == register.AssetRegistrationId)
	                                .Sum(x => (decimal?)x.DepreciationAmount) ?? 0;
									bookvalue += register.BookValue;
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