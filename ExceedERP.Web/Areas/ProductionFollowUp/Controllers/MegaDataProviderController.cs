using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers
{
    public class MegaDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: ProductionFollowUp/MegaDataProvider
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetJobTypes()
        {
            var types = db.JobTypes.ToList();

            return Json(types.Select(p => new
            {
                JobTypeId = p.JobTypeId,
                Type = p.Type,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTypeItems(int jobTypeId)
        {
            var types = db.JobTypeItems.Where(x => x.JobTypeId == jobTypeId).ToList();

            return Json(types.Select(p => new
            {
                JobTypeItemId = p.JobTypeItemId,
                Name = p.Name,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTaxTypes()
        {
            var taxTypes = db.GLTaxRates.ToList();

            return Json(taxTypes.Select(p => new
            {
                GLTaxRateID = p.GLTaxRateID,
                TaxName = p.TaxName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProformas()
        {
            var proformas = db.Proformas.ToList();

            return Json(proformas.Select(p => new
            {
                ProformaId = p.ProformaId,
                CustomerName = p.ProformaNo + "-" + p.CustomerName,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetJobTypesByProforma(int proformaId = 0)
        {
            var jobTypes = db.JobTypes.ToList();

            var proformaItems = db.PFProformaItems.Where(x => x.ProformaId == proformaId).Select(x => x.JobTypeId).ToList();
            if (proformaItems.Any())
            {
                jobTypes = jobTypes.Where(x => proformaItems.Contains(x.JobTypeId)).ToList();
            }

            return Json(jobTypes.Select(p => new
            {
                JobTypeId = p.JobTypeId,
                Type = p.Type,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobOrders()
        {
            var jobOrders = db.Jobs.ToList();

            return Json(jobOrders.Select(p => new
            {
                JobId = p.JobId,
                JobNo = p.JobNo,
            }), JsonRequestBehavior.AllowGet);
        }
    }
}