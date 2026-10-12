using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using ExceedERP.Core.Domain.Global;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using System.Data.OleDb;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using Kendo.Mvc;
using Lookup = ExceedERP.Core.Domain.Common.HRM.Lookup;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.Helpers.Finance;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;
using ExceedERP.Web.Models;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRProfileUser, HrmConstants.HRProfileAdmin)]
    public class EmployeeProfileController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        private readonly string cultureName = GlobalHelper.GetCurrentCulture();

        string ErrorMessage = "";
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {

         
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            ViewData["personalTitle"] = db.Lookups
                .Where(x => x.LookupType == LookUpTypes.PERSON_TITLE)
                .Select(x => new { Title = x.LookupId, Description = x.Description });

           
            ViewData["commitment"] = db.Lookups
                .Where(x => x.LookupType == LookUpTypes.COMMITMENT_TYPE);

            
            ViewData["Measures"] = db.DisciplineMeasures;
            ViewData["MeasuresDetails"] = db.DisciplineMeasuresDetail;
           
            ViewData["jobTitle"] = db.Lookups.Where(x=>x.LookupType==LookUpTypes.JOB_TITLE
                                                        ||x.LookupType==LookUpTypes.OLD_JOB_TITLE
                                                        ||x.LookupType==LookUpTypes.Jobtitle_External_Experience).Select(
               i => new
               {
                   Description = i.Description,
                   LookupId = i.LookupId,
               }).AsQueryable();

            ViewData["AllJobTitles"] = db.Lookups.Where(x=>x.LookupType==LookUpTypes.JOB_TITLE
                                                        ||x.LookupType==LookUpTypes.OLD_JOB_TITLE
                                                        ||x.LookupType==LookUpTypes.Jobtitle_External_Experience).Select(
               i => new
               {
                   Description = i.Description,
                   LookupId = i.LookupId,
               }).ToList();
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_FIELD).AsQueryable();
            ViewData["EduFiledList"] = lookups;
            var lookupEduLevels = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL).AsQueryable();
            ViewData["EduLevelList"] = lookupEduLevels;

            ViewData["Branch"] = CacheData.GetBranchCaches().Select(
              i => new
              {
                  Name = i.Name,
                  BranchId = i.Id,
              }).AsQueryable();

           
            ViewData["workunits"] = CacheData.GetWorkUnitCaches().Select(
               i => new
               {
                   Name = i.Name,
                   OrgStructureId = i.Id,
               }).AsQueryable();
            ViewData["employeeType"] = db.EmployeeTypes
                .Select(x => new { x.EmployeeTypeId, x.Name });

            // Load placement-related data
            GetPlacementAfterSystem();

            // Settings flags
            ViewData["isManual"] = db.GeneralJobtitleSettings
                .Select(x => (bool?)x.IsManual)
                .FirstOrDefault() ?? false;

            ViewData["isEthiopicDate"] = db.DatePreferenceSettings
                .Select(x => (bool?)x.UseEthiopianDate)
                .FirstOrDefault();
            ViewData["EmployeeList"] = CacheData.GetEmployeeCaches().Select(
               i => new
               {
                   FullNameAmharic = i.AmharicName,
                   FullNameaEnglish = i.Name,
                   EmployeeId = i.Id,
                   HeadId = i.Id
               }).AsQueryable();
            ViewData["CosignType"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.COSIGN_TYPE).Select(x=>new { x.LookupId,x.Description}).AsQueryable();
            ViewData["autoEmployeeReg"] = db.Configurations.Any(x => x.Reference == "BiometricEmployeeAutoRegistration" && x.Attribute == "BiometricEmployeeAutoRegistration");
            ViewData["autoEmployeeExempt"] = db.Configurations.Any(x => x.Reference == "BiometricEmployeeAutoExempt" && x.Attribute == "BiometricEmployeeAutoExempt");

            var periodList = db.Periods;

            var languageList = db.Languages;
            var relations = db.Lookups.Where(l => l.LookupType == LookUpTypes.RELATIONSHIP);
            ViewData["LanguageList"] = languageList != null ? languageList.ToList() : new List<Language>();

            ViewData["PeriodList"] = db.GLPeriods.ToList();// periodList != null ? periodList.ToList() : new List<Period>();

            ViewData["Relationship"] = relations != null ? relations.ToList() : new List<Lookup>();
            ViewData["EmployeeList"] = db.Person_Employee.ToList();//inputs.GetEmployees(User.Identity.Name);
            GetNation();
            GetNationality();
            var skillTypes = db.Lookups.Where(l => l.LookupType == LookUpTypes.SKILL_TYPE);
            ViewData["SkillTypes"] = skillTypes;

            ViewData["fiscalYearList"] = CacheData.GetFiscalYearCaches().Select(
              i => new
              {
                  Name = i.Name,
                  GlFiscalYearId = i.Id,
              }).AsQueryable();
            ViewData["Period"] = db.Quarters.Select(x => new { x.QuarterId, x.Name }).ToList();

            var measurments = db.MeasurementDefinitions.Select(p => new
            {
                MeasurementDefinitionId = p.MeasurementDefinitionId,
                Name = p.EvaluationCriteria + " - " + p.Lookup.Description + " - " + p.Role.ToString() + " " + p.DefnPercentageofWeight

            }).ToList();
            ViewData["EvalutionCrateria"] = measurments;
            ViewData["Measurments"] = db.Measurements.ToList();
            ViewData["PerformanceActions"] = db.PerformanceActions.ToList();

        }
        private int CalculateProbationAmount(DateTime hiringDate )
        {
            int probationAmount = 0;
            var employeeProbationSetting = db.EmployeeProbationSettings.FirstOrDefault();
            if(employeeProbationSetting !=null && employeeProbationSetting.IncludeWeekend&&employeeProbationSetting.IncludeHoliday)
            {
                for (DateTime currentDate = hiringDate; probationAmount <= 60 && currentDate.Date <= DateTime.Now.Date; currentDate = currentDate.AddDays(1))
                {
                    // including weekends (Saturday and Sunday) and holidays
                    probationAmount++;


                  

                }
            }
            if (employeeProbationSetting !=null && !employeeProbationSetting.IncludeWeekend&&employeeProbationSetting.IncludeHoliday)
            {
               
                for (DateTime currentDate = hiringDate; probationAmount <= 60 && currentDate.Date <= DateTime.Now.Date; currentDate = currentDate.AddDays(1))
                {

                    // Exclude weekends (Saturday and Sunday) and  include holliday
                    if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        probationAmount++;


                    }
                   




                }
            }
            if (employeeProbationSetting !=null && employeeProbationSetting.IncludeWeekend&& !employeeProbationSetting.IncludeHoliday)
            {
                List<DateTime> holidays = db.Holidays.Select(x => x.Date).ToList();
                for (DateTime currentDate = hiringDate; probationAmount <= 60 && currentDate.Date <= DateTime.Now.Date; currentDate = currentDate.AddDays(1))
                {
                    // include  weekends (Saturday and Sunday) and exclude  holidays

                    if (!holidays.Contains(currentDate))
                    {
                        probationAmount++;


                    }




                }
            }

            else
            {
                List<DateTime> holidays = db.Holidays.Select(x => x.Date).ToList();
                for (DateTime currentDate = hiringDate; probationAmount <= 60 && currentDate.Date <= DateTime.Now.Date; currentDate = currentDate.AddDays(1))
                {
                    // Exclude weekends (Saturday and Sunday) and holidays
                    if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
                    currentDate.DayOfWeek != DayOfWeek.Sunday &&
                    !holidays.Contains(currentDate))
                    {
                        probationAmount++;


                    }

                }

            }
            

            return probationAmount;
        }
        // Working-day count from hiringDate through today, honoring Include Weekend / Include Holiday
        // (same rule as CalculateProbationAmount, but reusable with a pre-loaded holidays set - no per-call DB hit).
        private int CalculateProbationWorkingDays(DateTime hiringDate, DateTime today, bool includeWeekend, bool includeHoliday, HashSet<DateTime> holidays)
        {
            int days = 0;
            for (DateTime currentDate = hiringDate.Date; currentDate <= today.Date; currentDate = currentDate.AddDays(1))
            {
                bool isWeekend = currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday;
                bool isHoliday = holidays.Contains(currentDate.Date);

                if (!includeWeekend && isWeekend)
                    continue;
                if (!includeHoliday && isHoliday)
                    continue;

                days++;
            }

            return days;
        }
        public void GetPlacementAfterSystem()
        {
            ViewData["branches"] = db.Branch.Select(x=>new {x.BranchId,x.Name }).ToList();
            ViewData["workunits"] = db.MainCompanyStructures.Select(x => new {x.OrgStructureId,x.Name }).ToList();
            ViewData["Jobtitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).Select(x => new {x.LookupId,x.Description }).ToList();
            ViewData["RankList"] = db.Ranks.Select(x => new {x.RankId,x.RankName}).ToList();
            ViewData["StepList"] = db.Steps.Select(x => new {x.StepId,x.StepName }).ToList();
        }
        public JsonResult GenerateProfile(string id, string text)
        {
            int EmployeeId = Convert.ToInt32(id);
            string NewEmployeeCode = text;

            string EmpCodeGenerated = GenerateEmployeeCode(DateTime.Today);

            if (!string.IsNullOrEmpty(EmpCodeGenerated))
            {
                NewEmployeeCode = EmpCodeGenerated;
            }

            int EmployeeIdNew = 0;

            if (id != null && !string.IsNullOrEmpty(NewEmployeeCode))
            {
                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == EmployeeId);
                var educations = db.Education.Where(x => x.EmployeeId == EmployeeId).ToList();
                var experiences = db.Experiences.Where(x => x.EmployeeId == EmployeeId).ToList();
                var trainings = db.EmpTrainingEducations.Where(x => x.EmployeeId == EmployeeId).ToList();
                var languages = db.LanguageOfProficiencies.Where(x => x.EmployeeId == EmployeeId).ToList();
                var competencies = db.Competencies.Where(x => x.EmployeeId == EmployeeId).ToList();
                var associasions = db.EmployeeAssociation.Where(x => x.EmployeeId == EmployeeId).ToList();
                var emergencies = db.Emergencies.Where(x => x.EmployeeId == EmployeeId).ToList();
                var disabilities = db.Disabilities.Where(x => x.EmployeeId == EmployeeId).ToList();
                var addresses = db.Addresses.Where(x => x.Reference == EmployeeId.ToString()).ToList();
                var identifications = db.Identifications.Where(x => x.Reference == EmployeeId.ToString()).ToList();
                var attachmentsPhoto = db.Attachments.Where(x => x.Reference == EmployeeId.ToString() && x.ReferenceType == AttachmentType.PHOTO && x.Remark == EmployeeId.ToString()).ToList();
                var attachmentsDocument = db.Attachments.Where(x => x.Reference == EmployeeId.ToString() && x.ReferenceType == AttachmentType.DOCUMENT && x.Remark == EmployeeId.ToString()).ToList();
                var contracts = db.Contract.Where(x => x.EmployeeId == EmployeeId).ToList();

                if (employee != null && !employee.IsActive)
                {
                    var person = new Person_Employee
                    {
                        EmployeeCode = NewEmployeeCode,
                        MothersFullName = employee.MothersFullName,
                        BirthPlace = employee.BirthPlace,
                        Nation = employee.Nation,
                        Region = employee.Region,
                        Religion = employee.Religion,
                        MartialStatus = employee.MartialStatus,
                        Status = employee.Status,
                        HiredDate = employee.HiredDate,
                        FirstName = employee.FirstName,//FirstName,
                        MiddleName = employee.MiddleName, //MiddleName ,
                        LastName = employee.LastName,//LastName
                        Nationality = employee.Nationality,
                        DateOfBirth = employee.DateOfBirth,
                        Title = employee.Title,
                        Gender = employee.Gender,
                        CategoryID = employee.CategoryID,
                        IsActive = true,
                        Remark = employee.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = employee.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = employee.ModifiedBy,
                        //DateCreatedAM = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year),
                        //DateLastModifiedAM = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year),
                        FirstNameEnglish = employee.FirstNameEnglish,
                        MiddleNameEnglish = employee.MiddleNameEnglish,
                        LastNameEnglish = employee.LastNameEnglish,
                        DateOfBirthAm = XAPI.EthiopicDateTime.GetEthiopicDate(employee.DateOfBirth.Day, employee.DateOfBirth.Month, employee.DateOfBirth.Year),
                        HiredDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year),
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db.Person_Employee.Add(person);
                    db.SaveChanges();

                    EmployeeIdNew = person.EmployeeId;



                    #region Education
                    foreach (var edu in educations)
                    {
                        var education = new Education
                        {
                            EmployeeId = EmployeeIdNew,
                            OrganizationName = edu.OrganizationName,
                            OrgAddress = edu.OrgAddress,
                            From = edu.From,
                            To = edu.To,
                            Score = edu.Score,
                            EduLevel = edu.EduLevel,
                            EduField = edu.EduField,
                            TotalCost = edu.TotalCost,
                            PercentCoverdByOrg = edu.PercentCoverdByOrg,
                            Remark = edu.Remark,
                            FromEAmharic = edu.FromEAmharic,
                            ToEAmharic = edu.ToEAmharic,
                            IsHighLevel = edu.IsHighLevel,
                        };

                        db.Education.Add(education);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Experience
                    foreach (var exp in experiences)
                    {
                        var experience = new Experience
                        {
                            EmployeeId = EmployeeIdNew,
                            Organozation = exp.Organozation,
                            OrgFullAddress = exp.OrgFullAddress,
                            EmploymentType = exp.EmploymentType,
                            From = exp.From,
                            To = exp.To,
                            TotalDuration = exp.TotalDuration,
                            TotalExp = exp.TotalExp,
                            Responsibility = exp.Responsibility,
                            ClearanceDateTime = exp.ClearanceDateTime,
                            CleranceReason = exp.CleranceReason,
                            IncomeTaxPaymentRefNo = exp.IncomeTaxPaymentRefNo,
                            Remark = exp.Remark,
                            Salary = exp.Salary,
                            EmployeeCode = exp.EmployeeCode,
                            FromExAmharic = exp.FromExAmharic,
                            ToExAmharic = exp.ToExAmharic,
                            ClearanceDateTimeAm = exp.ClearanceDateTimeAm,
                            IsPrivate = exp.IsPrivate,
                        };

                        db.Experiences.Add(experience);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Training
                    foreach (var tr in trainings)
                    {
                        var training = new Training
                        {
                            OrganizationName = tr.OrganizationName,
                            EmployeeId = EmployeeIdNew,
                            OrgAddress = tr.OrgAddress,
                            IsTraining = true,
                            From = tr.From,
                            To = tr.To,
                            Score = tr.Score,
                            TotalCost = tr.TotalCost,
                            TrainingName = tr.TrainingName,
                            PercentCoverdByOrg = tr.PercentCoverdByOrg,
                            Remark = tr.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = tr.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = tr.ModifiedBy,
                            FromTrAm = tr.FromTrAm,
                            ToTraAm = tr.ToTraAm
                        };

                        db.EmpTrainingEducations.Add(training);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Language
                    foreach (var lg in languages)
                    {
                        var language = new LanguageProficiency
                        {
                            EmployeeId = EmployeeIdNew,
                            LanguageId = lg.LanguageId,
                            Reading = lg.Reading,
                            Listening = lg.Listening,
                            Writing = lg.Writing,
                            Speaking = lg.Speaking,
                            Remark = lg.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = lg.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = lg.ModifiedBy
                        };

                        db.LanguageOfProficiencies.Add(language);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Competency
                    foreach (var cmp in competencies)
                    {
                        var comptncy = new Competency
                        {
                            EmployeeId = EmployeeIdNew,
                            Level = cmp.Level,
                            LookupId = cmp.LookupId,
                            Status = cmp.Status,
                            From = cmp.From,
                            To = cmp.To,
                            Remark = cmp.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = cmp.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = cmp.ModifiedBy,
                            FromComAm = cmp.FromComAm,
                            ToCompAm = cmp.ToCompAm,
                        };

                        db.Competencies.Add(comptncy);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Association
                    foreach (var assos in associasions)
                    {
                        var association = new EmployeeAssociation
                        {
                            AssociationId = assos.AssociationId,
                            Name = assos.Name,
                            LookupId = assos.LookupId,
                            EmployeeId = EmployeeIdNew,

                            Remark = assos.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = assos.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = assos.ModifiedBy
                        };

                        db.EmployeeAssociation.Add(association);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Emergencies
                    foreach (var em in emergencies)
                    {
                        var emergency = new Emergency
                        {
                            EmergencyId = em.EmergencyId,
                            Name = em.Name,
                            LookupId = em.LookupId,
                            EmployeeId = EmployeeIdNew,
                            HomePhoneNo = em.HomePhoneNo,
                            MobilePhoneNo = em.MobilePhoneNo,
                            WorkPhoneno = em.WorkPhoneno,
                            Remark = em.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = em.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = em.ModifiedBy
                        };

                        db.Emergencies.Add(emergency);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Disabilities
                    foreach (var disab in disabilities)
                    {
                        var disability = new Disability
                        {
                            EmployeeId = EmployeeIdNew,
                            Type = disab.Type,
                            Remark = disab.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = disab.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = disab.ModifiedBy
                        };

                        db.Disabilities.Add(disability);
                        db.SaveChanges();

                    }
                    #endregion

                    #region Address
                    foreach (var add in addresses)
                    {
                        var address = new Address
                        {
                            Reference = EmployeeIdNew.ToString(),
                            Category = add.Category,
                            Value = add.Value,
                            ReferenceType = add.ReferenceType,
                            Remark = add.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = add.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = add.ModifiedBy
                        };

                        db.Addresses.Add(address);
                        db.SaveChanges();

                    }
                    #endregion

                    #region Identification
                    foreach (var iden in identifications)
                    {
                        var identification = new Identification
                        {
                            Reference = EmployeeIdNew.ToString(),
                            ReferenceType = iden.ReferenceType,
                            Description = iden.Description,
                            IdNumber = iden.IdNumber,
                            Type = iden.Type,
                            IssueDate = iden.IssueDate,
                            ExpiryDate = iden.ExpiryDate,
                            Issuer = iden.Issuer,
                            Remark = iden.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = iden.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = iden.ModifiedBy,
                            IssueDateIdAm = iden.IssueDateIdAm,
                            ExpiryDateAm = iden.ExpiryDateAm
                        };

                        db.Identifications.Add(identification);
                        db.SaveChanges();

                    }
                    #endregion

                    #region Attachment Photo
                    foreach (var atach in attachmentsPhoto)
                    {
                        var entity = new Attachment
                        {
                            ReferenceType = atach.ReferenceType,
                            Reference = EmployeeIdNew.ToString(),
                            Description = atach.Description,
                            URL = atach.URL,
                            BinaryFile = atach.BinaryFile,
                            FileType = atach.FileType,
                            Index = atach.Index,
                            Remark = EmployeeIdNew.ToString(),
                            FileName = atach.FileName
                        };

                        db.Attachments.Add(entity);
                        db.SaveChanges();

                    }
                    #endregion

                    #region Attachment Document
                    foreach (var atachD in attachmentsDocument)
                    {
                        var entity = new Attachment
                        {
                            ReferenceType = atachD.ReferenceType,
                            Reference = EmployeeIdNew.ToString(),
                            Description = atachD.Description,
                            URL = atachD.URL,
                            BinaryFile = atachD.BinaryFile,
                            FileType = atachD.FileType,
                            Index = atachD.Index,
                            Remark = EmployeeIdNew.ToString(),
                            FileName = atachD.FileName
                        };

                        db.Attachments.Add(entity);
                        db.SaveChanges();

                    }
                    #endregion

                    #region Contracts
                    foreach (var cont in contracts)
                    {
                        var contract = new Contract
                        {
                            ContractId = cont.ContractId,
                            From = cont.From,
                            To = cont.To,
                            MonthlySalary = cont.MonthlySalary,
                            EmployeeId = EmployeeIdNew,
                            Remark = cont.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = cont.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = cont.ModifiedBy,
                            FromContAm = cont.FromContAm,
                            ToContAm = cont.ToContAm
                        };

                        db.Contract.Add(contract);
                        db.SaveChanges();

                    }
                    #endregion
                }
                else if (employee.IsActive)
                {
                    var duplication = new
                    {
                        Message = "This Employee is active can't duplicate",
                        Status = "Fail"
                    };

                    return Json(duplication, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var nulReturn = new
                    {
                        Message = "No such employee",
                        Status = "Fail"
                    };

                    return Json(nulReturn, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var formatedData1 = new
                {
                    Message = "First enter the new ID",
                    Status = "OK"
                };

                return Json(formatedData1, JsonRequestBehavior.AllowGet);
            }

            var formatedData2 = new
            {
                Message = "Successfully Generated",
                Status = "OK"
            };

            return Json(formatedData2, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Refresh()
        {
            var allEmployee = db.Person_Employee.ToList();
            var placements = db.Placements.ToList();

            foreach (var p in allEmployee)
            {
                try
                {
                    if (p.IsActive)
                    {
                        var placement = placements.FirstOrDefault(x => x.EmployeeId == p.EmployeeId && x.isCurrent);
                        if (placement != null)
                        {
                            p.BranchId = placement.BranchId;
                            p.OrgBranchName = placement.BranchId.ToString();

                            db.Entry(p).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var placement = placements.Where(x => x.EmployeeId == p.EmployeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
                        if (placement != null)
                        {
                            p.BranchId = placement.BranchId;
                            p.OrgBranchName = placement.OrgBranchName.ToString();

                            db.Entry(p).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                catch(Exception ex)
                {
                    var formatedData1 = new
                    {
                        Message = $"Error occured on Emplyee {p.EmployeeCode} - {ex.Message}",
                        Status = "NOT_OK"
                    };
                    return Json(formatedData1, JsonRequestBehavior.AllowGet);
                }
            }
            
            var formatedData = new
            {
                Message = "Branch successfully updated",
                Status = "OK"
            };
            return Json(formatedData, JsonRequestBehavior.AllowGet);
            //var messages = "branch successfully refreshed";

            //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            ////return RedirectToAction("Index");

        }
        public JsonResult GetEducationFiled()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_FIELD).AsQueryable();
            ViewData["EduFiledList"] = lookups;
            return Json(lookups.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEducationLevel()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL).AsQueryable();
            ViewData["EduLevelList"] = lookups;
            return Json(lookups.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        //[HttpPost]
        //public ActionResult Upload([DataSourceRequest] DataSourceRequest request, AttachmentUploadViewModel model)
        //{
        //    if (model.File == null || model.File.ContentLength == 0)
        //    {
        //        ModelState.AddModelError("File", "File is empty.");
        //        return Json(new List<Attachment>().ToDataSourceResult(request, ModelState));
        //    }

        //    var extension = Path.GetExtension(model.File.FileName).ToLower();
        //    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
        //    if (!allowedExtensions.Contains(extension))
        //    {
        //        ModelState.AddModelError("File", "Invalid file type. Only images are allowed.");
        //        return Json(new List<Attachment>().ToDataSourceResult(request, ModelState));
        //    }

        //    const int maxFileSize = 2 * 1024 * 1024; // 2MB
        //    if (model.File.ContentLength > maxFileSize)
        //    {
        //        ModelState.AddModelError("File", $"File exceeds {maxFileSize / (1024 * 1024)} MB.");
        //        return Json(new List<Attachment>().ToDataSourceResult(request, ModelState));
        //    }

        //    Build path
        //    var folderName = $"{model.EmployeeId}-{model.ReferenceType}";
        //    var folderPath = Path.Combine(Server.MapPath("~/Content/Images/"), folderName);
        //    if (!Directory.Exists(folderPath))
        //        Directory.CreateDirectory(folderPath);

        //    var fileName = $"{model.EmployeeId}{extension}";
        //    var path = Path.Combine(folderPath, fileName);

        //    model.File.SaveAs(path);

        //    Create Attachment record
        //   var attachment = new Attachment
        //   {
        //       Reference = model.EmployeeId.ToString(),
        //       ReferenceType = model.ReferenceType,
        //       Description = model.File.FileName,
        //       FileType = model.File.ContentType,
        //       URL = Url.Content($"~/Content/Images/{folderName}/{fileName}"),
        //       Remark = model.Remark,
        //       Index = 0
        //   };

        //    db.Attachments.Add(attachment);
        //    db.SaveChanges();

        //    Return the uploaded attachment for Kendo Grid
        //    return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        //}

        [HttpPost]
        public ActionResult Upload([DataSourceRequest] DataSourceRequest request, HttpPostedFileBase file)
        {
            try
            {
                if (file == null || file.ContentLength == 0)
                {
                    ModelState.AddModelError("File", "File is Empty.");

                    return Json(new List<Attachment>().ToDataSourceResult(request, ModelState));

                }

                // Validate extension and MIME type
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension) || !allowedMimeTypes.Contains(file.ContentType.ToLower()))
                {
                    ModelState.AddModelError("File", "Invalid file type. Only images are allowed.");
                    return Json(new List<Attachment>().ToDataSourceResult(request, ModelState));
                }

                const int maxFileSize = 2 * 1024 * 1024; // 2MB
                if (file.ContentLength > maxFileSize)
                {
                    ModelState.AddModelError("File", $"File exceeds {maxFileSize / (1024 * 1024)} MB.");
                    return Json(new[] { file }.ToDataSourceResult(request, ModelState));
                }
                var employeeId = HRMInputs.EmployeeId;
                // Build path
                var folderName = $"{employeeId}-{AttachmentType.PHOTO}";
                var folderPath = Path.Combine(Server.MapPath("~/Content/Images/"), folderName);
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileName = $"{employeeId}{extension}";
                var path = Path.Combine(folderPath, fileName);

                // Delete existing file if exists
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);

                // Save new file
                file.SaveAs(path);

                // Remove existing attachment records
                var existing = db.Attachments.Where(a => a.Reference == employeeId.ToString() && a.ReferenceType == AttachmentType.PHOTO);
                if (existing.Any())
                {
                    db.Attachments.RemoveRange(existing);
                    db.SaveChanges();
                }

                // Add new attachment record
                var employee = db.Person_Employee.Find(employeeId);
                var attachment = new Attachment
                {
                    Reference = employeeId.ToString(),
                    Description = employee != null ? $"{employee.EmployeeCode}{extension}" : file.FileName,
                    FileType = file.ContentType,
                    ReferenceType = AttachmentType.PHOTO,
                    Index = 0,
                    URL = path,
                    Remark = HRMInputs.Owner
                };

                db.Attachments.Add(attachment);
                db.SaveChanges();

                return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("File", $"Upload failed: {ex.Message}");
                return Json(new List<Attachment>().ToDataSourceResult(request, ModelState));
            }
        }

        //public ActionResult Upload([DataSourceRequest] DataSourceRequest request, HttpPostedFileBase file)
        //{
        //    try
        //    {
        //        if (file != null && file.ContentLength > 0)
        //        {
        //            int MaxFileSize = 2 * 1024 * 1024;
        //            // Allowed extensions
        //            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };

        //            var fileExtension = Path.GetExtension(file.FileName).ToLower();

        //            if (!allowedExtensions.Contains(fileExtension))
        //            {
        //                ModelState.AddModelError("File", $"Invalid file type. Please upload only image files (.jpg, .jpeg, .png, .gif).");

        //                return Json(new[] { file }.ToDataSourceResult(request, ModelState));

        //            }
        //            if (file.ContentLength > MaxFileSize)
        //            {
        //                ModelState.AddModelError("File", $"The uploaded file exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)} MB.");
        //                return Json(new[] { file }.ToDataSourceResult(request, ModelState));

        //            }
        //            // Allowed MIME types (extra validation)
        //            var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
        //            if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
        //            {
        //                ModelState.AddModelError("File", $"Invalid file type. Please upload only image files (.jpg, .jpeg, .png, .gif).");
        //                return Json(new[] { file }.ToDataSourceResult(request, ModelState));

        //            }

        //            var folderName = HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.PHOTO;
        //            var mainPath = Path.Combine(Server.MapPath("~/Content/Images/"), folderName);

        //            if (!Directory.Exists(mainPath))
        //            {
        //                Directory.CreateDirectory(mainPath);
        //            }

        //            var fileName = HRMInputs.EmployeeId.ToString() + fileExtension;
        //            var path = Path.Combine(mainPath, fileName);

        //            if (System.IO.File.Exists(path))
        //            {
        //                try
        //                {
        //                    System.IO.File.Delete(path);
        //                }
        //                catch (System.IO.IOException e)
        //                {
        //                    Console.WriteLine(e.Message);
        //                }
        //            }

        //            file.SaveAs(path);

        //            var employeeCode = db.Person_Employee.Find(HRMInputs.EmployeeId);

        //            var checkExistance = db.Attachments
        //                .Where(x => x.Reference == HRMInputs.EmployeeId.ToString()
        //                         && x.ReferenceType == AttachmentType.PHOTO)
        //                .ToList();

        //            if (checkExistance.Any())
        //            {
        //                db.Attachments.RemoveRange(checkExistance);
        //                db.SaveChanges();
        //            }

        //            db = new ExceedDbContext();
        //            var entity = new Attachment
        //            {
        //                Reference = HRMInputs.EmployeeId.ToString(),
        //                Description = employeeCode != null
        //                                ? employeeCode.EmployeeCode + fileExtension
        //                                : file.FileName,
        //                FileType = file.ContentType,
        //                ReferenceType = AttachmentType.PHOTO,
        //                Index = 0,
        //                URL = path,
        //                Remark = HRMInputs.Owner
        //            };
        //            db.Attachments.Add(entity);
        //            db.SaveChanges();

        //            return Json(new[] { file }.ToDataSourceResult(request, ModelState));

        //        }

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        ModelState.AddModelError("File", $"Upload failed.");

        //        return Json(new[] { file }.ToDataSourceResult(request, ModelState));

        //    }
        //}

        //public ActionResult Upload(HttpPostedFileBase file)
        //{
        //    try
        //    {
        //        if (file.ContentLength > 0)
        //        {
        //            var fileExtension = Path.GetFileName(file.FileName).Split('.')[1];
        //            var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.PHOTO), HRMInputs.EmployeeId.ToString() + "." + fileExtension);
        //            var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.PHOTO));

        //            if (!Directory.Exists(mainPath))
        //            {
        //                Directory.CreateDirectory(mainPath);
        //            }
        //            if (System.IO.File.Exists(path))
        //            {

        //                try
        //                {
        //                    System.IO.File.Delete(path);
        //                }
        //                catch (System.IO.IOException e)
        //                {
        //                    Console.WriteLine(e.Message);
        //                    //  return;
        //                }
        //            }
        //            file.SaveAs(path);
        //            var employeeCode = db.Person_Employee.Find(HRMInputs.EmployeeId);

        //            var checkExistance = db.Attachments.Where(x => x.Reference == HRMInputs.EmployeeId.ToString() && x.Description == HRMInputs.EmployeeId.ToString() + "." + fileExtension && x.ReferenceType == AttachmentType.PHOTO).ToList();
        //            if (checkExistance != null)
        //            {
        //                db.Attachments.RemoveRange(checkExistance);
        //                db.SaveChanges();
        //            }
        //            db = new ExceedDbContext();
        //            var entity = new Attachment
        //            {

        //                Reference = HRMInputs.EmployeeId.ToString(),
        //                Description = employeeCode != null ? employeeCode.EmployeeCode + "." + fileExtension : file.FileName,
        //                FileType = file.ContentType,
        //                ReferenceType = AttachmentType.PHOTO,
        //                Index = 0,
        //                URL = path,
        //                Remark = HRMInputs.Owner
        //            };
        //            db.Attachments.Add(entity);

        //            db.SaveChanges();
        //            ViewBag.Message = "Upload successful";
        //        }

        //        //}
        //        //else
        //        //{

        //        //    ViewBag.Message = "The file is existed";

        //        //}
        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        ViewBag.Message = "Upload failed";
        //        return RedirectToAction("Upload");
        //    }
        //}
        public ActionResult UploadDocument(HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT), HRMInputs.EmployeeId.ToString() + "-" + txtValue + "-" + fileName);

                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT));

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        file.SaveAs(path);
                        var employeeCode = db.Person_Employee.Find(HRMInputs.EmployeeId);

                        var entity = new Attachment
                        {

                            Reference = HRMInputs.EmployeeId.ToString(),
                            Description = employeeCode != null ? employeeCode.EmployeeCode  + "-" + txtValue + "-" + fileName: txtValue + "-" + fileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = HRMInputs.Owner
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public ActionResult UploadOtherDocument(HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT), HRMInputs.EmployeeId.ToString() + "-" + txtValue + "-" + fileName);

                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT));

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        file.SaveAs(path);
                        var employeeCode = db.Person_Employee.Find(HRMInputs.EmployeeId);

                        var entity = new Attachment
                        {

                            Reference = HRMInputs.EmployeeId.ToString(),
                            Description = employeeCode != null ? employeeCode.EmployeeCode  + "-" + txtValue + "-" + fileName: txtValue + "-" + fileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.OTHER,
                            Index = 0,
                            URL = path,
                            Remark = HRMInputs.Owner
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public ActionResult DownloadFile(string filename, string attachId)
        {
            if (attachId == "0")
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            try
            {
                int.TryParse(attachId, out int attachmentId);

                var attachment = db.Attachments.FirstOrDefault(x => x.Reference == filename && x.ReferenceType == AttachmentType.PHOTO && x.AttachmentId == attachmentId);

                if (attachment != null)
                {
                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition",
                        "attachment;filename=\"" + attachment.Description + "\"");
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.PHOTO + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (Exception)
            {

                // throw;
            }
            HRMInputs.SelectedFile = 0;
            return null;
        }
        public ActionResult DownloadDoument(string filename, string attachId)
        {
            if (attachId == "0")
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            //try
            //{
            int.TryParse(attachId, out int attachmentId);
            var attachment =
                    db.Attachments.FirstOrDefault(x => x.Reference == filename && x.ReferenceType == AttachmentType.DOCUMENT && x.AttachmentId == attachmentId);

            if (attachment != null)
            {
                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition",
                    "attachment;filename=\"" + attachment.Description + "\"");
                byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT + "/" + attachment.Description));

                // byte[] datas11 = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.PHOTO + "/" + attachment.Description));

                response.BinaryWrite(datas);
                response.End();
                HRMInputs.SelectedFile = 0;
                return File(filename, attachment.FileType);
            }
            //}
            //catch (Exception)
            //{

            //    // throw;
            //}
            HRMInputs.SelectedFile = 0;
            return null;
        }
        public ActionResult DownloadAttachment(int attachmentId)
        {
            var record = db.Attachments.Find(attachmentId);

            return File(record.URL, "application/force-download", Path.GetFileName(record.URL));
        }
       
        public void CheckEmployeeRetirment()
        {
            int maxAge;
            List<string> employeesList = inputs.GetUserBranchEmployees(User.Identity.Name);
            var employees = db.Person_Employee
                                    .Where(x => employeesList.Contains(x.EmployeeId.ToString()))
                                    .Where(x => x.IsActive 
                                                && !x.IsReHired 
                                                && x.Remark != "R-Active" 
                                                && x.Status != EmployeeStatus.CONTRACT);
            db = new ExceedDbContext();
            bool isActive = true;
            var terminations = db.Terminations;
            var setting = db.Configurations.FirstOrDefault(x => x.Reference == "PENSION" && x.Attribute == "RETIREMENT_AGE");
            if (setting != null)
            {
                maxAge = Convert.ToInt32(setting.CurrentValue);
                foreach (var e in employees)
                {
                    double age = DateTime.Now.Date.Subtract(new DateTime(e.DateOfBirth.Year, e.DateOfBirth.Month, e.DateOfBirth.Day)).TotalDays / 365.25;
                    if (terminations.Any())
                    {
                        var checkTermintedEmp = terminations.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);

                        if (checkTermintedEmp == null)
                        {
                            isActive = true;
                        }
                        else
                        {
                            if (checkTermintedEmp.TerminationDate.Value.Date >= DateTime.Now.Date)
                            {
                                isActive = false;
                            }
                        }
                    }

                    if (age >= maxAge)
                    {
                        isActive = false;
                        if (e.IsActive)
                        {
                            db = new ExceedDbContext();
                            var entity = new Person_Employee
                            {
                                EmployeeId = e.EmployeeId,
                                EmployeeCode = e.EmployeeCode,
                                MothersFullName = e.MothersFullName,
                                BirthPlace = e.BirthPlace,
                                Nation = e.Nation,
                                Region = e.Region,
                                Religion = e.Religion,
                                MartialStatus = e.MartialStatus,
                                Status = e.Status,
                                HiredDate = e.HiredDate,
                                FirstName = e.FirstName,
                                MiddleName = e.MiddleName,
                                LastName = e.LastName,
                                Nationality = e.Nationality,
                                DateOfBirth = e.DateOfBirth,
                                Title = e.Title,
                                Gender = e.Gender,
                                CategoryID = e.CategoryID,
                                IsActive = isActive,
                                Remark = "Retirement ",
                                DateCreated = e.DateCreated,
                                LastModified = DateTime.Now,
                                CreatedBy = e.CreatedBy,
                                ModifiedBy = User.Identity.Name,
                                FirstNameEnglish = e.FirstNameEnglish,
                                MiddleNameEnglish = e.MiddleNameEnglish,
                                LastNameEnglish = e.LastNameEnglish,
                                DateOfBirthAm = e.DateOfBirthAm,
                                HiredDateAmharic = e.HiredDateAmharic,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                            };
                            db = new ExceedDbContext();
                            db.Person_Employee.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else if (e.IsActive == false && e.Remark == "Retirement")
                    {
                        db = new ExceedDbContext();
                        var entity = new Person_Employee
                        {
                            EmployeeId = e.EmployeeId,
                            EmployeeCode = e.EmployeeCode,
                            MothersFullName = e.MothersFullName,
                            BirthPlace = e.BirthPlace,
                            Nation = e.Nation,
                            Region = e.Region,
                            Religion = e.Religion,
                            MartialStatus = e.MartialStatus,
                            Status = e.Status,
                            HiredDate = e.HiredDate,
                            FirstName = e.FirstName,
                            MiddleName = e.MiddleName,
                            LastName = e.LastName,
                            Nationality = e.Nationality,
                            DateOfBirth = e.DateOfBirth,
                            Title = e.Title,
                            Gender = e.Gender,
                            CategoryID = e.CategoryID,
                            IsActive = isActive,
                            Remark = "Retirement",
                            DateCreated = e.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = e.CreatedBy,
                            ModifiedBy = User.Identity.Name,
                            FirstNameEnglish = e.FirstNameEnglish,
                            MiddleNameEnglish = e.MiddleNameEnglish,
                            LastNameEnglish = e.LastNameEnglish,
                            DateOfBirthAm = e.DateOfBirthAm,
                            HiredDateAmharic = e.HiredDateAmharic,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        };
                        db = new ExceedDbContext();
                        db.Person_Employee.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        //db = new ExceedDbContext();
                        //var entity = new Person_Employee
                        //{
                        //    EmployeeId = e.EmployeeId,
                        //    EmployeeCode = e.EmployeeCode,
                        //    MothersFullName = e.MothersFullName,
                        //    BirthPlace = e.BirthPlace,
                        //    Nation = e.Nation,
                        //    Region = e.Region,
                        //    Religion = e.Religion,
                        //    MartialStatus = e.MartialStatus,
                        //    Status = e.Status,
                        //    HiredDate = e.HiredDate,
                        //    FirstName = e.FirstName,
                        //    MiddleName = e.MiddleName,
                        //    LastName = e.LastName,
                        //    Nationality = e.Nationality,
                        //    DateOfBirth = e.DateOfBirth,
                        //    Title = e.Title,
                        //    Gender = e.Gender,
                        //    CategoryID = e.CategoryID,
                        //    IsActive = isActive,
                        //    Remark = e.Remark,
                        //    DateCreated = e.DateCreated,
                        //    LastModified = DateTime.Now,
                        //    CreatedBy = e.CreatedBy,
                        //    ModifiedBy = User.Identity.Name,
                        //    FirstNameEnglish = e.FirstNameEnglish,
                        //    MiddleNameEnglish = e.MiddleNameEnglish,
                        //    LastNameEnglish = e.LastNameEnglish,
                        //    DateOfBirthAm = e.DateOfBirthAm,
                        //    HiredDateAmharic = e.HiredDateAmharic,
                        //    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        //};
                        ////db = new ExceedDbContext();
                        //db.Person_Employee.Attach(entity);
                        //db.Entry(entity).State = EntityState.Modified;
                        //db.SaveChanges();
                    }
                }
            }

        }
        private IQueryable<Person_Employee> ModifyFilters(IEnumerable<IFilterDescriptor> filters, IQueryable<Person_Employee> person_employee)
        {
            IQueryable<Person_Employee> person_employees = new Person_Employee[] { }.AsQueryable(); ;
            if (filters.Any())
            {
                string searchedValue = "";

                dd: if (filters.Any())
                {

                    foreach (var filter in filters)
                    {
                        var descriptor = filter as FilterDescriptor;
                        if (descriptor != null)
                        {
                            if (descriptor.Member == "FirstName")
                            {

                                searchedValue = descriptor.Value.ToString();
                                var value = searchedValue;
                                if (person_employees.Any())
                                {
                                    var person =
                                        person_employees.ToList().Where(
                                            j =>
                                                j.FirstName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                value.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    person_employees = person;
                                }
                                else
                                {
                                    var person =
                                           person_employee.ToList().Where(
                                               j =>
                                                   j.FirstName.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                   value.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    person_employees = person_employees.Union(person).AsQueryable();
                                }

                            }
                            if (descriptor.Member == "EmployeeCode")
                            {

                                searchedValue = descriptor.Value.ToString();
                                var value = searchedValue;
                                if (person_employees.Any())
                                {
                                    var person =
                                        person_employees.ToList().Where(
                                            j =>
                                                j.EmployeeCode.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                value.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    person_employees = person;
                                }
                                else
                                {
                                    var person =
                                           person_employee.ToList().Where(
                                               j =>
                                                   j.EmployeeCode.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                   value.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    person_employees = person_employees.Union(person).AsQueryable();
                                }

                            }
                            if (descriptor.Member == "MiddleName")
                            {

                                searchedValue = descriptor.Value.ToString();
                                var MiddleName = searchedValue;
                                if (person_employees.Any())
                                {
                                    var Middle =
                                        person_employees.ToList().Where(
                                            j =>
                                                j.MiddleName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(MiddleName.ToUpper(XAPI.Common.exceedculture)) ==
                                                MiddleName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(MiddleName.ToUpper(XAPI.Common.exceedculture)))
                                            .AsQueryable();
                                    person_employees = Middle;
                                }
                                else
                                {
                                    var Middle =
                                        person_employee.ToList().Where(
                                            j =>
                                                j.MiddleName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(MiddleName.ToUpper(XAPI.Common.exceedculture)) ==
                                                MiddleName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(MiddleName.ToUpper(XAPI.Common.exceedculture)))
                                            .AsQueryable();
                                    person_employees = person_employees.Union(Middle);
                                }


                            }

                            if (descriptor.Member == "LastName")
                            {
                                searchedValue = descriptor.Value.ToString();
                                var LastName = searchedValue;
                                if (person_employees.Any())
                                {
                                    var Last =
                                        person_employees.ToList().Where(
                                            j =>
                                                j.LastName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(LastName.ToUpper(XAPI.Common.exceedculture)) ==
                                                LastName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(LastName.ToUpper(XAPI.Common.exceedculture)))
                                            .AsQueryable();
                                    person_employees = Last;

                                }
                                else
                                {
                                    var Last =
                                       person_employee.ToList().Where(
                                           j =>
                                               j.LastName.ToUpper(XAPI.Common.exceedculture)
                                                   .Contains(LastName.ToUpper(XAPI.Common.exceedculture)) ==
                                               LastName.ToUpper(XAPI.Common.exceedculture)
                                                   .Contains(LastName.ToUpper(XAPI.Common.exceedculture)))
                                           .AsQueryable();

                                    person_employees = person_employees.Union(Last);
                                }
                            }
                        }
                        else if (filter is CompositeFilterDescriptor)
                        {

                            filters = ((CompositeFilterDescriptor)filter).FilterDescriptors;
                            goto dd;
                            //ModifyFilters(((CompositeFilterDescriptor) filter).FilterDescriptors, person_employee);

                        }

                    }

                }
            }
            return person_employees.AsQueryable();
        }
        public bool Probation(int id)
        {
            var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id && x.IsActive);
            if (personEmployee == null)
                return false;
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == id);
            int probationdDays = 60;
            HashSet<DateTime> holidays =new HashSet<DateTime>( db.Holidays.Select(x => x.Date));
            DateTime currentDate = personEmployee.HiredDate.Date;
            DateTime today = DateTime.Today;

            int workingDays = 0;
            if(placement != null)
            {
                var probation = db.ProbationsSettings.FirstOrDefault(x => x.ResponsibilityLevel == placement.ResponsibilityLevel);
               if(probation ==null)
                    probation = db.ProbationsSettings.FirstOrDefault();

                if (probation != null && probation.ProbationDays > 0)
                    probationdDays = probation?.ProbationDays ?? 60;

            }
            while (currentDate <= today && workingDays < 60)
            {
                if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
                    currentDate.DayOfWeek != DayOfWeek.Sunday &&
                    !holidays.Contains(currentDate))
                {
                    workingDays++;
                }

                currentDate = currentDate.AddDays(1);
            }

            return workingDays >= 60;
        }
        #region probation 
        public void UpdateContractandProbationStatus()
        {
            var today = DateTime.Today;

            // Pre-fetch all data needed
            var holidays = new HashSet<DateTime>(db.Holidays.Select(x => x.Date));

            // Get all contracts grouped by employee
            var employeeContracts = db.Contract
                .GroupBy(x => x.EmployeeId)
                .Select(g => new {
                    EmployeeId = g.Key,
                    LatestContractEnd = g.Max(x => x.To)
                })
                .ToDictionary(x => x.EmployeeId, x => x.LatestContractEnd);

            // Process all active employees (including those who may have completed probation)
            var activeEmployees = db.Person_Employee
                .Where(x => x.IsActive)
                .Where(x=>!x.IsProbationEnd.HasValue || !x.IsProbationEnd.Value||x.Status==EmployeeStatus.PROBATION)
                .ToList();

            foreach (var employee in activeEmployees)
            {
                bool needsUpdate = false;

                // Update contract days for contract employees (regardless of probation status)
                if (employee.Status == EmployeeStatus.CONTRACT)
                {
                    if (employeeContracts.TryGetValue(employee.EmployeeId, out var contractEndDate))
                    {
                        var daysLeft = (contractEndDate - today).Days;
                        if (employee.ContratLeftDay != daysLeft)
                        {
                            employee.ContratLeftDay = daysLeft;
                            needsUpdate = true;
                        }
                    }
                }

                // Only calculate probation if not already completed
                if (!employee.IsProbationEnd.HasValue || !employee.IsProbationEnd.Value)
                {
                    int workingDays = CalculateWorkingDays(employee.HiredDate.Date, today, holidays, 60);
                    bool probationEnded = workingDays >= 60;

                    if (employee.IsProbationEnd != probationEnded ||
                        employee.ProhabitionState != (probationEnded ? 0 : 2))
                    {
                        employee.IsProbationEnd = probationEnded;
                        employee.ProhabitionState = workingDays < 60 ? 2 : 0;
                        needsUpdate = true;
                    }
                }

                if (needsUpdate)
                {
                    db.Entry(employee).State = EntityState.Modified;
                }
            }

            db.SaveChanges();
        }

        [HttpPost]
        public JsonResult UpdateAllContractandProbationStatus()
        {
            var today = DateTime.Today;

            // Holidays
            var holidays = new HashSet<DateTime>(
                db.Holidays.Select(x => x.Date)
            );

            // Probation settings
            var probationSettings = db.ProbationsSettings.ToList();

            // Current placements
            var placements = db.Placements
                .Where(x => x.isCurrent)
                .ToList();

            var placementLookup = placements
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(x => x.Key, x => x.FirstOrDefault());

            // Latest contract per employee
            var employeeContracts = db.Contract
                .GroupBy(x => x.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    LatestContractEnd = g.Max(x => x.To)
                })
                .ToDictionary(x => x.EmployeeId, x => x.LatestContractEnd);

            // Default probation setting fallback, plus the most recent ProbationPerformance evaluation per employee
            var defaultProbationSetting = probationSettings.FirstOrDefault(x => x.ResponsibilityLevel == null)
                                           ?? probationSettings.FirstOrDefault();
            var latestProbationPerformance = db.ProbationPerformances
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.ProbationPerformanceId).FirstOrDefault());

            // Active employees
            var activeEmployees = db.Person_Employee
                .Where(x => x.IsActive)
                .Where(x =>
                    !x.IsProbationEnd.HasValue ||
                    !x.IsProbationEnd.Value ||
                    x.Status == EmployeeStatus.PROBATION ||
                    x.Status == EmployeeStatus.CONTRACT)
                .ToList();
            int updatedContractCount = 0;
            int updatedProbationCount = 0;

            foreach (var employee in activeEmployees)
            {
                bool needsUpdate = false;

                // ======================================================
                // CONTRACT DAYS UPDATE
                // ======================================================

                if (employee.Status == EmployeeStatus.CONTRACT)
                {
                    if (employeeContracts.TryGetValue(employee.EmployeeId, out var contractEndDate))
                    {
                        var daysLeft = (contractEndDate - today).Days;

                        if (employee.ContratLeftDay != daysLeft)
                        {
                            employee.ContratLeftDay = daysLeft;
                            needsUpdate = true;
                            updatedContractCount++;
                        }
                    }
                }

                // ======================================================
                // PROBATION SETTINGS
                // ======================================================

                int probationDays = probationSettings
                    .FirstOrDefault()?.ProbationDays ?? 60;
                decimal minimumFitValue = defaultProbationSetting?.MinimumFitValue ?? 0;

                // Employee placement
                placementLookup.TryGetValue(employee.EmployeeId, out var placement);

                // Responsibility level specific setting
                if (placement != null)
                {
                    var setting = probationSettings.FirstOrDefault(x =>
                        x.ResponsibilityLevel == placement.ResponsibilityLevel);

                    if (setting != null)
                    {
                        if (setting.ProbationDays > 0)
                        {
                            probationDays = setting.ProbationDays;
                        }
                        minimumFitValue = setting.MinimumFitValue;
                    }
                }

                // ======================================================
                // PROBATION CALCULATION
                // ======================================================

                int workingDays = CalculateWorkingDays(
                    employee.HiredDate.Date,
                    today,
                    holidays,
                    probationDays
                );

                bool probationEnded = workingDays >= probationDays;

                // Update probation fields
                if (employee.IsProbationEnd != probationEnded ||
                    employee.ProhabitionState != (probationEnded ? 0 : 2))
                {
                    employee.IsProbationEnd = probationEnded;
                    employee.ProhabitionState = probationEnded ? 0 : 2;

                    needsUpdate = true;

                }

                // ======================================================
                // AUTO CHANGE STATUS BASED ON PROBATION PERFORMANCE RESULT
                // ======================================================

                if (probationEnded &&
                    employee.Status == EmployeeStatus.PROBATION)
                {
                    latestProbationPerformance.TryGetValue(employee.EmployeeId, out var performance);

                    if (performance == null)
                    {
                        // No evaluation recorded yet - can't decide Fit/UnFit, leave them on probation
                    }
                    else if (performance.Result >= minimumFitValue)
                    {
                        // Fit - confirm as permanent
                        employee.Status = EmployeeStatus.PERMANENT;
                        if (placement != null && placement.EmployementType == EmployeeStatus.PROBATION)
                        {
                            placement.EmployementType = EmployeeStatus.PERMANENT;
                            db.Entry(placement).State = EntityState.Modified;

                        }
                        needsUpdate = true;
                        updatedProbationCount++;
                    }
                    else
                    {
                        // UnFit - take them out of the probation tab and record the termination
                        employee.IsActive = false;
                        employee.IsPlaced = false;
                        needsUpdate = true;
                        updatedProbationCount++;

                        if (placement != null)
                        {
                            placement.IsApprove = false;
                            placement.isCurrent = false;
                            db.Entry(placement).State = EntityState.Modified;
                        }

                        db.Terminations.Add(new ExceedERP.Core.Domain.HRM.EmployeeDetail.Termination
                        {
                            EmployeeId = employee.EmployeeId,
                            LookupId = placement?.LookupId ?? 0,
                            Reason = "probation unfit",
                            ApplicationDate = DateTime.Now,
                            TerminationDate = DateTime.Now,
                            Remark = "Unfit",
                            TerminationDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                            ApplicationDateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                            DateCreated = DateTime.Now,
                            LastModified = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = User.Identity.Name,
                            IsOnlineApproved = true,
                            IsDirect = true,
                            IsForciblyTerminated = true,
                            IsDepartmentSendForApproval = true,
                            IsApproved = true,
                            EmployeeCode = employee.EmployeeCode,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
                        });
                    }
                }

                // ======================================================
                // AUDIT
                // ======================================================

                if (needsUpdate)
                {
                    employee.LastModified = DateTime.Now;

                    db.Entry(employee).State = EntityState.Modified;
                }
            }

            db.SaveChanges();
            return Json(new
            {
                Message = updatedProbationCount + $" employee(s) probation status &{updatedContractCount} employee(s) oin contract updated successfully.",
                Status = "Okay"
            }, JsonRequestBehavior.AllowGet);
        }
        private int CalculateWorkingDays(DateTime startDate, DateTime endDate, HashSet<DateTime> holidays, int maxDays)
        {
            int workingDays = 0;
            DateTime currentDate = startDate;

            while (currentDate <= endDate && workingDays < maxDays)
            {
                if (IsWorkingDay(currentDate, holidays))
                {
                    workingDays++;
                }
                currentDate = currentDate.AddDays(1);
            }

            return workingDays;
        }

        private bool IsWorkingDay(DateTime date, HashSet<DateTime> holidays)
        {
            return date.DayOfWeek != DayOfWeek.Saturday &&
                   date.DayOfWeek != DayOfWeek.Sunday &&
                   !holidays.Contains(date);
        }
        #endregion
        
        public int Branch(int id)
        {
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            if (placement != null)
            {
                return placement.BranchId;
            }
            return 0;
        }
        public ActionResult UpdateExperience()
        {


            var experiences = db.Experiences.ToList();

            foreach (var ex in experiences)
            {


                double month = (ex.To - ex.From).Days / 30;

                var y = (int)month / 12;

                var ym = month % 12;

                var exp = y + " አመት ከ " + ym + " ወር";


                ex.TotalDuration = y;
                ex.TotalExp = exp;


                db.Entry(ex).State = EntityState.Modified;
                db.SaveChanges();

            }

            return RedirectToAction("Index");

        }

        public int? GetContractLeftDay(int id)
        {
            var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id && x.IsActive && x.Status == EmployeeStatus.CONTRACT);
            if (profile != null)
            {
                var contract = db.Contract.Where(x => x.EmployeeId == profile.EmployeeId).ToList().OrderByDescending(x => x.To).FirstOrDefault();
                if (contract != null)
                {
                    int leftDay = (contract.To - DateTime.Today).Days;

                    return leftDay;
                }
            }

            return null;
        }

        private void UpdateEmployeeBranch(List<Placement> placments)
        {
            foreach (var placement in placments)
            {
                var employee = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == placement.EmployeeId);
                if (employee != null)
                {
                    employee.Status = placement.EmployementType;
                    employee.OrgBranchName = placement.BranchId.ToString();
                    employee.ModifiedBy = User.Identity.Name;
                    employee.LastModified = DateTime.Now;

                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        private int CheckProhabition(int empId)
        {
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
            if (employee == null || employee.Status != EmployeeStatus.PROBATION)
                return 1;

            var prohibition = db.Prohibitions.FirstOrDefault(x => x.EmployeeId == empId);
            if (prohibition == null)
                return 1;

            // Load holidays once as a HashSet for fast lookup
            HashSet<DateTime> holidays = new HashSet<DateTime>( db.Holidays.Select(x => x.Date));

            int workingDays = 0;
            DateTime currentDate = employee.HiredDate.Date;
            DateTime endDate = prohibition.EndDate.Date;

            while (currentDate <= endDate && workingDays < 60)
            {
                if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
                    currentDate.DayOfWeek != DayOfWeek.Sunday &&
                    !holidays.Contains(currentDate))
                {
                    workingDays++;
                }
                currentDate = currentDate.AddDays(1);
            }

            return workingDays < 60 ? 2 : 0;
        }
        private List<int> GetEmployeeIdInProhabition(List<Person_Employee> employees)
        {
            List<int> employeeIds = new List<int>(); 
            int probationAmount = 0;
            employees = employees.Where(x => (DateTime.Now.Date-x.HiredDate.Date ).TotalDays < 90).ToList();
           
            foreach (var employee in employees)
            {
                
                List<DateTime> holidays = db.Holidays.Select(x => x.Date).ToList();
                probationAmount = Enumerable.Range(0, 60)
               .Select(offset => employee.HiredDate.AddDays(1))
               .Where(date => date.DayOfWeek != DayOfWeek.Sunday && !holidays.Contains(date))
               .Count();
                if(probationAmount <= 60)
                {

                    employeeIds.Add(employee.EmployeeId);
                }
                 
            }
            return employeeIds;
        }
        private List<int> GetContactEmployeeId(List<ContactVM> employees)
        {
            List<int> employeeIds = new List<int>(); 
            foreach (var employee in employees)
            {
                var contract = db.Contract.Where(x =>x.EmployeeId==employee.EmployeeId).Where(x=>x.To>=employee.AssignmentDate&&x.To>=DateTime.Now).OrderByDescending(x=>x.To).FirstOrDefault();
                if(contract != null&& ((contract.To.Date - DateTime.Now.Date).TotalDays<=30))
                {
                    employeeIds.Add(employee.EmployeeId);


                }
                 
            }
            return employeeIds;
        }
        public void GetEmployeePhoto(IQueryable<Person_Employee> person_employee)
        {
            EmployeeIdView view = new EmployeeIdView();
            List<EmployeeIdView> views = new List<EmployeeIdView>();
            foreach (var person in person_employee)
            {
                view = new EmployeeIdView();
                Attachment att = db.Attachments.Where(a => a.Reference == person.EmployeeId.ToString() && a.ReferenceType == AttachmentType.PHOTO).FirstOrDefault();
                view.PersonId = person.EmployeeId;
                view.FullName = person.FirstName;
                if (att != null)
                {

                    view.URL = att.URL;
                }
                HRMInputs.EmployeeId = person.EmployeeId;
            }


        }
        public string GenerateEmployeeCode(DateTime date)
        {
            var empCode = "";
            var name = "";
            var sequenceNo = 0;
            int digitLength = 0;
            var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == EmployeeIdAttribute.Auto_Generate.ToString());
            if (config != null)
            {
                var empCodeSettings = db.EmployeeCodeSettings.OrderBy(x => x.Index).ToList();
                List<string> emp = new List<string>();
                if (empCodeSettings.Any())
                {
                    foreach (var code in empCodeSettings)
                    {
                        if (code.Type == EmployeeIDAbbreviationType.Company)
                        {
                            var company = db.Companies.FirstOrDefault();
                            if (company != null)
                            {
                                emp.Add(company.Abbreviation);

                                name = company.Abbreviation;
                            }
                        }
                        if (code.Type == EmployeeIDAbbreviationType.Sequential_Number)
                        {
                            var company = db.Companies.FirstOrDefault();
                            if (company != null)
                            {
                                var setting = db.EmployeeCodeSequenceSettings.Where(x => x.Name == company.Abbreviation).ToList();
                                if (setting.Any())
                                {
                                    var last = (setting.OrderByDescending(x => x.IndexCount).FirstOrDefault().IndexCount + 1).ToString();
                                    var length = setting.OrderByDescending(x => x.IndexCount).FirstOrDefault().ItemCodeTrim;
                                    //for (int i = 1; i <= length; i++)
                                    //{
                                    //    last = "0" + last;
                                    //}
                                    string newId=last.PadLeft(length, '0');
                                    emp.Add(newId);

                                    sequenceNo = setting.OrderByDescending(x => x.IndexCount).FirstOrDefault().IndexCount + 1;
                                    digitLength = length;
                                }
                                else
                                {
                                    emp.Add("1");
                                }
                            }
                            else
                            {
                                emp.Add("1");
                            }
                        }
                        if (code.Type == EmployeeIDAbbreviationType.Year)
                        {
                            emp.Add(date.Year.ToString());

                        }

                    }

                    for (int i = 0; i < emp.Count(); i++)
                    {
                        if (i == (emp.Count() - 1))
                        {
                            empCode = empCode + emp[i];
                        }
                        else
                        {
                            empCode = empCode + emp[i] + "-";
                        }

                    }

                }
                var employeecode = db.EmployeeCodeSequenceSettings.Where(x => x.Name.Trim() == name.Trim()).FirstOrDefault();
                if (employeecode !=null)
                {

                    employeecode.ItemCodeTrim = digitLength;
                    employeecode.IndexCount = sequenceNo;
                    employeecode.Name = name;
                    employeecode.Status = true;
                   

                    db.EmployeeCodeSequenceSettings.Attach(employeecode);
                    db.Entry(employeecode).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var idSequence = new EmployeeCodeSequenceSetting
                    {
                        ItemCodeTrim = digitLength,
                        IndexCount = sequenceNo,
                        Name = name,
                        Status = true,
                    };

                    db.EmployeeCodeSequenceSettings.Add(idSequence);
                    db.SaveChanges();
                }
            }

            return empCode;

        }
        [AuthorizeRoles(HrmConstants.HRProfileUser, HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        public ActionResult Person_Employee_InProbation_Read([DataSourceRequest] DataSourceRequest request)
        {
            var today = DateTime.Today;

            //  Get user branches
            List<int> mybranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            // Probation settings (loaded first - needed to bound the base query below)
            var probationSettings = db.ProbationsSettings.ToList();
            var defaultProbationSetting = probationSettings.FirstOrDefault(x => x.ResponsibilityLevel == null)
                                           ?? probationSettings.FirstOrDefault();
            int maxProbationDays = probationSettings.Any() ? probationSettings.Max(x => x.ProbationDays) : 90;

            // Day-count rule (Include Weekend / Include Holiday) applied when deciding if an
            // employee is still within their probation window - same setting used by CalculateProbationAmount.
            var employeeProbationSetting = db.EmployeeProbationSettings.FirstOrDefault();
            bool includeWeekendInCount = employeeProbationSetting?.IncludeWeekend ?? false;
            bool includeHolidayInCount = employeeProbationSetting?.IncludeHoliday ?? false;

            // When weekends/holidays are excluded from the count, the same number of probation
            // "days" spans a longer calendar period - widen the SQL-level bound generously so we
            // don't fetch too narrow a set; the exact cutoff is still enforced precisely below.
            int maxProbationCalendarDays = (includeWeekendInCount && includeHolidayInCount)
                ? maxProbationDays
                : (int)Math.Ceiling(maxProbationDays * 1.5) + 30;

            //  Base query (FILTER IN DATABASE)
            //  Include anyone still flagged PROBATION, plus anyone hired directly under another
            //  status (e.g. Permanent from day one) who hasn't yet passed the longest configured
            //  probation window - the exact per-employee cut-off is enforced further down.
            var baseQuery = db.Person_Employee
                .AsNoTracking()
                .Where(x => mybranches.Contains(x.BranchId)
                         && x.IsActive
                         && (x.Status == EmployeeStatus.PROBATION
                             || DbFunctions.DiffDays(x.HiredDate, today) < maxProbationCalendarDays));

            //  Get employee IDs (needed for bulk loading)
            var employeeIds = baseQuery.Select(x => x.EmployeeId).ToList();

            //  Load related data ONCE

            // Contracts (latest per employee)
            var contracts = db.Contract
                .Where(c => employeeIds.Contains(c.EmployeeId))
                .GroupBy(c => c.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    Contract = g.OrderByDescending(x => x.To).FirstOrDefault()
                })
                .ToDictionary(x => x.EmployeeId, x => x.Contract);

            // Placements
            var placements = db.Placements
                .Where(p => employeeIds.Contains(p.EmployeeId) && p.isCurrent)
                .ToDictionary(p => p.EmployeeId, p => p);

            // Holidays (for working day logic if needed)
            var holidays = new HashSet<DateTime>(db.Holidays.Select(x => x.Date));

            // Materialize employees AFTER filtering
            var employees = baseQuery.ToList();

            //  Final projection (NO DB CALLS HERE)
            var result = employees.Select(emp =>
            {
                // Placement
                placements.TryGetValue(emp.EmployeeId, out var placement);

                // Probation days
                int probationDays = defaultProbationSetting?.ProbationDays ?? 60;

                if (placement != null)
                {
                    var setting = probationSettings
                        .FirstOrDefault(x => x.ResponsibilityLevel == placement.ResponsibilityLevel);

                    if (setting != null && setting.ProbationDays > 0)
                        probationDays = setting.ProbationDays;
                }

                // Working days elapsed since hire, honoring the Include Weekend / Include Holiday setting
                int totalDays = CalculateProbationWorkingDays(emp.HiredDate.Date, today, includeWeekendInCount, includeHolidayInCount, holidays);

                return new { emp, placement, probationDays, totalDays };
            })
            // Exclude anyone whose own configured probation period has already elapsed,
            // even if they haven't been evaluated as Fit/UnFit yet.
            .Where(x => x.totalDays < x.probationDays)
            .Select(x =>
            {
                var emp = x.emp;

                // Contract
                contracts.TryGetValue(emp.EmployeeId, out var contract);
                int? contractLeftDay = contract != null
                    ? (contract.To - today).Days
                    : (int?)null;

                // Age (simple calculation, no DB call)
                int age = today.Year - emp.DateOfBirth.Year;
                if (emp.DateOfBirth > today.AddYears(-age)) age--;

                return new
                {
                    emp.EmployeeId,
                    emp.EmployeeCode,
                    emp.MothersFullName,
                    emp.BirthPlace,
                    emp.Nation,
                    emp.Region,
                    emp.Religion,
                    emp.MartialStatus,
                    emp.Status,
                    emp.HiredDate,
                    emp.FirstName,
                    emp.MiddleName,
                    emp.LastName,
                    emp.Nationality,
                    emp.DateOfBirth,
                    emp.Title,
                    emp.Gender,
                    emp.CategoryID,
                    emp.IsActive,
                    emp.Remark,
                    emp.DateCreated,
                    emp.LastModified,
                    emp.CreatedBy,
                    emp.ModifiedBy,
                    emp.OrgBranchName,
                    emp.FirstNameEnglish,
                    emp.MiddleNameEnglish,
                    emp.LastNameEnglish,
                    emp.DateOfBirthAm,
                    emp.HiredDateAmharic,
                    BranchId = emp.BranchId,
                    emp.PhoneNo,
                    emp.City,
                    emp.SubCity,
                    emp.Wereda,
                    emp.EmployeeTypeId,
                    emp.IsPlaced,
                    emp.IsAttendanceExempt,

                    //  Optimized fields
                    IsProbationEnd = false,
                    ContratLeftDay = contractLeftDay,
                    Age = age,

                    // If you still need this, we can optimize separately
                    ProhabitionState = false // placeholder (avoid DB call)
                };
            });

            return Json(result.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


      
        public ActionResult ContractEmolyee30DaysLeft_Read([DataSourceRequest] DataSourceRequest request)
        {
            var today = DateTime.Now.Date;
            var myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            // Step 1: Get employees whose contract ends in ≤30 days
            var expiringEmployeeIds = (from p in db.Placements
                                       join c in db.Contract on p.EmployeeId equals c.EmployeeId
                                       where p.EmployementType == EmployeeStatus.CONTRACT
                                             && p.isCurrent
                                             && c.To >= today
                                       group c by c.EmployeeId into g
                                       let latestContract = g.OrderByDescending(c => c.To).FirstOrDefault()
                                       where DbFunctions.DiffDays(today, latestContract.To) <= 30
                                       select g.Key).ToList();

            // Step 2: Filter Person_Employee once
            var employees = db.Person_Employee
                              .Where(x => expiringEmployeeIds.Contains(x.EmployeeId)
                                          && myBranches.Contains(x.BranchId)
                                          && x.IsActive
                                          && x.IsPlaced)
                              .ToList();

            // Step 3: Precompute helper values for performance
            var remainingContractDaysDict = employees.ToDictionary(
                e => e.EmployeeId,
                e => HrHelper.GetRemainingContractDays(e.EmployeeId)
            );

            var probationDict = employees.ToDictionary(
                e => e.EmployeeId,
                e => HrHelper.IsProbationCompleted(e.EmployeeId)//e.Status == EmployeeStatus.PROBATION ? Probation(e.EmployeeId) : false
            );

            var prohibitionDict = employees.ToDictionary(
                e => e.EmployeeId,
                e => HrHelper.CheckProhibition(e.EmployeeId)
            );

            // Step 4: Project to DataSourceResult
            var result = employees.ToDataSourceResult(request, emp => new
            {
                emp.EmployeeId,
                emp.EmployeeCode,
                emp.MothersFullName,
                emp.BirthPlace,
                emp.Nation,
                emp.Region,
                emp.Religion,
                emp.MartialStatus,
                emp.Status,
                emp.HiredDate,
                emp.FirstName,
                emp.MiddleName,
                emp.LastName,
                emp.Nationality,
                emp.DateOfBirth,
                emp.Title,
                emp.Gender,
                emp.CategoryID,
                emp.IsActive,
                emp.Remark,
                emp.DateCreated,
                emp.LastModified,
                emp.CreatedBy,
                emp.ModifiedBy,
                emp.OrgBranchName,
                emp.FirstNameEnglish,
                emp.MiddleNameEnglish,
                emp.LastNameEnglish,
                emp.IsAttendanceExempt,
                HiredDateAmharic = emp.HiredDateAmharic ,
                DateOfBirthAm = emp.DateOfBirthAm ,
                ProhabitionState = prohibitionDict[emp.EmployeeId],
                BranchId = emp.BranchId,
                IsProbationEnd = probationDict[emp.EmployeeId],
                ContratLeftDay = remainingContractDaysDict[emp.EmployeeId],
                PhoneNo = emp.PhoneNo,
                City = emp.City,
                SubCity = emp.SubCity,
                Wereda = emp.Wereda,
                EmployeeTypeId = emp.EmployeeTypeId,
                IsPlaced = emp.IsPlaced,
                RemainingContractDays = remainingContractDaysDict[emp.EmployeeId],
                Age =emp.Age
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser, HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        public ActionResult RetiringEmolyeeNotificationBefore180DaysLeft_Read([DataSourceRequest]DataSourceRequest request)
        {


            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var reiremetsetting = db.RetirementSettings.FirstOrDefault();
            int retirementYeas = (int)(reiremetsetting?.CurrentValue ?? 60);
            var placements = db.Placements.Where(x => x.isCurrent).ToList();
            var headIds = placements.Where(x => x.IsHead).Select(x=>x.EmployeeId);
            var performerIds = placements.Where(x => !x.IsHead).Select(x=>x.EmployeeId);
            var performerReirementSetting = (int)(db.RetirementSettings.FirstOrDefault(x => x.RetireTypeSetting == RetireTypeSetting.Performers)?.CurrentValue ?? 60);
            var headReirementSetting = (int)(db.RetirementSettings.FirstOrDefault(x => x.RetireTypeSetting == RetireTypeSetting.Heads)?.CurrentValue ?? 60);
            //DateTime sixtyYearsAgo =  DateTime.Now.AddYears(-60);
            //DateTime retirementNotificationDate = sixtyYearsAgo.AddDays(180);
            //var person_employees = db.Person_Employee
            //    .Where(x => x.DateOfBirth <= retirementNotificationDate)
            //    .Where(x => myBranches.Contains(x.BranchId)&& x.IsActive && x.IsPlaced)
            //    .ToList().Where(emp =>
            //    {
            //        int retirementYears = performerReirementSetting;

            //        if (headIds.Contains(emp.EmployeeId))
            //            retirementYears = (int)headReirementSetting;

            //        DateTime retirementDate = emp.DateOfBirth.AddYears(retirementYears);
            //        DateTime notificationDate = retirementDate.AddDays(-180);

            //        return DateTime.Now >= notificationDate && DateTime.Now <= retirementDate;
            //    }).ToList(); ;
            var person_employees = db.Person_Employee.Where(x => myBranches.Contains(x.BranchId) 
                                                            && x.IsActive && x.IsPlaced).ToList()
                                    .Where(emp =>
                                    {
                                        int retirementYears = performerReirementSetting;

                                        if (headIds.Contains(emp.EmployeeId))
                                            retirementYears = (int)headReirementSetting;

                                        DateTime retirementDate = emp.DateOfBirth.AddYears(retirementYears);
                                        DateTime notificationDate = retirementDate.AddDays(-180);

                                        return DateTime.Now >= notificationDate && DateTime.Now <= retirementDate;
                                    }).ToList();
            DataSourceResult result = person_employees.ToDataSourceResult(request, person_Employee => new
            {
                EmployeeId = person_Employee.EmployeeId,
                EmployeeCode = person_Employee.EmployeeCode,
                MothersFullName = person_Employee.MothersFullName,
                BirthPlace = person_Employee.BirthPlace,
                Nation = person_Employee.Nation,
                Region = person_Employee.Region,
                Religion = person_Employee.Religion,
                MartialStatus = person_Employee.MartialStatus,
                Status = person_Employee.Status,
                HiredDate = person_Employee.HiredDate,
                FirstName = person_Employee.FirstName,
                MiddleName = person_Employee.MiddleName,
                LastName = person_Employee.LastName,
                Nationality = person_Employee.Nationality,
                DateOfBirth = person_Employee.DateOfBirth,
                Title = person_Employee.Title,
                Gender = person_Employee.Gender,
                CategoryID = person_Employee.CategoryID,
                IsActive = person_Employee.IsActive,
                Remark = person_Employee.Remark,
                DateCreated = person_Employee.DateCreated,
                LastModified = person_Employee.LastModified,
                CreatedBy = person_Employee.CreatedBy,
                ModifiedBy = person_Employee.ModifiedBy,
                OrgBranchName = person_Employee.OrgBranchName,
                FirstNameEnglish = person_Employee.FirstNameEnglish,
                MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                LastNameEnglish = person_Employee.LastNameEnglish,
                HiredDateAmharic = person_Employee.HiredDateAmharic ,
                DateOfBirthAm = person_Employee.DateOfBirthAm,
                BranchId = person_Employee.BranchId,
                PhoneNo = person_Employee.PhoneNo,
                City = person_Employee.City,
                SubCity = person_Employee.SubCity,
                Wereda = person_Employee.Wereda,
                EmployeeTypeId = person_Employee.EmployeeTypeId,
                IsPlaced = person_Employee.IsPlaced,
                IsAttendanceExempt = person_Employee.IsAttendanceExempt,
                Age = person_Employee.Age
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser, HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        public ActionResult Person_Employee_Read_New([DataSourceRequest] DataSourceRequest request)
        {
            List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var query = db.Person_Employee
                .AsNoTracking() // no change tracking = faster
                .Where(x => myBranches.Contains(x.BranchId) && x.IsActive && x.IsPlaced)
                .Select(person_Employee => new
                {
                    person_Employee.EmployeeId,
                    person_Employee.EmployeeCode,
                    person_Employee.FirstName,
                    person_Employee.MiddleName,
                    person_Employee.LastName,
                    person_Employee.BranchId,
                    person_Employee.OrgBranchName,
                    person_Employee.IsPlaced,
                    person_Employee.IsActive,
                    person_Employee.Age,
                    person_Employee.DateOfBirth,
                    person_Employee.HiredDate,
                    person_Employee.PhoneNo,
                    person_Employee.MothersFullName,
                    person_Employee.BirthPlace,
                    person_Employee.Nation,
                    person_Employee.Region,
                    person_Employee.Religion,
                    person_Employee.MartialStatus,
                    person_Employee.Status,
                    person_Employee.Nationality,
                    person_Employee.Title,
                    person_Employee.Gender,
                    person_Employee.CategoryID,
                    person_Employee.Remark,
                    person_Employee.DateCreated,
                    person_Employee.LastModified,
                    person_Employee.CreatedBy,
                    person_Employee.ModifiedBy,
                    person_Employee.FirstNameEnglish,
                    person_Employee.MiddleNameEnglish,
                    person_Employee.LastNameEnglish,
                    person_Employee.DateOfBirthAm,
                    person_Employee.HiredDateAmharic,
                    person_Employee.ProhabitionState,
                    person_Employee.IsProbationEnd,
                    person_Employee.ContratLeftDay,
                    person_Employee.IsAttendanceExempt,
                    person_Employee.City,
                    person_Employee.SubCity,
                    person_Employee.Wereda,
                    person_Employee.EmployeeTypeId
                    //  add only what the grid actually displays
                });

            var result = query.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Person_Employee_Read([DataSourceRequest] DataSourceRequest request)
        {
            var today = DateTime.Today;
            var myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

            // 1. Base Query (IQueryable - Stays on SQL Server)
            var baseQuery = from emp in db.Person_Employee
                            where myBranches.Contains(emp.BranchId)
                                  && emp.IsActive
                                  && emp.IsPlaced
                            join con in db.Contract
                                on emp.EmployeeId equals con.EmployeeId into empContracts
                            from contract in empContracts
                                .OrderByDescending(c => c.To)
                                .Take(1)
                                .DefaultIfEmpty()
                            select new
                            {
                                emp.EmployeeId,
                                emp.EmployeeCode,
                                emp.FirstName,
                                emp.MiddleName,
                                emp.LastName,
                                emp.FirstNameEnglish,
                                emp.MiddleNameEnglish,
                                emp.LastNameEnglish,
                                emp.MothersFullName,
                                emp.DateOfBirth,
                                emp.DateOfBirthAm,
                                emp.HiredDate,
                                emp.HiredDateAmharic,
                                emp.BirthPlace,
                                emp.BranchId,
                                emp.Title,
                                emp.Age,
                                emp.EmployeeTypeId,
                                emp.CategoryID,
                                emp.Gender,
                                emp.Nation,
                                emp.OrgBranchName,
                                emp.Religion,
                                emp.Region,
                                emp.Nationality,
                                emp.ProhabitionState,
                                emp.ContratLeftDay,
                                emp.Status,
                                emp.MartialStatus,
                                emp.City,
                                emp.IsInsured,
                                emp.IsPlaced,
                                emp.IsReHired,
                                emp.IsBiometricRegistered,
                                emp.BiometricEmployeeId,
                                emp.PhoneNo,
                                emp.SubCity,
                                emp.Wereda,
                                emp.IsActive,
                                emp.IsProbationEnd,
                                emp.IsAttendanceExempt,
                                emp.Remark,
                                emp.DateCreated,
                                emp.CreatedBy,
                                emp.LastModified,
                                emp.ModifiedBy,
                                RemainingContractDays = contract != null
                                    ? (int?)DbFunctions.DiffDays(today, contract.To)
                                    : null
                            };

            // 2. Execute Paging / Sorting / Filtering at SQL level FIRST
            var pagedDataResult = baseQuery.ToDataSourceResult(request);
            var pagedData = pagedDataResult.Data.Cast<dynamic>().ToList();

            if (!pagedData.Any())
            {
                return Json(pagedDataResult, JsonRequestBehavior.AllowGet);
            }

            // Extract ONLY the 10-20 Employee IDs on the current page
            var empIds = pagedData.Select(x => (int)x.EmployeeId).ToList();

            // 3. Batch fetch ONLY lightweight columns for the paged IDs
            var allExperiences = db.Experiences
                .Where(x => empIds.Contains(x.EmployeeId))
                .Select(x => new { x.EmployeeId, x.From, x.To })
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var allPlacements = db.Placements
                .Where(x => empIds.Contains(x.EmployeeId))
                .Select(x => new { x.EmployeeId, x.AssignmentDate, x.isCurrent })
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(p => p.AssignmentDate).ToList());

            var allTerminations = db.Terminations
                .Where(x => x.IsOnlineApproved && empIds.Contains(x.EmployeeId))
                .Select(x => new { x.EmployeeId, x.TerminationDate })
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(t => t.TerminationDate).FirstOrDefault());

            var allPlacementHistories = db.PlacementHistories
                .Where(x => empIds.Contains(x.EmployeeId))
                .Select(x => new { x.EmployeeId, x.From, x.To })
                .GroupBy(x => x.EmployeeId)
                .ToDictionary(g => g.Key, g => g.ToList());

            bool isAmharic = cultureName?.ToLower() == "am" || cultureName?.ToLower() == "am-et";

            // 4. Calculate string representations only for the visible page
            var finalTransformedData = pagedData.Select(x =>
            {
                int employeeId = x.EmployeeId;
                double externalExpDays = 0;
                double internalExpDays = 0;

                // A. External Experience
                if (allExperiences.TryGetValue(employeeId, out var empExtExps))
                {
                    externalExpDays = empExtExps.Sum(ex => (ex.To - ex.From).Days);
                }

                // Earliest placement/history start date we actually found records for,
                // used below to recover service time lost when a rehire has no
                // placement/placement-history row covering its original hire period.
                DateTime? earliestRecordedStart = null;

                // B. Placements timeline
                if (allPlacements.TryGetValue(employeeId, out var empPlacements))
                {
                    allTerminations.TryGetValue(employeeId, out var termination);
                    bool firstIteration = true;
                    DateTime? previousEndDate = today;

                    foreach (var perPl in empPlacements)
                    {
                        DateTime startDate = perPl.AssignmentDate;
                        DateTime? endDate = null;

                        if (perPl.isCurrent)
                        {
                            endDate = today;
                        }
                        else if (termination != null && termination.TerminationDate > startDate)
                        {
                            endDate = firstIteration ? termination.TerminationDate : previousEndDate;
                        }
                        else
                        {
                            endDate = firstIteration ? (DateTime?)null : previousEndDate;
                        }

                        if (endDate.HasValue)
                        {
                            internalExpDays += (endDate.Value - startDate).Days;
                        }

                        previousEndDate = startDate;
                        firstIteration = false;

                        if (!earliestRecordedStart.HasValue || startDate < earliestRecordedStart.Value)
                        {
                            earliestRecordedStart = startDate;
                        }
                    }
                }

                // C. Placement Histories
                if (allPlacementHistories.TryGetValue(employeeId, out var histories))
                {
                    foreach (var ex in histories)
                    {
                        internalExpDays += (ex.To - ex.From).Days;

                        if (!earliestRecordedStart.HasValue || ex.From < earliestRecordedStart.Value)
                        {
                            earliestRecordedStart = ex.From;
                        }
                    }
                }

                // D. Recover any pre-rehire service that has no placement/history row
                // (e.g. terminated then rehired employees whose original placement
                // wasn't carried into PlacementHistories) by counting from HiredDate.
                if (earliestRecordedStart.HasValue && x.HiredDate < earliestRecordedStart.Value)
                {
                    internalExpDays += (earliestRecordedStart.Value - x.HiredDate).Days;
                }

                double totalDays = externalExpDays + internalExpDays;

                string externalExpText = isAmharic
                    ? EmployeeHelper.GetDaystoWordsAmharic(externalExpDays)
                    : EmployeeHelper.GetDaystoWords(externalExpDays);

                string internalExpText = isAmharic
                    ? EmployeeHelper.GetDaystoWordsAmharic(internalExpDays)
                    : EmployeeHelper.GetDaystoWords(internalExpDays);

                string totalExpText = isAmharic
                    ? EmployeeHelper.GetDaystoWordsAmharic(totalDays)
                    : EmployeeHelper.GetDaystoWords(totalDays);

                string ageText = GetEmployeeAge(x.DateOfBirth);

                return new
                {
                    x.EmployeeId,
                    x.EmployeeCode,
                    x.FirstName,
                    x.MiddleName,
                    x.LastName,
                    x.FirstNameEnglish,
                    x.MiddleNameEnglish,
                    x.LastNameEnglish,
                    x.MothersFullName,
                    x.DateOfBirth,
                    x.DateOfBirthAm,
                    x.Age,
                    x.HiredDate,
                    x.HiredDateAmharic,
                    x.BirthPlace,
                    x.BranchId,
                    x.Title,
                    x.EmployeeTypeId,
                    x.CategoryID,
                    x.Gender,
                    x.Nation,
                    x.OrgBranchName,
                    x.Religion,
                    x.Region,
                    x.Nationality,
                    x.ProhabitionState,
                    x.Status,
                    x.MartialStatus,
                    x.City,
                    x.IsInsured,
                    x.IsPlaced,
                    x.IsReHired,
                    x.IsBiometricRegistered,
                    x.BiometricEmployeeId,
                    x.PhoneNo,
                    x.ContratLeftDay,
                    x.SubCity,
                    x.Wereda,
                    x.IsActive,
                    x.IsProbationEnd,
                    x.IsAttendanceExempt,
                    x.Remark,
                    x.DateCreated,
                    x.CreatedBy,
                    x.LastModified,
                    x.ModifiedBy,

                    EmployeeExperience = totalExpText,
                    InternalExperience = internalExpText,
                    ExternalExperience = externalExpText,
                    EmployeeAge = ageText,
                    RemainingContractDays = x.RemainingContractDays
                };
            }).ToList();

            // Reassign data array while preserving Kendo's Total count
            pagedDataResult.Data = finalTransformedData;

            return Json(pagedDataResult, JsonRequestBehavior.AllowGet);
        }
        private string GetEmployeeExpreince(DateTime HiredDate)
        {
            if (HiredDate == null)
                return string.Empty;

            var today = DateTime.Today;
            var ageYears = today.Year - HiredDate.Year;
            var ageMonths = today.Month - HiredDate.Month;
            var ageDays = today.Day - HiredDate.Day;

            // Adjust if birthday hasn’t occurred yet this year
            if (ageDays < 0)
            {
                ageMonths--;
                ageDays += DateTime.DaysInMonth(today.Year, (today.Month == 1 ? 12 : today.Month - 1));
            }

            if (ageMonths < 0)
            {
                ageYears--;
                ageMonths += 12;
            }

            return $"{ageYears} years, {ageMonths} months, {ageDays} days";
        }
        private string GetEmployeeAge(DateTime DateOfBirth)
        {
            if (DateOfBirth == null)
                return string.Empty;

            var today = DateTime.Today;
            var ageYears = today.Year - DateOfBirth.Year;
            var ageMonths = today.Month - DateOfBirth.Month;
            var ageDays = today.Day - DateOfBirth.Day;

            // Adjust if birthday hasn’t occurred yet this year
            if (ageDays < 0)
            {
                ageMonths--;
                ageDays += DateTime.DaysInMonth(today.Year, (today.Month == 1 ? 12 : today.Month - 1));
            }

            if (ageMonths < 0)
            {
                ageYears--;
                ageMonths += 12;
            }

            return $"{ageYears} years, {ageMonths} months, {ageDays} days";
        }
        //public ActionResult Person_Employee_Read([DataSourceRequest]DataSourceRequest request)
        //{

        //    List<int> myBranches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

        //    IQueryable<Person_Employee> personEmployees = db.Person_Employee
        //        .Where(x => myBranches.Contains(x.BranchId) && x.IsActive && x.IsPlaced);


        //    DataSourceResult result = personEmployees.ToDataSourceResult(request, person_Employee => new
        //    {
        //        EmployeeId = person_Employee.EmployeeId,
        //        EmployeeCode = person_Employee.EmployeeCode,
        //        MothersFullName = person_Employee.MothersFullName,
        //        BirthPlace = person_Employee.BirthPlace,
        //        Nation = person_Employee.Nation,
        //        Region = person_Employee.Region,
        //        Religion = person_Employee.Religion,
        //        MartialStatus = person_Employee.MartialStatus,
        //        Status = person_Employee.Status,
        //        HiredDate = person_Employee.HiredDate,
        //        FirstName = person_Employee.FirstName,
        //        MiddleName = person_Employee.MiddleName,
        //        LastName = person_Employee.LastName,
        //        Nationality = person_Employee.Nationality,
        //        DateOfBirth = person_Employee.DateOfBirth,
        //        Title = person_Employee.Title,
        //        Gender = person_Employee.Gender,
        //        CategoryID = person_Employee.CategoryID,
        //        IsActive = person_Employee.IsActive,
        //        Remark = person_Employee.Remark,
        //        DateCreated = person_Employee.DateCreated,
        //        LastModified = person_Employee.LastModified,
        //        CreatedBy = person_Employee.CreatedBy,
        //        ModifiedBy = person_Employee.ModifiedBy,
        //        OrgBranchName = person_Employee.OrgBranchName,
        //        FirstNameEnglish = person_Employee.FirstNameEnglish,
        //        MiddleNameEnglish = person_Employee.MiddleNameEnglish,
        //        LastNameEnglish = person_Employee.LastNameEnglish,
        //        DateOfBirthAm = person_Employee.DateOfBirthAm,
        //        HiredDateAmharic = person_Employee.HiredDateAmharic,
        //        ProhabitionState = person_Employee.ProhabitionState,
        //        //ProhabitionState = CheckProhabition(person_Employee.EmployeeId),
        //        BranchId = person_Employee.BranchId,//Branch(person_Employee.EmployeeId),
        //        IsProbationEnd = person_Employee.IsProbationEnd,
        //        //IsProbationEnd = person_Employee.Status == EmployeeStatus.PROBATION ? Probation(person_Employee.EmployeeId) : false,
        //        ContratLeftDay = person_Employee.ContratLeftDay,
        //        //ContratLeftDay = GetContractLeftDay(person_Employee.EmployeeId),
        //        PhoneNo = person_Employee.PhoneNo,
        //        City = person_Employee.City,
        //        SubCity = person_Employee.SubCity,
        //        Wereda = person_Employee.Wereda,
        //        EmployeeTypeId = person_Employee.EmployeeTypeId,
        //        IsPlaced = person_Employee.IsPlaced,
        //        Age = person_Employee.Age
        //        //Age = EmployeeHelper.GetAge(person_Employee.EmployeeId)
        //    });

        //    return Json(result);
        //}
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Person_Employee_Create([DataSourceRequest]DataSourceRequest request, Person_Employee person_Employee)
        {
            ModelState.Clear();

            char [] nameFilter = "0123456789".ToArray();

            if (person_Employee.FirstName.IndexOfAny(nameFilter) != -1 || 
                person_Employee.MiddleName.IndexOfAny(nameFilter) != -1 ||
                person_Employee.LastName.IndexOfAny(nameFilter) != -1  ||
                person_Employee.FirstNameEnglish.IndexOfAny(nameFilter) != -1||
                person_Employee.MiddleNameEnglish.IndexOfAny(nameFilter) != -1||
                person_Employee.LastNameEnglish.IndexOfAny(nameFilter) != -1)
            {
                ModelState.AddModelError("Name", "Name should not contain numbers.");
                return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
            }

            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                // Ethiopian Date Parsing (Amharic)
                if (!string.IsNullOrWhiteSpace(person_Employee.DateOfBirthAm))
                {
                    person_Employee.DateOfBirth = HrHelper.ParseEthiopicDate(person_Employee.DateOfBirthAm);
                }
                else
                {
                    ModelState.AddModelError("", "Birth Date is required.");
                    return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
                }

                if (!string.IsNullOrWhiteSpace(person_Employee.HiredDateAmharic))
                {
                    person_Employee.HiredDate = HrHelper.ParseEthiopicDate(person_Employee.HiredDateAmharic);
                }
                else
                {
                    ModelState.AddModelError("", "Hire Date is required.");
                    return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
                }
            }
            else
            {
                // Convert Gregorian to Amharic format
                person_Employee.DateOfBirthAm = XAPI.EthiopicDateTime.GetEthiopicDate(
                    person_Employee.DateOfBirth.Day,
                    person_Employee.DateOfBirth.Month,
                    person_Employee.DateOfBirth.Year
                );

                person_Employee.HiredDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(
                    person_Employee.HiredDate.Day,
                    person_Employee.HiredDate.Month,
                    person_Employee.HiredDate.Year
                );
            }


            //this variable uses for if the setting is not added it helps to take manualy filled id
            // string empCode = person_Employee.EmployeeCode;

            string generated_code = GenerateEmployeeCode(person_Employee.HiredDate);

            if (!string.IsNullOrEmpty(generated_code))
            {
                person_Employee.EmployeeCode = generated_code;
            }
            var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == EmployeeIdAttribute.Auto_Generate.ToString()&&x.Reference== "EmployeeID");
            if(config != null&& string.IsNullOrEmpty(generated_code))
            {
                int.TryParse(config.CurrentValue, out int length);
                if(length != person_Employee.EmployeeCode.Length)
                {
                    ModelState.AddModelError("exist", "The employee code  must be length Of "+length);
                    return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));

                }
            }
            else  if(string.IsNullOrEmpty(generated_code))
            {
                var configman = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == EmployeeIdAttribute.Manual.ToString() && x.Reference == "EmployeeID");
                if (configman != null && !string.IsNullOrEmpty(person_Employee.EmployeeCode))
                {
                    int.TryParse(configman.CurrentValue, out int length);
                    if (length != person_Employee.EmployeeCode.Length)
                    {
                        ModelState.AddModelError("exist", "The employee code  must be length Of " + length);
                        return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
                    }
                }
            }
            #region Validation 
            int age = DateTime.Now.Year - person_Employee.DateOfBirth.Year;
            var checkExistance = db.Person_Employee.FirstOrDefault(x => x.EmployeeCode == person_Employee.EmployeeCode);
            if (checkExistance != null)
            {
                ModelState.AddModelError("", "This Employee Id is already assigned for other employee!!");
                return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));

            }
            if (person_Employee.HiredDate < person_Employee.DateOfBirth)
            {
                ModelState.AddModelError("", "Date of birth should be less than hire date.");
                return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
            }

            if (age < 18)
            {
                ModelState.AddModelError("", "The Employee age should be 18 or more.");
                return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
            }

            //if (person_Employee.HiredDate.Date > DateTime.Now.Date)
            //{

            //    ModelState.AddModelError("", "Hire date should be less or equal to the current date!!");
            //    return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));

            //}

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new Person_Employee
                {
                    EmployeeId = person_Employee.EmployeeId,
                    EmployeeCode = person_Employee.EmployeeCode,
                    MothersFullName = person_Employee.MothersFullName,
                    BirthPlace = person_Employee.BirthPlace,
                    Nation = person_Employee.Nation,
                    Region = person_Employee.Region,
                    Religion = person_Employee.Religion,
                    MartialStatus = person_Employee.MartialStatus,
                    Status = EmployeeStatus.NO_STATUS,
                    HiredDate = person_Employee.HiredDate,
                    FirstName = person_Employee.FirstName,
                    MiddleName = person_Employee.MiddleName, 
                    LastName = person_Employee.LastName,
                    Nationality = person_Employee.Nationality,
                    DateOfBirth = person_Employee.DateOfBirth,
                    Title = person_Employee.Title,
                    Gender = person_Employee.Gender,
                    CategoryID = person_Employee.CategoryID,
                    IsActive = true,
                    IsPlaced = false,
                    Remark = person_Employee.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = person_Employee.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = person_Employee.ModifiedBy,
                    //DateCreatedAM = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day,DateTime.Today.Month,DateTime.Today.Year),
                    //DateLastModifiedAM = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year),
                    FirstNameEnglish = person_Employee.FirstNameEnglish,
                    MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                    LastNameEnglish = person_Employee.LastNameEnglish,
                    DateOfBirthAm = person_Employee.DateOfBirthAm,
                    HiredDateAmharic = person_Employee.HiredDateAmharic,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    PhoneNo = person_Employee.PhoneNo,
                    City = person_Employee.City,
                    SubCity = person_Employee.SubCity,
                    Wereda = person_Employee.Wereda,
                    EmployeeTypeId = person_Employee.EmployeeTypeId,
                };

                db.Person_Employee.Add(entity);
                db.SaveChanges();

                person_Employee.EmployeeId = entity.EmployeeId;
                var fullName = $"{entity.FirstNameEnglish} {entity.MiddleNameEnglish } {entity.LastNameEnglish}";
                //new DynamicAccountHelper.CreateEmployeeSubAccount(entity.EmployeeId, fullName);
                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Profile", "Success", User.Identity.Name, " Profile Info for " + person.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

                }
                #endregion
                //reset cashe memepory to add the newly created 
                CacheHelper.InvalidateEmployeeCache();
            }

            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
        }

        private bool CheckValdationToDelete(int id, out string errorMessage)
        {
            errorMessage = string.Empty;
            string idStr = id.ToString();

            if (db.Identifications.Any(x => x.Reference == idStr && x.ReferenceType == "Employee"))
            {
                errorMessage = "You cannot delete this employee. Please Delete Identification information first.";
                return false;
            }
            if (db.Addresses.Any(x => x.Reference == idStr && x.ReferenceType == "Employee"))
            {
                errorMessage = "You cannot delete this employee. Please Delete Address information first.";
                return false;
            }
            if (db.Education.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Educational information first.";
                return false;
            }
            if (db.EmpTrainingEducations.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Training information first.";
                return false;
            }
            if (db.Competencies.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Competency information first.";
                return false;
            }
            if (db.Emergencies.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Emergency information first.";
                return false;
            }
            if (db.EmployeeAssociation.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Association information first.";
                return false;
            }
            if (db.Contract.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Contract information first.";
                return false;
            }
            if (db.Experiences.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Experience information first.";
                return false;
            }
            if (db.Placements.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Placement information first.";
                return false;
            }
            if (db.PlacementHistories.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Placement history information first.";
                return false;
            }
            if (db.Commitments.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Commitment information first.";
                return false;
            }
            if (db.Disabilities.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Disability information first.";
                return false;
            }
            if (db.Cosigns.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Cosign information first.";
                return false;
            }
            if (db.Prohibitions.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Probation information first.";
                return false;
            }
            if (db.CommitteeMembers.Any(x => x.EmployeeId == id))
            {
                errorMessage = "You cannot delete this employee. Please Delete Committees information first.";
                return false;
            }

            return true;
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Person_Employee_Update([DataSourceRequest]DataSourceRequest request, Person_Employee person_Employee)
        {
            
            char[] nameFilter = "0123456789".ToArray();

            if (person_Employee.FirstName.IndexOfAny(nameFilter) != -1|| 
                person_Employee.MiddleName.IndexOfAny(nameFilter) != -1 ||
                person_Employee.LastName.IndexOfAny(nameFilter) != -1 || 
                person_Employee.FirstNameEnglish.IndexOfAny(nameFilter) != -1 || 
                person_Employee.MiddleNameEnglish.IndexOfAny(nameFilter) != -1 || 
                person_Employee.LastNameEnglish.IndexOfAny(nameFilter) != -1)
            {
                ModelState.AddModelError("Name", "Name should not contain numbers.");
                return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));

            }
            bool datePreference = db.DatePreferenceSettings.FirstOrDefault()?.UseEthiopianDate ?? true;
            if (datePreference)
            {
                // Ethiopian Date Parsing (Amharic)
                if (!string.IsNullOrWhiteSpace(person_Employee.DateOfBirthAm))
                {
                    int index = person_Employee.DateOfBirthAm.IndexOf(" (");
                    person_Employee.DateOfBirthAm = (index > 0) ? person_Employee.DateOfBirthAm.Substring(0, index) : person_Employee.DateOfBirthAm;
                    
                    person_Employee.DateOfBirth = HrHelper.ParseEthiopicDate(person_Employee.DateOfBirthAm);
                }
                else
                {
                    ModelState.AddModelError("", "Birth Date is required.");
                    return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
                }

                if (!string.IsNullOrWhiteSpace(person_Employee.HiredDateAmharic))
                {
                    int index2 = person_Employee.HiredDateAmharic.IndexOf(" (");
                    person_Employee.HiredDateAmharic = (index2 > 0) ? person_Employee.HiredDateAmharic.Substring(0, index2) : person_Employee.HiredDateAmharic;

                    person_Employee.HiredDate = HrHelper.ParseEthiopicDate(person_Employee.HiredDateAmharic);
                }
                else
                {
                    ModelState.AddModelError("", "Hire Date is required.");
                    return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
                }
            }
            else
            {
                // Convert Gregorian to Amharic format
                person_Employee.DateOfBirthAm = XAPI.EthiopicDateTime.GetEthiopicDate(
                    person_Employee.DateOfBirth.Day,
                    person_Employee.DateOfBirth.Month,
                    person_Employee.DateOfBirth.Year
                );

                person_Employee.HiredDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(
                    person_Employee.HiredDate.Day,
                    person_Employee.HiredDate.Month,
                    person_Employee.HiredDate.Year
                );
            }
            
            // var days = DateTime.Today.Date.Subtract(person_Employee.HiredDate.Date).Days;
            // var month = days / 30;
            // var year = (int)month / 12;

            //var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == ProbationSetting.Probation_Period.ToString());
            //if (config != null)
            //{
            //    int days = Convert.ToInt32(HrHelper.GetWorkingDays(person_Employee.HiredDate.Date, DateTime.Today.Date));
            //    if (days < Convert.ToInt32(config.CurrentValue) && person_Employee.Status != EmployeeStatus.PROBATION)
            //    {
            //        ModelState.AddModelError("Probation Validation", "This employee didn't finish Probation period you can't change the status");
            //        return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));

            //    }
            //}
            
            int age = DateTime.Now.Year - person_Employee.DateOfBirth.Year;
            if (person_Employee.HiredDate < person_Employee.DateOfBirth)
            {
                ModelState.AddModelError("", "Date of birth should be less than hire date.");
                return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));

            }
            if (age < 18)
            {
                ModelState.AddModelError("", "The Employee age should be 18 or more.");
                return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));

            }
            //occ
            //if (person_Employee.HiredDate.Date > DateTime.Now.Date)
            //{

            //    ModelState.AddModelError("", "Hire date should be less or equal to the current date!!");
            //    return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.DenyGet);
            //}

            var checkCode = db.Person_Employee.FirstOrDefault(x => x.EmployeeId != person_Employee.EmployeeId 
            && x.EmployeeCode == person_Employee.EmployeeCode);
            if (checkCode != null)
            {
                ModelState.AddModelError("", "This employee code is already taken by another employee");
                return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));

            }
            //var configForCode = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == EmployeeIdAttribute.Auto_Generate.ToString() && x.Reference == "EmployeeID");
            //if (configForCode != null)
            //{
            //    int.TryParse(configForCode.CurrentValue, out int length);
            //    if (length != person_Employee.EmployeeCode.Length)
            //    {
            //        ModelState.AddModelError("exist", "The Employee Code  Must be Length Of " + length);
            //        return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));

            //    }
            //}
                
           

            if (ModelState.IsValid)
            {
                var emp = db.Person_Employee.Find(person_Employee.EmployeeId);
                if (emp != null)
                {
                    bool noNameChange = emp.FirstName == person_Employee.FirstName &&
                                         emp.MiddleName == person_Employee.MiddleName &&
                                         emp.LastName == person_Employee.LastName &&
                                         emp.FirstNameEnglish == person_Employee.FirstNameEnglish &&
                                         emp.MiddleNameEnglish == person_Employee.MiddleNameEnglish &&
                                         emp.LastNameEnglish == person_Employee.LastNameEnglish;
                    if (!noNameChange)
                    {
                        // Reset cache memory to add the updated name
                        CacheHelper.InvalidateEmployeeCache();
                    }

                    emp.EmployeeCode = person_Employee.EmployeeCode;
                    emp.MothersFullName = person_Employee.MothersFullName;
                    emp.BirthPlace = person_Employee.BirthPlace;
                    emp.Nation = person_Employee.Nation;
                    emp.Region = person_Employee.Region;
                    emp.Religion = person_Employee.Religion;
                    emp.MartialStatus = person_Employee.MartialStatus;
                    emp.Status = person_Employee.Status;
                    emp.HiredDate = person_Employee.HiredDate;
                    emp.FirstName = person_Employee.FirstName;//FirstName,
                    emp.MiddleName = person_Employee.MiddleName; //MiddleName ,
                    emp.LastName = person_Employee.LastName;//LastName
                    emp.Nationality = person_Employee.Nationality;
                    emp.DateOfBirth = person_Employee.DateOfBirth;
                    emp.Title = person_Employee.Title;
                    emp.Gender = person_Employee.Gender;
                    emp.CategoryID = person_Employee.CategoryID;
                    emp.IsActive = person_Employee.IsActive;
                    emp.Remark = person_Employee.Remark;
                    emp.LastModified = DateTime.Now;
                    emp.ModifiedBy = User.Identity.Name;
                    emp.FirstNameEnglish = person_Employee.FirstNameEnglish;
                    emp.MiddleNameEnglish = person_Employee.MiddleNameEnglish;
                    emp.LastNameEnglish = person_Employee.LastNameEnglish;
                    emp.DateOfBirthAm = person_Employee.DateOfBirthAm;
                    emp.OrgBranchName = person_Employee.OrgBranchName;
                    emp.HiredDateAmharic = person_Employee.HiredDateAmharic;
                    emp.PhoneNo = person_Employee.PhoneNo;
                    emp.City = person_Employee.City;
                    emp.SubCity = person_Employee.SubCity;
                    emp.Wereda = person_Employee.Wereda;
                    emp.EmployeeTypeId = person_Employee.EmployeeTypeId;

                    db.Entry(emp).State = EntityState.Modified;
                    db.SaveChanges();

                    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.isCurrent);
                    if (place != null)
                    {
                        place.EmployementType = emp.Status;

                        db.Entry(place).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                   
                }

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Profile", "Success", User.Identity.Name, " Profile Info of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name, Modules.HRM);
                }
                #endregion

            }


            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Person_Employee_Destroy([DataSourceRequest]DataSourceRequest request, Person_Employee person_Employee)
        {
            ModelState.Clear();

            string validationError;
            if (!CheckValdationToDelete(person_Employee.EmployeeId, out validationError))
            {
                ModelState.AddModelError("", validationError);
                return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.DenyGet);
            }

            var employee = db.Person_Employee.Find(person_Employee.EmployeeId);
            if (employee != null)
            {
                db.Entry(employee).State = EntityState.Deleted;
                db.SaveChanges();
                CacheHelper.InvalidateEmployeeCache();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee information of employee Id " + person_Employee.EmployeeCode);
                #endregion
            }
            else
            {
                ModelState.AddModelError("", "Employee record not found in database.");
            }

            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
        }
        //public ActionResult Person_Employee_Destroy([DataSourceRequest]DataSourceRequest request, Person_Employee person_Employee)
        //{
        //    ModelState.Clear();
        //    if (ModelState.IsValid)
        //    {
        //        if (!CheckValdationToDelete(person_Employee.EmployeeId))
        //        {

        //            ModelState.AddModelError("", ErrorMessage);
        //            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.DenyGet);
        //        }
        //        else
        //        {

        //            var employee = db.Person_Employee.Find(person_Employee.EmployeeId);
        //            if (employee != null)
        //            {
        //                db.Entry(employee).State = EntityState.Deleted;
        //                db.SaveChanges();
        //                CacheHelper.InvalidateEmployeeCache();
        //                #region delete log
        //                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee information of employee Id" + person_Employee.EmployeeCode);
        //                #endregion
        //            }
        //        }
        //    }

        //    return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
        //}
        public ActionResult Placements_AfterSystem_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<OldPlacementVM> placeList = new List<OldPlacementVM>();

            var placements = db.Placements.Where(x => x.EmployeeId == id).OrderBy(x => x.AssignmentDate).ToArray();//.Where(x => mybraches.Contains(x.BranchId.ToString()));
            int lenght = placements.Length;
            for (int i = 0; i <= lenght - 1; i++)
            {
                var pp = new OldPlacementVM();
                var brId = placements[i].BranchId;
                var orgId = placements[i].OrgStructureId;
                var lookId = placements[i].LookupId;
                pp.PlaceId = placements[i].PlacementId;
                var branch = db.Branch.FirstOrDefault(x => x.BranchId == brId);
                if (branch != null)
                {
                    pp.Branch = branch.Name;
                }
                var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == orgId);
                if (department != null)
                {
                    pp.Department = department.Name;
                }
                var jobtitle = db.Lookups.FirstOrDefault(x => x.LookupId == lookId && x.LookupType == LookUpTypes.JOB_TITLE);
                if (jobtitle != null)
                {
                    pp.JobTitle = jobtitle.Description;
                }
                pp.LookupId = placements[i].LookupId;
                pp.AssignmentDate = placements[i].AssignmentDateAmharic;
                pp.FromDate = placements[i].AssignmentDate;
                pp.From = placements[i].AssignmentDateAmharic;
                if (i == lenght - 1)
                {
                    if (placements[i].isCurrent)
                    {
                        DateTime now = DateTime.Now;
                        pp.To = XAPI.EthiopicDateTime.GetEthiopicDate(now.Day, now.Month, now.Year);
                        pp.ToDate = now;
                    }
                    else
                    {
                        pp.ToDate = placements[i].AssignmentDate;
                        pp.To = placements[i].AssignmentDateAmharic;
                    }

                }
                else
                {
                    pp.ToDate = placements[i + 1].AssignmentDate;
                    pp.To = placements[i + 1].AssignmentDateAmharic;
                }

                pp.IsHead = placements[i].IsHead;
                pp.IsCurrent = placements[i].isCurrent;
                pp.Salary = placements[i].Salary;
                pp.AssignmentType = placements[i].AssignmentType;
                placeList.Add(pp);
            }
            //foreach (var place in placements)
            //{
            //    var pp = new OldPlacementVM();

            //    pp.PlaceId = place.PlacementId;
            //    var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
            //    if (branch != null)
            //    {
            //        pp.Branch = branch.Name;
            //    }
            //    var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
            //    if (department != null)
            //    {
            //        pp.Department = department.Name;
            //    }
            //    var jobtitle = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
            //    if (jobtitle != null)
            //    {
            //        pp.JobTitle = jobtitle.Description;
            //    }
            //    pp.LookupId = place.LookupId;
            //    pp.AssignmentDate = place.AssignmentDateAmharic;
            //    pp.Date = place.AssignmentDate;
            //    pp.IsHead = place.IsHead;
            //    pp.IsCurrent = place.isCurrent;
            //    pp.Salary = place.Salary;
            //    pp.AssignmentType = place.AssignmentType;

            //    placeList.Add(pp);
            //}
            DataSourceResult result = placeList.ToDataSourceResult(request, placement => new
            {
                PlaceId = placement.PlaceId,
                Branch = placement.Branch,
                Department = placement.Department,
                LookupId = placement.LookupId,
                JobTitle = placement.JobTitle,
                IsHead = placement.IsHead,
                IsCurrent = placement.IsCurrent,
                AssignmentDate = placement.AssignmentDate,
                From = placement.From,
                To = placement.To,
                FromDate = placement.FromDate,
                ToDate = placement.ToDate,
                Salary = placement.Salary,
                AssignmentType = placement.AssignmentType

            });

            return Json(result);


        }

        public ActionResult Placements_HistoryForProfile_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<OldPlacementVM> placeList = new List<OldPlacementVM>();

            var placements = db.Placements.Where(x => x.EmployeeId == id ).OrderBy(x => x.AssignmentDate).ToArray();//.Where(x => mybraches.Contains(x.BranchId.ToString()));
            int lenght = placements.Length;
            for (int i = 0; i <= lenght - 1; i++)
            {
                var pp = new OldPlacementVM();
                var brId = placements[i].BranchId;
                var orgId = placements[i].OrgStructureId;
                var lookId = placements[i].LookupId;
                pp.PlaceId = placements[i].PlacementId;
                var branch = db.Branch.FirstOrDefault(x => x.BranchId == brId);
                if (branch != null)
                {
                    pp.Branch = branch.Name;
                }
                var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == orgId);
                if (department != null)
                {
                    pp.Department = department.Name;
                }
                var jobtitle = db.Lookups.FirstOrDefault(x => x.LookupId == lookId && x.LookupType == LookUpTypes.JOB_TITLE);
                if (jobtitle != null)
                {
                    pp.JobTitle = jobtitle.Description;
                }
                pp.LookupId = placements[i].LookupId;
                pp.AssignmentDate = $"{placements[i].AssignmentDateAmharic} ({placements[i].AssignmentDate.ToString("MMMM dd, yyyy")})";
                pp.FromDate = placements[i].AssignmentDate;
                pp.From = placements[i].AssignmentDateAmharic;
                if (i == lenght - 1)
                {
                    if (placements[i].isCurrent)
                    {
                        DateTime now = DateTime.Now;
                        pp.To = XAPI.EthiopicDateTime.GetEthiopicDate(now.Day, now.Month, now.Year);
                        pp.ToDate = now;
                    }
                    else
                    {
                        pp.ToDate = placements[i].AssignmentDate;
                        pp.To = placements[i].AssignmentDateAmharic;
                    }

                }
                else
                {
                    pp.ToDate = placements[i + 1].AssignmentDate;
                    pp.To = placements[i + 1].AssignmentDateAmharic;
                }

                pp.IsHead = placements[i].IsHead;
                pp.IsCurrent = placements[i].isCurrent;
                pp.Salary = placements[i].Salary;
                pp.AssignmentType = placements[i].AssignmentType;
                placeList.Add(pp);
            }

            DataSourceResult result = placeList.ToDataSourceResult(request, placement => new
            {
                PlaceId = placement.PlaceId,
                Branch = placement.Branch,
                Department = placement.Department,
                LookupId = placement.LookupId,
                JobTitle = placement.JobTitle,
                IsHead = placement.IsHead,
                IsCurrent = placement.IsCurrent,
                AssignmentDate = placement.AssignmentDate,
                FromDate = placement.FromDate,
                ToDate = placement.ToDate,
                From = placement.From,
                To = placement.To,
                Salary = placement.Salary,

                AssignmentType = placement.AssignmentType

            });

            return Json(result);


        }
        public ActionResult Placements_HistoryForProfile_ReadCC([DataSourceRequest]DataSourceRequest request, int id)
        {
            HashSet<int> placementIds = new HashSet<int>(db.Placements.Select(x => x.PlacementId));
            var placements = db.PlacementAfterSystems.Where(x=>x.EmployeeId==id&&placementIds.Contains(x.PlacementId)).AsQueryable();
            DataSourceResult result = placements.ToDataSourceResult(request, placement => new
            {
                PlacementId = placement.PlacementId,
                EmployeeId = placement.EmployeeId,
                IsHead = placement.IsHead,
                From = placement.From,
                To = placement.To,
                OrgStructureId = placement.OrgStructureId,
                LookupId = placement.LookupId,
                RankId = placement.RankId,
                StepId = placement.StepId,
                BranchId = placement.BranchId,
                Salary = placement.Salary,
                AssignmentType = placement.AssignmentType,
                ResponsibilityLevel = placement.ResponsibilityLevel,
                EmployementType = placement.EmployementType,
                HireDate = HrHelper.GetHireDate(placement.EmployeeId),
                Age = EmployeeHelper.GetAge(placement.EmployeeId)
            });

            return Json(result);


        }

        //public ActionResult Placements_HistoryForProfile_Read([DataSourceRequest]DataSourceRequest request, int id)
        //{
        //    List<OldPlacementVM> placeList = new List<OldPlacementVM>();

        //    var placements = db.Placements.Where(x => x.EmployeeId == id).OrderByDescending(x=>x.PlacementId).ToList();

        //    foreach (var place in placements)
        //    {
        //        var pp = new OldPlacementVM();
        //        pp.PlaceId = place.PlacementId;
        //        pp.BranchId = place.BranchId;
        //        pp.DepartmentId = place.OrgStructureId;
        //        pp.LookupId = place.LookupId;
        //        pp.AssignmentDate = place.AssignmentDateAmharic;
        //        pp.IsHead = place.IsHead;
        //        pp.IsCurrent = place.isCurrent;
        //        pp.Salary = place.Salary;
        //        pp.AssignmentType = place.AssignmentType;

        //        placeList.Add(pp);
        //    }
        //    DataSourceResult result = placeList.ToDataSourceResult(request, placement => new
        //    {
        //        PlaceId = placement.PlaceId,
        //        BranchId = placement.BranchId,
        //        DepartmentId = placement.DepartmentId,
        //        LookupId = placement.LookupId,
        //        JobTitle = placement.JobTitle,
        //        IsHead = placement.IsHead,
        //        IsCurrent = placement.IsCurrent,
        //        AssignmentDate = placement.AssignmentDate,
        //        Salary = placement.Salary,
        //        AssignmentType = placement.AssignmentType

        //    });

        //    return Json(result);


        //}
        public ActionResult Placements_HistoryForProfile_Update([DataSourceRequest]DataSourceRequest request, OldPlacementVM oldPlacement, int id)
        {
            if (oldPlacement.IsCurrent)
            {
                ModelState.AddModelError("exist", "This is the current Placement could you can edit it in Placement ");
            }
            var placement = db.Placements.Where(x => x.EmployeeId == id &&x.PlacementId==oldPlacement.PlaceId&& !x.isCurrent).FirstOrDefault();

            if ( placement !=null)
            {

               // placement.BranchId = oldPlacement.BranchId;
               // placement.OrgStructureId = oldPlacement.DepartmentId;
                placement.LookupId = oldPlacement.LookupId;
                if(oldPlacement.AssignmentDate != null)
                {

                    var splitDateH = oldPlacement.AssignmentDate.Split('/');
                    placement.AssignmentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateH[0]), Convert.ToInt32(splitDateH[1]), Convert.ToInt32(splitDateH[2]));

                }
                placement.IsHead = oldPlacement.IsHead;
                placement.Salary = oldPlacement.Salary;
                placement.AssignmentType = oldPlacement.AssignmentType;

                db.Entry(placement).State = EntityState.Modified;
                db.SaveChanges();
            }
           

            return Json(new[] { oldPlacement }.ToDataSourceResult(request, ModelState));



        }
        public ActionResult NotPlaced_Employees_Read([DataSourceRequest] DataSourceRequest request)
        {
            var person_employee = db.Person_Employee
                .Where(x => x.IsActive && !x.IsPlaced) // Keep this as IQueryable
                .Select(person_Employee => new
                {
                    EmployeeId = person_Employee.EmployeeId,
                    EmployeeCode = person_Employee.EmployeeCode,
                    Title = person_Employee.Title,
                    FirstName = person_Employee.FirstName,
                    MiddleName = person_Employee.MiddleName,
                    LastName = person_Employee.LastName,
                    Nationality = person_Employee.Nationality,
                    DateOfBirth = person_Employee.DateOfBirth,
                    DateOfBirthAm = person_Employee.DateOfBirthAm,
                    Gender = person_Employee.Gender,
                    CategoryID = person_Employee.CategoryID,
                    IsActive = person_Employee.IsActive,
                    MothersFullName = person_Employee.MothersFullName,
                    BirthPlace = person_Employee.BirthPlace,
                    Nation = person_Employee.Nation,
                    Region = person_Employee.Region,
                    Religion = person_Employee.Religion,
                    MartialStatus = person_Employee.MartialStatus,
                    Status = person_Employee.Status,
                    HiredDate = person_Employee.HiredDate,
                    HiredDateAmharic = person_Employee.HiredDateAmharic,
                    Remark = person_Employee.Remark,
                    FirstNameEnglish = person_Employee.FirstNameEnglish,
                    MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                    LastNameEnglish = person_Employee.LastNameEnglish,
                    IsInsured = person_Employee.IsInsured,
                    BranchId = person_Employee.BranchId,
                    OrgBranchName = person_Employee.OrgBranchName,
                    DateCreated = person_Employee.DateCreated,
                    LastModified = person_Employee.LastModified,
                    CreatedBy = person_Employee.CreatedBy,
                    ModifiedBy = person_Employee.ModifiedBy
                });

            // Apply filtering, sorting, and paging directly on the IQueryable
            DataSourceResult result = person_employee.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Employee Profile");
            #endregion
            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Employee Profile");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        public JsonResult GetNation()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.NATION && l.IsActive).AsQueryable();
            ViewData["NationList"] = lookups;
            return Json(lookups.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNationality()
        {
            var countries = db.Countries.AsQueryable();
            ViewData["Nationalites"] = countries;
            return Json(countries.Select(p => new { CountryId = p.CountryId, Nationality = p.Nationality }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save(IEnumerable<HttpPostedFileBase> files, int id)
        {
            // The Name of the Upload component is "files"

            try
            {
                int i = 0;

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        byte[] imgData;
                        using (Stream inputStream = file.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }

                            imgData = memoryStream.ToArray();
                        }

                        var fileName = Path.GetFileName(file.FileName);
                        string physicalPath = Server.MapPath("~/Content/Images/" + fileName);
                        file.SaveAs(physicalPath);
                        var entity = new Attachment
                        {

                            Reference = id.ToString(),
                            Description = file.FileName,
                            FileType = fileName.Split('.')[1],
                            ReferenceType = (AttachmentType)Enum.Parse(typeof(AttachmentType), fileName.Split('.')[1]),
                            Index = i,
                            BinaryFile = imgData,
                            URL = physicalPath

                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        i++;
                    }

                    // Some browsers send file names with full path.
                    // We are only interested in the file name.

                }

            }
            catch (Exception)
            {

                // throw;
            }
            // Return an empty string to signify success
            return Content("");
        }
        public ActionResult Remove(string[] fileNames, int id)
        {
            // The parameter of the Remove action must be called "fileNames"
            var attachments = db.Attachments.Where(a => a.Reference == id.ToString());
            db = new ExceedDbContext();
            if (attachments.Count() > 0)
            {
                if (fileNames != null)
                {
                    foreach (var a in attachments)
                    {
                        foreach (var fullName in fileNames)
                        {

                            var fileName = Path.GetFileName(fullName);
                            if (a.Description == fileName)
                            {

                                if (System.IO.File.Exists(a.URL))
                                {
                                    // The files are not actually removed in this demo
                                    System.IO.File.Delete(a.URL);
                                }

                                db.Attachments.Attach(a);
                                db.Attachments.Remove(a);
                                db.SaveChanges();
                            }

                        }
                    }
                }

            }
            // Return an empty string to signify success
            return Content("");
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        public ActionResult Attachments_Destroy([DataSourceRequest]DataSourceRequest request, Attachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment
                {
                    AttachmentId = attachment.AttachmentId,
                    ReferenceType = attachment.ReferenceType,
                    Reference = attachment.Reference,
                    Description = attachment.Description,
                    URL = attachment.URL,
                    BinaryFile = attachment.BinaryFile,
                    FileType = attachment.FileType,
                    Index = attachment.Index,
                    Remark = attachment.Remark,
                    FileName = attachment.FileName
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Attachment" + attachment.ReferenceType.ToString() + " " + attachment.Description);
                #endregion
                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
                #region delete file from path
                //string fullPath = Request.MapPath(attachment.URL);
                if (System.IO.File.Exists(attachment.URL))
                {
                    System.IO.File.Delete(attachment.URL);
                }
                #endregion
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FilterMenuCustomization_Cities()
        {
            var db = new ExceedDbContext();
            var emps = db.Person_Employee;
            return Json(emps.Select(p => new { Name = p.FirstName, Id = p.EmployeeId }), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult EmployeeChart([DataSourceRequest]DataSourceRequest request, int id)
        {
            var leaveList = db.Leaves.Where(x => x.EmployeeId == id).Count();
            int internalExpriance = 0;
            int externalExpriance = 0;
            var displineList = db.DisciplinaryCases.Where(x => x.EmployeeId == id).Count();
            // var abcentList = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == id).Count();
            var externalExpriances = db.Experiences.Where(x => x.EmployeeId == id).ToList();
            if (externalExpriances.Any())
            {
                externalExpriance = Convert.ToInt32(externalExpriances.Sum(x => x.TotalDuration));
            }
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id);
            if (employee != null)
            {
                internalExpriance = (DateTime.Now.Year - employee.HiredDate.Year);
            }

            var viewModel = new List<EmployeeChartModel>();
            var data = new EmployeeChartModel();
            if (leaveList > 0)
            {
                //data.EmployeeChartModels = new List<EmployeeChartModel>();

                data.Category = "Leave";
                data.Value = leaveList;
                data.Color = "SteelBlue";
                //data.EmployeeChartModels.Add(data);
                viewModel.Add(data);
            }
            if (displineList > 0)
            {
                data = new EmployeeChartModel();
                //data.EmployeeChartModels = new List<EmployeeChartModel>();
                data.Category = "Dcipline";
                data.Value = displineList;

                data.Color = "LimeGreen";
                //data.EmployeeChartModels.Add(data);
                viewModel.Add(data);
            }
            //if (abcentList > 0)
            //{
            //    data = new EmployeeChartModel();
            //    //data.EmployeeChartModels = new List<EmployeeChartModel>();
            //    data.Category = "Abcent";
            //    data.Value = abcentList;

            //    data.Color = "OrangeRed";
            //    viewModel.Add(data);
            //    //data.EmployeeChartModels.Add(data);
            //}
            data = new EmployeeChartModel();
            //data.EmployeeChartModels = new List<EmployeeChartModel>();
            data.Category = "Internal Expriance";
            data.Value = internalExpriance;
            data.Color = "MediumSlateBlue";
            //data.EmployeeChartModels.Add(data);
            viewModel.Add(data);

            data = new EmployeeChartModel();
            //data.EmployeeChartModels = new List<EmployeeChartModel>();
            data.Category = "External Expriance";
            data.Value = externalExpriance;
            data.Color = "Chocolate";
            //data.EmployeeChartModels.Add(data);
            viewModel.Add(data);
            return Json(viewModel);
        }
        public ActionResult Employee_Import([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        {
            string errors = "Employee Id";
            #region Import

            string addtionalName = DateTime.Now.Year.ToString() +
                DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                DateTime.Now.Second.ToString();
            System.Data.DataSet ds = new System.Data.DataSet();
            Person_Employee issuee = db.Person_Employee.FirstOrDefault();
            List<Person_Employee> issues = new List<Person_Employee>();
            foreach (HttpPostedFileBase fileBase in file)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                {
                    string fileLocation = Server.MapPath("~/Content/Import/HRM/") + addtionalName + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);
                    var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsm")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    System.Data.DataTable dt = new System.Data.DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }

                }

            }
            #region List Of Enums

            var regions = Enum.GetValues(typeof(Region))
                .Cast<Region>()
                .ToList();
            var religionList = Enum.GetValues(typeof(ReligionList))
                .Cast<ReligionList>()
                .ToList();
            var martialStatusList = Enum.GetValues(typeof(MartialStatus))
              .Cast<MartialStatus>()
              .ToList();
            var employeeStatusList = Enum.GetValues(typeof(EmployeeStatus))
             .Cast<EmployeeStatus>()
             .ToList();
            var genderList = Enum.GetValues(typeof(Gender))
            .Cast<Gender>()
            .ToList();
            var personTitleList = db.Lookups.Where(x => x.LookupType == LookUpTypes.PERSON_TITLE).ToList();
            #endregion
            List<Person_Employee> issueImports = new List<Person_Employee>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                {
                    Person_Employee import = new Person_Employee();
                    import.EmployeeCode = ds.Tables[0].Rows[j].ItemArray[0].ToString();
                    if (personTitleList != null)
                    {
                        import.Title = personTitleList.Where(x => x.LookupId.ToString() == (ds.Tables[0].Rows[j].ItemArray[1].ToString()).Trim()).FirstOrDefault().LookupId;
                    }
                    import.FirstName = ds.Tables[0].Rows[j].ItemArray[2].ToString();
                    import.MiddleName = ds.Tables[0].Rows[j].ItemArray[3].ToString();
                    import.LastName = ds.Tables[0].Rows[j].ItemArray[4].ToString();
                    import.Nationality = ds.Tables[0].Rows[j].ItemArray[5].ToString();
                    var splitDate = ds.Tables[0].Rows[j].ItemArray[18].ToString().Split('/');
                    import.DateOfBirth = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                    //import.DateOfBirth = DateOfBirth;
                    import.Gender = genderList.Where(x => x.ToString() == (ds.Tables[0].Rows[j].ItemArray[6].ToString()).Trim()).FirstOrDefault();
                    import.IsActive = Convert.ToBoolean(ds.Tables[0].Rows[j].ItemArray[7].ToString());
                    import.MothersFullName = ds.Tables[0].Rows[j].ItemArray[7].ToString();
                    import.BirthPlace = ds.Tables[0].Rows[j].ItemArray[9].ToString();
                    import.Nation = ds.Tables[0].Rows[j].ItemArray[10].ToString();
                    import.Region = regions.Where(x => x.ToString() == (ds.Tables[0].Rows[j].ItemArray[11].ToString()).Trim()).FirstOrDefault();
                    import.Religion = religionList.Where(x => x.ToString() == (ds.Tables[0].Rows[j].ItemArray[11].ToString()).Trim()).FirstOrDefault();
                    import.MartialStatus = martialStatusList.Where(x => x.ToString() == (ds.Tables[0].Rows[j].ItemArray[13].ToString()).Trim()).FirstOrDefault();
                    import.Status = employeeStatusList.Where(x => x.ToString() == (ds.Tables[0].Rows[j].ItemArray[14].ToString()).Trim()).FirstOrDefault();
                    splitDate = ds.Tables[0].Rows[j].ItemArray[19].ToString().Split('/');
                    import.HiredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                    //import.HiredDate = HireDate;
                    import.FirstNameEnglish = ds.Tables[0].Rows[j].ItemArray[15].ToString();
                    import.MiddleNameEnglish = ds.Tables[0].Rows[j].ItemArray[16].ToString();
                    import.LastNameEnglish = ds.Tables[0].Rows[j].ItemArray[17].ToString();
                    import.DateOfBirthAm = ds.Tables[0].Rows[j].ItemArray[18].ToString();
                    import.HiredDateAmharic = ds.Tables[0].Rows[j].ItemArray[19].ToString();
                    import.IsInsured = Convert.ToBoolean(ds.Tables[0].Rows[j].ItemArray[20].ToString());
                    import.Remark = ds.Tables[0].Rows[j].ItemArray[21].ToString();
                    issueImports.Add(import);
                    //  db.SaveChanges();
                }
            }
            foreach (var person_Employee in issueImports)
            {
                var checkExistance =
                    db.Person_Employee.Where(x => x.EmployeeCode == person_Employee.EmployeeCode).FirstOrDefault();
                if (checkExistance == null)
                {
                    var entity = new Person_Employee
                    {
                        EmployeeCode = person_Employee.EmployeeCode,
                        MothersFullName = person_Employee.MothersFullName,
                        BirthPlace = person_Employee.BirthPlace,
                        Nation = person_Employee.Nation,
                        Region = person_Employee.Region,
                        Religion = person_Employee.Religion,
                        MartialStatus = person_Employee.MartialStatus,
                        Status = person_Employee.Status,
                        HiredDate = person_Employee.HiredDate,
                        FirstName = person_Employee.FirstName, //FirstName,
                        MiddleName = person_Employee.MiddleName, //MiddleName ,
                        LastName = person_Employee.LastName, //LastName
                        Nationality = person_Employee.Nationality,
                        DateOfBirth = person_Employee.DateOfBirth,
                        Title = person_Employee.Title,
                        Gender = person_Employee.Gender,
                        CategoryID = 0,
                        IsActive = person_Employee.IsActive,
                        Remark = person_Employee.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = person_Employee.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = person_Employee.ModifiedBy,
                        FirstNameEnglish = person_Employee.FirstNameEnglish,
                        MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                        LastNameEnglish = person_Employee.LastNameEnglish,
                        DateOfBirthAm = person_Employee.DateOfBirthAm,
                        HiredDateAmharic = person_Employee.HiredDateAmharic,
                        PhoneNo = person_Employee.PhoneNo,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db.Person_Employee.Add(entity);
                    db.SaveChanges();
                    person_Employee.EmployeeId = entity.EmployeeId;
                }
                else
                {
                    errors += " " + person_Employee.EmployeeCode + ",";
                }
            }


            if (errors != "Employee Id")
            {
                ModelState.AddModelError("", errors + "is Exist");
            }
            #endregion

            return Json(new[] { issuee }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult DownloadTemplate(string filename)
        {


            db = new ExceedDbContext();
            try
            {

                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition",
                    "attachment;filename=\"" + filename + "\"");
                byte[] datas = req.DownloadData(Server.MapPath("~/Content/Template/HRM/" + filename));
                response.BinaryWrite(datas);
                response.End();
                HRMInputs.SelectedFile = 0;
                return File(filename, "application/vnd.ms-excel");// attachment.FileType);
            }

            catch (Exception)
            {

                // throw;
            }
            HRMInputs.SelectedFile = 0;
            return null;
        }
        public ActionResult Region_Update()
        {
            var persons = db.Person_Employee.ToList();

            foreach (var person in persons)
            {
                switch (Convert.ToInt32(person.Region))
                {
                    case 0:
                        person.Region = Region.TIGRAY;
                        break;
                    case 1:
                        person.Region = Region.AFAR;
                        break;
                    case 2:
                        person.Region = Region.AMHARA;
                        break;
                    case 3:
                        person.Region = Region.OROMIA;
                        break;
                    case 4:
                        person.Region = Region.SOMALIA;
                        break;
                    case 5:
                        person.Region = Region.SOUTH_NATION_AND_NATIONALITIES;
                        break;
                    case 6:
                        person.Region = Region.BENSHANGUL_GUMUZ;
                        break;
                    case 7:
                        person.Region = Region.HARRARI;
                        break;
                    case 8:
                        person.Region = Region.GAMBELLA;
                        break;
                    case 9:
                        person.Region = Region.ADDIS_ABEBA;
                        break;
                    case 10:
                        person.Region = Region.DIRE_DAWA;
                        break;
                    case 11:
                        person.Region = Region.OTHER;
                        break;
                }

                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Religion_Update()
        {
            var persons = db.Person_Employee.ToList();

            foreach (var person in persons)
            {
                switch (Convert.ToInt32(person.Religion))
                {
                    case 0:
                        person.Religion = ReligionList.ORTHODOX;
                        break;
                    case 1:
                        person.Religion = ReligionList.MUSLIM;
                        break;
                    case 2:
                        person.Religion = ReligionList.CATHOLIC;
                        break;
                    case 3:
                        person.Religion = ReligionList.PROTESTANT;
                        break;
                    case 4:
                        person.Religion = ReligionList.WAAQEFFATAA;
                        break;
                    case 5:
                        person.Religion = ReligionList.OTHER;
                        break;

                }

                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Gender_Update()
        {
            var persons = db.Person_Employee.Where(x => x.Gender != Gender.MALE && x.Gender != Gender.FEMALE).ToList();

            foreach (var person in persons)
            {
                switch (Convert.ToInt32(person.Gender))
                {
                    case 0:
                        person.Gender = Gender.MALE;
                        break;
                    case 1:
                        person.Gender = Gender.FEMALE;
                        break;
                }

                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult MartialStatus_Update()
        {
            var persons = db.Person_Employee.ToList();

            foreach (var person in persons)
            {
                switch (Convert.ToInt32(person.MartialStatus))
                {
                    case 0:
                        person.MartialStatus = MartialStatus.SINGLE;
                        break;
                    case 1:
                        person.MartialStatus = MartialStatus.MARRIED;
                        break;
                    case 2:
                        person.MartialStatus = MartialStatus.SEPARATED;
                        break;
                    case 3:
                        person.MartialStatus = MartialStatus.DIVORCED;
                        break;
                    case 4:
                        person.MartialStatus = MartialStatus.OTHER;
                        break;

                }

                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult IsPlacedField_Update()
        {
            var persons = db.Person_Employee.ToList();

            foreach (var person in persons)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == person.EmployeeId);
                if (placement != null)
                {
                    person.IsPlaced = true;

                    db.Entry(person).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult ActivateUnplacedEmployees_Update()
        {
            //activate the employee if he/she has no placement , because they are not terminated or retired employee
            var persons = db.Person_Employee.ToList();

            foreach (var person in persons)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == person.EmployeeId);
                if (placement == null)
                {
                    person.IsActive = true;

                    db.Entry(person).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult ReHireRetiredEmployees(int employeeId)
        {
            try
            {
                var employee = db.Person_Employee
                                 .FirstOrDefault(x => x.EmployeeId == employeeId);
               

                  
                if (employee == null)
                {
                    return Json(new
                    {
                        Status = "NOT_FOUND",
                        Message = "Employee not found."
                    });
                }

                if (employee.IsActive)
                {
                    return Json(new
                    {
                        Status = "ALREADY_ACTIVE",
                        Message = "Employee is already active."
                    });
                }

                employee.IsActive = true;
                employee.IsReHired = true;

                using (var adb = new ApplicationDbContext())
                {
                    var user = adb.Users.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
                    //enable deactivate account
                    if (user != null)
                        user.IsEnabled = true;//make active 
                    adb.SaveChanges();  // Identity DB
                }
                db.SaveChanges();

                return Json(new
                {
                    Status = "OK",
                    Message = "Employee rehired successfully."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Status = "ERROR",
                    Message = "An error occurred while rehiring the employee."
                    // optionally: ex.Message (avoid exposing in production)
                });
            }
        }
        public ActionResult EmployeeFolderAttachmentForEmployeeProfiles_Read([DataSourceRequest]DataSourceRequest request, int empId)
        {
            var personalArchiveNoIds = db.PersonalArchiveNo.Where(x => x.EmployeeId == empId).Select(x => x.PersonalArchiveNoId);
            var employeefolderattachments = db.EmployeeFolderAttachments.Where(x => personalArchiveNoIds.Contains(x.PersonalArchiveNoId)).ToList();

            DataSourceResult result = employeefolderattachments.ToDataSourceResult(request, employeeFolderAttachment => new
            {
                EmployeeFolderAttachmentId = employeeFolderAttachment.EmployeeFolderAttachmentId,
                PersonalArchiveNoId = employeeFolderAttachment.PersonalArchiveNoId,
                FileName = employeeFolderAttachment.FileName,
                FilePath = employeeFolderAttachment.FilePath,
                ShortPath = employeeFolderAttachment.ShortPath,
                FileExtension = employeeFolderAttachment.FileExtension,
                Description = employeeFolderAttachment.Description,
                Remark = employeeFolderAttachment.Remark
            });

            return Json(result);
        }
        public ActionResult EmployeeFolderAttachments_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeFolderAttachment employeeFolderAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeFolderAttachment
                {
                    EmployeeFolderAttachmentId = employeeFolderAttachment.EmployeeFolderAttachmentId,
                    PersonalArchiveNoId = employeeFolderAttachment.PersonalArchiveNoId,
                    FileName = employeeFolderAttachment.FileName,
                    FilePath = employeeFolderAttachment.FilePath,
                    Remark = employeeFolderAttachment.Remark
                };

                db.EmployeeFolderAttachments.Attach(entity);
                db.EmployeeFolderAttachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeFolderAttachment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult ResolveImage(int id)
        {
            var folderPath = Server.MapPath($"~/Content/Images/{id}-Photo/");
            if (!Directory.Exists(folderPath))
            {
                return File(Server.MapPath("~/Content/Images/default.png"), "image/png");
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var file = Directory.GetFiles(folderPath)
                                .FirstOrDefault(f =>
                                    Path.GetFileNameWithoutExtension(f).Equals(id.ToString(), StringComparison.OrdinalIgnoreCase) &&
                                    allowedExtensions.Contains(Path.GetExtension(f).ToLower()));

            if (file != null)
            {
                return File(file, MimeMapping.GetMimeMapping(file));
            }

            return File(Server.MapPath("~/Content/Images/default.png"), "image/png");
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttachmentTemplateFiles_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> employeeProfileFiles, string attachmentReferenceType, int? id, string fileOwner = "")
        {
            if (!id.HasValue)
            {
                id = HRMInputs.EmployeeId;
                fileOwner = HRMInputs.EmployeeId.ToString();
            }
            if (employeeProfileFiles != null)
            {
                AttachmentType value = AttachmentType.OTHER;
                if (attachmentReferenceType != null)
                {
                    switch (attachmentReferenceType.ToLower())
                    {
                        case "photo":
                            value = AttachmentType.PHOTO;
                            break;
                        case "experience":
                            value = AttachmentType.EXPERIENCE;
                            break;
                        case "education":
                            value = AttachmentType.EDUCATION;
                            break;
                        case "training":
                            value = AttachmentType.TRAINING;
                            break;
                        case "clearance":
                            value = AttachmentType.CLEARANCE;
                            break;
                        case "commitment":
                            value = AttachmentType.COMMITMENT;
                            break;
                        case "incomingletter":
                            value = AttachmentType.INCOMING_LETTER;
                            break;
                        case "outgoingletter":
                            value = AttachmentType.OUTGOING_LETTER;
                            break;
                        case "other":
                            value = AttachmentType.OTHER;
                            break;
                        case "logo":
                            value = AttachmentType.LOGO;
                            break;
                        case "cosign":
                            value = AttachmentType.COSIGN;
                            HRMInputs.CosignId = id.Value;
                            break;
                        case "letter":
                            value = AttachmentType.LETTER;
                            HRMInputs.LetterId = id.ToString();
                            break;
                        case "minute":
                            value = AttachmentType.MINUTE;
                            break;
                        case "document":
                            value = AttachmentType.DOCUMENT;
                            break;
                        case "project_document":
                            value = AttachmentType.PROJECT_DOCUMENT;
                            break;
                    }
                }
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                if (value == AttachmentType.PHOTO)
                {
                    AllowedExtensions = new[] { ".jpg", ".png" };
                }
                int MaxFileSize = 2 * 1024 * 1024;

                string subPath = "~/Content/Images/";
                string shortPath = "/Content/Images/";
                bool exists = Directory.Exists(Server.MapPath("~/Content/Images/HRMFiles/"));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath("~/Content/Images/HRMFiles/"));
                string fileName = "";
                foreach (var f in employeeProfileFiles)
                {

                    if (f != null)
                    {
                        #region file validation
                        var fileExtension = Path.GetExtension(f.FileName).ToLower();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Invalid file type.");
                            return Json(new[] { employeeProfileFiles }.ToDataSourceResult(request, ModelState));

                        }

                        if (f.ContentLength > MaxFileSize)
                        {
                            ModelState.AddModelError("File", $"The uploaded file exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)} MB.");
                            return Json(new[] { employeeProfileFiles }.ToDataSourceResult(request, ModelState));

                        }
                        #endregion
                        if (ModelState.IsValid)
                        {
                            if(value == AttachmentType.PHOTO)
                            {
                                // Validate extension and MIME type
                                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                                var extension = Path.GetExtension(f.FileName).ToLower();

                                if (!allowedExtensions.Contains(extension) || !allowedMimeTypes.Contains(f.ContentType.ToLower()))
                                {
                                    ModelState.AddModelError("File", "Invalid file type. Only images are allowed.");
                                    return Json(new List<Attachment>().ToDataSourceResult(request, ModelState));
                                }

                                const int maxFileSize = 2 * 1024 * 1024; // 2MB
                                if (f.ContentLength > maxFileSize)
                                {
                                    ModelState.AddModelError("File", $"File exceeds {maxFileSize / (1024 * 1024)} MB.");
                                    return Json(new[] { employeeProfileFiles }.ToDataSourceResult(request, ModelState));
                                }
                                var employeeId = HRMInputs.EmployeeId;
                                // Build path
                                var folderName = $"{employeeId}-{AttachmentType.PHOTO}";
                                var folderPath = Path.Combine(Server.MapPath("~/Content/Images/"), folderName);
                                if (!Directory.Exists(folderPath))
                                    Directory.CreateDirectory(folderPath);

                                var fileNamee = $"{employeeId}{extension}";
                                var path = Path.Combine(folderPath, fileNamee);

                                // Delete existing file if exists
                                if (System.IO.File.Exists(path))
                                    System.IO.File.Delete(path);

                                // Save new file
                                f.SaveAs(path);

                                // Remove existing attachment records
                                var existing = db.Attachments.Where(a => a.Reference == employeeId.ToString() && a.ReferenceType == AttachmentType.PHOTO);
                                if (existing.Any())
                                {
                                    db.Attachments.RemoveRange(existing);
                                    db.SaveChanges();
                                }

                                // Add new attachment record
                                var employee = db.Person_Employee.Find(employeeId);
                                var attachment = new Attachment
                                {
                                    Reference = employeeId.ToString(),
                                    Description = employee != null ? $"{employee.EmployeeCode}{extension}" : f.FileName,
                                    FileType = f.ContentType,
                                    ReferenceType = AttachmentType.PHOTO,
                                    Index = 0,
                                    URL = path,
                                    Remark = HRMInputs.Owner
                                };

                                db.Attachments.Add(attachment);
                                db.SaveChanges();
                            }
                            else
                            {
                                fileName = Path.GetFileName(f.FileName);
                                string physicalPath = Server.MapPath("~/Content/Images/" + fileName);
                                //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                                f.SaveAs(physicalPath);
                                db.Attachments.Add(new Attachment()
                                {
                                    ReferenceType = value,
                                    Reference = id.ToString(),
                                    Description = value.ToString(),
                                    URL = physicalPath,
                                    BinaryFile = ConvertToBytes(f),
                                    FileType = fileExtension,
                                    Index = 0,
                                    Remark = fileOwner,
                                    FileName = Path.GetFileName(f.FileName),

                                });
                                db.SaveChanges();
                            }
                            
                        }
                    }
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "HRM Attachment template", "Success", User.Identity.Name, $"uploaded HRM Attachment template {fileName} ", Modules.HRM);
                #endregion

            }
            return Json(new[] { employeeProfileFiles }.ToDataSourceResult(request, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttachmentTemplateDocumentFiles_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> employeeProfileDocumentFiles, string attachmentReferenceType, int? id, string fileOwner = "")
        {
            if (!id.HasValue)
            {
                id = HRMInputs.EmployeeId;
                fileOwner = HRMInputs.EmployeeId.ToString();
            }
            if (employeeProfileDocumentFiles != null)
            {
                AttachmentType value = AttachmentType.OTHER;
                if (attachmentReferenceType != null)
                {
                    switch (attachmentReferenceType.ToLower())
                    {
                        case "photo":
                            value = AttachmentType.PHOTO;
                            break;
                        case "experience":
                            value = AttachmentType.EXPERIENCE;
                            break;
                        case "education":
                            value = AttachmentType.EDUCATION;
                            break;
                        case "training":
                            value = AttachmentType.TRAINING;
                            break;
                        case "clearance":
                            value = AttachmentType.CLEARANCE;
                            break;
                        case "commitment":
                            value = AttachmentType.COMMITMENT;
                            break;
                        case "incomingletter":
                            value = AttachmentType.INCOMING_LETTER;
                            break;
                        case "outgoingletter":
                            value = AttachmentType.OUTGOING_LETTER;
                            break;
                        case "other":
                            value = AttachmentType.OTHER;
                            break;
                        case "logo":
                            value = AttachmentType.LOGO;
                            break;
                        case "cosign":
                            value = AttachmentType.COSIGN;
                            HRMInputs.CosignId = id.Value;
                            break;
                        case "letter":
                            value = AttachmentType.LETTER;
                            HRMInputs.LetterId = id.ToString();
                            break;
                        case "minute":
                            value = AttachmentType.MINUTE;
                            break;
                        case "document":
                            value = AttachmentType.DOCUMENT;
                            break;
                        case "project_document":
                            value = AttachmentType.PROJECT_DOCUMENT;
                            break;
                    }
                }
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                if (value == AttachmentType.PHOTO)
                {
                    AllowedExtensions = new[] { ".jpg", ".png" };
                }
                int MaxFileSize = 6 * 1024 * 1024;

                string subPath = "~/Content/Images/";
                string shortPath = "/Content/Images/";
                bool exists = Directory.Exists(Server.MapPath("~/Content/Images/HRMFiles/"));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath("~/Content/Images/HRMFiles/"));
                string fileName = "";
                foreach (var f in employeeProfileDocumentFiles)
                {

                    if (f != null)
                    {
                        #region file validation
                        var fileExtension = Path.GetExtension(f.FileName).ToLower();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Invalid file type.");
                            return Json(new[] { employeeProfileDocumentFiles }.ToDataSourceResult(request, ModelState));

                        }

                        if (f.ContentLength > MaxFileSize)
                        {
                            ModelState.AddModelError("File", $"The uploaded file exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)} MB.");
                            return Json(new[] { employeeProfileDocumentFiles }.ToDataSourceResult(request, ModelState));

                        }
                        #endregion
                        if (ModelState.IsValid)
                        {
                            if(value == AttachmentType.PHOTO)
                            {
                                // Validate extension and MIME type
                                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                                var extension = Path.GetExtension(f.FileName).ToLower();

                                if (!allowedExtensions.Contains(extension) || !allowedMimeTypes.Contains(f.ContentType.ToLower()))
                                {
                                    ModelState.AddModelError("File", "Invalid file type. Only images are allowed.");
                                    return Json(new List<Attachment>().ToDataSourceResult(request, ModelState));
                                }

                                const int maxFileSize = 6 * 1024 * 1024; // 2MB
                                if (f.ContentLength > maxFileSize)
                                {
                                    ModelState.AddModelError("File", $"File exceeds {maxFileSize / (1024 * 1024)} MB.");
                                    return Json(new[] { employeeProfileDocumentFiles }.ToDataSourceResult(request, ModelState));
                                }
                                var employeeId = HRMInputs.EmployeeId;
                                // Build path
                                var folderName = $"{employeeId}-{AttachmentType.PHOTO}";
                                var folderPath = Path.Combine(Server.MapPath("~/Content/Images/"), folderName);
                                if (!Directory.Exists(folderPath))
                                    Directory.CreateDirectory(folderPath);

                                var fileNamee = $"{employeeId}{extension}";
                                var path = Path.Combine(folderPath, fileNamee);

                                // Delete existing file if exists
                                if (System.IO.File.Exists(path))
                                    System.IO.File.Delete(path);

                                // Save new file
                                f.SaveAs(path);

                                // Remove existing attachment records
                                var existing = db.Attachments.Where(a => a.Reference == employeeId.ToString() && a.ReferenceType == AttachmentType.PHOTO);
                                if (existing.Any())
                                {
                                    db.Attachments.RemoveRange(existing);
                                    db.SaveChanges();
                                }

                                // Add new attachment record
                                var employee = db.Person_Employee.Find(employeeId);
                                var attachment = new Attachment
                                {
                                    Reference = employeeId.ToString(),
                                    Description = employee != null ? $"{employee.EmployeeCode}{extension}" : f.FileName,
                                    FileType = f.ContentType,
                                    ReferenceType = AttachmentType.PHOTO,
                                    Index = 0,
                                    URL = path,
                                    Remark = HRMInputs.Owner
                                };

                                db.Attachments.Add(attachment);
                                db.SaveChanges();
                            }
                            else
                            {
                                fileName = Path.GetFileName(f.FileName);
                                string physicalPath = Server.MapPath("~/Content/Images/" + fileName);
                                //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                                f.SaveAs(physicalPath);
                                db.Attachments.Add(new Attachment()
                                {
                                    ReferenceType = value,
                                    Reference = id.ToString(),
                                    Description =value.ToString(),
                                    URL = physicalPath,
                                    BinaryFile = ConvertToBytes(f),
                                    FileType = fileExtension,
                                    Index = 0,
                                    Remark = fileOwner,
                                    FileName = Path.GetFileName(f.FileName),

                                });
                                db.SaveChanges();
                            }
                            
                        }
                    }
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "HRM Attachment template", "Success", User.Identity.Name, $"uploaded HRM Attachment template {fileName} ", Modules.HRM);
                #endregion

            }
            return Json(new[] { employeeProfileDocumentFiles }.ToDataSourceResult(request, ModelState));

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttachmentOtheDocumentFiles_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> documentFiles, string attachmentReferenceType, int? id, string fileOwner = "")
        {
            if (!id.HasValue)
            {
                id = HRMInputs.EmployeeId;
                fileOwner = HRMInputs.EmployeeId.ToString();
            }
            if (documentFiles != null)
            {
                AttachmentType value = AttachmentType.OTHER;
                if (attachmentReferenceType != null)
                {
                    switch (attachmentReferenceType.ToLower())
                    {
                        case "photo":
                            value = AttachmentType.PHOTO;
                            break;
                        case "experience":
                            value = AttachmentType.EXPERIENCE;
                            break;
                        case "education":
                            value = AttachmentType.EDUCATION;
                            break;
                        case "training":
                            value = AttachmentType.TRAINING;
                            break;
                        case "clearance":
                            value = AttachmentType.CLEARANCE;
                            break;
                        case "commitment":
                            value = AttachmentType.COMMITMENT;
                            break;
                        case "incomingletter":
                            value = AttachmentType.INCOMING_LETTER;
                            break;
                        case "outgoingletter":
                            value = AttachmentType.OUTGOING_LETTER;
                            break;
                        case "other":
                            value = AttachmentType.OTHER;
                            break;
                        case "logo":
                            value = AttachmentType.LOGO;
                            break;
                        case "cosign":
                            value = AttachmentType.COSIGN;
                            HRMInputs.CosignId = id.Value;
                            break;
                        case "letter":
                            value = AttachmentType.LETTER;
                            HRMInputs.LetterId = id.ToString();
                            break;
                        case "minute":
                            value = AttachmentType.MINUTE;
                            break;
                        case "document":
                            value = AttachmentType.DOCUMENT;
                            break;
                        case "project_document":
                            value = AttachmentType.PROJECT_DOCUMENT;
                            break;
                    }
                }
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                if (value == AttachmentType.PHOTO)
                {
                    AllowedExtensions = new[] { ".jpg", ".png" };
                }
                int MaxFileSize = 6 * 1024 * 1024;//6MB

                string subPath = "~/Content/Images/";
                string shortPath = "/Content/Images/";
                bool exists = Directory.Exists(Server.MapPath("~/Content/Images/HRMFiles/"));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath("~/Content/Images/HRMFiles/"));
                string fileName = "";
                foreach (var f in documentFiles)
                {

                    if (f != null)
                    {
                        #region file validation
                        var fileExtension = Path.GetExtension(f.FileName).ToLower();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Invalid file type.");
                            return Json(new[] { documentFiles }.ToDataSourceResult(request, ModelState));

                        }

                        if (f.ContentLength > MaxFileSize)
                        {
                            ModelState.AddModelError("File", $"The uploaded file exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)} MB.");
                            return Json(new[] { documentFiles }.ToDataSourceResult(request, ModelState));

                        }
                        #endregion
                        if (ModelState.IsValid)
                        {
                            if(value == AttachmentType.PHOTO)
                            {
                                // Validate extension and MIME type
                                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                                var allowedMimeTypes = new[] { "image/jpeg", "image/png", "image/gif" };
                                var extension = Path.GetExtension(f.FileName).ToLower();

                                if (!allowedExtensions.Contains(extension) || !allowedMimeTypes.Contains(f.ContentType.ToLower()))
                                {
                                    ModelState.AddModelError("File", "Invalid file type. Only images are allowed.");
                                    return Json(new List<Attachment>().ToDataSourceResult(request, ModelState));
                                }

                                const int maxFileSize = 6 * 1024 * 1024; // 6MB
                                if (f.ContentLength > maxFileSize)
                                {
                                    ModelState.AddModelError("File", $"File exceeds {maxFileSize / (1024 * 1024)} MB.");
                                    return Json(new[] { documentFiles }.ToDataSourceResult(request, ModelState));
                                }
                                var employeeId = HRMInputs.EmployeeId;
                                // Build path
                                var folderName = $"{employeeId}-{AttachmentType.PHOTO}";
                                var folderPath = Path.Combine(Server.MapPath("~/Content/Images/"), folderName);
                                if (!Directory.Exists(folderPath))
                                    Directory.CreateDirectory(folderPath);

                                var fileNamee = $"{employeeId}{extension}";
                                var path = Path.Combine(folderPath, fileNamee);

                                // Delete existing file if exists
                                if (System.IO.File.Exists(path))
                                    System.IO.File.Delete(path);

                                // Save new file
                                f.SaveAs(path);

                                // Remove existing attachment records
                                var existing = db.Attachments.Where(a => a.Reference == employeeId.ToString() && a.ReferenceType == AttachmentType.PHOTO);
                                if (existing.Any())
                                {
                                    db.Attachments.RemoveRange(existing);
                                    db.SaveChanges();
                                }

                                // Add new attachment record
                                var employee = db.Person_Employee.Find(employeeId);
                                var attachment = new Attachment
                                {
                                    Reference = employeeId.ToString(),
                                    Description = employee != null ? $"{employee.EmployeeCode}{extension}" : f.FileName,
                                    FileType = f.ContentType,
                                    ReferenceType = AttachmentType.PHOTO,
                                    Index = 0,
                                    URL = path,
                                    Remark = HRMInputs.Owner
                                };

                                db.Attachments.Add(attachment);
                                db.SaveChanges();
                            }
                            else
                            {
                                fileName = Path.GetFileName(f.FileName);
                                string physicalPath = Server.MapPath("~/Content/Images/" + fileName);
                                //   string physicalPath = Server.MapPath("~/Content/Memberimage/" + FileName);
                                f.SaveAs(physicalPath);
                                db.Attachments.Add(new Attachment()
                                {
                                    ReferenceType = value,
                                    Reference = id.ToString(),
                                    Description = value.ToString(),
                                    URL = physicalPath,
                                    BinaryFile = ConvertToBytes(f),
                                    FileType = fileExtension,
                                    Index = 0,
                                    Remark = fileOwner,
                                    FileName = Path.GetFileName(f.FileName),

                                });
                                db.SaveChanges();
                            }
                            
                        }
                    }
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "HRM Attachment template", "Success", User.Identity.Name, $"uploaded HRM Attachment template {fileName} ", Modules.HRM);
                #endregion

            }
            return Json(new[] { documentFiles }.ToDataSourceResult(request, ModelState));

        }

        private byte[] ConvertToBytes(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                file.InputStream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
        #region Add employee to biometric

        [HttpPost]
        public async Task<JsonResult> RegisterEmployeeToBiometric(int empId)
        {
            try
            {
                string baseUrl = WebConfigurationManager.AppSettings["BiometricBaseUrl"];
                string apiUrl = WebConfigurationManager.AppSettings["EmployeeRegistrationApiUrl"];
                string username = WebConfigurationManager.AppSettings["JwtUsername"];
                string password = WebConfigurationManager.AppSettings["JwtPassword"];

                using (var context = new ExceedDbContext())
                {
                    var emp = context.Person_Employee
                        .FirstOrDefault(x => x.EmployeeId == empId && x.IsActive);

                    if (emp == null)
                        return JsonError("Employee not found.");

                    if (emp.IsBiometricRegistered)
                        return JsonError("Employee already registered.");
                    var empCode = emp.EmployeeCode ?? "";
                    var firstName = emp.FirstNameEnglish ?? emp.FirstName ?? "";
                    var lastName = emp.LastNameEnglish ?? emp.LastName ?? "";
                    var nickName = emp.MiddleNameEnglish ?? emp.MiddleName ?? "";

                    // Check if ALL are null or empty
                    if (string.IsNullOrWhiteSpace(empCode) &&
                        string.IsNullOrWhiteSpace(firstName) &&
                        string.IsNullOrWhiteSpace(lastName) &&
                        string.IsNullOrWhiteSpace(nickName))
                    {
                        return JsonError("Authentication failed: employee record does not contain required identity fields (code, first name, last name, or mddle(nickname).");
                    }

                    var placement = context.Placements.Where(p => p.EmployeeId == empId && p.isCurrent)
                                                      .OrderByDescending(p => p.AssignmentDate).FirstOrDefault();

                    if (string.IsNullOrWhiteSpace(baseUrl))
                        return JsonError("BiometricBaseUrl is empty.");

                    if (string.IsNullOrWhiteSpace(apiUrl))
                        return JsonError("EmployeeRegistrationApiUrl is empty.");

                    if (string.IsNullOrWhiteSpace(empCode))
                        return JsonError("Employee code is required.");

                    if (string.IsNullOrWhiteSpace(firstName))
                        return JsonError("Employee first name is required.");
                  
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseUrl);

                        // Login
                        var loginData = new
                        {
                            username,
                            password
                        };

                        var loginContent = new StringContent(
                            JsonConvert.SerializeObject(loginData),
                            Encoding.UTF8,
                            "application/json"
                        );

                        var loginResponse = await client.PostAsync("jwt-api-token-auth/", loginContent);

                        if (!loginResponse.IsSuccessStatusCode)
                            return JsonError("JWT authentication failed.");

                        var loginResult = await loginResponse.Content.ReadAsStringAsync();
                        dynamic tokenObj = JsonConvert.DeserializeObject(loginResult);
                        string token = tokenObj.token;

                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("JWT", token);

                        // SAFE DATA MAPPING
                        var biometricEmployee = new
                        {
                            emp_code = empCode,
                            first_name = firstName,
                            last_name = lastName,
                            nickname = nickName,

                            hire_date = emp.HiredDate != null
                                ? emp.HiredDate.ToString("yyyy-MM-dd")
                                : "2018-01-01",

                            birthday = emp.DateOfBirth != null
                                ? emp.DateOfBirth.ToString("yyyy-MM-dd")
                                : "2000-01-01",

                            // IMPORTANT FIX
                            //gender = emp.Gender == null
                            //    ? (int?)null
                            //    : (emp.Gender == Gender.MALE ? 1 : 2),


                            // DO NOT SEND gender

                            department = 1,
                            area = new int[] { 2 }, //

                            mobile = string.IsNullOrWhiteSpace(emp.PhoneNo) ? "N/A" : emp.PhoneNo,
                            city = string.IsNullOrWhiteSpace(emp.City) ? "Addis Ababa" : emp.City,

                            address = string.IsNullOrWhiteSpace($"{emp.SubCity}{emp.Wereda}")? "Not Specified": $"{emp.SubCity ?? ""} {emp.Wereda ?? ""}".Trim(),

                            national = string.IsNullOrWhiteSpace(emp.Nationality) ? "Ethiopian" : emp.Nationality,

                            verify_mode = 0
                        };

                        // Ignore nulls (VERY IMPORTANT)
                        var json = JsonConvert.SerializeObject(
                            biometricEmployee,
                            new JsonSerializerSettings
                            {
                                NullValueHandling = NullValueHandling.Ignore
                            });
                        //System.IO.File.AppendAllText(@"C:\Temp\BiometricLog.txt",json + Environment.NewLine + Environment.NewLine);

                        var content = new StringContent(json, Encoding.UTF8, "application/json");

                        var response = await client.PostAsync(apiUrl, content);

                        if (!response.IsSuccessStatusCode)
                        {
                            var error = await response.Content.ReadAsStringAsync();
                            return JsonError(error);
                        }
                        var responseContent = await response.Content.ReadAsStringAsync();
                        dynamic result = JsonConvert.DeserializeObject(responseContent);

                        emp.IsBiometricRegistered = true;
                        emp.BiometricEmployeeId = result.id;

                        context.SaveChanges();
                       
                        return JsonSuccess("Employee successfully registered to biometric system.");
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }


        [HttpPost]
        public JsonResult ToggleBiometricExemption(int empId)
        {
            var employee = db.Person_Employee.FirstOrDefault(x => x.IsActive && x.EmployeeId == empId);
            if (employee == null)
            {
                return JsonError("No active employee found.");
            }

            var placements = db.Placements.Where(x => x.EmployeeId == empId && x.isCurrent).ToList();
            if (!placements.Any())
            {
                return JsonError("No current placement found for this employee.");
            }

            // Toggle the current state
            bool newState = !employee.IsAttendanceExempt;

            employee.IsAttendanceExempt = newState;
            db.Entry(employee).State = EntityState.Modified;

            foreach (var placement in placements)
            {
                placement.IsAttendanceExempt = newState;
                db.Entry(placement).State = EntityState.Modified;
            }

            db.SaveChanges();

            string actionText = newState ? "exempted from" : "included back in";
            return JsonSuccess($"Employee successfully {actionText} the attendance system.");
        }

        [HttpPost]
        public async Task<JsonResult> RegisterAllEmployeesToBiometric()
        {
            try
            {
                string baseUrl = WebConfigurationManager.AppSettings["BiometricBaseUrl"];
                string apiUrl = WebConfigurationManager.AppSettings["EmployeeRegistrationApiUrl"];
                string username = WebConfigurationManager.AppSettings["JwtUsername"];
                string password = WebConfigurationManager.AppSettings["JwtPassword"];

                using (var context = new ExceedDbContext())
                {
                    // Get branch IDs the current user has access to
                    var branchIds = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);

                    // Fetch active employees in those branches who are not yet registered
                    var employees = context.Person_Employee
                        .Where(emp => emp.IsActive
                                      && !emp.IsBiometricRegistered
                                      && branchIds.Contains(emp.BranchId))
                        .ToList();
                    if (!employees.Any())
                        return JsonError("No employees found to register.");

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(baseUrl);

                        // JWT login once
                        var loginData = new { username, password };
                        var loginContent = new StringContent(
                            JsonConvert.SerializeObject(loginData),
                            Encoding.UTF8,
                            "application/json"
                        );

                        var loginResponse = await client.PostAsync("jwt-api-token-auth/", loginContent);
                        if (!loginResponse.IsSuccessStatusCode)
                            return JsonError("JWT authentication failed.");

                        var loginResult = await loginResponse.Content.ReadAsStringAsync();
                        dynamic tokenObj = JsonConvert.DeserializeObject(loginResult);
                        string token = tokenObj.token;
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("JWT", token);

                        int successCount = 0;
                        var failedEmployees = new List<string>();

                        // Loop through each employee
                        foreach (var emp in employees)
                        {
                            var empCode = emp.EmployeeCode ?? "";
                            var firstName = emp.FirstNameEnglish ?? emp.FirstName ?? "";
                            var lastName = emp.LastNameEnglish ?? emp.LastName ?? "";
                            var nickName = emp.MiddleNameEnglish ?? emp.MiddleName ?? "";

                            // Check if ALL are null or empty
                            if (string.IsNullOrWhiteSpace(empCode) &&
                                string.IsNullOrWhiteSpace(firstName) &&
                                string.IsNullOrWhiteSpace(lastName) &&
                                string.IsNullOrWhiteSpace(nickName))
                            {
                                continue;
                            }

                            // Prepare employee payload
                            var biometricEmployee = new
                            {
                                emp_code = empCode,
                                first_name = firstName,
                                last_name = lastName,
                                nickname = nickName,

                                hire_date = emp?.HiredDate.ToString("yyyy-MM-dd") ?? "2000-01-01",
                                birthday = emp?.DateOfBirth.ToString("yyyy-MM-dd") ?? "2019-01-01",

                                //  DO NOT SEND gender

                                department = 1,
                                area = new int[] { 2 },

                                mobile = emp.PhoneNo ?? "",
                                city = emp.City ?? "",
                                address = $"{emp.SubCity ?? ""} {emp.Wereda ?? ""}".Trim(),

                                national = emp.Nationality ?? "",
                                verify_mode = 0
                            };

                            var json = JsonConvert.SerializeObject(
                                biometricEmployee,
                                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }
                            );

                            var content = new StringContent(json, Encoding.UTF8, "application/json");

                            try
                            {
                                var response = await client.PostAsync(apiUrl, content);
                                if (response.IsSuccessStatusCode)
                                {
                                    var responseContent = await response.Content.ReadAsStringAsync();
                                    dynamic result = JsonConvert.DeserializeObject(responseContent);

                                    // Update employee record
                                    emp.IsBiometricRegistered = true;
                                    emp.BiometricEmployeeId = result.id;
                                    successCount++;
                                }
                                else
                                {
                                    var error = await response.Content.ReadAsStringAsync();
                                    failedEmployees.Add($"{emp.EmployeeCode}: {error}");
                                }
                            }
                            catch (Exception ex)
                            {
                                failedEmployees.Add($"{emp.EmployeeCode}: {ex.Message}");
                            }
                        }

                        // Save all changes at once
                        context.SaveChanges();

                        string message = $"Successfully registered {successCount} employees.";
                        if (failedEmployees.Any())
                            message += $" Failed: {string.Join("; ", failedEmployees)}";

                        return JsonSuccess(message);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonError(ex.Message);
            }
        }
        private JsonResult JsonSuccess(string message, object data = null)
        {
            return Json(new
            {
                success = true,
                message = message,
                data = data
            }, JsonRequestBehavior.AllowGet);
        }

        private JsonResult JsonError(string message)
        {
            return Json(new
            {
                success = false,
                message = message
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        [HttpPost]
        public JsonResult GetEmployeeCode(DateTime? hireDate, string hirDateAm)
        {
            DateTime effectiveDate = DateTime.Now;

            if (!string.IsNullOrWhiteSpace(hirDateAm))
            {
                // Ethiopian date takes priority (explicit)
                effectiveDate = HrHelper.ParseEthiopicDate(hirDateAm);
            }
            else if (hireDate.HasValue)
            {
                effectiveDate = hireDate.Value;
            }

            var code = PreviewEmployeeCode(effectiveDate);

            return Json(code, JsonRequestBehavior.AllowGet);
        }

        public string PreviewEmployeeCode(DateTime date)
        {
            var empCode = "";
            var name = "";
            int sequenceNo = 0;
            int digitLength = 0;

            var config = db.Configurations
                .FirstOrDefault(x => x.Category == "HRM" && x.Attribute == EmployeeIdAttribute.Auto_Generate.ToString());

            if (config != null)
            {
                var empCodeSettings = db.EmployeeCodeSettings
                    .OrderBy(x => x.Index)
                    .ToList();

                List<string> emp = new List<string>();

                if (empCodeSettings.Any())
                {
                    foreach (var code in empCodeSettings)
                    {
                        // Company Abbreviation
                        if (code.Type == EmployeeIDAbbreviationType.Company)
                        {
                            var company = db.Companies.FirstOrDefault();
                            if (company != null)
                            {
                                emp.Add(company.Abbreviation);
                                name = company.Abbreviation;
                            }
                        }

                        // Sequential Number (READ ONLY)
                        if (code.Type == EmployeeIDAbbreviationType.Sequential_Number)
                        {
                            var company = db.Companies.FirstOrDefault();
                            if (company != null)
                            {
                                var setting = db.EmployeeCodeSequenceSettings
                                    .Where(x => x.Name == company.Abbreviation)
                                    .OrderByDescending(x => x.IndexCount)
                                    .FirstOrDefault();

                                if (setting != null)
                                {
                                    sequenceNo = setting.IndexCount + 1;
                                    digitLength = setting.ItemCodeTrim;

                                    string newId = sequenceNo.ToString().PadLeft(digitLength, '0');
                                    emp.Add(newId);
                                }
                                else
                                {
                                    emp.Add("0001"); // default
                                }
                            }
                            else
                            {
                                emp.Add("0001");
                            }
                        }

                        //  Year
                        if (code.Type == EmployeeIDAbbreviationType.Year)
                        {
                            emp.Add(date.Year.ToString());
                        }
                    }

                    // Join with "-"
                    empCode = string.Join("-", emp);
                }
            }

            return empCode;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
    //public class AttachmentUploadViewModel
    //{
    //    public int EmployeeId { get; set; }                   // Employee reference
    //    public AttachmentType ReferenceType { get; set; }     // e.g., PHOTO
    //    public string Remark { get; set; }                    // Optional remark
    //    public HttpPostedFileBase File { get; set; }          // Uploaded file
    //}
   
}
