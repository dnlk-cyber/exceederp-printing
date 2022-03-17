using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.Core.Domain.PrintingEstimation.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.ProductionFollowUp.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.ProductionFollowUp.DataSource
{
    [DataObject]
    public class JobOrderODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<JobOrderVM> GetJobOrder(string id)
        {
            List<JobOrderVM> jobOrderList = new List<JobOrderVM>();

            if(id != null)
            {
                int.TryParse(id, out int jobId);

                var job = db.Jobs.FirstOrDefault(x=>x.JobId == jobId);

                var jobOrder = new JobOrderVM();
                var customerName = db.OrganizationCustomers.Find(job.CustomerId)?.TradeName;
                jobOrder.JobNo = job.JobNo;
                jobOrder.CustomerName = customerName;
                var cus = db.Companies.FirstOrDefault();
                jobOrder.ContactName = cus?.TradeName;
                //jobOrder.ContactPhone = db.Addresses.Where(q => q.Reference == cus.MainCompanyId.ToString() && q.Category == AddressType.MOBILE_PHONE_1).FirstOrDefault()?.Value;
                //jobOrder.City = job.City;
                //jobOrder.SubCity = job.SubCity;
                //jobOrder.Wereda = job.Woreda;
                //jobOrder.Pobox = job.POBox;             


                jobOrder.Quantity = job.Quantity;
                jobOrder.Copies = job.Copies;
                jobOrder.TotalImpression = job.TotalImpression;
                jobOrder.TextWeight = job.TextWeight;
                jobOrder.ColorText = job.ColorText;
                jobOrder.FinishingSize = job.FinishingSize;
                jobOrder.PrintingLine = job.PrintingLine;
                jobOrder.PrintingMachine = job.PrintingMachine;
                jobOrder.Sewing = job.Sewing;           
                jobOrder.NoOfPage = job.Pages;
                jobOrder.CoverWeight = job.CoverWeight;
                jobOrder.ColorCover = job.Colorcover;
                jobOrder.New = job.New;
                jobOrder.Sewing = job.Sewing;
                jobOrder.PerfectBinding = job.PerfectBinding;
                jobOrder.Repeat = job.Repeat;
                jobOrder.Lamnating = job.Lamnating;
                jobOrder.Varnish = job.Varnish;
                jobOrder.Perforate = job.Perforate;
                jobOrder.Camsur = job.Camsur;


                jobOrder.PaperTypes = new List<JobOrderMaterialVM>();
                jobOrder.FilmOrPlates = new List<JobOrderMaterialVM>();

                var jobType = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == job.JobTypeId);

                var papers = db.JobOrderMaterials.Where(x=>x.JobId == job.JobId && x.Type == JobOrderMaterialType.PaperType).ToList();
                foreach(var mat in papers)
                {
                    var paper = new JobOrderMaterialVM();

                    paper.Name = mat.Name;
                    paper.Type = mat.Type.ToString();
                    paper.Quantity = mat.Quantity;
                    paper.Gram = mat.Gram;
                    paper.Size = mat.Size;
                    paper.SIVNo = mat.SIVNo;                   
                    paper.UnitPrice = Convert.ToDecimal(mat.UnitPrice);
                    paper.BeforeTax = Convert.ToDecimal(mat.UnitPrice * mat.Quantity);

                    if (jobOrder != null)
                    {
                        var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
                        if (tax != null)
                        {
                            paper.Tax = paper.UnitPrice * Convert.ToDecimal(paper.Quantity) * tax.CalculatedRate;
                            paper.TotalPrice = (paper.UnitPrice * Convert.ToDecimal(paper.Quantity)) + paper.Tax;
                        }
                    }
                    
                    paper.Remark = mat.Remark;

                    jobOrder.PaperTypes.Add(paper);
                }


                var filmorplates = db.JobOrderMaterials.Where(x => x.JobId == job.JobId && x.Type == JobOrderMaterialType.FilmOrPlate).ToList();
                foreach (var mat in filmorplates)
                {
                    var plate = new JobOrderMaterialVM();

                    plate.Name = mat.Name;
                    plate.Type = mat.Type.ToString();
                    plate.Quantity = mat.Quantity;
                    plate.Gram = mat.Gram;
                    plate.Size = mat.Size;
                    plate.SIVNo = mat.SIVNo;
                    plate.UnitPrice = Convert.ToDecimal(mat.UnitPrice);
                    plate.BeforeTax = Convert.ToDecimal(mat.UnitPrice * mat.Quantity);

                    if (jobOrder != null)
                    {
                        var tax = db.GLTaxRates.FirstOrDefault(x => x.GLTaxRateID == jobType.GLTaxRateID);
                        if (tax != null)
                        {
                            plate.Tax = plate.UnitPrice * Convert.ToDecimal(plate.Quantity) * tax.CalculatedRate;
                            plate.TotalPrice = (plate.UnitPrice * Convert.ToDecimal(plate.Quantity)) + plate.Tax;
                        }
                    }

                    plate.Remark = mat.Remark;

                    jobOrder.FilmOrPlates.Add(plate);
                }

                decimal priceSumBeforeVat = 0;
                decimal totalPrice = 0;
                decimal taxTotal = 0;

                if(jobOrder.PaperTypes.Any())
                {
                   priceSumBeforeVat = priceSumBeforeVat + jobOrder.PaperTypes.Sum(x=>x.BeforeTax);
                    totalPrice = totalPrice + jobOrder.PaperTypes.Sum(x => x.TotalPrice);
                    taxTotal = taxTotal + jobOrder.PaperTypes.Sum(x => x.Tax);
                }
                if(jobOrder.FilmOrPlates.Any())
                {
                    priceSumBeforeVat = priceSumBeforeVat + jobOrder.FilmOrPlates.Sum(x=>x.BeforeTax);
                    totalPrice = totalPrice + jobOrder.FilmOrPlates.Sum(x => x.TotalPrice);
                    taxTotal = taxTotal + jobOrder.FilmOrPlates.Sum(x => x.Tax);
                }

                jobOrder.PriceBeforeVat = priceSumBeforeVat;
                jobOrder.Tax = taxTotal;
                jobOrder.TotalPrice = totalPrice;
               
                if (jobType != null)
                {
                   if(jobType.HaveParent)
                    {
                        var jobTypeItem = db.JobCategories.FirstOrDefault(x => x.JobCategoryId == jobType.ParentId);
                        if (jobTypeItem != null)
                        {
                            jobOrder.TypeOfJob = jobTypeItem.JobName + "-" + jobType.JobName;
                            jobOrder.SubJobType = jobTypeItem.JobName;
                        }
                    }
                   else
                    {
                        jobOrder.TypeOfJob = jobType.JobName;

                    }



                    //var tax = db.GLTaxRates.FirstOrDefault(x=>x.GLTaxRateID == jobType.GLTaxRateID);
                    //if(tax !=  null)
                    //{
                    //    jobOrder.Tax = jobOrder.PriceBeforeVat * tax.CalculatedRate;
                    //    jobOrder.TotalPrice = jobOrder.PriceBeforeVat + jobOrder.Tax;
                    //}
                }

                jobOrderList.Add(jobOrder);
            }

            return jobOrderList;
        }
    }
}
