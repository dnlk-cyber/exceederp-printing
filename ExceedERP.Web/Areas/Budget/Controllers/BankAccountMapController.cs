using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    public class BankAccountMapController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        //
        // GET: /Budget/BankAccountMap/
        public ActionResult Index()
        {

            ViewData["Branches"] = db.Branch.ToList();
            return View();
        }

        public ActionResult BudgetBankAccountMap_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetBankAccountMap> BudgetBankAccountMap = db.BudgetBankAccountMaps;
            DataSourceResult result = BudgetBankAccountMap.ToDataSourceResult(request, budgetBankAccountMap => new BudgetBankAccountMap
            {
                BudgetBankAccountMapId = budgetBankAccountMap.BudgetBankAccountMapId,
                Name = budgetBankAccountMap.Name,
                AccountCode = budgetBankAccountMap.AccountCode,
                Address = budgetBankAccountMap.Address,
                BankNumber = budgetBankAccountMap.BankNumber,
                BranchId = budgetBankAccountMap.BranchId,
                IsHeadOffice = budgetBankAccountMap.IsHeadOffice,
                CreatedBy = budgetBankAccountMap.CreatedBy,
                DateCreated = budgetBankAccountMap.DateCreated,
                Remark = budgetBankAccountMap.Remark,


            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetBankAccountMap_Create([DataSourceRequest]DataSourceRequest request, BudgetBankAccountMap BudgetBankAccountMap)
        {

            if (!ModelState.IsValid)
            {
                return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));

            }
            if (string.IsNullOrWhiteSpace(BudgetBankAccountMap.AccountCode))
            {
                ModelState.AddModelError("AccCodeRequired", "   Account Code Required !");
                return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));


            }
            else if (db.BudgetBankAccountMaps.Any(x => x.Name == BudgetBankAccountMap.Name))
            {
                ModelState.AddModelError("SameName", "   Name Already Exists !");
                return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));


            }
            else if (db.BudgetBankAccountMaps.Any(x => x.BranchId == BudgetBankAccountMap.BranchId))
            {
                ModelState.AddModelError("SameBranch", "   Branch Already Exists !");
                return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));


            }
            else if (db.BudgetBankAccountMaps.Any(x => x.IsHeadOffice ) && BudgetBankAccountMap.IsHeadOffice)
            {

                ModelState.AddModelError("SameAccountCode", " Head Office  Account Already Exists !");
                return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));

            }

            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetBankAccountMap
                    {
                        BudgetBankAccountMapId = BudgetBankAccountMap.BudgetBankAccountMapId,
                        Name = BudgetBankAccountMap.Name,
                        AccountCode = BudgetBankAccountMap.AccountCode,
                        Address = BudgetBankAccountMap.Address,
                        BankNumber = BudgetBankAccountMap.BankNumber,
                        BranchId = BudgetBankAccountMap.BranchId,
                        IsHeadOffice = BudgetBankAccountMap.IsHeadOffice,
                        Remark = BudgetBankAccountMap.Remark,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.BudgetBankAccountMaps.Add(entity);
                    db.SaveChanges();



                    BudgetBankAccountMap.BudgetBankAccountMapId = entity.BudgetBankAccountMapId;
                }
                else
                {
                    return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));

                }
            }

            return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetBankAccountMap_Update([DataSourceRequest]DataSourceRequest request, BudgetBankAccountMap BudgetBankAccountMap)
        {


            if (string.IsNullOrWhiteSpace(BudgetBankAccountMap.AccountCode))
            {
                ModelState.AddModelError("AccCodeRequired", "   Account Code Required !");
                return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));


            }
            else  if (db.BudgetBankAccountMaps.Any(x => x.Name == BudgetBankAccountMap.Name && x.BudgetBankAccountMapId != BudgetBankAccountMap.BudgetBankAccountMapId))
            {
                ModelState.AddModelError("SameName", "   Name Already Exists !");
                return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));


            }
            else if ( db.BudgetBankAccountMaps.Any(x =>  x.IsHeadOffice &&    x.BudgetBankAccountMapId != BudgetBankAccountMap.BudgetBankAccountMapId))
            {

                ModelState.AddModelError("SameAccountCode", " Head Office Account Already Exists !");
                return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));

            }
            else if (db.BudgetBankAccountMaps.Any(x => x.BranchId == BudgetBankAccountMap.BranchId && x.BudgetBankAccountMapId != BudgetBankAccountMap.BudgetBankAccountMapId))
            {
                ModelState.AddModelError("SameBranch", "   Branch Already Exists !");
                return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));


            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetBankAccountMap
                    {
                        BudgetBankAccountMapId = BudgetBankAccountMap.BudgetBankAccountMapId,
                        Name = BudgetBankAccountMap.Name,
                        AccountCode = BudgetBankAccountMap.AccountCode,
                        Address = BudgetBankAccountMap.Address,
                        BankNumber = BudgetBankAccountMap.BankNumber,
                        BranchId = BudgetBankAccountMap.BranchId,
                        IsHeadOffice = BudgetBankAccountMap.IsHeadOffice,
                        CreatedBy = BudgetBankAccountMap.CreatedBy,
                        DateCreated = BudgetBankAccountMap.DateCreated,
                        Remark = BudgetBankAccountMap.Remark,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.BudgetBankAccountMaps.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetBankAccountMap_Destroy([DataSourceRequest]DataSourceRequest request, BudgetBankAccountMap BudgetBankAccountMap)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetBankAccountMap
                {
                    BudgetBankAccountMapId = BudgetBankAccountMap.BudgetBankAccountMapId,


                };

                db.BudgetBankAccountMaps.Attach(entity);
                db.BudgetBankAccountMaps.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Account Map number " + BudgetBankAccountMap.BudgetBankAccountMapId);
            }

            return Json(new[] { BudgetBankAccountMap }.ToDataSourceResult(request, ModelState));
        }

       
    }
}