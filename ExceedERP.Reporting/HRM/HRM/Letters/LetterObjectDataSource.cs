using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Reporting.HRM.HRM.Letters
{
    public class LetterObjectDataSource
    {
    }

    #region Termination

    [DataObject]
    public class EmployeeTerminationObjectDataSource : List<EmployeeTerminationLetter>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeTerminationLetter> GetEmployeeTermination(string id, bool header)
        {
            List<EmployeeTerminationLetter> EmployeeInfoList = new List<EmployeeTerminationLetter>();
            EmployeeTerminationLetter EmployeeInfo = new EmployeeTerminationLetter();
            if (header)
            {
                var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();
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

                                EmployeeInfo.WebSite = "Website: " + a.Value;

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
            db = new ExceedDbContext();
            var personEmployee = db.Person_Employee.Where(x => x.EmployeeCode == id).FirstOrDefault();
            if (personEmployee != null)
            {
                var employyee = personEmployee.EmployeeId;
                var termination = db.Terminations.Where(x => x.EmployeeId == employyee).FirstOrDefault();
                if (termination != null)
                {
                    db = new ExceedDbContext();
                    var identifications =
                        db.Identifications.Where(
                            x =>
                                x.Type == IdentificationType.PENSION_NO && x.Reference == employyee.ToString() &&
                                x.ReferenceType == "Employee").ToList();
                    db = new ExceedDbContext();
                    var jobs = db.Lookups.ToList();
                    db = new ExceedDbContext();
                    var education = db.Education.Where(y => y.EmployeeId == employyee).ToList().OrderBy(x => x.To);
                    db = new ExceedDbContext();
                    var placements = db.Placements.Where(y => y.EmployeeId == employyee)
                        .ToList()
                        .OrderBy(x => x.AssignmentDate);
                    db = new ExceedDbContext();
                    var expriance = db.Experiences.Where(y => y.EmployeeId == employyee && y.IsPrivate == false).OrderBy(x => x.To).ToList();
                    var emplist = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == employyee);

                    db = new ExceedDbContext();

                    int other = 0;
                    int currexp = 0;
                    decimal totalexp = 0;
                    int month = 0;
                    int day = 0;
                    decimal totalOther = 0;

                    if (emplist != null)
                    {
                        var e = emplist;

                        {

                            EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                            EmployeeInfo.BirthDate = e.DateOfBirthAm;
                            EmployeeInfo.HireDate = e.HiredDateAmharic;

                            if (placements.Count() > 0)
                            {
                                var place = placements.Max(x => x.AssignmentDate);
                                if (place != null)
                                {
                                    var placement = placements.FirstOrDefault(x => x.AssignmentDate == place);
                                    if (placement != null)
                                    {
                                        EmployeeInfo.Salary = placement.Salary;
                                        EmployeeInfo.JobTitle =
                                            jobs.Where(x => x.LookupId == placement.LookupId).FirstOrDefault().Description;
                                    }
                                }
                            }
                            if (education.Count() > 0)
                            {
                                var filed = education.FirstOrDefault();
                                EmployeeInfo.EduLevel =
                                    jobs.Where(x => x.LookupId == filed.EduLevel).FirstOrDefault().Description;
                            }
                            if (identifications.Any())
                            {
                                var pensionId = identifications.FirstOrDefault();
                                if (pensionId != null)
                                {
                                    EmployeeInfo.PensionId = pensionId.IdNumber;
                                }
                            }
                            if (expriance.Count() > 0)
                            {
                                foreach (var ex in expriance)
                                {
                                    var from = new DateTime(ex.From.Year, ex.From.Month, ex.From.Day);
                                    var to = new DateTime(ex.To.Year, ex.To.Month, ex.To.Day);

                                    var totalmonth = (to.Year - from.Year) * 12 + to.Month - from.Month;
                                    totalmonth += to.Day < from.Day ? -1 : 0;

                                    var year = totalmonth / 12;
                                    var monthh = totalmonth % 12;
                                    var days = from.Subtract(to.AddMonths(totalmonth)).Days;
                                    currexp += year;
                                    month += monthh;
                                    day += days;
                                    totalOther = Math.Round((decimal)(other + (((month * 30) + day) / 365.25)), 2);

                                }
                                EmployeeInfo.OtherExprinace = XAPI.Common.GetYearAndMonth(currexp, month, day);
                            }
                            var fromc = new DateTime(e.HiredDate.Year, e.HiredDate.Month, e.HiredDate.Day);
                            var toc = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                            var totalmonths = (toc.Year - fromc.Year) * 12 + toc.Month - fromc.Month;
                            totalmonths += toc.Day < fromc.Day ? -1 : 0;

                            var years = totalmonths / 12;
                            var months = totalmonths % 12;
                            var dayss = fromc.Subtract(toc.AddMonths(totalmonths)).Days;
                            currexp += years;
                            month += months;
                            day += dayss;
                            decimal totalcurr = Math.Round((decimal)(currexp + (((month * 30) + day) / 365.25)), 2); ;
                            EmployeeInfo.CurrentExprinace = XAPI.Common.GetYearMonthDay(e.HiredDate, DateTime.Now.Date); ;
                            totalexp = totalcurr + totalOther;
                            EmployeeInfo.TotalExprinace = XAPI.Common.GetYearAndMonth(currexp, month, day);
                            if (termination != null)
                            {
                                //EmployeeInfo.ApplicationDate = termination.ApplicationDate.ToShortDateString();
                                EmployeeInfo.ApplicationDate = XAPI.EthiopicDateTime.GetEthiopicYear(termination.ApplicationDate.Day, termination.ApplicationDate.Month, termination.ApplicationDate.Year).ToString();
                                EmployeeInfo.TerminationDate = termination.TerminationDate.ToShortDateString();
                                EmployeeInfo.TerminationReason = jobs.Where(x => x.LookupId == termination.LookupId)
                                        .FirstOrDefault()
                                        .Description;
                            }
                            EmployeeInfo.Year = XAPI.EthiopicDateTime.GetEthiopicYear(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year).ToString();
                            EmployeeInfoList.Add(EmployeeInfo);
                        }

                    }


                }
            }

            return EmployeeInfoList;
        }

    }



    #endregion

    #region Recruitment
    [DataObject]
    public class EmployeeRecruitmentExterVaccLetterObjectDataSource : List<EmployeeRecruitmentExternalVaccLetter>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EmployeeRecruitmentExternalVaccLetter> GetEmployeeRecruitmentVacc(string id, bool header)
        {
            List<EmployeeRecruitmentExternalVaccLetter> EmployeeInfoList = new List<EmployeeRecruitmentExternalVaccLetter>();
            EmployeeRecruitmentExternalVaccLetter EmployeeInfo = new EmployeeRecruitmentExternalVaccLetter();
            var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();

            var personEmployee = db.Person_Employee.Where(x => x.EmployeeCode == id).FirstOrDefault();

            if (personEmployee != null)
            {
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

                                    EmployeeInfo.WebSite = "Website: " + a.Value;

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
                db = new ExceedDbContext();
                var employee = db.Person_Employee.Where(x => x.EmployeeId == personEmployee.EmployeeId && x.IsActive).FirstOrDefault();
                if (employee != null)
                {
                    db = new ExceedDbContext();
                    var structurtes =
                        db.MainCompanyStructures.ToList();
                    db = new ExceedDbContext();
                    var jobs = db.Lookups.ToList();
                    db = new ExceedDbContext();
                    var ranks = db.Ranks;
                    db = new ExceedDbContext();
                    var branches = db.Branch;
                    db = new ExceedDbContext();
                    var placements = db.Placements.Where(y => y.EmployeeId == employee.EmployeeId)
                        .ToList()
                        .OrderBy(x => x.AssignmentDate);
                    db = new ExceedDbContext();
                    var expriance = db.Experiences.Where(y => y.EmployeeId == employee.EmployeeId && y.IsPrivate == false).OrderBy(x => x.To).ToList();
                    var emplist = db.Person_Employee.Where(x => x.EmployeeId == employee.EmployeeId && x.IsActive).ToList();

                    db = new ExceedDbContext();


                    if (emplist.Any())
                    {

                        foreach (var e in emplist)
                        {

                            EmployeeInfo.FullName = e.FirstName + " " + e.MiddleName + " " + e.LastName;
                            EmployeeInfo.BirthDate = e.DateOfBirth.ToShortDateString();
                            EmployeeInfo.HireDate = e.HiredDate.ToShortDateString();

                            if (placements.Count() > 0)
                            {
                                var placement = placements.Where(x => x.isCurrent).FirstOrDefault();

                                if (placement != null)
                                {
                                    EmployeeInfo.Salary = placement.Salary;
                                    EmployeeInfo.JobTitle =
                                        jobs.Where(x => x.LookupId == placement.LookupId).FirstOrDefault().Description;
                                    if (structurtes != null)
                                    {
                                        EmployeeInfo.Department =
                                            structurtes.Where(x => x.OrgStructureId == placement.Department)
                                                .FirstOrDefault()
                                                .Name;
                                    }
                                    if (ranks.Count() > 0)
                                    {

                                        EmployeeInfo.RankName = ranks.
                                            Where(x => x.RankId == placement.RankId).FirstOrDefault().RankName;
                                    }
                                    if (branches.Count() > 0)
                                    {
                                        if (placement.BranchId != 0)
                                        {

                                            EmployeeInfo.Branch =
                                                branches.Where(x => x.BranchId == placement.BranchId).FirstOrDefault().AmharicName;
                                        }
                                    }

                                }
                            }
                            EmployeeInfo.CurrentDateAmh = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);


                            EmployeeInfoList.Add(EmployeeInfo);
                        }

                    }


                }

            }

            return EmployeeInfoList;
        }


    }
    #endregion

    #region

    

    #endregion

    #region Experience






    #endregion

    #region Termination




   



    #endregion

    //=====================Ermi==================================================================

    #region MediaVacancyCode

    [DataObject]
    public class VacancyCodeForMediaObjectDataSource : VacancyForMedias
    {
        private ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<RecruitmentVacancy> GetMediaVacancyCode()
        {
            List<RecruitmentVacancy> list = new List<RecruitmentVacancy> { };

            var vacancies = db.RecruitmentVacancy.DistinctByField(x => x.MediaVacancyCode).ToList();

            if (!vacancies.Any())
            {
                return list;
            }

            return vacancies;

        }
    }
    [DataObject]
    public class InternalMediaVacancyCodeObjectDataSource : VacancyForMedias
    {
        private ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<RecruitmentVacancy> GetMediaVacancyCode()
        {
            List<RecruitmentVacancy> list = new List<RecruitmentVacancy> { };
            RecruitmentVacancy recruit = new RecruitmentVacancy();

            var recruitementVacancies = db.RecruitmentVacancy.ToList();

            foreach (var recruitmentVacancy in recruitementVacancies)
            {
                var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == recruitmentVacancy.VacancyId && x.IsActive);
                if (vacancy != null && vacancy.RecruSource == RecruitmentSource.INTERNAL)
                {
                    recruit = new RecruitmentVacancy();
                    recruit.MediaVacancyCode = recruitmentVacancy.MediaVacancyCode;
                    list.Add(recruit);
                }

            }


            return list.DistinctByField(x => x.MediaVacancyCode).ToList();

        }
    }
    [DataObject]
    public class InternalApplicantMediaVacancyCodeObjectDataSource : VacancyForMedias
    {
        private ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<RecruitmentVacancy> GetMediaVacancyCode()
        {
            List<RecruitmentVacancy> list = new List<RecruitmentVacancy> { };
            RecruitmentVacancy recruit = new RecruitmentVacancy();

            var recruitementVacancies = db.RecruitmentVacancy.ToList();

            foreach (var recruitmentVacancy in recruitementVacancies)
            {
                var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == recruitmentVacancy.VacancyId && !x.IsComplete);
                if (vacancy != null && vacancy.RecruSource == RecruitmentSource.INTERNAL)
                {
                    recruit = new RecruitmentVacancy();
                    recruit.MediaVacancyCode = recruitmentVacancy.MediaVacancyCode;
                    list.Add(recruit);
                }

            }


            return list.DistinctByField(x => x.MediaVacancyCode).ToList();

        }
    }
    [DataObject]
    public class ExternalMediaVacancyCodeObjectDataSource : VacancyForMedias
    {
        private ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<RecruitmentVacancy> GetMediaVacancyCode()
        {
            List<RecruitmentVacancy> list = new List<RecruitmentVacancy> { };
            RecruitmentVacancy recruit = new RecruitmentVacancy();

            var recruitementVacancies = db.RecruitmentVacancy.ToList();

            foreach (var recruitmentVacancy in recruitementVacancies)
            {
                var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == recruitmentVacancy.VacancyId && x.IsActive);
                if (vacancy != null && vacancy.RecruSource == RecruitmentSource.EXTERNAL)
                {
                    recruit = new RecruitmentVacancy();
                    recruit.MediaVacancyCode = recruitmentVacancy.MediaVacancyCode;
                    list.Add(recruit);
                }

            }


            return list.DistinctByField(x => x.MediaVacancyCode).ToList();

        }
    }

    [DataObject]
    public class ExternalApplicantMediaVacancyCodeObjectDataSource : VacancyForMedias
    {
        private ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<RecruitmentVacancy> GetMediaVacancyCode()
        {
            List<RecruitmentVacancy> list = new List<RecruitmentVacancy> { };
            RecruitmentVacancy recruit = new RecruitmentVacancy();

            var recruitementVacancies = db.RecruitmentVacancy.ToList();

            foreach (var recruitmentVacancy in recruitementVacancies)
            {
                var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == recruitmentVacancy.VacancyId && !x.IsComplete);
                if (vacancy != null && vacancy.RecruSource == RecruitmentSource.EXTERNAL)
                {
                    recruit = new RecruitmentVacancy();
                    recruit.MediaVacancyCode = recruitmentVacancy.MediaVacancyCode;
                    list.Add(recruit);
                }

            }


            return list.DistinctByField(x => x.MediaVacancyCode).ToList();

        }
    }
    #endregion
    //=====================================================================================================
    #region Vacancy for media
    [DataObject]
    public class VacancyForMediaLetterObjectDataSource : VacancyForMedias
    {
        private ExceedDbContext db = new ExceedDbContext();

        ////=====================Ermi============================
        //[DataObjectMethod(DataObjectMethodType.Select)]

        //public List<RecruitmentVacancy> GetMediaVacancyCode(int id)
        //{
        //    List<RecruitmentVacancy> list = new List<RecruitmentVacancy> { };

        //    var vacancies = db.RecruitmentVacancy.ToList();

        //    if (!vacancies.Any())
        //    {
        //        return list;
        //    }

        //    return vacancies;

        //}
        ////==================================================

        [DataObjectMethod(DataObjectMethodType.Select)]
        public VacancyForMedias GetVacancyForMedia(string id, bool header)
        {

            VacancyForMedias Info = new VacancyForMedias();
            var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();

            db = new ExceedDbContext();
            var vacancy = db.VaccancyPlans.Where(x => x.VaccancyCode == id).FirstOrDefault();

            if (vacancy != null)
            {
                if (header)
                {
                    if (Organization != null)
                    {
                        Info.Name = Organization.TradeName;
                        Info.AmharicName = Organization.AmharicName;
                        Info.Abbreviation = Organization.Abbreviation;
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
                                    Info.Address = "Subcity: " + a.Value + " ";
                                }
                                if (a.Category == AddressType.KEBELE)
                                {
                                    Info.Address += "Kebele: " + a.Value + " ";
                                }
                                if (a.Category == AddressType.CITYPROVINCE)
                                {
                                    if (Info.Address == null)
                                    {
                                        Info.Address += a.Value + ", Ethiopia";
                                    }
                                }
                                else if (a.Category == AddressType.POBOX)
                                {

                                    Info.Pobox = "P.O.Box: " + a.Value;

                                }
                                else if (a.Category == AddressType.OFFICE_EMAIL)
                                {

                                    Info.Email = "Email: " + a.Value;

                                }
                                else if (a.Category == AddressType.WEBSITE)
                                {

                                    Info.WebSite = "Website: " + a.Value;

                                }
                                else if (a.Category == AddressType.FAX)
                                {

                                    Info.Fax = "Fax:  " + a.Value;

                                }
                                else if (a.Category == AddressType.OFFICE_PHONE)
                                {

                                    Info.Tel = "Tel:  " + a.Value;

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
                            Info.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                        }
                    }
                }




            }

            return Info;
        }
    }
    #endregion
}
