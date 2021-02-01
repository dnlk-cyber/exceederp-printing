
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.Common;
using System.Collections.Generic;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = HR_Benefit_User + "," + HR_Benefit_Admin)]
    public class AllowanceCourtController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Benefit_Admin = "HR_Benefit_Admin";
        private const string HR_Benefit_User = "HR_Benefit_User";
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            var lookups = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE);
            ViewData["jobTitles"] = lookups;
            var branches = db.Branch;
            ViewData["Branches"] = branches;
            return View();
        }

        public ActionResult AllowanceDefinations_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AllowanceDefination> allowancedefinations = db.AllowanceDefinations;
            DataSourceResult result = allowancedefinations.ToDataSourceResult(request, allowanceDefination => new {
                AllowanceDefinationId = allowanceDefination.AllowanceDefinationId,
                AllowanceName = allowanceDefination.AllowanceName,
                Amount = allowanceDefination.Amount,
                AllowanceType = allowanceDefination.AllowanceType,
                //LookupId = allowanceDefination.LookupId,
                Remark = allowanceDefination.Remark,
                DateCreated = allowanceDefination.DateCreated,
                LastModified = allowanceDefination.LastModified,
                CreatedBy = allowanceDefination.CreatedBy,
                ModifiedBy = allowanceDefination.ModifiedBy,
                AmtPercent = allowanceDefination.AmtPercent
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceDefinations_Create([DataSourceRequest]DataSourceRequest request, AllowanceDefination allowanceDefination)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
              
                var checkExistance = db.AllowanceDefinations.Where(x =>  x.AllowanceType == allowanceDefination.AllowanceType && x.AllowanceName == allowanceDefination.AllowanceName).FirstOrDefault();
               if (checkExistance != null)
                {
                    ModelState.AddModelError("", "This Definition is already exist!!");
                }
                else if (allowanceDefination.Amount<=0)
                {
                    ModelState.AddModelError("", "The amount should be greater  than 0!!");

                    }

                else
                {
                    var entity = new AllowanceDefination
                    {
                        AllowanceName = allowanceDefination.AllowanceName,
                        AllowanceType = allowanceDefination.AllowanceType,
                        Amount = allowanceDefination.Amount,
                        Remark = allowanceDefination.Remark,
                       // LookupId = allowanceDefination.LookupId,
                        DateCreated = DateTime.Now,
                        LastModified = allowanceDefination.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = allowanceDefination.ModifiedBy,
                        AmtPercent = allowanceDefination.AmtPercent
                    };

                    db.AllowanceDefinations.Add(entity);
                    db.SaveChanges();
                    allowanceDefination.AllowanceDefinationId = entity.AllowanceDefinationId;
                   
                }
            }

            return Json(new[] { allowanceDefination }.ToDataSourceResult(request, ModelState));
        }
        //public void PerapreEmployeeAllowance(int id=0)
        //{
        //    var alloanceList = db.AllowanceDefinations.Where (x=> x.LookupId == id).AsQueryable();
        //    var list = inputs.GetEmployees(User.Identity.Name).Where(x => x.IsActive);
        //    var placment = db.Placements.Where(x => x.isCurrent && x.LookupId==id).ToList();

        //    foreach (var l in list)
        //    {
        //        db = new ExceedDbContext();
        //        var checkExistance = db.AllowancePerEmployees.Where(g => g.EmployeeId == l.PersonId).ToList();
        //        if (checkExistance.Any())
        //        {
        //            db.AllowancePerEmployees.RemoveRange(checkExistance);
        //            db.SaveChanges();
        //        }
        //        foreach (var x in alloanceList)
        //        {

        //            var empJobtitle = placment.Where(y => y.EmployeeId == l.PersonId).FirstOrDefault();
        //            if (empJobtitle != null)
        //            {
        //                if (x.LookupId == empJobtitle.LookupId)
        //                {
        //                    var entity = new AllowancePerEmployee
        //                    {
        //                        EmployeeId = l.PersonId,
        //                        AllowanceDefinationId = x.AllowanceDefinationId,
        //                        Remark = null,
        //                        DateCreated = DateTime.Now,
        //                        LastModified = null,
        //                        CreatedBy = User.Identity.Name,
        //                        ModifiedBy = null,
        //                        EmployeeCode = l.EmployeeCode,
        //                        From = null,
        //                        To = null,
        //                        Pend = false,
        //                        OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
        //                    };

        //                    db.AllowancePerEmployees.Add(entity);
        //                    db.SaveChanges();
        //                    // allowancePerEmployee.AllowanceId = entity.AllowanceId;
        //                }
        //            }

        //        }
        //    }
          
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceDefinations_Update([DataSourceRequest]DataSourceRequest request, AllowanceDefination allowanceDefination)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                //var process = db.PayrollProcessAllowances.Where(x => x.AllowanceType == allowanceDefination.AllowanceType).FirstOrDefault();;
                //if (process != null)
                //{
                //    ModelState.AddModelError("", "You cannot upadate this allowance definition. It already processed!!");

                //}
                //else
                //{
                    var entity = new AllowanceDefination
                    {
                        AllowanceDefinationId = allowanceDefination.AllowanceDefinationId,
                        AllowanceName = allowanceDefination.AllowanceName,
                        AllowanceType = allowanceDefination.AllowanceType,
                        Amount = allowanceDefination.Amount,
                        Remark = allowanceDefination.Remark,
                     //   LookupId = allowanceDefination.LookupId,
                        DateCreated = allowanceDefination.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = allowanceDefination.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        AmtPercent = allowanceDefination.AmtPercent
                    };

                    db.AllowanceDefinations.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    
                //}
               
            }

            return Json(new[] { allowanceDefination }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceDefinations_Destroy([DataSourceRequest]DataSourceRequest request, AllowanceDefination allowanceDefination)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessAllowances.Where(x => x.AllowanceType == allowanceDefination.AllowanceType).FirstOrDefault();
                if (checkexistance != null)
                {
                    ModelState.AddModelError("", "You can not delete this type of allowance is already processed!!");
                }
                else
                {
                    var entity = new AllowanceDefination
                    {
                        AllowanceDefinationId = allowanceDefination.AllowanceDefinationId,
                        AllowanceName = allowanceDefination.AllowanceName,
                        AllowanceType = allowanceDefination.AllowanceType,
                        Amount = allowanceDefination.Amount,
                        Remark = allowanceDefination.Remark,
                       // LookupId = allowanceDefination.LookupId,
                        DateCreated = allowanceDefination.DateCreated,
                        LastModified = allowanceDefination.LastModified,
                        CreatedBy = allowanceDefination.CreatedBy,
                        ModifiedBy = allowanceDefination.ModifiedBy,
                        AmtPercent = allowanceDefination.AmtPercent
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted allowance information of type " + allowanceDefination.AllowanceName);
                    #endregion
                    db.AllowanceDefinations.Attach(entity);
                    db.AllowanceDefinations.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { allowanceDefination }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
