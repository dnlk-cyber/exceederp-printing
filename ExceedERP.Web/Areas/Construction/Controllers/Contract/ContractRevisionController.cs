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
    public class ContractRevisionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult ContractRevision_Read([DataSourceRequest]DataSourceRequest request, int ctrId)
        {
            IQueryable<ContractRevision> revisions = db.ContractRevision.Where(cr => cr.ProjectContractId == ctrId);
            DataSourceResult result = revisions.ToDataSourceResult(request, contractRevision => new {
                ContractRevisionId = contractRevision.ContractRevisionId,
                RevisionDate = contractRevision.RevisionDate,
                ProjectContractId = contractRevision.ProjectContractId,
                RevisedContractAmount = contractRevision.RevisedContractAmount,
                RevisedContractTime = contractRevision.RevisedContractTime,
                Remark = contractRevision.Remark,

                //OrgBranchName = contractRevision.OrgBranchName,
                //DateCreated = contractRevision.DateCreated,
                //LastModified = contractRevision.LastModified,
                //CreatedBy = contractRevision.CreatedBy,
                //ModifiedBy = contractRevision.ModifiedBy
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractRevision_Create([DataSourceRequest]DataSourceRequest request, ContractRevision contractRevision, int contractId)
        {
            if (ModelState.IsValid)
            {
                
                var entity = new ContractRevision
                {
                    ContractRevisionId = contractRevision.ContractRevisionId,
                    RevisionDate = contractRevision.RevisionDate,
                    ProjectContractId = contractId,
                    RevisedContractAmount = contractRevision.RevisedContractAmount,
                    RevisedContractTime = contractRevision.RevisedContractTime,
                    Remark = contractRevision.Remark,

                    //OrgBranchName = contractRevision.OrgBranchName,
                    //DateCreated = DateTime.Now,
                    //LastModified = contractRevision.LastModified,
                    //CreatedBy = User.Identity.Name,
                    //ModifiedBy = contractRevision.ModifiedBy
                };

                db.ContractRevision.Add(entity);
                db.SaveChanges();
                contractRevision.ContractRevisionId = entity.ContractRevisionId;
            }

            return Json(new[] { contractRevision }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractRevision_Update([DataSourceRequest]DataSourceRequest request, ContractRevision contractRevision)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractRevision
                {
                    ContractRevisionId = contractRevision.ContractRevisionId,
                    RevisionDate = contractRevision.RevisionDate,
                    ProjectContractId = contractRevision.ProjectContractId,
                    RevisedContractAmount = contractRevision.RevisedContractAmount,
                    RevisedContractTime = contractRevision.RevisedContractTime,
                    Remark = contractRevision.Remark,

                    //OrgBranchName = contractRevision.OrgBranchName,
                    //DateCreated = contractRevision.DateCreated,
                    //LastModified = DateTime.Now,
                    //CreatedBy = contractRevision.CreatedBy,
                    //ModifiedBy = User.Identity.Name
                };

                db.ContractRevision.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { contractRevision }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ContractRevision_Destroy([DataSourceRequest]DataSourceRequest request, ContractRevision contractRevision)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractRevision
                {
                    ContractRevisionId = contractRevision.ContractRevisionId,
                    RevisionDate = contractRevision.RevisionDate,
                    ProjectContractId = contractRevision.ProjectContractId,
                    RevisedContractAmount = contractRevision.RevisedContractAmount,
                    RevisedContractTime = contractRevision.RevisedContractTime,
                    Remark = contractRevision.Remark,

                    //OrgBranchName = contractRevision.OrgBranchName,
                    //DateCreated = contractRevision.DateCreated,
                    //LastModified = contractRevision.LastModified,
                    //CreatedBy = contractRevision.CreatedBy,
                    //ModifiedBy = contractRevision.ModifiedBy
                };

                db.ContractRevision.Attach(entity);
                db.ContractRevision.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { contractRevision }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}