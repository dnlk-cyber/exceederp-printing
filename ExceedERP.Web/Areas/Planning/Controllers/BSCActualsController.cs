﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin,
       PlanningRoles.Planning_User,
       HrmConstants.HRDepartmentHead,
       HrmConstants.HREmployeeSelfService)]
    public class BSCActualsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

       
        public ActionResult BSCActuals_Read([DataSourceRequest]DataSourceRequest request,int actualId)
        {

            IQueryable<BSCActual> bSCActuals = db.BSCActuals.Where(b=>b.ActualId== actualId);
            DataSourceResult result = bSCActuals.ToDataSourceResult(request, bSCActual => new {
                BSCActualId = bSCActual.BSCActualId,
                ObjectiveId = bSCActual.ObjectiveId,
                OrgStructureId = bSCActual.OrgStructureId,
                BSCPlanId = bSCActual.BSCPlanId,
                ActualId = bSCActual.ActualId
            });

            return Json(result);
        }
      
        public ActionResult BSCActuals_Create([DataSourceRequest]DataSourceRequest request, BSCActual bSCActual, int id)
        {
            var plan = db.Actuals.Find(id);
            if (plan != null)
            {
                var bscplan = db.BSCPlans.FirstOrDefault(x => x.PlanId ==plan.PlanId && x.ObjectiveId == bSCActual.ObjectiveId && x.OrgStructureId == bSCActual.OrgStructureId);
                if (bscplan != null)
                {
                    bSCActual.BSCPlanId = bscplan.BSCPlanId;
                }
                else
                {
                    ModelState.AddModelError("exists", "you have to plan for this fiscal year, Work Unit and and objective  ");

                }

            }
            //var bscPlanId = bSCActual.ObjectiveId;
            //var objective = db.BSCPlans.FirstOrDefault(x => x.BSCPlanId == bscPlanId && x.OrgStructureId == bSCActual.OrgStructureId);
            //if (objective != null)
            //{
            var exists = db.BSCActuals.Any(s => s.ObjectiveId == bSCActual.ObjectiveId &&s.OrgStructureId ==bSCActual.OrgStructureId&& s.ActualId == id);
            if (exists)
            {
                ModelState.AddModelError("exists", "This is already registered ");
            }
            bSCActual.ActualId = id;
            // bSCActual.ObjectiveId = objective.ObjectiveId;
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(bSCActual);
                db.BSCActuals.Add(entity);
                db.SaveChanges();
                bSCActual.BSCActualId = entity.BSCActualId;
                PopulatetheActualDetails(bSCActual.OrgStructureId,bSCActual.ObjectiveId, entity.BSCPlanId, entity.BSCActualId, id);
                //PopulateThePeriodForActual(entity.BSCActualId, id);
                //PopulateDataToPlannedValueForMonthlyActual(bSCPlan.ObjectiveId, bSCPlan.OrgStructureId);
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Bsc Actuals", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);

                #endregion


            }
           // }
            return Json(new[] { bSCActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BSCActuals_Update([DataSourceRequest]DataSourceRequest request, BSCActual bSCActual)
        {
            var plan = db.Actuals.Find(bSCActual.ActualId);
            if(plan != null)
            {
                var bscplan = db.BSCPlans.FirstOrDefault(x => x.PlanId == plan.PlanId && x.ObjectiveId == bSCActual.ObjectiveId && x.OrgStructureId == bSCActual.OrgStructureId);
                bSCActual.BSCPlanId = bscplan != null ? bscplan.BSCPlanId:0;
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(bSCActual);
                db.BSCActuals.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "BSCActual", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
                #endregion

            }

            return Json(new[] { bSCActual }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BSCActuals_Destroy([DataSourceRequest]DataSourceRequest request, BSCActual bSCActual)
        {
            var actualDetails = db.ActualDetails.Where(x => x.BSCActualId == bSCActual.BSCActualId).ToList();
            if (actualDetails.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this ! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(bSCActual);
                #region delete log
                var objective = db.Objectives.Find(bSCActual.ObjectiveId);
                var delete = objective.Name;
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     Objective deleted  " + delete);
                #endregion

                db.BSCActuals.Attach(entity);
                db.BSCActuals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { bSCActual }.ToDataSourceResult(request, ModelState));
        }

        private BSCActual GetMappedObject(BSCActual bSCActual)
        {
            var entity = new BSCActual
            {
                BSCActualId = bSCActual.BSCActualId,
                ObjectiveId = bSCActual.ObjectiveId,
                OrgStructureId = bSCActual.OrgStructureId,
                BSCPlanId = bSCActual.BSCPlanId,
                IsFirstLevelApproved = bSCActual.IsFirstLevelApproved,
                IsSecondLevel = bSCActual.IsSecondLevel,
                IsThirdLevel = bSCActual.IsThirdLevel,
                IsCopied = bSCActual.IsCopied,
                ActualId = bSCActual.ActualId
            };
            return entity;
        }
        private int? PopulatetheActualDetails(int orgStructureId, int objectiveId,int? bscPlanId, int bSCActualId, int actualId)
        {
            if (bscPlanId != null || bscPlanId != 0)
            {
                //var placement = db.Placements.Where(x => x.isCurrent && x.IsHead && x.OrgStructureId == orgStructureId).FirstOrDefault();
                //if (placement != null)
                //{
                //    var orgstructureIds = db.MainCompanyStructures.Where(x => x.Parent == orgStructureId).Select(x => x.OrgStructureId);
                //    if (orgstructureIds.Count() > 0)
                //    {
                //        var bscActuals = db.BSCActuals.Where(x => x.ActualId == actualId && orgstructureIds.Contains(x.OrgStructureId)).ToList();
                //        if (bscActuals.Count() > 0)
                //        {
                //            var bscActualIds = bscActuals.Select(x => x.BSCActualId);
                //            if (bscActualIds.Count()>0)

                //            {
                //                var actualDetails = db.ActualDetails.Where(x => bscActualIds.Contains(x.BSCActualId)).ToList();

                //                var actualDetailDisticts = actualDetails.DistinctByField(x => x.IndicatorId).ToList();
                //               // var indicatorIds = new HashSet<int>(actualDetails.Select(d => d.IndicatorId));

                //                foreach (var actualDetail in actualDetailDisticts)
                //                {
                //                    //var actualDetail = actualDetails.FirstOrDefault(x => x.IndicatorId == indicatorId);
                //                    var actualDetailSingleindicaors = actualDetails.Where(x => x.IndicatorId == actualDetail.IndicatorId).ToList();
                //                    if( actualDetailSingleindicaors.Count() > 0)
                //                    {


                //                        var entity = new ActualDetail
                //                        {
                //                            IndicatorId = actualDetail.IndicatorId,
                //                            PlanningDetailId = actualDetail.PlanningDetailId,
                //                            ParentId = actualDetail.ParentId,
                //                            Unit = actualDetail.Unit,
                //                            KPIType = actualDetail.KPIType,
                //                            HasSubIndicator = actualDetail.HasSubIndicator,
                //                            Plan = actualDetailSingleindicaors.Sum(x=>x.Plan),
                //                            Baseline = actualDetailSingleindicaors.Sum(x => x.Baseline),
                //                            Target = actualDetailSingleindicaors.Sum(x => x.Target),
                //                            BSCActualId = bSCActualId

                //                        };
                //                        db.ActualDetails.Add(entity);
                //                        db.SaveChanges();
                //                        var actualDetailIds = actualDetails.Select(x => x.ActualDetailId);
                //                        var monthlyActuals = db.MonthlyActuals.Where(x => actualDetailIds.Contains(x.ActualDetailId)).ToList();
                //                        var monthlyActualsForSinglePeriod = monthlyActuals.DistinctByField(x => x.GLPeriodId).ToList();
                //                        foreach (var period in monthlyActualsForSinglePeriod)
                //                        {

                //                            var SinglemonthlyActuals = monthlyActuals.Where(x => x.GLPeriodId == period.GLPeriodId&&x.GetDetail.IndicatorId==actualDetail.IndicatorId);
                //                            if (SinglemonthlyActuals.Count() > 0)
                //                            {
                //                                var entity2 = new MonthlyActual
                //                                {
                //                                    GLPeriodId = period.GLPeriodId,
                //                                    PlanValue = SinglemonthlyActuals.Sum(x=>x.PlanValue),
                //                                    ActualValue = SinglemonthlyActuals.Sum(x=>x.ActualValue),
                //                                    MonthlyPlanId = period.MonthlyPlanId,
                //                                    ActualDetailId = entity.ActualDetailId,


                //                                };
                //                                db.MonthlyActuals.Add(entity2);
                //                                db.SaveChanges();
                //                                var monthlyActualId = monthlyActuals.Select(x => x.MonthlyActualId).ToList();
                //                                var weeklyActuals = monthlyActualId.Count()>0? db.WeeklyActuals.Where(x => monthlyActualId.Contains(x.MonthlyActualId)).ToList():null;
                //                                if (weeklyActuals.Count() > 0)
                //                                {
                //                                    var entity3 = new WeeklyActual
                //                                    {
                //                                        MonthlyActualId = entity2.MonthlyActualId,
                //                                        WeekOne = weeklyActuals.Sum(x => x.WeekOne),
                //                                        WeekTwo = weeklyActuals.Sum(x => x.WeekTwo),
                //                                        WeekThree = weeklyActuals.Sum(x => x.WeekThree),
                //                                        WeekFour = weeklyActuals.Sum(x => x.WeekFour),


                //                                    };
                //                                    db.WeeklyActuals.Add(entity3);
                //                                    db.SaveChanges();

                //                                }
                //                            }
                                       


                //                        }

                //                    }
                                    
                //                }
                               

                //            }
                //        }
                //        else
                //        {
                //            var PlaceWithindicators = db.PlanningDetails.Where(x => x.BSCPlanId == bscPlanId).ToList();
                //            foreach (var indicator in PlaceWithindicators)
                //            {

                //                var entity = new ActualDetail
                //                {
                //                    IndicatorId = indicator.IndicatorId,
                //                    PlanningDetailId = indicator.PlanningDetailId,
                //                    ParentId = indicator.ParentId,
                //                    Unit = indicator.Unit,
                //                    KPIType = indicator.KPIType,
                //                    HasSubIndicator = indicator.HasSubIndicator,
                //                    Plan = indicator.Plan,
                //                    Baseline = indicator.Baseline,
                //                    Target = indicator.Target,
                //                    BSCActualId = bSCActualId

                //                };
                //                db.ActualDetails.Add(entity);
                //                db.SaveChanges();
                //                var periods = db.MonthlyPlans.Where(x => x.PlanningDetailId == indicator.PlanningDetailId).ToList();

                //                foreach (var period in periods)
                //                {


                //                    var entity2 = new MonthlyActual
                //                    {
                //                        GLPeriodId = period.GLPeriodId,
                //                        PlanValue = period.Value,
                //                        ActualValue = 0,
                //                        MonthlyPlanId = period.MonthlyPlanId,
                //                        ActualDetailId = entity.ActualDetailId,


                //                    };
                //                    db.MonthlyActuals.Add(entity2);
                //                    db.SaveChanges();


                //                }



                //            }
                //        }
                       
                //    }
                //    else
                //    {
                //        var Placeindicators = db.PlanningDetails.Where(x => x.BSCPlanId == bscPlanId).ToList();
                //        foreach (var indicator in Placeindicators)
                //        {

                //            var entity = new ActualDetail
                //            {
                //                IndicatorId = indicator.IndicatorId,
                //                PlanningDetailId = indicator.PlanningDetailId,
                //                ParentId = indicator.ParentId,
                //                Unit = indicator.Unit,
                //                KPIType = indicator.KPIType,
                //                HasSubIndicator = indicator.HasSubIndicator,
                //                Plan = indicator.Plan,
                //                Baseline = indicator.Baseline,
                //                Target = indicator.Target,
                //                BSCActualId = bSCActualId

                //            };
                //            db.ActualDetails.Add(entity);
                //            db.SaveChanges();
                //            var periods = db.MonthlyPlans.Where(x => x.PlanningDetailId == indicator.PlanningDetailId).ToList();

                //            foreach (var period in periods)
                //            {


                //                var entity2 = new MonthlyActual
                //                {
                //                    GLPeriodId = period.GLPeriodId,
                //                    PlanValue = period.Value,
                //                    ActualValue = 0,
                //                    MonthlyPlanId = period.MonthlyPlanId,
                //                    ActualDetailId = entity.ActualDetailId,


                //                };
                //                db.MonthlyActuals.Add(entity2);
                //                db.SaveChanges();


                //            }



                //        }

                //    }

                //}
                
                var indicators = db.PlanningDetails.Where(x =>x.BSCPlanId== bscPlanId).ToList();
                foreach (var indicator in indicators)
                {

                    var entity = new ActualDetail
                    {
                        IndicatorId = indicator.IndicatorId,
                        PlanningDetailId = indicator.PlanningDetailId,
                        ParentId = indicator.ParentId,
                        Unit = indicator.Unit,
                        KPIType = indicator.KPIType,
                        HasSubIndicator = indicator.HasSubIndicator,
                        Plan = indicator.Plan,
                        Baseline = indicator.Baseline,
                        Target = indicator.Target,
                        BSCActualId = bSCActualId

                    };
                    db.ActualDetails.Add(entity);
                    db.SaveChanges();
                    var periods = db.MonthlyPlans.Where(x => x.PlanningDetailId==indicator.PlanningDetailId).ToList();

                    foreach (var period in periods)
                    {

                               
                            var entity2 = new MonthlyActual
                            {
                                GLPeriodId = period.GLPeriodId,
                                PlanValue = period.Value,
                                ActualValue = 0,
                                MonthlyPlanId= period.MonthlyPlanId,
                                ActualDetailId = entity.ActualDetailId,


                            };
                            db.MonthlyActuals.Add(entity2);
                            db.SaveChanges();
                               

                    }
                  


                }

                return 0;
            }
            return null;
        }
        public JsonResult RefreshToAddNewAddedItems(int id)
        {
            int count = 0;
            var record = db.BSCActuals.Find(id);
            if (record != null)
            {
                #region validate item Addition

                var indicators = db.PlanningDetails.Where(x => x.BSCPlanId == record.BSCPlanId).ToList();
                if (!indicators.Any())
                {
                    indicators = db.PlanningDetails.Where(x => x.GetBSCPlan.OrgStructureId == record.OrgStructureId && x.GetBSCPlan.ObjectiveId == record.ObjectiveId).ToList();
                }
                foreach (var indicator in indicators)
                {
                    var exist = db.ActualDetails.Where(x => x.IndicatorId == indicator.IndicatorId&&x.BSCActualId==id).FirstOrDefault();
                    if (exist == null)
                    {


                        var entity = new ActualDetail
                        {
                            IndicatorId = indicator.IndicatorId,
                            PlanningDetailId = indicator.PlanningDetailId,
                            GlobalIndicatorId = indicator.GlobalIndicatorId,
                            ParentId = indicator.ParentId,
                            Unit = indicator.Unit,
                            KPIType = indicator.KPIType,
                            HasSubIndicator = indicator.HasSubIndicator,
                            Plan = indicator.Plan,
                            Baseline = indicator.Baseline,
                            Target = indicator.Target,
                            BSCActualId = record.BSCActualId

                        };
                        db.ActualDetails.Add(entity);
                        db.SaveChanges();
                        var periods = db.MonthlyPlans.Where(x => x.PlanningDetailId == indicator.PlanningDetailId).ToList();

                        foreach (var period in periods)
                        {

                            var entity2 = new MonthlyActual
                            {
                                GLPeriodId = period.GLPeriodId,
                                PlanValue = period.Value,
                                ActualValue = 0,
                                MonthlyPlanId = period.MonthlyPlanId,
                                ActualDetailId = entity.ActualDetailId,


                            };
                            db.MonthlyActuals.Add(entity2);
                            db.SaveChanges();


                        }
                        count++;
                    }
                    else
                    {
                        exist.ParentId = indicator.ParentId;
                        exist.GlobalIndicatorId = indicator.GlobalIndicatorId;
                        exist.Unit = indicator.Unit;
                        exist.KPIType = indicator.KPIType;
                        exist.HasSubIndicator = indicator.HasSubIndicator;
                        exist.Plan = indicator.Plan;
                        exist.Baseline = indicator.Baseline;
                        exist.Target = indicator.Target;
                        db.Entry(exist).State = EntityState.Modified;
                        db.SaveChanges();
                        var periods = db.MonthlyPlans.Where(x => x.PlanningDetailId == indicator.PlanningDetailId).ToList();
                        foreach (var period in periods)
                        {

                            var monthlyactual = db.MonthlyActuals.Where(x => x.ActualDetailId == exist.ActualDetailId && x.GLPeriodId == period.GLPeriodId &&x.PlanValue !=period.Value).FirstOrDefault();
                            if(monthlyactual != null)
                            {
                                monthlyactual.PlanValue = period.Value;
                                db.Entry(monthlyactual).State = EntityState.Modified;
                                db.SaveChanges();
                               
                            }
                         
                        }
                        count++;
                    }

                }
                if (count > 0)
                {
                    var formattedData = new
                    {
                        Message = "Item Updated Successfully .",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = " there is no changed  Item to  Update .",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
            var formattedData12 = new
            {
                Message = "The record is not found.",
                Status = "Not_OK"
            };
            return Json(formattedData12, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
