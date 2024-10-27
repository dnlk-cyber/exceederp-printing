using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Common.Controllers.DocumentApproval
{
    public class AssignApprovalGroupAssignmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AssignApprovalGroupAssignments_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AssignApprovalGroupAssignment> assignapprovalgroupassignments = db.AssignApprovalGroupAssignments.Where(x=>x.AssignApprovalGroupId==id);
            DataSourceResult result = assignapprovalgroupassignments.ToDataSourceResult(request, assignApprovalGroupAssignment => new {
                AssignApprovalGroupAssignmentId = assignApprovalGroupAssignment.AssignApprovalGroupAssignmentId,
                AssignApprovalGroupId = assignApprovalGroupAssignment.AssignApprovalGroupId,
                ApprovalGroupId = assignApprovalGroupAssignment.ApprovalGroupId,
                DocumentType = assignApprovalGroupAssignment.DocumentType,
                From = assignApprovalGroupAssignment.From,
                To = assignApprovalGroupAssignment.To
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignApprovalGroupAssignments_Create(int id,[DataSourceRequest]DataSourceRequest request, AssignApprovalGroupAssignment assign)
        {

            
            var approvalgroup = db.ApprovalGroups.Find(assign.ApprovalGroupId);
            if (approvalgroup != null)
            {
                var document = db.DocumentApprovalRules.Find(approvalgroup.DocumentApprovalRuleId);
                if (document != null) assign.DocumentType = document.DocumentType;
            }

            var exist = db.AssignApprovalGroupAssignments
                .Where(x => x.DocumentType == assign.DocumentType)
                .Where(x => x.AssignApprovalGroupId == id).ToList();

            if(exist.Any())
                ModelState.AddModelError("exist","Assignment for the same document type already exisits.");

            if (ModelState.IsValid)
            {
                var entity = new AssignApprovalGroupAssignment
                {
                    AssignApprovalGroupId = id,
                    ApprovalGroupId = assign.ApprovalGroupId,
                    DocumentType = assign.DocumentType,
                    From = assign.From,
                    To = assign.To
                };

                db.AssignApprovalGroupAssignments.Add(entity);
                db.SaveChanges();
                assign.AssignApprovalGroupAssignmentId = entity.AssignApprovalGroupAssignmentId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Assign approval group detail", "Success", User.Identity.Name, $"created detail of group ID {assign.ApprovalGroupId} , document type {assign.DocumentType}, from assignment group {assign.AssignApprovalGroupId} ", Modules.Administration);
                #endregion
            }

            return Json(new[] { assign }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignApprovalGroupAssignments_Update([DataSourceRequest]DataSourceRequest request, AssignApprovalGroupAssignment assign)
        {
            var approvalgroup = db.ApprovalGroups.Find(assign.ApprovalGroupId);
            if (approvalgroup != null)
            {
                var document = db.DocumentApprovalRules.Find(approvalgroup.DocumentApprovalRuleId);
                if (document != null) assign.DocumentType = document.DocumentType;
            }

            var exist = db.AssignApprovalGroupAssignments
                .Where(x => x.AssignApprovalGroupAssignmentId != assign.AssignApprovalGroupAssignmentId)
                .Where(x => x.DocumentType == assign.DocumentType)
                .Where(x => x.AssignApprovalGroupId == assign.AssignApprovalGroupId).ToList();

            if (exist.Any())
                ModelState.AddModelError("exist", "Assignment for the same document type already exisits.");
            if (ModelState.IsValid)
            {
                var entity = new AssignApprovalGroupAssignment
                {
                    AssignApprovalGroupAssignmentId = assign.AssignApprovalGroupAssignmentId,
                    AssignApprovalGroupId = assign.AssignApprovalGroupId,
                    ApprovalGroupId = assign.ApprovalGroupId,
                    DocumentType = assign.DocumentType,
                    From = assign.From,
                    To = assign.To
                };

                db.AssignApprovalGroupAssignments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Assign approval group detail", "Success", User.Identity.Name, $"updated detail of group ID {assign.ApprovalGroupId} , document type {assign.DocumentType}, from assignment group {assign.AssignApprovalGroupId} ", Modules.Administration);
                #endregion
            }

            return Json(new[] { assign }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssignApprovalGroupAssignments_Destroy([DataSourceRequest]DataSourceRequest request, AssignApprovalGroupAssignment assign)
        {
            if (ModelState.IsValid)
            {
                var entity = new AssignApprovalGroupAssignment
                {
                    AssignApprovalGroupAssignmentId = assign.AssignApprovalGroupAssignmentId,
                    AssignApprovalGroupId = assign.AssignApprovalGroupId,
                    ApprovalGroupId = assign.ApprovalGroupId,
                    DocumentType = assign.DocumentType,
                    From = assign.From,
                    To = assign.To
                };

                db.AssignApprovalGroupAssignments.Attach(entity);
                db.AssignApprovalGroupAssignments.Remove(entity);
                db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted approval group assignment detail of group ID {assign.ApprovalGroupId} , document type {assign.DocumentType}, from assignment group {assign.AssignApprovalGroupId} ");
                #endregion
            }

            return Json(new[] { assign }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
