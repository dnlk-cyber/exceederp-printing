using ExceedERP.Core.Domain.FleetManagement;
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

        [Authorize]
        public ActionResult RentFromExternal()
        {
            return View();
        }

        [Authorize]
        public ActionResult EquipmentMaintenaceSummary()
        {
            return View();
        }
         [Authorize]
        public ActionResult PlanPerEquipmentEvaluation()
        {
            return View();
        }
         [Authorize]
         public ActionResult PlanVsAccomplishment()
         {
             return View();
         }
         [Authorize]
         public ActionResult RentTimeSheet()
         {
             return View();
         }
         [Authorize]
         public ActionResult RentTimeSheetEvaluation()
         {
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
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

        public ActionResult EquipmentUtilizationReport()
        {
            return View();
        }


        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]

        public ActionResult ProjectSummaryReport()
        {
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
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult FuelLogReport()
        {
            return View();
        }
        [AuthorizeRoles(FleetManagementRoles.FleetResourceReport)]
        public ActionResult EquipmentDistrubtionReport()
        {
            return View();
        }

        [AuthorizeRoles(FleetManagementRoles.FleetReport)]
        public ActionResult TimeSheetReport()
        {
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
            return View();
        }
        //good
        [AuthorizeRoles(FleetManagementRoles.FleetRegistrationReport)]
        public ActionResult OperatorReport()
        {
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


    }
}