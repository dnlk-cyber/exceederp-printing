using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    //[AuthorizeRoles(MaintenanceRoles.MaintenanceAdmin)]
    public class MechanicCrewController : Controller
    {
        private IMechanicCrewService _MechanicCrewService;
        private readonly IFleetService _FleetService;
        private ExceedDbContext db = new ExceedDbContext();
        public MechanicCrewController()
        {
            _MechanicCrewService = new MechanicCrewService();
            _FleetService = new FleetService();
        }
        public ActionResult Index()
        {
            ViewData["Sections"] = db.WorkUnitSection.ToList();
            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.EmployeeCode + " "+i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            ViewData["MechanicType"] = db.MechanicTypeClasses.Select(s => new
            {
                Value = s.MechanicTypeClassId,
                Text = s.MechanicTypeTitle
            });
            
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


            //Placement Placements = db.Placements.Where(x => x.EmployeeId == MechanicCrew.EmployeeId).FirstOrDefault();
            //if (Placements != null)
            //{
            //    Person_Employee Person_Employee = db.Person_Employee.Where(x => x.EmployeeId == Placements.ImmediateSupervisor).FirstOrDefault();
            //    if (Person_Employee != null)
            //    {
            //        MechanicCrew.SupervisorName = Person_Employee.FirstName + " " + Person_Employee.MiddleName + " " + Person_Employee.LastName;
            //    }
            //}

            List<MechanicCrew> Mechanics = _MechanicCrewService.GetAll().Where(f => f.EmployeeId == MechanicCrew.EmployeeId).ToList();
            if (Mechanics.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Mechanic with Name Already Exists ");
            }
            else
            {
                //var Employee = db.Person_Employee.Where(x => x.EmployeeId == MechanicCrew.EmployeeId).FirstOrDefault();

                //if (Employee != null)
                //{
                try
                {
                    var placement = db.Placements.Where(q => q.EmployeeId == MechanicCrew.EmployeeId).First();
                    MechanicCrew.MechanicCrewID = Guid.NewGuid();
                    //MechanicCrew.FullName = Employee.FirstName + " " + Employee.MiddleName + " " + Employee.LastName;

                    MechanicCrew.EmployeeId = MechanicCrew.EmployeeId;
                    MechanicCrew.HourlyWage = placement.Salary/240;
                    MechanicCrew.CreatedBy = User.Identity.Name;
                    MechanicCrew.DateCreated = DateTime.Now;

                    _MechanicCrewService.Insert(MechanicCrew);
                }
                catch (Exception e)
            {
                ModelState.AddModelError("EmployeeSalary", "  Employee salary not defined ");
            }
           

                //}
            }

            return Json(new[] { MechanicCrew }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MechanicCrew_Update([DataSourceRequest] DataSourceRequest request, MechanicCrew MechanicCrew)
        {

            if (MechanicCrew != null)
            {
                //var Employee = db.Person_Employee.Where(x => x.EmployeeId == MechanicCrew.EmployeeId).FirstOrDefault();

                //Placement Placements = db.Placements.Where(x => x.EmployeeId == MechanicCrew.EmployeeId).FirstOrDefault();
                //if (Placements != null)
                //{
                //    Person_Employee Person_Employee = db.Person_Employee.Where(x => x.EmployeeId == Placements.ImmediateSupervisor).FirstOrDefault();
                //    if (Person_Employee != null)
                //    {
                //        MechanicCrew.SupervisorName = Person_Employee.FirstName + " " + Person_Employee.MiddleName + " " + Person_Employee.LastName;
                //    }
                //}

                //if (Employee != null)
                //{
                    //MechanicCrew.FullName = Employee.FirstName + " " + Employee.MiddleName + " " + Employee.LastName;
                    MechanicCrew.ModifiedBy = User.Identity.Name;
                    MechanicCrew.LastModified = DateTime.Now;

                    _MechanicCrewService.Update(MechanicCrew);
                //}
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
        public ActionResult MechanicCrewDDBind(Guid? jobOrderId)
        {
            var mechanicCrews = db.MechanicCrew.Select(s => new 
            {
                MechanicCrewID = s.MechanicCrewID,
                FullName = s.Person_Employee.FirstName + " " + s.Person_Employee.MiddleName + " " + s.Person_Employee.LastName
            }).ToList();
            if(jobOrderId!=null)
            {
                var mechanicIds = db.ServiceCrew.Where(q => q.ServiceID == jobOrderId).Select(s => s.MechanicCrewID);
                mechanicCrews = mechanicCrews.Where(q => mechanicIds.Contains(q.MechanicCrewID)).ToList();            }
            return Json(mechanicCrews, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RefreshMechanic()
        {
            var count = 0;
            var mapping = db.HRFleetMapping.Where(x => x.PlacmentType == FleetPlacmentType.MECHANIC).ToList();
            foreach (var map in mapping)
            {
                var placements = db.Placements
                    .Where(x => x.isCurrent)
                    .Where(x => x.LookupId == map.JobTitleCode).ToList();
                foreach (var placement in placements)
                {
                    var Employee = db.Person_Employee.Find(placement.EmployeeId);
                    var exist = db.MechanicCrew.Where(x => x.EmployeeId == placement.EmployeeId).FirstOrDefault();
                    if (exist == null)
                    {
                        var entity = new MechanicCrew
                        {
                            MechanicCrewID= Guid.NewGuid(),
                            EmployeeId = placement.EmployeeId,
                            SectionId=Guid.NewGuid(),
                            MechanicType= MechanicType.Mechanical,
                            FullName=Employee.FullNameaEnglish,
                            SupervisorName= "Supervisor Name",
                            HourlyWage=placement.Salary/240,
                            Postionenum = PostionenumCrew.Assistant,
                            Postion= "Postion",
                            DateHired=DateTime.Now,
                            OrgBranchName= "OrgBranch Name",
                            CreatedBy= "Created By",
                            ModifiedBy= "Modified By"
                        };
                        db.MechanicCrew.Add(entity);
                        db.SaveChanges();
                        count++;
                    }
                }
            }
            //return Json(mapping, JsonRequestBehavior.AllowGet);
            return Json(count+" items added");
        }



    }

}


