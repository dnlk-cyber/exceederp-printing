using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeExperienceObjectDataSource : List<EmployeeExperience>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeExperience> GetEmployeeExperience(string dept, string branchId, string id, string type, string fromE, string toE)
        {
            List<EmployeeExperience> EmployeeInfoList = new List<EmployeeExperience>();
            EmployeeExperience EmployeeInfo = new EmployeeExperience();
            EmployeeInfo.EmployeeExperienceList = new List<EmployeeExperienceList>();
            db = new ExceedDbContext();
            var branches = db.Branch.ToList();
            db = new ExceedDbContext();
            var jobs = db.Lookups.ToList();
            db = new ExceedDbContext();
            var providers = db.Companies.Where(x => x.Category == "1").ToList();
            db = new ExceedDbContext();

            var placementHistories = db.PlacementHistories.ToList();
           // var placementHistories = db.Placements.ToList();

            var placements = db.Placements.ToList();
            List<Person_Employee> employees;
            if (branchId != null && dept != null)
            {
                employees = EmployeeHelper.GetEmployeesByBranchAndWorkUnit(Convert.ToInt32(branchId), Convert.ToInt32(dept)).ToList();
            }
            else if (branchId != null && dept == null)
            {
                employees = EmployeeHelper.GetEmployeesByBranch(Convert.ToInt32(branchId)).ToList();
            }
            else if (branchId == null && dept != null)
            {
                employees = EmployeeHelper.GetEmployeesByWorkUnit(Convert.ToInt32(dept)).ToList();
            }
            else
            {
                employees = db.Person_Employee.Where(x => x.IsActive).ToList();
            }

            if (id != null)
            {
                employees = employees.Where(x => x.EmployeeId.ToString() == id).ToList();
            }
            List<int> empColList = new List<int>();
            double checkTotalExp = 0;

            if (fromE != null || toE != null)
            {
                foreach (var emp in employees)
                {
                    var exps = db.Experiences.Where(x => x.EmployeeId == emp.EmployeeId).ToList();
                    foreach (var ex in exps)
                    {
                        checkTotalExp = checkTotalExp + (ex.To.Subtract(ex.From).Days);
                    }
                    checkTotalExp = checkTotalExp + (DateTime.Today.Subtract(emp.HiredDate).Days);

                    int check = (int)checkTotalExp / 365;
                    if (check > 0)
                    {
                        int fr = -1, tt = -1;
                        //if (int.Parse(fromE.ToString()).GetType().Equals(typeof(int)))
                        //{
                        //     fr = Convert.ToInt32(fromE);
                        //     tt = Convert.ToInt32(toE);
                        //}
                        if (fromE != null)
                        {
                            foreach (char c in fromE)
                            {
                                if (char.IsDigit(c))
                                    fr = Convert.ToInt32(fromE);
                            }
                        }
                        if (toE != null)
                        {
                            foreach (char c in toE)
                            {
                                if (char.IsDigit(c))
                                    tt = Convert.ToInt32(toE);
                            }
                        }

                        if (fromE != null && toE != null)
                        {
                            if (check >= fr && check <= tt)
                            {

                                empColList.Add(emp.EmployeeId);
                            }
                        }
                        if (fromE != null && toE == null)
                        {
                            if (check >= fr)
                            {

                                empColList.Add(emp.EmployeeId);
                            }
                        }
                        if (fromE == null && toE != null)
                        {
                            if (check <= tt)
                            {
                                empColList.Add(emp.EmployeeId);
                            }
                        }

                        checkTotalExp = 0;
                    }
                }

                placements = placements.Where(x => empColList.Contains(x.EmployeeId)).ToList();
            }

            #region new code - get report data
            var externalExperiences = db.Experiences.ToList();

            placements = placements.Where(x => x.isCurrent).ToList();
            foreach (var pl in placements)
            {
                //foreach (var e in employees.Where(x => x.EmployeeId == pl.EmployeeId && x.IsActive))
                //{
                var e = employees.FirstOrDefault(x => x.EmployeeId == pl.EmployeeId && x.IsActive);
                if (e != null)
                {


                    EmployeeInfo = new EmployeeExperience();
                    EmployeeInfo.EmployeeId = e.EmployeeCode;
                    if (e.Gender == 0)
                    {
                        EmployeeInfo.Sex = "ወንድ";
                    }
                    else
                    {
                        EmployeeInfo.Sex = "ሴት";
                    }
                    EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                    //EmployeeInfo.FullName = e.FirstNameEnglish + "" + e.MiddleNameEnglish + "" + e.LastNameEnglish;


                    
                        var deptt = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == pl.OrgStructureId);
                        if (deptt != null)
                        {
                            EmployeeInfo.Departement = deptt.Name;
                        }
                    
                    double totalExperience = 0;



                    if (type == null || type == "External")
                    {
                        var empexterExp = externalExperiences.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                        foreach (var experience in empexterExp)
                        {
                            var experienceList = new EmployeeExperienceList();
                            DateTime from = new DateTime(experience.From.Year, experience.From.Month, experience.From.Day);
                            experienceList.From = XAPI.EthiopicDateTime.GetEthiopicDate(from.Day, from.Month, from.Year);//from.ToShortDateString();
                            DateTime to = new DateTime(experience.To.Year, experience.To.Month, experience.To.Day);
                            experienceList.To = XAPI.EthiopicDateTime.GetEthiopicDate(to.Day, to.Month, to.Year);//to.ToShortDateString();
                            experienceList.Responsibility = experience.Responsibility;
                            experienceList.Totalyear = EmployeeHelper.GetDaystoWordsAmharic(Convert.ToDouble(to.Date.Subtract(from.Date).Days));
                            //experienceList.Totalyear = experience.TotalDuration;

                            totalExperience = totalExperience + (to.Date.Subtract(from.Date).Days);

                            if(totalExperience > 0)
                            {
                                EmployeeInfo.EmployeeExperienceList.Add(experienceList);
                            }
                           
                        }
                    }
                    //history
                    if (type == null || type == "Internal")
                    {

                        var histories = placementHistories.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                        foreach (var experience in histories)
                        {
                            var experienceList = new EmployeeExperienceList();
                            DateTime from = new DateTime(experience.From.Year, experience.From.Month, experience.From.Day);
                            experienceList.From = XAPI.EthiopicDateTime.GetEthiopicDate(from.Day, from.Month, from.Year);//from.ToShortDateString();
                            DateTime to = new DateTime(experience.To.Year, experience.To.Month, experience.To.Day);
                            experienceList.To = XAPI.EthiopicDateTime.GetEthiopicDate(to.Day, to.Month, to.Year);//to.ToShortDateString();
                            experienceList.Responsibility = experience.JobTitle;
                            //experienceList.Totalyear = (decimal)Math.Round(((to - from).TotalDays / 365), 2);
                            experienceList.Totalyear = EmployeeHelper.GetDaystoWordsAmharic((to - from).Days);


                            totalExperience = totalExperience + (to - from).Days;

                            if(totalExperience > 0)
                            {
                                EmployeeInfo.EmployeeExperienceList.Add(experienceList);
                            }
                         
                        }
                    }

                    
                        EmployeeInfo.TotalExp = EmployeeHelper.GetDaystoWordsAmharic(totalExperience);
                    
                    

                    ////currenct
                    //var cuplacements = placements.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    //foreach (var experience in cuplacements)
                    //{
                    //    var experienceList = new EmployeeExperienceList();
                    //    DateTime from = new DateTime(experience.AssignmentDate.Year, experience.AssignmentDate.Month, experience.AssignmentDate.Day);
                    //    experienceList.From = from.ToShortDateString();
                    //    DateTime to = new DateTime(experience.To.Year, experience.To.Month, experience.To.Day);
                    //    experienceList.To = to.ToShortDateString();
                    //    experienceList.Responsibility = experience.JobTitleName;
                    //    experienceList.Totalyear = experience.TotalDuration;
                    //    EmployeeInfo.EmployeeExperienceList.Add(experienceList);
                    //}
                    if(totalExperience > 0)
                    {
                        EmployeeInfoList.Add(EmployeeInfo);
                    }
                   
                   
                }
                // }
            }
            #endregion

            foreach (var emp in EmployeeInfoList)
            {
                if (emp.EmployeeExperienceList.Count > 0)
                    emp.Assigned = true;
                else
                    emp.Assigned = false;
            }

            return EmployeeInfoList;


        }
    }


}