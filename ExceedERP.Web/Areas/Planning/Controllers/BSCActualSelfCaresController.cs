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
    public class BSCActualSelfCaresController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        private bool GetHasChid(int id, int bsId)
        {

            var placement = db.Placements.Where(x => x.isCurrent && x.IsHead && x.OrgStructureId == id).FirstOrDefault();
            if (placement != null)
            {
                var orgstructureIds = db.MainCompanyStructures
                                   .Where(x => x.Parent == id && x.OrgStructureId != id).Select(x=>x.OrgStructureId)
                                   .ToList();
                var Recordorgstructures = (orgstructureIds.Count() > 0) ? db.BSCActuals.Where(x => x.BSCActualId == bsId&& orgstructureIds.Contains(x.OrgStructureId)).FirstOrDefault() : null;
                if (Recordorgstructures != null)
                {
                    return false;

                }
                return true;
            }
            return false;
        }
        public ActionResult BSCActualSelfCares_Read([DataSourceRequest]DataSourceRequest request,int actualId)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var workUnitId = EmployeeHelper.GetEmployeeWorkUnitId(employeeId);
            var bSCActuals = db.BSCActuals.Where(x => x.OrgStructureId == workUnitId).Where(b => b.ActualId == actualId).ToList();
            var placement = db.Placements.Where(x => x.isCurrent && x.IsHead && x.OrgStructureId == workUnitId).FirstOrDefault();
            if (placement != null)
            {
               // var orgstructureIds = db.MainCompanyStructures.Where(x => x.Parent == workUnitId).Select(x => x.OrgStructureId);
                List<int> orgstructureIds = db.MainCompanyStructures
                              .Where(x => x.Parent == workUnitId)
                              .Select(x => x.OrgStructureId)
                              .ToList();
                orgstructureIds.Add(workUnitId);
                if (orgstructureIds.Count() > 0)
                {
                    bSCActuals = db.BSCActuals.Where(x => x.ActualId == actualId && orgstructureIds.Contains(x.OrgStructureId) /*|| x.OrgStructureId==workUnitId*/).ToList();

                }

            }

            DataSourceResult result = bSCActuals.ToDataSourceResult(request, bSCActual => new {
                BSCActualId = bSCActual.BSCActualId,
                ObjectiveId = bSCActual.ObjectiveId,
                OrgStructureId = bSCActual.OrgStructureId,
                BSCPlanId = bSCActual.BSCPlanId,
                IsFirstLevelApproved = bSCActual.IsFirstLevelApproved,
                IsSecondLevel = bSCActual.IsSecondLevel,
                IsThirdLevel = bSCActual.IsThirdLevel,
                IsCopied = bSCActual.IsCopied,
                HasChild = GetHasChid(workUnitId, bSCActual.BSCActualId),

                ActualId = bSCActual.ActualId
            });

            return Json(result);
        }
      
        public ActionResult BSCActualSelfCares_Create([DataSourceRequest]DataSourceRequest request, BSCActual bSCActual, int id)
        {
            var plan = db.Actuals.Find(id);
            if (plan != null)
            {
                var bscplan = db.BSCPlans.FirstOrDefault(x => x.PlanId ==plan.PlanId&&x.IsFirstLevelApproved && x.ObjectiveId == bSCActual.ObjectiveId && x.OrgStructureId == bSCActual.OrgStructureId);
                if (bscplan != null)
                {
                    bSCActual.BSCPlanId = bscplan.BSCPlanId;
                }
                else
                {
                    ModelState.AddModelError("exists", "First, you have to plan and Approve for this fiscal year, Work Unit and and objective  ");

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
                PopulatetheActualDetails(bSCActual.ObjectiveId, entity.BSCPlanId, entity.BSCActualId, id);
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
        public ActionResult BSCActualSelfCares_Update([DataSourceRequest]DataSourceRequest request, BSCActual bSCActual)
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
        public ActionResult BSCActualSelfCares_Destroy([DataSourceRequest]DataSourceRequest request, BSCActual bSCActual)
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
        private int? PopulatetheActualDetails(int objectiveId,int? bscPlanId, int bSCActualId, int actualId)
        {
            if (bscPlanId != null || bscPlanId != 0)
            {

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
        
        public JsonResult UpdateCopiedPlanInActual(int id)
        {
            int count = 0;
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var workUnitId = EmployeeHelper.GetEmployeeWorkUnitId(employeeId);
            var placement = db.Placements.Where(x => x.isCurrent && x.IsHead && x.OrgStructureId == workUnitId).FirstOrDefault();
            if (placement != null)
            {
                var orgstructureIds = db.MainCompanyStructures
                                .Where(x => x.Parent == workUnitId && x.OrgStructureId != workUnitId).Select(x=>x.OrgStructureId)
                                .ToList();
                if (orgstructureIds.Count()>0)
                {
                    var record = db.BSCActuals.Find(id);
                    if (record != null)
                    {
                        var bscActuals = db.BSCActuals.Where(x => x.IsFirstLevelApproved
                                                                && x.ActualId == record.ActualId
                                                                && orgstructureIds.Contains(x.OrgStructureId)).ToList();
                        #region validate item Addition
                        if (bscActuals.Count() > 0)
                        {
                            HashSet<int> bscAcualIds = new HashSet<int>(bscActuals.Select(x => x.BSCActualId));
                            var indicators = db.ActualDetails.Where(x => x.BSCActualId == record.BSCActualId).ToList();
                            foreach (var indicator in indicators)
                            {
                                var actualDetails = db.ActualDetails.Where(x => x.IndicatorId == indicator.IndicatorId && bscAcualIds.Contains(x.BSCActualId)).ToList();
                                if (actualDetails.Count()>0)
                                {
                                    HashSet<int> ActualDetailIdS = new HashSet<int>(actualDetails.Select(x => x.ActualDetailId));

                                    var monthlyActuals = db.MonthlyActuals.Where(x => x.ActualDetailId == indicator.ActualDetailId).ToList();

                                    foreach (var period in monthlyActuals)
                                    {

                                        var monthlyactualUppers = db.MonthlyActuals.Where(x => ActualDetailIdS.Contains( x.ActualDetailId) && x.GLPeriodId == period.GLPeriodId ).ToList();
                                        if (monthlyactualUppers.Count()>0)
                                        {
                                            period.ActualValue = monthlyactualUppers.Sum(x=>x.ActualValue);
                                            db.Entry(period).State = EntityState.Modified;
                                            db.SaveChanges();
                                            HashSet<int> weeklyActualIdS = new HashSet<int>(monthlyactualUppers.Select(x => x.MonthlyActualId));

                                            var weeklyActuals = db.WeeklyActuals.Where(x => x.MonthlyActualId == period.MonthlyActualId).ToList();
                                            foreach(var weeklActual in weeklyActuals)
                                            {
                                                var weeklyActualLower = db.WeeklyActuals.Where(x => weeklyActualIdS.Contains(x.MonthlyActualId)).ToList();
                                                if (weeklyActualLower.Count() > 0)
                                                {
                                                    weeklActual.WeekOne = weeklyActualLower.Sum(x => x.WeekOne);
                                                    weeklActual.WeekTwo = weeklyActualLower.Sum(x => x.WeekTwo);
                                                    weeklActual.WeekThree = weeklyActualLower.Sum(x => x.WeekThree);
                                                    weeklActual.WeekFour = weeklyActualLower.Sum(x => x.WeekFour);
                                                    db.Entry(weeklActual).State = EntityState.Modified;
                                                    db.SaveChanges();
                                                }
                                            }
                                            count++;
                                        }
                                        else
                                        {
                                            period.ActualValue = period.ActualValue;
                                            db.Entry(period).State = EntityState.Modified;
                                            db.SaveChanges();
                                            var weeklyActuals = db.WeeklyActuals.Where(x => x.MonthlyActualId == period.MonthlyActualId).ToList();
                                            foreach (var weeklActual in weeklyActuals)
                                            {
                                                
                                                    weeklActual.WeekOne = weeklActual.WeekOne;
                                                    weeklActual.WeekTwo = weeklActual.WeekTwo;
                                                    weeklActual.WeekThree =weeklActual.WeekThree;
                                                    weeklActual.WeekFour = weeklActual.WeekFour;
                                                    db.Entry(weeklActual).State = EntityState.Modified;
                                                    db.SaveChanges();
                                                
                                            }
                                            count++;
                                        }

                                    }

                                }
                                else
                                {
                                    var monthlyActuals = db.MonthlyActuals.Where(x => x.ActualDetailId == indicator.ActualDetailId).ToList();

                                    foreach (var period in monthlyActuals)
                                    {
                                        period.ActualValue = period.ActualValue;
                                        db.Entry(period).State = EntityState.Modified;
                                        db.SaveChanges();
                                        var weeklyActuals = db.WeeklyActuals.Where(x => x.MonthlyActualId == period.MonthlyActualId).ToList();
                                        foreach (var weeklActual in weeklyActuals)
                                        {

                                            weeklActual.WeekOne = weeklActual.WeekOne;
                                            weeklActual.WeekTwo = weeklActual.WeekTwo;
                                            weeklActual.WeekThree = weeklActual.WeekThree;
                                            weeklActual.WeekFour = weeklActual.WeekFour;
                                            db.Entry(weeklActual).State = EntityState.Modified;
                                            db.SaveChanges();

                                        }
                                        count++;
                                    }


                                }




                            }
                            if (count > 0)
                            {
                                record.IsCopied = true;
                                db.Entry(record).State = EntityState.Modified;
                                db.SaveChanges();
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
                                    Message = " no new Item is  added .",
                                    Status = "OK"
                                };
                                return Json(formattedData, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var formattedData13 = new
                            {
                                Message = " The   Work Unit  under you are not approved",
                                Status = "Not-okay"
                            };
                            return Json(formattedData13, JsonRequestBehavior.AllowGet);

                        }

                        
                        #endregion
                    }
                    var formattedData18 = new
                    {
                        Message = "The record is not found.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData18, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    var formattedData13 = new
                    {
                        Message = "you can not copy b/c This is your own dept Or no  Work Unit  is  under you",
                        Status = "Not-okay"
                    };
                    return Json(formattedData13, JsonRequestBehavior.AllowGet);

                }

            }
            var formattedData12 = new
            {
                Message = "your are not Department Head.",
                Status = "okay"
            };
            return Json(formattedData12, JsonRequestBehavior.AllowGet);
            
        }
        public JsonResult ApproveBScActual(int id)
        {
            var record = db.BSCActuals.Find(id);
            if (record != null)
            {
                record.IsFirstLevelApproved = true;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData12 = new
                {
                    Message = "It is approved succesfully.",
                    Status = "okay"
                };
                return Json(formattedData12, JsonRequestBehavior.AllowGet);
            }

            var formattedData = new
            {
                Message = "Record is not found.",
                Status = "NOT_OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
