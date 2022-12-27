using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class BasicSalaryObjectDataSource : List<BasicSalaryList>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BasicSalaryList> GetPayrollBasicSalaryList(string branchId, string user,string status)
        {
            List<BasicSalaryList> list = new List<BasicSalaryList> { };
            BasicSalaryList EmployeeInfo;
            List<EmployeeIdView> emplist = new List<EmployeeIdView>();

            if (status== "Active")
                emplist = ListOfEmployeeIdView.GetEmployees();
            else if (status == "InActive")
                emplist = ListOfEmployeeIdView.GetEmployees(false);

            decimal totalAmount = 0;
            int maxid = 0;

            foreach (EmployeeIdView emp in emplist)
            {

                var assignmentList = db.Placements.Where(x => x.EmployeeId == emp.PersonId && x.isCurrent).ToList();


                if (branchId != null)
                {
                    List<string> branchs = branchId.Split('-').ToList();
                    assignmentList = assignmentList.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
                }
                else if (UserHelper.CheckUserRole(user, "All_Branch_Payroll_Report"))
                {
                    List<string> branchs = UserHelper.GetUserBranchId(user);
                    assignmentList = assignmentList.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
                }
                else
                {
                    assignmentList = null;
                }
                if (assignmentList != null && assignmentList.Any())
                {
                    EmployeeInfo = new BasicSalaryList();
                    EmployeeInfo.EmpID = emp.EmployeeCode;
                    EmployeeInfo.EmpFullName = emp.FullName;
                    maxid = assignmentList.Max(x => x.PlacementId);
                    EmployeeInfo.BasicSalary = (decimal)assignmentList.Where(x => x.PlacementId == maxid).Select(x => x.Salary).FirstOrDefault();
                    list.Add(EmployeeInfo);
                    totalAmount = totalAmount + EmployeeInfo.BasicSalary;
                }



            }

            return list;
        }
    }
}