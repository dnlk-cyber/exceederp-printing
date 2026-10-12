using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class BenefitODS:IDisposable
    {

        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<BenefitVM> GetBenefit(string gender, string branchId,string businessUnit,string befitType)
        {
            List<BenefitVM> benefitsList = new List<BenefitVM>();
            // Get lookup IDs of current placements
           
            HashSet<int> lookupIds = new HashSet<int>(db.Placements.Where(x => x.isCurrent).Select(x => x.LookupId));
            var benefits = db.OtherBenefits.Where(x => lookupIds.Contains((int)x.LookupId) ).ToList();
            var placements = db.Placements
                                  .Where(x => x.isCurrent).ToList();
            if (!string.IsNullOrEmpty(gender))
            {
                if(gender != "BOTH")
                {
                    HashSet<int> empIds = new HashSet<int>(db.Person_Employee.Where(x => x.IsActive && x.Gender.ToString() == gender).Select(x => x.EmployeeId));
                    lookupIds = new HashSet<int>(placements.Where(x =>  empIds.Contains(x.EmployeeId)).Select(x => x.LookupId));
                    placements = placements.Where(x => empIds.Contains(x.EmployeeId)).ToList();
                }
                 benefits = db.OtherBenefits.Where(x => lookupIds.Contains((int)x.LookupId)).Where(x=>x.Gender.ToString()==gender).ToList();


            }
            if (!string.IsNullOrEmpty(branchId))
            {
                int.TryParse(branchId, out int braId);
                placements = placements.Where(x => x.BranchId == braId).ToList();

                lookupIds = new HashSet<int>(placements.Select(x => x.LookupId));
                benefits = db.OtherBenefits.Where(x => lookupIds.Contains((int)x.LookupId)).ToList();

            }
            if (!string.IsNullOrEmpty(businessUnit) && int.TryParse(businessUnit, out int businessUnitId)&& businessUnitId !=0)
            {
                placements = placements.Where(x => x.BusinessUnitId == businessUnitId).ToList();
                lookupIds = new HashSet<int>(placements.Select(x => x.LookupId));
                benefits = db.OtherBenefits.Where(x => lookupIds.Contains((int)x.LookupId)).ToList();

            }
            if (!string.IsNullOrEmpty(befitType))
            {
                int.TryParse(befitType, out int benfitId);
                benefits = db.OtherBenefits.Where(x => x.BenefitName== benfitId).ToList();
            }

            // Get distinct benefits based on LookupId
            var distinctBenefits = benefits.DistinctByField(x => x.LookupId).ToList();
            foreach (var benefit in distinctBenefits)
            {
                var benefitVM = new BenefitVM
                {
                    JobTitle = db.Lookups.Find(benefit.LookupId)?.Description,
                    BenefitName = db.Lookups.Find(benefit.BenefitName)?.Description,
                    Quantity = benefits.Where(x=>x.LookupId==benefit.LookupId).Sum(x=>x.Quantity),
                    UnitOFMeasuremnet = db.Lookups.Find(benefit.UnitofMeasurement)?.Description ?? " ",
                    Description = benefit.TimeInterval
                };
                var placementPerJobTitles = placements.Where(x => x.LookupId == benefit.LookupId).ToList();

                benefitVM.EmployeeBenefitDetails = new List<EmployeeBenefitDetailVM>();
                foreach (var placement in placementPerJobTitles)
                {
                    if(benefit.Gender == GenderBenefit.BOTH||string.IsNullOrEmpty(gender))
                    {
                        var employee = db.Person_Employee.Find(placement.EmployeeId);
                        var branch = db.Branch.Find(placement.BranchId);

                        if (employee != null && branch != null)
                        {
                            var benefitDetail = new EmployeeBenefitDetailVM
                            {
                                Employee = employee.EmployeeCode + "-" + employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish,
                                Branch = branch.Name
                            };

                            benefitVM.EmployeeBenefitDetails.Add(benefitDetail);
                        }
                    }
                    else
                    {
                        var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == placement.EmployeeId && x.Gender.ToString() == gender);
                        var branch = db.Branch.Find(placement.BranchId);
                        if (employee != null && branch != null)
                        {
                            var benefitDetail = new EmployeeBenefitDetailVM
                            {
                                Employee = employee.EmployeeCode + "-" + employee.FirstNameEnglish + " " + employee.MiddleNameEnglish + " " + employee.LastNameEnglish,
                                Branch = branch.Name
                            };
                            benefitVM.EmployeeBenefitDetails.Add(benefitDetail);
                        }
                    }
                   
                }
                var Jobbenefits = benefits.Where(x => x.LookupId == benefit.LookupId).ToList();
                benefitVM.BenefitDetails = new List<BenefitDetailVM>();
                foreach (var Jobbenefit in Jobbenefits)
                {
                    var benefitDetail = new BenefitDetailVM
                    {
                        BenefitName = db.Lookups.Find(Jobbenefit.BenefitName)?.Description,
                        Quantity = Jobbenefit.Quantity,
                        Description = Jobbenefit.TimeInterval,
                        UnitOFMeasuremnet = db.Lookups.Find(Jobbenefit.UnitofMeasurement)?.Description ?? " ",
                        TotalQuantity = Jobbenefit.Quantity* benefitVM.EmployeeBenefitDetails.Count
                    };
                    benefitVM.BenefitDetails.Add(benefitDetail);
                }
                benefitVM.NumberOfEmployee = benefitVM.EmployeeBenefitDetails.Count;
                benefitVM.TotalQuantity = benefitVM.NumberOfEmployee * benefitVM.BenefitDetails.Sum(x=>x.Quantity);
                benefitsList.Add(benefitVM);

            }
            return benefitsList;

        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
