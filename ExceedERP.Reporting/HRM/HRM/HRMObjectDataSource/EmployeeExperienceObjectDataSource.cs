using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeExperienceObjectDataSource : List<EmployeeExperience>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeExperience> GetEmployeeExperience(string dept, string branchId,string businessUnitId, string id, string type, string fromE, string toE, string status)
        {
            List<Person_Employee> employees = db.Person_Employee.ToList();
            var placements = db.Placements.ToList();
            if(branchId != null)
            {
                int.TryParse(branchId, out int braId);
               var placeEmpIds = EmployeeHelper.GetEmployees_ByBranch_FromPlacement(placements, braId).Select(x=>x.EmployeeId).ToList();
                employees = employees.Where(x=>placeEmpIds.Contains(x.EmployeeId)).ToList();
            }
            if (businessUnitId != null)
            {
                int.TryParse(businessUnitId, out int bussUnitId);
               var placeEmpIds = placements.Where(x=>x.BusinessUnitId == bussUnitId).Select(x=>x.EmployeeId).ToList();
                employees = employees.Where(x=>placeEmpIds.Contains(x.EmployeeId)).ToList();
            }
            if(dept != null)
            {
                int.TryParse(dept, out int orgId);
                var placeEmpIds = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements, orgId).Select(x => x.EmployeeId).ToList();
                employees = employees.Where(x => placeEmpIds.Contains(x.EmployeeId)).ToList();
            }
            if(id != null)
            {
                int.TryParse(id, out int empId);
                employees = employees.Where(x=>x.EmployeeId == empId).ToList();
            }            

            if(status == "Active")
            {
                employees = employees.Where(x => x.IsActive).ToList();
            }
            else if (status == "InActive")
            {
                employees = employees.Where(x => !x.IsActive).ToList();
            }

            if (type == "External")
            {
                return GetExternalExperience(employees);
            }
            else if (type == "Internal")
            {
                return GetInternalExperience(employees);
            }
            else
            {
                return GetBothExperience(employees);
            }
        }
    
        
        public  List<EmployeeExperience> GetExternalExperience(List<Person_Employee> employees)
        {
            List<EmployeeExperience> experienceList = new List<EmployeeExperience>();

            foreach (var emp in employees)
            {
                var experience = new EmployeeExperience
                {
                    EmployeeExperienceList = new List<EmployeeExperienceList>(),
                    EmployeeEducationList = new List<EmployeeEducationList>(),

                    EmployeeId = emp.EmployeeCode,
                    FullName = emp.FullNameaEnglish,
                    Sex = emp.Gender.ToString()
                };

                var department = db.Placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.isCurrent);
                if (department != null)
                {
                    var org = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == department.OrgStructureId);
                    if (org != null)
                    {
                        experience.Departement = org.Name;
                    }
                }

                var experiences = db.Experiences.Where(x => x.EmployeeId == emp.EmployeeId).ToList();

                double totalExperience = 0;

                foreach (var exp in experiences)
                {
                    var extExp = new EmployeeExperienceList();

                    extExp.From = XAPI.EthiopicDateTime.GetEthiopicDate(exp.From.Day, exp.From.Month, exp.From.Year);
                    extExp.To = XAPI.EthiopicDateTime.GetEthiopicDate(exp.To.Day, exp.To.Month, exp.To.Year);
                    extExp.Responsibility = exp.Responsibility;
                    extExp.Totalyear = EmployeeHelper.GetDaystoWordsAmharic(Convert.ToDouble(exp.To.Date.Subtract(exp.From.Date).Days));

                    totalExperience = totalExperience + (exp.To - exp.From).Days;

                    experience.EmployeeExperienceList.Add(extExp);
                }

                experience.TotalExp = EmployeeHelper.GetDaystoWordsAmharic(totalExperience);

                var educations = db.Education.Where(x => x.EmployeeId == emp.EmployeeId).ToList();
                foreach (var edu in educations)
                {
                    var education = new EmployeeEducationList();

                    if(edu.From != null)
                    {
                        education.From = XAPI.EthiopicDateTime.GetEthiopicDate(edu.From.Value.Day, edu.From.Value.Month, edu.From.Value.Year);
                    }
                    if(edu.To != null)
                    {
                        education.To = XAPI.EthiopicDateTime.GetEthiopicDate(edu.To.Day, edu.To.Month, edu.To.Year);
                    }                  

                    var level = db.Lookups.FirstOrDefault(x => x.LookupId == edu.EduLevel);
                    if (level != null)
                    {
                        education.Level = level.Description;
                    }

                    var field = db.Lookups.FirstOrDefault(x => x.LookupId == edu.EduField);
                    if (field != null)
                    {
                        education.Department = field.Description;
                    }

                    experience.EmployeeEducationList.Add(education);
                }
                experienceList.Add(experience);
            }

            return experienceList;
        }

        public  List<EmployeeExperience> GetInternalExperience(List<Person_Employee> employees)
        {
            List<EmployeeExperience> experienceList = new List<EmployeeExperience>();


            foreach (var emp in employees)
            {
                var experience = new EmployeeExperience();
                experience.EmployeeExperienceList = new List<EmployeeExperienceList>();

                experience.EmployeeId = emp.EmployeeCode;
                experience.FullName = emp.FullNameaEnglish;
                experience.Sex = emp.Gender.ToString();

                var department = db.Placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.isCurrent);
                if (department != null)
                {
                    var org = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == department.OrgStructureId);
                    if (org != null)
                    {
                        experience.Departement = org.Name;
                    }
                }

                // this placement history is the experience before they begin using the system
                var placementHistories = db.PlacementHistories.Where(x => x.EmployeeId == emp.EmployeeId).ToList();

                double totalExperience = 0;

                foreach (var place in placementHistories)
                {
                    var intExp = new EmployeeExperienceList();
                    intExp.Responsibility = place.JobTitle;
                    var jobtitle = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId );
                    if (jobtitle != null)
                    {
                        intExp.Responsibility = jobtitle.Description;

                    }
                    intExp.From = XAPI.EthiopicDateTime.GetEthiopicDate(place.From.Day, place.From.Month, place.From.Year);
                    intExp.To = XAPI.EthiopicDateTime.GetEthiopicDate(place.To.Day, place.To.Month, place.To.Year);
                    intExp.Totalyear = EmployeeHelper.GetDaystoWordsAmharic(Convert.ToDouble(place.To.Date.Subtract(place.From.Date).Days));

                    totalExperience = totalExperience + (place.To - place.From).Days;

                    experience.EmployeeExperienceList.Add(intExp);
                }

                //this is after they begin using the system
                var placements = db.Placements.Where(x => x.EmployeeId == emp.EmployeeId).OrderBy(x => x.AssignmentDate).ToList();

                for (int i = 0; i < placements.Count(); i++)
                {
                    var intExp = new EmployeeExperienceList();

                    // this date is used for calculate experience
                    DateTime from = DateTime.Now;
                    DateTime to = DateTime.Now;

                    if (placements[i].isCurrent)
                    {
                        intExp.From = XAPI.EthiopicDateTime.GetEthiopicDate(placements[i].AssignmentDate.Day, placements[i].AssignmentDate.Month, placements[i].AssignmentDate.Year);
                        intExp.To = "Present";

                        from = placements[i].AssignmentDate;
                        to = DateTime.Now;
                    }
                    else if (i < placements.Count - 1)
                    {
                        intExp.From = XAPI.EthiopicDateTime.GetEthiopicDate(placements[i].AssignmentDate.Day, placements[i].AssignmentDate.Month, placements[i].AssignmentDate.Year);
                        intExp.To = XAPI.EthiopicDateTime.GetEthiopicDate(placements[i + 1].AssignmentDate.Day, placements[i + 1].AssignmentDate.Month, placements[i + 1].AssignmentDate.Year);

                        from = placements[i].AssignmentDate;
                        to = placements[i + 1].AssignmentDate;
                    }
                    int lookUpId = placements[i].LookupId;
                    var job = db.Lookups.FirstOrDefault(x => x.LookupId == lookUpId && x.LookupType == LookUpTypes.JOB_TITLE);
                    if (job != null)
                    {
                        intExp.Responsibility = job.Description;
                    }

                    intExp.Totalyear = EmployeeHelper.GetDaystoWordsAmharic(Convert.ToDouble(to.Subtract(from).Days));

                    totalExperience = totalExperience + (to - from).Days;                    

                    experience.EmployeeExperienceList.Add(intExp);
                }

                experience.TotalExp = EmployeeHelper.GetDaystoWordsAmharic(totalExperience);

                var educations = db.Education.Where(x => x.EmployeeId == emp.EmployeeId).ToList();
                foreach (var edu in educations)
                {
                    var education = new EmployeeEducationList();

                   if(edu.From != null)
                    {
                        education.From = XAPI.EthiopicDateTime.GetEthiopicDate(edu.From.Value.Day, edu.From.Value.Month, edu.From.Value.Year);
                    }
                    if(edu.To != null)
                    {
                        education.To = XAPI.EthiopicDateTime.GetEthiopicDate(edu.To.Day, edu.To.Month, edu.To.Year);
                    } 

                    var level = db.Lookups.FirstOrDefault(x => x.LookupId == edu.EduLevel);
                    if (level != null)
                    {
                        education.Level = level.Description;
                    }

                    var field = db.Lookups.FirstOrDefault(x => x.LookupId == edu.EduField);
                    if (field != null)
                    {
                        education.Department = field.Description;
                    }

                    experience.EmployeeEducationList.Add(education);
                }

                experienceList.Add(experience);
            }

            return experienceList;
        }

        public  List<EmployeeExperience> GetBothExperience(List<Person_Employee> employees)
        {
            List<EmployeeExperience> experienceList = new List<EmployeeExperience>();


            foreach (var emp in employees)
            {
                var experience = new EmployeeExperience();
                experience.EmployeeExperienceList = new List<EmployeeExperienceList>();

                experience.EmployeeId = emp.EmployeeCode;
                experience.FullName = emp.FullNameaEnglish;
                experience.Sex = emp.Gender.ToString();

                var department = db.Placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.isCurrent);
                if (department != null)
                {
                    var org = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == department.OrgStructureId);
                    if (org != null)
                    {
                        experience.Departement = org.Name;
                    }
                }

                // External Experience
                var experiences = db.Experiences.Where(x => x.EmployeeId == emp.EmployeeId).ToList();

                foreach (var exp in experiences)
                {
                    var extExp = new EmployeeExperienceList();

                    extExp.From = XAPI.EthiopicDateTime.GetEthiopicDate(exp.From.Day, exp.From.Month, exp.From.Year);
                    extExp.To = XAPI.EthiopicDateTime.GetEthiopicDate(exp.To.Day, exp.To.Month, exp.To.Year);
                    extExp.Responsibility = exp.Responsibility;
                    extExp.Totalyear = EmployeeHelper.GetDaystoWordsAmharic(Convert.ToDouble(exp.To.Date.Subtract(exp.From.Date).Days));

                    experience.EmployeeExperienceList.Add(extExp);
                }


                // this placement history is the experience before they begin using the system
                var placementHistories = db.PlacementHistories.Where(x => x.EmployeeId == emp.EmployeeId).ToList();

                foreach (var place in placementHistories)
                {

                    var intExp = new EmployeeExperienceList();
                    intExp.Responsibility = place.JobTitle;
                    var jobtitle = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                    if(jobtitle != null)
                    {
                        intExp.Responsibility = jobtitle.Description;

                    }

                    intExp.From = XAPI.EthiopicDateTime.GetEthiopicDate(place.From.Day, place.From.Month, place.From.Year);
                    intExp.To = XAPI.EthiopicDateTime.GetEthiopicDate(place.To.Day, place.To.Month, place.To.Year);
                    intExp.Totalyear = EmployeeHelper.GetDaystoWordsAmharic(Convert.ToDouble(place.To.Date.Subtract(place.From.Date).Days));

                    experience.EmployeeExperienceList.Add(intExp);
                }

                //this is after they begin using the system
                var placements = db.Placements.Where(x => x.EmployeeId == emp.EmployeeId).OrderBy(x => x.AssignmentDate).ToList();

                for (int i = 0; i < placements.Count(); i++)
                {
                    var intExp = new EmployeeExperienceList();

                    // this date is used for calculate experience
                    DateTime from = DateTime.Now;
                    DateTime to = DateTime.Now;

                    if (placements[i].isCurrent)
                    {
                        intExp.From = XAPI.EthiopicDateTime.GetEthiopicDate(placements[i].AssignmentDate.Day, placements[i].AssignmentDate.Month, placements[i].AssignmentDate.Year);
                        intExp.To = "Present";

                        from = placements[i].AssignmentDate;
                        to = DateTime.Now;
                    }
                    else if (i < placements.Count - 1)
                    {
                        intExp.From = XAPI.EthiopicDateTime.GetEthiopicDate(placements[i].AssignmentDate.Day, placements[i].AssignmentDate.Month, placements[i].AssignmentDate.Year);
                        intExp.To = XAPI.EthiopicDateTime.GetEthiopicDate(placements[i + 1].AssignmentDate.Day, placements[i + 1].AssignmentDate.Month, placements[i + 1].AssignmentDate.Year);

                        from = placements[i].AssignmentDate;
                        to = placements[i + 1].AssignmentDate;
                    }
                    int lookUpId = placements[i].LookupId;
                    var job = db.Lookups.FirstOrDefault(x => x.LookupId == lookUpId && x.LookupType == LookUpTypes.JOB_TITLE);
                    if (job != null)
                    {
                        intExp.Responsibility = job.Description;
                    }

                    intExp.Totalyear = EmployeeHelper.GetDaystoWordsAmharic(Convert.ToDouble(to.Subtract(from).Days));

                    experience.EmployeeExperienceList.Add(intExp);
                }

                var educations = db.Education.Where(x => x.EmployeeId == emp.EmployeeId).ToList();
                foreach (var edu in educations)
                {
                    var education = new EmployeeEducationList();

                    if (edu.From != null)
                    {
                        education.From = XAPI.EthiopicDateTime.GetEthiopicDate(edu.From.Value.Day, edu.From.Value.Month, edu.From.Value.Year);
                    }
                    if (edu.To != null)
                    {
                        education.To = XAPI.EthiopicDateTime.GetEthiopicDate(edu.To.Day, edu.To.Month, edu.To.Year);
                    }

                    var level = db.Lookups.FirstOrDefault(x => x.LookupId == edu.EduLevel);
                    if (level != null)
                    {
                        education.Level = level.Description;
                    }

                    var field = db.Lookups.FirstOrDefault(x => x.LookupId == edu.EduField);
                    if (field != null)
                    {
                        education.Department = field.Description;
                    }

                    experience.EmployeeEducationList.Add(education);
                }

                experienceList.Add(experience);
            }

            return experienceList;
        }

    }
}