using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Templates;
using ExceedERP.Core.Localization;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class ExperienceObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExpLetter> GetEmployeeExperience(string id, bool header, bool includeExt, string language)
        {
            List<EmployeeExperienceLetter> EmployeeInfoList = new List<EmployeeExperienceLetter>();
            EmployeeExperienceLetter EmployeeInfo = new EmployeeExperienceLetter();
            List<ExpLetter> expLetterList = new List<ExpLetter>();
            var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            if (header)
            {
                if (Organization != null)
                {
                    EmployeeInfo.Name = Organization.TradeName;
                    EmployeeInfo.AmharicName = Organization.AmharicName;
                    EmployeeInfo.Abbreviation = Organization.Abbreviation;
                    var address =
                        db.Addresses.Where(a =>
                                a.Reference == Organization.MainCompanyId.ToString() &&
                                a.ReferenceType.ToLower() == "company").ToList();

                    if (address.Any())
                    {
                        foreach (var a in address)
                        {
                            if (a.Category == AddressType.KIFLEKETEMA)
                            {
                                EmployeeInfo.Address = "Subcity: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.KEBELE)
                            {
                                EmployeeInfo.Address += "Kebele: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (EmployeeInfo.Address == null)
                                {
                                    EmployeeInfo.Address += a.Value + " -Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                EmployeeInfo.Pobox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                EmployeeInfo.Email = "Email: " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                EmployeeInfo.WebSite = "Website:" + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                EmployeeInfo.Fax = "Fax:  " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                EmployeeInfo.Tel = "Tel:  " + a.Value;

                            }

                        }

                    }
                    var logo =
                        db.Attachments.Where(
                            a =>
                                a.Reference == Organization.MainCompanyId.ToString() &&
                                a.ReferenceType == AttachmentType.LOGO).FirstOrDefault();
                    if (logo != null)
                    {
                        EmployeeInfo.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                        EmployeeInfo.LogoFile = logo.BinaryFile;
                    }
                }
            }
            if (id != null)
            {
                int.TryParse(id, out int empId);

                var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                if (personEmployee != null)
                {

                    var experienceRequest = db.ExprianceRequests.FirstOrDefault(x => x.EmployeeId == personEmployee.EmployeeId && x.RequestType == SelfServiceRequestType.Expreience);
                    if (experienceRequest != null)
                    {
                        var termination = db.Terminations.FirstOrDefault(x => x.EmployeeId == personEmployee.EmployeeId && x.IsOnlineApproved);
                        if (termination != null)
                        {
                            EmployeeInfo.IsTerminated = true;
                        }
                        else
                        {
                            EmployeeInfo.IsTerminated = false;
                        }
                        if (language != null 
                            && language.ToLower() == "local" 
                            && (cultureName.ToLower() == "am"
                            || cultureName.ToLower() == "am-et")
                            || cultureName.ToLower() == "om") 
                        {
                            EmployeeInfo.CheckTermination = (cultureName.ToLower() == "am"
                            || cultureName.ToLower() == "am-et")?" ጠቅላላ የወር ደሞዝ ": "Mindaa waliigalaa ji'aa";
                            EmployeeInfo.CheckCurrent = "";
                            EmployeeInfo.TerminationMassage = "";
                            EmployeeInfo.Date = experienceRequest.ApplicationDateAm;

                            if (personEmployee.Gender == Gender.FEMALE)
                            {
                                EmployeeInfo.HimHer = "her";
                                EmployeeInfo.HisHer = "her";
                            }
                            else
                            {
                                EmployeeInfo.HimHer = "him";
                                EmployeeInfo.HisHer = "his";
                            }

                            var placements = db.Placements.Where(x => x.EmployeeId == personEmployee.EmployeeId).OrderByDescending(x => x.AssignmentDate).ToList();

                            EmployeeInfo.ApplicantName = personEmployee.FullNameAmharic;
                            EmployeeInfo.BirthDate = personEmployee.DateOfBirthAm /*+ "E.c (" + personEmployee.DateOfBirth.ToString("MMMM dd, yyyy") + "G.c)"*/;
                            EmployeeInfo.HiredDate = personEmployee.HiredDateAmharic /*+ "E.c (" + personEmployee.HiredDate.ToString("MMMM dd, yyyy") + "G.C)"*/;
                            var educations = db.Education.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();
                            foreach(var education in educations)
                            {
                                var level = db.Lookups.Find(education.EduLevel);
                                var field = db.Lookups.Find(education.EduField);

                                if (level !=null&& field != null)
                                {
                                    EmployeeInfo.EducationInfo += $" {level.Description} በ {field.Description},  ";
                                }
                            }
                            var currentPlacement = placements.FirstOrDefault(x => x.isCurrent);
                            if (currentPlacement != null)
                            {
                                EmployeeInfo.CurrentSalary = currentPlacement.Salary;
                                EmployeeInfo.SalaryInWord = (cultureName.ToLower() == "am"
                            || cultureName.ToLower() == "am-et") ? Convert.ToInt32(EmployeeInfo.CurrentSalary).ToWordAmharic(): Convert.ToInt32(EmployeeInfo.CurrentSalary).ToWordEnglish();
                            }
                            else
                            {
                                var lastPlacement = placements.FirstOrDefault();
                                if (lastPlacement != null)
                                {
                                    EmployeeInfo.CurrentSalary = lastPlacement.Salary;
                                    EmployeeInfo.SalaryInWord = (cultureName.ToLower() == "am"
                                                                || cultureName.ToLower() == "am-et") ? Convert.ToInt32(EmployeeInfo.CurrentSalary).ToWordAmharic() 
                                                                : Convert.ToInt32(EmployeeInfo.CurrentSalary).ToWordEnglish();
                                }
                            }
                            string allowanceList = "";
                            var employeeAllowances = db.AllowancePerEmployees.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();
                            foreach(var allowance in employeeAllowances)
                            {
                                var allownacevaName = db.AllowanceDefinations.Find(allowance.AllowanceDefinationId);
                                if(allownacevaName != null)
                                {
                                    allowanceList += allownacevaName.AllowanceType.ToString() +"-"+allownacevaName.Amount+""+allownacevaName.AmtPercent.ToString()+",";
                                }
                            }
                            EmployeeInfo.Allowance = allowanceList;
                            EmployeeInfo.Expriance = new List<CurrentEmployeeExpriance>();

                            for (int e = 0; e < placements.Count(); e++)
                            {
                                var currentEmployeeExpriance = new CurrentEmployeeExpriance();

                                var difference = new YearMonthDay();

                                if (e >= 1)
                                {
                                    currentEmployeeExpriance.From = placements[e].AssignmentDateAmharic;

                                    currentEmployeeExpriance.To = placements[e - 1].AssignmentDateAmharic;

                                    var fromc = new DateTime(placements[e].AssignmentDate.Year, placements[e].AssignmentDate.Month, placements[e].AssignmentDate.Day);
                                    var toc = new DateTime(placements[e - 1].AssignmentDate.Year, placements[e - 1].AssignmentDate.Month, placements[e - 1].AssignmentDate.Day);

                                    difference = GetYearMonthDays(fromc, toc).FirstOrDefault();

                                }
                                else
                                {
                                    currentEmployeeExpriance.From = placements[e].AssignmentDateAmharic;

                                    currentEmployeeExpriance.To = (cultureName.ToLower() == "am"
                                                                    || cultureName.ToLower() == "am-et") ? "አሁን" : cultureName.ToLower() == "am"? "amma":"now";
                                    var fromc = new DateTime(placements[e].AssignmentDate.Year, placements[e].AssignmentDate.Month, placements[e].AssignmentDate.Day);
                                    var toc = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day);


                                    difference = GetYearMonthDays(fromc, toc).FirstOrDefault();
                                }

                                currentEmployeeExpriance.JobTitleName = placements[e].JobTitleName;

                                int branchId = placements[e].BranchId;
                                var branch = db.Branch.FirstOrDefault(x => x.BranchId == branchId);
                                if (branch != null)
                                {
                                    currentEmployeeExpriance.Branch = branch.AmharicName;
                                }
                                //currentEmployeeExpriance.RecruitmentType = GetDisplayNameByCulture(placements[e].EmployementType, cultureName).ToLower();
                                //currentEmployeeExpriance.RecruitmentType = GetLocalizedDisplayName(placements[e].EmployementType).ToLower(CultureInfo.CurrentUICulture);



                                currentEmployeeExpriance.Year = difference.Years.ToString() + " " + " አመት " +
                                                                difference.Months.ToString() + " " + " ወር " +
                                                                difference.Days.ToString() + " " + " ቀን ";

                                currentEmployeeExpriance.Salary = placements[e].Salary;
                                EmployeeInfo.Expriance.Add(currentEmployeeExpriance);
                            }

                            #region History

                            var empHistory = db.PlacementHistories.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();

                            foreach (var e in empHistory)
                            {
                                var currentEmployeeExpriance = new CurrentEmployeeExpriance();

                                currentEmployeeExpriance.From = e.From.ToShortDateString();
                                currentEmployeeExpriance.To = e.To.ToShortDateString();

                                currentEmployeeExpriance.JobTitleName = e.JobTitle;

                                var difference = GetYearMonthDays(e.From, e.To).FirstOrDefault();

                                currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);

                                currentEmployeeExpriance.Salary = e.Salary;
                               //currentEmployeeExpriance.RecruitmentType = GetDisplayNameByCulture(e.EmployementType, cultureName).ToLower();

                                EmployeeInfo.Expriance.Add(currentEmployeeExpriance);
                            }
                            #endregion

                            EmployeeInfo.OtherExpriance = new List<OtherEmployeeExpriance>();

                            var externalExperiences = db.Experiences.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();

                            foreach (var e in externalExperiences)
                            {
                                var otherexplist = new OtherEmployeeExpriance();

                                otherexplist.From = e.From.Date.ToShortDateString();
                                otherexplist.To = e.To.Date.ToShortDateString();
                                otherexplist.CompanyName = e.Organozation;
                                otherexplist.JobTitleName =  e.Responsibility;

                                var difference = GetYearMonthDays(e.From, e.To).FirstOrDefault();

                                otherexplist.Year = difference.Years.ToString() + " " + " አመት " +
                                                    difference.Months.ToString() + " ወር " +
                                                    difference.Days.ToString() + " " + " ቀን ";

                                otherexplist.Salary = e.Salary;

                                EmployeeInfo.OtherExpriance.Add(otherexplist);
                            }

                            EmployeeInfoList.Add(EmployeeInfo);
                        }
                        else
                        {

                            EmployeeInfo.CheckTermination = " monthly gross salary was ";
                            EmployeeInfo.CheckCurrent = "";
                            EmployeeInfo.TerminationMassage = "";
                            EmployeeInfo.Date = experienceRequest.ApplicationDate.ToShortDateString();

                            if (personEmployee.Gender == Gender.FEMALE)
                            {
                                EmployeeInfo.HimHer = "her";
                                EmployeeInfo.HisHer = "her";
                            }
                            else
                            {
                                EmployeeInfo.HimHer = "him";
                                EmployeeInfo.HisHer = "his";
                            }

                            var placements = db.Placements.Where(x => x.EmployeeId == personEmployee.EmployeeId).OrderByDescending(x => x.AssignmentDate).ToList();

                            EmployeeInfo.ApplicantName = personEmployee.FullNameaEnglish;
                            EmployeeInfo.BirthDate = personEmployee.DateOfBirthAm /*+ " E.c(" + personEmployee.DateOfBirth.ToString("MMMM dd, yyyy") + "G.C)"*/;
                            EmployeeInfo.HiredDate = personEmployee.HiredDateAmharic/* + "E.c (" + personEmployee.HiredDate.ToString("MMMM dd, yyyy") + "G.C)"*/;
                            var educations = db.Education.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();
                            foreach (var education in educations)
                            {
                                var level = db.Lookups.Find(education.EduLevel);
                                var field = db.Lookups.Find(education.EduField);

                                if (level != null && field != null)
                                {
                                    EmployeeInfo.EducationInfo += $" {level.Description} in {field.Description} \n";
                                }
                            }
                            var currentPlacement = placements.FirstOrDefault(x => x.isCurrent);
                            if (currentPlacement != null)
                            {
                                EmployeeInfo.CurrentSalary = currentPlacement.Salary;
                                EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords(Convert.ToInt32(EmployeeInfo.CurrentSalary));
                            }
                            else
                            {
                                var lastPlacement = placements.FirstOrDefault();
                                if (lastPlacement != null)
                                {
                                    EmployeeInfo.CurrentSalary = lastPlacement.Salary;
                                    EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords(Convert.ToInt32(EmployeeInfo.CurrentSalary));
                                }
                            }
                            string allowanceList = "";
                            var employeeAllowances = db.AllowancePerEmployees.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();
                            foreach (var allowance in employeeAllowances)
                            {
                                var allownacevaName = db.AllowanceDefinations.Find(allowance.AllowanceDefinationId);
                                if (allownacevaName != null)
                                {
                                    allowanceList += allownacevaName.AllowanceName + "-" + allownacevaName.AllowanceType.ToString() + "-" + allownacevaName.Amount + " " + allownacevaName.AmtPercent.ToString() + ",";
                                }

                            }
                            EmployeeInfo.Allowance = allowanceList;
                            EmployeeInfo.Expriance = new List<CurrentEmployeeExpriance>();

                            for (int e = 0; e < placements.Count(); e++)
                            {
                                var currentEmployeeExpriance = new CurrentEmployeeExpriance();

                                var difference = new YearMonthDay();

                                if (e >= 1)
                                {
                                    currentEmployeeExpriance.From = placements[e].AssignmentDate.ToShortDateString();

                                    currentEmployeeExpriance.To = placements[e - 1].AssignmentDate.ToShortDateString();

                                    var fromc = new DateTime(placements[e].AssignmentDate.Year, placements[e].AssignmentDate.Month, placements[e].AssignmentDate.Day);
                                    var toc = new DateTime(placements[e - 1].AssignmentDate.Year, placements[e - 1].AssignmentDate.Month, placements[e - 1].AssignmentDate.Day);

                                    difference = GetYearMonthDays(fromc, toc).FirstOrDefault();

                                }
                                else
                                {
                                    currentEmployeeExpriance.From = placements[e].AssignmentDate.Date.ToShortDateString();

                                    currentEmployeeExpriance.To = " Now";
                                    var fromc = new DateTime(placements[e].AssignmentDate.Year, placements[e].AssignmentDate.Month, placements[e].AssignmentDate.Day);
                                    var toc = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day);


                                    difference = GetYearMonthDays(fromc, toc).FirstOrDefault();
                                }

                                currentEmployeeExpriance.JobTitleName = placements[e].JobTitleName;

                                int branchId = placements[e].BranchId;
                                var branch = db.Branch.FirstOrDefault(x => x.BranchId == branchId);
                                if (branch != null)
                                {
                                    currentEmployeeExpriance.Branch = branch.Name;
                                }
                                //currentEmployeeExpriance.RecruitmentType = GetDisplayNameByCulture(placements[e].EmployementType, cultureName).ToLower();


                                currentEmployeeExpriance.Year = difference.Years.ToString() + " " + " Year " +
                                                                difference.Months.ToString() + " " + " Month " +
                                                                difference.Days.ToString() + " " + " Days ";

                                currentEmployeeExpriance.Salary = placements[e].Salary;
                                EmployeeInfo.Expriance.Add(currentEmployeeExpriance);
                            }

                            #region History

                            var empHistory = db.PlacementHistories.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();

                            foreach (var e in empHistory)
                            {
                                var currentEmployeeExpriance = new CurrentEmployeeExpriance();

                                currentEmployeeExpriance.From = e.From.ToShortDateString();
                                currentEmployeeExpriance.To = e.To.ToShortDateString();

                                currentEmployeeExpriance.JobTitleName = e.JobTitle;

                                var difference = GetYearMonthDays(e.From, e.To).FirstOrDefault();

                                currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);
                                //currentEmployeeExpriance.RecruitmentType = GetLocalizedDisplayName(e.EmployementType).ToLower(CultureInfo.CurrentUICulture);
                                //currentEmployeeExpriance.RecruitmentType = GetDisplayNameByCulture(e.EmployementType, cultureName).ToLower(); ;

                                currentEmployeeExpriance.Salary = e.Salary;

                                EmployeeInfo.Expriance.Add(currentEmployeeExpriance);
                            }
                            #endregion

                            EmployeeInfo.OtherExpriance = new List<OtherEmployeeExpriance>();

                            var externalExperiences = db.Experiences.Where(x => x.EmployeeId == personEmployee.EmployeeId).ToList();

                            foreach (var e in externalExperiences)
                            {
                                var otherexplist = new OtherEmployeeExpriance();

                                otherexplist.From = e.From.Date.ToShortDateString();
                                otherexplist.To = e.To.Date.ToShortDateString();
                                otherexplist.CompanyName = e.Organozation;
                                otherexplist.JobTitleName = e.Responsibility;

                                var difference = GetYearMonthDays(e.From, e.To).FirstOrDefault();

                                otherexplist.Year = difference.Years.ToString() + " " + " Year " +
                                                    difference.Months.ToString() + " Month " +
                                                    difference.Days.ToString() + " " + " Days";

                                otherexplist.Salary = e.Salary;

                                EmployeeInfo.OtherExpriance.Add(otherexplist);
                            }

                            EmployeeInfoList.Add(EmployeeInfo);
                        }
                    }

                }



            }
            foreach (var exp in EmployeeInfoList)
            {
                var letter = new ExpLetter
                {
                    Abbreviation = exp.Abbreviation,
                    Address = exp.Address,
                    AmharicName = exp.AmharicName,
                    City = exp.City,
                    Region = exp.Region,
                    Country = exp.Country,
                    Email = exp.Email,
                    EnglishName = exp.EnglishName,
                    Fax = exp.Fax,
                    LogoFileName = exp.LogoFileName,
                    Name = exp.Name,
                    Pobox = exp.Pobox,
                    Tel = exp.Tel,
                    WebSite = exp.WebSite
                };

                string lang = language?.ToLower();
                string culture = cultureName?.ToLower();
                Template template = null;

                if (lang == "local" && (culture == "am" || culture == "am-et"))
                {
                    template = db.Templates.FirstOrDefault(x => x.TemplateType == TemplateTypes.ExperianceAm);
                    letter.Title = "ለሚመለከተው ሁሉ ";
                }
                else if (lang == "local" && culture == "om")
                {
                    template = db.Templates.FirstOrDefault(x => x.TemplateType == TemplateTypes.ExperianceOromo);
                    letter.Title = "KAN IRRATTI YAADACHUU DANDA'A";
                }
                else
                {
                    template = db.Templates.FirstOrDefault(x => x.TemplateType == TemplateTypes.ExperianceEng);
                    letter.Title = "TO WHOM IT MAY CONCERN";
                }

                if (template != null)
                {
                    var doc = template.Document;
                    doc = doc.Replace("-ApplicantName-", exp.ApplicantName)
                             .Replace("-BirthDate-", exp.BirthDate)
                             .Replace("-HiredDate-", exp.HiredDate)
                             .Replace("-EducationInfo-", exp.EducationInfo)
                             .Replace("-ApplicationDate-", exp.Date)
                             .Replace("-HimHer-", exp.HimHer)
                             .Replace("-HisHer-", exp.HisHer);

                    // Replace termination details
                    if (exp.IsTerminated)
                    {
                        switch (culture)
                        {
                            case "am":
                            case "am-et":
                                doc = doc.Replace("-IsTerminated-", "ይከፈላቸው የነበረው")
                                         .Replace("-IfTerminated-", "");
                                break;

                            case "om":
                                doc = doc.Replace("-IsTerminated-", "kan kaffalamu ture")
                                         .Replace("-IfTerminated-", "");
                                break;

                            default: // English and others
                                doc = doc.Replace("-IsTerminated-", "was formerly being paid")
                                         .Replace("-IfTerminated-", "");
                                break;
                        }
                    }
                    else
                    {
                        switch (culture)
                        {
                            case "am":
                            case "am-et":
                                doc = doc.Replace("-IsTerminated-", " አሁን እየተከፈላቸው ያለ ")
                                         .Replace("-IfTerminated-", " ይህ የስራ ልምድ እንደ ስራ መልቀቂያ አያገለግልም፡፡ ");
                                break;

                            case "om":
                                doc = doc.Replace("-IsTerminated-", "amma kaffalama jira")
                                         .Replace("-IfTerminated-", " Galmeen hojii kun ragaa addaan kutamuu miti.");
                                break;

                            default: // English and others
                                doc = doc.Replace("-IsTerminated-", "currently being paid")
                                         .Replace("-IfTerminated-", " This experience letter is not a resignation confirmation.");
                                break;
                        }
                    }


                    // Build experience string
                    var experienceConcat = new StringBuilder();
                    foreach (var con in exp.Expriance)
                    {
                        if (culture == "am" || culture == "am-et")
                            experienceConcat.AppendLine($"- {con.JobTitleName} ከ {con.From} እስከ {con.To} በ {con.Branch} {con.RecruitmentType}");
                        else if (culture == "om")
                            experienceConcat.AppendLine($"- {con.JobTitleName} irraa {con.From} gara {con.To} itti {con.Branch} {con.RecruitmentType}");
                        else
                            experienceConcat.AppendLine($"- {con.JobTitleName} from {con.From} to {con.To} at {con.Branch} {con.RecruitmentType}");
                    }

                    foreach (var con in exp.OtherExpriance)
                    {
                        if (culture == "am" || culture == "am-et")
                            experienceConcat.AppendLine($"- {con.JobTitleName} ከ {con.From} እስከ {con.To} በ {con.CompanyName}");
                        else if (culture == "om")
                            experienceConcat.AppendLine($"- {con.JobTitleName} irraa {con.From} gara {con.To} itti {con.CompanyName}");
                        else
                            experienceConcat.AppendLine($"- {con.JobTitleName} from {con.From} to {con.To} at {con.CompanyName}");
                    }

                    doc = doc.Replace("-Experiences-", experienceConcat.ToString());

                    string salary = $"{exp.CurrentSalary}/{exp.SalaryInWord}/, {(culture == "am" || culture == "am-et" ? "አበል" : "allowance of")} {exp.Allowance}";
                    doc = doc.Replace("-Salary-", salary);

                    letter.Experience = doc;
                    expLetterList.Add(letter);
                }
            }

            

            return expLetterList;


        }
        public static string GetDisplayNameByCulture(Enum value, string cultureName)
        {
            if (value == null) return string.Empty;

            var field = value.GetType().GetField(value.ToString());
            var displayAttr = field?.GetCustomAttribute<DisplayAttribute>();

            if (displayAttr == null || string.IsNullOrEmpty(displayAttr.Name))
                return value.ToString();

            // Use the GetName() method — it uses the current thread's UI culture and ResourceType
            return displayAttr.GetName() ?? value.ToString();
        }



        public List<YearMonthDay> GetYearMonthDays(DateTime from, DateTime to)
        {
            List<YearMonthDay> differenceList = new List<YearMonthDay>();

            var difference = new YearMonthDay();

            double totalDays = (to - from).Days;

            difference.Years = Math.Truncate(totalDays / 365);
            difference.Months = Math.Truncate((totalDays % 365) / 30);
            difference.Days = Math.Truncate((totalDays % 365) % 30);

            differenceList.Add(difference);

            return differenceList;
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn != null)
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
            return null;

        }
     
            public void Dispose()
            {
                db.Dispose();
            }
        
    }

    public class YearMonthDay
    {
        public double Years { get; set; }
        public double Months { get; set; }
        public double Days { get; set; }
    }
    public class ExpLetter
    {
        public string Experience { get; set; }
        public string Title { get; set; }
        public string Allowance { get; set; }
        public string HeadName { get; set; }
        public string HeadJobTitle { get; set; }
        ////// Header ///////////
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string EnglishName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public byte[] LogoFile { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public string Fax { get; set; }
        public string Pobox { get; set; }
        public string Tel { get; set; }
        public string ApprovedBy { get; set; }
        public Image WaterMark { get; set; }
        public Image Signature { get; set; }
        public byte[] SignatureFile { get; set; }
    }

}
