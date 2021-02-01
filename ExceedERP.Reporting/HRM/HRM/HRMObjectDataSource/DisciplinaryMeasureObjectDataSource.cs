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
    public class DisciplinaryMeasureObjectDataSource : List<EmployeeSummary>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetDisciplinaryMeasureSummary(DateTime from, DateTime to, string branchId, string orgStructureId, string status)
        {
            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();
            db = new ExceedDbContext();
            var types = db.DisciplineMeasures.ToList();
            List<Person_Employee> employees;
            if (branchId != null && orgStructureId != null)
            {
                employees = EmployeeHelper.GetAllEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(orgStructureId)).ToList();
            }
            else if (branchId != null && orgStructureId == null)
            {
                employees = EmployeeHelper.GetAllEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
            }
            else if (branchId == null && orgStructureId != null)
            {
                employees = EmployeeHelper.GetAllEmployeesByWorkUnit(Convert.ToInt32(orgStructureId)).ToList();
            }
            else
            {
                employees = db.Person_Employee.ToList();
            }
            if (status == "Active")
            {
                employees = employees.Where(x => x.IsActive).ToList();
            }
            if (status == "InActive")
            {
                employees = employees.Where(x => !x.IsActive).ToList();
            }
            if (types.Any())
            {
                db = new ExceedDbContext();
                foreach (var type in types)
                {
                    employee = new EmployeeSummary();
                    db = new ExceedDbContext();
                    var disipliceMeasure = db.EmpDisciplineMeasure.ToList();
                    disipliceMeasure = disipliceMeasure.Where(
                        x =>
                            x.DecisionDate.Date >= from.Date && x.DecisionDate.Date <= to.Date &&
                            x.MeasuresId == type.MeasuresId).ToList();
                    employee.Criteria = type.MeasuresType;
                    if (disipliceMeasure.Count() > 0)
                    {
                        db = new ExceedDbContext();
                        foreach (var t in disipliceMeasure)
                        {
                            db = new ExceedDbContext();
                            var diciplineCase =
                                db.DisciplinaryCases.Where(x => x.DisciplinaryCaseId == t.DisciplinaryCaseId).ToList();
                            if (diciplineCase.Any())
                            {
                                foreach (var c in diciplineCase)
                                {


                                    var empList = employees.Where(x => x.EmployeeId == c.EmployeeId).ToList();
                                    foreach (var e in empList)
                                    {

                                        if (e.Gender == Gender.FEMALE)
                                        {
                                            employee.NumberOfFemaleEmployee += 1;

                                        }
                                        if (e.Gender == Gender.MALE)
                                        {
                                            employee.NumberOfMaleEmployee += 1;

                                        }



                                    }
                                }
                            }
                            employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
                        }
                    }
                    list.Add(employee);
                }

            }
            return list;
        }
    }
}