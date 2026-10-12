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
    public class EmployeePromotionSummaryObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeePromotionSummaryVM> GetEmployeePromotionSummary(string branchId, string orgStructureId)
        {
            // Get current culture
            string cultureName = GlobalHelper.GetCurrentCulture();

            // Filter placements based on AssignmentType
            var placements = db.Placements.Where(x =>
                (x.AssignmentType == AssignmentType.PROMOTION || x.AssignmentType == AssignmentType.YEARLY_PROMOTION)
                && x.isCurrent).ToList();

            // Apply branch filter if branchId is provided
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
            {
                placements = placements.Where(x => x.BranchId == braId).ToList();
            }

            // Apply organizational structure filter if orgStructureId is provided
            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
            {
                placements = placements.Where(x => x.OrgStructureId == orgId).ToList();
            }

           
            // List to hold the summary
            List<EmployeePromotionSummaryVM> list = new List<EmployeePromotionSummaryVM>();

            // Enumerate over the selected promotion types
            List<string> selectedTypes = new List<string>
            {
                "PROMOTION",
                "YEARLY_PROMOTION"
            };
            if (placements.Any())
            {


                foreach (var promoType in selectedTypes)
                {
                    // Filter placements by current promotion type
                    var employeeIds = placements
                        .Where(x => x.AssignmentType.ToString() == promoType)
                        .Select(x => x.EmployeeId).ToList();
                    if (employeeIds.Any())
                    {


                        // Get the corresponding employees
                        var employees = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();

                        // Create a summary view model for the current promotion type
                        if (employees.Any())
                        {


                            var employeeSummary = new EmployeePromotionSummaryVM
                            {
                                PromationType = promoType.ToString(),
                                NumberOfFemaleEmployee = employees.Count(x => x.Gender == Gender.FEMALE),
                                NumberOfMaleEmployee = employees.Count(x => x.Gender == Gender.MALE),
                                TotalEmployee = employees.Count()
                            };

                            // Add the summary to the list
                            list.Add(employeeSummary);
                        }
                    }
                }
            }

            // Return the summary list
            return list;
        }

        //public List<EmployeePromotionSummaryVM> GetEmployeePromotionSummary(string branchId, string orgStructureId)
        //{
        //    string cultureName = GlobalHelper.GetCurrentCulture();
        //    var placements = db.Placements.Where(x=>x.AssignmentType== AssignmentType.PROMOTION||x.AssignmentType==AssignmentType.YEARLY_PROMOTION).Where(x=>x.isCurrent).ToList();
        //    List<EmployeePromotionSummaryVM> list = new List<EmployeePromotionSummaryVM>();
        //    if (branchId != null)
        //    {
        //        int.TryParse(branchId, out int braId);
        //        var branchEmpId = placements.Where(x => x.BranchId == braId).Select(x => x.EmployeeId).ToList();
        //    }
        //    if (orgStructureId != null)
        //    {
        //        int.TryParse(orgStructureId, out int orgId);
        //        placements = placements.Where(x => x.OrgStructureId==orgId).ToList();

        //    }

        //    var selectedTypes = new AssignmentType[]
        //    {
        //        AssignmentType.PROMOTION,
        //        AssignmentType.YEARLY_PROMOTION
        //    };
        //    foreach (var promoType in selectedTypes)
        //    {
        //        var employee = new EmployeePromotionSummaryVM();
        //        var employeeIds = placements.Where(x => x.AssignmentType == promoType).Select(x => x.EmployeeId);
        //        var employees = db.Person_Employee.Where(x => employeeIds.Contains(x.EmployeeId));
        //        employee.PromationType = promoType.ToString();
        //        employee.NumberOfFemaleEmployee =employees.Count(x => x.Gender==Gender.FEMALE);

        //        employee.NumberOfMaleEmployee = employees.Count(x => x.Gender == Gender.MALE);

        //        employee.TotalEmployee = employee.NumberOfFemaleEmployee + employee.NumberOfMaleEmployee;

        //        list.Add(employee);
        //    }



        //    return list;
        //}
        public void Dispose()
        {
            db.Dispose();
        }
    }
}