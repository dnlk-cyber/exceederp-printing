using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.SelfCare
{
    public class TravelExpenseExpenseRequestController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        private readonly FinanceDistributionHelper _distributionHelper = new FinanceDistributionHelper();


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
        public ActionResult TravelExpenseExpenseRequest_Read(int id, [DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApTravelExpenseExpense> apTravelExpenseExpenses = _db.ApTravelExpenseExpenses.Where(x => x.ApTravelExpenseClaimId == id);
            DataSourceResult result = apTravelExpenseExpenses.ToDataSourceResult(request, apTravelExpenseExpense => new
            {
                ApTravelExpenseClaimId = apTravelExpenseExpense.ApTravelExpenseClaimId,
                ApTravelExpenseExpenseId = apTravelExpenseExpense.ApTravelExpenseExpenseId,
                Description = apTravelExpenseExpense.Description,
                ItemName = apTravelExpenseExpense.ItemName,
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
        public ActionResult TravelExpenseExpenseRequest_Create([DataSourceRequest]DataSourceRequest request, ApTravelExpenseExpense apTravelExpenseExpense, int id)
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
                UpdateTravelExpenseBalance(id);
                _distributionHelper.UpdateApTravelOrdeExpenseClaimDistribution(id);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care expense claim expense item", "Success", User.Identity.Name, $"created expense claim expense item {entity.Description} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion
            }

            return Json(new[] { apTravelExpenseExpense }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelExpenseExpenseRequest_Update([DataSourceRequest]DataSourceRequest request, ApTravelExpenseExpense apTravelExpenseExpense)
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
                UpdateTravelExpenseBalance(apTravelExpenseExpense.ApTravelExpenseClaimId);
                _distributionHelper.UpdateApTravelOrdeExpenseClaimDistribution(apTravelExpenseExpense.ApTravelExpenseClaimId);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Self care expense claim expense item", "Success", User.Identity.Name, $"updated expense claim expense item {entity.Description} under record {entity.ApTravelExpenseClaimId} ", Modules.Finance);
                #endregion

            }

            return Json(new[] { apTravelExpenseExpense }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TravelExpenseExpenseRequest_Destroy([DataSourceRequest]DataSourceRequest request, ApTravelExpenseExpense apTravelExpenseExpense)
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
                UpdateTravelExpenseBalance(apTravelExpenseExpense.ApTravelExpenseClaimId);
                _distributionHelper.UpdateApTravelOrdeExpenseClaimDistribution(apTravelExpenseExpense.ApTravelExpenseClaimId);

            }

            return Json(new[] { apTravelExpenseExpense }.ToDataSourceResult(request, ModelState));
        }
        private void UpdateTravelExpenseBalance(int id)
        {
            decimal advanceAmount = 0;
            decimal expenseAmount = 0;
            var record = _db.ApTravelExpenseClaims.Find(id);
            var travelorder = _db.ApTravelOrders.FirstOrDefault(x => x.ApTravelOrderId == record.ApTravelOrderId);
            var advancesTaken = _db.ApTravelOrderAdvancePayments.Where(x => x.ApTravelOrderId == record.ApTravelOrderId);
            if (FinanceConfigurationHelper.IgnorePerdiemInClaim())
            {
                advancesTaken = advancesTaken.Where(x => x.Type != TravelPaymentType.PerDiem);
            }
            if (advancesTaken.Any())
                advanceAmount = advancesTaken.Sum(x => x.Quantity * x.UnitPrice);

            var expenses = _db.ApTravelExpenseExpenses.Where(x => x.ApTravelExpenseClaimId == id);

            if (expenses.Any())
                expenseAmount = expenses.Sum(x => x.Total);

            record.Difference = advanceAmount - expenseAmount;
            _db.ApTravelExpenseClaims.Attach(record);
            _db.Entry(record).State = EntityState.Modified;
            _db.SaveChanges();

            if (advanceAmount > expenseAmount)
            {
                //refund company
                decimal amount = advanceAmount - expenseAmount;
                var payment = _db.ApTravelExpensePayments.Where(x => x.ApTravelExpenseClaimId == id).ToList();
                _db.ApTravelExpensePayments.RemoveRange(payment);
                var refunds = _db.ApTravelExpenseRefunds.Where(x => x.ApTravelExpenseClaimId == id).ToList();
                _db.ApTravelExpenseRefunds.RemoveRange(refunds);
                _db.SaveChanges();
                if (amount > 0)
                {
                    var entity = new ApTravelExpenseRefund
                    {
                        ApTravelExpenseClaimId = id,
                        Description = "NA",
                        AccountCode = "NA",
                        Amount = amount,
                        PaymentMethod = "NA",
                    };

                    _db.ApTravelExpenseRefunds.Add(entity);
                    _db.SaveChanges();
                }
            }
            else
            {
                //pay employee
                decimal amount = expenseAmount - advanceAmount;
                var payment = _db.ApTravelExpensePayments.Where(x => x.ApTravelExpenseClaimId == id).ToList();
                _db.ApTravelExpensePayments.RemoveRange(payment);
                var refunds = _db.ApTravelExpenseRefunds.Where(x => x.ApTravelExpenseClaimId == id).ToList();
                _db.ApTravelExpenseRefunds.RemoveRange(refunds);
                _db.SaveChanges();

                if (amount > 0)
                {
                    var entity = new ApTravelExpensePayment
                    {
                        ApTravelExpenseClaimId = id,
                        Description = "NA",
                        AccountCode = "NA",
                        Amount = amount,
                        PaymentMethod = "NA",
                    };

                    _db.ApTravelExpensePayments.Add(entity);
                    _db.SaveChanges();
                }
            }

        }
        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}