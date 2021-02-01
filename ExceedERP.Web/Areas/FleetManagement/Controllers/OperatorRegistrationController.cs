using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using ExceedERP.Web.Areas.FleetManagement.Models;
using System.IO;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using System.Data.Entity.Validation;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.RegistrationUser)]
    public class OperatorRegistrationController : Controller
    {
        IOperatorService _OperatorService;
        IFleetService fleetService = new FleetService();
        ExceedDbContext db = new ExceedDbContext();
        public OperatorRegistrationController()
        {
            _OperatorService = new OperatorService();
        }
        //
        // GET: /FleetManagement/Operator/


        public ActionResult Index()
        {
            ExceedDbContext db = new ExceedDbContext();

            ViewData["Employee"] = db.Person_Employee.ToList();

            return View();
        }


        public JsonResult Operator_Read([DataSourceRequest] DataSourceRequest request)
        {
            _OperatorService = new OperatorService();
            var oper = _OperatorService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).ToList();
            var Opers = oper.Select(o => new Operator
            {
                OperatorID = o.OperatorID,
                FullName = o.FullName,
                MobilePhone = o.MobilePhone,
                GuaranteePhoneNumber = o.GuaranteePhoneNumber,
                GuaranteeName = o.GuaranteeName,
                GuaranteeAddress = o.GuaranteeAddress,
                SalaryIncentive = o.SalaryIncentive,
                Gender = o.Gender,
                Email = o.Email,
                Postion = o.Postion,
                NameOfTraningCenter = o.NameOfTraningCenter,
                LicenseNo = o.LicenseNo,
                LicenseCity = o.LicenseCity,
                LicenseExpireDate = o.LicenseExpireDate,
                LicenseIssuedDate = o.LicenseIssuedDate,
                LicenseLevel = o.LicenseLevel,
                LicenseRenewalDate = o.LicenseRenewalDate,
                LicenseType = o.LicenseType,
                HireDate = o.HireDate,
                FleetsID = o.FleetsID,
                EmployeeId = o.EmployeeId,
                EducationLevel = o.EducationLevel


            });

            return this.Json(Opers.ToDataSourceResult(request, ModelState));
        }


        public ActionResult UploadPic(ExceedERP.Web.Areas.FleetManagement.Models.UploadBindingModel model, IEnumerable<HttpPostedFileBase> AttachmentFile)
        {



            try
            {
                foreach (var fileBase in model.AttachmentFile)
                {

                    string path = Server.MapPath("~/Content/Images/Fleet/Operator/");
                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    fileBase.SaveAs(Path.Combine(Server.MapPath("~/Content/Images/Fleet/Operator/" + model.Id + ".jpg")));


                }
            }
            catch (Exception e)
            {
                
            }
            return RedirectToAction("Index");
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Operator_Create([DataSourceRequest] DataSourceRequest request, Operator Operator)
        {
            ModelState.Remove("OperatorID");
            ModelState.Remove("FleetsID");
            List<Operator> Operators = _OperatorService.GetAll().Where(f => f.FullName == Operator.FullName).ToList();
           
                if (Operators.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Operator with Name Already Exists !");
                return this.Json(new[] { Operator }.ToDataSourceResult(request, ModelState));
            }

            else
            {
                var Employee = db.Person_Employee.Where(x => x.EmployeeId == Operator.EmployeeId).FirstOrDefault();

                if (Employee != null)
                {
                    try
                    {
                        Operator.OperatorID = Guid.NewGuid();
                        Operator.EmployeeId = Employee.EmployeeId;
                        Operator.FullName = Employee.FirstName + " " + Employee.LastName;
                        Operator.HireDate = DateTime.Now;
                        Operator.CreatedBy = User.Identity.Name;
                        Operator.DateCreated = DateTime.Now;
                        Operator.HireDate = DateTime.Now;
                        Operator.LicenseLevel = "--";
                        Operator.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                        _OperatorService.Insert(Operator);
                    }
                    catch (DbEntityValidationException e)
                    {
                        Operator.LicenseIssuedDate = DateTime.Now;
                        _OperatorService.Insert(Operator);
                    }
                }
            }
            return this.Json(new[] { Operator }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Operator_Update([DataSourceRequest] DataSourceRequest request, Operator Operator)
        {
            List<Operator> Operators = _OperatorService.GetAll().Where(f => f.FullName == Operator.FullName && f.OperatorID != Operator.OperatorID).ToList();
            var Assigned = fleetService.GetAll().Where(x => x.OperatorName == Operator.FullName ).ToList();

            if (Operators.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Operator with Name Already Exists !");
            }
            else if (Assigned.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingEquipment", " Equipment Already Assigned ! ");
            }
            else if (Operator != null)
            {
                var Employee = db.Person_Employee.Where(x => x.EmployeeId == Operator.EmployeeId).FirstOrDefault();

                if (Employee != null)
                {
                    Operator.FullName = Employee.FirstName + " " + Employee.MiddleName + " " + Employee.LastName;
             
                    Operator.ModifiedBy = User.Identity.Name;
                    Operator.LastModified = DateTime.Now;
                    Operator.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
               
                    _OperatorService = new OperatorService();

                    _OperatorService.Update(Operator);
                }
            }


            return this.Json(new[] { Operator }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Operator_Destroy([DataSourceRequest] DataSourceRequest request, Operator Operator)
        {
            var operators = _OperatorService.GetById(Operator.OperatorID);
            if (operators != null)
            {
                _OperatorService.Delete(operators);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Operator Id " + Operator.OperatorID);

            }

            return this.Json(new[] { Operator }.ToDataSourceResult(request));
        }




        [AllowAnonymous]
        public ActionResult OperatorUnAssignedDDBind()
        {

            var fleets = _OperatorService.GetAll().Where(x => x.IsActive == false).Select(s => new Operator
            {
                FullName = s.FullName,
                OperatorID = s.OperatorID
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult OperatorDDBind()
        {

            var fleets = _OperatorService.GetAll().Select(s => new Operator
            {
                FullName = s.FullName,
                OperatorID = s.OperatorID
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult OperatorEquipmentDDBind()
        {
            IFleetService _FleetService = new FleetService();


            var fleets = _FleetService.GetAll().Where(x => x.OperatorName != MaintenanceConst.UnAssigned ).Select(s => new Operator
            {
                FullName = s.OperatorName + "-" + s.PlateNumber,
                Remark = s.OperatorName
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }


        public ActionResult OperatorTransferDDBind()
        {
            IFleetService _FleetService = new FleetService();


            var fleets = _FleetService.GetAll().Where(x => x.OperatorName != string.Empty && x.OperatorName != MaintenanceConst.UnAssigned).Select(s => new Operator
            {
                FullName = s.OperatorName + "-" + s.UnitNum,
                Remark = s.OperatorName
            });

            return Json(fleets.Where(x=>x.Remark != null || x.Remark == "" ), JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult OperatorTODDBind(string FromOperatorId)
        {

            var fleets = _OperatorService.GetAll().Where(x => x.FullName != FromOperatorId).Select(s => new Operator
            {
                FullName = s.FullName,
                OperatorID = s.OperatorID
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult OperatorFleetTODDBind(string fleetId)
        {

            var fleets = _OperatorService.GetAll().Where(x => x.FleetsID != Guid.Parse(fleetId)).Select(s => new Operator
            {
                FullName = s.FullName,
                OperatorID = s.OperatorID
            }).FirstOrDefault();

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadPhoto([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        {



            foreach (HttpPostedFileBase fileBase in file)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                if (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".gif")
                {
                    string path = Server.MapPath("~/Content/Import/Operator/") + DateTime.Now.Year.ToString() +
                                          DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                                          DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                                          DateTime.Now.Second.ToString() + Request.Files["file"].FileName;
                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    Request.Files["file"].SaveAs(path);
                }
            }
            var operators = _OperatorService.GetAll();

            return this.Json(operators.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public ActionResult UploadAttachment([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        {



            foreach (HttpPostedFileBase fileBase in file)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                if (fileExtension == ".pdf" || fileExtension == ".doc" || fileExtension == ".docx")
                {
                    string path = Server.MapPath("~/Content/Import/Operator/Attachment/") + DateTime.Now.Year.ToString() +
                                          DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() +
                                          DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() +
                                          DateTime.Now.Second.ToString() + Request.Files["file"].FileName;
                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    Request.Files["file"].SaveAs(path);
                }
            }
            var operators = _OperatorService.GetAll();

            return this.Json(operators.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


    }
}
