using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using ExceedERP.Core.Domain.Global;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
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
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Web.Hubs;
using ExceedERP.Helpers.HR;
using ExceedERP.Core.Domain.ArchiveManagement;

namespace ExceedERP.Web.Areas.HRM.Controllers.Profile
{

    [AuthorizeRoles(HrmConstants.HRProfileUser, HrmConstants.HRProfileAdmin)]
    public class EmployeeProfileController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        string ErrorMessage = "";
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            ViewData["JobTitleAutoComplate"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).Select(x => x.Description).ToList();
            ViewData["JobTitleExternalExperience"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.Jobtitle_External_Experience).ToList();
            var commtTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COMMITMENT_TYPE);
            ViewData["commitment"] = commtTypes;
            var cosignTypes = db.Lookups.Where(x => x.LookupType == LookUpTypes.COSIGN_TYPE);
            ViewData["CosignType"] = cosignTypes;

            ViewData["Measures"] = db.DisciplineMeasures;
            ViewData["MeasuresDetails"] = db.DisciplineMeasuresDetail;

            GetEducationFiled();
            GetEducationLevel();
            SetInitialDatas();
            ViewData["Branch"] = db.Branch.Select(x => new { x.BranchId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") }).ToList();

            ViewData["OldJobTitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.OLD_JOB_TITLE || x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            ViewData["jobTitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            //var branches = db.Branch;
            //ViewData["branches"] = branches;
            //var workunits = db.MainCompanyStructures.ToList();
            //ViewData["workunits"] = workunits;
            var branches = db.Branch.Select(x => new { x.BranchId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["branches"] = branches;
            var businessUnits = db.BusinessUnits.Select(x => new { x.BusinessUnitId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") });
            ViewData["businessUnits"] = businessUnits;
            var workunits = db.MainCompanyStructures.Select(x => new { x.OrgStructureId, Name = x.Name.Replace("\n", "").Replace("\r", "").Replace("#", "").Replace("+", " ") }).ToList();
            ViewData["workunits"] = workunits;
            ViewData["employeeType"] = db.EmployeeTypes.Select(x => new { x.EmployeeTypeId, x.Name }).AsQueryable();
            GetPlacementAfterSystem();

            return View();
        }
        private int CalculateProbationAmount(DateTime hiringDate )
        {
            int probationAmount = 0;
            var employeeProbationSetting = db.EmployeeProbationSettings.FirstOrDefault();
            if(employeeProbationSetting !=null && employeeProbationSetting.IncludeWeekend&&employeeProbationSetting.IncludeHoliday)
            {
                for (DateTime currentDate = hiringDate; probationAmount <= 60 && currentDate.Date <= DateTime.Now.Date; currentDate = currentDate.AddDays(1))
                {
                    // including weekends (Saturday and Sunday) and holidays
                    probationAmount++;


                  

                }
            }
            if (employeeProbationSetting !=null && !employeeProbationSetting.IncludeWeekend&&employeeProbationSetting.IncludeHoliday)
            {
               
                for (DateTime currentDate = hiringDate; probationAmount <= 60 && currentDate.Date <= DateTime.Now.Date; currentDate = currentDate.AddDays(1))
                {

                    // Exclude weekends (Saturday and Sunday) and  include holliday
                    if (currentDate.DayOfWeek != DayOfWeek.Saturday && currentDate.DayOfWeek != DayOfWeek.Sunday)
                    {
                        probationAmount++;


                    }
                   




                }
            }
            if (employeeProbationSetting !=null && employeeProbationSetting.IncludeWeekend&& !employeeProbationSetting.IncludeHoliday)
            {
                List<DateTime> holidays = db.Holidays.Select(x => x.Date).ToList();
                for (DateTime currentDate = hiringDate; probationAmount <= 60 && currentDate.Date <= DateTime.Now.Date; currentDate = currentDate.AddDays(1))
                {
                    // include  weekends (Saturday and Sunday) and exclude  holidays

                    if (!holidays.Contains(currentDate))
                    {
                        probationAmount++;


                    }




                }
            }

            else
            {
                List<DateTime> holidays = db.Holidays.Select(x => x.Date).ToList();
                for (DateTime currentDate = hiringDate; probationAmount <= 60 && currentDate.Date <= DateTime.Now.Date; currentDate = currentDate.AddDays(1))
                {
                    // Exclude weekends (Saturday and Sunday) and holidays
                    if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
                    currentDate.DayOfWeek != DayOfWeek.Sunday &&
                    !holidays.Contains(currentDate))
                    {
                        probationAmount++;


                    }

                }

            }
            

            return probationAmount;
        }
        public void GetPlacementAfterSystem()
        {
            ViewData["branches"] = db.Branch.Select(x=>new {x.BranchId,x.Name }).ToList();
            ViewData["workunits"] = db.MainCompanyStructures.Select(x => new {x.OrgStructureId,x.Name }).ToList();
            ViewData["Jobtitle"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).Select(x => new {x.LookupId,x.Description }).ToList();
            ViewData["RankList"] = db.Ranks.Select(x => new {x.RankId,x.RankName}).ToList();
            ViewData["StepList"] = db.Steps.Select(x => new {x.StepId,x.StepName }).ToList();
        }
        public JsonResult GenerateProfile(string id, string text)
        {
            int EmployeeId = Convert.ToInt32(id);
            string NewEmployeeCode = text;

            string EmpCodeGenerated = GenerateEmployeeCode(DateTime.Today);

            if (!string.IsNullOrEmpty(EmpCodeGenerated))
            {
                NewEmployeeCode = EmpCodeGenerated;
            }

            int EmployeeIdNew = 0;

            if (id != null && !string.IsNullOrEmpty(NewEmployeeCode))
            {
                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == EmployeeId);
                var educations = db.Education.Where(x => x.EmployeeId == EmployeeId).ToList();
                var experiences = db.Experiences.Where(x => x.EmployeeId == EmployeeId).ToList();
                var trainings = db.EmpTrainingEducations.Where(x => x.EmployeeId == EmployeeId).ToList();
                var languages = db.LanguageOfProficiencies.Where(x => x.EmployeeId == EmployeeId).ToList();
                var competencies = db.Competencies.Where(x => x.EmployeeId == EmployeeId).ToList();
                var associasions = db.EmployeeAssociation.Where(x => x.EmployeeId == EmployeeId).ToList();
                var emergencies = db.Emergencies.Where(x => x.EmployeeId == EmployeeId).ToList();
                var disabilities = db.Disabilities.Where(x => x.EmployeeId == EmployeeId).ToList();
                var addresses = db.Addresses.Where(x => x.Reference == EmployeeId.ToString()).ToList();
                var identifications = db.Identifications.Where(x => x.Reference == EmployeeId.ToString()).ToList();
                var attachmentsPhoto = db.Attachments.Where(x => x.Reference == EmployeeId.ToString() && x.ReferenceType == AttachmentType.PHOTO && x.Remark == EmployeeId.ToString()).ToList();
                var attachmentsDocument = db.Attachments.Where(x => x.Reference == EmployeeId.ToString() && x.ReferenceType == AttachmentType.DOCUMENT && x.Remark == EmployeeId.ToString()).ToList();
                var contracts = db.Contract.Where(x => x.EmployeeId == EmployeeId).ToList();

                if (employee != null && !employee.IsActive)
                {
                    var person = new Person_Employee
                    {
                        EmployeeCode = NewEmployeeCode,
                        MothersFullName = employee.MothersFullName,
                        BirthPlace = employee.BirthPlace,
                        Nation = employee.Nation,
                        Region = employee.Region,
                        Religion = employee.Religion,
                        MartialStatus = employee.MartialStatus,
                        Status = employee.Status,
                        HiredDate = employee.HiredDate,
                        FirstName = employee.FirstName,//FirstName,
                        MiddleName = employee.MiddleName, //MiddleName ,
                        LastName = employee.LastName,//LastName
                        Nationality = employee.Nationality,
                        DateOfBirth = employee.DateOfBirth,
                        Title = employee.Title,
                        Gender = employee.Gender,
                        CategoryID = employee.CategoryID,
                        IsActive = true,
                        Remark = employee.Remark,
                        DateCreated = DateTime.Now,
                        LastModified = employee.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = employee.ModifiedBy,
                        //DateCreatedAM = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year),
                        //DateLastModifiedAM = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year),
                        FirstNameEnglish = employee.FirstNameEnglish,
                        MiddleNameEnglish = employee.MiddleNameEnglish,
                        LastNameEnglish = employee.LastNameEnglish,
                        DateOfBirthAm = XAPI.EthiopicDateTime.GetEthiopicDate(employee.DateOfBirth.Day, employee.DateOfBirth.Month, employee.DateOfBirth.Year),
                        HiredDateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year),
                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    };

                    db.Person_Employee.Add(person);
                    db.SaveChanges();

                    EmployeeIdNew = person.EmployeeId;



                    #region Education
                    foreach (var edu in educations)
                    {
                        var education = new Education
                        {
                            EmployeeId = EmployeeIdNew,
                            OrganizationName = edu.OrganizationName,
                            OrgAddress = edu.OrgAddress,
                            From = edu.From,
                            To = edu.To,
                            Score = edu.Score,
                            EduLevel = edu.EduLevel,
                            EduField = edu.EduField,
                            TotalCost = edu.TotalCost,
                            PercentCoverdByOrg = edu.PercentCoverdByOrg,
                            Remark = edu.Remark,
                            FromEAmharic = edu.FromEAmharic,
                            ToEAmharic = edu.ToEAmharic,
                            IsHighLevel = edu.IsHighLevel,
                        };

                        db.Education.Add(education);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Experience
                    foreach (var exp in experiences)
                    {
                        var experience = new Experience
                        {
                            EmployeeId = EmployeeIdNew,
                            Organozation = exp.Organozation,
                            OrgFullAddress = exp.OrgFullAddress,
                            EmploymentType = exp.EmploymentType,
                            From = exp.From,
                            To = exp.To,
                            TotalDuration = exp.TotalDuration,
                            TotalExp = exp.TotalExp,
                            Responsibility = exp.Responsibility,
                            ClearanceDateTime = exp.ClearanceDateTime,
                            CleranceReason = exp.CleranceReason,
                            IncomeTaxPaymentRefNo = exp.IncomeTaxPaymentRefNo,
                            Remark = exp.Remark,
                            Salary = exp.Salary,
                            EmployeeCode = exp.EmployeeCode,
                            FromExAmharic = exp.FromExAmharic,
                            ToExAmharic = exp.ToExAmharic,
                            ClearanceDateTimeAm = exp.ClearanceDateTimeAm,
                            IsPrivate = exp.IsPrivate,
                        };

                        db.Experiences.Add(experience);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Training
                    foreach (var tr in trainings)
                    {
                        var training = new Training
                        {
                            OrganizationName = tr.OrganizationName,
                            EmployeeId = EmployeeIdNew,
                            OrgAddress = tr.OrgAddress,
                            IsTraining = true,
                            From = tr.From,
                            To = tr.To,
                            Score = tr.Score,
                            TotalCost = tr.TotalCost,
                            TrainingName = tr.TrainingName,
                            PercentCoverdByOrg = tr.PercentCoverdByOrg,
                            Remark = tr.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = tr.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = tr.ModifiedBy,
                            FromTrAm = tr.FromTrAm,
                            ToTraAm = tr.ToTraAm
                        };

                        db.EmpTrainingEducations.Add(training);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Language
                    foreach (var lg in languages)
                    {
                        var language = new LanguageProficiency
                        {
                            EmployeeId = EmployeeIdNew,
                            LanguageId = lg.LanguageId,
                            Reading = lg.Reading,
                            Listening = lg.Listening,
                            Writing = lg.Writing,
                            Speaking = lg.Speaking,
                            Remark = lg.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = lg.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = lg.ModifiedBy
                        };

                        db.LanguageOfProficiencies.Add(language);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Competency
                    foreach (var cmp in competencies)
                    {
                        var comptncy = new Competency
                        {
                            EmployeeId = EmployeeIdNew,
                            Level = cmp.Level,
                            LookupId = cmp.LookupId,
                            Status = cmp.Status,
                            From = cmp.From,
                            To = cmp.To,
                            Remark = cmp.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = cmp.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = cmp.ModifiedBy,
                            FromComAm = cmp.FromComAm,
                            ToCompAm = cmp.ToCompAm,
                        };

                        db.Competencies.Add(comptncy);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Association
                    foreach (var assos in associasions)
                    {
                        var association = new EmployeeAssociation
                        {
                            AssociationId = assos.AssociationId,
                            Name = assos.Name,
                            LookupId = assos.LookupId,
                            EmployeeId = EmployeeIdNew,

                            Remark = assos.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = assos.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = assos.ModifiedBy
                        };
                        db.EmployeeAssociation.Add(association);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Emergencies
                    foreach (var em in emergencies)
                    {
                        var emergency = new Emergency
                        {
                            EmergencyId = em.EmergencyId,
                            Name = em.Name,
                            LookupId = em.LookupId,
                            EmployeeId = EmployeeIdNew,
                            HomePhoneNo = em.HomePhoneNo,
                            MobilePhoneNo = em.MobilePhoneNo,
                            WorkPhoneno = em.WorkPhoneno,
                            Remark = em.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = em.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = em.ModifiedBy
                        };

                        db.Emergencies.Add(emergency);
                        db.SaveChanges();
                    }
                    #endregion

                    #region Disabilities
                    foreach (var disab in disabilities)
                    {
                        var disability = new Disability
                        {
                            EmployeeId = EmployeeIdNew,
                            Type = disab.Type,
                            Remark = disab.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = disab.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = disab.ModifiedBy
                        };

                        db.Disabilities.Add(disability);
                        db.SaveChanges();

                    }
                    #endregion

                    #region Address
                    foreach (var add in addresses)
                    {
                        var address = new Address
                        {
                            Reference = EmployeeIdNew.ToString(),
                            Category = add.Category,
                            Value = add.Value,
                            ReferenceType = add.ReferenceType,
                            Remark = add.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = add.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = add.ModifiedBy
                        };

                        db.Addresses.Add(address);
                        db.SaveChanges();

                    }
                    #endregion

                    #region Identification
                    foreach (var iden in identifications)
                    {
                        var identification = new Identification
                        {
                            Reference = EmployeeIdNew.ToString(),
                            ReferenceType = iden.ReferenceType,
                            Description = iden.Description,
                            IdNumber = iden.IdNumber,
                            Type = iden.Type,
                            IssueDate = iden.IssueDate,
                            ExpiryDate = iden.ExpiryDate,
                            Issuer = iden.Issuer,
                            Remark = iden.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = iden.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = iden.ModifiedBy,
                            IssueDateIdAm = iden.IssueDateIdAm,
                            ExpiryDateAm = iden.ExpiryDateAm
                        };

                        db.Identifications.Add(identification);
                        db.SaveChanges();

                    }
                    #endregion

                    #region Attachment Photo
                    foreach (var atach in attachmentsPhoto)
                    {
                        var entity = new Attachment
                        {
                            ReferenceType = atach.ReferenceType,
                            Reference = EmployeeIdNew.ToString(),
                            Description = atach.Description,
                            URL = atach.URL,
                            BinaryFile = atach.BinaryFile,
                            FileType = atach.FileType,
                            Index = atach.Index,
                            Remark = EmployeeIdNew.ToString(),
                            FileName = atach.FileName
                        };

                        db.Attachments.Add(entity);
                        db.SaveChanges();

                    }
                    #endregion

                    #region Attachment Document
                    foreach (var atachD in attachmentsDocument)
                    {
                        var entity = new Attachment
                        {
                            ReferenceType = atachD.ReferenceType,
                            Reference = EmployeeIdNew.ToString(),
                            Description = atachD.Description,
                            URL = atachD.URL,
                            BinaryFile = atachD.BinaryFile,
                            FileType = atachD.FileType,
                            Index = atachD.Index,
                            Remark = EmployeeIdNew.ToString(),
                            FileName = atachD.FileName
                        };

                        db.Attachments.Add(entity);
                        db.SaveChanges();

                    }
                    #endregion

                    #region Contracts
                    foreach (var cont in contracts)
                    {
                        var contract = new Contract
                        {
                            ContractId = cont.ContractId,
                            From = cont.From,
                            To = cont.To,
                            MonthlySalary = cont.MonthlySalary,
                            EmployeeId = EmployeeIdNew,
                            Remark = cont.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = cont.LastModified,
                            CreatedBy = User.Identity.Name,
                            ModifiedBy = cont.ModifiedBy,
                            FromContAm = cont.FromContAm,
                            ToContAm = cont.ToContAm
                        };

                        db.Contract.Add(contract);
                        db.SaveChanges();

                    }
                    #endregion
                }
                else if (employee.IsActive)
                {
                    var duplication = new
                    {
                        Message = "This Employee is active can't duplicate",
                        Status = "Fail"
                    };

                    return Json(duplication, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var nulReturn = new
                    {
                        Message = "No such employee",
                        Status = "Fail"
                    };

                    return Json(nulReturn, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var formatedData1 = new
                {
                    Message = "First enter the new ID",
                    Status = "OK"
                };

                return Json(formatedData1, JsonRequestBehavior.AllowGet);
            }

            var formatedData2 = new
            {
                Message = "Successfully Generated",
                Status = "OK"
            };

            return Json(formatedData2, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Refresh()
        {
            var allEmployee = db.Person_Employee.ToList();
            var placements = db.Placements.ToList();

            foreach (var p in allEmployee)
            {
                try
                {
                    if (p.IsActive)
                    {
                        var placement = placements.FirstOrDefault(x => x.EmployeeId == p.EmployeeId && x.isCurrent);
                        if (placement != null)
                        {
                            p.BranchId = placement.BranchId;
                            p.OrgBranchName = placement.BranchId.ToString();

                            db.Entry(p).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        var placement = placements.Where(x => x.EmployeeId == p.EmployeeId).OrderByDescending(x => x.AssignmentDate).FirstOrDefault();
                        if (placement != null)
                        {
                            p.BranchId = placement.BranchId;
                            p.OrgBranchName = placement.OrgBranchName.ToString();

                            db.Entry(p).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                catch(Exception ex)
                {
                    var formatedData1 = new
                    {
                        Message = $"Error occured on Emplyee {p.EmployeeCode} - {ex.Message}",
                        Status = "NOT_OK"
                    };
                    return Json(formatedData1, JsonRequestBehavior.AllowGet);
                }
            }
            
            var formatedData = new
            {
                Message = "Branch successfully updated",
                Status = "OK"
            };
            return Json(formatedData, JsonRequestBehavior.AllowGet);
            //var messages = "branch successfully refreshed";

            //return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            ////return RedirectToAction("Index");

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
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    var fileExtension = Path.GetFileName(file.FileName).Split('.')[1];
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.PHOTO), HRMInputs.EmployeeId.ToString() + "." + fileExtension);
                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.PHOTO));

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
                    var employeeCode = db.Person_Employee.Find(HRMInputs.EmployeeId);

                    var checkExistance = db.Attachments.Where(x => x.Reference == HRMInputs.EmployeeId.ToString() && x.Description == HRMInputs.EmployeeId.ToString() + "." + fileExtension && x.ReferenceType == AttachmentType.PHOTO).ToList();
                    if (checkExistance != null)
                    {
                        db.Attachments.RemoveRange(checkExistance);
                        db.SaveChanges();
                    }
                    db = new ExceedDbContext();
                    var entity = new Attachment
                    {

                        Reference = HRMInputs.EmployeeId.ToString(),
                        Description = employeeCode !=null? employeeCode.EmployeeCode + "." + fileExtension: file.FileName,
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
        public ActionResult UploadDocument(HttpPostedFileBase file, string txtValue)
        {
            try
            {
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT), HRMInputs.EmployeeId.ToString() + "-" + txtValue + "-" + fileName);

                    var mainPath = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.DOCUMENT));

                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {
                        file.SaveAs(path);
                        var employeeCode = db.Person_Employee.Find(HRMInputs.EmployeeId);

                        var entity = new Attachment
                        {

                            Reference = HRMInputs.EmployeeId.ToString(),
                            Description = employeeCode != null ? employeeCode.EmployeeCode  + "-" + txtValue + "-" + fileName: txtValue + "-" + fileName,
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
        public ActionResult DownloadFile(string filename, string attachId)
        {
            if (attachId == "0")
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            try
            {
                int.TryParse(attachId, out int attachmentId);

                var attachment = db.Attachments.FirstOrDefault(x => x.Reference == filename && x.ReferenceType == AttachmentType.PHOTO && x.AttachmentId == attachmentId);

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
        public ActionResult DownloadDoument(string filename, string attachId)
        {
            if (attachId == "0")
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            //try
            //{
            int.TryParse(attachId, out int attachmentId);
            var attachment =
                    db.Attachments.FirstOrDefault(x => x.Reference == filename && x.ReferenceType == AttachmentType.DOCUMENT && x.AttachmentId == attachmentId);

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

                // byte[] datas11 = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.EmployeeId.ToString() + "-" + AttachmentType.PHOTO + "/" + attachment.Description));

                response.BinaryWrite(datas);
                response.End();
                HRMInputs.SelectedFile = 0;
                return File(filename, attachment.FileType);
            }
            //}
            //catch (Exception)
            //{

            //    // throw;
            //}
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

            ViewData["PeriodList"] = db.GLPeriods.ToList();// periodList != null ? periodList.ToList() : new List<Period>();

            ViewData["Relationship"] = relations != null ? relations.ToList() : new List<Lookup>();
            ViewData["EmployeeList"] = db.Person_Employee.ToList();//inputs.GetEmployees(User.Identity.Name);
            GetNation();
            GetNationality();
            var skillTypes = db.Lookups.Where(l => l.LookupType == LookUpTypes.SKILL_TYPE);
            ViewData["SkillTypes"] = skillTypes;
            // CheckEmployeeRetirment();
        }
        public void CheckEmployeeRetirment()
        {
            int maxAge;
            List<string> employeesList = inputs.GetUserBranchEmployees(User.Identity.Name);
            var employees = db.Person_Employee.Where(x => employeesList.Contains(x.EmployeeId.ToString())).Where(x => x.IsActive && !x.IsReHired && x.Remark != "R-Active" && x.Status != EmployeeStatus.CONTRACT);
            db = new ExceedDbContext();
            bool isActive = true;
            var terminations = db.Terminations;
            var setting = db.Configurations.FirstOrDefault(x => x.Reference == "PENSION" && x.Attribute == "RETIREMENT_AGE");
            if (setting != null)
            {
                maxAge = Convert.ToInt32(setting.CurrentValue);
                foreach (var e in employees)
                {
                    double age = DateTime.Now.Date.Subtract(new DateTime(e.DateOfBirth.Year, e.DateOfBirth.Month, e.DateOfBirth.Day)).TotalDays / 365.25;
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
                                Remark = "Retirement ",
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
        public bool Probation(int id)
        {
            var personEmploye = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id && x.IsActive);
            if (personEmploye != null)
            {
                int probationAmount = 0;
                // var  ProbationDays = db.ProbationPerformanceSettings.FirstOrDefault()?.EndWeight;
                List<DateTime> holidays = db.Holidays.Select(x => x.Date).ToList();
                for (DateTime currentDate = personEmploye.HiredDate; probationAmount <= 60 && currentDate.Date <= DateTime.Now.Date; currentDate = currentDate.AddDays(1))
                {
                    // Exclude weekends (Saturday and Sunday) and holidays
                    if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
                    currentDate.DayOfWeek != DayOfWeek.Sunday &&
                    !holidays.Contains(currentDate))
                    {
                        probationAmount++;


                    }

                }
                if(probationAmount >= 60)
                {
                    return true;
                }
                return false;
                //int probDate = (DateTime.Now - personEmploye.HiredDate).Days;
                //if (probDate > 40)
                //{
                //    return true;
                //}
            }
            return false;
        }
        public int Branch(int id)
        {
            var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == id && x.isCurrent);
            if (placement != null)
            {
                return placement.BranchId;
            }
            return 0;
        }
        public ActionResult UpdateExperience()
        {


            var experiences = db.Experiences.ToList();

            foreach (var ex in experiences)
            {


                double month = (ex.To - ex.From).Days / 30;

                var y = (int)month / 12;

                var ym = month % 12;

                var exp = y + " አመት ከ " + ym + " ወር";


                ex.TotalDuration = y;
                ex.TotalExp = exp;


                db.Entry(ex).State = EntityState.Modified;
                db.SaveChanges();

            }

            return RedirectToAction("Index");

        }
        public int? GetContractLeftDay(int id)
        {
            var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id && x.IsActive && x.Status == EmployeeStatus.CONTRACT);
            if (profile != null)
            {
                var contract = db.Contract.Where(x => x.EmployeeId == profile.EmployeeId).ToList().OrderByDescending(x => x.To).FirstOrDefault();
                if (contract != null)
                {
                    int leftDay = (contract.To - DateTime.Today).Days;

                    return leftDay;
                }
            }

            return null;
        }
        private void UpdateEmployeeBranch(List<Placement> placments)
        {
            foreach (var placement in placments)
            {
                var employee = db.Person_Employee.FirstOrDefault(p => p.EmployeeId == placement.EmployeeId);
                if (employee != null)
                {
                    employee.Status = placement.EmployementType;
                    employee.OrgBranchName = placement.BranchId.ToString();
                    employee.ModifiedBy = User.Identity.Name;
                    employee.LastModified = DateTime.Now;

                    db.Entry(employee).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
        private int CheckProhabition(int empId)
        {
            
            int probationAmount = 0;
            var employee = db.Person_Employee.Where(x => x.EmployeeId == empId).FirstOrDefault();

            if (employee != null)
            {
                if (employee.Status == EmployeeStatus.PROBATION)
                {
                    var prohabition = db.Prohibitions.Where(x => x.EmployeeId == empId).FirstOrDefault();
                    if (prohabition != null)
                    {
                        List<DateTime> holidays = db.Holidays.Select(x => x.Date).ToList();
                        for (DateTime currentDate = employee.HiredDate; probationAmount <= 60 && currentDate.Date <= prohabition.EndDate.Date; currentDate = currentDate.AddDays(1))
                        {
                            // Exclude weekends (Saturday and Sunday) and holidays
                            if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
                            currentDate.DayOfWeek != DayOfWeek.Sunday &&
                            !holidays.Contains(currentDate))
                            {
                                probationAmount++;


                            }

                        }

                        if (probationAmount < 60)
                        {
                            return 2;
                        }
                        if (probationAmount >= 60)
                        {
                            return 0;
                        }
                        //double dateDiff = (prohabition.EndDate - employee.HiredDate).TotalDays;

                        //if (dateDiff >= 40 && dateDiff <= 45)
                        //{
                        //    return 2;
                        //}
                        //if (dateDiff >= 45)
                        //{
                        //    return 0;
                        //}
                    }


                }
            }
            return 1;
        }
        private List<int> GetEmployeeIdInProhabition(List<Person_Employee> employees)
        {
            List<int> employeeIds = new List<int>(); 
            int probationAmount = 0;
            employees = employees.Where(x => (DateTime.Now.Date-x.HiredDate.Date ).TotalDays < 90).ToList();
           
            foreach (var employee in employees)
            {
                
                List<DateTime> holidays = db.Holidays.Select(x => x.Date).ToList();
                probationAmount = Enumerable.Range(0, 60)
               .Select(offset => employee.HiredDate.AddDays(1))
               .Where(date => date.DayOfWeek != DayOfWeek.Sunday && !holidays.Contains(date))
               .Count();
                if(probationAmount <= 60)
                {

                    employeeIds.Add(employee.EmployeeId);
                }
                 
            }
            return employeeIds;
        }
        private List<int> GetContactEmployeeId(List<ContactVM> employees)
        {
            List<int> employeeIds = new List<int>(); 
            foreach (var employee in employees)
            {
                var contract = db.Contract.Where(x =>x.EmployeeId==employee.EmployeeId).Where(x=>x.To>=employee.AssignmentDate&&x.To>=DateTime.Now).OrderByDescending(x=>x.To).FirstOrDefault();
                if(contract != null&& ((contract.To.Date - DateTime.Now.Date).TotalDays<=30))
                {
                    employeeIds.Add(employee.EmployeeId);


                }
                 
            }
            return employeeIds;
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
        public string GenerateEmployeeCode(DateTime date)
        {
            var empCode = "";
            var name = "";
            var sequenceNo = 0;
            int digitLength = 0;

            var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == EmployeeIdAttribute.Auto_Generate.ToString());
            if (config != null)
            {
                var empCodeSettings = db.EmployeeCodeSettings.OrderBy(x => x.Index).ToList();
                List<string> emp = new List<string>();
                if (empCodeSettings.Any())
                {
                    foreach (var code in empCodeSettings)
                    {
                        if (code.Type == EmployeeIDAbbreviationType.Company)
                        {
                            var company = db.Companies.FirstOrDefault();
                            if (company != null)
                            {
                                emp.Add(company.Abbreviation);

                                name = company.Abbreviation;
                            }
                        }
                        if (code.Type == EmployeeIDAbbreviationType.Sequential_Number)
                        {
                            var company = db.Companies.FirstOrDefault();
                            if (company != null)
                            {
                                var setting = db.EmployeeCodeSequenceSettings.Where(x => x.Name == company.Abbreviation).ToList();
                                if (setting.Any())
                                {
                                    var last = (setting.OrderByDescending(x => x.IndexCount).FirstOrDefault().IndexCount + 1).ToString();
                                    var length = setting.OrderByDescending(x => x.IndexCount).FirstOrDefault().ItemCodeTrim;
                                    //for (int i = 1; i <= length; i++)
                                    //{
                                    //    last = "0" + last;
                                    //}
                                    string newId=last.PadLeft(length, '0');
                                    emp.Add(newId);

                                    sequenceNo = setting.OrderByDescending(x => x.IndexCount).FirstOrDefault().IndexCount + 1;
                                    digitLength = length;
                                }
                                else
                                {
                                    emp.Add("1");
                                }
                            }
                            else
                            {
                                emp.Add("1");
                            }
                        }
                        if (code.Type == EmployeeIDAbbreviationType.Year)
                        {
                            emp.Add(date.Year.ToString());

                        }

                    }

                    for (int i = 0; i < emp.Count(); i++)
                    {
                        if (i == (emp.Count() - 1))
                        {
                            empCode = empCode + emp[i];
                        }
                        else
                        {
                            empCode = empCode + emp[i] + "-";
                        }

                    }

                }
                var employeecode = db.EmployeeCodeSequenceSettings.Where(x => x.Name.Trim() == name.Trim()).FirstOrDefault();
                if (employeecode !=null)
                {

                    employeecode.ItemCodeTrim = digitLength;
                    employeecode.IndexCount = sequenceNo;
                    employeecode.Name = name;
                    employeecode.Status = true;
                   

                    db.EmployeeCodeSequenceSettings.Attach(employeecode);
                    db.Entry(employeecode).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    var idSequence = new EmployeeCodeSequenceSetting
                    {
                        ItemCodeTrim = digitLength,
                        IndexCount = sequenceNo,
                        Name = name,
                        Status = true,
                    };

                    db.EmployeeCodeSequenceSettings.Add(idSequence);
                    db.SaveChanges();
                }
            }

            return empCode;

        }
        [AuthorizeRoles(HrmConstants.HRProfileUser, HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        public ActionResult Person_Employee_InProbation_Read([DataSourceRequest]DataSourceRequest request)
        {

            //int branchId = UserInformationHelper.GetUserBranchByUserName(User.Identity.Name);

            //var employees = placements.Where(x => x.BranchId == branchId).Select(x => x.EmployeeId);
            //IQueryable<Person_Employee> person_employees = db.Person_Employee.Where(x => employees.Contains(x.EmployeeId) && x.IsActive);
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);

            var person_employees = db.Person_Employee.AsNoTracking().ToList().Where(x => mybraches.Contains(x.BranchId.ToString()) && x.IsActive && x.IsPlaced).Where(x=>(DateTime.Now.Date-x.HiredDate.Date).TotalDays<90).ToList();
            List<int> employeeIds = GetEmployeeIdInProhabition(person_employees);
            person_employees = person_employees.Where(x => employeeIds.Contains(x.EmployeeId)).ToList();
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
                BranchId = person_Employee.BranchId,//Branch(person_Employee.EmployeeId),
                IsProbationEnd = person_Employee.Status == EmployeeStatus.PROBATION ? Probation(person_Employee.EmployeeId) : false,
                ContratLeftDay = GetContractLeftDay(person_Employee.EmployeeId),
                PhoneNo = person_Employee.PhoneNo,
                City = person_Employee.City,
                SubCity = person_Employee.SubCity,
                Wereda = person_Employee.Wereda,
                EmployeeTypeId = person_Employee.EmployeeTypeId,
                IsPlaced = person_Employee.IsPlaced,
                Age = EmployeeHelper.GetAge(person_Employee.EmployeeId)
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser, HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        public ActionResult ContractEmolyee30DaysLeft_Read([DataSourceRequest]DataSourceRequest request)
        {

            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<ContactVM>contacts = db.Placements.Where(x =>x.EmployementType ==EmployeeStatus.CONTRACT &&x.isCurrent).Select(x=>new ContactVM {EmployeeId= x.EmployeeId, AssignmentDate = x.AssignmentDate}).ToList();
            List<int> employeeIds = GetContactEmployeeId(contacts);
            var person_employees = db.Person_Employee
                            .Where(x=> employeeIds.Contains(x.EmployeeId))
                            .Where(x => mybraches.Contains(x.BranchId.ToString()) 
                            && x.IsActive 
                            && x.IsPlaced).ToList();
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
                BranchId = person_Employee.BranchId,//Branch(person_Employee.EmployeeId),
                IsProbationEnd = person_Employee.Status == EmployeeStatus.PROBATION ? Probation(person_Employee.EmployeeId) : false,
                ContratLeftDay = GetContractLeftDay(person_Employee.EmployeeId),
                PhoneNo = person_Employee.PhoneNo,
                City = person_Employee.City,
                SubCity = person_Employee.SubCity,
                Wereda = person_Employee.Wereda,
                EmployeeTypeId = person_Employee.EmployeeTypeId,
                IsPlaced = person_Employee.IsPlaced,
                Age = EmployeeHelper.GetAge(person_Employee.EmployeeId)
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser, HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        public ActionResult RetiringEmolyeeNotificationBefore180DaysLeft_Read([DataSourceRequest]DataSourceRequest request)
        {

            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //60 years * 365.25days = 21915 days , before 6 month = 6 * 30 = 180=>21915 - 180 = 21735
            //var person_employees = db.Person_Employee
            //                .Where(x => (DateTime.Now - x.DateOfBirth).TotalDays >= 21735)
            //                .Where(x => mybraches.Contains(x.BranchId.ToString())
            //                && x.IsActive
            //                && x.IsPlaced).ToList();
            List<string> myBranches = UserInformationHelper.GetUserBranchs(User.Identity.Name);

            //// 60 years * 365.25 days = 21915 days (including leap years)
            //// 180 days before reaching 60 years = 21915 - 180 = 21735 days
            //double retirementThresholdInDays = 60 * 365.25 - 180; // 21735 days

            //var person_employees = db.Person_Employee
            //    .Where(x => (DateTime.Now - x.DateOfBirth).TotalDays >= retirementThresholdInDays)
            //    .Where(x => myBranches.Contains(x.BranchId.ToString()) && x.IsActive && x.IsPlaced)
            //    .ToList();

            // Calculate the threshold date for employees who have 180 days or less until they turn 60

            DateTime sixtyYearsAgo = DateTime.Now.AddYears(-60);
            DateTime retirementNotificationDate = sixtyYearsAgo.AddDays(180);

            var person_employees = db.Person_Employee
                .Where(x => x.DateOfBirth <= retirementNotificationDate)
                .Where(x => myBranches.Contains(x.BranchId.ToString()) && x.IsActive && x.IsPlaced)
                .ToList();
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
                BranchId = person_Employee.BranchId,//Branch(person_Employee.EmployeeId),
                IsProbationEnd = person_Employee.Status == EmployeeStatus.PROBATION ? Probation(person_Employee.EmployeeId) : false,
                ContratLeftDay = GetContractLeftDay(person_Employee.EmployeeId),
                PhoneNo = person_Employee.PhoneNo,
                City = person_Employee.City,
                SubCity = person_Employee.SubCity,
                Wereda = person_Employee.Wereda,
                EmployeeTypeId = person_Employee.EmployeeTypeId,
                IsPlaced = person_Employee.IsPlaced,
                Age = EmployeeHelper.GetAge(person_Employee.EmployeeId)
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRProfileUser, HrmConstants.HRProfileAdmin, HrmConstants.HRAdmin)]
        public ActionResult Person_Employee_Read([DataSourceRequest]DataSourceRequest request)
        {

            //int branchId = UserInformationHelper.GetUserBranchByUserName(User.Identity.Name);

            //var employees = placements.Where(x => x.BranchId == branchId).Select(x => x.EmployeeId);
            //IQueryable<Person_Employee> person_employees = db.Person_Employee.Where(x => employees.Contains(x.EmployeeId) && x.IsActive);

            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            //List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            var person_employees = db.Person_Employee.Where(x => mybraches.Contains(x.BranchId.ToString()) && x.IsActive && x.IsPlaced).ToList();


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
                BranchId = person_Employee.BranchId,//Branch(person_Employee.EmployeeId),
                IsProbationEnd = person_Employee.Status == EmployeeStatus.PROBATION ? Probation(person_Employee.EmployeeId) : false,
                ContratLeftDay = GetContractLeftDay(person_Employee.EmployeeId),
                PhoneNo = person_Employee.PhoneNo,
                City = person_Employee.City,
                SubCity = person_Employee.SubCity,
                Wereda = person_Employee.Wereda,
                EmployeeTypeId = person_Employee.EmployeeTypeId,
                IsPlaced = person_Employee.IsPlaced,
                Age = EmployeeHelper.GetAge(person_Employee.EmployeeId)
            });

            return Json(result);
        }
        [AuthorizeRoles(HrmConstants.HRProfileUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Person_Employee_Create([DataSourceRequest]DataSourceRequest request, Person_Employee person_Employee)
        {
            ModelState.Clear();

            char [] nameFilter = "0123456789".ToArray();

            if (person_Employee.FirstName.IndexOfAny(nameFilter) != -1 || 
                person_Employee.MiddleName.IndexOfAny(nameFilter) != -1 ||
                person_Employee.LastName.IndexOfAny(nameFilter) != -1  ||
                person_Employee.FirstNameEnglish.IndexOfAny(nameFilter) != -1||
                person_Employee.MiddleNameEnglish.IndexOfAny(nameFilter) != -1||
                person_Employee.LastNameEnglish.IndexOfAny(nameFilter) != -1)
            {
                ModelState.AddModelError("Name", "Name should not contain numbers.");
            }

            if (person_Employee.DateOfBirthAm != null)
            {
                var splitDateB = person_Employee.DateOfBirthAm.Split('/');
                person_Employee.DateOfBirth = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateB[0]), Convert.ToInt32(splitDateB[1]), Convert.ToInt32(splitDateB[2]));

            }
            else
            {
                ModelState.AddModelError("", "Birth Date is required");
            }

            if (person_Employee.HiredDateAmharic != null)
            {
                var splitDateH = person_Employee.HiredDateAmharic.Split('/');
                person_Employee.HiredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateH[0]), Convert.ToInt32(splitDateH[1]), Convert.ToInt32(splitDateH[2]));
            }
            else
            {
                ModelState.AddModelError("", "Hire Date is required");
            }

            //this variable uses for if the setting is not added it helps to take manualy filled id
           // string empCode = person_Employee.EmployeeCode;

            string generated_code= GenerateEmployeeCode(person_Employee.HiredDate);

            if (!string.IsNullOrEmpty(generated_code))
            {
                person_Employee.EmployeeCode = generated_code;
            }
            else
            {
                var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == EmployeeIdAttribute.Auto_Generate.ToString()&&x.Reference== "EmployeeID");
                if(config != null)
                {
                    int.TryParse(config.CurrentValue, out int length);
                    if(length != person_Employee.EmployeeCode.Length)
                    {
                        ModelState.AddModelError("exist", "The Employee Code  Must be Length Of "+length);
                    }
                }
                //person_Employee.EmployeeCode = "---";
            }

            #region Validation 

            int age = DateTime.Now.Year - person_Employee.DateOfBirth.Year;
            var checkExistance = db.Person_Employee.FirstOrDefault(x => x.EmployeeCode == person_Employee.EmployeeCode);
            if (checkExistance != null)
            {
                ModelState.AddModelError("", "This Employee Id is already assigned for other employee!!");
            }
            if (person_Employee.HiredDate < person_Employee.DateOfBirth)
            {
                ModelState.AddModelError("", "Date of birth should be less than hire date.");
            }
            if (age < 18)
            {
                ModelState.AddModelError("", "The Employee age should be 18 or more.");
            }
            if (person_Employee.HiredDate.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Hire date should be less or equal to the current date!!");
            }

            #endregion

            if (ModelState.IsValid)
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
                    Status = EmployeeStatus.NO_STATUS,
                    HiredDate = person_Employee.HiredDate,
                    FirstName = person_Employee.FirstName,
                    MiddleName = person_Employee.MiddleName, 
                    LastName = person_Employee.LastName,
                    Nationality = person_Employee.Nationality,
                    DateOfBirth = person_Employee.DateOfBirth,
                    Title = person_Employee.Title,
                    Gender = person_Employee.Gender,
                    CategoryID = person_Employee.CategoryID,
                    IsActive = true,
                    IsPlaced = false,
                    Remark = person_Employee.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = person_Employee.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = person_Employee.ModifiedBy,
                    //DateCreatedAM = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day,DateTime.Today.Month,DateTime.Today.Year),
                    //DateLastModifiedAM = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year),
                    FirstNameEnglish = person_Employee.FirstNameEnglish,
                    MiddleNameEnglish = person_Employee.MiddleNameEnglish,
                    LastNameEnglish = person_Employee.LastNameEnglish,
                    DateOfBirthAm = person_Employee.DateOfBirthAm,
                    HiredDateAmharic = person_Employee.HiredDateAmharic,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    PhoneNo = person_Employee.PhoneNo,
                    City = person_Employee.City,
                    SubCity = person_Employee.SubCity,
                    Wereda = person_Employee.Wereda,
                    EmployeeTypeId = person_Employee.EmployeeTypeId,
                };

                db.Person_Employee.Add(entity);
                db.SaveChanges();

                person_Employee.EmployeeId = entity.EmployeeId;

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Profile", "Success", User.Identity.Name, " Profile Info for " + person.EmployeeCode + " is Created By  " + User.Identity.Name, Modules.HRM);

                }
                #endregion

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
            char[] nameFilter = "0123456789".ToArray();

            if (person_Employee.FirstName.IndexOfAny(nameFilter) != -1|| 
                person_Employee.MiddleName.IndexOfAny(nameFilter) != -1 ||
                person_Employee.LastName.IndexOfAny(nameFilter) != -1 || 
                person_Employee.FirstNameEnglish.IndexOfAny(nameFilter) != -1 || 
                person_Employee.MiddleNameEnglish.IndexOfAny(nameFilter) != -1 || 
                person_Employee.LastNameEnglish.IndexOfAny(nameFilter) != -1)
            {
                ModelState.AddModelError("Name", "Name should not contain numbers.");
            }
            if (person_Employee.DateOfBirthAm != null)
            {
                var splitDateB = person_Employee.DateOfBirthAm.Split('/');
                person_Employee.DateOfBirth = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateB[0]), Convert.ToInt32(splitDateB[1]), Convert.ToInt32(splitDateB[2]));
            }
            else
            {
                ModelState.AddModelError("", "Birth Date is required");
            }

            if (person_Employee.HiredDateAmharic != null)
            {
                var splitDateH = person_Employee.HiredDateAmharic.Split('/');
                person_Employee.HiredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateH[0]), Convert.ToInt32(splitDateH[1]), Convert.ToInt32(splitDateH[2]));

            }
            else
            {
                ModelState.AddModelError("", "Hire Date is required");
            }

            // var days = DateTime.Today.Date.Subtract(person_Employee.HiredDate.Date).Days;
            // var month = days / 30;
            // var year = (int)month / 12;
            var config = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == ProbationSetting.Probation_Period.ToString());
            if (config != null)
            {
                int days = Convert.ToInt32(HrHelper.GetWorkingDays(person_Employee.HiredDate.Date, DateTime.Today.Date));
                if (days < Convert.ToInt32(config.CurrentValue) && person_Employee.Status != EmployeeStatus.PROBATION)
                {
                    ModelState.AddModelError("Probation Validation", "This employee didn't finish Probation period you can't change the status");
                }
            }
            
            int age = DateTime.Now.Year - person_Employee.DateOfBirth.Year;

            if (person_Employee.HiredDate < person_Employee.DateOfBirth)
            {
                ModelState.AddModelError("", "Date of birth should be less than hire date.");
            }
            if (age < 18)
            {
                ModelState.AddModelError("", "The Employee age should be 18 or more.");
            }
            if (person_Employee.HiredDate.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Hire date should be less or equal to the current date!!");
                return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState), JsonRequestBehavior.DenyGet);
            }

            var checkCode = db.Person_Employee.FirstOrDefault(x => x.EmployeeId != person_Employee.EmployeeId && x.EmployeeCode == person_Employee.EmployeeCode);
            if (checkCode != null)
            {
                ModelState.AddModelError("", "This employee code is already taken by another employee");
            }
            else if(checkCode != null
                     &&checkCode.EmployeeId == person_Employee.EmployeeId 
                     && checkCode.EmployeeCode != person_Employee.EmployeeCode)

            {
                var configForCode = db.Configurations.FirstOrDefault(x => x.Category == "HRM" && x.Attribute == EmployeeIdAttribute.Auto_Generate.ToString() && x.Reference == "EmployeeID");
                if (configForCode != null)
                {
                    int.TryParse(configForCode.CurrentValue, out int length);
                    if (length != person_Employee.EmployeeCode.Length)
                    {
                        ModelState.AddModelError("exist", "The Employee Code  Must be Length Of " + length);
                    }
                }
                
            }
            if (ModelState.IsValid)
            {
                var emp = db.Person_Employee.Find(person_Employee.EmployeeId);
                if (emp != null)
                {
                    emp.EmployeeCode = person_Employee.EmployeeCode;
                    emp.MothersFullName = person_Employee.MothersFullName;
                    emp.BirthPlace = person_Employee.BirthPlace;
                    emp.Nation = person_Employee.Nation;
                    emp.Region = person_Employee.Region;
                    emp.Religion = person_Employee.Religion;
                    emp.MartialStatus = person_Employee.MartialStatus;
                    emp.Status = person_Employee.Status;
                    emp.HiredDate = person_Employee.HiredDate;
                    emp.FirstName = person_Employee.FirstName;//FirstName,
                    emp.MiddleName = person_Employee.MiddleName; //MiddleName ,
                    emp.LastName = person_Employee.LastName;//LastName
                    emp.Nationality = person_Employee.Nationality;
                    emp.DateOfBirth = person_Employee.DateOfBirth;
                    emp.Title = person_Employee.Title;
                    emp.Gender = person_Employee.Gender;
                    emp.CategoryID = person_Employee.CategoryID;
                    emp.IsActive = person_Employee.IsActive;
                    emp.Remark = person_Employee.Remark;
                    emp.LastModified = DateTime.Now;
                    emp.ModifiedBy = User.Identity.Name;
                    emp.FirstNameEnglish = person_Employee.FirstNameEnglish;
                    emp.MiddleNameEnglish = person_Employee.MiddleNameEnglish;
                    emp.LastNameEnglish = person_Employee.LastNameEnglish;
                    emp.DateOfBirthAm = person_Employee.DateOfBirthAm;
                    emp.OrgBranchName = person_Employee.OrgBranchName;
                    emp.HiredDateAmharic = person_Employee.HiredDateAmharic;
                    emp.PhoneNo = person_Employee.PhoneNo;
                    emp.City = person_Employee.City;
                    emp.SubCity = person_Employee.SubCity;
                    emp.Wereda = person_Employee.Wereda;
                    emp.EmployeeTypeId = person_Employee.EmployeeTypeId;

                    db.Entry(emp).State = EntityState.Modified;
                    db.SaveChanges();

                    var place = db.Placements.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId && x.isCurrent);
                    if (place != null)
                    {
                        place.EmployementType = emp.Status;

                        db.Entry(place).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Employee Profile", "Success", User.Identity.Name, " Profile Info of " + person.EmployeeCode + " is Updated By  " + User.Identity.Name, Modules.HRM);
                }
                #endregion

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

                    var employee = db.Person_Employee.Find(person_Employee.EmployeeId);
                    if (employee != null)
                    {
                        db.Entry(employee).State = EntityState.Deleted;
                        db.SaveChanges();

                        #region delete log
                        UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee information of employee Id" + person_Employee.EmployeeCode);
                        #endregion
                    }
                }
            }

            return Json(new[] { person_Employee }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Placements_HistoryForProfile_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            List<OldPlacementVM> placeList = new List<OldPlacementVM>();

            var placements = db.Placements.Where(x => x.EmployeeId == id && !x.isCurrent).ToList();//.Where(x => mybraches.Contains(x.BranchId.ToString()));

            foreach (var place in placements)
            {
                var pp = new OldPlacementVM();

                pp.PlaceId = place.PlacementId;
                var branch = db.Branch.FirstOrDefault(x => x.BranchId == place.BranchId);
                if (branch != null)
                {
                    pp.Branch = branch.Name;
                }
                var department = db.MainCompanyStructures.FirstOrDefault(x => x.OrgStructureId == place.OrgStructureId);
                if (department != null)
                {
                    pp.Department = department.Name;
                }
                var jobtitle = db.Lookups.FirstOrDefault(x => x.LookupId == place.LookupId && x.LookupType == LookUpTypes.JOB_TITLE);
                if (jobtitle != null)
                {
                    pp.JobTitle = jobtitle.Description;
                }
                var step = db.Steps.FirstOrDefault(x => x.StepId == place.StepId );
                if (step != null)
                {
                    pp.Step = step.StepName;
                }
                var grade = db.Ranks.FirstOrDefault(x => x.RankId == place.RankId );
                if (grade != null)
                {
                    pp.Grade = grade.RankName;
                }
                pp.LookupId = place.LookupId;
                pp.AssignmentDate = place.AssignmentDateAmharic;
                pp.Date = place.AssignmentDate;
                pp.IsHead = place.IsHead;
                pp.IsCurrent = place.isCurrent;
                pp.Salary = place.Salary;
                pp.AssignmentType = place.AssignmentType;

                placeList.Add(pp);
            }
            DataSourceResult result = placeList.ToDataSourceResult(request, placement => new
            {
                PlaceId = placement.PlaceId,
                Branch = placement.Branch,
                Department = placement.Department,
                LookupId = placement.LookupId,
                JobTitle = placement.JobTitle,
                Grade = placement.Grade,
                Step = placement.Step,
                IsHead = placement.IsHead,
                IsCurrent = placement.IsCurrent,
                AssignmentDate = placement.AssignmentDate,
                Salary = placement.Salary,
                AssignmentType = placement.AssignmentType

            });

            return Json(result);


        }

        //public ActionResult Placements_HistoryForProfile_Read([DataSourceRequest]DataSourceRequest request, int id)
        //{
        //    List<OldPlacementVM> placeList = new List<OldPlacementVM>();

        //    var placements = db.Placements.Where(x => x.EmployeeId == id).OrderByDescending(x=>x.PlacementId).ToList();

        //    foreach (var place in placements)
        //    {
        //        var pp = new OldPlacementVM();
        //        pp.PlaceId = place.PlacementId;
        //        pp.BranchId = place.BranchId;
        //        pp.DepartmentId = place.OrgStructureId;
        //        pp.LookupId = place.LookupId;
        //        pp.AssignmentDate = place.AssignmentDateAmharic;
        //        pp.IsHead = place.IsHead;
        //        pp.IsCurrent = place.isCurrent;
        //        pp.Salary = place.Salary;
        //        pp.AssignmentType = place.AssignmentType;

        //        placeList.Add(pp);
        //    }
        //    DataSourceResult result = placeList.ToDataSourceResult(request, placement => new
        //    {
        //        PlaceId = placement.PlaceId,
        //        BranchId = placement.BranchId,
        //        DepartmentId = placement.DepartmentId,
        //        LookupId = placement.LookupId,
        //        JobTitle = placement.JobTitle,
        //        IsHead = placement.IsHead,
        //        IsCurrent = placement.IsCurrent,
        //        AssignmentDate = placement.AssignmentDate,
        //        Salary = placement.Salary,
        //        AssignmentType = placement.AssignmentType

        //    });

        //    return Json(result);


        //}
        public ActionResult Placements_HistoryForProfile_Update([DataSourceRequest]DataSourceRequest request, OldPlacementVM oldPlacement, int id)
        {
            if (oldPlacement.IsCurrent)
            {
                ModelState.AddModelError("exist", "This is the current Placement could you can edit it in Placement ");
            }
            var placement = db.Placements.Where(x => x.EmployeeId == id &&x.PlacementId==oldPlacement.PlaceId&& !x.isCurrent).FirstOrDefault();

            if ( placement !=null)
            {

               // placement.BranchId = oldPlacement.BranchId;
               // placement.OrgStructureId = oldPlacement.DepartmentId;
                placement.LookupId = oldPlacement.LookupId;
                if(oldPlacement.AssignmentDate != null)
                {

                    var splitDateH = oldPlacement.AssignmentDate.Split('/');
                    placement.AssignmentDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateH[0]), Convert.ToInt32(splitDateH[1]), Convert.ToInt32(splitDateH[2]));

                }
                placement.IsHead = oldPlacement.IsHead;
                placement.Salary = oldPlacement.Salary;
                placement.AssignmentType = oldPlacement.AssignmentType;

                db.Entry(placement).State = EntityState.Modified;
                db.SaveChanges();
            }
           

            return Json(new[] { oldPlacement }.ToDataSourceResult(request, ModelState));



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
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Attachment" + attachment.ReferenceType.ToString() + " " + attachment.Description);
                #endregion
                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
                #region delete file from path
                //string fullPath = Request.MapPath(attachment.URL);
                if (System.IO.File.Exists(attachment.URL))
                {
                    System.IO.File.Delete(attachment.URL);
                }
                #endregion
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FilterMenuCustomization_Cities()
        {
            var db = new ExceedDbContext();
            var emps = db.Person_Employee;
            return Json(emps.Select(p => new { Name = p.FirstName, Id = p.EmployeeId }), JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult EmployeeChart([DataSourceRequest]DataSourceRequest request, int id)
        {
            var leaveList = db.Leaves.Where(x => x.EmployeeId == id).Count();
            int internalExpriance = 0;
            int externalExpriance = 0;
            var displineList = db.DisciplinaryCases.Where(x => x.EmployeeId == id).Count();
            // var abcentList = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == id).Count();
            var externalExpriances = db.Experiences.Where(x => x.EmployeeId == id).ToList();
            if (externalExpriances.Any())
            {
                externalExpriance = Convert.ToInt32(externalExpriances.Sum(x => x.TotalDuration));
            }
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == id);
            if (employee != null)
            {
                internalExpriance = (DateTime.Now.Year - employee.HiredDate.Year);
            }

            var viewModel = new List<EmployeeChartModel>();
            var data = new EmployeeChartModel();
            if (leaveList > 0)
            {
                //data.EmployeeChartModels = new List<EmployeeChartModel>();

                data.Category = "Leave";
                data.Value = leaveList;
                data.Color = "SteelBlue";
                //data.EmployeeChartModels.Add(data);
                viewModel.Add(data);
            }
            if (displineList > 0)
            {
                data = new EmployeeChartModel();
                //data.EmployeeChartModels = new List<EmployeeChartModel>();
                data.Category = "Dcipline";
                data.Value = displineList;

                data.Color = "LimeGreen";
                //data.EmployeeChartModels.Add(data);
                viewModel.Add(data);
            }
            //if (abcentList > 0)
            //{
            //    data = new EmployeeChartModel();
            //    //data.EmployeeChartModels = new List<EmployeeChartModel>();
            //    data.Category = "Abcent";
            //    data.Value = abcentList;

            //    data.Color = "OrangeRed";
            //    viewModel.Add(data);
            //    //data.EmployeeChartModels.Add(data);
            //}
            data = new EmployeeChartModel();
            //data.EmployeeChartModels = new List<EmployeeChartModel>();
            data.Category = "Internal Expriance";
            data.Value = internalExpriance;
            data.Color = "MediumSlateBlue";
            //data.EmployeeChartModels.Add(data);
            viewModel.Add(data);

            data = new EmployeeChartModel();
            //data.EmployeeChartModels = new List<EmployeeChartModel>();
            data.Category = "External Expriance";
            data.Value = externalExpriance;
            data.Color = "Chocolate";
            //data.EmployeeChartModels.Add(data);
            viewModel.Add(data);
            return Json(viewModel);
        }
        public ActionResult Employee_Import([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
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
                    import.FirstName = ds.Tables[0].Rows[j].ItemArray[2].ToString();
                    import.MiddleName = ds.Tables[0].Rows[j].ItemArray[3].ToString();
                    import.LastName = ds.Tables[0].Rows[j].ItemArray[4].ToString();
                    import.Nationality = ds.Tables[0].Rows[j].ItemArray[5].ToString();
                    var splitDate = ds.Tables[0].Rows[j].ItemArray[18].ToString().Split('/');
                    import.DateOfBirth = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                    //import.DateOfBirth = DateOfBirth;
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
                    import.HiredDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                    //import.HiredDate = HireDate;
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
                        PhoneNo = person_Employee.PhoneNo,
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
        public ActionResult Region_Update()
        {
            var persons = db.Person_Employee.ToList();

            foreach (var person in persons)
            {
                switch (Convert.ToInt32(person.Region))
                {
                    case 0:
                        person.Region = Region.TIGRAY;
                        break;
                    case 1:
                        person.Region = Region.AFAR;
                        break;
                    case 2:
                        person.Region = Region.AMHARA;
                        break;
                    case 3:
                        person.Region = Region.OROMIA;
                        break;
                    case 4:
                        person.Region = Region.SOMALIA;
                        break;
                    case 5:
                        person.Region = Region.SOUTH_NATION_AND_NATIONALITIES;
                        break;
                    case 6:
                        person.Region = Region.BENSHANGUL_GUMUZ;
                        break;
                    case 7:
                        person.Region = Region.HARRARI;
                        break;
                    case 8:
                        person.Region = Region.GAMBELLA;
                        break;
                    case 9:
                        person.Region = Region.ADDIS_ABEBA;
                        break;
                    case 10:
                        person.Region = Region.DIRE_DAWA;
                        break;
                    case 11:
                        person.Region = Region.OTHER;
                        break;
                }

                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Religion_Update()
        {
            var persons = db.Person_Employee.ToList();

            foreach (var person in persons)
            {
                switch (Convert.ToInt32(person.Religion))
                {
                    case 0:
                        person.Religion = ReligionList.ORTHODOX;
                        break;
                    case 1:
                        person.Religion = ReligionList.MUSLIM;
                        break;
                    case 2:
                        person.Religion = ReligionList.CATHOLIC;
                        break;
                    case 3:
                        person.Religion = ReligionList.PROTESTANT;
                        break;
                    case 4:
                        person.Religion = ReligionList.WAAQEFFATAA;
                        break;
                    case 5:
                        person.Religion = ReligionList.OTHER;
                        break;

                }

                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Gender_Update()
        {
            var persons = db.Person_Employee.Where(x => x.Gender != Gender.MALE && x.Gender != Gender.FEMALE).ToList();

            foreach (var person in persons)
            {
                switch (Convert.ToInt32(person.Gender))
                {
                    case 0:
                        person.Gender = Gender.MALE;
                        break;
                    case 1:
                        person.Gender = Gender.FEMALE;
                        break;
                }

                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult MartialStatus_Update()
        {
            var persons = db.Person_Employee.ToList();

            foreach (var person in persons)
            {
                switch (Convert.ToInt32(person.MartialStatus))
                {
                    case 0:
                        person.MartialStatus = MartialStatus.SINGLE;
                        break;
                    case 1:
                        person.MartialStatus = MartialStatus.MARRIED;
                        break;
                    case 2:
                        person.MartialStatus = MartialStatus.SEPARATED;
                        break;
                    case 3:
                        person.MartialStatus = MartialStatus.DIVORCED;
                        break;
                    case 4:
                        person.MartialStatus = MartialStatus.OTHER;
                        break;

                }

                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult IsPlacedField_Update()
        {
            var persons = db.Person_Employee.ToList();

            foreach (var person in persons)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == person.EmployeeId);
                if (placement != null)
                {
                    person.IsPlaced = true;

                    db.Entry(person).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        public ActionResult ActivateUnplacedEmployees_Update()
        {
            //activate the employee if he/she has no placement , because they are not terminated or retired employee
            var persons = db.Person_Employee.ToList();

            foreach (var person in persons)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == person.EmployeeId);
                if (placement == null)
                {
                    person.IsActive = true;

                    db.Entry(person).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }
        public string ReHireRetiredEmployees(int employeeId)
        {
            var employee = db.Person_Employee.FirstOrDefault(x=>x.EmployeeId == employeeId);
            if(employee != null)
            {
                employee.IsActive = true;
                employee.IsReHired = true;

                db.Entry(employee).State = EntityState.Modified;
                db.SaveChanges();
            }

            return "Successfully ReHired";
        }
        public ActionResult EmployeeFolderAttachmentForEmployeeProfiles_Read([DataSourceRequest]DataSourceRequest request, int empId)
        {
            var personalArchiveNoIds = db.PersonalArchiveNo.Where(x => x.EmployeeId == empId).Select(x => x.PersonalArchiveNoId);
            var employeefolderattachments = db.EmployeeFolderAttachments.Where(x => personalArchiveNoIds.Contains(x.PersonalArchiveNoId)).ToList();

            DataSourceResult result = employeefolderattachments.ToDataSourceResult(request, employeeFolderAttachment => new
            {
                EmployeeFolderAttachmentId = employeeFolderAttachment.EmployeeFolderAttachmentId,
                PersonalArchiveNoId = employeeFolderAttachment.PersonalArchiveNoId,
                FileName = employeeFolderAttachment.FileName,
                FilePath = employeeFolderAttachment.FilePath,
                ShortPath = employeeFolderAttachment.ShortPath,
                FileExtension = employeeFolderAttachment.FileExtension,
                Description = employeeFolderAttachment.Description,
                Remark = employeeFolderAttachment.Remark
            });

            return Json(result);
        }
        public ActionResult EmployeeFolderAttachments_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeFolderAttachment employeeFolderAttachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeFolderAttachment
                {
                    EmployeeFolderAttachmentId = employeeFolderAttachment.EmployeeFolderAttachmentId,
                    PersonalArchiveNoId = employeeFolderAttachment.PersonalArchiveNoId,
                    FileName = employeeFolderAttachment.FileName,
                    FilePath = employeeFolderAttachment.FilePath,
                    Remark = employeeFolderAttachment.Remark
                };

                db.EmployeeFolderAttachments.Attach(entity);
                db.EmployeeFolderAttachments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeFolderAttachment }.ToDataSourceResult(request, ModelState));
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
    public class ContactVM
    {
        public  int EmployeeId { get; set; }
        public  DateTime AssignmentDate { get; set; }
    }
}
