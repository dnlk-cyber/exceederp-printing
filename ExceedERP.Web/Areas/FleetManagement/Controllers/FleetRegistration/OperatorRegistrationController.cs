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
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using System.Data.Entity.Validation;
using ExceedERP.Core.Domain.Global;
using System.Data.Entity;
using ExceedERP.Core.Domain.Maintenance;
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
        public ActionResult Operator_Read([DataSourceRequest] DataSourceRequest request)
        {
            _OperatorService = new OperatorService();
            var oper = _OperatorService.GetAll().ToList();
            var Opers = oper.Select(o => new Operator
            {
                OperatorID = o.OperatorID,
                FullName = o.FullName,
                JobTitle = GetPlacmentCategory(o.EmployeeId),
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
        public string GetPlacmentCategory(int Emp)
        {
            var Placements = db.Placements.Where(x => x.EmployeeId == Emp).FirstOrDefault();
            if (Placements != null)
            {
                var Title = db.Lookups.Where(x => x.LookupId == Placements.LookupId).FirstOrDefault();
                if(Title!=null)
                {
                    return Title.Description;
                }
            }
            return "";            
        }
        public JsonResult Seed()
        {
            var operators = db.Operator.ToList();
            foreach (var item in operators)
            {
                var employee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == item.EmployeeId);
                if(employee != null)
                {
                    item.EmployeeCode = employee?.EmployeeCode ?? "";
                }
            }
            db.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Operator_Create([DataSourceRequest] DataSourceRequest request, Operator Operator)
        {
            ModelState.Remove("OperatorID");
            ModelState.Remove("FleetsID");
            List<Operator> Operators = _OperatorService.GetAll().Where(f => f.EmployeeId == Operator.EmployeeId).ToList();           
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
                        Operator.EmployeeCode = Employee.EmployeeCode;
                        Operator.OperatorID = Guid.NewGuid();
                        Operator.EmployeeId = Employee.EmployeeId;
                        Operator.FullName = Employee.FirstName + " " + Employee.MiddleName+" "+Employee.LastName;
                        Operator.HireDate = DateTime.Now;
                        Operator.CreatedBy = User.Identity.Name;
                        Operator.DateCreated = DateTime.Now;
                        Operator.HireDate = DateTime.Now;
                        Operator.Gender = Employee.Gender.ToString();
                        Operator.LicenseLevel = "--";
                        Operator.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        _OperatorService.Insert(Operator);
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Operator registration", "Success", User.Identity.Name, $"created operator {Operator.OperatorID}", Modules.FleetManagement);
                        #endregion
                    }
                    catch (DbEntityValidationException e)
                    {
                        Operator.LicenseIssuedDate = DateTime.Now;
                        _OperatorService.Insert(Operator);
                        //#region operation log
                        //UserHelper.OperationLog(Request.UserHostAddress, "Operator registration", "Success", User.Identity.Name, $"created operator {Operator.OperatorID}", Modules.FleetManagement);
                        //#endregion
                    }
                }
            }
            return this.Json(new[] { Operator }.ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Operator_Update([DataSourceRequest] DataSourceRequest request, Operator Operator)
        {
            List<Operator> Operators = _OperatorService.GetAll().Where(f => f.EmployeeId == Operator.EmployeeId && f.OperatorID != Operator.OperatorID).ToList();
            var Assigned = fleetService.GetAll().Where(x => x.OperatorName == Operator.FullName ).ToList();
            if (Operators.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Operator with Name Already Exists !");
            }
            //else if (Assigned.Any())
            //{
            //    ModelState.Clear();
            //    ModelState.AddModelError("ExistingEquipment", " Equipment Already Assigned ! ");
            //}
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
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Operator registration", "Success", User.Identity.Name, $"updated operator {Operator.OperatorID}", Modules.FleetManagement);
                    #endregion
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
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Operator " + Operator.OperatorID);
            }
            return this.Json(new[] { Operator }.ToDataSourceResult(request));
        }
        
        public ActionResult OperatorUnAssignedDDBind()
        {
            var entity = _OperatorService.GetAll()/*.Where(x => x.IsActive == false)*/.Select(s => new Operator
            {
                FullName = s.FullName,
                OperatorID = s.OperatorID
            });

            return Json(entity, JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult OperatorDDBind()
        {
            var fleets = _OperatorService.GetAll().Select(s => new Operator
            {
                FullName = s.FullName,
                OperatorID = s.OperatorID
            });
            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        
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

       
        public JsonResult RefreshOperator()
        {
            var count = 0;
            var mapping = db.HRFleetMapping.Where(x => x.PlacmentType == FleetPlacmentType.OPERATOR || x.PlacmentType == FleetPlacmentType.DRIVER).ToList();
            foreach (var map in mapping)
            {
                var placements = db.Placements
                    .Where(x=>x.isCurrent)
                    .Where(x => x.LookupId == map.JobTitleCode).ToList();
                foreach(var placement in placements)
                {
                    var Employee = db.Person_Employee.Find(placement.EmployeeId);
                    var exist = db.Operator.Where(x => x.EmployeeId == placement.EmployeeId).FirstOrDefault();
                    if (exist == null)
                    {
                        var entity = new Operator
                        {
                            OperatorID = Guid.NewGuid(),
                            EmployeeId = placement.EmployeeId,
                            FullName = Employee.FullNameaEnglish,
                            SalaryIncentive = "12",
                            Gender = Employee.Gender.ToString(),
                            TypeofEmploymentenum = TypeofEmploymentenum.Permanent,
                            HireDate = DateTime.Now,
                            DateOfTermination = DateTime.Now,
                            Postionenum = Postionenum.AssistantOperator,
                            Postion = placement.JobTitleName,
                            Genderenum = Genderenum.Female,
                            SalaryIncentiveenum = SalaryIncentiveenum.DailyPremium,
                            EducationLevel = "Education Level",
                            LicenseNo = "License No",
                            LicenseType = "License Type",
                            LicenseTypeenum = LicenseTypeenum.NewVersion,
                            LicenseLevelenum = LicenseLevelenum.Hizeb1,
                            LicenseLevel = "License Level",
                            NameOfTraningCenter = "Name of Traning Center",
                            COCResult = COCResult.Attached,
                            LicenseCity = "License City",
                            MobilePhone = "0910000000",
                            Email = "exceed@gmail.com",
                            GuaranteeName = "Addis",
                            GuaranteeAddress = "Addis",
                            GuaranteePhoneNumber = "0910000000",
                            IsActive = true,
                            Attachment = "Addis",
                            OperatorPhoto = "tde.jpg",
                            Status = "Status",
                            Remark = "Remark",
                            IsVoid = false,
                            VoidBy = "Some one",
                            IsOnlineApproved = false,
                            OnlineApprovedBy = "to be",
                            IsOnlineTransferred = false,
                            IsOnlineTransferredBy = "",
                            CreatedBy = User.Identity.Name,
                            ModifiedBy=User.Identity.Name,
                            LicenseIssuedDate=DateTime.Now

                        };
                        db.Operator.Add(entity);
                        db.SaveChanges();
                        count = count + 1;
                    }
                }
            }
            // return Json(mapping, JsonRequestBehavior.AllowGet);
            return Json(count + " items added");
        }
        
        public ActionResult OperatorTODDBind(string FromOperatorId)
        {
            var fleets = _OperatorService.GetAll().Where(x => x.FullName != FromOperatorId).Select(s => new Operator
            {
                FullName = s.FullName,
                OperatorID = s.OperatorID
            });
            return Json(fleets, JsonRequestBehavior.AllowGet);
        }
        
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
        public ActionResult LicenseLevel__Read([DataSourceRequest]DataSourceRequest request, Guid Id)
        {
            IQueryable<LicenseLevel> licenseLevels = db.LicenseLevels.Where(x => x.OperatorId == Id);
            DataSourceResult result = licenseLevels.ToDataSourceResult(request, licenseLevel => new {
                LicenseLevelId = licenseLevel.LicenseLevelId,
                Name = licenseLevel.Name,
                Description = licenseLevel.Description,
                DateCreated = licenseLevel.DateCreated,
                LastModified = licenseLevel.LastModified,
                CreatedBy = licenseLevel.CreatedBy,
                ModifiedBy = licenseLevel.ModifiedBy
            });
            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LicenseLevel_Create([DataSourceRequest]DataSourceRequest request, LicenseLevel licenseLevel, Guid Id)
        {
            ModelState.Remove("OperatorId");
            var licenselevel = db.LicenseLevels.Where(x => x.OperatorId == Id).ToList();
            if (ModelState.IsValid)
            {
                var entity = new LicenseLevel
                {
                    Name = licenseLevel.Name,
                    Description = licenseLevel.Description,
                    OperatorId = Id,
                    DateCreated = DateTime.Now,
                    LastModified = licenseLevel.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = licenseLevel.ModifiedBy
                };
                db.LicenseLevels.Add(entity);
                db.SaveChanges();
                licenseLevel.LicenseLevelId = entity.LicenseLevelId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "License level registration", "Success", User.Identity.Name, $"created license level {entity.LicenseLevelId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { licenseLevel }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LicenseLevel_Update([DataSourceRequest] DataSourceRequest request, LicenseLevel licenseLevel,Guid Id)
        {
            if (ModelState.IsValid)
            {
                var entity = new LicenseLevel
                {
                    LicenseLevelId = licenseLevel.LicenseLevelId,
                    Name = licenseLevel.Name,
                    Description = licenseLevel.Description,
                    OperatorId = Id,
                    DateCreated = licenseLevel.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = licenseLevel.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                db.LicenseLevels.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "License level registration", "Success", User.Identity.Name, $"updated license level {entity.LicenseLevelId}", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { licenseLevel }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult LicenseLevel_Destroy([DataSourceRequest] DataSourceRequest request, LicenseLevel licenseLevel)
        {
            if (ModelState.IsValid)
            {
                var entity = new LicenseLevel
                {
                    LicenseLevelId = licenseLevel.LicenseLevelId,
                    OperatorId = licenseLevel.OperatorId,
                    Name = licenseLevel.Name,
                    Description = licenseLevel.Description,
                };
                db.LicenseLevels.Attach(entity);
                db.LicenseLevels.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted license level number {entity.LicenseLevelId }");
                #endregion
            }
            return Json(new[] { licenseLevel }.ToDataSourceResult(request, ModelState));
        }
    }
}
