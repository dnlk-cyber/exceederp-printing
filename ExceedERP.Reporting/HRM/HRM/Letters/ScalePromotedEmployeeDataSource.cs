using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{


    [DataObject]
    public class ScalePromotedEmployeeDataSource : IDisposable
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeViewModel> GetEmployees()
        {
            var employeeIds = db.Placements
                .Where(p =>  p.AssignmentType == AssignmentType.SCALE_PROMOTION)
                .OrderByDescending(x=>x.PlacementId)
                .Select(p => p.EmployeeId)
                .Distinct();

            var employees = db.Person_Employee
                .Where(e => employeeIds.Contains(e.EmployeeId)).ToList()
                .Select(e => new EmployeeViewModel
                {
                    FullName = string.Join(" ", e.EmployeeCode, e.FirstNameEnglish, e.MiddleNameEnglish, e.LastNameEnglish),
                    FullAmharicName = string.Join(" ",e.EmployeeCode, e.FirstName, e.MiddleName, e.LastName),
                    EmpId = e.EmployeeId,
                    //EmpCodeWithName = $"{e.EmployeeCode}-{e.FirstName} {e.MiddleName} {e.LastName}",
                    //EmpCodeWithNameEng = $"{e.EmployeeCode}-{e.FirstNameEnglish} {e.MiddleNameEnglish} {e.LastNameEnglish}"
                });

            return employees.ToList();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeCodeViewModel> GetEmployeeForScale()
        {
            return db.Person_Employee
                .Select(e => new EmployeeCodeViewModel
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeCode = e.EmployeeCode,
                    EmployeeName = e.EmployeeCode + "-" + e.FirstName + " " + e.MiddleName + " " + e.LastName
                })
                .ToList();
        }
        public void Dispose()
        {
            db.Dispose();
        }
        
    }




}
