using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class ClearanceProviderObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ClearanceProvider> GetClearanceProvider(string empId)
        {
            List<ClearanceProvider> list = new List<ClearanceProvider>();

            var allProviders = db.ClearanceProvideres.ToList();

            foreach (var a in allProviders)
            {
                var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == a.OrgStructureId);
                var placementEmpId = db.Placements.Where(x => x.OrgStructureId == a.OrgStructureId&&x.IsHead).Select(x => x.EmployeeId);

                if(mainCompany != null)
                {
                    var provider = new ClearanceProvider();
                    provider.Name = mainCompany.Name;
                    var employee = db.Person_Employee.Where(x => placementEmpId.Contains(x.EmployeeId)).FirstOrDefault();
                    if(employee != null)
                    {
                        provider.FullName = employee.FirstNameEnglish+" "+employee.MiddleNameEnglish+" "+employee.LastNameEnglish;

                    }

                    list.Add(provider);
                }
               
            }
            if (empId != null)
            {
                int.TryParse(empId, out int employeeId);
                var placement = db.Placements.Where(x => x.EmployeeId == employeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
                if (placement != null)
                {
                    var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                    if (mainCompany != null)
                    {
                        var checkDup = list.FirstOrDefault(x => x.Name.Trim().ToLower() == mainCompany.Name.Trim().ToLower());
                        if (checkDup == null)
                        {
                            var pro = new ClearanceProvider();

                            pro.Name = mainCompany.Name;
                            var employee = db.Person_Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
                            if (employee != null)
                            {
                                pro.FullName =  $"{employee.FirstNameEnglish} { employee.MiddleNameEnglish} {employee.LastNameEnglish}";

                            }
                            list.Add(pro);
                        }
                    }
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