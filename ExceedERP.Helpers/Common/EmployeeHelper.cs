using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Helpers.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;

namespace ExceedERP.Helpers.Common
{

    public static class EmployeeHelper

    {
        static ExceedDbContext db = new ExceedDbContext();

        public static List<Person_Employee> GetEmployeesByBranch(int branchId)
        {
            db = new ExceedDbContext();
            var list = new List<Person_Employee>();
            var allEmployee = db.Person_Employee.ToList();
            if (allEmployee.Any())
            {
                var placements = db.Placements.Where(x => x.BranchId == branchId).ToList();
                if (placements.Any())
                {
                    foreach (var p in placements)
                    {
                        var check = allEmployee.FirstOrDefault(x => x.EmployeeId == p.EmployeeId);
                        if (check != null)
                        {
                            list.Add(check);
                        }
                    }
                }
            }

            return list;

        }
        public static List<Person_Employee> GetEmployeesBySelectedBranch(List<string> branchId)
        {
            db = new ExceedDbContext();
            var list = new List<Person_Employee>();
            var allEmployee = db.Person_Employee.ToList();

            var placements = db.Placements.Where(x => branchId.Contains(x.BranchId.ToString()) && x.isCurrent).ToList();
            if (placements.Any())
            {
                foreach (var p in placements)
                {
                    var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                    if (check != null)
                    {
                        list.Add(check);
                    }
                }
            }
            return list;

        }
        public static List<Person_Employee> GetEmployeesByReportsTo(int reportstoId)
        {
            db = new ExceedDbContext();
            var list = new List<Person_Employee>();
            var allEmployee = db.Person_Employee.ToList();
            var placements = db.Placements.Where(x => x.ReportsTo == reportstoId && x.isCurrent).ToList();
            if (placements.Any())
            {
                foreach (var p in placements)
                {
                    var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                    if (check != null)
                    {
                        list.Add(check);
                    }
                }
            }
            return list;

        }
        public static List<Person_Employee> GetEmployeesByBranchAndWorkUnit(int branchId, int orgStructureId)
        {
            db = new ExceedDbContext();
            var list = new List<Person_Employee>();
            var allEmployee = db.Person_Employee.ToList();
            var placements = db.Placements.Where(x => x.BranchId == branchId && x.OrgStructureId == orgStructureId && x.isCurrent).ToList();
            if (placements.Any())
            {
                var filterd = from f in placements
                              join e in allEmployee
                              on f.EmployeeId equals e.EmployeeId
                              where f.EmployeeId == e.EmployeeId
                              select (new { e });

                foreach (var p in filterd)
                {

                    list.Add(p.e);

                }
            }
            return list;

        }

        public static List<Person_Employee> GetEmployeesBySelectedBranchAndWorkUnit(List<string> branchId, int orgStructureId)
        {
            db = new ExceedDbContext();
            var list = new List<Person_Employee>();
            var allEmployee = db.Person_Employee.ToList();
            var placements = db.Placements.Where(x => branchId.Contains(x.BranchId.ToString()) && x.OrgStructureId == orgStructureId && x.isCurrent).ToList();
            if (placements.Any())
            {
                var filterd = from f in placements
                              join e in allEmployee
                              on f.EmployeeId equals e.EmployeeId
                              where f.EmployeeId == e.EmployeeId
                              select (new { e });

                foreach (var p in filterd)
                {

                    list.Add(p.e);

                }
            }
            return list;

        }
        public static List<Person_Employee> GetEmployeesByWorkUnit(int orgStructureId)
        {
            db = new ExceedDbContext();
            var list = new List<Person_Employee>();
            var allEmployee = db.Person_Employee.ToList();
            if (allEmployee.Any())
            {
                var placements = db.Placements.Where(x => x.OrgStructureId == orgStructureId && x.isCurrent).ToList();
                if (placements.Any())
                {
                    foreach (var p in placements)
                    {
                        var check = allEmployee.FirstOrDefault(x => x.EmployeeId == p.EmployeeId);
                        if (check != null)
                        {
                            list.Add(check);
                        }
                    }
                }

            }
            return list;
        }
        public static string GetWorkUnitName(int orgStructureId)
        {
            db = new ExceedDbContext();
            string Name = "";
            var Structure = db.MainCompanyStructures.Where(x => x.OrgStructureId == orgStructureId).FirstOrDefault();
            if (Structure != null)
            {
                Name = Structure.Name;
            }
            return Name;
        }
        //new
        public static string GetEmployeeJobTitle(int empid)
        {
            string jobTitle = "";
            var assignment = db.Placements
                .FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);
            if (assignment != null) jobTitle = assignment.JobTitleName;
            return jobTitle;
        }
        public static string GetEmployeeEnglishName(int empid)
        {
            string name = "";
            var employee = db.Person_Employee
                .FirstOrDefault(x => x.EmployeeId == empid);
            if (employee != null) name = employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish;
            return name;
        }

