using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.FixedAsset.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    public class DepreciationExpenseByDepartmentOds : List<DepreciationExpenseByDepartmentVm>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public DepreciationHeaderViewModel DepreciationExpenseByDepartmentHeader(string fiscalYear, string quarter)
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
        public List<DepreciationExpenseByDepartmentVm> GetDepreciationExpenseByDepartment(string fiscalYear, string fromPeriod, string toPeriod, string quarter)
        {
            List<DepreciationExpenseByDepartmentVm> byDepartmentVms = new List<DepreciationExpenseByDepartmentVm>();
            List<DepreciationExpenseVm> deprciationLists = new List<DepreciationExpenseVm>();

            if (!String.IsNullOrWhiteSpace(fromPeriod) && !String.IsNullOrWhiteSpace(toPeriod))
            {
                #region calculate depreciation

                int from = int.Parse(fromPeriod);
                int to = int.Parse(toPeriod);
                DateTime dateFrom = _db.GLPeriods.Find(from).DateFrom;
                DateTime dateTo = _db.GLPeriods.Find(to).DateTo;


                var assetRegistrations = _db.AssetRegistrations
                    .Where(x => x.GRNDate >= dateFrom)
                    .Where(x => x.GRNDate <= dateTo).ToList();

                List<int> categorIds = assetRegistrations.Select(x => x.FixedAssetCategoryID).Distinct().ToList();

                foreach (var catId in categorIds)
                {
                    var category = _db.FixedAssetCategories.Find(catId);
                    if (category != null)
                    {
                        var registrations = assetRegistrations.Where(x => x.FixedAssetCategoryID == catId)
                            .AsQueryable();

                        var group = _db.FixedAssetGroups.Find(category.FixedAssetGroupID);

                        if (group != null)
                        {
                            var depreciationExpenseVm = new DepreciationExpenseVm
                            {
                                Category = category.CategoryName,
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
                                        PurchaseDate = registration.PurchaseDate,
                                        Reference = registration.GRN,
                                        OriginalValue = registration.Quanity * registration.PurchasePrice,
                                        NoOfMonths = (int)(days / 30),
                                        NoOfDays = (int)days,
                                        Rate = rate,
                                        TotalDepForYear = Math.Round(deprection, 2),
                                        TotalDepForQuarter = Math.Round(deprection, 2),
                                        BookValueAtEnd = Math.Round((registration.Quanity * registration.PurchasePrice - deprection), 2),
                                    };
                                    depreciationExpenseVm.DepreciationExpenseLineVms.Add(depreciationExpenseLineVm);
                                }
                            }
                            if (depreciationExpenseVm.DepreciationExpenseLineVms.Any())
                                deprciationLists.Add(depreciationExpenseVm);
                        }
                    }

                }
                #endregion
                foreach (var catId in categorIds)
                {
                    decimal beginning = 0;
                    var category = _db.FixedAssetCategories.Find(catId);
                    if (category != null)
                    {
                        var catDeprciations =
                            deprciationLists.FirstOrDefault(x => x.Category == category.CategoryName);

                        var lines = catDeprciations?.DepreciationExpenseLineVms.ToList();
                        int preYear = new FiscalYearHelper().GetPreviousFYearId(int.Parse(fiscalYear));
                        var begBalance = _db.CategoryDepreciationValues.Where(x => x.GlFiscalYearId == preYear)
                            .FirstOrDefault(x => x.FixedAssetCategoryID == catId);
                        if (begBalance != null) beginning = begBalance.DepreciationAmount;

                        if (lines != null)
                        {
                            decimal total = beginning + lines.Sum(x => x.TotalDepForQuarter);
                            var depreciationExpenseByDepartmentVm = new DepreciationExpenseByDepartmentVm
                            {
                                Category = category.CategoryName,
                                Beginning=beginning,
                                TotalDepExpense = total,
                                PoHDepExpenseTv = category.CategoryName== "TvEquipment"? total * (decimal)0.89: (total * (decimal)0.89)/2,
                                PoHDepExpenseRadio = category.CategoryName == "TvEquipment" ? 0 : (total * (decimal)0.89) / 2,
                                MarketingDepExpenseTv = category.CategoryName == "TvEquipment" ? total * (decimal)0.03 : (total * (decimal)0.03) / 2,
                                MarketingDepExpenseRadio = category.CategoryName == "TvEquipment" ? 0 : (total * (decimal)0.03) / 2,
                                AdminDepExpenseTv = category.CategoryName == "TvEquipment" ? total * (decimal)0.08 : (total * (decimal)0.08) / 2,
                                AdminDepExpenseRadio = category.CategoryName == "TvEquipment" ? 0 : (total * (decimal)0.08) / 2,
                            };
                            byDepartmentVms.Add(depreciationExpenseByDepartmentVm);
                        }
                    }

                }


            }

            return byDepartmentVms;
        }
    }
}
