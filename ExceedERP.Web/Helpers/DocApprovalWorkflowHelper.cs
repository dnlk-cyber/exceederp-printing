using System;
using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Hubs;
using Microsoft.Ajax.Utilities;

namespace ExceedERP.Web.Helpers
{
    public class DocApprovalWorkflowHelper
    {
        
        public static void ForwardToApprover(int docid,decimal docAmount,string docAcc, string fromuser, ApprovalDocumentType doctype,bool isNew,int step=0)
        {
           
            int empid = UserInformationHelper.GetEmployeeId(fromuser);
            int branchid = DocApprovalHelper.GetEmployeeBranch(empid);
            if (isNew)
            {
                ApprovalHierarchy hierarchy = DocApprovalHelper.HierarchyType(doctype);
                if (DocApprovalHelper.CanOwnerApprove(doctype))
                {
                    if (hierarchy == ApprovalHierarchy.Placement_Hierarchy)
                    {
                        Route(docid, docAmount, docAcc, empid, fromuser, doctype);
                    }
                    else if (hierarchy == ApprovalHierarchy.Adhoc_Hierarchy)
                    {
                        //route group
                        RouteGroup(docid, docAmount, docAcc, fromuser, doctype, branchid, step);
                    }
                }
                else
                {                  

                    if (hierarchy == ApprovalHierarchy.Placement_Hierarchy)
                    {
                        int nextApprover = DocApprovalHelper.GetNextSupervisor(empid, branchid);
                        Route(docid, docAmount, docAcc, nextApprover, fromuser, doctype);
                    }
                    else if (hierarchy == ApprovalHierarchy.Adhoc_Hierarchy)
                    {
                        //route group
                        RouteGroup(docid, docAmount, docAcc, fromuser, doctype, branchid, step);
                    }
                }
            }
            else
            {
                ApprovalHierarchy hierarchy = DocApprovalHelper.HierarchyType(doctype);

                if (hierarchy == ApprovalHierarchy.Placement_Hierarchy)
                {
                    int nextApprover = DocApprovalHelper.GetNextSupervisor(empid, branchid);
                    Route(docid, docAmount, docAcc, nextApprover, fromuser, doctype);
                }
                else if (hierarchy == ApprovalHierarchy.Adhoc_Hierarchy)
                {
                    //route group
                    RouteGroup(docid, docAmount, docAcc, fromuser, doctype, branchid,step);
                }

            }

        }

        public static void Reject(int docid, decimal docAmount, string docAcc, string fromuser, ApprovalDocumentType doctype)
        {
            int empid = UserInformationHelper.GetEmployeeId(fromuser);
            int branchid = DocApprovalHelper.GetEmployeeBranch(empid);
            if (DocApprovalHelper.CanOwnerApprove(doctype))
            {
                Route(docid, docAmount, docAcc, empid, fromuser, doctype);
            }
            else
            {
                int nextApprover = DocApprovalHelper.GetNextSupervisor(empid,branchid);
                Route(docid, docAmount, docAcc, nextApprover, fromuser, doctype);
            }

        }

        private static void Route(int docid, decimal docAmount, string docAcc, int empid,string fromuser, ApprovalDocumentType doctype)
        {
            int job = DocApprovalHelper.GetJobInformation(empid);
            int branchid = DocApprovalHelper.GetEmployeeBranch(empid);

            var approvalAssignmentForJob = DocApprovalHelper.GetApprovalAssignmentForJob(job,doctype,branchid);
            if (approvalAssignmentForJob != null)
            {
                var approvalGroup = DocApprovalHelper.GetApprovalGroupForApprovalAssignment(approvalAssignmentForJob);
                if (approvalGroup != null)
                {
                    bool withinAmountLimit = DocApprovalHelper.IsApprovalGroupWithinAmountLimit(approvalGroup, docAmount);
                    bool withinAccountLimit = DocApprovalHelper.IsApprovalGroupWithinAccountLimit(approvalGroup, docAcc);

                    if (withinAccountLimit && withinAmountLimit)
                    {
                        //update approver on doc
                        UpdateDocumentApproverInformation(docid, empid, fromuser, doctype,true,0);
                        SendNotificationToNext(docid, docAmount, empid, fromuser, doctype);
                    }
                    else
                    {
                        //if limit does not allow
                      
                        ForwardMethod method = DocApprovalHelper.GetRoutingMethod(doctype);
                       
                        if (method == ForwardMethod.Hierarchy)
                        {
                            //update approver on doc
                            UpdateDocumentApproverInformation(docid, empid, fromuser, doctype,false,0);
                            SendNotificationToNext(docid, docAmount, empid, fromuser, doctype);
                        }
                        if (method == ForwardMethod.Direct)
                        {
                            int nextApprover = DocApprovalHelper.GetNextSupervisor(empid,branchid);
                            Route(docid, docAmount, docAcc, nextApprover, fromuser, doctype);
                        }
                        
                    }
                    
                }
                else
                {
                    //does not have approval group
                    //log exception 
                    Logger.Debug($"Job approval assignment for employee {empid} not found.");
                    if (empid == 0)
                    {
                        //reachs the end of hierarchy
                        //if does not have assignment..log exception
                        Logger.Debug($"Job approval assignment for employee {empid} not found.The target reached end of hierarchy.");
                    }
                    else
                    {
                        int nextApprover = DocApprovalHelper.GetNextSupervisor(empid,branchid);
                        Route(docid, docAmount, docAcc, nextApprover, fromuser, doctype);
                    }
                }

            }
            else
            {
                //jumps to next supervisior if the current does not have job information

                if (empid == 0)
                {
                    //reachs the end of hierarchy
                    //if does not have assignment..log exception
                }
                else
                {
                    int nextApprover = DocApprovalHelper.GetNextSupervisor(empid,branchid);
                    Route(docid, docAmount, docAcc, nextApprover, fromuser, doctype);
                }
             
            }


        }

