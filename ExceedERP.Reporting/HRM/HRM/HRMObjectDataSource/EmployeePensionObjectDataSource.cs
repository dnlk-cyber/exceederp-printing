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
    public class EmployeePensionObjectDataSource : List<EmployeeInfo>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeInfo> GetEmployeePension(string sex, string isPesionAveal, string branchId, string orgStructureId)
        {
            List<EmployeeInfo> EmployeeInfoList = new List<EmployeeInfo>();
            EmployeeInfo EmployeeInfo = new EmployeeInfo();

            #region set Gender

            string gender = "";
            switch (sex)
            {
                case "MALE":
                    gender = "ወንድ";
                    break;
                case "FEMALE":
                    gender = "ሴት";
                    break;
            }

            #endregion

            var placements = db.Placements.ToList();

            if (branchId != null)
            {
                int.TryParse(branchId, out int braId);
                placements = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId);
            }
            if (orgStructureId != null)
            {
                int.TryParse(orgStructureId, out int orgId);
                placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId);
            }
            if (gender != null)
            {
                placements = EmployeeHelper.Get_Employees_ByGender_From_Placement(placements, gender);
            }

            placements = placements.Where(x => x.isCurrent).OrderByDescending(x => x.AssignmentDate).ToList();

            if (isPesionAveal != null)
            {
                if (isPesionAveal != null && isPesionAveal == "ASSIGNED")
                {
                    foreach (var e in placements)
                    {
                        EmployeeInfo = new EmployeeInfo();

                        var value = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.PENSION_NO);

                        if (value != null)
                        {
                            EmployeeInfo.CriteriaName = value.IdNumber;

                            EmployeeInfo.Sex = gender;

                            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);

                            if (employee != null)
                            {
                                EmployeeInfo.IdNumber = employee.EmployeeCode;
                                EmployeeInfo.FullName = employee.FullNameAmharic;
                                EmployeeInfo.CriteriaName = value.IdNumber;
                                EmployeeInfo.Value = (DateTime.Now.Year - employee.DateOfBirth.Year).ToString();
                            }

                            var tin = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.TIN);

                            if (tin != null)
                            {
                                EmployeeInfo.TIN = tin.IdNumber;
                            }

                            EmployeeInfoList.Add(EmployeeInfo);
                        }

                    }
                }
                else
                {

                    foreach (var e in placements)
                    {
                        EmployeeInfo = new EmployeeInfo();

                        var value = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.PENSION_NO);

                        if (value == null)
                        {
                            EmployeeInfo.CriteriaName = "--";

                            EmployeeInfo.Sex = gender;

                            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
                            if (employee != null)
                            {
                                EmployeeInfo.IdNumber = employee.EmployeeCode;
                                EmployeeInfo.FullName = employee.FullNameAmharic;
                               // EmployeeInfo.CriteriaName = value.IdNumber;
                                EmployeeInfo.Value = (DateTime.Now.Year - employee.DateOfBirth.Year).ToString();
                            }

                            var tin = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.TIN);

                            if (tin != null)
                            {
                                EmployeeInfo.TIN = tin.IdNumber;
                            }

                            EmployeeInfoList.Add(EmployeeInfo);

                        }
                    }
                }
            }
            else
            {
                foreach (var e in placements)
                {
                    EmployeeInfo = new EmployeeInfo();

                    EmployeeInfo.IdNumber = e.EmployeeCode;

                    EmployeeInfo.Sex = gender;

                    var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
                    if (employee != null)
                    {
                        EmployeeInfo.IdNumber = employee.EmployeeCode;
                        EmployeeInfo.FullName = employee.FullNameAmharic;
                        EmployeeInfo.Value = (DateTime.Now.Year - employee.DateOfBirth.Year).ToString();
                    }

                    var tin = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.TIN);

                    if (tin != null)
                    {
                        EmployeeInfo.TIN = tin.IdNumber;
                    }
                    var value = db.Identifications.FirstOrDefault(x => x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" && x.Type == IdentificationType.PENSION_NO);

                    if (value != null)
                    {
                        EmployeeInfo.CriteriaName = value.IdNumber;
                    }
                    else
                    {
                        EmployeeInfo.CriteriaName = "--";
                    }

                    EmployeeInfoList.Add(EmployeeInfo);
                }
            }

            return EmployeeInfoList.OrderBy(x => x.IdNumber).ToList();
        }
    }
}