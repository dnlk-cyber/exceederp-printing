using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;


namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeBirthDateObjectDataSource : List<EmployeeInfo>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeInfo> GetEmployeeBirthDate(string branchId, string orgStructureId, int agemin, int ageMax)
        {
            List<EmployeeInfo> EmployeeInfoList = new List<EmployeeInfo>();
            EmployeeInfo EmployeeInfo = new EmployeeInfo();
           

            var palcement = db.Placements.Where(x => x.isCurrent);
            List<Person_Employee> emplist;
            db = new ExceedDbContext();
       
            emplist = HrHelper.PersonEmployeesOneBranch(branchId, orgStructureId, "");

            if (emplist.Any())
            {
                foreach (var e in emplist)
                {
                    int diff = (DateTime.Now.Year - e.DateOfBirth.Year);
                 
                    if (ageMax !=0&& agemin !=0)
                    {
                        if (diff >= agemin && diff <= ageMax)
                        {
                            EmployeeInfo = new EmployeeInfo();
                            EmployeeInfo.IdNumber = e.EmployeeCode;
                            EmployeeInfo.Sex = HrHelper.GetGender(e.Gender);

                            EmployeeInfo.FullName = HrHelper.GetEmployeeFullName(e.EmployeeId);
                            ;
                            EmployeeInfo.Dob = e.DateOfBirthAm;
                            if (palcement.Any())
                            {
                                var place =
                                    palcement
                                        .FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.isCurrent);
                                if (place != null)
                                {
                                    var deptt =
                                        db.MainCompanyStructures
                                            .FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                                    if (deptt != null)
                                    {
                                        EmployeeInfo.Department = deptt.Name;
                                    }
                                }
                            }
                            EmployeeInfo.CriteriaName = (DateTime.Now.Year - e.DateOfBirth.Year).ToString();
                          


                            EmployeeInfoList.Add(EmployeeInfo);
                        }
                    }
                    else
                    {
                        EmployeeInfo = new EmployeeInfo();
                        EmployeeInfo.IdNumber = e.EmployeeCode;
                        EmployeeInfo.Sex = HrHelper.GetGender(e.Gender);

                        EmployeeInfo.FullName = HrHelper.GetEmployeeFullName(e.EmployeeId); ;
                        EmployeeInfo.Dob = e.DateOfBirthAm;
                        if (palcement.Any())
                        {
                            var place =
                                palcement
                                    .FirstOrDefault(x => x.EmployeeId == e.EmployeeId && x.isCurrent);
                            if (place != null)
                            {
                                var deptt =
                                    db.MainCompanyStructures
                                        .FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                                if (deptt != null)
                                {
                                    EmployeeInfo.Department = deptt.Name;
                                }
                            }
                        }
                        EmployeeInfo.CriteriaName = (DateTime.Now.Year - e.DateOfBirth.Year).ToString();
                       


                        EmployeeInfoList.Add(EmployeeInfo);
                    }
                }



            }
            return EmployeeInfoList;
        }
    }
}