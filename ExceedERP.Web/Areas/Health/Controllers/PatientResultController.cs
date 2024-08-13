using ExceedERP.Core.Domain.APTS;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Health;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Health.Controllers
{
    public class PatientResultController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/PatientResult
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["employees"] = db.Person_Employee.ToList();
            ViewData["drugList"] = db.DrugLists.ToList();
            ViewData["drugUnit"] = db.DrugUnits.ToList();
            var hospitals = db.Companies.Where(x => x.Category == "2" && x.IsActive);
            ViewData["Hospitals"] = hospitals;
        }
        public ActionResult PatientResult_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PatientResult> patientResults = db.PatientResults;
            DataSourceResult result = patientResults.ToDataSourceResult(request, patientResult => new
            {
                PatientResultId = patientResult.PatientResultId,
                PatientId = patientResult.PatientId,
                DoctorId = patientResult.DoctorId,
                Description = patientResult.Description,
                RegisterDate = patientResult.RegisterDate,
                RegisterDateAm = patientResult.RegisterDateAm,
                IsForDespensary = patientResult.IsForDespensary,
                IsForLaboratory = patientResult.IsForLaboratory,
                IsRefer = patientResult.IsRefer,
                OnChecking = patientResult.OnChecking,
                MainCompanyId = patientResult.MainCompanyId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientResult_Create([DataSourceRequest]DataSourceRequest request, PatientResult patientResult)
        {

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (patientResult.MainCompanyId == null)
                {
                    patientResult.MainCompanyId = 0;
                }
                var registerDate = patientResult.RegisterDateAm.Split('/');
                var StartDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(registerDate[0]), Convert.ToInt32(registerDate[1]), Convert.ToInt32(registerDate[2]));
                var entity = new PatientResult
                {
                    PatientId = patientResult.PatientId,
                    DoctorId = UserInformationHelper.GetEmployeeId(User.Identity.Name),
                    Description = patientResult.Description,
                    RegisterDate = StartDate,
                    RegisterDateAm = patientResult.RegisterDateAm,
                    IsForDespensary = patientResult.IsForDespensary,
                    IsForLaboratory = patientResult.IsForLaboratory,
                    OnChecking = false,
                    MainCompanyId = patientResult.MainCompanyId
                };

                db.PatientResults.Add(entity);
                db.SaveChanges();
                patientResult.PatientResultId = entity.PatientResultId;

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Patient Result", "Success", User.Identity.Name, User.Identity.Name + " add Patient result at date " + DateTime.Today, Modules.Health);

                #endregion
            }

            return Json(new[] { patientResult }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientResult_Update([DataSourceRequest]DataSourceRequest request, PatientResult patientResult)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var registerDate = patientResult.RegisterDateAm.Split('/');
                var StartDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(registerDate[0]), Convert.ToInt32(registerDate[1]), Convert.ToInt32(registerDate[2]));

                var entity = new PatientResult
                {
                    PatientResultId = patientResult.PatientResultId,
                    PatientId = patientResult.PatientId,
                    DoctorId = patientResult.DoctorId,
                    Description = patientResult.Description,
                    RegisterDate = StartDate,
                    RegisterDateAm = patientResult.RegisterDateAm,
                    IsForDespensary = patientResult.IsForDespensary,
                    IsForLaboratory = patientResult.IsForLaboratory,
                    OnChecking = patientResult.OnChecking,
                    MainCompanyId = patientResult.MainCompanyId
                };

                db.PatientResults.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Patient Result", "Success", User.Identity.Name, User.Identity.Name + " update Patient result at date " + DateTime.Today, Modules.Health);

                #endregion
            }

            return Json(new[] { patientResult }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientResult_Destroy([DataSourceRequest]DataSourceRequest request, PatientResult patientResult)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new PatientResult
                {
                    PatientResultId = patientResult.PatientResultId,
                    PatientId = patientResult.PatientId,
                    DoctorId = patientResult.DoctorId,
                    Description = patientResult.Description,
                    RegisterDate = patientResult.RegisterDate,
                    RegisterDateAm = patientResult.RegisterDateAm,
                    IsForDespensary = patientResult.IsForDespensary,
                    IsForLaboratory = patientResult.IsForLaboratory,
                    OnChecking = patientResult.OnChecking,
                    MainCompanyId = patientResult.MainCompanyId
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Patient Result " + patientResult.PatientId);
                #endregion
                db.PatientResults.Attach(entity);
                db.PatientResults.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { patientResult }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult SendToDespensary(int id)
        {
            if (ModelState.IsValid)
            {
                var patientRegister = db.PatientResults.FirstOrDefault(x => x.PatientResultId == id);

                if (patientRegister != null)
                {

                    var entity = new PatientRegister
                    {
                        EmployeeId = patientRegister.PatientId,
                        DateCreated = DateTime.Now,
                        RegisterDate = patientRegister.RegisterDate,

                    };
                    db.PatientRegisters.Add(entity);
                    db.SaveChanges();

                    var patientDrugOrders = db.PatientDrugOrders.Where(x => x.PatientDrugOrderId == patientRegister.PatientResultId).ToList();
                    foreach (var patientDrug in patientDrugOrders)
                    {

                        var entityDetail = new PatientRegisterDetail
                        {
                            PatientRegisterId = entity.PatientRegisterId,
                            DrugListsId = patientDrug.DrugListsId,
                            DrugUnitId = patientDrug.DrugUnitId,
                            TotalQty = patientDrug.Quantity
                        };
                        db.PatientRegisterDetails.Add(entityDetail);
                        db.SaveChanges();

                        patientRegister.IsForDespensary = true;

                        db.Entry(patientRegister).State = EntityState.Modified;
                        db.SaveChanges();


                    }

                }


            }

            //string messages = "Successfully Send";

            //return new JavaScriptResult() { Script = "alert('" + messages + "');" };

            return RedirectToAction("Index");

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Refer(int id)
        {

            var status = db.PatientResults.FirstOrDefault(x => x.PatientResultId == id);

            if (status != null)
            {
                if (status.MainCompanyId == 0)
                {
                    //ModelState.AddModelError("", "you should select which hospital you have refers to");
                    return new JavaScriptResult() { Script = "alert(' you should select which hospital you have refers to ');" };
                }
                else if(status.IsRefer == true)
                {
                    //ModelState.AddModelError("", "you refered this employee before");
                    return new JavaScriptResult() { Script = "alert(' you refered this employee before ');" };
                }
                else
                {
                    status.IsRefer = true;


                    db.PatientResults.Attach(status);
                    db.Entry(status).State = EntityState.Modified;
                    db.SaveChanges();

                    var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == status.PatientId);
                    if (person != null)
                    {
                        var patientCard = new PatientCard
                        {
                            //PatientId = status.PatientId,
                            EmployeeId = status.PatientId,
                            PatientCardId = person.EmployeeCode,
                            Age = (DateTime.Now.Year - person.DateOfBirth.Year),
                            Sex = person.Gender.ToString(),
                            Date = status.RegisterDate,
                            DateAmharic = status.RegisterDateAm,

                        };
                        db.PatientCard.Add(patientCard);
                        db.SaveChanges();
                        var diagnosises = db.Diagnosis.Where(x => x.PatientResultId == id).ToList();
                        foreach (var diagnosis in diagnosises)
                        {
                            var referalSheet = new ReferralSheet
                            {
                                Diagnosis = diagnosis.Description,
                                PatientId = patientCard.PatientId,
                                Date = DateTime.Now,
                                MainCompanyId = Convert.ToInt32(status.MainCompanyId)
                            };
                            db.ReferralSheet.Add(referalSheet);
                            db.SaveChanges();
                        }

                    }
                    return new JavaScriptResult() { Script = "alert(' This employee is sent to referal sheet ');" };
                }

            }
            return RedirectToAction("Index");
        }
        public ActionResult Checked(int id)
        {
            var status = db.PatientResults.FirstOrDefault(x => x.PatientResultId == id);
            if (status != null)
            {
                status.OnChecking = false;


                db.PatientResults.Attach(status);
                db.Entry(status).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        public ActionResult Laboratory(int id)
        {
            var status = db.PatientResults.FirstOrDefault(x => x.PatientResultId == id);
            if (status != null)
            {
                status.OnChecking = true;
                status.IsForLaboratory = true;

                db.PatientResults.Attach(status);
                db.Entry(status).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}