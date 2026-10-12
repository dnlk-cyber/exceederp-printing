using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeBirthDateObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeInfo> GetEmployeeBirthDate(object[] branchId, string bussinessUnitId, string orgStructureId, int agemin, int ageMax, string status, string gender)
        {
            // Pre-load lookup dictionaries to eliminate in-loop database calls
            var branchDict = db.Branch.ToDictionary(b => b.BranchId, b => b.Name);
            var deptDict = db.MainCompanyStructures.ToDictionary(d => d.OrgStructureId, d => d.Name);

            //  Pre-load phone addresses grouped by employee reference string
            var phoneCategories = new[]
            {
                AddressType.HOME_PHONE,
                AddressType.MOBILE_PHONE_1,
                AddressType.MOBILE_PHONE_2,
                AddressType.OFFICE_PHONE
            };

            var addressDict = db.Addresses
                .Where(x => phoneCategories.Contains(x.Category))
                .GroupBy(x => x.Reference)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Value).ToList());

            //  Normalize multi-branch IDs
            HashSet<int> branchIds = new HashSet<int>();
            if (branchId != null && branchId.Length > 0)
            {
                foreach (var b in branchId)
                {
                    if (b != null && int.TryParse(b.ToString(), out int bId))
                    {
                        branchIds.Add(bId);
                    }
                }
            }

            int? orgId = !string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int oId) ? (int?)oId : null;
            int? bUId = !string.IsNullOrEmpty(bussinessUnitId) && int.TryParse(bussinessUnitId, out int buId) ? (int?)buId : null;

            //  Perform optimized Single-Query Join for Employees and Current Placements
            var query = from emp in db.Person_Employee
                        join pl in db.Placements.Where(p => p.isCurrent) on emp.EmployeeId equals pl.EmployeeId into plJoin
                        from pl in plJoin.DefaultIfEmpty()
                        select new { Employee = emp, Placement = pl };

            // Apply Branch Filter
            if (branchIds.Any())
            {
                query = query.Where(x => x.Placement != null && branchIds.Contains(x.Placement.BranchId));
            }

            // Apply Department Filter
            if (orgId.HasValue)
            {
                query = query.Where(x => x.Placement != null && x.Placement.OrgStructureId == orgId.Value);
            }

            // Apply Business Unit Filter
            if (bUId.HasValue)
            {
                query = query.Where(x => x.Placement != null && x.Placement.BusinessUnitId == bUId.Value);
            }

            // Apply Gender Filter
            if (!string.IsNullOrEmpty(gender))
            {
                string lowerGender = gender.ToLower();
                query = query.Where(x => x.Employee.Gender.ToString().ToLower() == lowerGender);
            }

            // Apply Status Filter
            if (!string.IsNullOrEmpty(status))
            {
                if (status.ToLower() == "active")
                {
                    query = query.Where(x => x.Employee.IsActive);
                }
                else if (status.ToLower() == "inactive")
                {
                    query = query.Where(x => !x.Employee.IsActive);
                }
            }

            var rawData = query.ToList();
            var employeeInfoList = new List<EmployeeInfo>();
            DateTime now = DateTime.Now;

            //  Process records in-memory without any further database queries
            foreach (var item in rawData)
            {
                var employee = item.Employee;
                var place = item.Placement;

                decimal diff = (decimal)(now - employee.DateOfBirth).TotalDays / 365.25m;
                int diffAge = (int)diff;

                // Age filtering logic
                if (ageMax > 0 && agemin >= 0 && agemin <= ageMax)
                {
                    if (diffAge < agemin || diffAge > ageMax)
                        continue; // Skip out-of-range ages
                }

                var info = new EmployeeInfo
                {
                    IdNumber = employee.EmployeeCode,
                    BirthDate = employee.DateOfBirthAm,
                    Sex = HrHelper.GetLocalizedGender(employee.Gender),
                    FullName = !string.IsNullOrEmpty(employee.FirstNameEnglish)
                        ? $"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}"
                        : employee.FullNameAmharic,
                    Dob = employee.DateOfBirthAm,
                    CriteriaName = diffAge.ToString()
                };

                // Get phone numbers from pre-loaded dictionary
                if (addressDict.TryGetValue(employee.EmployeeId.ToString(), out var empPhones) && empPhones.Any())
                {
                    info.Phone = string.Join(", ", empPhones);
                }
                else
                {
                    info.Phone = employee.PhoneNo;
                }

                // Map Branch and Department from dictionaries
                if (place != null)
                {
                    if (branchDict.TryGetValue(place.BranchId, out string branchName))
                    {
                        info.Branch = branchName;
                    }
                    if (deptDict.TryGetValue(place.OrgStructureId, out string deptName))
                    {
                        info.Department = deptName;
                    }
                }

                employeeInfoList.Add(info);
            }

            return employeeInfoList;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
    //[DataObject]
    //public class EmployeeBirthDateObjectDataSource : IDisposable
    //{
    //    private ExceedDbContext db = new ExceedDbContext();

    //    [DataObjectMethod(DataObjectMethodType.Select)]
    //    public List<EmployeeInfo> GetEmployeeBirthDate(/*string branchId,*/ object[] branchId,string bussinessUnitId, string orgStructureId, int agemin, int ageMax, string status, string gender)
    //    {
    //        List<EmployeeInfo> EmployeeInfoList = new List<EmployeeInfo>();
    //        string cultureName = GlobalHelper.GetCurrentCulture();
    //        var placements = db.Placements.ToList();
    //        var employeesQuery = db.Person_Employee.AsQueryable();
    //        var phoneCategories = new[]
    //        {
    //                AddressType.HOME_PHONE,
    //                AddressType.MOBILE_PHONE_1,
    //                AddressType.MOBILE_PHONE_2,
    //                AddressType.OFFICE_PHONE

    //        };

    //        var empAdresses = db.Addresses
    //            .Where(x => phoneCategories.Contains(x.Category))
    //            .AsQueryable();


    //        // Handle multi-branch selection safely. If branchId is null, empty, or contains null/empty items, treat it as "All Branches".
    //        bool hasValidBranches = branchId != null && branchId.Length > 0 && branchId.Any(b => b != null && !string.IsNullOrEmpty(b.ToString()));

    //        if (hasValidBranches)
    //        {
    //            HashSet<int> branchIds = new HashSet<int>();
    //            foreach (var b in branchId)
    //            {
    //                if (b != null && int.TryParse(b.ToString(), out int bId))
    //                {
    //                    branchIds.Add(bId);

    //                }
    //            }

    //            if (branchIds.Any())
    //            {
    //                placements = placements.Where(x => branchIds.Contains(x.BranchId)).ToList();
    //                HashSet<int> empIds = new HashSet<int>(placements.Select(x => x.EmployeeId));
    //                employeesQuery = employeesQuery.Where(x => empIds.Contains(x.EmployeeId));
    //            }
    //        }
    //        if (!string.IsNullOrEmpty(orgStructureId))
    //        {
    //            int.TryParse(orgStructureId, out int orgId);
    //            HashSet<int> empIds = new HashSet<int>(placements.Where(x => x.OrgStructureId==orgId).Select(x => x.EmployeeId));
    //            employeesQuery = employeesQuery.Where(x => empIds.Contains(x.EmployeeId));
    //        }
    //        if (!string.IsNullOrEmpty(bussinessUnitId))
    //        {
    //            int.TryParse(bussinessUnitId, out int bUId);
    //            HashSet<int> empIds = new HashSet<int>(placements.Where(x => x.BusinessUnitId == bUId).Select(x => x.EmployeeId));
    //            employeesQuery = employeesQuery.Where(x => empIds.Contains(x.EmployeeId));

    //        }
    //        if (!string.IsNullOrEmpty(gender))
    //        {
    //            employeesQuery = employeesQuery.Where(x => x.Gender.ToString().ToLower()==gender);

    //        }
    //        if (!string.IsNullOrEmpty(status) && status.ToLower() == "active")
    //        {
    //            employeesQuery = employeesQuery.Where(x => x.IsActive);

    //        }
    //        if (!string.IsNullOrEmpty(status) && status.ToLower() == "inactive")
    //        {
    //            employeesQuery = employeesQuery.Where(x => !x.IsActive);
    //        }
    //        if (!string.IsNullOrEmpty(status) && status.ToLower() == "inactive")
    //        {
    //            employeesQuery = employeesQuery.Where(x => !x.IsActive);
    //        }
    //        if (ageMax != 0 && agemin >= 0 && ageMax>agemin)
    //        {
    //            DateTime minDate = DateTime.Now.AddYears(-agemin);
    //            DateTime MaxDate = DateTime.Now.AddYears(-ageMax);
    //            employeesQuery = employeesQuery.Where(x => (DateTime.Now.Year - x.DateOfBirth.Year) >= agemin && (DateTime.Now.Year - x.DateOfBirth.Year)<=ageMax);

    //        }

    //        foreach (var employee in employeesQuery)
    //        {
    //            EmployeeInfo EmployeeInfo = new EmployeeInfo();
    //            var empAdresse = empAdresses
    //                .Where(x => x.Reference == employee.EmployeeId.ToString())
    //                .Select(x => x.Value) // assuming the phone number is in 'Value'
    //                .ToList();
    //            var place = db.Placements.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId&&x.isCurrent);
    //            decimal diff = (decimal)(DateTime.Now - employee.DateOfBirth).TotalDays/365.25m;
    //            int diffage = (int)diff;
    //            //decimal diff = DateTime.Now.Year - employee.DateOfBirth.Year;
    //            if (ageMax > 0 && agemin >= 0 && agemin <= ageMax)
    //            {
    //                if (diffage >= agemin && diffage <= ageMax)
    //                {

    //                    EmployeeInfo.IdNumber = employee.EmployeeCode;
    //                    EmployeeInfo.BirthDate = employee.DateOfBirthAm;
    //                    EmployeeInfo.Sex = HrHelper.GetLocalizedGender(employee.Gender);
    //                    EmployeeInfo.FullName = $"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}";
    //                    EmployeeInfo.Dob = employee.DateOfBirthAm;
    //                    EmployeeInfo.CriteriaName = GetAge(employee.DateOfBirth).ToString();

    //                    EmployeeInfo.Phone = empAdresse.Any()
    //                        ? string.Join(", ", empAdresse)
    //                        : employee.PhoneNo; // fallback to main phone if none found                        EmployeeInfo.CriteriaName = GetAge(employee.DateOfBirth).ToString();
    //                    if (place != null)
    //                    {
    //                        var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
    //                        if (branch != null)
    //                        {
    //                            EmployeeInfo.Branch = branch.Name;
    //                        }
    //                        var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
    //                        if (deptt != null)
    //                        {
    //                            EmployeeInfo.Department = deptt.Name;
    //                        }
    //                    }

    //                    EmployeeInfoList.Add(EmployeeInfo);
    //                }
    //            }
    //            else
    //            {
    //                EmployeeInfo.IdNumber = employee.EmployeeCode;
    //                EmployeeInfo.BirthDate = employee.DateOfBirthAm;
    //                EmployeeInfo.Sex = HrHelper.GetLocalizedGender(employee.Gender);
    //                EmployeeInfo.FullName = employee.FullNameAmharic;
    //                EmployeeInfo.Dob = employee.DateOfBirthAm;
    //                EmployeeInfo.Phone = empAdresse.Any()
    //                    ? string.Join(", ", empAdresse)
    //                    : employee.PhoneNo; // fallback to main phone if none found                    
    //                if (place != null)
    //                {
    //                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
    //                    if (branch != null)
    //                    {
    //                        EmployeeInfo.Branch = branch.Name;
    //                    }
    //                    var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
    //                    if (deptt != null)
    //                    {
    //                        EmployeeInfo.Department = deptt.Name;
    //                    }
    //                }
    //                EmployeeInfo.CriteriaName = GetAge(employee.DateOfBirth).ToString();
    //                EmployeeInfoList.Add(EmployeeInfo);

    //            }

    //        }

    //        return EmployeeInfoList;
    //    }

    //    private static int GetAge(DateTime dateOfBirth)
    //    {
    //        decimal diff = (decimal)(DateTime.Now - dateOfBirth).TotalDays / 365.25m;

    //        // int age = DateTime.Now.Year - dateOfBirth.Year;

    //        //// Adjust for leap years and birthdays not yet occurring in the current year
    //        // if (DateTime.Now < dateOfBirth.AddYears(age))
    //        // {

    //        //     age--;

    //        // }

    //        //return age;
    //        return (int)diff;
    //    }
    //    public void Dispose()
    //    {
    //        db.Dispose();
    //    }
    //}
}