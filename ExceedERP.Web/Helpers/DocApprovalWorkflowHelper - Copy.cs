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
        
        public static void ForwardToApprover(int docid,decimal docAmount,string docAcc, string fromuser, ApprovalDocumentType doctype,bool isNew)
        {
           
            int empid = UserInformationHelper.GetEmployeeId(fromuser);
            if (isNew)
            {
                if (DocApprovalHelper.CanOwnerApprove(doctype))
                {
                    Route(docid, docAmount, docAcc, empid, fromuser, doctype);
                }
            }
            else
            {
                int nextSupervisor = DocApprovalHelper.GetNextSupervisor(empid);
                Route(docid,docAmount,docAcc, nextSupervisor,fromuser, doctype);
            }

        }

        public static void Reject(int docid, decimal docAmount, string docAcc, string fromuser, ApprovalDocumentType doctype)
        {
            int empid = UserInformationHelper.GetEmployeeId(fromuser);
            if (DocApprovalHelper.CanOwnerApprove(doctype))
            {
                Route(docid, docAmount, docAcc, empid, fromuser, doctype);
            }
            else
            {
                int nextSupervisor = DocApprovalHelper.GetNextSupervisor(empid);
                Route(docid, docAmount, docAcc, nextSupervisor, fromuser, doctype);
            }

        }

        private static void Route(int docid, decimal docAmount, string docAcc, int empid,string fromuser, ApprovalDocumentType doctype)
        {
            string job = DocApprovalHelper.GetJobInformation(empid);

            var approvalAssignmentForJob = DocApprovalHelper.GetApprovalAssignmentForJob(job,doctype);
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
                        UpdateDocumentApproverInformation(docid, empid, fromuser, doctype,true);
                        SendNotificationToNext(docid, docAmount, empid, fromuser, doctype);
                    }
                    else
                    {
                        //if limit does not allow
                      
                        ForwardMethod method = DocApprovalHelper.GetRoutingMethod(doctype);
                       
                        if (method == ForwardMethod.Hierarchy)
                        {
                            //update approver on doc
                            UpdateDocumentApproverInformation(docid, empid, fromuser, doctype,false);
                            SendNotificationToNext(docid, docAmount, empid, fromuser, doctype);
                        }
                        if (method == ForwardMethod.Direct)
                        {
                            int nextSupervisor = DocApprovalHelper.GetNextSupervisor(empid);
                            Route(docid, docAmount, docAcc, nextSupervisor, fromuser, doctype);
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
                        int nextSupervisor = DocApprovalHelper.GetNextSupervisor(empid);
                        Route(docid, docAmount, docAcc, nextSupervisor, fromuser, doctype);
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
                    int nextSupervisor = DocApprovalHelper.GetNextSupervisor(empid);
                    Route(docid, docAmount, docAcc, nextSupervisor, fromuser, doctype);
                }
             
            }


        }

        private static void UpdateDocumentApproverInformation(int docid, int empid,string fromuser, ApprovalDocumentType doctyp,bool finalApproval)
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
                            //Db.PurchaseOrders.Attach(po);
                            Db.Entry(po).State = EntityState.Modified;
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
    }
}
