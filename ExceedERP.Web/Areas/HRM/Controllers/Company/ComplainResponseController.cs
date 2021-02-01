//using ExceedERP.Core.Domain.HRM.EmployeeDetail;
//using ExceedERP.DataAccess.Context;
//using ExceedERP.Helpers.Common;
//using Kendo.Mvc.Extensions;
//using Kendo.Mvc.UI;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace ExceedERP.Web.Areas.HRM.Controllers.Company
//{
//    public class ComplainResponseController : Controller
//    {
//        private ExceedDbContext db = new ExceedDbContext();
//        // GET: HRM/ComplainResponse
//        public ActionResult Index()
//        {
//            return View();
//        }
//        public ActionResult ComplainResponse_Read([DataSourceRequest]DataSourceRequest request, int id)
//        {
//            var  responess = db.ComplainResponses.Where(x=>x.ComplainId == id).ToList();
//            DataSourceResult result = responess.ToDataSourceResult(request, complain => new
//            {
//                ComplainId = complain.ComplainId,
//                ComplainResponseId = complain.ComplainResponseId,
//                Response = complain.Response
//            });

//            return Json(result);
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult ComplainResponse_Create([DataSourceRequest]DataSourceRequest request, ComplainResponse response, int id)
//        {
//            ModelState.Clear();
//            if (ModelState.IsValid)
//            {
              
//                    var entity = new ComplainResponse
//                    {
//                        ComplainId = id,
//                        Response = response.Response
//                    };

//                    db.ComplainResponses.Add(entity);
//                    db.SaveChanges();
               
//            }

//            return Json(new[] { response }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult ComplainResponse_Update([DataSourceRequest]DataSourceRequest request, ComplainResponse response)
//        {
//            if (ModelState.IsValid)
//            {
//                var entity = new ComplainResponse
//                {
//                    ComplainResponseId = response.ComplainResponseId,
//                    ComplainId = response.ComplainId,
//                    Response = response.Response
//                };

//                db.ComplainResponses.Attach(entity);
//                db.Entry(entity).State = EntityState.Modified;
//                db.SaveChanges();
//            }

//            return Json(new[] { response }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult ComplainResponse_Destroy([DataSourceRequest]DataSourceRequest request, ComplainResponse response)
//        {
//            if (ModelState.IsValid)
//            {
//                var entity = new ComplainResponse
//                {
//                    ComplainResponseId = response.ComplainResponseId,
//                    ComplainId = response.ComplainId,
//                    Response = response.Response
//                };
//                #region delete log
//                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Attachment" + response.ComplainId.ToString());
//                #endregion
//                db.ComplainResponses.Attach(entity);
//                db.ComplainResponses.Remove(entity);
//                db.SaveChanges();
//            }

//            return Json(new[] { response }.ToDataSourceResult(request, ModelState));
//        }

//        protected override void Dispose(bool disposing)
//        {
//            db.Dispose();
//            base.Dispose(disposing);
//        }
//    }
//}