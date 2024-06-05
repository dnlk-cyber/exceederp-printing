using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{

    public class FleetReportController : Controller
    {
        //
        // GET: /FleetManagement/FleetReport/

        #region TimeSheet
        public ActionResult WagonDrillTimesheetReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.WagondrillCostsheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult WagoneDrillTimeSheetOnly()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.WagondrillTimesheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        
        public ActionResult MachineTimeSheetReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.MachineryCostsheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();

        }
        public ActionResult MachineTimeSheetOnlyReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.MachineryTimesheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult LightVehicleTimeSheetReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.LightVehicleCostsheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();

        }
        public ActionResult LightVehicleTimeSheetOnlyReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.LightVehicleTimesheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult DryLoadingTimeSheetReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DryLoadingCostsheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult DryLoadingTimeSheetOnly()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DryLoadingTimesheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult DumpTruckTimesheetReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DumpTruckCostsheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult DumpTruckTimeSheetOnlyView()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DumpTruckTimesheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        //-----------------------External-------------------------------//
        public ActionResult WagonDrillExternalTimesheetReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.ExternalWagondrillTimesheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ExternalMachineTimeSheetReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.ExternalMachineryTimesheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult ExternalLightVehicleTimeSheetReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.ExternalLightVehicleTimeSheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult DumpTruckExternalTimeSheetReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.ExternalDumpTruckTimesheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult DryLoadingExternalTimesheetReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.ExternalDryLoadingTimesheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        
        #endregion

        [Authorize]
        public ActionResult RentFromExternal()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.RentFromExternal, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult MachineryAndVehicleStatusReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.MachineryAndVehicleStatus, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult FleetInsuranceReport()
        {
            return View();
        }
        
        [Authorize]
        public ActionResult EquipmentMaintenaceSummary()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentMaintenaceSummary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
         [Authorize]
        public ActionResult PlanPerEquipmentEvaluation()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.PlanPerEquipmentEvaluation, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
         [Authorize]
         public ActionResult PlanVsAccomplishment()
         {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.PlanVsAccomplishment, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
         }
         [Authorize]
         public ActionResult RentTimeSheet()
         {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.RentTimeSheet, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
         [Authorize]
         public ActionResult RentTimeSheetEvaluation()
         {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.RentTimeSheetEvaluation, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
         }
        [Authorize]
        public ActionResult JObRequestStatus()
        {
            return View();
        }
         [Authorize]
        public ActionResult EquipmentPlanEvaluations()
        {
            return View();
        }
        
        [Authorize]
        public ActionResult EquipmentMechanicSummary()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentMechanicSummary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

      

        [Authorize]
        public ActionResult SparePartConsumption()
        {
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

        public ActionResult HeadOfficeSummary()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.HeadOfficeSummary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

        public ActionResult EquipmentSummary()
        {
            return View();
        }



        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

        public ActionResult DailyWorkOrder()
        {
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

        public ActionResult PMActualSchedule()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.PMActualSchedule, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

        public ActionResult EquipmentUtilizationReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentUtilizationReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }


        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

        public ActionResult ProjectSummaryReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.HeadOfficeSummary, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

        public ActionResult ProjectSummaryByTypeReport()
        {
            return View();
        }


        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult FuelConsumptionReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.FuelConsumptionReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult FuelLogReport()
        {
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult FuelReport()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult HeadOfficeReportReport()
        {
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetResourceReport)]
        public ActionResult EquipmentDistrubtionReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentDistrubtionReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult TimeSheetReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.TimeSheetReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }


        public ActionResult EquipentTypeCost()
        {
            return View();
        }

        public ActionResult MaintenaceSummary()
        {
            return View();
        }

        public ActionResult ServiceType()
        {
            return View();
        }

        public ActionResult WorkOrder()
        {
            return View();
        }
        public ActionResult DownTime()
        {
            return View();
        }
        public ActionResult EquipmentTypeCostSummary()
        {
            return View();
        }

        //
        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

        public ActionResult EquipmentReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        //good
        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]
        public ActionResult OperatorReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.OperatorReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }


        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult SalvageStoreReport()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult TireLogReport()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult EquipmentPlanEvaluationReport()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult TyreReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.TyreReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult EquipmentInspectionReport()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]
        public ActionResult OperatorTransferReport()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetResourceReport)]
        public ActionResult EquipmentSiteTransferReport()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentSiteTransferReport, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetResourceReport)]
        public ActionResult EquipmentReturnReport()
        {
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]
        public ActionResult AccidentReport()
        {
            return View();
        }


        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult RentRequestReport()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult InternalRentReport()
        {
            return View();
        }


        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult RentToExternalReport()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult RentFromExternalReport()
        {
            return View();
        }





        //

        [AuthorizeRoles(FleetManagementRoles.FleetMaintenanceReport)]
        public ActionResult JobRequestReport()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult CannibalizationReport()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetMaintenanceReport)]
        public ActionResult JobOrderReport()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetMaintenanceReport)]
        public ActionResult MaintenanceForExternalReport()
        {
            return View();
        }



        public ActionResult PMSchedule()
        {
            return View();
        }

        public ActionResult LogisticPlan()
        {
            return View();
        }

        public ActionResult FleetInsurance()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.FleetInsurance, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }

        public ActionResult FleetAccident()
        {
            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.FleetAccident, out string title, out string number);
            ViewBag.FormTitle = title;
            ViewBag.FormNumber = number;
            return View();
        }
        public ActionResult FleetAccidentNested()
        {
            return View();
        }
    }
}