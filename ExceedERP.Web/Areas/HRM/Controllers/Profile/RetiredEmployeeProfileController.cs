using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.UI.Fluent;
using System.Data.OleDb;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using Kendo.Mvc;
using Microsoft.Ajax.Utilities;
using WebGrease.Css.Extensions;
using Lookup = ExceedERP.Core.Domain.Common.HRM.Lookup;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin,HrmConstants.HRProfileUser)]
    public class RetiredEmployeeProfileController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string FirstName { get; set; }
        public static string MiddleName { get; set; }
        public static string LastName { get; set; }
        public static DateTime DateOfBirth { get; set; }
        public static DateTime HireDate { get; set; }
        public static string AmharicDateOfBirth { get; set; }
        public static string AmharicHireDate { get; set; }
        string ErrorMessage = "";

        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            GetEducationFiled();
            GetEducationLevel();
            SetInitialDatas();

            return View();
        }
        public void GetDate(string date)
        {

        }
        public void GetFirstName(string car)
        {
            FirstName = car;
        }
        public void GetMiddleName(string car)
        {
            MiddleName = car;
        }
        public void GetLastName(string car)
        {
            LastName = car;
        }
        public JsonResult GetEducationFiled()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_FIELD).AsQueryable();
            ViewData["EduFiledList"] = lookups;
            return Json(lookups.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEducationLevel()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL).AsQueryable();
            ViewData["EduLevelList"] = lookups;
            return Json(lookups.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadFile(string filename)
        {
            if (HRMInputs.SelectedFile == 0)
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            try
            {
                var attachment =
                        db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.PHOTO && x.Remark == HRMInputs.Owner && x.AttachmentId == HRMInputs.SelectedFile)
                            .FirstOrDefault();
                if (attachment != null)
                {
                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition",
                        "attachment;filename=\"" + attachment.Description + "\"");
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.PHOTO + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (Exception)
            {

                // throw;
            }
            HRMInputs.SelectedFile = 0;
            return null;
        }

        public ActionResult DownloadDoument(string filename)
        {
            if (HRMInputs.SelectedFile == 0)
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            try
            {
                var attachment =
                        db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.DOCUMENT && x.Remark == HRMInputs.Owner && x.AttachmentId == HRMInputs.SelectedFile)
                            .FirstOrDefault();
                if (attachment != null)
                {
                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition",
                        "attachment;filename=\"" + attachment.Description + "\"");
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (Exception)
            {

                // throw;
            }
            HRMInputs.SelectedFile = 0;
            return null;
        }
        public void SetInitialDatas()
        {

            HRMInputs inputs = new HRMInputs();
            var periodList = db.Periods;

            var languageList = db.Languages;
            var relations = db.Lookups.Where(l => l.LookupType == LookUpTypes.RELATIONSHIP);
            ViewData["LanguageList"] = languageList != null ? languageList.ToList() : new List<Language>();

            ViewData["PeriodList"] = periodList != null ? periodList.ToList() : new List<Period>();

            ViewData["Relationship"] = relations != null ? relations.ToList() : new List<Lookup>();
            ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);
            GetNation();
            GetNationality();
            var skillTypes = db.Lookups.Where(l => l.LookupType == LookUpTypes.SKILL_TYPE);
            ViewData["SkillTypes"] = skillTypes;
            //CheckEmployeeRetirment();
        }
        public void CheckEmployeeRetirment()
        {
            double maxAge;
            List<string> employeesList = inputs.GetUserBranchEmployees(User.Identity.Name);
            var employees = db.Person_Employee.Where(x => employeesList.Contains(x.EmployeeId.ToString())).Where(x => x.IsActive && !x.IsReHired && x.Remark != "R-Active");
            db = new ExceedDbContext();
            bool isActive = true;
            var terminations = db.Terminations;
            var setting = db.Configurations.FirstOrDefault(x => x.Reference == "PENSION" && x.Attribute == "RETIREMENT_AGE");
            if (setting != null)
            {
                maxAge = Convert.ToInt32(setting.CurrentValue);
                foreach (var e in employees)
                {
                    double age = DateTime.Now.Date.Subtract(new DateTime(e.DateOfBirth.Year, e.DateOfBirth.Month,  e.DateOfBirth.Day)).TotalDays / 365.25;

                    if (terminations.Any())
                    {
                        var checkTermintedEmp = terminations.FirstOrDefault(x => x.EmployeeId == e.EmployeeId);

                        if (checkTermintedEmp == null)
                        {

                            isActive = true;

                        }
                        else
                        {
                            if (checkTermintedEmp.TerminationDate.Date >= DateTime.Now.Date)
                            {
                                isActive = false;
                            }
                        }
                    }
                    if (age >= maxAge)
                    {
                        isActive = false;
                        if (e.IsActive)
                        {
                            db = new ExceedDbContext();
                            var entity = new Person_Employee
                            {
                                EmployeeId = e.EmployeeId,
                                EmployeeCode = e.EmployeeCode,
                                MothersFullName = e.MothersFullName,
                                BirthPlace = e.BirthPlace,
                                Nation = e.Nation,
                                Region = e.Region,
                                Religion = e.Religion,
                                MartialStatus = e.MartialStatus,
                                Status = e.Status,
                                HiredDate = e.HiredDate,
                                FirstName = e.FirstName,
                                MiddleName = e.MiddleName,
                                LastName = e.LastName,
                                Nationality = e.Nationality,
                                DateOfBirth = e.DateOfBirth,
                                Title = e.Title,
                                Gender = e.Gender,
                                CategoryID = e.CategoryID,
                                IsActive = isActive,
                                Remark = "Retirement",
                                DateCreated = e.DateCreated,
                                LastModified = DateTime.Now,
                                CreatedBy = e.CreatedBy,
                                ModifiedBy = User.Identity.Name,
                                FirstNameEnglish = e.FirstNameEnglish,
                                MiddleNameEnglish = e.MiddleNameEnglish,
                                LastNameEnglish = e.LastNameEnglish,
                                DateOfBirthAm = e.DateOfBirthAm,
                                HiredDateAmharic = e.HiredDateAmharic,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                            };
                            db = new ExceedDbContext();
                            db.Person_Employee.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else if (e.IsActive == false && e.Remark == "Retirement")
                    {

                        {
                            db = new ExceedDbContext();
                            var entity = new Person_Employee
                            {
                                EmployeeId = e.EmployeeId,
                                EmployeeCode = e.EmployeeCode,
                                MothersFullName = e.MothersFullName,
                                BirthPlace = e.BirthPlace,
                                Nation = e.Nation,
                                Region = e.Region,
                                Religion = e.Religion,
                                MartialStatus = e.MartialStatus,
                                Status = e.Status,
                                HiredDate = e.HiredDate,
                                FirstName = e.FirstName,
                                MiddleName = e.MiddleName,
                                LastName = e.LastName,
                                Nationality = e.Nationality,
                                DateOfBirth = e.DateOfBirth,
                                Title = e.Title,
                                Gender = e.Gender,
                                CategoryID = e.CategoryID,
                                IsActive = isActive,
                                Remark = "Retirement",
                                DateCreated = e.DateCreated,
                                LastModified = DateTime.Now,
                                CreatedBy = e.CreatedBy,
                                ModifiedBy = User.Identity.Name,
                                FirstNameEnglish = e.FirstNameEnglish,
                                MiddleNameEnglish = e.MiddleNameEnglish,
                                LastNameEnglish = e.LastNameEnglish,
                                DateOfBirthAm = e.DateOfBirthAm,
                                HiredDateAmharic = e.HiredDateAmharic,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                            };
                            db = new ExceedDbContext();
                            db.Person_Employee.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        //db = new ExceedDbContext();
                        //var entity = new Person_Employee
                        //{
                        //    EmployeeId = e.EmployeeId,
                        //    EmployeeCode = e.EmployeeCode,
                        //    MothersFullName = e.MothersFullName,
                        //    BirthPlace = e.BirthPlace,
                        //    Nation = e.Nation,
                        //    Region = e.Region,
                        //    Religion = e.Religion,
                        //    MartialStatus = e.MartialStatus,
                        //    Status = e.Status,
                        //    HiredDate = e.HiredDate,
                        //    FirstName = e.FirstName,
                        //    MiddleName = e.MiddleName,
                        //    LastName = e.LastName,
                        //    Nationality = e.Nationality,
                        //    DateOfBirth = e.DateOfBirth,
                        //    Title = e.Title,
                        //    Gender = e.Gender,
                        //    CategoryID = e.CategoryID,
                        //    IsActive = isActive,
                        //    Remark = e.Remark,
                        //    DateCreated = e.DateCreated,
                        //    LastModified = DateTime.Now,
                        //    CreatedBy = e.CreatedBy,
                        //    ModifiedBy = User.Identity.Name,
                        //    FirstNameEnglish = e.FirstNameEnglish,
                        //    MiddleNameEnglish = e.MiddleNameEnglish,
                        //    LastNameEnglish = e.LastNameEnglish,
                        //    DateOfBirthAm = e.DateOfBirthAm,
                        //    HiredDateAmharic = e.HiredDateAmharic,
                        //    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                        //};
                        ////db = new ExceedDbContext();
                        //db.Person_Employee.Attach(entity);
                        //db.Entry(entity).State = EntityState.Modified;
                        //db.SaveChanges();
                    }
                }
            }

        }

        private IQueryable<Person_Employee> ModifyFilters(IEnumerable<IFilterDescriptor> filters, IQueryable<Person_Employee> person_employee)
        {
            IQueryable<Person_Employee> person_employees = new Person_Employee[] { }.AsQueryable(); ;
            if (filters.Any())
            {
                string searchedValue = "";

                dd: if (filters.Any())
                {

                    foreach (var filter in filters)
                    {
                        var descriptor = filter as FilterDescriptor;
                        if (descriptor != null)
                        {
                            if (descriptor.Member == "FirstName")
                            {

                                searchedValue = descriptor.Value.ToString();
                                var value = searchedValue;
                                if (person_employees.Any())
                                {
                                    var person =
                                        person_employees.ToList().Where(
                                            j =>
                                                j.FirstName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                value.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    person_employees = person;
                                }
                                else
                                {
                                    var person =
                                           person_employee.ToList().Where(
                                               j =>
                                                   j.FirstName.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                   value.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    person_employees = person_employees.Union(person).AsQueryable();
                                }

                            }
                            if (descriptor.Member == "EmployeeCode")
                            {

                                searchedValue = descriptor.Value.ToString();
                                var value = searchedValue;
                                if (person_employees.Any())
                                {
                                    var person =
                                        person_employees.ToList().Where(
                                            j =>
                                                j.EmployeeCode.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                value.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    person_employees = person;
                                }
                                else
                                {
                                    var person =
                                           person_employee.ToList().Where(
                                               j =>
                                                   j.EmployeeCode.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture)) ==
                                                   value.ToUpper(XAPI.Common.exceedculture)
                                                       .Contains(value.ToUpper(XAPI.Common.exceedculture))).AsQueryable();
                                    person_employees = person_employees.Union(person).AsQueryable();
                                }

                            }
                            if (descriptor.Member == "MiddleName")
                            {

                                searchedValue = descriptor.Value.ToString();
                                var MiddleName = searchedValue;
                                if (person_employees.Any())
                                {
                                    var Middle =
                                        person_employees.ToList().Where(
                                            j =>
                                                j.MiddleName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(MiddleName.ToUpper(XAPI.Common.exceedculture)) ==
                                                MiddleName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(MiddleName.ToUpper(XAPI.Common.exceedculture)))
                                            .AsQueryable();
                                    person_employees = Middle;
                                }
                                else
                                {
                                    var Middle =
                                        person_employee.ToList().Where(
                                            j =>
                                                j.MiddleName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(MiddleName.ToUpper(XAPI.Common.exceedculture)) ==
                                                MiddleName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(MiddleName.ToUpper(XAPI.Common.exceedculture)))
                                            .AsQueryable();
                                    person_employees = person_employees.Union(Middle);
                                }


                            }

                            if (descriptor.Member == "LastName")
                            {
                                searchedValue = descriptor.Value.ToString();
                                var LastName = searchedValue;
                                if (person_employees.Any())
                                {
                                    var Last =
                                        person_employees.ToList().Where(
                                            j =>
                                                j.LastName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(LastName.ToUpper(XAPI.Common.exceedculture)) ==
                                                LastName.ToUpper(XAPI.Common.exceedculture)
                                                    .Contains(LastName.ToUpper(XAPI.Common.exceedculture)))
                                            .AsQueryable();
                                    person_employees = Last;

                                }
                                else

                                {
                                    var Last =
                                       person_employee.ToList().Where(
                                           j =>
                                               j.LastName.ToUpper(XAPI.Common.exceedculture)
                                                   .Contains(LastName.ToUpper(XAPI.Common.exceedculture)) ==
                                               LastName.ToUpper(XAPI.Common.exceedculture)
                                                   .Contains(LastName.ToUpper(XAPI.Common.exceedculture)))
                                           .AsQueryable();

                                    person_employees = person_employees.Union(Last);
                                }
                            }
                        }
                        else if (filter is CompositeFilterDescriptor)
                        {

                            filters = ((CompositeFilterDescriptor)filter).FilterDescriptors;
                            goto dd;
                            //ModifyFilters(((CompositeFilterDescriptor) filter).FilterDescriptors, person_employee);

                        }

                    }

                }
            }
            return person_employees.AsQueryable();
        }
   
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin, HrmConstants.HRProfileUser)]
        public ActionResult Person_Employee_Read([DataSourceRequest]DataSourceRequest request)
        {

            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<Person_Employee> person_employees = db.Person_Employee.Where(x => mybraches.Contains(x.OrgBranchName.ToString()) && x.IsActive == false && x.Remark.Contains("Retirement"));

            #region Commented
            //string searchedValue = "";

            //if (request.Filters != null)
            //{
            //    dd: if (request.Filters.Any())
            //    {
            //        foreach (var filter in request.Filters)
            //        {
            //            var descriptor = filter as FilterDescriptor;
            //            if (descriptor != null)
            //            {
            //                if (descriptor.Member == "LastName" || descriptor.Member == "EmployeeCode" || descriptor.Member == "MiddleName" || descriptor.Member == "FirstName")
            //                {
            //                    person_employees = ModifyFilters(request.Filters, person_employees).AsQueryable();

            //                }

            //            }
            //            else if (filter is CompositeFilterDescriptor)
            //            {

            //                request.Filters = ((CompositeFilterDescriptor)filter).FilterDescriptors;
            //                goto dd;
            //                //ModifyFilters(((CompositeFilterDescriptor) filter).FilterDescriptors, person_employee);

            //            }
            //        }
            //        //request.Filters.Clear();
            //    }
            //}

            #endregion

            DataSourceResult result = person_employees.ToDataSourceResult(request, person_Employee => new
            {
                EmployeeId = person_Employee.EmployeeId,
                EmployeeCode = person_Employee.EmployeeCode,
                MothersFullName = person_Employee.MothersFullName,
                BirthPlace = person_Employee.BirthPlace,
                Nation = person_Employee.Nation,
                Region = person_Employee.Region,
                Religion = person_Employee.Religion,
                MartialStatus = person_Employee.MartialStatus,
                Status = person_Employee.Status,
                HiredDate = person_Employee.HiredDate,
                FirstName = person_Employee.FirstName,
                MiddleName = person_Employee.MiddleName,
                LastName = person_Employee.LastName,
                Nationality = person_Employee.Nationality,
                DateOfBirth = person_Employee.DateOfBirth,
                Title = person_Employee.Title,
                Gender = person_Employee.Gender,
                CategoryID = person_Employee.CategoryID,
                IsActive = person_Employee.IsActive,
                Remark = person_Employee.Remark,
                DateCreated = person_Employee.DateCreated,
                LastModified = person_Employee.LastModified,
                CreatedBy = person_Employee.CreatedBy,
                ModifiedBy = person_Employee.ModifiedBy,
                OrgBranchName = person_Employee.OrgBranchName,
                FirstNameEnglish = person_Employee.FirstNameEnglish,
                MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                LastNameEnglish = person_Employee.LastNameEnglish,
                DateOfBirthAm = person_Employee.DateOfBirthAm,
                HiredDateAmharic = person_Employee.HiredDateAmharic,
                ProhabitionState = CheckProhabition(person_Employee.EmployeeId),
            });

            return Json(result);
        }
        private int CheckProhabition(int empId)
        {

            var employee = db.Person_Employee.Where(x => x.EmployeeId == empId).FirstOrDefault();

            if (employee != null)
            {
                if (employee.Status == EmployeeStatus.PROBATION)
                {
                    var prohabition = db.Prohibitions.Where(x => x.EmployeeId == empId).FirstOrDefault();
                    if (prohabition != null)
                    {
                        double dateDiff = (prohabition.EndDate - employee.HiredDate).TotalDays;
                        if (dateDiff >= 40 && dateDiff <= 45)
                        {
                            return 2;
                        }
                        if (dateDiff >= 45)
                        {
                            return 0;
                        }
                    }


                }
            }
            return 1;
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateOfBirthAm")
            {
                AmharicDateOfBirth = value;
                var splitDate = AmharicDateOfBirth.Split('/');
                DateOfBirth = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "HiredDateAmharic")
            {
                AmharicHireDate = value;
                var splitDate = AmharicHireDate.Split('/');
                HireDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
        }
        public void GetEmployeePhoto(IQueryable<Person_Employee> person_employee)
        {
            EmployeeIdView view = new EmployeeIdView();
            List<EmployeeIdView> views = new List<EmployeeIdView>();
            foreach (var person in person_employee)
            {
                view = new EmployeeIdView();
                Attachment att = db.Attachments.Where(a => a.Reference == person.EmployeeId.ToString() && a.ReferenceType == AttachmentType.PHOTO).FirstOrDefault();
                view.PersonId = person.EmployeeId;
                view.FullName = person.FirstName;
                if (att != null)
                {

                    view.URL = att.URL;
                }
                HRMInputs.EmployeeId = person.EmployeeId;
            }


        }
     
        [AuthorizeRoles( HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Person_Employee_Create([DataSourceRequest]DataSourceRequest request, Person_Employee person_Employee)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                int age = DateTime.Now.Year - DateOfBirth.Year;
                var checkExistance =
                    db.Person_Employee.Where(x => x.EmployeeCode == person_Employee.EmployeeCode).FirstOrDefault();
                if (checkExistance != null)
                {
                    ModelState.AddModelError("", "This Employee Id is already assigned for other employee!!");
                }
                else if (HireDate < DateOfBirth)
                {
                    ModelState.AddModelError("", "Date of birth should be less than hire date.");
                }
                else if (age < 18)
                {
                    ModelState.AddModelError("", "The Employee age should be 18 or more.");
                }
                else if (AmharicHireDate == null)
                {
                    ModelState.AddModelError("", "Hire date value is mandatory!!");
                }
                else if (AmharicDateOfBirth == null)
                {
                    ModelState.AddModelError("", "Date of birth date value is mandatory!!");
                }
                else if (HireDate.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Hire date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new Person_Employee
                    {

                        EmployeeId = person_Employee.EmployeeId,
                        EmployeeCode = person_Employee.EmployeeCode,
                        MothersFullName = person_Employee.MothersFullName,
                        BirthPlace = person_Employee.BirthPlace,
                        Nation = person_Employee.Nation,
                        Region = person_Employee.Region,
                        Religion = person_Employee.Religion,
                        MartialStatus = person_Employee.MartialStatus,
                        Status = person_Employee.Status,
                        HiredDate = HireDate,
                        FirstName = person_Employee.FirstName,//FirstName,
                        MiddleName = person_Employee.MiddleName, //MiddleName ,
                        LastName = person_Employee.LastName,//LastName
                        Nationality = person_Employee.Nationality,
                        DateOfBirth = DateOfBirth,
                        Title = person_Employee.Title,
                        Gender = person_Employee.Gender,
                        CategoryID = person_Employee.CategoryID,
                        IsActive = false,
                        Remark = person_Employee.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = person_Employee.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = person_Employee.ModifiedBy,
                        FirstNameEnglish = person_Employee.FirstNameEnglish,
                        MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                        LastNameEnglish = person_Employee.LastNameEnglish,
                        DateOfBirthAm = AmharicDateOfBirth,
                        HiredDateAmharic = AmharicHireDate,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db.Person_Employee.Add(entity);
                    db.SaveChanges();
                    person_Employee.EmployeeId = entity.EmployeeId;

                    #region operation log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                            UserHelper.OperationLog(Request.UserHostAddress, "Retired Employee", "Success", User.Identity.Name, "Employee Retirement Info for " + person.EmployeeCode + " is Created By  " + User.Identity.Name,Modules.HRM);
                        
                    }
                    #endregion
                }


            }

            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
        }
        private bool CheckValdationToDelete(int id)
        {
            ErrorMessage = "";
            #region Identification
            var identifications = db.Identifications.Where(x => x.Reference == id.ToString() && x.ReferenceType == "Employee").FirstOrDefault();
            if (identifications != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Identification information first.";
                return false;
            }
            #endregion
            #region Address
            var Address = db.Addresses.Where(x => x.Reference == id.ToString() && x.ReferenceType == "Employee").FirstOrDefault();
            if (Address != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Address information first.";
                return false;
            }
            #endregion

            #region Education
            var education = db.Education.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (education != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Educational information first.";
                return false;
            }
            #endregion
            #region Training
            var training = db.EmpTrainingEducations.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (training != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Training information first";
                return false;
            }
            #endregion
            #region competency
            var competency = db.Competencies.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (competency != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Competency information first";
                return false;
            }
            #endregion
            #region Emeregency
            var Emergencies = db.Emergencies.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (Emergencies != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Emergency information first";
                return false;
            }
            #endregion
            #region Assocation
            var Assocation = db.EmployeeAssociation.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (Assocation != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Assocation information first";
                return false;
            }
            #endregion
            #region Contract
            var Contract = db.Contract.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (Contract != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Contract information first";
                return false;
            }
            #endregion
            #region Experiences
            var Experiences = db.Experiences.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (Experiences != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Experience information first";
                return false;
            }
            #endregion
            #region Placments
            var Placments = db.Placements.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (Placments != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Placement information first";
                return false;
            }
            #endregion
            #region PlacmentsHistory
            var PlacmentsHistory = db.PlacementHistories.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (PlacmentsHistory != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Placement history information first";
                return false;
            }
            #endregion
            #region Commitments History
            var Commitments = db.Commitments.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (Commitments != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Commitment information first";
                return false;
            }
            #endregion
            #region Disablity History
            var Disabilities = db.Disabilities.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (Disabilities != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Disability information first";
                return false;
            }
            #endregion
            #region Cosigns History
            var Cosigns = db.Cosigns.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (Cosigns != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Cosign information first";
                return false;
            }
            #endregion
            #region Probation
            var Probation = db.Prohibitions.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (Probation != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Probation information first";
                return false;
            }
            #endregion
            #region Committees
            var Committees = db.CommitteeMembers.Where(x => x.EmployeeId == id).FirstOrDefault();

            if (Committees != null)
            {
                ErrorMessage = "You cannot delete this employee. Please Delete Committees information first";
                return false;
            }
            #endregion
            return true;
        }
      
        [AuthorizeRoles(HrmConstants.HRProfileUser)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Person_Employee_Update([DataSourceRequest]DataSourceRequest request, Person_Employee person_Employee)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                if (MiddleName == null) MiddleName = person_Employee.MiddleName;
                if (FirstName == null) FirstName = person_Employee.FirstName;
                if (LastName == null) LastName = person_Employee.LastName;
                if (AmharicDateOfBirth == null)
                {
                    AmharicDateOfBirth = person_Employee.DateOfBirthAm;
                    var splitDate = AmharicDateOfBirth.Split('/');
                    DateOfBirth = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (AmharicHireDate == null)
                {
                    AmharicHireDate = person_Employee.HiredDateAmharic;
                    var splitDate = AmharicHireDate.Split('/');
                    HireDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                int age = DateTime.Now.Year - DateOfBirth.Year;
                var checkExistance =
                    db.Person_Employee.Where(x => x.EmployeeCode == person_Employee.EmployeeCode).ToList();

                if (checkExistance.Any())
                {
                    var check = checkExistance.Where(x => x.EmployeeId == person_Employee.EmployeeId).FirstOrDefault();
                    if (check == null)
                    {


                        // RedirectToAction("Index");
                        ModelState.AddModelError("", "This Employee Id is already assigned for other employee!!");
                    }
                    else if (HireDate < DateOfBirth)
                    {
                        ModelState.AddModelError("", "Date of birth should be less than hire date.");
                    }
                    else if (age < 18)
                    {
                        ModelState.AddModelError("", "The Employee age should be 18 or more.");
                    }
                    else if (HireDate.Date > DateTime.Now.Date)
                    {

                        ModelState.AddModelError("", "Hire date should be less or equal to the current date!!");
                        return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.DenyGet);
                    }
                    else
                    {

                        var entity = new Person_Employee
                        {
                            EmployeeId = person_Employee.EmployeeId,
                            EmployeeCode = person_Employee.EmployeeCode,
                            MothersFullName = person_Employee.MothersFullName,
                            BirthPlace = person_Employee.BirthPlace,
                            Nation = person_Employee.Nation,
                            Region = person_Employee.Region,
                            Religion = person_Employee.Religion,
                            MartialStatus = person_Employee.MartialStatus,
                            Status = person_Employee.Status,
                            HiredDate = HireDate,
                            FirstName = person_Employee.FirstName,//FirstName,
                            MiddleName = person_Employee.MiddleName, //MiddleName ,
                            LastName = person_Employee.LastName,//LastName
                            Nationality = person_Employee.Nationality,
                            DateOfBirth = DateOfBirth,
                            Title = person_Employee.Title,
                            Gender = person_Employee.Gender,
                            CategoryID = person_Employee.CategoryID,
                            IsActive = person_Employee.IsActive,
                            Remark = "R-Active",
                            DateCreated = person_Employee.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = person_Employee.CreatedBy,
                            ModifiedBy = User.Identity.Name,
                            FirstNameEnglish = person_Employee.FirstNameEnglish,
                            MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                            LastNameEnglish = person_Employee.LastNameEnglish,
                            DateOfBirthAm = AmharicDateOfBirth,
                            OrgBranchName = person_Employee.OrgBranchName,
                            HiredDateAmharic = AmharicHireDate,
                        };
                        db = new ExceedDbContext();
                        db.Person_Employee.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();

                        #region operation log
                        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                        if (person != null)
                        {
                                UserHelper.OperationLog(Request.UserHostAddress, "Retired Employee", "Success", User.Identity.Name, "Employee Retirement Info of  " + person.EmployeeCode + " is Updated By  " + User.Identity.Name,Modules.HRM);
                            
                        }
                        #endregion
                    }
                }
                else
                {
                    if (HireDate < DateOfBirth)
                    {
                        ModelState.AddModelError("", "Date of birth should be less than hire date.");
                    }
                    else
                    {

                        if (MiddleName == null) MiddleName = person_Employee.MiddleName;
                        if (FirstName == null) FirstName = person_Employee.FirstName;
                        if (LastName == null) LastName = person_Employee.LastName;
                        if (AmharicDateOfBirth == null)
                        {
                            AmharicDateOfBirth = person_Employee.DateOfBirthAm;
                            var splitDate = AmharicDateOfBirth.Split('/');
                            DateOfBirth = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                        }
                        if (AmharicHireDate == null)
                        {
                            AmharicHireDate = person_Employee.HiredDateAmharic;
                            var splitDate = AmharicHireDate.Split('/');
                            HireDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                        }
                        var entity = new Person_Employee
                        {
                            EmployeeId = person_Employee.EmployeeId,
                            EmployeeCode = person_Employee.EmployeeCode,
                            MothersFullName = person_Employee.MothersFullName,
                            BirthPlace = person_Employee.BirthPlace,
                            Nation = person_Employee.Nation,
                            Region = person_Employee.Region,
                            Religion = person_Employee.Religion,
                            MartialStatus = person_Employee.MartialStatus,
                            Status = person_Employee.Status,
                            HiredDate = HireDate,
                            FirstName = FirstName,
                            MiddleName = MiddleName,
                            LastName = LastName,
                            Nationality = person_Employee.Nationality,
                            DateOfBirth = DateOfBirth,
                            Title = person_Employee.Title,
                            Gender = person_Employee.Gender,
                            CategoryID = person_Employee.CategoryID,
                            IsActive = person_Employee.IsActive,
                            Remark = "R-Active",
                            DateCreated = person_Employee.DateCreated,
                            LastModified = DateTime.Now,
                            CreatedBy = person_Employee.CreatedBy,
                            ModifiedBy = User.Identity.Name,
                            FirstNameEnglish = person_Employee.FirstNameEnglish,
                            MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                            LastNameEnglish = person_Employee.LastNameEnglish,
                            DateOfBirthAm = AmharicDateOfBirth,
                            OrgBranchName = person_Employee.OrgBranchName,
                            HiredDateAmharic = AmharicHireDate,

                        };
                        db = new ExceedDbContext();
                        db.Person_Employee.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();

                        #region operation log
                        var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                        if (person != null)
                        {
                            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                            var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                            if (name != null)
                            {
                                UserHelper.OperationLog(Request.UserHostAddress, "Retired Employee", "Success", User.Identity.Name, "Employee Retirement Info of " + person.EmployeeCode + " is Updated By  " + name.EmployeeCode,Modules.HRM);

                            }
                        }
                        #endregion
                    }

                }
            }


            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
        }
       
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Person_Employee_Destroy([DataSourceRequest]DataSourceRequest request, Person_Employee person_Employee)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!CheckValdationToDelete(person_Employee.EmployeeId))
                {

                    ModelState.AddModelError("", ErrorMessage);
                    return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.DenyGet);
                }
                else
                {
                    var entity = new Person_Employee
                    {
                        EmployeeId = person_Employee.EmployeeId,
                        EmployeeCode = person_Employee.EmployeeCode,
                        MothersFullName = person_Employee.MothersFullName,
                        BirthPlace = person_Employee.BirthPlace,
                        Nation = person_Employee.Nation,
                        Region = person_Employee.Region,
                        Religion = person_Employee.Religion,
                        MartialStatus = person_Employee.MartialStatus,
                        Status = person_Employee.Status,
                        HiredDate = person_Employee.HiredDate,
                        FirstName = person_Employee.FirstName,
                        MiddleName = person_Employee.MiddleName,
                        LastName = person_Employee.LastName,
                        Nationality = person_Employee.Nationality,
                        DateOfBirth = person_Employee.DateOfBirth,
                        Title = person_Employee.Title,
                        Gender = person_Employee.Gender,
                        CategoryID = person_Employee.CategoryID,
                        IsActive = person_Employee.IsActive,
                        Remark = person_Employee.Remark,
                        DateCreated = person_Employee.DateCreated,
                        LastModified = person_Employee.LastModified,
                        CreatedBy = person_Employee.CreatedBy,
                        ModifiedBy = person_Employee.ModifiedBy,
                        FirstNameEnglish = person_Employee.FirstNameEnglish,
                        MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                        LastNameEnglish = person_Employee.LastNameEnglish,
                        DateOfBirthAm = person_Employee.DateOfBirthAm,
                        HiredDateAmharic = person_Employee.HiredDateAmharic,
                        OrgBranchName = person_Employee.OrgBranchName,
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee information of employee Id" + person_Employee.EmployeeCode);
                    #endregion
                    db.Person_Employee.Attach(entity);
                    db.Person_Employee.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Employee Profile");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Employee Profile");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        public JsonResult GetNation()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.NATION && l.IsActive).AsQueryable();
            ViewData["NationList"] = lookups;
            return Json(lookups.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNationality()
        {
            var countries = db.Countries.AsQueryable();
            ViewData["Nationalites"] = countries;
            return Json(countries.Select(p => new { CountryId = p.CountryId, Nationality = p.Nationality }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Save(IEnumerable<HttpPostedFileBase> files, int id)
        {
            // The Name of the Upload component is "files"

            try
            {
                int i = 0;

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        byte[] imgData;
                        using (Stream inputStream = file.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }

                            imgData = memoryStream.ToArray();
                        }

                        var fileName = Path.GetFileName(file.FileName);
                        string physicalPath = Server.MapPath("~/Content/Images/" + fileName);
                        file.SaveAs(physicalPath);
                        var entity = new Attachment
                        {

                            Reference = id.ToString(),
                            Description = file.FileName,
                            FileType = fileName.Split('.')[1],
                            ReferenceType = (AttachmentType)Enum.Parse(typeof(AttachmentType), fileName.Split('.')[1]),
                            Index = i,
                            BinaryFile = imgData,
                            URL = physicalPath

                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        i++;
                    }

                    // Some browsers send file names with full path.
                    // We are only interested in the file name.

                }

            }
            catch (Exception)
            {

                // throw;
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Remove(string[] fileNames, int id)
        {
            // The parameter of the Remove action must be called "fileNames"
            var attachments = db.Attachments.Where(a => a.Reference == id.ToString());
            db = new ExceedDbContext();
            if (attachments.Count() > 0)
            {
                if (fileNames != null)
                {
                    foreach (var a in attachments)

                    {
                        foreach (var fullName in fileNames)
                        {

                            var fileName = Path.GetFileName(fullName);
                            if (a.Description == fileName)
                            {

                                if (System.IO.File.Exists(a.URL))
                                {
                                    // The files are not actually removed in this demo
                                    System.IO.File.Delete(a.URL);
                                }

                                db.Attachments.Attach(a);
                                db.Attachments.Remove(a);
                                db.SaveChanges();
                            }

                        }
                    }
                }

            }
            // Return an empty string to signify success
            return Content("");
        }
     
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName).Split('.')[1];
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                                    AttachmentType.PHOTO),
                            HRMInputs.EmployeeId.ToString() + "." + fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                                    AttachmentType.PHOTO));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (System.IO.File.Exists(path))
                    {

                        try
                        {
                            System.IO.File.Delete(path);
                        }
                        catch (System.IO.IOException e)
                        {
                            Console.WriteLine(e.Message);
                            //  return;
                        }
                    }
                    file.SaveAs(path);
                    var checkExistance =
                        db.Attachments.Where(
                            x =>
                                x.Reference == HRMInputs.EmployeeId.ToString() &&
                                x.Description == HRMInputs.EmployeeId.ToString() + "." + fileName &&
                                x.ReferenceType == AttachmentType.PHOTO).ToList();
                    if (checkExistance != null)
                    {
                        db.Attachments.RemoveRange(checkExistance);
                        db.SaveChanges();
                    }
                    db = new ExceedDbContext();
                    var entity = new Attachment
                    {

                        Reference = HRMInputs.EmployeeId.ToString(),
                        Description = HRMInputs.EmployeeId.ToString() + "." + fileName,
                        FileType = file.ContentType,
                        ReferenceType = AttachmentType.PHOTO,
                        Index = 0,
                        URL = path,
                        Remark = HRMInputs.Owner
                    };
                    db.Attachments.Add(entity);

                    db.SaveChanges();
                    ViewBag.Message = "Upload successful";
                }

                //}
                //else
                //{

                //    ViewBag.Message = "The file is existed";

                //}
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
     
        [AuthorizeRoles( HrmConstants.HRProfileUser)]
        public ActionResult UploadDocument(HttpPostedFileBase file, string url, string txtValue, string txtDecsion, string id)
        {
            try
            {
                HRMInputs.LetterId = id;
                HRMInputs.Owner = "Institution";
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId.ToString() + "-" +
                                           AttachmentType.DOCUMENT), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId.ToString() + "-" +
                                           AttachmentType.DOCUMENT));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {


                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = HRMInputs.LetterId.ToString(),
                            Description = file.FileName,
                            FileName = txtValue,
                            FileType = file.ContentType,
                            ReferenceType = AttachmentType.DOCUMENT,
                            Index = 0,
                            URL = path,
                            Remark = HRMInputs.Owner
                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        ViewBag.Message = "Upload successful";
                    }

                }
                else
                {

                    ViewBag.Message = "The file is existed";

                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
     
        [AuthorizeRoles(HrmConstants.HRProfileAdmin, HrmConstants.HRProfileUser)]
        public ActionResult Attachments_Destroy([DataSourceRequest]DataSourceRequest request, Attachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment
                {
                    AttachmentId = attachment.AttachmentId,
                    ReferenceType = attachment.ReferenceType,
                    Reference = attachment.Reference,
                    Description = attachment.Description,
                    URL = attachment.URL,
                    BinaryFile = attachment.BinaryFile,
                    FileType = attachment.FileType,
                    Index = attachment.Index,
                    Remark = attachment.Remark,
                    FileName = attachment.FileName
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Attachment" + attachment.ReferenceType.ToString() + " " + attachment.Description);
                #endregion
                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult FilterMenuCustomization_Cities()
        {
            var db = new ExceedDbContext();
            var emps = db.Person_Employee;
            return Json(emps.Select(p => new { Name = p.FirstName, Id = p.EmployeeId }), JsonRequestBehavior.AllowGet);

        }
        //[HttpPost]
        //public ActionResult EmployeeChart([DataSourceRequest]DataSourceRequest request, int id)
        //{
        //    var leaveList = db.Leaves.Where(x => x.EmployeeId == id).Count();
        //    int internalExpriance = 0;
        //    int externalExpriance = 0;
        //    var displineList = db.DisciplinaryCases.Where(x => x.EmployeeId == id).Count();
        //    var abcentList = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == id).Count();
        //    var externalExpriances = db.Experiences.Where(x => x.EmployeeId == id);
        //    if (externalExpriances.Any())
        //    {
        //        externalExpriance = Convert.ToInt32(externalExpriances.Sum(x => x.TotalDuration));
        //    }
        //    var employee = db.Person_Employee.Where(x => x.EmployeeId == id).FirstOrDefault();
        //    if (employee != null)
        //    {
        //        internalExpriance = (DateTime.Now.Year - employee.HiredDate.Year);
        //    }

        //    var viewModel = new List<EmployeeChartModel>();
        //    var data = new EmployeeChartModel();
        //    if (leaveList > 0)
        //    {
        //        //data.EmployeeChartModels = new List<EmployeeChartModel>();

        //        data.Category = "Leave";
        //        data.Value = leaveList;
        //        data.Color = "SteelBlue";
        //        //data.EmployeeChartModels.Add(data);
        //        viewModel.Add(data);
        //    }
        //    if (displineList > 0)
        //    {
        //        data = new EmployeeChartModel();
        //        //data.EmployeeChartModels = new List<EmployeeChartModel>();
        //        data.Category = "Dcipline";
        //        data.Value = displineList;

        //        data.Color = "LimeGreen";
        //        //data.EmployeeChartModels.Add(data);
        //        viewModel.Add(data);
        //    }
        //    if (abcentList > 0)
        //    {
        //        data = new EmployeeChartModel();
        //        //data.EmployeeChartModels = new List<EmployeeChartModel>();
        //        data.Category = "Abcent";
        //        data.Value = abcentList;

        //        data.Color = "OrangeRed";
        //        viewModel.Add(data);
        //        //data.EmployeeChartModels.Add(data);
        //    }
        //    data = new EmployeeChartModel();
        //    //data.EmployeeChartModels = new List<EmployeeChartModel>();
        //    data.Category = "Internal Expriance";
        //    data.Value = internalExpriance;

        //    data.Color = "MediumSlateBlue";
        //    //data.EmployeeChartModels.Add(data);
        //    viewModel.Add(data);
        //    data = new EmployeeChartModel();
        //    //data.EmployeeChartModels = new List<EmployeeChartModel>();
        //    data.Category = "External Expriance";
        //    data.Value = externalExpriance;

        //    data.Color = "Chocolate";
        //    //data.EmployeeChartModels.Add(data);
        //    viewModel.Add(data);
        //    return Json(viewModel);
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public ActionResult Employee_Import([DataSourceRequest] DataSourceRequest request,
           IEnumerable<HttpPostedFileBase> file)
        {
            string errors = "Employee Id";
            #region Import

            string addtionalName = DateTime.Now.Year.ToString() +
                DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                DateTime.Now.Second.ToString();
            System.Data.DataSet ds = new System.Data.DataSet();
            Person_Employee issuee = db.Person_Employee.FirstOrDefault();
            List<Person_Employee> issues = new List<Person_Employee>();
            foreach (HttpPostedFileBase fileBase in file)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                {
                    string fileLocation = Server.MapPath("~/Content/Import/HRM/") + addtionalName + Request.Files["file"].FileName;
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    Request.Files["file"].SaveAs(fileLocation);
                    var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                    if (fileExtension == ".xls")
                    {
                        excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                    }
                    //connection String for xlsx file format.
                    else if (fileExtension == ".xlsx")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    else if (fileExtension == ".xlsm")
                    {

                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                    }
                    //Create Connection to Excel work book and add oledb namespace
                    OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                    excelConnection.Open();
                    System.Data.DataTable dt = new System.Data.DataTable();

                    dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    if (dt == null)
                    {
                        return null;
                    }

                    String[] excelSheets = new String[dt.Rows.Count];
                    int t = 0;
                    //excel data saves in temp file here.
                    foreach (System.Data.DataRow row in dt.Rows)
                    {
                        excelSheets[t] = row["TABLE_NAME"].ToString();
                        t++;
                    }
                    OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);


                    string query = string.Format("Select * from [{0}]", excelSheets[0]);
                    using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                    {
                        dataAdapter.Fill(ds);
                    }

                }

            }
            #region List Of Enums

            var regions = Enum.GetValues(typeof(Region))
                .Cast<Region>()
                .ToList();
            var religionList = Enum.GetValues(typeof(ReligionList))
                .Cast<ReligionList>()
                .ToList();
            var martialStatusList = Enum.GetValues(typeof(MartialStatus))
              .Cast<MartialStatus>()
              .ToList();
            var employeeStatusList = Enum.GetValues(typeof(EmployeeStatus))
             .Cast<EmployeeStatus>()
             .ToList();
            var genderList = Enum.GetValues(typeof(Gender))
            .Cast<Gender>()
            .ToList();
            var personTitleList = db.Lookups.Where(x => x.LookupType == LookUpTypes.PERSON_TITLE).ToList();
            #endregion
            List<Person_Employee> issueImports = new List<Person_Employee>();
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                {
                    Person_Employee import = new Person_Employee();
                    import.EmployeeCode = ds.Tables[0].Rows[j].ItemArray[0].ToString();
                    if (personTitleList != null)
                    {
                        import.Title = personTitleList.Where(x => x.LookupId.ToString() == (ds.Tables[0].Rows[j].ItemArray[1].ToString()).Trim()).FirstOrDefault().LookupId;
                    }
                    import.Title = personTitleList.Where(x => x.LookupId.ToString() == (ds.Tables[0].Rows[j].ItemArray[1].ToString()).Trim()).FirstOrDefault().LookupId;
                    import.FirstName = ds.Tables[0].Rows[j].ItemArray[2].ToString();
                    import.MiddleName = ds.Tables[0].Rows[j].ItemArray[3].ToString();
                    import.LastName = ds.Tables[0].Rows[j].ItemArray[4].ToString();
                    import.Nationality = ds.Tables[0].Rows[j].ItemArray[5].ToString();
                    var splitDate = ds.Tables[0].Rows[j].ItemArray[18].ToString().Split('/');
                    DateOfBirth = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                    import.DateOfBirth = DateOfBirth;
                    import.Gender = genderList.Where(x => x.ToString() == (ds.Tables[0].Rows[j].ItemArray[6].ToString()).Trim()).FirstOrDefault();
                    import.IsActive = Convert.ToBoolean(ds.Tables[0].Rows[j].ItemArray[7].ToString());
                    import.MothersFullName = ds.Tables[0].Rows[j].ItemArray[7].ToString();
                    import.BirthPlace = ds.Tables[0].Rows[j].ItemArray[9].ToString();
                    import.Nation = ds.Tables[0].Rows[j].ItemArray[10].ToString();
                    import.Region = regions.Where(x => x.ToString() == (ds.Tables[0].Rows[j].ItemArray[11].ToString()).Trim()).FirstOrDefault();
                    import.Religion = religionList.Where(x => x.ToString() == (ds.Tables[0].Rows[j].ItemArray[11].ToString()).Trim()).FirstOrDefault();
                    import.MartialStatus = martialStatusList.Where(x => x.ToString() == (ds.Tables[0].Rows[j].ItemArray[13].ToString()).Trim()).FirstOrDefault();
                    import.Status = employeeStatusList.Where(x => x.ToString() == (ds.Tables[0].Rows[j].ItemArray[14].ToString()).Trim()).FirstOrDefault();
                    splitDate = ds.Tables[0].Rows[j].ItemArray[19].ToString().Split('/');
                    HireDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                    import.HiredDate = HireDate;
                    import.FirstNameEnglish = ds.Tables[0].Rows[j].ItemArray[15].ToString();
                    import.MiddleNameEnglish = ds.Tables[0].Rows[j].ItemArray[16].ToString();
                    import.LastNameEnglish = ds.Tables[0].Rows[j].ItemArray[17].ToString();
                    import.DateOfBirthAm = ds.Tables[0].Rows[j].ItemArray[18].ToString();
                    import.HiredDateAmharic = ds.Tables[0].Rows[j].ItemArray[19].ToString();
                    import.IsInsured = Convert.ToBoolean(ds.Tables[0].Rows[j].ItemArray[20].ToString());
                    import.Remark = ds.Tables[0].Rows[j].ItemArray[21].ToString();
                    issueImports.Add(import);
                    //  db.SaveChanges();
                }
            }
            foreach (var person_Employee in issueImports)
            {
                var checkExistance =
                    db.Person_Employee.Where(x => x.EmployeeCode == person_Employee.EmployeeCode).FirstOrDefault();
                if (checkExistance == null)
                {
                    var entity = new Person_Employee
                    {
                        EmployeeCode = person_Employee.EmployeeCode,
                        MothersFullName = person_Employee.MothersFullName,
                        BirthPlace = person_Employee.BirthPlace,
                        Nation = person_Employee.Nation,
                        Region = person_Employee.Region,
                        Religion = person_Employee.Religion,
                        MartialStatus = person_Employee.MartialStatus,
                        Status = person_Employee.Status,
                        HiredDate = person_Employee.HiredDate,
                        FirstName = person_Employee.FirstName, //FirstName,
                        MiddleName = person_Employee.MiddleName, //MiddleName ,
                        LastName = person_Employee.LastName, //LastName
                        Nationality = person_Employee.Nationality,
                        DateOfBirth = person_Employee.DateOfBirth,
                        Title = person_Employee.Title,
                        Gender = person_Employee.Gender,
                        CategoryID = 0,
                        IsActive = person_Employee.IsActive,
                        Remark = person_Employee.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = person_Employee.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = person_Employee.ModifiedBy,
                        FirstNameEnglish = person_Employee.FirstNameEnglish,
                        MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                        LastNameEnglish = person_Employee.LastNameEnglish,
                        DateOfBirthAm = person_Employee.DateOfBirthAm,
                        HiredDateAmharic = person_Employee.HiredDateAmharic,
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db.Person_Employee.Add(entity);
                    db.SaveChanges();
                    person_Employee.EmployeeId = entity.EmployeeId;
                }
                else

                {
                    errors += " " + person_Employee.EmployeeCode + ",";
                }
            }


            if (errors != "Employee Id")
            {
                ModelState.AddModelError("", errors + "is Exist");
            }
            #endregion

            return Json(new[] { issuee }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult DownloadTemplate(string filename)
        {


            db = new ExceedDbContext();
            try
            {

                WebClient req = new WebClient();
                HttpResponse response = System.Web.HttpContext.Current.Response;
                response.Clear();
                response.ClearContent();
                response.ClearHeaders();
                response.Buffer = true;
                response.AddHeader("Content-Disposition",
                    "attachment;filename=\"" + filename + "\"");
                byte[] datas = req.DownloadData(Server.MapPath("~/Content/Template/HRM/" + filename));
                response.BinaryWrite(datas);
                response.End();
                HRMInputs.SelectedFile = 0;
                return File(filename, "application/vnd.ms-excel");// attachment.FileType);
            }

            catch (Exception)
            {

                // throw;
            }
            HRMInputs.SelectedFile = 0;
            return null;
        }

    }

    public class EmployeeChartModel
    {
        public string Color { get; set; }
        public string Category { get; set; }
        public int Value { get; set; }

    }
    //public static class Filter
    //{
    //public static   DataSourceFilterDescriptorFactory<Person_Employee>      GetInitialFilters(this DataSourceFilterDescriptorFactory<Person_Employee> filter, Person_Employee model)
    //    {
    //        if (!model.FirstName.IsNullOrEmpty())
    //        {
    //            filter.Add(x => x.FirstName).Contains(model.FirstName);
    //        }

    //        if (!model.LastName.IsNullOrEmpty(model.LastName))
    //        {
    //            filter.Add(x => x.LastName).Contains(model.LastName);
    //        }

    //        if (!model.MiddleName.IsNullOrEmpty(model.MiddleName))
    //        {
    //            filter.Add(x => x.MiddleName).Contains(model.MiddleName);
    //        }

    //        // Additional filters redacted for brevity
    //        return filter;
    //    }
    //}
}
