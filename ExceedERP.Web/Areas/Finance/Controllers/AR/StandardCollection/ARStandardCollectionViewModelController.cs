using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.StandardCollection
{
    public class ArStandardCollectionViewModelController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ARStandardCollectionViewModels_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ArStandardCollectionViewModel> arstandardcollectionviewmodels = _db.ARStandardCollectionViewModels;
            DataSourceResult result = arstandardcollectionviewmodels.ToDataSourceResult(request, aRStandardCollectionViewModel => new
            {
                ArStandardCollectionViewModelId = aRStandardCollectionViewModel.ArStandardCollectionViewModelId,
                InvoiceNumber = aRStandardCollectionViewModel.InvoiceNumber,
                InstallementNo = aRStandardCollectionViewModel.InstallementNo,
                ApplyAmount = aRStandardCollectionViewModel.ApplyAmount,
                BalanceDue = aRStandardCollectionViewModel.BalanceDue,
                ApplyDate = aRStandardCollectionViewModel.ApplyDate,
                OrgBranchName = aRStandardCollectionViewModel.OrgBranchName,
                DateCreated = aRStandardCollectionViewModel.DateCreated,
                LastModified = aRStandardCollectionViewModel.LastModified,
                CreatedBy = aRStandardCollectionViewModel.CreatedBy,
                ModifiedBy = aRStandardCollectionViewModel.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollectionViewModels_Create(int id, [DataSourceRequest]DataSourceRequest request, ArStandardCollectionViewModel aRStandardCollectionViewModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArStandardCollectionViewModel
                {

                    InvoiceNumber = aRStandardCollectionViewModel.InvoiceNumber,
                    InstallementNo = aRStandardCollectionViewModel.InstallementNo,
                    ApplyAmount = aRStandardCollectionViewModel.ApplyAmount,
                    BalanceDue = aRStandardCollectionViewModel.BalanceDue,
                    ApplyDate = aRStandardCollectionViewModel.ApplyDate,
                    OrgBranchName = aRStandardCollectionViewModel.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = aRStandardCollectionViewModel.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRStandardCollectionViewModel.ModifiedBy
                };

                _db.ARStandardCollectionViewModels.Add(entity);
                _db.SaveChanges();
                aRStandardCollectionViewModel.ArStandardCollectionViewModelId = entity.ArStandardCollectionViewModelId;
            }

            return Json(new[] { aRStandardCollectionViewModel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollectionViewModels_Update(int id, [DataSourceRequest]DataSourceRequest request, ArStandardCollectionViewModel aRStandardCollectionViewModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArStandardCollectionViewModel
                {
                    ArStandardCollectionViewModelId = aRStandardCollectionViewModel.ArStandardCollectionViewModelId,
                    InvoiceNumber = aRStandardCollectionViewModel.InvoiceNumber,
                    InstallementNo = aRStandardCollectionViewModel.InstallementNo,
                    ApplyAmount = aRStandardCollectionViewModel.ApplyAmount,
                    BalanceDue = aRStandardCollectionViewModel.BalanceDue,
                    ApplyDate = aRStandardCollectionViewModel.ApplyDate,
                    OrgBranchName = aRStandardCollectionViewModel.OrgBranchName,
                    DateCreated = aRStandardCollectionViewModel.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aRStandardCollectionViewModel.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ARStandardCollectionViewModels.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { aRStandardCollectionViewModel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARStandardCollectionViewModels_Destroy([DataSourceRequest]DataSourceRequest request, ArStandardCollectionViewModel aRStandardCollectionViewModel)
        {
            if (ModelState.IsValid)
            {
                var entity = new ArStandardCollectionViewModel
                {
                    ArStandardCollectionViewModelId = aRStandardCollectionViewModel.ArStandardCollectionViewModelId,
                    InvoiceNumber = aRStandardCollectionViewModel.InvoiceNumber,
                    InstallementNo = aRStandardCollectionViewModel.InstallementNo,
                    ApplyAmount = aRStandardCollectionViewModel.ApplyAmount,
                    BalanceDue = aRStandardCollectionViewModel.BalanceDue,
                    ApplyDate = aRStandardCollectionViewModel.ApplyDate,
                    OrgBranchName = aRStandardCollectionViewModel.OrgBranchName,
                    DateCreated = aRStandardCollectionViewModel.DateCreated,
                    LastModified = aRStandardCollectionViewModel.LastModified,
                    CreatedBy = aRStandardCollectionViewModel.CreatedBy,
                    ModifiedBy = aRStandardCollectionViewModel.ModifiedBy
                };

                _db.ARStandardCollectionViewModels.Attach(entity);
                _db.ARStandardCollectionViewModels.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { aRStandardCollectionViewModel }.ToDataSourceResult(request, ModelState));
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
