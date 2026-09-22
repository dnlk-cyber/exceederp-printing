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
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class TireIssueApprovalController : Controller
    {
        //
        // GET: /FleetManagement/TireIssueApproval/
                  ITireIssueService _TireIssueService;
                  ExceedDbContext db = new ExceedDbContext();
      
                  public TireIssueApprovalController()
        {
            _TireIssueService = new TireIssueService();
        }

                  public ActionResult Index()
                  {

                      return View();
                  }

         [AuthorizeRoles(FleetManagementRoles.TyreUser, FleetManagementRoles.TyreApprover)]

          public ActionResult TireIssueApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
          {

              Guid Id = Guid.Parse(EId);

              List<TireIssueApproval> fleetApproval = db.TireIssueApproval.Where(x => x.TireIssueId == Id).ToList();
              List<TireIssue> Category = db.TireIssue.Where(x => x.TireIssueID == Id).ToList();
              bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
              if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
              {
                  fleetApproval.Add(new TireIssueApproval()
                  {
                      
                      TireIssueId = Id,
                      Remark = string.Empty,
                      Status = MaintenanceConst.Pending,

                  });
              }
         

              return Json(fleetApproval.ToDataSourceResult(request, approval => new TireIssueApproval()
              {
                  TireIssueApprovalID = approval.TireIssueApprovalID,
                  TireIssueId = approval.TireIssueId,
                  Remark = approval.Remark,
                  Status = approval.Status,
                  DateCreated = DateTime.Now,
                  CreatedBy = User.Identity.Name


              }));
          }


            [AuthorizeRoles(FleetManagementRoles.TyreUser)] 
   
          public ActionResult TireIssue_SendForOnlineApproval(string EId)
          {

              Guid Id = Guid.Parse(EId);
              var TireIssue = db.TireIssue.Where(x => x.TireIssueID == Id).FirstOrDefault();
              if (TireIssue != null)
              {
                  TireIssue.IsSendForApproval = true;
                  TireIssue.Status = MaintenanceConst.WaitingApproval;
                  db.TireIssue.Attach(TireIssue);
                  db.Entry(TireIssue).State = EntityState.Modified;
                  db.SaveChanges();
              }


              return RedirectToActionPermanent("Index", "TyreManagementIssue", new { area = "FleetManagement" });
          }

           [AuthorizeRoles(FleetManagementRoles.TyreApprover)]
          [AcceptVerbs(HttpVerbs.Post)]
          public ActionResult TireIssue_Approve([DataSourceRequest]DataSourceRequest request, TireIssueApproval TireIssueApproval, string EId)
          {
              //try
              //{
                  Guid Id = Guid.Parse(EId);
                  if (TireIssueApproval.Status == MaintenanceConst.Approved)
                  {
                      var entity = db.TireIssue.Where(x => x.TireIssueID == Id).FirstOrDefault();
                    if (entity != null)
                    {
                        entity.IsOnlineApproved = true;
                        entity.OnlineApprovedBy = User.Identity.Name;
                        entity.OnlineApprovedTime = DateTime.Now;
                        entity.Status = MaintenanceConst.Approved;

                        db.TireIssue.Attach(entity);
                        db.Entry(entity).State = EntityState.Modified;
                      //  db.SaveChanges();
                 
                      var Return = db.TireReturn.Where(x => x.TireReturnID == entity.TireReturnId).FirstOrDefault();
                    if (Return != null)
                    {
                        Return.IsOnlineApproved = true;
                        Return.OnlineApprovedBy = User.Identity.Name;
                        Return.OnlineApprovedTime = DateTime.Now;
                        Return.Status = MaintenanceConst.Approved;

                        db.TireReturn.Attach(Return);
                        db.Entry(Return).State = EntityState.Modified;
                      //  db.SaveChanges();
                    }
                       var fleet = db.Fleets.Where(x => x.FleetsID == entity.FleetId).FirstOrDefault();
                      EquipmentTyre Tyre = db.EquipmentTyres.Where(x => x.fleetId == entity.FleetId && x.SerialNo == entity.SerialNo).FirstOrDefault();


                      var EquipmentTyre = new EquipmentTyre
                      {
                          EquipmentTyreId = Guid.NewGuid(),
                          Condition = "Good",
                          CreatedBy = User.Identity.Name,
                          DateCreated = DateTime.Now,
                          fleetId = fleet.FleetsID,
                          Make = entity.Brand,
                          MakeCountry = ".MakeCountry",
                          OdoMeter = decimal.Parse(fleet.CurentOdoMeter.ToString()),
                          SerialNo = entity.SerialNo,
                          Size = entity.TireSize,
                          Type = entity.TireType,
                          Tube = entity.TireTube,
                          Isattached = true,
                          UnitPrice = entity.UnitPrice
                          
                      };
                      db.EquipmentTyres.Add(EquipmentTyre);
                      db.SaveChanges();
                    }


                }
                  if (TireIssueApproval.Status == MaintenanceConst.Rejected)
                  {
                      var entity = db.TireIssue.Where(x => x.TireIssueID == Id).FirstOrDefault();

                      entity.IsSendForApproval = false;
                      
                      db.TireIssue.Attach(entity);
                      db.Entry(entity).State = EntityState.Modified;
                      db.SaveChanges();

                  }
                  var approval = new TireIssueApproval()
                  {
                      Status = TireIssueApproval.Status,
                      TireIssueId = Id,
                      Remark = TireIssueApproval.Remark,
                      DateCreated = DateTime.Now,
                      CreatedBy = User.Identity.Name

                  };

                  db.TireIssueApproval.Add(approval);
                  db.SaveChanges();
                  TireIssueApproval.TireIssueApprovalID = approval.TireIssueApprovalID;

            //  }
            //  catch (Exception e)
            //  {

                
            //    return Json(e.Message, JsonRequestBehavior.AllowGet);
            //}
              return Json(new[] { TireIssueApproval }.ToDataSourceResult(request));
          }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
 }