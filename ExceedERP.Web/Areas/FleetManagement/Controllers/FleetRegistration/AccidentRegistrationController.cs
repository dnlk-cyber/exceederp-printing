using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Web.Areas.FleetManagement.Models;
using System.IO;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.RegistrationUser, FleetManagementRoles.RegistrationApprover)]
    public class AccidentRegistrationController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        IAccidentService _Accidentervice;
        IFleetService _FleetService;
        public AccidentRegistrationController()
        {
            _Accidentervice = new AccidentService();
            _FleetService = new FleetService();
        }
        //
        // GET: /FleetManagement/Accident/
        public ActionResult Index()
        {
            ViewData["Operators"] = db.Operator.ToList().Select(s => new {
                Id = s.OperatorID,
                Name = s.FullName
            });
            return View();
        }
        public JsonResult Accident_Read([DataSourceRequest] DataSourceRequest request)
        {
            var accident = _Accidentervice.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).ToList();
            var accidents = accident.Select(a => new Accident
            {
                FleetsID = a.FleetsID,
                PlateNo = a.Fleet.PlateNumber,
                AssetCode = a.Fleet.UnitNum,
                SerialNo = a.Fleet.SerialNo,
                ChasisNo = a.Fleet.ChasisNumber,
                AccidentLocation = a.AccidentLocation,
                Accountablebody = a.Accountablebody,
                CauseOfAccident = a.CauseOfAccident,
                CostcoveredbyCompany = a.CostcoveredbyCompany,
                DateOfOccurrence = a.DateOfOccurrence,
                CostcoveredbyInsurance = a.CostcoveredbyInsurance,
                CostcoveredbyOperator = a.CostcoveredbyOperator,
                CostcoveredbyOtherBodies = a.CostcoveredbyOtherBodies,
                Description = a.Description,
                LevelOfDamage = a.LevelOfDamage,
                WitnessInYourCar = a.WitnessInYourCar,
                LicenseNo = a.LicenseNo,
                Status = a.Status,
                IsSendForApproval = a.IsSendForApproval,
                AccidentID = a.AccidentID,
                DateCreated = a.DateCreated,
                LastModified = a.LastModified,
                ModifiedBy = a.ModifiedBy,
                CreatedBy = a.CreatedBy,
                IsHornHeared = a.IsHornHeared,
                PersonWasAtAccident = a.PersonWasAtAccident,
                InjuredPersons = a.InjuredPersons,
                ThirdPartyDamage = a.ThirdPartyDamage,
                WittnessDescription = a.WittnessDescription,
                AssetDamage = a.AssetDamage,
                PolicyNo = a.PolicyNo,
                VehicleSpeed = a.VehicleSpeed,
                OpionionResponsible = a.OpionionResponsible,
                VehicleDistanceFromRoadEdge = a.VehicleDistanceFromRoadEdge,
                OperatorId = a.OperatorId,
                Insured = a.Insured,
                Address = a.Address,
                PhoneNo = a.PhoneNo,
                FullName = a.FullName,
                OtherPartyFullName = a.OtherPartyFullName,
                OtherPartyAddress = a.OtherPartyAddress,
                PoliceReportAddress = a.PoliceReportAddress,
                PoliceReportDate = a.PoliceReportDate

            }).ToList();

            return Json(accidents.ToDataSourceResult(request));
        }
        public JsonResult Accident_Create([DataSourceRequest] DataSourceRequest request, Accident Accident)
        {

            ModelState.Remove("AccidentID");
            Accident.AccidentID = Guid.NewGuid();
            Accident.CreatedBy = User.Identity.Name;
            Accident.DateCreated = DateTime.Now;
            Accident.Status = "Pending";
            Accident.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

            if (Accident != null)
            {
                _Accidentervice.Insert(Accident);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Accident registration", "Success", User.Identity.Name, $"created accident {Accident.AccidentID} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { Accident }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Accident_Update([DataSourceRequest] DataSourceRequest request, Accident Accident)
        {

            if (Accident != null)
            {
                Accident.LastModified = DateTime.Now;
                Accident.RenewalDate = DateTime.Now;
                Accident.ModifiedBy = User.Identity.Name;
                Accident.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                _Accidentervice.Update(Accident);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Accident update", "Success", User.Identity.Name, $"updated accident {Accident.AccidentID}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { Accident }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Accident_Destroy([DataSourceRequest] DataSourceRequest request, Accident Accident)
        {
            var accident = _Accidentervice.GetById(Accident.AccidentID);

            if (accident != null)
            {
                _Accidentervice.Delete(accident);

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted accident " + accident.AccidentID);

            }

            return Json(new[] { Accident }.ToDataSourceResult(request));
        }


        public ActionResult UploadReport([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        {



            foreach (HttpPostedFileBase fileBase in file)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                if (fileExtension == ".pdf" || fileExtension == ".doc" || fileExtension == ".docx")
                {
                    string path = Server.MapPath("~/Content/Import/Accident/") + DateTime.Now.Year.ToString() +
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
            var accident = _Accidentervice.GetAll();

            return this.Json(accident.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }



        public ActionResult SendForOnlineApproval(string Id)
        {
            ExceedDbContext db = new ExceedDbContext();
            var Accident = db.Accident.Where(x => x.AccidentID.ToString() == Id).FirstOrDefault();
            if (Accident != null)
            {
                Accident.Status = "Sent For Insurance";
                Accident.IsSendForApproval = true;
                Accident.SendForApprovalBy = User.Identity.Name;
                Accident.SendForApprovalTime = DateTime.Now;


                db.Accident.Attach(Accident);
                db.Entry(Accident).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            return RedirectToAction("Index");

        }

    }
}