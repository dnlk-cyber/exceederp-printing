using System;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
	public class SalesLotResultsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetWinnersByHighestPrice(DateTime? evaluationDate)
        {
            if (evaluationDate == null)
            {
                return Json(new { error = "Evaluation date is required" }, JsonRequestBehavior.AllowGet);
            }

            var winners = db.SalesLotParticipantItems
                .Where(item => item.EvaluationDate == evaluationDate.Value) // Use the selected date
                .GroupBy(item => item.ProductID)
                .Select(group => group
                    .OrderByDescending(item => item.UnitPrice) // Select the highest price
                    .FirstOrDefault()
                )
                .Select(winner => new
                {
                    winner.ProductID,
                    winner.SalesLotParticipantId,
                    winner.UnitPrice,
                    winner.EvaluationDate
                })
                .ToList();

            return Json(winners, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}