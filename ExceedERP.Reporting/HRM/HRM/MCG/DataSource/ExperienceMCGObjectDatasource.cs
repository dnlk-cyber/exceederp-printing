using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.Letters.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ExceedERP.Reporting.HRM.HRM.MCG.DataSource
{
    [DataObject]
    public class ExperienceMCGObjectDatasource : IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();
        private object placeWorkedBuilder;

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ExperienceVM GetExperienceMCG(string empId)
        {
            ExperienceVM EmployeeInfo = new ExperienceVM();

            if (string.IsNullOrEmpty(empId))
                return EmployeeInfo;

            if (!int.TryParse(empId, out int employeeId))
                return EmployeeInfo;

            var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);
            if (employee == null)
                employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId);

            if (employee == null)
                return EmployeeInfo;

            // Helper to escape HTML special characters
            string EscapeHtml(string input) =>
                string.IsNullOrEmpty(input) ? "" :
                System.Web.HttpUtility.HtmlEncode(input);

            // Set gender and name based on culture
            EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
            EmployeeInfo.EmployeeCode = employee.EmployeeCode;

            bool isAmharic = cultureName?.ToLower() == "am" || cultureName?.ToLower() == "am-et";
            bool isOromo = cultureName?.ToLower() == "om";
            bool isEnglish = !isAmharic && !isOromo; // default English

            // FullName logic
            if (isAmharic)
                EmployeeInfo.FullName = employee.FullNameAmharic;
            else
                EmployeeInfo.FullName = employee.FullNameaEnglish;

            // Get current placement
            var placements = _db.Placements
                .Where(x => x.EmployeeId == employee.EmployeeId)
                .OrderBy(x => x.PlacementId);

            var placement = placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId);
            if (placement == null)
                placement = placements.Where(x => x.EmployeeId == employee.EmployeeId)
                                      .OrderByDescending(x=>x.AssignmentDate)
                                      .ThenByDescending(x=>x.PlacementId).FirstOrDefault();

            if (placement == null)
                return EmployeeInfo;

            // Job title
            var lookup = _db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
            EmployeeInfo.JobTitle = lookup?.Description ?? "";
            //
            var branch = _db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
            EmployeeInfo.Branch = branch?.Name ?? "";
            //
            var termination = _db.Terminations.Where(x => x.EmployeeId == employee.EmployeeId&&(x.IsOnlineApproved||x.IsForciblyTerminated)).OrderByDescending(x => x.TerminationDate).ThenByDescending(x => x.TerminationId).FirstOrDefault();
            string gorCal = "";
            string EthCal = "";
            if (isAmharic)
            {
                gorCal = "እ.እ.አ";
                EthCal = "ዓ.ም";
            }
            else
            {
                gorCal = "G.C";
                EthCal = "E.C";
            }
            EmployeeInfo.TerminationDate = termination?.TerminationDate != null? $"{termination.TerminationDate.Value:MMMM dd, yyyy} {gorCal} / {termination.TerminationDateAm ?? ""} {EthCal}/": "";
            // Department / WorkUnit
            var department = _db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
            EmployeeInfo.WorkUnit = department?.Name ?? "";

            EmployeeInfo.Earnings = "Monthly Gross Salary";
            EmployeeInfo.Salary = placement.Salary;

            // Build experience history (HTML)
            var placeWorkedBuilder = new StringBuilder();

            var placementHistories = _db.PlacementHistories
                .Where(x => x.EmployeeId == placement.EmployeeId)
                .OrderByDescending(x => x.From)
                .ToList();

            var individualPlacements = _db.Placements
                .Where(x => x.EmployeeId == placement.EmployeeId)
                .OrderBy(x => x.AssignmentDate)
                .ToList();

            DateTime today = DateTime.Now;
            string fromDateStr = "";

            // Localized labels
            string labelFrom, labelTo, labelNow, labelDash;
            if (isAmharic)
            {
                labelFrom = "ከ:";
                labelTo = "እስከ:";
                labelNow = "አሁን";
                labelDash = "-";
            }
            else if (isOromo)
            {
                labelFrom = "Irraa:";
                labelTo = "Gara:";
                labelNow = "amma";
                labelDash = "-";
            }
            else // English
            {
                labelFrom = "From:";
                labelTo = "to:";
                labelNow = "now";
                labelDash = "-";
            }

            // Function to get job title from history or placement
            string GetJobTitle(object source)
            {
                if (source is PlacementHistory ph && !string.IsNullOrEmpty(ph.JobTitle))
                    return EscapeHtml(ph.JobTitle);
                if (source is Placement p && !string.IsNullOrEmpty(p.JobTitleName))
                    return EscapeHtml(p.JobTitleName);
                if (source is PlacementHistory ph2 && ph2.LookupId != null)
                {
                    var jt = _db.Lookups.Find(ph2.LookupId);
                    return EscapeHtml(jt?.Description);
                }
                if (source is Placement p2 && p2.LookupId != null)
                {
                    var jt = _db.Lookups.Find(p2.LookupId);
                    return EscapeHtml(jt?.Description);
                }
                return "";
            }

            // Process placement histories
            foreach (var history in placementHistories)
            {
                string fromDate = isAmharic || isOromo ? history.FromAmharic : history.From.ToString("dd.MM.yyyy");
                string toDate = isAmharic || isOromo ? history.ToAmharic : history.To.ToString("dd.MM.yyyy");
                string jobTitle = GetJobTitle(history);
                placeWorkedBuilder.Append($"<br/> + {labelFrom} {fromDate} {labelTo} {toDate} {jobTitle}<br/>");
            }

            // Process individual placements (current and terminated)
            int index = 0;
            foreach (var perPl in individualPlacements)
            {
                bool isCurrentPlacement = perPl.isCurrent;
                string assignmentDate = (isAmharic || isOromo) ? perPl.AssignmentDateAmharic :
                                        perPl.AssignmentDate.ToString("dd.MM.yyyy");
                string jobTitle = GetJobTitle(perPl);

                if (isCurrentPlacement)
                {
                    // Current placement: "from ... to now"
                    placeWorkedBuilder.Append($"<br/> + {labelFrom} {assignmentDate} {labelTo} {labelNow}  {jobTitle}<br/>");
                    fromDateStr = assignmentDate;
                }
                else
                {
                    // Terminated or past placement
                    var terminationn = _db.Terminations
                        .Where(x => x.IsOnlineApproved && x.EmployeeId == perPl.EmployeeId && x.TerminationDate > perPl.AssignmentDate)
                        .OrderByDescending(x => x.TerminationDate)
                        .FirstOrDefault();

                    string toDate;
                    if (terminationn != null)
                    {
                        toDate = (isAmharic || isOromo) ? terminationn.TerminationDateAm :
                                 terminationn?.TerminationDate.Value.ToString("dd.MM.yyyy");
                        if (index == 0) fromDateStr = toDate;
                    }
                    else
                    {
                        toDate = (index == 0) ? labelDash : fromDateStr;
                    }

                    placeWorkedBuilder.Append($"<br/> + {labelFrom} {assignmentDate} {labelTo} {toDate} {jobTitle}<br/>");
                    fromDateStr = assignmentDate;
                }
                index++;
            }

            EmployeeInfo.Expereince = placeWorkedBuilder.ToString();

            // Salary in words
            if (EmployeeInfo.Salary>0)
            {
                double salaryValue = (double)EmployeeInfo.Salary;
                if (isAmharic)
                    EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWordsAmharic(salaryValue);
                else
                    EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords(salaryValue);
            }
            else
            {
                EmployeeInfo.SalaryInWord = "";
            }

            // Allowances
            EmployeeInfo.EmployeeAllowances = new List<EmployeeAllowanceVM>();
            var employeeAllowances = _db.AllowancePerEmployees
                .Where(x => x.EmployeeId == employeeId && !x.Pend)
                .ToList();

            foreach (var allowance in employeeAllowances)
            {
                var allowanceDef = _db.AllowanceDefinations
                    .FirstOrDefault(x => x.AllowanceDefinationId == allowance.AllowanceDefinationId);
                if (allowanceDef == null) continue;

                string allowanceName = EscapeHtml(allowanceDef?.AllowanceName ?? "");
                string allowanceType = allowanceDef?.AllowanceType.ToString() ?? "";
                string earnings = $"{allowanceName} {allowanceType}".Trim();
                decimal amount = allowanceDef?.Amount ?? 0;

                EmployeeInfo.EmployeeAllowances.Add(new EmployeeAllowanceVM
                {
                    Earnings = earnings,
                    Amounts = amount
                });
            }

            return EmployeeInfo;
        }
        //public ExperienceVM GetTransferForEcwc(string empId)
        //{
        //    ExperienceVM EmployeeInfo = new ExperienceVM();
        //    if (!string.IsNullOrEmpty(empId))
        //    {
        //        int.TryParse(empId, out int employeeId);
        //        var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);
        //        if (employee != null)
        //        {
        //            EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);

        //            EmployeeInfo.EmployeeCode =  employee.EmployeeCode;
        //            if (employee.Gender == Gender.FEMALE)
        //            {
        //                EmployeeInfo.FullName = employee.FullNameaEnglish;

        //            }
        //            if (employee.Gender == Gender.MALE)
        //            {
        //                EmployeeInfo.FullName = employee.FullNameaEnglish;

        //            }
        //            if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
        //            {
        //                if (employee.Gender == Gender.FEMALE)
        //                {
        //                    EmployeeInfo.FullName =  employee.FullNameAmharic;

        //                }
        //                if (employee.Gender == Gender.MALE)
        //                {
        //                    EmployeeInfo.FullName = employee.FullNameAmharic;

        //                }
        //            }

        //            var placements = _db.Placements.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x=>x.PlacementId);
        //            var placement = placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId);
        //            if (placement != null)
        //            {

        //                var lookup = _db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
        //                if (lookup != null)
        //                {
        //                    EmployeeInfo.JobTitle = lookup.Description;
        //                }
        //                EmployeeInfo.Salary = placement.Salary;
        //                var department = _db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
        //                if (department != null)
        //                {
        //                    EmployeeInfo.WorkUnit = department.Name;
        //                }
        //                EmployeeInfo.Earnings = "Monthly Gross Salary";
        //                EmployeeInfo.Salary = placement.Salary;

        //                var branch = _db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
        //                var individualPlacements = _db.Placements.Where(x => x.EmployeeId == placement.EmployeeId).OrderBy(x => x.AssignmentDate).ToList();
        //                var placementHistories = _db.PlacementHistories.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.From).ToList();
        //                DateTime today = DateTime.Now;
        //                var fromdatestr = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
        //                bool isCurrent = false;
        //                int i = 0;
        //                string PlaceWorkedInternal = "";
        //                if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
        //                {

        //                    foreach (var placementHistory in placementHistories)
        //                    {
        //                        string jobTitleN = placementHistory.JobTitle;
        //                        if (string.IsNullOrEmpty(jobTitleN))
        //                        {
        //                            var jobT = _db.Lookups.Find(placementHistory.LookupId);
        //                            if(jobT != null)
        //                            {
        //                                jobTitleN = jobT.Description;
        //                            }
        //                        }
        //                        PlaceWorkedInternal += "<br/> + ከ: " + placementHistory.FromAmharic + " " + "እስከ:" + placementHistory.ToAmharic + "  " + jobTitleN + "<br/>";

        //                    }

        //                    foreach (var perPl in individualPlacements)
        //                    {
        //                        var currentPlacement = _db.Placements.Find(perPl.PlacementId);
        //                        if (currentPlacement != null && currentPlacement.isCurrent)
        //                        {
        //                            isCurrent = true;
        //                        }
        //                        if (isCurrent)
        //                        {
        //                            PlaceWorkedInternal += "<br/> + ከ: " + perPl.AssignmentDateAmharic + " " + "እስከ: አሁን" + " " + perPl.JobTitleName + "<br/>";
        //                            fromdatestr = perPl.AssignmentDateAmharic;
        //                        }
        //                        else
        //                        {
        //                            i++;
        //                            var termination = _db.Terminations.Where(x => x.IsOnlineApproved && x.EmployeeId == perPl.EmployeeId).OrderByDescending(x => x.TerminationDate).FirstOrDefault();
        //                            if (termination != null && termination.TerminationDate > perPl.AssignmentDate)
        //                            {
        //                                if (i == 1)
        //                                    fromdatestr = termination.TerminationDateAm;

        //                                PlaceWorkedInternal += "<br/>+ ከ: " + perPl.AssignmentDateAmharic + " " + "እስከ:" + fromdatestr + " " + perPl.JobTitleName + "<br/>";
        //                                fromdatestr = perPl.AssignmentDateAmharic;
        //                            }
        //                            else
        //                            {
        //                                if (i == 1)
        //                                    fromdatestr = "-";

        //                                PlaceWorkedInternal += " <br/> + ከ: " + perPl.AssignmentDateAmharic + " " + "እስከ:" + fromdatestr + " " + perPl.JobTitleName + "<br/>";
        //                                fromdatestr = perPl.AssignmentDateAmharic;
        //                            }

        //                        }

        //                    }
        //                    EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWordsAmharic((double)EmployeeInfo.Salary);

        //                }
        //                if (cultureName.ToLower() == "om")
        //                {

        //                    foreach (var placementHistory in placementHistories)
        //                    {
        //                        string jobTitleN = placementHistory.JobTitle;
        //                        if (string.IsNullOrEmpty(jobTitleN))
        //                        {
        //                            var jobT = _db.Lookups.Find(placementHistory.LookupId);
        //                            if (jobT != null)
        //                            {
        //                                jobTitleN = jobT.Description;
        //                            }
        //                        }
        //                        PlaceWorkedInternal += "<br/> + Irraa: " + placementHistory.FromAmharic + " " + "Gara:" + placementHistory.ToAmharic + " " + jobTitleN + "<br/>";

        //                    }
        //                    foreach (var perPl in individualPlacements)
        //                    {

        //                        //PlaceWorkedInternal += "\n  Irraa: " + perPl.AssignmentDateAmharic + " " + "Gara:" + fromdatestr + " " + perPl.JobTitleName + "\n";
        //                        //fromdatestr = perPl.AssignmentDateAmharic;
        //                        var currentPlacement = _db.Placements.Find(perPl.PlacementId);
        //                        if (currentPlacement != null && currentPlacement.isCurrent)
        //                        {

        //                            PlaceWorkedInternal += "<br/>+ Irraa: " + perPl.AssignmentDateAmharic + " " + "Gara:amma"  + " " + perPl.JobTitleName + "<br/>";
        //                            fromdatestr = perPl.AssignmentDateAmharic;
        //                        }
        //                        else
        //                        {
        //                            i++;
        //                            var termination = _db.Terminations.Where(x => x.IsOnlineApproved && x.EmployeeId == perPl.EmployeeId).OrderByDescending(x => x.TerminationDate).FirstOrDefault();
        //                            if (termination != null && termination.TerminationDate > perPl.AssignmentDate)
        //                            {
        //                                if (i == 1)
        //                                    fromdatestr = termination.TerminationDateAm;

        //                                PlaceWorkedInternal += "<br/> + Irraa: " + perPl.AssignmentDateAmharic + " " + "Gara:" + fromdatestr + " " + perPl.JobTitleName + "<br/>";
        //                                fromdatestr = perPl.AssignmentDateAmharic;
        //                            }
        //                            else
        //                            {
        //                                if (i == 1)
        //                                    fromdatestr = "-";

        //                                PlaceWorkedInternal += "<br/> + Irraa: " + perPl.AssignmentDateAmharic + " " + "Gara:" + fromdatestr + " " + perPl.JobTitleName + "<br/>";
        //                                fromdatestr = perPl.AssignmentDateAmharic;
        //                            }

        //                        }
        //                    }
        //                    EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords((double)EmployeeInfo.Salary);

        //                }
        //                if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
        //                {
        //                    EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords((double)EmployeeInfo.Salary);
        //                    foreach (var placementHistory in placementHistories)
        //                    {
        //                        string jobTitleN = placementHistory.JobTitle;
        //                        if (string.IsNullOrEmpty(jobTitleN))
        //                        {
        //                            var jobT = _db.Lookups.Find(placementHistory.LookupId);
        //                            if (jobT != null)
        //                            {
        //                                jobTitleN = jobT.Description;
        //                            }
        //                        }
        //                        PlaceWorkedInternal += "<br/> + From: " + placementHistory.FromAmharic + " " + "to:" + placementHistory.ToAmharic + " " +jobTitleN + "<br/>";

        //                    }
        //                    foreach (var perPl in individualPlacements)
        //                    {

        //                        //PlaceWorkedInternal += "\n  From: " + perPl.AssignmentDateAmharic + " " + "to:" + fromdatestr + " " + perPl.JobTitleName + "\n";
        //                        //fromdatestr = perPl.AssignmentDateAmharic;
        //                        var currentPlacement = _db.Placements.Find(perPl.PlacementId);
        //                        if (currentPlacement != null && currentPlacement.isCurrent)
        //                        {

        //                            PlaceWorkedInternal += "<br/> + From: " + perPl.AssignmentDate.ToString("dd.MM.yyy") + " " + "to: now  as "   +perPl.JobTitleName + "<br/>";
        //                            fromdatestr = perPl.AssignmentDate.ToString("dd.MM.yyy");
        //                        }
        //                        else
        //                        {
        //                            i++;
        //                            var termination = _db.Terminations.Where(x => x.IsOnlineApproved && x.EmployeeId == perPl.EmployeeId).OrderByDescending(x => x.TerminationDate).FirstOrDefault();
        //                            if (termination != null && termination.TerminationDate > perPl.AssignmentDate)
        //                            {
        //                                if (i == 1)
        //                                    fromdatestr = termination.TerminationDateAm;

        //                                PlaceWorkedInternal += "<br/>+  From: " + perPl.AssignmentDateAmharic + " " + "to:" + fromdatestr + " " + perPl.JobTitleName + "<br/>";
        //                                fromdatestr = perPl.AssignmentDate.ToString("dd.MM.yyy");
        //                            }
        //                            else
        //                            {
        //                                if (i == 1)
        //                                    fromdatestr = "-";

        //                                PlaceWorkedInternal += "<br/> + From: " + perPl.AssignmentDate.ToString("dd.MM.yyy") + " " + "to:" + fromdatestr + " " + perPl.JobTitleName + "<br/>";
        //                                fromdatestr = perPl.AssignmentDate.ToString("dd.MM.yyy");
        //                            }

        //                        }
        //                    }

        //                }
        //                EmployeeInfo.EmployeeAllowances = new List<EmployeeAllowanceVM>();
        //                var employeeAllowances = _db.AllowancePerEmployees.Where(x => x.EmployeeId == employeeId && !x.Pend).ToList();
        //                foreach(var allowance in employeeAllowances)
        //                {
        //                    var currentEmployeeExpriance = new EmployeeAllowanceVM();

        //                    var allowanceDef = _db.AllowanceDefinations.FirstOrDefault(x => x.AllowanceDefinationId == allowance.AllowanceDefinationId);

        //                    currentEmployeeExpriance.Earnings = allowanceDef?.AllowanceName+" "+ allowanceDef?.AllowanceType.ToString();
        //                    currentEmployeeExpriance.Amounts = allowanceDef?.Amount??0;



        //                    EmployeeInfo.EmployeeAllowances.Add(currentEmployeeExpriance);
        //                }
        //                EmployeeInfo.Expereince = PlaceWorkedInternal;

        //            }
        //        }
        //    }

        //    return EmployeeInfo;

        //}
        public void Dispose()
        {
            _db.Dispose();
        }
    }
    //[DataObject]
    //public class ContractHirement1ObjectDatasource : ContractHirementVM
    //{
    //    private ExceedDbContext _db = new ExceedDbContext();
    //    [DataObjectMethod(DataObjectMethodType.Select)]
    //    public ContractHirementVM GetContractHirement(string empId)
    //    {

    //        ContractHirementVM EmployeeInfo = new ContractHirementVM();
    //        if (empId != null)
    //        {
    //            int.TryParse(empId, out int employeeId);
    //            var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);
    //            if (employee != null)
    //            {
    //                EmployeeInfo = new ContractHirementVM();
    //                EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);
    //                EmployeeInfo.FullName = employee.EmployeeCode+"-"+employee.FullNameAmharic;
    //                var placement = _db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId 
    //                                                                    && x.EmployementType == EmployeeStatus.CONTRACT);
    //                if (placement != null)
    //                {
    //                    var rank = _db.Ranks.FirstOrDefault(x => x.RankId == placement.RankId);
    //                    if (rank != null)
    //                    {
    //                        EmployeeInfo.Rank = rank.RankName;
    //                    }
    //                    var lookup = _db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
    //                    if (lookup != null)
    //                    {
    //                        EmployeeInfo.JobTitle = lookup.Description;
    //                    }
    //                    EmployeeInfo.Salary = placement.Salary;
    //                    var department = _db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
    //                    if (department != null)
    //                    {
    //                        EmployeeInfo.WorkUnit = department.Name;
    //                    }
    //                    var branch = _db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
    //                    if (branch != null)
    //                    {
    //                        EmployeeInfo.Branch = branch.Name;
    //                    }
    //                    EmployeeInfo.AssignmentDate = placement.AssignmentDate;
    //                    EmployeeInfo.AssignmentDateAmh = placement.AssignmentDateAmharic;
    //                    EmployeeInfo.Salary = placement.Salary;
    //                    EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords((double)EmployeeInfo.Salary);

    //                }

    //            }
    //        }
    //        return EmployeeInfo;
    //    }

    //}
}
