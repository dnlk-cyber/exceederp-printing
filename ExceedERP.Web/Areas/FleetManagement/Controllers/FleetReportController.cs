//using ExceedERP.Core.Domain.FleetManagement;
//using ExceedERP.Core.Domain.Global;
//using ExceedERP.DataAccess.Context;
//using ExceedERP.Helpers.Common;
//using ExceedERP.Web.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;     

//namespace ExceedERP.Web.Areas.FleetManagement.Controllers
//{

//    public class FleetReportController : Controller
//    {
//        //
//        // GET: /FleetManagement/FleetReport/
//        ExceedDbContext db = new ExceedDbContext();

//        #region TimeSheet
//        public ActionResult FleetOID()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.FleetOIDStatus, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        public ActionResult WagonDrillTimesheetReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.WagondrillCostsheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            bool IsMCG = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            if (fetchSetting != null && fetchSetting.Remark == "MCG")
//            {
//                IsMCG = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            ViewBag.IsMCG = IsMCG;
//            return View();
//        }
        
//        public ActionResult WagonDrillTimesheetReportSummary()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.WagondrillCostsheetSummary, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            bool IsMCG = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            if (fetchSetting != null && fetchSetting.Remark == "MCG")
//            {
//                IsMCG = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            ViewBag.IsMCG = IsMCG;
//            return View();
//        }
//        public ActionResult DumpTruckTimesheetDailyEvaluation()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DumpTruckTimesheetDailyEvaluation, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        public ActionResult MachineTimeSheetDailyEvaluation()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.MachineTimeSheetDailyEvaluation, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        public ActionResult WagoneDrillTimeSheetOnly()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.WagondrillTimesheet, out string title, out string number,out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            ViewBag.IsMCG = db.RequestToInspections.FirstOrDefault()?.Remark=="MCG";
//            return View();
//        }
//        //stop
//        public ActionResult MachineTimeSheetReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.MachineryCostsheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            return View();

//        }
//        public ActionResult MachineTimeSheetReportSummary()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.MachineryCostsheetSummary, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            bool IsMCG = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            if (fetchSetting != null && fetchSetting.Remark == "MCG")
//            {
//                IsMCG = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            ViewBag.IsMCG = IsMCG;
//            return View();

//        }
//        public ActionResult ProformaReport()
//        {
//            return View();
//        }
//        public ActionResult MachineTimeSheetOnlyReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.MachineryTimesheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            ViewBag.IsMCG = db.RequestToInspections.FirstOrDefault()?.Remark == "MCG";
//            return View();
//        }
//        public ActionResult LightVehicleTimeSheetReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.LightVehicleCostsheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            bool IsMCG = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            if (fetchSetting != null && fetchSetting.Remark == "MCG")
//            {
//                IsMCG = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            ViewBag.IsMCG = IsMCG;
//            return View();

//        }
//        public ActionResult LightVehicleTimeSheetReportSummary()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.LightVehicleCostsheetSummary, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            bool IsMCG = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            if (fetchSetting != null && fetchSetting.Remark == "MCG")
//            {
//                IsMCG = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            ViewBag.IsMCG = IsMCG;
//            return View();

//        }
//        //stop
//        public ActionResult LightVehicleTimeSheetOnlyReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.LightVehicleTimesheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;

//            bool ISecwc = false;
//            bool IsMcg = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC") 
//            {
//                ISecwc = true;
//            }
//            if (fetchSetting != null && fetchSetting.Remark == "MCG") 
//            {
//                IsMcg = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            ViewBag.IsMCG = IsMcg;
//            return View();
//        }
//        public ActionResult DryLoadingTimeSheetReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DryLoadingCostsheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            return View();
//        }
//        public ActionResult DryLoadingTimeSheetReportSummary()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DryLoadingCostsheetSummary, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            return View();
//        }
//        public ActionResult DryLoadingTimeSheetOnly()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DryLoadingTimesheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        public ActionResult DumpTruckTimesheetReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DumpTruckCostsheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            bool IsMCG = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            if (fetchSetting != null && fetchSetting.Remark == "MCG")
//            {
//                IsMCG = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            ViewBag.IsMCG = IsMCG;
//            return View();
//        }


//        //stop
//        public ActionResult DumpTruckTimesheetReportSummary()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DumpTruckCostsheetSummary, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            bool IsMCG = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            if (fetchSetting != null && fetchSetting.Remark == "MCG")
//            {
//                IsMCG = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            ViewBag.IsMCG = IsMCG;
//            return View();
//        }
//        public ActionResult DumpTruckTimeSheetOnlyView()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DumpTruckTimesheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        public ActionResult DumpTruckTimesheetMCGReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DumpTruckTimesheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            bool IsMCG = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            if (fetchSetting != null && fetchSetting.Remark == "MCG")
//            {
//                IsMCG = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            ViewBag.IsMCG = IsMCG;
//            return View();
//        }

