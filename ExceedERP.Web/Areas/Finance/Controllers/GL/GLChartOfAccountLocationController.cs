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
    public class GlChartOfAccountLocationController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
     

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        public ActionResult Index()
        {
           
            var catagories = _db.GLChartOfAccountLocations.Where(x => x.Parent == true);

            ViewData["parents"] = catagories.Select(e => new 
            {
                HasParent = e.Values + "-" + e.Description
            }).AsQueryable();
            return View();
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        public ActionResult GLChartOfAccountLocations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<GLChartOfAccountLocation> glchartofaccountlocations = _db.GLChartOfAccountLocations;
            DataSourceResult result = glchartofaccountlocations.ToDataSourceResult(request, gLChartOfAccountLocation => new {
                
                Values = gLChartOfAccountLocation.Values,
                Description = gLChartOfAccountLocation.Description,
                HasParent = gLChartOfAccountLocation.HasParent,
                Enable = gLChartOfAccountLocation.Enable,
                Parent = gLChartOfAccountLocation.Parent,
                AllowBudgeting = gLChartOfAccountLocation.AllowBudgeting,
                AllowPosting = gLChartOfAccountLocation.AllowPosting,
                OrgBranchName = gLChartOfAccountLocation.OrgBranchName,
                DateCreated = gLChartOfAccountLocation.DateCreated,
                LastModified = gLChartOfAccountLocation.LastModified,
                CreatedBy = gLChartOfAccountLocation.CreatedBy,
                ModifiedBy = gLChartOfAccountLocation.ModifiedBy
            });

            return Json(result);
        }

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLChartOfAccountLocations_Create([DataSourceRequest]DataSourceRequest request, GLChartOfAccountLocation gLChartOfAccountLocation, string hasParent)
        {
            #region validate length

            int length = AccountHelper.GetSegmentLength(GlSegmentTypes.Location);
            bool valid = (gLChartOfAccountLocation.Values.Length == length);

            if (!valid)
            {
                ModelState.AddModelError("", "Segment length should be " + length);
            }
            #endregion
            if (ModelState.IsValid)
            {
               
                var entity = new GLChartOfAccountLocation
                {
                    Values = gLChartOfAccountLocation.Values,
                    Description = gLChartOfAccountLocation.Description,
                    HasParent = hasParent,
                    Enable = gLChartOfAccountLocation.Enable,
                    Parent = gLChartOfAccountLocation.Parent,
                    AllowBudgeting = gLChartOfAccountLocation.AllowBudgeting,
                    AllowPosting = gLChartOfAccountLocation.AllowPosting,
                    OrgBranchName = gLChartOfAccountLocation.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = gLChartOfAccountLocation.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = gLChartOfAccountLocation.ModifiedBy
                };
                _db.GLChartOfAccountLocations.Add(entity);
                _db.SaveChanges();
                gLChartOfAccountLocation.Values = entity.Values;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL chart of account location/branch", "Success", User.Identity.Name, $"created chart of account location {entity.Values}  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLChartOfAccountLocation }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLChartOfAccountLocations_Update([DataSourceRequest]DataSourceRequest request, GLChartOfAccountLocation gLChartOfAccountLocation)
        {
            #region validate length

            int length = AccountHelper.GetSegmentLength(GlSegmentTypes.Location);
            bool valid = (gLChartOfAccountLocation.Values.Length == length);

            if (!valid)
            {
                ModelState.AddModelError("", "Segment length should be " + length);
            }
            #endregion
            if (ModelState.IsValid)
            {

               
                var entity = new GLChartOfAccountLocation
                {
                    Values = gLChartOfAccountLocation.Values,
                    Description = gLChartOfAccountLocation.Description,
                    HasParent = gLChartOfAccountLocation.HasParent,
                    Enable = gLChartOfAccountLocation.Enable,
                    Parent = gLChartOfAccountLocation.Parent,
                    AllowBudgeting = gLChartOfAccountLocation.AllowBudgeting,
                    AllowPosting = gLChartOfAccountLocation.AllowPosting,
                    OrgBranchName = gLChartOfAccountLocation.OrgBranchName,
                    DateCreated = gLChartOfAccountLocation.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = gLChartOfAccountLocation.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                _db.GLChartOfAccountLocations.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "GL chart of account location/branch", "Success", User.Identity.Name, $"updated chart of account location {entity.Values}  {entity.Description}", Modules.Finance);
                #endregion
            }

            return Json(new[] { gLChartOfAccountLocation }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GLChartOfAccountLocations_Destroy([DataSourceRequest]DataSourceRequest request, GLChartOfAccountLocation gLChartOfAccountLocation)
        {
            if (ModelState.IsValid)
            {
                var entity = new GLChartOfAccountLocation
                {
                    Values = gLChartOfAccountLocation.Values,
                    Description = gLChartOfAccountLocation.Description,
                    HasParent = gLChartOfAccountLocation.HasParent,
                    Enable = gLChartOfAccountLocation.Enable,
                    Parent = gLChartOfAccountLocation.Parent,
                    AllowBudgeting = gLChartOfAccountLocation.AllowBudgeting,
                    AllowPosting = gLChartOfAccountLocation.AllowPosting,
                    OrgBranchName = gLChartOfAccountLocation.OrgBranchName,
                    DateCreated = gLChartOfAccountLocation.DateCreated,
                    LastModified = gLChartOfAccountLocation.LastModified,
                    CreatedBy = gLChartOfAccountLocation.CreatedBy,
                    ModifiedBy = gLChartOfAccountLocation.ModifiedBy
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress,"Delete","Success",User.Identity.Name,"Deleted chart of account location code"+gLChartOfAccountLocation.Values);
                #endregion
                _db.GLChartOfAccountLocations.Attach(entity);
                _db.GLChartOfAccountLocations.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { gLChartOfAccountLocation }.ToDataSourceResult(request, ModelState));
        }

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported chart of account location");
            #endregion
            return File(fileContents, contentType, fileName);
        }

       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin , FinanceConstants.GeneralLedgerView)]
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported chart of account location");
            #endregion
            return File(fileContents, contentType, fileName);
        }

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        public ActionResult Enable_Disable(string id)
        {
            if (id != null)
            {
                var account = _db.GLChartOfAccountLocations.FirstOrDefault(p => p.Values == id);
                if (account != null && account.Enable)
                {
                    account.Enable = false;
                }
                else
                {
                    if (account != null) account.Enable = true;
                }
                _db.GLChartOfAccountLocations.Attach(account);
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
