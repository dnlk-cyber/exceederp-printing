using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePromotionPerYearObjectDataSource : List<EmployeePromotion>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePromotion> GetEmployeePromotionPerYer(string dept, string branchId)
        {
            List<EmployeePromotion> EmployeeInfoList = new List<EmployeePromotion>();
            EmployeePromotion EmployeeInfo = new EmployeePromotion();

            int PromotionPerYear = 0;

            var configration = db.Configurations.FirstOrDefault(x => x.Reference == "YEARLY_PROMOTION_PER_YEAR" && x.Attribute == "Promotion");
            if (configration != null)
            {
                PromotionPerYear = Convert.ToInt32(configration.CurrentValue);
            }

            var placements = db.Placements.Where(x => x.AssignmentType == AssignmentType.PROMOTION).ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            }
            if (dept != null)
            {
                int.TryParse(dept, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
            }

            foreach (var e in placements)
            {
                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);

                if (employee != null)
                {

                    EmployeeInfo = new EmployeePromotion();

                    if (employee.Gender == Gender.MALE)
                    {
                        EmployeeInfo.Sex = "ወንድ";
                    }
                    else
                    {
                        EmployeeInfo.Sex = "ሴት";
                    }

                    EmployeeInfo.FullName = employee.FullNameAmharic;
                    EmployeeInfo.EmployeeId = employee.EmployeeCode;

                    var previousPlacment = db.Placements.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();

                    if (previousPlacment != null)
                    {
                        var prevStep = db.Steps.FirstOrDefault(x => x.StepId == previousPlacment.StepId);
                        if (prevStep != null)
                        {
                            EmployeeInfo.FromStep = prevStep.StepName;
                        }

                    }

                    var toStep = db.Steps.FirstOrDefault(x => x.StepId == e.StepId);

                    if (toStep != null)
                    {
                        var nextStep = db.Steps.FirstOrDefault(x => x.StepSequence == toStep.StepSequence + 1);

                        if (nextStep != null)
                        {
                            EmployeeInfo.ToStep = nextStep.StepName;

                            var salary = db.Salaries.FirstOrDefault(x => x.RankId == e.RankId && x.StepId == nextStep.StepId);
                            if (salary != null)
                            {
                                EmployeeInfo.Salary = salary.SalaryAmount;
                            }
                            else
                            {
                                var JobTitlesalary = db.SalariesJobTitles.FirstOrDefault(x => x.RankId == e.RankId && x.StepId == nextStep.StepId && x.LookupId == e.LookupId);
                                if (JobTitlesalary != null)
                                {
                                    EmployeeInfo.Salary = JobTitlesalary.Salary;
                                }
                            }
                        }
                    }

                    EmployeeInfo.YearOfPromotion = e.AssignmentDate.Year;

                    var job = db.Lookups.FirstOrDefault(x => x.LookupId == e.LookupId);

                    if (job != null)
                    {
                        EmployeeInfo.JobTitle = job.Description;
                    }
                    var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == e.OrgStructureId);

                    if (deptt != null)
                    {
                        EmployeeInfo.Departement = deptt.Name;
                    }

                    EmployeeInfoList.Add(EmployeeInfo);

                }
            }

            return EmployeeInfoList;
        }
    }
}