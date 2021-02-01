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

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class BoQOfExecutedWorkController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            var uoms = db.UnitOfMeasurement.ToList();
            ViewData["uom"] = uoms;

            return View();
        }
        public ActionResult BoQs_Read([DataSourceRequest]DataSourceRequest request, int projectId)
        {
            IQueryable<BoQOfExecutedWork> boqs = db.BoQOfExcutedWork.Where(adp => adp.ProjectInformationId == projectId);
            DataSourceResult result = boqs.ToDataSourceResult(request, boq => new
            {
                BoQOfExecutedWorkId = boq.BoQOfExecutedWorkId,
                ProjectInformationId = boq.ProjectInformationId,
                BillReference = boq.BillReference,
                BillNumber = boq.BillNumber,
                Date = boq.Date,
                Remark = boq.Remark,

                OrgBranchName = boq.OrgBranchName,
                DateCreated = boq.DateCreated,
                LastModified = boq.LastModified,
                CreatedBy = boq.CreatedBy,
                ModifiedBy = boq.ModifiedBy
            });

            return Json(result);
        }

        //private bool CheckExistance(int projectId)
        //{
        //    bool boqExists = false;
        //    var boq = db.ProjectAdvancePayments.Where(ap => ap.ProjectInformationId == projectId).FirstOrDefault();
        //    if (boq != null)
        //    {
        //        boqExists = true;
        //    }
        //    return boqExists;
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BoQ_Create([DataSourceRequest]DataSourceRequest request, BoQOfExecutedWork boq, int projectId)
        {
            if (ModelState.IsValid)
            {
                //    if (CheckExistance(projectIdCreateAdvance))
                //    {
                //        ModelState.AddModelError("", "Advance Payment Exists for this project!!");
                //    }
                //    else
                //    {
                var entity = new BoQOfExecutedWork
                {
                    ProjectInformationId = projectId,
                    BillNumber = this.GetBillCounter(projectId),
                    BillReference = boq.BillReference,
                    Date = boq.Date,
                    Remark = boq.Remark,

                    OrgBranchName = boq.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = boq.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = boq.ModifiedBy
                };


                db.BoQOfExcutedWork.Add(entity);
                db.SaveChanges();
                boq.BoQOfExecutedWorkId = entity.BoQOfExecutedWorkId;

                UpdateBoQReference(entity);

                this.AddBoQContent(entity.BoQOfExecutedWorkId);
                this.AddRepresentativeElement(entity.BoQOfExecutedWorkId);
                //}
            }

            return Json(new[] { boq }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BoQ_Update([DataSourceRequest]DataSourceRequest request, BoQOfExecutedWork boq)
        {
            if (ModelState.IsValid)
            {
                var entity = new BoQOfExecutedWork
                {
                    BoQOfExecutedWorkId = boq.BoQOfExecutedWorkId,
                    ProjectInformationId = boq.ProjectInformationId,
                    BillReference = boq.BillReference,
                    BillNumber = boq.BillNumber,
                    Date = boq.Date,
                    Remark = boq.Remark,

                    OrgBranchName = boq.OrgBranchName,
                    DateCreated = boq.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = boq.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.BoQOfExcutedWork.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //UpdateBoQReference(entity);


            }

            return Json(new[] { boq }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BoQ_Destroy([DataSourceRequest]DataSourceRequest request, BoQOfExecutedWork boq)
        {
            if (ModelState.IsValid)
            {
                var entity = new BoQOfExecutedWork
                {
                    BoQOfExecutedWorkId = boq.BoQOfExecutedWorkId,
                    ProjectInformationId = boq.ProjectInformationId,
                    BillReference = boq.BillReference,
                    BillNumber = boq.BillNumber,
                    Date = boq.Date,
                    Remark = boq.Remark,

                    OrgBranchName = boq.OrgBranchName,
                    DateCreated = boq.DateCreated,
                    LastModified = boq.LastModified,
                    CreatedBy = boq.CreatedBy,
                    ModifiedBy = boq.ModifiedBy
                };

                db.BoQOfExcutedWork.Attach(entity);
                db.BoQOfExcutedWork.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { boq }.ToDataSourceResult(request, ModelState));
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
        private int GetBillCounter(int projectId)
        {
            int counter = 0;
            var availableBoQs = db.BoQOfExcutedWork.Where(boq => boq.ProjectInformationId == projectId).ToList();
            // this is the first payment of the project - excluding advance payment
            if (!availableBoQs.Any())
            {
                counter = 1;
            }
            else
            {
                counter = availableBoQs.Max(p => p.BillNumber) + 1;
            }
            return counter;
        }
        private void AddBoQContent(int id)
        {
            // this holds the information to the rebate % from main contract of the project
            var rebateRate = new decimal();
            var currentBoQ = db.BoQOfExcutedWork.Find(id);
            if (currentBoQ != null)
            {
                var currentProject = db.ProjectInformation.Find(currentBoQ.ProjectInformationId);
                var projContract = ContractMgmtHelper.GetProjectMainContractIfExists(currentBoQ.ProjectInformationId);
                if(projContract != null)
                {
                    rebateRate = projContract.Rebate ?? 0;
                }
                if (currentProject != null)
                {
                    var existingContent = db.BoQOfExcutedWorkContent.Where(c => c.BoQOfExecutedWorkId == currentBoQ.BoQOfExecutedWorkId).ToList();

                    var workItems = db.WorkItemPerProject.Where(wi => wi.ProjectInformationId == currentProject.ProjectInformationId && wi.ParentId == 0).ToList();
                    var customWorkItems = db.ProjectCustomWorkItems.Where(wi => wi.ProjectInformationId == currentProject.ProjectInformationId).ToList();

                    if (existingContent.Any())
                    {
                        db.BoQOfExcutedWorkContent.RemoveRange(existingContent);
                        db.SaveChanges();
                    }

                    if (workItems.Any())
                    {
                        foreach (WorkItemPerProject workItem in workItems)
                        {
                            this.AddWorkItemToBoQContent(currentBoQ, workItem, null, rebateRate);
                        }
                    }
                }
            }
        }
        private void AddRepresentativeElement(int boqId)
        {
            var boq = db.BoQOfExcutedWork.Find(boqId);
            if (boq != null)
            {
                //if()
                //{
                var representativeElements = db.ProjectRepresentativeElements.Where(r => r.ProjectInformationId == boq.ProjectInformationId).ToList();
                if (representativeElements.Any() && boq.Project.ProjectType == AvailableProjectTypes.Building)
                {
                    foreach (ProjectRepresentativeElement element in representativeElements)
                    {
                        var entity = new RepresentativeElementCostInBill
                        {
                            BasicPrice = element.BasicPrice,
                            BoQOfExecutedWorkId = boqId,
                            CurrentPrice = element.BasicPrice,
                            ItemCode = element.ItemCode,
                            Unit = element.Unit,
                            ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                            ItemName = element.Description + "( " + element.IndexCode + " )",

                        };
                        db.RepresentativeElementCostInBill.Add(entity);
                        db.SaveChanges();
                    }
                }
                //}
                //else if(boq.Project.ProjectType == AvailableProjectTypes.Road)
                //{
                //var representativeElements = db.ProjectRepresentativeElements.Where(r => r.ProjectInformationId == boq.ProjectInformationId).ToList();
                else if (representativeElements.Any() && boq.Project.ProjectType == AvailableProjectTypes.Road)
                {
                    var elementNum = representativeElements.Count;
                    foreach (ProjectRepresentativeElement element in representativeElements)
                    {
                        var entity = new RepresentativeElementCostInBill
                        {
                            BasicPrice = element.BasicPrice,
                            BoQOfExecutedWorkId = boqId,
                            CurrentPrice = element.BasicPrice,
                            ItemCode = element.ItemCode,
                            Unit = element.Unit,
                            ProjectRepresentativeElementId = element.ProjectRepresentativeElementId,
                            ItemName = element.Description + "( " + element.IndexCode + " )",
                            BidderProposedWeight = Math.Round((decimal)100/elementNum, 0),

                        };
                        db.RepresentativeElementCostInBill.Add(entity);
                        db.SaveChanges();
                    }
                }
                //}

            }
        }
        private void AddWorkItemToBoQContent(BoQOfExecutedWork boqHeader, WorkItemPerProject workItem, BoQOfExecutedWorkContent parentLineItem, decimal rebateRate)
        {
            var boqContentToAd = new BoQOfExecutedWorkContent
            {
                BoQOfExecutedWorkId = boqHeader.BoQOfExecutedWorkId,
                ItemCode = workItem.ItemCode,
                Index = parentLineItem == null ? GetBoQSubIndex(workItem.WorkCategoryId, workItem.WorkCategory.ConfigurationId, workItem.ProjectInformationId) :
                GetBoQSubIndexForSubWorkItem(parentLineItem),
                WorkItemPerProjectId = workItem.WorkItemPerProjectId,
                WorkCategoryId = workItem.WorkCategoryId,
                ConfigurationId = workItem.WorkCategory.ConfigurationId,

                Description = workItem.ItemName,
                UnitOfMeasurementId = workItem.UnitOfMeasurementId ?? 0,
                ContractQuantity = workItem.TotalQuantityOfWork,
                QuantityTillPrevPayment = this.GetQuantityTillPrevPay(boqHeader.BoQOfExecutedWorkId, workItem.WorkItemPerProjectId),
                //QuantityThisPayment = workItem.QuantityThisPayment,
                QuantityToDate = this.GetQuantityExecutedToDate(boqHeader.BoQOfExecutedWorkId, workItem.WorkItemPerProjectId),
                UnitRate = workItem.TotalUnitCost * (1 - rebateRate/100),

                //AmountTillPrevPayment = workItem.AmountTillPrevPayment,
                //AmountThisPayment = workItem.AmountThisPayment,
                //AmountToDatePayment = workItem.AmountToDatePayment,

                //SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(workItem.BlockId),
                //BlockName = ProjectEstimationHelper.GetBlockName(workItem.BlockId),
                //StructureyType = ConsSettingHelper.GetStructureTypeCode(workItem.WorkCategory.ConfigurationId, workItem.ProjectType) + " " + workItem.WorkCategory.Configuration.CurrentValue.Replace('_', ' '),
                //WorkCategory = workItem.WorkCategory.Index + " " + workItem.WorkCategory.Name,
            };
            if(parentLineItem == null)
            {
                boqContentToAd.SubProjectTypeName = ProjectEstimationHelper.GetWorkItemSubProjectTypeName(workItem.BlockId);
                boqContentToAd.BlockName = ProjectEstimationHelper.GetBlockName(workItem.BlockId);
                boqContentToAd.StructureyType = ConsSettingHelper.GetStructureTypeCode(workItem.WorkCategory.ConfigurationId, workItem.ProjectType) + " " + workItem.WorkCategory.Configuration.CurrentValue.Replace('_', ' ');
                boqContentToAd.WorkCategory = workItem.WorkCategory.Index + " " + workItem.WorkCategory.Name;

                if (workItem.ProjectInformation.ProjectType == AvailableProjectTypes.Road && workItem.ItemCode != null)
                {
                    boqContentToAd.ItemCode = workItem.ItemCode;
                }
                else
                {
                    boqContentToAd.ItemCode = workItem.WorkCategory.Index + "." + boqContentToAd.Index;
                }
            }
            // if its a child content ... give the parent content's properties
            else
            {
                boqContentToAd.ParentBoQContentId = parentLineItem.BoQOfExecutedWorkContentId;
                boqContentToAd.SubProjectTypeName = parentLineItem.SubProjectTypeName;
                boqContentToAd.BlockName = parentLineItem.BlockName;
                boqContentToAd.StructureyType = parentLineItem.StructureyType;
                boqContentToAd.WorkCategory = parentLineItem.WorkCategory;

                if (workItem.ProjectInformation.ProjectType == AvailableProjectTypes.Road && workItem.ItemCode != null)
                {
                    boqContentToAd.ItemCode = workItem.ItemCode;
                }
                else
                {
                    boqContentToAd.ItemCode = parentLineItem.ItemCode + "." + boqContentToAd.Index;
                }
            }
            boqContentToAd.QuantityThisPayment = boqContentToAd.QuantityToDate - boqContentToAd.QuantityTillPrevPayment;
            boqContentToAd.AmountTillPrevPayment = boqContentToAd.QuantityTillPrevPayment * boqContentToAd.UnitRate;
            boqContentToAd.AmountThisPayment = boqContentToAd.QuantityThisPayment * boqContentToAd.UnitRate;
            boqContentToAd.AmountToDatePayment = boqContentToAd.QuantityToDate * boqContentToAd.UnitRate;

            db.BoQOfExcutedWorkContent.Add(boqContentToAd);
            db.SaveChanges();
            if (workItem.IsSubCategory)
            {
                var childItems = db.WorkItemPerProject.Where(wi => wi.ParentId == workItem.WorkItemPerProjectId).ToList();
                if (childItems.Any())
                {
                    foreach(WorkItemPerProject childItem in childItems)
                    {
                        this.AddWorkItemToBoQContent(boqHeader, childItem, boqContentToAd, rebateRate);
                    }
                }
            }
        }
        private int GetBoQSubIndex(int workCategoryId, int configurationId, int projectId)
        {
            int counter = 1;
            //var currentWorkCategory = db.WorkCategory.Find(workCategoryId);
            var existingBoQComponents = db.BoQOfExcutedWorkContent
                .Where(boq => boq.WorkCategoryId == workCategoryId &&
                boq.BoQOfExecutedWork.ProjectInformationId == projectId &&
                boq.ConfigurationId == configurationId);
            if (existingBoQComponents.Any())
            {
                counter = existingBoQComponents.Max(c => c.Index) + 1;
            }
            return counter;
        }
        private decimal GetQuantityTillPrevPay(int boqId, int workItemId)
        {
            var qty = new decimal();
            var currentBoQ = db.BoQOfExcutedWork.Find(boqId);
            if (currentBoQ != null)
            {
                if (currentBoQ.BillNumber == 1)
                {
                    //var takeOffSheet = db.ProjectTakeOff.FirstOrDefault(t => t.WorkItemPerProjectId == workItemId);
                    //if (takeOffSheet != null)
                    //{
                    //    var contents = db.ProjectTakeOffLine.Where(tc => tc.ProjectTakeOffId == takeOffSheet.ProjectTakeOffId).ToList();
                    //    if (contents.Any())
                    //    {
                    //        qty = contents.Sum(c => c.Quantity);
                    //    }
                    //}
                }
                //}
                else
                {
                    var previousBoQ = db.BoQOfExcutedWork
                        .FirstOrDefault(boq => boq.ProjectInformationId == currentBoQ.ProjectInformationId && boq.BillNumber == currentBoQ.BillNumber - 1);
                    if (previousBoQ != null)
                    {
                        var boqContent = db.BoQOfExcutedWorkContent.FirstOrDefault(boq => boq.BoQOfExecutedWorkId == previousBoQ.BoQOfExecutedWorkId &&
                                        boq.WorkItemPerProjectId == workItemId);
                        if (boqContent != null)
                        {
                            qty = boqContent.QuantityToDate;
                        }
                    }
                }
            }

            return qty;
        }
        private decimal GetQuantityExecutedToDate(int id, int workItemId)
        {
            var qty = new decimal();
            var currentBoQ = db.BoQOfExcutedWork.Find(id);
            if (currentBoQ != null)
            {
                var takeOffSheet = db.ProjectTakeOff.FirstOrDefault(t => t.WorkItemPerProjectId == workItemId);
                if (takeOffSheet != null)
                {
                    var contents = db.ProjectTakeOffLine.Where(tc => tc.ProjectTakeOffId == takeOffSheet.ProjectTakeOffId).ToList();
                    if (contents.Any())
                    {
                        qty = contents.Sum(c => c.Quantity);
                    }
                }
            }
            return qty;
        }
        private void UpdateBoQReference(BoQOfExecutedWork model)
        {
            model.BillReference = model.BoQOfExecutedWorkId.ToString("D10");

            db.BoQOfExcutedWork.Attach(model);
            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();
        }
        private int GetBoQSubIndexForSubWorkItem(BoQOfExecutedWorkContent model)
        {
            var counter = 1;
            var existingSubComponents = db.BoQOfExcutedWorkContent.Where(bc => bc.ParentBoQContentId == model.BoQOfExecutedWorkContentId).ToList();
            if (existingSubComponents.Any())
            {
                counter = existingSubComponents.Max(s => s.Index) + 1;
            }
            return counter;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}