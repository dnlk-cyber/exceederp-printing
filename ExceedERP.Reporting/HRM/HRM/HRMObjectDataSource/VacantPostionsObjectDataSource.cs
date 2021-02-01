using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class VacantPostionsObjectDataSource : VacantPostions
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public VacantPostions GetVacantPostions(string branchId, string org, string title)
        {
            VacantPostions VacantPostions = new VacantPostions();
            List<Person_Employee> employees;
            
            VacantPostions = new VacantPostions();
           
            employees = HrHelper.PersonEmployeesOneBranch(branchId, org, "");

            var toBepostions =db.JobTitleMapping.ToList();
            var currentStaff = db.Placements.Where(x => x.isCurrent).ToList();
            //list.Add(VacantPostions);

            if (org != null )
            {
                //var lookups = db.Lookups.Where(x => x.LookupId.ToString() == title).FirstOrDefault().LookupId;
                //var Org = db.MainCompanyStructures.Where(x => x.OrgStructureId.ToString () == org).FirstOrDefault().OrgStructureId;


                toBepostions =
                    toBepostions.Where(x => x.OrgStructureId.ToString() == org )
                        .ToList();
                currentStaff =
                    currentStaff.Where(x => x.OrgStructureId.ToString() == org )
                        .ToList();
                int quntity = 0;
            }
            if (title != null)
            {
                //var lookups = db.Lookups.Where(x => x.LookupId.ToString() == title).FirstOrDefault().LookupId;
                //var Org = db.MainCompanyStructures.Where(x => x.OrgStructureId.ToString () == org).FirstOrDefault().OrgStructureId;


                toBepostions =
                    toBepostions.Where(x =>   x.LookupId.ToString() == title)
                        .ToList();
                currentStaff =
                    currentStaff.Where(x =>  x.LookupId.ToString() == title)
                        .ToList();
                int quntity = 0;
            }
            if (toBepostions.Any())
                {
                    foreach (var to in toBepostions)
                    {
                          VacantPostions.ToBeStaff += to.Quntity;
                    }
                  

                }
                if (currentStaff.Any())
                {
                    var assignedEmployee = from c in currentStaff
                        join e in employees
                            on c.EmployeeId equals e.EmployeeId
                        select (new {e.Gender});

                    VacantPostions.NumberOfMaleEmployee = assignedEmployee.ToList().Where(x => x.Gender == Gender.MALE).Count();
                    
                  
                          VacantPostions.NumberOfFemaleEmployee = assignedEmployee.ToList().Where(x => x.Gender == Gender.FEMALE).Count();
                       
                    
                }
                VacantPostions.TotalEmployee = VacantPostions.NumberOfFemaleEmployee + VacantPostions.NumberOfMaleEmployee;
                VacantPostions.VacantPos = VacantPostions.ToBeStaff -
                                           (VacantPostions.TotalEmployee);
            
           
            return VacantPostions;
        }
    }
}