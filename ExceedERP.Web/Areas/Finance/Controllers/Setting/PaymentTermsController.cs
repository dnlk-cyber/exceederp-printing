using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting
{
     [AuthorizeRoles(FinanceConstants.FinanceAdmin)]
    public class PaymentTermsController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PaymentTerms_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PaymentTerms> paymentterms = _db.PaymentTerms;
            DataSourceResult result = paymentterms.ToDataSourceResult(request, paymentTerms => new {
                PaymentTermsID = paymentTerms.PaymentTermsID,
                PaymentTermsDays = paymentTerms.PaymentTermsDays,
                Description = paymentTerms.Description,
                OrgBranchName = paymentTerms.OrgBranchName,
                DateCreated = paymentTerms.DateCreated,
                LastModified = paymentTerms.LastModified,
                CreatedBy = paymentTerms.CreatedBy,
                ModifiedBy = paymentTerms.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentTerms_Create([DataSourceRequest]DataSourceRequest request, PaymentTerms paymentTerms)
        {
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var exists = _db.PaymentTerms.Where(x => x.PaymentTermsID != paymentTerms.PaymentTermsID).Where(x => x.PaymentTermsDays.Equals(paymentTerms.PaymentTermsDays)).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", paymentTerms.PaymentTermsDays + " already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new PaymentTerms
                    {
                        PaymentTermsDays = paymentTerms.PaymentTermsDays,
                        Description = paymentTerms.Description,
                        OrgBranchName = paymentTerms.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = paymentTerms.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = paymentTerms.ModifiedBy
                    };

                    _db.PaymentTerms.Add(entity);
                    _db.SaveChanges();
                    paymentTerms.PaymentTermsID = entity.PaymentTermsID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Finance payment terms", "Success", User.Identity.Name, $"created payment terms {entity.PaymentTermsDays}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { paymentTerms }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentTerms_Update([DataSourceRequest]DataSourceRequest request, PaymentTerms paymentTerms)
        {
            if (ModelState.IsValid)
            {
                 #region validation

                bool valid = true;
                var exists = _db.PaymentTerms.Where(x => x.PaymentTermsID != paymentTerms.PaymentTermsID).Where(x => x.PaymentTermsDays.Equals(paymentTerms.PaymentTermsDays)).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", paymentTerms.PaymentTermsDays + " already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new PaymentTerms
                    {
                        PaymentTermsID = paymentTerms.PaymentTermsID,
                        PaymentTermsDays = paymentTerms.PaymentTermsDays,
                        Description = paymentTerms.Description,
                        OrgBranchName = paymentTerms.OrgBranchName,
                        DateCreated = paymentTerms.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = paymentTerms.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.PaymentTerms.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Finance payment terms", "Success", User.Identity.Name, $"updated payment terms {entity.PaymentTermsDays}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { paymentTerms }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentTerms_Destroy([DataSourceRequest]DataSourceRequest request, PaymentTerms paymentTerms)
        {
            if (ModelState.IsValid)
            {
                var entity = new PaymentTerms
                {
                    PaymentTermsID = paymentTerms.PaymentTermsID,
                    PaymentTermsDays = paymentTerms.PaymentTermsDays,
                    Description = paymentTerms.Description,
                    OrgBranchName = paymentTerms.OrgBranchName,
                    DateCreated = paymentTerms.DateCreated,
                    LastModified = paymentTerms.LastModified,
                    CreatedBy = paymentTerms.CreatedBy,
                    ModifiedBy = paymentTerms.ModifiedBy
                };

                _db.PaymentTerms.Attach(entity);
                _db.PaymentTerms.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { paymentTerms }.ToDataSourceResult(request, ModelState));
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
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
