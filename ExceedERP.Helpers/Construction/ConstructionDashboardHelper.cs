using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Dashboard;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Construction
{
    public static class ConstructionDashboardHelper
    {
        private static ExceedDbContext _db = new ExceedDbContext();

        public static void AddConsPlanAcheivmentData()
        {
            var activeProjects = _db.ProjectInformation.Where(p => p.ProjectStatus != ProjectStatus.CLOSED && p.ProjectStatus != ProjectStatus.ESTIMATION).ToList();
            if (activeProjects.Any())
            {
                foreach (ProjectInformation project in activeProjects)
                {
                    var plannedCost = new decimal();
                    var actualCost = new decimal();

                    #region Plan
                    var existingPlanData =
                                    _db.DashboardDictionaries
                                    .FirstOrDefault(d => d.Category == ConstructionConstants.ProjectPlanAndAchivementStatusCategory &&
                                                         d.Name == ConstructionConstants.ProjectPlan &&
                                                         d.Dimesion == project.Name);
                    var workItemsInProject = _db.WorkItemPerProject.Where(w => w.ProjectInformationId == project.ProjectInformationId).ToList();
                    if (workItemsInProject.Any())
                    {
                        plannedCost = workItemsInProject.Sum(w => (w.TotalUnitCost * w.TotalQuantityOfWork));
                    }
                    if (existingPlanData != null)
                    {
                        existingPlanData.Value = plannedCost;

                        _db.DashboardDictionaries.Attach(existingPlanData);
                        _db.Entry(existingPlanData).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = ConstructionConstants.ProjectPlanAndAchivementStatusCategory,
                            Name = ConstructionConstants.ProjectPlan,
                            Dimesion = project.Name,
                            Value = plannedCost
                        };
                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion

                    #region Achivement
                    var existingAchivementData =
                                    _db.DashboardDictionaries
                                    .FirstOrDefault(d => d.Category == ConstructionConstants.ProjectPlanAndAchivementStatusCategory &&
                                                         d.Name == ConstructionConstants.ProjectAchivement &&
                                                         d.Dimesion == project.Name);


                    var utilizationWorkItems = _db.ResourceUtilizationWorkItem
                        .Where(uw => uw.WorkItemPerProject.ProjectInformation.ProjectInformationId == project.ProjectInformationId).ToList();
                    if (utilizationWorkItems.Any())
                    {
                        actualCost = utilizationWorkItems.Sum(u => u.TotalCost);
                    }

                    if (existingAchivementData != null)
                    {
                        existingAchivementData.Value = actualCost;

                        _db.DashboardDictionaries.Attach(existingAchivementData);
                        _db.Entry(existingAchivementData).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = ConstructionConstants.ProjectPlanAndAchivementStatusCategory,
                            Name = ConstructionConstants.ProjectAchivement,
                            Dimesion = project.Name,
                            Value = actualCost
                        };
                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    } 
                    #endregion

                }
            }
        }
        public static void AddCostStatusData()
        {
            var activeprojects = _db.ProjectInformation.Where(p => p.ProjectStatus != ProjectStatus.CLOSED && p.ProjectStatus != ProjectStatus.ESTIMATION).ToList();
            if (activeprojects.Any())
            {
                foreach(ProjectInformation project in activeprojects)
                {
                    var costToDate = new decimal();
                    #region actual Cost Utilization
                    var existingCostStatusData =
                                    _db.DashboardDictionaries
                                    .FirstOrDefault(d => d.Category == ConstructionConstants.ProjectCostStatusCategory &&
                                                         d.Name == ConstructionConstants.ProjectCostTodate &&
                                                         d.Dimesion == project.Name);


                    var utilizationWorkItems = _db.ResourceUtilizationWorkItem
                        .Where(uw => uw.WorkItemPerProject.ProjectInformation.ProjectInformationId == project.ProjectInformationId).ToList();
                    if (utilizationWorkItems.Any())
                    {
                        costToDate = utilizationWorkItems.Sum(u => u.TotalCost);
                    }

                    if (existingCostStatusData != null)
                    {
                        existingCostStatusData.Value = costToDate;

                        _db.DashboardDictionaries.Attach(existingCostStatusData);
                        _db.Entry(existingCostStatusData).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = ConstructionConstants.ProjectCostStatusCategory,
                            Name = ConstructionConstants.ProjectCostTodate,
                            Dimesion = project.Name,
                            Value = costToDate
                        };
                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion

                }
            }
        }
        public static void AddTimeStatusData()
        {
            var activeprojects = _db.ProjectInformation.Where(p => p.ProjectStatus != ProjectStatus.CLOSED && p.ProjectStatus != ProjectStatus.ESTIMATION).ToList();
            if (activeprojects.Any())
            {
                foreach (ProjectInformation project in activeprojects)
                {
                    var timeElapsed = new decimal();
                    var mainContract = ContractMgmtHelper.GetProjectMainContract(project.ProjectInformationId);
                    #region time elapsed
                    var existingTimeStatusData =
                                    _db.DashboardDictionaries
                                    .FirstOrDefault(d => d.Category == ConstructionConstants.ProjectTimeStatusCategory &&
                                                         d.Name == ConstructionConstants.ProjectTimeElapsedToDate &&
                                                         d.Dimesion == project.Name);

                    var difference = DateTime.Now - mainContract.CommencementDate;
                    var days = (decimal)difference.Days + 1;
                    if (days > mainContract.TotalProjectPeriod)
                    {
                        timeElapsed = (decimal)mainContract.TotalProjectPeriod;

                    }
                    else if (days < 0)
                    {
                        timeElapsed = 0;
                    }
                    else
                    {
                        timeElapsed = days;
                    }

                    if (existingTimeStatusData != null)
                    {
                        existingTimeStatusData.Value = timeElapsed;

                        _db.DashboardDictionaries.Attach(existingTimeStatusData);
                        _db.Entry(existingTimeStatusData).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = ConstructionConstants.ProjectTimeStatusCategory,
                            Name = ConstructionConstants.ProjectTimeElapsedToDate,
                            Dimesion = project.Name,
                            Value = timeElapsed
                        };
                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion

                }
            }
        }

        public static void AddProjectPaymentAmountData()
        {
            var activeprojects = _db.ProjectInformation.Where(p => p.ProjectStatus != ProjectStatus.CLOSED && p.ProjectStatus != ProjectStatus.ESTIMATION).ToList();
            if (activeprojects.Any())
            {
                foreach (ProjectInformation project in activeprojects)
                {
                    var totalPayment = new decimal();
                    var advanceAmount = new decimal();
                    var paymentAmount = new decimal();

                    #region payments
                    var existingPaymentData =
                                    _db.DashboardDictionaries
                                    .FirstOrDefault(d => d.Category == ConstructionConstants.ProjectPaymentAmountCategory &&
                                                         d.Name == ConstructionConstants.ProjectPaymentAmountTodate &&
                                                         d.Dimesion == project.Name);

                    var advancePayments = _db.ProjectAdvancePayments.Where(adp => adp.ProjectInformationId == project.ProjectInformationId).ToList();
                    var paymentCertificates = _db.PaymentCertificates.Where(pc => pc.ProjectInformationId == project.ProjectInformationId).ToList();
                    if (advancePayments.Any())
                    {
                        advanceAmount = advancePayments.Sum(p => p.ApprovedAmount);
                    }
                    if (paymentCertificates.Any())
                    {
                        paymentAmount = paymentCertificates.Sum(pc => pc.NetPayableAmount);
                    }
                    totalPayment = advanceAmount + paymentAmount;
                    if (existingPaymentData != null)
                    {
                        existingPaymentData.Value = totalPayment;

                        _db.DashboardDictionaries.Attach(existingPaymentData);
                        _db.Entry(existingPaymentData).State = System.Data.Entity.EntityState.Modified;
                        _db.SaveChanges();
                    }
                    else
                    {
                        var entity = new DashboardDictionary
                        {
                            Category = ConstructionConstants.ProjectPaymentAmountCategory,
                            Name = ConstructionConstants.ProjectPaymentAmountTodate,
                            Dimesion = project.Name,
                            Value = totalPayment
                        };
                        _db.DashboardDictionaries.Add(entity);
                        _db.SaveChanges();
                    }
                    #endregion

                }
            }
        }

    }
}
