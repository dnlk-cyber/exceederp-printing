using ExceedERP.Core.Domain.Budget;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;

using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles( BudgetRoles.BudgetRequestor)] 
	public class RequestedMonthController : Controller
	{
		//
		// GET: /Budget/RequestedMonth/

		private ExceedDbContext db = new ExceedDbContext();

		public ActionResult Index()
		{
			return View();
		}

		public ActionResult RequestedMonth_Read( int RId,[DataSourceRequest]DataSourceRequest request)
		{
			IQueryable<RequestedMonth> RequestedMonth = db.RequestedMonths;
			DataSourceResult result = RequestedMonth.ToDataSourceResult(request, requestedMonth => new RequestedMonth
			{
				RequestedMonthID = requestedMonth.RequestedMonthID,
				Name = requestedMonth.Name,
				Amount = requestedMonth.Amount,
				CreatedBy = requestedMonth.CreatedBy,
				DateCreated = requestedMonth.DateCreated,
				LastModified = requestedMonth.LastModified,
				ModifiedBy = requestedMonth.ModifiedBy,


			});

			return Json(result);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult RequestedMonth_Create(int CId, [DataSourceRequest]DataSourceRequest request, RequestedMonth RequestedMonth)
		{

			if (ModelState.IsValid)
			{
				var entity = new RequestedMonth
				{
					RequestedMonthID = RequestedMonth.RequestedMonthID,
					Name = RequestedMonth.Name,
					Amount=RequestedMonth.Amount,
					BudgetRequestID=CId,
					DateCreated = DateTime.Now,
					CreatedBy = User.Identity.Name

				};

				db.RequestedMonths.Add(entity);
				db.SaveChanges();



				RequestedMonth.RequestedMonthID = entity.RequestedMonthID;
			}


			return Json(new[] { RequestedMonth }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult RequestedMonth_Update([DataSourceRequest]DataSourceRequest request, RequestedMonth RequestedMonth)
		{

			if (ModelState.IsValid)
			{
				var entity = new RequestedMonth
				{
					RequestedMonthID = RequestedMonth.RequestedMonthID,
					Name = RequestedMonth.Name,
					Amount = RequestedMonth.Amount,
					BudgetRequestID = RequestedMonth.BudgetRequestID,
					LastModified = DateTime.Now,
					ModifiedBy = User.Identity.Name
				};

				db.RequestedMonths.Attach(entity);
				db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
				db.SaveChanges();

			}

			return Json(new[] { RequestedMonth }.ToDataSourceResult(request, ModelState));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult RequestedMonth_Destroy([DataSourceRequest]DataSourceRequest request, RequestedMonth RequestedMonth)
		{
			if (ModelState.IsValid)
			{
				var entity = new RequestedMonth
				{
					RequestedMonthID = RequestedMonth.RequestedMonthID,


				};

				db.RequestedMonths.Attach(entity);
				db.RequestedMonths.Remove(entity);
				db.SaveChanges();
				UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Requested Month number " + RequestedMonth.BudgetRequestID);
	  

			}

			return Json(new[] { RequestedMonth }.ToDataSourceResult(request, ModelState));
		}


		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}

	}
}