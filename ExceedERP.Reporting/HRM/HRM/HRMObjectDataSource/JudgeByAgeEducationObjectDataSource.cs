using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class JudgeByAgeEducationObjectDataSource : List<JudgeByAgeAndEducation>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JudgeByAgeAndEducation> GetJudgeByAgeEduaction()
        {
            List<JudgeByAgeAndEducation> JudgeByAgeList = new List<JudgeByAgeAndEducation>();
            JudgeByAgeAndEducation JudgeByAge = new JudgeByAgeAndEducation();
            JudgeEduaction JudgeEduaction = new JudgeEduaction();
            var employees = db.Person_Employee.Where(x => x.IsActive);
            var branchs = db.Branch;
            var placment = db.Placements.Where(x => x.isCurrent);
            var education = db.Education;
            var eduLevels = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL);
            if (branchs.Any())
            {
                foreach (var b in branchs)
                {
                    JudgeByAge = new JudgeByAgeAndEducation();
                    JudgeByAge.JudgeEduactions = new List<JudgeEduaction>();
                    JudgeByAge.BranchName = b.Name;
                    var assigned = placment.Where(x => x.BranchId == b.BranchId);
                    foreach (var edu in eduLevels)
                    {
                        JudgeEduaction = new JudgeEduaction();
                        var geteduca = education.Where(x => x.EduLevel == edu.LookupId).Count();
                        JudgeEduaction.EducationLevel = edu.Description;

                        foreach (var a in assigned)
                        {
                            var checkEducation = education.Where(x => x.EmployeeId == a.EmployeeId && x.EduLevel == edu.LookupId).FirstOrDefault();
                            if (checkEducation != null)
                            {

                                var getEmp = employees.Where(x => x.EmployeeId == a.EmployeeId).FirstOrDefault();

                                if (getEmp != null)
                                {
                                    if (getEmp.Gender == Gender.MALE)
                                    {
                                        JudgeEduaction.NoofMale += 1;

                                    }
                                    else
                                    {
                                        JudgeEduaction.NoofFemale += 1;
                                    }
                                }
                            }

                        }
                        JudgeEduaction.EducationLevelCount = JudgeEduaction.NoofFemale + JudgeEduaction.NoofMale;
                        JudgeByAge.JudgeEduactions.Add(JudgeEduaction);

                    }

                    JudgeByAge.TotalMale += JudgeByAge.JudgeEduactions.Sum(x => x.NoofMale);
                    JudgeByAge.TotalFemale += JudgeByAge.JudgeEduactions.Sum(x => x.NoofFemale);
                    JudgeByAge.TotalEduction += JudgeByAge.JudgeEduactions.Sum(x => x.EducationLevelCount);
                    JudgeByAgeList.Add(JudgeByAge);

                }
            }

            return JudgeByAgeList;
        }
    }
}