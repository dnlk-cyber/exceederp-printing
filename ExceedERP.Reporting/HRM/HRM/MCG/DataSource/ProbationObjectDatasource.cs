using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.Letters.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ExceedERP.Reporting.HRM.HRM.MCG.DataSource
{
    [DataObject]
    public class ProbationObjectDatasource : IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();
        private object placeWorkedBuilder;

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ProbationVM GetProbation(string empId)
        {
            var result = new ProbationVM
            {
                EmployeeAllowances = new List<EmployeeAllowanceVM>()
            };

            if (string.IsNullOrWhiteSpace(empId))
                return result;

            if (!int.TryParse(empId, out int employeeId))
                return result;

            var employee = _db.Person_Employee
                .AsNoTracking()
                .FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);

            if (employee == null)
                return result;

            bool isAmharic = !string.IsNullOrWhiteSpace(cultureName) &&
                             (cultureName.Equals("am", StringComparison.OrdinalIgnoreCase) ||
                              cultureName.Equals("am-et", StringComparison.OrdinalIgnoreCase));

            result.EmployeeCode = employee.EmployeeCode;
            result.Sex = HrHelper.GetGender(employee.Gender);
            result.FullName = isAmharic
                ? employee.FullNameAmharic
                : employee.FullNameaEnglish;

            var placement = _db.Placements
                .AsNoTracking()
                .Where(x => x.EmployeeId == employeeId && x.isCurrent)
                .Select(x => new
                {
                    x.EmployeeId,
                    x.LookupId,
                    x.RankId,
                    x.StepId,
                    x.BranchId,
                    x.OrgStructureId,
                    x.Salary,
                    x.AssignmentDate,
                    x.AssignmentDateAmharic,
                    x.ResponsibilityLevel})
                .FirstOrDefault();

            if (placement == null)
                return result;

            // Load all related lookup data in parallel queries
            var lookup = _db.Lookups
                .AsNoTracking()
                .Where(x => x.LookupId == placement.LookupId)
                .Select(x => x.Description)
                .FirstOrDefault();

            var rank = _db.Ranks
                .AsNoTracking()
                .Where(x => x.RankId == placement.RankId)
                .Select(x => x.RankName)
                .FirstOrDefault();

            var step = _db.Steps
                .AsNoTracking()
                .Where(x => x.StepId == placement.StepId)
                .Select(x => x.StepName)
                .FirstOrDefault();

            var branch = _db.Branch
                .AsNoTracking()
                .Where(x => x.BranchId == placement.BranchId)
                .Select(x => x.Name)
                .FirstOrDefault();

            var department = _db.MainCompanyStructures
                .AsNoTracking()
                .Where(x => x.OrgStructureId == placement.OrgStructureId)
                .Select(x => x.Name)
                .FirstOrDefault();
            var probation = _db.ProbationsSettings.FirstOrDefault(x => x.ResponsibilityLevel == placement.ResponsibilityLevel);

            result.JobTitle = lookup ?? string.Empty;
            result.GradeAndScale = isAmharic? $"ደ={rank ?? ""} & እ={step ?? ""}": $"G={rank ?? ""} & S={step ?? ""}";
            result.Branch = branch ?? string.Empty;
            result.WorkUnit = department ?? string.Empty;
            result.Earnings = "Salary";
            result.Salary = placement.Salary;
            result.ProbationPeriod = probation?.ProbationDays??60m;

            result.LastAssignmentDate = isAmharic ?
                $"እ.እ.አ {placement.AssignmentDate:MMMM dd, yyyy}  / {placement.AssignmentDateAmharic} ዓ.ም"
                :$"{placement.AssignmentDate:MMMM dd, yyyy} G.C / {placement.AssignmentDateAmharic} E.C";

            // Load all employee allowances
            var employeeAllowances = _db.AllowancePerEmployees
                .AsNoTracking()
                .Where(x => x.EmployeeId == employeeId && !x.Pend)
                .ToList();

            if (employeeAllowances.Any())
            {
                var allowanceIds = employeeAllowances
                    .Select(x => x.AllowanceDefinationId)
                    .Distinct()
                    .ToList();

                var allowanceDefinitions = _db.AllowanceDefinations
                    .AsNoTracking()
                    .Where(x => allowanceIds.Contains(x.AllowanceDefinationId))
                    .Select(x => new
                    {
                        x.AllowanceDefinationId,
                        x.AllowanceName,
                        x.AllowanceType,
                        x.Amount
                    })
                    .ToDictionary(x => x.AllowanceDefinationId);

                result.EmployeeAllowances = employeeAllowances
                    .Where(a => allowanceDefinitions.ContainsKey(a.AllowanceDefinationId))
                    .Select(a =>
                    {
                        var def = allowanceDefinitions[a.AllowanceDefinationId];

                        return new EmployeeAllowanceVM
                        {
                            Earnings = $"{def?.AllowanceName??null} {def?.AllowanceType.ToString()}",
                            Amounts = def?.Amount??0
                        };
                    })
                    .ToList();
            }

            return result;
        }
        public void Dispose()
        {
            _db.Dispose();
        }
    }
   
}
