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
        public List<EmployeeDicipline> GetEmployeeDisciplinaryMeasureSummary(string branchId, string orgStructureId, DateTime from, DateTime to, string measure, bool isActive)
        {
            List<EmployeeDicipline> list = new List<EmployeeDicipline>();
            EmployeeDicipline employee = new EmployeeDicipline();

            var placements = db.Placements.Where(x => x.isCurrent).ToList();

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

            var types = db.DisciplineMeasures.ToList();

            if (measure != null)
            {
                int.TryParse(measure, out int measureId);
                types = db.DisciplineMeasures.Where(x => x.MeasuresId == measureId).ToList();
            }

            var empIds = placements.Select(x => x.EmployeeId).ToList();

            foreach (var type in types)
            {
                employee = new EmployeeDicipline();

                db = new ExceedDbContext();

                var disipliceMeasures = db.EmpDisciplineMeasure.Where(x => x.MeasuresId == type.MeasuresId).Select(x => x.DisciplinaryCaseId).ToList();

                //foreach (var t in disipliceMeasure)
                //{
                // DateTime decisionDate = new DateTime(t.DecisionDate.Year, t.DecisionDate.Month, t.DecisionDate.Day);

                var disciplineCases = db.DisciplinaryCases.Where(x => disipliceMeasures.Contains(x.DisciplinaryCaseId)).ToList();

                if(isActive)
                {
                    disciplineCases = disciplineCases.Where(x => x.IsActive).ToList();
                }

                foreach (var c in disciplineCases)
                {
                    employee = new EmployeeDicipline();

                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == c.EmployeeId);

                    if (person != null)
                    {
                        var detail = db.DisciplineMeasuresDetail.Where(x => x.MeasuresId == type.MeasuresId).FirstOrDefault();

                        if (detail != null)
                        {
                            employee.MeasureRank = detail.Level.ToString();
                        }

                        employee.IdNumber = person.EmployeeCode;

                        if (person.Gender == 0)
                        {
                            employee.Sex = "ወንድ";
                        }
                        else
                        {
                            employee.Sex = "ሴት";
                        }

                        var disByMoney = db.DisciplineMeasuerByMoney.FirstOrDefault(x=>x.DisciplinaryCaseId == c.DisciplinaryCaseId);
                        if(disByMoney != null)
                        {
                            employee.PunishmentAmount = disByMoney.Amount.ToString();
                        }
                        else
                        {
                            employee.PunishmentAmount = "በገንዘብ አልተቀጡም";
                        }

                        var empDisc = db.EmpDisciplineMeasure.FirstOrDefault(x => x.DisciplinaryCaseId == c.DisciplinaryCaseId);
                        if (empDisc != null)
                        {
                            employee.MeasureTakenDate = empDisc.DecisionDateAm;
                        }

                        employee.MeasureType = type.MeasuresType;

                        var place = db.Placements.FirstOrDefault(x => x.EmployeeId == person.EmployeeId && x.isCurrent);
                        if (place != null)
                        {
                            var department = db.MainCompanyStructures.Where(x => x.OrgStructureId == place.OrgStructureId).FirstOrDefault();

                            if (department != null)
                            {
                                employee.Departement = department.Name;
                            }
                        }
                        else
                        {
                            var pl = db.Placements.Where(x => x.EmployeeId == person.EmployeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
                            if (pl != null)
                            {
                                var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == pl.OrgStructureId);

                                if (department != null)
                                {
                                    employee.Departement = department.Name;
                                }
                            }
                        }

                        employee.FullName = person.FullNameAmharic;
                    }

                    list.Add(employee);
                    // }
                }
            }

            return list;
        }
    }
}