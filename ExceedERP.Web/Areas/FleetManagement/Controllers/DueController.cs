using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Web.Areas.FleetManagement.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Hubs;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
         [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestApprover, FleetManagementRoles.MaintenanceRequestor, FleetManagementRoles.MaintenanceOrderUser)] 
    public class DueController : Controller
    {
        IJobOrderDetailService _JobOrderDetailService;
        IJobOrderService _JobOrderService;
        IFleetService _FleetService;
        //
        // GET: /FleetManagement/Due/
        public DueController()
        {
            _JobOrderDetailService = new JobOrderDetailService();
            _JobOrderService = new JobOrderService();
            _FleetService = new FleetService();
        }
        public ActionResult Index()
        {
            return View();
        }


             [OutputCache(Duration = 100000)]
        public ActionResult Due_Read([DataSourceRequest] DataSourceRequest request)
        {
            var equipmentDue = GetMaintainaceDue();

            return Json(equipmentDue.ToDataSourceResult(request));
        }

        public ActionResult DueService_Read([DataSourceRequest] DataSourceRequest request, string fleetID)
        {
            Guid fleetsID = Guid.Parse(fleetID);
            var serviceDue = GetOverDueService(fleetsID).Select(s => new  
            {
                ApprovedBy = s.ApprovedBy,
                DateCreated = s.DateCreated,
                DateInterval = s.DateInterval,
                DateIntervalType = s.DateIntervalType,
                IsPm = s.IsPm,
                LabourCost = s.LabourCost,

                MileageInterval = s.MileageInterval,
                PartCost = s.STDTime,
                PlannerID = s.PlannerID,
                Priority = s.Priority,
                ServiceID = s.ServiceID,
                ServiceName = s.ServiceName,
                ServiceType = s.ServiceType,

                Type = s.Type

            });

            return Json(serviceDue.ToDataSourceResult(request));
        }
        public ActionResult Due_Destroy([DataSourceRequest] DataSourceRequest request, MaintainceDueViewModel dues)
        {
            _JobOrderDetailService = new JobOrderDetailService();
            _JobOrderService = new JobOrderService();
            var fleet = _FleetService.GetById(dues.AssetId);
            List<Service> services = GetOverDueService(dues.AssetId);
            if (services.Any())
            {
       
                  
               
                Guid JobID = Guid.NewGuid();
                JobOrder jobOrder = new JobOrder();
                jobOrder.JobOrderID = JobID;
                jobOrder.FleetsID = dues.AssetId;
                jobOrder.MaintenanceCenter = "";
                jobOrder.DateIssued = DateTime.Now;
                jobOrder.Type = "Preventative Maintenance";
                jobOrder.ProjectName = fleet.Location;
                jobOrder.OperatorName = fleet.OperatorName;
                jobOrder.InspectorComments = "Automatic Planned Maintenance";
                   jobOrder.Status = MaintenanceConst.Pending;
                _JobOrderService.Insert(jobOrder);
                foreach (var service in services)
                {
                    JobOrderDetail jbd = new JobOrderDetail();
                    jbd.JobOrderDetailID = Guid.NewGuid();
                    jbd.JobOrderID = JobID;
                    jbd.ServiceID = service.ServiceID;
                    jbd.ServiceName = service.ServiceName;
                    jbd.Status = MaintenanceConst.Pending;
                    _JobOrderDetailService.Insert(jbd);
                }
              
                fleet.Status = MaintenanceConst.OnMaintenance;
                _FleetService.Update(fleet);
            }
            var equipmentDue = GetMaintainaceDue();
            return Json(new[] { dues }.ToDataSourceResult(request));
        }

       [AuthorizeRoles(FleetManagementRoles.MaintenanceRequestApprover, FleetManagementRoles.MaintenanceServiceApprover)] 
 
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Dispatch_Due([DataSourceRequest] DataSourceRequest request, string fleetID)
        {
            List<Service> services = GetOverDueService(Guid.Parse(fleetID));
            if (services.Any())
            {
                int jobOrderNum = 1;
                JobOrder jb = _JobOrderService.GetAll().OrderByDescending(x => x.JobOrderID).FirstOrDefault();
                if (jb == null)
                {

                }
                else
                {
                    jobOrderNum = int.Parse(jb.JobOrderNum.ToString()) + 1;
                }
                  
                Guid JobID = Guid.NewGuid();
                JobOrder jobOrder = new JobOrder();
                jobOrder.JobOrderID = JobID;
                jobOrder.MaintenanceCenter = "Unknown";
                jobOrder.FleetsID = Guid.Parse(fleetID);
                jobOrder.ProjectName = "Preventative Maintenance";
                jobOrder.ProjectName = "Automatic Planned Maintenance";
                jobOrder.JobOrderNum = jobOrderNum;
                _JobOrderService.Insert(jobOrder);
                foreach (var service in services)
                {
                    JobOrderDetail jbd = new JobOrderDetail();
                    jbd.JobOrderDetailID = Guid.NewGuid();
                    jbd.JobOrderID = JobID;
                    jbd.ServiceID = service.ServiceID;
                    _JobOrderDetailService.Insert(jbd);
                }
            }
            var equipmentDue = GetMaintainaceDue();

            return Json(equipmentDue.ToDataSourceResult(request));
        }





        #region Support Method
        public List<MaintainceDueViewModel> GetMaintainaceDue()
        {
              IFleetService _FleetService = new FleetService();
                IServiceService _ServiceService = new ServiceService();
                IPlannerService _PlannerService = new PlannerService();
                List<MaintainceDueViewModel> FinalAssetsDues = new List<MaintainceDueViewModel>();
                Guid AssetsId = Guid.Empty;
                int LabourCost = 0;
                int PartCost = 0;
                String PartUsed = null;
                String PlateNum = null;
                String ServiceName = null;
                String LastMaintained = null;
                bool IsDue = false;
                Guid AssetID = Guid.Empty;
                List<Fleets> AssetsDue = _FleetService.GetAll().Where(x => x.Status != MaintenanceConst.OnMaintenance).ToList();
                String AssetName = "";

                int CountDue = 0;
                String IsMileageOrDate = null;
                try
                {
            
                foreach (var AssetsDues in AssetsDue)
                {
                    LabourCost = 0;
                    PartCost = 0;
                    PartUsed = null;
                    PlateNum = null;
                    ServiceName = null;
                    LastMaintained = null;
                    IsDue = false;
                    AssetID = Guid.Empty;
                    AssetName = AssetsDues.EquipmentName + "--" + AssetsDues.PlateNumber;
                    List<Service> AssetsDuess = _ServiceService.GetAll().Where(x => x.PlannerID == AssetsDues.PlannerID).ToList();// MDueMgt.getMaintenaceDueInforamation().Where(x => x.Status != "OnMaintenance" && x.AssetId == AssetsDues).ToList();
                    int TotDues = AssetsDuess.Count;

                    foreach (var Services in AssetsDuess)
                    {
                        CountDue = CountDue + 1;
                        IsDue = false;
                        IsMileageOrDate = null;

                        Service PMM = _ServiceService.GetAll().Where(x => x.ServiceID == Services.ServiceID).FirstOrDefault();

                        if (PMM.IsPm == MaintenanceConst.Planned)
                        {
                            if (PMM.Planner.TrackByDate)
                            {

                                if (Services.DateIntervalType == MaintenanceConst.Day)
                                {

                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddDays(Double.Parse(Services.DateInterval.ToString()));
                                    if (DateTime.Now > newDay)
                                    {
                                        IsDue = true;
                                        IsMileageOrDate = IsMileageOrDate + " By Date";
                                    }

                                }
                                if (Services.DateIntervalType == MaintenanceConst.Week)
                                {

                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddDays(Double.Parse(Services.DateInterval.ToString()) * 7);
                                    if (DateTime.Now > newDay)
                                    {
                                        IsDue = true;
                                        IsMileageOrDate = IsMileageOrDate + " By Date";
                                    }

                                }
                                else if (Services.DateIntervalType == MaintenanceConst.Month)
                                {
                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddMonths(int.Parse(Services.DateInterval.ToString()));
                                    if (DateTime.Now > newDay)
                                    {
                                        IsDue = true;
                                        IsMileageOrDate = IsMileageOrDate + " By Date";

                                    }

                                }
                                else if (Services.DateIntervalType == MaintenanceConst.Year)
                                {
                                    DateTime newDay = DateTime.Parse(AssetsDues.LastMaintained.ToString()).AddYears(int.Parse(Services.DateInterval.ToString()));
                                    if (DateTime.Now > newDay)
                                    {
                                        IsDue = true;
                                        IsMileageOrDate = IsMileageOrDate + " By Date";

                                    }

                                }
                            }

                            if (PMM.Planner.TrackByMileage)
                            {

                                decimal OdoMeterMeasure =  AssetsDues.CurentOdoMeter -  AssetsDues.PreviousOdoMeter;
                                if (OdoMeterMeasure > Services.MileageInterval)
                                {
                                    IsDue = true;
                                    IsMileageOrDate = IsMileageOrDate + " By Mileage";


                                }

                            }
                        }

                        if (IsDue)
                        {
                            AssetID = AssetsDues.FleetsID;
                            LabourCost = 0 ; //+ int.Parse(Services.PartCost.ToString());
                            PlateNum = AssetsDues.PlateNumber;
                            ServiceName = " " + ServiceName + "   " + Services.ServiceName;
                            LastMaintained = AssetsDues.LastMaintained.ToString();
                            // PartUsed = "  " + PartUsed + "  " + Services.PartUsed;
                            PartCost = 0;// PartCost + int.Parse(Services.PartCost.ToString());
                            Notify.NotifyUser(new NotifModels { UserId = User.Identity.Name, DeadLine = DateTime.Now, Message = "Equipment with Plate No " + AssetsDues.PlateNumber + " For Service " + Services.ServiceName + " Approved ", Title = " Equipment Due for " + AssetsDues.UnitNum, URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });


                        }

                    }
                    if (AssetID == AssetsDues.FleetsID && IsDue)
                    {
                      

                        FinalAssetsDues.Add(new MaintainceDueViewModel(Guid.NewGuid(), AssetID, PlateNum, LastMaintained, ServiceName, LabourCost, PartCost, PartUsed, AssetName, "OVER " + IsMileageOrDate));
                    }

                }
            }
            catch(Exception e)
            {
                string ee = e.Message;
            }
            return FinalAssetsDues;
        }
        private List<Service> GetOverDueService(Guid fleetId)
        {
            IFleetService _FleetService = new FleetService();
            IServiceService _ServiceService = new ServiceService();
            IPlannerService _PlannerService = new PlannerService();
            List<Service> DueService = new List<Service>();
            Guid AssetsId = Guid.Empty;
            int LabourCost = 0;
            int PartCost = 0;
            String PartUsed = null;
            String PlateNum = null;
            String ServiceName = null;
            String LastMaintained = null;
            bool IsDue = false;
            Guid AssetID = Guid.Empty;
            Fleets AssetsDue = _FleetService.GetAll().Where(x => x.FleetsID == fleetId).FirstOrDefault();


            int CountDue = 0;
            String IsMileageOrDate = null;

            LabourCost = 0;
            PartCost = 0;
            PartUsed = null;
            PlateNum = null;
            ServiceName = null;
            LastMaintained = null;
            IsDue = false;
            AssetID = Guid.Empty;
            List<Service> AssetsDuess = _ServiceService.GetAll().Where(x => x.PlannerID == AssetsDue.PlannerID).ToList();// MDueMgt.getMaintenaceDueInforamation().Where(x => x.Status != "OnMaintenance" && x.AssetId == AssetsDues).ToList();
            int TotDues = AssetsDuess.Count;
            foreach (var Services in AssetsDuess)
            {
                CountDue = CountDue + 1;
                IsDue = false;
                IsMileageOrDate = null;

                Service PMM = _ServiceService.GetAll().Where(x => x.ServiceID == Services.ServiceID).FirstOrDefault();

                if (PMM.IsPm == MaintenanceConst.Planned)
                {
                    if (PMM.Planner.TrackByDate)
                    {
                        if (Services.DateIntervalType == MaintenanceConst.Day)
                        {

                            DateTime newDay = DateTime.Parse(AssetsDue.LastMaintained.ToString()).AddHours(Double.Parse(Services.DateInterval.ToString()));
                            if (DateTime.Now > newDay)
                            {
                                IsDue = true;
                                IsMileageOrDate = " By Date";
                            }

                        }

                        if (Services.DateIntervalType == MaintenanceConst.Week)
                        {

                            DateTime newDay = DateTime.Parse(AssetsDue.LastMaintained.ToString()).AddDays(Double.Parse(Services.DateInterval.ToString()) * 7);
                            if (DateTime.Now > newDay)
                            {
                                IsDue = true;
                                IsMileageOrDate = IsMileageOrDate + " By Date";
                            }

                        }
                        else if (Services.DateIntervalType == MaintenanceConst.Month)
                        {
                            DateTime newDay = DateTime.Parse(AssetsDue.LastMaintained.ToString()).AddMonths(int.Parse(Services.DateInterval.ToString()));
                            if (DateTime.Now > newDay)
                            {
                                IsDue = true;
                                IsMileageOrDate = IsMileageOrDate + " By Date";

                            }

                        }
                        else if (Services.DateIntervalType == MaintenanceConst.Year)
                        {
                            DateTime newDay = DateTime.Parse(AssetsDue.LastMaintained.ToString()).AddYears(int.Parse(Services.DateInterval.ToString()));
                            if (DateTime.Now > newDay)
                            {
                                IsDue = true;
                                IsMileageOrDate = IsMileageOrDate + " By Date";

                            }

                        }
                    }

                    if (PMM.Planner.TrackByMileage)
                    {

                        int OdoMeterMeasure = int.Parse(AssetsDue.CurentOdoMeter.ToString()) - int.Parse(AssetsDue.PreviousOdoMeter.ToString());
                        if (OdoMeterMeasure > Services.MileageInterval)
                        {
                            IsDue = true;
                            IsMileageOrDate = IsMileageOrDate + " By Mileage";


                        }

                    }
                }
                if (IsDue)
                {
                    DueService.Add(PMM);
                }

            }

            return DueService;
        }
        #endregion
    }
}
