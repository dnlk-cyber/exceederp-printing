using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.Equipment_Plan_User, FleetManagementRoles.Equipment_Plan_Approver)]
    public class EquipmentPlanandEvaluationsController : Controller
    {
        IEquipmentPlanandEvaluationsService _EquipmentPlanandEvaluationsService;
        IFleetService _FleetService;
        ExceedDbContext db = new ExceedDbContext();
        public EquipmentPlanandEvaluationsController()
        {
            _FleetService = new FleetService();
            _EquipmentPlanandEvaluationsService = new EquipmentPlanandEvaluationsService();
        }
        //
        // GET: /FleetManagement/EquipmentPlanandEvaluations/
        public ActionResult Index()
        {
            //ViewData["EquipmentType"] = db.EquipmentType.ToList();
            //["EquipmentCategory"] = db.EquipmentIdentity.ToList();
            return View();
        }

        public ActionResult EquipmentPlanandEvaluations_Read([DataSourceRequest] DataSourceRequest request)
        {
            var EquipmentPlanandEvaluations = db.EquipmentPlanandEvaluation.Select(e => new
                {
                    RentalRate = e.RentalRate,
                    UnitOfMeasurement = e.UnitOfMeasurement,
                    PlanEquipmentsPittyCashExpenditure = e.PlanEquipmentsPittyCashExpenditure,
                    EquipmentPlanandEvaluationsID = e.EquipmentPlanandEvaluationsID,
                    Category = e.Category,
                    Type = e.Type,
                    EquipmentName = e.EquipmentName,
                    PlanDowntime = e.PlanDowntime,
                    PlanFuelConsumption = e.PlanFuelConsumption,
                    PlanIdletime = e.PlanIdletime,
                    PlanIncome = e.PlanIncome,
                    PlanTyreUtilization = e.PlanTyreUtilization,
                    PlanWorkingtime = e.PlanWorkingtime,
                    IsOnlineApproved = e.IsOnlineApproved,
                    IsSendForApproval = e.IsSendForApproval,
                    Status = e.Status,
                    DateCreated = e.DateCreated,
                    LastModified = e.LastModified,
                    CreatedBy = e.CreatedBy,
                    ModifiedBy = e.ModifiedBy


                });

            return Json(EquipmentPlanandEvaluations.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentPlanandEvaluations_Create([DataSourceRequest] DataSourceRequest request, EquipmentPlanandEvaluations EquipmentPlanandEvaluations)
        {
            ModelState.Remove("EquipmentPlanandEvaluationsID");
            _FleetService = new FleetService();
            IEquipmentTypeService _EquipmentTypeService = new EquipmentTypeService();
            EquipmentPlanandEvaluations.EquipmentPlanandEvaluationsID = Guid.NewGuid();
            var fleet= _FleetService.GetById(EquipmentPlanandEvaluations.FleetId);
            var EquipmentType = _EquipmentTypeService.GetById(EquipmentPlanandEvaluations.EquipmentTypeId);
            if (EquipmentType != null && fleet!= null)
            {
                if (EquipmentPlanandEvaluations != null)
                {
                    EquipmentPlanandEvaluations.Category = EquipmentType.EquipmentIdentity.EquipmentCategoryName;
                    EquipmentPlanandEvaluations.Type = EquipmentType.EquipmentTypeName;
                    EquipmentPlanandEvaluations.RentalRate = 0;
                    EquipmentPlanandEvaluations.EquipmentName = fleet.UnitNum + " - " + fleet.PlateNumber;
                    EquipmentPlanandEvaluations.CreatedBy = User.Identity.Name;
                    EquipmentPlanandEvaluations.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    EquipmentPlanandEvaluations.DateCreated = DateTime.Now;
                    db.EquipmentPlanandEvaluation.Add(EquipmentPlanandEvaluations);
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Equipment plan and evaluation registration", "Success", User.Identity.Name, $"created equipment plan and evaluation {EquipmentPlanandEvaluations.EquipmentPlanandEvaluationsID}", Modules.FleetManagement);
                    #endregion
                    //  _EquipmentPlanandEvaluationsService.Insert(EquipmentPlanandEvaluations);
                }
            }
            return Json(new[] { EquipmentPlanandEvaluations }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentPlanandEvaluations_Update([DataSourceRequest] DataSourceRequest request, EquipmentPlanandEvaluations EquipmentPlanandEvaluations)
        {
            _FleetService = new FleetService();
            IEquipmentTypeService _EquipmentTypeService = new EquipmentTypeService();
            EquipmentPlanandEvaluations.EquipmentPlanandEvaluationsID = Guid.NewGuid();
            var fleet = _FleetService.GetById(EquipmentPlanandEvaluations.FleetId);
        
            var EquipmentType = _EquipmentTypeService.GetById(EquipmentPlanandEvaluations.EquipmentCategoryId);
            if (EquipmentType != null)
            {
                if (EquipmentPlanandEvaluations != null)
                {
                    EquipmentPlanandEvaluations.Category = EquipmentType.EquipmentIdentity.EquipmentCategoryName;
                    EquipmentPlanandEvaluations.Type = EquipmentType.EquipmentTypeName;
                    EquipmentPlanandEvaluations.PlanIncome = EquipmentPlanandEvaluations.PlanIncome;
                    EquipmentPlanandEvaluations.EquipmentName = fleet.UnitNum + " - " + fleet.PlateNumber;
                    EquipmentPlanandEvaluations.RentalRate = 0;
                    EquipmentPlanandEvaluations.ModifiedBy = User.Identity.Name;
                    EquipmentPlanandEvaluations.LastModified = DateTime.Now;
                    EquipmentPlanandEvaluations.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                
                    _EquipmentPlanandEvaluationsService.Update(EquipmentPlanandEvaluations);
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Equipment plan and evaluation", "Success", User.Identity.Name, $"updated equipment plan and evaluation {EquipmentPlanandEvaluations.EquipmentPlanandEvaluationsID}", Modules.FleetManagement);
                    #endregion
                }
            }

            return Json(new[] { EquipmentPlanandEvaluations }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentPlanandEvaluations_Destroy([DataSourceRequest] DataSourceRequest request, EquipmentPlanandEvaluations equipmentPlanandEvaluations)
        {
            var EquipmentPlanandEvaluations = _EquipmentPlanandEvaluationsService.GetById(equipmentPlanandEvaluations.EquipmentPlanandEvaluationsID);
            if (EquipmentPlanandEvaluations != null)
            {
                _EquipmentPlanandEvaluationsService.Delete(EquipmentPlanandEvaluations);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted equipment plan and evaluations registration number " + equipmentPlanandEvaluations.EquipmentPlanandEvaluationsID);

            }

            return Json(new[] { EquipmentPlanandEvaluations }.ToDataSourceResult(request));
        }



    }
}

