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
using System.IO;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.HRM.HealthAndInsurance;
using ExceedERP.Web.Areas.FleetManagement.Models; 


namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.RegistrationUser)]
    public class InsuranceRegistrationController : Controller
    {
        IInsuranceService _InsuranceService;
        IFleetService _FleetService;
        ExceedDbContext db = new ExceedDbContext();
        public InsuranceRegistrationController()
        {
            _InsuranceService = new InsuranceService();
            _FleetService = new FleetService();
        }

        //
        // GET: /FleetManagement/Insurance/
        public ActionResult Index()
        {
            ViewData["Fleets"] = db.Fleets.ToList();
            return View();
        }
        public ActionResult InsuranceFollowUp()
        {
            ViewData["Fleets"] = db.Fleets.ToList();
            return View();
        }
        public ActionResult InsuranceFollowUp_Read([DataSourceRequest] DataSourceRequest request)
        {
            var Insurance = _InsuranceService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Where(x => x.IsSendForApproval == true).Select(i => new 

            {
                FleetInsuranceID = i.FleetInsuranceID,
                InsuranceCompany = i.InsuranceCompany,
                PremiumCost = i.PremiumCost,
                Description = i.Description,
                PhoneNo = i.PhoneNo,
                Email = i.Email,
                City = i.City,
                DueDate = i.DueDate,
                ExpireDate = i.ExpireDate,
                FleetsID = i.FleetsID,
                PlateNo = i.Fleet.PlateNumber,
                SerialNo = i.Fleet.SerialNo,
                ChasisNo = i.Fleet.ChasisNumber,
                IsSendForApproval = i.IsSendForApproval,
                ModifiedBy = i.ModifiedBy,
                CreatedBy = i.CreatedBy,
                Status = i.Status,
                DateCreated = i.DateCreated
            }

            );

            return Json(Insurance.ToDataSourceResult(request));
        }

        public ActionResult Insurance_Read([DataSourceRequest] DataSourceRequest request)
        {
            var Insurance = _InsuranceService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(i => new InsuranceViewModel

            {
                FleetInsuranceID = i.FleetInsuranceID,
                InsuranceCompany = i.InsuranceCompany,
                PremiumCost = i.PremiumCost,
                Description = i.Description,
                PhoneNo = i.PhoneNo,
                Email = i.Email,
                City = i.City,
                DueDate = i.DueDate,
                ExpireDate = i.ExpireDate,
                FleetsID = i.FleetsID,
                PlateNo = i.Fleet.PlateNumber,
                SerialNo = i.Fleet.SerialNo,
                ChasisNo = i.Fleet.ChasisNumber,
                IsSendForApproval = i.IsSendForApproval,
             Status = i.Status,
                ModifiedBy = i.ModifiedBy,
                CreatedBy = i.CreatedBy,
                DateCreated = i.DateCreated
            }

            );

            return Json(Insurance.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Insurance_Create([DataSourceRequest] DataSourceRequest request, FleetInsurance Insurance)
        {
            ModelState.Remove("FleetInsuranceID");

            if (Insurance != null)
            {
                Insurance.InsuranceCompany = "Not Assigned";//InsuranceCompany;
                Insurance.PremiumCost = 0;//PremiumCost;
                Insurance.Description = "Not Assigned";//Description;
                Insurance.PhoneNo = "Not Assigned";//PhoneNo;
                Insurance.Email = "Not Assigned";//Email;
                Insurance.City = "Not Assigned";//City;
                Insurance.DueDate = DateTime.Now;//DueDate;
                Insurance.ExpireDate = DateTime.Now;//ExpireDate;
                Insurance.FleetInsuranceID = Guid.NewGuid();
                 Insurance.CreatedBy = User.Identity.Name;
                Insurance.DateCreated = DateTime.Now;
                Insurance.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
               
                _InsuranceService.Insert(Insurance);

             }

            return Json(new[] { Insurance }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Insurance_Update([DataSourceRequest] DataSourceRequest request, FleetInsurance Insurance)
        {
            if (Insurance != null)
            {
                Insurance.ModifiedBy = User.Identity.Name;
                Insurance.LastModified = DateTime.Now;
                Insurance.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                _InsuranceService.Update(Insurance);
            }

            return Json(new[] { Insurance }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Insurance_Destroy([DataSourceRequest] DataSourceRequest request, FleetInsurance Insurance)
        {
            var insurance = _InsuranceService.GetById(Insurance.FleetInsuranceID);
            if (insurance != null)
            {

                _InsuranceService.Delete(insurance);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment Type number " + insurance.FleetInsuranceID);

            }

            return Json(new[] { Insurance }.ToDataSourceResult(request));
        }

        public ActionResult UploadAttachment([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> file)
        {




            foreach (HttpPostedFileBase fileBase in file)
            {
                string fileExtension = System.IO.Path.GetExtension(Request.Files["file"].FileName);
                if (fileExtension == ".png" || fileExtension == ".jpg" || fileExtension == ".gif")
                {
                    string path = Server.MapPath("~/Content/Import/Insurance/") + DateTime.Now.Year.ToString() +
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
            var insurances = _InsuranceService.GetAll();

            return this.Json(insurances.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendForOnlineApproval(string Id)
        {
            var FleetInsurance = db.FleetInsurance.Where(x => x.FleetInsuranceID.ToString() == Id).FirstOrDefault();
            if (FleetInsurance != null)
            {
                FleetInsurance.Status ="Sent For Insurance";
                FleetInsurance.IsSendForApproval = true;
                FleetInsurance.SendForApprovalBy = User.Identity.Name;
                FleetInsurance.SendForApprovalTime = DateTime.Now;

               
                    db.FleetInsurance.Attach(FleetInsurance);
                    db.Entry(FleetInsurance).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                
            }
            return RedirectToAction("Index");

        }



    }
}

