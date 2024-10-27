using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting
{
    //[AuthorizeRoles(FinanceConstants.FinanceAdmin)]
    public class InvoiceArchiveController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Data_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InvoiceArchive> archives = _db.InvoiceArchives;
            DataSourceResult result = archives.ToDataSourceResult(request, archive => new InvoiceArchive{
                InvoiceArchiveId = archive.InvoiceArchiveId,
                PaidFor = archive.PaidFor,
                InvoiceNo = archive.InvoiceNo,
                VoucherNo = archive.VoucherNo,
                AmountPaid = archive.AmountPaid,
                DatePaid = archive.DatePaid,
                BoxFileNo = archive.BoxFileNo,
                

                DateCreated = archive.DateCreated,
                LastModified = archive.LastModified,
                CreatedBy = archive.CreatedBy,
                ModifiedBy = archive.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Create([DataSourceRequest]DataSourceRequest request, InvoiceArchive archive)
        {
            if (ModelState.IsValid)
            {
                #region validation

                bool valid = true;
                var exists = _db.InvoiceArchives.Where(x => x.InvoiceArchiveId != archive.InvoiceArchiveId).Where(x => x.InvoiceNo.Equals(archive.InvoiceNo)).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", archive.InvoiceNo + " already exists.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new InvoiceArchive
                    {
                        InvoiceArchiveId = archive.InvoiceArchiveId,
                        PaidFor = archive.PaidFor,
                        InvoiceNo = archive.InvoiceNo,
                        VoucherNo = archive.VoucherNo,
                        AmountPaid = archive.AmountPaid,
                        DatePaid = archive.DatePaid,
                        BoxFileNo = archive.BoxFileNo,

                        DateCreated = DateTime.Now,
                        LastModified = archive.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = archive.ModifiedBy
                    };

                    _db.InvoiceArchives.Add(entity);
                    _db.SaveChanges();
                    archive.InvoiceArchiveId = entity.InvoiceArchiveId;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Invoice Archive", "Success", User.Identity.Name, $"created invoice archive {entity.InvoiceNo}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { archive }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Update([DataSourceRequest]DataSourceRequest request, InvoiceArchive archive)
        {
            if (ModelState.IsValid)
            {
                 #region validation

                bool valid = true;
                var exists = _db.InvoiceArchives.Where(x => x.InvoiceArchiveId != archive.InvoiceArchiveId).Where(x => x.InvoiceNo.Equals(archive.InvoiceNo)).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("", archive.InvoiceNo + " already exists.");
                    valid = false;
                }

                #endregion

                if (valid)
                {
                    var entity = new InvoiceArchive
                    {
                        InvoiceArchiveId = archive.InvoiceArchiveId,
                        PaidFor = archive.PaidFor,
                        InvoiceNo = archive.InvoiceNo,
                        VoucherNo = archive.VoucherNo,
                        AmountPaid = archive.AmountPaid,
                        DatePaid = archive.DatePaid,
                        BoxFileNo = archive.BoxFileNo,

                        DateCreated = archive.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = archive.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.InvoiceArchives.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Finance invoice archive", "Success", User.Identity.Name, $"updated invoice archive {entity.InvoiceNo}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { archive }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Data_Destroy([DataSourceRequest]DataSourceRequest request, InvoiceArchive archive)
        {
            if (ModelState.IsValid)
            {
                var entity = new InvoiceArchive
                {
                    InvoiceArchiveId = archive.InvoiceArchiveId,
                    PaidFor = archive.PaidFor,
                    InvoiceNo = archive.InvoiceNo,
                    VoucherNo = archive.VoucherNo,
                    AmountPaid = archive.AmountPaid,
                    DatePaid = archive.DatePaid,
                    BoxFileNo = archive.BoxFileNo,

                    DateCreated = archive.DateCreated,
                    LastModified = archive.LastModified,
                    CreatedBy = archive.CreatedBy,
                    ModifiedBy = archive.ModifiedBy
                };

                _db.InvoiceArchives.Attach(entity);
                _db.InvoiceArchives.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { archive }.ToDataSourceResult(request, ModelState));
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
