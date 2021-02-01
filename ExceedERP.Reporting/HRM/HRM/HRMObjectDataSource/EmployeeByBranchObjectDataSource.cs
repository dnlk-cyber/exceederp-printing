using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeByBranchObjectDataSource : List<EmployeeByBranch>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeByBranch> GetEmployeeByBranch(string branch)
        {
            List<EmployeeByBranch> EmployeeByBranchList = new List<EmployeeByBranch>();
            EmployeeByBranch EmployeeByBranch = new EmployeeByBranch();
            var lookups = db.Lookups.ToList();
            //var barnch = db.Branch.Where(x => x.BranchId.ToString() == branch).FirstOrDefault();
            var place = db.Placements.Where(x => x.isCurrent).ToList();
            if (branch != null)
            {
                place = place.Where(x => x.isCurrent && x.BranchId.ToString() == branch).ToList();
            }
            
                //var employes = db.Person_Employee.Where(x => x.IsActive );
                //var place = db.Placements.Where(x => x.isCurrent && x.BranchId.ToString() == branch).ToList();
                foreach (var e in place)
                {

                    EmployeeByBranch = new EmployeeByBranch();
                    //var placment = db.Placements.Where(x => x.isCurrent && x.BranchId.ToString()==branch).FirstOrDefault();
                    var employes = db.Person_Employee.FirstOrDefault(x => x.IsActive && x.EmployeeId==e.EmployeeId);
                    if (employes != null)
                    {
                        
                            EmployeeByBranch.FullName = employes.FirstName + " " + employes.MiddleName + " " + employes.LastName;
                          //  EmployeeByBranch.Sex = employes.Gender;
                        if (employes.Gender == Gender.MALE)
                        {
                            EmployeeByBranch.Sex = "ወንድ";
                        }
                        else
                        {
                            EmployeeByBranch.Sex = "ሴት";
                        }
                        var Educations =
                                db.Education.Where(x => x.EmployeeId == employes.EmployeeId)
                                    .OrderByDescending(x => x.To)
                                    .FirstOrDefault();
                            if (Educations != null)
                            {
                                var firstOrDefault =
                                    lookups.Where(x => x.LookupId == Educations.EduLevel).FirstOrDefault();
                                if (firstOrDefault != null)
                                    EmployeeByBranch.Education = firstOrDefault.Description;

                                var firstOrDefaults =
                                    lookups.Where(x => x.LookupId == Educations.EduField).FirstOrDefault();
                                if (firstOrDefaults != null)
                                    EmployeeByBranch.Training = firstOrDefaults.Description;
                            }
                            #region
                            //var tarinings =
                            //    db.EmpTrainingEducations.Where(x => x.EmployeeId == e.EmployeeId)
                            //        .OrderByDescending(x => x.To)
                            //        .FirstOrDefault();
                            //if (tarinings != null)
                            //{
                            //    EmployeeByBranch.Training = tarinings.TrainingName;

                            #endregion
                            if (Educations != null) EmployeeByBranch.Institution = Educations.OrganizationName;
                            //  }
                               if(employes.EmployeeId==e.EmployeeId)
                               {

                            //EmployeeByBranch.Type = employes.Status;
                        if(employes.Status == EmployeeStatus.CONTRACT)
                        {
                            EmployeeByBranch.Type = "ጊዜያዊ";
                        }
                        else if(employes.Status == EmployeeStatus.PERMANENT)
                        {
                            EmployeeByBranch.Type = "ቋሚ";
                        }
                        else if (employes.Status == EmployeeStatus.FREELANCER)
                        {
                            EmployeeByBranch.Type = "ነጻ ሰራተኛ";
                        }
                        else if (employes.Status == EmployeeStatus.PROBATION)
                        {
                            EmployeeByBranch.Type = "የሙከራ ጊዜ";
                        }
                        else
                        {
                            EmployeeByBranch.Type = "አይታወቅም";
                        }

                        var jobTitle = lookups.Where(x => x.LookupId == e.LookupId).FirstOrDefault();
                            if (jobTitle != null)
                            {

                                EmployeeByBranch.JobTitle = jobTitle.Description;
                            }
                               }
                            EmployeeByBranchList.Add(EmployeeByBranch);
                        
                    }
                }
            
            return EmployeeByBranchList;

        }
    }
}