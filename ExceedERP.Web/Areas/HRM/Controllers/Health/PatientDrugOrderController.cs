using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Health
{
    public class PatientDrugOrderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/PatientDrugOrder
        public ActionResult Index()
        {
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["employees"] = db.Person_Employee.ToList();
            ViewData["drugList"] = db.DrugLists.ToList();
            ViewData["drugUnit"] = db.DrugUnits.ToList();
        }
        public ActionResult PatientDrugOrder_Read([DataSourceRequest]DataSourceRequest request, int PatientCardId, int cardDetailId)
        {
            var patientResults = db.PatientDrugOrders.Where(x => x.PatientId == PatientCardId && x.PatientCardDetailId == cardDetailId);
            DataSourceResult result = patientResults.ToDataSourceResult(request, patientDrugOrder => new
            {
                PatientCardDetailId = patientDrugOrder.PatientCardDetailId,
                PatientId = patientDrugOrder.PatientId,
                DrugListsId = patientDrugOrder.DrugListsId,
                DrugUnitId = patientDrugOrder.DrugUnitId,
                PatientDrugOrderId = patientDrugOrder.PatientDrugOrderId,
                Quantity = patientDrugOrder.Quantity,
                Prescription = patientDrugOrder.Prescription,
                IsSentToPatientRegiser = patientDrugOrder.IsSentToPatientRegiser
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientDrugOrder_Create([DataSourceRequest]DataSourceRequest request, PatientDrugOrder patientDrugOrder, int cardDetailId, int PatientCardId)
        {
            var checkOrder = db.PatientDrugOrders.Where(x => x.PatientId == PatientCardId && x.PatientCardDetailId == cardDetailId && x.IsSentToPatientRegiser).ToList();
            if (checkOrder.Any())
            {
                ModelState.AddModelError("", "You can't add new drug the previous are sent");
            }
            if (ModelState.IsValid)
            {
                var entity = new PatientDrugOrder
                {
                    PatientCardDetailId = cardDetailId,
                    PatientId = PatientCardId,
                    DrugListsId = patientDrugOrder.DrugListsId,
                    DrugUnitId = patientDrugOrder.DrugUnitId,
                    Quantity = patientDrugOrder.Quantity,
                    Prescription = patientDrugOrder.Prescription,
                    IsSentToPatientRegiser = false
                };

                db.PatientDrugOrders.Add(entity);
                db.SaveChanges();
                patientDrugOrder.PatientDrugOrderId = entity.PatientDrugOrderId;

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Patient Drug Order", "Success", User.Identity.Name, User.Identity.Name + " add Patient drug order Id " + entity.PatientDrugOrderId, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { patientDrugOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientDrugOrder_Update([DataSourceRequest]DataSourceRequest request, PatientDrugOrder patientDrugOrder)
        {
            ModelState.Clear();
            if (patientDrugOrder.IsSentToPatientRegiser)
            {
                ModelState.AddModelError("", "You can't update this drug it's already sent");
            }
            if (ModelState.IsValid)
            {
                var entity = new PatientDrugOrder
                {
                    PatientDrugOrderId = patientDrugOrder.PatientDrugOrderId,
                    PatientId = patientDrugOrder.PatientId,
                    PatientCardDetailId = patientDrugOrder.PatientCardDetailId,
                    DrugListsId = patientDrugOrder.DrugListsId,
                    DrugUnitId = patientDrugOrder.DrugUnitId,
                    Quantity = patientDrugOrder.Quantity,
                    Prescription = patientDrugOrder.Prescription,
                    IsSentToPatientRegiser = patientDrugOrder.IsSentToPatientRegiser
                };


                db.PatientDrugOrders.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                

                UserHelper.OperationLog(Request.UserHostAddress, "Patient Drug Order", "Success", User.Identity.Name, User.Identity.Name + " update Patient drug order Id " + entity.PatientDrugOrderId, Modules.HealthAndInsurance);

                #endregion
            }

            return Json(new[] { patientDrugOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PatientDrugOrder_Destroy([DataSourceRequest]DataSourceRequest request, PatientDrugOrder patientDrugOrder)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new PatientDrugOrder
                {
                    PatientDrugOrderId = patientDrugOrder.PatientDrugOrderId,
                    PatientId = patientDrugOrder.PatientId,
                    PatientCardDetailId = patientDrugOrder.PatientCardDetailId,
                    DrugListsId = patientDrugOrder.DrugListsId,
                    DrugUnitId = patientDrugOrder.DrugUnitId,
                    Quantity = patientDrugOrder.Quantity,
                    Prescription = patientDrugOrder.Prescription,
                    IsSentToPatientRegiser = patientDrugOrder.IsSentToPatientRegiser
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Patient Drug Order Id" + patientDrugOrder.PatientDrugOrderId);
                #endregion
                db.PatientDrugOrders.Attach(entity);
                db.PatientDrugOrders.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { patientDrugOrder }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult SendToDespensary(int id)
        {
            var orderId = db.PatientDrugOrders.FirstOrDefault(x => x.PatientDrugOrderId == id);
            if (orderId != null && !orderId.IsSentToPatientRegiser)
            {
                orderId.IsSentToPatientRegiser = true;

                db.Entry(orderId).State = EntityState.Modified;
                //  db.SaveChanges();


                var patientCard = db.PatientCard.FirstOrDefault(x => x.PatientId == orderId.PatientId);
                if (patientCard != null)
                {
                    int age = 0;
                    var gender = Gender.MALE;
                    var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == patientCard.EmployeeId);
                    if (personEmployee != null)
                    {
                        age = DateTime.Today.Year - personEmployee.DateOfBirth.Year;
                        gender = personEmployee.Gender;
                    }
                    var dateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year);

                    var patientRegister = new PatientRegister
                    {
                        PatientName = "",
                        CardNumber = patientCard.PatientCardId,
                        RegisterDate = DateTime.Today,
                        RegisterDateAmh = dateAm,
                        Age = age,
                        Sex = gender,
                        NameOfOrg = "",
                        PatientSignature = "",
                        DateCreated = DateTime.Now,
                        Unit = "",
                        CreatedBy = User.Identity.Name,
                        IsCredit = false,
                        EmployeeId = patientCard.EmployeeId,
                        DrugistId = 0,//UserInformationHelper.GetEmployeeId(User.Identity.Name),
                        IsDone = false,
                    };

                    db.PatientRegisters.Add(patientRegister);

                    var patientRegisterDetail = new PatientRegisterDetail
                    {
                        PatientRegisterId = patientRegister.PatientRegisterId,
                        DrugListsId = orderId.DrugListsId,
                        TotalQty = orderId.Quantity,
                        RetailPrice = 0,//patientRegisterDetail.RetailPrice,
                        TotalRetailPrice = 0,//patientRegisterDetail.TotalQty * patientRegisterDetail.RetailPrice,
                        DrugUnitId = orderId.DrugUnitId,//patientRegisterDetail.DrugUnitId,
                        InternalCode = "",//patientRegisterDetail.InternalCode,
                        IsUpdated = false,//patientRegisterDetail.IsUpdated,
                    };
                    db.PatientRegisterDetails.Add(patientRegisterDetail);
                    db.SaveChanges();

                    var formattedData = new
                    {
                        Message = "Successfully Sent To Despensary.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }


            }
            else
            {
                var formattedData = new
                {
                    Message = "This Request is sent before can't sent twice.",
                    Status = "FAIL"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            var formattedData2 = new
            {
                Message = "Failed To Send.",
                Status = "FAIL"
            };
            return Json(formattedData2, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SendToDespensaryPatientRegister(int patientId, int cardDetailId)
        {
            var drugOrderLists = db.PatientDrugOrders.Where(x => x.PatientId == patientId && x.PatientCardDetailId == cardDetailId).ToList();
            if (drugOrderLists.Any())
            {
                var orderlists = drugOrderLists.Where(x => !x.IsSentToPatientRegiser).ToList();
                if (orderlists.Any())
                {
                    var singlePatientCard = orderlists.FirstOrDefault();
                  
                    var patientCard = db.PatientCard.FirstOrDefault(x => x.PatientId == singlePatientCard.PatientId);
                    if (patientCard != null)
                    {
                        var dateAm = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Today.Day, DateTime.Today.Month, DateTime.Today.Year);
                        int age = 0;
                        var gender = Gender.MALE;
                        var personEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == patientCard.EmployeeId);
                        if (personEmployee != null)
                        {
                            age = DateTime.Today.Year - personEmployee.DateOfBirth.Year;
                            gender = personEmployee.Gender;
                        }

                        var patientRegister = new PatientRegister
                        {
                            PatientName = "",
                            CardNumber = patientCard.PatientCardId,
                            RegisterDate = DateTime.Today,
                            RegisterDateAmh = dateAm,
                            Age = age,
                            Sex = gender,
                            NameOfOrg = "",
                            PatientSignature = "",
                            DateCreated = DateTime.Now,
                            Unit = "",
                            CreatedBy = User.Identity.Name,
                            IsCredit = false,
                            EmployeeId = patientCard.EmployeeId,
                            DrugistId = 0,//UserInformationHelper.GetEmployeeId(User.Identity.Name),
                            IsDone = false,
                        };

                        db.PatientRegisters.Add(patientRegister);
                        db.SaveChanges();


                        foreach (var order in orderlists)
                        {
                            if (!order.IsSentToPatientRegiser)
                            {

                                db = new ExceedDbContext();

                                order.IsSentToPatientRegiser = true;
                                db.Entry(order).State = EntityState.Modified;


                                var patientRegisterDetail = new PatientRegisterDetail
                                {
                                    PatientRegisterId = patientRegister.PatientRegisterId,
                                    DrugListsId = order.DrugListsId,
                                    TotalQty = order.Quantity,
                                    RetailPrice = 0,//patientRegisterDetail.RetailPrice,
                                    TotalRetailPrice = 0,//patientRegisterDetail.TotalQty * patientRegisterDetail.RetailPrice,
                                    DrugUnitId = order.DrugUnitId,//patientRegisterDetail.DrugUnitId,
                                    InternalCode = "",//patientRegisterDetail.InternalCode,
                                    IsUpdated = false,//patientRegisterDetail.IsUpdated,
                                };

                                db.PatientRegisterDetails.Add(patientRegisterDetail);

                                var patientCardDetail = db.PatientCardDetail.FirstOrDefault(x => x.PatientCardDetailId == cardDetailId);
                                if (patientCardDetail != null)
                                {
                                    patientCardDetail.IsSentDespensary = true;

                                    db.Entry(patientCardDetail).State = EntityState.Modified;
                                }
                                db.SaveChanges();

                            }

                        }
                        var formattedData = new
                        {
                            Message = "Successfully Sent To Despensary.",
                            Status = "OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var formattedData = new
                        {
                            Message = "Invalid Request.",
                            Status = "OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }                   

                }
                else
                {
                    var formattedData = new
                    {
                        Message = "All are sent before.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var formattedData = new
                {
                    Message = "First add drugs to sent.",
                    Status = "FAIL"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}