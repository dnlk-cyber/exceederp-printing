using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles( BudgetRoles.BudgetAdmin)] 
    public class BudgetCategoryController : Controller
    {
        //
        // GET: /Budget/BudgetCategory/

        
        private ExceedDbContext db = new ExceedDbContext();
        private AccountHelper _accountHelper = new AccountHelper();

        public ActionResult Index()
        {
            ViewData["BudgetControl"] = db.BudgetControls.ToList();

            return View();
        }

        public ActionResult BudgetCategory_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetCategory> BudgetCategory = db.BudgetCategorys;
            DataSourceResult result = BudgetCategory.ToDataSourceResult(request, budgetCategory => new BudgetCategory
            {
                BudgetCategoryID = budgetCategory.BudgetCategoryID,
                BudgetCategoryCode = budgetCategory.BudgetCategoryCode,//added as requested
                //BudgetCategoryDescription = budgetCategory.BudgetCategoryDescription, //Added as request
                Name = budgetCategory.Name,
                BudgetControlId = budgetCategory.BudgetControlId,
                FromAccountCode = budgetCategory.FromAccountCode,
                ToAccountCode = budgetCategory.ToAccountCode,
                BudgetType = budgetCategory.BudgetType,
                CreatedBy = budgetCategory.CreatedBy,
                DateCreated = budgetCategory.DateCreated,
                LastModified = budgetCategory.LastModified,
                ModifiedBy = budgetCategory.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetCategory_Create([DataSourceRequest]DataSourceRequest request, BudgetCategory budgetCategory)
        {
            List<BudgetCategory> category = db.BudgetCategorys.Where(x => x.Name == budgetCategory.Name || x.BudgetCategoryCode == budgetCategory.BudgetCategoryCode).ToList();

            var fromAccountCodeSeg1 = budgetCategory.FromAccountCode.Split('-')[0];
            var fromAccountCodeSeg2 = budgetCategory.FromAccountCode.Split('-')[1];
            var fromAccountCodeSeg3 = budgetCategory.FromAccountCode.Split('-')[2];
            var fromAccountCodeSeg4 = budgetCategory.FromAccountCode.Split('-')[3];
            
            var toAccountCodeSeg1 = budgetCategory.ToAccountCode.Split('-')[0];
            var toAccountCodeSeg2 = budgetCategory.ToAccountCode.Split('-')[1];
            var toAccountCodeSeg3 = budgetCategory.ToAccountCode.Split('-')[2];
            var toAccountCodeSeg4 = budgetCategory.ToAccountCode.Split('-')[3];
     

            if (!ModelState.IsValid)
            {
                return Json(new[] { budgetCategory }.ToDataSourceResult(request, ModelState));
   
            }

            
            if(category.Any())
            {
                ModelState.AddModelError("SameName", " Category Name Already Exists !");
                return Json(new[] { budgetCategory }.ToDataSourceResult(request, ModelState));
   
      
            }
            //else if (BudgetCategory.FromAccountCode.Equals(BudgetCategory.ToAccountCode)) 
            //{

            //    ModelState.AddModelError("SameAccountCode", " the FromAccountCode and To Account Code must Be Different !");
            //    return Json(new[] { BudgetCategory }.ToDataSourceResult(request, ModelState));

            //}
            else if(string.Compare(fromAccountCodeSeg1, toAccountCodeSeg1) > 0 || string.Compare(fromAccountCodeSeg2, toAccountCodeSeg2) > 0 || string.Compare(fromAccountCodeSeg3, toAccountCodeSeg3) > 0 || string.Compare(fromAccountCodeSeg4, toAccountCodeSeg4) > 0)
            {
                ModelState.AddModelError(string.Empty, "From Account Code values must not be greater than To Account Code values, Please change the values and try again!");
                return Json(new[] { budgetCategory }.ToDataSourceResult(request, ModelState));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    
                    var entity = new BudgetCategory
                    {
                        BudgetCategoryID = budgetCategory.BudgetCategoryID,
                        BudgetCategoryCode = budgetCategory.BudgetCategoryCode,//added as requested
                        //BudgetCategoryDescription = budgetCategory.BudgetCategoryDescription, //Added as request
                        Name = budgetCategory.Name,
                        FromAccountCode = budgetCategory.FromAccountCode,
                        ToAccountCode = budgetCategory.ToAccountCode,
                        BudgetType = budgetCategory.BudgetType,
                        BudgetControlId = budgetCategory.BudgetControlId,
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name

                    };

                    db.BudgetCategorys.Add(entity);
                    db.SaveChanges();



                    budgetCategory.BudgetCategoryID = entity.BudgetCategoryID;
                }
                else
                {
                    return Json(new[] { budgetCategory }.ToDataSourceResult(request, ModelState));
      
                }
            }

            return Json(new[] { budgetCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetCategory_Update([DataSourceRequest]DataSourceRequest request, BudgetCategory budgetCategory)
        {

            List<BudgetCategory> category = db.BudgetCategorys.Where(x => x.Name == budgetCategory.Name && x.BudgetCategoryID != budgetCategory.BudgetCategoryID || x.BudgetCategoryCode == budgetCategory.BudgetCategoryCode && x.BudgetCategoryID != budgetCategory.BudgetCategoryID).ToList();

            var fromAccountCodeSeg1 = budgetCategory.FromAccountCode.Split('-')[0];
            var fromAccountCodeSeg2 = budgetCategory.FromAccountCode.Split('-')[1];
            var fromAccountCodeSeg3 = budgetCategory.FromAccountCode.Split('-')[2];
            var fromAccountCodeSeg4 = budgetCategory.FromAccountCode.Split('-')[3];

            var toAccountCodeSeg1 = budgetCategory.ToAccountCode.Split('-')[0];
            var toAccountCodeSeg2 = budgetCategory.ToAccountCode.Split('-')[1];
            var toAccountCodeSeg3 = budgetCategory.ToAccountCode.Split('-')[2];
            var toAccountCodeSeg4 = budgetCategory.ToAccountCode.Split('-')[3];
            //if (BudgetCategory.FromAccountCode.Equals(BudgetCategory.ToAccountCode))
            //{

            //    ModelState.AddModelError("SameAccountCode", " the FromAccountCode and To Account Code must Be Different !");
            //}
            if (string.Compare(fromAccountCodeSeg1, toAccountCodeSeg1) > 0 || string.Compare(fromAccountCodeSeg2, toAccountCodeSeg2) > 0 || string.Compare(fromAccountCodeSeg3, toAccountCodeSeg3) > 0 || string.Compare(fromAccountCodeSeg4, toAccountCodeSeg4) > 0)
            {
                ModelState.AddModelError(string.Empty, "From Account Code values must not be greater than To Account Code values, Please change the values and try again!");
                return Json(new[] { budgetCategory }.ToDataSourceResult(request, ModelState));
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var entity = new BudgetCategory
                    {
                        BudgetCategoryID = budgetCategory.BudgetCategoryID,
                        BudgetCategoryCode = budgetCategory.BudgetCategoryCode,//added as requested
                        //BudgetCategoryDescription = budgetCategory.BudgetCategoryDescription, //Added as request
                        Name = budgetCategory.Name,
                        BudgetControlId = budgetCategory.BudgetControlId,
                        FromAccountCode = budgetCategory.FromAccountCode,
                        ToAccountCode = budgetCategory.ToAccountCode,
                        BudgetType = budgetCategory.BudgetType,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    db.BudgetCategorys.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                
                }
            }
            return Json(new[] { budgetCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetCategory_Destroy([DataSourceRequest]DataSourceRequest request, BudgetCategory budgetCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetCategory
                {
                    BudgetCategoryID = budgetCategory.BudgetCategoryID,

                };

                db.BudgetCategorys.Attach(entity);
                db.BudgetCategorys.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Category number " + budgetCategory.BudgetCategoryID);
            }

            return Json(new[] { budgetCategory }.ToDataSourceResult(request, ModelState));
        }




        /// <summary>
        /// ModelCategoryDetail Actions have one-to-many r/n with BudgetCategory
        /// </summary>
        
        public ActionResult Detail()
        {
            //ViewData["BudgetControl"] = db.BudgetControls.ToList();

            return View();
        }

        public ActionResult BudgetCategoryDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IEnumerable<BudgetCategoryDetail> BudgetCategoryDetail = db.BudgetCategoryDetails.Where(x => x.BudgetCategoryId == id).ToList();
            DataSourceResult result = BudgetCategoryDetail.ToDataSourceResult(request, budgetCategoryDetail => new BudgetCategoryDetail
            {
                BudgetCategoryDetailID = budgetCategoryDetail.BudgetCategoryDetailID,
                BudgetCode = budgetCategoryDetail.BudgetCode,
                BudgetItem = budgetCategoryDetail.BudgetItem,
                AccountCode = budgetCategoryDetail.AccountCode,
                BudgetCategoryDetailDescription = budgetCategoryDetail.BudgetCategoryDetailDescription,
                BudgetCategoryId =budgetCategoryDetail.BudgetCategoryId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetCategoryDetail_Create([DataSourceRequest]DataSourceRequest request, BudgetCategoryDetail budgetCategoryDetail, int id)
        {
            if (ModelState.IsValid)
            {
                budgetCategoryDetail.BudgetCode = _accountHelper.GetNaturalAccountSegment(budgetCategoryDetail.AccountCode);
                var entity = new BudgetCategoryDetail
                {
                   BudgetCategoryDetailID = budgetCategoryDetail.BudgetCategoryDetailID,
                    BudgetCode = budgetCategoryDetail.BudgetCode,
                    BudgetItem = budgetCategoryDetail.BudgetItem,
                    AccountCode = budgetCategoryDetail.AccountCode,
                    BudgetCategoryDetailDescription = budgetCategoryDetail.BudgetCategoryDetailDescription,
                   BudgetCategoryId = id
                };

                db.BudgetCategoryDetails.Add(entity);
                db.SaveChanges();



                budgetCategoryDetail.BudgetCategoryDetailID = entity.BudgetCategoryDetailID;
            }
            return Json(new[] { budgetCategoryDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetCategoryDetail_Update([DataSourceRequest]DataSourceRequest request, BudgetCategoryDetail budgetCategoryDetail)
        {
            if (ModelState.IsValid)
            {
                budgetCategoryDetail.BudgetCode = _accountHelper.GetNaturalAccountSegment(budgetCategoryDetail.AccountCode);

                var entity = new BudgetCategoryDetail
                {
                    BudgetCategoryDetailID = budgetCategoryDetail.BudgetCategoryDetailID,
                    BudgetCode = budgetCategoryDetail.BudgetCode,
                    BudgetItem = budgetCategoryDetail.BudgetItem,
                    AccountCode = budgetCategoryDetail.AccountCode,
                    BudgetCategoryDetailDescription = budgetCategoryDetail.BudgetCategoryDetailDescription,
                    BudgetCategoryId = budgetCategoryDetail.BudgetCategoryId
                };

                db.BudgetCategoryDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }
            return Json(new[] { budgetCategoryDetail }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetCategoryDetail_Destroy([DataSourceRequest]DataSourceRequest request, BudgetCategoryDetail budgetCategoryDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetCategoryDetail
                {
                    BudgetCategoryDetailID = budgetCategoryDetail.BudgetCategoryDetailID,
                    BudgetCode = budgetCategoryDetail.BudgetCode,
                    BudgetItem = budgetCategoryDetail.BudgetItem,
                    AccountCode = budgetCategoryDetail.AccountCode,

                };

                db.BudgetCategoryDetails.Attach(entity);
                db.BudgetCategoryDetails.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Category number " + budgetCategoryDetail.BudgetCategoryDetailID);
            }

            return Json(new[] { budgetCategoryDetail }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }


    }
}