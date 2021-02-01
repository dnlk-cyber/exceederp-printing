using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
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
   public class ProbationPerformanceMeasureODS
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ProbationMeasureVM> GetProbation(string empId)
        {
            List<ProbationMeasureVM> measureLists = new List<ProbationMeasureVM>();
            ProbationMeasureVM pro = new ProbationMeasureVM();
            pro.ProbationMeasureDetail = new List<ProbationMeasureDetailVM>();
            pro.EmployementType = new List<EmployementTypeVM>();
            EmployementTypeVM emt = new EmployementTypeVM();
            var probation = db.ProbationPerformances.FirstOrDefault(x=>x.EmployeeId.ToString() == empId);

            if(probation != null)
            {
                pro.ProbationEndDate = probation.ProbationEndDateAm;

                var person = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == probation.EmployeeId);
                if(person != null)
                {
                    pro.EmpName = person.FullNameAmharic;
                    pro.HireDate = person.HiredDateAmharic;
                    if(person.Status == Core.Domain.Common.EmployeeStatus.PROBATION)
                    {
                        emt.Probation = "√";
                    }
                    if (person.Status == Core.Domain.Common.EmployeeStatus.CONTRACT)
                    {
                        emt.Contrat = "√";
                    }
                    if (person.Status == Core.Domain.Common.EmployeeStatus.FREELANCER)
                    {
                        emt.Freelance = "√";
                    }

                    pro.EmployementType.Add(emt);
                }
                var place = db.Placements.FirstOrDefault(x=>x.EmployeeId == probation.EmployeeId && x.isCurrent);
                if(place != null)
                {
                    var mainCompany = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId == place.OrgStructureId);
                    if(mainCompany != null)
                    {
                        pro.Workunit = mainCompany.Name;
                    }
                    var look = db.Lookups.FirstOrDefault(x=>x.LookupId == place.LookupId && x.LookupType == Core.Domain.Common.LookUpTypes.JOB_TITLE);
                    if(look != null)
                    {
                        pro.JobTitle = look.Description;
                    }
                }

                var probationDetails = db.ProbationPerformanceDetails.Where(x=>x.ProbationPerformanceId == probation.ProbationPerformanceId).ToList();
                foreach(var pd in probationDetails)
                {
                    var proDetail = new ProbationMeasureDetailVM();

                    var lookType = db.Lookups.FirstOrDefault(x=>x.LookupId == pd.LookupId && x.LookupType == Core.Domain.Common.LookUpTypes.PROBATION_MEASUREMENT);
                    if(lookType != null)
                    {
                        proDetail.Measurment = lookType.Description;
                    }
                    if(pd.Measurement == ProbationMeasurement.Very_Good)
                    {
                        proDetail.VeryGood = "√";
                    }
                    if (pd.Measurement == ProbationMeasurement.Good)
                    {
                        proDetail.Good = "√";
                    }
                    if (pd.Measurement == ProbationMeasurement.Enough)
                    {
                        proDetail.Enough = "√";
                    }
                    if (pd.Measurement == ProbationMeasurement.Low)
                    {
                        proDetail.Low = "√";
                    }

                    pro.ProbationMeasureDetail.Add(proDetail);
                }

                measureLists.Add(pro);
            }


            return measureLists;
        }
    }
}