        public static int GetSupervisor(int empid)
        {
            db = new ExceedDbContext();
            int nextempid = 0;
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

            if (placement != null && placement.IsHead)
            {
                var currentWorkunit = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                if (currentWorkunit != null)
                {
                    var parentworkunit = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == currentWorkunit.Parent);
                    if (parentworkunit != null)
                    {
                        var headPlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == parentworkunit.OrgStructureId);
                        if (headPlacement != null)
                        {
                            nextempid = headPlacement.EmployeeId;
                        }
                    }

                }

            }
            else if (placement != null)
            {
                var head = db.Placements.FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == placement.OrgStructureId);
                if (head != null)
                {
                    nextempid = head.EmployeeId;
                }
            }
            return nextempid;
        }

        public static List<int> GetSubordinates(int empid)
        {
            ExceedDbContext db = new ExceedDbContext();
            List<int> employeeList = new List<int>();

            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);


            if (placement != null && placement.IsHead)
            {
                var workunits = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                if (workunits != null)
                {
                    var empPlacement = db.Placements.Where(x => x.isCurrent && x.OrgStructureId == workunits.OrgStructureId).ToList();

                    foreach (var emp in empPlacement)
                    {
                        employeeList.Add(emp.EmployeeId);
                    }
                    var childWorkunits = db.MainCompanyStructures.Where(x => x.Parent == workunits.OrgStructureId).ToList();
                    foreach (var ch in childWorkunits)
                    {
                        if (childWorkunits.Any())
                        {
                            var headPlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.IsHead && x.OrgStructureId == ch.OrgStructureId);
                            if (headPlacement != null)
                            {
                                employeeList.Add(headPlacement.EmployeeId);
                            }
                        }
                    }
                }

                return employeeList;
            }

            else
            {
                if (placement != null) employeeList.Add(placement.EmployeeId);
                return employeeList;
            }
            //child head


        }
        //new 
        public static string GetDaystoWords(double totalday)
        {
            //int totalday = (int)days;
            int months = 0;
            int year = 0;
            string words = "";
            if (totalday < 30)
            {
                words = words + totalday + " days ";
            }
            else if ((totalday >= 30) && (totalday <= 365))
            {
                months = (int)(totalday / 30);
                words = words + months + " month ";
                double monthremainder = totalday % 30;
                if (monthremainder < 30)
                {
                    words = words + (int)monthremainder + " days ";
                }
            }
            else if (totalday >= 356)
            {
                year = (int)(totalday / 365);
                words = words + year + " year ";
                double yearremainder = totalday % 365;
                if ((yearremainder >= 30) && (yearremainder <= 365))
                {
                    months = (int)(yearremainder / 30);
                    words = words + months + " month ";
                    double monthremainder = yearremainder % 30;
                    if (monthremainder < 30)
                    {
                        words = words + (int)monthremainder + " days ";
                    }
                }
            }

            return words;
        }

        public static string GetDaystoWordsAmharic(double totalday)
        {
            //int totalday = (int)days;
            int months = 0;
            int year = 0;
            string words = "";
            if (totalday < 30)
            {
                words = words + totalday + " ቀን ";
            }
            else if ((totalday >= 30) && (totalday <= 365))
            {
                months = (int)(totalday / 30);
                words = words + months + " ወር ";
                double monthremainder = totalday % 30;
                if (monthremainder < 30)
                {
                    words = words + "ከ " + (int)monthremainder + " ቀን ";
                }
            }
            else if (totalday >= 356)
            {
                year = (int)(totalday / 365);
                words = words + year + " አመት ";
                double yearremainder = totalday % 365;
                if ((yearremainder >= 30) && (yearremainder <= 365))
                {
                    months = (int)(yearremainder / 30);
                    words = words + "ከ " + months + " ወር ";
                    double monthremainder = yearremainder % 30;
                    if (monthremainder < 30)
                    {
                        words = words + "ከ " + (int)monthremainder + " ቀን ";
                    }
                }
            }

            return words;
        }
        public static string ToWordAmharic(this int number)
        {
            if (number == 0)
                return "ዜሮ";

            if (number < 0)
                return "ኔጌቲቭ " + Math.Abs(number).ToWordAmharic();

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += (number / 1000000).ToWordAmharic().TrimEnd() + " ሚሊዮን ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += (number / 1000).ToWordAmharic().TrimEnd() + " ሺ ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += (number / 100).ToWordAmharic().TrimEnd() + " መቶ ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += " ";

                var unitsMap = new[] { "ዜሮ", "አንድ", "ሁለት", "ሶስት", "አራት", "አምስት", "ስድስት", "ሰባት", "ስምንት", "ዘጠኝ", "አስር", "አስራ አንድ", "አስራ ሁለት", "አስራ ሶስት", "አስራ አራት", "አስራ አምስት", "አስራ ስድስት", "አስራ ሰባት", "አስራ ስምንት", "አስራ ዘጠኝ" };
                var tensMap = new[] { "ዜሮ", "አስር", "ሃያ", "ሰላሳ", "አርባ", "ሃምሳ", "ስልሳ", "ሰባ", "ሰማኒያ", "ዘጠና" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }

            return words;
        }

        public static int GetEmployeePlacementBranch(int empid)
        {
            ExceedDbContext db = new ExceedDbContext();
            int branch = 0;
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

            if (placement != null)
            {
                branch = placement.BranchId;

            }
            return branch;
        }
        //new 
        public static string GetEmployeePlacementBranchName(int empid)
        {
            ExceedDbContext db = new ExceedDbContext();
            string branchName = "";

            var branch = db.Branch.Find(GetEmployeePlacementBranch(empid));


            if (branch != null)
            {
                branchName = branch.Name;

            }
            return branchName;
        }
        public static decimal GetEmployeeSalary(int empid)
        {
            ExceedDbContext db = new ExceedDbContext();
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

            if (placement != null)
            {
                return placement.Salary;

            }
            return 0;
        }
        public static string GetEmployeeWorkUnit(int empid)
        {
            ExceedDbContext db = new ExceedDbContext();
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

            if (placement != null)
            {
                return GetWorkUnitName(placement.OrgStructureId);

            }
            return "";
        }
        public static int GetEmployeeWorkUnitId(int empid)
        {
            ExceedDbContext db = new ExceedDbContext();
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empid);

            if (placement != null)
            {
                return placement.OrgStructureId;

            }
            return 0;
        }

        public static int GetEmployeeDepartmentIdByEmpId(int empid)
        {

            int department = 0;

            var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == empid && x.isCurrent);
            if (placment != null)
            {

                department = placment.OrgStructureId;
            }

            return department;

        }
        public static string GetEmployeeDepartmentNameByEmpId(int empid)
        {
            string department = "";

            var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == empid && x.isCurrent);
            if (placment != null)
            {
                var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placment.OrgStructureId);
                if (mainCompany != null)
                {
                    department = mainCompany.Name;
                }
            }

            return department;

        }

        ////////////////////////////////////
        public static List<Person_Employee> GetAllEmployeesByBranchAndWorkUnit(int branchId, int orgStructureId)
        {
            db = new ExceedDbContext();
            var list = new List<Person_Employee>();
            var allEmployee = db.Person_Employee.ToList();
            var placements = db.Placements.Where(x => x.BranchId == branchId && x.OrgStructureId == orgStructureId).ToList();
            if (placements.Any())
            {
                var filterd = from f in placements
                              join e in allEmployee
                              on f.EmployeeId equals e.EmployeeId
                              where f.EmployeeId == e.EmployeeId
                              select (new { e });

                foreach (var p in filterd)
                {
                    list.Add(p.e);
                }
            }
            return list;

        }
        public static List<Person_Employee> GetAllEmployeesByBranch(int branchId)
        {
            db = new ExceedDbContext();
            var list = new List<Person_Employee>();
            var allEmployee = db.Person_Employee.ToList();
            var placements = db.Placements.Where(x => x.BranchId == branchId).ToList();
            if (placements.Any())
            {
                foreach (var p in placements)
                {
                    var check = allEmployee.FirstOrDefault(x => x.EmployeeId == p.EmployeeId);
                    if (check != null)
                    {
                        list.Add(check);
                    }
                }
            }
            return list;
        }
        public static List<Person_Employee> GetAllEmployeesByWorkUnit(int orgStructureId)
        {
            db = new ExceedDbContext();
            var list = new List<Person_Employee>();
            var allEmployee = db.Person_Employee.ToList();
            var placements = db.Placements.Where(x => x.OrgStructureId == orgStructureId).ToList();
            if (placements.Any())
            {
                foreach (var p in placements)
                {
                    var check = allEmployee.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                    if (check != null)
                    {
                        list.Add(check);
                    }
                }
            }
            return list;
        }

        public static List<Person_Employee> GetEmployeesByBranchInPlacement(List<Person_Employee> employees, int branchId)
        {
            var placements = db.Placements.Where(x => x.BranchId == branchId).DistinctByField(x => x.EmployeeId).ToList();
            if (placements.Any())
            {
                List<int> empIds = new List<int>();
                empIds.AddRange(placements.Select(x => x.EmployeeId));

                employees = employees.Where(x => empIds.Contains(x.EmployeeId)).ToList();

                return employees;
            }
            return null;
        }

        public static List<Person_Employee> GetEmployeesByDepartment(List<Person_Employee> employees, int orgStructureId)
        {
            var placements = db.Placements.Where(x => x.OrgStructureId == orgStructureId).DistinctByField(x => x.EmployeeId).ToList();
            if (placements.Any())
            {
                List<int> empIds = new List<int>();
                empIds.AddRange(placements.Select(x => x.EmployeeId));

                employees = employees.Where(x => empIds.Contains(x.EmployeeId)).ToList();

                return employees;
            }
            return null;
        }


        ///////// Data From Placement Table
        public static List<Placement> GetEmployees_ByBranch_FromPlacement(List<Placement> placements, int branchId)
        {
            placements = placements.Where(x => x.BranchId == branchId).ToList();

            return placements;
        }
        public static List<Placement> GetEmployees_ByDepartment_FromPlacement(List<Placement> placements, int orgId)
        {
            placements = placements.Where(x => x.OrgStructureId == orgId).ToList();

            return placements;
        }
        public static List<Placement> GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(List<Placement> placements, int orgId)
        {
            var structureIds = db.MainCompanyStructures.Where(x => x.Parent == orgId).Select(x => x.OrgStructureId).ToList();
            placements = placements.Where(x => structureIds.Contains(x.OrgStructureId)).ToList();

            return placements;
        }
        public static List<Placement> GetEmployees_ByJobTitle_FromPlacement(List<Placement> placements, int lookupId)
        {
            placements = placements.Where(x => x.LookupId == lookupId).ToList();

            return placements;
        }
        public static List<Placement> Get_Active_EmployeesFrom_Placement(List<Placement> placements)
        {
            var empIds = db.Person_Employee.Where(x => x.IsActive).Select(x => x.EmployeeId).ToList();
            placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

            return placements;
        }
        public static List<Placement> Get_InActive_EmployeesFrom_Placement(List<Placement> placements)
        {
            var empIds = db.Person_Employee.Where(x => !x.IsActive).Select(x => x.EmployeeId).ToList();
            placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

            return placements;
        }

        public static List<Placement> Get_Male_EmployeesFrom_Placement(List<Placement> placements)
        {
            var empIds = db.Person_Employee.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
            placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

            return placements;
        }
        public static List<Placement> Get_Female_EmployeesFrom_Placement(List<Placement> placements)
        {
            var empIds = db.Person_Employee.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();
            placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();

            return placements;
        }
        public static List<Placement> Get_Employees_ByGender_From_Placement(List<Placement> placements, string gender)
        {
            if (gender.ToLower() == "male")
            {
                var empIds = db.Person_Employee.Where(x => x.Gender == Gender.MALE).Select(x => x.EmployeeId).ToList();
                placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }
            else if (gender.ToLower() == "female")
            {
                var empIds = db.Person_Employee.Where(x => x.Gender == Gender.FEMALE).Select(x => x.EmployeeId).ToList();
                placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
            }

            return placements;
        }
    }
}
