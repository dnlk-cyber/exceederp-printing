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
    public class HealthAndValueObjectDataSource : List<HealthValueViewModel>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<HealthValueViewModel> GetClinicReport(string branch, string department, string empId, DateTime fromTreatment, DateTime toTreatment, string status, string usage, bool unlimited,DateTime fromRecored,DateTime toRecored)
        {
            List<HealthValueViewModel> clinicList = new List<HealthValueViewModel>();

            var prescripitons = db.Prescription.ToList();
            var placements = db.Placements.OrderByDescending(x => x.PlacementId).ToList();
            var employees = db.Person_Employee.ToList();
            var patientCard = db.PatientCard.ToList();

            if (status == "Active")
            {
                placements = placements.Where(x => x.isCurrent).ToList();
                employees = employees.Where(x => x.IsActive).ToList();
            }
            if (status == "InActive")
            {
                placements = placements.Where(x => !x.isCurrent).ToList();
                employees = employees.Where(x => !x.IsActive).ToList();
            }

            var healthandvalues = db.HealthRankAndValues.ToList();
            if (!string.IsNullOrEmpty( branch ))
            {
                int.TryParse(branch, out int branchId);
                healthandvalues = healthandvalues.Where(x => x.BranchId == branchId).ToList();
            }
            if (!string.IsNullOrEmpty(department))
            {
                int.TryParse(department, out int departmentId);

                var placement = placements.OrderByDescending(x => x.PlacementId).FirstOrDefault(x => x.OrgStructureId == departmentId);
                if (placement != null)
                {
                    healthandvalues = healthandvalues.Where(x => x.EmployeeId == placement.EmployeeId).ToList();
                }

            }
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(empId, out int employeeId);
                healthandvalues = healthandvalues.Where(x => x.EmployeeId == employeeId).ToList();
            }

            if (fromTreatment != null && toTreatment != null && fromTreatment <= toTreatment)
            {
                healthandvalues = healthandvalues.Where(x => x.Date >= fromTreatment && x.Date <= toTreatment).ToList();
            }

            if (fromRecored != null && toRecored != null && fromRecored <= toRecored)
            {
                healthandvalues = healthandvalues.Where(x => x.RecoredDate >= fromRecored && x.RecoredDate <= toRecored).ToList();
            }

           var healthandValueDistictsList = healthandvalues.DistinctByField(x => x.EmployeeId).ToList();
           foreach (var healthandvalue in healthandvalues)
           {
                

                var healthvalue = new HealthValueViewModel();
               
                var employee = employees.FirstOrDefault(x => x.EmployeeId == healthandvalue.EmployeeId);
                if (employee != null)
                {
                    healthvalue.EmployeeId = employee.EmployeeId;
                    healthvalue.EmployeeCode = employee.EmployeeCode;
                    healthvalue.EmployeeName = employee.FullNameaEnglish;
                    healthvalue.GlFiscalYearId = healthandvalue.GlFiscalYearId;

                    var place = placements.FirstOrDefault(x => x.EmployeeId == healthandvalue.EmployeeId);
                    if (place != null)
                    {
                        var orgStructure = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                        if (orgStructure != null)
                        {
                            healthvalue.Department = orgStructure.Name;
                        }
                    }

                    var empBranch = db.Branch.FirstOrDefault(x => x.BranchId == healthandvalue.BranchId);
                    if (empBranch != null)
                    {
                        healthvalue.Branch = empBranch.Name;
                    }
                    var healthvalueDetailLists = db.HealthRankAndValues.Where(x => x.EmployeeId == employee.EmployeeId && x.GlFiscalYearId == healthandvalue.GlFiscalYearId).ToList().OrderByDescending(x => x.RecoredDate);
                 
                    healthvalue.HealthValueList = new List<HealthValueListViewModel>();

                    foreach (var health in healthvalueDetailLists)
                    {
                        var healthvalueList = new HealthValueListViewModel();
                        var fiscalyear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == health.GlFiscalYearId);
                        if (fiscalyear != null)
                        {
                            healthvalueList.FiscalYear = fiscalyear.Name;
                        }
                        var period = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == health.GLPeriodId);
                        if (period != null)
                        {
                            healthvalueList.Month = period.Name;
                        }

                        //healthvalueList.AllowedAmount = health.AllowedAmount.ToString();

                        healthvalueList.TreatedDate = health.DateAmh;
                        healthvalueList.ReferalDate = health.DateAmh;
                        healthvalueList.RecoredDate = health.RecoredDateAmh;
                        healthvalueList.UsedAmount = health.UsedAmount;
                        healthvalueList.RecoredDate = health.RecoredDateAmh;
                        healthvalueList.RecoredDate = health.RecoredDateAmh;
                        healthvalueList.RecoredDate = health.RecoredDateAmh;
                        healthvalueList.AllowedAmount = health.AllowedAmount.ToString();
                        healthvalueList.AllowedSum = health.AllowedAmount.ToString();
                        healthvalueList.RemainingSum = health.RemainingAmount.ToString();
                        healthvalueList.UsedSum = health.UsedAmount;
                        var pl = placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId);
                        if (pl != null)
                        {
                            var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == pl.RankId);
                           
                            if (rank != null && IsAlphabetOnly(rank.CurrentValue))
                            {
                                
                                healthvalueList.UsedAmount = health.UsedAmount;
                                healthvalueList.UsedSum = healthvalueDetailLists.Where(x=>x.EmployeeId == health.EmployeeId && x.GlFiscalYearId == health.GlFiscalYearId).Sum(x=>x.UsedAmount);
                                healthvalueList.AllowedAmount = "unlimited";
                                healthvalueList.AllowedSum = "unlimited";
                                healthvalueList.RemainingSum = "unlimited";
                                healthvalueList.RemainingAmount = "unlimited";//(Convert.ToDecimal(allowedAmounttt) - health.UsedAmount).ToString();
                            }
                            
                        }
                        healthvalue.HealthValueList.Add(healthvalueList);

                    }
                    healthvalue.HealthValueList.OrderByDescending(x => x.RecoredDate);

                    clinicList.Add(healthvalue);
                }
                //var item = healthandvalues.FirstOrDefault(x=>x.EmployeeId == healthandvalue.EmployeeId);
                //if (item != null)
                //     healthandvalues.RemoveAll(x=>x.EmployeeId == item.EmployeeId);
            }

            return clinicList;
        }

        public bool IsAlphabetOnly(string input)
        {
            foreach (char c in input)
            {
                if (!char.IsDigit(c))
                {
                    return true; // Found a Non chackter  character
                }
            }
            return false; // All characters are numeric
        }
    }
}

