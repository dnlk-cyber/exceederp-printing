using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeePensionObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeInfo> GetEmployeePension(string sex, string isPensionAvail, string branchId, string orgStructureId)
        {

            if (string.IsNullOrEmpty(branchId))
                return new List<EmployeeInfo>();

            // Convert gender string to enum
            Gender? genderFilter = null;
            if (!string.IsNullOrEmpty(sex))
            {
                switch (sex.ToUpper())
                {
                    case "MALE": genderFilter = Gender.MALE; break;
                    case "FEMALE": genderFilter = Gender.FEMALE; break;
                }
            }

            // Base placements query
            var placementsQuery = db.Placements.AsQueryable();

            // Filter by branch
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
                placementsQuery = placementsQuery.Where(x => x.BranchId == braId);

            // Filter by org structure
            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                var structureIds = db.MainCompanyStructures
                                     .Where(x => x.Parent == orgId)
                                     .Select(x => x.OrgStructureId)
                                     .ToList();
                placementsQuery = placementsQuery.Where(x => structureIds.Contains(x.OrgStructureId));
            }

            placementsQuery = placementsQuery.Where(x => x.isCurrent);

            // Filter by gender
            if (genderFilter.HasValue)
            {
                var empIds = db.Person_Employee
                               .Where(x => x.Gender == genderFilter.Value)
                               .Select(x => x.EmployeeId)
                               .ToList();
                placementsQuery = placementsQuery.Where(x => empIds.Contains(x.EmployeeId));
            }

            // Get placements list
            var placements = placementsQuery
                             .OrderByDescending(x => x.AssignmentDate)
                             .ToList();

            // Prepare employee IDs as strings for consistent comparisons
            var empIdsStr = placements.Select(p => p.EmployeeId.ToString()).ToList();

            // Bulk fetch employees
            var employees = db.Person_Employee
                              .Where(x => empIdsStr.Contains(x.EmployeeId.ToString()))
                              .ToList();
            var employeesDict = employees.ToDictionary(x => x.EmployeeId);

            // Bulk fetch pensions
            var pensions = db.Identifications
                             .Where(x => empIdsStr.Contains(x.Reference) &&
                                         x.ReferenceType == "Employee" &&
                                         x.Type == IdentificationType.PENSION_NO)
                             .ToList();
            var pensionsDict = pensions.ToDictionary(x => x.Reference, x => x.IdNumber);

            // Bulk fetch TIN
            var tins = db.Identifications
                         .Where(x => empIdsStr.Contains(x.Reference) &&
                                     x.ReferenceType == "Employee" &&
                                     x.Type == IdentificationType.TIN)
                         .ToList();
            var tinsDict = tins.ToDictionary(x => x.Reference, x => x.IdNumber);

            // Bulk fetch phones
            var phoneCategories = new[]
            {
                AddressType.HOME_PHONE,
                AddressType.MOBILE_PHONE_1,
                AddressType.MOBILE_PHONE_2,
                AddressType.OFFICE_PHONE
            };

            var phones = db.Addresses
                           .Where(x => empIdsStr.Contains(x.Reference) && phoneCategories.Contains(x.Category)).ToList();
            var phonesDict = phones
                             .GroupBy(x => x.Reference)
                             .ToDictionary(g => g.Key, g => g.Select(x => x.Value).ToList());

            // Build EmployeeInfo list
            var employeeInfoList = new List<EmployeeInfo>();

            foreach (var e in placements)
            {
                if (!employeesDict.TryGetValue(e.EmployeeId, out var employee))
                    continue;

                var empIdStr = e.EmployeeId.ToString();
                var info = new EmployeeInfo
                {
                    IdNumber = employee.EmployeeCode,
                    FullName = employee.FullNameAmharic,
                    Sex = HrHelper.GetGender(employee.Gender),
                    Value = (DateTime.Now.Year - employee.DateOfBirth.Year).ToString(),
                    Phone = phonesDict.ContainsKey(empIdStr) ? string.Join(", ", phonesDict[empIdStr].Take(4)) : employee.PhoneNo,
                    CriteriaName = pensionsDict.ContainsKey(empIdStr)
                                    ? pensionsDict[empIdStr]
                                    : (isPensionAvail == "ASSIGNED" ? null : "--"),
                    TIN = tinsDict.ContainsKey(empIdStr) ? tinsDict[empIdStr] : null
                };

                // Skip if filtering assigned pensions only
                if (isPensionAvail == "ASSIGNED" && string.IsNullOrEmpty(info.CriteriaName))
                    continue;

                employeeInfoList.Add(info);
            }

            return employeeInfoList.OrderBy(x => x.IdNumber).ToList();
        }      
        
        
        //public List<EmployeeInfo> GetEmployeePension(string sex, string isPesionAveal, string branchId, string orgStructureId)
        //{
        //    List<EmployeeInfo> EmployeeInfoList = new List<EmployeeInfo>();
        //    EmployeeInfo EmployeeInfo = new EmployeeInfo();

        //    #region set Gender

        //    string gender = "";
        //    switch (sex)
        //    {
        //        case "MALE":
        //            gender = Gender.MALE.ToString();
        //            break;
        //        case "FEMALE":
        //            gender = Gender.FEMALE.ToString();
        //            break;
        //    }

        //    #endregion

        //    var placements = db.Placements.ToList();

        //    if (branchId != null)
        //    {
        //        int.TryParse(branchId, out int braId);
        //        placements = placements.Where(x => x.BranchId == braId).ToList();
        //    }
        //    if (orgStructureId != null)
        //    {
        //        int.TryParse(orgStructureId, out int orgId);
        //        var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
        //        placements = placements.Where(x => structureIds.Contains(x.OrgStructureId)).ToList();
        //    }
        //    if (gender != null)
        //    {
        //        if (gender.ToLower() == "male")
        //        {
        //            var empIds = db.Person_Employee.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
        //            placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
        //        }
        //        else if (gender.ToLower() == "female")
        //        {
        //            var empIds = db.Person_Employee.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();
        //            placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
        //        }
        //    }

        //    placements = placements.Where(x => x.isCurrent).OrderByDescending(x => x.AssignmentDate).ToList();
        //    var phoneCategories = new[]
        //   {
        //            AddressType.HOME_PHONE,
        //            AddressType.MOBILE_PHONE_1,
        //            AddressType.MOBILE_PHONE_2,
        //            AddressType.OFFICE_PHONE

        //    };

        //    var empAdresses = db.Addresses
        //        .Where(x => phoneCategories.Contains(x.Category))
        //        .AsQueryable();

        //    if (isPesionAveal != null)
        //    {
        //        if (isPesionAveal != null && isPesionAveal == "ASSIGNED")
        //        {
        //            foreach (var e in placements)
        //            {
        //                EmployeeInfo = new EmployeeInfo();

        //                var value = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.PENSION_NO);
        //                var empAdresse = empAdresses
        //                       .Where(x => x.Reference == e.EmployeeId.ToString())
        //                       .Select(x => x.Value) // assuming the phone number is in 'Value'
        //                       .ToList();
        //                if (value != null)
        //                {
        //                    EmployeeInfo.CriteriaName = value.IdNumber;                            

        //                    var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);

        //                    if (employee != null)
        //                    {
        //                        EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
        //                        EmployeeInfo.IdNumber = employee.EmployeeCode;
        //                        EmployeeInfo.FullName = employee.FullNameAmharic;
        //                        EmployeeInfo.CriteriaName = value.IdNumber;
        //                        EmployeeInfo.Phone = empAdresse.Any()
        //                                                   ? string.Join(", ", empAdresse)
        //                                                   : employee.PhoneNo; // fallback to main phone if none found                                     EmployeeInfo.Value = (DateTime.Now.Year - employee.DateOfBirth.Year).ToString();
        //                        //EmployeeInfo.PensionDate = CalculatePensionDate(60, employee.DateOfBirth);
        //                    }

        //                    var tin = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.TIN);

        //                    if (tin != null)
        //                    {
        //                        EmployeeInfo.TIN = tin.IdNumber;
        //                    }

        //                    EmployeeInfoList.Add(EmployeeInfo);
        //                }

        //            }
        //        }
        //        else
        //        {

        //            foreach (var e in placements)
        //            {
        //                EmployeeInfo = new EmployeeInfo();

        //                var value = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.PENSION_NO);

        //                if (value == null)
        //                {
        //                    EmployeeInfo.CriteriaName = "--";



        //                    var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
        //                    if (employee != null)
        //                    {
        //                        EmployeeInfo.Sex = EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);

        //                        EmployeeInfo.IdNumber = employee.EmployeeCode;
        //                        EmployeeInfo.FullName = employee.FullNameAmharic;
        //                       // EmployeeInfo.CriteriaName = value.IdNumber;
        //                        EmployeeInfo.Value = (DateTime.Now.Year - employee.DateOfBirth.Year).ToString();
        //                    }

        //                    var tin = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.TIN);

        //                    if (tin != null)
        //                    {
        //                        EmployeeInfo.TIN = tin.IdNumber;
        //                    }

        //                    EmployeeInfoList.Add(EmployeeInfo);

        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        foreach (var e in placements)
        //        {
        //            EmployeeInfo = new EmployeeInfo();

        //            EmployeeInfo.IdNumber = e.EmployeeCode;

        //            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
        //            if (employee != null)
        //            {
        //                EmployeeInfo.Sex = EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
        //                EmployeeInfo.IdNumber = employee.EmployeeCode;
        //                EmployeeInfo.FullName = employee.FullNameAmharic;
        //                EmployeeInfo.Value = (DateTime.Now.Year - employee.DateOfBirth.Year).ToString();

        //            }

        //            var tin = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.TIN);

        //            if (tin != null)
        //            {
        //                EmployeeInfo.TIN = tin.IdNumber;
        //            }
        //            var value = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.PENSION_NO);

        //            if (value != null)
        //            {
        //                EmployeeInfo.CriteriaName = value.IdNumber;
        //            }
        //            else
        //            {
        //                EmployeeInfo.CriteriaName = "--";
        //            }

        //            EmployeeInfoList.Add(EmployeeInfo);
        //        }
        //    }

        //    return EmployeeInfoList.OrderBy(x => x.IdNumber).ToList();
        //}
        public DateTime CalculatePensionDate(int retirementAge,DateTime DateOfBirth)
        {
            // Calculate the current age of the employee
            int currentAge = DateTime.Now.Year - DateOfBirth.Year;

            // Check if the birthday has occurred this year
            if (DateTime.Now < DateOfBirth.AddYears(currentAge))
            {
                currentAge--;
            }

            // Calculate the pension date by adding the remaining years to retirement age
            int remainingYears = retirementAge - currentAge;

            // Calculate the pension date based on years of service and retirement age
           DateTime pensionDate = DateTime.Now.AddYears(remainingYears);

            return pensionDate;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}