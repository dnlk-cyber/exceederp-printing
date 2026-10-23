using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Budget.Controllers.TravelRequests
{
    public class TravelExpenseExpenseRequestBudgetController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
      
        // GET: Finance/ApTravelExpenseExpenseDeparture
        public ActionResult Index()
        {
            PupulateViewData();
            return View();
        }
        public void PupulateViewData()
        {
            //ViewData["TravelOrder"] = db.ApTravelExpenseExpenses.ToList();
        }
        public ActionResult ApTravelExpenseExpenseRequest_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelExpenseExpense> apTravelExpenseExpenses = _db.ApTravelExpenseExpenses.Where(x => x.ApTravelExpenseClaimId == id);
            DataSourceResult result = apTravelExpenseExpenses.ToDataSourceResult(request, apTravelExpenseExpense => new
            {
                ApTravelExpenseClaimId = apTravelExpenseExpense.ApTravelExpenseClaimId,
                ApTravelExpenseExpenseId = apTravelExpenseExpense.ApTravelExpenseExpenseId,
                ItemName = apTravelExpenseExpense.ItemName,
                Description = apTravelExpenseExpense.Description,
                Total = apTravelExpenseExpense.Total,
                Type = apTravelExpenseExpense.Type,
                UnitPrice = apTravelExpenseExpense.UnitPrice,
                UoM = apTravelExpenseExpense.UoM,
                Quantity = apTravelExpenseExpense.Quantity,
                OrgBranchName = apTravelExpenseExpense.OrgBranchName,
                DateCreated = apTravelExpenseExpense.DateCreated,
                LastModified = apTravelExpenseExpense.LastModified,
                CreatedBy = apTravelExpenseExpense.CreatedBy,
                ModifiedBy = apTravelExpenseExpense.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseExpenseRequest_Create([DataSourceRequest]DataSourceRequest request, ApTravelExpenseExpense apTravelExpenseExpense, int id)
        {
            apTravelExpenseExpense.Total = apTravelExpenseExpense.UnitPrice * apTravelExpenseExpense.Quantity;

            if (ModelState.IsValid)
            {


                var entity = new ApTravelExpenseExpense
                {
                    ApTravelExpenseClaimId = id,
                    ItemName = apTravelExpenseExpense.ItemName,
                    Description = apTravelExpenseExpense.Description,
                    Total = apTravelExpenseExpense.Total,
                    Type = apTravelExpenseExpense.Type,
                    UnitPrice=apTravelExpenseExpense.UnitPrice,
                    UoM =apTravelExpenseExpense.UoM,
                    Quantity = apTravelExpenseExpense.Quantity,
                    OrgBranchName = apTravelExpenseExpense.OrgBranchName,
                    DateCreated = apTravelExpenseExpense.DateCreated,
                    LastModified = apTravelExpenseExpense.LastModified,
                    CreatedBy = apTravelExpenseExpense.CreatedBy,
                    ModifiedBy = apTravelExpenseExpense.ModifiedBy
                };

                _db.ApTravelExpenseExpenses.Add(entity);
                _db.SaveChanges();
                apTravelExpenseExpense.ApTravelExpenseExpenseId = entity.ApTravelExpenseExpenseId;
              
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care expense claim expense item", "Success", User.Identity.Name, $"created expense claim expense item {entity.Description} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpenseExpense }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseExpenseRequest_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseExpense apTravelExpenseExpense)
        {
            apTravelExpenseExpense.Total = apTravelExpenseExpense.UnitPrice * apTravelExpenseExpense.Quantity;
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseExpense
                {
                    ApTravelExpenseExpenseId = apTravelExpenseExpense.ApTravelExpenseExpenseId,
                    ApTravelExpenseClaimId = apTravelExpenseExpense.ApTravelExpenseClaimId,
                    ItemName = apTravelExpenseExpense.ItemName,
                    Description = apTravelExpenseExpense.Description,
                    Total = apTravelExpenseExpense.Total,
                    Type = apTravelExpenseExpense.Type,
                    UnitPrice = apTravelExpenseExpense.UnitPrice,
                    UoM = apTravelExpenseExpense.UoM,
                    Quantity = apTravelExpenseExpense.Quantity,
                    OrgBranchName = apTravelExpenseExpense.OrgBranchName,
                    DateCreated = apTravelExpenseExpense.DateCreated,
                    LastModified = apTravelExpenseExpense.LastModified,
                    CreatedBy = apTravelExpenseExpense.CreatedBy,
                    ModifiedBy = apTravelExpenseExpense.ModifiedBy
                };

                _db.ApTravelExpenseExpenses.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
              

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care expense claim expense item", "Success", User.Identity.Name, $"updated expense claim expense item {entity.Description} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { apTravelExpenseExpense }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApTravelExpenseExpenseRequest_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseExpense apTravelExpenseExpense)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApTravelExpenseExpense
                {
                    ApTravelExpenseExpenseId = apTravelExpenseExpense.ApTravelExpenseExpenseId,
                    ApTravelExpenseClaimId = apTravelExpenseExpense.ApTravelExpenseClaimId,
                    ItemName = apTravelExpenseExpense.ItemName,
                    Description = apTravelExpenseExpense.Description,
                    Total = apTravelExpenseExpense.Total,
                    Type = apTravelExpenseExpense.Type,
                    UnitPrice = apTravelExpenseExpense.UnitPrice,
                    UoM = apTravelExpenseExpense.UoM,
                    Quantity = apTravelExpenseExpense.Quantity,
                    OrgBranchName = apTravelExpenseExpense.OrgBranchName,
                    DateCreated = apTravelExpenseExpense.DateCreated,
                    LastModified = apTravelExpenseExpense.LastModified,
                    CreatedBy = apTravelExpenseExpense.CreatedBy,
                    ModifiedBy = apTravelExpenseExpense.ModifiedBy
                };

                _db.ApTravelExpenseExpenses.Attach(entity);
                _db.ApTravelExpenseExpenses.Remove(entity);
                _db.SaveChanges();
               

            }

            return Json(new[] { apTravelExpenseExpense }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}