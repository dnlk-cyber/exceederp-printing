using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = HR_Admin + "," + HR_Benefit_Admin + "," + HR_Benefit_User)]
    public class AllowanceTypeRuleController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string HR_Admin = "HR_Admin";
        private const string HR_Benefit_Admin = "HR_Benefit_Admin";
        private const string HR_Benefit_User = "HR_Benefit_User";
        public ActionResult Index()
        {
            return View();
        }
          [Authorize(Roles = HR_Admin + "," + HR_Benefit_Admin + "," + HR_Benefit_User)]
        public ActionResult AllowanceTypeRule_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AllowanceTypeRule> allowancetyperule = db.AllowanceTypeRule;
            if (!allowancetyperule.Any())
            {
                var allowanceType = Enum.GetValues(typeof (AllowanceType))
                    .Cast<AllowanceType>()
                    .ToList();
                foreach (var a in allowanceType)
                {
                 
                    db = new ExceedDbContext();
                  
                    var entity = new AllowanceTypeRule
                    {
                        AllowanceType = a,
                        TaxTrasholdAmount = 0,
                        Percentage = 0,
                        Remark = ""
                    };

                    db.AllowanceTypeRule.Add(entity);
                    db.SaveChanges();
                }

            }

            DataSourceResult result = allowancetyperule.ToDataSourceResult(request, allowanceTypeRule => new {
                AllowanceTypeRuleId = allowanceTypeRule.AllowanceTypeRuleId,
                AllowanceType = allowanceTypeRule.AllowanceType,
                TaxTrasholdAmount = allowanceTypeRule.TaxTrasholdAmount,
                Percentage = allowanceTypeRule.Percentage,
                Remark = allowanceTypeRule.Remark,
                DateCreated = allowanceTypeRule.DateCreated,
                LastModified = allowanceTypeRule.LastModified,
                CreatedBy = allowanceTypeRule.CreatedBy,
                ModifiedBy = allowanceTypeRule.ModifiedBy
            });

            return Json(result);
        }
          [Authorize(Roles = HR_Benefit_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceTypeRule_Create([DataSourceRequest]DataSourceRequest request, AllowanceTypeRule allowanceTypeRule)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new AllowanceTypeRule
                {
                    AllowanceType = allowanceTypeRule.AllowanceType,
                    TaxTrasholdAmount = allowanceTypeRule.TaxTrasholdAmount,
                    Percentage = allowanceTypeRule.Percentage,
                    Remark = allowanceTypeRule.Remark,
                    DateCreated = DateTime .Now ,
                    LastModified = allowanceTypeRule.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = allowanceTypeRule.ModifiedBy
                };
               
                db.AllowanceTypeRule.Add(entity);
                db.SaveChanges();
                allowanceTypeRule.AllowanceTypeRuleId = entity.AllowanceTypeRuleId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance rule", "Success", User.Identity.Name, $"added rule  {entity.AllowanceType} tax threshold {entity.TaxTrasholdAmount} percent amount {entity.Percentage}", Modules.Payroll);
                #endregion
            }

            return Json(new[] { allowanceTypeRule }.ToDataSourceResult(request, ModelState));
        }
          public bool GetSetting()
          {
              var configuration =

                    db.Configurations.Where(x => x.Reference == "BENEFIT_ZERO_THRESHOLD").FirstOrDefault();
              if (configuration != null)
              {
                  if (configuration.Attribute == "TAXABLE")
                  {
                      return true;
                  }
                  
              }
              return false;
          }
         public void SetSetting(bool value)
         {
             var configuration =

                   db.Configurations.Where(x => x.Reference == "BENEFIT_ZERO_THRESHOLD").FirstOrDefault();
             if (configuration != null)
             {
                 db.Configurations.Attach(configuration);
                 db.Configurations.Remove(configuration);
                 db.SaveChanges();
             }
             if (!value)
             {

                 var entityy = new Configuration
                 {
                     Reference = "BENEFIT_ZERO_THRESHOLD",
                     Attribute = "NON_TAXABLE",
                     CurrentValue = "0",
                     PreviousValue = null,
                     Category = "Payroll",
                     Remark = null
                 };
                 db = new ExceedDbContext();
                 db.Configurations.Add(entityy);
                 db.SaveChanges();
             }
             else
             {

                 var entityy = new Configuration
                 {
                     Reference = "BENEFIT_ZERO_THRESHOLD",
                     Attribute = "TAXABLE",
                     CurrentValue = "0",
                     PreviousValue = null,
                     Category = "Payroll",
                     Remark = null
                 };
                 db = new ExceedDbContext();
                 db.Configurations.Add(entityy);
                 db.SaveChanges();
             }
         }
          [Authorize(Roles =  HR_Benefit_User)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AllowanceTypeRule_Update([DataSourceRequest]DataSourceRequest request, AllowanceTypeRule allowanceTypeRule)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new AllowanceTypeRule
                {
                    AllowanceTypeRuleId = allowanceTypeRule.AllowanceTypeRuleId,
                    AllowanceType = allowanceTypeRule.AllowanceType,
                    TaxTrasholdAmount = allowanceTypeRule.TaxTrasholdAmount,
                    Percentage = allowanceTypeRule.Percentage,
                    Remark = allowanceTypeRule.Remark,
                    DateCreated = allowanceTypeRule.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = allowanceTypeRule.CreatedBy,
                    ModifiedBy =User .Identity .Name 
                };

                db.AllowanceTypeRule.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll allowance rule", "Success", User.Identity.Name, $"updated rule  {entity.AllowanceType} tax threshold {entity.TaxTrasholdAmount} percent amount {entity.Percentage}", Modules.Payroll);
                #endregion
            }

            return Json(new[] { allowanceTypeRule }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
