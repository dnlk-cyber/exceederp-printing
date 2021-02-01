using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    
    [DataObject]
    public class LongTermEducationObjectDataSource : List<LongTermEducation>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<LongTermEducation> GetLongTermEducation(string branch, string workunit)
        {
            List<LongTermEducation> longTermEducationList = new List<LongTermEducation>();
            LongTermEducation longTermEducation = new LongTermEducation();
          
            var employees = db.Person_Employee.Where(x => x.IsActive);
          var placment = db.Placements.Where(x => x.isCurrent);
            if (branch != null && workunit != null)
            {
                placment = db.Placements.Where(x => x.isCurrent && x.BranchId.ToString()==branch && x.OrgStructureId.ToString()==workunit);
            }
            else if(branch != null && workunit == null)
            {
                placment = db.Placements.Where(x => x.isCurrent && x.BranchId.ToString() == branch);

            }
            else if(branch == null && workunit != null)
            {
                placment = db.Placements.Where(x => x.isCurrent && x.OrgStructureId.ToString() == workunit);
            }
            var courseTracking = db.CourseTrackings.ToList();
            var trainee = db.Trainee.ToList();
            var course= db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.COURSE).ToList();
            var educationLevel = db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.EDUCATION_LEVEL).ToList();
            var longTermTrainee = from cr in courseTracking
                           join tr in trainee 
                           on cr.CourseTrackingId equals tr.CourseTrackingId 
                           join em in employees 
                           on tr.EmployeeId equals em.EmployeeId 
                           join c in placment on em.EmployeeId equals c.EmployeeId                      
                                                      join l in course on cr.LookupId equals l.LookupId
                           join el in educationLevel on tr.EducationLevel equals  el.LookupId
                           join comp in db.Companies 
                           on cr.MainCompanyId  equals comp .MainCompanyId 
                           where comp .Category =="1" && cr.TrainingTerm ==Core.Domain.Common.TrainingTerm.LONG
                           select new
                           {
                               Name = l.Description ,
                               level = el.Description,
                               provider= comp.TradeName 
                           };

            if (longTermTrainee.Any())
            {
                foreach (var b in longTermTrainee)
                {
                    longTermEducation = new LongTermEducation();

                    longTermEducation.EducationType = b.Name;
                    longTermEducation.EducationLevel = b.level;
                    longTermEducation.EducationProvider = b.provider ;
                    longTermEducationList.Add(longTermEducation);

                }
            }

            return longTermEducationList;
        }
    }
}
