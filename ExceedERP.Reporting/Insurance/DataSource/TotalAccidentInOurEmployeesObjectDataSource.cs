using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Insurance.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Insurance.DataSource
{

    [DataObject]
    public class TotalAccidentInOurEmployeesObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TotalAccidentInOurEmployeesViewModel> GetAccident(string year)
        {
            List<TotalAccidentInOurEmployeesViewModel> accidentList = new List<TotalAccidentInOurEmployeesViewModel>();
            if (year != null)
            {
                var glyear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId.ToString() == year);
                if (glyear != null)
                {
                    var empId = 0;
                    var employeeAccidents = db.EmployeeAccidents.ToList();
                    foreach (var accident in employeeAccidents)
                    {
                        if(accident.EmployeeId == empId)
                        {
                            continue;
                        }
                        empId = accident.EmployeeId;
                        var empAccident = new TotalAccidentInOurEmployeesViewModel();
                        var employeeAccident = employeeAccidents.FirstOrDefault(x =>x.EmployeeAccidentId == accident.EmployeeAccidentId &&
                                                                       x.AccidentDate >= glyear.DateFrom && x.AccidentDate <= glyear.DateTo);
                        var totalCurrent = employeeAccidents.Where(x=> x.AccidentDate >= glyear.DateFrom && x.AccidentDate <= glyear.DateTo);
                        if (employeeAccident != null)
                        {
                            empAccident.AccidentType = "";
                            empAccident.AccidentsInCurrentYear = totalCurrent.Count();

                        }
                        var employeeAccidentPrev = employeeAccidents.FirstOrDefault(x => x.EmployeeAccidentId == accident.EmployeeAccidentId &&
                                                                          x.AccidentDate >= glyear.DateFrom && x.AccidentDate <= glyear.DateTo);
                        var totalPrev = employeeAccidents.Where(x => x.AccidentDate >= glyear.DateFrom && x.AccidentDate <= glyear.DateTo);
                        if (employeeAccidentPrev != null)
                        {
                            empAccident.AccidentType = "";
                            empAccident.AccidentsInCurrentYear = totalPrev.Count();

                        }
                        if(empAccident.AccidentsInCurrentYear >= empAccident.AccidentsInpreviousYear)
                        {
                            empAccident.Difference = (empAccident.AccidentsInCurrentYear - empAccident.AccidentsInpreviousYear).ToString();
                        }
                        else
                        {
                            empAccident.Difference = (empAccident.AccidentsInpreviousYear - empAccident.AccidentsInCurrentYear).ToString();

                        }
                        accidentList.Add(empAccident);
                    }
                }
            }
            return accidentList;
        }


    }
}

