using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{

    [DataObject]
    public class LongTermEducationObjectDataSource : List<LongTermEducation>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LongTermEducation> GetLongTermEducation(string branch, string workunit, string eduType, string provider, DateTime from, DateTime to)
        {
            List<LongTermEducation> longTermEducationList = new List<LongTermEducation>();
            LongTermEducation longTermEducation = new LongTermEducation();

            var courseTracking = db.CourseTrackings.ToList();
            var employees = db.Person_Employee.Where(x => x.IsActive);
            var placments = db.Placements.Where(x => x.isCurrent).ToList();
            var course = db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.COURSE).ToList();
            var educationLevel = db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.EDUCATION_LEVEL).ToList();
            var providers = db.Companies.ToList();

            if (branch != null)
            {
                int.TryParse(branch, out int branchId);
                placments = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placments, branchId).ToList();
            }
            if(workunit != null)
            {
                int.TryParse(workunit, out int orgId);
                placments = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placments, orgId);
            }
            if(eduType != null)
            {
                int.TryParse(eduType, out int lookUpId);
                course = course.Where(x=>x.LookupId == lookUpId).ToList();
            }
            if (provider != null)
            {
                int.TryParse(provider, out int mainCompanyId);
                providers = providers.Where(x => x.MainCompanyId == mainCompanyId).ToList();
            }

            if(from <= to)
            {
                courseTracking = courseTracking.Where(x=>x.From.Date >= from.Date && x.To.Date <= to.Date).ToList();
            }           
           
            var trainee = db.Trainee.ToList();           
         
            var longTermTrainee = from cr in courseTracking
                                  join tr in trainee
                                  on cr.CourseTrackingId equals tr.CourseTrackingId
                                  join em in employees
                                  on tr.EmployeeId equals em.EmployeeId
                                  join c in placments on em.EmployeeId equals c.EmployeeId
                                  join l in course on cr.LookupId equals l.LookupId
                                  join el in educationLevel on tr.EducationLevel equals el.LookupId
                                  join comp in providers
                                  on cr.MainCompanyId equals comp.MainCompanyId
                                  where comp.Category == "1" && cr.TrainingTerm == Core.Domain.Common.TrainingTerm.LONG
                                  select new
                                  {
                                      Name = l.Description,
                                      level = el.Description,
                                      provider = comp.TradeName
                                  };

            if (longTermTrainee.Any())
            {
                foreach (var b in longTermTrainee)
                {
                    longTermEducation = new LongTermEducation();

                    longTermEducation.EducationType = b.Name;
                    longTermEducation.EducationLevel = b.level;
                    longTermEducation.EducationProvider = b.provider;
                    longTermEducationList.Add(longTermEducation);

                }
            }

            return longTermEducationList;
        }
    }
}
