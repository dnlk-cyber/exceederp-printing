using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{
  
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService)]
    public class PersonalProfileController : BaseController
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
            // ViewData["EmployeeList"]= inputs. GetEmployees(User.Identity.Name );
            GetNation();
            GetNationality();
            var skillTypes = db.Lookups.Where(l => l.LookupType == LookUpTypes.SKILL_TYPE);
            ViewData["SkillTypes"] = skillTypes;
           // CheckEmployeeRetirment();
        }
        //public void CheckEmployeeRetirment()
        //{
        //    int maxAge;
        //    var employees = db.Person_Employee.Where(x=>!x.IsReHired && x.Status != EmployeeStatus.CONTRACT);
        //    db = new ExceedDbContext();
        //    var setting = db.Configurations.Where(x => x.Reference == "PENSION" && x.Attribute == "RETIREMENT_AGE").FirstOrDefault();
        //    if (setting != null)
        //    {
        //        maxAge = Convert.ToInt32(setting.CurrentValue);
        //        foreach (var e in employees)
        //        {
        //            int age = DateTime.Now.Year - e.DateOfBirth.Year;
        //            if (age >= maxAge)
        //            {
        //                if (e.IsActive)
        //                {
        //                    db = new ExceedDbContext();
        //                    var entity = new Person_Employee
        //                    {
        //                        EmployeeId = e.EmployeeId,
        //                        EmployeeCode = e.EmployeeCode,
        //                        MothersFullName = e.MothersFullName,
        //                        BirthPlace = e.BirthPlace,
        //                        Nation = e.Nation,
        //                        Region = e.Region,
        //                        Religion = e.Religion,
        //                        MartialStatus = e.MartialStatus,
        //                        Status = e.Status,
        //                        HiredDate = e.HiredDate,
        //                        FirstName = e.FirstName,
        //                        MiddleName = e.MiddleName,
        //                        LastName = e.LastName,
        //                        Nationality = e.Nationality,
        //                        DateOfBirth = e.DateOfBirth,
        //                        Title = e.Title,
        //                        Gender = e.Gender,
        //                        CategoryID = e.CategoryID,
        //                        IsActive = false,
        //                        Remark = "Retirement ",
        //                        DateCreated = e.DateCreated,
        //                        LastModified = DateTime.Now,
        //                        CreatedBy = e.CreatedBy,
        //                        ModifiedBy = User.Identity.Name,
        //                        FirstNameEnglish = e.FirstNameEnglish,
        //                        MiddleNameEnglish = e.MiddleNameEnglish,
        //                        LastNameEnglish = e.LastNameEnglish,
        //                        DateOfBirthAm = e.DateOfBirthAm,
        //                        HiredDateAmharic = e.HiredDateAmharic,
        //                    };
        //                    db = new ExceedDbContext();
        //                    db.Person_Employee.Attach(entity);
        //                    db.Entry(entity).State = EntityState.Modified;
        //                    db.SaveChanges();
        //                }
        //            }
        //            else
        //            {
        //                if (e.IsActive == false && e.Remark == "Retirement")
        //                {
        //                    db = new ExceedDbContext();
        //                    var entity = new Person_Employee
        //                    {
        //                        EmployeeId = e.EmployeeId,
        //                        EmployeeCode = e.EmployeeCode,
        //                        MothersFullName = e.MothersFullName,
        //                        BirthPlace = e.BirthPlace,
        //                        Nation = e.Nation,
        //                        Region = e.Region,
        //                        Religion = e.Religion,
        //                        MartialStatus = e.MartialStatus,
        //                        Status = e.Status,
        //                        HiredDate = e.HiredDate,
        //                        FirstName = e.FirstName,
        //                        MiddleName = e.MiddleName,
        //                        LastName = e.LastName,
        //                        Nationality = e.Nationality,
        //                        DateOfBirth = e.DateOfBirth,
        //                        Title = e.Title,
        //                        Gender = e.Gender,
        //                        CategoryID = e.CategoryID,
        //                        IsActive = true,
        //                        Remark = "Retirement",
        //                        DateCreated = e.DateCreated,
        //                        LastModified = DateTime.Now,
        //                        CreatedBy = e.CreatedBy,
        //                        ModifiedBy = User.Identity.Name,
        //                        FirstNameEnglish = e.FirstNameEnglish,
        //                        MiddleNameEnglish = e.MiddleNameEnglish,
        //                        LastNameEnglish = e.LastNameEnglish,
        //                        DateOfBirthAm = e.DateOfBirthAm,
        //                        HiredDateAmharic = e.HiredDateAmharic,
        //                    };
        //                    db = new ExceedDbContext();
        //                    db.Person_Employee.Attach(entity);
        //                    db.Entry(entity).State = EntityState.Modified;
        //                    db.SaveChanges();
        //                }
        //            }
        //        }
        //    }

        //}
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
        public ActionResult Person_Employee_Read([DataSourceRequest]DataSourceRequest request)
        {
            var users = new ApplicationDbContext();
            int id = 0;
            var db = new ExceedDbContext();
            var user = users.Users.Where(x => x.UserName == User.Identity.Name).FirstOrDefault();
            if (user != null)
            {
                if (user.EmployeeId != null)
                {
                    int empid = Convert.ToInt32(user.EmployeeId);
                    var employes = db.Person_Employee.Where(x => x.EmployeeId == empid).FirstOrDefault();
                    if (employes != null)
                    {
                        id = employes.EmployeeId;
                    }
                }
            }
            IQueryable<Person_Employee> person_employee = db.Person_Employee.Where(x => x.EmployeeId == id);
            if (request.Filters != null)
            {
                dd: if (request.Filters.Any())
                {
                    foreach (var filter in request.Filters)
                    {
                        var descriptor = filter as FilterDescriptor;
                        if (descriptor != null)
                        {
                            if (descriptor.Member == "LastName" || descriptor.Member == "EmployeeCode" || descriptor.Member == "MiddleName" || descriptor.Member == "FirstName")
                            {
                                person_employee = ModifyFilters(request.Filters, person_employee).AsQueryable();

                            }

                        }
                        else if (filter is CompositeFilterDescriptor)
                        {

                            request.Filters = ((CompositeFilterDescriptor)filter).FilterDescriptors;
                            goto dd;
                            //ModifyFilters(((CompositeFilterDescriptor) filter).FilterDescriptors, person_employee);

                        }
                    }
                    //request.Filters.Clear();
                }
            }


            DataSourceResult result = person_employee.ToDataSourceResult(request, person_Employee => new
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
                        FirstName = FirstName,
                        MiddleName = MiddleName,
                        LastName = LastName,
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
                            FirstName = FirstName,
                            MiddleName = MiddleName,
                            LastName = LastName,
                            Nationality = person_Employee.Nationality,
                            DateOfBirth = DateOfBirth,
                            Title = person_Employee.Title,
                            Gender = person_Employee.Gender,
                            CategoryID = person_Employee.CategoryID,
                            IsActive = person_Employee.IsActive,
                            Remark = person_Employee.Remark,
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
                            Remark = person_Employee.Remark,
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
                    }

                }
            }


            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Person_Employee_Destroy([DataSourceRequest]DataSourceRequest request, Person_Employee person_Employee)
        {
            if (ModelState.IsValid)
            {
                if (!CheckValdationToDelete(person_Employee.EmployeeId))
                {

                    ModelState.AddModelError("", ErrorMessage);
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
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Employee " + person_Employee.EmployeeCode);
                    #endregion
                    db.Person_Employee.Attach(entity);
                    db.Person_Employee.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
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
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.PHOTO), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.PHOTO));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {


                        file.SaveAs(path);

                        var entity = new Attachment
                        {

                            Reference = HRMInputs.EmployeeId.ToString(),
                            Description = file.FileName,
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
        public ActionResult UploadDocument(HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {



                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
                                           AttachmentType.DOCUMENT), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" +
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

                            Reference = HRMInputs.EmployeeId.ToString(),
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

                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
