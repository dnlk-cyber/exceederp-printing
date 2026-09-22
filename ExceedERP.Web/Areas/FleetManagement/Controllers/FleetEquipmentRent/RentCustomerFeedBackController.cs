using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class RentCustomerFeedBackController : Controller
    {
        IRentCustomerFeedBackService _RentCustomerFeedBackService;

        ExceedDbContext db = new ExceedDbContext();
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
            //IQueryable<RentCustomerFeedBack> RentCustomerFeedBack = db.RentCustomerFeedBack.AsQueryable();

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
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Feedback from rent customer ", "Success", User.Identity.Name, $"created rent customer feedback reg {RentCustomerFeedBack.RentCustomerFeedBackID}", Modules.FleetManagement);
                #endregion
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
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Feedback from rent customer", "Success", User.Identity.Name, $"updated rent customer feedback reg {RentCustomerFeedBack.RentCustomerFeedBackID}", Modules.FleetManagement);
                #endregion

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
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted rent customer feedback registration number {rentCustomerFeedBack.RentCustomerFeedBackID }");
                #endregion
            }

            return Json(new[] { RentCustomerFeedBack }.ToDataSourceResult(request));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

