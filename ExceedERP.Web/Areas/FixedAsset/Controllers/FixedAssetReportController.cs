using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Web.Helpers;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FixedAsset.Setting;

namespace ExceedERP.Web.Areas.FixedAsset.Controllers
{
    [AuthorizeRoles(FixedAssetConstants.FixedAssetReport)]

    public class FixedAssetReportController : Controller
    {
        // GET: FixedAsset/FixedAssetReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AssetMovement()
        {
            return View();
        }

        public ActionResult DepreciationCostByDepartment()
        {
            return View();
        }

        public ActionResult DepreciationCostQuarterly()
        {
            return View();
        }

        public ActionResult DepreciationCostAnnual()
        {
            return View();
        }


        public ActionResult AnnualDepreciationList()
        {
            return View();
        }

        public ActionResult Depreciation()
        {
            return View();
        }

        public ActionResult AssetCountSheet()
        {
            return View();
        }

        public ActionResult AssetCountSummary()
        {
            return View();
        }

        public ActionResult ScafoldingAndPanelCountSheet()
        {
            return View();
        }

        public ActionResult ScafoldingAndPanelCountSummary()
        {
            return View();
        }

        public ActionResult AssetMachineCountSheet()
        {
            return View();
        }

        public ActionResult AssetMachineCountSummary()
        {
            return View();
        }

        public ActionResult FixedAssetReportByCategory()
        {
            return View();
        }

        public ActionResult FixedAssetPurchaseDetail()
        {
            return View();
        }

        public ActionResult PurchasedAsset()
        {
            return View();
        }
        public ActionResult DepreciationByBranch()
        {
            return View();
        }
        public ActionResult AssetRegister()
        {
            return View();
        }

        public ActionResult Disposal()
        {
            return View();
        }

        public ActionResult DisposalDetails()
        {
            return View();
        }


    }
}