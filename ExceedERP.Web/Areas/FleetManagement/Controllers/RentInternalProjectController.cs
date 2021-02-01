using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
   //  [AuthorizeRoles(FleetManagementRoles.RentRequestor)] 
    public class RentInternalProjectController : Controller
    {
        IRentInternalProjectService _RentInternalProjectService;
        public RentInternalProjectController()
        {

            _RentInternalProjectService = new RentInternalProjectService();
        }
        //
        // GET: /FleetManagement/RentInternalProject/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RentInternalProject_Read([DataSourceRequest] DataSourceRequest request)
        {
            var RentInternalProject = _RentInternalProjectService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name));

            return Json(RentInternalProject.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentInternalProject_Create([DataSourceRequest] DataSourceRequest request, RentInternalProject RentInternalProject)
        {
            ModelState.Remove("RentInternalProjectID");
            if (RentInternalProject.StartingDate > RentInternalProject.ToDate)
            {
               
                ModelState.Clear();
                ModelState.AddModelError("DateIncorrect", " Start Date Is Above End Date ");

            }
            else
            {
                if (RentInternalProject != null)
                {
                    RentInternalProject.RentInternalProjectID = Guid.NewGuid();
                    RentInternalProject.Status = MaintenanceConst.Pending;
                    RentInternalProject.CreatedBy = User.Identity.Name;
                    RentInternalProject.DateCreated = DateTime.Now;
                    RentInternalProject.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    _RentInternalProjectService.Insert(RentInternalProject);
                }
            }

            return Json(new[] { RentInternalProject }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentInternalProject_Update([DataSourceRequest] DataSourceRequest request, RentInternalProject RentInternalProject)
        {
            if (RentInternalProject.StartingDate > RentInternalProject.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("DateIncorrect", " Start Date Is Above End Date ");

            }
            else
            {
                if (RentInternalProject != null)
                {
                    RentInternalProject.ModifiedBy = User.Identity.Name;
                    RentInternalProject.LastModified = DateTime.Now;
                    RentInternalProject.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                 
                    _RentInternalProjectService.Update(RentInternalProject);
                }
            }
            return Json(new[] { RentInternalProject }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentInternalProject_Destroy([DataSourceRequest] DataSourceRequest request, RentInternalProject RentInternalProject)
        {
            var rentInternalProject = _RentInternalProjectService.GetById(RentInternalProject.RentInternalProjectID);
            if (rentInternalProject != null)
            {
                _RentInternalProjectService.Delete(rentInternalProject);

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Rent for Internal number " + RentInternalProject.RentInternalProjectID);
            }

            return Json(new[] { RentInternalProject }.ToDataSourceResult(request));
        }



    }
}

