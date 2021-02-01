using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeDisciplinaryMeasureObjectDataSource : List<EmployeeDicipline>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeDicipline> GetEmployeeDisciplinaryMeasureSummary(string branchId, string orgStructureId, DateTime from, DateTime to, string measure)
        {
            List<EmployeeDicipline> list = new List<EmployeeDicipline>();
            EmployeeDicipline employee = new EmployeeDicipline();

            db = new ExceedDbContext();
            var palcement = db.Placements.Where(x => x.isCurrent);
            db = new ExceedDbContext();
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
            if (measure != null)
            {
                var types = db.DisciplineMeasures.Where(x => x.MeasuresId.ToString() == measure).ToList();
                DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
                DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
                if (types.Any())
                {
                    db = new ExceedDbContext();
                    foreach (var type in types)
                    {
                        employee = new EmployeeDicipline();

                        var disipliceMeasure = db.EmpDisciplineMeasure.ToList();
                        disipliceMeasure = disipliceMeasure.Where(x =>
                            x.MeasuresId == type.MeasuresId).ToList();
                        db = new ExceedDbContext();
                        var detail =
                            db.DisciplineMeasuresDetail.Where(x => x.MeasuresId == type.MeasuresId).FirstOrDefault();
                        if (disipliceMeasure.Count() > 0)
                        {

                            db = new ExceedDbContext();
                            foreach (var t in disipliceMeasure)
                            {
                                db = new ExceedDbContext();
                                DateTime decisionDate = new DateTime(t.DecisionDate.Year, t.DecisionDate.Month,
                                    t.DecisionDate.Day);

                                if (decisionDate >= dateOne && decisionDate <= dateTow)
                                {

                                    var diciplineCase =
                                        db.DisciplinaryCases.Where(x => x.DisciplinaryCaseId == t.DisciplinaryCaseId)
                                            .ToList();
                                    if (diciplineCase.Any())
                                    {
                                        foreach (var c in diciplineCase)
                                        {
                                            // employee .MeasureRank =type.
                                            var emplist =
                                                employees.Where(x => x.EmployeeId == c.EmployeeId && x.IsActive).ToList();
                                            if (emplist.Any())
                                            {


                                                foreach (var e in emplist)
                                                {


                                                    employee = new EmployeeDicipline();
                                                    if (detail != null)
                                                    {
                                                        employee.MeasureRank = detail.Level.ToString();
                                                    }
                                                    employee.IdNumber = e.EmployeeCode;
                                                    if (e.Gender == 0)
                                                    {
                                                        employee.Sex = "ወንድ";
                                                    }
                                                    else
                                                    {
                                                        employee.Sex = "ሴት";
                                                    }
                                                    employee.MeasureTakenDate = t.DecisionDate.ToShortDateString();
                                                    employee.MeasureType = type.MeasuresType;
                                                    if (palcement.Count() > 0)
                                                    {
                                                        var place =
                                                            palcement.Where(x => x.EmployeeId == e.EmployeeId && x.isCurrent)
                                                                .FirstOrDefault();
                                                        if (place != null)
                                                        {
                                                            var deptt =
                                                                db.MainCompanyStructures.Where(
                                                                        x => x.OrgStructureId == place.OrgStructureId)
                                                                    .FirstOrDefault();
                                                            employee.Departement = deptt.Name;
                                                        }
                                                    }
                                                    employee.FullName = e.FirstName + " " + e.MiddleName + " " +
                                                                        e.LastName;
                                                    list.Add(employee);
                                                }


                                            }



                                        }
                                    }
                                }
                            }

                        }

                    }

                }
            }
            else
            {
                var types = db.DisciplineMeasures.ToList();
                //  DateTime dateOne = new DateTime(from.Year, from.Month, from.Day);
                //  DateTime dateTow = new DateTime(to.Year, to.Month, to.Day);
                if (types.Any())
                {
                    db = new ExceedDbContext();
                    foreach (var type in types)
                    {
                        employee = new EmployeeDicipline();

                        var disipliceMeasure = db.EmpDisciplineMeasure.ToList();
                        disipliceMeasure = disipliceMeasure.Where(x =>
                            x.MeasuresId == type.MeasuresId).ToList();
                        db = new ExceedDbContext();
                        var detail =
                            db.DisciplineMeasuresDetail.Where(x => x.MeasuresId == type.MeasuresId).FirstOrDefault();
                        if (disipliceMeasure.Count() > 0)
                        {

                            db = new ExceedDbContext();
                            foreach (var t in disipliceMeasure)
                            {
                                db = new ExceedDbContext();
                                DateTime decisionDate = new DateTime(t.DecisionDate.Year, t.DecisionDate.Month,
                                    t.DecisionDate.Day);


                                var diciplineCase =
                                    db.DisciplinaryCases.Where(x => x.DisciplinaryCaseId == t.DisciplinaryCaseId)
                                        .ToList();
                                if (diciplineCase.Any())
                                {
                                    foreach (var c in diciplineCase)
                                    {
                                        // employee .MeasureRank =type.
                                        var emplist =
                                            employees.Where(x => x.EmployeeId == c.EmployeeId && x.IsActive).ToList();
                                        if (emplist.Any())
                                        {


                                            foreach (var e in emplist)
                                            {


                                                employee = new EmployeeDicipline();
                                                if (detail != null)
                                                {
                                                    employee.MeasureRank = detail.Level.ToString();
                                                }
                                                employee.IdNumber = e.EmployeeCode;
                                                if (e.Gender == 0)
                                                {
                                                    employee.Sex = "ወንድ";
                                                }
                                                else
                                                {
                                                    employee.Sex = "ሴት";
                                                }
                                                employee.MeasureTakenDate = t.DecisionDate.ToShortDateString();
                                                employee.MeasureType = type.MeasuresType;
                                                if (palcement.Count() > 0)
                                                {
                                                    var place =
                                                        palcement.Where(x => x.EmployeeId == e.EmployeeId && x.isCurrent)
                                                            .OrderByDescending(x => x.AssignmentDate)
                                                            .FirstOrDefault();
                                                    if (place != null)
                                                    {
                                                        var deptt =
                                                            db.MainCompanyStructures.Where(
                                                                    x => x.OrgStructureId == place.OrgStructureId)
                                                                .FirstOrDefault();
                                                        employee.Departement = deptt.Name;
                                                    }
                                                }
                                                employee.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                                                list.Add(employee);
                                            }


                                        }




                                    }
                                }
                            }

                        }

                    }

                }
            }
            return list;
        }
    }
}