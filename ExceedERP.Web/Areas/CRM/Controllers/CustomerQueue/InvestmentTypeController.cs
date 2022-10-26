using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMQueueSetting)]
    public class InvestmentTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InvestmentTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InvestmentType> investmenttypes = db.InvestmentTypes;
            DataSourceResult result = investmenttypes.ToDataSourceResult(request, investmentType => new {
                InvestmentTypeId = investmentType.InvestmentTypeId,
                InvestmentName = investmentType.InvestmentName,
                Description = investmentType.Description,
                OrgBranchName = investmentType.OrgBranchName,
                DateCreated = investmentType.DateCreated,
                LastModified = investmentType.LastModified,
                CreatedBy = investmentType.CreatedBy,
                ModifiedBy = investmentType.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvestmentTypes_Create([DataSourceRequest]DataSourceRequest request, InvestmentType investmentType)
        {
            if (ModelState.IsValid)
            {
                var entity = new InvestmentType
                {
                    InvestmentName = investmentType.InvestmentName,
                    Description = investmentType.Description,
                    OrgBranchName = investmentType.OrgBranchName,
                    DateCreated = investmentType.DateCreated,
                    LastModified = investmentType.LastModified,
                    CreatedBy = investmentType.CreatedBy,
                    ModifiedBy = investmentType.ModifiedBy
                };

                db.InvestmentTypes.Add(entity);
                db.SaveChanges();
                investmentType.InvestmentTypeId = entity.InvestmentTypeId;
            }

            return Json(new[] { investmentType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvestmentTypes_Update([DataSourceRequest]DataSourceRequest request, InvestmentType investmentType)
        {
            if (ModelState.IsValid)
            {
                var entity = new InvestmentType
                {
                    InvestmentTypeId = investmentType.InvestmentTypeId,
                    InvestmentName = investmentType.InvestmentName,
                    Description = investmentType.Description,
                    OrgBranchName = investmentType.OrgBranchName,
                    DateCreated = investmentType.DateCreated,
                    LastModified = investmentType.LastModified,
                    CreatedBy = investmentType.CreatedBy,
                    ModifiedBy = investmentType.ModifiedBy
                };

                db.InvestmentTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { investmentType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvestmentTypes_Destroy([DataSourceRequest]DataSourceRequest request, InvestmentType investmentType)
        {
            if (ModelState.IsValid)
            {
                var entity = new InvestmentType
                {
                    InvestmentTypeId = investmentType.InvestmentTypeId,
                    InvestmentName = investmentType.InvestmentName,
                    Description = investmentType.Description,
                    OrgBranchName = investmentType.OrgBranchName,
                    DateCreated = investmentType.DateCreated,
                    LastModified = investmentType.LastModified,
                    CreatedBy = investmentType.CreatedBy,
                    ModifiedBy = investmentType.ModifiedBy
                };

                db.InvestmentTypes.Attach(entity);
                db.InvestmentTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { investmentType }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
