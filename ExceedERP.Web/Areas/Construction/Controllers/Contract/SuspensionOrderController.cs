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
    public class SuspensionOrderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/SuspensionOrder
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult SuspensionOrder_Read([DataSourceRequest]DataSourceRequest request, int ctrctIdRead)
        {
            IQueryable<ContractSuspensionOrder> suspensionOrder = db.ContractSuspensionOrder.Where(x => x.ProjectContractId == ctrctIdRead);
            DataSourceResult result = suspensionOrder.ToDataSourceResult(request, sr => new {
                ContractSuspensionOrderId = sr.ContractSuspensionOrderId,
                ProjectContractId = sr.ProjectContractId,
                SuspensionOrderNumber = sr.SuspensionOrderNumber,
                SuspensionAmount = sr.SuspensionAmount,
                ReasonOfSuspension = sr.ReasonOfSuspension,
                AffectedWork = sr.AffectedWork
                

                //OrgBranchName = eot.OrgBranchName,
                //DateCreated = eot.DateCreated,
                //LastModified = eot.LastModified,
                //CreatedBy = eot.CreatedBy,
                //ModifiedBy = eot.ModifiedBy
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SuspensionOrder_Create([DataSourceRequest]DataSourceRequest request, ContractSuspensionOrder suspensionOrder, int ctrctIdWrite)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(suspensionOrder.ReasonOfSuspension))
                {
                    suspensionOrder.ReasonOfSuspension = "unknown reason";
                }
                var entity = new ContractSuspensionOrder
                {
                    ContractSuspensionOrderId = suspensionOrder.ContractSuspensionOrderId,
                    ProjectContractId = ctrctIdWrite,
                    SuspensionOrderNumber = suspensionOrder.SuspensionOrderNumber,
                    SuspensionAmount = suspensionOrder.SuspensionAmount,
                    ReasonOfSuspension = suspensionOrder.ReasonOfSuspension,
                    AffectedWork = suspensionOrder.AffectedWork

                    //OrgBranchName = suspensionOrder.OrgBranchName,
                    //DateCreated = DateTime.Now,,
                    //LastModified = suspensionOrder.LastModified,
                    //CreatedBy = User.Identity.Name,
                    //ModifiedBy = suspensionOrder.ModifiedBy
                };

                db.ContractSuspensionOrder.Add(entity);
                db.SaveChanges();
                suspensionOrder.ContractSuspensionOrderId = entity.ContractSuspensionOrderId;
            }

            return Json(new[] { suspensionOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SuspensionOrder_Update([DataSourceRequest]DataSourceRequest request, ContractSuspensionOrder suspensionOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractSuspensionOrder
                {
                    ContractSuspensionOrderId = suspensionOrder.ContractSuspensionOrderId,
                    ProjectContractId = suspensionOrder.ProjectContractId,
                    SuspensionOrderNumber = suspensionOrder.SuspensionOrderNumber,
                    SuspensionAmount = suspensionOrder.SuspensionAmount,
                    ReasonOfSuspension = suspensionOrder.ReasonOfSuspension,
                    AffectedWork = suspensionOrder.AffectedWork

                    //OrgBranchName = suspensionOrder.OrgBranchName,
                    //DateCreated = suspensionOrder.DateCreated,
                    //LastModified = DateTime.Now,
                    //CreatedBy = suspensionOrder.CreatedBy,
                    //ModifiedBy = User.Identity .Name
                };

                db.ContractSuspensionOrder.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { suspensionOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SuspensionOrder_Destroy([DataSourceRequest]DataSourceRequest request, ContractSuspensionOrder suspensionOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractSuspensionOrder
                {
                    ContractSuspensionOrderId = suspensionOrder.ContractSuspensionOrderId,
                    ProjectContractId = suspensionOrder.ProjectContractId,
                    SuspensionOrderNumber = suspensionOrder.SuspensionOrderNumber,
                    SuspensionAmount = suspensionOrder.SuspensionAmount,
                    ReasonOfSuspension = suspensionOrder.ReasonOfSuspension,
                    AffectedWork = suspensionOrder.AffectedWork

                    //OrgBranchName = suspensionOrder.OrgBranchName,
                    //DateCreated = suspensionOrder.DateCreated,
                    //LastModified = suspensionOrder.LastModified,
                    //CreatedBy = suspensionOrder.CreatedBy,
                    //ModifiedBy = suspensionOrder.ModifiedBy
                };

                db.ContractSuspensionOrder.Attach(entity);
                db.ContractSuspensionOrder.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { suspensionOrder }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}