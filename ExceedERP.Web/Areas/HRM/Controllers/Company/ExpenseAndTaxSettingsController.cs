using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    public class ExpenseAndTaxSettingsController : BaseController
    {

        ExceedDbContext db = new ExceedDbContext();

   

        public ActionResult ExpenseAndTaxSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
           
           
            IQueryable<ExpenseAndTaxSetting> expenseAndTaxSettings = db.ExpenseAndTaxSettings;

            DataSourceResult result = expenseAndTaxSettings.ToDataSourceResult(request, expenseAndTaxSetting => new
            {
                ExpenseAndTaxSettingId = expenseAndTaxSetting.ExpenseAndTaxSettingId,
                ExpenseItemType = expenseAndTaxSetting.ExpenseItemType,
                ApSetupItemId = expenseAndTaxSetting.ApSetupItemId,
                Description = expenseAndTaxSetting.Description,
                DateCreated = expenseAndTaxSetting.DateCreated,
                CreatedBy = expenseAndTaxSetting.CreatedBy,
                LastModified = expenseAndTaxSetting.LastModified,
                ModifiedBy = expenseAndTaxSetting.ModifiedBy
            });

            return Json(result);
        }

       

        public JsonResult ExpenseAndTaxSettings_Create([DataSourceRequest] DataSourceRequest request, ExpenseAndTaxSetting expenseAndTaxSetting)
        {
            var exist = db.ExpenseAndTaxSettings.Where(x=>x.ExpenseItemType ==expenseAndTaxSetting.ExpenseItemType).FirstOrDefault();
            if (exist !=null)
            {
                ModelState.AddModelError("exist", " already you have  Setted One !  you can modify it");
                return Json(new[] { expenseAndTaxSetting }.ToDataSourceResult(request));

            }
            expenseAndTaxSetting.DateCreated = DateTime.Now;
            expenseAndTaxSetting.CreatedBy = User.Identity.Name;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(expenseAndTaxSetting);
                db.ExpenseAndTaxSettings.Add(entity);
                db.SaveChanges();

                #region operation log                
              
                    UserHelper.OperationLog(Request.UserHostAddress, " Expense And Tax Setting ", "Success", User.Identity.Name, User.Identity.Name + " add  benefit setting for  "+ expenseAndTaxSetting.ApSetupItemId + " at date " + DateTime.Today, Modules.HRM);
                #endregion
            }

            return Json(new[] { expenseAndTaxSetting }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ExpenseAndTaxSettings_Update([DataSourceRequest] DataSourceRequest request, ExpenseAndTaxSetting expenseAndTaxSetting)
        {
            var exist = db.ExpenseAndTaxSettings.Where(x => x.ExpenseItemType == expenseAndTaxSetting.ExpenseItemType
                                                            &&x.ExpenseAndTaxSettingId !=expenseAndTaxSetting.ExpenseAndTaxSettingId)
                                                            .FirstOrDefault();

            if (exist != null)
            {
                ModelState.AddModelError("exist", " already you have  Setted One !  you can modify it");
                return Json(new[] { expenseAndTaxSetting }.ToDataSourceResult(request));
            }

            expenseAndTaxSetting.LastModified = DateTime.Now;
            expenseAndTaxSetting.ModifiedBy = User.Identity.Name;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(expenseAndTaxSetting);
                db.ExpenseAndTaxSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log                


                UserHelper.OperationLog(Request.UserHostAddress, "Expense And Tax Setting", "Success", User.Identity.Name, User.Identity.Name + " update expense And TaxSetting setting of  " + expenseAndTaxSetting.ApSetupItemId + " at date " + DateTime.Today, Modules.HRM);
                

                #endregion

            }

            return Json(new[] { expenseAndTaxSetting }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult ExpenseAndTaxSettings_Destroy([DataSourceRequest] DataSourceRequest request, ExpenseAndTaxSetting expenseAndTaxSetting)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(expenseAndTaxSetting);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted expense And TaxSetting" + expenseAndTaxSetting?.ApSetup?.Name);
                #endregion
                db.ExpenseAndTaxSettings.Attach(entity);
                db.ExpenseAndTaxSettings.Remove(entity);
                db.SaveChanges();

               
            }

            return Json(new[] { expenseAndTaxSetting }.ToDataSourceResult(request));
        }
        private ExpenseAndTaxSetting GetMappedObject(ExpenseAndTaxSetting expenseAndTaxSetting)
        {
            var entity = new ExpenseAndTaxSetting
            {
                ExpenseAndTaxSettingId = expenseAndTaxSetting.ExpenseAndTaxSettingId,
                ExpenseItemType = expenseAndTaxSetting.ExpenseItemType,
                ApSetupItemId = expenseAndTaxSetting.ApSetupItemId,
                Description = expenseAndTaxSetting.Description,
                DateCreated = expenseAndTaxSetting.DateCreated,
                CreatedBy = expenseAndTaxSetting.CreatedBy,
                LastModified = expenseAndTaxSetting.LastModified,
                ModifiedBy = expenseAndTaxSetting.ModifiedBy

            };
            return entity;

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}