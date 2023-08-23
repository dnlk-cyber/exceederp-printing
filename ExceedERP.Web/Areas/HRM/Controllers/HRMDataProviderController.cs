using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Web.Areas.HRM.Controllers.Company;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using System.Net;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.Helpers.Finance;
using ExceedERP.Core.Domain.HRM.Payroll.Vm;

using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.HRM.ArchiveManagement;

namespace ExceedERP.Web.Areas.HRM.Controllers
{

    //new
    public class HRMDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //new
        private HRMInputs inputs = new HRMInputs();

        public JsonResult GetAllEmployeesForSelfCare(string text)
        {
            List<EmployeeVm> list = new List<EmployeeVm>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var employees = db.Person_Employee
                    .Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
                        x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                          x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                            x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                             (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text.ToUpper()) ||
                              (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper())
                    )
                    .Select(x => new EmployeeVm
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeCode = x.EmployeeCode,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        FirstNameEnglish = x.FirstNameEnglish,
                        MiddleNameEnglish = x.MiddleNameEnglish,
                        LastNameEnglish = x.LastNameEnglish
                    }).ToList();

                list = employees;
            }

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + " - " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
                FullNameAmharic = p.EmployeeCode + " - " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeTypes()
        {
            var data = db.EmployeeTypes.ToList();
            return Json(data.Select(p => new
            {
                p.EmployeeTypeId,
                p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEvaluationCriterias(string level)
        {
            List<MeasurementDefinition> measurments = db.MeasurementDefinitions.ToList();

            if (level == "Global" || level == "1")
            {
                measurments = db.MeasurementDefinitions.Where(x => !x.IsDepartmentLevel).ToList();
            }
            else if (level == "Department" || level == "2")
            {
                measurments = db.MeasurementDefinitions.Where(x => x.IsDepartmentLevel).ToList();
            }

            return Json(measurments.Select(p => new
            {
                MeasurementDefinitionId = p.MeasurementDefinitionId,
                Name = p.EvaluationCriteria + " - " + GetEvaluationTypeName(p.LookupId) + " - " + p.Role.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        public string GetEvaluationTypeName(int lookupId)
        {
            var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == lookupId);
            if (lookup != null)
            {
                return lookup.Description;
            }

            return "";
        }

        public JsonResult GetOldandNewJobTitles()
        {
            var oldJobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.OLD_JOB_TITLE || x.LookupType == LookUpTypes.JOB_TITLE).ToList();

            return Json(oldJobTitle.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriod()
        {

            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            var list = from p in periodList
                       join g in db.GLPeriods
                           on p.Name equals g.Name
                       select (new
                       {
                           g
                       });

            return Json(list.Select(p => new
            {
                GLPeriodId = p.g.GLPeriodId,
                Name = p.g.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriodOther()
        {

            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            var listt = from p in periodList
                        join g in db.GLPeriods
                            on p.Name equals g.Name
                        select (new
                        {
                            g
                        });
            // var fisicalyear = listt.Select(x => x.g).ToList(); ;

            return Json(listt.Select(x => x.g), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetAllEmployeesForPlacement(string text)
        {
            List<EmployeeVm> list = new List<EmployeeVm>();

            if (!String.IsNullOrWhiteSpace(text))
            {
                var employees = db.Person_Employee
                    .Where(x => x.EmployeeCode.ToUpper().Contains(text.ToUpper()) ||
                        x.FirstNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                          x.MiddleNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                            x.LastNameEnglish.ToUpper().Contains(text.ToUpper()) ||
                             (x.FirstName + " " + x.MiddleName + " " + x.LastName).ToUpper().Contains(text.ToUpper()) ||
                              (x.FirstNameEnglish + " " + x.MiddleNameEnglish + " " + x.LastNameEnglish).ToUpper().Contains(text.ToUpper())
                    )
                    .Select(x => new EmployeeVm
                    {
                        EmployeeId = x.EmployeeId,
                        EmployeeCode = x.EmployeeCode,
                        FirstName = x.FirstName,
                        MiddleName = x.MiddleName,
                        LastName = x.LastName,
                        FirstNameEnglish = x.FirstNameEnglish,
                        MiddleNameEnglish = x.MiddleNameEnglish,
                        LastNameEnglish = x.LastNameEnglish
                    }).ToList();

                list = employees;
            }

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + " - " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
                FullNameAmharic = p.EmployeeCode + " - " + p.FirstName + " " + p.MiddleName + " " + p.LastName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFisicalYear()
        {
            var periodList = db.GLFiscalYears.ToList();
            var fisicalYear = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCurrentPeriods()
        {
            List<GLPeriod> glPeriods = new List<GLPeriod>();
            var year = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom <= DateTime.Today && x.DateTo >= DateTime.Today);
            if (year != null)
            {
                var periods = db.GLPeriods.Where(x => x.GlFiscalYearId == year.GlFiscalYearId).ToList();
                glPeriods.AddRange(periods);
            }

            return Json(glPeriods.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPeriods(string GlFiscalYearId)
        {
            List<GLPeriod> periodList = new List<GLPeriod>();
            var periods = db.GLPeriods.Where(x => x.GlFiscalYearId.ToString() == GlFiscalYearId).ToList();

            if (periods.Any())
            {
                periodList.AddRange(periods);
            }
            return Json(periodList.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetGlPeriod()
        {
            var periodList = db.GLPeriods.OrderByDescending(x => x.GLPeriodId).ToList();
            //var fisicalYear = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetGlPeriodFromOpenBudgetYear()
        {

            var periodList = new List<GLPeriod>();//db.GLPeriods.OrderByDescending(x => x.GLPeriodId).ToList();
            var year = db.GLFiscalYears.Where(x => x.Status == FiscalYearStatus.Open).Select(x => x.GlFiscalYearId);

            if (year.Any())
            {
                var periods = db.GLPeriods.Where(x => year.Contains(x.GlFiscalYearId)).OrderByDescending(x => x.GLPeriodId).ToList();
                periodList.AddRange(periods);
            }

            return Json(periodList.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCurrentYearPeriods()
        {
            var periodList = db.GLPeriods.ToList();
            var year = db.GLFiscalYears.FirstOrDefault(x => x.DateFrom.Year == DateTime.Now.Year);
            if (year != null)
            {
                periodList = periodList.Where(x => x.GlFiscalYearId == year.GlFiscalYearId).ToList();
            }

            //var fisicalYear = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(periodList.Select(p => new
            {
                GLPeriodId = p.GLPeriodId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetSelectedPeriod()
        {
            var periodList = db.GLFiscalYears.AsQueryable();
            var selectedperiod = periodList != null ? periodList.ToList() : new List<GlFiscalYear>();

            return Json(selectedperiod.Select(p => new
            {
                GlFiscalYearId = p.GlFiscalYearId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetTrainingProvider()
        {

            var providerlist = db.Companies.AsQueryable();
            if (providerlist.Any())
            {
                providerlist = providerlist.Where(c => c.Category == "1" && c.IsActive).AsQueryable();
            }

            return Json(providerlist.Select(p => new
            {
                MainCompanyId = p.MainCompanyId,
                TradeName = p.TradeName
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetTrainingTypeList()
        {
            var traTypeList = db.Lookups.AsQueryable();
            if (traTypeList.Any())
            {
                traTypeList = traTypeList.Where(l => l.LookupType == LookUpTypes.TRAINING_TYPE).OrderBy(o => o.LookupType);

            }
            return Json(traTypeList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCourseList()
        {
            var courseList = db.Lookups.AsQueryable();

            if (courseList.Any())
            {
                courseList = courseList.Where(l => l.LookupType == LookUpTypes.COURSE && l.IsActive).AsQueryable();

            }
            var courses = courseList != null ? courseList.ToList() : new List<Lookup>();
            return Json(courses.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetEducationLevel()
        {
            var eduLevel = db.Lookups.AsQueryable();

            if (eduLevel.Any())
            {
                eduLevel = eduLevel.Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL && x.IsActive).AsQueryable();

            }
            var level = eduLevel != null ? eduLevel.ToList() : new List<Lookup>();
            return Json(level.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetProject()
        {
            var types = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE).ToList();

            return Json(types.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProbationType()
        {
            var types = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROBATION_STATUS).ToList();

            return Json(types.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRequestStatus()
        {
            var requests = Enum.GetValues(typeof(RequestStatus))
                         .Cast<RequestStatus>()
                         .Select(v => v).Where(item => item != RequestStatus.DEPARTMENT_APPROVAL && item != RequestStatus.PEND)
                         .ToList();
            //foreach (var kk in requests)
            //{
            //    int ii = (int)kk;
            //    string ll = kk.ToString();
            //}
            return Json(requests.Select(p => new
            {
                Value = (int)p,
                Text = p.ToString(),
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartments(int BranchId)
        {
            var jobTitleMappings = db.JobTitleMapping.Where(x => x.BranchId == BranchId).ToList();
            if (jobTitleMappings.Any())
            {
                var orgIds = jobTitleMappings.Select(x => x.OrgStructureId).ToList();
                if (orgIds.Any())
                {
                    var structures = db.MainCompanyStructures.Where(x => orgIds.Contains(x.OrgStructureId)).AsQueryable();
                    return Json(structures.Select(p => new
                    {
                        OrgStructureId = p.OrgStructureId,
                        Name = p.Name

                    }), JsonRequestBehavior.AllowGet);
                }
            }

            return null;
        }

        public JsonResult GetDepartmentsForPlacement()
        {
            var structures = db.MainCompanyStructures.AsQueryable();
            return Json(structures.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetDepartmentsToMakeParent()
        {
            var structures = db.MainCompanyStructures.AsQueryable();

            return Json(structures.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitle(int BranchId, int OrgStructureId)
        {
            var manPowerPlannings = db.JobTitleMapping.Where(x => x.BranchId == BranchId && x.OrgStructureId == OrgStructureId).ToList();

            if (manPowerPlannings.Any())
            {
                var jobIds = manPowerPlannings.Select(x => x.LookupId).ToList();

                var jobTitle = db.Lookups.Where(x => jobIds.Contains(x.LookupId) && x.LookupType == LookUpTypes.JOB_TITLE && x.IsActive).ToList();

                return Json(jobTitle.Select(p => new
                {
                    LookupId = p.LookupId,
                    Description = p.Description,
                }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult GetAllJobTitle()
        {
            var jobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE && x.IsActive).ToList();

            return Json(jobTitle.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetOldJobTitles()
        {
            var oldJobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.OLD_JOB_TITLE && x.IsActive).ToList();

            return Json(oldJobTitle.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleForExternalExperience()
        {
            var externalJobtitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.Jobtitle_External_Experience && x.IsActive).ToList();

            return Json(externalJobtitle.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleForRecruitment(string OrgStructureId, string searchText)
        {
            List<Jobs> jobList = new List<Jobs>();
            //var jobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE && x.IsActive && x.Description==searchText).ToList();
            //var jobTitle = db.RecruitmentPlans.Where(x => x.WorkUnitId.ToString() == OrgStructureId && x.JobtitleName.ToUpper(XAPI.Common.exceedculture).Contains(searchText.ToUpper(XAPI.Common.exceedculture)));

            //var jobTitle = db.RecruitmentPlans.Where(x => x.OrgStructureId.ToString() == OrgStructureId).ToList();

            int.TryParse(OrgStructureId, out int orgStructId);
            var jobTitles = db.JobTitleMapping.Where(x => x.OrgStructureId == orgStructId).ToList();
            foreach (var jj in jobTitles)
            {
                var job = new Jobs();

                job.LookupId = jj.LookupId;
                var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == jj.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                if (lookup != null)
                {
                    job.Description = lookup.Description;
                }
                jobList.Add(job);

            }
            //if (!String.IsNullOrEmpty(searchText))
            //{
            //jobTitle = jobTitle.Where(x =>
            //    x.JobtitleName.ToUpper(XAPI.Common.exceedculture)
            //        .Contains(searchText.ToUpper(XAPI.Common.exceedculture))).ToList();

            //    jobTitles = jobTitles.Where(x =>x.id).ToList();
            //}
            return Json(jobList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWorkUnitFromRecruitmentPlan(string BranchId, string searchText)
        {
            List<MainCompanyStructure> workUnitsList = new List<MainCompanyStructure>();
            var jobTitleMappings = db.JobTitleMapping.Where(x => x.BranchId.ToString() == BranchId).ToList();
            if (!String.IsNullOrEmpty(searchText))
            {
                foreach (var recruit in jobTitleMappings)
                {
                    var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == recruit.OrgStructureId);
                    if (mainCompany != null)
                    {
                        workUnitsList.Add(mainCompany);
                    }
                }
                var searched = workUnitsList.Where(x => x.Name.ToString().ToUpper(XAPI.Common.exceedculture).Contains(searchText.ToUpper(XAPI.Common.exceedculture))).ToList();
                workUnitsList.Clear();
                workUnitsList.AddRange(searched);
            }
            else
            {
                foreach (var recruit in jobTitleMappings)
                {
                    var mainCompany = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == recruit.OrgStructureId);
                    if (mainCompany != null)
                    {
                        workUnitsList.Add(mainCompany);
                    }
                }
            }

            //var recruitmentPlan = db.RecruitmentPlans.Where(x => x.BranchId.ToString() == BranchId).ToList();
            //if (recruitmentPlan.Any())
            //{
            //    foreach (var plan in recruitmentPlan)
            //    {
            //        var mainCompany =
            //            db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == plan.OrgStructureId);
            //        if (mainCompany != null)
            //        {
            //            workUnitsList.Add(mainCompany);
            //        }
            //    }
            //}

            return Json(workUnitsList.DistinctByField(x => x.OrgStructureId).Select(p => new
            {
                Name = p.Name,
                OrgStructureId = p.OrgStructureId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleForPromotion(string mediacode)
        {
            db = new ExceedDbContext();
            var plans = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode.ToString() == mediacode).ToList();
            List<ApplicantRegistrationViewModel> titleList = new List<ApplicantRegistrationViewModel>();
            if (plans.Any())
            {
                ApplicantRegistrationViewModel appRegister = new ApplicantRegistrationViewModel();
                foreach (var p in plans)
                {
                    appRegister = new ApplicantRegistrationViewModel();
                    var detail = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == p.VacancyId && x.RecruSource == RecruitmentSource.INTERNAL);
                    if (detail != null)
                    {
                        var recruitPlan = db.RecruitmentPlans.Find(detail.RecruitmentPlanId);
                        if (recruitPlan != null)
                        {
                            var lookup = db.Lookups.Where(x => x.LookupId == recruitPlan.LookupId).FirstOrDefault();
                            if (lookup != null)
                            {
                                appRegister.LookUpId = lookup.LookupId;
                                appRegister.VacancyId = detail.VacancyId;
                                appRegister.Description = lookup.Description;
                                titleList.Add(appRegister);
                            }
                        }

                    }


                }
            }
            return Json(titleList.Select(p => new
            {
                LookupId = p.LookUpId + "-" + p.VacancyId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetJobTitles(int OrgStructureId)
        {
            var orgs = db.JobTitleMapping.Where(x => x.OrgStructureId == OrgStructureId).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (orgs.Any())
            {
                titleList = new List<Lookup>();
                foreach (var title in orgs)
                {
                    var lookup = db.Lookups.Where(x => x.LookupId == title.LookupId).FirstOrDefault();
                    if (!titleList.Contains(lookup))
                    {
                        titleList.Add(lookup);
                    }

                }
            }
            return Json(titleList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetSalary()
        {
            var Salaries = db.Salaries.ToList();

            return Json(Salaries.Select(p => new
            {
                SalaryStructureId = p.SalaryStructureId,
                SalaryAmount = p.SalaryAmount,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleSelected()
        {
            var jobTitle = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE);
            var jobtitleselected = jobTitle != null ? jobTitle.ToList() : new List<Lookup>();

            return Json(jobtitleselected.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleMapping(string mediacode)
        {
            //this method is for Applicant Registration
            db = new ExceedDbContext();
            List<ApplicantRegistrationViewModel> titleList = new List<ApplicantRegistrationViewModel>();
            //for (int i=0;i<VacancyId.Length;i++)
            //{
            var plans = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode.ToString() == mediacode).ToList();

            if (plans.Any())
            {
                titleList = new List<ApplicantRegistrationViewModel>();
                ApplicantRegistrationViewModel appRegister = new ApplicantRegistrationViewModel();
                foreach (var p in plans)
                {
                    appRegister = new ApplicantRegistrationViewModel();
                    var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == p.VacancyId && x.RecruSource == RecruitmentSource.EXTERNAL && x.IsActive /*&& !x.IsComplete*/);
                    if (vacancy != null)
                    {

                        var vacPlan = db.RecruitmentPlans.Find(vacancy.RecruitmentPlanId);
                        if (vacPlan != null)
                        {
                            var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == vacPlan.LookupId);
                            if (lookup != null)
                            {
                                appRegister.VacancyId = vacancy.VacancyId;
                                appRegister.LookUpId = lookup.LookupId;
                                appRegister.Description = lookup.Description;
                                titleList.Add(appRegister);
                            }
                        }

                    }


                }
            }
            //}
            return Json(titleList.Select(p => new
            {
                LookupId = p.LookUpId + "-" + p.VacancyId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetRelationshipList()
        {
            var relations = db.Lookups.Where(l => l.LookupType == LookUpTypes.RELATIONSHIP);

            var relationship = relations != null ? relations.ToList() : new List<Lookup>();
            return Json(relationship.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetAssetLists()
        //{

        //    var assetList = db.Fleets;
        //    return Json(assetList.Select(p => new
        //    {
        //        FleetsID = p.FleetsID,
        //        Name = p.EquipmentName,
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetEducationFieldList()
        {

            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_FIELD).AsQueryable();
            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEducationLevelList()
        {

            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL).AsQueryable();
            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCommitmentCosignType()
        {

            var cosignTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COSIGN_TYPE);
            return Json(cosignTypes.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCommitment()
        {

            var commtTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COMMITMENT_TYPE);
            return Json(commtTypes.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllEmployees()
        {

            var employees = db.Person_Employee.Where(x => x.IsActive).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + "-" + p.FullNameaEnglish,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllActiveEmployees()
        {

            var employees = db.Person_Employee.Where(x => x.IsActive).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.EmployeeCode + "-" + p.FullNameaEnglish,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTotalEmployees()
        {

            var employees = db.Person_Employee.ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.FullNameaEnglish,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBankLists()
        {

            var bankList = db.Banks;
            return Json(bankList.Select(p => new
            {
                BankId = p.BankId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTermination()
        {
            var terminationTypes = db.Lookups.Where(l => l.LookupType == LookUpTypes.TERMINATIONTYPE);
            return Json(terminationTypes.Select(p => new
            {
                Description = p.Description,
                LookupId = p.LookupId,
            }), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetZone()
        //{
        //    var zone = db.Zone.AsQueryable();
        //    return Json(zone.Select(p => new
        //    {
        //        ZoneId = p.ZoneId,
        //        ZoneName = p.ZoneName,
        //    }), JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetInstitutionType()
        //{
        //    var institution = db.InstitutionType.AsQueryable();
        //    return Json(institution.Select(p => new
        //    {
        //        InstitutionName = p.InstitutionName,
        //        InstitutionTypeId = p.InstitutionTypeId,
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetInsuranceCategory(string InsuranceTypeId)
        {

            var catagories = db.InsuranceCategory.Where(x => x.InsuranceTypeId.ToString() == InsuranceTypeId);
            return Json(catagories.Select(p => new
            {
                Name = p.Name,
                InsuranceCategoryId = p.InsuranceCategoryId,

            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategory()
        {
            List<string> attributes = Enum.GetValues(typeof(PromotionSetting))
                .Cast<PromotionSetting>()
                .Select(v => v.ToString())
                .ToList();

            return Json(attributes.Select(s => new
            {
                Text = s.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategories()
        {
            List<string> attributes = Enum.GetValues(typeof(InsuranceAttribute))
                .Cast<InsuranceAttribute>()
                .Select(v => v.ToString())
                .ToList();

            return Json(attributes.Select(s => new { Text = s.ToString() }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInsuranceType()
        {
            List<InsuranceType> insuranceTypeList = new List<InsuranceType>();
            var types = db.InsuranceType.Where(x => x.InsuranceTypeGroup == InsuranceGroupType.Other).ToList();
            foreach (var type in types)
            {
                InsuranceType insuranceType = new InsuranceType();
                var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == type.LookupId);
                if (lookup != null)
                {
                    insuranceType.LookupId = lookup.LookupId;
                    insuranceType.Name = lookup.Description;
                    insuranceType.InsuranceTypeId = type.InsuranceTypeId;
                }
                insuranceTypeList.Add(insuranceType);
            }
            return Json(insuranceTypeList.Select(p => new
            {
                Name = p.Name,
                InsuranceTypeId = p.InsuranceTypeId,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMonths(int DetailMeasuresId)
        {
            var months = db.DisciplineMeasuresType.Where(X => X.DetailMeasuresId == DetailMeasuresId).ToList();
            return Json(months.Select(p => new
            {
                ExpireYear = p.ExpireYear
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInsuranceTypePPE()
        {
            List<InsuranceType> insuranceTypeList = new List<InsuranceType>();
            var types = db.InsuranceType.Where(x => x.InsuranceTypeGroup == InsuranceGroupType.PPE).ToList();
            foreach (var type in types)
            {
                InsuranceType insuranceType = new InsuranceType();
                var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == type.LookupId);
                if (lookup != null)
                {
                    insuranceType.LookupId = lookup.LookupId;
                    insuranceType.Name = lookup.Description;
                    insuranceType.InsuranceTypeId = type.InsuranceTypeId;
                }
                insuranceTypeList.Add(insuranceType);
            }
            return Json(insuranceTypeList.Select(p => new
            {
                Name = p.Name,
                InsuranceTypeId = p.InsuranceTypeId,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetInsuranceGroup(string InsuranceTypeId)
        {
            List<InsuranceGroup> insuranceCategoryList = new List<InsuranceGroup>();
            var insuranceType = db.InsuranceType.FirstOrDefault(x => x.InsuranceTypeId.ToString() == InsuranceTypeId);
            if (insuranceType != null)
            {
                var insuranceCatagories = db.InsuranceCategory.Where(x => x.InsuranceTypeId == insuranceType.InsuranceTypeId).ToList();


                foreach (var insuranceCatagory in insuranceCatagories)
                {
                    InsuranceGroup insuranceGrp = new InsuranceGroup();


                    insuranceGrp.Name = insuranceCatagory.Name;
                    insuranceGrp.InsuranceGroupId = insuranceCatagory.InsuranceCategoryId;


                    insuranceCategoryList.Add(insuranceGrp);
                }
            }


            return Json(insuranceCategoryList.Select(p => new
            {
                Name = p.Name,
                InsuranceGroupId = p.InsuranceGroupId,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBusinessClassList()
        {
            var businessClass = db.Lookups.Where(x => x.LookupType == LookUpTypes.INSURANCE_CLASS_OF_BUSINESS);
            return Json(businessClass.Select(p => new
            {
                Description = p.Description,
                LookupId = p.LookupId,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUsersList()
        {
            var context = new ApplicationDbContext();
            var allUsers = context.Users.ToList();
            return Json(allUsers.Select(p => new
            {
                UserName = p.UserName,
                Email = p.Email,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserEmployeesList()
        {
            var allEmployees = db.Person_Employee.ToList();
            return Json(allEmployees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.FullNameaEnglish,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMeasureDetails(int MeasuresId)
        {
            var measureDetails = db.DisciplineMeasuresDetail.Where(x => x.MeasuresId == MeasuresId);
            return Json(measureDetails.Select(p => new
            {
                DetailMeasuresId = p.DetailMeasuresId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMeasures()
        {
            var measurs = db.DisciplineMeasures;
            return Json(measurs.Select(p => new
            {
                MeasuresId = p.MeasuresId,
                MeasuresType = p.MeasuresType,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEvaluationType()
        {

            var types = db.Lookups.Where(l => l.LookupType == LookUpTypes.EVALUATION_TYPE);
            return Json(types.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranchType()
        {

            var types = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE);
            return Json(types.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranch()
        {

            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);

            var branches = db.Branch.Where(x => mybraches.Contains(x.BranchId.ToString()));
            return Json(branches.Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranchFromRecruitmentPlan()
        {
            List<Branch> branchList = new List<Branch>();
            //var recruitmentPlan = db.RecruitmentPlans.ToList();
            var branches = db.Branch.ToList();

            branchList.AddRange(branches);

            return Json(branchList.DistinctByField(x => x.BranchId).Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllBranchs()
        {
            var branches = db.Branch.ToList();


            return Json(branches.DistinctByField(x => x.BranchId).Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBranches()
        {

            var branches = db.Branch;
            return Json(branches.Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.AmharicName,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllWorkUnit()
        {
            List<RecruitmentPlan> planList = new List<RecruitmentPlan>();

            var vacancies = db.VaccancyPlans;
            foreach (var vac in vacancies)
            {
                var plan = db.RecruitmentPlans.FirstOrDefault(x => x.RecruitmentPlanId == vac.RecruitmentPlanId);
                if (plan != null)
                {
                    planList.Add(plan);
                }
            }

            return Json(vacancies.Select(p => new
            {
                WorkUnitId = p.VacancyId,
                RecruitmentPlan = planList.FirstOrDefault(x => x.RecruitmentPlanId == p.RecruitmentPlanId)
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOverTimeEmployees()
        {
            List<Person_Employee> personList = new List<Person_Employee>();
            var overRequests = db.OverTimeRequestEmployee.ToList();
            foreach (var overRequest in overRequests)
            {
                var requestStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == overRequest.OverTimeRequestId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "OverTime");
                if (requestStatus != null)
                {
                    var requests = db.OverTimeRequestEmployee.Where(x => x.OverTimeRequestId == requestStatus.RequestId);
                    foreach (var request in requests)
                    {
                        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == request.EmployeeId);
                        if (person != null)
                        {
                            personList.Add(person);
                        }
                    }

                }

            }
            return Json(personList.DistinctByField(x => x.EmployeeId).Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.FullNameaEnglish
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllBranch()
        {
            var branches = db.Branch;
            return Json(branches.Select(p => new
            {
                BranchId = p.BranchId,
                Name = p.AmharicName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSelectedBranchs()
        {

            var branches = db.Branch.Where(x => x.BranchTypes == BranchTypes.PROJECT);
            return Json(branches.Select(p => new
            {
                BranchId = p.BranchId,
                AmharicName = p.AmharicName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVacancy()
        {
            var vacancies = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.EXTERNAL).ToList();
            return Json(vacancies.Select(p => new
            {
                VaccancyCode = p.VacancyId,
                Name = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVacancyId()
        {
            var vacancies = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.EXTERNAL).ToList();
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMediaVacancyCode()
        {
            List<RecruitmentVacancy> recruit = new List<RecruitmentVacancy>();
            var vacancies = db.RecruitmentVacancy.ToList();
            if (vacancies.Any())
            {
                foreach (var recruitmentVacancy in vacancies)
                {
                    var vacany = db.VaccancyPlans.FirstOrDefault(x =>
                        x.VacancyId == recruitmentVacancy.VacancyId && x.IsActive /*&& !x.IsComplete*/ && x.RecruSource == RecruitmentSource.EXTERNAL);
                    if (vacany != null)
                    {
                        recruit.Add(recruitmentVacancy);
                    }
                }
            }
            return Json(recruit.DistinctByField(x => x.MediaVacancyCode).Select(p => new
            {
                VaccancyCode = p.MediaVacancyCode,
                MediaVacancyCode = p.MediaVacancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMediaVacancyCodeForInternal()
        {
            List<RecruitmentVacancy> recruit = new List<RecruitmentVacancy>();
            var vacancies = db.RecruitmentVacancy.ToList();
            if (vacancies.Any())
            {
                foreach (var recruitmentVacancy in vacancies)
                {
                    var vacany = db.VaccancyPlans.FirstOrDefault(x =>
                        x.VacancyId == recruitmentVacancy.VacancyId && x.IsActive /*&& !x.IsComplete*/ && x.RecruSource == RecruitmentSource.INTERNAL);
                    if (vacany != null)
                    {
                        recruit.Add(recruitmentVacancy);
                    }
                }
            }
            return Json(recruit.DistinctByField(x => x.MediaVacancyCode).Select(p => new
            {
                VaccancyCode = p.MediaVacancyCode,
                MediaVacancyCode = p.MediaVacancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVacancyAllId()
        {
            var vacancies = db.VaccancyPlans;
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMedia()
        {
            var media = db.Lookups.Where(x => x.LookupType == LookUpTypes.MEDIA);
            return Json(media.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRecruitmentPlan()
        {
            var recruit = db.RecruitmentPlans.DistinctByField(x => x.JobtitleName).Where(x => x.To >= DateTime.Now);
            return Json(recruit.Select(p => new
            {
                RecruitmentPlanId = p.RecruitmentPlanId,
                JobtitleName = p.JobtitleName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleFromRecruitmentPlan(string OrgStructureId)
        {
            //List<Lookup> lookupList=new List<Lookup>();
            int branchId = Convert.ToInt32(OrgStructureId.Split('-')[0]);
            int orgId = Convert.ToInt32(OrgStructureId.Split('-')[1]);
            var recruit = db.RecruitmentPlans.Where(x => x.BranchId == branchId && x.OrgStructureId == orgId).DistinctByField(x => x.LookupId).ToList();//.Where(x => x.To >= DateTime.Now);
            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            //if (recruit.Any())
            //{
            //    foreach (var recruitmentPlan in recruit)
            //    {
            //        var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == recruitmentPlan.LookupId && x.LookupType==LookUpTypes.JOB_TITLE);
            //        if (lookup!=null)
            //        {
            //            lookupList.Add(lookup);
            //        }
            //    }

            //}

            var lookupList = from r in recruit
                             join l in lookups
                                 on r.LookupId equals l.LookupId
                             select (new
                             {
                                 r.LookupId,
                                 l.Description

                             });

            return Json(lookupList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleFromPlanToVac(string OrgStructureId)
        {
            List<RecruitmentPlanJob> lookupL = new List<RecruitmentPlanJob>();
            if (OrgStructureId != "undefined-undefined")
            {
                int branchId = Convert.ToInt32(OrgStructureId.Split('-')[0]);
                int orgId = Convert.ToInt32(OrgStructureId.Split('-')[1]);

                var recruit = db.RecruitmentPlans
                             .Where(x => x.BranchId == branchId && x.OrgStructureId == orgId && x.To >= DateTime.Now &&
                              x.NoOfVacantPost > 0).DistinctByField(x => x.LookupId);


                List<RecruitmentPlanJob> planLists = new List<RecruitmentPlanJob>();

                foreach (var plan in recruit)
                {
                    var pl = new RecruitmentPlanJob();

                    pl.RecruitmentPlanId = plan.RecruitmentPlanId;

                    var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == plan.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                    if (lookup != null)
                    {
                        pl.JobTitle = lookup.Description;
                    }

                    planLists.Add(pl);
                }

                return Json(planLists.Select(p => new
                {
                    RecruitmentPlanId = p.RecruitmentPlanId,
                    JobTitle = p.JobTitle,
                }), JsonRequestBehavior.AllowGet);
            }

            return Json(lookupL);
        }

        public JsonResult GetJobTitleFromManPowerToVacancy(string OrgStructureId)
        {
            List<ManPowerPlanningJob> manPowerList = new List<ManPowerPlanningJob>();

            if (OrgStructureId != "undefined-undefined")
            {
                int branchId = Convert.ToInt32(OrgStructureId.Split('-')[0]);
                int orgId = Convert.ToInt32(OrgStructureId.Split('-')[1]);

                var manPowerPlannings = db.JobTitleMapping.Where(x => x.BranchId == branchId && x.OrgStructureId == orgId).ToList();

                foreach (var plan in manPowerPlannings)
                {
                    var manPower = new ManPowerPlanningJob();

                    var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == plan.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                    if (lookup != null)
                    {
                        manPower.JobTitleName = lookup.Description;
                        manPower.LookupId = lookup.LookupId;
                    }

                    manPowerList.Add(manPower);
                }
                return Json(manPowerList.Select(p => new
                {
                    LookupId = p.LookupId,
                    JobTitleName = p.JobTitleName,
                }), JsonRequestBehavior.AllowGet);
            }
            return null;
        }

        public JsonResult GetEmployeAllowance()
        {
            var alloanceList = db.AllowanceDefinations.AsQueryable();

            return Json(alloanceList.Select(p => new
            {
                AllowanceDefinationId = p.AllowanceDefinationId,
                AllowanceName = p.AllowanceName,
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployees([DataSourceRequest] DataSourceRequest request)
        {
            HRMInputs inputs = new HRMInputs();
            db = new ExceedDbContext();
            var employees = inputs.GetEmployees(User.Identity.Name);

            return Json(employees.OrderBy(x => x.EmployeeCode).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployee()
        {
            HRMInputs inputs = new HRMInputs();

            var list = inputs.GetEmployees(User.Identity.Name);

            return Json(list.Select(p => new
            {
                PersonId = p.PersonId,
                FullNameWithOutCode = p.FullNameWithOutCode
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetActiveEmployees()
        {
            var list = db.Person_Employee.Where(x => x.IsActive).ToList();

            return Json(list.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.FullNameaEnglish,
                FullNameAmharic = p.FullNameAmharic
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployeesForPlacement([DataSourceRequest] DataSourceRequest request)
        {
            HRMInputs inputs = new HRMInputs();
            db = new ExceedDbContext();
            var employees = inputs.GetEmployeesForPlacment(User.Identity.Name);

            return Json(employees.OrderBy(x => x.EmployeeCode).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployeesSupervisor([DataSourceRequest] DataSourceRequest request, string OrgStructureId)
        {


            HRMInputs inputs = new HRMInputs();
            db = new ExceedDbContext();
            Placement placement;
            List<Person_Employee> employeesList = new List<Person_Employee>();
            Person_Employee employeeName;
            List<Placement> placementList = new List<Placement>();
            List<MainCompanyStructure> workunitList = new List<MainCompanyStructure>();

            var employees = inputs.GetEmployeesForPlacment(User.Identity.Name);
            //  get all parent work units
            var workunit = db.MainCompanyStructures.Where(x => x.StructureId.ToString() == OrgStructureId).FirstOrDefault();

            if (workunit != null)
            {

                workunitList.Add(workunit);
                int parent = workunit.Parent;

                while (parent != 0)
                {
                    var immediateParenet = db.MainCompanyStructures.Where(x => x.OrgStructureId == parent).FirstOrDefault();
                    //add to list
                    if (immediateParenet != null)
                    {

                        workunitList.Add(immediateParenet);
                        parent = immediateParenet.Parent;
                    }

                }
            }

            //get current placemen=true employes in the above work units
            foreach (var place in workunitList)
            {
                employeeName = new Person_Employee();
                placement = new Placement();
                placement = db.Placements.Where(x => x.OrgStructureId == place.OrgStructureId && x.isCurrent).FirstOrDefault();

                if (placement != null)
                {
                    employeeName = db.Person_Employee.Where(x => x.EmployeeId == placement.EmployeeId).FirstOrDefault();
                    if (employeeName != null)
                    {

                        //employeeName.FirstName = employeeName.FirstName + " " + employeeName.MiddleName;
                        employeesList.Add(employeeName);
                    }

                }
                //placementList.Add(placement);
            }

            return Json(employeesList.Select(x => new
            {
                FirstName = x.FirstName + " " + x.MiddleName + " " + x.LastName,
                EmployeeId = x.EmployeeId,
                OrgStructureId = OrgStructureId
            }).ToDataSourceResult(request), JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetEmployeesByReportTo([DataSourceRequest] DataSourceRequest request)
        {
            var employees = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);

            return Json(employees.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployeesAskForOther()
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            // var employees = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);


            List<Person_Employee> personEmployees = new List<Person_Employee>();
            var employeesId = EmployeeHelper.GetSubordinates(employeeId).ToList();
            employeesId.Remove(employeeId);
            foreach (var id in employeesId)
            {

                Person_Employee person = new Person_Employee();

                var emp = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id && x.IsActive);
                if (emp != null)
                {

                    personEmployees.Add(emp);
                }
            }

            return Json(personEmployees.DistinctByField(x => x.FirstName + " " + x.MiddleName + " " + x.LastName).Select(p => new
            {
                PersonId = p.EmployeeId,
                FullName = p.FullNameAmharic //p.FirstName + " " + p.MiddleName + " " + p.LastName
            }), JsonRequestBehavior.AllowGet);



        }
        public JsonResult GetSelectedEmployee()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name);

            var selectlist = list.Where(x => x.IsActive);

            return Json(selectlist.Select(p => new
            {
                FullName = p.FullName,
                PersonId = p.PersonId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllowance()
        {
            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE);

            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetBenefit()
        {
            var benefits = db.Lookups.Where(l => l.LookupType == LookUpTypes.OTHER_BENEFIT).ToList();
            return Json(benefits.Select(p => new
            {
                Description = p.Description,
                LookupId = p.LookupId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPayrollPlacmentEmployees()
        {
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            return Json(list.Select(p => new
            {
                FullName = p.FullName,
                PersonId = p.PersonId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApprovedEmpForOverTime()
        {
            List<Person_Employee> empLists = new List<Person_Employee>();
            var emps = db.OverTimeRequestEmployee.ToList();
            foreach (var emp in emps)
            {
                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                Person_Employee emplist = new Person_Employee();

                emplist.EmployeeId = employee.EmployeeId;
                emplist.FirstName = employee.FullNameaEnglish;

                empLists.Add(emplist);
            }
            return Json(empLists.Select(p => new
            {
                FullNameaEnglish = p.FullNameaEnglish,
                EmployeeId = p.EmployeeId
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUOM()
        {
            var uoms = db.Lookups.Where(l => l.LookupType == LookUpTypes.UOM).ToList();
            return Json(uoms.Select(p => new
            {
                Description = p.Description,
                LookupId = p.LookupId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAdditionLookUp()
        {
            var addtions = db.Lookups.Where(l => l.LookupType == LookUpTypes.ADDITION_TYPE).ToList();
            return Json(addtions.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);



        }
        public JsonResult GetComplaintType()
        {
            var lookups = db.Lookups.Where(x => x.LookupType == Core.Domain.Common.LookUpTypes.COMPLAIN_TYPE);

            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Name = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTrainingPlan()
        {
            var structures = db.TrainingPlans.AsQueryable();
            return Json(structures.DistinctByField(x => x.LookupId).Select(p => new
            {
                TrainingPlanId = p.TrainingPlanId,
                Name = p.GlFiscalYear.Name + " " + p.Lookup.Description

            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetSelectedDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.FirstOrDefault(x => x.Parent == 0);
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo = db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                var departmentLevelThree = db.MainCompanyStructures.Where(x => x.Parent == b.OrgStructureId).ToList();
                                if (departmentLevelThree.Any())
                                {
                                    foreach (var c in departmentLevelThree)
                                    {
                                        var deparmentLeveFour = db.MainCompanyStructures.Where(x => x.Parent == c.OrgStructureId).AsEnumerable().DistinctByField(x => x.OrgStructureId).ToList();
                                        if (deparmentLeveFour.Any())
                                        {
                                            foreach (var d in deparmentLeveFour)
                                            {
                                                allDepartment.Add(d);
                                            }
                                        }
                                        allDepartment.Add(c);
                                    }

                                }
                                allDepartment.Add(b);
                            }
                        }
                        allDepartment.Add(a);
                    }
                }
            }
            var department = allDepartment;
            return Json(department.DistinctByField(x => x.OrgStructureId).Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetDepartmentOfHeads()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();

            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var employeePlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employeeId && x.IsHead);
            if (employeePlacement != null)
            {
                var workunits = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == employeePlacement.OrgStructureId);
                if (workunits != null)
                {
                    // var companyStructure = db.MainCompanyStructures.Find(workunits.OrgStructureId);
                    allDepartment.Add(workunits);
                    var heads = db.MainCompanyStructures.Where(x => x.Parent == workunits.OrgStructureId).AsEnumerable().DistinctByField(x => x.OrgStructureId).ToList();
                    if (heads.Any())
                    {
                        foreach (var lowh in heads)
                        {
                            var companyStructure = new MainCompanyStructure();

                            companyStructure = db.MainCompanyStructures.Find(lowh.OrgStructureId);

                            allDepartment.Add(companyStructure);
                        }
                    }

                }

            }

            return Json(allDepartment.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetDepartmentOfHeadsFromRecPlan()
        {

            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();

            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var employeePlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employeeId && x.IsHead);
            if (employeePlacement != null)
            {
                var recruitmentPlan =
                    db.RecruitmentPlans.Where(x => x.OrgStructureId == employeePlacement.OrgStructureId);
                var workunits = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == employeePlacement.OrgStructureId);
                if (workunits != null)
                {

                    var companyStructure = db.MainCompanyStructures.Find(workunits.OrgStructureId);
                    allDepartment.Add(companyStructure);
                    var heads = db.MainCompanyStructures.Where(x => x.Parent == workunits.OrgStructureId).AsEnumerable().DistinctByField(x => x.OrgStructureId).ToList();
                    if (heads.Any())
                    {
                        foreach (var lowh in heads)
                        {
                            companyStructure = new MainCompanyStructure();
                            companyStructure = db.MainCompanyStructures.Find(lowh.OrgStructureId);
                            allDepartment.Add(companyStructure);
                        }
                    }

                }

            }

            return Json(allDepartment.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetDepartmentHead()
        {
            List<OrgChartViewModel> structureList = new List<OrgChartViewModel>();
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employeePlacement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employeeId && x.IsHead);
            if (employeePlacement != null)
            {
                var workunits = db.MainCompanyStructures.ToList();
                if (workunits.Any())
                {
                    foreach (var workunit in workunits)
                    {
                        var structure = new OrgChartViewModel();
                        structure.relationship = workunit.OrgStructureId;
                        structure.name = workunit.Name;
                        structure.parent = workunit.Parent;

                        structureList.Add(structure);
                    }

                }

                var root = structureList.FirstOrDefault(x => x.parent == employeePlacement.OrgStructureId);

                setChildren(root, structureList);
                // return Json(root, JsonRequestBehavior.AllowGet);
                //structureList.Add(Chil);

            }

            return Json(structureList, JsonRequestBehavior.AllowGet);


        }
        public void setChildren(OrgChartViewModel orgModel, List<OrgChartViewModel> workunitList)
        {
            var childs = workunitList.Where(x => x.parent == orgModel.relationship).ToList();
            if (childs.Any())
            {
                foreach (var child in childs)
                {
                    setChildren(child, workunitList);
                    orgModel.children.Add(child);
                }
            }

        }
        public JsonResult GetCommittee()
        {
            var committees = db.Committees.AsQueryable();
            return Json(committees.Select(p => new
            {
                CommitteeId = p.CommitteeId,
                Name = p.Description
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetCostCenter(int BranchId)
        {

            GlCostCenterTypes type;
            List<SelectorVm> costcenterList = new List<SelectorVm>();
            var branch = db.Branch.FirstOrDefault(x => x.BranchId == BranchId);
            if (branch != null)
            {
                if (branch.BranchTypes == BranchTypes.HEAD_OFFICE)
                {
                    type = GlCostCenterTypes.HeadOffice;
                }
                else
                {
                    type = GlCostCenterTypes.Project;
                }
                AccountHelper accountHelper = new AccountHelper();
                if (accountHelper.IsDynamicCoA())
                {
                    var glCostCenterSeg = db.GlSegmentSetups.Where(x => x.SegmentType == DynamicSegmentTypes.CostCenter).FirstOrDefault();
                    if (glCostCenterSeg != null)
                    {
                        var glcostcenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == glCostCenterSeg.GLSegmentSetupId).Where(x => x.Enable && !x.Parent).ToList();
                        foreach (var item in glcostcenters)
                        {
                            var record = new SelectorVm
                            {
                                Code = item.Values,
                                Name = item.Description
                            };
                            costcenterList.Add(record);
                        }
                    }
                }
                else
                {
                    //var staticcostcenters = db.GLChartOfAccountCostCenters
                    //    .Where(x => x.Type == type && x.Enable && !x.Parent)
                    //   .ToList();
                    //foreach (var item in staticcostcenters)
                    //{
                    //    var record = new SelectorVm
                    //    {
                    //        Code = item.Values,
                    //        Name = item.Description
                    //    };
                    //    costcenterList.Add(record);
                    //}
                }

            }

            return Json(costcenterList.Select(p => new
            {
                Values = p.Code,
                Description = p.Name
            }), JsonRequestBehavior.AllowGet);


        }
        public JsonResult GetVacancyPlan()
        {
            var list = db.VaccancyPlans.ToList();
            return Json(list.Select(p => new
            {
                VaccancyCode = p.VacancyId,
                Name = p.VaccancyCode
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetVacancyGeneratedCode()
        {
            var list = db.RecruitmentVacancy.DistinctByField(x => x.MediaVacancyCode).ToList();
            return Json(list.Select(p => new
            {
                VacancyId = p.VacancyId,
                MediaVacancyCode = p.MediaVacancyCode
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCommitteType()
        {
            var committeTypes = db.Lookups.Where(l => l.LookupType == LookUpTypes.COMMITTEE_TYPE);
            var committeType = committeTypes;
            return Json(committeType.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStructures()
        {
            var structures = db.Structure;

            return Json(structures.Select(s => new
            {
                StructureId = s.StructureId,
                Name = s.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCountry()
        {
            var countries = db.Countries.AsQueryable();
            return Json(countries.Select(p => new
            {
                CountryId = p.CountryId,
                Nationality = p.Nationality
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNation()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.NATION && l.IsActive).AsQueryable();

            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPersonTitle()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.PERSON_TITLE && l.IsActive).AsQueryable();

            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetLaboratoryCatagories()
        //{
        //    var catagories = db.LaboratoryCatagories.AsQueryable();
        //    return Json(catagories.Select(p => new
        //    {
        //        LaboratoryCatagoryId = p.LaboratoryCatagoryId,
        //        Catagory = p.Catagory
        //    }), JsonRequestBehavior.AllowGet);
        //}
        //public JsonResult GetLaboratoryTests()
        //{
        //    var tests = db.LaboratoryTests.AsQueryable();
        //    return Json(tests.Select(p => new
        //    {
        //        LaboratoryTestsId = p.LaboratoryTestsId,
        //        Test = p.Test
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetRank()
        {
            var ranks = db.Ranks.AsQueryable();
            return Json(ranks.Select(p => new
            {
                RankId = p.RankId,
                RankName = p.RankName
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStep()
        {

            var stepss = db.Steps.AsQueryable();
            ViewData["StepList"] = stepss;
            return Json(stepss.Select(p => new
            {
                StepId = p.StepId,
                StepName = p.StepName
            }), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetContributionType()
        //{
        //    var ConrtibutionTypes = db.ContributionTypes.Where(x => x.ContributionCategory == ContributionCategory.FIXED);
        //    return Json(ConrtibutionTypes.Select(p => new
        //    {
        //        Description = p.Description,
        //        ContributionTypeId = p.ContributionTypeId
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetWorkUnit()
        {
            List<MainCompanyStructure> workUnits = new List<MainCompanyStructure>();
            var branchCostCenter = db.BranchCostCenter.ToList();

            foreach (var b in branchCostCenter)
            {

                var oneWorkUnit = db.MainCompanyStructures.Where(x => x.OrgStructureId == b.OrgStructureId).FirstOrDefault();
                if (oneWorkUnit != null)
                {
                    workUnits.Add(oneWorkUnit);
                }

            }

            return Json(workUnits.Select(p => new
            {
                Name = p.Name,
                OrgStructureId = p.OrgStructureId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetWorkUnits()
        {
            List<MainCompanyStructure> workUnits = new List<MainCompanyStructure>();
            var mainCompany = db.MainCompanyStructures.ToList();
            workUnits.AddRange(mainCompany);


            return Json(workUnits.Select(p => new
            {
                Name = p.Name,
                OrgStructureId = p.OrgStructureId
            }), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult GetPeriodForOvertime()
        //{
        //    var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
        //    //var list = from p in periodList
        //    //           join g in db.GLPeriods
        //    //               on p.Name equals g.Name
        //    //           select (new
        //    //           {
        //    //               g
        //    //           });
        //    ViewData["PeriodList"] = periodList.Select(x => x.Name).ToList(); //!= null ? list.ToList() : new List<PayrollPeriod>();

        //    return Json(periodList.Select(p => new
        //    {
        //        GLPeriodId = p.GLPeriodId,
        //        Name = p.Name
        //    }), JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetWorkUnitFromVacancy(string mediacode)
        {
            List<MainCompanyStructure> workUnitsList = new List<MainCompanyStructure>();

            var vacancyIds = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode == mediacode).Select(x => x.VacancyId).ToList();

            var departments = db.VaccancyPlans.Where(x => vacancyIds.Contains(x.VacancyId) && x.IsActive).Select(x => x.Department).ToList();

            if (departments.Any())
            {
                workUnitsList = db.MainCompanyStructures.Where(x => departments.Contains(x.OrgStructureId)).ToList();
            }

            return Json(workUnitsList.DistinctByField(x => x.OrgStructureId).Select(p => new
            {
                Name = p.Name,
                OrgStructureId = p.OrgStructureId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTitleFromVacancy(int OrgStructureId, string MediaVacancyCode)
        {
            List<ApplicantRegistrationViewModel> titleList = new List<ApplicantRegistrationViewModel>();
            ApplicantRegistrationViewModel appRegister = new ApplicantRegistrationViewModel();

            var vacancyIds = db.RecruitmentVacancy.Where(x => x.MediaVacancyCode == MediaVacancyCode).Select(x => x.VacancyId).ToList();

            var vacancyJobtitleIds = db.VaccancyPlans.Where(x => vacancyIds.Contains(x.VacancyId) && x.Department == OrgStructureId).Select(x => x.JobTitle).ToList();

            var lookups = db.Lookups.Where(x => vacancyJobtitleIds.Contains(x.LookupId)).ToList();

            //var vacancy = db.VaccancyPlans.Where(x => x.OrgStructureId.ToString() == OrgStructureId && x.IsActive && !x.IsComplete).ToList();
            //foreach (var vacancy1 in vacancy)
            //{
            //    var recruitmentPlan =
            //        db.RecruitmentPlans.FirstOrDefault(x => x.RecruitmentPlanId == vacancy1.RecruitmentPlanId);
            //    if (recruitmentPlan != null)
            //    {
            //        var lookup = db.Lookups.FirstOrDefault(x =>
            //            x.LookupId == recruitmentPlan.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
            //        if (lookup != null)
            //        {
            //            appRegister.LookUpId = lookup.LookupId;
            //            appRegister.VacancyId = vacancy1.VacancyId;
            //            appRegister.Description = lookup.Description;
            //            titleList.Add(appRegister);
            //        }
            //    }
            //}

            return Json(lookups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetCompany()
        {
            var hospitals = db.Companies.Where(x => x.Category == "2" && x.IsActive);
            return Json(hospitals.Select(p => new
            {
                TradeName = p.TradeName,
                MainCompanyId = p.MainCompanyId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHospitals()
        {
            var hospitals = db.Companies.Where(x => x.Category == "2" && x.IsActive);
            return Json(hospitals.Select(p => new
            {
                TradeName = p.TradeName,
                MainCompanyId = p.MainCompanyId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHealthCompany()
        {
            var hospitals = db.Companies.Where(x => x.Category == "3" && x.IsActive);
            return Json(hospitals.Select(p => new
            {
                TradeName = p.TradeName,
                MainCompanyId = p.MainCompanyId
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeLists()
        {
            var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);
            var employeeList = list.Where(x => x.IsActive);
            return Json(employeeList.Select(p => new
            {
                PersonId = p.PersonId,
                FullName = p.FullName
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Employee_ValueMapper(string value)
        {
            HRMInputs inputs = new HRMInputs();

            var dataItemIndex = -1;
            var data = inputs.GetEmployees(User.Identity.Name);
            if (value != "")
            {
                var index = 0;
                foreach (var vm in data)
                {
                    if (vm.PersonId.ToString() == value)
                    {
                        dataItemIndex = index;
                        break;
                    }

                    index += 1;
                }
            }
            return Json(dataItemIndex, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTargetGroup()
        {
            var targetGroups = db.Lookups.Where(x => x.LookupType == LookUpTypes.TARGET_GROUP);
            return Json(targetGroups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProbationMeasurments()
        {
            var targetGroups = db.Lookups.Where(x => x.LookupType == LookUpTypes.PROBATION_MEASUREMENT);
            return Json(targetGroups.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeesByUser()
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var subordinates = EmployeeHelper.GetSubordinates(id);
            var employees = db.Person_Employee.Where(x => subordinates.Contains(x.EmployeeId)).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.FullNameaEnglish
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeesByUserForPerformanceEvaluation()
        {
            var id = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var subordinates = EmployeeHelper.GetSubordinates(id);
            var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == PerformanceSelfEvaluation.Enable_Self_Evaluation.ToString());
            if (config != null)
            {
                subordinates.Remove(id);
            }
            var employees = db.Person_Employee.Where(x => subordinates.Contains(x.EmployeeId)).ToList();
            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameaEnglish = p.FullNameaEnglish
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubjectType()
        {
            IQueryable<SubjectType> subjectTypes = db.SubjectTypes;
            return Json(subjectTypes.Select(p => new
            {
                SubjectTypeId = p.SubjectTypeId,
                SubjectTypeName = p.SubjectTypeName
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPayrollEmployeeList([DataSourceRequest] DataSourceRequest request)
        {
            List<EmployeeSelectorVm> list = new List<EmployeeSelectorVm>();
            db = new ExceedDbContext();
            var employees = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);

            foreach (var emp in employees)
            {

                var record = new EmployeeSelectorVm
                {
                    PersonId = emp.PersonId,
                    FullName = emp.FullName
                };
                list.Add(record);

            }

            return Json(list.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            }
            public ActionResult PayrollEmployee_ValueMapper(string value)
        {

            var dataItemIndex = -1;
            var data = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            if (value != "")
            {
                var index = 0;
                foreach (var vm in data)
                {
                    if (vm.PersonId.ToString() == value)
                    {
                        dataItemIndex = index;
                        break;
                    }

                    index += 1;
                }
            }
            return Json(dataItemIndex, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadFile(int filename, string type)
        {
            string referenceType = "";

            db = new ExceedDbContext();
            try
            {
                var attachment = db.Attachments.FirstOrDefault(x => x.AttachmentId == filename);
                if (attachment != null)
                {
                    switch (type)
                    {
                        case "Photo":
                            referenceType = AttachmentType.PHOTO.ToString();
                            break;
                        case "Logo":
                            referenceType = AttachmentType.LOGO.ToString();
                            break;
                        case "Doc":
                            referenceType = AttachmentType.DOCUMENT.ToString();
                            break;
                        case "Cosign":
                            referenceType = AttachmentType.COSIGN.ToString();
                            break;
                        case "Clearance":
                            referenceType = AttachmentType.CLEARANCE.ToString();
                            break;
                        case "Minute":
                            referenceType = AttachmentType.MINUTE.ToString();
                            break;
                        case "Incoming":
                            referenceType = AttachmentType.INCOMING_LETTER.ToString();
                            break;
                        case "Outgoing":
                            referenceType = AttachmentType.OUTGOING_LETTER.ToString();
                            break;
                        case "Letter":
                            referenceType = AttachmentType.LETTER.ToString();
                            break;

                    }
                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition",
                        "attachment;filename=\"" + attachment.Description + "\"");
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + attachment.Reference + "-" + referenceType + "/" + attachment.Description));

                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(attachment.Reference, attachment.FileType);
                }
            }
            catch (Exception e)
            {


            }
            HRMInputs.SelectedFile = 0;
            return null;
        }
        public ActionResult DownloadFileFromAttachmentArchive(int filename, string type)
        {
            //string referenceType = "";




            //db = new ExceedDbContext();
            //try
            //{
            //    var attachment = db.AttachmentArchives.FirstOrDefault(x => x.AttachmentArchiveId == filename);
            //    if (attachment != null)
            //    {
            //        switch (type)
            //        {
            //            case "Photo":
            //                referenceType = AttachmentType.PHOTO.ToString();
            //                break;
            //            case "Logo":
            //                referenceType = AttachmentType.LOGO.ToString();
            //                break;
            //            case "Doc":
            //                referenceType = AttachmentType.DOCUMENT.ToString();
            //                break;
            //            case "Cosign":
            //                referenceType = AttachmentType.COSIGN.ToString();
            //                break;
            //            case "Clearance":
            //                referenceType = AttachmentType.CLEARANCE.ToString();
            //                break;
            //            case "Minute":
            //                referenceType = AttachmentType.MINUTE.ToString();
            //                break;
            //            case "Incoming":
            //                referenceType = AttachmentType.INCOMING_LETTER.ToString();
            //                break;
            //            case "Outgoing":
            //                referenceType = AttachmentType.OUTGOING_LETTER.ToString();
            //                break;
            //            case "Letter":
            //                referenceType = AttachmentType.LETTER.ToString();
            //                break;

            //        }
            //        WebClient req = new WebClient();
            //        HttpResponse response = System.Web.HttpContext.Current.Response;
            //        response.Clear();
            //        response.ClearContent();
            //        response.ClearHeaders();
            //        response.Buffer = true;
            //        response.AddHeader("Content-Disposition",
            //            "attachment;filename=\"" + attachment.Description + "\"");
            //        byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + attachment.Reference + "-" + referenceType + "/" + attachment.Description));

            //        response.BinaryWrite(datas);
            //        response.End();
            //        HRMInputs.SelectedFile = 0;
            //        return File(attachment.Reference, attachment.FileType);
            //    }
            //}
            //catch (Exception e)
            //{


            //}
            //HRMInputs.SelectedFile = 0;
            return null;
        }

        public JsonResult GetAllBusinessUnits()
        {
            List<SelectorVm> list = new List<SelectorVm>();
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = db.BusinessUnits;
                foreach (var bu in businessUnits)
                {
                    var record = new SelectorVm();
                    record.Name = bu.Name;
                    record.Code = bu.BusinessUnitId.ToString();
                    list.Add(record);
                }
            }
            else
            {
                var record = new SelectorVm();
                record.Name = "No Business Unit";
                record.Code = "0";
                list.Add(record);
            }
            return Json(list.Select(x => new
            {
                Name = x.Name,
                Code = x.Code
            }), JsonRequestBehavior.AllowGet);
        }

        //////// Recruitment New ////////////////////
        public JsonResult GetAllVacacy()
        {
            var vacancies = db.VaccancyPlans.ToList();
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetVacancyLists()
        {
            var vacancies = db.VaccancyPlans.ToList();
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEducationLevelLists()
        {
            var educationLevels = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL).ToList();
            return Json(educationLevels.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEducationFieldLists()
        {
            var educationFields = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_FIELD).ToList();
            return Json(educationFields.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLookupJobtitle()
        {
            var experienceJobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            return Json(experienceJobtitles.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        //
        public ActionResult GetLookupJobtitleForInternal()
        {
            // this "Jobtitle External Experience" is for internal applicants that they have external experiences registered in profile
            var experienceJobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.Jobtitle_External_Experience).ToList();
            return Json(experienceJobtitles.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEducationLevels()
        {
            var educationLevels = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_LEVEL).ToList();

            return Json(educationLevels.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEducationFields()
        {
            var educationFields = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_FIELD).ToList();

            return Json(educationFields.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVacancyExternal()
        {
            var vacancies = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.EXTERNAL && x.IsActive).ToList();
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVacancyInternal()
        {
            var vacancies = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.INTERNAL && x.IsActive).ToList();
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetVacancyCodeForInternalApplicant()
        {
            var vacancies = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.INTERNAL && x.IsActive).ToList();
            return Json(vacancies.Select(p => new
            {
                VacancyId = p.VacancyId,
                VaccancyCode = p.VaccancyCode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentsByVacancy(int VacancyId)
        {
            List<MainCompanyStructure> mainCompanys = new List<MainCompanyStructure>();

            var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == VacancyId);
            if (vacancy != null)
            {
                var mainCompanyStructure = db.MainCompanyStructures.Where(x => x.OrgStructureId == vacancy.Department).ToList();

                mainCompanys.AddRange(mainCompanyStructure);
            }
            return Json(mainCompanys.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDepartmentsByBranchFromManPowerPlanning(int BranchId, string searchText)
        {
            List<MainCompanyStructure> departments = new List<MainCompanyStructure>();

            var structures = db.JobTitleMapping.Where(x => x.BranchId == BranchId).DistinctByField(x => x.OrgStructureId).ToList();

            if (!String.IsNullOrEmpty(searchText))
            {
                var ids = db.MainCompanyStructures.ToList().Where(x => !string.IsNullOrEmpty(x.Name) && x.Name.Contains(searchText)).Select(x => x.OrgStructureId).ToList();

                structures = structures.Where(x => ids.Contains(x.OrgStructureId)).ToList();

                foreach (var str in structures)
                {
                    var structure = new MainCompanyStructure();
                    var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == str.OrgStructureId);
                    if (department != null)
                    {
                        structure.Name = department.Name;
                        structure.OrgStructureId = department.OrgStructureId;

                        departments.Add(structure);
                    }
                }

            }
            else
            {
                foreach (var str in structures)
                {
                    var structure = new MainCompanyStructure();
                    var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == str.OrgStructureId);
                    if (department != null)
                    {
                        structure.Name = department.Name;
                        structure.OrgStructureId = department.OrgStructureId;

                        departments.Add(structure);
                    }
                }
            }


            return Json(departments.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobtitlesByVacancy(int VacancyId)
        {
            List<Lookup> jobTitles = new List<Lookup>();

            var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == VacancyId);
            if (vacancy != null)
            {
                var jobTitle = db.Lookups.Where(x => x.LookupId == vacancy.JobTitle).ToList();

                jobTitles.AddRange(jobTitle);
            }
            return Json(jobTitles.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllDepartments()
        {
            var departments = db.MainCompanyStructures.ToList();

            return Json(departments.Select(p => new
            {
                OrgStructureId = p.OrgStructureId,
                Name = p.Name
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTrainingCriterias()
        {
            var criterias = db.TrainingEvaluationCriterias.ToList();

            return Json(criterias.Select(p => new
            {
                TrainingEvaluationCriteriaId = p.TrainingEvaluationCriteriaId,
                Criteria = p.Criteria
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeesInSelectedDepartment(int OrgStructureId)
        {
            var empIds = db.Placements.Where(x => x.OrgStructureId == OrgStructureId && x.isCurrent).Select(x => x.EmployeeId).ToList();

            var employees = db.Person_Employee.Where(x => empIds.Contains(x.EmployeeId)).ToList();

            return Json(employees.Select(p => new
            {
                EmployeeId = p.EmployeeId,
                FullNameAmharic = p.FullNameAmharic
            }), JsonRequestBehavior.AllowGet);
        }
        //////
        public JsonResult GetEnumWithOutApproveOption()
        {

            var statuses = Enum.GetValues(typeof(RequestStatus))
                           .Cast<RequestStatus>()
                           .Where(x => x != RequestStatus.APPROVE)
                           .Select(v => v)
                           .ToList();

            return Json(statuses.Select(p => new
            {
                Value = Convert.ToInt32(p),
                Display = p.ToString()
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEnumWithOutWaitingForApprovalandDepartmentApproveOption()
        {

            var statuses = Enum.GetValues(typeof(RequestStatus))
                           .Cast<RequestStatus>()
                           .Where(x => x != RequestStatus.WAITING_FOR_APPROVAL && x != RequestStatus.DEPARTMENT_APPROVAL)
                           .Select(v => v)
                           .ToList();

            return Json(statuses.Select(p => new
            {
                Value = Convert.ToInt32(p),
                Display = p.ToString()
            }), JsonRequestBehavior.AllowGet);
        }
    }
    public class Jobs
    {
        public string Description { get; set; }
        public int LookupId { get; set; }
    }
    public class RecruitmentPlanJob
    {
        public int RecruitmentPlanId { get; set; }
        public string JobTitle { get; set; }
    }

    public class ManPowerPlanningJob
    {
        public string JobTitleName { get; set; }
        public int LookupId { get; set; }
    }
}