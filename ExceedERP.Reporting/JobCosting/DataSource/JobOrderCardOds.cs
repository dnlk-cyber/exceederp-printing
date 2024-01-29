using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.JobCosting.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.JobCosting.DataSource
{
    [DataObject]
    public class JobOrderCardOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public JobOrderCardVM GetJobOrderCardODS(string jobOrderId)
        {
            var model = new JobOrderCardVM();
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            var jobCost = db.JobCosts.Where(q => q.JobId.ToString() == jobOrderId).FirstOrDefault();
            var jobOrder = db.Jobs.Where(q => q.JobId.ToString() == jobOrderId).FirstOrDefault();
            Image LogoFileName = null;
            string AmharicName = "";
            string Name = "";
            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var logo =
                                   db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                                           a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }

            model.LogoFileName = LogoFileName;
            model.Name = Name;
            model.AmharicName = AmharicName;
            if(jobCost!=null)
            {
                model.Customer = db.OrganizationCustomers.Find(jobCost.CustomerId)?.TradeName;
                if(jobOrder!=null)
                {
                    decimal finishedQuantity = 0;
                    var jobStatus = db.JobStatuses.Where(q => q.JobId.ToString() == jobOrder.JobId.ToString()&&
                    q.JobPhase==ProductionDepartment.SalesDelivery).FirstOrDefault();
                   
                     model.JobOrderNo = jobOrder.JobNo;
                    model.Quantity = jobOrder.Quantity;
                    model.Remaining = jobOrder.Quantity - finishedQuantity;
                    model.ReceiptNo = jobCost.ReciptNo;
                    model.Description = "Code "+db.JobCategories.Find(jobOrder.JobTypeId)?.JobCode;
                    model.SalesPrice = jobOrder.TotalPrice;
                    model.OrderSize = jobOrder.FinishingSize;
                    model.ReceivedDate = jobOrder.ReceivedDate?.ToString("MM/dd/yyyy");
                    model.Date = DateTime.Now.ToString("MM/dd/yyyy");
                    string year = "";
                    if (jobStatus != null)
                    {
                        finishedQuantity = db.JobStatusLineItems.Where(q => q.JobStatusId == jobStatus.JobStatusId).ToList().Sum(s => s.TotalPrice);
                    }
                        var jobCardItems = new List<JobOrderCardItem>();
                      //  var materialCosts =db.IssueItems.Where(q => q.ProductionJobNo == jobOrder.JobNo).ToList();
                        var materialCosts =db.IssueItems/*.Where(q => q.ProductionJobNo == jobOrder.JobNo)*/.ToList();
                    decimal accTotal = 0;
                        foreach(var material in materialCosts)
                        {
                            var jobCardItem = new JobOrderCardItem();
                        var issueReq = db.Issues.FirstOrDefault(q => q.IssueID == material.IssueID);
                        var fiscalYear = db.InventoryPeriods.Find(issueReq?.GLPeriodId);
                            jobCardItem.CostCenter = db.InventoryItems.Find(material.ItemCode)?.Name;
                        if(issueReq!=null)
                        {
                            jobCardItem.Year = fiscalYear.DateFrom.Year.ToString();
                            year = jobCardItem.Year;
                            jobCardItem.Month = issueReq.IssueDate.ToString("MMMM");
                        }
                           
                            //jobCardItem.Reference = material.ProductionJobNo;
                            jobCardItem.MaterialQty = material.IssuedQuantity;
                            jobCardItem.Date = issueReq.IssueDate.Day.ToString();
                            jobCardItem.WorkingHour = 0;
                            jobCardItem.DirectLabor = 0;
                            jobCardItem.DirectMaterial = material.Total;
                            jobCardItem.MOH = 0;
                        accTotal += material.Total;
                            jobCardItem.Total = accTotal;
                            jobCardItems.Add(jobCardItem);
                        }
                        var labourCosts = db.JobLaborCosts.Where(q => q.JobCostId == jobCost.JobCostId).ToList();
                        foreach (var labour in labourCosts)
                        {
                            var jobCardItem = new JobOrderCardItem();
                            jobCardItem.CostCenter = db.GLChartOfAccountCostCenters.Find(labour.CostCenter)?.Values;
                            jobCardItem.Year = db.GLFiscalYears.Find(labour.GlFiscalYearId)?.Name;
                            year = jobCardItem.Year;

                            jobCardItem.Month = db.GLPeriods.Find(labour.GLPeriodId)?.Name;
                            jobCardItem.Reference = labour.Reference;
                            jobCardItem.MaterialQty = 0;
                            jobCardItem.WorkingHour = 0;
                        jobCardItem.Date = "30";
                        jobCardItem.DirectLabor = labour.PermanetLaborCost+labour.ContractLaborCost;
                            jobCardItem.DirectMaterial = 0;
                            jobCardItem.MOH = labour.ContractOverHeadCost + labour.PermanetOverHeadCost;
                        accTotal += jobCardItem.MOH + jobCardItem.DirectLabor;
                        jobCardItem.Total = accTotal;
                            jobCardItems.Add(jobCardItem);

                        }
                        model.Year = year;
                        model.JobOrderCardItems = jobCardItems;

                    }
                
                   
             }
           
              

            return model;
        }
    }
}
