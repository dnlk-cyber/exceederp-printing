using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Setting
{
     [Authorize(Roles = FinanceConstants.AccountReceivableAdmin)]
    public class ArCustomerTypeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
       

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ARCustomerTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArCustomerType> arcustomertypes = _db.ARCustomerTypes;
            DataSourceResult result = arcustomertypes.ToDataSourceResult(request, aRCustomerType => new {
                ArCustomerTypeId = aRCustomerType.ArCustomerTypeId,
                CustomerTypeName = aRCustomerType.CustomerTypeName,
                Description = aRCustomerType.Description,
                Enable = aRCustomerType.Enable,
                DateCreated = aRCustomerType.DateCreated,
                LastModified = aRCustomerType.LastModified,
                CreatedBy = aRCustomerType.CreatedBy,
                ModifiedBy = aRCustomerType.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARCustomerTypes_Create([DataSourceRequest]DataSourceRequest request, ArCustomerType aRCustomerType)
        {
            #region validation

            var exists = _db.ARCustomerTypes.Where(x => x.CustomerTypeName == aRCustomerType.CustomerTypeName).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The type already exists.");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ArCustomerType
                {
                    CustomerTypeName = aRCustomerType.CustomerTypeName,
                    Description = aRCustomerType.Description,
                    Enable = aRCustomerType.Enable,
                    DateCreated = DateTime.Now,
                    LastModified = aRCustomerType.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRCustomerType.ModifiedBy
                };

                _db.ARCustomerTypes.Add(entity);
                _db.SaveChanges();
                aRCustomerType.ArCustomerTypeId = entity.ArCustomerTypeId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR customer type", "Success", User.Identity.Name, $"created  {aRCustomerType.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRCustomerType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARCustomerTypes_Update([DataSourceRequest]DataSourceRequest request, ArCustomerType aRCustomerType)
        {
            #region validation

            var exists = _db.ARCustomerTypes.Where(x => x.ArCustomerTypeId != aRCustomerType.ArCustomerTypeId)
                                            .Where(x => x.CustomerTypeName == aRCustomerType.CustomerTypeName).AsQueryable();
            if (exists.Any())
            {
                ModelState.AddModelError("exist", "The type already exists.");
            }
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ArCustomerType
                {
                    ArCustomerTypeId = aRCustomerType.ArCustomerTypeId,
                    CustomerTypeName = aRCustomerType.CustomerTypeName,
                    Description = aRCustomerType.Description,
                    Enable = aRCustomerType.Enable,
                    DateCreated = aRCustomerType.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aRCustomerType.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ARCustomerTypes.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR customer type", "Success", User.Identity.Name, $"updated {aRCustomerType.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRCustomerType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARCustomerTypes_Destroy([DataSourceRequest]DataSourceRequest request, ArCustomerType aRCustomerType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArCustomerType
                {
                    ArCustomerTypeId = aRCustomerType.ArCustomerTypeId,
                    CustomerTypeName = aRCustomerType.CustomerTypeName,
                    Description = aRCustomerType.Description,
                    Enable = aRCustomerType.Enable,
                    DateCreated = aRCustomerType.DateCreated,
                    LastModified = aRCustomerType.LastModified,
                    CreatedBy = aRCustomerType.CreatedBy,
                    ModifiedBy = aRCustomerType.ModifiedBy
                };

                _db.ARCustomerTypes.Attach(entity);
                _db.ARCustomerTypes.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted customer type " + aRCustomerType.ArCustomerTypeId + " " + aRCustomerType.CustomerTypeName, Modules.Finance);
                #endregion
            }

            return Json(new[] { aRCustomerType }.ToDataSourceResult(request, ModelState));
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

        public ActionResult Enable_Disable(string id)
        {
            if (id != null)
            {
                var customerType = _db.ARCustomerTypes.Find(id);
                if (customerType != null && customerType.Enable)
                {
                    customerType.Enable = false;
                }
                else
                {
                    if (customerType != null) customerType.Enable = true;
                }
                if (customerType != null)
                {
                    _db.ARCustomerTypes.Attach(customerType);
                    _db.Entry(customerType).State = EntityState.Modified;
                    _db.SaveChanges();
                }
            }

            return RedirectToAction("Index");

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
