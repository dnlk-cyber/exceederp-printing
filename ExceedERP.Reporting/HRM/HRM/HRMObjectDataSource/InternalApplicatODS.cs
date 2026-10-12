using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class InternalApplicatODS:IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalApplicatVM> GetInternalApplicat(string id)
        {
            var screenedVacancId = db.InternalApplicantScreeningResults.Select(x => x.VacancyId).Distinct();
            var applicants = db.ApplicantionFormes.ToList();
            var vacancies = db.VaccancyPlans.Where(x=> screenedVacancId.Contains(x.VacancyId)).ToList();
            List<InternalApplicatVM>internalApplicats = new List<InternalApplicatVM>();
            //if (!string.IsNullOrEmpty(id))
            //{
            //    int.TryParse(id, out int vacancyId);
            //     applicants = applicants.Where(x => x.VacancyId == vacancyId).ToList();
            //    vacancies = vacancies.Where(x => x.VacancyId == vacancyId).ToList();

            //}
            //HashSet<int> vacacyIds = new HashSet<int>(applicants.Select(x => x.VacancyId));
            //vacancies = vacancies.Where(x =>vacacyIds.Contains(x.VacancyId)).ToList();


            //foreach (var vacancy in vacancies)
            //{
            //    var internalApplicat = new InternalApplicatVM();

            //    internalApplicat.VacancyCode = vacancy.VaccancyCode;
            //    internalApplicat.VacancyType = vacancy.VacancyType.ToString();
            //    internalApplicat.Gender = vacancy.Gender.ToString();
            //    internalApplicat.ExpeienceNeeded = vacancy.TotalYearOfExperience;
            //    var expereiceJobTitle = db.ExperienceJobTitles
            //                      .FirstOrDefault(x => x.LookupId == vacancy.JobTitle);
            //    if (expereiceJobTitle == null)
            //    {
            //        continue;
            //    }
            //    var experienceJobTitleDetails = db.ExperienceJobTitleDetails
            //                       .Where(x => x.ExperienceJobTitleId == expereiceJobTitle.ExperienceJobTitleId)
            //                       .ToList();
            //    var jobtitleIds = experienceJobTitleDetails
            //                     .Select(x => x.LookupId).Distinct()
            //                     .ToList();
            //    var relatedJobtitleIds = experienceJobTitleDetails
            //                    .Where(x => x.ExamType == ExamType.RELATED_EXPERIENCE)
            //                    .Select(x => x.LookupId).Distinct()
            //                    .ToList();
            //    var nonRelatedjobtitleIds = experienceJobTitleDetails
            //                     .Where(x => x.ExamType == ExamType.NON_RELATED_EXPERIENCE)
            //                     .Select(x => x.LookupId).Distinct().ToList();

            //    var relevantJobtitleIds = db.ExperienceJobTitleDetails
            //                     .Where(x => x.ExamType == ExamType.RELEVANT_EXPERIENCE)
            //                     .Select(x => x.LookupId).Distinct()
            //                     .ToList();
            //    var levelIds = db.EducationLevelDetails
            //                     .Where(x => x.ExperienceJobTitleId == expereiceJobTitle.ExperienceJobTitleId)
            //                     .Select(x => x.LookupId).Distinct()
            //                     .ToList();
            //    var FieldIds = db.EducationFieldDetails
            //                     .Where(x => x.ExperienceJobTitleId == expereiceJobTitle.ExperienceJobTitleId)
            //                     .Select(x => x.LookupId).Distinct()
            //                     .ToList();
            //    var relatededuFiledIds = db.RelatedEducationFieldDetails
            //                                  .Where(x => x.ExperienceJobTitleId == expereiceJobTitle.ExperienceJobTitleId)
            //                                  .Select(x => x.LookupId)
            //                                  .ToList();
            //    internalApplicat.internalApplicatDetails = new List<InternalApplicatDetailVM>();
            //    var applicantsPerVancies = applicants.Where(x => x.VacancyId == vacancy.VacancyId).ToList();
            //    foreach (var applicant in applicantsPerVancies)
            //    {
            //        var internalApplicatDetails = new InternalApplicatDetailVM();
            //        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == applicant.EmployeeId);
            //        if (person != null)
            //        {
            //            internalApplicatDetails.ApplicantFullName =$"{person.EmployeeCode}-{person.FirstNameEnglish} {person.MiddleNameEnglish} {person.LastNameEnglish}" ;
            //        }
                   
            //        if (expereiceJobTitle != null)
            //        {
                       
            //            decimal relevantex = 0;
            //            decimal relatedex = 0;
            //            decimal nonRelatedex = 0;
            //            decimal curretExp = 0;
            //            decimal totalExp = 0;
            //            decimal totalYear = 0;
            //            double sumOfExp = 0;
            //            var person1 = db.Person_Employee.Find(applicant.EmployeeId);
            //            if (person != null)
            //            {
            //                sumOfExp = DateTime.Now.Subtract(person.HiredDate).TotalDays;
            //                var empExps = db.Experiences.Where(x => x.EmployeeId == person.EmployeeId).ToList();
            //                foreach (var ex in empExps)
            //                {
            //                    sumOfExp = sumOfExp + ex.To.Subtract(ex.From).TotalDays;
            //                }


            //                var appEducations = db.Education.Where(x => x.EmployeeId == applicant.EmployeeId
            //                                               && levelIds.Contains(x.EduLevel))
            //                                               .ToList();
            //                var experiences = db.InternalRecruitmentCriteriaTypeSettings
            //                    .Where(x => x.ExamType == ExamType.RELEVANT_EXPERIENCE
            //                                || x.ExamType == ExamType.RELATED_EXPERIENCE
            //                                || x.ExamType == ExamType.NON_RELATED_EXPERIENCE).ToList();
            //                foreach (var experience in experiences)
            //                {
            //                    var placement = db.Placements.Where(x => x.isCurrent
            //                                                                && x.EmployeeId == applicant.EmployeeId
            //                                                                && (relatedJobtitleIds.Contains(x.LookupId)
            //                                                                || nonRelatedjobtitleIds.Contains(x.LookupId)
            //                                                                || relevantJobtitleIds.Contains(x.LookupId))).ToList();



            //                    if (experience.ExamType == ExamType.RELATED_EXPERIENCE)
            //                    {
            //                        var placement1 = placement
            //                                        .Where(x =>x.isCurrent&& relatedJobtitleIds.Contains(x.LookupId))
            //                                            .FirstOrDefault();

            //                        if (placement1 != null)
            //                        {
            //                            curretExp += curretExp + (decimal)DateTime.Now.Subtract(placement1.AssignmentDate).TotalDays;
            //                            var related = db.Lookups.Find(placement1.LookupId);
            //                            if (related != null)
            //                            {
            //                                internalApplicatDetails.RelatedJobTitles += $"* {related.Description} \n";
            //                            }
            //                        }
                                  

            //                        var empExpss = db.PlacementHistories.Where(x => x.EmployeeId == applicant.EmployeeId
            //                                                && relatedJobtitleIds.Contains(x.LookupId))
            //                                                .ToList();

            //                        var empExpBeforeSystem = db.PlacementAfterSystems.Where(x => x.EmployeeId == applicant.EmployeeId
            //                                                && relatedJobtitleIds.Contains(x.LookupId)).ToList();
            //                        var empExternalExps = db.Experiences
            //                           .Where(x => x.LookupId != null
            //                                       && x.EmployeeId == applicant.EmployeeId
            //                                       && relatedJobtitleIds.Contains((int)x.LookupId));

            //                        foreach (var ex in empExpss)
            //                        {
            //                            relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) ;
            //                            var related = db.Lookups.Find(ex.LookupId);
            //                            if (related != null)
            //                            {
            //                                internalApplicatDetails.RelatedJobTitles += $"* {related.Description} \n";
            //                            }
            //                            else
            //                            {
            //                                internalApplicatDetails.RelatedJobTitles += $"* {ex.JobTitle} \n";

            //                            }
            //                        }
            //                        foreach (var ex in empExpBeforeSystem)
            //                        {
            //                            relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) ;
            //                            var related = db.Lookups.Find(ex.LookupId);
            //                            if (related != null)
            //                            {
            //                                internalApplicatDetails.RelatedJobTitles += $"* {related.Description} \n";
            //                            }
            //                        }
            //                        foreach (var ex in empExternalExps)
            //                        {
            //                            relatedex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1);
            //                            var related = db.Lookups.Find(ex.LookupId);
            //                            if ( related !=null)
            //                            {
            //                                internalApplicatDetails.RelatedJobTitles += $"* {related.Description} \n";
            //                            }
            //                            else
            //                            {
            //                                internalApplicatDetails.RelatedJobTitles += $"* {ex.Responsibility} \n";

            //                            }

            //                        }
            //                        internalApplicatDetails.RelatedRate = experience.Rate;
            //                        internalApplicatDetails.TotalRelatedExperience = Math.Round(relatedex * experience.Rate / 365.25m,2);

            //                    }
            //                    if (experience.ExamType == ExamType.NON_RELATED_EXPERIENCE)
            //                    {
            //                        nonRelatedex += (decimal)sumOfExp * experience.Rate;

            //                        internalApplicatDetails.NonRelaetedRates = experience.Rate.ToString();

            //                        internalApplicatDetails.TotalNonRelaetedExperience =Math.Round( nonRelatedex / 365.25m,2);
            //                        internalApplicatDetails.NonRelaetedJobTitle = $"this is rate multiplied by Total expreince {(decimal)Math.Round((decimal)sumOfExp / 365.25m, 2)}";

            //                    }
            //                    if (experience.ExamType == ExamType.RELEVANT_EXPERIENCE)
            //                    {
            //                        var placement1 = placement.Where(x => x.isCurrent && relevantJobtitleIds.Contains(x.LookupId)).FirstOrDefault();

            //                        if (placement1 != null)
            //                        {
            //                            curretExp += curretExp + (decimal)DateTime.Now.Subtract(placement1.AssignmentDate).TotalDays;
            //                            var relevant = db.Lookups.Find(placement1.LookupId);
            //                            if (relevant != null)
            //                                internalApplicatDetails.RelavantJobTitles += $"* {relevant.Description} \n";
                                       
            //                        }
                                   
            //                        var empExpss = db.PlacementHistories.Where(x => x.EmployeeId == applicant.EmployeeId
            //                                                                    && relevantJobtitleIds.Contains(x.LookupId));
            //                        foreach (var ex in empExpss)
            //                        {
            //                            relevantex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) ;
            //                            var relevant = db.Lookups.Find(ex.LookupId);
            //                            if (relevant != null)
            //                                internalApplicatDetails.RelavantJobTitles += $"* {relevant.Description} \n";
            //                            else
            //                            {
            //                                internalApplicatDetails.RelatedJobTitles += $"* {ex.JobTitle} \n";

            //                            }
            //                        }
            //                        var empExpAfterSystems = db.PlacementAfterSystems.Where(x => x.EmployeeId == applicant.EmployeeId
            //                                                                    && relevantJobtitleIds.Contains(x.LookupId));
            //                        foreach (var ex in empExpAfterSystems)
            //                        {
            //                            relevantex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1);
            //                            var relevant = db.Lookups.Find(ex.LookupId);
            //                            if (relevant != null)
            //                                internalApplicatDetails.RelavantJobTitles += $"* {relevant.Description} or \n";

            //                        }
            //                        var empExternalExps = db.Experiences
            //                            .Where(x => x.LookupId != null
            //                                        && x.EmployeeId == applicant.EmployeeId
            //                                        && relevantJobtitleIds.Contains((int)x.LookupId));
            //                        foreach (var ex in empExternalExps)
            //                        {
            //                            relevantex += ((decimal)ex.To.Subtract(ex.From).TotalDays + 1) ;
            //                            var relevant = db.Lookups.Find(ex.LookupId);
            //                            if (relevant != null)
            //                                internalApplicatDetails.RelavantJobTitles += $"* {relevant.Description} \n";
            //                            else
            //                            {
            //                                internalApplicatDetails.RelatedJobTitles += $"* {ex.Responsibility} \n";

            //                            }
            //                        }

            //                        internalApplicatDetails.RelavantRate = experience.Rate;

            //                        internalApplicatDetails.TotalRelavanceExperience = Math.Round(((relevantex + curretExp)* experience.Rate) / 365.25m,2);

            //                    }


            //                }


            //                totalExp = relatedex + nonRelatedex + relevantex + curretExp;
            //                totalYear = (decimal)((double)totalExp / 365.25)/*+(decimal)expYear*/;
            //                internalApplicatDetails.TotalExperience = totalYear;

            //                internalApplicat.internalApplicatDetails.Add(internalApplicatDetails);

            //            }
            //        }




            //    }
            //    internalApplicats.Add(internalApplicat);
            //}
            return internalApplicats;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Vacancy> GetVacancyCode()
        {
            return db.VaccancyPlans
             .Where(x => x.IsActive &&
                         (x.RecruSource == RecruitmentSource.INTERNAL ||
                          x.RecruSource == RecruitmentSource.INTERNALPROMOTION ||
                          x.RecruSource == RecruitmentSource.INTERNALTRANSFER))
             //.Select(x => new Vacancy
             //{
             //    VacancyId = x.VacancyId,
             //    VaccancyCode = x.VaccancyCode
             //})
             .ToList();
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}
