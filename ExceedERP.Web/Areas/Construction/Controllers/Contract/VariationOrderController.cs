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
    public class VariationOrderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/VariationOrder
        public ActionResult Index()
        {

            return View();
        }
        public JsonResult ht()
        {
            var items = db.VariationOrderType;
            //var items = db.ProjectInformation .Where (X=> X.ProjectStatus == ProjectStatus.APPROVED || X.ProjectStatus == ProjectStatus.STARTED);
            return Json(items.Select(p => new { VariationOrderTypeId = p.VariationOrderTypeId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult VariationOrder_Read([DataSourceRequest]DataSourceRequest request, int contractId)
        {
            IQueryable<ContractVariationOrder> changeOrders = db.ContractVariationOrder.Where(vo => vo.ProjectContractId == contractId);
            DataSourceResult result = changeOrders.ToDataSourceResult(request, variationOrder => new
            {
                ContractVariationOrderId = variationOrder.ContractVariationOrderId,
                ProjectContractId = variationOrder.ProjectContractId,
                VariationOrderTypeId = variationOrder.VariationOrderTypeId,
                VariationOrderNumber = variationOrder.VariationOrderNumber,
                //OrderStatus = variationOrder.OrderStatus,
                //Priority = variationOrder.Priority,
                //DateIssued = variationOrder.DateIssued,
                //IssuedBy = variationOrder.IssuedBy,
                DaysAdded = variationOrder.DaysAdded,
                ChangeCost = variationOrder.ChangeCost,

                OrgBranchName = variationOrder.OrgBranchName,
                DateCreated = variationOrder.DateCreated,
                LastModified = variationOrder.LastModified,
                CreatedBy = variationOrder.CreatedBy,
                ModifiedBy = variationOrder.ModifiedBy
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VariationOrder_Create([DataSourceRequest]DataSourceRequest request, ContractVariationOrder variationOrder, int cId)
        {
            if (ModelState.IsValid)
            {

                var entity = new ContractVariationOrder
                {
                    ContractVariationOrderId = variationOrder.ContractVariationOrderId,
                    ProjectContractId = cId,
                    VariationOrderTypeId = variationOrder.VariationOrderTypeId,
                    VariationOrderNumber = variationOrder.VariationOrderNumber,
                    //OrderStatus = variationOrder.OrderStatus,
                    //Priority = variationOrder.Priority,
                    //DateIssued = variationOrder.DateIssued,
                    //IssuedBy = variationOrder.IssuedBy,
                    DaysAdded = variationOrder.DaysAdded,
                    ChangeCost = variationOrder.ChangeCost,

                    OrgBranchName = variationOrder.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = variationOrder.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = variationOrder.ModifiedBy
                };

                db.ContractVariationOrder.Add(entity);
                db.SaveChanges();
                variationOrder.VariationOrderTypeId = entity.VariationOrderTypeId;

                ContractMgmtHelper.UpdateContract(entity.ProjectContractId);

            }

            return Json(new[] { variationOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VariationOrder_Update([DataSourceRequest]DataSourceRequest request, ContractVariationOrder variationOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractVariationOrder
                {
                    ContractVariationOrderId = variationOrder.ContractVariationOrderId,
                    ProjectContractId = variationOrder.ProjectContractId,
                    VariationOrderTypeId = variationOrder.VariationOrderTypeId,
                    VariationOrderNumber = variationOrder.VariationOrderNumber,
                    //OrderStatus = variationOrder.OrderStatus,
                    //Priority = variationOrder.Priority,
                    //DateIssued = variationOrder.DateIssued,
                    //IssuedBy = variationOrder.IssuedBy,
                    DaysAdded = variationOrder.DaysAdded,
                    ChangeCost = variationOrder.ChangeCost,

                    OrgBranchName = variationOrder.OrgBranchName,
                    DateCreated = variationOrder.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = variationOrder.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ContractVariationOrder.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                ContractMgmtHelper.UpdateContract(entity.ProjectContractId);
            }

            return Json(new[] { variationOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VariationOrder_Destroy([DataSourceRequest]DataSourceRequest request, ContractVariationOrder variationOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = new ContractVariationOrder
                {
                    ContractVariationOrderId = variationOrder.ContractVariationOrderId,
                    ProjectContractId = variationOrder.ProjectContractId,
                    VariationOrderTypeId = variationOrder.VariationOrderTypeId,
                    VariationOrderNumber = variationOrder.VariationOrderNumber,
                    //OrderStatus = variationOrder.OrderStatus,
                    //Priority = variationOrder.Priority,
                    //DateIssued = variationOrder.DateIssued,
                    //IssuedBy = variationOrder.IssuedBy,
                    DaysAdded = variationOrder.DaysAdded,
                    ChangeCost = variationOrder.ChangeCost,

                    OrgBranchName = variationOrder.OrgBranchName,
                    DateCreated = variationOrder.DateCreated,
                    LastModified = variationOrder.LastModified,
                    CreatedBy = variationOrder.CreatedBy,
                    ModifiedBy = variationOrder.ModifiedBy
                };

                db.ContractVariationOrder.Attach(entity);
                db.ContractVariationOrder.Remove(entity);
                db.SaveChanges();

                ContractMgmtHelper.UpdateContract(entity.ProjectContractId);
            }

            return Json(new[] { variationOrder }.ToDataSourceResult(request, ModelState));
        }
        //private void UpdateMainContractAfterAddingVariation(int contractId)
        //{
        //    var currentContract = db.ProjectContract.Find(contractId);
        //    if (currentContract != null)
        //    {

        //    }
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}