using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Hubs;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    //[AuthorizeRoles(MaintenanceRoles.MaintenanceRequestDepartmentApprover)]
    public class CanibalizationDepartmentController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();


        // GET: /FleetManagement/JobRequest/
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult ApproveRequest(int id)
        {
           
            var Request = db.Canibalizations.FirstOrDefault(x => x.CanibalizationId == id);
            if (Request != null/* && fleet != null*/)
            {
                Request.IsSecondOnlineApproved = true;
                Request.SecondOnlineApprovedBy = User.Identity.Name;
                Request.SecondOnlineApprovedTime = DateTime.Now;
                // Notify.NotifyUser(new NotifModels { DeadLine = DateTime.Now, Message = "Request with No " + Request.Num + " Approved ", Title = " Approved Job Request ", URL = Url.Action("Index", "JobRequest", new { area = "FleetManagment" }) });

                Request.Status = ApprovalStatuses.Approve.ToString();
                db.Canibalizations.Attach(Request);
                db.Entry(Request).State = EntityState.Modified;
                db.SaveChanges();
                var CanApproval = db.CanibalizationDepartmentApproval.FirstOrDefault(x => x.CanibalizationId == id);
                if (CanApproval != null)
                {
                    CanApproval.CanibalizationId = Request.CanibalizationId;
                    CanApproval.Status = ApprovalStatuses.Approve;
                    CanApproval.CreatedBy = User.Identity.Name;
                    CanApproval.DateCreated = DateTime.Now;
                    db.Entry(CanApproval).State = EntityState.Modified;
                    db.SaveChanges();
                }


            }
            var formattedData = new
            {
                Message = "Approved Successefuly.",
                Status = "Approved"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Admin_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            var Admin = new List<CanibalizationDepartmentApproval>();
            var Request = db.Canibalizations.Where(x => x.CanibalizationId.ToString() == Id).FirstOrDefault();
            var CheckApproval = db.CanibalizationDepartmentApproval.Where(x => x.CanibalizationId.ToString() == Id).ToList(); 
            if (CheckApproval.Count() == 0 && Request != null && Request.IsSecondSendForApproval && Request.IsOnlineApproved && !Request.IsSecondOnlineApproved)
            {
                var adminApproval = new CanibalizationDepartmentApproval();
                adminApproval.CanibalizationDepartmentApprovalId = adminApproval.CanibalizationDepartmentApprovalId;
                adminApproval.CanibalizationId = int.Parse(Id);
                adminApproval.Status = ApprovalStatuses.Pending;
                adminApproval.Remark = string.Empty;
               // adminApproval.Desk = "desc";
                adminApproval.CreatedBy = User.Identity.Name;
                adminApproval.DateCreated = DateTime.Now;
                db.CanibalizationDepartmentApproval.Add(adminApproval);
                db.SaveChanges();
            }
            Admin = db.CanibalizationDepartmentApproval.Where(x => x.CanibalizationId.ToString() == Id).ToList();

            return Json(Admin.ToDataSourceResult(request));
        }

        public ActionResult RejectRequest(string id)

        {
            try
            {

                var Request = db.Canibalizations.Where(x => x.CanibalizationId.ToString() == id).FirstOrDefault();
                if (Request != null)
                {

                    Request.IsSecondSendForApproval = false;
                    Request.IsSecondOnlineApproved = false;
                    Request.Status = MaintenanceConst.Rejected;
                    db.Canibalizations.Attach(Request);
                    db.Entry(Request).State = EntityState.Modified;
                    db.SaveChanges();
                    var adminApproval = db.CanibalizationDepartmentApproval.FirstOrDefault(x => x.CanibalizationId.ToString() == id);
                    adminApproval.Status = ApprovalStatuses.Reject;
                    // adminApproval.Remark = model.Remark;
                    db.Entry(adminApproval).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }

            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);

            }
            var formattedData = new
            {
                Message = "Rejected Successefuly.",
                Status = "Rejected"
            };
            return Json("Rejected Successefuly.", JsonRequestBehavior.AllowGet);
        }

        public JsonResult SendForDepartmentApproval(int id)
        {
            var record = db.Canibalizations.Find(id);
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var checkItems = db.CanibalizedParts.Where(sli => sli.CanibalizationId == id).ToList();
            if (!checkItems.Any())
            {
                var formattedData = new
                {
                    Message = "You have to Add Items under The Lot",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if(record.IsSecondOnlineApproved)
            {

                var formattedData = new
                {
                    Message = "Record already approved",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record.IsSecondSendForApproval)
            {

                var formattedData = new
                {
                    Message = "Record is already sent for department approval",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            } if(!record.IsOnlineApproved)
            {

                var formattedData = new
                {
                    Message = "Record is not first level approved",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (checkItems.Any())
            {
                if (record != null && record.IsOnlineApproved && !record.IsSecondOnlineApproved && !record.IsSecondSendForApproval)
                {

                    record.Status = Enum.GetName(typeof(OperationStatuses), 16);
                    record.IsSecondSendForApproval = true;
                    record.SecondSendForApprovalBy = User.Identity.Name;
                    record.SecondSendForApprovalTime = DateTime.Now;
                    db.Canibalizations.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedData = new
                    {
                        Message = "Successfully Sent.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
            }


            if (record.IsSecondSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for department approval.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsSecondOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;


        }





    }



}