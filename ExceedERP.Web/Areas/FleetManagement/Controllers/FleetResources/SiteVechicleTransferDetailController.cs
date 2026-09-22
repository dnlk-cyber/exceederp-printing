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
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class SiteVechicleTransferDetailController : Controller
    {
ISiteVechicleTransferDetailService _SiteVechicleTransferDetailService;
        ExceedDbContext db = new ExceedDbContext();
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
            var oper =db.SiteVechicleTransferDetails.Where(x=>x.SiteVechicleTransferId == Id);
            var Opers = oper.Select(o => new SiteVechicleTransferDetail
            {
                SiteVechicleTransferDetailId = o.SiteVechicleTransferDetailId,
                SiteVechicleTransferId = o.SiteVechicleTransferId,
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
            ModelState.Remove("SiteVechicleTransferId");
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
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer registration", "Success", User.Identity.Name, $"created site vehicle transfer {SiteVechicleTransferDetail.SiteVechicleTransferDetailId}", Modules.FleetManagement);
                    #endregion
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
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer registration", "Success", User.Identity.Name, $"created site vehicle transfer {SiteVechicleTransferDetail.SiteVechicleTransferDetailId}", Modules.FleetManagement);
                #endregion
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
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted vehicle site transfer detail" + SiteVechicleTransferDetail.SiteVechicleTransferDetailId);

            }
            return Json(new[] { SiteVechicleTransferDetail }.ToDataSourceResult(request, ModelState));
        }

}}