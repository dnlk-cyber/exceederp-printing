using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers
{
    [AuthorizeRoles(ConstructionConstants.ProjectReporting)]
    public class ConstructionReportController : Controller
    {
        // GET: Construction/ConstructionReport
        //public ActionResult Index()
        //{
        //    return View();
        //}
        public ActionResult TemplateWorkItems()
        {
            return View();
        }
        public ActionResult BoQSummaryReport()
        {
            return View();
        }
        public ActionResult ResourceUtilizationReport()
        {
            return View();
        }
        public ActionResult GrandSummaryReport()
        {
            return View();
        }
        public ActionResult SummaryReport()
        {
            return View();
        }
        public ActionResult EarnedValueAnalysisPerProjectReport()
        {
            return View();
        }
        public ActionResult EarnedValueAnalysisGraphicalReport()
        {
            return View();
        }
        public ActionResult EarnedValuePerPerProjectYearlyReport()
        {
            return View();
        }
        public ActionResult EarnedValuePerPerProjectGraph()
        {
            return View();
        }
        public ActionResult EarnedValuePerItemReport()
        {
            return View();
        }
        public ActionResult EVPerProjectSummaryReport()
        {
            return View();
        }
        // Summary
        //      Performance evaluation report of Road Projects
        public ActionResult PerformanceEvaluation()
        {
            return View();
        }
        // Summary :
        //      Performance evaluation of building projects
        public ActionResult PerformanceEvaluationReport()
        {
            return View();
        }
        public ActionResult PaymentCertificateReport()
        {
            return View();
        }
        public ActionResult MaterialExpenseReport()
        {
            return View();
        }

        public ActionResult EstimationPieceRateReport()
        {
            return View();
        }

        public ActionResult EstimationSubcontractReport()
        {
            return View();
        }

        public ActionResult CustomWorkItemReport()
        {
            return View();
        }

        public ActionResult EstimationTemplateWorkItemsReport()
        {
            return View();
        }

        public ActionResult ScheduleChartReport()
        {
            return View();
        }
        public ActionResult BoQReport()
        {
            return View();
        }
        public ActionResult PriceAdjustment()
        {
            return View();
        }
        public ActionResult WorkProgressSummaryReport()
        {
            return View();
        }
        public ActionResult WorkProgressReport()
        {
            return View();
        }
        public ActionResult WorkProgressSummarySummaryReport()
        {
            return View();
        }
        public ActionResult DirectFuelAndLubricantReport()
        {
            return View();
        }
        public ActionResult IndirectFuelAndLubricantReport()
        {
            return View();
        }
        public ActionResult MiscellaneousExpensesReport()
        {
            return View();
        }
        public ActionResult DirectMaterialExpense()
        {
            return View();
        }
        public ActionResult InDirectMaterialExpense()
        {
            return View();
        }
        public ActionResult ProjectEstimationReport()
        {
            return View();
        }
        public ActionResult ProjectWorkItems()
        {
            return View();
        }
        public ActionResult ProjectCostAnalysis()
        {
            return View();
        }
        public ActionResult CashFlowReport()
        {
            return View();
        }
        public ActionResult MajorMaterialsReport()
        {
            return View();
        }
        public ActionResult RepresentativeElementsReport()
        {
            return View();
        }
        public ActionResult WorkCategory()
        {
            return View();
        }
        public ActionResult RoadGrandSummary()
        {
            return View();
        }
        public ActionResult EvaluationSummary()
        {
            return View();
        }
        public ActionResult RoadEvaluationSummary()
        {
            return View();
        }
    }
}