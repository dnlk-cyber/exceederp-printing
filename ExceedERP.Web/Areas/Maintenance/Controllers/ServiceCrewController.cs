using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    [AuthorizeRoles(MaintenanceRoles.ServiceUser, MaintenanceRoles.MaintenanceWorkShopUser)]
    public class ServiceCrewController : Controller
    {
        private IServiceCrewService _ServiceCrewService;
        private readonly IJobOrderService _JobOrderService;
        private ExceedDbContext db = new ExceedDbContext();
        public ServiceCrewController()
        {
            _ServiceCrewService = new ServiceCrewService();
            _JobOrderService = new JobOrderService();
        }

        //
        // GET: /FleetManagement/ServiceCrew/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult ServiceCrew_Read([DataSourceRequest] DataSourceRequest request)
        {
            var ServiceCrew = _ServiceCrewService.GetAll().Select(i => new ServiceCrew

            {
                ServiceCrewID = i.ServiceCrewID,
                MechanicCrewID = i.MechanicCrewID,
                ActivatedDate = i.ActivatedDate,
                ServiceID = i.ServiceID,

                ServiceName = i.MechanicCrews.FullName,
                DateCreated = i.DateCreated
            }

            );

            return Json(ServiceCrew.ToDataSourceResult(request));
        }

        public ActionResult ServiceCrewByService_Read(string Id, [DataSourceRequest] DataSourceRequest request)
        {
            var ServiceCrew = _ServiceCrewService.GetAll().Where(x => x.ServiceID.ToString() == Id)
                //.DistinctByField(x => x.MechanicCrewID)
                .Select(i => new ServiceCrew

                {
                    ServiceCrewID = i.ServiceCrewID,
                    HourlyWage = i.HourlyWage,
                    TotalLabourHours = i.TotalLabourHours,
                    TotalLabourCost = i.TotalLabourCost,
                    MechanicCrewID = i.MechanicCrewID,
                    ActivatedDate = i.ActivatedDate,
                    ServiceID = i.ServiceID,
                    ServiceName = i.MechanicCrews.FullName,
                    DateCreated = i.DateCreated,
                    MechanicType = i.MechanicType
                }

            );

            return Json(ServiceCrew.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceCrew_Create(string cserviceID, [DataSourceRequest] DataSourceRequest request, ServiceCrew serviceCrew)
        {
            ModelState.Remove("ServiceCrewID");
            ModelState.Remove("MechanicCrewID");
            ModelState.Remove("ServiceID");
            IMechanicCrewService CrewService = new MechanicCrewService();
            
            //Guid IdentityID = CrewService.GetAll().Where(x => x.FullName == serviceCrew.ServiceName).Select(x => x.MechanicCrewID).FirstOrDefault();
            List<ServiceCrew> Services = _ServiceCrewService.GetAll().Where(f => f.MechanicCrewID == serviceCrew.MechanicCrewID && f.ServiceID == Guid.Parse(cserviceID)).ToList();
            if (Services.Any())
            {
               // ModelState.Clear();
                ModelState.AddModelError("ExistingName", "  Crew Already Exists in Service ");
            }
            else
            {
                var selectedCrewSetting = db.MechanicCrew.Find(serviceCrew.MechanicCrewID);
                try
                {
                    var placement = db.Placements.Where(q => q.EmployeeId == selectedCrewSetting.EmployeeId).First();
                
                    serviceCrew.ServiceCrewID = Guid.NewGuid();

                
                if (serviceCrew != null && selectedCrewSetting != null)
                {
                    //serviceCrew.MechanicCrewID = serviceCrew.MechanicCrewID;
                    serviceCrew.IsActive = true;
                    serviceCrew.HourlyWage = placement.Salary * 1/240;
                    
                    serviceCrew.TotalLabourCost = serviceCrew.TotalLabourHours * serviceCrew.HourlyWage;
                    serviceCrew.ServiceID = Guid.Parse(cserviceID);
                    serviceCrew.CreatedBy = User.Identity.Name;
                    serviceCrew.DateCreated = DateTime.Now;

                    

                    _ServiceCrewService.Insert(serviceCrew);
                }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("EmployeeSalary", "  Employee salary not defined ");
                }
            }
            return Json(new[] { serviceCrew }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ExternalServiceCrew_Create(string cserviceID, [DataSourceRequest] DataSourceRequest request, ExternalCrew ServiceCrew)
        {
            ModelState.Remove("ExternalCrewID");
            ModelState.Remove("JobId");
            Guid JobId = Guid.Parse(cserviceID);
            List<ExternalCrew> Services = db.ExternalCrew.Where(f => f.Name == ServiceCrew.Name && f.JobId == JobId).ToList();
            if (Services.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", "  Crew Already Attached To Job ");
            }
            else
            {
                ServiceCrew.ExternalCrewID = Guid.NewGuid();


                if (ServiceCrew != null)
                {


                    ServiceCrew.JobId = Guid.Parse(cserviceID);
                    ServiceCrew.CreatedBy = User.Identity.Name;
                    ServiceCrew.ApprovalStatuses = ApprovalStatuses.Pending;
                    ServiceCrew.DateCreated = DateTime.Now;

                    db.ExternalCrew.Add(ServiceCrew);

                    db.SaveChanges();

                }
            }
            return Json(new[] { ServiceCrew }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceCrew_Update([DataSourceRequest] DataSourceRequest request, ServiceCrew serviceCrew)
        {
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();
            //var results = new List<ServiceCrew>();
            ModelState.Clear();
            var selectedCrewSetting = db.MechanicCrew.Find(serviceCrew.MechanicCrewID);
            var serviceCrewFromContext = _ServiceCrewService.GetById(serviceCrew.ServiceCrewID);
            var placement = db.Placements.Where(q => q.EmployeeId == selectedCrewSetting.EmployeeId).First();
            serviceCrew.ServiceCrewID = Guid.NewGuid();

            if (placement == null)
            {
                ModelState.AddModelError("EmployeeSalary", "  Employee salary not defined ");
            }
                if (serviceCrewFromContext != null && selectedCrewSetting != null)
            {
                serviceCrewFromContext.MechanicCrewID = serviceCrew.MechanicCrewID;

                    serviceCrew.HourlyWage = placement.Salary * 1 / 240;
                    serviceCrew.TotalLabourCost = serviceCrew.TotalLabourHours * serviceCrew.HourlyWage;

                serviceCrewFromContext.LastModified = DateTime.Now;
                serviceCrewFromContext.ModifiedBy = User.Identity.Name;

                _ServiceCrewService.Update(serviceCrewFromContext);
            }
            //if (serviceCrew != null)
            //{
            //    //foreach (var ServiceCrew in ServiceCrews)
            //    //{
            //    //    if (ServiceCrew.IsActive == true)
            //    //    {
            //    //        ServiceCrew.ActivatedDate = DateTime.Now;
            //    //    }
            //    //    else
            //    //    {
            //    //        ServiceCrew.ActivatedDate = null;
            //    //    }
            //    //    _ServiceCrewService.Update(ServiceCrew);

            //    //}
            //}

            return Json(new[] { serviceCrewFromContext }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Crew_Update([DataSourceRequest] DataSourceRequest request, ServiceCrew serviceCrew)
        {
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();


            //if (ServiceCrews != null)
            //{
            //    foreach (var s in ServiceCrews)
            //    {
            //        if (s.IsActive)
            //        {
            //            s.ActivatedDate = DateTime.Now;
            //        }
            //        else
            //        {
            //            s.ActivatedDate = null;
            //        }
            //        _ServiceCrewService.Update(s);

            //    }
            //}

            //var Crews = db.ServiceCrew.Where(x => x.ServiceID == ServiceCrews.FirstOrDefault().ServiceID).ToList();

            return Json(new[] { serviceCrew }.ToDataSourceResult(request));
        }
        public JsonResult ServiceCrewActiveByService(string Id, [DataSourceRequest] DataSourceRequest request)
        {
            FleetMaintenaceHelper FleetMaintenaceHelper = new FleetMaintenaceHelper();

            IServiceCrewService _ServiceCrewService = new ServiceCrewService();
            var Crews = new List<ServiceCrew>();

            var MechanicLogs = db.ServiceCrewWorkLogs.Where(x => x.JobId.ToString() == Id).ToList();
            IMechanicCrewService mechanicCrewService = new MechanicCrewService();



            foreach (var log in MechanicLogs.DistinctByField(x => x.MecanicId))
            {
                var Crew = new ServiceCrew();
                var Mechanic = mechanicCrewService.GetById(log.MecanicId);

                if (Mechanic != null)
                {
                    var MechanicLog = MechanicLogs.Where(x => x.MecanicId == log.MecanicId).ToList();

                    Crew.Type = Mechanic.Postion;
                    Crew.CreatedBy = Mechanic.FullName;
                    Crew.ServiceName = Mechanic.FullName;
                    //Crew.TotalLabourCost = decimal.Parse(FleetMaintenaceHelper.GetMechanicDuration(MechanicLog).ToString()) * Mechanic.HourlyWage;
                    //Crew.HourlyWage = FleetMaintenaceHelper.GetMechanicDuration(MechanicLog);
                    Crews.Add(Crew);
                }
            }

            return Json(Crews.Select(s => new
            {
                ServiceCrewID = s.ServiceCrewID,
                CreatedBy = s.CreatedBy,
                ServiceName = s.ServiceName,
                Type = s.Type,
                IsActive = s.IsActive,
                ActivatedDate = s.ActivatedDate,
                HourlyWage = s.HourlyWage,
                LabourCost = s.TotalLabourCost.ToString()

            }).ToDataSourceResult(request));

        }

        public JsonResult ServiceCrewByService(string Id, [DataSourceRequest] DataSourceRequest request)
        {
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();
            var Crews = new List<ServiceCrew>();
            var JobOrder = db.JobOrder.Where(x => x.JobOrderID.ToString() == Id).FirstOrDefault();

            var Request = db.JobRequest.Where(x => x.Num == JobOrder.Referance).FirstOrDefault();

            var Services = db.JobOrderDetail.Where(x => x.JobOrderID == JobOrder.JobOrderID || x.JobOrderID == Request.JobRequestId).ToList();
            foreach (var Service in Services)
            {

                //var ServiceCrew = db.MechanicCrew.Where(x => x.SectionId  == Service.Service.SectionId ).Select(s => new MechanicCrewViewModel
                //{ 
                //MechanicCrewID = s.MechanicCrewID,
                //ServiceName = s.FullName,
                //ServiceID = Service.ServiceID,
                //}).ToList();

                //Crews.AddRange(ServiceCrew);
                var ServiceCrew = _ServiceCrewService.GetAll().Where(x => x.ServiceID == Service.ServiceID).ToList();

                Crews.AddRange(ServiceCrew);

                //  var Job = db.JobOrder.Where(x => x.JobOrderID == JobOrder.JobOrderID).FirstOrDefault();

                //if (Job == null)
                //{
                //    var Request = db.JobRequest.Where(x => x.JobRequestId == JobOrder.JobOrderID).FirstOrDefault();
                //    Job = db.JobOrder.Where(x => x.Referance == Request.Num).FirstOrDefault();
                //}
                //   foreach (var Crew in Crews)
                //{
                //    if (Crew.IsActive)
                //    {
                //        if (Job.IsOnlineTransferred)
                //        {
                //            var Mechanic = db.MechanicCrew.Where(x => x.MechanicCrewID == Crew.MechanicCrewID).FirstOrDefault();
                //            var Duration = (DateTime.Parse(Job.OnlineTransferredTime.ToString()) - DateTime.Parse(Crew.ActivatedDate.ToString())).TotalHours;

                //            if (Mechanic != null)
                //            {
                //                Crew.Type = Mechanic.Postion;
                //            }
                //            Crew.LabourCost = decimal.Parse(Duration.ToString()) * Crew.HourlyWage;
                //        }
                //        else
                //        {
                //            var Duration = (DateTime.Parse(DateTime.Now.ToString()) - DateTime.Parse(Crew.ActivatedDate.ToString())).TotalHours;
                //            Crew.LabourCost = decimal.Parse(Duration.ToString()) * Crew.HourlyWage;

                //        }
                //    }
                //    else
                //    {
                //        Crew.LabourCost = 0;
                //    }
                //}
            }

            return Json(Crews.Select(s => new
            {
                MechanicCrewID = s.MechanicCrewID,
                ServiceCrewID = s.ServiceCrewID,
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName,
                Type = s.Type,
                IsActive = s.IsActive,
                ActivatedDate = s.ActivatedDate,
                HourlyWage = s.HourlyWage,
                //     LabourCost  = decimal.ToInt64(decimal.Parse( s.LabourCost.ToString()) ) 

            }).ToDataSourceResult(request));

        }


        public JsonResult GetServiceCrewByService(string Id, [DataSourceRequest] DataSourceRequest request)
        {
            IServiceCrewService _ServiceCrewService = new ServiceCrewService();


            var Service = db.JobOrderDetail.Where(x => x.JobOrderDetailID.ToString() == Id).FirstOrDefault();

            var Crews = _ServiceCrewService.GetAll().Where(x => x.ServiceID.ToString() == Id).ToList();

            var Job = db.JobOrder.Where(x => x.JobOrderID == Service.JobOrderID).FirstOrDefault();

            foreach (var Crew in Crews)
            {
                if (Crew.IsActive)
                {
                    if (Job.IsOnlineTransferred)
                    {
                        var Duration = (DateTime.Parse(Job.OnlineTransferredTime.ToString()) - DateTime.Parse(Crew.ActivatedDate.ToString())).TotalHours;
                        Crew.TotalLabourCost = decimal.Parse(Duration.ToString()) * Crew.HourlyWage;
                    }
                    else
                    {
                        var Duration = (DateTime.Parse(DateTime.Now.ToString()) - DateTime.Parse(Crew.ActivatedDate.ToString())).TotalHours;
                        Crew.TotalLabourCost = decimal.Parse(Duration.ToString()) * Crew.HourlyWage;

                    }
                }
                else
                {
                    Crew.TotalLabourCost = 0;
                }
            }

            return Json(Crews.Select(s => new
            {
                MechanicCrewID = s.MechanicCrewID,
                ServiceCrewID = s.ServiceCrewID,
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName,
                IsActive = s.IsActive,
                ActivatedDate = s.ActivatedDate,
                HourlyWage = s.HourlyWage,
                LabourCost = decimal.Round(decimal.Parse(s.TotalLabourCost.ToString()), 2)

            }).ToList(), JsonRequestBehavior.AllowGet);

        }

        public JsonResult ExternalServiceCrewByService(string ErserviceID, [DataSourceRequest] DataSourceRequest request)
        {
            _ServiceCrewService = new ServiceCrewService();
            Guid Id = Guid.Parse(ErserviceID.ToString());
            return Json(db.ExternalCrew.Where(s => s.JobId == Id).Select(f => new { ExternalCrewID = f.ExternalCrewID, JobId = f.JobId, Name = f.Name, ApprovalStatuses = f.ApprovalStatuses, DateCreated = f.DateCreated, ApprovalStatus = f.ApprovalStatuses }).ToDataSourceResult(request));
        }

        public JsonResult ExternalServiceCrewByServiceRaw(string SId)
        {
            _ServiceCrewService = new ServiceCrewService();
            List<ServiceCrew> Crews = new List<ServiceCrew>();
            var Services = db.JobOrderDetail.Where(x => x.JobOrderID.ToString() == SId).ToList();
            foreach (var Service in Services)
            {

                //    var Crew =   _ServiceCrewService.GetAll().Where(s => s.ServiceID.ToString() == SId).Select(f => new ServiceCrew { ServiceID = f.ServiceID,  = f.JobId, Name = f.Name, ApprovalStatuses = f.ApprovalStatuses, DateCreated = f.DateCreated, ApprovalStatus = f.ApprovalStatuses });
            }
            return Json(string.Empty);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServiceCrew_Destroy([DataSourceRequest] DataSourceRequest request, ServiceCrew serviceCrew)
        {
            var ServiceCrew = _ServiceCrewService.GetById(serviceCrew.ServiceCrewID);
            if (ServiceCrew != null)
            {
                _ServiceCrewService.Delete(ServiceCrew);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete Service Crew number " + serviceCrew.ServiceCrewID);

            }

            return Json(new[] { ServiceCrew }.ToDataSourceResult(request));
        }


        //[AuthorizeRoles(MaintenanceRoles.MaintenanceServiceApprover)]
        public ActionResult AssignServiceCrew(Crews model)
        {
            try
            {
                _ServiceCrewService = new ServiceCrewService();
                if (model.ServiceCrews.Any())
                {
                    foreach (var m in model.ServiceCrews)
                    {
                        var Crews = db.ServiceCrew.Where(x => x.ServiceCrewID == m.ServiceCrewId).FirstOrDefault();

                        if (Crews != null)
                        {
                            Crews.ServiceCrewID = m.ServiceCrewId;

                            if (m.IsActive == "true")
                            {



                                Crews.IsActive = true;
                                Crews.ActivatedDate = DateTime.Now;

                                _ServiceCrewService = new ServiceCrewService();

                                db.ServiceCrew.Attach(Crews);
                                db.Entry(Crews).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();


                            }
                            if (m.IsActive == "false")
                            {

                                Crews.IsActive = false;
                                Crews.LastModified = DateTime.Now;

                                _ServiceCrewService = new ServiceCrewService();

                                db.ServiceCrew.Attach(Crews);
                                db.Entry(Crews).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();


                            }



                        }


                    }

                }


            }
            catch (Exception e)
            {
                return Json(e.Message);  //  string ee = e.Message;
            }

            return Json(model);
        }



    }
}
