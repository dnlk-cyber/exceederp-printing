using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    [DataObject]
    public class ExperienceEnglishObjectDataSourceFBC
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExperienceVM> GetExperience(bool header, string empId, bool includeExt)
        {

            List<ExperienceVM> experienceList = new List<ExperienceVM>();
            List<ExperienceVM> checkList = new List<ExperienceVM>();
            ExperienceVM EmployeeInfo = new ExperienceVM();




            var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");
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

            if (empId != null)
            {
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId.ToString() == empId);
                if (person != null)
                {
                    var placements = db.Placements.Where(x => x.EmployeeId == person.EmployeeId).ToList();

                    var experienceRequest = db.ExprianceRequests.FirstOrDefault(x => x.EmployeeId == person.EmployeeId);
                    if (experienceRequest != null)
                    {
                        var reqeustStatus = db.RequestesStatus.FirstOrDefault(x => x.RequestId == experienceRequest.ExperienceRequestId && x.Status == RequestStatus.APPROVE && x.ReferenceType == "Experience");
                        if (reqeustStatus != null)
                        {
                            if (placements.Any())
                            {
                                var placement = placements.FirstOrDefault(x => x.EmployeeId == person.EmployeeId);
                                if (placement != null)
                                {
                                    var template = db.Templates.FirstOrDefault(x => x.TemplateType == TemplateTypes.ExperianceEng);
                                    if (template != null)
                                    {
                                        EmployeeInfo.TempOne = template.TempOne;
                                    }
                                    // EmployeeInfo.ApplicantName = " " + person.FullNameaEnglish.ToLower();
                                    EmployeeInfo.ApplicantName = HrHelper.CapitalizeEacgWordFirstLetter(person.FullNameaEnglish);
                                    var lookTitle = db.Lookups.FirstOrDefault(x => x.LookupId == person.Title && x.LookupType == LookUpTypes.PERSON_TITLE);
                                    if (lookTitle != null)
                                    {
                                        EmployeeInfo.Title = " " + lookTitle.Description;
                                    }

                                    EmployeeInfo.Date = experienceRequest.ApplicationDate.ToString("MMMM dd, yyyy");
                                    if (person.Gender == Gender.MALE)
                                    {
                                        EmployeeInfo.HisHer = "his ";
                                    }
                                    if (person.Gender == Gender.FEMALE)
                                    {
                                        EmployeeInfo.HisHer = "her ";
                                    }
                                    var places = placements.Where(x => x.EmployeeId == person.EmployeeId).OrderByDescending(x => x.PlacementId).ToList();
                                    var jobs = "";
                                    for (int pl = 0; pl < places.Count(); pl++)
                                    {
                                        ExperienceVM checkInfo = new ExperienceVM();

                                        var lookupid = places[pl].LookupId;
                                        var look = db.Lookups.FirstOrDefault(x => x.LookupId == lookupid);
                                        if (look != null)
                                        {
                                            var branchid = places[pl].BranchId;
                                            var branch = db.Branch.FirstOrDefault(x => x.BranchId == branchid);
                                            if (branch != null)
                                            {
                                                if (pl >= 1)
                                                {
                                                    var job = places[pl].JobTitleName;
                                                    var b = places[pl].BranchId;
                                                    var br = db.Branch.FirstOrDefault(x => x.BranchId == b);
                                                    var bra = "";
                                                    if (br != null)
                                                    {
                                                        bra = br.AmharicName;
                                                    }
                                                    var checkJob = checkList.FirstOrDefault(x => x.JobTitleName == job && x.Branch == bra);
                                                    if (checkJob != null)
                                                    {
                                                        continue;
                                                    }
                                                    checkInfo.JobTitleName = job;
                                                    checkInfo.Branch = bra;

                                                    var assignmetDates = db.Placements.Where(x => x.JobTitleName == job && x.BranchId == b).Select(x => x.AssignmentDate).ToList();
                                                    var min = assignmetDates.Min().ToString("MMMM dd, yyyy");
                                                    var max = assignmetDates.Max().ToString("MMMM dd, yyyy");


                                                    jobs = "" + " From " + places[pl].AssignmentDate.ToString("MMMM dd, yyyy") + " To " + places[pl - 1].AssignmentDate.ToString("MMMM dd, yyyy") + " " + places[pl].EmployementType.ToString().ToLower() + " as " + look.Description + " at " + branch.Name + ", " + jobs;
                                                }
                                                else
                                                {
                                                    var job = places[pl].JobTitleName;
                                                    var b = places[pl].BranchId;
                                                    var br = db.Branch.FirstOrDefault(x => x.BranchId == b);
                                                    var bra = "";
                                                    if (br != null)
                                                    {
                                                        bra = br.AmharicName;
                                                    }
                                                    var checkJob = checkList.FirstOrDefault(x => x.JobTitleName == job && x.Branch == bra);
                                                    if (checkJob != null)
                                                    {
                                                        continue;
                                                    }

                                                    checkInfo.JobTitleName = job;
                                                    checkInfo.Branch = bra;

                                                    var assignmetDates = db.Placements.Where(x => x.JobTitleName == job && x.BranchId == b).Select(x => x.AssignmentDate).ToList();
                                                    var min = assignmetDates.Min();
                                                    var max = assignmetDates.Max();

                                                    jobs = jobs + " From " + places[pl].AssignmentDate.ToString("MMMM dd, yyyy") + " To " + "now " + places[pl].EmployementType.ToString().ToLower() + " as " + look.Description + " at " + branch.Name + " ";
                                                }


                                                var currentSalary = placements.FirstOrDefault(x => x.isCurrent);
                                                if (currentSalary != null)
                                                {
                                                    EmployeeInfo.CurrentSalary = currentSalary.Salary;
                                                    EmployeeInfo.SalaryInWord = XAPI.NumberExtensions.toWords(Convert.ToInt32(EmployeeInfo.CurrentSalary));
                                                    //EmployeeInfo.SalaryInWord =  Convert.ToInt32(EmployeeInfo.CurrentSalary).ToWordAmharic();

                                                }
                                                var placementHistories = db.PlacementHistories.Where(x => x.EmployeeId == person.EmployeeId).ToList();
                                                if (placementHistories.Any())
                                                {
                                                    var history = "";
                                                    foreach (var ph in placementHistories)
                                                    {
                                                        history = history + " From " + ph.From.ToString("MMMM dd, yyyy") + " To " + ph.To.ToString("MMMM dd, yyyy") + " " + ph.EmployementType.ToString().ToLower() + " as " + ph.JobTitle + ", ";

                                                    }
                                                    jobs = history + jobs;
                                                }

                                                EmployeeInfo.JobAndDate = jobs;

                                                if (includeExt)
                                                {
                                                    var experiences = db.Experiences.Where(x => x.EmployeeId == person.EmployeeId).ToList();

                                                    var expJobs = "";
                                                    foreach (var exp in experiences)
                                                    {
                                                        expJobs = expJobs + "" + exp.Organozation + " Job Title " + exp.Responsibility + " " + " From " + exp.From.ToShortDateString() + " To " + exp.To.ToShortDateString() + ", ";
                                                    }
                                                    EmployeeInfo.ExtExp = "and external experiences " + expJobs;
                                                }

                                                checkList.Add(checkInfo);

                                            }


                                        }

                                    }
                                    experienceList.Add(EmployeeInfo);
                                }
                            }
                        }
                    }
                }
            }
            return experienceList;
        }


    }
}
