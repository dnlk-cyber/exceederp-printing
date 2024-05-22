using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class ContractEmployeeObjectDataSource : List<ContractEmployeeVM>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ContractEmployeeVM> GetContractEmployee(string branch, string orgId,DateTime? from, DateTime? to, decimal? dayLeft )
        {
            List<ContractEmployeeVM> contractEmployeeList = new List<ContractEmployeeVM>();

            var place = db.Placements.Where(x => x.isCurrent&&x.EmployementType ==EmployeeStatus.CONTRACT).ToList();
            if (branch != null)
            {
                int.TryParse(branch, out int branchId);
                place = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(place, branchId);
            }
            if (orgId != null)
            {
                int.TryParse(orgId, out int orgStructureId);
                place = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(place, orgStructureId);
            }
            if (from.HasValue)
            {
                var employeeIds = db.Contract.AsNoTracking().Where(x => x.From >= from.Value).Select(x=>x.EmployeeId).Distinct().ToList();
                place = place.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
            }
            if (to.HasValue)
            {
                var employeeIds = db.Contract.AsNoTracking().Where(x => x.To <= to.Value).Select(x=>x.EmployeeId).Distinct().ToList();
                place = place.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
            }
            if(dayLeft > 0||dayLeft != null||!string.IsNullOrEmpty(dayLeft.ToString()))
            {
                var employeeIds = db.Contract.AsNoTracking().ToList().Where(x => ((decimal)(x.To - DateTime.Now).TotalDays>=0)&& ((decimal)(x.To - DateTime.Now).TotalDays<=dayLeft)).Select(x => x.EmployeeId).Distinct().ToList();
                place = place.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
            }
            foreach (var e in place)
            {
                var contratEmployee = new ContractEmployeeVM();

                var employes = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);
                if (employes != null)
                {
                    contratEmployee.EmployeeId = employes.EmployeeCode;
                    contratEmployee.FirstName = employes.FirstNameEnglish;
                    contratEmployee.MiddleName = employes.MiddleNameEnglish;
                    contratEmployee.LastName = employes.LastNameEnglish;
                    contratEmployee.Status = e.EmployementType.ToString();
                    contratEmployee.HireDate = employes.HiredDateAmharic+"("+employes.HiredDate.Day+"/"+employes.HiredDate.Month+"/" + employes.HiredDate.Year+"G.C.)";
                    var contract = db.Contract.Where(x => x.EmployeeId == employes.EmployeeId).OrderByDescending(x => x.To).FirstOrDefault();
                    if (contract != null)
                    {
                        contratEmployee.ExpireDate = contract.ToContAm + "(" + contract.To.Day + "/" + contract.To.Month + "/" + contract.To + "G.C.)";
                        

                       
                    }
                    var brach = db.Branch.Where(x => x.BranchId == e.BranchId).FirstOrDefault();
                    if (brach != null)
                    {
                        contratEmployee.Branch = brach.Name;
                    }
                    var lookJobTitle = db.Lookups.FirstOrDefault(x => x.LookupId == e.LookupId);
                    if (lookJobTitle != null)
                    {
                        contratEmployee.JobTitle = lookJobTitle.Description;
                    }
                    var workUnit = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == e.OrgStructureId);
                    if (workUnit != null)
                    {
                        contratEmployee.WorkUnit = workUnit.Name;
                    }
                    
                    contractEmployeeList.Add(contratEmployee);


                }
            }

            return contractEmployeeList;

        }

    }
}