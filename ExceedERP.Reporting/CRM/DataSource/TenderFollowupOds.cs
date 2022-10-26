using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class TenderFollowupOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TenderFollowupCustomerViewModel> GetCustomers(string customerId, DateTime? startDate, DateTime? endDate)
        {
            List<TenderFollowupCustomerViewModel> list = new List<TenderFollowupCustomerViewModel>();

            var tenders = db.TenderFollowup.ToList();
            if (!string.IsNullOrEmpty(customerId))
            {
                int.TryParse(customerId, out int cId);
                tenders = tenders.Where(t => t.CustomerId == cId).ToList();
            }
            if(startDate != null)
            {
                DateTime sDate = startDate.Value;
                tenders = tenders.Where(t => t.OpeningDate.Date >= sDate.Date).ToList();
            }
            if (endDate != null)
            {
                DateTime date = endDate.Value;
                tenders = tenders.Where(t => t.OpeningDate.Date <= date.Date).ToList();
            }
            var customerIds = new HashSet<int>(tenders.Select(t => t.CustomerId));
            var customers = db.OrganizationCustomers.Where(c => customerIds.Contains(c.OrganizationCustomerID)).ToList();
            foreach (var customer in customers)
            {
                var record = new TenderFollowupCustomerViewModel
                {
                    Customer = customer.TradeName,
                };
                var tendersOfCurrentCustomer = tenders.Where(t => t.CustomerId == customer.OrganizationCustomerID).ToList();
                foreach (var tender in tendersOfCurrentCustomer)
                {
                    var days = (tender.ClosingDate.Date - DateTime.Now.Date).Days;
                    var countDown = "";
                    if (days <= 0)
                    {
                        countDown = "Submission Date Passed";
                    }
                    else if (days == 1)
                    {
                        countDown = $"{days} day";
                    }
                    else
                    {
                        countDown = $"{days} days";
                    }
                    #region 
                    var tenderRecord = new TenderFollowupViewModel
                    {
                        BidBondAmount = tender.BidBondAmount,
                        DateAnnounceBidValidatiy = tender.DateAnnounceBidValidatiy,
                        ServiceCharge = tender.ServiceCharge,
                        ClosingDate = tender.ClosingDate,
                        OpeningDate = tender.OpeningDate,
                        Location = tender.Location,
                        CountDownTillSubmission = countDown,
                        DocumentPreparationStartDate = tender.DocumentPreparationStartDate,
                        Status = tender.Status,
                        TenderResult = tender.TenderResult,
                        LatestLetter = tender.LatestLetter,
                        ContactPerson = tender.ContactPerson,
                        BidAmount = tender.BidAmount,
                        BidSecurityCharge = tender.BidSecurityCharge,
                        BidValidty = tender.BidValidty,
                        ReasonOfNotParticipating = tender.ReasonOfNotParticipating,
                        ReasonOfLosing = tender.ReasonOfLosing,
                        Remark = tender.Remark
                    };
                    #endregion
                    var lines = db.TenderFollowupLineItem.Where(l => l.TenderFollowupId == tender.TenderFollowupId).ToList();

                    foreach (var line in lines)
                    {
                        var item = db.InventoryItems.Find(line.ItemCode);
                        var productRecord = new TenderFollowupPrductViewModel
                        {
                            Product = line.IsExternalProduct? line.Product : item?.Name,
                            Quantity = line.Quantity
                        };
                        tenderRecord.ProductList.Add(productRecord);
                    }
                    record.TendersList.Add(tenderRecord);
                }
                list.Add(record);
            }
            return list;
        }
        private string GetCustomerCategory(string id)
        {
            return db.OrganizationCategories.FirstOrDefault(c => c.OrganizationCategoryID.ToString() == id)?.Description;
        }
    }
}