        #region adhoc hierarchy
        private static void RouteGroup(int docid, decimal docAmount, string docAcc, string fromuser, ApprovalDocumentType doctype, int branchid,int step)
        {
            ExceedDbContext db = new ExceedDbContext();
            
            var rule = db.DocumentApprovalRules.Where(x => x.DocumentType == doctype).FirstOrDefault();
            if (rule != null)
            {
                var approvalGroup = db.ApprovalGroups.Where(x => x.DocumentApprovalRuleId == rule.DocumentApprovalRuleId).AsQueryable();
                //check specific branch,business unit
                var specificGroup = approvalGroup.Where(x => x.OpertatingUnit == branchid).ToList();
                if (specificGroup.Any())
                {
                    // sort and compare step
                    var nextGroup = specificGroup.OrderBy(x => x.Step).Where(x => x.Step > step).FirstOrDefault();
                    ForwardApprovalGroup(docid, docAmount, docAcc, fromuser, doctype, nextGroup, branchid);
                }
                else
                {
                    //check specific branch,business unit
                    var commonGroup = approvalGroup.Where(x => x.OpertatingUnit == null).ToList();
                    if (commonGroup.Any())
                    {
                        // sort and compare step
                        var nextGroup = commonGroup.OrderBy(x => x.Step).Where(x => x.Step > step).FirstOrDefault();
                        ForwardApprovalGroup(docid,docAmount,docAcc,fromuser,doctype,nextGroup, branchid);
                    }
                    else
                    {
                        // log no group defined
                    }

                }
            }
            
        }
        private static void ForwardApprovalGroup(int docid, decimal docAmount, string docAcc, string fromuser, ApprovalDocumentType doctype, ApprovalGroup group, int branchid)
        {
            ExceedDbContext db = new ExceedDbContext();
            int empid = 0;
            if (group != null)
            {
                bool withinAmountLimit = DocApprovalHelper.IsApprovalGroupWithinAmountLimit(group, docAmount);
                bool withinAccountLimit = DocApprovalHelper.IsApprovalGroupWithinAccountLimit(group, docAcc);

                #region get the approval assignment of the group
                //get the assignemnt
                var assignmentIds = db.AssignApprovalGroupAssignments.Where(x => x.ApprovalGroupId == group.ApprovalGroupId).Select(x => x.AssignApprovalGroupId).ToList();
                var specificAssignment = db.AssignApprovalGroups.Where(x => x.OpertatingUnit == branchid).Where(x => assignmentIds.Contains(x.AssignApprovalGroupId)).ToList();
                if (specificAssignment.Any())
                {
                    var specifiEmp = specificAssignment.Where(x => x.EmployeeId != 0&& x.EmployeeId != null).FirstOrDefault();
                    if(specifiEmp!=null)
                    {
                        empid = (int)specifiEmp.EmployeeId;
                    }
                    else
                    {
                        var specifiJob = specificAssignment.Where(x => x.Job != 0 && x.Job != null).FirstOrDefault();
                        if (specifiJob != null)
                        {
                            empid = DocApprovalHelper.GetEmployeeWithJobTitle((int)specifiJob.Job, branchid);
                        }
                        else
                        {
                            //no job and employee
                        }
                    }
                }
                else
                {
                    var commonAssignment = db.AssignApprovalGroups.Where(x => assignmentIds.Contains(x.AssignApprovalGroupId)).ToList();
                    var specifiEmp = specificAssignment.Where(x => x.EmployeeId != 0 && x.EmployeeId != null).FirstOrDefault();
                    if (specifiEmp != null)
                    {
                        empid = (int)specifiEmp.EmployeeId;
                    }
                    else
                    {
                        var specifiJob = specificAssignment.Where(x => x.Job != 0 && x.Job != null).FirstOrDefault();
                        if (specifiJob != null)
                        {
                            empid = DocApprovalHelper.GetEmployeeWithJobTitle((int)specifiJob.Job, branchid);
                        }
                        else
                        {
                            //no job and employee
                        }
                    }
                }
                #endregion
                if (withinAccountLimit && withinAmountLimit)
                {
                    //update approver on doc
                    UpdateDocumentApproverInformation(docid, empid, fromuser, doctype, true,group.Step);
                    SendNotificationToNext(docid, docAmount, empid, fromuser, doctype);
                }
                else
                {
                    //if limit does not allow

                    ForwardMethod method = DocApprovalHelper.GetRoutingMethod(doctype);

                    if (method == ForwardMethod.Hierarchy)
                    {
                        //update approver on doc
                        UpdateDocumentApproverInformation(docid, empid, fromuser, doctype, false,group.Step);
                        SendNotificationToNext(docid, docAmount, empid, fromuser, doctype);
                    }
                    if (method == ForwardMethod.Direct)
                    {                      
                        RouteGroup(docid, docAmount, docAcc, fromuser, doctype,branchid,group.Step);
                    }

                }
                
            }
            else
            {
                //log 
                //does not have approval group
                //log exception 
                Logger.Debug($"Job approval assignment for employee {empid} not found.");
                if (empid == 0)
                {
                    //reachs the end of hierarchy
                    //if does not have assignment..log exception
                    Logger.Debug($"Job approval assignment for employee {empid} not found.The target reached end of hierarchy.");
                }
                else
                {
                    int nextApprover = DocApprovalHelper.GetNextSupervisor(empid, branchid);
                    Route(docid, docAmount, docAcc, nextApprover, fromuser, doctype);
                }
            }

        }
        #endregion
        private static void UpdateDocumentApproverInformation(int docid, int empid,string fromuser, ApprovalDocumentType doctyp,bool finalApproval,int step)
        {
            ExceedDbContext Db = new ExceedDbContext();

            string nextUser = UserInformationHelper.GetEmployeeUserId(empid);
            if (String.IsNullOrWhiteSpace(nextUser))
            {
                //log user not found
            }
            else
            {
               switch (doctyp)
                {
                    case ApprovalDocumentType.ApInvoice:
                        break;
                    case ApprovalDocumentType.ApPayment:
                        break;
                    case ApprovalDocumentType.ApMiscPayment:
                        break;
                    case ApprovalDocumentType.ApPettyCash:
                        #region petty cash  approval

                        var apPettyCash = Db.ApPettyCash.Find(docid);
                        if (apPettyCash != null)
                        {
                            apPettyCash.ForwardedFrom = fromuser;
                            apPettyCash.ForwardedTo = nextUser;
                            apPettyCash.IsFinalApproval = finalApproval;
                            apPettyCash.CurrentApprovalStep = step;
                            Db.Entry(apPettyCash).State = EntityState.Modified;
                            Db.SaveChanges();
                        }

                        #endregion
                        break;
                    case ApprovalDocumentType.ApTravelOrder:
                        #region travel order  approval

                        var apTravelOrder = Db.ApTravelOrders.Find(docid);
                        if (apTravelOrder != null)
                        {
                            apTravelOrder.ForwardedFrom = fromuser;
                            apTravelOrder.ForwardedTo = nextUser;
                            apTravelOrder.IsFinalApproval = finalApproval;
                            apTravelOrder.CurrentApprovalStep = step;
                            Db.Entry(apTravelOrder).State = EntityState.Modified;
                            Db.SaveChanges();
                        }

                        #endregion
                        break;
                    case ApprovalDocumentType.ApTravelExpenseClaim:
                        #region travel order  approval

                        var apTravelExpenseClaim = Db.ApTravelExpenseClaims.Find(docid);
                        if (apTravelExpenseClaim != null)
                        {
                            apTravelExpenseClaim.ForwardedFrom = fromuser;
                            apTravelExpenseClaim.ForwardedTo = nextUser;
                            apTravelExpenseClaim.IsFinalApproval = finalApproval;
                            apTravelExpenseClaim.CurrentApprovalStep = step;
                            Db.Entry(apTravelExpenseClaim).State = EntityState.Modified;
                            Db.SaveChanges();
                        }

                        #endregion
                        break;
                
                    case ApprovalDocumentType.FixedAssetRequisition:
                        break;
                    case ApprovalDocumentType.PurchaseOrder:
                        #region purchase order approval

                        var po = Db.PurchaseOrders.FirstOrDefault(x => x.PurchaseOrderID==docid);
                        if (po != null)
                        {
                            po.ForwardedFrom = fromuser;
                            po.ForwardedTo = nextUser;
                            po.IsFinalApproval = finalApproval;
                            po.CurrentApprovalStep = step;
                            //Db.PurchaseOrders.Attach(po);
                            Db.Entry(po).State = EntityState.Modified;
                            Db.SaveChanges();
                        }

                        #endregion
                        break;
                    case ApprovalDocumentType.PurchasePaymentRequest:
                        #region purchase purchase payment request

                        var purchasepayment = Db.PurchasePayments.FirstOrDefault(x => x.PurchasePaymentId == docid);
                        if (purchasepayment != null)
                        {
                            purchasepayment.ForwardedFrom = fromuser;
                            purchasepayment.ForwardedTo = nextUser;
                            purchasepayment.IsFinalApproval = finalApproval;
                            purchasepayment.CurrentApprovalStep = step;
                            //Db.PurchaseOrders.Attach(po);
                            Db.Entry(purchasepayment).State = EntityState.Modified;
                            Db.SaveChanges();
                        }

                        #endregion
                        break;
                    case ApprovalDocumentType.ApPaymentRequestSelfCare:
                        #region purchase purchase payment request

                        var payReq = Db.PaymentRequests.FirstOrDefault(x => x.PaymentRequestId == docid);
                        if (payReq != null)
                        {
                            payReq.ForwardedFrom = fromuser;
                            payReq.ForwardedTo = nextUser;
                            payReq.IsFinalApproval = finalApproval;
                            payReq.CurrentApprovalStep = step;
                            //Db.PurchaseOrders.Attach(po);
                            Db.Entry(payReq).State = EntityState.Modified;
                            Db.SaveChanges();
                        }

                        #endregion
                        break;
                    case ApprovalDocumentType.ApTravelOrderSelfCare:
                        #region purchase purchase payment request

                        var travelOr = Db.ApTravelOrders.FirstOrDefault(x => x.ApTravelOrderId == docid);
                        if (travelOr != null)
                        {
                            travelOr.ForwardedFrom = fromuser;
                            travelOr.ForwardedTo = nextUser;
                            travelOr.IsFinalApproval = finalApproval;
                            travelOr.CurrentApprovalStep = step;
                            //Db.PurchaseOrders.Attach(po);
                            Db.Entry(travelOr).State = EntityState.Modified;
                            Db.SaveChanges();
                        }

                        #endregion
                        break;
                    case ApprovalDocumentType.ApTravelExpenseClaimSelfCare:
                        #region purchase purchase payment request

                        var expenseClaim = Db.ApTravelExpenseClaims.FirstOrDefault(x => x.ApTravelExpenseClaimId == docid);
                        if (expenseClaim != null)
                        {
                            expenseClaim.ForwardedFrom = fromuser;
                            expenseClaim.ForwardedTo = nextUser;
                            expenseClaim.IsFinalApproval = finalApproval;
                            expenseClaim.CurrentApprovalStep = step;
                            //Db.PurchaseOrders.Attach(po);
                            Db.Entry(expenseClaim).State = EntityState.Modified;
                            Db.SaveChanges();
                        }

                        #endregion
                        break;

                    case ApprovalDocumentType.ContractManagement:
                        #region contract management approval

                        var contractManagement = Db.ContractManagements.FirstOrDefault(x => x.ContractManagementID == docid);
                        if (contractManagement != null)
                        {
                            contractManagement.ForwardedFrom = fromuser;
                            contractManagement.ForwardedTo = nextUser;
                            contractManagement.IsFinalApproval = finalApproval;
                            contractManagement.CurrentApprovalStep = step;
                            //Db.PurchaseOrders.Attach(po);
                            Db.Entry(contractManagement).State = EntityState.Modified;
                            Db.SaveChanges();
                        }

                        #endregion
                        break;
                } 
            }
            
        }

