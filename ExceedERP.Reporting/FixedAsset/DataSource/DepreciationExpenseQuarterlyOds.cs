using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Migrations;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.FixedAsset;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.FixedAsset.DataSource
{

	[DataObject]
	public class DepreciationExpenseQuarterlyOds : List<DepreciationExpenseVm>
	{
		private readonly ExceedDbContext _db = new ExceedDbContext();
		private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

		[DataObjectMethod(DataObjectMethodType.Select)]
		public DepreciationHeaderViewModel DepreciationExpenseQuarterlyHeader(string fiscalYear, string quarter)
		{

			var organization = _db.Companies.FirstOrDefault(x => x.Category == "0");

			DepreciationHeaderViewModel headerDs = new DepreciationHeaderViewModel();
			if (!String.IsNullOrWhiteSpace(fiscalYear) && !String.IsNullOrWhiteSpace(quarter))
			{
			   
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
		public List<DepreciationExpenseVm> GetDepreciationExpenseQuarterly(string fiscalYear, string fromPeriod, string toPeriod, string quarter, string businessUnitId)
		{
			List<DepreciationExpenseVm> deprciationLists = new List<DepreciationExpenseVm>();

			if (!String.IsNullOrWhiteSpace(fromPeriod) && !String.IsNullOrWhiteSpace(toPeriod))
			{
				int from = int.Parse(fromPeriod);
				int to = int.Parse(toPeriod);
				DateTime dateFrom = _db.GLPeriods.Find(from).DateFrom;
				DateTime dateTo = _db.GLPeriods.Find(to).DateTo;

				var groups = _db.FixedAssetGroups.AsNoTracking().ToList();
				var categories = _db.FixedAssetCategories.AsNoTracking().ToList();
				var assetRegistrations = _db.AssetRegistrations
					.Where(x => x.GRNDate >= dateFrom)
					.Where(x => x.GRNDate <= dateTo).AsNoTracking().AsQueryable();

				if (!String.IsNullOrEmpty(businessUnitId))
				{
					int did = int.Parse(businessUnitId);
					assetRegistrations = assetRegistrations.Where(x => x.BusinessUnitId == did);
				}

				List<int> categorIds = assetRegistrations.Select(x => x.FixedAssetCategoryID).Distinct().ToList();

				foreach (var catId in categorIds)
				{
					var category = categories.FirstOrDefault(x => x.FixedAssetCategoryID == catId);
					if (category != null)
					{
						var registrations = assetRegistrations.Where(x => x.FixedAssetCategoryID == catId).ToList();

						var group = groups.FirstOrDefault(x => x.FixedAssetGroupID == category.FixedAssetGroupID);

						if (group != null)
						{
							var depreciationExpenseVm = new DepreciationExpenseVm
							{
								Category = category.CategoryName,
								OriginalValue = 0,
								DepreciationExpenseLineVms = new List<DepreciationExpenseLineVm>(),
							};
							foreach (var registration in registrations)
							{
								if (registration.GRNDate != null)
								{
									double days = 0;
									days = dateTo > DateTime.Today ? (DateTime.Today - (DateTime)registration.GRNDate).TotalDays : (dateTo - (DateTime)registration.GRNDate).TotalDays;
									decimal rate = (decimal)group.PoolingPercentage;
									decimal deprection = (registration.Quanity) * (rate * registration.PurchasePrice * (decimal)days) / (365 * 100);
									var depreciationExpenseLineVm = new DepreciationExpenseLineVm
									{
										PurchaseDate = registration.GRNDate,
										Reference = registration.GRN,
										OriginalValue = registration.Quanity * registration.PurchasePrice,
										NoOfMonths = (int)(days / 30),
										NoOfDays = (int)days,
										Rate = rate,
										TotalDepForYear = Math.Round(deprection, 2),
										TotalDepForQuarter = Math.Round(deprection, 2),
										BookValueAtEnd = Math.Round((registration.PurchasePrice - deprection), 2),
									};
									depreciationExpenseVm.DepreciationExpenseLineVms.Add(depreciationExpenseLineVm);
								}
							}
							if (depreciationExpenseVm.DepreciationExpenseLineVms.Any())
							{
								depreciationExpenseVm.OriginalValue = depreciationExpenseVm.DepreciationExpenseLineVms.Sum(x => x.OriginalValue);
								depreciationExpenseVm.TotalDepForYear = depreciationExpenseVm.DepreciationExpenseLineVms.Sum(x => x.TotalDepForYear);
								depreciationExpenseVm.TotalDepForQuarter = depreciationExpenseVm.DepreciationExpenseLineVms.Sum(x => x.TotalDepForQuarter);
								depreciationExpenseVm.BookValueAtEnd = depreciationExpenseVm.DepreciationExpenseLineVms.Sum(x => x.BookValueAtEnd);
								deprciationLists.Add(depreciationExpenseVm);
							}
						}
					}

				}

			}

			return deprciationLists;
		}

		//public List<DepreciationExpenseVm> GetDepreciationExpenseQuarterly1(string fiscalYear, string fromPeriod, string toPeriod string quarter)
		//{
		//    List<DepreciationExpenseVm> deprciationLists = new List<DepreciationExpenseVm>();

		//    if (!String.IsNullOrWhiteSpace(fromPeriod) && !String.IsNullOrWhiteSpace(toPeriod))
		//    {
		//        var year = _db.GLFiscalYears.Find(int.Parse(fiscalYear));
		//        if (year != null)
		//        {
		//            DateTime dateFrom = _fiscalYearHelper.GetQuarterStart(year.GlFiscalYearId, 1);
		//            //DateTime dateTo = _fiscalYearHelper.GetQuarterEnd(year.GlFiscalYearId, int.Parse(quarter));

		//            var groups = _db.FixedAssetGroups.AsNoTracking().ToList();
		//            var categories = _db.FixedAssetCategories.AsNoTracking().ToList();
		//            var assetRegistrations = _db.AssetRegistrations
		//                .Where(x => x.GRNDate >= dateFrom)
		//                .Where(x => x.GRNDate <= dateTo).AsNoTracking().ToList();

		//            List<int> categorIds = assetRegistrations.Select(x => x.FixedAssetCategoryID).Distinct().ToList();

		//            foreach (var catId in categorIds)
		//            {
		//                var category = categories.FirstOrDefault(x => x.FixedAssetCategoryID == catId);
		//                if (category != null)
		//                {
		//                    var registrations = assetRegistrations.Where(x => x.FixedAssetCategoryID == catId).ToList();

		//                    var group = groups.FirstOrDefault(x => x.FixedAssetGroupID == category.FixedAssetGroupID);

		//                    if (group != null)
		//                    {
		//                        var depreciationExpenseVm = new DepreciationExpenseVm
		//                        {
		//                            Category = category.CategoryName,
		//                            OriginalValue = 0,
		//                            DepreciationExpenseLineVms = new List<DepreciationExpenseLineVm>(),
		//                        };
		//                        foreach (var registration in registrations)
		//                        {
		//                            if (registration.GRNDate != null)
		//                            {
		//                                int months = FixedAssetHelper.GetDepreciationMonths(registration.GRNDate, dateTo);
		//                                decimal rate = (decimal)group.PoolingPercentage;
		//                                decimal deprection = (rate * registration.PurchasePrice * months) / (12 * 100);
		//                                var depreciationExpenseLineVm = new DepreciationExpenseLineVm
		//                                {
		//                                    PurchaseDate = registration.GRNDate,
		//                                    Reference = registration.GRN,
		//                                    OriginalValue = registration.PurchasePrice,
		//                                    NoOfMonths = months,
		//                                    NoOfDays = (dateTo - (DateTime)registration.GRNDate).Days,
		//                                    Rate = rate,
		//                                    TotalDepForYear = Math.Round(deprection, 2),
		//                                    TotalDepForQuarter = Math.Round(deprection, 2),
		//                                    BookValueAtEnd = Math.Round((registration.PurchasePrice - deprection), 2),
		//                                };
		//                                depreciationExpenseVm.DepreciationExpenseLineVms.Add(depreciationExpenseLineVm);
		//                            }
		//                        }
		//                        if (depreciationExpenseVm.DepreciationExpenseLineVms.Any())
		//                        {
		//                            depreciationExpenseVm.OriginalValue = depreciationExpenseVm.DepreciationExpenseLineVms.Sum(x => x.OriginalValue);
		//                            depreciationExpenseVm.TotalDepForYear = depreciationExpenseVm.DepreciationExpenseLineVms.Sum(x => x.TotalDepForYear);
		//                            depreciationExpenseVm.TotalDepForQuarter = depreciationExpenseVm.DepreciationExpenseLineVms.Sum(x => x.TotalDepForQuarter);
		//                            depreciationExpenseVm.BookValueAtEnd = depreciationExpenseVm.DepreciationExpenseLineVms.Sum(x => x.BookValueAtEnd);
		//                            deprciationLists.Add(depreciationExpenseVm);
		//                        }
		//                    }
		//                }

		//            }
		//        }

		//    }

		//    return deprciationLists;
		//}
	}
}
