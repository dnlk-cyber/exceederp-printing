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
    public class DisciplinaryMeasureObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeSummary> GetDisciplinaryMeasureSummary(DateTime from, DateTime to, string branchId, string orgStructureId, string status, string measure, bool nonZero)
        {
            List<EmployeeSummary> list = new List<EmployeeSummary>();
            EmployeeSummary employee = new EmployeeSummary();

            var types = db.DisciplineMeasures.ToList();

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

            if (status == "Active")
            {
                placements = EmployeeHelper.Get_Active_EmployeesFrom_Placement(placements);
            }
            if (status == "InActive")
            {
                placements = EmployeeHelper.Get_InActive_EmployeesFrom_Placement(placements);
            }

            var males = EmployeeHelper.Get_Male_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId);
            var females = EmployeeHelper.Get_Female_EmployeesFrom_Placement(placements).Select(x => x.EmployeeId);

            foreach (var type in types)
            {
                employee = new EmployeeSummary();

                var disipliceMeasures = db.EmpDisciplineMeasure.Where(x => x.MeasuresId == type.MeasuresId && !x.IsExpired).ToList();
                if (from <= to)
                {
                    disipliceMeasures = disipliceMeasures.Where(x => x.DecisionDate.Date >= from.Date && x.DecisionDate.Date <= to.Date).ToList();
                }

                employee.Criteria = type.MeasuresType;

                var disciplinCaseIds = disipliceMeasures.Select(x => x.DisciplinaryCaseId).ToList();

                var disciplineCases = db.DisciplinaryCases.Where(x => disciplinCaseIds.Contains(x.DisciplinaryCaseId)).ToList();

                employee.NumberOfMaleEmployee = disciplineCases.Where(x => males.Contains(x.EmployeeId)).Count();
                employee.NumberOfFemaleEmployee = disciplineCases.Where(x => females.Contains(x.EmployeeId)).Count();
                employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;
               

                if (nonZero)
                {
                    if (employee.TotalEmployee > 0)
                    {
                        list.Add(employee);
                    }
                }
                else
                {
                    list.Add(employee);
                }

            }

            return list;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}