using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
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
    public class ExtensionOfTimeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Construction/ExtensionOfTime
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EoT_Read([DataSourceRequest]DataSourceRequest request, int ctrId)
        {
            IQueryable<ExtensionOfTime> extensionofTime = db.ExtensionOfTime.Where(x => x.ProjectContractId == ctrId);
            DataSourceResult result = extensionofTime.ToDataSourceResult(request, eot => new {
                ExtensionOfTimeId = eot.ExtensionOfTimeId,
                ProjectContractId = eot.ProjectContractId,
                Reason = eot.Reason,
                Justification = eot.Justification,
                RequestedDays = eot.RequestedDays,
                ApprovedDays = eot.ApprovedDays,
                Comment = eot.Comment,
                EoTRequesteDate = eot.EoTRequesteDate,
                EoTApproveDate = eot.EoTApproveDate,

                OrgBranchName = eot.OrgBranchName,
                DateCreated = eot.DateCreated,
                LastModified = eot.LastModified,
                CreatedBy = eot.CreatedBy,
                ModifiedBy = eot.ModifiedBy
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EoT_Create([DataSourceRequest]DataSourceRequest request, ExtensionOfTime extensionOfTime, int idTwo)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(extensionOfTime.Reason)) {
                    extensionOfTime.Reason = "unknown reason";
                }
                var entity = new ExtensionOfTime
                {
                    ExtensionOfTimeId = extensionOfTime.ExtensionOfTimeId,
                    ProjectContractId = idTwo,
                    Reason = extensionOfTime.Reason,
                    Justification = extensionOfTime.Justification,
                    RequestedDays = extensionOfTime.RequestedDays,
                    ApprovedDays = extensionOfTime.ApprovedDays,
                    Comment = extensionOfTime.Comment,
                    EoTRequesteDate = extensionOfTime.EoTRequesteDate,
                    EoTApproveDate = extensionOfTime.EoTApproveDate,

                    OrgBranchName = extensionOfTime.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = extensionOfTime.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = extensionOfTime.ModifiedBy
                };

                db.ExtensionOfTime.Add(entity);
                db.SaveChanges();
                extensionOfTime.ExtensionOfTimeId = entity.ExtensionOfTimeId;

                ContractMgmtHelper.UpdateContract(entity.ProjectContractId);
            }

            return Json(new[] { extensionOfTime }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EoT_Update([DataSourceRequest]DataSourceRequest request, ExtensionOfTime extensionOfTime)
        {
            if (ModelState.IsValid)
            {
                var entity = new ExtensionOfTime
                {
                    ExtensionOfTimeId = extensionOfTime.ExtensionOfTimeId,
                    ProjectContractId = extensionOfTime.ProjectContractId,
                    Reason = extensionOfTime.Reason,
                    Justification = extensionOfTime.Justification,
                    RequestedDays = extensionOfTime.RequestedDays,
                    ApprovedDays = extensionOfTime.ApprovedDays,
                    Comment = extensionOfTime.Comment,
                    EoTRequesteDate = extensionOfTime.EoTRequesteDate,
                    EoTApproveDate = extensionOfTime.EoTApproveDate,

                    OrgBranchName = extensionOfTime.OrgBranchName,
                    DateCreated = extensionOfTime.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = extensionOfTime.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ExtensionOfTime.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ContractMgmtHelper.UpdateContract(entity.ProjectContractId);
            }

            return Json(new[] { extensionOfTime }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EoT_Destroy([DataSourceRequest]DataSourceRequest request, ExtensionOfTime extensionOfTime)
        {
            if (ModelState.IsValid)
            {
                var entity = new ExtensionOfTime
                {
                    ExtensionOfTimeId = extensionOfTime.ExtensionOfTimeId,
                    ProjectContractId = extensionOfTime.ProjectContractId,
                    Reason = extensionOfTime.Reason,
                    Justification = extensionOfTime.Justification,
                    RequestedDays = extensionOfTime.RequestedDays,
                    ApprovedDays = extensionOfTime.ApprovedDays,
                    Comment = extensionOfTime.Comment,
                    EoTRequesteDate = extensionOfTime.EoTRequesteDate,
                    EoTApproveDate = extensionOfTime.EoTApproveDate,

                    OrgBranchName = extensionOfTime.OrgBranchName,
                    DateCreated = extensionOfTime.DateCreated,
                    LastModified = extensionOfTime.LastModified,
                    CreatedBy = extensionOfTime.CreatedBy,
                    ModifiedBy = extensionOfTime.ModifiedBy
                };

                db.ExtensionOfTime.Attach(entity);
                db.ExtensionOfTime.Remove(entity);
                db.SaveChanges();

                ContractMgmtHelper.UpdateContract(entity.ProjectContractId);
            }

            return Json(new[] { extensionOfTime }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}