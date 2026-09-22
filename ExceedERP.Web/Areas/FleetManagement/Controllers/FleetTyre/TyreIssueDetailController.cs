using ExceedERP.ApplicationServices.FleetManagement.FleetTireManagement;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using System.Data.Entity.Validation;
using static ExceedERP.Core.Domain.FleetManagement.FleetTireManagement.TireReturn;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
   // [AuthorizeRoles(FleetManagementRoles.TyreUser, FleetManagementRoles.TyreApprover)]
    public class TyreIssueDetailController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
     

      
        //
        // GET: /FleetManagement/TireIssue/
        public ActionResult Index()
        {
           
            return View();
        }


        public ActionResult TireIssueDetail_Read([DataSourceRequest] DataSourceRequest request,string id)
        {
            IQueryable<TireIssueDetail> TireIssueDetail = db.TireIssueDetails.Where(x=>x.TireIssueId.ToString()==id);
            DataSourceResult result = TireIssueDetail.ToDataSourceResult(request, t => new
            {
                TireIssueDetailId=t.TireIssueDetailId,
                UnitPrice = t.UnitPrice,
                Brand = t.Brand,
                IssueType = t.IssueType,
                TireSize = t.TireSize,
                TireTube = t.TireTube,                
                TireType = t.TireType,
                SerialNo = t.SerialNo
               
            }
             );

            return Json(result);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireIssueDetail_Create([DataSourceRequest] DataSourceRequest request, TireIssueDetail TireIssue,string id)
        {

            //Fleets fleet = fleetService.GetAll().ToList().Where(x => x.FleetsID == TireIssue.FleetId).FirstOrDefault();  
            if (ModelState.IsValid)
            {
                var TireIssueDetail = new TireIssueDetail();
                TireIssueDetail.TireIssueDetailId = TireIssueDetail.TireIssueDetailId;
                TireIssueDetail.TireIssueId = id;
                TireIssueDetail.Brand = TireIssue.Brand;
                TireIssueDetail.SerialNo = TireIssue.SerialNo;
                TireIssueDetail.IssueType = TireIssue.IssueType;
                TireIssueDetail.TireSize = TireIssue.TireSize;
                TireIssueDetail.TireTube = TireIssue.TireTube;
                TireIssueDetail.TireType = TireTypeenum.Wire.ToString();
                TireIssueDetail.UnitPrice = TireIssue.UnitPrice;
                db.TireIssueDetails.Add(TireIssueDetail);
                db.SaveChanges();
            }
            #region operation log
            #endregion


            return Json(new[] { TireIssue }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireIssueDetail_Update([DataSourceRequest] DataSourceRequest request, TireIssueDetail TireIssue)
        {
            var FetchTireIssueDetail = db.TireIssueDetails.FirstOrDefault(x => x.TireIssueDetailId == TireIssue.TireIssueDetailId);
            if(ModelState.IsValid)
            {
                FetchTireIssueDetail.Brand = TireIssue.Brand;
                FetchTireIssueDetail.SerialNo = TireIssue.SerialNo;
                FetchTireIssueDetail.IssueType = TireIssue.IssueType;
                FetchTireIssueDetail.TireSize = TireIssue.TireSize;
                FetchTireIssueDetail.TireTube = TireIssue.TireTube;
                FetchTireIssueDetail.TireType = TireTypeenum.Wire.ToString();
                db.Entry(FetchTireIssueDetail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                
                #region operation log
                #endregion
            }

            return Json(new[] { TireIssue }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireIssueDetail_Destroy([DataSourceRequest] DataSourceRequest request, TireIssueDetail tireIssue)
        {
            var FetchTireIssueDetail = db.TireIssueDetails.FirstOrDefault(x => x.TireIssueDetailId == tireIssue.TireIssueDetailId);
            if (ModelState.IsValid)
            {
                db.TireIssueDetails.Remove(FetchTireIssueDetail);
                db.SaveChanges();

                #region operation log
                #endregion
            }

            return Json(new[] { tireIssue }.ToDataSourceResult(request, ModelState));
        }


        
      

        
        public ActionResult GetTyreInfo(Guid Id)
        {
            string Response = string.Empty;
            var Tyre = db.TireReturn.Where(x => x.TireReturnID == Id && x.IsOnlineApproved == true && x.IsVoid == false).FirstOrDefault();
            if (Tyre != null)
            {
                Response = Tyre.Brand + "_" + Tyre.TireTube + "_" + Tyre.TireSize + "_" + Tyre.TireType + "_" + Tyre.KMReading + "_" + Tyre.UnitPrice + "_" + DateTime.Parse(Tyre.DateCreated.ToString()).ToString("dd/mm/yyyy");
            }

            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
