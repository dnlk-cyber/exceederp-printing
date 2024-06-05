//﻿using System;
//using System.Collections.Generic;

//using System.Data.Entity;
//using System.Linq;
//using System.Net;
//using System.Web;
//using System.Web.Mvc;     
//using Kendo.Mvc.Extensions;
//using Kendo.Mvc.UI;
//using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
//using ExceedERP.DataAccess.Context;
//using ExceedERP.Core.Domain.FleetManagement;
//using ExceedERP.Web.Helpers;
//using ExceedERP.Core.Domain.HRM.HealthAndInsurance;

//namespace ExceedERP.Web.Areas.FleetManagement.Controllers
//{
//         [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
//    public class InsuranceTypeController : Controller
//    {
//        private ExceedDbContext db = new ExceedDbContext();

//        public ActionResult Index()
//        {
//            return View();
//        }

//        public ActionResult FleetInsuranceTypes_Read([DataSourceRequest]DataSourceRequest request)
//        {
//            IQueryable<InsuranceType> FleetInsuranceTypes = db.InsuranceType;
//            DataSourceResult result = FleetInsuranceTypes.ToDataSourceResult(request, insuranceType => new {
//                InsuranceTypeId = insuranceType.InsuranceTypeId,
//                Name = insuranceType.Name,
//                OrgBranchName = insuranceType.OrgBranchName,
//                DateCreated = insuranceType.DateCreated,
//                LastModified = insuranceType.LastModified,
//                CreatedBy = insuranceType.CreatedBy,
//                ModifiedBy = insuranceType.ModifiedBy
//            });

//            return Json(result);
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult FleetInsuranceTypes_Create([DataSourceRequest]DataSourceRequest request, FleetInsuranceType insuranceType)
//        {
//            var Types = db.FleetInsuranceTypes.Where(x => x.Name == insuranceType.Name).ToList();
//            if( Types.Any())
//            {
//                ModelState.AddModelError("", "Type Already Exist!");
//                return Json(new[] { insuranceType }.ToDataSourceResult(request, ModelState));
     
//            }

//            if (ModelState.IsValid)
//            {
//                var entity = new FleetInsuranceType
//                {
//                    Name = insuranceType.Name,
//                    Description = insuranceType.Description,
//                    OrgBranchName = insuranceType.OrgBranchName,
//                    DateCreated = DateTime.Now,
//                    LastModified = insuranceType.LastModified,
//                    CreatedBy = User.Identity.Name,
//                    ModifiedBy = insuranceType.ModifiedBy
//                };

//                db.FleetInsuranceTypes.Add(entity);
//                db.SaveChanges();
//                insuranceType.FleetInsuranceTypeId = entity.FleetInsuranceTypeId;
//            }

//            return Json(new[] { insuranceType }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult FleetInsuranceTypes_Update([DataSourceRequest]DataSourceRequest request, FleetInsuranceType insuranceType)
//        {
//            var Types = db.FleetInsuranceTypes.Where(x => x.Name == insuranceType.Name && x.FleetInsuranceTypeId != insuranceType.FleetInsuranceTypeId).ToList();
//            if (Types.Any())
//            {
//                ModelState.AddModelError("", "Type Already Exist!");
//                return Json(new[] { insuranceType }.ToDataSourceResult(request, ModelState));

//            }
//            if (ModelState.IsValid)
//            {
//                var entity = new FleetInsuranceType
//                {
//                    FleetInsuranceTypeId = insuranceType.FleetInsuranceTypeId,
//                    Name = insuranceType.Name,
//                    Description = insuranceType.Description,
//                    OrgBranchName = insuranceType.OrgBranchName,
//                    DateCreated = insuranceType.DateCreated,
//                    LastModified = insuranceType.LastModified,
//                    CreatedBy = insuranceType.CreatedBy,
//                    ModifiedBy = insuranceType.ModifiedBy
//                };

//                db.FleetInsuranceTypes.Attach(entity);
//                db.Entry(entity).State = EntityState.Modified;
//                db.SaveChanges();
//            }

//            return Json(new[] { insuranceType }.ToDataSourceResult(request, ModelState));
//        }

//        [AcceptVerbs(HttpVerbs.Post)]
//        public ActionResult FleetInsuranceTypes_Destroy([DataSourceRequest]DataSourceRequest request, FleetInsuranceType insuranceType)
//        {
//            if (ModelState.IsValid)
//            {
//                var entity = new FleetInsuranceType
//                {
//                    FleetInsuranceTypeId = insuranceType.FleetInsuranceTypeId,
//                    Name = insuranceType.Name,
//                    Description = insuranceType.Description,
//                    OrgBranchName = insuranceType.OrgBranchName,
//                    DateCreated = insuranceType.DateCreated,
//                    LastModified = insuranceType.LastModified,
//                    CreatedBy = insuranceType.CreatedBy,
//                    ModifiedBy = insuranceType.ModifiedBy
//                };

//                db.FleetInsuranceTypes.Attach(entity);
//                db.FleetInsuranceTypes.Remove(entity);
//                db.SaveChanges();
//            }

//            return Json(new[] { insuranceType }.ToDataSourceResult(request, ModelState));
//        }

//        protected override void Dispose(bool disposing)
//        {
//            db.Dispose();
//            base.Dispose(disposing);
//        }
//    }
//}
