﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
     [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class FleetInsuranceTypeDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FleetInsuranceTypeDetails_Read([DataSourceRequest]DataSourceRequest request, Guid Id)
        {
            IQueryable<FleetInsuranceTypeDetail> fleetinsurancetypedetails = db.FleetInsuranceTypeDetails.Where(x => x.InsuranceId ==  Id);
            DataSourceResult result = fleetinsurancetypedetails.ToDataSourceResult(request, fleetInsuranceTypeDetail => new {
                FleetInsuranceTypeDetailId = fleetInsuranceTypeDetail.FleetInsuranceTypeDetailId,
                Name = fleetInsuranceTypeDetail.Name,
                Description = fleetInsuranceTypeDetail.Description,
                OrgBranchName = fleetInsuranceTypeDetail.OrgBranchName,
                DateCreated = fleetInsuranceTypeDetail.DateCreated,
                LastModified = fleetInsuranceTypeDetail.LastModified,
                CreatedBy = fleetInsuranceTypeDetail.CreatedBy,
                ModifiedBy = fleetInsuranceTypeDetail.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetInsuranceTypeDetails_Create([DataSourceRequest]DataSourceRequest request, FleetInsuranceTypeDetail fleetInsuranceTypeDetail, Guid Id)
        {
            ModelState.Remove("InsuranceId");
            var InsuranceType = db.FleetInsuranceTypeDetails.Where(x => x.InsuranceId ==Id && x.Name == fleetInsuranceTypeDetail.Name).ToList();
            if (InsuranceType.Any())
            {
                ModelState.AddModelError("Dupicate", "Insurance Type Already Registered ");
                return Json(new[] { fleetInsuranceTypeDetail }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new FleetInsuranceTypeDetail
                {
                    Name = fleetInsuranceTypeDetail.Name,
                    Description = fleetInsuranceTypeDetail.Description,

                    InsuranceId =  Id ,
                    DateCreated = DateTime.Now,
                    LastModified = fleetInsuranceTypeDetail.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = fleetInsuranceTypeDetail.ModifiedBy
                };

                db.FleetInsuranceTypeDetails.Add(entity);
                db.SaveChanges();
                fleetInsuranceTypeDetail.FleetInsuranceTypeDetailId = entity.FleetInsuranceTypeDetailId;
            }

            return Json(new[] { fleetInsuranceTypeDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetInsuranceTypeDetails_Update([DataSourceRequest]DataSourceRequest request, FleetInsuranceTypeDetail fleetInsuranceTypeDetail, Guid Id)
        {
            var InsuranceType = db.FleetInsuranceTypeDetails.Where(x => x.InsuranceId == Id && x.Name == fleetInsuranceTypeDetail.Name).ToList();
          if(InsuranceType.Any())
          {
              ModelState.AddModelError("Dupicate", "Insurance Type Already Registered ");
              return Json(new[] { fleetInsuranceTypeDetail }.ToDataSourceResult(request, ModelState));
          }
            
            if (ModelState.IsValid)
            {
                var entity = new FleetInsuranceTypeDetail
                {
                    FleetInsuranceTypeDetailId = fleetInsuranceTypeDetail.FleetInsuranceTypeDetailId,
                    Name = fleetInsuranceTypeDetail.Name,
                    Description = fleetInsuranceTypeDetail.Description,
                    InsuranceId = Id,
                    DateCreated = fleetInsuranceTypeDetail.DateCreated,
                    LastModified = fleetInsuranceTypeDetail.LastModified,
                    CreatedBy = fleetInsuranceTypeDetail.CreatedBy,
                    ModifiedBy = fleetInsuranceTypeDetail.ModifiedBy
                };

                db.FleetInsuranceTypeDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { fleetInsuranceTypeDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetInsuranceTypeDetails_Destroy([DataSourceRequest]DataSourceRequest request, FleetInsuranceTypeDetail fleetInsuranceTypeDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetInsuranceTypeDetail
                {
                    FleetInsuranceTypeDetailId = fleetInsuranceTypeDetail.FleetInsuranceTypeDetailId,
                    Name = fleetInsuranceTypeDetail.Name,
                    Description = fleetInsuranceTypeDetail.Description,
                    OrgBranchName = fleetInsuranceTypeDetail.OrgBranchName,
                    DateCreated = fleetInsuranceTypeDetail.DateCreated,
                    LastModified = fleetInsuranceTypeDetail.LastModified,
                    CreatedBy = fleetInsuranceTypeDetail.CreatedBy,
                    ModifiedBy = fleetInsuranceTypeDetail.ModifiedBy
                };

                db.FleetInsuranceTypeDetails.Attach(entity);
                db.FleetInsuranceTypeDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { fleetInsuranceTypeDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
