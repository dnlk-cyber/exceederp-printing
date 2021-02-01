using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.Finance.Common;

namespace ExceedERP.Web.Areas.CRM.Controllers
    {
       [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class ReceiveMethodController : Controller
        {

            private ExceedDbContext db = new ExceedDbContext();
            public ActionResult Index()
            {
           
                return View();
            }

            public ActionResult ReceiveMethods_Read([DataSourceRequest]DataSourceRequest request)
            {
                IQueryable<PaymentMethod> ReceiveMethods = db.PaymentMethods;
                DataSourceResult result = ReceiveMethods.ToDataSourceResult(request, ReceiveMethod => new
                {
                    ReceiveMethodID = ReceiveMethod.PaymentMethodID,
                    Name = ReceiveMethod.Method,
                    Description = ReceiveMethod.Description,
                    CreatedBy = ReceiveMethod.CreatedBy,
                    DateCreated = ReceiveMethod.DateCreated,
                    LastModified = ReceiveMethod.LastModified,
                    ModifiedBy = ReceiveMethod.ModifiedBy,
                   
                    OrgBranchName = ReceiveMethod.OrgBranchName


               
                });

                return Json(result);
            }

            [AcceptVerbs(HttpVerbs.Post)]
            public ActionResult ReceiveMethods_Create([DataSourceRequest]DataSourceRequest request, ReceiveMethod ReceiveMethod)
            {
                if (ModelState.IsValid)
                    {
                        var entity = new ReceiveMethod
                            {
                                Name = ReceiveMethod.Name,
                                Description = ReceiveMethod.Description

                            };
                                db.ReceiveMethod.Add(entity);
                  db.SaveChanges();
                  ReceiveMethod.ReceiveMethodID = entity.ReceiveMethodID;
                            
                    }
                return Json(new[] { ReceiveMethod }.ToDataSourceResult(request, ModelState));
            }

            [AcceptVerbs(HttpVerbs.Post)]
            public ActionResult ReceiveMethods_Update([DataSourceRequest]DataSourceRequest request, ReceiveMethod ReceiveMethod)
            {
                if (ModelState.IsValid)
                {
                    var entity = new ReceiveMethod
                    {
                        ReceiveMethodID = ReceiveMethod.ReceiveMethodID,
                        Name = ReceiveMethod.Name,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Description = ReceiveMethod.Description
                       
                    };

                    db.ReceiveMethod.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return Json(new[] { ReceiveMethod }.ToDataSourceResult(request, ModelState));
            }

            [AcceptVerbs(HttpVerbs.Post)]
            public ActionResult ReceiveMethods_Destroy([DataSourceRequest]DataSourceRequest request, ReceiveMethod ReceiveMethod)
            {
                if (ModelState.IsValid)
                {
                    var entity = new ReceiveMethod
                    {
                        ReceiveMethodID = ReceiveMethod.ReceiveMethodID,
                        Name = ReceiveMethod.Name,
                        Description = ReceiveMethod.Description
                    };

                    db.ReceiveMethod.Attach(entity);
                    db.ReceiveMethod.Remove(entity);
                    db.SaveChanges();

                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted  Receive Method number " + ReceiveMethod.ReceiveMethodID);
      
                }

                return Json(new[] { ReceiveMethod }.ToDataSourceResult(request, ModelState));
            }
            public ActionResult ReceiveMethodDDBind()
            {

                var PaymentMethods = db.PaymentMethods.Where(x => x.Enable == true).Select(s => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
                {
                    Name = s.Method ,
                    Id = s.PaymentMethodID
                });

                return Json(PaymentMethods, JsonRequestBehavior.AllowGet);
            }

            protected override void Dispose(bool disposing)
            {
                db.Dispose();
                base.Dispose(disposing);
            }
        }
}