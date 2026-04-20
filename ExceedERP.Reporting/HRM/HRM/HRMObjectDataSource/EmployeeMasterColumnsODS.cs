using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeMasterColumnsODS
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetEmployeeMasterColumns()
        {
            List<string> columns = new List<string>();
            columns.Add("MartialStatus");
            columns.Add("Branch");
            columns.Add("Department");
            columns.Add("Jobtitle");
            columns.Add("ResponsibilityLevel");
            columns.Add("EmployementType");
            columns.Add("Rank");
            columns.Add("Step");
            columns.Add("LeaveBalance");
            columns.Add("EducationField");
            columns.Add("EducationLevel");
            columns.Add("EduFrom");
            columns.Add("EduTo");
            columns.Add("EduInstitution");
            columns.Add("TotalInternalExperience");
            columns.Add("ExternalExperience");
            columns.Add("TotalExperience");
            columns.Add("HireDate");
            columns.Add("BirthDate");
            columns.Add("RetirementDate");
            columns.Add("RecruitmentType");
            columns.Add("Salary");
            columns.Add("Training");
            columns.Add("Language");
            columns.Add("Performance");
            columns.Add("Descipline");
            columns.Add("InternalExpriance");
            columns.Add("InternalExprianceDetail");
            columns.Add(".ExpirianceDetails");

            return columns;
        }
    }
}
