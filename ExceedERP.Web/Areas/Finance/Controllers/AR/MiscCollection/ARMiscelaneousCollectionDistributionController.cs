using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.AR.MiscCollection
{
    public class ArMiscelaneousCollectionDistributionController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly AccountHelper _accountHelper = new AccountHelper();


        [AuthorizeRoles(FinanceConstants.AccountReceivableView  , FinanceConstants.AccountReceivableApprover ,FinanceConstants.AccountReceivableCollection)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView  , FinanceConstants.AccountReceivableApprover ,FinanceConstants.AccountReceivableCollection)]
        public ActionResult ARMiscelaneousCollectionDistributions_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ARMiscelaneousCollectionDistribution> armiscelaneouscollectiondistributions = _db.ARMiscelaneousCollectionDistributions.Where(x=>x.ARMiscelaneousCollectionID==id);
            DataSourceResult result = armiscelaneouscollectiondistributions.ToDataSourceResult(request, aRMiscelaneousCollectionDistribution => new {
                ARMiscelaneousCollectionDistributionID = aRMiscelaneousCollectionDistribution.ARMiscelaneousCollectionDistributionID,
                ARMiscelaneousCollectionID=id,
                AccountCode = aRMiscelaneousCollectionDistribution.AccountCode,
                 AccountDesc = _accountHelper.GetAccountDesc(aRMiscelaneousCollectionDistribution.AccountCode),
                AdditionalDescription = aRMiscelaneousCollectionDistribution.AdditionalDescription,
                Debit = aRMiscelaneousCollectionDistribution.Debit,
                Credit = aRMiscelaneousCollectionDistribution.Credit,
                OrgBranchName = aRMiscelaneousCollectionDistribution.OrgBranchName,
                DateCreated = aRMiscelaneousCollectionDistribution.DateCreated,
                LastModified = aRMiscelaneousCollectionDistribution.LastModified,
                CreatedBy = aRMiscelaneousCollectionDistribution.CreatedBy,
                ModifiedBy = aRMiscelaneousCollectionDistribution.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARMiscelaneousCollectionDistributions_Create(int id,[DataSourceRequest]DataSourceRequest request, ARMiscelaneousCollectionDistribution aRMiscelaneousCollectionDistribution)
        {
             bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ARMiscelaneousCollection parent =
                    _db.ARMiscelaneousCollections.FirstOrDefault(
                        x =>
                            x.ARMiscelaneousCollectionID ==
                            aRMiscelaneousCollectionDistribution.ARMiscelaneousCollectionID);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ARMiscelaneousCollectionDistribution
                    {
                        ARMiscelaneousCollectionID = id,
                        AccountCode = aRMiscelaneousCollectionDistribution.AccountCode,
                         AccountDesc = _accountHelper.GetAccountDesc(aRMiscelaneousCollectionDistribution.AccountCode),
                        AdditionalDescription = aRMiscelaneousCollectionDistribution.AdditionalDescription,
                        Debit = aRMiscelaneousCollectionDistribution.Debit,
                        Credit = aRMiscelaneousCollectionDistribution.Credit,
                        OrgBranchName = aRMiscelaneousCollectionDistribution.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = aRMiscelaneousCollectionDistribution.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = aRMiscelaneousCollectionDistribution.ModifiedBy
                    };

                    _db.ARMiscelaneousCollectionDistributions.Add(entity);
                    _db.SaveChanges();
                    aRMiscelaneousCollectionDistribution.ARMiscelaneousCollectionDistributionID =
                        entity.ARMiscelaneousCollectionDistributionID;

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AR misc collection distribution", "Success", User.Identity.Name, $"created misc collection distribution {entity.AccountCode} under record {entity.ARMiscelaneousCollectionID} ", Modules.Finance);
                    #endregion
                }
            }
            return Json(new[] { aRMiscelaneousCollectionDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARMiscelaneousCollectionDistributions_Update(int id,[DataSourceRequest]DataSourceRequest request, ARMiscelaneousCollectionDistribution aRMiscelaneousCollectionDistribution)
        {
             bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ARMiscelaneousCollection parent =
                    _db.ARMiscelaneousCollections.FirstOrDefault(
                        x => x.ARMiscelaneousCollectionID == aRMiscelaneousCollectionDistribution.ARMiscelaneousCollectionID);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ARMiscelaneousCollectionDistribution
                    {
                        ARMiscelaneousCollectionID = id,
                        ARMiscelaneousCollectionDistributionID =aRMiscelaneousCollectionDistribution.ARMiscelaneousCollectionDistributionID,
                        AccountCode = aRMiscelaneousCollectionDistribution.AccountCode,
                         AccountDesc = _accountHelper.GetAccountDesc(aRMiscelaneousCollectionDistribution.AccountCode),
                        AdditionalDescription = aRMiscelaneousCollectionDistribution.AdditionalDescription,
                        Debit = aRMiscelaneousCollectionDistribution.Debit,
                        Credit = aRMiscelaneousCollectionDistribution.Credit,
                        OrgBranchName = aRMiscelaneousCollectionDistribution.OrgBranchName,
                        DateCreated = aRMiscelaneousCollectionDistribution.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = aRMiscelaneousCollectionDistribution.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ARMiscelaneousCollectionDistributions.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "AR misc collection distribution", "Success", User.Identity.Name, $"updated misc collection distribution {entity.AccountCode} under record {entity.ARMiscelaneousCollectionID} ", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { aRMiscelaneousCollectionDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableCollection)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ARMiscelaneousCollectionDistributions_Destroy([DataSourceRequest]DataSourceRequest request, ARMiscelaneousCollectionDistribution aRMiscelaneousCollectionDistribution)
        {
             bool isvalid = true;
            if (ModelState.IsValid)
            {
                #region validated record

                ARMiscelaneousCollection parent =
                    _db.ARMiscelaneousCollections.FirstOrDefault(
                        x => x.ARMiscelaneousCollectionID == aRMiscelaneousCollectionDistribution.ARMiscelaneousCollectionID);
                if (parent != null && parent.IsOnlineApproved)
                {
                    isvalid = false;
                    //ViewBag.Note = "You cannot manipulate this record";
                    //return View("Error");
                }

                #endregion

                if (isvalid)
                {
                    var entity = new ARMiscelaneousCollectionDistribution
                    {
                        ARMiscelaneousCollectionDistributionID =
                            aRMiscelaneousCollectionDistribution.ARMiscelaneousCollectionDistributionID,
                        AccountCode = aRMiscelaneousCollectionDistribution.AccountCode,
                         AccountDesc = _accountHelper.GetAccountDesc(aRMiscelaneousCollectionDistribution.AccountCode),
                        AdditionalDescription = aRMiscelaneousCollectionDistribution.AdditionalDescription,
                        Debit = aRMiscelaneousCollectionDistribution.Debit,
                        Credit = aRMiscelaneousCollectionDistribution.Credit,
                        OrgBranchName = aRMiscelaneousCollectionDistribution.OrgBranchName,
                        DateCreated = aRMiscelaneousCollectionDistribution.DateCreated,
                        LastModified = aRMiscelaneousCollectionDistribution.LastModified,
                        CreatedBy = aRMiscelaneousCollectionDistribution.CreatedBy,
                        ModifiedBy = aRMiscelaneousCollectionDistribution.ModifiedBy
                    };

                    _db.ARMiscelaneousCollectionDistributions.Attach(entity);
                    _db.ARMiscelaneousCollectionDistributions.Remove(entity);
                    _db.SaveChanges();
                }
            }

            return Json(new[] { aRMiscelaneousCollectionDistribution }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView  , FinanceConstants.AccountReceivableApprover ,FinanceConstants.AccountReceivableCollection)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.AccountReceivableView  , FinanceConstants.AccountReceivableApprover ,FinanceConstants.AccountReceivableCollection)]
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