//        //-----------------------External-------------------------------//
//        public ActionResult WagonDrillExternalTimesheetReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.ExternalWagondrillTimesheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            bool IsMCG = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            if (fetchSetting != null && fetchSetting.Remark == "MCG")
//            {
//                IsMCG = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            ViewBag.IsMCG = IsMCG;
//            return View();
//        }
        
//        public ActionResult ExternalMachineTimeSheetReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.ExternalMachineryTimesheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            return View();
//        }
//        public ActionResult ExternalLightVehicleTimeSheetReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.ExternalLightVehicleTimeSheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            return View();
//        }
//        //stop
//        public ActionResult DumpTruckExternalTimeSheetReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.ExternalDumpTruckTimesheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            return View();
//        }
//        public ActionResult DryLoadingExternalTimesheetReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.ExternalDryLoadingTimesheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            bool ISecwc = false;
//            var fetchSetting = db.RequestToInspections.FirstOrDefault();
//            if (fetchSetting != null && fetchSetting.Remark == "ECWC")
//            {
//                ISecwc = true;
//            }
//            ViewBag.IsECWC = ISecwc;
//            return View();
//        }
        
//        #endregion

//        [Authorize]
//        public ActionResult RentFromExternal()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.RentFromExternal, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        public ActionResult MachineryAndVehicleStatusReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.MachineryAndVehicleStatus, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        public ActionResult FleetInsuranceReport()
//        {
//            return View();
//        }
        
//        [Authorize]
//        public ActionResult EquipmentMaintenaceSummary()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentMaintenaceSummary, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        //stop
//         [Authorize]
//        public ActionResult PlanPerEquipmentEvaluation()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.PlanPerEquipmentEvaluation, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//         [Authorize]
//         public ActionResult PlanVsAccomplishment()
//         {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.PlanVsAccomplishment, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//         }
//         [Authorize]
//         public ActionResult RentTimeSheet()
//         {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.RentTimeSheet, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//         [Authorize]
//         public ActionResult RentTimeSheetEvaluation()
//         {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.RentTimeSheetEvaluation, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//         }
//        [Authorize]
//        public ActionResult JObRequestStatus()
//        {
//            return View();
//        }
//         [Authorize]
//        public ActionResult EquipmentPlanEvaluations()
//        {
//            return View();
//        }
        
//        [Authorize]
//        public ActionResult EquipmentMechanicSummary()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentMechanicSummary, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }

//        [Authorize]
//      public ActionResult LightVehicleAvailabilityDesign()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.LightVehicleAvailabilityReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//      public ActionResult SparePartConsumptionDesign()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.SparePartConsumption, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }[Authorize]
//      public ActionResult FuelMConsumption()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.FuelConsumptionReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }[Authorize]
//      public ActionResult OilLubConsumptionDesign()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.OilLubricantConsumption, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//      public ActionResult EquipmentPerformance()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentPerformanceDesign, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        public ActionResult EquipmentFuelConsumptionDesign()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentFuelConsumptionDesign, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//      public ActionResult TimeSheetProfitLoose()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.TimeSheetProfitLoose, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//      public ActionResult TimeSheetOwnRent()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.TimeSheetOwnRent, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//      public ActionResult TimeSheetAvailability()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.TimeSheetAvailability, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//      public ActionResult TimeSheetAvailabilityByCategory()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.TimeSheetAvailabilityByCategory, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//      public ActionResult TimeSheetAvailabilityByBranch()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.TimeSheetAvailabilityByBranch, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        public ActionResult AvailabilityBasedEquipmentStatus()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.AvailabilityBasedEquipmentStatus, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//      public ActionResult MachineryAvailabilityDesign()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.MachineryAvailabilityReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//      public ActionResult WagonDrillAvailabilityDesign()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.WagonDrillAvailabilityReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//      public ActionResult DumpTruckAvailabilityDesign()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DumpTruckAvailabilityReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//      public ActionResult DryLoadingAvailabilityDesign()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DryLoadingAvailabilityReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }  [Authorize]
//      public ActionResult LightVehicleOwnRent()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.LightVehicleOwnRent, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }  [Authorize]
//      public ActionResult MachineryOwnRent()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.MachineryOwnRent, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }  [Authorize]
//      public ActionResult WagonDrillOwnRent()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.WagonDrillOwnRent, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//        public ActionResult DumpTruckOwnRent()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DumpTruckOwnRent, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [Authorize]
//        public ActionResult DryLoadingOwnRent()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DryLoadingOwnRent, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }[Authorize]
//        public ActionResult LightVehicleProfitLoose()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.LightVehicleProfitLoose, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }[Authorize]
//        public ActionResult MachineryProfitLoose()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.MachineryProfitLoose, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }[Authorize]
//        public ActionResult WagonDrillProfitLoose()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.WagonDrillProfitLoose, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }[Authorize]
//        public ActionResult DumpTruckProfitLoose()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DumpTruckProfitLoose, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }[Authorize]
//        public ActionResult DryLoadingProfitLoose()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.DryLoadingProfitLoose, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }

//        [Authorize]
//        public ActionResult SparePartConsumption()
//        {
//            return View();
//        }

//        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

