using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers
{
    [Authorize]
    public class ProcurementPrintController : Controller
    {
        // GET: Procurement/ProcurmentReport
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProcurementPlanPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ProcurementNeedPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult ForeignPurchaseCost(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult PurchaseRequistionPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult PurchaseRequistionReviewPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult InvitationLetter(int id)
            {
            ViewBag.Id = id;
            return View(); 
            }

        public ActionResult ServiceRequisitionPrint(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult PurchaseOrderPrint(int id)
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
        public ActionResult TenderTechnicalOpening(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult TenderFinancialOpening(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult TenderCancellation(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult TechnicalResult(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult TechnicalResultAm(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult TechnicalPassFailResult(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult TechnicalPassFailResultAm(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ProformaResultAm(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ProformaResultEn(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult LotResultAm(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult LotResultEn(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult GuaranteeExtension(string id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult GuaranteeRelease(string id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult GuaranteeForfeited(string id)
        {
            ViewBag.Id = id;
            return View();
        }
    }
}