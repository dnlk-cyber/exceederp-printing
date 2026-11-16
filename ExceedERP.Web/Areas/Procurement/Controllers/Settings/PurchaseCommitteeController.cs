using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Procurement.Controllers.Settings
{
    [AuthorizeRoles(ProcurementRoles.ProcurementAdmin)]
    public class PurchaseCommitteeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            ViewData["personEmployees"] = db.Person_Employee.Select(i => new
            {
                Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                Code = i.EmployeeId
            });
        }

        public ActionResult ProformaCommittee_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Read(request, PurchaseCommitteeType.Proforma);
        }

        public ActionResult TenderCommittee_Read([DataSourceRequest] DataSourceRequest request)
        {
            return Read(request, PurchaseCommitteeType.Tender);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaCommittee_Create([DataSourceRequest] DataSourceRequest request, PurchaseCommitteeMember member)
        {
            return Create(request, member, PurchaseCommitteeType.Proforma);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderCommittee_Create([DataSourceRequest] DataSourceRequest request, PurchaseCommitteeMember member)
        {
            return Create(request, member, PurchaseCommitteeType.Tender);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaCommittee_Update([DataSourceRequest] DataSourceRequest request, PurchaseCommitteeMember member)
        {
            return Update(request, member, PurchaseCommitteeType.Proforma);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderCommittee_Update([DataSourceRequest] DataSourceRequest request, PurchaseCommitteeMember member)
        {
            return Update(request, member, PurchaseCommitteeType.Tender);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProformaCommittee_Destroy([DataSourceRequest] DataSourceRequest request, PurchaseCommitteeMember member)
        {
            return Destroy(request, member);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TenderCommittee_Destroy([DataSourceRequest] DataSourceRequest request, PurchaseCommitteeMember member)
        {
            return Destroy(request, member);
        }

        private ActionResult Read(DataSourceRequest request, PurchaseCommitteeType committeeType)
        {
            var members = db.PurchaseCommitteeMembers.Where(x => x.CommitteeType == committeeType);
            var result = members.ToDataSourceResult(request, member => new
            {
                member.PurchaseCommitteeMemberId,
                member.CommitteeType,
                member.EmployeeId,
                member.Title,
                member.OrgBranchName,
                member.DateCreated,
                member.LastModified,
                member.CreatedBy,
                member.ModifiedBy
            });
            return Json(result);
        }

        private ActionResult Create(DataSourceRequest request, PurchaseCommitteeMember member, PurchaseCommitteeType committeeType)
        {
            ValidateMember(member, committeeType, 0);

            if (ModelState.IsValid)
            {
                var entity = new PurchaseCommitteeMember
                {
                    CommitteeType = committeeType,
                    EmployeeId = member.EmployeeId,
                    Title = member.Title,
                    DateCreated = DateTime.Today,
                    CreatedBy = User.Identity.Name
                };
                db.PurchaseCommitteeMembers.Add(entity);
                db.SaveChanges();
                member.PurchaseCommitteeMemberId = entity.PurchaseCommitteeMemberId;
                member.CommitteeType = committeeType;
                UserHelper.OperationLog(Request.UserHostAddress, "Purchase committee", "Success", User.Identity.Name, $"added employee {entity.EmployeeId} to {committeeType} committee", Modules.Procurement);
            }

            return Json(new[] { member }.ToDataSourceResult(request, ModelState));
        }

        private ActionResult Update(DataSourceRequest request, PurchaseCommitteeMember member, PurchaseCommitteeType committeeType)
        {
            ValidateMember(member, committeeType, member.PurchaseCommitteeMemberId);

            if (ModelState.IsValid)
            {
                var entity = db.PurchaseCommitteeMembers.FirstOrDefault(x =>
                    x.PurchaseCommitteeMemberId == member.PurchaseCommitteeMemberId &&
                    x.CommitteeType == committeeType);

                if (entity == null)
                {
                    ModelState.AddModelError("PurchaseCommitteeMemberId", "Committee member was not found.");
                }
                else
                {
                    entity.EmployeeId = member.EmployeeId;
                    entity.Title = member.Title;
                    entity.LastModified = DateTime.Today;
                    entity.ModifiedBy = User.Identity.Name;
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    UserHelper.OperationLog(Request.UserHostAddress, "Purchase committee", "Success", User.Identity.Name, $"updated employee {entity.EmployeeId} on {committeeType} committee", Modules.Procurement);
                }
            }

            return Json(new[] { member }.ToDataSourceResult(request, ModelState));
        }

        private ActionResult Destroy(DataSourceRequest request, PurchaseCommitteeMember member)
        {
            var entity = db.PurchaseCommitteeMembers.Find(member.PurchaseCommitteeMemberId);
            if (entity != null)
            {
                db.PurchaseCommitteeMembers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { member }.ToDataSourceResult(request, ModelState));
        }

        private void ValidateMember(PurchaseCommitteeMember member, PurchaseCommitteeType committeeType, int currentId)
        {
            if (member.EmployeeId <= 0)
            {
                ModelState.AddModelError("EmployeeId", "Please select an employee.");
                return;
            }

            var duplicate = db.PurchaseCommitteeMembers.Any(x =>
                x.CommitteeType == committeeType &&
                x.EmployeeId == member.EmployeeId &&
                x.PurchaseCommitteeMemberId != currentId);

            if (duplicate)
                ModelState.AddModelError("EmployeeId", "This employee is already on the committee.");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
