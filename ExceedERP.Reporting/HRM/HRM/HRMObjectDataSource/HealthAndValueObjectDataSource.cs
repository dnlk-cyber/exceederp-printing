using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{


    [DataObject]
    public class HealthAndValueObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<HealthValueViewModel> GetClinicReport(string branch,string department, string empId, DateTime? fromTreatment, DateTime? toTreatment,
                                                          string paymentType, string status, string usage, bool unlimited,  DateTime? fromRecored, DateTime? toRecored,string benefitType)
        {
            // Base query (defer ToList until the end)
            var healthQuery = db.HealthRankAndValues.AsQueryable();
            var placementQuery = db.Placements.AsQueryable();

            // Status filter
            if (status == "Active")
            {
                placementQuery = placementQuery.Where(x => x.isCurrent);
            }
            else if (status == "InActive")
            {
                placementQuery = placementQuery.Where(x => !x.isCurrent);
            }

            // Branch filter
            if (int.TryParse(branch, out int branchId))
            {
                healthQuery = healthQuery.Where(x => x.BranchId == branchId);
            }

            // Department filter
            if (int.TryParse(department, out int departmentId))
            {
                var employeeIds = placementQuery
                    .Where(x => x.OrgStructureId == departmentId)
                    .Select(x => x.EmployeeId)
                    .ToList();
                healthQuery = healthQuery.Where(x => employeeIds.Contains(x.EmployeeId));
            }

            // Employee filter
            if (int.TryParse(empId, out int employeeId))
            {
                healthQuery = healthQuery.Where(x => x.EmployeeId == employeeId);
            }
            // benefit filter
            if (!string.IsNullOrWhiteSpace(benefitType))
            {
                healthQuery = healthQuery.Where(x => x.BenefitType.ToString() == benefitType);
            }

            // Date filters
            if (fromTreatment.HasValue)
                healthQuery = healthQuery.Where(x => x.Date >= fromTreatment.Value);

            if (toTreatment.HasValue)
                healthQuery = healthQuery.Where(x => x.Date <= toTreatment.Value);

            if (fromRecored.HasValue)
                healthQuery = healthQuery.Where(x => x.RecoredDate >= fromRecored.Value);

            if (toRecored.HasValue)
                healthQuery = healthQuery.Where(x => x.RecoredDate <= toRecored.Value);

            // Payment type
            if (!string.IsNullOrEmpty(paymentType))
            {
                healthQuery = healthQuery.Where(x => x.PaymentType.ToString() == paymentType);
            }

            // Execute queries
            var healthList = healthQuery.ToList();
            var placements = placementQuery.ToList();

            // Distinct Employee-Year
            var distinctHealthAndValues = healthList
                .GroupBy(x => new { x.EmployeeId, x.GlFiscalYearId,x.BenefitType })
                .Select(g => g.First())
                .ToList();

            var clinicList = new List<HealthValueViewModel>();

            foreach (var hv in distinctHealthAndValues)
            {
                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == hv.EmployeeId);
                if (employee == null) continue;

                var viewModel = new HealthValueViewModel
                {
                    EmployeeId = employee.EmployeeId,
                    EmployeeCode = employee.EmployeeCode,
                    EmployeeName = employee.FullNameaEnglish
                };

                var place = placements.FirstOrDefault(x => x.EmployeeId == hv.EmployeeId&&x.isCurrent)??placements.FirstOrDefault(x => x.EmployeeId == hv.EmployeeId);
                if (place != null)
                {
                    viewModel.Department = db.MainCompanyStructures
                        .FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId)?.Name;
                }

                viewModel.Branch = db.Branch.FirstOrDefault(x => x.BranchId == hv.BranchId)?.Name;

                var fiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == hv.GlFiscalYearId);
                if (fiscalYear != null)
                {
                    viewModel.FiscalYear = fiscalYear.Name;
                    viewModel.YeearStartDate = fiscalYear.DateFrom;
                    viewModel.FiscalYearId = fiscalYear.GlFiscalYearId;
                }

                // Allowed amount logic
                string allowed = GetAllowedAmount(place);
                viewModel.AllowedAmount = allowed;
                viewModel.BenefitType = hv.BenefitType.ToString();

                // Health details
                var healthDetails = healthList
                    .Where(x => x.EmployeeId == hv.EmployeeId && x.GlFiscalYearId == x.GlFiscalYearId && x.BenefitType ==hv.BenefitType)
                    .OrderBy(x => x.RecoredDate)
                    .ToList();

                viewModel.HealthValueList = new List<HealthValueDetailVM>();

                foreach (var health in healthDetails)
                {
                    var detail = new HealthValueDetailVM
                    {
                        Month = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == health.GLPeriodId)?.Name,
                        TreatedDate = health.DateAmh,
                        ReferalDate = health.DateAmh,
                        RecoredDate = health.RecoredDateAmh,
                        UsedAmount = health.UsedAmount
                    };

                    if (allowed == "Unlimited")
                    {
                        // No computation needed
                        detail.AllowedAmount = "Unlimited";
                        detail.RemainingAmount = "Unlimited";
                        detail.AllowedSum = 0;
                        detail.RemainingSum = 0;
                        detail.UsedSum = 0; // Still useful for total usage
                    }
                    else
                    {
                        detail.AllowedAmount = health.AllowedAmount.ToString();
                        detail.RemainingAmount = health.RemainingAmount.ToString();
                        detail.AllowedSum = health.AllowedAmount;
                        detail.RemainingSum = health.RemainingAmount;
                        detail.UsedSum = healthDetails.Sum(x => x.UsedAmount);
                    }

                    viewModel.HealthValueList.Add(detail);
                }

                viewModel.HealthValueList = viewModel.HealthValueList
                    .OrderByDescending(x => x.RecoredDate)
                    .ToList();

                clinicList.Add(viewModel);
            }

            return clinicList;
        }

        /// <summary>
        /// Returns "Unlimited" if rank/level is non-numeric.
        /// Otherwise returns numeric string value.
        /// </summary>
        private string GetAllowedAmount(Placement place)
        {
            if (place == null) return string.Empty;

            var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == place.RankId);
            var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == place.ResponsibilityLevel);

            if (rank != null && !IsNumeric(rank.CurrentValue)) return "Unlimited";
            if (level != null && !IsNumeric(level.CurrentValue)) return "Unlimited";

            return rank?.CurrentValue ?? level?.CurrentValue ?? string.Empty;
        }

        private bool IsNumeric(string input)
        {
            return input.All(char.IsDigit);
        }
        public void Dispose()
        {
            db.Dispose();
        }
        //public List<HealthValueViewModel> GetClinicReport(string branch, string department, string empId, DateTime? fromTreatment, DateTime? toTreatment, string paymentType, string status, string usage, bool unlimited, DateTime? fromRecored, DateTime? toRecored)
        //{
        //    List<HealthValueViewModel> clinicList = new List<HealthValueViewModel>();
        //    List<int> employeeIds = new List<int>();
        //    var healthandvalues = db.HealthRankAndValues.ToList();
        //    var placements = db.Placements.ToList();
        //    if (status == "Active")
        //    {
        //        placements = db.Placements.Where(x => x.isCurrent).ToList();
        //        employeeIds = placements.Select(x => x.EmployeeId).ToList();
        //        if (employeeIds.Any())
        //        {
        //            healthandvalues = healthandvalues.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
        //        }

        //    }
        //    if (status == "InActive")
        //    {
        //        placements = db.Placements.Where(x => !x.isCurrent).ToList();

        //        employeeIds = placements.Select(x => x.EmployeeId).ToList();
        //        if (employeeIds.Any())
        //        {
        //            healthandvalues = healthandvalues.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
        //        }
        //    }


        //    if (!string.IsNullOrEmpty(branch))
        //    {
        //        int.TryParse(branch, out int branchId);
        //        healthandvalues = healthandvalues.Where(x => x.BranchId == branchId).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(department))
        //    {
        //        int.TryParse(department, out int departmentId);

        //        employeeIds = placements.Where(x => x.OrgStructureId == departmentId).Select(x => x.PlacementId).ToList();
        //        if (employeeIds.Any())
        //        {
        //            healthandvalues = healthandvalues.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
        //        }

        //    }
        //    if (!string.IsNullOrEmpty(empId))
        //    {
        //        int.TryParse(empId, out int employeeId);
        //        healthandvalues = healthandvalues.Where(x => x.EmployeeId == employeeId).ToList();
        //    }

        //    if (fromTreatment.HasValue)
        //    {
        //        healthandvalues = healthandvalues.Where(x => x.Date >= fromTreatment.Value).ToList();
        //    }
        //    if (toTreatment.HasValue)
        //    {
        //        healthandvalues = healthandvalues.Where(x => x.Date <= toTreatment.Value).ToList();
        //    }

        //    if (fromRecored.HasValue)
        //    {
        //        healthandvalues = healthandvalues.Where(x => x.RecoredDate >= fromRecored.Value).ToList();
        //    }
        //    if (toRecored.HasValue)
        //    {
        //        healthandvalues = healthandvalues.Where(x => x.RecoredDate <= toRecored.Value).ToList();
        //    }
        //    if (!string.IsNullOrEmpty(paymentType))
        //    {
        //        healthandvalues = healthandvalues.Where(x => x.PaymentType.ToString() == paymentType).ToList();
        //    }
        //    var distinctHealthAndValues = healthandvalues
        //                 .GroupBy(x => new { x.EmployeeId, x.GlFiscalYearId })
        //                 .Select(group => group.First()).ToList();

        //    foreach (var healthandvalue in distinctHealthAndValues)
        //    {

        //        var healthvalue = new HealthValueViewModel();

        //        var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == healthandvalue.EmployeeId);
        //        if (employee != null)
        //        {
        //            healthvalue.EmployeeId = employee.EmployeeId;
        //            healthvalue.EmployeeCode = employee.EmployeeCode;
        //            healthvalue.EmployeeName = employee.FullNameaEnglish;

        //            var place = placements.FirstOrDefault(x => x.EmployeeId == healthandvalue.EmployeeId);
        //            if (place != null)
        //            {
        //                var orgStructure = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
        //                if (orgStructure != null)
        //                {
        //                    healthvalue.Department = orgStructure.Name;
        //                }
        //            }

        //            var empBranch = db.Branch.FirstOrDefault(x => x.BranchId == healthandvalue.BranchId);
        //            if (empBranch != null)
        //            {
        //                healthvalue.Branch = empBranch.Name;
        //            }
        //            var fiscalyear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == healthandvalue.GlFiscalYearId);
        //            if (fiscalyear != null)
        //            {
        //                healthvalue.FiscalYear = fiscalyear.Name;
        //                healthvalue.YeearStartDate = fiscalyear.DateFrom;
        //                healthvalue.FiscalYearId = fiscalyear.GlFiscalYearId;

        //            }
        //            var placem = placements.FirstOrDefault(x => x.EmployeeId == healthvalue.EmployeeId);
        //            if (placem != null)
        //            {
        //                var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == placem.RankId);
        //                var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == placem.ResponsibilityLevel);
        //                if (rank != null && IsAlphabetOnly(rank.CurrentValue))
        //                {

        //                    healthvalue.AllowedAmount = "Unlimited";

        //                }
        //                else if (level != null && IsAlphabetOnly(level.CurrentValue))
        //                {

        //                    healthvalue.AllowedAmount = "Unlimited";
        //                }
        //                else
        //                {
        //                    healthvalue.AllowedAmount = rank !=null?rank.CurrentValue:level !=null?level.CurrentValue:healthvalue.AllowedAmount;

        //                }

        //            }
        //            var healthvalueDetailLists = healthandvalues.Where(x=>x.GlFiscalYearId== healthandvalue.GlFiscalYearId).Where(x => x.EmployeeId == healthandvalue.EmployeeId).ToList().OrderBy(x => x.RecoredDate);

        //            healthvalue.HealthValueList = new List<HealthValueDetailVM>();


        //            foreach (var health in healthvalueDetailLists)
        //            {
        //                var healthvalueList = new HealthValueDetailVM();
        //                var period = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == health.GLPeriodId);
        //                if (period != null)
        //                {
        //                    healthvalueList.Month = period.Name;
        //                }


        //                healthvalueList.TreatedDate = health.DateAmh;
        //                healthvalueList.ReferalDate = health.DateAmh;
        //                healthvalueList.RecoredDate = health.RecoredDateAmh;
        //                healthvalueList.AllowedAmount = health.AllowedAmount.ToString();
        //                healthvalueList.UsedAmount = health.UsedAmount;
        //                healthvalueList.RemainingAmount = health.RemainingAmount.ToString();
        //                healthvalueList.AllowedSum = health.AllowedAmount;
        //                healthvalueList.RemainingSum = health.RemainingAmount;
        //                var pl = placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId);
        //                if (pl != null)
        //                {
        //                    var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == pl.RankId);
        //                    var level = db.HealthResponsiblityLevelValueSettings.FirstOrDefault(x => x.Level == pl.ResponsibilityLevel);
        //                    if (rank != null && IsAlphabetOnly(rank.CurrentValue))
        //                    {

        //                        healthvalueList.UsedAmount = health.UsedAmount;
        //                        healthvalueList.UsedSum = healthvalueDetailLists.Where(x => x.EmployeeId == health.EmployeeId && x.GlFiscalYearId == health.GlFiscalYearId).Sum(x => x.UsedAmount);
        //                        //healthvalueList.UsedSum = healthvalueList.UsedAmount;
        //                        healthvalueList.AllowedAmount = "Unlimited";
        //                        healthvalueList.RemainingSum = 0;
        //                        healthvalueList.RemainingSum = 0;
        //                        healthvalueList.RemainingAmount = "Unlimited";//(Convert.ToDecimal(allowedAmounttt) - health.UsedAmount).ToString();
        //                    }
        //                    else if (level != null && IsAlphabetOnly(level.CurrentValue))
        //                    {

        //                        healthvalueList.UsedAmount = health.UsedAmount;
        //                        healthvalueList.UsedSum = healthvalueDetailLists.Where(x => x.EmployeeId == health.EmployeeId && x.GlFiscalYearId == health.GlFiscalYearId).Sum(x => x.UsedAmount);
        //                        healthvalueList.AllowedAmount = "Unlimited";
        //                        healthvalueList.AllowedSum = 0;
        //                        healthvalueList.RemainingSum = 0;
        //                        healthvalueList.RemainingAmount = "Unlimited";//(Convert.ToDecimal(allowedAmounttt) - health.UsedAmount).ToString();
        //                    }

        //                }

        //                healthvalue.HealthValueList.Add(healthvalueList);

        //                healthvalue.HealthValueList.OrderByDescending(x => x.RecoredDate)/*.ThenBy(x=>x.FiscalYearId)*/;
        //            }
        //            healthvalue.HealthValueList.OrderByDescending(x => x.RecoredDate)/*.ThenBy(x => x.FiscalYearId)*/;

        //            clinicList.Add(healthvalue);
        //        }


        //    }

        //    return clinicList;
        //}

        //public bool IsAlphabetOnly(string input)
        //{
        //    foreach (char c in input)
        //    {
        //        if (!char.IsDigit(c))
        //        {
        //            return true; // Found a Non chackter  character
        //        }
        //    }
        //    return false; // All characters are numeric
        //}
    }
}

