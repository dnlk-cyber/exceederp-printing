using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.IO;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class InsuranceController : Controller
    {
        IInsuranceService _InsuranceService;
        IFleetService _FleetService;
        public InsuranceController()
        {
            _InsuranceService = new InsuranceService();
            _FleetService = new FleetService();
        }

        //
        // GET: /FleetManagement/Insurance/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Insurance_Read([DataSourceRequest] DataSourceRequest request)
        {
            var Insurance = _InsuranceService.GetAll().Select(i => new FleetInsurance

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
                InsuranceType = i.InsuranceType,
                FleetsID = i.FleetsID,
                Attachment = i.Fleet.PlateNumber,
           
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
                Insurance.FleetInsuranceID = Guid.NewGuid();
                Insurance.CreatedBy = User.Identity.Name;
                Insurance.DateCreated = DateTime.Now;
               
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


    }
}

