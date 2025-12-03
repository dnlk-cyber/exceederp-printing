using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM.Payroll.BackPays;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    public class BackPaymentLineController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
       
      
      
        public ActionResult BackPaymentLines_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
          
            IQueryable<BackPaymentLine> BackPaymentLines = db.BackPaymentLines.Where(x => x.BackPayId==id);
            DataSourceResult result = BackPaymentLines.ToDataSourceResult(request, BackPaymentLine => new {
                BackPaymentLineId = BackPaymentLine.BackPaymentLineId,
                BackPayId = BackPaymentLine.BackPayId,
                EmployeeId = BackPaymentLine.EmployeeId,
                BasicSalary = BackPaymentLine.BasicSalary,
                IncomeTax = BackPaymentLine.IncomeTax,
                Pension = BackPaymentLine.Pension,
                Allowances = BackPaymentLine.Allowances,
                Net = BackPaymentLine.Net,
               
                DateCreated = BackPaymentLine.DateCreated,
                LastModified = BackPaymentLine.LastModified,
                CreatedBy = BackPaymentLine.CreatedBy,
                OrgBranchName = BackPaymentLine.OrgBranchName,
                ModifiedBy = BackPaymentLine.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BackPaymentLines_Create([DataSourceRequest]DataSourceRequest request, BackPaymentLine BackPaymentLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new BackPaymentLine
                {
                    BasicSalary = BackPaymentLine.BasicSalary,
                    BackPayId = BackPaymentLine.BackPayId,
                    EmployeeId = BackPaymentLine.EmployeeId,
                    IncomeTax = BackPaymentLine.IncomeTax,
                    Pension = BackPaymentLine.Pension,
                    Allowances = BackPaymentLine.Allowances,
                    Net = BackPaymentLine.Net,
                  
                    DateCreated = DateTime.Now,
                    LastModified = BackPaymentLine.LastModified,
                    CreatedBy = User.Identity.Name,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    ModifiedBy = BackPaymentLine.ModifiedBy
                };

                db.BackPaymentLines.Add(entity);
                db.SaveChanges();
                BackPaymentLine.BackPaymentLineId = entity.BackPaymentLineId;
            }

            return Json(new[] { BackPaymentLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BackPaymentLines_Update([DataSourceRequest]DataSourceRequest request, BackPaymentLine BackPaymentLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new BackPaymentLine
                {
                    BackPaymentLineId = BackPaymentLine.BackPaymentLineId,
                    BackPayId = BackPaymentLine.BackPayId,
                    EmployeeId = BackPaymentLine.EmployeeId,
                    BasicSalary = BackPaymentLine.BasicSalary,
                    IncomeTax = BackPaymentLine.IncomeTax,
                    Pension = BackPaymentLine.Pension,
                    Allowances = BackPaymentLine.Allowances,
                    Net = BackPaymentLine.Net,
                   
                    DateCreated = BackPaymentLine.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = BackPaymentLine.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = BackPaymentLine.OrgBranchName,
                };

                db.BackPaymentLines.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { BackPaymentLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BackPaymentLines_Destroy([DataSourceRequest]DataSourceRequest request, BackPaymentLine BackPaymentLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new BackPaymentLine
                {
                    BackPaymentLineId = BackPaymentLine.BackPaymentLineId,
                    BackPayId = BackPaymentLine.BackPayId,
                    EmployeeId = BackPaymentLine.EmployeeId,
                    BasicSalary = BackPaymentLine.BasicSalary,
                    IncomeTax = BackPaymentLine.IncomeTax,
                    Pension = BackPaymentLine.Pension,
                    Allowances = BackPaymentLine.Allowances,
                    Net = BackPaymentLine.Net,
                   
                    DateCreated = BackPaymentLine.DateCreated,
                    LastModified = BackPaymentLine.LastModified,
                    CreatedBy = BackPaymentLine.CreatedBy,
                    OrgBranchName = BackPaymentLine.OrgBranchName,
                    ModifiedBy = BackPaymentLine.ModifiedBy
                };
                #region delete log
               // UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee loan information of employee " + loan.EmployeeId + " and period " + loan.BackPayId);
                #endregion
                db.BackPaymentLines.Attach(entity);
                db.BackPaymentLines.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { BackPaymentLine }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
