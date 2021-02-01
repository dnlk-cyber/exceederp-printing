using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.FixedAsset.DataSource
{
    [DataObject]
    public class FixedAssetAnnualDepreciationOds : List<FixedAssetHeader>
    {
        ExceedDbContext _db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public FixedAssetHeader GetAnnualDepreciationHeader(string year, string branch)
        {
            var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");
            FixedAssetHeader headerDs = new FixedAssetHeader();

            if (!String.IsNullOrEmpty(year))
            {
                int yearid = int.Parse(year);

                var fiscalYear = _db.GLFiscalYears.Find(yearid);
                if (fiscalYear != null) headerDs.Year = fiscalYear.Name;
            }

            if (!String.IsNullOrEmpty(branch))
            {
                int branchid = int.Parse(branch);
                var branchs = _db.Branch.Find(branchid);
                if (branchs != null)
                    headerDs.Branch = branchs.Name;
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
        public List<AnnualDepreciationVm> GetAnnualDepreciations(string year, string branch)
        {
            List<AnnualDepreciationVm> depreciations = new List<AnnualDepreciationVm>();
            var annualDepreciations = _db.AnnualDepreciationItems.AsQueryable();
            if (!String.IsNullOrEmpty(year))
            {
                int yearid = int.Parse(year);
                annualDepreciations = annualDepreciations.Where(x => x.AnnualDepreciation.GlFiscalYearId == yearid);
                //if (!String.IsNullOrEmpty(branch))
                //{
                //    int branchid = int.Parse(branch);
                //    annualDepreciations = annualDepreciations.Where(x => x == branchid);
                //}
                var recordList = annualDepreciations.ToList();
                foreach (var depreciation in recordList)
                {
                    var item = new AnnualDepreciationVm
                    {
                        Tag = depreciation.AssetRegistration.Tag,
                        Description = depreciation.AssetRegistration.Name,
                        OriginalValue = depreciation.AssetRegistration.Quanity*depreciation.AssetRegistration.OriginalValue,
                        DepreciationAmount = depreciation.DepreciationAmount,
                        AccumulatedDepreciationAmount = 0,//depreciation.AccumulatedDepreciationAmount,
                        BookValue = depreciation.BookValue
                    };

                   

                    depreciations.Add(item);
                }
            }
            return depreciations;
        }

    }
}
