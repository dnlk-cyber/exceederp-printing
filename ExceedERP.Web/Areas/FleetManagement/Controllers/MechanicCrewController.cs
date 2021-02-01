using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;     
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class MechanicCrewController : Controller
    {
        IMechanicCrewService _MechanicCrewService;
        IFleetService _FleetService;
        ExceedDbContext db = new ExceedDbContext();
        public MechanicCrewController()
        {
            _MechanicCrewService = new MechanicCrewService();
            _FleetService = new FleetService();
        }

        //
        // GET: /FleetManagement/MechanicCrew/
        public ActionResult Index()
        {
            ViewData["Sections"] = db.WorkUnitSection.ToList();
            return View();
        }


        public ActionResult MechanicCrew_Read([DataSourceRequest] DataSourceRequest request)
        {
            var MechanicCrew = _MechanicCrewService.GetAll();

            return Json(MechanicCrew.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MechanicCrew_Create([DataSourceRequest] DataSourceRequest request, MechanicCrew MechanicCrew)
        {
            ModelState.Remove("MechanicCrewID");


            Placement Placements = db.Placements.Where(x => x.EmployeeId  == MechanicCrew.EmployeeId     ).FirstOrDefault();
            if (Placements != null)
            {
                Person_Employee Person_Employee = db.Person_Employee.Where(x => x.EmployeeId == Placements.ImmediateSupervisor).FirstOrDefault();
                if (Person_Employee != null)
                {
                  MechanicCrew.SupervisorName = Person_Employee.FirstName + " " + Person_Employee.MiddleName + " " + Person_Employee.LastName;
                }
            }
       
            List<MechanicCrew> Mechanics = _MechanicCrewService.GetAll().Where(f => f.FullName == MechanicCrew.FullName).ToList();
            if (Mechanics.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Mechanic with Name Already Exists ");
            }
            else
            {
                var Employee = db.Person_Employee.Where(x => x.EmployeeId == MechanicCrew.EmployeeId).FirstOrDefault();

                if (Employee != null)
                {
                    MechanicCrew.MechanicCrewID = Guid.NewGuid();
                    MechanicCrew.FullName = Employee.FirstName + " " + Employee.MiddleName + " " + Employee.LastName;
             
                    MechanicCrew.CreatedBy = User.Identity.Name;
                    MechanicCrew.DateCreated = DateTime.Now;

                    _MechanicCrewService.Insert(MechanicCrew);

                }
            }

            return Json(new[] { MechanicCrew }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MechanicCrew_Update([DataSourceRequest] DataSourceRequest request, MechanicCrew MechanicCrew)
        {

            if (MechanicCrew != null)
            {
                var Employee = db.Person_Employee.Where(x => x.EmployeeId == MechanicCrew.EmployeeId).FirstOrDefault();


                Placement Placements = db.Placements.Where(x => x.EmployeeId == MechanicCrew.EmployeeId).FirstOrDefault();
                if (Placements != null)
                {
                    Person_Employee Person_Employee = db.Person_Employee.Where(x => x.EmployeeId == Placements.ImmediateSupervisor).FirstOrDefault();
                    if (Person_Employee != null)
                    {
                        MechanicCrew.SupervisorName = Person_Employee.FirstName + " " + Person_Employee.MiddleName + " " + Person_Employee.LastName;
                    }
                }
       
                if (Employee != null)
                {
                    MechanicCrew.FullName = Employee.FirstName + " " + Employee.MiddleName + " " + Employee.LastName;
                    MechanicCrew.ModifiedBy = User.Identity.Name;
                    MechanicCrew.LastModified = DateTime.Now;

                    _MechanicCrewService.Update(MechanicCrew);
                }
            }


            return Json(new[] { MechanicCrew }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MechanicCrew_Destroy([DataSourceRequest] DataSourceRequest request, MechanicCrew mechanicCrew)
        {
            var MechanicCrew = _MechanicCrewService.GetById(mechanicCrew.MechanicCrewID);
            if (MechanicCrew != null)
            {
                _MechanicCrewService.Delete(MechanicCrew);

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Mechanic Id " + MechanicCrew.MechanicCrewID);

            }

            return Json(new[] { MechanicCrew }.ToDataSourceResult(request));
        }
        [AllowAnonymous]
        public ActionResult MechanicCrewDDBind()
        {

            var fleets = _MechanicCrewService.GetAll().Select(s => new MechanicCrew
            {
                MechanicCrewID = s.MechanicCrewID,
                FullName = s.FullName
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }


    }

}


