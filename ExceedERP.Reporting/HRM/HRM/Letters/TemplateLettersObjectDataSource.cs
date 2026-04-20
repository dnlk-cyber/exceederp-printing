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
using System.Text;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class TemplateLettersObjectDataSource : List<EmployeeExperienceLetter>, IDisposable
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        private readonly string cultureName = GlobalHelper.GetCurrentCulture();

        public void Dispose()
        {
            db?.Dispose();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExpLetter> GetEmployeeTemplateLetters(string id, bool header, bool includeExt, string language, string type,DateTime? from, DateTime?to)
        {
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(type))
            {
                return new List<ExpLetter>();
            }

            var expLetterList = new List<ExpLetter>();
            var organization = header ? GetOrganizationInfo() : null;
            var employeeInfo = GetEmployeeInfo(id, language, type, organization);

            if (employeeInfo != null)
            {
                employeeInfo.FromDate = from.HasValue ? from.Value.ToString("dd.MM.yyyy") : DateTime.Now.ToString("dd.MM.yyyy");
                employeeInfo.ToDate = to.HasValue ? to.Value.ToString("dd.MM.yyyy") : DateTime.Now.ToString("dd.MM.yyyy");
                var template = db.Templates.FirstOrDefault(x => x.TemplateType.ToString().ToLower() == type.ToLower());
                if (template != null)
                {
                    expLetterList.Add(CreateEmployeeInfoLetter(employeeInfo, template, language));
                }
            }

            return expLetterList;
        }

        private EmployeeExperienceLetter GetOrganizationInfo()
        {
            var orgInfo = new EmployeeExperienceLetter();
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                orgInfo.Name = organization.TradeName;
                orgInfo.AmharicName = organization.AmharicName;
                orgInfo.Abbreviation = organization.Abbreviation;

                var addresses = db.Addresses.Where(a =>
                    a.Reference == organization.MainCompanyId.ToString() &&
                    a.ReferenceType.Equals("company", StringComparison.OrdinalIgnoreCase)).ToList();

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
                    }
                }

                var logo = db.Attachments.FirstOrDefault(a =>
                    a.Reference == organization.MainCompanyId.ToString() &&
                    a.ReferenceType == AttachmentType.LOGO);

                if (logo != null)
                {
                    orgInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            return orgInfo;
        }

        private EmployeeExperienceLetter GetEmployeeInfo(string id, string language, string type, EmployeeExperienceLetter orgInfo)
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
            // Set language-specific properties
            if (isLocalLanguage && isAmharic)
            {
                ConfigureAmharicExperience(employeeInfo, personEmployee);
            }
            else
            {
                ConfigureEnglishExperience(employeeInfo, personEmployee);
            }

            return employeeInfo;
        }

        private void ConfigureAmharicExperience(EmployeeExperienceLetter employeeInfo, Person_Employee personEmployee)
        {
            employeeInfo.CheckTermination = " ጠቅላላ የወር ደሞዝ ";
            employeeInfo.ApplicantName = personEmployee.FullNameAmharic;
            employeeInfo.BirthDate = $"{personEmployee.DateOfBirthAm} ({personEmployee.DateOfBirth:MMMM dd, yyyy})";
            employeeInfo.HiredDate = $"{personEmployee.HiredDateAmharic} ({personEmployee.HiredDate:MMMM dd, yyyy})";

            var educationInfo = new StringBuilder();
            var educations = db.Education.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();

            foreach (var education in educations)
            {
                var level = db.Lookups.Find(education.EduLevel);
                var field = db.Lookups.Find(education.EduField);

                if (level != null && field != null)
                {
                    educationInfo.Append($" {level.Description} በ {field.Description},  ");
                }
            }

            employeeInfo.EducationInfo = educationInfo.ToString();
            ConfigurePlacementsAndSalary(employeeInfo, personEmployee, true);
        }

        private void ConfigureOromoExperience(EmployeeExperienceLetter employeeInfo, Person_Employee personEmployee)
        {
            // Similar to ConfigureAmharicExperience but with Oromo translations
            // Implementation omitted for brevity
        }

        private void ConfigureEnglishExperience(EmployeeExperienceLetter employeeInfo, Person_Employee personEmployee)
        {
            var cosign = db.Cosigns.Where(x => x.EmployeeId == personEmployee.EmployeeId).OrderByDescending(x => x.AgreementDate).FirstOrDefault();
            employeeInfo.CheckTermination = " monthly gross salary was ";
            employeeInfo.ApplicantName = personEmployee.FullNameaEnglish;
            var employee = db.Person_Employee.Find(personEmployee.EmployeeId);
            employeeInfo.ToName =$"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}";
            employeeInfo.BirthDate = $"{personEmployee.DateOfBirthAm} ({personEmployee.DateOfBirth:MMMM dd, yyyy})";
            employeeInfo.HiredDate = $"{personEmployee.HiredDateAmharic} ({personEmployee.HiredDate:MMMM dd, yyyy})";
            var educationInfo = new StringBuilder();
            var educations = db.Education.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();
            var employeeTransfers = db.EmployeeTransferRequests.Where(x => x.EmployeeId == personEmployee.EmployeeId).FirstOrDefault();

            foreach (var education in educations)
            {
                var level = db.Lookups.Find(education.EduLevel);
                var field = db.Lookups.Find(education.EduField);

                if (level != null && field != null)
                {
                    educationInfo.AppendLine($" {level.Description} in {field.Description}");
                }
            }

            employeeInfo.EducationInfo = educationInfo.ToString();
            ConfigurePlacementsAndSalary(employeeInfo, personEmployee, false);
        }

        private void ConfigurePlacementsAndSalary(EmployeeExperienceLetter employeeInfo, Person_Employee personEmployee, bool isAmharic)
        {
            var placements = db.Placements
                .Where(x => x.EmployeeId == personEmployee.EmployeeId)
                .OrderByDescending(x => x.AssignmentDate)
                .ToList();

            var currentPlacement = placements.FirstOrDefault(x => x.isCurrent) ?? placements.FirstOrDefault();

            if (currentPlacement != null)
            {
                var branch = db.Branch.Find(currentPlacement.BranchId);
                if(branch != null)
                {
                    employeeInfo.Branch = branch.Name;

                }
                var workUnit = db.MainCompanyStructures.Find(currentPlacement.OrgStructureId);
                if(workUnit != null)
                {
                    employeeInfo.WorkUnit = workUnit.Name;

                }
                var businessUnit = db.BusinessUnits.Find(currentPlacement.BusinessUnitId);
                if(businessUnit != null)
                {
                    employeeInfo.BusinessUnit = businessUnit.Name;

                }

                employeeInfo.CurrentSalary = currentPlacement.Salary;
                employeeInfo.SalaryInWord = isAmharic
                    ? Convert.ToInt32(employeeInfo.CurrentSalary).ToWordAmharic()
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
            employeeInfo.Expriance = GetEmployeeExperiences(placements, isAmharic);
            employeeInfo.OtherExpriance = GetOtherExperiences(personEmployee.EmployeeId, isAmharic);
        }

        private List<CurrentEmployeeExpriance> GetEmployeeExperiences(List<Placement> placements, bool isAmharic)
        {
            var experiences = new List<CurrentEmployeeExpriance>();

            for (int i = 0; i < placements.Count; i++)
            {
                var experience = new CurrentEmployeeExpriance
                {
                    JobTitleName = placements[i].JobTitleName,
                    Salary = placements[i].Salary
                };
                var branchId = placements[i].BranchId;
                var branch = db.Branch.FirstOrDefault(x => x.BranchId == branchId);
                experience.Branch = isAmharic ? branch?.AmharicName : branch?.Name;
                experience.RecruitmentType = placements[i].EmployementType.ToString();

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
                    : (i >= 1 ? placements[i - 1].AssignmentDate.ToShortDateString() : " Now");

                experience.Year = FormatYearMonthDay(difference, isAmharic);
                experiences.Add(experience);
            }

            // Add placement history
            var employeeId = placements[0].EmployeeId;
            var history = db.PlacementHistories.Where(x => x.EmployeeId == employeeId).ToList();
            experiences.AddRange(history.Select(h => new CurrentEmployeeExpriance
            {
                From = h.From.ToShortDateString(),
                To = h.To.ToShortDateString(),
                JobTitleName = h.JobTitle,
                Salary = h.Salary,
                RecruitmentType=h.EmployementType.ToString(),
               // Year = isAmharic ? XAPI.Common.GetYearMonthDay(h.From, h.To) : FormatYearMonthDay(GetYearMonthDays(h.From, h.To).First()
                }));

            return experiences;
        }

        private List<OtherEmployeeExpriance> GetOtherExperiences(int employeeId, bool isAmharic)
        {
            return db.Experiences.AsNoTracking().ToList()
                .Where(x => x.EmployeeId == employeeId)
                .Select(e => new OtherEmployeeExpriance
                {
                    From = e.From.Date.ToShortDateString(),
                    To = e.To.Date.ToShortDateString(),
                    CompanyName = e.Organozation,
                    JobTitleName = e.Responsibility,
                    Year = FormatYearMonthDay(GetYearMonthDays(e.From, e.To).First(), isAmharic),
                    Salary = e.Salary
                })
                .ToList();
        }

        private string FormatYearMonthDay(YearMonthDay difference, bool isAmharic)
        {
            return isAmharic
                ? $"{difference.Years} አመት {difference.Months} ወር {difference.Days} ቀን"
                : $"{difference.Years} Year {difference.Months} Month {difference.Days} Days";
        }

        private ExpLetter CreateEmployeeInfoLetter(EmployeeExperienceLetter employeeInfo, Template template, string language)
        {
            var letter = new ExpLetter
            {
                // Header information
                Abbreviation = employeeInfo.Abbreviation,
                Address = employeeInfo.Address,
                AmharicName = employeeInfo.AmharicName,
                Email = employeeInfo.Email,
                EnglishName = employeeInfo.EnglishName,
                Fax = employeeInfo.Fax,
                LogoFileName = employeeInfo.LogoFileName,
                Name = employeeInfo.Name,
                Pobox = employeeInfo.Pobox,
                Tel = employeeInfo.Tel,
                WebSite = employeeInfo.WebSite
            };

            bool isLocal = language?.ToLower() == "local";
            bool isAmharic = cultureName.ToLower() == "am" || cultureName.ToLower() =="am-et";
            bool isOromo = cultureName.ToLower() == "om";

            string doc = template.Document;
            doc = doc.Replace("-ApplicantName-", employeeInfo.ApplicantName)
                    .Replace("-BirthDate-", employeeInfo.BirthDate)
                    .Replace("-HiredDate-", employeeInfo.HiredDate)
                    .Replace("-HireDate-", employeeInfo.HiredDate)
                    .Replace("-EducationInfo-", employeeInfo.EducationInfo)
                    .Replace("-ApplicationDate-", employeeInfo.Date)
                    .Replace("-HimHer-", employeeInfo.HimHer)
                    .Replace("-HisHer-", employeeInfo.HisHer)
                    .Replace("-FromWorkUnit-", employeeInfo.FromWorkUnit)
                    .Replace("-WorkUnit-", employeeInfo.WorkUnit)
                    .Replace("-BusinessUnit-", employeeInfo.BusinessUnit)
                    .Replace("-FromBusinessUnit-", employeeInfo.FromBusinessUnit)
                    .Replace("-JobTitle-", employeeInfo.JobTitle)
                    .Replace("-FromJobTitle-", employeeInfo.FromJobTitle)
                    .Replace("-FromStep-", employeeInfo.FromStep)
                    .Replace("-Step-", employeeInfo.Step)
                    .Replace("-FromGrade-", employeeInfo.FromGrade)
                    .Replace("-Grade-", employeeInfo.Grade)
                    .Replace("-EducationLevel-", employeeInfo.EducationLevel)
                    .Replace("-TotalExpriance-", employeeInfo.TotalExpriance)
                    .Replace("-FromBranch-", employeeInfo.FromBranch)
                    .Replace("-Branch-", employeeInfo.FromBranch)
                    .Replace("-FromSalary-", employeeInfo.FromSalary)
                    .Replace("-ToName-", employeeInfo.ToName)
                    .Replace("-FromDate-", employeeInfo.FromDate)
                    .Replace("-ToDate-", employeeInfo.ToDate)
                    .Replace("-Branch-", employeeInfo.Branch);

            if (employeeInfo.IsTerminated)
            {
                doc = doc.Replace("-IsTerminated-", isAmharic ? "ይከፈላቸው የነበረው" 
                                                    :isOromo? "Inni/isheen kaffaltiin itti kaffalame" 
                                                    : "He/she was paid");

                doc = doc.Replace("-IfTerminated-", isAmharic ? " ይህ የስራ ልምድ እንደ ስራ መልቀቂያ አያገለግልም፡፡ "
                                                               :isOromo? "Muuxannoon hojii kun akka sababa aangoo gadhiisuuf hin tajaajilu."
                                                               : "This work experience does not serve as a reason for resignation.");
            }
            else
            {
                doc = doc.Replace("-IsTerminated-", isAmharic ? " አሁን እየተከፈላቸው ያለ "
                                                    :isOromo? "yeroo ammaa kafalamaa jira" 
                                                    : "is currently being paid");
                doc = doc.Replace("-IfTerminated-", "");
            }

            var experienceBuilder = new StringBuilder();
            var salaryBuilder = new StringBuilder();
            foreach (var exp in employeeInfo.Expriance)
            {
                experienceBuilder.AppendLine(isAmharic
                    ? $" - {exp.JobTitleName} ከ {exp.From} እስከ {exp.To} በ {exp.Branch} {exp.RecruitmentType}"
                    :isOromo? $" - {exp.JobTitleName} irraa {exp.From} gara {exp.To} itti {exp.Branch} {exp.RecruitmentType}"
                    : $" - {exp.JobTitleName} from {exp.From} to {exp.To} at {exp.Branch} {exp.RecruitmentType}");
            }

            foreach (var otherExp in employeeInfo.OtherExpriance)
            {
                experienceBuilder.AppendLine(isAmharic
                    ? $" - {otherExp.JobTitleName} ከ {otherExp.From} እስከ {otherExp.To} በ {otherExp.CompanyName}"
                    :isOromo? $" - {otherExp.JobTitleName} irraa {otherExp.From} gara {otherExp.To} itti {otherExp.CompanyName}"
                    : $" - {otherExp.JobTitleName} from {otherExp.From} to {otherExp.To} at {otherExp.CompanyName}");
            }

            doc = doc.Replace("-Experiences-", experienceBuilder.ToString());
            //string salary = isAmharic
            //    ? $"{employeeInfo.CurrentSalary} ({employeeInfo.SalaryInWord} ), አበል {employeeInfo.Allowance}"
            //    : $"{employeeInfo.CurrentSalary} /{employeeInfo.SalaryInWord} /, allowance of {employeeInfo.Allowance}";

            string salary = isAmharic
                ? $"{employeeInfo.CurrentSalary} ({employeeInfo.SalaryInWord})"
                : $"{employeeInfo.CurrentSalary} ({employeeInfo.SalaryInWord}) ";

            doc = doc.Replace("-Salary-", salary);
            letter.Title = template?.Title ??
                (isAmharic? "ለሚመለከተው ሁሉ"
                :isOromo? "KAN IRRATTI YAADACHUU DANDA'A"
                :"To Whom it may concern");

            letter.Experience = doc;

            return letter;
        }

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
    }
    //[DataObject]
    //public class TemplateLettersObjectDataSource : List<EmployeeExperienceLetter>
    //{
    //    private ExceedDbContext db = new ExceedDbContext();
    //    string cultureName = GlobalHelper.GetCurrentCulture();

    //    [DataObjectMethod(DataObjectMethodType.Select)]
    //    public List<ExpLetter> GetEmployeeExperience(string id, bool header, bool includeExt, string language,string type)
    //    {
    //        List<EmployeeExperienceLetter> EmployeeInfoList = new List<EmployeeExperienceLetter>();
    //        EmployeeExperienceLetter EmployeeInfo = new EmployeeExperienceLetter();
    //        List<ExpLetter> expLetterList = new List<ExpLetter>();
    //        var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");

    //        if (header)
    //        {
    //            if (Organization != null)
    //            {
    //                EmployeeInfo.Name = Organization.TradeName;
    //                EmployeeInfo.AmharicName = Organization.AmharicName;
    //                EmployeeInfo.Abbreviation = Organization.Abbreviation;
    //                var address =
    //                    db.Addresses.Where(a =>
    //                            a.Reference == Organization.MainCompanyId.ToString() &&
    //                            a.ReferenceType.ToLower() == "company").ToList();

    //                if (address.Any())
    //                {
    //                    foreach (var a in address)
    //                    {
    //                        if (a.Category == AddressType.KIFLEKETEMA)
    //                        {
    //                            EmployeeInfo.Address = "Subcity: " + a.Value + " ";
    //                        }
    //                        if (a.Category == AddressType.KEBELE)
    //                        {
    //                            EmployeeInfo.Address += "Kebele: " + a.Value + " ";
    //                        }
    //                        if (a.Category == AddressType.CITYPROVINCE)
    //                        {
    //                            if (EmployeeInfo.Address == null)
    //                            {
    //                                EmployeeInfo.Address += a.Value + " -Ethiopia";
    //                            }
    //                        }
    //                        else if (a.Category == AddressType.POBOX)
    //                        {

    //                            EmployeeInfo.Pobox = "P.O.Box: " + a.Value;

    //                        }
    //                        else if (a.Category == AddressType.OFFICE_EMAIL)
    //                        {

    //                            EmployeeInfo.Email = "Email: " + a.Value;

    //                        }
    //                        else if (a.Category == AddressType.WEBSITE)
    //                        {

    //                            EmployeeInfo.WebSite = "Website:" + a.Value;

    //                        }
    //                        else if (a.Category == AddressType.FAX)
    //                        {

    //                            EmployeeInfo.Fax = "Fax:  " + a.Value;

    //                        }
    //                        else if (a.Category == AddressType.OFFICE_PHONE)
    //                        {

    //                            EmployeeInfo.Tel = "Tel:  " + a.Value;

    //                        }

    //                    }

    //                }
    //                var logo =
    //                    db.Attachments.Where(
    //                        a =>
    //                            a.Reference == Organization.MainCompanyId.ToString() &&
    //                            a.ReferenceType == AttachmentType.LOGO).FirstOrDefault();
    //                if (logo != null)
    //                {
    //                    EmployeeInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
    //                }
    //            }
    //        }
    //        if (!string.IsNullOrEmpty(id)&& !string.IsNullOrEmpty(type))
    //        {
    //            int.TryParse(id, out int empId);

    //            var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
    //            if (personEmployee != null)
    //            {

    //                var experienceRequest = db.ExprianceRequests.FirstOrDefault(x => x.EmployeeId == personEmployee.EmployeeId && x.RequestType == SelfServiceRequestType.Expreience);
    //                if (experienceRequest != null)
    //                {
    //                    var termination = db.Terminations.FirstOrDefault(x => x.EmployeeId == personEmployee.EmployeeId && x.IsOnlineApproved);
    //                    if (termination != null)
    //                    {
    //                        EmployeeInfo.IsTerminated = true;
    //                    }
    //                    else
    //                    {
    //                        EmployeeInfo.IsTerminated = false;
    //                    }



    //                    if (language != null && language.ToLower() == "local")
    //                    {
    //                        EmployeeInfo.CheckTermination = " ጠቅላላ የወር ደሞዝ ";
    //                        EmployeeInfo.CheckCurrent = "";
    //                        EmployeeInfo.TerminationMassage = "";
    //                        EmployeeInfo.Date = experienceRequest.ApplicationDateAm;

    //                        if (personEmployee.Gender == Gender.FEMALE)
    //                        {
    //                            EmployeeInfo.HimHer = "her";
    //                            EmployeeInfo.HisHer = "her";
    //                        }
    //                        else
    //                        {
    //                            EmployeeInfo.HimHer = "him";
    //                            EmployeeInfo.HisHer = "his";
    //                        }

    //                        var placements = db.Placements.Where(x => x.EmployeeId == personEmployee.EmployeeId).OrderByDescending(x => x.AssignmentDate).ToList();

    //                        EmployeeInfo.ApplicantName = personEmployee.FullNameAmharic;
    //                        EmployeeInfo.BirthDate = personEmployee.DateOfBirthAm + " (" + personEmployee.DateOfBirth.ToString("MMMM dd, yyyy") + ")";
    //                        EmployeeInfo.HiredDate = personEmployee.HiredDateAmharic + " (" + personEmployee.HiredDate.ToString("MMMM dd, yyyy") + ")";
    //                        var educations = db.Education.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();
    //                        foreach(var education in educations)
    //                        {
    //                            var level = db.Lookups.Find(education.EduLevel);
    //                            var field = db.Lookups.Find(education.EduField);

    //                            if (level !=null&& field != null)
    //                            {
    //                                EmployeeInfo.EducationInfo += $" {level.Description} በ {field.Description},  ";
    //                            }
    //                        }
    //                        var currentPlacement = placements.FirstOrDefault(x => x.isCurrent);
    //                        if (currentPlacement != null)
    //                        {
    //                            EmployeeInfo.CurrentSalary = currentPlacement.Salary;
    //                            EmployeeInfo.SalaryInWord = Convert.ToInt32(EmployeeInfo.CurrentSalary).ToWordAmharic();
    //                        }
    //                        else
    //                        {
    //                            var lastPlacement = placements.FirstOrDefault();
    //                            if (lastPlacement != null)
    //                            {
    //                                EmployeeInfo.CurrentSalary = lastPlacement.Salary;
    //                                EmployeeInfo.SalaryInWord = Convert.ToInt32(EmployeeInfo.CurrentSalary).ToWordAmharic();
    //                            }
    //                        }
    //                        string allowanceList = "";
    //                        var employeeAllowances = db.AllowancePerEmployees.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();
    //                        foreach(var allowance in employeeAllowances)
    //                        {
    //                            var allownacevaName = db.AllowanceDefinations.Find(allowance.AllowanceDefinationId);
    //                            if(allownacevaName != null)
    //                            {
    //                                allowanceList += allownacevaName.AllowanceType.ToString() +"-"+allownacevaName.Amount+""+allownacevaName.AmtPercent.ToString()+",";
    //                            }

    //                        }
    //                        EmployeeInfo.Allowance = allowanceList;
    //                        EmployeeInfo.Expriance = new List<CurrentEmployeeExpriance>();

    //                        for (int e = 0; e < placements.Count(); e++)
    //                        {
    //                            var currentEmployeeExpriance = new CurrentEmployeeExpriance();

    //                            var difference = new YearMonthDay();

    //                            if (e >= 1)
    //                            {
    //                                currentEmployeeExpriance.From = placements[e].AssignmentDateAmharic + "E.c (" + placements[e].AssignmentDate.ToString("MMMM dd, yyyy") + "G.c)"; ;

    //                                currentEmployeeExpriance.To = placements[e - 1].AssignmentDateAmharic + "E.c(" + placements[e - 1].AssignmentDate.ToString("MMMM dd, yyyy") + "G.c)"; ;

    //                                var fromc = new DateTime(placements[e].AssignmentDate.Year, placements[e].AssignmentDate.Month, placements[e].AssignmentDate.Day);
    //                                var toc = new DateTime(placements[e - 1].AssignmentDate.Year, placements[e - 1].AssignmentDate.Month, placements[e - 1].AssignmentDate.Day);

    //                                difference = GetYearMonthDays(fromc, toc).FirstOrDefault();

    //                            }
    //                            else
    //                            {
    //                                currentEmployeeExpriance.From = placements[e].AssignmentDateAmharic;

    //                                currentEmployeeExpriance.To = " አሁን";
    //                                var fromc = new DateTime(placements[e].AssignmentDate.Year, placements[e].AssignmentDate.Month, placements[e].AssignmentDate.Day);
    //                                var toc = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day);


    //                                difference = GetYearMonthDays(fromc, toc).FirstOrDefault();
    //                            }

    //                            currentEmployeeExpriance.JobTitleName = placements[e].JobTitleName;

    //                            int branchId = placements[e].BranchId;
    //                            var branch = db.Branch.FirstOrDefault(x => x.BranchId == branchId);
    //                            if (branch != null)
    //                            {
    //                                currentEmployeeExpriance.Branch = branch.AmharicName;
    //                            }
    //                            currentEmployeeExpriance.RecruitmentType = placements[e].EmployementType.ToString();


    //                            currentEmployeeExpriance.Year = difference.Years.ToString() + " " + " አመት " +
    //                                                            difference.Months.ToString() + " " + " ወር " +
    //                                                            difference.Days.ToString() + " " + " ቀን ";

    //                            currentEmployeeExpriance.Salary = placements[e].Salary;
    //                            EmployeeInfo.Expriance.Add(currentEmployeeExpriance);
    //                        }

    //                        #region History

    //                        var empHistory = db.PlacementHistories.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();

    //                        foreach (var e in empHistory)
    //                        {
    //                            var currentEmployeeExpriance = new CurrentEmployeeExpriance();

    //                            currentEmployeeExpriance.From = e.From.ToShortDateString();
    //                            currentEmployeeExpriance.To = e.To.ToShortDateString();

    //                            currentEmployeeExpriance.JobTitleName = e.JobTitle;

    //                            var difference = GetYearMonthDays(e.From, e.To).FirstOrDefault();

    //                            currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);
    //                            currentEmployeeExpriance.RecruitmentType = e.EmployementType.ToString();

    //                            currentEmployeeExpriance.Salary = e.Salary;

    //                            EmployeeInfo.Expriance.Add(currentEmployeeExpriance);
    //                        }
    //                        #endregion

    //                        EmployeeInfo.OtherExpriance = new List<OtherEmployeeExpriance>();

    //                        var externalExperiences = db.Experiences.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();

    //                        foreach (var e in externalExperiences)
    //                        {
    //                            var otherexplist = new OtherEmployeeExpriance();

    //                            otherexplist.From = e.From.Date.ToShortDateString();
    //                            otherexplist.To = e.To.Date.ToShortDateString();
    //                            otherexplist.CompanyName = e.Organozation;
    //                            otherexplist.JobTitleName = e.Responsibility;

    //                            var difference = GetYearMonthDays(e.From, e.To).FirstOrDefault();

    //                            otherexplist.Year = difference.Years.ToString() + " " + " አመት " +
    //                                                difference.Months.ToString() + " ወር " +
    //                                                difference.Days.ToString() + " " + " ቀን ";

    //                            otherexplist.Salary = e.Salary;

    //                            EmployeeInfo.OtherExpriance.Add(otherexplist);
    //                        }

    //                        EmployeeInfoList.Add(EmployeeInfo);
    //                    }
    //                    else
    //                    {

    //                        EmployeeInfo.CheckTermination = " monthly gross salary was ";
    //                        EmployeeInfo.CheckCurrent = "";
    //                        EmployeeInfo.TerminationMassage = "";
    //                        EmployeeInfo.Date = experienceRequest.ApplicationDate.ToShortDateString();

    //                        if (personEmployee.Gender == Gender.FEMALE)
    //                        {
    //                            EmployeeInfo.HimHer = "her";
    //                            EmployeeInfo.HisHer = "her";
    //                        }
    //                        else
    //                        {
    //                            EmployeeInfo.HimHer = "him";
    //                            EmployeeInfo.HisHer = "his";
    //                        }

    //                        var placements = db.Placements.Where(x => x.EmployeeId == personEmployee.EmployeeId).OrderByDescending(x => x.AssignmentDate).ToList();

    //                        EmployeeInfo.ApplicantName = personEmployee.FullNameaEnglish;
    //                        EmployeeInfo.BirthDate = personEmployee.DateOfBirthAm + " (" + personEmployee.DateOfBirth.ToString("MMMM dd, yyyy") + ")";
    //                        EmployeeInfo.HiredDate = personEmployee.HiredDateAmharic + " (" + personEmployee.HiredDate.ToString("MMMM dd, yyyy") + ")";
    //                        var educations = db.Education.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();
    //                        foreach (var education in educations)
    //                        {
    //                            var level = db.Lookups.Find(education.EduLevel);
    //                            var field = db.Lookups.Find(education.EduField);

    //                            if (level != null && field != null)
    //                            {
    //                                EmployeeInfo.EducationInfo += $" {level.Description} በ {field.Description} \n";
    //                            }
    //                        }
    //                        var currentPlacement = placements.FirstOrDefault(x => x.isCurrent);
    //                        if (currentPlacement != null)
    //                        {
    //                            EmployeeInfo.CurrentSalary = currentPlacement.Salary;
    //                            EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords(Convert.ToInt32(EmployeeInfo.CurrentSalary));
    //                        }
    //                        else
    //                        {
    //                            var lastPlacement = placements.FirstOrDefault();
    //                            if (lastPlacement != null)
    //                            {
    //                                EmployeeInfo.CurrentSalary = lastPlacement.Salary;
    //                                EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords(Convert.ToInt32(EmployeeInfo.CurrentSalary));
    //                            }
    //                        }
    //                        string allowanceList = "";
    //                        var employeeAllowances = db.AllowancePerEmployees.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();
    //                        foreach (var allowance in employeeAllowances)
    //                        {
    //                            var allownacevaName = db.AllowanceDefinations.Find(allowance.AllowanceDefinationId);
    //                            if (allownacevaName != null)
    //                            {
    //                                allowanceList += allownacevaName.AllowanceName + "-" + allownacevaName.AllowanceType.ToString() + "-" + allownacevaName.Amount + " " + allownacevaName.AmtPercent.ToString() + ",";
    //                            }

    //                        }
    //                        EmployeeInfo.Allowance = allowanceList;
    //                        EmployeeInfo.Expriance = new List<CurrentEmployeeExpriance>();

    //                        for (int e = 0; e < placements.Count(); e++)
    //                        {
    //                            var currentEmployeeExpriance = new CurrentEmployeeExpriance();

    //                            var difference = new YearMonthDay();

    //                            if (e >= 1)
    //                            {
    //                                currentEmployeeExpriance.From = placements[e].AssignmentDate.ToShortDateString();

    //                                currentEmployeeExpriance.To = placements[e - 1].AssignmentDate.ToShortDateString();

    //                                var fromc = new DateTime(placements[e].AssignmentDate.Year, placements[e].AssignmentDate.Month, placements[e].AssignmentDate.Day);
    //                                var toc = new DateTime(placements[e - 1].AssignmentDate.Year, placements[e - 1].AssignmentDate.Month, placements[e - 1].AssignmentDate.Day);

    //                                difference = GetYearMonthDays(fromc, toc).FirstOrDefault();

    //                            }
    //                            else
    //                            {
    //                                currentEmployeeExpriance.From = placements[e].AssignmentDate.Date.ToShortDateString();

    //                                currentEmployeeExpriance.To = " Now";
    //                                var fromc = new DateTime(placements[e].AssignmentDate.Year, placements[e].AssignmentDate.Month, placements[e].AssignmentDate.Day);
    //                                var toc = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day);


    //                                difference = GetYearMonthDays(fromc, toc).FirstOrDefault();
    //                            }

    //                            currentEmployeeExpriance.JobTitleName = placements[e].JobTitleName;

    //                            int branchId = placements[e].BranchId;
    //                            var branch = db.Branch.FirstOrDefault(x => x.BranchId == branchId);
    //                            if (branch != null)
    //                            {
    //                                currentEmployeeExpriance.Branch = branch.Name;
    //                            }
    //                            currentEmployeeExpriance.RecruitmentType = placements[e].EmployementType.ToString();


    //                            currentEmployeeExpriance.Year = difference.Years.ToString() + " " + " Year " +
    //                                                            difference.Months.ToString() + " " + " Month " +
    //                                                            difference.Days.ToString() + " " + " Days ";

    //                            currentEmployeeExpriance.Salary = placements[e].Salary;
    //                            EmployeeInfo.Expriance.Add(currentEmployeeExpriance);
    //                        }

    //                        #region History

    //                        var empHistory = db.PlacementHistories.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();

    //                        foreach (var e in empHistory)
    //                        {
    //                            var currentEmployeeExpriance = new CurrentEmployeeExpriance();

    //                            currentEmployeeExpriance.From = e.From.ToShortDateString();
    //                            currentEmployeeExpriance.To = e.To.ToShortDateString();

    //                            currentEmployeeExpriance.JobTitleName = e.JobTitle;

    //                            var difference = GetYearMonthDays(e.From, e.To).FirstOrDefault();

    //                            currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);
    //                            currentEmployeeExpriance.RecruitmentType = e.EmployementType.ToString();

    //                            currentEmployeeExpriance.Salary = e.Salary;

    //                            EmployeeInfo.Expriance.Add(currentEmployeeExpriance);
    //                        }
    //                        #endregion

    //                        EmployeeInfo.OtherExpriance = new List<OtherEmployeeExpriance>();

    //                        var externalExperiences = db.Experiences.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();

    //                        foreach (var e in externalExperiences)
    //                        {
    //                            var otherexplist = new OtherEmployeeExpriance();

    //                            otherexplist.From = e.From.Date.ToShortDateString();
    //                            otherexplist.To = e.To.Date.ToShortDateString();
    //                            otherexplist.CompanyName = e.Organozation;
    //                            otherexplist.JobTitleName = e.Responsibility;

    //                            var difference = GetYearMonthDays(e.From, e.To).FirstOrDefault();

    //                            otherexplist.Year = difference.Years.ToString() + " " + " Year " +
    //                                                difference.Months.ToString() + " Month " +
    //                                                difference.Days.ToString() + " " + " Days";

    //                            otherexplist.Salary = e.Salary;

    //                            EmployeeInfo.OtherExpriance.Add(otherexplist);
    //                        }

    //                        EmployeeInfoList.Add(EmployeeInfo);
    //                    }
    //                }

    //            }



    //        }
    //        foreach (var exp in EmployeeInfoList)
    //        {
    //            var letter = new ExpLetter();

    //            #region Headers

    //            letter.Abbreviation = exp.Abbreviation;
    //            letter.Address = exp.Address;
    //            letter.AmharicName = exp.AmharicName;
    //            letter.City = exp.City;
    //            letter.Email = exp.Email;
    //            letter.EnglishName = exp.EnglishName;
    //            letter.Fax = exp.Fax;
    //            letter.LogoFileName = exp.LogoFileName;
    //            letter.Name = exp.Name;
    //            letter.Pobox = exp.Pobox;
    //            letter.Tel = exp.Tel;
    //            letter.WebSite = exp.WebSite;

    //            #endregion

    //            if (language != null 
    //                && language.ToLower() == "local"
    //                && (cultureName.ToLower() == "am" 
    //                || cultureName.ToLower() == "am-et"))
    //            {
    //                var template = db.Templates.FirstOrDefault(x => x.TemplateType.ToString() == type);

    //                if (template != null)
    //                {
    //                    letter.Title = "ለሚመለከተው ሁሉ ";

    //                    string doc = "";

    //                    //doc = template.Document.Replace("-ApplicantName-", exp.ApplicantName);
    //                    //doc = template.Document.Replace("-BirthDate-", exp.BirthDate);
    //                    //doc = template.Document.Replace("-HiredDate-", exp.HiredDate);
    //                    //doc = template.Document.Replace("-ApplicantName-", exp.ApplicantName);
    //                    //doc = template.Document.Replace("-BirthDate-", exp.BirthDate);
    //                    //doc = template.Document.Replace("-HiredDate-", exp.HiredDate);
    //                    doc = template.Document.Replace("-ApplicantName-", exp.ApplicantName);
    //                    doc = doc.Replace("-ApplicantName-", exp.ApplicantName);
    //                    doc = doc.Replace("-BirthDate-", exp.BirthDate);
    //                    doc = doc.Replace("-HiredDate-", exp.HiredDate);
    //                    doc = doc.Replace("-EducationInfo-", exp.EducationInfo);
    //                    doc = doc.Replace("-ApplicationDate-", exp.Date);
    //                    doc = doc.Replace("-HimHer-", exp.HimHer);
    //                    doc = doc.Replace("-HisHer-", exp.HisHer);

    //                    if (exp.IsTerminated)
    //                    {
    //                        doc = doc.Replace("-IsTerminated-", "ይከፈላቸው የነበረው");
    //                        doc = doc.Replace("-IfTerminated-", "");
    //                    }
    //                    else
    //                    {
    //                        doc = doc.Replace("-IsTerminated-", " አሁን እየተከፈላቸው ያለ ");
    //                        doc = doc.Replace("-IfTerminated-", " ይህ የስራ ልምድ እንደ ስራ መልቀቂያ አያገለግልም፡፡ ");

    //                    }

    //                    string experienceConcat = "";
    //                    int rowNo = 1;

    //                    foreach (var con in exp.Expriance)
    //                    {
    //                        experienceConcat = experienceConcat + " - " + con.JobTitleName + " ከ " + con.From + " እስከ " + con.To + " በ " + con.Branch + " " + con.RecruitmentType + "</br>";
    //                        rowNo++;
    //                    }

    //                    foreach (var con in exp.OtherExpriance)
    //                    {
    //                        experienceConcat = experienceConcat + " - " + con.JobTitleName + " ከ " + con.From + " እስከ " + con.To + " በ " + con.CompanyName + "</br>";
    //                        rowNo++;
    //                    }

    //                    doc = doc.Replace("-Experiences-", experienceConcat);

    //                    string salary = exp.CurrentSalary + " /" + exp.SalaryInWord + " /" + ", አበል" + exp.Allowance;
    //                    doc = doc.Replace("-Salary-", salary);

    //                    letter.Experience = doc;

    //                    expLetterList.Add(letter);
    //                }
    //            }
    //            else if(language != null
    //                && language.ToLower() == "local" 
    //                && cultureName.ToLower() == "om")
    //            {
    //                    letter.Title = "KAN IRRATTI YAADACHUU DANDA'A";

    //                    var template = db.Templates.FirstOrDefault(x => x.TemplateType.ToString() == type);




    //                if (template != null)
    //                {

    //                    string doc = "";
    //                    doc = template.Document.Replace("-ApplicantName-", exp.ApplicantName);
    //                    doc = doc.Replace("-BirthDate-", exp.BirthDate);
    //                    doc = doc.Replace("-HiredDate-", exp.HiredDate);
    //                    doc = doc.Replace("-EducationInfo-", exp.EducationInfo);
    //                    doc = doc.Replace("-ApplicationDate-", exp.Date);
    //                    doc = doc.Replace("-HimHer-", exp.HimHer);
    //                    doc = doc.Replace("-HisHer-", exp.HisHer);
    //                    string experienceConcat = "";
    //                    int rowNo = 1;
    //                    foreach (var con in exp.Expriance)
    //                    {
    //                        experienceConcat += $" - {con.JobTitleName} irraa {con.From} gara {con.To} itti {con.Branch} {con.RecruitmentType} </br>";
    //                        rowNo++;

    //                    }

    //                    foreach (var con in exp.OtherExpriance)
    //                    {
    //                        experienceConcat = experienceConcat + " - " + con.JobTitleName + " from " + con.From + " to " + con.To + " at " + con.CompanyName + "</br>";
    //                        rowNo++;
    //                    }


    //                        doc = doc.Replace("-Experiences-", experienceConcat);

    //                        string salary = exp.CurrentSalary + " / " + exp.SalaryInWord + " / " + ",  allowance of " + exp.Allowance;
    //                        doc = doc.Replace("-Salary-", salary);

    //                        letter.Experience = doc;

    //                        expLetterList.Add(letter);



    //                }


    //            } 
    //            else
    //            {

    //                var template = db.Templates.FirstOrDefault(x => x.TemplateType.ToString() == type);
    //                if (template != null)
    //                {

    //                    string doc = "";


    //                    doc = template.Document.Replace("-ApplicantName-", exp.ApplicantName);
    //                    doc = doc.Replace("-BirthDate-", exp.BirthDate);
    //                    doc = doc.Replace("-HiredDate-", exp.HiredDate);
    //                    doc = doc.Replace("-EducationInfo-", exp.EducationInfo);
    //                    doc = doc.Replace("-ApplicationDate-", exp.Date);
    //                    doc = doc.Replace("-HimHer-", exp.HimHer);
    //                    doc = doc.Replace("-HisHer-", exp.HisHer);
    //                    string experienceConcat = "";
    //                    int rowNo = 1;


    //                    foreach (var con in exp.Expriance)
    //                    {
    //                            experienceConcat = experienceConcat + " - " + con.JobTitleName + " from " + con.From + " to " + con.To + " at " + con.Branch + " " + con.RecruitmentType + "</br>";
    //                            rowNo++;

    //                    }

    //                    foreach (var con in exp.OtherExpriance)
    //                    {
    //                        experienceConcat = experienceConcat + " - " + con.JobTitleName + " from " + con.From + " to " + con.To + " at " + con.CompanyName + "</br>";
    //                        rowNo++;
    //                    }


    //                    doc = doc.Replace("-Experiences-", experienceConcat);

    //                    string salary = exp.CurrentSalary + " /" + exp.SalaryInWord + " /" + ",  allowance of " + exp.Allowance;
    //                    doc = doc.Replace("-Salary-", salary);
    //                    letter.Title = template.Title;
    //                    letter.Experience = doc;

    //                    expLetterList.Add(letter);

    //                }

    //            }


    //        }


    //        return expLetterList;


    //    }

    //    public List<YearMonthDay> GetYearMonthDays(DateTime from, DateTime to)
    //    {
    //        List<YearMonthDay> differenceList = new List<YearMonthDay>();

    //        var difference = new YearMonthDay();

    //        double totalDays = (to - from).Days;

    //        difference.Years = Math.Truncate(totalDays / 365);
    //        difference.Months = Math.Truncate((totalDays % 365) / 30);
    //        difference.Days = Math.Truncate((totalDays % 365) % 30);

    //        differenceList.Add(difference);

    //        return differenceList;
    //    }


    //}


}
