﻿using System;
 using System.Data.Entity;
﻿using System.Linq;
﻿using System.Web.Mvc;
﻿using ExceedERP.Core.Domain.Inventory;
﻿using ExceedERP.DataAccess.Context;
 using ExceedERP.Web.Areas.Inventory.Models;
 using ExceedERP.Web.Helpers;
 using Kendo.Mvc.Extensions;
﻿using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{
    
    public class RevaluationValidationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult RevaluationValidations_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<RevaluationValidation> reevaluationvalidations = db.RevaluationValidations.Where(i => i.ReEvaluationId == id);
            DataSourceResult result = reevaluationvalidations.ToDataSourceResult(request, reEvaluationValidation => new {
                RevaluationValidationId = reEvaluationValidation.RevaluationValidationId,
                ReEvaluationId = reEvaluationValidation.ReEvaluationId,
                Status = reEvaluationValidation.Status,
                Remark = reEvaluationValidation.Remark,
                OrgBranchName = reEvaluationValidation.OrgBranchName,
                DateCreated = reEvaluationValidation.DateCreated,
                LastModified = reEvaluationValidation.LastModified,
                CreatedBy = reEvaluationValidation.CreatedBy,
                ModifiedBy = reEvaluationValidation.ModifiedBy
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevaluationValidations_Create([DataSourceRequest]DataSourceRequest request, RevaluationValidation revaluationValidation , int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevaluationValidation
                {
                    ReEvaluationId = id,
                    Status = revaluationValidation.Status,
                    Remark = revaluationValidation.Remark,
                    OrgBranchName = revaluationValidation.OrgBranchName,
                  
                    LastModified = revaluationValidation.LastModified,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = revaluationValidation.ModifiedBy
                };

                db.RevaluationValidations.Add(entity);
                db.SaveChanges();
                revaluationValidation.RevaluationValidationId = entity.RevaluationValidationId;
            }

            return Json(new[] { revaluationValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevaluationValidations_Update([DataSourceRequest]DataSourceRequest request, RevaluationValidation revaluationValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevaluationValidation
                {
                    RevaluationValidationId = revaluationValidation.RevaluationValidationId,
                    ReEvaluationId = revaluationValidation.ReEvaluationId,
                    Status = revaluationValidation.Status,
                    Remark = revaluationValidation.Remark,
                    OrgBranchName = revaluationValidation.OrgBranchName,
                    DateCreated = revaluationValidation.DateCreated,
                    LastModified = revaluationValidation.LastModified,
                    CreatedBy = revaluationValidation.CreatedBy,
                    ModifiedBy = revaluationValidation.ModifiedBy
                };

                db.RevaluationValidations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { revaluationValidation }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyApprover)]
        public ActionResult RevaluationValidations_Destroy([DataSourceRequest]DataSourceRequest request, RevaluationValidation revaluationValidation)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevaluationValidation
                {
                    RevaluationValidationId = revaluationValidation.RevaluationValidationId,
                    ReEvaluationId = revaluationValidation.ReEvaluationId,
                    Status = revaluationValidation.Status,
                    Remark = revaluationValidation.Remark,
                    OrgBranchName = revaluationValidation.OrgBranchName,
                    DateCreated = revaluationValidation.DateCreated,
                    LastModified = revaluationValidation.LastModified,
                    CreatedBy = revaluationValidation.CreatedBy,
                    ModifiedBy = revaluationValidation.ModifiedBy
                };

                db.RevaluationValidations.Attach(entity);
                db.RevaluationValidations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { revaluationValidation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
