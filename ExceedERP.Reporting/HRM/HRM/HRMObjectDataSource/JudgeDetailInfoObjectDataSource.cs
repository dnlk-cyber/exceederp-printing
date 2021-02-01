using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class JudgeDetailInfoObjectDataSource : List<JudgeDetailInformation>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JudgeDetailInformation> GetJudgeDetailInfo(string dept)
        {
            var employees = db.Person_Employee.Where(x => x.IsActive);
            JudgeDetailInformation JudgeDetailInformation = new JudgeDetailInformation();
            List<JudgeDetailInformation> JudgeDetailInformationList = new List<JudgeDetailInformation>();
            var placmenets = db.Placements;
            var educations = db.Education;
            var lookups = db.Lookups;
            var exprinces = db.Experiences;
            var validator = db.Configurations.Where(x => x.Attribute == "RETIREMENT_AGE").FirstOrDefault();
            var identifications = db.Identifications;
            if (dept == null)
            {
                foreach (var e in employees)
                {
                    JudgeDetailInformation = new JudgeDetailInformation();
                    JudgeDetailInformation.DateOfBirth = e.DateOfBirthAm;
                    JudgeDetailInformation.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                    JudgeDetailInformation.Age = DateTime.Now.Year - e.DateOfBirth.Year;
                    var nation = lookups.Where(x => x.LookupId.ToString() == e.Nation).FirstOrDefault();
                    if (nation != null)
                    {
                        JudgeDetailInformation.Nation = nation.Description;
                    }
                    var lastEducation = educations.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.To).FirstOrDefault();
                    if (lastEducation != null)
                    {
                        var name = lookups.Where(x => x.LookupId == lastEducation.EduField).FirstOrDefault();
                        if (name != null)
                        {
                            JudgeDetailInformation.EduLevel = name.Description;
                        }
                    }
                    var currentExpriance = exprinces.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    if (currentExpriance.Any())
                    {

                        foreach (var ex in currentExpriance)
                        {

                            JudgeDetailInformation.Expiriance += ex.To.Year - ex.From.Year;
                        }

                    }
                    var value = identifications.Where(
                        x =>
                            x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" &&
                            x.Type == IdentificationType.PENSION_NO).FirstOrDefault();
                    if (value != null)
                    {

                        JudgeDetailInformation.PensionNo = value.IdNumber;
                    }
                    if (validator != null)
                    {
                        int retirementAge = Convert.ToInt32(validator.CurrentValue) - JudgeDetailInformation.Age;

                        DateTime time = DateTime.Now.AddYears(retirementAge);
                        JudgeDetailInformation.PensionDate = XAPI.EthiopicDateTime.GetEthiopicDate(time.Day, time.Month, time.Year).ToString();
                    }
                    JudgeDetailInformation.MartialStatus = e.MartialStatus;
                    var hire = placmenets.Where(x => x.EmployeeId == e.EmployeeId && x.isCurrent).FirstOrDefault();
                    if (hire != null)
                    {
                        JudgeDetailInformation.HireDate = hire.AssignmentDateAmharic;
                    }

                    JudgeDetailInformationList.Add(JudgeDetailInformation);
                }
            }
            else
            {
                var placmenetses = placmenets.Where(x => x.OrgStructureId.ToString() == dept).ToList();

                foreach (var e in employees)
                {
                    JudgeDetailInformation = new JudgeDetailInformation();
                    var checkdept = placmenetses.Where(x => x.EmployeeId == e.EmployeeId).FirstOrDefault();
                    if (checkdept == null) continue;
                    var hire = placmenetses.Where(x => x.EmployeeId == e.EmployeeId && x.isCurrent).FirstOrDefault();
                    if (hire != null)
                    {
                        JudgeDetailInformation.HireDate = hire.AssignmentDateAmharic;
                    }
                    JudgeDetailInformation.DateOfBirth = e.DateOfBirthAm;
                    JudgeDetailInformation.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                    JudgeDetailInformation.Age = DateTime.Now.Year - e.DateOfBirth.Year;
                    var nation = lookups.Where(x => x.LookupId.ToString() == e.Nation).FirstOrDefault();
                    if (nation != null)
                    {
                        JudgeDetailInformation.Nation = nation.Description;
                    }
                    var lastEducation = educations.Where(x => x.EmployeeId == e.EmployeeId).OrderByDescending(x => x.To).FirstOrDefault();
                    if (lastEducation != null)
                    {
                        var name = lookups.Where(x => x.LookupId == lastEducation.EduField).FirstOrDefault();
                        if (name != null)
                        {
                            JudgeDetailInformation.EduLevel = name.Description;
                        }
                    }
                    var currentExpriance = exprinces.Where(x => x.EmployeeId == e.EmployeeId).ToList();
                    if (currentExpriance.Any())
                    {

                        foreach (var ex in currentExpriance)
                        {

                            JudgeDetailInformation.Expiriance += ex.To.Year - ex.From.Year;
                        }

                    }
                    var value = identifications.Where(
                        x =>
                            x.Reference == e.EmployeeId.ToString() && x.ReferenceType == "Employee" &&
                            x.Type == IdentificationType.PENSION_NO).FirstOrDefault();
                    if (value != null)
                    {

                        JudgeDetailInformation.PensionNo = value.IdNumber;
                    }
                    if (validator != null)
                    {
                        int retirementAge = Convert.ToInt32(validator.CurrentValue) - JudgeDetailInformation.Age;

                        // JudgeDetailInformation.PensionDate = DateTime.Now.AddYears(retirementAge).ToShortDateString();
                        DateTime time = DateTime.Now.AddYears(retirementAge);
                        JudgeDetailInformation.PensionDate = XAPI.EthiopicDateTime.GetEthiopicDate(time.Day, time.Month, time.Year).ToString();

                    }
                    JudgeDetailInformation.MartialStatus = e.MartialStatus;


                    JudgeDetailInformationList.Add(JudgeDetailInformation);
                }
            }
            return JudgeDetailInformationList;
        }
    }
}