using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class EducationallevelController : Controller
    {
        IEducationallevelService _EducationallevelService;

        public EducationallevelController()
        {
            _EducationallevelService = new EducationallevelService();
        }
        //
        // GET: /FleetManagement/Educationallevel/
        public ActionResult Index()
        {

            return View();
        }


        public JsonResult Educationallevel_Read([DataSourceRequest] DataSourceRequest request)
        {
            _EducationallevelService = new EducationallevelService();
            var oper = _EducationallevelService.GetAll();
            var Opers = oper.Select(o => new Educationallevel
            {
                EducationallevelID = o.EducationallevelID,
                Name = o.Name,
                Description = o.Description


            });

            return this.Json(Opers.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Educationallevel_Create([DataSourceRequest] DataSourceRequest request, Educationallevel Educationallevel)
        {
            ModelState.Remove("EducationallevelID");
            List<Educationallevel> Educationallevels = _EducationallevelService.GetAll().Where(f => f.Name == Educationallevel.Name && f.EducationallevelID != Educationallevel.EducationallevelID).ToList();
            if (Educationallevels.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Educational level Already Exists ");
                return this.Json(new[] { Educationallevel }.ToDataSourceResult(request, ModelState));
            }



            if (Educationallevel != null)
            {
                Educationallevel.EducationallevelID = Guid.NewGuid();
                Educationallevel.CreatedBy = User.Identity.Name;
                Educationallevel.DateCreated = DateTime.Now;
                _EducationallevelService.Insert(Educationallevel);
            }

            return this.Json(new[] { Educationallevel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Educationallevel_Update([DataSourceRequest] DataSourceRequest request, Educationallevel Educationallevel)
        {
            List<Educationallevel> Educationallevels = _EducationallevelService.GetAll().Where(f => f.Name == Educationallevel.Name && f.EducationallevelID != Educationallevel.EducationallevelID).ToList();
            if (Educationallevels.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Educational level Already Exists ");
                return this.Json(new[] { Educationallevel }.ToDataSourceResult(request, ModelState));
            }

            if (Educationallevel != null)
            {
                Educationallevel.LastModified = DateTime.Now;
                Educationallevel.ModifiedBy = User.Identity.Name;
                _EducationallevelService.AddOrUpdate(Educationallevel);
            }


            return this.Json(new[] { Educationallevel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult Educationallevel_Destroy([DataSourceRequest] DataSourceRequest request, Educationallevel Educationallevel)
        {
            var Educationallevels = _EducationallevelService.GetById(Educationallevel.EducationallevelID);
            if (Educationallevels != null)
            {
                Educationallevel.ModifiedBy = User.Identity.Name;
                Educationallevel.LastModified = DateTime.Now;
                _EducationallevelService.Delete(Educationallevels);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Education Level number " + Educationallevel.EducationallevelID);

            }

            return this.Json(new[] { Educationallevel }.ToDataSourceResult(request));
        }

        [AllowAnonymous]
        public ActionResult EducationallevelDDBind()
        {

            var fleets = _EducationallevelService.GetAll().Select(s => new Educationallevel
            {
                Name = s.Name,
                EducationallevelID = s.EducationallevelID
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }


    }
}