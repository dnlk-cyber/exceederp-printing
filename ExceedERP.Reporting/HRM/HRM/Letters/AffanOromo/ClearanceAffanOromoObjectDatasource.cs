using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.Letters.ViewModel;
using System;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.Letters.DataSource.AffanOromo
{
    [DataObject]
    public class ClearanceAffanOromoObjectDatasource : IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public CLearanceVM GetCLearance(string empId)
        {
            CLearanceVM EmployeeInfo = new CLearanceVM();
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(empId, out int employeeId);
                var clearance = _db.ClearanceRequests.Where(x => x.EmployeeId == employeeId).OrderByDescending(x => x.ClearanceRequestId).FirstOrDefault();
                var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);
                if (employee != null)
                {
                    EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
                    EmployeeInfo.HiredDate =  $"{employee.HiredDate.ToString("dd.MM.yyyy")}({employee.HiredDateAmharic})";
                    EmployeeInfo.BirthDate = $"{employee.DateOfBirth:dd.MM.yyyy} ({employee.DateOfBirthAm})";
                    EmployeeInfo.EmployeeCode =  employee.EmployeeCode;
                    EmployeeInfo.FullName =  employee.FullNameaEnglish;
                    var educations = _db.Education.Where(x => x.EmployeeId == employeeId).ToList();
                    if (educations.Any())
                    {
                        var levelIds = educations.Select(x => x.EduLevel);
                        var fieldIds = educations.Select(x => x.EduField);
                        foreach(var level in levelIds)
                        {
                            var lookup = _db.Lookups.Find(level);
                            if(lookup !=null)
                            {
                                EmployeeInfo.EducationLevel += lookup.Description + ", ";
                            }
                        }
                        foreach (var field in fieldIds)
                        {
                            var lookup = _db.Lookups.Find(field);
                            if(lookup !=null)
                            {
                                EmployeeInfo.EducationField += lookup.Description + ", ";
                            }
                        }
                    }
                    var placement = _db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId);
                    if (placement != null)
                    {
                        var businessUnit = _db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == placement.BusinessUnitId);
                        if (businessUnit != null)
                        {
                            EmployeeInfo.BusinessUnit = businessUnit.Name;
                        }
                        var rank = _db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
                        if (rank != null)
                        {
                            EmployeeInfo.Rank = rank.RankName;
                        }
                        var lookup = _db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
                        if (lookup != null)
                        {
                            EmployeeInfo.JobTitle = lookup.Description;
                        }
                        EmployeeInfo.Salary = placement.Salary;
                        var department = _db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                        if (department != null)
                        {
                            EmployeeInfo.WorkUnit = department.Name;
                        }
                        var branch = _db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                        if (branch != null)
                        {
                            EmployeeInfo.Branch = branch.Name;
                        }
                        EmployeeInfo.AssignmentDate = placement.AssignmentDate.ToString("dd.MM.yyyy");
                        EmployeeInfo.AssignmentDateAmh = placement.AssignmentDateAmharic;
                        EmployeeInfo.LastSalaryDate = placement.SalarChangeDate.HasValue?
                                                        EmployeeInfo.LastSalaryDate = $"{placement.SalarChangeDate.Value:dd.MM.yyyy} ({placement.SalarChangeDateAmharic})":"NA";
                        EmployeeInfo.Salary = placement.Salary;
                        EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords((double)EmployeeInfo.Salary);

                    }
                    var volume = _db.PersonalArchiveNo
                        .Where(x => x.EmployeeId == employeeId)
                        .OrderByDescending(x=>x.PersonalArchiveNoId).FirstOrDefault();
                    if(volume != null)
                    {
                        EmployeeInfo.FileVolume = volume.Volume;
                    }
                    var annualLeave = _db.LeavesEntitle
                        .Where(x => x.EmployeeId == employeeId
                        && !x.IsExpired);
                    if (annualLeave.Any())
                    {
                        EmployeeInfo.AnnualLeave = annualLeave.Sum(x => x.Balance).ToString();
                    }

                    var disciplines = _db.DisciplinaryCases.Where(x => x.EmployeeId == employeeId).ToList();
                    foreach(var discipline in disciplines)
                    {
                        EmployeeInfo.Discipline += discipline.NatureofTransgression+",";
                    }
                    if(clearance != null)
                    {
                        EmployeeInfo.ClearanceRequestDate = clearance.DateCreated.HasValue? $"{clearance.DateCreated.Value.ToString("dd.MM.yyyy")}":null;
                        EmployeeInfo.ResonForClearance = clearance.Remark;
                    }
                    //var pension =_db.Pens.FirstOrDefault(x=>x.em)
                }
            }

            return EmployeeInfo;
           
        }
        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
