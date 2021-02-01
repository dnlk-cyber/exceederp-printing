using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Helpers.Finance;

namespace ExceedERP.Web.Areas.Construction.Controllers.Billing
{
    public class WorkInProgressController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var taxRates = db.GLTaxRates;
            var taxWithConcatnatedValue = new List<GLTaxRate>();
            foreach (GLTaxRate taxRate in taxRates)
            {
                var taxName = new GLTaxRate
                {
                    GLTaxRateID = taxRate.GLTaxRateID,
                    TaxName = taxRate.TaxName + "( " + Math.Round(taxRate.Rate, 0) + "% )"
                };
                taxWithConcatnatedValue.Add(taxName);
            }
            ViewData["TaxTypes"] = taxWithConcatnatedValue;
            ViewData["Branchs"] = db.Branch.Select(b => new { b.Name, b.BranchId}).ToList();
            #region concatnated work items
            var workItems = db.WorkItemPerProject.OrderBy(x => x.ItemName).Select(w => new { w.WorkItemPerProjectId, w.ItemName}).ToList();
            //var workItemsWithConcatnatedValue = new List<WorkItemPerProject>();
            //var workItem = new WorkItemPerProject();
            //foreach (var item in workItems)
            //{
            //    var tempWorkItem = new WorkItemPerProject();
            //    if (item.BlockId != null)
            //    {
            //        tempWorkItem = item;
            //        var workItemBlock = db.Blocks.Find(item.BlockId);
            //        if (workItemBlock != null)
            //        {
            //            tempWorkItem.ItemName = item.ItemName + " of " + workItemBlock.Name;
            //        }
            //    }
            //    workItemsWithConcatnatedValue.Add(tempWorkItem);
            //}
            #endregion
            ViewData["WorkItems"] = workItems;
            return View();
        }
        public ActionResult ProjectInformation_Read([DataSourceRequest]DataSourceRequest request)
        {

            var currentUser = User.Identity.Name;
            var assignments = db.ConstructionProjectUserAssignments.Where(assgn => assgn.UserName == currentUser).ToList();
            var ids = new HashSet<int>(assignments.Select(a => a.ProjectInformationId));

            IQueryable<ProjectInformation> projectinformation = db.ProjectInformation
                .Where (x=> (x.ProjectStatus == ProjectStatus .STARTED || x.ProjectStatus == ProjectStatus.APPROVED) && !x.IsDeleted && ids.Contains(x.ProjectInformationId)).AsQueryable();
            DataSourceResult result = projectinformation.ToDataSourceResult(request, projectInformation => new {
                ProjectInformationId = projectInformation.ProjectInformationId,
                Name = projectInformation.Name,
                Code = projectInformation.Code,
                Category = projectInformation.Category,
                ProjectType = projectInformation.ProjectType,
                BranchId = projectInformation.BranchId,
                ContractAmount = projectInformation.ContractAmount,
                ProjectScope = projectInformation.ProjectScope,
                EmployeeId = projectInformation.EmployeeId,
                Client = projectInformation.Client,
                Consultant = projectInformation.Consultant,
                ProjectStatus = projectInformation.ProjectStatus,

                OrgBranchName = projectInformation.OrgBranchName,
                DateCreated = projectInformation.DateCreated,
                LastModified = projectInformation.LastModified,
                CreatedBy = projectInformation.CreatedBy,
                ModifiedBy = projectInformation.ModifiedBy,
            });

            return Json(result);
        }
        public ActionResult WorkInProgress_Read([DataSourceRequest]DataSourceRequest request, int projectId)
        {
            IQueryable<WorkInProgress> progresses = db.WorkInProgress.Where(wip => wip.ProjectInformationId == projectId);
            DataSourceResult result = progresses.ToDataSourceResult(request, progress => new
            {
                WorkInProgressId = progress.WorkInProgressId,
                ProjectInformationId = progress.ProjectInformationId,
                WorkInProgressNumber = progress.WorkInProgressNumber,
                ClientName = progress.ClientName,
                Date = progress.Date,
                WorkInProgressStatus = progress.WorkInProgressStatus,

                OrgBranchName = progress.OrgBranchName,
                DateCreated = progress.DateCreated,
                LastModified = progress.LastModified,
                CreatedBy = progress.CreatedBy,
                ModifiedBy = progress.ModifiedBy?? " ",

                Status = progress.Status,
                IsSendForApproval = progress.IsSendForApproval,
                SendForApprovalBy = progress.SendForApprovalBy,
                SendForApprovalTime = progress.SendForApprovalTime,
                IsOnlineApproved = progress.IsOnlineApproved,
                OnlineApprovedBy = progress.OnlineApprovedBy,
                OnlineApprovedTime = progress.OnlineApprovedTime,
                IsOnlineTransferred = progress.IsOnlineTransferred,
                IsOnlineTransferredBy = progress.IsOnlineTransferredBy,
                OnlineTransferredTime = progress.OnlineTransferredTime,
                IsVoid = progress.IsVoid,
                VoidBy = progress.VoidBy,
                VoidTime = progress.VoidTime,
            });

            return Json(result);
        }

        //private bool CheckExistance(int projectId)
        //{
        //    bool paymentExists = false;
        //    var progress = db.ProjectAdvancePayments.Where(ap => ap.ProjectInformationId == projectId).FirstOrDefault();
        //    if (progress != null)
        //    {
        //        paymentExists = true;
        //    }
        //    return paymentExists;
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkInProgress_Create([DataSourceRequest]DataSourceRequest request, WorkInProgress progress, int projectIdCreate)
        {
            if (ModelState.IsValid)
            {
                //if (CheckExistance(projectIdCreateAdvance))
                //{
                //    ModelState.AddModelError("", "Advance Payment Exists for this project!!");
                //}
                //else
                //{
                var entity = new WorkInProgress
                {
                    //WorkInProgressId = progress.WorkInProgressId,
                    ProjectInformationId = projectIdCreate,
                    WorkInProgressNumber = progress.WorkInProgressNumber,
                    ClientName = ProjectInformationHelper.GetProjectClient(projectIdCreate),
                    Date = progress.Date,
                    WorkInProgressStatus = progress.WorkInProgressStatus,

                    OrgBranchName = progress.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = progress.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = progress.ModifiedBy,

                    Status = OperationStatuses.New.ToString(),
                    IsSendForApproval = progress.IsSendForApproval,
                    SendForApprovalBy = progress.SendForApprovalBy,
                    SendForApprovalTime = progress.SendForApprovalTime,
                    IsOnlineApproved = progress.IsOnlineApproved,
                    OnlineApprovedBy = progress.OnlineApprovedBy,
                    OnlineApprovedTime = progress.OnlineApprovedTime,
                    IsOnlineTransferred = progress.IsOnlineTransferred,
                    IsOnlineTransferredBy = progress.IsOnlineTransferredBy,
                    OnlineTransferredTime = progress.OnlineTransferredTime,
                    IsVoid = progress.IsVoid,
                    VoidBy = progress.VoidBy,
                    VoidTime = progress.VoidTime,
                };
                entity.WorkInProgressNumber = entity.WorkInProgressId.ToString("D10");

                db.WorkInProgress.Add(entity);
                db.SaveChanges();
                progress.WorkInProgressId = entity.WorkInProgressId;
                progress.WorkInProgressNumber = entity.WorkInProgressNumber;

                this.UpdateWIPNumber(entity);
                //}
            }

            return Json(new[] { progress }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkInProgress_Update([DataSourceRequest]DataSourceRequest request, WorkInProgress progress)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkInProgress
                {
                    WorkInProgressId = progress.WorkInProgressId,
                    ProjectInformationId = progress.ProjectInformationId,
                    WorkInProgressNumber = progress.WorkInProgressNumber,
                    ClientName = ProjectInformationHelper.GetProjectClient(progress.ProjectInformationId),
                    Date = progress.Date,
                    WorkInProgressStatus = progress.WorkInProgressStatus,

                    OrgBranchName = progress.OrgBranchName,
                    DateCreated = progress.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = progress.CreatedBy,
                    ModifiedBy = User.Identity.Name,

                    Status = progress.Status,
                    IsSendForApproval = progress.IsSendForApproval,
                    SendForApprovalBy = progress.SendForApprovalBy,
                    SendForApprovalTime = progress.SendForApprovalTime,
                    IsOnlineApproved = progress.IsOnlineApproved,
                    OnlineApprovedBy = progress.OnlineApprovedBy,
                    OnlineApprovedTime = progress.OnlineApprovedTime,
                    IsOnlineTransferred = progress.IsOnlineTransferred,
                    IsOnlineTransferredBy = progress.IsOnlineTransferredBy,
                    OnlineTransferredTime = progress.OnlineTransferredTime,
                    IsVoid = progress.IsVoid,
                    VoidBy = progress.VoidBy,
                    VoidTime = progress.VoidTime,
                };

                db.WorkInProgress.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { progress }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkInProgress_Destroy([DataSourceRequest]DataSourceRequest request, WorkInProgress progress)
        {
            if (ModelState.IsValid)
            {
                var entity = new WorkInProgress
                {
                    WorkInProgressId = progress.WorkInProgressId,
                    ProjectInformationId = progress.ProjectInformationId,
                    WorkInProgressNumber = progress.WorkInProgressNumber,
                    ClientName = progress.ClientName,
                    Date = progress.Date,
                    WorkInProgressStatus = progress.WorkInProgressStatus,

                    OrgBranchName = progress.OrgBranchName,
                    DateCreated = progress.DateCreated,
                    LastModified = progress.LastModified,
                    CreatedBy = progress.CreatedBy,
                    ModifiedBy = progress.ModifiedBy,

                    Status = progress.Status,
                    IsSendForApproval = progress.IsSendForApproval,
                    SendForApprovalBy = progress.SendForApprovalBy,
                    SendForApprovalTime = progress.SendForApprovalTime,
                    IsOnlineApproved = progress.IsOnlineApproved,
                    OnlineApprovedBy = progress.OnlineApprovedBy,
                    OnlineApprovedTime = progress.OnlineApprovedTime,
                    IsOnlineTransferred = progress.IsOnlineTransferred,
                    IsOnlineTransferredBy = progress.IsOnlineTransferredBy,
                    OnlineTransferredTime = progress.OnlineTransferredTime,
                    IsVoid = progress.IsVoid,
                    VoidBy = progress.VoidBy,
                    VoidTime = progress.VoidTime,
                };

                db.WorkInProgress.Attach(entity);
                db.WorkInProgress.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { progress }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateWIPNumber(WorkInProgress workInProgress)
        {
            workInProgress.WorkInProgressNumber = workInProgress.WorkInProgressId.ToString("D10");
            db.WorkInProgress.Attach(workInProgress);
            db.Entry(workInProgress).State = EntityState.Modified;
            db.SaveChanges();
        }
        public JsonResult SendForApproval(int id)
        {
            var record = db.WorkInProgress.Find(id);
            if (record != null && !record.IsSendForApproval && !record.IsOnlineApproved && record.WorkInProgressItems.Count > 0)
            {
                #region validate price and tax

                foreach (var item in record.WorkInProgressItems)
                {
                    if (item.UnitPrice <= 0 || item.GLTaxRateID == 0)
                    {
                        var formattedData3 = new
                        {
                            Message = "Please fill unit price and tax information for all items.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData3, JsonRequestBehavior.AllowGet);
                    }
                }
                #endregion

                //record.OperationStatus = OperationStatuses.WaitingApproval;
                record.Status = Enum.GetName(typeof(OperationStatuses), 4);
                record.IsSendForApproval = true;
                record.SendForApprovalBy = User.Identity.Name;
                record.SendForApprovalTime = DateTime.Now;
                db.WorkInProgress.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                var formattedData = new
                {
                    Message = "Successfully Sent.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.WorkInProgressItems.Count == 0)
            {
                var formattedData = new
                {
                    Message = "Please add line items first.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsSendForApproval)
            {
                var formattedData = new
                {
                    Message = "The record is already sent for approval you cannot send again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is already approved.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            return null;
        }
        public JsonResult TransferDistribution(string id, string description)
        {
            AccountHelper accountHelper = new AccountHelper();
            int myid = int.Parse(id);
            WorkInProgress record = db.WorkInProgress.Find(myid);

            if (record != null && record.IsOnlineApproved && !record.IsOnlineTransferred)
            {
                #region check segment length validation

                foreach (var dist in record.WorkInProgressItems)
                {
                    string message = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(message))
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
                var wipDistributions = db.WorkInProgressDistribution.Where(d => d.WorkInProgressId == record.WorkInProgressId).ToList();
                //var distribution = record.IssueDistribution.ToList();
                if (wipDistributions.Any())
                {
                    debit = Math.Round(wipDistributions.Sum(x => x.Debit), 2);
                    credit = Math.Round(wipDistributions.Sum(x => x.Credit), 2);
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
                string glperiod = "";
                string glyear = "";
                #region get period

                var period = db.ConstructionPeriod.FirstOrDefault(p => p.DateFrom < record.Date && p.DateTo > record.Date);
                //var period = db.InventoryPeriods.Find(record.GLPeriodId);


                if (period != null)
                {
                    var glperiodObj = db.GLPeriods.FirstOrDefault(x => x.Name == period.Name);

                    if (glperiodObj != null)
                    {
                        glperiod = glperiodObj.Name;
                        glyear = glperiodObj.GlFiscalYear.Name;
                        if (glperiodObj.Status == FiscalYearStatus.Close)
                        {
                            var formattedData = new
                            {
                                Message = $"The period { glperiodObj.Name } is closed,the record cannot be transfered to closed period.",
                                Status = "NOT_OK"
                            };
                            return Json(formattedData, JsonRequestBehavior.AllowGet);
                        }
                    }

                    else
                    {
                        var formattedData = new
                        {
                            Message = $"The corresponding GL period {period.Name} is not found.",
                            Status = "NOT_OK"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    var formattedData = new
                    {
                        Message = $"The period is not found in construction period.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }

                #endregion
                if (wipDistributions.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        DateCreated = DateTime.Now,
                        CreatedBy = User.Identity.Name,
                        JournalName = "WIP",
                        Description = !String.IsNullOrEmpty(description) ? description : "WIP",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "WIP",
                        SourcesName = "WIP",
                        JournalNumber = "WIP." + record.WorkInProgressId.ToString(),
                        EffectiveDate = record.Date,
                        Status = "Approved",
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now,
                        OrgBranchName = record.OrgBranchName
                    };

                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    foreach (WorkInProgressDistribution i in wipDistributions)
                    {
                        #region debit and credit

                        var journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            AccountDesc = i.AccountDesc,
                            AdditionalDescription = i.AdditionalDescription,
                            Debit = i.Debit,
                            Credit = i.Credit
                        };
                        db.GLRecordJournalDetails.Add(journalDetail);
                        db.SaveChanges();
                        #endregion
                    }
                    record.IsOnlineTransferred = true;
                    record.IsOnlineTransferredBy = User.Identity.Name;
                    record.OnlineTransferredTime = DateTime.Now;
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                    var formattedData = new
                    {
                        Message = "Successfully transferred.",
                        Status = "OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var formattedData = new
                    {
                        Message = "There is no distribution to be transferred.",
                        Status = "NOT_OK"
                    };
                    return Json(formattedData, JsonRequestBehavior.AllowGet);
                }
                #endregion

            }
            if (record != null && !record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record cannot be transfered before validation.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record != null && record.IsOnlineTransferred)
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        
    }
}