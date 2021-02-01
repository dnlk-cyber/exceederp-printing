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

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class BoQOfExecutedWorkContentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
       
        public ActionResult BoQContents_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<BoQOfExecutedWorkContent> boqContentContents = db.BoQOfExcutedWorkContent.Where(c => c.BoQOfExecutedWorkId  == id);
            DataSourceResult result = boqContentContents.ToDataSourceResult(request, boqContent => new {
                BoQOfExecutedWorkContentId = boqContent.BoQOfExecutedWorkContentId,
                BoQOfExecutedWorkId = boqContent.BoQOfExecutedWorkId,
                ItemCode = boqContent.ItemCode,
                Index = boqContent.Index,
                WorkItemPerProjectId = boqContent.WorkItemPerProjectId,
                WorkCategoryId = boqContent.WorkCategoryId,

                Description = boqContent.Description,
                UnitOfMeasurementId = boqContent.UnitOfMeasurementId,
                ContractQuantity = boqContent.ContractQuantity,
                QuantityTillPrevPayment = boqContent.QuantityTillPrevPayment,
                QuantityThisPayment = boqContent.QuantityThisPayment,
                QuantityToDate = boqContent.QuantityToDate,
                UnitRate = boqContent.UnitRate,

                AmountTillPrevPayment = boqContent.AmountTillPrevPayment,
                AmountThisPayment = boqContent.AmountThisPayment,
                AmountToDatePayment = boqContent.AmountToDatePayment,

                SubProjectTypeName = boqContent.SubProjectTypeName,
                BlockName = boqContent.BlockName,
                StructureyType = boqContent.StructureyType,
                WorkCategory = boqContent.WorkCategory,

                //OrgBranchName = boqContent.OrgBranchName,
                //DateCreated = boqContent.DateCreated,
                //LastModified = boqContent.LastModified,
                //CreatedBy = boqContent.CreatedBy,
                //ModifiedBy = boqContent.ModifiedBy
            });

            return Json(result);
        }

        //private bool CheckExistance(int projectId)
        //{
        //    bool boqContentExists = false;
        //    var boqContent = db.ProjectAdvancePayments.Where(ap => ap.ProjectInformationId == projectId).FirstOrDefault();
        //    if (boqContent != null)
        //    {
        //        boqContentExists = true;
        //    }
        //    return boqContentExists;
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BoQContent_Create([DataSourceRequest]DataSourceRequest request, BoQOfExecutedWorkContent boqContent, int id)
        {
            if (ModelState.IsValid)
            {
                //    if (CheckExistance(projectIdCreateAdvance))
                //    {
                //        ModelState.AddModelError("", "Advance Payment Exists for this project!!");
                //    }
                //    else
                //    {
                var entity = new BoQOfExecutedWorkContent
                {
                    //BoQOfExecutedWorkContentId = boqContent.BoQOfExecutedWorkContentId,
                    BoQOfExecutedWorkId = boqContent.BoQOfExecutedWorkId,
                    ItemCode = boqContent.ItemCode,
                    Index = boqContent.Index,
                    WorkItemPerProjectId = boqContent.WorkItemPerProjectId,
                    WorkCategoryId = boqContent.WorkCategoryId,

                    Description = boqContent.Description,
                    UnitOfMeasurementId = boqContent.UnitOfMeasurementId,
                    ContractQuantity = boqContent.ContractQuantity,
                    QuantityTillPrevPayment = boqContent.QuantityTillPrevPayment,
                    QuantityThisPayment = boqContent.QuantityThisPayment,
                    QuantityToDate = boqContent.QuantityToDate,
                    UnitRate = boqContent.UnitRate,

                    AmountTillPrevPayment = boqContent.AmountTillPrevPayment,
                    AmountThisPayment = boqContent.AmountThisPayment,
                    AmountToDatePayment = boqContent.AmountToDatePayment,
                    SubProjectTypeName = boqContent.SubProjectTypeName,
                    BlockName = boqContent.BlockName,
                    StructureyType = boqContent.StructureyType,
                    WorkCategory = boqContent.WorkCategory,

                    //OrgBranchName = boqContent.OrgBranchName,
                    //DateCreated = DateTime.Now,
                    //LastModified = boqContent.LastModified,
                    //CreatedBy = User.Identity.Name,
                    //ModifiedBy = boqContent.ModifiedBy
                };


                db.BoQOfExcutedWorkContent.Add(entity);
                db.SaveChanges();
                boqContent.BoQOfExecutedWorkContentId = entity.BoQOfExecutedWorkContentId;

                //}
            }

            return Json(new[] { boqContent }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BoQContent_Update([DataSourceRequest]DataSourceRequest request, BoQOfExecutedWorkContent boqContent)
        {
            if (ModelState.IsValid)
            {
                var entity = new BoQOfExecutedWorkContent
                {
                    BoQOfExecutedWorkContentId = boqContent.BoQOfExecutedWorkContentId,
                    BoQOfExecutedWorkId = boqContent.BoQOfExecutedWorkId,
                    ItemCode = boqContent.ItemCode,
                    Index = boqContent.Index,
                    WorkItemPerProjectId = boqContent.WorkItemPerProjectId,
                    WorkCategoryId = boqContent.WorkCategoryId,

                    Description = boqContent.Description,
                    UnitOfMeasurementId = boqContent.UnitOfMeasurementId,
                    ContractQuantity = boqContent.ContractQuantity,
                    QuantityTillPrevPayment = boqContent.QuantityTillPrevPayment,
                    QuantityThisPayment = boqContent.QuantityThisPayment,
                    QuantityToDate = boqContent.QuantityToDate,
                    UnitRate = boqContent.UnitRate,

                    AmountTillPrevPayment = boqContent.AmountTillPrevPayment,
                    AmountThisPayment = boqContent.AmountThisPayment,
                    AmountToDatePayment = boqContent.AmountToDatePayment,
                    SubProjectTypeName = boqContent.SubProjectTypeName,
                    BlockName = boqContent.BlockName,
                    StructureyType = boqContent.StructureyType,
                    WorkCategory = boqContent.WorkCategory,

                    //OrgBranchName = boqContent.OrgBranchName,
                    //DateCreated = boqContent.DateCreated,
                    //LastModified = DateTime.Now,
                    //CreatedBy = boqContent.CreatedBy,
                    //ModifiedBy = User.Identity.Name
                };

                db.BoQOfExcutedWorkContent.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { boqContent }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BoQContent_Destroy([DataSourceRequest]DataSourceRequest request, BoQOfExecutedWorkContent boqContent)
        {
            if (ModelState.IsValid)
            {
                var entity = new BoQOfExecutedWorkContent
                {
                    BoQOfExecutedWorkContentId = boqContent.BoQOfExecutedWorkContentId,
                    BoQOfExecutedWorkId = boqContent.BoQOfExecutedWorkId,
                    ItemCode = boqContent.ItemCode,
                    Index = boqContent.Index,
                    WorkItemPerProjectId = boqContent.WorkItemPerProjectId,
                    WorkCategoryId = boqContent.WorkCategoryId,

                    Description = boqContent.Description,
                    UnitOfMeasurementId = boqContent.UnitOfMeasurementId,
                    ContractQuantity = boqContent.ContractQuantity,
                    QuantityTillPrevPayment = boqContent.QuantityTillPrevPayment,
                    QuantityThisPayment = boqContent.QuantityThisPayment,
                    QuantityToDate = boqContent.QuantityToDate,
                    UnitRate = boqContent.UnitRate,

                    AmountTillPrevPayment = boqContent.AmountTillPrevPayment,
                    AmountThisPayment = boqContent.AmountThisPayment,
                    AmountToDatePayment = boqContent.AmountToDatePayment,
                    SubProjectTypeName = boqContent.SubProjectTypeName,
                    BlockName = boqContent.BlockName,
                    StructureyType = boqContent.StructureyType,
                    WorkCategory = boqContent.WorkCategory,

                    //OrgBranchName = boqContent.OrgBranchName,
                    //DateCreated = boqContent.DateCreated,
                    //LastModified = boqContent.LastModified,
                    //CreatedBy = boqContent.CreatedBy,
                    //ModifiedBy = boqContent.ModifiedBy
                };

                db.BoQOfExcutedWorkContent.Attach(entity);
                db.BoQOfExcutedWorkContent.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { boqContent }.ToDataSourceResult(request, ModelState));
        }
        //private decimal GetTotalProjectAmount(int projId)
        //{
        //    decimal amount = new decimal();
        //    var mainContract = ContractMgmtHelper.GetProjectMainContract(projId);
        //    if (mainContract.TotalProjectAmount != null)
        //    {
        //        amount = (decimal)mainContract.TotalProjectAmount;
        //    }
        //    return amount;
        //}
        //private int GetBillCounter(int projectId)
        //{
        //    int counter = 0;
        //    var availableBoQs = db.BoQOfExcutedWork.Where(boqContent => boqContent.ProjectInformationId == projectId).ToList();
        //    // this is the first payment of the project - excluding advance payment
        //    if (!availableBoQs.Any())
        //    {
        //        counter = 1;
        //    }
        //    else
        //    {
        //        counter = availableBoQs.Max(p => p.BillNumber) + 1;
        //    }
        //    return counter;
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}