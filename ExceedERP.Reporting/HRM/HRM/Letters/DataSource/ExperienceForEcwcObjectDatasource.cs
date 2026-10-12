using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.Letters.ViewModel;
using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ExceedERP.Reporting.HRM.HRM.Letters.DataSource
{
    [DataObject]
    public class ExperienceForEcwcObjectDatasource : IDisposable
    {
        private ExceedDbContext _db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();
        private object placeWorkedBuilder;

        [DataObjectMethod(DataObjectMethodType.Select)]
        public ExperienceVM GetTransferForEcwc(string empId)
        {
            ExperienceVM EmployeeInfo = new ExperienceVM();
            if (!string.IsNullOrEmpty(empId))
            {
                int.TryParse(empId, out int employeeId);
                var employee = _db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employeeId && x.IsActive);
                if (employee != null)
                {
                    EmployeeInfo.Sex = HrHelper.GetGender(employee.Gender);

                    EmployeeInfo.EmployeeCode =  employee.EmployeeCode;
                    if (employee.Gender == Gender.FEMALE)
                    {
                        EmployeeInfo.FullName = "W/ro "+employee.FullNameaEnglish;

                    }
                    if (employee.Gender == Gender.MALE)
                    {
                        EmployeeInfo.FullName = "Ato "+employee.FullNameaEnglish;

                    }
                    if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                    {
                        if (employee.Gender == Gender.FEMALE)
                        {
                            EmployeeInfo.FullName = " ወ/ሮ " + employee.FullNameAmharic;

                        }
                        if (employee.Gender == Gender.MALE)
                        {
                            EmployeeInfo.FullName = " አቶ " + employee.FullNameAmharic;

                        }
                    }
                        var placements = _db.Placements.Where(x => x.EmployeeId == employee.EmployeeId).OrderBy(x=>x.PlacementId);
                    var placement = placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == employee.EmployeeId);
                    if (placement != null)
                    {
                        
                        var lookup = _db.Lookups.FirstOrDefault(x => x.LookupId == placement.LookupId);
                        if (lookup != null)
                        {
                            EmployeeInfo.JobTitle = lookup.Description;
                        }
                        EmployeeInfo.Salary = placement.Salary;
                        var department = _db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == placement.OrgStructureId);
                        if (department != null)
                        {
                            EmployeeInfo.WorkUnit = department.Name;
                        }
                        EmployeeInfo.Salary = placement.Salary;

                        var branch = _db.Branch.FirstOrDefault(x => x.BranchId == placement.BranchId);
                        var individualPlacements = _db.Placements.Where(x => x.EmployeeId == placement.EmployeeId).OrderBy(x => x.AssignmentDate).ToList();
                        var placementHistories = _db.PlacementHistories.Where(x => x.EmployeeId == placement.EmployeeId).OrderByDescending(x => x.From).ToList();
                        DateTime today = DateTime.Now;
                        var fromdatestr = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                        bool isCurrent = false;
                        int i = 0;
                        string PlaceWorkedInternal = "";
                        if (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                        {

                            foreach (var placementHistory in placementHistories)
                            {
                                string jobTitleN = placementHistory.JobTitle;
                                if (string.IsNullOrEmpty(jobTitleN))
                                {
                                    var jobT = _db.Lookups.Find(placementHistory.LookupId);
                                    if(jobT != null)
                                    {
                                        jobTitleN = jobT.Description;
                                    }
                                }
                                PlaceWorkedInternal += "<br/> + ከ: " + placementHistory.FromAmharic + " " + "እስከ:" + placementHistory.ToAmharic + "  " + jobTitleN + "<br/>";

                            }

                            foreach (var perPl in individualPlacements)
                            {
                                var currentPlacement = _db.Placements.Find(perPl.PlacementId);
                                if (currentPlacement != null && currentPlacement.isCurrent)
                                {
                                    isCurrent = true;
                                }
                                if (isCurrent)
                                {
                                    PlaceWorkedInternal += "<br/> + ከ: " + perPl.AssignmentDateAmharic + " " + "እስከ: አሁን" + " " + perPl.JobTitleName + "<br/>";
                                    fromdatestr = perPl.AssignmentDateAmharic;
                                }
                                else
                                {
                                    i++;
                                    var termination = _db.Terminations.Where(x => x.IsOnlineApproved && x.EmployeeId == perPl.EmployeeId).OrderByDescending(x => x.TerminationDate).FirstOrDefault();
                                    if (termination != null && termination.TerminationDate > perPl.AssignmentDate)
                                    {
                                        if (i == 1)
                                            fromdatestr = termination.TerminationDateAm;

                                        PlaceWorkedInternal += "<br/>+ ከ: " + perPl.AssignmentDateAmharic + " " + "እስከ:" + fromdatestr + " " + perPl.JobTitleName + "<br/>";
                                        fromdatestr = perPl.AssignmentDateAmharic;
                                    }
                                    else
                                    {
                                        if (i == 1)
                                            fromdatestr = "-";

                                        PlaceWorkedInternal += " <br/> + ከ: " + perPl.AssignmentDateAmharic + " " + "እስከ:" + fromdatestr + " " + perPl.JobTitleName + "<br/>";
                                        fromdatestr = perPl.AssignmentDateAmharic;
                                    }

                                }

                            }
                            EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWordsAmharic((double)EmployeeInfo.Salary);

                        }
                        if (cultureName.ToLower() == "om")
                        {

                            foreach (var placementHistory in placementHistories)
                            {
                                string jobTitleN = placementHistory.JobTitle;
                                if (string.IsNullOrEmpty(jobTitleN))
                                {
                                    var jobT = _db.Lookups.Find(placementHistory.LookupId);
                                    if (jobT != null)
                                    {
                                        jobTitleN = jobT.Description;
                                    }
                                }
                                PlaceWorkedInternal += "<br/> + Irraa: " + placementHistory.FromAmharic + " " + "Gara:" + placementHistory.ToAmharic + " " + jobTitleN + "<br/>";

                            }
                            foreach (var perPl in individualPlacements)
                            {

                                //PlaceWorkedInternal += "\n  Irraa: " + perPl.AssignmentDateAmharic + " " + "Gara:" + fromdatestr + " " + perPl.JobTitleName + "\n";
                                //fromdatestr = perPl.AssignmentDateAmharic;
                                var currentPlacement = _db.Placements.Find(perPl.PlacementId);
                                if (currentPlacement != null && currentPlacement.isCurrent)
                                {
                                 
                                    PlaceWorkedInternal += "<br/>+ Irraa: " + perPl.AssignmentDateAmharic + " " + "Gara:amma"  + " " + perPl.JobTitleName + "<br/>";
                                    fromdatestr = perPl.AssignmentDateAmharic;
                                }
                                else
                                {
                                    i++;
                                    var termination = _db.Terminations.Where(x => x.IsOnlineApproved && x.EmployeeId == perPl.EmployeeId).OrderByDescending(x => x.TerminationDate).FirstOrDefault();
                                    if (termination != null && termination.TerminationDate > perPl.AssignmentDate)
                                    {
                                        if (i == 1)
                                            fromdatestr = termination.TerminationDateAm;

                                        PlaceWorkedInternal += "<br/> + Irraa: " + perPl.AssignmentDateAmharic + " " + "Gara:" + fromdatestr + " " + perPl.JobTitleName + "<br/>";
                                        fromdatestr = perPl.AssignmentDateAmharic;
                                    }
                                    else
                                    {
                                        if (i == 1)
                                            fromdatestr = "-";

                                        PlaceWorkedInternal += "<br/> + Irraa: " + perPl.AssignmentDateAmharic + " " + "Gara:" + fromdatestr + " " + perPl.JobTitleName + "<br/>";
                                        fromdatestr = perPl.AssignmentDateAmharic;
                                    }

                                }
                            }
                            EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords((double)EmployeeInfo.Salary);

                        }
                        if (cultureName.ToLower() == "en" || cultureName.ToLower() == "en-us")
                        {
                            EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords((double)EmployeeInfo.Salary);
                            foreach (var placementHistory in placementHistories)
                            {
                                string jobTitleN = placementHistory.JobTitle;
                                if (string.IsNullOrEmpty(jobTitleN))
                                {
                                    var jobT = _db.Lookups.Find(placementHistory.LookupId);
                                    if (jobT != null)
                                    {
                                        jobTitleN = jobT.Description;
                                    }
                                }
                                PlaceWorkedInternal += "<br/> + From: " + placementHistory.FromAmharic + " " + "to:" + placementHistory.ToAmharic + " " +jobTitleN + "<br/>";

                            }
                            foreach (var perPl in individualPlacements)
                            {

                                //PlaceWorkedInternal += "\n  From: " + perPl.AssignmentDateAmharic + " " + "to:" + fromdatestr + " " + perPl.JobTitleName + "\n";
                                //fromdatestr = perPl.AssignmentDateAmharic;
                                var currentPlacement = _db.Placements.Find(perPl.PlacementId);
                                if (currentPlacement != null && currentPlacement.isCurrent)
                                {
                                 
                                    PlaceWorkedInternal += "<br/> + From: " + perPl.AssignmentDate.ToString("dd.MM.yyy") + " " + "to: now  as "   +perPl.JobTitleName + "<br/>";
                                    fromdatestr = perPl.AssignmentDate.ToString("dd.MM.yyy");
                                }
                                else
                                {
                                    i++;
                                    var termination = _db.Terminations.Where(x => x.IsOnlineApproved && x.EmployeeId == perPl.EmployeeId).OrderByDescending(x => x.TerminationDate).FirstOrDefault();
                                    if (termination != null && termination.TerminationDate > perPl.AssignmentDate)
                                    {
                                        if (i == 1)
                                            fromdatestr = termination.TerminationDateAm;

                                        PlaceWorkedInternal += "<br/>+  From: " + perPl.AssignmentDateAmharic + " " + "to:" + fromdatestr + " " + perPl.JobTitleName + "<br/>";
                                        fromdatestr = perPl.AssignmentDate.ToString("dd.MM.yyy");
                                    }
                                    else
                                    {
                                        if (i == 1)
                                            fromdatestr = "-";

                                        PlaceWorkedInternal += "<br/> + From: " + perPl.AssignmentDate.ToString("dd.MM.yyy") + " " + "to:" + fromdatestr + " " + perPl.JobTitleName + "<br/>";
                                        fromdatestr = perPl.AssignmentDate.ToString("dd.MM.yyy");
                                    }

                                }
                            }

                        }
                        
                        EmployeeInfo.Expereince = PlaceWorkedInternal;
                      
                    }
                }
            }

            return EmployeeInfo;
           
        }
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
