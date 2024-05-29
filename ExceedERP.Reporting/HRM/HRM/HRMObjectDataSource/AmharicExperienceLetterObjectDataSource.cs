using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{


    [DataObject]
    public class AmharicExperienceLetterObjectDataSource : List<EmployeeExperienceLetter>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeExperienceLetter> GetEmployeeAmharicExperience(string id, bool header)
        {


            List<EmployeeExperienceLetter> EmployeeInfoList = new List<EmployeeExperienceLetter>();
            EmployeeExperienceLetter EmployeeInfo = new EmployeeExperienceLetter();
            OtherEmployeeExpriance otherexplist = new OtherEmployeeExpriance();
            CurrentEmployeeExpriance currentEmployeeExpriance = new CurrentEmployeeExpriance();
            int TotalYear = 0;
            int TotalMonth = 0;
            int TotalDays = 0;
            var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();
            var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId.ToString() == id);

            
            EmployeeInfo.OtherExpriance = new List<OtherEmployeeExpriance>();
            EmployeeInfo.Expriance = new List<CurrentEmployeeExpriance>();
            if (personEmployee != null)
            {
                var employyee = personEmployee.EmployeeId;
                if (header)
                {
                    if (Organization != null)
                    {
                        EmployeeInfo.Name = Organization.TradeName;
                        EmployeeInfo.AmharicName = Organization.AmharicName;
                        EmployeeInfo.Abbreviation = Organization.Abbreviation;
                        var address =
                            db.Addresses.Where(
                                a =>
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
                                        EmployeeInfo.Address += a.Value + ", Ethiopia";
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
                            db.Attachments.FirstOrDefault(a => a.Reference == Organization.MainCompanyId.ToString() &&
                                    a.ReferenceType == AttachmentType.LOGO);
                        if (logo != null)
                        {
                            EmployeeInfo.LogoFileName =
                                OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                        }
                    }

                }



                var termination = db.Terminations.FirstOrDefault(x => x.EmployeeId == employyee);
                if (termination != null)
                {
                    EmployeeInfo.CheckTermination = " ይከፈላቸው የነበረው ";
                    EmployeeInfo.TerminationMassage = "";
                    EmployeeInfo.CheckCurrent = "";



                    //var ExprianceRequest =
                    //    db.ExprianceRequests.Where(x => x.EmployeeId == employyee).OrderBy(y => y.ApplicationDate)
                    //        .ToList();

                    //if (ExprianceRequest.Any() )
                    //{



                    var otherexpriances = db.Experiences.ToList();
                    var currentex = db.Placements.ToList();
                    var employees = db.Person_Employee.ToList();


                    //var emp = ExprianceRequest.FirstOrDefault();
                    //if (emp == null ) return new List<EmployeeExperienceLetter>();
                    //var approval =
                    //    db.RequestesStatus.Where(x =>
                    //        x.ReferenceType == "Experience" && x.RequestId == emp.ExperienceRequestId &&
                    //        x.Status == RequestStatus.APPROVE).ToList();
                    //if (approval.Any())
                    //{
                    //    EmployeeInfo.Date = emp.ApplicationDateAm;
                    var name = employees.FirstOrDefault(x => x.EmployeeId == employyee);
                    var currentSalary = currentex
                        .FirstOrDefault(x => x.EmployeeId == employyee && x.isCurrent);
                    if (name != null)
                    {
                        EmployeeInfo.ApplicantName = name.FirstName + " " + name.MiddleName + " " + name.LastName;
                        //EmployeeInfo.ApplicantName =
                        //    name.FirstNameEnglish + "" + name.MiddleNameEnglish + "" + name.LastNameEnglish;
                        if (currentSalary != null)
                        {

                            EmployeeInfo.CurrentSalary = currentSalary.Salary;
                            EmployeeInfo.SalaryInWord =
                                Convert.ToInt32(EmployeeInfo.CurrentSalary).ToWordAmharic();
                            // XAPI.NumberExtensions.toWords(Convert.ToInt32(EmployeeInfo.CurrentSalary));
                        }

                    }

                    EmployeeInfo.Expriance = new List<CurrentEmployeeExpriance>();
                    var empCurrrEx =
                        currentex.Where(x => x.EmployeeId == employyee).OrderByDescending(x => x.PlacementId)
                            .ToList();
                    for (int e = 0; e < empCurrrEx.Count(); e++)
                    {
                        currentEmployeeExpriance = new CurrentEmployeeExpriance();
                        int year = 0;
                        int month;
                        int days;
                        if (e >= 1)
                        {
                            var job = empCurrrEx[e].JobTitleName;
                            var b = empCurrrEx[e].BranchId;
                            var br = db.Branch.FirstOrDefault(x => x.BranchId == b);
                            var branch = "";
                            if (br != null)
                            {
                                branch = br.AmharicName;
                            }
                            var checkJob = EmployeeInfo.Expriance.FirstOrDefault(x => x.JobTitleName == job && x.Branch == branch);
                            if (checkJob != null)
                            {
                                continue;
                            }
                            var assignmetDates = db.Placements.Where(x => x.JobTitleName == job && x.BranchId == b).Select(x => x.AssignmentDate).ToList();
                            var min = assignmetDates.Min();
                            var max = assignmetDates.Max();

                            currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(min.Day, min.Month, min.Year);
                            //currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e].AssignmentDate.Day, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Year);
                            empCurrrEx[e].AssignmentDate.Date.ToShortDateString();
                            currentEmployeeExpriance.To = XAPI.EthiopicDateTime.GetEthiopicDate(max.Day, max.Month, max.Year);
                            //currentEmployeeExpriance.To = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e - 1].AssignmentDate.Day, empCurrrEx[e - 1].AssignmentDate.Month, empCurrrEx[e - 1].AssignmentDate.Year);
                            var fromc = new DateTime(empCurrrEx[e].AssignmentDate.Year, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Day);
                            var toc = new DateTime(empCurrrEx[e - 1].AssignmentDate.Year, empCurrrEx[e - 1].AssignmentDate.Month, empCurrrEx[e - 1].AssignmentDate.Day);

                            var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                            totalmonths += toc.Day < fromc.Day ? -1 : 0;

                            var years = totalmonths / 12;
                            var months = totalmonths % 12;
                            var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                            year = years;
                            month = months;
                            days = dayss;
                        }
                        else
                        {
                            var job = empCurrrEx[e].JobTitleName;
                            var b = empCurrrEx[e].BranchId;
                            var br = db.Branch.FirstOrDefault(x => x.BranchId == b);
                            var branch = "";
                            if (br != null)
                            {
                                branch = br.AmharicName;
                            }
                            var checkJob = EmployeeInfo.Expriance.FirstOrDefault(x => x.JobTitleName == job && x.Branch == branch);
                            if (checkJob != null)
                            {
                                continue;
                            }
                            var assignmetDates = db.Placements.Where(x => x.JobTitleName == job && x.BranchId == b).Select(x => x.AssignmentDate).ToList();
                            var min = assignmetDates.Min();
                            var max = assignmetDates.Max();


                            currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(min.Day, min.Month, min.Year);
                            //currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e].AssignmentDate.Day, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Year);
                            empCurrrEx[e].AssignmentDate.Date.ToShortDateString();
                            currentEmployeeExpriance.To = " አሁን";
                            var fromc = new DateTime(empCurrrEx[e].AssignmentDate.Year, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Day);
                            var toc = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day);

                            var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                            totalmonths += toc.Day < fromc.Day ? -1 : 0;

                            var years = totalmonths / 12;
                            var months = totalmonths % 12;
                            var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                            year = years;
                            month = months;
                            days = dayss;
                        }


                        currentEmployeeExpriance.JobTitleName = empCurrrEx[e].JobTitleName;
                       
                        var bra = db.Branch.FirstOrDefault(x=>x.BranchId == empCurrrEx[e].BranchId);
                        if(bra != null)
                        {
                            currentEmployeeExpriance.Branch = bra.AmharicName;
                        }
                        currentEmployeeExpriance.RecruitmentType = Helpers.HR.HrHelper.GetEmployeeStatus(empCurrrEx[e].EmployementType);
                        currentEmployeeExpriance.Year = XAPI.Common.GetConcatinatedValue(year.ToString(), month.ToString(), days.ToString());
                        TotalYear += year;
                        TotalMonth += month;
                        TotalDays += days;
                        currentEmployeeExpriance.Salary = empCurrrEx[e].Salary;
                        EmployeeInfo.Expriance.Add(currentEmployeeExpriance);
                    }
                    #region History
                    var empHistory = db.PlacementHistories.Where(x => x.EmployeeId == employyee).ToList();
                    foreach (var e in empHistory)
                    {
                        currentEmployeeExpriance = new CurrentEmployeeExpriance();
                        currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(e.From.Day, e.From.Month, e.From.Year);
                        currentEmployeeExpriance.To = XAPI.EthiopicDateTime.GetEthiopicDate(e.To.Day, e.To.Month, e.To.Year);

                        currentEmployeeExpriance.JobTitleName = e.JobTitle;
                        var fromc = new DateTime(e.From.Year, e.From.Month, e.From.Day);
                        var toc = new DateTime(e.To.Year, e.To.Month, e.To.Day);

                        var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                        totalmonths += toc.Day < fromc.Day ? -1 : 0;

                        var years = totalmonths / 12;
                        var months = totalmonths % 12;
                        var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                        int year = years;
                        int month = months;
                        int days = dayss;
                        if (year < 0)
                        {
                            year = 0;
                        }
                        if (month < 0)
                        {
                            month = 0;
                        }
                        if (days < 0)
                        {
                            days = 0;
                        }
                        currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);
                        currentEmployeeExpriance.RecruitmentType = Helpers.HR.HrHelper.GetEmployeeStatus(e.EmployementType);
                        TotalYear += year;
                        TotalMonth += month;
                        TotalDays += days;
                        otherexplist.Salary = e.Salary;
                        EmployeeInfo.Expriance.Add(currentEmployeeExpriance);
                    }
                    #endregion
                    EmployeeInfo.OtherExpriance = new List<OtherEmployeeExpriance>();
                    var empOtherEx = otherexpriances.Where(x => x.EmployeeId == employyee)
                        .Select(x=> new {x.From,x.To,x.Organozation,x.Responsibility,x.Salary })
                        .ToList();
                    foreach (var e in empOtherEx)
                    {
                        otherexplist = new OtherEmployeeExpriance();
                        otherexplist.From = e.From.Date.ToShortDateString();
                        otherexplist.To = e.To.Date.ToShortDateString();
                        otherexplist.CompanyName = e.Organozation;
                        otherexplist.JobTitleName = e.Responsibility;
                        var fromc = new DateTime(e.From.Year, e.From.Month, e.From.Day);
                        var toc = new DateTime(e.To.Year, e.To.Month, e.To.Day);

                        var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                        totalmonths += toc.Day < fromc.Day ? -1 : 0;

                        var years = totalmonths / 12;
                        var months = totalmonths % 12;
                        var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                        int year = years;
                        int month = months;
                        int days = dayss;
                        if (year < 0)
                        {
                            year = 0;
                        }
                        if (month < 0)
                        {
                            month = 0;
                        }
                        if (days < 0)
                        {
                            days = 0;
                        }
                        otherexplist.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);

                        TotalYear += year;
                        TotalMonth += month;
                        TotalDays += days;
                        otherexplist.Salary = e.Salary;
                        EmployeeInfo.OtherExpriance.Add(otherexplist);


                        EmployeeInfoList.Add(EmployeeInfo);
                    }
                }

                else
                {
                    EmployeeInfo.CheckTermination = " አሁን እየተከፈላቸው ያለ ";
                    EmployeeInfo.TerminationMassage = " ይህ የስራ ልምድ እንደ ስራ መልቀቂያ አያገለግልም፡፡";

                    var ExprianceRequest =
                        db.ExprianceRequests.Where(x => x.EmployeeId == employyee).OrderBy(y => y.ApplicationDate)
                            .ToList();

                    if (ExprianceRequest.Any())
                    {



                        var otherexpriances = db.Experiences.ToList();
                        var currentex = db.Placements.ToList();
                        var employees = db.Person_Employee.ToList();


                        var emp = ExprianceRequest.FirstOrDefault();
                        if (emp == null) return new List<EmployeeExperienceLetter>();
                        var approval =
                            db.RequestesStatus.Where(x =>
                                x.ReferenceType == "Experience" && x.RequestId == emp.ExperienceRequestId &&
                                x.Status == RequestStatus.APPROVE).ToList();
                        if (approval.Any())
                        {
                            EmployeeInfo.Date = emp.ApplicationDateAm;
                            var name = employees.FirstOrDefault(x => x.EmployeeId == employyee);
                            var currentSalary = currentex
                                .FirstOrDefault(x => x.EmployeeId == employyee && x.isCurrent);
                            if (name != null)
                            {
                                EmployeeInfo.ApplicantName = name.FirstName + " " + name.MiddleName + " " + name.LastName;
                                //EmployeeInfo.ApplicantName =
                                //    name.FirstNameEnglish + "" + name.MiddleNameEnglish + "" + name.LastNameEnglish;
                                if (currentSalary != null)
                                {

                                    EmployeeInfo.CurrentSalary = currentSalary.Salary;
                                    EmployeeInfo.SalaryInWord =
                                        Convert.ToInt32(EmployeeInfo.CurrentSalary).ToWordAmharic();
                                    // XAPI.NumberExtensions.toWords(Convert.ToInt32(EmployeeInfo.CurrentSalary));
                                }

                            }

                            EmployeeInfo.Expriance = new List<CurrentEmployeeExpriance>();
                            var empCurrrEx =
                                currentex.Where(x => x.EmployeeId == employyee).OrderByDescending(x => x.PlacementId)
                                    .ToList();
                            for (int e = 0; e < empCurrrEx.Count(); e++)
                            {
                                currentEmployeeExpriance = new CurrentEmployeeExpriance();
                                int year = 0;
                                int month;
                                int days;
                                if (e >= 1)
                                {
                                    var job = empCurrrEx[e].JobTitleName;
                                    var b = empCurrrEx[e].BranchId;
                                    var br = db.Branch.FirstOrDefault(x => x.BranchId == b);
                                    var branch = "";
                                    if (br != null)
                                    {
                                        branch = br.AmharicName;
                                    }
                                    var checkJob = EmployeeInfo.Expriance.FirstOrDefault(x => x.JobTitleName == job && x.Branch == branch);
                                    if (checkJob != null)
                                    {
                                        continue;
                                    }
                                    var assignmetDates = db.Placements.Where(x => x.JobTitleName == job && x.BranchId == b).Select(x => x.AssignmentDate).ToList();
                                    var min = assignmetDates.Min();
                                    var max = assignmetDates.Max();

                                    currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(min.Day, min.Month, min.Year);
                                    //currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e].AssignmentDate.Day, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Year);
                                    empCurrrEx[e].AssignmentDate.Date.ToShortDateString();
                                    currentEmployeeExpriance.To = XAPI.EthiopicDateTime.GetEthiopicDate(max.Day, max.Month, max.Year);
                                    //currentEmployeeExpriance.To = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e - 1].AssignmentDate.Day, empCurrrEx[e - 1].AssignmentDate.Month, empCurrrEx[e - 1].AssignmentDate.Year);
                                    var fromc = new DateTime(empCurrrEx[e].AssignmentDate.Year, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Day);
                                    var toc = new DateTime(empCurrrEx[e - 1].AssignmentDate.Year, empCurrrEx[e - 1].AssignmentDate.Month, empCurrrEx[e - 1].AssignmentDate.Day);

                                    var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                                    totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                    var years = totalmonths / 12;
                                    var months = totalmonths % 12;
                                    var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                                    year = years;
                                    month = months;
                                    days = dayss;
                                }
                                else
                                {
                                    var job = empCurrrEx[e].JobTitleName;
                                    var b = empCurrrEx[e].BranchId;
                                    var br = db.Branch.FirstOrDefault(x => x.BranchId == b);
                                    var branch = "";
                                    if (br != null)
                                    {
                                        branch = br.AmharicName;
                                    }
                                    var checkJob = EmployeeInfo.Expriance.FirstOrDefault(x => x.JobTitleName == job && x.Branch == branch);
                                    if (checkJob != null)
                                    {
                                        continue;
                                    }
                                    var assignmetDates = db.Placements.Where(x => x.JobTitleName == job && x.BranchId == b).Select(x => x.AssignmentDate).ToList();
                                    var min = assignmetDates.Min();
                                    var max = assignmetDates.Max();


                                    currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(min.Day, min.Month, min.Year);
                                    //currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(empCurrrEx[e].AssignmentDate.Day, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Year);
                                    empCurrrEx[e].AssignmentDate.Date.ToShortDateString();
                                    currentEmployeeExpriance.To = " አሁን";
                                    var fromc = new DateTime(empCurrrEx[e].AssignmentDate.Year, empCurrrEx[e].AssignmentDate.Month, empCurrrEx[e].AssignmentDate.Day);
                                    var toc = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Date.Day);

                                    var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                                    totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                    var years = totalmonths / 12;
                                    var months = totalmonths % 12;
                                    var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                                    year = years;
                                    month = months;
                                    days = dayss;
                                }


                                currentEmployeeExpriance.JobTitleName = empCurrrEx[e].JobTitleName;
                               // currentEmployeeExpriance.Branch = empCurrrEx[e].Branch.AmharicName;
                                var bra = db.Branch.FirstOrDefault(x => x.BranchId == empCurrrEx[e].BranchId);
                                if (bra != null)
                                {
                                    currentEmployeeExpriance.Branch = bra.AmharicName;
                                }
                                currentEmployeeExpriance.RecruitmentType = Helpers.HR.HrHelper.GetEmployeeStatus(empCurrrEx[e].EmployementType);
                                currentEmployeeExpriance.Year = XAPI.Common.GetConcatinatedValue(year.ToString(), month.ToString(), days.ToString());
                                TotalYear += year;
                                TotalMonth += month;
                                TotalDays += days;
                                currentEmployeeExpriance.Salary = empCurrrEx[e].Salary;
                                EmployeeInfo.Expriance.Add(currentEmployeeExpriance);
                            }
                            #region History
                            //var empHistory = db.PlacementHistories.Where(x => x.EmployeeId == employyee).ToList();
                            //foreach (var e in empHistory)
                            //{
                            //    currentEmployeeExpriance = new CurrentEmployeeExpriance();
                            //    currentEmployeeExpriance.From = XAPI.EthiopicDateTime.GetEthiopicDate(e.From.Day, e.From.Month, e.From.Year);
                            //    currentEmployeeExpriance.To = XAPI.EthiopicDateTime.GetEthiopicDate(e.To.Day, e.To.Month, e.To.Year);

                            //    currentEmployeeExpriance.JobTitleName = e.JobTitle;
                            //    var fromc = new DateTime(e.From.Year, e.From.Month, e.From.Day);
                            //    var toc = new DateTime(e.To.Year, e.To.Month, e.To.Day);

                            //    var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                            //    totalmonths += toc.Day < fromc.Day ? -1 : 0;

                            //    var years = totalmonths / 12;
                            //    var months = totalmonths % 12;
                            //    var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                            //    int year = years;
                            //    int month = months;
                            //    int days = dayss;
                            //    if (year < 0)
                            //    {
                            //        year = 0;
                            //    }
                            //    if (month < 0)
                            //    {
                            //        month = 0;
                            //    }
                            //    if (days < 0)
                            //    {
                            //        days = 0;
                            //    }
                            //    currentEmployeeExpriance.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);
                            //    currentEmployeeExpriance.RecruitmentType = Helpers.HR.HrHelper.GetEmployeeStatus(e.EmployementType);
                            //    TotalYear += year;
                            //    TotalMonth += month;
                            //    TotalDays += days;
                            //    otherexplist.Salary = e.Salary;
                            //    EmployeeInfo.Expriance.Add(currentEmployeeExpriance);
                            //}
                            #endregion
                            EmployeeInfo.OtherExpriance = new List<OtherEmployeeExpriance>();
                            var empOtherEx = otherexpriances.Where(x => x.EmployeeId == employyee).ToList();
                            foreach (var e in empOtherEx)
                            {
                                otherexplist = new OtherEmployeeExpriance();
                                otherexplist.From = e.From.Date.ToShortDateString();
                                otherexplist.To = e.To.Date.ToShortDateString();
                                otherexplist.CompanyName = e.Organozation;
                                otherexplist.JobTitleName = e.Responsibility;
                                var fromc = new DateTime(e.From.Year, e.From.Month, e.From.Day);
                                var toc = new DateTime(e.To.Year, e.To.Month, e.To.Day);

                                var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                                totalmonths += toc.Day < fromc.Day ? -1 : 0;

                                var years = totalmonths / 12;
                                var months = totalmonths % 12;
                                var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;

                                int year = years;
                                int month = months;
                                int days = dayss;
                                if (year < 0)
                                {
                                    year = 0;
                                }
                                if (month < 0)
                                {
                                    month = 0;
                                }
                                if (days < 0)
                                {
                                    days = 0;
                                }
                                otherexplist.Year = XAPI.Common.GetYearMonthDay(e.From, e.To);

                                TotalYear += year;
                                TotalMonth += month;
                                TotalDays += days;
                                otherexplist.Salary = e.Salary;
                                EmployeeInfo.OtherExpriance.Add(otherexplist);
                            }
                            var temp = db.Templates.FirstOrDefault(x=>x.TemplateType == TemplateTypes.ExperianceAm);
                            if(temp != null)
                            {
                                EmployeeInfo.TempOne = temp.TempOne;
                            }

                            EmployeeInfoList.Add(EmployeeInfo);
                        }
                    }
                }
            }
            

            return EmployeeInfoList;
        }


    }

}
