using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.Setting
{
     [Authorize(Roles = AccountReceivableAdmin)]
    public class ArSetupTransactionTypeController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private const string AccountReceivableAdmin = "Account_Receivable_Admin";

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ARSetupTransactionTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ARSetupTransactionType> arsetuptransactiontypes = _db.ARSetupTransactionTypes;
            DataSourceResult result = arsetuptransactiontypes.ToDataSourceResult(request, aRSetupTransactionType => new {
                ARSetupTransactionTypeID = aRSetupTransactionType.ARSetupTransactionTypeID,
                TransactionName = aRSetupTransactionType.TransactionName,
                TransactionClass = aRSetupTransactionType.TransactionClass,
                Description = aRSetupTransactionType.Description,
                StartDate = aRSetupTransactionType.StartDate,
                CreationSign = aRSetupTransactionType.CreationSign,
                OpenReceivable = aRSetupTransactionType.OpenReceivable,
                PosttoGL = aRSetupTransactionType.PosttoGL,
                IsActive = aRSetupTransactionType.IsActive,
                ReceivableAccount = aRSetupTransactionType.ReceivableAccount,
                RevenueAccount = aRSetupTransactionType.RevenueAccount,
                TaxAccount = aRSetupTransactionType.TaxAccount,
                FreightAccount = aRSetupTransactionType.FreightAccount,
                ClearingAccount = aRSetupTransactionType.ClearingAccount,
                UnearnedAccount = aRSetupTransactionType.UnearnedAccount,
                OrgBranchName = aRSetupTransactionType.OrgBranchName,
                DateCreated = aRSetupTransactionType.DateCreated,
                LastModified = aRSetupTransactionType.LastModified,
                CreatedBy = aRSetupTransactionType.CreatedBy,
                ModifiedBy = aRSetupTransactionType.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupTransactionTypes_Create([DataSourceRequest]DataSourceRequest request, ARSetupTransactionType aRSetupTransactionType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupTransactionType
                {
                    TransactionName = aRSetupTransactionType.TransactionName,
                    TransactionClass = aRSetupTransactionType.TransactionClass,
                    Description = aRSetupTransactionType.Description,
                    StartDate = aRSetupTransactionType.StartDate,
                    CreationSign = aRSetupTransactionType.CreationSign,
                    OpenReceivable = aRSetupTransactionType.OpenReceivable,
                    PosttoGL = aRSetupTransactionType.PosttoGL,
                    IsActive = aRSetupTransactionType.IsActive,
                    ReceivableAccount = aRSetupTransactionType.ReceivableAccount,
                    RevenueAccount = aRSetupTransactionType.RevenueAccount,
                    TaxAccount = aRSetupTransactionType.TaxAccount,
                    FreightAccount = aRSetupTransactionType.FreightAccount,
                    ClearingAccount = aRSetupTransactionType.ClearingAccount,
                    UnearnedAccount = aRSetupTransactionType.UnearnedAccount,
                    OrgBranchName = aRSetupTransactionType.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = aRSetupTransactionType.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = aRSetupTransactionType.ModifiedBy
                };

                _db.ARSetupTransactionTypes.Add(entity);
                _db.SaveChanges();
                aRSetupTransactionType.ARSetupTransactionTypeID = entity.ARSetupTransactionTypeID;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR transaction type", "Success", User.Identity.Name, $"created  {aRSetupTransactionType.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRSetupTransactionType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupTransactionTypes_Update([DataSourceRequest]DataSourceRequest request, ARSetupTransactionType aRSetupTransactionType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupTransactionType
                {
                    ARSetupTransactionTypeID = aRSetupTransactionType.ARSetupTransactionTypeID,
                    TransactionName = aRSetupTransactionType.TransactionName,
                    TransactionClass = aRSetupTransactionType.TransactionClass,
                    Description = aRSetupTransactionType.Description,
                    StartDate = aRSetupTransactionType.StartDate,
                    CreationSign = aRSetupTransactionType.CreationSign,
                    OpenReceivable = aRSetupTransactionType.OpenReceivable,
                    PosttoGL = aRSetupTransactionType.PosttoGL,
                    IsActive = aRSetupTransactionType.IsActive,
                    ReceivableAccount = aRSetupTransactionType.ReceivableAccount,
                    RevenueAccount = aRSetupTransactionType.RevenueAccount,
                    TaxAccount = aRSetupTransactionType.TaxAccount,
                    FreightAccount = aRSetupTransactionType.FreightAccount,
                    ClearingAccount = aRSetupTransactionType.ClearingAccount,
                    UnearnedAccount = aRSetupTransactionType.UnearnedAccount,
                    OrgBranchName = aRSetupTransactionType.OrgBranchName,
                    DateCreated = aRSetupTransactionType.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = aRSetupTransactionType.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                _db.ARSetupTransactionTypes.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "AR transaction type", "Success", User.Identity.Name, $"updated  {aRSetupTransactionType.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { aRSetupTransactionType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARSetupTransactionTypes_Destroy([DataSourceRequest]DataSourceRequest request, ARSetupTransactionType aRSetupTransactionType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ARSetupTransactionType
                {
                    ARSetupTransactionTypeID = aRSetupTransactionType.ARSetupTransactionTypeID,
                    TransactionName = aRSetupTransactionType.TransactionName,
                    TransactionClass = aRSetupTransactionType.TransactionClass,
                    Description = aRSetupTransactionType.Description,
                    StartDate = aRSetupTransactionType.StartDate,
                    CreationSign = aRSetupTransactionType.CreationSign,
                    OpenReceivable = aRSetupTransactionType.OpenReceivable,
                    PosttoGL = aRSetupTransactionType.PosttoGL,
                    IsActive = aRSetupTransactionType.IsActive,
                    ReceivableAccount = aRSetupTransactionType.ReceivableAccount,
                    RevenueAccount = aRSetupTransactionType.RevenueAccount,
                    TaxAccount = aRSetupTransactionType.TaxAccount,
                    FreightAccount = aRSetupTransactionType.FreightAccount,
                    ClearingAccount = aRSetupTransactionType.ClearingAccount,
                    UnearnedAccount = aRSetupTransactionType.UnearnedAccount,
                    OrgBranchName = aRSetupTransactionType.OrgBranchName,
                    DateCreated = aRSetupTransactionType.DateCreated,
                    LastModified = aRSetupTransactionType.LastModified,
                    CreatedBy = aRSetupTransactionType.CreatedBy,
                    ModifiedBy = aRSetupTransactionType.ModifiedBy
                };

                _db.ARSetupTransactionTypes.Attach(entity);
                _db.ARSetupTransactionTypes.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { aRSetupTransactionType }.ToDataSourceResult(request, ModelState));
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
