using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.GL
{
    public class GlChartOfAccountSubAccountController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        
       

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        public ActionResult Index()
        {
            
            var catagories = _db.GLChartOfAccountSubAccounts.Where(x => x.Parent == true);

            ViewData["parents"] = catagories.Select(e => new 
            {
                HasParent = e.Values + "-" + e.Description
            }).AsQueryable();
            return View();
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        public ActionResult GLChartOfAccountSubAccounts_Read([DataSourceRequest]DataSourceRequest request, string hasParent)
        {
            IQueryable<GLChartOfAccountSubAccount> glchartofaccountsubaccounts = _db.GLChartOfAccountSubAccounts;
            DataSourceResult result = glchartofaccountsubaccounts.ToDataSourceResult(request, gLChartOfAccountSubAccount => new {
                
                Values = gLChartOfAccountSubAccount.Values,
                Description = gLChartOfAccountSubAccount.Description,
                HasParent = hasParent,
                Enable = gLChartOfAccountSubAccount.Enable,
                Parent = gLChartOfAccountSubAccount.Parent,
                AllowBudgeting = gLChartOfAccountSubAccount.AllowBudgeting,
                AllowPosting = gLChartOfAccountSubAccount.AllowPosting,
                
                OrgBranchName = gLChartOfAccountSubAccount.OrgBranchName,
                DateCreated = gLChartOfAccountSubAccount.DateCreated,
                LastModified = gLChartOfAccountSubAccount.LastModified,
                CreatedBy = gLChartOfAccountSubAccount.CreatedBy,
                ModifiedBy = gLChartOfAccountSubAccount.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLChartOfAccountSubAccounts_Create([DataSourceRequest]DataSourceRequest request, GLChartOfAccountSubAccount gLChartOfAccountSubAccount)
        {
            #region validate length

            int length = AccountHelper.GetSegmentLength(GlSegmentTypes.SubAccount);
            bool valid = (gLChartOfAccountSubAccount.Values.Length == length);

            if (!valid)
            {
                ModelState.AddModelError("", "Segment length should be " + length);
            }
            #endregion
            if (ModelState.IsValid)
            {
               
                var entity = new GLChartOfAccountSubAccount
                {
                    Values = gLChartOfAccountSubAccount.Values,
                    Description = gLChartOfAccountSubAccount.Description,
                    HasParent = gLChartOfAccountSubAccount.HasParent,
                    Enable = gLChartOfAccountSubAccount.Enable,
                    Parent = gLChartOfAccountSubAccount.Parent,
                    AllowBudgeting = gLChartOfAccountSubAccount.AllowBudgeting,
                    AllowPosting = gLChartOfAccountSubAccount.AllowPosting,
                    
                    OrgBranchName = gLChartOfAccountSubAccount.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = gLChartOfAccountSubAccount.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = gLChartOfAccountSubAccount.ModifiedBy
                };
                _db.GLChartOfAccountSubAccounts.Add(entity);
                _db.SaveChanges();
                gLChartOfAccountSubAccount.Values = entity.Values;
                CacheHelper.ResetCache();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL chart of account sub account", "Success", User.Identity.Name, $"created chart of account sub account {entity.Values}  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLChartOfAccountSubAccount }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLChartOfAccountSubAccounts_Update([DataSourceRequest]DataSourceRequest request, GLChartOfAccountSubAccount gLChartOfAccountSubAccount)
        {
            #region validate length

            int length = AccountHelper.GetSegmentLength(GlSegmentTypes.SubAccount);
            bool valid = (gLChartOfAccountSubAccount.Values.Length == length);

            if (!valid)
            {
                ModelState.AddModelError("", "Segment length should be " + length);
            }
            #endregion
            if (ModelState.IsValid)
            {
                
                var entity = new GLChartOfAccountSubAccount
                {
                    
                    Values = gLChartOfAccountSubAccount.Values,
                    Description = gLChartOfAccountSubAccount.Description,
                    HasParent = gLChartOfAccountSubAccount.HasParent,
                    Enable = gLChartOfAccountSubAccount.Enable,
                    Parent = gLChartOfAccountSubAccount.Parent,
                    AllowBudgeting = gLChartOfAccountSubAccount.AllowBudgeting,
                    AllowPosting = gLChartOfAccountSubAccount.AllowPosting,
                    OrgBranchName = gLChartOfAccountSubAccount.OrgBranchName,
                    DateCreated = gLChartOfAccountSubAccount.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = gLChartOfAccountSubAccount.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                _db.GLChartOfAccountSubAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                CacheHelper.ResetCache();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL chart of account sub account", "Success", User.Identity.Name, $"updated chart of account sub account {entity.Values}  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLChartOfAccountSubAccount }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLChartOfAccountSubAccounts_Destroy([DataSourceRequest]DataSourceRequest request, GLChartOfAccountSubAccount gLChartOfAccountSubAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLChartOfAccountSubAccount
                {
                    
                    Values = gLChartOfAccountSubAccount.Values,
                    Description = gLChartOfAccountSubAccount.Description,
                    HasParent = gLChartOfAccountSubAccount.HasParent,
                    Enable = gLChartOfAccountSubAccount.Enable,
                    Parent = gLChartOfAccountSubAccount.Parent,
                    AllowBudgeting = gLChartOfAccountSubAccount.AllowBudgeting,
                    AllowPosting = gLChartOfAccountSubAccount.AllowPosting,
                    OrgBranchName = gLChartOfAccountSubAccount.OrgBranchName,
                    DateCreated = gLChartOfAccountSubAccount.DateCreated,
                    LastModified = gLChartOfAccountSubAccount.LastModified,
                    CreatedBy = gLChartOfAccountSubAccount.CreatedBy,
                    ModifiedBy = gLChartOfAccountSubAccount.ModifiedBy
                };

                _db.GLChartOfAccountSubAccounts.Attach(entity);
                _db.GLChartOfAccountSubAccounts.Remove(entity);
                _db.SaveChanges();
                CacheHelper.ResetCache();
            }

            return Json(new[] { gLChartOfAccountSubAccount }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

         [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        public ActionResult Enable_Disable(string id)
        {
            if (id != null)
            {
                var account = _db.GLChartOfAccountSubAccounts.FirstOrDefault(p => p.Values == id);
                if (account != null && account.Enable)
                {
                    account.Enable = false;
                }
                else
                {
                    if (account != null) account.Enable = true;
                }

                if (account != null)
                {
                    _db.GLChartOfAccountSubAccounts.Attach(account);
                    _db.Entry(account).State = EntityState.Modified;
                }

                _db.SaveChanges();
                CacheHelper.ResetCache();
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
