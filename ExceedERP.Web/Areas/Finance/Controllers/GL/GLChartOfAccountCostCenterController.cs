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
    public class GlChartOfAccountCostCenterController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        
     

       [AuthorizeRoles( FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        public ActionResult Index()
        {
           
            var catagories = _db.GLChartOfAccountCostCenters.Where(x => x.Parent == true);

            ViewData["parents"] = catagories.Select(e => new 
            {
                HasParent = e.Values + "-" + e.Description
            }).AsQueryable();
            return View();
        }

        [AuthorizeRoles( FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        public ActionResult GLChartOfAccountCostCenters_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLChartOfAccountCostCenter> glchartofaccountcostcenters = _db.GLChartOfAccountCostCenters;
            DataSourceResult result = glchartofaccountcostcenters.ToDataSourceResult(request, gLChartOfAccountCostCenter => new {
                Values = gLChartOfAccountCostCenter.Values,
                Description = gLChartOfAccountCostCenter.Description,
                HasParent = gLChartOfAccountCostCenter.HasParent,
                Enable = gLChartOfAccountCostCenter.Enable,
                Parent = gLChartOfAccountCostCenter.Parent,
                AllowBudgeting = gLChartOfAccountCostCenter.AllowBudgeting,
                AllowPosting = gLChartOfAccountCostCenter.AllowPosting,
                Type = gLChartOfAccountCostCenter.Type,
                OrgBranchName = gLChartOfAccountCostCenter.OrgBranchName,
                DateCreated = gLChartOfAccountCostCenter.DateCreated,
                LastModified = gLChartOfAccountCostCenter.LastModified,
                CreatedBy = gLChartOfAccountCostCenter.CreatedBy,
                ModifiedBy = gLChartOfAccountCostCenter.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles( FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLChartOfAccountCostCenters_Create([DataSourceRequest]DataSourceRequest request, GLChartOfAccountCostCenter gLChartOfAccountCostCenter)
        {
            #region validate length

            int length = AccountHelper.GetSegmentLength(GlSegmentTypes.CostCenter);
            bool valid = (gLChartOfAccountCostCenter.Values.Length == length);

            if (!valid)
            {
                ModelState.AddModelError("", "Segment length should be " + length);
            }
            #endregion
            if (ModelState.IsValid)
            {
               
                var entity = new GLChartOfAccountCostCenter
                {
                    Values = gLChartOfAccountCostCenter.Values,
                    Description = gLChartOfAccountCostCenter.Description,
                    HasParent = gLChartOfAccountCostCenter.HasParent,
                    Enable = gLChartOfAccountCostCenter.Enable,
                    Parent = gLChartOfAccountCostCenter.Parent,
                    AllowBudgeting = gLChartOfAccountCostCenter.AllowBudgeting,
                    AllowPosting = gLChartOfAccountCostCenter.AllowPosting,
                    Type = gLChartOfAccountCostCenter.Type,
                    OrgBranchName = gLChartOfAccountCostCenter.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = gLChartOfAccountCostCenter.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = gLChartOfAccountCostCenter.ModifiedBy
                };

                _db.GLChartOfAccountCostCenters.Add(entity);
                _db.SaveChanges();
                gLChartOfAccountCostCenter.Values = entity.Values;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL chart of account cost center", "Success", User.Identity.Name, $"created chart of account cost center {entity.Values}  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLChartOfAccountCostCenter }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles( FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLChartOfAccountCostCenters_Update([DataSourceRequest]DataSourceRequest request, GLChartOfAccountCostCenter gLChartOfAccountCostCenter)
        {
            #region validate length

            int length = AccountHelper.GetSegmentLength(GlSegmentTypes.CostCenter);
            bool valid = (gLChartOfAccountCostCenter.Values.Length == length);

            if (!valid)
            {
                ModelState.AddModelError("", "Segment length should be " + length);
            }
            #endregion
            if (ModelState.IsValid)
            {
               
                var entity = new GLChartOfAccountCostCenter
                {
                    Values = gLChartOfAccountCostCenter.Values,
                    Description = gLChartOfAccountCostCenter.Description,
                    HasParent = gLChartOfAccountCostCenter.HasParent,
                    Enable = gLChartOfAccountCostCenter.Enable,
                    Parent = gLChartOfAccountCostCenter.Parent,
                    AllowBudgeting = gLChartOfAccountCostCenter.AllowBudgeting,
                    AllowPosting = gLChartOfAccountCostCenter.AllowPosting,
                    Type = gLChartOfAccountCostCenter.Type,
                    OrgBranchName = gLChartOfAccountCostCenter.OrgBranchName,
                    DateCreated = gLChartOfAccountCostCenter.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = gLChartOfAccountCostCenter.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                _db.GLChartOfAccountCostCenters.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL chart of account cost center", "Success", User.Identity.Name, $"updated chart of account cost center {entity.Values}  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLChartOfAccountCostCenter }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles( FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLChartOfAccountCostCenters_Destroy([DataSourceRequest]DataSourceRequest request, GLChartOfAccountCostCenter gLChartOfAccountCostCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLChartOfAccountCostCenter
                {
                    Values = gLChartOfAccountCostCenter.Values,
                    Description = gLChartOfAccountCostCenter.Description,
                    HasParent = gLChartOfAccountCostCenter.HasParent,
                    Enable = gLChartOfAccountCostCenter.Enable,
                    Parent = gLChartOfAccountCostCenter.Parent,
                    AllowBudgeting = gLChartOfAccountCostCenter.AllowBudgeting,
                    AllowPosting = gLChartOfAccountCostCenter.AllowPosting,
                    Type = gLChartOfAccountCostCenter.Type,
                    OrgBranchName = gLChartOfAccountCostCenter.OrgBranchName,
                    DateCreated = gLChartOfAccountCostCenter.DateCreated,
                    LastModified = gLChartOfAccountCostCenter.LastModified,
                    CreatedBy = gLChartOfAccountCostCenter.CreatedBy,
                    ModifiedBy = gLChartOfAccountCostCenter.ModifiedBy
                };

                _db.GLChartOfAccountCostCenters.Attach(entity);
                _db.GLChartOfAccountCostCenters.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { gLChartOfAccountCostCenter }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles( FinanceConstants.GeneralLedgerAdmin)]
        public ActionResult Enable_Disable(string id)
        {
            if (id != null)
            {
                var account = _db.GLChartOfAccountCostCenters.FirstOrDefault(p => p.Values == id);
                if (account != null && account.Enable)
                {
                    account.Enable = false;
                }
                else
                {
                    if (account != null) account.Enable = true;
                }
                _db.GLChartOfAccountCostCenters.Attach(account);
                _db.Entry(account).State = EntityState.Modified;
                _db.SaveChanges();
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
