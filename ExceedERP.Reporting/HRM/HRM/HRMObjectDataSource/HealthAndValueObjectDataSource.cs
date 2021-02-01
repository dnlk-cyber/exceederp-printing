using ExceedERP.DataAccess.Context;
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
            if (branch != null)
            {
                healthandvalues = healthandvalues.Where(x => x.BranchId.ToString() == branch).ToList();
            }
            if (department != null)
            {
                var placement = placements.OrderByDescending(x => x.PlacementId).FirstOrDefault(x => x.OrgStructureId.ToString() == department);
                if (placement != null)
                {
                    healthandvalues = healthandvalues.Where(x => x.EmployeeId == placement.EmployeeId).ToList();
                }

            }
            if (empId != null)
            {
                healthandvalues = healthandvalues.Where(x => x.EmployeeId.ToString() == empId).ToList();
            }

            if (fromTreatment != null && toTreatment != null && fromTreatment <= toTreatment)
            {
                healthandvalues = healthandvalues.Where(x => x.Date >= fromTreatment && x.Date <= toTreatment).ToList();
            }

            if (fromRecored != null && toRecored != null && fromRecored <= toRecored)
            {
                healthandvalues = healthandvalues.Where(x => x.RecoredDate >= fromRecored && x.RecoredDate <= toRecored).ToList();
            }

            //if(usage == "On Limit")
            //{
            //    healthandvalues = healthandvalues.Where(x => x.RemainingAmount >= 0).ToList();
            //}
            //if (usage == "Above Limit")
            //{
            //    healthandvalues = healthandvalues.Where(x => x.RemainingAmount < 0).ToList();
            //}
            if (unlimited)
            {
                var healthsettings = db.HealthRankAndValueSettings.Where(x => x.CurrentValue.ToLower() == "unlimited".ToLower());
                if (healthsettings.Any())
                {
                    var ranks = healthsettings.Select(x => x.RankId);
                    if (ranks.Any())
                    {
                        var places = db.Placements.Where(x => ranks.Contains(x.RankId) && x.isCurrent);
                        if (places.Any())
                        {
                            var emps = places.Select(x => x.EmployeeId);
                            if (emps.Any())
                            {
                                healthandvalues = healthandvalues.Where(x => emps.Contains(x.EmployeeId)).ToList();
                            }
                        }
                    }
                }
            }

            var ddd = 0;
            foreach (var healthandvalue in healthandvalues)
            {
                var checkRemaining = healthandvalues.Where(x => x.EmployeeId == healthandvalue.EmployeeId && x.GlFiscalYearId == healthandvalue.GlFiscalYearId);
                if (checkRemaining.Any())
                {
                    var sumOfUsed = checkRemaining.Sum(x => x.UsedAmount);
                    if (usage == "On Limit" && healthandvalue.AllowedAmount != 0 && sumOfUsed > healthandvalue.AllowedAmount)
                    {
                        continue;
                    }
                    if (usage == "Above Limit" && (sumOfUsed <= healthandvalue.AllowedAmount || healthandvalue.AllowedAmount == 0))
                    {
                        continue;
                    }
                }
                var checkExistance = clinicList.Where(x => x.EmployeeId == healthandvalue.EmployeeId && x.GlFiscalYearId == healthandvalue.GlFiscalYearId);


                var healthvalue = new HealthValueViewModel();


                healthvalue.HealthValueList = new List<HealthValueListViewModel>();
                if (checkExistance.Any())
                {
                    continue;
                }
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

                    var healthvalueDetailLists = healthandvalues.Where(x => x.EmployeeId == employee.EmployeeId && x.GlFiscalYearId == healthandvalue.GlFiscalYearId).ToList().OrderByDescending(x => x.RecoredDate);
                    decimal allowed = 0;
                    decimal sumofUsed = 0;
                    decimal sumofRemaining = 0;
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
                        var pl = placements.FirstOrDefault(x => x.EmployeeId == health.EmployeeId);
                        if (pl != null)
                        {
                            var rank = db.HealthRankAndValueSettings.FirstOrDefault(x => x.RankId == pl.RankId);
                            if (rank != null && rank.CurrentValue.ToLower() != "unlimited")
                            {
                                healthvalueList.UsedAmount = health.UsedAmount;
                                var allowedAmount = rank.CurrentValue;
                                healthvalueList.AllowedAmount = allowedAmount;
                                healthvalueList.AllowedSum = allowedAmount;
                                healthvalueList.RemainingSum = (Convert.ToDecimal(allowedAmount) - healthvalueDetailLists.Where(x => x.EmployeeId == health.EmployeeId && x.GlFiscalYearId == health.GlFiscalYearId).Sum(x => x.UsedAmount)).ToString();
                                healthvalueList.UsedSum = healthvalueDetailLists.Where(x => x.EmployeeId == health.EmployeeId && x.GlFiscalYearId == health.GlFiscalYearId).Sum(x => x.UsedAmount);
                              var totalUsed = healthvalueDetailLists.Where(x => x.GlFiscalYearId == health.GlFiscalYearId && x.RecoredDate < health.RecoredDate);
                                if (totalUsed.Any())
                                {
                                    var totalSum = totalUsed.Sum(x => x.UsedAmount) + health.UsedAmount;
                                    healthvalueList.RemainingAmount = (Convert.ToDecimal(allowedAmount) - totalSum).ToString();
                                   
                                }
                                else
                                {
                                    healthvalueList.RemainingAmount = (Convert.ToDecimal(allowedAmount) - health.UsedAmount).ToString();
                                }

                            }
                            else if (rank != null && rank.CurrentValue.ToLower() == "unlimited")
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
    }
}

