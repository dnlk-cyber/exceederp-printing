
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = HR_Admin + "," + HR_Benefit_User + "," + HR_Benefit_Admin)]
    public class AllowanceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Benefit_Admin = "HR_Benefit_Admin";
        private const string HR_Benefit_User = "HR_Benefit_User";
        private const string HR_Admin = "HR_Admin";
        HRMInputs inputs = new HRMInputs();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
        {

            ViewData["branches"] = CacheData.GetBranchCaches().Select(
              i => new
              {
                  Name = i.Name,
                  BranchId = i.Id,
              }).AsQueryable();

            ViewData["jobTitles"] = CacheData.GetJobTitlesCaches().Select(
             i => new
             {
                 Description = i.Name,
                 LookupId = i.Id,
             }).AsQueryable();
            ViewData["RankList"] = CacheData.GetRankCaches().Select(
               i => new
               {
                   RankName = i.Name,
                   RankId = i.Id,
               }).AsQueryable();
            return View();
        }
           [Authorize(Roles = HR_Admin + "," + HR_Benefit_Admin + "," + HR_Benefit_User)]
        public ActionResult AllowanceDefinations_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<AllowanceDefination> allowancedefinations = db.AllowanceDefinations;
            DataSourceResult result = allowancedefinations.ToDataSourceResult(request, allowanceDefination => new {
                AllowanceDefinationId = allowanceDefination.AllowanceDefinationId,
                AllowanceName = allowanceDefination.AllowanceName,
                Amount = allowanceDefination.Amount,
                AllowanceType = allowanceDefination.AllowanceType,
                Category = allowanceDefination.Category,
                Remark = allowanceDefination.Remark,
                DateCreated = allowanceDefination.DateCreated,
                LastModified = allowanceDefination.LastModified,
                CreatedBy = allowanceDefination.CreatedBy,
                ModifiedBy = allowanceDefination.ModifiedBy,
                AmtPercent = allowanceDefination.AmtPercent
            });

            return Json(result);
        }

        [Authorize(Roles = HR_Benefit_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceDefinations_Create([DataSourceRequest]DataSourceRequest request, AllowanceDefination allowanceDefination)
        {
            ModelState.Clear();
            var checkExistance = db.AllowanceDefinations.Where(x => x.AllowanceType == allowanceDefination.AllowanceType && x.AllowanceName == allowanceDefination.AllowanceName).FirstOrDefault();
            if (checkExistance != null)
            {
                ModelState.AddModelError("", "This Definition is already exist!!");
                return Json(new[] { allowanceDefination }.ToDataSourceResult(request, ModelState));

            }
            else if (allowanceDefination.Amount <= 0)
            {
                ModelState.AddModelError("", "The amount should be greater  than 0!!");
                return Json(new[] { allowanceDefination }.ToDataSourceResult(request, ModelState));


            }

            if (ModelState.IsValid)
            {
              
                var entity = new AllowanceDefination
                {
                    AllowanceName = allowanceDefination.AllowanceName,
                    AllowanceType = allowanceDefination.AllowanceType,
                    Amount = allowanceDefination.Amount,
                    Remark = allowanceDefination.Remark,
                    Category = allowanceDefination.Category,
                    DateCreated = DateTime.Now,
                    LastModified = allowanceDefination.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = allowanceDefination.ModifiedBy,
                    AmtPercent = allowanceDefination.AmtPercent
                };

                db.AllowanceDefinations.Add(entity);
                db.SaveChanges();
                allowanceDefination.AllowanceDefinationId = entity.AllowanceDefinationId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance", "Success", User.Identity.Name, $"created new allowance name {entity.AllowanceName} and type {entity.AllowanceType} amount {entity.Amount} ", Modules.Payroll);
                #endregion
                
            }

            return Json(new[] { allowanceDefination }.ToDataSourceResult(request, ModelState));
        }

        [Authorize(Roles =  HR_Benefit_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceDefinations_Update([DataSourceRequest]DataSourceRequest request, AllowanceDefination allowanceDefination)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                
                    var entity = new AllowanceDefination
                    {
                        AllowanceDefinationId = allowanceDefination.AllowanceDefinationId,
                        AllowanceName = allowanceDefination.AllowanceName,
                        AllowanceType = allowanceDefination.AllowanceType,
                        Amount = allowanceDefination.Amount,
                        Category = allowanceDefination.Category,
                        Remark = allowanceDefination.Remark,
                        DateCreated = allowanceDefination.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = allowanceDefination.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        AmtPercent = allowanceDefination.AmtPercent
                    };

                    db.AllowanceDefinations.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance", "Success", User.Identity.Name, $"updated allowance name {entity.AllowanceName} and type {entity.AllowanceType} amount {entity.Amount} ", Modules.Payroll);
                #endregion

             

            }

            return Json(new[] { allowanceDefination }.ToDataSourceResult(request, ModelState));
        }
           [Authorize(Roles = HR_Admin + "," + HR_Benefit_Admin )]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceDefinations_Destroy([DataSourceRequest]DataSourceRequest request, AllowanceDefination allowanceDefination)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new AllowanceDefination
                {
                    AllowanceDefinationId = allowanceDefination.AllowanceDefinationId,
                    AllowanceName = allowanceDefination.AllowanceName,
                    AllowanceType = allowanceDefination.AllowanceType,
                    Amount = allowanceDefination.Amount,
                    Remark = allowanceDefination.Remark,
                    DateCreated = allowanceDefination.DateCreated,
                    LastModified = allowanceDefination.LastModified,
                    CreatedBy = allowanceDefination.CreatedBy,
                    ModifiedBy = allowanceDefination.ModifiedBy,
                    AmtPercent = allowanceDefination.AmtPercent
                };
                   
                db.AllowanceDefinations.Attach(entity);
                db.AllowanceDefinations.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance", "Success", User.Identity.Name, $"deleted allowance name {allowanceDefination.AllowanceName} and type {allowanceDefination.AllowanceType} amount {allowanceDefination.Amount} ", Modules.Payroll);
                #endregion
                
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
