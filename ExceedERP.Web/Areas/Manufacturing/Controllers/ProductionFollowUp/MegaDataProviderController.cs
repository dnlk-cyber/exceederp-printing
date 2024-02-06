using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp


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
            var types = db.JobCategories.ToList();

            return Json(types.Select(p => new
            {
                JobTypeId = p.JobCategoryId,
                Type = p.JobName,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetJobTypeItems(int jobTypeId)
        {
            var types = db.JobCategories.Where(x => x.JobCategoryId == jobTypeId&& x.HaveParent).ToList();

            return Json(types.Select(p => new
            {
                JobTypeItemId = p.JobCategoryId,
                Name = p.JobName,
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
            var jobTypes = db.JobCategories.ToList();

            var proformaItems = db.PFProformaItems.Where(x => x.ProformaId == proformaId).Select(x => x.JobTypeId).ToList();
            if (proformaItems.Any())
            {
                jobTypes = jobTypes.Where(x => proformaItems.Contains(x.JobCategoryId)).ToList();
            }

            return Json(jobTypes.Select(p => new
            {
                JobTypeId = p.JobCategoryId,
                Type = p.JobName,
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