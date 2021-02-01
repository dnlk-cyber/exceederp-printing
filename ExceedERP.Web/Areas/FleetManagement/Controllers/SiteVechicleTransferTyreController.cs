using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class SiteVechicleTransferTyreController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

public SiteVechicleTransferTyreController()
        {
         }
        //
        // GET: /FleetManagement/SiteVechicleTransferTyreDetail/
        public ActionResult Index()
        {

            return View();
        }


        public JsonResult SiteVechicleTransferTyre_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {

            var SiteVechicleTransferTyreDetailes = db.SiteVechicleTransferTyreDetailes.Where(x => x.SiteVechicleTransferId == Id).ToList();


            return this.Json(SiteVechicleTransferTyreDetailes.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SiteVechicleTransferTyre_Create([DataSourceRequest] DataSourceRequest request, SiteVechicleTransferTyreDetail SiteVechicleTransferTyreDetail, Guid Id)
        {
            ModelState.Remove("SiteVechicleTransferTyreId");
            ModelState.Remove("SiteVechicleTransferId");
            List<SiteVechicleTransferTyreDetail> SiteVechicleTransferTyres = db.SiteVechicleTransferTyreDetailes.Where(f => f.Type == SiteVechicleTransferTyreDetail.Type).ToList();
            if (SiteVechicleTransferTyres.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Transfer with Type Already Exists ");
            }
            else
            {
                
                if (SiteVechicleTransferTyreDetail != null)
                {
                    SiteVechicleTransferTyreDetail.SiteVechicleTransferId = Id;
                    SiteVechicleTransferTyreDetail.CreatedBy = User.Identity.Name;
                    SiteVechicleTransferTyreDetail.DateCreated = DateTime.Now;
                    db.SiteVechicleTransferTyreDetailes.Add(SiteVechicleTransferTyreDetail);
                    db.SaveChanges();
                }
            }
            return this.Json(new[] { SiteVechicleTransferTyreDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SiteVechicleTransferTyre_Update([DataSourceRequest] DataSourceRequest request, SiteVechicleTransferTyreDetail SiteVechicleTransferTyreDetail)
        {

            if (SiteVechicleTransferTyreDetail != null)
            {
                SiteVechicleTransferTyreDetail.ModifiedBy = User.Identity.Name;
                SiteVechicleTransferTyreDetail.LastModified = DateTime.Now;
             //   _SiteVechicleTransferTyreService.Update(SiteVechicleTransferTyreDetail);
            }


            return this.Json(new[] { SiteVechicleTransferTyreDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SiteVechicleTransferTyre_Destroy([DataSourceRequest] DataSourceRequest request, SiteVechicleTransferTyreDetail SiteVechicleTransferTyreDetail)
        {
            var SiteVechicleTransferTyres = db.SiteVechicleTransferTyreDetailes.Where(x=>x.SiteVechicleTransferTyreDetailId == SiteVechicleTransferTyreDetail.SiteVechicleTransferTyreDetailId).FirstOrDefault();
            if (SiteVechicleTransferTyres != null)
            {
              //  _SiteVechicleTransferTyreService.Delete(SiteVechicleTransferTyres);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted   SiteVechicleTransferTyreDetail number " + SiteVechicleTransferTyreDetail.SiteVechicleTransferTyreDetailId);

            }
            return Json(new[] { SiteVechicleTransferTyreDetail }.ToDataSourceResult(request, ModelState));
        }
}}