        private static void SendNotificationToNext(int docid, decimal docAmount, int empid, string fromuser, ApprovalDocumentType doctyp)
        {
            string fromemployee =
                EmployeeHelper.GetEmployeeEnglishName(UserInformationHelper.GetUserEmployeeId(fromuser));
            string nextUser = UserInformationHelper.GetEmployeeUserId(empid);
            if (String.IsNullOrWhiteSpace(nextUser))
            {
                //log user not found
            }
            else
            {
                NotifModels notif = new NotifModels
                {
                    UserId = nextUser,
                    URL= GetDocUrl(doctyp,docid),
                    Message =
                        $"A {doctyp.ToString()} document from {fromemployee} is waiting your approval with amount {docAmount} and document ID {docid}"
                };

                SendNotification(notif);
            }
        }

        public static void SendNotification(NotifModels obj)
        {
            Notify.NotifyUser(obj);
        }

        public static string GetDocUrl(ApprovalDocumentType doctyp,int docid)
        {
           
            ExceedDbContext Db = new ExceedDbContext();
            string url = "";
            switch (doctyp)
            {
                case ApprovalDocumentType.ApInvoice:
                    break;
                case ApprovalDocumentType.ApPayment:
                    break;
                case ApprovalDocumentType.ApMiscPayment:
                    break;
                case ApprovalDocumentType.ApPettyCash:
                  
                    break;
                case ApprovalDocumentType.ApTravelOrder:

                    #region travel order  approval

                    //var apTravelOrder = Db.ApTravelOrders.Find(docid);
                    //if (apTravelOrder != null)
                    //{
                    //    apTravelOrder.ForwardedFrom = fromuser;
                    //    apTravelOrder.ForwardedTo = nextUser;
                    //    apTravelOrder.IsFinalApproval = finalApproval;
                    //    Db.Entry(apTravelOrder).State = EntityState.Modified;
                    //    Db.SaveChanges();
                    //}

                    #endregion
                    break;
                case ApprovalDocumentType.ApTravelExpenseClaim:
                    #region travel order  approval

                    //var apTravelExpenseClaim = Db.ApTravelExpenseClaims.Find(docid);
                    //if (apTravelExpenseClaim != null)
                    //{
                    //    apTravelExpenseClaim.ForwardedFrom = fromuser;
                    //    apTravelExpenseClaim.ForwardedTo = nextUser;
                    //    apTravelExpenseClaim.IsFinalApproval = finalApproval;
                    //    Db.Entry(apTravelExpenseClaim).State = EntityState.Modified;
                    //    Db.SaveChanges();
                    //}

                    #endregion
                    break;

                case ApprovalDocumentType.FixedAssetRequisition:
                    break;
                case ApprovalDocumentType.PurchaseOrder:
                    #region purchase order approval

                    var po = Db.PurchaseOrders.FirstOrDefault(x => x.PurchaseOrderID == docid);
                    if (po != null)
                    {
                       if(po.IsManual)
                        {
                            url = "/Procurement/PurchaseOrderDirect";
                        }
                       else
                        {
                            url = "/Procurement/PurchaseOrder";
                        }

                    }

                    #endregion
                    break;
                case ApprovalDocumentType.PurchasePaymentRequest:
                    #region purchase payment

                    url = "/Procurement/PurchasePayment";

                    #endregion
                    break;

                case ApprovalDocumentType.ContractManagement:
                    #region contract management approval

                    //var contractManagement = Db.ContractManagements.FirstOrDefault(x => x.ContractManagementID == docid);
                    //if (contractManagement != null)
                    //{
                    //    contractManagement.ForwardedFrom = fromuser;
                    //    contractManagement.ForwardedTo = nextUser;
                    //    contractManagement.IsFinalApproval = finalApproval;
                    //    //Db.PurchaseOrders.Attach(po);
                    //    Db.Entry(contractManagement).State = EntityState.Modified;
                    //    Db.SaveChanges();
                    //}

                    #endregion
                    break;

                default:
                    return url;
            }

            return url;

        }

        public static bool CanUserGiveFinalApproval(string user, ApprovalDocumentType doctype, decimal docAmount, int branchid)
        {
            int empId = UserInformationHelper.GetUserEmployeeId(user);
            return DocApprovalHelper.IsEmployeeWithinLimit(empId,doctype,docAmount,branchid);
        }
    }
}
