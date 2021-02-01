using ExceedERP.Core.Domain.APTS;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class ApprovedDrugRequestController : Controller
    {
        // GET: APTS/ApprovedDrugRequest
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();

            return View();
        }
        public void PopulateViewData()
        {

            ViewData["UnitsList"] = db.DrugUnits.ToList();
            var drug = db.DrugLists.Where(x => x.IsActive);
            ViewData["druglists"] = drug;
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            ViewData["Period"] = db.GLPeriods.ToList();
            ViewData["Branches"] = db.Branch.ToList();

        }
        public ActionResult GetDrugLists()
        {
            var drugLists = db.DrugLists.Where(x => x.IsActive && x.IsOur);
            return Json(drugLists.Select(p => new
            {
                DrugListsId = p.DrugListsId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ApprovedDrugRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<DrugRequest> drugList = new List<DrugRequest>();
            var requests = db.DrugRequests.ToList();
            foreach (var r in requests)
            {
                var status = db.APTSRequestStatus.FirstOrDefault(x => x.Status == DrugRequestStatus.APPROVE && x.RequestId == r.DrugRequestId && x.ReferenceType == "DrugRequest");
                if (status != null)
                {
                    drugList.Add(r);
                }
            }
            DataSourceResult result = drugList.ToDataSourceResult(request, drugRequest => new
            {
                DrugRequestId = drugRequest.DrugRequestId,
                GlFiscalYearId = drugRequest.GlFiscalYearId,
                BranchId = drugRequest.BranchId,
                RequestDate = drugRequest.RequestDate,
                RequestDateAm = drugRequest.RequestDateAm,
                DateCreated = drugRequest.DateCreated,
                Remark = drugRequest.Remark,
                IsClosed = drugRequest.IsClosed,
                IsConfirmed = drugRequest.IsConfirmed,
                IsSendForApproval = drugRequest.IsSendForApproval,
                IsOnlineApproved = drugRequest.IsOnlineApproved,
                IsRejected = drugRequest.IsRejected,
                IsPending = drugRequest.IsPending,
                IsDepartmentApproved = drugRequest.IsDepartmentApproved,

            });

            return Json(result);
        }
        public ActionResult ApprovedDrugRequest_Create([DataSourceRequest]DataSourceRequest request, DrugRequest drugRequest)
        {
            if (ModelState.IsValid)
            {

                var entity = new DrugRequest
                {
                    DrugRequestId = drugRequest.DrugRequestId,
                    GlFiscalYearId = drugRequest.GlFiscalYearId,
                    BranchId = drugRequest.BranchId,
                    RequestDate = drugRequest.RequestDate,
                    RequestDateAm = drugRequest.RequestDateAm,
                    DateCreated = drugRequest.DateCreated,
                    Remark = drugRequest.Remark,
                };
                db.DrugRequests.Add(entity);
                db.SaveChanges();
                drugRequest.DrugRequestId = entity.DrugRequestId;
            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult ApprovedDrugRequest_Update([DataSourceRequest]DataSourceRequest request, DrugRequest drugRequest)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new DrugRequest
                {
                    DrugRequestId = drugRequest.DrugRequestId,
                    GlFiscalYearId = drugRequest.GlFiscalYearId,
                    BranchId = drugRequest.BranchId,
                    RequestDate = drugRequest.RequestDate,
                    RequestDateAm = drugRequest.RequestDateAm,
                    DateCreated = drugRequest.DateCreated,
                    Remark = drugRequest.Remark,
                    IsSendForApproval = drugRequest.IsSendForApproval,
                    IsOnlineApproved = drugRequest.IsOnlineApproved,
                    IsRejected = drugRequest.IsRejected,
                    IsPending = drugRequest.IsPending,
                    IsDepartmentApproved = drugRequest.IsDepartmentApproved,
                };

                db.DrugRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult ApprovedDrugRequest_Destroy([DataSourceRequest]DataSourceRequest request, DrugRequest drugRequest)
        {
            if (ModelState.IsValid)
            {

                var entity = new DrugRequest
                {
                    DrugRequestId = drugRequest.DrugRequestId,
                    GlFiscalYearId = drugRequest.GlFiscalYearId,
                    BranchId = drugRequest.BranchId,
                    RequestDate = drugRequest.RequestDate,
                    RequestDateAm = drugRequest.RequestDateAm,
                    DateCreated = drugRequest.DateCreated,
                    Remark = drugRequest.Remark,
                };
                db.DrugRequests.Attach(entity);
                db.DrugRequests.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public string SendForApproval(int id)
        {
            var status = db.DrugRequests.FirstOrDefault(x => x.DrugRequestId == id);
            if (status != null)
            {
                status.IsSendForApproval = true;
                
                db.Entry(status).State = EntityState.Modified;
                db.SaveChanges();
            }
            return status?.ToString();
        }


        public string Close(int id)
        {
            var record = db.DrugRequests.FirstOrDefault(x => x.DrugRequestId == id);
            //var record = db.Issues.Find(id);
            //var issueItems = db.IssueItems.Where(x => x.IssueID == id).AsNoTracking().ToList();
            var pricesetting = db.AveragePriceSettings.FirstOrDefault();
            #region validation 

            if (record != null)
            {
                if (record.IsOnlineApproved)
                {
                    string message = "You cannot manipulate this record,it  is already approved";
                    ModelState.AddModelError("approved", message);
                }

            }
            else
            {
                ModelState.AddModelError("No Found", "Issue record not found");
            }


            #endregion
            if (record != null)
            {
                record.IsClosed = true;

                //db.DrugRequests.Attach(status);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                var drugItems = db.DrugRequestItems.Where(x => x.DrugRequestId == record.DrugRequestId).ToList();
                foreach (var drugItem in drugItems)
                {
                    if (drugItem.QtyAllowed > 0)
                    {

                        var drugStoreItem = db.DrugStoreItems.FirstOrDefault(x => x.DrugListsId == drugItem.DrugListsId && x.DrugUnitId == drugItem.DrugUnitId);
                        if (drugStoreItem != null)
                        {

                            drugStoreItem.Balance = drugStoreItem.Balance - drugItem.QtyAllowed;

                            db.Entry(drugStoreItem).State = EntityState.Modified;
                            db.SaveChanges();

                            var despensary = db.DespensaryStores.FirstOrDefault(x => x.DrugListsId == drugItem.DrugListsId && x.DrugUnitId == drugItem.DrugUnitId);
                            if (despensary != null)
                            {
                                despensary.Balance = despensary.Balance + drugItem.QtyAllowed;
                                despensary.BegStock = despensary.BegStock + despensary.NewReceived;
                                despensary.NewReceived = drugItem.QtyAllowed;
                                despensary.LastUpdateDate = DateTime.Now;


                                db.Entry(despensary).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                var entity = new DespensaryStore
                                {
                                    DrugListsId = drugItem.DrugListsId,
                                    DrugUnitId = drugItem.DrugUnitId,
                                    Balance = drugItem.QtyAllowed,
                                    BegStock = drugItem.BegStock,
                                    LastUpdateDate = DateTime.Now,
                                    NewReceived = drugItem.QtyAllowed
                                };

                                db.DespensaryStores.Add(entity);
                                db.SaveChanges();
                            }

                        }

                        var receivedItem = db.DrugReceivalDetails.Where(x => x.DrugListsId == drugItem.DrugListsId && x.DrugUnitId == drugItem.DrugUnitId && x.Balance > 0).ToList();

                        if (receivedItem.Any())
                        {
                            decimal balance = 0;
                            decimal qntApprove = drugItem.QtyAllowed;
                            do
                            {

                                var minDate = receivedItem.Min(x => x.ExpiryDate);

                                var filterByDate = receivedItem.FirstOrDefault(x => x.ExpiryDate == minDate);
                                if (filterByDate != null)
                                {

                                    if (filterByDate.Balance >= qntApprove)
                                    {
                                        filterByDate.Balance = filterByDate.Balance - qntApprove;

                                        balance = 0;
                                        db.Entry(filterByDate).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        balance = qntApprove - filterByDate.Balance;
                                        filterByDate.Balance = 0;

                                        db.Entry(filterByDate).State = EntityState.Modified;
                                        db.SaveChanges();
                                        qntApprove = balance;
                                        receivedItem = receivedItem.Where(x => x.Balance > 0).ToList();
                                    }

                                }



                            } while (balance > 0);

                        }

                        var drugList = db.DrugLists.FirstOrDefault(x => x.DrugListsId == drugItem.DrugListsId);
                        if (drugList != null)
                        {
                            var storeItem = db.StoreItemAssignments.FirstOrDefault(x => x.StoreCode == record.StoreCode && x.ItemCode == drugList.ItemCode);
                            if (storeItem != null)
                            {
                                //if (drugItem.QtyAllowed > storeItem.Balance)
                                //{
                                //    drugItem.QtyAllowed = storeItem.Balance;
                                //    storeItem.Balance = 0;

                                //}
                                //else if (drugItem.QtyAllowed <= storeItem.Balance)
                                //{
                                storeItem.Balance = storeItem.Balance - drugItem.QtyAllowed;
                                //}

                                //drugItem.Total = i.IssuedQuantity * i.UnitPrice;
                                //db.Entry(i).State = EntityState.Modified;
                                // db.SaveChanges();

                                db.Entry(storeItem).State = EntityState.Modified;
                                // db.SaveChanges();
                                #region update bin card

                                #region bin card line

                                BinCardLine binCardLine = new BinCardLine
                                {
                                    StoreItemAssignment = storeItem,
                                    ReferenceDoc = "Drug Issue " + record.DrugRequestId.ToString(),
                                    Reference = record.Remark,
                                    Date = record.RequestDate,
                                    IssuedQTY = drugItem.QtyAllowed,
                                    IssuedUPrice = 0,//drugItem.UnitPrice,
                                    IssuedTPrice = 0,//i.Total,
                                    BalanceQTY = storeItem.Balance,
                                    BalanceUPrice = 0,//drugItem.UnitPrice,
                                    BalanceTPrice = 0,//i.UnitPrice * storeItem.Balance,
                                    IssuedTo = "",
                                    //new    
                                    StoreCode = record.StoreCode,
                                    GLPeriodId = 0,//record.GLPeriodId,
                                    TransactionType = BinCardTransactionTypes.Issue,
                                    StatusType = BinCardTransactionStatus.Regular,

                                };
                                db.BinCardLines.Add(binCardLine);
                                // db.SaveChanges();

                                //if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerBranch)
                                //{
                                //    int storeBranchId = ExceedERP.Helpers.Inventory.InventoryHelper.GetStoreBranch(storecode);
                                //    var branchItem = db.BranchItemAssignments.Where(x => x.BranchId == storeBranchId)
                                //        .FirstOrDefault(x => x.ItemCode == i.ItemCode);
                                //    if (branchItem != null)
                                //    {
                                //        branchItem.Balance = branchItem.Balance - i.IssuedQuantity;
                                //        db.Entry(branchItem).State = EntityState.Modified;
                                //        // db.SaveChanges();
                                //    }
                                //}

                               

                                #endregion

                                #endregion
                            }
                        }
                        if (pricesetting != null && pricesetting.Level == AveragePriceLvel.PerCompany)
                        {
                            var stockItem = db.InventoryItems.Find(drugList.ItemCode);
                            if (stockItem != null)
                            {
                                stockItem.Balance = stockItem.Balance - drugItem.QtyAllowed;
                                db.Entry(stockItem).State = EntityState.Modified;
                                // db.SaveChanges();
                            }
                        }

                        db.SaveChanges();

                    }

                }
                //var messages = "successfully closed";
                //return new JavaScriptResult() { Script = "alert('" + messages + "');" };

                return "true";
            }
            // return status?.ToString();
            //return new JavaScriptResult() { Script = "alert('" + record?.ToString() + "');" };
            return "false";
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}