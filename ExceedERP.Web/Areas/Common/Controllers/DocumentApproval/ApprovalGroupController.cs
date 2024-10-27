using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common.DocumentApproval;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.Common.Controllers.DocumentApproval
{
    public class ApprovalGroupController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ApprovalGroups_Read(int id,[DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApprovalGroup> approvalgroups = db.ApprovalGroups.Where(x=>x.DocumentApprovalRuleId==id);
            DataSourceResult result = approvalgroups.ToDataSourceResult(request, approvalGroup => new {
                DocumentApprovalRuleId = approvalGroup.DocumentApprovalRuleId,
                ApprovalGroupId = approvalGroup.ApprovalGroupId,
                BusinessUnit = approvalGroup.BusinessUnit,
                OpertatingUnit = approvalGroup.OpertatingUnit,
                Name = approvalGroup.Name,
                Step = approvalGroup.Step,
                Enabled = approvalGroup.Enabled,
                Description = approvalGroup.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovalGroups_Create(int id,[DataSourceRequest]DataSourceRequest request, ApprovalGroup approvalGroup)
        {
            #region validation
            if (String.IsNullOrEmpty(approvalGroup.Name))
                ModelState.AddModelError("required","Please give name to approval group");

            if(approvalGroup.Step<=0)
                ModelState.AddModelError("positive", "Step should start at 1");

            var exisit = db.ApprovalGroups
                .Where(x => x.DocumentApprovalRuleId == id)
                .Where(x => x.BusinessUnit == approvalGroup.BusinessUnit)
                .Where(x => x.OpertatingUnit == approvalGroup.OpertatingUnit)
                .Where(x => x.Step == approvalGroup.Step).ToList();
            if (exisit.Any())
                ModelState.AddModelError("exisit", "Step already exisits");

            var nameExisit = db.ApprovalGroups
                .Where(x => x.DocumentApprovalRuleId == id)
                .Where(x => x.BusinessUnit == approvalGroup.BusinessUnit)
                .Where(x => x.OpertatingUnit == approvalGroup.OpertatingUnit)
                .Where(x => x.Name == approvalGroup.Name).ToList();
            if (nameExisit.Any())
                ModelState.AddModelError("exisit", "Name already exisits");
            #endregion

            if (ModelState.IsValid)
            {
                var entity = new ApprovalGroup
                {
                    DocumentApprovalRuleId=id,
                    BusinessUnit = approvalGroup.BusinessUnit,
                    OpertatingUnit = approvalGroup.OpertatingUnit,
                    Name = approvalGroup.Name,
                    Step = approvalGroup.Step,
                    Enabled = approvalGroup.Enabled,
                    Description = approvalGroup.Description
                };

                db.ApprovalGroups.Add(entity);
                db.SaveChanges();
                approvalGroup.ApprovalGroupId = entity.ApprovalGroupId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Approval group", "Success", User.Identity.Name, $"created approval group for business unit {entity.BusinessUnit} ,branch {entity.OpertatingUnit} {entity.Description} with name {entity.Name} step {entity.Step} under rule {entity.DocumentApprovalRuleId} ", Modules.Administration);
                #endregion
            }

            return Json(new[] { approvalGroup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovalGroups_Update([DataSourceRequest]DataSourceRequest request, ApprovalGroup approvalGroup)
        {
            #region validation
            if (String.IsNullOrEmpty(approvalGroup.Name))
                ModelState.AddModelError("required", "Please give name to approval group");

            if (approvalGroup.Step <= 0)
                ModelState.AddModelError("positive", "Step should start at 1");

            var exisit = db.ApprovalGroups
                .Where(x => x.ApprovalGroupId != approvalGroup.ApprovalGroupId)
                .Where(x => x.DocumentApprovalRuleId == approvalGroup.DocumentApprovalRuleId)
                .Where(x => x.BusinessUnit == approvalGroup.BusinessUnit)
                .Where(x => x.OpertatingUnit == approvalGroup.OpertatingUnit)
                .Where(x => x.Step == approvalGroup.Step).ToList();
            if (exisit.Any())
                ModelState.AddModelError("exisit", "Step already exisits");

            var nameExisit = db.ApprovalGroups
                .Where(x=>x.ApprovalGroupId!=approvalGroup.ApprovalGroupId)
                .Where(x => x.DocumentApprovalRuleId == approvalGroup.DocumentApprovalRuleId)
                .Where(x => x.BusinessUnit == approvalGroup.BusinessUnit)
                .Where(x => x.OpertatingUnit == approvalGroup.OpertatingUnit)
                .Where(x => x.Name == approvalGroup.Name).ToList();
            if (nameExisit.Any())
                ModelState.AddModelError("exisit", "Name already exisits");
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ApprovalGroup
                {
                    DocumentApprovalRuleId=approvalGroup.DocumentApprovalRuleId,
                    ApprovalGroupId = approvalGroup.ApprovalGroupId,
                    BusinessUnit = approvalGroup.BusinessUnit,
                    OpertatingUnit = approvalGroup.OpertatingUnit,
                    Name = approvalGroup.Name,
                    Step = approvalGroup.Step,
                    Enabled = approvalGroup.Enabled,
                    Description = approvalGroup.Description
                };

                db.ApprovalGroups.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Approval group", "Success", User.Identity.Name, $"updated approval group for business unit {entity.BusinessUnit} ,branch {entity.OpertatingUnit} {entity.Description} with name {entity.Name} step {entity.Step} under rule {entity.DocumentApprovalRuleId} ", Modules.Administration);
                #endregion
            }

            return Json(new[] { approvalGroup }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApprovalGroups_Destroy([DataSourceRequest]DataSourceRequest request, ApprovalGroup approvalGroup)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApprovalGroup
                {
                    DocumentApprovalRuleId = approvalGroup.DocumentApprovalRuleId,
                    ApprovalGroupId = approvalGroup.ApprovalGroupId,
                    BusinessUnit = approvalGroup.BusinessUnit,
                    OpertatingUnit = approvalGroup.OpertatingUnit,
                    Name = approvalGroup.Name,
                    Step = approvalGroup.Step,
                    Enabled = approvalGroup.Enabled,
                    Description = approvalGroup.Description
                };

                db.ApprovalGroups.Attach(entity);
                db.ApprovalGroups.Remove(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.DeleteLog(Request.UserHostAddress, "Approval group", "Success", User.Identity.Name, $"deleted approval group {approvalGroup.ApprovalGroupId} {approvalGroup.BusinessUnit} ,branch {approvalGroup.OpertatingUnit} {approvalGroup.Description} with name {approvalGroup.Name} step {entity.Step} under rule {entity.DocumentApprovalRuleId} ", Modules.Administration);
                #endregion
            }

            return Json(new[] { approvalGroup }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
