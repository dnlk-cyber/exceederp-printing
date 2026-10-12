using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class NatinalBankGeneralSummaryODS : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<NatinalBankGeneralSummaryVM> GetNatinalBankGeneralSummary()
        {
            var result = new List<NatinalBankGeneralSummaryVM>();

            var organization = db.Companies
                .AsNoTracking()
                .FirstOrDefault(x => x.Category == "0");

            if (organization == null)
                return result;
            var fyearId = _fiscalYearHelper.GetCurrentFYearId();
            var fiscalYear = db.GLFiscalYears.Find(fyearId);
            var comp = new NatinalBankGeneralSummaryVM
            {
                InstitutionName = organization.TradeName ?? organization.AmharicName,
                PeriodEnding = fiscalYear?.DateTo.ToString("dd/MM/yyyy") + " G.C / "+ HrHelper.GetECDate(fiscalYear.DateTo) + " E.C",

                BranchNetworks = new List<BranchNetworkItemViewModel>()
            };

            //  Get addresses once
            var addresses = db.Addresses
                .AsNoTracking()
                .Where(a => a.Reference == organization.MainCompanyId.ToString()
                         && a.ReferenceType.ToLower() == "company")
                .ToList();

            foreach (var a in addresses)
            {
                switch (a.Category)
                {
                    case AddressType.CITYPROVINCE:
                        comp.City = $"City: {a.Value}";
                        break;
                    case AddressType.KIFLEKETEMA:
                        comp.SubCityOrWoreda = $"Subcity: {a.Value}";
                        break;

                    case AddressType.KEBELE:
                        comp.Kebele = $"Kebele: {a.Value}";
                        break;

                    case AddressType.REGION:
                        comp.Region = $"{a.Value} - Ethiopia";
                        break;

                    case AddressType.OFFICE_EMAIL:
                        comp.Email = $"Email: {a.Value}";
                        break;

                    case AddressType.WEBSITE:
                        comp.Website = $"Website: {a.Value}";
                        break;

                    case AddressType.FAX:
                        comp.Fax = $"Fax: {a.Value}";
                        break;

                    case AddressType.OFFICE_PHONE:
                        comp.Telephone = $"Tel: {a.Value}";
                        break;
                }
            }

            //  Preload branch counts (avoid N+1 query)
            //var branchCounts = db.BusinessUnitBranches
            //    .AsNoTracking()
            //    .GroupBy(x => x.BusinessUnitId)
            //    .Select(g => new
            //    {
            //        BusinessUnitId = g.Key,
            //        Branches = g.Count(x => !x.IsSubBranch),
            //        SubBranches = g.Count(x => x.IsSubBranch)
            //    })
            //    .ToList();

            //// Convert to dictionary for fast lookup
            //var branchDict = branchCounts.ToDictionary(x => x.BusinessUnitId);

            //  Get business units (lightweight)
            var businessUnits = db.BusinessUnits
                .AsNoTracking()
                .Select(x => new
                {
                    x.BusinessUnitId,
                    x.Name,
                    x.Accountcode
                })
                .ToList();

            foreach (var bu in businessUnits)
            {
               // branchDict.TryGetValue(bu.BusinessUnitId, out var count);

                comp.BranchNetworks.Add(new BranchNetworkItemViewModel
                {
                    BusinessUnit = bu.Name ?? bu.Accountcode,
                    NumberOfBranches = /*count?.Branches ??*/ 0,
                    NumberOfSubBranches = /*count?.SubBranches ??*/ 0
                });
            }

            //  FIX: Add to result
            result.Add(comp);

            return result;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
