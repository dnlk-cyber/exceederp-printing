using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.APTS;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class PatientRegisterController : Controller
    {
        // GET: APTS/PatientRegister
        ExceedDbContext db = new ExceedDbContext();
        public static DateTime engDate;
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        public void PopulateViewData()
        {
            var drug = db.DrugLists.ToList();
            ViewData["drugList"] = drug;
            ViewData["Units"] = db.DrugUnits.ToList();
            ViewData["employees"] = db.Person_Employee.ToList();
        }
        public bool GetIsUpdated(int id)
        {
            var patientRegisterDetail = db.PatientRegisterDetails.Where(x => x.PatientRegisterId == id && !x.IsUpdated).ToList();
            if (patientRegisterDetail.Any())
            {
                return false;
            }
            return true;
        }
        public ActionResult PatientRegister_Read([DataSourceRequest]DataSourceRequest request)
        {
            var medisince = db.PatientRegisters.ToList();
            DataSourceResult result = medisince.ToDataSourceResult(request, medice => new
            {
                PatientRegisterId = medice.PatientRegisterId,
                RegisterDate = medice.RegisterDate,
                RegisterDateAmh = medice.RegisterDateAmh,
                PatientName = medice.PatientName,
                CardNumber = medice.CardNumber,
                Sex = medice.Sex,
                Age = medice.Age,
                NameOfOrg = medice.NameOfOrg,
                PatientSignature = medice.PatientSignature,
                Unit = medice.Unit,
                IsCredit = medice.IsCredit,
                EmployeeId = medice.EmployeeId,
                DrugistId = medice.DrugistId,
                IsDone = medice.IsDone,
                IsAllDeducted = GetIsUpdated(medice.PatientRegisterId)
            });

            return Json(result);
        }
        public ActionResult PatientRegister_Create([DataSourceRequest]DataSourceRequest request, PatientRegister patientRegister)
        {
            ModelState.Clear();

            if (patientRegister.RegisterDateAmh != null)
            {
                var splitDate = patientRegister.RegisterDateAmh.Split('/');
                engDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

            }

            var age = 0;
            var gender = Gender.MALE;
            var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == patientRegister.EmployeeId);
            if (employee != null)
            {
                var bDay = DateTime.Today.Date.Subtract(employee.DateOfBirth.Date).Days;
                var bMonth = bDay / 30;
                var bYear = bMonth / 12;

                age = bYear;
                gender = employee.Gender;
            }

            if (ModelState.IsValid)
            {
                //var internalCode = db.DrugLists.FirstOrDefault(x=>x.DrugListsId == patientRegister.DrugListsId);

                var entity = new PatientRegister
                {
                    PatientName = patientRegister.PatientName,
                    CardNumber = patientRegister.CardNumber,
                    RegisterDate = engDate,
                    RegisterDateAmh = patientRegister.RegisterDateAmh,
                    Age = age,
                    Sex = gender,
                    NameOfOrg = patientRegister.NameOfOrg,
                    PatientSignature = patientRegister.PatientSignature,
                    DateCreated = patientRegister.DateCreated,
                    Unit = patientRegister.Unit,
                    CreatedBy = patientRegister.CreatedBy,
                    IsCredit = patientRegister.IsCredit,
                    EmployeeId = patientRegister.EmployeeId,
                    DrugistId = UserInformationHelper.GetEmployeeId(User.Identity.Name),
                    IsDone = patientRegister.IsDone
                };
                db.PatientRegisters.Add(entity);
                db.SaveChanges();
                patientRegister.PatientRegisterId = entity.PatientRegisterId;

                //var despensary = db.DespensaryStores.FirstOrDefault(x=>x.DrugListsId == entity.DrugListsId && x.DrugUnitId == entity.DrugUnitId);
                //if(despensary != null)
                //{
                //    despensary.Balance = despensary.Balance - entity.TotalQty;

                //    db.Entry(despensary).State = EntityState.Modified;
                //    db.SaveChanges();
                //}
            }

            return Json(new[] { patientRegister }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult PatientRegister_Update([DataSourceRequest]DataSourceRequest request, PatientRegister patientRegister)
        {
            if (ModelState.IsValid)
            {
                //var internalCode = db.DrugLists.FirstOrDefault(x => x.DrugListsId == patientRegister.DrugListsId);

                var entity = new PatientRegister
                {
                    PatientRegisterId = patientRegister.PatientRegisterId,
                    PatientName = patientRegister.PatientName,
                    CardNumber = patientRegister.CardNumber,
                    RegisterDate = patientRegister.RegisterDate,
                    RegisterDateAmh = patientRegister.RegisterDateAmh,
                    Age = patientRegister.Age,
                    Sex = patientRegister.Sex,
                    NameOfOrg = patientRegister.NameOfOrg,
                    PatientSignature = patientRegister.PatientSignature,
                    DateCreated = patientRegister.DateCreated,
                    CreatedBy = patientRegister.CreatedBy,
                    Unit = patientRegister.Unit,
                    EmployeeId = patientRegister.EmployeeId,
                    IsCredit = patientRegister.IsCredit,
                    DrugistId = patientRegister.DrugistId,
                    IsDone = patientRegister.IsDone

                };

                db.PatientRegisters.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { patientRegister }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult PatientRegister_Destroy([DataSourceRequest]DataSourceRequest request, PatientRegister patientRegister)
        {
            if (ModelState.IsValid)
            {
                //var internalCode = db.DrugLists.FirstOrDefault(x => x.DrugListsId == patientRegister.DrugListsId);

                var entity = new PatientRegister
                {

                    PatientRegisterId = patientRegister.PatientRegisterId,
                    PatientName = patientRegister.PatientName,
                    CardNumber = patientRegister.CardNumber,
                    RegisterDate = patientRegister.RegisterDate,
                    RegisterDateAmh = patientRegister.RegisterDateAmh,
                    Age = patientRegister.Age,
                    Sex = patientRegister.Sex,
                    NameOfOrg = patientRegister.NameOfOrg,
                    PatientSignature = patientRegister.PatientSignature,
                    DateCreated = patientRegister.DateCreated,
                    CreatedBy = patientRegister.CreatedBy,
                    Unit = patientRegister.Unit,
                    IsCredit = patientRegister.IsCredit,
                    EmployeeId = patientRegister.EmployeeId,
                    DrugistId = patientRegister.DrugistId,
                    IsDone = patientRegister.IsDone

                };

                db.PatientRegisters.Attach(entity);
                db.PatientRegisters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { patientRegister }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Done(int id)
        {
            bool left = false;
            bool success = false;

            var patientRegister = db.PatientRegisters.FirstOrDefault(x => x.PatientRegisterId == id);

            var allPatientDrugs = db.PatientRegisterDetails.Where(x => x.PatientRegisterId == id && !x.IsUpdated).ToList();
            if (patientRegister == null)
            {
                var formattedData = new
                {
                    Message = "Please Select the row first.",
                    Status = "FAIL"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (patientRegister.IsDone && !allPatientDrugs.Any())
            {
                var formattedData = new
                {
                    Message = "this recored is submited can't submit twice.",
                    Status = "FAIL"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (patientRegister != null)
            {
                var patientRegisterDetails = db.PatientRegisterDetails.Where(x => x.PatientRegisterId == patientRegister.PatientRegisterId).ToList();
                var countList = patientRegisterDetails.Count();
                if (countList <= 0)
                {
                    //message = "first recored drugs for this patient";
                    //return new JavaScriptResult() { Script = "alert('" + message + "');" };
                    var formattedData = new
                    {
                        Message = "first recored drugs for this patient.",
                        Status = "FAIL"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                foreach (var patientRegisterDetail in patientRegisterDetails.Where(x => !x.IsUpdated))
                {
                    var despensary = db.DespensaryStores.FirstOrDefault(x => x.DrugListsId == patientRegisterDetail.DrugListsId && x.DrugUnitId == patientRegisterDetail.DrugUnitId);

                    if ((despensary == null) || (despensary != null && despensary.Balance < patientRegisterDetail.TotalQty))
                    {
                        var drugList = db.DrugLists.FirstOrDefault(x => x.DrugListsId == patientRegisterDetail.DrugListsId);

                        left = true;

                        patientRegisterDetail.IsUpdated = false;

                        db.Entry(patientRegisterDetail).State = EntityState.Modified;
                        db.SaveChanges();
                        continue;
                    }
                    if (despensary != null && despensary.Balance >= patientRegisterDetail.TotalQty)
                    {
                        despensary.Balance = despensary.Balance - patientRegisterDetail.TotalQty;

                        db.Entry(despensary).State = EntityState.Modified;
                        db.SaveChanges();

                        patientRegister.IsDone = true;
                        db.Entry(patientRegister).State = EntityState.Modified;
                        db.SaveChanges();

                        patientRegisterDetail.IsUpdated = true;
                        db.Entry(patientRegisterDetail).State = EntityState.Modified;
                        db.SaveChanges();

                        success = true;

                    }


                }
                if (left && success)
                {
                    var formattedData = new
                    {
                        Message = "Some drugs are not exist check the detail",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else if (left && !success)
                {
                    var formattedData = new
                    {
                        Message = "Fail to register check drugs balance",
                        Status = "FAIL"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else if (!left && success)
                {
                    var formattedData = new
                    {
                        Message = "Successfully Registerd",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else if (!left && !success)
                {
                    var formattedData = new
                    {
                        Message = "Fail to register",
                        Status = "FAIL"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            }

            var formattedData1 = new
            {
                Message = "This employee not exist in patient register",
                Status = "FAIL"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Update(int id)
        {
            var message = "";

            var patientRegisterDetail = db.PatientRegisterDetails.FirstOrDefault(x => x.PatientRegisterDetailId == id);
            if (patientRegisterDetail != null)
            {
                var despensary = db.DespensaryStores.FirstOrDefault(x => x.DrugListsId == patientRegisterDetail.DrugListsId && x.DrugUnitId == patientRegisterDetail.DrugUnitId);
                if (despensary == null && despensary.Balance < patientRegisterDetail.TotalQty)
                {
                    message = "have not enough balance";
                    return new JavaScriptResult() { Script = "alert('" + message + "');" };
                }
                else
                {
                    var patientRegister = db.PatientRegisters.FirstOrDefault(x => x.PatientRegisterId == patientRegisterDetail.PatientRegisterId);
                    if (patientRegister != null)
                    {
                        patientRegister.IsDone = true;
                        db.Entry(patientRegister).State = EntityState.Modified;
                        db.SaveChanges();

                        patientRegisterDetail.IsUpdated = true;
                        db.Entry(patientRegisterDetail).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }



            }

            return new JavaScriptResult() { Script = "alert('" + message + "');" };
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}