using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Contract
{
    public class ContractClaimTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/ContractClaimType
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ClaimType_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ContractClaimType> claimTypes = db.ContractClaimType;
            DataSourceResult result = claimTypes.ToDataSourceResult(request, claimType => new {
                ContractClaimTypeId = claimType.ContractClaimTypeId,
                Name = claimType.Name,
                Description = claimType.Description,

                //OrgBranchName = claimTypes.OrgBranchName,
                //DateCreated = claimTypes.DateCreated,
                //LastModified = claimTypes.LastModified,
                //CreatedBy = claimTypes.CreatedBy,
                //ModifiedBy = claimTypes.ModifiedBy
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClaimType_Create([DataSourceRequest]DataSourceRequest request, ContractClaimType claimType)
        {
            if (ModelState.IsValid)
            {

                var entity = new ContractClaimType
                {
                    ContractClaimTypeId = claimType.ContractClaimTypeId,
                    Name = claimType.Name,
                    Description = claimType.Description,

                    //OrgBranchName = claimType.OrgBranchName,
                    //DateCreated = DateTime.Now,
                    //LastModified = claimType.LastModified,
                    //CreatedBy = User.Identity .Name,
                    //ModifiedBy = variationType.ModifiedBy
                };

                db.ContractClaimType.Add(entity);
                db.SaveChanges();
                claimType.ContractClaimTypeId = entity.ContractClaimTypeId;
            }

            return Json(new[] { claimType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClaimType_Update([DataSourceRequest]DataSourceRequest request, ContractClaimType claimType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractClaimType
                {
                    ContractClaimTypeId = claimType.ContractClaimTypeId,
                    Name = claimType.Name,
                    Description = claimType.Description,

                    //OrgBranchName = variationType.OrgBranchName,
                    //DateCreated = variationType.DateCreated,
                    //LastModified = DateTime.Now,
                    //CreatedBy = variationType.CreatedBy,
                    //ModifiedBy = User.Identity .Name
                };

                db.ContractClaimType.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { claimType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ClaimType_Destroy([DataSourceRequest]DataSourceRequest request, ContractClaimType claimType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractClaimType
                {
                    ContractClaimTypeId = claimType.ContractClaimTypeId,
                    Name = claimType.Name,
                    Description = claimType.Description,

                    //OrgBranchName = variationType.OrgBranchName,
                    //DateCreated = variationType.DateCreated,
                    //LastModified = variationType.LastModified,
                    //CreatedBy = variationType.CreatedBy,
                    //ModifiedBy = variationType.ModifiedBy
                };

                db.ContractClaimType.Attach(entity);
                db.ContractClaimType.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { claimType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}