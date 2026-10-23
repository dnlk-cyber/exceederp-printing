using ExceedERP.DataAccess.Context;
using System;
using ExceedERP.Web.Helpers;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.Budget.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetUser)]
    public class BudgetUsageApprovalController : Controller
    {


        private ExceedDbContext db = new ExceedDbContext();

 


        public ActionResult BudgetUsageApproval_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<BudgetUsageApproval> BudgetUsageApprovals = db.BudgetUsageApprovals.Where(x=>x.BudgetUsageID==id);
            DataSourceResult result = BudgetUsageApprovals.ToDataSourceResult(request, usageDetail => new
            {
                BudgetUsageApprovalId = usageDetail.BudgetUsageApprovalId,
                BudgetUsageID = usageDetail.BudgetUsageID,
                Status = usageDetail.Status,
                Remark = usageDetail.Remark,

                DateCreated = usageDetail.DateCreated,
                LastModified = usageDetail.LastModified,
                CreatedBy = usageDetail.CreatedBy,
                ModifiedBy = usageDetail.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetUsageApproval_Create([DataSourceRequest]DataSourceRequest request, BudgetUsageApproval usageDetail, int id)
        {
            var usage = db.BudgetUsages.Find(id);
            if (usage != null && usage.IsOnlineApproved)
            {
                ModelState.AddModelError(String.Empty, "You Can Not Manipulate this Record: It is Already Approved");
            }
            if (usage != null && usage.BudgetUsageDetails.Count <= 0 && usageDetail.Status == ApprovalStatuses.Approve)
            {
                ModelState.AddModelError("no items", "Please add at least one item to the budget usage detail.");
            }
                      

            if (ModelState.IsValid)
            {
               
                if (usageDetail.Status == ApprovalStatuses.Approve)
                {
                    usage.Status = "Approved";
                    usage.IsOnlineApproved = true;
                    usage.OnlineApprovedBy = User.Identity.Name;
                    usage.OnlineApprovedTime = DateTime.Now.Date;
                    db.Entry(usage).State = EntityState.Modified;
                    db.SaveChanges();

                    #region send notification

                    //List<UserVM> users = new List<UserVM>();
                    //users.AddRange(UserInformationHelper.GetUsersInRole("Property_User"));
                    //users.AddRange(UserInformationHelper.GetUsersInRole("Property_Authorizer"));
                    //var uniqueusers = users.DistinctByField(x => x.UserId);

                    //string message = "New store requisition generated.";
                    //foreach (var user in uniqueusers)
                    //{
                    //    NotifModels notif = new NotifModels
                    //    {
                    //        UserId = user.UserName,
                    //        Title = "store requisition",
                    //        Message = message,
                    //        URL = "~/Inventory/StoreRequisitionPropertyReview"
                    //    };
                    //    Notify.NotifyUser(notif);
                    //}

                    #endregion
                }
                else if (usageDetail.Status == ApprovalStatuses.Reject)
                {
                    usage.IsSendForApproval = false;
                    usage.Status = Enum.GetName(typeof(OperationStatuses), 8);
                    db.Entry(usage).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var entity = new BudgetUsageApproval
                {
                    BudgetUsageID = id,
                    Status = usageDetail.Status,
                    Remark = usageDetail.Remark,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.BudgetUsageApprovals.Add(entity);
                db.SaveChanges();
                usageDetail.BudgetUsageApprovalId = entity.BudgetUsageApprovalId;
            }


            return Json(new[] { usageDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetUsageApproval_Update([DataSourceRequest]DataSourceRequest request, BudgetUsageApproval usageDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetUsageApproval
                {
                    BudgetUsageApprovalId = usageDetail.BudgetUsageApprovalId,
                    BudgetUsageID = usageDetail.BudgetUsageID,
                    Status = usageDetail.Status,
                    Remark = usageDetail.Remark,
                    DateCreated =usageDetail.DateCreated,
                    CreatedBy = usageDetail.CreatedBy,
                    LastModified = DateTime.Now,
                    ModifiedBy = User.Identity.Name
                };

                db.BudgetUsageApprovals.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { usageDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BudgetUsageApproval_Destroy([DataSourceRequest]DataSourceRequest request, BudgetUsageApproval usageDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetUsageApproval
                {
                    BudgetUsageApprovalId = usageDetail.BudgetUsageApprovalId,
                    BudgetUsageID = usageDetail.BudgetUsageID,
                    Status = usageDetail.Status,
                    Remark = usageDetail.Remark,
                   
                };

                db.BudgetUsageApprovals.Attach(entity);
                db.BudgetUsageApprovals.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Budget Usage number " + usageDetail.BudgetUsageApprovalId);

            }

            return Json(new[] { usageDetail }.ToDataSourceResult(request, ModelState));
        }

     
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}






