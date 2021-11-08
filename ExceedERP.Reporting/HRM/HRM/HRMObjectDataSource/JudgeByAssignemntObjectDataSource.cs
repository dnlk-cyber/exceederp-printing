using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class JudgeByAssignemntObjectDataSource : List<CourtAllEmployeeList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CourtAllEmployeeList> GetJudgeByAssignemnt()
        {
            List<CourtAllEmployeeList> JudgeByAgeList = new List<CourtAllEmployeeList>();
            CourtAllEmployeeList JudgeByAge = new CourtAllEmployeeList();
            List<CourtWorkUnit> CourtWorkUnitList = new List<CourtWorkUnit>();
            CourtWorkUnit CourtWorkUnit = new CourtWorkUnit();
            CourtEmployees CourtEmployees = new CourtEmployees();
            var employees = db.Person_Employee.Where(x => x.IsActive);
            var branchs = db.Branch.ToList();
            var placment = db.Placements.Where(x => x.isCurrent);
            if (branchs.Any())
            {
                foreach (var b in branchs)
                {
                    JudgeByAge = new CourtAllEmployeeList();
                    JudgeByAge.CourtWorkUnitList = new List<CourtWorkUnit>();

                    JudgeByAge.Branch = b.Name;
                    var assigned = placment.Where(x => x.BranchId == b.BranchId);
                    //  JudgeByAge.BranchTotal = assigned .Count ();
                    var workunits = assigned.Select(x => x.OrgStructureId).Distinct().ToList();
                    foreach (var a in assigned)
                    {
                        CourtWorkUnit = new CourtWorkUnit();
                        CourtWorkUnit.CourtEmployeesList = new List<CourtEmployees>();
                        var mainCompany = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId == a.OrgStructureId);
                        if(mainCompany != null)
                        {
                            CourtWorkUnit.Name = mainCompany.Name;
                        }    
                        foreach (var asign in workunits)
                        {
                            var assignedWorkunit = assigned.Where(x => x.OrgStructureId == asign).Distinct();

                            foreach (var emp in assignedWorkunit)
                            {

                                var getEmp = employees.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
                                if (getEmp != null)
                                {

                                    CourtEmployees = new CourtEmployees();
                                    CourtEmployees.EmpName = getEmp.FirstName + " " + getEmp.MiddleName + " " + getEmp.LastName;
                                    CourtEmployees.Sex = getEmp.Gender.ToString();
                                    CourtEmployees.AssignmentDate = emp.AssignmentDateAmharic;
                                    var jobName = CourtEmployees.EmpName;
                                    var check = CourtWorkUnit.CourtEmployeesList.FirstOrDefault(j => j.EmpName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)) == jobName.ToUpper(XAPI.Common.exceedculture).Equals(jobName.ToUpper(XAPI.Common.exceedculture)));
                                    if (check == null)
                                    {
                                        CourtWorkUnit.CourtEmployeesList.Add(CourtEmployees);
                                    }
                                    CourtWorkUnit.Total += CourtWorkUnit.CourtEmployeesList.Count;
                                    //    var check = JudgeByAge.CourtWorkUnitList.Where(x => x.CourtEmployeesList == CourtWorkUnit.CourtEmployeesList).ToList();

                                    JudgeByAge.CourtWorkUnitList.Add(CourtWorkUnit);


                                }

                            }

                        }
                        JudgeByAge.BranchTotal = JudgeByAge.CourtWorkUnitList.Count;
                    }


                    JudgeByAgeList.Add(JudgeByAge);
                }
            }
            return JudgeByAgeList;
        }
    }
}