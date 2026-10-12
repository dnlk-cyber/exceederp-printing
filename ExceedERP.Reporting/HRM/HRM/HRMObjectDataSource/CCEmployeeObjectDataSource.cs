using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    

    [DataObject]
    public class CCEmployeeObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LetThemKnow> GetCCEmployee(object[] letthem)
        {
            if (letthem == null || letthem.Length == 0)
                return new List<LetThemKnow>();

            // Convert object[] to IEnumerable<int> safely
            var employeeIds = letthem
                .Where(x => x != null) // Remove nulls
                .Select(x =>
                {
                    if (int.TryParse(x.ToString(), out int id))
                        return id;
                    return (int?)null; // Ignore non-integer values

                })
                .Where(id => id.HasValue) // Remove failed conversions
                .Select(id => id.Value) // Extract valid integers
                .ToList();

            if (!employeeIds.Any())
                return new List<LetThemKnow>();

            // Fetch employees in one query
            var employees = db.Person_Employee.Where(e => employeeIds.Contains(e.EmployeeId)).ToList();

            // Fetch placements for these employees
            var placements = db.Placements
                .Where(p => employeeIds.Contains(p.EmployeeId) && p.isCurrent)
                .ToList();

            // Fetch organization structures
            var orgStructureIds = placements.Select(p => p.OrgStructureId).Distinct().ToList();
            var mainCompanies = db.MainCompanyStructures.Where(mc => orgStructureIds.Contains(mc.OrgStructureId)).ToList();

            // Construct the final list
            var letThemKnowList = employees.Select(employee =>
            {
                var letThemKnow = new LetThemKnow
                {
                    EmployeeId = employee.EmployeeId,
                    EmployeeFullName = $"{employee.EmployeeCode}-{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}"
                };

                var placement = placements.FirstOrDefault(p => p.EmployeeId == employee.EmployeeId);
                if (placement != null)
                {
                    var mainCompany = mainCompanies.FirstOrDefault(mc => mc.OrgStructureId == placement.OrgStructureId);
                    if (mainCompany != null)
                    {
                        letThemKnow.OrgStructureId = mainCompany.OrgStructureId;
                        letThemKnow.Name = mainCompany.Name;
                    }
                }

                return letThemKnow;
            }).ToList();

            return letThemKnowList;
        }
        public void Dispose()
        {
            db.Dispose();
        }

        //public List<LetThemKnow> GetCCEmployee(object[] letthem)
        //{


        //    List<LetThemKnow> letThemKnowList = new List<LetThemKnow>();

        //    if (letthem != null)
        //    {
        //        if (letthem.Any())
        //        {
        //            foreach (var let in letthem)
        //            {
        //                int id = Convert.ToInt32(let);
        //                var employee = db.Person_Employee
        //                    .Find(id);
        //               var  letThemKnow = new LetThemKnow();
        //                if(employee != null)
        //                {

        //                    letThemKnow.EmployeeId = employee.EmployeeId;
        //                    letThemKnow.EmployeeFullName =$"{employee.EmployeeCode}-{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}";
        //                    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == employee.EmployeeId && x.isCurrent);
        //                    if (place != null)
        //                    {
        //                        var mainCompany = db.MainCompanyStructures.Find(place.OrgStructureId);
        //                        if (mainCompany != null)
        //                        {
        //                            letThemKnow.OrgStructureId = mainCompany.OrgStructureId;
        //                            letThemKnow.Name = mainCompany.Name;
        //                        }

        //                    }
        //                    letThemKnowList.Add(letThemKnow);
        //                }

        //            }


        //        }
        //    }

        //    return letThemKnowList;
        //}
    }
}