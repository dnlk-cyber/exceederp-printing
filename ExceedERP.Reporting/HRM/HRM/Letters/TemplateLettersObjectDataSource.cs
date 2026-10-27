using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.Templates;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class TemplateLettersObjectDataSource :IDisposable
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        private readonly string cultureName = GlobalHelper.GetCurrentCulture();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExpLetter> GetEmployeeTemplateLetters(string id, bool header, bool includeExt, string language, string type,DateTime? from, DateTime?to, string templatetype,string headId)
        {
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(type)&& string.IsNullOrEmpty(templatetype))
            {
                return new List<ExpLetter>();
            }

            var expLetterList = new List<ExpLetter>();
            int.TryParse(templatetype, out int templateTypesettingId);
            var organization = GetOrganizationInfo(header,headId);
            var employeeInfo = GetEmployeeInfo(id, language, organization);
            var template = db.Templates.Where(x => x.TemplateTypeSettingId == templateTypesettingId).FirstOrDefault();
            if(template == null)
                template = db.Templates.Where(x => x.TemplateType.ToString().ToLower() == type.ToLower()).FirstOrDefault();
       
                
            if (employeeInfo != null)
            {
                employeeInfo.FromDate = from.HasValue ? from.Value.ToString("dd.MM.yyyy") : DateTime.Now.ToString("dd.MM.yyyy");
                employeeInfo.ToDate = to.HasValue ? to.Value.ToString("dd.MM.yyyy") : DateTime.Now.ToString("dd.MM.yyyy");
                if (template != null)
                {
                    expLetterList.Add(CreateEmployeeInfoLetter(employeeInfo, template, language));
                }
            }

            return expLetterList;
        }
        public List<ExpLetter> GetTemplateLetters(string empId, int id, bool header, bool includeExt, string language, string type,DateTime? from, DateTime?to, string templatetype,string headId)
        {
            if (string.IsNullOrEmpty(empId))
            {
                return new List<ExpLetter>();
            }

            var expLetterList = new List<ExpLetter>();
            int.TryParse(templatetype, out int templateTypesettingId);
            var organization = GetOrganizationInfo(header,headId);
            var employeeInfo = GetEmployeeInfo(empId, language, organization);
            var template = db.Templates.Where(x => x.TemplateId == id).FirstOrDefault();
            
            if (employeeInfo != null)
            {
                employeeInfo.FromDate = from.HasValue ? from.Value.ToString("dd.MM.yyyy") : DateTime.Now.ToString("dd.MM.yyyy");
                employeeInfo.ToDate = to.HasValue ? to.Value.ToString("dd.MM.yyyy") : DateTime.Now.ToString("dd.MM.yyyy");
                if (template != null)
                {
                    expLetterList.Add(CreateEmployeeInfoLetter(employeeInfo, template, language));
                }
            }

            return expLetterList;
        }
        private EmployeeExperienceLetter GetOrganizationInfo(bool hasHead,string employeeId)
        {
            var orgInfo = new EmployeeExperienceLetter();
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            int.TryParse(employeeId, out int empId);
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == empId);
            int head = empId > 0 ? empId : placement?.HeadId?? 0;


                var personEmployee = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId ==head);
                if (personEmployee != null)
                {
                    orgInfo.HeadName = personEmployee.FirstNameEnglish + " " + personEmployee.MiddleNameEnglish + " " + personEmployee.LastNameEnglish;
                    orgInfo.ApprovedBy = UserHelper.GetUserNameByEmployeeId(personEmployee.EmployeeId);
                    if (placement.HeadId.HasValue)
                    {
                        var headId = placement?.LookupId ?? 0;
                        var lookUp = db.Lookups.FirstOrDefault(x=>x.LookupId==headId);
                        if (lookUp != null)
                            orgInfo.HeadJobTitle = lookUp.Description ?? lookUp.DescriptionEng??string.Empty;
                    }
                }
                   
               
            

            if (organization != null&&hasHead)
            {
                orgInfo.Name = organization.TradeName;
                orgInfo.AmharicName = organization.AmharicName;
                orgInfo.Abbreviation = organization.Abbreviation;

                var addresses = db.Addresses.Where(a =>
                    a.Reference == organization.MainCompanyId.ToString()).ToList();

                foreach (var address in addresses)
                {
                    switch (address.Category)
                    {
                        case AddressType.KIFLEKETEMA:
                            orgInfo.Address = $"Subcity: {address.Value} ";
                            break;
                        case AddressType.KEBELE:
                            orgInfo.Address += $"Kebele: {address.Value} ";
                            break;
                        case AddressType.CITYPROVINCE:
                            orgInfo.Address = orgInfo.Address == null ? $"{address.Value} -Ethiopia" : orgInfo.Address;
                            break;
                        case AddressType.POBOX:
                            orgInfo.Pobox = $"P.O.Box: {address.Value}";
                            break;
                        case AddressType.OFFICE_EMAIL:
                            orgInfo.Email = $"Email: {address.Value}";
                            break;
                        case AddressType.WEBSITE:
                            orgInfo.WebSite = $"Website:{address.Value}";
                            break;
                        case AddressType.FAX:
                            orgInfo.Fax = $"Fax: {address.Value}";
                            break;
                        case AddressType.OFFICE_PHONE:
                            orgInfo.Tel = $"Tel: {address.Value}";
                            break;
                        case AddressType.REGION:
                            orgInfo.Region = $"{address.Value}";
                            break;
                        case AddressType.COUNTRY:
                            orgInfo.Country = $"{address.Value}";
                            break;
                    }
                }

                var logo = db.Attachments.FirstOrDefault(a =>
                    a.Reference == organization.MainCompanyId.ToString() &&
                    a.ReferenceType == AttachmentType.LOGO);

                if (logo != null)
                {
                    orgInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                    orgInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            return orgInfo;
        }
        private EmployeeExperienceLetter GetEmployeeInfo(string id, string language, EmployeeExperienceLetter orgInfo)
        {
            if (!int.TryParse(id, out int empId)) return null;

            var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
            if (personEmployee == null) return null;

            var experienceRequest = db.ExprianceRequests.FirstOrDefault(x =>
                x.EmployeeId == personEmployee.EmployeeId &&
                x.RequestType == SelfServiceRequestType.Expreience);
            //if (experienceRequest == null) return null;

            var employeeInfo = orgInfo ?? new EmployeeExperienceLetter();
            employeeInfo.IsTerminated = db.Terminations.Any(x =>
                x.EmployeeId == personEmployee.EmployeeId &&
                x.IsOnlineApproved);

            bool isLocalLanguage = language?.ToLower() == "local";
            bool isAmharic = cultureName.ToLower() == "am" || cultureName.ToLower()== "am-et";
            bool isOromo = cultureName.ToLower() == "om";

            // Set common properties
            employeeInfo.HimHer = personEmployee.Gender == Gender.FEMALE ? "her" : "him";
            employeeInfo.HisHer = personEmployee.Gender == Gender.FEMALE ? "her" : "his";
            employeeInfo.Date = isLocalLanguage
                ? experienceRequest?.ApplicationDateAm ?? DateTime.Now.ToString("dd.MM.yyyy")
                : experienceRequest?.ApplicationDate.ToString("dd.MM.yyyy") ?? DateTime.Now.ToString("dd.MM.yyyy");

      
                ConfigureEnglishExperience(employeeInfo, personEmployee, language);

            return employeeInfo;
        }       
        private void ConfigureEnglishExperience(EmployeeExperienceLetter employeeInfo, Person_Employee personEmployee,string language)
        {
            var cosign = db.Cosigns.Where(x => x.EmployeeId == personEmployee.EmployeeId).OrderByDescending(x => x.AgreementDate).FirstOrDefault();
            bool isLocalLanguage = language?.ToLower() == "local";
            bool isAmharic = cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et";
            bool isOromo = cultureName.ToLower() == "om";
           
            employeeInfo.CheckTermination = " monthly gross salary was ";
            employeeInfo.ApplicantName = personEmployee.FullNameaEnglish;
            employeeInfo.ApplicantNameAmharic = personEmployee.FullNameAmharic;
            if (isAmharic || isLocalLanguage)
            {
                employeeInfo.CheckTermination = isAmharic?" ጠቅላላ የወር ደሞዝ ":isOromo? "mindaa waliigalaa ji'aa": "monthly gross salary was";
            }
            var personTitle = db.Lookups.Find(personEmployee.Title);
            employeeInfo.ToName = $"{personTitle?.Description}{personEmployee.FirstNameEnglish} {personEmployee.MiddleNameEnglish} {personEmployee.LastNameEnglish}";
            employeeInfo.BirthDate = $"{personEmployee.DateOfBirthAm} ({personEmployee.DateOfBirth:MMMM dd, yyyy})";
            employeeInfo.HiredDate = personEmployee.HiredDate.ToString("dd.MM.yyyy"); ;
            employeeInfo.HiredDateAm = personEmployee.HiredDateAmharic;
            employeeInfo.BirthPlace = personEmployee.BirthPlace;
            employeeInfo.EmployeeCode =personEmployee.EmployeeCode;
            employeeInfo.EmploymentType = personEmployee.Status.ToString();
            
            employeeInfo.PersonalTitle = db.Lookups
                .FirstOrDefault(x => x.LookupType == LookUpTypes.PERSON_TITLE&&x.LookupId==personEmployee.Title)?.Description??"";
            employeeInfo.Gender = personEmployee.Gender.ToString();
            employeeInfo.MarriageStatus = personEmployee.MartialStatus.ToString();
            employeeInfo.City = personEmployee.Wereda;
            employeeInfo.KebeleSubDistrict = personEmployee.SubCity;
            employeeInfo.MarriageStatus = personEmployee.MartialStatus.ToString();
            employeeInfo.Region = personEmployee.Region.ToString();
            employeeInfo.TelephoneNo =personEmployee.PhoneNo;
            employeeInfo.HouseNo = personEmployee.MartialStatus.ToString();
            employeeInfo.DistrictWoreda = personEmployee.Wereda;
            var educationInfo = new StringBuilder();
            var allowanceInfo = new StringBuilder();
            var educations = db.Education.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();
            var employeeTransfer = db.EmployeeTransferRequests.Where(x => x.EmployeeId == personEmployee.EmployeeId).FirstOrDefault();
            if(employeeTransfer !=null)
            {
                employeeInfo.FromBranch = db.Branch.Find(employeeTransfer.FromBranchId)?.Name;
                employeeInfo.FromBusinessUnit = db.BusinessUnits.Find(employeeTransfer.FromBusinessUnitId??0)?.Name;
                employeeInfo.BusinessUnit = db.BusinessUnits.Find(employeeTransfer.BusinessUnitId??0)?.Name;
                employeeInfo.FromWorkUnit = db.MainCompanyStructures.Find(employeeTransfer.OrgStructureId)?.Name;
                //employeeInfo.FromJobTitle = db.Lookups.Find(employeeTransfer.lo??0)?.Description;
                employeeInfo.FromStep = db.Steps.Find(employeeTransfer.CurrentApprovalStep??0)?.StepName;

            }
            var PrePlacement = db.Placements.Where(x => !x.isCurrent && x.EmployeeId == personEmployee.EmployeeId)
                                                .OrderByDescending(x => x.PlacementId).FirstOrDefault();
            if (employeeTransfer == null)
            {
                
                if (PrePlacement != null)
                {
                    employeeInfo.FromBranch = db.Branch.Find(PrePlacement.BranchId)?.Name ?? null;
                    employeeInfo.FromBusinessUnit = db.BusinessUnits.Find(PrePlacement.BusinessUnitId)?.Name??null;
                    employeeInfo.BusinessUnit = db.BusinessUnits.Find(PrePlacement.BusinessUnitId)?.Name ?? null;
                    employeeInfo.FromWorkUnit = db.MainCompanyStructures.Find(PrePlacement.OrgStructureId)?.Name ?? null;
                    employeeInfo.FromJobTitle = db.Lookups.Find(PrePlacement.LookupId)?.Description ?? null;
                    employeeInfo.FromStep = db.Steps.Find(PrePlacement.StepId )?.StepName ?? null;
                    employeeInfo.FromGrade = db.Ranks.Find(PrePlacement.RankId )?.RankName ?? null;

                }

            }
            employeeInfo.FromSalary = PrePlacement?.Salary??0;

            var termination = db.Terminations.FirstOrDefault(x => x.EmployeeId == personEmployee.EmployeeId && x.IsOnlineApproved);
            if(termination !=null)
            {
                employeeInfo.TerminationReason = termination.Reason;
                employeeInfo.TerminationDate = $"{termination.TerminationDateAm} ({termination.TerminationDate:MMMM dd, yyyy})"; 
                employeeInfo.TerminationApplicationDate = $"{termination.ApplicationDateAm} ({termination.ApplicationDate:MMMM dd, yyyy})";
            }
            var desciplineCase = db.DisciplinaryCases.Where(x => x.EmployeeId == personEmployee.EmployeeId).OrderByDescending(x => x.DisciplinaryCaseId).FirstOrDefault();
            if(desciplineCase != null)
            {
                employeeInfo.DisciplineTaken = desciplineCase.Evidence;

            }
            foreach (var education in educations)
            {
                var level = db.Lookups.Find(education.EduLevel);
                var field = db.Lookups.Find(education.EduField);
                if (level != null && field != null)
                {
                    educationInfo.AppendLine($" {level.Description} in {field.Description}");
                }
            }

            var allowances = db.AllowancePerEmployees.Where(x => x.EmployeeId == personEmployee.EmployeeId&&!x.Pend).ToList();
            foreach (var allowance in allowances)
            {
                allowanceInfo.AppendLine($" {allowance.AllowanceDefination.AllowanceType.ToString()} :-  {allowance.AllowanceDefination.Amount}");

                switch (allowance.AllowanceDefination.AllowanceType)
                {
                    case AllowanceType.TRANSPORT:
                        employeeInfo.TransportAllowance = allowance.AllowanceDefination.Amount;
                        break;

                    case AllowanceType.TELEPHONE:
                        employeeInfo.TelephoneAllowance = allowance.AllowanceDefination.Amount;
                        break;

                    case AllowanceType.HOME:
                        employeeInfo.HomeAllowance = allowance.AllowanceDefination.Amount;
                        break;

                    case AllowanceType.POSITION:
                        employeeInfo.PositionAllowance = allowance.AllowanceDefination.Amount;
                        break;

                    case AllowanceType.PROFESSIONAL:
                        employeeInfo.ProfessionalAllowance = allowance.AllowanceDefination.Amount;
                        break;

                    case AllowanceType.DESERT:
                        employeeInfo.DesertAllowance = allowance.AllowanceDefination.Amount;
                        break;

                    case AllowanceType.PROJECT:
                        employeeInfo.ProjectAllowance = allowance.AllowanceDefination.Amount;
                        break;

                    case AllowanceType.RESPONSIBILITY:
                        employeeInfo.ResponsibilityAllowance = allowance.AllowanceDefination.Amount;
                        break;
                }
            }

            employeeInfo.EducationInfo = educationInfo.ToString();
            employeeInfo.Allowance = allowanceInfo.ToString();
            employeeInfo.EmployeeAllowance = allowanceInfo.ToString();
            ConfigurePlacementsAndSalary(employeeInfo, personEmployee, isAmharic,isOromo);
        }
        private void ConfigurePlacementsAndSalary(EmployeeExperienceLetter employeeInfo, Person_Employee personEmployee, bool isAmharic,bool isAffanOromo)
        {
            var placements = db.Placements
                .Where(x => x.EmployeeId == personEmployee.EmployeeId)
                .OrderByDescending(x => x.AssignmentDate)
                .ToList();
            var delegation = db.Delegations.Where(x => x.FromEmployeeId == personEmployee.EmployeeId)
                .OrderByDescending(x=>x.DelegationId).FirstOrDefault();
            if(delegation != null)
            {
                employeeInfo.DelegationFromDate = delegation.FromDateTime.HasValue?delegation.FromDateTime.Value.ToString("dd.MM.yyyy"):null;
                employeeInfo.DelegationToDate = delegation.ToDateTime.HasValue?delegation.ToDateTime.Value.ToString("dd.MM.yyyy"):null;
                employeeInfo.DelegationFromDateAm = delegation.FromDateTimeAm;
                employeeInfo.DelegationToDateAm = delegation.ToDateTimeAm;

            }
            var contact = db.Contract.Where(x => x.EmployeeId == personEmployee.EmployeeId)
                .OrderByDescending(x=>x.ContractId).FirstOrDefault();
            if(contact != null)
            {
                employeeInfo.ContratStartDate = contact.From.ToString("dd.MM.yyyy");
                employeeInfo.ContratEndDate = contact.To.ToString("dd.MM.yyyy");
                employeeInfo.ContratStartDateAm = contact.FromContAm;
                employeeInfo.ContratEndDateAm = contact.ToContAm;
                var from = contact.From;
                var to = contact.To;

                // Total months difference
                int totalMonths = ((to.Year - from.Year) * 12) + (to.Month - from.Month);

                // If you want to count partial months when 'to day' is less than 'from day'
                if (to.Day < from.Day)
                {
                    totalMonths--;
                }

                employeeInfo.ContractMonth = totalMonths;

            }
            var probation = db.ProbationsSettings.FirstOrDefault();
            if(probation != null)
            {
                employeeInfo.ProbationMonth = Math.Round( (probation.ProbationDays / 30.0),2).ToString();

            }

            var currentPlacement = placements.FirstOrDefault(x => x.isCurrent) ?? placements.FirstOrDefault();

            if (currentPlacement != null)
            {
                employeeInfo.AssignmentDate = currentPlacement.AssignmentDate.ToString("dd.MM.yyyy");
                employeeInfo.AssignmentDateAm = currentPlacement.AssignmentDateAmharic;
                employeeInfo.Salary = currentPlacement.Salary;

                var branch = db.Branch.Find(currentPlacement.BranchId);
                if(branch != null)
                {
                    employeeInfo.Branch = branch.Name;

                }
                var rank = db.Ranks.Find(currentPlacement.RankId);
                if(rank != null)
                {
                    employeeInfo.Grade = rank.RankName;

                }
                var step = db.Steps.Find(currentPlacement.StepId);
                if(step != null)
                {
                    employeeInfo.Step = step.StepName;

                }
                var workUnit = db.MainCompanyStructures.Find(currentPlacement.OrgStructureId);
                if(workUnit != null)
                {
                    employeeInfo.WorkUnit = workUnit.Name;

                }
                var parentWorkUnit = db.MainCompanyStructures.FirstOrDefault(x=>x.OrgStructureId== currentPlacement.ParentOrgStructureId);
                if(parentWorkUnit != null)
                {
                    employeeInfo.ParentStructure = parentWorkUnit.Name;

                }
                var businessUnit = db.BusinessUnits.Find(currentPlacement.BusinessUnitId);
                if(businessUnit != null)
                {
                    employeeInfo.BusinessUnit = businessUnit.Name;

                }
                var jobtitle = db.Lookups.Find(currentPlacement.LookupId);
                if(jobtitle != null)
                {
                    employeeInfo.JobTitle = jobtitle.Description;
                    employeeInfo.JobTitleEnglish = jobtitle.DescriptionEng;

                }

                employeeInfo.CurrentSalary = currentPlacement.Salary;
                employeeInfo.SalaryInWordAffanOromo = Convert.ToInt32(employeeInfo.CurrentSalary).ToWordAfaanOromo();
                employeeInfo.SalaryInWordEnglish = XAPI.NumberExtensions.toWords(Convert.ToInt32(employeeInfo.CurrentSalary));
                employeeInfo.SalaryInWordAmharic = Convert.ToInt32(employeeInfo.CurrentSalary).ToWordAmharic();
                employeeInfo.SalaryInWord = isAmharic
                    ? Convert.ToInt32(employeeInfo.CurrentSalary).ToWordAmharic():
                   isAffanOromo? Convert.ToInt32(employeeInfo.CurrentSalary).ToWordAfaanOromo() 
                   : XAPI.NumberExtensions.toWords(Convert.ToInt32(employeeInfo.CurrentSalary));
            }

            var allowanceBuilder = new StringBuilder();
            var employeeAllowances = db.AllowancePerEmployees
                .Where(x => x.EmployeeId == personEmployee.EmployeeId)
                .ToList();

            foreach (var allowance in employeeAllowances)
            {
                var allowanceDef = db.AllowanceDefinations.Find(allowance.AllowanceDefinationId);
                if (allowanceDef != null)
                {
                    allowanceBuilder.AppendLine(isAmharic
                        ? $"{allowanceDef.AllowanceType}-{allowanceDef.Amount}{allowanceDef.AmtPercent},"
                        : $"{allowanceDef.AllowanceName}-{allowanceDef.AllowanceType}-{allowanceDef.Amount} {allowanceDef.AmtPercent},");
                }
            }
            employeeInfo.Allowance = allowanceBuilder.ToString();
            employeeInfo.Expriance = GetEmployeeExperiences(placements, isAmharic, isAffanOromo);
            employeeInfo.OtherExpriance = GetOtherExperiences(personEmployee.EmployeeId, isAmharic, isAffanOromo);
        }
        private List<CurrentEmployeeExpriance> GetEmployeeExperiences(List<Placement> placements, bool isAmharic, bool isAffanOromo)
        {
            var experiences = new List<CurrentEmployeeExpriance>();

            for (int i = 0; i < placements.Count; i++)
            {
                var lookupId = placements[i].LookupId;
                var lookup = db.Lookups.FirstOrDefault(x => x.LookupId == lookupId);

                var experience = new CurrentEmployeeExpriance
                {
                    JobTitleName = lookup?.Description ?? placements[i].JobTitleName,
                    JobTitleNameEnglish = lookup?.DescriptionEng??placements[i].JobTitleName,
                    Salary = placements[i].Salary
                };
                var branchId = placements[i].BranchId;
                var branch = db.Branch.FirstOrDefault(x => x.BranchId == branchId);
                experience.Branch = isAmharic ? branch?.AmharicName : branch?.Name;
                experience.RecruitmentType = placements[i].EmployementType.ToString().ToLower();

                DateTime fromDate = placements[i].AssignmentDate;
                DateTime toDate = (i >= 1) ? placements[i - 1].AssignmentDate : DateTime.Now.Date;

                var difference = GetYearMonthDays(fromDate, toDate).FirstOrDefault();
                experience.From = isAmharic
                    ? (i >= 1 ? $"{placements[i].AssignmentDateAmharic}E.c ({placements[i].AssignmentDate:MMMM dd, yyyy}G.c)"
                              : placements[i].AssignmentDateAmharic)
                    : placements[i].AssignmentDate.ToShortDateString();

                experience.To = isAmharic
                    ? (i >= 1 ? $"{placements[i - 1].AssignmentDateAmharic}E.c({placements[i - 1].AssignmentDate:MMMM dd, yyyy}G.c)"
                              : " አሁን")
                    : (i >= 1 ? placements[i - 1].AssignmentDate.ToString("dd.MM.yyyy") : " Now");

                experience.Year = FormatYearMonthDay(difference, isAmharic, isAffanOromo);
                experiences.Add(experience);
            }

            // Add placement history
            var employeeId = placements[0].EmployeeId;
            var history = db.PlacementHistories.Where(x => x.EmployeeId == employeeId).ToList();
            experiences.AddRange(history.Select(h => new CurrentEmployeeExpriance
            {
                From = h.From.ToString("dd.MM.yyyy"),
                To = h.To.ToString("dd.MM.yyyy"),
                JobTitleName = db.Lookups.FirstOrDefault(x => x.LookupId == h.LookupId)?.Description?? h.JobTitle,
                JobTitleNameEnglish = db.Lookups.FirstOrDefault(x => x.LookupId == h.LookupId)?.DescriptionEng ?? h.JobTitle,
                Salary = h.Salary,
                RecruitmentType=h.EmployementType.ToString(),
                }));

            return experiences;
        }
        private List<OtherEmployeeExpriance> GetOtherExperiences(int employeeId, bool isAmharic, bool isOromo)
        {
            return db.Experiences.AsNoTracking().ToList()
                .Where(x => x.EmployeeId == employeeId)
                .Select(e => new OtherEmployeeExpriance
                {
                    From = e.From.ToString("dd.MM.yyyy"),
                    To = e.To.ToString("dd.MM.yyyy"),
                    CompanyName = e.Organozation,
                    JobTitleName = db.Lookups.FirstOrDefault(x => x.LookupId == e.LookupId)?.Description ?? e.Responsibility,
                    JobTitleNameEnglish = db.Lookups.FirstOrDefault(x => x.LookupId == e.LookupId)?.DescriptionEng ?? e.Responsibility,
                    Year = FormatYearMonthDay(GetYearMonthDays(e.From, e.To).First(), isAmharic,isOromo),
                    Salary = e.Salary
                }).ToList();
        }
        private string FormatYearMonthDay(YearMonthDay difference, bool isAmharic, bool isAffanOromo)
        {
            if (isAmharic)
            {
                return $"{difference.Years} አመት {difference.Months} ወር {difference.Days} ቀን";
            }
            else if (isAffanOromo)
            {
                return $"{difference.Years} Waggaa {difference.Months} Ji’a {difference.Days} Guyyaa";
            }
            else
            {
                return $"{difference.Years} Year {difference.Months} Month {difference.Days} Day";
            }
        }
        private ExpLetter CreateEmployeeInfoLetter(EmployeeExperienceLetter employeeInfo, Template template, string language)
        {
            var letter = new ExpLetter
            {
                Abbreviation = employeeInfo.Abbreviation,
                Address = employeeInfo.Address,
                AmharicName = employeeInfo.AmharicName,
                HeadName = employeeInfo.HeadName,
                HeadJobTitle = employeeInfo.HeadJobTitle,
                Signature = SignatureHelper.GetUserSignature(employeeInfo.ApprovedBy),
                SignatureFile = UserHelper.GetUserSignature(employeeInfo.ApprovedBy),
                Email = employeeInfo.Email,
                EnglishName = employeeInfo.EnglishName,
                Fax = employeeInfo.Fax,
                LogoFileName = employeeInfo.LogoFileName,
                Name = employeeInfo.Name,
                Pobox = employeeInfo.Pobox,
                Tel = employeeInfo.Tel,
                WebSite = employeeInfo.WebSite
            };

            bool isAmharic = cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et";
            bool isOromo = cultureName.ToLower() == "om";
            string doc = template?.Document ?? string.Empty;

            // 1. Decode HTML entities from the editor
            doc = HttpUtility.HtmlDecode(doc);

            // 2. Build Clean Formatted Experience Lines (Telerik HtmlTextBox compatible)
            var experienceBuilder = new StringBuilder();
            var experienceEnglishBuilder = new StringBuilder();
            int rowIdx = 1;
            int rowIdxEng = 1;

            if (employeeInfo.Expriance != null && employeeInfo.Expriance.Any())
            {
                foreach (var exp in employeeInfo.Expriance)
                {
                    string titleAm = !string.IsNullOrWhiteSpace(exp.JobTitleName) ? exp.JobTitleName : exp.JobTitleNameEnglish;
                    string titleEn = !string.IsNullOrWhiteSpace(exp.JobTitleNameEnglish) ? exp.JobTitleNameEnglish : exp.JobTitleName;
                    string branch = !string.IsNullOrWhiteSpace(exp.Branch) ? exp.Branch : "";
                    string recruitmentType = !string.IsNullOrWhiteSpace(exp.RecruitmentType) ? exp.RecruitmentType : "";

                    string line = isAmharic
                        ? $"&#160;&#160;&#160;&#160;<b>{rowIdx++}. {titleAm}</b> ከ <b>{exp.From}</b> እስከ <b>{exp.To}</b> በ {branch} {recruitmentType}"
                        : isOromo
                        ? $"&#160;&#160;&#160;&#160;<b>{rowIdx++}. {titleAm}</b> irraa <b>{exp.From}</b> gara <b>{exp.To}</b> itti {branch} {recruitmentType}"
                        : $"&#160;&#160;&#160;&#160;<b>{rowIdx++}. {titleEn}</b> from <b>{exp.From}</b> to <b>{exp.To}</b> at {branch} {recruitmentType}";

                    experienceBuilder.Append(line + "<br/>");
                    experienceEnglishBuilder.Append($"&#160;&#160;&#160;&#160;<b>{rowIdxEng++}. {titleEn}</b> from <b>{exp.From}</b> to <b>{exp.To}</b> at {branch} {recruitmentType.ToLower()}<br/>");
                }
            }

            if (employeeInfo.OtherExpriance != null && employeeInfo.OtherExpriance.Any())
            {
                foreach (var otherExp in employeeInfo.OtherExpriance)
                {
                    string titleAm = !string.IsNullOrWhiteSpace(otherExp.JobTitleName) ? otherExp.JobTitleName : otherExp.JobTitleNameEnglish;
                    string titleEn = !string.IsNullOrWhiteSpace(otherExp.JobTitleNameEnglish) ? otherExp.JobTitleNameEnglish : otherExp.JobTitleName;
                    string company = !string.IsNullOrWhiteSpace(otherExp.CompanyName) ? otherExp.CompanyName : "";

                    string line = isAmharic
                        ? $"&#160;&#160;&#160;&#160;<b>{rowIdx++}. {titleAm}</b> ከ <b>{otherExp.From}</b> እስከ <b>{otherExp.To}</b> በ {company} (ውጭ/External)"
                        : isOromo
                        ? $"&#160;&#160;&#160;&#160;<b>{rowIdx++}. {titleAm}</b> irraa <b>{otherExp.From}</b> gara <b>{otherExp.To}</b> itti {company} (External)"
                        : $"&#160;&#160;&#160;&#160;<b>{rowIdx++}. {titleEn}</b> from <b>{otherExp.From}</b> to <b>{otherExp.To}</b> at {company} (External)";

                    experienceBuilder.Append(line + "<br/>");
                    experienceEnglishBuilder.Append($"&#160;&#160;&#160;&#160;<b>{rowIdxEng++}. {titleEn}</b> from <b>{otherExp.From}</b> to <b>{otherExp.To}</b> at {company} (External)<br/>");
                }
            }

            if (rowIdx == 1)
            {
                string noExp = isAmharic ? "&#160;&#160;&#160;&#160;ምንም የስራ ልምድ አልተመዘገበም<br/>" : "&#160;&#160;&#160;&#160;No experience record found<br/>";
                experienceBuilder.Append(noExp);
                experienceEnglishBuilder.Append("&#160;&#160;&#160;&#160;No experience record found<br/>");
            }

            // 3. Termination Text Replacement
            if (employeeInfo.IsTerminated)
            {
                doc = doc.Replace("-IsTerminated-", isAmharic ? "ይከፈላቸው የነበረው"
                                                : isOromo ? "Inni/isheen kaffaltiin itti kaffalame"
                                                : "He/she was paid");

                doc = doc.Replace("-IfTerminated-", isAmharic ? "<br/><br/><b>ማሳሰቢያ፡</b> ይህ የስራ ልምድ እንደ ስራ መልቀቂያ አያገለግልም፡፡"
                                                           : isOromo ? "<br/><br/><b>Hubachiisa:</b> Muuxannoon hojii kun akka sababa aangoo gadhiisuuf hin tajaajilu."
                                                           : "<br/><br/><b>Notice:</b> This work experience does not serve as a reason for resignation.");
            }
            else
            {
                doc = doc.Replace("-IsTerminated-", isAmharic ? "አሁን እየተከፈላቸው ያለ"
                                                : isOromo ? "yeroo ammaa kafalamaa jira"
                                                : "is currently being paid");
                doc = doc.Replace("-IfTerminated-", "");
            }

            // 4. Formatted Salary Strings
            string salaryDisplay = $"{employeeInfo.CurrentSalary:N2} ({employeeInfo.SalaryInWord})";

            // 5. Replace Every Single Configured Key
            doc = doc.Replace("-ApplicantName-", employeeInfo.ApplicantName ?? "")
                     .Replace("-ApplicantNameAmharic-", employeeInfo.ApplicantNameAmharic ?? "")
                     .Replace("-BirthDate-", employeeInfo.BirthDate ?? "")
                     .Replace("-AssignmentDate-", employeeInfo.AssignmentDate ?? "")
                     .Replace("-AssignmentDateAm-", employeeInfo.AssignmentDateAm ?? "")
                     .Replace("-HiredDate-", employeeInfo.HiredDate ?? "")
                     .Replace("-HiredDateAm-", employeeInfo.HiredDateAm ?? "")
                     .Replace("-HireDate-", employeeInfo.HiredDate ?? "")
                     .Replace("-EducationInfo-", employeeInfo.EducationInfo ?? "")
                     .Replace("-Education-", employeeInfo.EducationInfo ?? "")
                     .Replace("-EducationLevel-", employeeInfo.EducationLevel ?? "")
                     .Replace("-Allowance-", employeeInfo.Allowance ?? "")
                     .Replace("-EmployeeAllowance-", employeeInfo.EmployeeAllowance ?? "")
                     .Replace("-ApplicationDate-", employeeInfo.Date ?? "")
                     .Replace("-HimHer-", employeeInfo.HimHer ?? "")
                     .Replace("-HisHer-", employeeInfo.HisHer ?? "")
                     .Replace("-CurrentJobTitle-", employeeInfo.JobTitle ?? "")
                     .Replace("-JobTitleEnglish-", employeeInfo.JobTitleEnglish ?? "")
                     .Replace("-JobTitle-", employeeInfo.JobTitle ?? "")
                     .Replace("-PreviousJobTitle-", employeeInfo.FromJobTitle ?? "")
                     .Replace("-FromJobTitle-", employeeInfo.FromJobTitle ?? "")
                     .Replace("-FromWorkUnit-", employeeInfo.FromWorkUnit ?? "")
                     .Replace("-Department-", employeeInfo.WorkUnit ?? "")
                     .Replace("-WorkUnit-", employeeInfo.WorkUnit ?? "")
                     .Replace("-ParentStructure-", employeeInfo.ParentStructure ?? "")
                     .Replace("-CurrentDepartment-", employeeInfo.WorkUnit ?? "")
                     .Replace("-CurrentDepartement-", employeeInfo.WorkUnit ?? "")
                     .Replace("-PreviousDepartement-", employeeInfo.FromWorkUnit ?? "")
                     .Replace("-BusinessUnit-", employeeInfo.BusinessUnit ?? "")
                     .Replace("-FromBusinessUnit-", employeeInfo.FromBusinessUnit ?? "")
                     .Replace("-FromBranch-", employeeInfo.FromBranch ?? "")
                     .Replace("-Branch-", employeeInfo.Branch ?? "")
                     .Replace("-FromStep-", employeeInfo.FromStep ?? "")
                     .Replace("-Step-", employeeInfo.Step ?? "")
                     .Replace("-FromGrade-", employeeInfo.FromGrade ?? "")
                     .Replace("-Grade-", employeeInfo.Grade ?? "")
                     .Replace("-FromRank-", employeeInfo.FromGrade ?? "")
                     .Replace("-Rank-", employeeInfo.Grade ?? "")
                     .Replace("-CurrentRank-", employeeInfo.Grade ?? "")
                     .Replace("-Salary-", salaryDisplay)
                     .Replace("-CurrentSalary-", employeeInfo.CurrentSalary.ToString("N2"))
                     .Replace("-CurrentAmount-", employeeInfo.CurrentSalary.ToString("N2"))
                     .Replace("-FromSalary-", employeeInfo.FromSalary.ToString("N2"))
                     .Replace("-Diffrence-", (employeeInfo.CurrentSalary - employeeInfo.FromSalary).ToString("N2"))
                     .Replace("-DiffrenceAmount-", (employeeInfo.CurrentSalary - employeeInfo.FromSalary).ToString("N2"))
                     .Replace("-SalaryInWord-", employeeInfo.SalaryInWord ?? "")
                     .Replace("-SalaryInWordEnglish-", employeeInfo.SalaryInWordEnglish ?? "")
                     .Replace("-SalaryInWordAmharic-", employeeInfo.SalaryInWordAmharic ?? "")
                     .Replace("-SalaryInWordAffanOromo-", employeeInfo.SalaryInWordAffanOromo ?? "")
                     .Replace("-TotalExpriance-", employeeInfo.TotalExpriance ?? "")
                     .Replace("-ToName-", employeeInfo.ToName ?? "")
                     .Replace("-FromDate-", employeeInfo.FromDate ?? "")
                     .Replace("-ToDate-", employeeInfo.ToDate ?? "")
                     .Replace("-EmploymentType-", employeeInfo.EmploymentType ?? "")
                     .Replace("-PersonalTitle-", employeeInfo.PersonalTitle ?? "")
                     .Replace("-DisciplineTaken-", employeeInfo.DisciplineTaken ?? "")
                     .Replace("-TerminationDate-", employeeInfo.TerminationDate ?? "")
                     .Replace("-TerminationReason-", employeeInfo.TerminationReason ?? "")
                     .Replace("-EmployeeCode-", employeeInfo.EmployeeCode ?? "")
                     .Replace("-TerminationApplicationDate-", employeeInfo.TerminationApplicationDate ?? "")
                     .Replace("-MarriageStatus-", employeeInfo.MarriageStatus ?? "")
                     .Replace("-Age-", employeeInfo.Age ?? "")
                     .Replace("-Sex-", employeeInfo.Gender ?? "")
                     .Replace("-HouseNo-", employeeInfo.HouseNo ?? "")
                     .Replace("-TelephoneNo-", employeeInfo.TelephoneNo ?? "")
                     .Replace("-Region-", employeeInfo.Region ?? "")
                     .Replace("-DistrictWoreda-", employeeInfo.DistrictWoreda ?? "")
                     .Replace("-KebeleSubDistrict-", employeeInfo.KebeleSubDistrict ?? "")
                     .Replace("-POBoxNo-", employeeInfo.POBoxNo ?? "")
                     .Replace("-BirthPlace-", employeeInfo.BirthPlace ?? "")
                     .Replace("-EmergencyContactPerson-", employeeInfo.EmergencyContactPerson ?? "")
                     .Replace("-LanguageProficiency-", employeeInfo.LanguageProficiency ?? "")
                     .Replace("-ProbationMonth-", employeeInfo.ProbationMonth ?? "")
                     .Replace("-ContratMonth-", employeeInfo.ContractMonth.ToString())
                     .Replace("-ContratStartDate-", employeeInfo.ContratStartDate ?? "")
                     .Replace("-ContratEndDate-", employeeInfo.ContratEndDate ?? "")
                     .Replace("-ContratStartDateAm-", employeeInfo.ContratStartDateAm ?? "")
                     .Replace("-ContratEndDateAm-", employeeInfo.ContratEndDateAm ?? "")
                     .Replace("-DelegationFromDate-", employeeInfo.DelegationFromDate ?? "")
                     .Replace("-DelegationToDate-", employeeInfo.DelegationToDate ?? "")
                     .Replace("-DelegationFromDateAm-", employeeInfo.DelegationFromDateAm ?? "")
                     .Replace("-DelegationToDateAm-", employeeInfo.DelegationToDateAm ?? "")
                     .Replace("-TransportAllowance-", employeeInfo.TransportAllowance.ToString("N2"))
                     .Replace("-TelephoneAllowance-", employeeInfo.TelephoneAllowance.ToString("N2"))
                     .Replace("-HomeAllowance-", employeeInfo.HomeAllowance.ToString("N2"))
                     .Replace("-PositionAllowance-", employeeInfo.PositionAllowance.ToString("N2"))
                     .Replace("-ProfessionalAllowance-", employeeInfo.ProfessionalAllowance.ToString("N2"))
                     .Replace("-DesertAllowance-", employeeInfo.DesertAllowance.ToString("N2"))
                     .Replace("-ProjectAllowance-", employeeInfo.ProjectAllowance.ToString("N2"))
                     .Replace("-ResponsibilityAllowance-", employeeInfo.ResponsibilityAllowance.ToString("N2"));

            // 6. Inject Experiences (with regex to capture spaces or variations)
            doc = Regex.Replace(doc, @"-\s*ExperiencesEnglish\s*-", experienceEnglishBuilder.ToString(), RegexOptions.IgnoreCase);
            doc = Regex.Replace(doc, @"-\s*Experiences\s*-", experienceBuilder.ToString(), RegexOptions.IgnoreCase);

            // 7. Clean up HTML Tags for Telerik HtmlTextBox compatibility
            doc = Regex.Replace(doc, @"<p[^>]*>", "", RegexOptions.IgnoreCase);
            doc = Regex.Replace(doc, @"</p>", "<br/><br/>", RegexOptions.IgnoreCase);
            doc = Regex.Replace(doc, @"<div[^>]*>", "", RegexOptions.IgnoreCase);
            doc = Regex.Replace(doc, @"</div>", "<br/>", RegexOptions.IgnoreCase);
            doc = Regex.Replace(doc, @"<span[^>]*>", "", RegexOptions.IgnoreCase);
            doc = Regex.Replace(doc, @"</span>", "", RegexOptions.IgnoreCase);

            // Convert strong/em to standard b/i
            doc = Regex.Replace(doc, @"<(/?)strong[^>]*>", "<$1b>", RegexOptions.IgnoreCase);
            doc = Regex.Replace(doc, @"<(/?)em[^>]*>", "<$1i>", RegexOptions.IgnoreCase);

            // Strip unrenderable table tags if user pasted any
            doc = Regex.Replace(doc, @"</?(table|tbody|thead|tfoot|tr|td|th)[^>]*>", "", RegexOptions.IgnoreCase);

            // XML safe entity formatting
            doc = doc.Replace("&nbsp;", "&#160;");
            doc = Regex.Replace(doc, @"&(?!amp;|lt;|gt;|quot;|apos;|#\d+;|#x[0-9a-fA-F]+;)", "&amp;");

            // Clean duplicate breaks
            doc = Regex.Replace(doc, @"(<br\s*/?>\s*){3,}", "<br/><br/>", RegexOptions.IgnoreCase);

            letter.Title = template?.Title ?? (isAmharic ? "ለሚመለከተው ሁሉ" : "To Whom It May Concern");
            letter.Experience = doc.Trim();

            return letter;
        }
        //private ExpLetter CreateEmployeeInfoLetter(EmployeeExperienceLetter employeeInfo, Template template, string language)
        //{
        //    var letter = new ExpLetter
        //    {
        //        // Header information
        //        Abbreviation = employeeInfo.Abbreviation,
        //        Address = employeeInfo.Address,
        //        AmharicName = employeeInfo.AmharicName,
        //        HeadName = employeeInfo.HeadName,
        //        HeadJobTitle = employeeInfo.HeadJobTitle,
        //        Signature = SignatureHelper.GetUserSignature(employeeInfo.ApprovedBy),
        //        SignatureFile = UserHelper.GetUserSignature(employeeInfo.ApprovedBy),
        //        Email = employeeInfo.Email,
        //        EnglishName = employeeInfo.EnglishName,
        //        Fax = employeeInfo.Fax,
        //        LogoFileName = employeeInfo.LogoFileName,
        //        Name = employeeInfo.Name,
        //        Pobox = employeeInfo.Pobox,
        //        Tel = employeeInfo.Tel,
        //        WebSite = employeeInfo.WebSite
        //    };

        //    bool isLocal = language?.ToLower() == "local";
        //    bool isAmharic = cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et";
        //    bool isOromo = cultureName.ToLower() == "om";
        //    string doc = template?.Document ?? string.Empty;

        //    // 1. Process ordered lists (<ol>) with numbers
        //    doc = Regex.Replace(doc, @"<ol[^>]*>(.*?)</ol>", match =>
        //    {
        //        string listContent = match.Groups[1].Value;
        //        int counter = 1;
        //        return Regex.Replace(listContent, @"<li[^>]*>(.*?)</li>", m => $"{counter++}. {m.Groups[1].Value}<br/>", RegexOptions.Singleline);
        //    }, RegexOptions.Singleline | RegexOptions.IgnoreCase);

        //    // 2. Process unordered lists (<ul>) with bullets
        //    doc = Regex.Replace(doc, @"<ul[^>]*>(.*?)</ul>", match =>
        //    {
        //        string listContent = match.Groups[1].Value;
        //        return Regex.Replace(listContent, @"<li[^>]*>(.*?)</li>", m => $"• {m.Groups[1].Value}<br/>", RegexOptions.Singleline);
        //    }, RegexOptions.Singleline | RegexOptions.IgnoreCase);

        //    // 3. Normalize semantic formatting tags to those supported by HtmlTextBox
        //    doc = Regex.Replace(doc, @"<(/?)strong[^>]*>", "<$1b>", RegexOptions.IgnoreCase);
        //    doc = Regex.Replace(doc, @"<(/?)em[^>]*>", "<$1i>", RegexOptions.IgnoreCase);

        //    // 4. Convert table structures using standard XML space entity &#160; (NEVER use &nbsp;)
        //    doc = Regex.Replace(doc, @"<tr[^>]*>", "", RegexOptions.IgnoreCase);
        //    doc = Regex.Replace(doc, @"</tr>", "<br/>", RegexOptions.IgnoreCase);
        //    doc = Regex.Replace(doc, @"<t[dh][^>]*>", "", RegexOptions.IgnoreCase);
        //    doc = Regex.Replace(doc, @"</t[dh]>", "&#160;&#160;&#160;&#160;", RegexOptions.IgnoreCase);
        //    doc = Regex.Replace(doc, @"</?table[^>]*>", "<br/>", RegexOptions.IgnoreCase);

        //    // 5. Convert remaining structural tags to line breaks
        //    doc = Regex.Replace(doc, @"<br\s*/?>", "<br/>", RegexOptions.IgnoreCase);
        //    doc = Regex.Replace(doc, @"</?p\s*>", "<br/>", RegexOptions.IgnoreCase);
        //    doc = Regex.Replace(doc, @"</?div\s*>", "<br/>", RegexOptions.IgnoreCase);
        //    doc = Regex.Replace(doc, @"</?span[^>]*>", "", RegexOptions.IgnoreCase);
        //    doc = Regex.Replace(doc, @"</?(ul|ol)\s*>", "<br/>", RegexOptions.IgnoreCase);

        //    // 6. Strip any tag that is NOT supported by Telerik HtmlTextBox
        //    doc = Regex.Replace(
        //        doc,
        //        @"</?(?!b\b|i\b|u\b|br\b|font\b|a\b)[a-zA-Z][^>]*>",
        //        "",
        //        RegexOptions.IgnoreCase);

        //    // 7. Clean up multiple consecutive line breaks
        //    doc = Regex.Replace(doc, @"(<br/>\s*){3,}", "<br/><br/>", RegexOptions.IgnoreCase);
        //    doc = doc.Trim();

        //    // 8. Replace placeholders with employee data
        //    doc = doc.Replace("-ApplicantName-", employeeInfo.ApplicantName)
        //            .Replace("-ApplicantNameAmharic-", employeeInfo.ApplicantNameAmharic)
        //            .Replace("-BirthDate-", employeeInfo.BirthDate)
        //            .Replace("-AssignmentDate-", employeeInfo.AssignmentDate)
        //            .Replace("-AssignmentDateAm-", employeeInfo.AssignmentDateAm)
        //            .Replace("-HiredDate-", employeeInfo.HiredDate)
        //            .Replace("-HiredDateAm-", employeeInfo.HiredDateAm)
        //            .Replace("-HireDate-", employeeInfo.HiredDate)
        //            .Replace("-EducationInfo-", employeeInfo.EducationInfo)
        //            .Replace("-Education-", employeeInfo.EducationInfo)
        //            .Replace("-Allowance-", employeeInfo.Allowance)
        //            .Replace("-ApplicationDate-", employeeInfo.Date)
        //            .Replace("-HimHer-", employeeInfo.HimHer)
        //            .Replace("-HisHer-", employeeInfo.HisHer)
        //            .Replace("-CurrentJobTitle-", employeeInfo.JobTitle)
        //            .Replace("-JobTitleEnglish-", employeeInfo.JobTitleEnglish)
        //            .Replace("-FromWorkUnit-", employeeInfo.FromWorkUnit)
        //            .Replace("-Department-", employeeInfo.WorkUnit)
        //            .Replace("-WorkUnit-", employeeInfo.WorkUnit)
        //            .Replace("-ParentStructure-", employeeInfo.ParentStructure)
        //            .Replace("-CurrentDepartment-", employeeInfo.WorkUnit)
        //            .Replace("-CurrentDepartement-", employeeInfo.WorkUnit)
        //            .Replace("-PreviousDepartement-", employeeInfo.FromWorkUnit)
        //            .Replace("-BusinessUnit-", employeeInfo.BusinessUnit)
        //            .Replace("-FromBusinessUnit-", employeeInfo.FromBusinessUnit)
        //            .Replace("-JobTitle-", employeeInfo.JobTitle)
        //            .Replace("-PreviousJobTitle-", employeeInfo.FromJobTitle)
        //            .Replace("-FromJobTitle-", employeeInfo.FromJobTitle)
        //            .Replace("-FromStep-", employeeInfo.FromStep)
        //            .Replace("-Step-", employeeInfo.Step)
        //            .Replace("-FromGrade-", employeeInfo.FromGrade)
        //            .Replace("-Grade-", employeeInfo.Grade)
        //            .Replace("-FromRank-", employeeInfo.FromGrade)
        //            .Replace("-Rank-", employeeInfo.Grade)
        //            .Replace("-CurrentRank-", employeeInfo.Grade)
        //            .Replace("-CurrentSalary-", employeeInfo.CurrentSalary.ToString())
        //            .Replace("-SalaryInWord-", employeeInfo.SalaryInWord)
        //            .Replace("-SalaryInWordEnglish-", employeeInfo.SalaryInWordEnglish)
        //            .Replace("-SalaryInWordAmharic-", employeeInfo.SalaryInWordAmharic)
        //            .Replace("-SalaryInWordAffanOromo-", employeeInfo.SalaryInWordAffanOromo)
        //            .Replace("-Salary-", employeeInfo.CurrentSalary.ToString())
        //            .Replace("-CurrentAmount-", employeeInfo.CurrentSalary.ToString())
        //            .Replace("-Diffrence-", (employeeInfo.CurrentSalary - employeeInfo.FromSalary).ToString())
        //            .Replace("-DiffrenceAmount-", (employeeInfo.CurrentSalary - employeeInfo.FromSalary).ToString())
        //            .Replace("-EducationLevel-", employeeInfo.EducationLevel)
        //            .Replace("-TotalExpriance-", employeeInfo.TotalExpriance)
        //            .Replace("-FromBranch-", employeeInfo.FromBranch)
        //            .Replace("-Branch-", employeeInfo.Branch)
        //            .Replace("-FromSalary-", employeeInfo.FromSalary.ToString())
        //            .Replace("-ToName-", employeeInfo.ToName)
        //            .Replace("-FromDate-", employeeInfo.FromDate)
        //            .Replace("-ToDate-", employeeInfo.ToDate)
        //            .Replace("-EmploymentType-", employeeInfo.EmploymentType)
        //            .Replace("-EmployeeAllowance-", employeeInfo.EmployeeAllowance)
        //            .Replace("-PersonalTitle-", employeeInfo.PersonalTitle)
        //            .Replace("-DisciplineTaken-", employeeInfo.DisciplineTaken)
        //            .Replace("-TerminationDate-", employeeInfo.TerminationDate)
        //            .Replace("-TerminationReason-", employeeInfo.TerminationReason)
        //            .Replace("-EmployeeCode-", employeeInfo.EmployeeCode)
        //            .Replace("-TerminationApplicationDate-", employeeInfo.TerminationApplicationDate)
        //            .Replace("-MarriageStatus-", employeeInfo.MarriageStatus)
        //            .Replace("-Age-", employeeInfo.Age)
        //            .Replace("-Sex-", employeeInfo.Gender)
        //            .Replace("-HouseNo-", employeeInfo.HouseNo)
        //            .Replace("-TelephoneNo-", employeeInfo.TelephoneNo)
        //            .Replace("-Region-", employeeInfo.Region)
        //            .Replace("-DistrictWoreda-", employeeInfo.DistrictWoreda)
        //            .Replace("-KebeleSubDistrict-", employeeInfo.KebeleSubDistrict)
        //            .Replace("-POBoxNo-", employeeInfo.POBoxNo)
        //            .Replace("-BirthPlace-", employeeInfo.BirthPlace)
        //            .Replace("-EmergencyContactPerson-", employeeInfo.EmergencyContactPerson)
        //            .Replace("-LanguageProficiency-", employeeInfo.LanguageProficiency)
        //            .Replace("-ProbationMonth-", employeeInfo.ProbationMonth)
        //            .Replace("-ContratMonth-", employeeInfo.ContractMonth.ToString())
        //            .Replace("-ContratStartDate-", employeeInfo.ContratStartDate)
        //            .Replace("-ContratEndDate-", employeeInfo.ContratEndDate)
        //            .Replace("-ContratStartDateAm-", employeeInfo.ContratStartDateAm)
        //            .Replace("-ContratEndDateAm-", employeeInfo.ContratEndDateAm)
        //            .Replace("-DelegationFromDate-", employeeInfo.DelegationFromDate)
        //            .Replace("-DelegationToDate-", employeeInfo.DelegationToDate)
        //            .Replace("-DelegationFromDateAm-", employeeInfo.DelegationFromDateAm)
        //            .Replace("-DelegationToDateAm-", employeeInfo.DelegationToDateAm)
        //            .Replace("-TransportAllowance-", employeeInfo.TransportAllowance.ToString())
        //            .Replace("-TelephoneAllowance-", employeeInfo.TelephoneAllowance.ToString())
        //            .Replace("-HomeAllowance-", employeeInfo.HomeAllowance.ToString())
        //            .Replace("-PositionAllowance-", employeeInfo.PositionAllowance.ToString())
        //            .Replace("-ProfessionalAllowance-", employeeInfo.ProfessionalAllowance.ToString())
        //            .Replace("-DesertAllowance-", employeeInfo.DesertAllowance.ToString())
        //            .Replace("-ProjectAllowance-", employeeInfo.ProjectAllowance.ToString())
        //            .Replace("-ResponsibilityAllowance-", employeeInfo.ResponsibilityAllowance.ToString());

        //    // 9. Termination-specific text
        //    if (employeeInfo.IsTerminated)
        //    {
        //        doc = doc.Replace("-IsTerminated-", isAmharic ? "ይከፈላቸው የነበረው"
        //                                        : isOromo ? "Inni/isheen kaffaltiin itti kaffalame"
        //                                        : "He/she was paid");

        //        doc = doc.Replace("-IfTerminated-", isAmharic ? " ይህ የስራ ልምድ እንደ ስራ መልቀቂያ አያገለግልም፡፡ "
        //                                                   : isOromo ? "Muuxannoon hojii kun akka sababa aangoo gadhiisuuf hin tajaajilu."
        //                                                   : "This work experience does not serve as a reason for resignation.");
        //    }
        //    else
        //    {
        //        doc = doc.Replace("-IsTerminated-", isAmharic ? " አሁን እየተከፈላቸው ያለ "
        //                                        : isOromo ? "yeroo ammaa kafalamaa jira"
        //                                        : "is currently being paid");
        //        doc = doc.Replace("-IfTerminated-", "");
        //    }

        //    // 10. Experience lists
        //    var experienceBuilder = new StringBuilder();
        //    var experienceEnglishBuilder = new StringBuilder();

        //    foreach (var exp in employeeInfo.Expriance)
        //    {
        //        experienceBuilder.Append((isAmharic
        //            ? $" - {exp.JobTitleName} ከ {exp.From} እስከ {exp.To} በ {exp.Branch} {exp.RecruitmentType}"
        //            : isOromo ? $" - {exp.JobTitleName} irraa {exp.From} gara {exp.To} itti {exp.Branch} {exp.RecruitmentType}"
        //            : $" - {exp.JobTitleName} from {exp.From} to {exp.To} at {exp.Branch} {exp.RecruitmentType}") + "<br/>");

        //        experienceEnglishBuilder.Append($" - {exp.JobTitleNameEnglish} from {exp.From} to {exp.To} at {exp.Branch} {exp.RecruitmentType.ToLower()}<br/>");
        //    }

        //    foreach (var otherExp in employeeInfo.OtherExpriance)
        //    {
        //        experienceBuilder.Append((isAmharic
        //            ? $" - {otherExp.JobTitleName} ከ {otherExp.From} እስከ {otherExp.To} በ {otherExp.CompanyName}"
        //            : isOromo ? $" - {otherExp.JobTitleName} irraa {otherExp.From} gara {otherExp.To} itti {otherExp.CompanyName}"
        //            : $" - {otherExp.JobTitleName} from {otherExp.From} to {otherExp.To} at {otherExp.CompanyName}") + "<br/>");

        //        experienceEnglishBuilder.Append($" - {otherExp.JobTitleNameEnglish} from {otherExp.From} to {otherExp.To} at {otherExp.CompanyName}<br/>");
        //    }

        //    doc = doc.Replace("-Experiences-", experienceBuilder.ToString());
        //    doc = doc.Replace("-ExperiencesEnglish-", experienceEnglishBuilder.ToString());

        //    // 11. Salary formatting
        //    string salary = $"{employeeInfo.CurrentSalary} ({employeeInfo.SalaryInWord})";
        //    doc = doc.Replace("-Salary-", salary);

        //    // 12. XML-SAFE ENCODING (CRITICAL FIX FOR HtmlTextBox)
        //    // A. Replace any raw &nbsp; with &#160;
        //    doc = doc.Replace("&nbsp;", "&#160;");

        //    // B. Escape any lone '&' that isn't part of an existing valid XML entity
        //    doc = Regex.Replace(doc, @"&(?!amp;|lt;|gt;|quot;|apos;|#\d+;|#x[0-9a-fA-F]+;)", "&amp;");

        //    // Set letter properties
        //    letter.Title = template?.Title ??
        //        (isAmharic ? "ለሚመለከተው ሁሉ"
        //        : isOromo ? "KAN IRRATTI YAADACHUU DANDA'A"
        //        : "To Whom it may concern");

        //    letter.Experience = doc;

        //    return letter;
        //}

        public List<YearMonthDay> GetYearMonthDays(DateTime from, DateTime to)
        {
            double totalDays = (to - from).Days;
            return new List<YearMonthDay>
            {
                new YearMonthDay
                {
                    Years = Math.Truncate(totalDays / 365),
                    Months = Math.Truncate((totalDays % 365) / 30),
                    Days = Math.Truncate((totalDays % 365) % 30)

                }

            };
        }
        public void Dispose()
        {
            db?.Dispose();
        }
    }
}
