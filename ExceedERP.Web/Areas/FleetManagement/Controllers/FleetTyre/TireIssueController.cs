using ExceedERP.ApplicationServices.FleetManagement.FleetTireManagement;
using System;
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
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class TireIssueController : Controller
    {
        ITireIssueService _TireIssueService;

        public TireIssueController()
        {
            _TireIssueService = new TireIssueService();
        }
        //
        // GET: /FleetManagement/TireIssue/
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult TireIssue_Read([DataSourceRequest] DataSourceRequest request)
        {
            var TireIssue = _TireIssueService.GetAll().Select(t => new TireIssue
                {

                FleetId = t.FleetId,
                EquipmentIdentityID = t.EquipmentIdentityID,
                EquipmentTypeID = t.EquipmentTypeID,
                EquipmentNameID = t.EquipmentNameID,
                    UnitPrice = t.UnitPrice,
                    OperatorName = t.OperatorName,
                    Brand = t.Brand,
                    PlateNumber = t.PlateNumber,
                    DescriptionReason = t.DescriptionReason,
                    InstalledDate = t.InstalledDate,
                    KMReading = t.KMReading,
                    //Quantity = t.Quantity,
                    SerialNo = t.SerialNo,
                    TireIssueID = t.TireIssueID,
                    TireSize = t.TireSize,
                    TireTube = t.TireTube,
                    TireType = t.TireType,


                }
                );

            return Json(TireIssue.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireIssue_Create([DataSourceRequest] DataSourceRequest request, TireIssue TireIssue)
        {
            IOperatorService _OperatorService = new OperatorService();
            IFleetService fleetService = new FleetService();
            Fleets fleet = fleetService.GetAll().Where(x => x.PlateNumber == TireIssue.PlateNumber).FirstOrDefault();
            var operatorName = _OperatorService.GetAll().Where(x => x.FleetsID != fleet.FleetsID).FirstOrDefault();
            TireIssue.OperatorName = operatorName.FullName;
            TireIssue.TireIssueID = Guid.NewGuid();
       
            if (TireIssue != null)
            {
                TireIssue.CreatedBy = User.Identity.Name;
                TireIssue.DateCreated = DateTime.Now;
              
                _TireIssueService.Insert(TireIssue);
                TireIssue entity = TireIssue;
                TireIssue = entity;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Tyre issue registration", "Success", User.Identity.Name, $"created tyre issue {entity.TireIssueID}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { TireIssue }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireIssue_Update([DataSourceRequest] DataSourceRequest request, TireIssue TireIssue)
        {
            if (TireIssue != null)
            {
                TireIssue.ModifiedBy = User.Identity.Name;
                TireIssue.LastModified = DateTime.Now;
                _TireIssueService.Update(TireIssue);
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Tyre issue registration", "Success", User.Identity.Name, $"updated tyre issue {TireIssue.TireIssueID}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { TireIssue }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TireIssue_Destroy([DataSourceRequest] DataSourceRequest request, TireIssue tireIssue)
        {
            var TireIssue = _TireIssueService.GetById(tireIssue.TireIssueID);
            if (TireIssue != null)
            {
                _TireIssueService.Delete(TireIssue);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted tyre issue {TireIssue.TireIssueID}");
                #endregion
            }

            return Json(new[] { TireIssue }.ToDataSourceResult(request));
        }



    }
}
