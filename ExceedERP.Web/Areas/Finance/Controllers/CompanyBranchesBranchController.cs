using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;

namespace ExceedERP.Web.Areas.Finance.Controllers
{

    public class CompanyBranchesController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        public ActionResult Index()
        {
            var types = _db.Lookups.Where(x => x.LookupType == LookUpTypes.PROJECT_TYPE);
            ViewData["types"] = types;

            var subAccountSeg = _db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
            var subAccounts = _db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == subAccountSeg.GLSegmentSetupId);
            var branches = subAccounts;

            ViewData["subaccounts"] = branches;
            return View();
        }
    
          [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        public ActionResult Branch_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Branch> branchs = _db.Branch;
            DataSourceResult result = branchs.ToDataSourceResult(request, branch => new
            {
                BranchId = branch.BranchId,
                Name = branch.Name,
                Remark = branch.Remark,
                DateCreated = branch.DateCreated,
                LastModified = branch.LastModified,
                CreatedBy = branch.CreatedBy,
                ModifiedBy = branch.ModifiedBy,
                AmharicName = branch.AmharicName,
                SubAccount = branch.SubAccount,
                Code = branch.Code,
                BranchTypes = branch.BranchTypes,
                Abbreviation = branch.Abbreviation,
               
            });

            return Json(result);
        }


       [AuthorizeRoles(FinanceConstants.GeneralLedgerAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Branch_Update([DataSourceRequest]DataSourceRequest request, Branch branch)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new Branch
                {
                    BranchId = branch.BranchId,
                    Name = branch.Name,
                    Remark = branch.Remark,
                    DateCreated = branch.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = branch.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    AmharicName = branch.AmharicName,
                    Code = branch.Code,
                    BranchTypes = branch.BranchTypes,
                    Abbreviation = branch.Abbreviation,
                    SubAccount = branch.SubAccount,
                    

                };

                _db.Branch.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { branch }.ToDataSourceResult(request, ModelState));
        }
      
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
