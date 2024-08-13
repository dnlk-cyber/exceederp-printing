using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.APTS;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Health;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using XAPI;

namespace ExceedERP.Web.Areas.Health.Controllers
{

    [AuthorizeRoles(HrmConstants.HRHealthAdmin, HrmConstants.HRHealthUser)]
    public class PatientCardController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime Date { get; set; }
        public static string DateAmharic { get; set; }
        string ErrorMessage = ""; HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {           

            var list = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);

            ViewData["LabTests"] = db.LaboratoryTests.ToList();
            ViewData["LaboratoryCatagories"] = db.LaboratoryCatagories.ToList();

            ViewData["drugList"] = db.DrugLists.ToList();
            ViewData["drugUnit"] = db.DrugUnits.ToList();
            ViewData["employees"] = db.Person_Employee.ToList();

            return View();
        }
        private bool CheckCardValidation(int id)
        {
            ErrorMessage = "";


            #region card
            var card = db.PatientCardDetail.Where(x => x.PatientId == id).FirstOrDefault();
            if (card != null)
            {

                ErrorMessage = "You cannot delete this Patient Card.Please delete Patient card detail information first.";
                return false;


            }
            #endregion
            #region Prescription
            var Prescription = db.Prescription.Where(x => x.PatientId == id).FirstOrDefault();
            if (Prescription != null)
            {

                ErrorMessage = "You cannot delete this Patient Card.Please delete Prescription information first.";
                return false;


            }
            #endregion

            #region refferal
            var refferal = db.ReferralSheet.Where(x => x.PatientId == id).FirstOrDefault();
            if (refferal != null)
            {

                ErrorMessage = "You cannot delete this Patient Card.Please delete Referral Sheet information first.";
                return false;


            }
            #endregion
            #region Patients Leave
            var patientsLeave = db.PatientsLeave.Where(x => x.PatientId == id).FirstOrDefault();
            if (patientsLeave != null)
            {

                ErrorMessage = "You cannot delete this Patient Card.Please delete Patients Leave information first.";
                return false;


            }
            #endregion
            return true;
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateAmharic")
            {
                DateAmharic = value;
                var splitDate = DateAmharic.Split('/');
                Date = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }



        }
        
        public JsonResult SendToLaboratory(int id)
        {
            var patientCardDetail = db.PatientCardDetail.FirstOrDefault(x => x.PatientCardDetailId == id);
            if (patientCardDetail != null && !patientCardDetail.IsSentToLaboratory)
            {

                patientCardDetail.IsSentToLaboratory = true;
                patientCardDetail.IsTested = false;

                db.Entry(patientCardDetail).State = EntityState.Modified;
                db.SaveChanges();

                var formattedData = new
                {
                    Message = "Sent to laboratory successfully.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var formattedData = new
                {
                    Message = "It is sent before you can't send twice.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
          
        }
       
        public JsonResult CloseRequest(int id)
        {
            var patientCardDetail = db.PatientCardDetail.FirstOrDefault(x => x.PatientCardDetailId == id);
            if (patientCardDetail != null)
            {

                patientCardDetail.IsClosed = true;

                db.Entry(patientCardDetail).State = EntityState.Modified;
                db.SaveChanges();

                var formattedData = new
                {
                    Message = "This Proccess is closed successfully.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var formattedData1 = new
            {
                Message = "Fail To Close The Request.",
                Status = "FAIL"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendToDoctor(int id)
        {
            var patientCardLaboratoryTest = db.LaboratoryTestPatientCards.FirstOrDefault(x => x.LaboratoryTestPatientCardId == id);
            if (patientCardLaboratoryTest != null)
            {
                var patientCardDetail = db.PatientCardDetail.FirstOrDefault(x => x.PatientCardDetailId == patientCardLaboratoryTest.PatientCardDetailId);

                if (patientCardDetail != null)
                {
                    patientCardDetail.IsTested = true;
                    db.Entry(patientCardDetail).State = EntityState.Modified;

                    patientCardLaboratoryTest.IsSentBackToDoctor = true;
                    db.Entry(patientCardLaboratoryTest).State = EntityState.Modified;

                    db.SaveChanges();

                    var formattedData = new
                    {
                        Message = "The result sent to the doctor successfully.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "This request is sent before.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            }
            var formattedData1 = new
            {
                Message = "Failed to send the result.",
                Status = "FAIL"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }
        public bool IsSent(int id)
        {
            var patientCardDetail = db.PatientCardDetail.Where(x=>x.PatientId == id && x.IsSentToLaboratory);
            if(patientCardDetail.Any())
            {
                return true;
            }
            return false;
        }
        public bool IsTested(int id)
        {
            var patientCardDetail = db.PatientCardDetail.Where(x => x.PatientId == id && x.IsTested);
            if (patientCardDetail.Any())
            {
                return true;
            }
            return false;
        }
        public bool IsClosed(int id)
        {
            var patientCardDetail = db.PatientCardDetail.Where(x => x.PatientId == id && x.IsClosed);
            if (patientCardDetail.Any())
            {
                return true;
            }
            return false;
        }
        public ActionResult PatientCard_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<PatientCard> patientcard = db.PatientCard.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = patientcard.ToDataSourceResult(request, patientCard => new
            {
                PatientId = patientCard.PatientId,
                PatientCardId = patientCard.PatientCardId,
                EmployeeId = patientCard.EmployeeId,
                Date = patientCard.Date,
                DateAmharic = patientCard.DateAmharic,
                Age = patientCard.Age,
                Sex = patientCard.Sex,
                IsNew = patientCard.IsNew,
                IsSentToLaboratory = IsSent(patientCard.PatientId),
                IsTested = IsTested(patientCard.PatientId),
                IsClosed = IsTested(patientCard.PatientId),
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult PatientCard_Create([DataSourceRequest]DataSourceRequest request, PatientCard patientCard)
        {
            ModelState.Clear();
            var checkExistance = db.PatientCard.Where(x => x.PatientCardId == CardNo).FirstOrDefault();
            if (checkExistance != null)
            {
                ModelState.AddModelError("", "This Card No is already exist");
            }
            else if (DateAmharic == null)
            {
                ModelState.AddModelError("", "Date  value is mandatory!!");
            }
            else if (Date.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Date should be less or equal to the current date!!");
            }
            var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == patientCard.EmployeeId);
            if (person != null)
            {
                if (ModelState.IsValid)
                {
                    var entity = new PatientCard
                    {
                        PatientCardId = person.EmployeeCode,
                        EmployeeId = patientCard.EmployeeId,
                        //WorkSpace = patientCard.WorkSpace,
                        Date = Date,
                        Age = Age,
                        Sex = Sex,
                        DateAmharic = DateAmharic,
                        IsNew = true,
                        IsSent = false,
                        IsTested = false,
                        LastModifiedDate = DateTime.Now
                    };

                    db.PatientCard.Add(entity);
                    db.SaveChanges();
                    patientCard.PatientId = entity.PatientId;

                    #region operation log                

                    UserHelper.OperationLog(Request.UserHostAddress, "Patient Card", "Success", User.Identity.Name, User.Identity.Name + " add Patient Card at date " + DateTime.Today + " card no " + entity.PatientCardId, Modules.Health);

                    #endregion
                }
            }

            return Json(new[] { patientCard }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult PatientCard_Update([DataSourceRequest]DataSourceRequest request, PatientCard patientCard)
        {
            ModelState.Clear();
            if (DateAmharic == null)
            {
                DateAmharic = patientCard.DateAmharic;
                var splitDate = DateAmharic.Split('/');
                Date = EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (Date.Date > DateTime.Now.Date)
            {

                ModelState.AddModelError("", "Date should be less or equal to the current date!!");
            }
            if (ModelState.IsValid)
            {

                var entity = new PatientCard
                {
                    PatientId = patientCard.PatientId,
                    PatientCardId = patientCard.PatientCardId,
                    EmployeeId = patientCard.EmployeeId,
                    //WorkSpace = patientCard.WorkSpace,
                    Date = Date,
                    Age = patientCard.Age,
                    Sex = patientCard.Sex,
                    DateAmharic = DateAmharic,
                    LastModifiedDate = DateTime.Now
                };

                db.PatientCard.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Patient Card", "Success", User.Identity.Name, User.Identity.Name + " update Patient Card at date " + DateTime.Today + " card no " + entity.PatientCardId, Modules.Health);

                #endregion
            }

            return Json(new[] { patientCard }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult PatientCard_Destroy([DataSourceRequest]DataSourceRequest request, PatientCard patientCard)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!CheckCardValidation(patientCard.PatientId))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
                {
                    var entity = new PatientCard
                    {
                        PatientId = patientCard.PatientId,
                        PatientCardId = patientCard.PatientCardId,
                        EmployeeId = patientCard.EmployeeId,
                        //WorkSpace = patientCard.WorkSpace,
                        Date = patientCard.Date,
                        Age = patientCard.Age,
                        Sex = patientCard.Sex,
                        DateAmharic = patientCard.DateAmharic,
                    };
                    #region delete log
                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                    if (person != null)
                    {
                        UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted patient card information of employee" + person.EmployeeCode);
                    }
                    #endregion
                    db.PatientCard.Attach(entity);
                    db.PatientCard.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { patientCard }.ToDataSourceResult(request, ModelState));
        }

        public static int Age { get; set; }
        public static Gender Gender { get; set; }
        public static string Sex { get; set; }
        public static string CardNo { get; set; }
        public string GetAgeSex(int emp)
        {
            var ageSex = db.Person_Employee.Where(s => s.EmployeeId == emp).FirstOrDefault();

            if (ageSex != null)
            {
                Age = (DateTime.Now.Year - ageSex.DateOfBirth.Year);
                Gender = ageSex.Gender;
            }
            if (Gender == 0)
            {
                Sex = "Male";

            }
            else
            {
                Sex = "Female";

            }
            CardNo = ageSex.EmployeeCode;
            return Age.ToString() + "-" + Sex + "-" + CardNo;
        }
        public JsonResult CheckPanel()
        {

            var persons = db.Person_Employee.Where(x=>x.Status == EmployeeStatus.PROBATION).ToList();
            ViewBag.datas = persons;
            return Json(persons.Select(p => new
            {
                Name = p.EmployeeCode + " :       " + p.FullNameaEnglish,
                FullNameaEnglish = p.FullNameaEnglish,
                FullNameAmharic = p.FullNameAmharic
            }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
