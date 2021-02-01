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
    public class PriceAdjustmentWeightController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult PriceAdjustmentWeights_Read([DataSourceRequest]DataSourceRequest request, int projIdReadWeight)
        {
            IQueryable<PriceAdjustmentWeight> weights = db.PriceAdjustmentWeights.Where(e => e.ProjectInformationId == projIdReadWeight);
            DataSourceResult result = weights.ToDataSourceResult(request, weight => new
            {
                PriceAdjustmentWeightId = weight.PriceAdjustmentWeightId,
                ProjectInformationId = weight.ProjectInformationId,
                WorkCategoryId = weight.WorkCategoryId,
                ContractAmount = weight.ContractAmount,
                ActualAmount = weight.ActualAmount,


                OrgBranchName = weight.OrgBranchName,
                DateCreated = weight.DateCreated,
                LastModified = weight.LastModified,
                CreatedBy = weight.CreatedBy,
                ModifiedBy = weight.ModifiedBy
            });

            //return Json(result);
            return new LargeJsonResult
            {
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                Data = result
            };
        }
        // per project
        public ActionResult RepresentativeElementWeight_Read([DataSourceRequest]DataSourceRequest request, int projectId)
        {
            var representativeElements = db.ProjectRepresentativeElements.Where(r => r.ProjectInformationId == projectId).ToList();
            var priceAdjWeights = db.PriceAdjustmentWeights.Where(w => w.ProjectInformationId == projectId).ToList();
            List<RepresentativeElementWeightViewModel> list = new List<RepresentativeElementWeightViewModel>();
            if (representativeElements.Any() && priceAdjWeights.Any())
            {
                var totalCostSum = priceAdjWeights.Sum(w => w.ActualAmount);
                foreach (ProjectRepresentativeElement element in representativeElements)
                {
                    var totalCostOfItem = new decimal();
                    var weightItems = db.PriceAdjustmentWeightItems.Where(itm => itm.ProjectRepresentativeElementId == element.ProjectRepresentativeElementId && itm.PriceAdjustmentWeight.ProjectInformationId == projectId).ToList();

                    if (weightItems.Any())
                    {
                        totalCostOfItem = weightItems.Sum(i => i.ApprovedAmount);
                    }
                    var itemWeight = new RepresentativeElementWeightViewModel
                    {
                        Id = element.ProjectRepresentativeElementId,
                        Item = element.Description + "( " + element.IndexCode + " )",
                        Weight = totalCostSum > 0 ? totalCostOfItem / totalCostSum : 0
                    };
                    list.Add(itemWeight);
                }
                var nonAdjustableFactor = new RepresentativeElementWeightViewModel
                {
                    Item = "Non-adjustable",
                    Weight = 1 - list.Sum(w => w.Weight),
                };
                list.Add(nonAdjustableFactor);
            }
            IQueryable<RepresentativeElementWeightViewModel> itemsQueryable = list.AsQueryable();
            //return Json(itemsQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
            return new LargeJsonResult
            {
                MaxJsonLength = int.MaxValue,
                JsonRequestBehavior = System.Web.Mvc.JsonRequestBehavior.AllowGet,
                Data = itemsQueryable.ToDataSourceResult(request)
            };
        }
        // per BoQ
        public ActionResult RepresentativeElementWeightInBills_Read([DataSourceRequest]DataSourceRequest request, int boqId)
        {
            var boq = db.BoQOfExcutedWork.Find(boqId);
            List<RepresentativeElementWeightViewModel> list = new List<RepresentativeElementWeightViewModel>();

            if (boq != null)
            {
                var representativeElements = db.ProjectRepresentativeElements.Where(r => r.ProjectInformationId == boq.ProjectInformationId).ToList();
                var priceAdjWeights = db.PriceAdjustmentWeights.Where(w => w.ProjectInformationId == boq.ProjectInformationId).ToList();
                if (representativeElements.Any() && priceAdjWeights.Any())
                {
                    var totalCostSum = priceAdjWeights.Sum(w => w.ActualAmount);
                    foreach (ProjectRepresentativeElement element in representativeElements)
                    {
                        var totalCostOfItem = new decimal();
                        var weightItems = db.PriceAdjustmentWeightItems.Where(itm => itm.ProjectRepresentativeElementId == element.ProjectRepresentativeElementId && itm.PriceAdjustmentWeight.ProjectInformationId == boq.ProjectInformationId).ToList();

                        if (weightItems.Any())
                        {
                            totalCostOfItem = weightItems.Sum(i => i.ApprovedAmount);
                        }
                        var itemWeight = new RepresentativeElementWeightViewModel
                        {
                            Id = element.ProjectRepresentativeElementId,
                            Item = element.Description + "( " + element.IndexCode + " )",
                            Weight = totalCostSum > 0 ? totalCostOfItem / totalCostSum : 0
                        };
                        list.Add(itemWeight);
                    }
                    var nonAdjustableFactor = new RepresentativeElementWeightViewModel
                    {
                        Item = "Non-adjustable",
                        Weight = 1 - list.Sum(w => w.Weight),
                    };
                    list.Add(nonAdjustableFactor);
                }
            }

            IQueryable<RepresentativeElementWeightViewModel> itemsQueryable = list.AsQueryable();
            return Json(itemsQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        // price Adjustment Formula
        public ActionResult PriceAdjustmentFormula_Read([DataSourceRequest]DataSourceRequest request, int boqId)
        {
            var boq = db.BoQOfExcutedWork.Find(boqId);
            List<RepresentativeElementWeightViewModel> list = new List<RepresentativeElementWeightViewModel>();
            List<PriceAdjustmentFormulaViewModel> formulea = new List<PriceAdjustmentFormulaViewModel>();
            if (boq != null)
            {
                if (boq.Project.ProjectType == AvailableProjectTypes.Building)
                {
                    var currentSetFormula = (PriceAdjustmentFormulaType)ContractMgmtHelper.GetProjectFormulaType(boq.Project.ProjectInformationId);

                    var representativeElements = db.ProjectRepresentativeElements.Where(r => r.ProjectInformationId == boq.ProjectInformationId).ToList();
                    var priceAdjWeights = db.PriceAdjustmentWeights.Where(w => w.ProjectInformationId == boq.ProjectInformationId).ToList();
                    if (representativeElements.Any() && priceAdjWeights.Any())
                    {
                        var formula2006 = "";
                        var formula2011 = "";
                        var formula2006Result = new decimal();
                        var formula2011Result = new decimal();
                        var totalCostSum = priceAdjWeights.Sum(w => w.ActualAmount);
                        foreach (ProjectRepresentativeElement element in representativeElements)
                        {
                            var totalCostOfItem = new decimal();
                            var currentPrice = new decimal();
                            var basicPrice = element.BasicPrice;
                            var weightItems = db.PriceAdjustmentWeightItems.Where(itm => itm.ItemCode == element.ItemCode && itm.PriceAdjustmentWeight.ProjectInformationId == boq.ProjectInformationId).ToList();
                            var itemInBoQ = db.RepresentativeElementCostInBill.FirstOrDefault(itm => itm.ProjectRepresentativeElementId == element.ProjectRepresentativeElementId && itm.BoQOfExecutedWorkId == boqId);
                            if (weightItems.Any() && itemInBoQ != null)
                            {
                                totalCostOfItem = weightItems.Sum(i => i.ApprovedAmount);
                                currentPrice = itemInBoQ.CurrentPrice;
                            }
                            var Weight = Math.Round((totalCostSum > 0 ? totalCostOfItem / totalCostSum : 0), 4);
                            var itemWeight = new RepresentativeElementWeightViewModel
                            {
                                Id = element.ProjectRepresentativeElementId,
                                Item = element.Description + "( " + element.IndexCode + " )",
                                Weight = totalCostSum > 0 ? totalCostOfItem / totalCostSum : 0
                            };
                            formula2006 += " + " + Weight + "(" + currentPrice + "/" + basicPrice + ")";
                            formula2006Result += Weight * (basicPrice > 0 ? currentPrice / basicPrice : 0);
                            formula2011 += " + " + Weight + "(" + currentPrice + "-" + basicPrice + ")/" + basicPrice;
                            formula2011Result += Weight * (basicPrice > 0 ? (currentPrice - basicPrice) / basicPrice : 0);
                            list.Add(itemWeight);
                        }
                        var nonAdjustableFactor = new RepresentativeElementWeightViewModel
                        {
                            Item = "Non-adjustable",
                            Weight = Math.Round((1 - list.Sum(w => w.Weight)), 4),
                        };
                        formula2006Result += nonAdjustableFactor.Weight;
                        formula2011Result += nonAdjustableFactor.Weight;

                        formula2006 = "PA = " + nonAdjustableFactor.Weight + formula2006 + " = " + Math.Round(formula2006Result, 5);
                        formula2011 = "PA = " + nonAdjustableFactor.Weight + formula2011 + " = " + Math.Round(formula2011Result, 5);
                        list.Add(nonAdjustableFactor);
                        var formula1 = new PriceAdjustmentFormulaViewModel
                        {
                            Type = "2006",
                            Formula = formula2006
                        };
                        var formula2 = new PriceAdjustmentFormulaViewModel
                        {
                            Type = "2011",
                            Formula = formula2011
                        };
                        if (currentSetFormula == PriceAdjustmentFormulaType.TwoThousandSix)
                        {
                            formulea.Add(formula1);
                        }
                        else if(currentSetFormula == PriceAdjustmentFormulaType.TwoThousandEleven)
                        {
                            formulea.Add(formula2);
                        }
                        //formulea.Add(formula1);
                        //formulea.Add(formula2);
                    }
                }
                else if (boq.Project.ProjectType == AvailableProjectTypes.Road)
                {
                    var representativeElements = db.ProjectRepresentativeElements.Where(r => r.ProjectInformationId == boq.ProjectInformationId && r.IndexType != RepresentativeElementIndexType.NonAdjustable).ToList();
                    var nonAdjustable = db.ProjectRepresentativeElements.FirstOrDefault(r => r.ProjectInformationId == boq.ProjectInformationId && r.IndexType == RepresentativeElementIndexType.NonAdjustable);

                    if (representativeElements.Any() && nonAdjustable != null)
                    {
                        var nonAdjustableInBill = db.RepresentativeElementCostInBill.FirstOrDefault(itm => itm.ProjectRepresentativeElementId == nonAdjustable.ProjectRepresentativeElementId && itm.BoQOfExecutedWorkId == boqId);
                        var formula2006 = "";
                        var formula2006Result = new decimal();
                        foreach (ProjectRepresentativeElement element in representativeElements)
                        {
                            var basicPrice = element.BasicPrice;
                            var itemInBoQ = db.RepresentativeElementCostInBill.FirstOrDefault(itm => itm.ProjectRepresentativeElementId == element.ProjectRepresentativeElementId && itm.BoQOfExecutedWorkId == boqId);
                            if (itemInBoQ != null)
                            {
                                formula2006 += " + " + itemInBoQ.BidderProposedWeight / 100 + "(" + itemInBoQ.CurrentPrice + "/" + basicPrice + ")";
                                formula2006Result += itemInBoQ.BidderProposedWeight / 100 * (basicPrice > 0 ? itemInBoQ.CurrentPrice / basicPrice : 0);

                            }
                        }
                        if (nonAdjustableInBill != null)
                        {
                            formula2006Result += nonAdjustableInBill.BidderProposedWeight / 100;
                            formula2006 = "PA = " + nonAdjustableInBill.BidderProposedWeight / 100 + formula2006 + " = " + Math.Round(formula2006Result, 5);
                        }

                        var formula1 = new PriceAdjustmentFormulaViewModel
                        {
                            Type = " ",
                            Formula = formula2006
                        };

                        formulea.Add(formula1);
                    }
                }

            }

            IQueryable<PriceAdjustmentFormulaViewModel> itemsQueryable = formulea.AsQueryable();
            return Json(itemsQueryable.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PriceAdjustmentWeight_Create([DataSourceRequest]DataSourceRequest request, PriceAdjustmentWeight weight, int projIdCreateWeight)
        {
            if (ModelState.IsValid)
            {

                var entity = new PriceAdjustmentWeight
                {
                    PriceAdjustmentWeightId = weight.PriceAdjustmentWeightId,
                    ProjectInformationId = projIdCreateWeight,
                    WorkCategoryId = weight.WorkCategoryId,
                    ContractAmount = weight.ContractAmount,
                    ActualAmount = weight.ActualAmount,

                    OrgBranchName = weight.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = weight.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = weight.ModifiedBy
                };

                db.PriceAdjustmentWeights.Add(entity);
                db.SaveChanges();

                weight.PriceAdjustmentWeightId = entity.PriceAdjustmentWeightId;

            }

            return Json(new[] { weight }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PriceAdjustmentWeight_Update([DataSourceRequest]DataSourceRequest request, PriceAdjustmentWeight weight)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceAdjustmentWeight
                {
                    PriceAdjustmentWeightId = weight.PriceAdjustmentWeightId,
                    ProjectInformationId = weight.ProjectInformationId,
                    WorkCategoryId = weight.WorkCategoryId,
                    ContractAmount = weight.ContractAmount,
                    ActualAmount = weight.ActualAmount,

                    OrgBranchName = weight.OrgBranchName,
                    DateCreated = weight.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = weight.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.PriceAdjustmentWeights.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { weight }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PriceAdjustmentWeight_Destroy([DataSourceRequest]DataSourceRequest request, PriceAdjustmentWeight weight)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriceAdjustmentWeight
                {
                    PriceAdjustmentWeightId = weight.PriceAdjustmentWeightId,
                    ProjectInformationId = weight.ProjectInformationId,
                    WorkCategoryId = weight.WorkCategoryId,
                    ContractAmount = weight.ContractAmount,
                    ActualAmount = weight.ActualAmount,

                    OrgBranchName = weight.OrgBranchName,
                    DateCreated = weight.DateCreated,
                    LastModified = weight.LastModified,
                    CreatedBy = weight.CreatedBy,
                    ModifiedBy = weight.ModifiedBy
                };

                db.PriceAdjustmentWeights.Attach(entity);
                db.PriceAdjustmentWeights.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { weight }.ToDataSourceResult(request, ModelState));
        }

        private decimal GetItemUnitPrice(string itemCode)
        {
            var price = new decimal();
            var currentItem = db.MaterialPriceIndexIndices.FirstOrDefault(m => m.ItemCode == itemCode);
            if (currentItem != null)
            {
                price = currentItem.ApprovedCost;
            }
            return price;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}