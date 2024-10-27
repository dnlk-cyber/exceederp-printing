using System.Web.Mvc;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Procurement.Controllers
{
    [AuthorizeRoles(ProcurementRoles.ProcurementAdmin, ProcurementRoles.ProcurementReport,PlanningRoles.Planning_User )]

    public class ProcurementReportController : Controller
    {
        // GET: Procurement/ProcurmentReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PurchasedItemReport()
        {
            return View();
        }

        public ActionResult PurchaseReport()
        {
            return View();
        }

        public ActionResult MonthlyMaterialRequirement()
        {
            return View();
        }

        public ActionResult ProcurementPlanReport()
        {
            return View();
        }

        public ActionResult ProcurementNeedReport()
        {
            return View();
        }

        public ActionResult ProcurementPlanVsExecutedReport()
        {
            return View();
        }

        public ActionResult SupplyPlanVsExecutedReport()
        {
            return View();
        }

        public ActionResult Letter(string id)
            {
            ViewBag.Id = id;
            return View(); 
        } 
        public ActionResult AwardAmharic(string id)
            {
            ViewBag.Id = id;
            return View(); 
        } 
        public ActionResult Award(string id)
            {
            ViewBag.Id = id;
            return View(); 
        }
        public ActionResult TechnicalAmharic(string id)
            {
            ViewBag.Id = id;
            return View(); 
        }

      


        //new 
        public ActionResult ContractListReport()//ContractListReport
        {
            return View();
        }
        public ActionResult GuaranteeListReport()//GuaranteeListReport
        {
            return View();
        }
        public ActionResult MonthlyPurchaseByCategoryAndAmountReport()//MonthlyPurchaseByCategoryByAmountReport
        {
            return View();
        }
        public ActionResult PurchaseFollowUpReport()//PurchaseFollowUpReport
        {
            return View();
        }
        public ActionResult ContractByCategoryReport()//ContractByCategoryReport
        {
            return View();
        }
        public ActionResult PurchaseSummaryReportByPurchaseMethod()//PurchaseSummaryReportByPurchaseMethod
        {
            return View();
        }
        public ActionResult PurchaseFollowingForm()//
        {
            return View();
        }
        public ActionResult PurchaseRequisition()//
        {
            return View();
        }

        public ActionResult PurchaseRequisitionPurchased()
        {
            return View();
        }

        public ActionResult PurchaseRequisitionByStatus()
        {
            return View();
        }

        public ActionResult PurchaseRequisitionCompleted()
        {
            return View();
        }

    }
}