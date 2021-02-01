using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class RentCustomerFeedBackController : Controller
    {
        IRentCustomerFeedBackService _RentCustomerFeedBackService;
        
        
        public RentCustomerFeedBackController()
        {
            _RentCustomerFeedBackService = new RentCustomerFeedBackService();
        }
        //
        // GET: /FleetManagement/RentCustomerFeedBack/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RentCustomerFeedBack_Read([DataSourceRequest] DataSourceRequest request)
        {
            var RentCustomerFeedBack = _RentCustomerFeedBackService.GetAll();

            return Json(RentCustomerFeedBack.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentCustomerFeedBack_Create([DataSourceRequest] DataSourceRequest request, RentCustomerFeedBack RentCustomerFeedBack)
        {
            ModelState.Remove("RentCustomerFeedBackID");
            RentCustomerFeedBack.RentCustomerFeedBackID = Guid.NewGuid();
            RentCustomerFeedBack.FeedBackDate = DateTime.Now;
         
            if (RentCustomerFeedBack != null)
            {
                _RentCustomerFeedBackService.Insert(RentCustomerFeedBack);
            }

            return Json(new[] { RentCustomerFeedBack }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentCustomerFeedBack_Update([DataSourceRequest] DataSourceRequest request, RentCustomerFeedBack RentCustomerFeedBack)
        {
            RentCustomerFeedBack.FeedBackDate = DateTime.Now;
         
            if (RentCustomerFeedBack != null)
            {
                _RentCustomerFeedBackService.Update(RentCustomerFeedBack);
            }

            return Json(new[] { RentCustomerFeedBack }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RentCustomerFeedBack_Destroy([DataSourceRequest] DataSourceRequest request, RentCustomerFeedBack RentCustomerFeedBack)
        {
            var rentCustomerFeedBack = _RentCustomerFeedBackService.GetById(RentCustomerFeedBack.RentCustomerFeedBackID);
            if (rentCustomerFeedBack != null)
            {
                _RentCustomerFeedBackService.Delete(rentCustomerFeedBack);
            }

            return Json(new[] { RentCustomerFeedBack }.ToDataSourceResult(request));
        }



    }
}

