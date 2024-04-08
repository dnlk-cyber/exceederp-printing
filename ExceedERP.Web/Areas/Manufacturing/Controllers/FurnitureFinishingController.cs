﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class FurnitureFinishingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["JobType"] = db.FurnitureStandardJobTypes.Select(s => new
            {
                Value = s.FurnitureStandardJobTypeId,
                Text = s.JobTypeName
            }).ToList();
            ViewData["TaskCategory"] = db.ManifucturingTaskCategories.Select(s => new
            {
                Value = s.ManifucturingTaskCategoryId,
                Text = s.Name
            });
            ViewData["Employee"] = db.Person_Employee.Select(s => new
            {
                Text = s.FirstName + "-" + s.MiddleName + "-" + s.LastName,
                Value = s.EmployeeId
            });
            ViewData["Customer"] = db.OrganizationCustomers.Select(s => new
            {
                Value = s.OrganizationCustomerID,
                Text = s.TradeName
            }).ToList();
            return View();
        }

        public ActionResult FurnitureJobOrderProductionsFinishing_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJobOrderProduction> furniturejoborderproductions = db.FurnitureJobOrderProductions.Where(x => x.IsSendForFinishing);
            DataSourceResult result = furniturejoborderproductions.ToDataSourceResult(request, furnitureJobOrderProduction => new {
                FurnitureJobOrderProductionId = furnitureJobOrderProduction.FurnitureJobOrderProductionId,
                CustomerId = furnitureJobOrderProduction.CustomerId,
                JobTypeId = furnitureJobOrderProduction.JobTypeId,
                JobNo = furnitureJobOrderProduction.JobNo,
                Unit = furnitureJobOrderProduction.Unit,
                Quantity = furnitureJobOrderProduction.Quantity,
                DeliveryDate = furnitureJobOrderProduction.DeliveryDate,
                FurnitureJobOrderFormId = furnitureJobOrderProduction.FurnitureJobOrderFormId,
                IsSendForFinishing = furnitureJobOrderProduction.IsSendForFinishing,
                SendForFinishingBy = furnitureJobOrderProduction.SendForFinishingBy,
                SendForFinishingDate = furnitureJobOrderProduction.SendForFinishingDate,
                IsClosed = furnitureJobOrderProduction.IsClosed,
                ClosedBy = furnitureJobOrderProduction.ClosedBy,
                DateClosed = furnitureJobOrderProduction.DateClosed,
                Status = furnitureJobOrderProduction.Status,
                Remark = furnitureJobOrderProduction.Remark,
                IsVoid = furnitureJobOrderProduction.IsVoid,
                VoidBy = furnitureJobOrderProduction.VoidBy,
                VoidTime = furnitureJobOrderProduction.VoidTime,
                IsOnlineApproved = furnitureJobOrderProduction.IsOnlineApproved,
                OnlineApprovedBy = furnitureJobOrderProduction.OnlineApprovedBy,
                OnlineApprovedTime = furnitureJobOrderProduction.OnlineApprovedTime,
                IsOnlineTransferred = furnitureJobOrderProduction.IsOnlineTransferred,
                IsOnlineTransferredBy = furnitureJobOrderProduction.IsOnlineTransferredBy,
                OnlineTransferredTime = furnitureJobOrderProduction.OnlineTransferredTime,
                IsSendForApproval = furnitureJobOrderProduction.IsSendForApproval,
                OrgBranchName = furnitureJobOrderProduction.OrgBranchName,
                SendForApprovalBy = furnitureJobOrderProduction.SendForApprovalBy,
                SendForApprovalTime = furnitureJobOrderProduction.SendForApprovalTime,
                DateCreated = furnitureJobOrderProduction.DateCreated,
                LastModified = furnitureJobOrderProduction.LastModified,
                CreatedBy = furnitureJobOrderProduction.CreatedBy,
                ModifiedBy = furnitureJobOrderProduction.ModifiedBy
            });

            return Json(result);
        }

        #region Approval
        public JsonResult CloseJob(int id)
        {
            object response = null;
            var production = db.FurnitureJobOrderProductions.Find(id);

            if (production != null)
            {

                production.IsClosed = true;
                production.ClosedBy = User.Identity.Name;
                production.DateClosed = DateTime.Today;

                db.FurnitureJobOrderProductions.Attach(production);
                db.Entry(production).State = EntityState.Modified;
                db.SaveChanges();
                response = new { Success = true, Message = "Furniture production closed successfully!" };

            }
            else
            {
                response = new { Success = false, Message = "Something is wrong!" };
            }
            return Json(response);
        }
        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
