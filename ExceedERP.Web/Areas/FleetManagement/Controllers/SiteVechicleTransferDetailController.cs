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

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class SiteVechicleTransferDetailController : Controller
    {
ISiteVechicleTransferDetailService _SiteVechicleTransferDetailService;

        public SiteVechicleTransferDetailController()
        {
            _SiteVechicleTransferDetailService = new SiteVechicleTransferDetailService();
        }
        //
        // GET: /FleetManagement/SiteVechicleTransferDetail/
        public ActionResult Index()
        {

            return View();
        }


        public JsonResult SiteVechicleTransferDetail_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            _SiteVechicleTransferDetailService = new SiteVechicleTransferDetailService();
            var oper = _SiteVechicleTransferDetailService.GetAll().Where(x=>x.SiteVechicleTransferId == Id);
            var Opers = oper.Select(o => new SiteVechicleTransferDetail
            {
                SiteVechicleTransferDetailId = o.SiteVechicleTransferDetailId,
                Type = o.Type,
                DateCreated = o.DateCreated,
                CreatedBy = o.CreatedBy,
                Description = o.Description,
               

            });

            return this.Json(Opers.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SiteVechicleTransferDetail_Create([DataSourceRequest] DataSourceRequest request, SiteVechicleTransferDetail SiteVechicleTransferDetail, Guid Id)
        {
            ModelState.Remove("SiteVechicleTransferDetailId");
            List<SiteVechicleTransferDetail> SiteVechicleTransferDetails = _SiteVechicleTransferDetailService.GetAll().Where(f => f.Type == SiteVechicleTransferDetail.Type).ToList();
            if (SiteVechicleTransferDetails.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Transfer with Type Already Exists ");
            }
            else
            {
                
                if (SiteVechicleTransferDetail != null)
                {
                    SiteVechicleTransferDetail.SiteVechicleTransferId = Id;
                    SiteVechicleTransferDetail.CreatedBy = User.Identity.Name;
                    SiteVechicleTransferDetail.DateCreated = DateTime.Now;
                    _SiteVechicleTransferDetailService.Insert(SiteVechicleTransferDetail);
                }
            }
            return this.Json(new[] { SiteVechicleTransferDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SiteVechicleTransferDetail_Update([DataSourceRequest] DataSourceRequest request, SiteVechicleTransferDetail SiteVechicleTransferDetail)
        {

            if (SiteVechicleTransferDetail != null)
            {
                SiteVechicleTransferDetail.ModifiedBy = User.Identity.Name;
                SiteVechicleTransferDetail.LastModified = DateTime.Now;
                _SiteVechicleTransferDetailService.Update(SiteVechicleTransferDetail);
            }


            return this.Json(new[] { SiteVechicleTransferDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult SiteVechicleTransferDetail_Destroy([DataSourceRequest] DataSourceRequest request, SiteVechicleTransferDetail SiteVechicleTransferDetail)
        {
            var SiteVechicleTransferDetails = _SiteVechicleTransferDetailService.GetById(SiteVechicleTransferDetail.SiteVechicleTransferDetailId);
            if (SiteVechicleTransferDetails != null)
            {
                _SiteVechicleTransferDetailService.Delete(SiteVechicleTransferDetails);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Maintenance Center number " + SiteVechicleTransferDetail.SiteVechicleTransferDetailId);

            }
            return Json(new[] { SiteVechicleTransferDetail }.ToDataSourceResult(request, ModelState));
        }
}}