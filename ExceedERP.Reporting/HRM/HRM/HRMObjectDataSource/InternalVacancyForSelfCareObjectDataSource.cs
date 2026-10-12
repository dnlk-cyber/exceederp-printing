using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{

    [DataObject]
    public class InternalVacancyForSelfCareObjectDataSource :IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalVacancy> GetInternalVacancy(string vacancyCod, DateTime? from, DateTime? to, bool isActive,string vacancyType)
        {
            List<InternalVacancy> internalVacancies = new List<InternalVacancy>();


            //var structure = db.MainCompanyStructures;
            //var lookups = db.Lookups;
            //var recVacancy = db.RecruitmentVacancy.ToList();
            //var vaccancies = db.VaccancyPlans.ToList();
            //if (!string.IsNullOrEmpty(vacancyCod))
            //{
            //    recVacancy = recVacancy.Where(x => x.MediaVacancyCode == vacancyCod).ToList();
            //}
            //if (from.HasValue)
            //{

            //    vaccancies = vaccancies.Where(x => x.From.Date >= from.Value.Date||x.To>=from.Value).ToList();
            //}
            //if (to.HasValue)
            //{
            //    vaccancies = vaccancies.Where(x => x.To.Date <= to.Value.Date||x.To>to.Value).ToList();
            //}
            //if (isActive)
            //{
            //    vaccancies = vaccancies.Where(x => x.IsActive).ToList();

            //}
            //if (!string.IsNullOrEmpty(vacancyType))
            //{
            //    vaccancies = vaccancies.Where(x => x.RecruSource.ToString()==vacancyType).ToList();

            //}
           
            //foreach (var rec in vaccancies)
            //{
            //    var internalVacancy = new InternalVacancy();
            //    var branch = db.Branch.FirstOrDefault(x => x.BranchId == rec.BranchId);
            //    if (branch != null)
            //    {
            //        internalVacancy.Location = branch.AmharicName;
            //    }
            //    internalVacancy.VacancyCode = rec.VaccancyCode;
            //    if (rec.VacancyType == VacancyType.PERMANENT)
            //    {
            //        internalVacancy.RecruitmentType = VacancyType.PERMANENT.ToString().ToLower();

            //    }
            //    else if (rec.VacancyType != VacancyType.PERMANENT)
            //    {

            //        internalVacancy.RecruitmentType = rec.VacancyType.ToString().ToLower();

            //    }
            //    internalVacancy.Quntity = rec.NoOfVacant;
            //    internalVacancy.Salary = rec.Salary.ToString();
            //    internalVacancy.DeadLine = rec.To.ToString("dd.MM.yyyy")+"( "+rec.ToVacAm+" )";
            //    var firstOrDefau = lookups.FirstOrDefault(x => x.LookupId == rec.JobTitle);
            //    if (firstOrDefau != null)
            //    {

            //        internalVacancy.JobTitle = firstOrDefau.Description;
                            
            //    }
            //    var experienceJobTitle = db.ExperienceJobTitles.FirstOrDefault(x => x.LookupId == rec.JobTitle);
            //    if (experienceJobTitle != null)
            //    {
            //        var experienceJobtitleDetails = db.ExperienceJobTitleDetails
            //                                            .Where(x => x.ExperienceJobTitleId == experienceJobTitle.ExperienceJobTitleId
            //                                            &&( x.ExamType == ExamType.RELEVANT_EXPERIENCE
            //                                            || x.ExamType == ExamType.NON_RELATED_EXPERIENCE
            //                                            || x.ExamType == ExamType.RELATED_EXPERIENCE)).ToList();
            //        foreach (var experienceJobtitleDetail in experienceJobtitleDetails)
            //        {
            //            var firstOrDefault = lookups.FirstOrDefault(x => x.LookupId == experienceJobtitleDetail.LookupId);
            //            if (firstOrDefault != null)
            //            {

            //                internalVacancy.EduandExp += "* " + firstOrDefault.Description + "\n";

            //            }
            //        }

            //    var educationLevels = db.EducationLevelDetails
            //        .Where(x => x.ExperienceJobTitleId == experienceJobTitle.ExperienceJobTitleId).ToList();

            //    foreach (var educationLevel in educationLevels)
            //    {

            //        var firstOrDefault = lookups.Find(educationLevel.LookupId);
            //        if (firstOrDefault != null)
            //        {

            //            internalVacancy.EduandExpRelated += "*  " + firstOrDefault.Description + "  with Experience of  " + educationLevel.Experience.ToString() + " years \n";

            //        }



            //    }



            //}
            //var vacancyDetails = db.VacancyDetails.Where(x => x.VacancyId == rec.VacancyId);
            //foreach (var educationLevel in vacancyDetails)
            //{

            //    var firstOrDefault = lookups.Find(educationLevel.LookupId);
            //    if (firstOrDefault != null)
            //    {

            //        internalVacancy.EduandExpRelated += "*  " + firstOrDefault.Description +"  with Experience of " +educationLevel.Experience.ToString() + " years\n";

            //    }


            //}



            //    internalVacancies.Add(internalVacancy);
              

            //}
            

          

            return internalVacancies;


        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
