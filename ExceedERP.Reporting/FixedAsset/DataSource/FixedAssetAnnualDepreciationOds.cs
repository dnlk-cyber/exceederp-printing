using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data.Entity;

namespace ExceedERP.Reporting.FixedAsset.DataSource
{
	[DataObject]
	public class FixedAssetAnnualDepreciationOds : IDisposable
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
        public List<DepreciationVm> GetAnnualDepreciations(string year, string branch, string businessUnitId)
        {
            if (string.IsNullOrWhiteSpace(year))
                return new List<DepreciationVm>();

            int yearId = int.Parse(year);
            int? branchId = !string.IsNullOrWhiteSpace(branch) ? int.Parse(branch) : (int?)null;
            int? bizUnitId = !string.IsNullOrWhiteSpace(businessUnitId) ? int.Parse(businessUnitId) : (int?)null;

            var query = _db.AnnualDepreciationItems
                .Where(x => x.AnnualDepreciation.GlFiscalYearId == yearId);

            if (branchId.HasValue)
                query = query.Where(x => x.BranchId == branchId.Value);

            if (bizUnitId.HasValue)
                query = query.Where(x => x.BusinessUnitId == bizUnitId.Value);

            var result = query
                .Select(x => new DepreciationVm
                {
                    Tag = x.AssetRegistration.Tag,
                    Description = x.AssetRegistration.Name,
                    OriginalValue = x.AssetRegistration.Quanity * x.AssetRegistration.OriginalValue,
                    DepreciationAmount = x.DepreciationAmount,
                    AccumulatedDepreciationAmount = x.AccumulatedDepreciationAmount,
                    BookValue = x.BookValue
                })
                .ToList();

            return result;
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
