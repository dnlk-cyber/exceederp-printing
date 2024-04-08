﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Archive.Controllers.DepartmentToDepartment
{
    public class OutGoing_DepartmentToDepartmentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Departments"] = db.MainCompanyStructures.ToList();
            return View();
        }
        public ActionResult Index_Receiver()
        {
            ViewData["Departments"] = db.MainCompanyStructures.ToList();
            return View();
        }
        public ActionResult OutGoing_DepartmentToDepartments_Read([DataSourceRequest]DataSourceRequest request)
        {
            int orgId = 0;
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empId);
            if (placement != null)
            {
                orgId = placement.OrgStructureId;
            }

            var letters = db.OutGoing_DepartmentToDepartments.Where(x => x.OrgstructureId == orgId).ToList();
            DataSourceResult result = letters.ToDataSourceResult(request, outGoing_DepartmentToDepartment => new {
                OutGoing_DepartmentToDepartmentId = outGoing_DepartmentToDepartment.OutGoing_DepartmentToDepartmentId,
                OrgstructureId = outGoing_DepartmentToDepartment.OrgstructureId,
                IsForAll = outGoing_DepartmentToDepartment.IsForAll,
                IsSent = outGoing_DepartmentToDepartment.IsSent,
                Message = outGoing_DepartmentToDepartment.Message,
                Date = outGoing_DepartmentToDepartment.Date,
                DateEt = outGoing_DepartmentToDepartment.DateEt,
                Time = outGoing_DepartmentToDepartment.Time,
            });

            return Json(result);
        }
        public ActionResult OutGoing_DepartmentToDepartments_Sent_Read([DataSourceRequest]DataSourceRequest request)
        {
            int orgId = 0;
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empId);
            if(placement != null)
            {
                orgId = placement.OrgStructureId;
            }
            var sentToList = db.Receiver_DepartmentToDepartments.Where(x => x.OrgStructureId == orgId).Select(x => x.OutGoing_DepartmentToDepartmentId).ToList();

            var letters = db.OutGoing_DepartmentToDepartments.Where(x=>x.IsForAll || sentToList.Contains(x.OutGoing_DepartmentToDepartmentId)).Where(x => x.IsSent).ToList();

            DataSourceResult result = letters.ToDataSourceResult(request, outGoing_DepartmentToDepartment => new {
                OutGoing_DepartmentToDepartmentId = outGoing_DepartmentToDepartment.OutGoing_DepartmentToDepartmentId,
                OrgstructureId = outGoing_DepartmentToDepartment.OrgstructureId,
                IsForAll = outGoing_DepartmentToDepartment.IsForAll,
                IsSent = outGoing_DepartmentToDepartment.IsSent,
                Message = outGoing_DepartmentToDepartment.Message,
                Date = outGoing_DepartmentToDepartment.Date,
                DateEt = outGoing_DepartmentToDepartment.DateEt,
                Time = outGoing_DepartmentToDepartment.Time,
            });

            return Json(result);
        }
        //D:\Projects\ExceedERP-HR\ExceedERP.Web\Content\Files\Archive\
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutGoing_DepartmentToDepartments_Create([DataSourceRequest]DataSourceRequest request, OutGoing_DepartmentToDepartment outGoing_DepartmentToDepartment)
        {
            string username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);
            var placement = db.Placements.FirstOrDefault(x => x.isCurrent && x.EmployeeId == empId);
            if(placement != null)
            {
                outGoing_DepartmentToDepartment.OrgstructureId = placement.OrgStructureId;
            }
            if (ModelState.IsValid)
            {
                var entity = new OutGoing_DepartmentToDepartment
                {
                    OrgstructureId = outGoing_DepartmentToDepartment.OrgstructureId,
                    IsForAll = outGoing_DepartmentToDepartment.IsForAll,
                    Message = outGoing_DepartmentToDepartment.Message,
                    Date = DateTime.Now,
                    DateEt = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day,DateTime.Now.Month,DateTime.Now.Year),
                    Time = DateTime.Now.ToShortTimeString()
                };

                db.OutGoing_DepartmentToDepartments.Add(entity);
                db.SaveChanges();
                outGoing_DepartmentToDepartment.OutGoing_DepartmentToDepartmentId = entity.OutGoing_DepartmentToDepartmentId;
            }

            return Json(new[] { outGoing_DepartmentToDepartment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutGoing_DepartmentToDepartments_Update([DataSourceRequest]DataSourceRequest request, OutGoing_DepartmentToDepartment outGoing_DepartmentToDepartment)
        {
            if (ModelState.IsValid)
            {
                var entity = new OutGoing_DepartmentToDepartment
                {
                    OutGoing_DepartmentToDepartmentId = outGoing_DepartmentToDepartment.OutGoing_DepartmentToDepartmentId,
                    OrgstructureId = outGoing_DepartmentToDepartment.OrgstructureId,
                    IsForAll = outGoing_DepartmentToDepartment.IsForAll,
                    IsSent = outGoing_DepartmentToDepartment.IsSent,
                    Message = outGoing_DepartmentToDepartment.Message,
                    Date = outGoing_DepartmentToDepartment.Date,
                    DateEt = outGoing_DepartmentToDepartment.DateEt,
                    Time = outGoing_DepartmentToDepartment.Time,
                };

                db.OutGoing_DepartmentToDepartments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { outGoing_DepartmentToDepartment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult OutGoing_DepartmentToDepartments_Destroy([DataSourceRequest]DataSourceRequest request, OutGoing_DepartmentToDepartment outGoing_DepartmentToDepartment)
        {
            if (ModelState.IsValid)
            {
                var entity = new OutGoing_DepartmentToDepartment
                {
                    OutGoing_DepartmentToDepartmentId = outGoing_DepartmentToDepartment.OutGoing_DepartmentToDepartmentId,
                    OrgstructureId = outGoing_DepartmentToDepartment.OrgstructureId,
                    IsForAll = outGoing_DepartmentToDepartment.IsForAll,
                    Message = outGoing_DepartmentToDepartment.Message,
                    Date = outGoing_DepartmentToDepartment.Date,
                    DateEt = outGoing_DepartmentToDepartment.DateEt,
                    Time = outGoing_DepartmentToDepartment.Time,
                };

                db.OutGoing_DepartmentToDepartments.Attach(entity);
                db.OutGoing_DepartmentToDepartments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { outGoing_DepartmentToDepartment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
