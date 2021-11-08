using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Templates;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class ExperienceObjectDataSource : List<EmployeeExperienceLetter>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExpLetter> GetEmployeeExperience(string id, bool header, bool includeExt, string language)
        {
            List<EmployeeExperienceLetter> EmployeeInfoList = new List<EmployeeExperienceLetter>();
            EmployeeExperienceLetter EmployeeInfo = new EmployeeExperienceLetter();

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
                    }
                }
            }
            if (id != null)
            {
                int.TryParse(id, out int empId);

                var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                if (personEmployee != null)
                {
                    var experienceRequest = db.ExprianceRequests.FirstOrDefault(x => x.EmployeeId == personEmployee.EmployeeId && x.RequestType == SelfServiceRequestType.የሥራ_ልምድ);
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

                     

                        if (language != null && language.ToLower() == "amharic")
                        {
                            EmployeeInfo.CheckTermination = " ጠቅላላ የወር ደሞዝ ";
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

                            var currentPlacement = placements.FirstOrDefault(x => x.isCurrent);
                            if (currentPlacement != null)
                            {
                                EmployeeInfo.CurrentSalary = currentPlacement.Salary;
                                EmployeeInfo.SalaryInWord = Convert.ToInt32(EmployeeInfo.CurrentSalary).ToWordAmharic();
                            }
                            else
                            {
                                var lastPlacement = placements.FirstOrDefault();
                                if (lastPlacement != null)
                                {
                                    EmployeeInfo.CurrentSalary = lastPlacement.Salary;
                                    EmployeeInfo.SalaryInWord = Convert.ToInt32(EmployeeInfo.CurrentSalary).ToWordAmharic();
                                }
                            }

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

                                    currentEmployeeExpriance.To = " አሁን";
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
                                currentEmployeeExpriance.RecruitmentType = placements[e].EmployementType.ToString();


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
                                currentEmployeeExpriance.RecruitmentType = e.EmployementType.ToString();

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
                                currentEmployeeExpriance.RecruitmentType = placements[e].EmployementType.ToString();


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
                                currentEmployeeExpriance.RecruitmentType = e.EmployementType.ToString();

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


            List<ExpLetter> expLetterList = new List<ExpLetter>();

            foreach (var exp in EmployeeInfoList)
            {

                var letter = new ExpLetter();

                #region Headers

                letter.Abbreviation = exp.Abbreviation;
                letter.Address = exp.Address;
                letter.AmharicName = exp.AmharicName;
                letter.City = exp.City;
                letter.Email = exp.Email;
                letter.EnglishName = exp.EnglishName;
                letter.Fax = exp.Fax;
                letter.LogoFileName = exp.LogoFileName;
                letter.Name = exp.Name;
                letter.Pobox = exp.Pobox;
                letter.Tel = exp.Tel;
                letter.WebSite = exp.WebSite;

                #endregion

                if (language != null && language.ToLower() == "amharic")
                {
                    var template = db.Templates.FirstOrDefault(x => x.TemplateType == TemplateTypes.ExperianceAm);

                    if (template != null)
                    {
                        letter.Title = "ለሚመለከተው ሁሉ ";                       

                        string doc = "";

                        doc = template.Document.Replace("-ApplicantName-", exp.ApplicantName);
                        doc = doc.Replace("-ApplicationDate-", exp.Date);
                        doc = doc.Replace("-HimHer-", exp.HimHer);
                        doc = doc.Replace("-HisHer-", exp.HisHer);

                        if (exp.IsTerminated)
                        {
                            doc = doc.Replace("-IsTerminated-", "ይከፈላቸው የነበረው");
                            doc = doc.Replace("-IfTerminated-", "");
                        }
                        else
                        {
                            doc = doc.Replace("-IsTerminated-", " አሁን እየተከፈላቸው ያለ ");
                            doc = doc.Replace("-IfTerminated-", " ይህ የስራ ልምድ እንደ ስራ መልቀቂያ አያገለግልም፡፡ ");

                        }

                        string experienceConcat = "";
                        int rowNo = 1;

                        foreach (var con in exp.Expriance)
                        {
                            experienceConcat = experienceConcat + " - " + con.JobTitleName + " ከ " + con.From + " እስከ " + con.To + " በ " + con.Branch + " " + con.RecruitmentType + "\n";
                            rowNo++;
                        }

                        foreach (var con in exp.OtherExpriance)
                        {
                            experienceConcat = experienceConcat + " - " + con.JobTitleName + " ከ " + con.From + " እስከ " + con.To + " በ " + con.CompanyName + "\n";
                            rowNo++;
                        }

                        doc = doc.Replace("-Experiences-", experienceConcat);

                        string salary = exp.CurrentSalary + "/" + exp.SalaryInWord + "/";
                        doc = doc.Replace("-Salary-", salary);

                        letter.Experience = doc;

                        expLetterList.Add(letter);
                    }
                }
                else
                {
                    letter.Title = "TO WHOM IT MAY CONCERN";

                    var template = db.Templates.FirstOrDefault(x => x.TemplateType == TemplateTypes.ExperianceEng);

                    if (template != null)
                    {
                        string doc = "";

                        doc = template.Document.Replace("-ApplicantName-", exp.ApplicantName);
                        doc = doc.Replace("-ApplicationDate-", exp.Date);
                        doc = doc.Replace("-HimHer-", exp.HimHer);
                        doc = doc.Replace("-HisHer-", exp.HisHer);

                        string experienceConcat = "";
                        int rowNo = 1;


                        foreach (var con in exp.Expriance)
                        {
                            experienceConcat = experienceConcat + " - " + con.JobTitleName + " from " + con.From + " to " + con.To + " at " + con.Branch + " " + con.RecruitmentType + "\n";
                            rowNo++;
                        }

                        foreach (var con in exp.OtherExpriance)
                        {
                            experienceConcat = experienceConcat + " - " + con.JobTitleName + " from " + con.From + " to " + con.To + " at " + con.CompanyName + "\n";
                            rowNo++;
                        }


                        doc = doc.Replace("-Experiences-", experienceConcat);

                        string salary = exp.CurrentSalary + "/" + exp.SalaryInWord + "/";
                        doc = doc.Replace("-Salary-", salary);

                        letter.Experience = doc;

                        expLetterList.Add(letter);
                    }
                }
            }


            return expLetterList;
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

       ////// Header ///////////
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string EnglishName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string Pobox { get; set; }
        public string Tel { get; set; }
    }
}
