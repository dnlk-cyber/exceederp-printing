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

            Gender? gender = null;
            switch (sex)
            {
                case "MALE":
                    gender = Gender.MALE;
                    break;
                case "FEMALE":
                    gender = Gender.FEMALE;
                    break;

            }

            #endregion
            List<Person_Employee> employees;
            if (branchId != null && orgStructureId != null)
            {
                employees = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId)).ToList();
            }
            else if (branchId != null && orgStructureId == null)
            {
                employees = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
            }
            else if (branchId == null && orgStructureId != null)
            {
                employees = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).ToList();
            }
            else
            {

                employees = db.Person_Employee.Where(x => x.IsActive).ToList();

            }
            if (gender != null && isPesionAveal != null)
            {

                if (isPesionAveal == "ASSIGNED")
                {
                    db = new ExceedDbContext();
                    var identifications = db.Identifications.ToList();
                    db = new ExceedDbContext();
                    var emplist =
                        employees.Where(x => x.Gender == gender && x.IsActive).ToList();

                    if (emplist.Any())
                    {
                        foreach (var e in emplist)
                        {
                            EmployeeInfo = new EmployeeInfo();

                           
                          
                            var value =
                                identifications.Where(
                                    x =>
                                        x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" &&
                                        x.Type == IdentificationType.PENSION_NO).FirstOrDefault();
                            if (value != null)
                            {
                                EmployeeInfo.CriteriaName = value.IdNumber;

                                if (e.Gender == Gender.MALE)
                                {
                                    EmployeeInfo.Sex = "ወንድ";
                                }
                                else
                                {
                                    EmployeeInfo.Sex = "ሴት";
                                }
                                EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                                EmployeeInfo.IdNumber = e.EmployeeCode;
                                EmployeeInfo.Value = (DateTime.Now.Year - e.DateOfBirth.Year).ToString();

                                var tin = identifications.Where(
                                        x =>
                                            x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" &&
                                            x.Type == IdentificationType.TIN).FirstOrDefault();

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
                    db = new ExceedDbContext();
                    var identifications = db.Identifications.ToList();
                    db = new ExceedDbContext();
                    var emplist =
                        employees.Where(
                                x => x.Gender == gender && x.IsActive)
                            .ToList();

                    if (emplist.Any())
                    {
                        foreach (var e in emplist)
                        {
                            EmployeeInfo = new EmployeeInfo();

                            

                            var value =
                                identifications.Where(
                                    x =>
                                        x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" &&
                                        x.Type == IdentificationType.PENSION_NO).FirstOrDefault();
                            if (value == null)
                            {
                              
                                EmployeeInfo.CriteriaName = "--";

                                if (e.Gender == Gender.MALE)
                                {
                                    EmployeeInfo.Sex = "ወንድ";
                                }
                                else
                                {
                                    EmployeeInfo.Sex = "ሴት";
                                }
                                EmployeeInfo.IdNumber = e.EmployeeCode;
                                EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                                EmployeeInfo.Value = (DateTime.Now.Year - e.DateOfBirth.Year).ToString();

                                var tin = identifications.Where(
                                        x =>
                                            x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" &&
                                            x.Type == IdentificationType.TIN).FirstOrDefault();

                                if (tin != null)
                                {
                                    EmployeeInfo.TIN = tin.IdNumber;
                                }

                                EmployeeInfoList.Add(EmployeeInfo);

                            }

                           
                        }

                    }
                }
            }
            else if (isPesionAveal != null && gender == null)
            {
                if (isPesionAveal == "ASSIGNED")
                {
                    db = new ExceedDbContext();
                    var identifications = db.Identifications.ToList();
                    db = new ExceedDbContext();
                    var emplist =
                        employees.Where(
                                x => x.IsActive)
                            .ToList();

                    if (emplist.Any())
                    {
                        foreach (var e in emplist)
                        {
                            

                            var value =
                                identifications.Where(
                                    x =>
                                        x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" &&
                                        x.Type == IdentificationType.PENSION_NO).FirstOrDefault();
                            if (value != null)
                            {
                                EmployeeInfo = new EmployeeInfo();

                                var tin = identifications.Where(
                                   x =>
                                       x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" &&
                                       x.Type == IdentificationType.TIN).FirstOrDefault();

                                if (tin != null)
                                {
                                    EmployeeInfo.TIN = tin.IdNumber;
                                }

                                EmployeeInfo.IdNumber = e.EmployeeCode;

                                if (e.Gender == Gender.MALE)
                                {
                                    EmployeeInfo.Sex = "ወንድ";
                                }
                                else
                                {
                                    EmployeeInfo.Sex = "ሴት";
                                }
                                EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                                EmployeeInfo.CriteriaName = value.IdNumber;
                                EmployeeInfo.Value = (DateTime.Now.Year - e.DateOfBirth.Year).ToString();
                                EmployeeInfoList.Add(EmployeeInfo);
                            }
                        }

                    }
                }
                else
                {
                    db = new ExceedDbContext();
                    var identifications = db.Identifications.ToList();
                    db = new ExceedDbContext();
                    var emplist = employees.Where(x => x.IsActive).ToList();

                    if (emplist.Any())
                    {
                        foreach (var e in emplist)
                        {
                            var value =
                                identifications.Where(
                                    x =>
                                        x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" &&
                                        x.Type == IdentificationType.PENSION_NO).FirstOrDefault();
                            if (value == null)
                            {
                                EmployeeInfo = new EmployeeInfo();
                                if (e.Gender == Gender.MALE)
                                {
                                    EmployeeInfo.Sex = "ወንድ";
                                }
                                else
                                {
                                    EmployeeInfo.Sex = "ሴት";
                                }
                                EmployeeInfo.IdNumber = e.EmployeeCode;
                                EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                                EmployeeInfo.CriteriaName = "--";
                                EmployeeInfo.Value = (DateTime.Now.Year - e.DateOfBirth.Year).ToString();
                                EmployeeInfoList.Add(EmployeeInfo);
                            }
                        }

                    }
                }
            }
            else
            {
                db = new ExceedDbContext();
                var identifications = db.Identifications.ToList();
                db = new ExceedDbContext();
                var emplist =
                    employees.Where(x => x.IsActive).ToList();

                if (emplist.Any())
                {
                    foreach (var e in emplist)
                    {
                        EmployeeInfo = new EmployeeInfo();

                        EmployeeInfo.IdNumber = e.EmployeeCode;
                        if (e.Gender == Gender.MALE)
                        {
                            EmployeeInfo.Sex = "ወንድ";
                        }
                        else
                        {
                            EmployeeInfo.Sex = "ሴት";
                        }
                        EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                      
                        EmployeeInfo.Value = (DateTime.Now.Year - e.DateOfBirth.Year).ToString();


                        var tin = identifications.Where(
                                   x =>
                                       x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" &&
                                       x.Type == IdentificationType.TIN).FirstOrDefault();

                        if (tin != null)
                        {
                            EmployeeInfo.TIN = tin.IdNumber;
                        }
                        var value =
                            identifications.Where(
                                x =>
                                    x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" &&
                                    x.Type == IdentificationType.PENSION_NO).FirstOrDefault();
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
            }
            return EmployeeInfoList.OrderBy(x => x.IdNumber).ToList();
        }
    }
}