//        public ActionResult HeadOfficeSummary()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.HeadOfficeSummary, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        //stop
//        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

//        public ActionResult EquipmentSummary()
//        {
//            return View();
//        }



//        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

//        public ActionResult DailyWorkOrder()
//        {
//            return View();
//        }

//        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

//        public ActionResult PMActualSchedule()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.PMActualSchedule, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }

//        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

//        public ActionResult EquipmentUtilizationReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentUtilizationReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }


//        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

//        public ActionResult ProjectSummaryReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.HeadOfficeSummary, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }

//        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

//        public ActionResult ProjectSummaryByTypeReport()
//        {
//            return View();
//        }


//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult FuelConsumptionReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.FuelConsumptionReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }

//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult FuelLogReport()
//        {
//            return View();
//        }

//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult FuelReport()
//        {
//            return View();
//        }
//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult HeadOfficeReportReport()
//        {
//            return View();
//        }

//        [AuthorizeRoles(FleetManagementRoles.FleetResourceReport)]
//        public ActionResult EquipmentDistrubtionReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentDistrubtionReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        //stop
//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult TimeSheetReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.TimeSheetReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }


//        public ActionResult EquipentTypeCost()
//        {
//            return View();
//        }

//        public ActionResult MaintenaceSummary()
//        {
//            return View();
//        }

//        public ActionResult ServiceType()
//        {
//            return View();
//        }

//        public ActionResult WorkOrder()
//        {
//            return View();
//        }
//        public ActionResult DownTime()
//        {
//            return View();
//        }
//        public ActionResult EquipmentTypeCostSummary()
//        {
//            return View();
//        }

//        //
//        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

//        public ActionResult EquipmentReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        //good
//        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]
//        public ActionResult OperatorReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.OperatorReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }


//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult SalvageStoreReport()
//        {
//            return View();
//        }
//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult TireLogReport()
//        {
//            return View();
//        }
//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult EquipmentPlanEvaluationReport()
//        {
//            return View();
//        }
//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult TyreReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.TyreReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        public ActionResult BatteryConsumption()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.BatteryConsumption, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult EquipmentInspectionReport()
//        {
//            return View();
//        }
//        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]
//        public ActionResult OperatorTransferReport()
//        {
//            return View();
//        }
//        [AuthorizeRoles(FleetManagementRoles.FleetResourceReport)]
//        public ActionResult EquipmentSiteTransferReport()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentSiteTransferReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        //stop
//        [AuthorizeRoles(FleetManagementRoles.FleetResourceReport)]
//        public ActionResult EquipmentReturnReport()
//        {
//            return View();
//        }

//        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]
//        public ActionResult AccidentReport()
//        {
//            return View();
//        }


//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult RentRequestReport()
//        {
//            return View();
//        }
//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult InternalRentReport()
//        {
//            return View();
//        }


//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult RentToExternalReport()
//        {
//            return View();
//        }
//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult RentFromExternalReport()
//        {
//            return View();
//        }





//        //

//        [AuthorizeRoles(FleetManagementRoles.FleetMaintenanceReport)]
//        public ActionResult JobRequestReport()
//        {
//            return View();
//        }
//        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
//        public ActionResult CannibalizationReport()
//        {
//            return View();
//        }
//        [AuthorizeRoles(FleetManagementRoles.FleetMaintenanceReport)]
//        public ActionResult JobOrderReport()
//        {
//            return View();
//        }
//        [AuthorizeRoles(FleetManagementRoles.FleetMaintenanceReport)]
//        public ActionResult MaintenanceForExternalReport()
//        {
//            return View();
//        }



//        public ActionResult PMSchedule()
//        {
//            return View();
//        }

//        public ActionResult LogisticPlan()
//        {
//            return View();
//        }

//        public ActionResult FleetInsurance()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.FleetInsurance, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }

//        public ActionResult FleetAccident()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.FleetAccident, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        public ActionResult FleetAccidentNested()
//        {
//            return View();
//        }
//        public ActionResult TotalFleetFuelConsumption()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.TotalFleetFuelConsumption, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }
//        public ActionResult FleetAnnualPlanSummary()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.FleetAnnualPlanSummary, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }

//        public ActionResult UtilizationSummary()  
//        {   
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.UtilizationSummary, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }

//        public ActionResult MonthlyUtilizationSummary()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.MonthlyUtilizationSummary, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }

//        public ActionResult UtilizationSummaryToDate()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.UtilizationSummaryToDate, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }

//        public ActionResult EquipmentDistribution()
//        {
//            IsoDocumentHelper.GetIsoDocumentInfo(Modules.FleetManagement, FleetConst.EquipmentDistrubtionReport, out string title, out string number, out string issueNumber);
//            ViewBag.IssueNumber = issueNumber;
//            ViewBag.FormTitle = title;
//            ViewBag.FormNumber = number;
//            return View();
//        }

//        protected override void Dispose(bool disposing)
//        {
//            db.Dispose();
//            base.Dispose(disposing);
//        }
//    }
//}