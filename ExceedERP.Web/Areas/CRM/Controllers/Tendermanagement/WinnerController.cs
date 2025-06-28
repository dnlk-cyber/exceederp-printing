using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.CRM.TenderManagement;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
	public class WinnerController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Winner_Read([DataSourceRequest]DataSourceRequest request,int saleLotId)
        {

            var winnerList = db.Winners.Where(x => x.SalesLotId == saleLotId).ToList(); // Fetch all first

            foreach (var w in winnerList)
            {
                var checkAvailability = db.SalesLotParticipants
                    .SingleOrDefault(slp => slp.SalesLotId == saleLotId && slp.TinNumber == w.TinNumber);

                if (checkAvailability == null)
                {
                    db.Winners.Remove(w);
                }
            }

            db.SaveChanges();

            IQueryable<Winner> WinnerTenders = db.Winners.Where(x=>x.SalesLotId== saleLotId);
            DataSourceResult result = WinnerTenders.ToDataSourceResult(request, saleTender => new {
                WinnerId = saleTender.WinnerId,
                ProducCategoryId = saleTender.ProducCategoryId,
                ProductID = saleTender.ProductID,
                SalesLotId = saleTender.SalesLotId,
                SalesLotItemId = saleTender.SalesLotItemId,
                SalesLotName = saleTender.SalesLotName,
                CustormerAddress = saleTender.CustormerAddress,
                ProductCategoryDesc=saleTender.ProductCategoryDesc,
                ProductName = saleTender.ProductName,
                CustomerName = saleTender.CustomerName,
                TinNumber = saleTender.TinNumber,
                WinningPrice = saleTender.WinningPrice,
                UnitPrice = saleTender.UnitPrice,
               

            });

            return Json(result);
        }

    }
}