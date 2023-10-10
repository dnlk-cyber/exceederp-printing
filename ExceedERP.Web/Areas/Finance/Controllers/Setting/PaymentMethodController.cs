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
    public class PaymentMethodController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var branches = _db.Branch;
            ViewData["Branches"] = branches;

            var businessUnits = _db.BusinessUnits;
            ViewData["businessUnits"] = businessUnits;
        }

        public ActionResult PaymentMethods_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PaymentMethod> paymentmethods = _db.PaymentMethods;
            DataSourceResult result = paymentmethods.ToDataSourceResult(request, paymentMethod => new {
                PaymentMethodID = paymentMethod.PaymentMethodID,
                Method = paymentMethod.Method,
                BranchId = paymentMethod.BranchId,
                BusinessUnitId = paymentMethod.BusinessUnitId,
                ApplyToAr = paymentMethod.ApplyToAr,
                ArAccountCode = paymentMethod.ArAccountCode,
                ApplyToAp = paymentMethod.ApplyToAp,
                ApAccountCode = paymentMethod.ApAccountCode,
                Enable = paymentMethod.Enable,
                Description = paymentMethod.Description,
                OrgBranchName = paymentMethod.OrgBranchName,
                DateCreated = paymentMethod.DateCreated,
                LastModified = paymentMethod.LastModified,
                CreatedBy = paymentMethod.CreatedBy,
                ModifiedBy = paymentMethod.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentMethods_Create([DataSourceRequest]DataSourceRequest request, PaymentMethod paymentMethod)
        {
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var exists = _db.PaymentMethods
                    .Where(x => x.PaymentMethodID != paymentMethod.PaymentMethodID)
                    .Where(x => x.Method.ToUpper().Equals(paymentMethod.Method.ToUpper())).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", paymentMethod.Method + " already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new PaymentMethod
                    {
                        Method = paymentMethod.Method,
                        BranchId = paymentMethod.BranchId,
                        BusinessUnitId = paymentMethod.BusinessUnitId,
                        ApplyToAr = paymentMethod.ApplyToAr,
                        ArAccountCode = paymentMethod.ArAccountCode,
                        ApplyToAp = paymentMethod.ApplyToAp,
                        ApAccountCode = paymentMethod.ApAccountCode,
                        Enable = paymentMethod.Enable,
                        Description = paymentMethod.Description,
                        OrgBranchName = paymentMethod.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = paymentMethod.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = paymentMethod.ModifiedBy
                    };

                    _db.PaymentMethods.Add(entity);
                    _db.SaveChanges();
                    paymentMethod.PaymentMethodID = entity.PaymentMethodID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Finance payment methods", "Success", User.Identity.Name, $"created method {entity.Method}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { paymentMethod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentMethods_Update([DataSourceRequest]DataSourceRequest request, PaymentMethod paymentMethod)
        {
            if (ModelState.IsValid)
            {
                 #region validation

                bool valid = true;
                var exists = _db.PaymentMethods.Where(x => x.PaymentMethodID != paymentMethod.PaymentMethodID).Where(x => x.Method.ToUpper().Equals(paymentMethod.Method.ToUpper())).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", paymentMethod.Method + " already exists in the system.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new PaymentMethod
                    {
                        PaymentMethodID = paymentMethod.PaymentMethodID,
                        Method = paymentMethod.Method,
                        BranchId = paymentMethod.BranchId,
                        BusinessUnitId = paymentMethod.BusinessUnitId,
                        ApplyToAr = paymentMethod.ApplyToAr,
                        ArAccountCode = paymentMethod.ArAccountCode,
                        ApplyToAp = paymentMethod.ApplyToAp,
                        ApAccountCode = paymentMethod.ApAccountCode,
                        Description = paymentMethod.Description,
                        Enable = paymentMethod.Enable,
                        OrgBranchName = paymentMethod.OrgBranchName,
                        DateCreated = paymentMethod.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = paymentMethod.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.PaymentMethods.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Finance payment methods", "Success", User.Identity.Name, $"updated method {entity.Method}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { paymentMethod }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PaymentMethods_Destroy([DataSourceRequest]DataSourceRequest request, PaymentMethod paymentMethod)
        {
            if (ModelState.IsValid)
            {
                var entity = new PaymentMethod
                {
                    PaymentMethodID = paymentMethod.PaymentMethodID,
                    Method = paymentMethod.Method,
                    BranchId = paymentMethod.BranchId,
                    BusinessUnitId = paymentMethod.BusinessUnitId,
                    ApplyToAr = paymentMethod.ApplyToAr,
                    ArAccountCode = paymentMethod.ArAccountCode,
                    ApplyToAp = paymentMethod.ApplyToAp,
                    ApAccountCode = paymentMethod.ApAccountCode,
                    Description = paymentMethod.Description,
                    Enable = paymentMethod.Enable,
                    OrgBranchName = paymentMethod.OrgBranchName,
                    DateCreated = paymentMethod.DateCreated,
                    LastModified = paymentMethod.LastModified,
                    CreatedBy = paymentMethod.CreatedBy,
                    ModifiedBy = paymentMethod.ModifiedBy
                };

                _db.PaymentMethods.Attach(entity);
                _db.PaymentMethods.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { paymentMethod }.ToDataSourceResult(request, ModelState));
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
