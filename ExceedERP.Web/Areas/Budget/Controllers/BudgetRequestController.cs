using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetRequestor)]
    public class BudgetRequestController : Controller
    {


        private ExceedDbContext db = new ExceedDbContext();




        public ActionResult Index()
        {
            var costcenterSeg = db.GlSegmentSetups.Where(x => x.SegmentType == DynamicSegmentTypes.CostCenter).Select(x => x.GLSegmentSetupId).ToList();
            ViewData["MainCompanyStructures"] = db.GlChartOfAccountSegments.Where(x => costcenterSeg.Contains(x.GLSegmentSetupId)).ToList();

            ViewData["Branches"] = db.Branch.ToList();
            ViewData["PlanCategory"] = db.PlanCategorys.ToList();
            ViewData["BudgetCategorys"] = db.BudgetCategorys.ToList();
            return View();
        }





        [AuthorizeRoles(BudgetRoles.BudgetTransfer)]
        public ActionResult Transfer()
        {
            var costcenterSeg = db.GlSegmentSetups.Where(x => x.SegmentType == DynamicSegmentTypes.CostCenter).Select(x => x.GLSegmentSetupId).ToList();
            ViewData["MainCompanyStructures"] = db.GlChartOfAccountSegments.Where(x => costcenterSeg.Contains(x.GLSegmentSetupId)).ToList();
            ViewData["Branches"] = db.Branch.ToList();
            ViewData["PlanCategory"] = db.PlanCategorys.ToList();
            ViewData["BudgetCategorys"] = db.BudgetCategorys.ToList();
            return View();
        }





        public ActionResult RequestTransferHistory()
        {
            var costcenterSeg = db.GlSegmentSetups.Where(x => x.SegmentType == DynamicSegmentTypes.CostCenter).Select(x => x.GLSegmentSetupId).ToList();
            ViewData["MainCompanyStructures"] = db.GlChartOfAccountSegments.Where(x => costcenterSeg.Contains(x.GLSegmentSetupId)).ToList();
            ViewData["Branches"] = db.Branch.ToList();
            ViewData["PlanCategory"] = db.PlanCategorys.ToList();
            ViewData["BudgetCategorys"] = db.BudgetCategorys.ToList();
            return View(); 
        }





        #region BudgetRequest/CRUD


        public ActionResult BudgetRequest_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetRequest> BudgetRequest = db.BudgetRequests;
            DataSourceResult result = BudgetRequest.ToDataSourceResult(request, budgetRequest => new BudgetRequest
            {
                AccountCode = budgetRequest.AccountCode,//added
                BudgetRequestID = budgetRequest.BudgetRequestID,
                BudgetGroup = budgetRequest.BudgetGroup,
                BudgetYear = budgetRequest.BudgetYear,
                FromDate = budgetRequest.FromDate,
                ToDate = budgetRequest.ToDate,
                PlanCategoryId = budgetRequest.PlanCategoryId,
                //Amount = budgetRequest.BudgetRequestLine.Sum(x => x.Amount),
                Amount = budgetRequest.Amount,
                BranchId = budgetRequest.BranchId,
                WorkUnit = budgetRequest.WorkUnit,
                Reason = budgetRequest.Reason,
                IsSendForApproval = budgetRequest.IsSendForApproval,
                IsSecondSendForApproval = budgetRequest.IsSecondSendForApproval,
                IsOnlineTransferred = budgetRequest.IsOnlineTransferred,
                IsOnlineApproved = budgetRequest.IsOnlineApproved,
                IsSecondOnlineApproved = budgetRequest.IsSecondOnlineApproved
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequest_Create([DataSourceRequest]DataSourceRequest request, BudgetRequest BudgetRequest)
        {

            if (ModelState.IsValid)
            {
                FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();

                //gets the current year Id if there is any
                int fiscalYear = fiscalYearHelper.GetCurrentFYearId();

                var entity = new BudgetRequest
                {
                    BudgetRequestID = BudgetRequest.BudgetRequestID,
                    Amount = BudgetRequest.Amount,
                    FiscalYearId = fiscalYear,
                    BranchId = BudgetRequest.BranchId,
                    AccountCode = BudgetRequest.AccountCode,//added
                    WorkUnit = BudgetRequest.WorkUnit,
                    Reason = BudgetRequest.Reason,
                    PlanCategoryId = BudgetRequest.PlanCategoryId,
                    BudgetGroup = BudgetRequest.BudgetGroup,
                    FromDate = BudgetRequest.FromDate,
                    ToDate = BudgetRequest.ToDate,
                    BudgetYear = DateTime.Now.Year.ToString(),

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.BudgetRequests.Add(entity);
                db.SaveChanges();


                BudgetRequest.BudgetRequestID = entity.BudgetRequestID;
            }


            return Json(new[] { BudgetRequest }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequest_Update([DataSourceRequest]DataSourceRequest request, BudgetRequest BudgetRequest)
        {

            if (ModelState.IsValid)
            {
                FiscalYearHelper fiscalYearHelper = new FiscalYearHelper();

                int fiscalYear = fiscalYearHelper.GetCurrentFYearId();

                var entity = new BudgetRequest
                {
                    AccountCode = BudgetRequest.AccountCode,//added
                    BudgetRequestID = BudgetRequest.BudgetRequestID,
                    Amount = BudgetRequest.Amount,
                    BranchId = BudgetRequest.BranchId,
                    FiscalYearId = fiscalYear,
                    WorkUnit = BudgetRequest.WorkUnit,
                    Reason = BudgetRequest.Reason,
                    BudgetGroup = BudgetRequest.BudgetGroup,
                    BudgetYear = DateTime.Now.Year.ToString(),
                    FromDate = BudgetRequest.FromDate,
                    ToDate = BudgetRequest.ToDate,
                    PlanCategoryId = BudgetRequest.PlanCategoryId,

                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetRequests.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { BudgetRequest }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequest_Destroy([DataSourceRequest]DataSourceRequest request, BudgetRequest BudgetRequest)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetRequest
                {
                    BudgetRequestID = BudgetRequest.BudgetRequestID,


                };

                db.BudgetRequests.Attach(entity);
                db.BudgetRequests.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(HttpContext.Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Request number " + BudgetRequest.BudgetRequestID);

            }

            return Json(new[] { BudgetRequest }.ToDataSourceResult(request, ModelState));
        }
        #endregion





        #region BudgetRequestTransferApproval CRUD
        public ActionResult BudgetRequestTransferApproval_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {


            List<BudgetRequestApproval> usageApproval = db.BudgetRequestApprovals.Where(x => x.BudgetRequestId == Id && x.IsTransfer).ToList();
            var Category = db.BudgetRequests.Where(x => x.BudgetRequestID == Id).FirstOrDefault();

            if (Category.IsSecondOnlineApproved == false && Category.IsSecondSendForApproval == true)
            {
                usageApproval.Add(new BudgetRequestApproval()
                {
                    BudgetRequestId = Id,
                    Remark = string.Empty,
                    Status = ApprovalStatuses.Pending,

                });
            }



            return Json(usageApproval.ToDataSourceResult(request, approval => new BudgetRequestApproval()
            {
                BudgetRequestApprovalId = approval.BudgetRequestApprovalId,
                BudgetRequestId = approval.BudgetRequestId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequestTransfer_Approve([DataSourceRequest]DataSourceRequest request, BudgetRequestApproval BudgetRequestApproval, int Id)
        {
            
            if (BudgetRequestApproval.Status == ApprovalStatuses.Approve)
            {


                var entity = db.BudgetRequests.Where(x => x.BudgetRequestID == Id).FirstOrDefault();

                entity.IsSecondOnlineApproved = true;
                entity.SecondOnlineApprovedBy = User.Identity.Name;
                entity.SecondOnlineApprovedTime = DateTime.Now;

                db.BudgetRequests.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                //var RequestLine = db.BudgetRequestLines.Where(x => x.BudgetRequestId == entity.BudgetRequestID).ToList();


                BudgetRequestApproval budgetRequestApproval = new BudgetRequestApproval()
                {

                    BudgetRequestId = entity.BudgetRequestID,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Status = BudgetRequestApproval.Status,
                    IsTransfer = true,
                    Remark = BudgetRequestApproval.Remark,
                };

                db.BudgetRequestApprovals.Add(budgetRequestApproval);
                db.SaveChanges();

                BudgetRequestTransferLog TransferLog = new BudgetRequestTransferLog
                {
                    Amount = entity.Amount,//RequestLine.Sum(x => x.Amount), //changed 
                    RequestNo = entity.BudgetRequestID,
                    BranchId = entity.BranchId,
                    BudgetYear = entity.BudgetYear,
                    FromDate = entity.FromDate,
                    ToDate = entity.ToDate,
                    WorkUnit = entity.WorkUnit,

                    PlanCategoryId = entity.PlanCategoryId
                };

                db.BudgetRequestTransferLogs.Add(TransferLog);
                db.SaveChanges();

            }
            if (BudgetRequestApproval.Status == ApprovalStatuses.Reject)
            {
                var entity = db.BudgetRequests.Where(x => x.BudgetRequestID == Id).FirstOrDefault();

                entity.IsSecondOnlineApproved = true;
                entity.SecondOnlineApprovedBy = User.Identity.Name;
                entity.SecondOnlineApprovedTime = DateTime.Now;
                db.BudgetRequests.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            var approval = new BudgetRequestApproval()
            {

                Status = BudgetRequestApproval.Status,
                BudgetRequestId = Id,
                Remark = BudgetRequestApproval.Remark,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name

            };

            db.BudgetRequestApprovals.Add(approval);
            db.SaveChanges();
            BudgetRequestApproval.BudgetRequestApprovalId = approval.BudgetRequestApprovalId;

           
            return Json(new[] { BudgetRequestApproval }.ToDataSourceResult(request, ModelState));
        }
        #endregion



        #region BudgetRequest/Approve CRUD
        //Get/Budget Request Approval
        public ActionResult BudgetRequestApproval_Read([DataSourceRequest]DataSourceRequest request, int Id)
        {


            List<BudgetRequestApproval> usageApproval = db.BudgetRequestApprovals.Where(x => x.BudgetRequestId == Id).ToList();
            var Category = db.BudgetRequests.Where(x => x.BudgetRequestID == Id).FirstOrDefault();

            if (Category.IsSendForApproval == true && Category.IsOnlineApproved == false)
            {
                usageApproval.Add(new BudgetRequestApproval()
                {
                    BudgetRequestId = Id,
                    Remark = string.Empty,
                    Status = ApprovalStatuses.Pending,

                });
            }



            return Json(usageApproval.ToDataSourceResult(request, approval => new BudgetRequestApproval()
            {
                BudgetRequestApprovalId = approval.BudgetRequestApprovalId,
                BudgetRequestId = approval.BudgetRequestId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }

        //Post/Budget/BudgetRequest Approval/
        /*Approves the Request*/
        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequest_Approve([DataSourceRequest]DataSourceRequest request, BudgetRequestApproval BudgetRequestApproval, int Id)
        {
            try
            {
                AccountHelper accountHelper = new AccountHelper();

                if (BudgetRequestApproval.Status == ApprovalStatuses.Approve)
                {

                    var RequestLines = db.BudgetRequestLines.Where(x => x.BudgetRequestId == Id).ToList();

                    var entity = db.BudgetRequests.Where(x => x.BudgetRequestID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;

                    db.BudgetRequests.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    var headOfficeBankAccount = db.BudgetBankAccountMaps.Where(x => x.IsHeadOffice).FirstOrDefault();
                    if (headOfficeBankAccount != null)
                    {
                        string AccountDesc = accountHelper.GetAccountDesc(headOfficeBankAccount.AccountCode);

                        var budgetRequestTransferDistribution = new BudgetRequestTransferDistribution
                        {
                            BudgetRequestId = Id,
                            AccountCode = headOfficeBankAccount.AccountCode,
                            AccountDesc = AccountDesc,
                            Credit = RequestLines.Sum(x => x.Amount), //maybe change it to entity.Amount
                            Debit = 0,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                        };

                        db.BudgetRequestTransferDistributions.Add(budgetRequestTransferDistribution);
                        db.SaveChanges();
                    }
                    var BranchBankAccount = db.BudgetBankAccountMaps.Where(x => x.BranchId == entity.BranchId).FirstOrDefault();

                    if (BranchBankAccount != null)
                    {
                        string AccountDesc = accountHelper.GetAccountDesc(BranchBankAccount.AccountCode);

                        var budgetRequestTransferDistribution = new BudgetRequestTransferDistribution
                        {
                            BudgetRequestId = Id,
                            AccountCode = BranchBankAccount.AccountCode,
                            AccountDesc = AccountDesc,
                            Debit = RequestLines.Sum(x => x.Amount), //maybe change it to entity.Amount
                            Credit = 0,
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                        };

                        db.BudgetRequestTransferDistributions.Add(budgetRequestTransferDistribution);
                        db.SaveChanges();
                    }
                }
                if (BudgetRequestApproval.Status == ApprovalStatuses.Reject)
                {
                    var entity = db.BudgetRequests.Where(x => x.BudgetRequestID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;


                    entity.IsOnlineApproved = false;
                    entity.IsSendForApproval = false;
                    entity.IsOnlineTransferred = false;
                    db.BudgetRequests.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new BudgetRequestApproval()
                {

                    Status = BudgetRequestApproval.Status,
                    BudgetRequestId = Id,
                    Remark = BudgetRequestApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.BudgetRequestApprovals.Add(approval);
                db.SaveChanges();
                BudgetRequestApproval.BudgetRequestApprovalId = approval.BudgetRequestApprovalId;

            }
            catch (Exception)
            {


            }
            return Json(new[] { BudgetRequestApproval }.ToDataSourceResult(request, ModelState));
        }

        #endregion




        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetTransfer_Approve([DataSourceRequest]DataSourceRequest request, BudgetRequestApproval BudgetRequestApproval, int Id)
        {
            try
            {
                if (BudgetRequestApproval.Status == ApprovalStatuses.Approve)
                {


                    var entity = db.BudgetRequests.Where(x => x.BudgetRequestID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;

                    db.BudgetRequests.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                }
                if (BudgetRequestApproval.Status == ApprovalStatuses.Reject)
                {
                    var entity = db.BudgetRequests.Where(x => x.BudgetRequestID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;


                    entity.IsOnlineApproved = false;
                    entity.IsSendForApproval = false;
                    entity.IsOnlineTransferred = false;
                    db.BudgetRequests.Attach(entity);
                    db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new BudgetRequestApproval()
                {

                    Status = BudgetRequestApproval.Status,
                    BudgetRequestId = Id,
                    Remark = BudgetRequestApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.BudgetRequestApprovals.Add(approval);
                db.SaveChanges();
                BudgetRequestApproval.BudgetRequestApprovalId = approval.BudgetRequestApprovalId;

            }
            catch (Exception)
            {


            }
            return Json(new[] { BudgetRequestApproval }.ToDataSourceResult(request, ModelState));
        }


        
        public void GetXLs(System.Data.DataTable datatable)
        {

            try
            {
                System.Data.DataTable dt = datatable;
                string attachment = "attachment; filename=log.xls";
                Response.ClearContent();
                Response.AddHeader("content-disposition", attachment);
                Response.ContentType = "application/vnd.ms-excel";
                string tab = "";
                foreach (System.Data.DataColumn dc in dt.Columns)
                {
                    Response.Write(tab + dc.ColumnName);
                    tab = "\t";
                }
                Response.Write("\n");
                int i;
                foreach (System.Data.DataRow dr in dt.Rows)
                {
                    tab = "";
                    for (i = 0; i < dt.Columns.Count; i++)
                    {
                        Response.Write(tab + dr[i].ToString());
                        tab = "\t";
                    }
                    Response.Write("\n");
                }
                Response.End();
            }
            catch (Exception)
            { }
        }






        public ActionResult BudgetRequestAccept_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetRequest> BudgetRequest = db.BudgetRequests.Where(x => x.IsOnlineApproved == true);
            DataSourceResult result = BudgetRequest.ToDataSourceResult(request, budgetRequest => new BudgetRequest
            {
                BudgetRequestID = budgetRequest.BudgetRequestID,
                BudgetGroup = budgetRequest.BudgetGroup,
                BudgetYear = budgetRequest.BudgetYear,
                FromDate = budgetRequest.FromDate,
                ToDate = budgetRequest.ToDate,
                IsOnlineTransferred = budgetRequest.IsOnlineTransferred,
                PlanCategoryId = budgetRequest.PlanCategoryId,
                Amount = budgetRequest.Amount,//budgetRequest.BudgetRequestLine.Sum(x => x.Amount),
                BranchId = budgetRequest.BranchId,
                WorkUnit = budgetRequest.WorkUnit,
                Reason = budgetRequest.Reason,
                IsSendForApproval = budgetRequest.IsSendForApproval,
                IsOnlineApproved = budgetRequest.IsOnlineApproved,
                IsSecondSendForApproval = budgetRequest.IsSecondSendForApproval,
                IsSecondOnlineApproved = budgetRequest.IsSecondOnlineApproved

            });

            return Json(result);
        }






        public ActionResult BudgetRequestAccepted_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetRequestTransferLog> BudgetRequest = db.BudgetRequestTransferLogs;
            DataSourceResult result = BudgetRequest.ToDataSourceResult(request, budgetRequest => new
            {
                BudgetRequestID = budgetRequest.RequestNo,
                RequestNo = budgetRequest.RequestNo,
                BudgetYear = budgetRequest.BudgetYear,
                FromDate = budgetRequest.FromDate,
                ToDate = budgetRequest.ToDate,
                PlanCategoryId = budgetRequest.PlanCategoryId,
                Amount = budgetRequest.Amount,
                BranchId = budgetRequest.BranchId,
                WorkUnit = budgetRequest.WorkUnit,
                BudgetRequestTransferLogId = budgetRequest.BudgetRequestTransferLogId,
                Reason = budgetRequest.Reason,
                IsSendForApproval = budgetRequest.IsSendForApproval,
                IsOnlineApproved = budgetRequest.IsOnlineApproved,
                IsSecondOnlineApproved = budgetRequest.BudgetRequest.IsSecondOnlineApproved

            });

            return Json(result);
        }
      






        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetRequest_Transfer([DataSourceRequest]DataSourceRequest request, BudgetRequest BudgetRequest)
        {



            var RequestLine = db.BudgetRequestLines.Where(x => x.BudgetRequestId == BudgetRequest.BudgetRequestID).ToList();
            if (RequestLine.Any())
            {
                BudgetRequest.IsOnlineTransferred = true;
                BudgetRequest.IsSecondOnlineApproved = true;
                BudgetRequest.SecondOnlineApprovedBy = User.Identity.Name;
                BudgetRequest.SecondOnlineApprovedTime = DateTime.Now;
                db.BudgetRequests.Attach(BudgetRequest);
                db.Entry(BudgetRequest).State = EntityState.Modified;
                db.SaveChanges();

                BudgetRequestTransferLog TransferLog = new BudgetRequestTransferLog
                {
                    Amount = RequestLine.Sum(x => x.Amount),
                    RequestNo = BudgetRequest.BudgetRequestID,
                    BranchId = BudgetRequest.BranchId,
                    BudgetYear = BudgetRequest.BudgetYear,
                    FromDate = BudgetRequest.FromDate,
                    ToDate = BudgetRequest.ToDate,
                    WorkUnit = BudgetRequest.WorkUnit,

                    PlanCategoryId = BudgetRequest.PlanCategoryId
                };

                db.BudgetRequestTransferLogs.Add(TransferLog);
                db.SaveChanges();
            }
            return Json(new[] { BudgetRequest }.ToDataSourceResult(request));
        }







        //BudgetRequest/Transfer/sendforapproval
        //send for online approval transfer
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult SendForOnlineTransfer(string Id)
        {
            var Request = db.BudgetRequests.Where(x => x.BudgetRequestID.ToString() == Id).FirstOrDefault();
            if (Request != null)
            {
                Request.IsSecondSendForApproval = true;
                db.BudgetRequests.Attach(Request);
                db.Entry(Request).State = EntityState.Modified;
                db.SaveChanges();
            }
            var costcenterSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.CostCenter);
            var glChartOfAccountCostCenters = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == costcenterSeg.GLSegmentSetupId);
            ViewData["MainCompanyStructures"] = glChartOfAccountCostCenters.ToList();
            ViewData["Branches"] = db.Branch.ToList();
            ViewData["PlanCategory"] = db.PlanCategorys.ToList();
            ViewData["BudgetCategorys"] = db.BudgetCategorys.ToList();
            return View("Transfer");
        }


        



        /*Budget Request Send For Approval */
        public ActionResult SendForOnlineApproval(int Id)
        {

            var BudgetRequests = db.BudgetRequests.Where(x => x.BudgetRequestID == Id).FirstOrDefault();
            if (BudgetRequests != null)
            {
                //if (BudgetRequests.BudgetRequestLine.Any())
                //{
                    BudgetRequests.IsSendForApproval = true;
                    db.BudgetRequests.Attach(BudgetRequests);
                    db.Entry(BudgetRequests).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                //}

            }

            return RedirectToAction("Index", "BudgetRequest", null);
        }







        [AuthorizeRoles(BudgetRoles.BudgetApprover)]
        public ActionResult SecondSendForOnlineApproval(int Id)
        {

            var BudgetRequests = db.BudgetRequests.Where(x => x.BudgetRequestID == Id).FirstOrDefault();
            if (BudgetRequests != null)
            {
                BudgetRequests.IsSecondSendForApproval = true;
                db.BudgetRequests.Attach(BudgetRequests);
                db.Entry(BudgetRequests).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "BudgetRequest", null);
        }







        public JsonResult TransferDistribution(string id, string description)
        {
            ExceedDbContext _db = new ExceedDbContext();
            int myid = int.Parse(id);
            var record = _db.BudgetRequests.Find(myid);
            if (record != null && !record.IsOnlineApproved && !record.IsSecondOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && !record.IsOnlineTransferred)
            {
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in record.BudgetRequestLine)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!string.IsNullOrEmpty(message))
                    {
                        var formattedData = new
                        {
                            Message = message,
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }


                #endregion
                #region check if debit and credit are equal
                decimal debit = 0;
                decimal credit = 0;
                var distribution = db.BudgetRequestTransferDistributions.Where(x => x.BudgetRequestId == record.BudgetRequestID).ToList();
                if (distribution.Any())
                {
                    debit = distribution.Sum(x => x.Debit);
                    credit = distribution.Sum(x => x.Credit);
                }
                if (debit != credit)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is not balanced.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                if (debit == 0)
                {
                    var formattedData = new
                    {
                        Message = "Total debit and total credit is zero.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region send to GL
                string glyear = "";
                string glperiod = "";
                #region get period
                var period = _db.GLPeriods.Where(x => x.DateFrom <= record.DateCreated)
                    .Where(x => x.DateTo >= record.DateCreated).ToList();
                if (period.Any())
                {

                    if (period.Count() == 1)
                    {
                        var myPeriod = period.FirstOrDefault();
                        if (myPeriod != null)
                        {
                            glperiod = myPeriod.Name;
                            glyear = myPeriod.GlFiscalYear.Name.ToString();
                            if (myPeriod.Status == FiscalYearStatus.Close)
                            {
                                var formattedData = new
                                {
                                    Message = "The period " + glperiod +
                                              " is closed,the record cannot be transfered to closed period.",
                                    Status = "NOT_OK"
                                };
                                return Json(formattedData, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            var formattedData = new
                            {
                                Message = "Period not found.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }

                    else
                    {
                        var formattedData = new
                        {
                            Message = "There are multiple periods defined that contain the Invoice date.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There are no periods defined that contain the Invoice date.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
                distribution = db.BudgetRequestTransferDistributions.Where(x => x.BudgetRequestId == record.BudgetRequestID).ToList();

                if (distribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "Budget Transfer",
                        Description = !String.IsNullOrEmpty(description) ? description : "Budget Transfer",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "Budget Transfer",
                        SourcesName = "BudgetTransfer"+description,
                        JournalNumber = "BudgetTransfer." + record.BudgetRequestID.ToString(),
                        SourceId = record.BudgetRequestID.ToString(),
                        Source = JournalSources.BudgetTransfer.ToString(),
                        EffectiveDate = record.DateCreated.Value.Date,
                        OrgBranchName = record.OrgBranchName,
                        Status = "Approved",

                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now
                    };
                    _db.GLRecordJournals.Add(journal);
                    _db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    foreach (var i in distribution)
                    {
                        #region debit and credit

                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            Debit = i.Debit,
                            Credit = i.Credit,
                            AdditionalDescription = i.AdditionalDescription,
                        };
                        _db.GLRecordJournalDetails.Add(journalDetail);
                        _db.SaveChanges();
                        #endregion
                    }
                    #region update record
                    record.IsOnlineTransferred = true;
                    _db.Entry(record).State = EntityState.Modified;
                    _db.SaveChanges();
                    #endregion

                    #region split journal
                    var journaldetails = _db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion
                    var formattedData = new
                    {
                        Message = "Successfully transfered.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);

                }
                #endregion

            }
            else
            {
                var formattedData = new
                {
                    Message = "You cannot transfer this transaction, it is already transfered.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            return null;
        }








        
        public ActionResult BranchDDBind()
        {

            var workunit = db.Branch.Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = s.BranchId.ToString()
            });

            return Json(workunit, JsonRequestBehavior.AllowGet);
        }
    }
}