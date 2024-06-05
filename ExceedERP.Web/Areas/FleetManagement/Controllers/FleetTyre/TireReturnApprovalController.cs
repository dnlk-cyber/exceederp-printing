using ExceedERP.ApplicationServices.FleetManagement.FleetTireManagement;



using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class TireReturnApprovalController : Controller
    {
        //
        // GET: /FleetManagement/TireReturnApproval/
        ITireReturnService _TireReturnService;
        ExceedDbContext db = new ExceedDbContext();

        public TireReturnApprovalController()
        {
            _TireReturnService = new TireReturnService();
        }
          [AuthorizeRoles(FleetManagementRoles.TyreUser, FleetManagementRoles.TyreApprover)] 
  
        public ActionResult Index()
        {

            return View();
        }

        //  [AuthorizeRoles(FleetManagementRoles.TyreUser, FleetManagementRoles.TyreApprover)]
          public ActionResult TireReturnApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
          {

              Guid Id = Guid.Parse(EId);

              List<TireReturnApproval> fleetApproval = db.TireReturnApproval.Where(x => x.TireReturnId == Id).ToList();
              List<TireReturn> Category = db.TireReturn.Where(x => x.TireReturnID == Id).ToList();
              bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
              if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
              {
                  fleetApproval.Add(new TireReturnApproval()
                  {
                      TireReturnId = Id,
                      Remark = string.Empty,
                      Status = MaintenanceConst.Pending,

                  });
              }


              return Json(fleetApproval.ToDataSourceResult(request, approval => new TireReturnApproval()
              {
                  TireReturnApprovalID = approval.TireReturnApprovalID,
                  TireReturnId = approval.TireReturnId,
                  Remark = approval.Remark,
                  Status = approval.Status,
                  DateCreated = DateTime.Now,
                  CreatedBy = User.Identity.Name


              }));
          }



          public ActionResult TireReturn_SendForOnlineApproval(string EId)
          {

              Guid Id = Guid.Parse(EId);
              var TireReturn = db.TireReturn.Where(x => x.TireReturnID == Id).FirstOrDefault();
              if (TireReturn != null)
              {
                  TireReturn.IsSendForApproval = true;
                  TireReturn.Status = MaintenanceConst.WaitingApproval;
                  db.TireReturn.Attach(TireReturn);
                  db.Entry(TireReturn).State = EntityState.Modified;
                  db.SaveChanges();
              }


              return RedirectToActionPermanent("Index", "TyreManagementReturn", new { area = "FleetManagement" });
          }

          //        [AuthorizeRoles(FleetManagementRoles.TireReturnApprover)]
          [AcceptVerbs(HttpVerbs.Post)]
          public ActionResult TireReturn_Approve([DataSourceRequest]DataSourceRequest request, TireReturnApproval TireReturnApproval, string EId)
          {
              //try
              //{
                  Guid Id = Guid.Parse(EId);
                  if (TireReturnApproval.Status == MaintenanceConst.Approved)
                  {
                      var entity = db.TireReturn.Where(x => x.TireReturnID == Id).FirstOrDefault();

                      entity.IsOnlineApproved = true;
                      entity.OnlineApprovedBy = User.Identity.Name;
                      entity.OnlineApprovedTime = DateTime.Now;
                      entity.Status = MaintenanceConst.Approved;

                      db.TireReturn.Attach(entity);
                      db.Entry(entity).State = EntityState.Modified;
                      //db.SaveChanges();

                      IFleetService fleetService = new FleetService();
                      Fleets fleet = fleetService.GetAll().Where(x => x.FleetsID == entity.FleetId).FirstOrDefault();
                      EquipmentTyre Tyre = db.EquipmentTyres.Where(x => x.fleetId == fleet.FleetsID).FirstOrDefault();

                      Tyre.Isattached = false;

                      db.EquipmentTyres.Attach(Tyre);
                      db.Entry(Tyre).State = EntityState.Modified;
                    //  db.SaveChanges();

                      var tyrelog = new EquipmentTyreLog
                      {
                          EquipmentTyreLogId = Guid.NewGuid(),
                          FleetId = fleet.FleetsID,
                          StoreCode = entity.StoreCode,
                          Equipment = fleet.UnitNum +" - "+ fleet.PlateNumber,
                          Condition = "",
                          CreatedBy = User.Identity.Name,
                          IntialOdoMeter = Tyre.OdoMeter,
                          FinalOdoMeter = decimal.Parse(fleet.CurentOdoMeter.ToString()),
                          DateCreated = DateTime.Now,
                          Make = entity.Brand,
                          ProjectName = fleet.Location,
                          Reason = entity.DescriptionReason,
                          SerialNo = entity.SerialNo.ToString(),
                          Size = entity.TireSize,
                          Type = entity.TireType,
                          UnitPrice = entity.UnitPrice,
                          DiffrenceOdoMeter = decimal.Parse(fleet.CurentOdoMeter.ToString()) - Tyre.OdoMeter,
                          Efficency = (decimal.Parse(fleet.CurentOdoMeter.ToString()) - Tyre.OdoMeter) / entity.UnitPrice

                      };
                      db.EquipmentTyreLog.Add(tyrelog);
                  }
                  if (TireReturnApproval.Status == MaintenanceConst.Rejected)
                  {
                     
                    
                      var entity = db.TireReturn.Where(x => x.TireReturnID == Id).FirstOrDefault();
                      entity.IsSendForApproval = false;

                      db.TireReturn.Attach(entity);
                      db.Entry(entity).State = EntityState.Modified;
                      db.SaveChanges();

                  }
                
                  var approval = new TireReturnApproval()
                  {

                      Status = TireReturnApproval.Status,
                      TireReturnId = Id,
                      Remark = TireReturnApproval.Remark,
                      DateCreated = DateTime.Now,
                      CreatedBy = User.Identity.Name

                  };

                  db.TireReturnApproval.Add(approval);
                  db.SaveChanges();
                  TireReturnApproval.TireReturnApprovalID = approval.TireReturnApprovalID;

              //}
              //catch (Exception e)
              //{

              //    string ee = e.Message;
              //}
              return Json(new[] { TireReturnApproval }.ToDataSourceResult(request));
          }




    }
}