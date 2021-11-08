using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeByBranchObjectDataSource : List<EmployeeByBranch>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeByBranch> GetEmployeeByBranch(string branch, string orgId, string sex, string empType)
        {
            List<EmployeeByBranch> EmployeeByBranchList = new List<EmployeeByBranch>();
            EmployeeByBranch EmployeeByBranch = new EmployeeByBranch();

            var place = db.Placements.Where(x => x.isCurrent).ToList();
            if (branch != null)
            {
                int.TryParse(branch, out int branchId);
                place = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(place, branchId);
            }
            if (orgId != null)
            {
                int.TryParse(orgId, out int orgStructureId);
                place = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(place, orgStructureId);
            }
            if (sex == "FEMALE")
            {
                place = EmployeeHelper.Get_Female_EmployeesFrom_Placement(place);
            }
            else if (sex == "MALE")
            {
                place = EmployeeHelper.Get_Male_EmployeesFrom_Placement(place);
            }
            if (!string.IsNullOrEmpty(empType))
            {
                var type = GetEmpType(empType);
                place = place.Where(x => x.EmployementType == type).ToList();
            }

            foreach (var e in place)
            {
                EmployeeByBranch = new EmployeeByBranch();

                var employes = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
                if (employes != null)
                {
                    EmployeeByBranch.FullName = employes.FullNameAmharic;

                    if (employes.Gender == Gender.MALE)
                    {
                        EmployeeByBranch.Sex = "ወንድ";
                    }
                    else
                    {
                        EmployeeByBranch.Sex = "ሴት";
                    }

                    EmployeeByBranch.HireDate = employes.HiredDateAmharic;
                    EmployeeByBranch.Salary = e.Salary;

                    var Education = db.Education.Where(x => x.EmployeeId == employes.EmployeeId).OrderByDescending(x => x.To).FirstOrDefault();
                    if (Education != null)
                    {
                        var lookEduLevel = db.Lookups.FirstOrDefault(x => x.LookupId == Education.EduLevel);
                        if (lookEduLevel != null)
                        {
                            EmployeeByBranch.Education = lookEduLevel.Description;
                        }

                        var lookEduField = db.Lookups.Where(x => x.LookupId == Education.EduField).FirstOrDefault();
                        if (lookEduField != null)
                        {
                            EmployeeByBranch.Training = lookEduField.Description;
                        }
                        var lookJobTitle = db.Lookups.FirstOrDefault(x => x.LookupId == e.LookupId);
                        if (lookJobTitle != null)
                        {
                            EmployeeByBranch.JobTitle = lookJobTitle.Description;
                        }
                    }

                    #region
                    //var tarinings =
                    //    db.EmpTrainingEducations.Where(x => x.EmployeeId == e.EmployeeId)
                    //        .OrderByDescending(x => x.To)
                    //        .FirstOrDefault();
                    //if (tarinings != null)
                    //{
                    //    EmployeeByBranch.Training = tarinings.TrainingName;

                    #endregion

                    if (Education != null)
                    {
                        EmployeeByBranch.Institution = Education.OrganizationName;
                    }

                    EmployeeByBranch.Type = GetEmpStatus(employes.Status);

                    EmployeeByBranchList.Add(EmployeeByBranch);
                }
            }

            return EmployeeByBranchList;

        }

        public EmployeeStatus GetEmpType(string status)
        {
            EmployeeStatus empStatus;
            if (status.ToLower() == EmployeeStatus.CONTRACT.ToString().ToLower())
            {
                empStatus = EmployeeStatus.CONTRACT;
            }
            else if (status.ToLower() == EmployeeStatus.PERMANENT.ToString().ToLower())
            {
                empStatus = EmployeeStatus.PERMANENT;
            }
            else if (status.ToLower() == EmployeeStatus.FREELANCER.ToString().ToLower())
            {
                empStatus = EmployeeStatus.FREELANCER;
            }
            else if (status.ToLower() == EmployeeStatus.PROBATION.ToString().ToLower())
            {
                empStatus = EmployeeStatus.PROBATION;
            }
            else
            {
                empStatus = EmployeeStatus.NO_STATUS;
            }

            return empStatus;
        }
        public string GetEmpStatus(EmployeeStatus status)
        {
            string empStatus = "";
            if (status == EmployeeStatus.CONTRACT)
            {
                empStatus = "ጊዜያዊ";
            }
            else if (status == EmployeeStatus.PERMANENT)
            {
                empStatus = "ቋሚ";
            }
            else if (status == EmployeeStatus.FREELANCER)
            {
                empStatus = "ነጻ ሰራተኛ";
            }
            else if (status == EmployeeStatus.PROBATION)
            {
                empStatus = "የሙከራ ጊዜ";
            }
            else
            {
                empStatus = "አይታወቅም";
            }

            return empStatus;
        }
    }
}