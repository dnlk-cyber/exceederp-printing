//using System.Data
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CashSalesUser, CRMRoles.CashSalesApprover)]
    public class CRMDepositLineController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CRMDepositLine_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<CRMDepositLine> crmdepositline = db.CRMDepositLine.Where(l => l.CRMDepositId == id);
            DataSourceResult result = crmdepositline.ToDataSourceResult(request, cRMDepositLine => new {
                CRMDepositLineId = cRMDepositLine.CRMDepositLineId,
                CRMDepositId = cRMDepositLine.CRMDepositId,
                ReceiveMethodId = cRMDepositLine.ReceiveMethodId,
                DepositAmount = cRMDepositLine.DepositAmount,
                Reference = cRMDepositLine.Reference,
                Remark = cRMDepositLine.Remark,
                OrgBranchName = cRMDepositLine.OrgBranchName,
                DateCreated = cRMDepositLine.DateCreated,
                LastModified = cRMDepositLine.LastModified,
                CreatedBy = cRMDepositLine.CreatedBy,
                ModifiedBy = cRMDepositLine.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CRMDepositLine_Create([DataSourceRequest]DataSourceRequest request, CRMDepositLine cRMDepositLine, int id)
        {
            if(DuplicateLineItemExists(cRMDepositLine, id))
            {
                ModelState.AddModelError("", "Duplicate line item not allowed");
            }
            if (ModelState.IsValid)
            {
                var entity = new CRMDepositLine
                {
                    ReceiveMethodId = cRMDepositLine.ReceiveMethodId,
                    CRMDepositId = id,
                    DepositAmount = cRMDepositLine.DepositAmount,
                    Reference = cRMDepositLine.Reference,
                    Remark = cRMDepositLine.Remark,
                    OrgBranchName = cRMDepositLine.OrgBranchName,
                    DateCreated = cRMDepositLine.DateCreated,
                    LastModified = cRMDepositLine.LastModified,
                    CreatedBy = cRMDepositLine.CreatedBy,
                    ModifiedBy = cRMDepositLine.ModifiedBy
                };

                db.CRMDepositLine.Add(entity);
                db.SaveChanges();
                cRMDepositLine.CRMDepositLineId = entity.CRMDepositLineId;

                var header = GetDepositHeader(entity.CRMDepositId);
                if (header != null)
                {

                    new CRMPaymentDistibutionHelper().UpdateCRMDepositDistribution(header);
                }
            }

            return Json(new[] { cRMDepositLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CRMDepositLine_Update([DataSourceRequest]DataSourceRequest request, CRMDepositLine cRMDepositLine)
        {
            if (DuplicateLineItemExists(cRMDepositLine, cRMDepositLine.CRMDepositId))
            {
                ModelState.AddModelError("", "Duplicate line item not allowed");
            }
            if (ModelState.IsValid)
            {
                var entity = new CRMDepositLine
                {
                    CRMDepositLineId = cRMDepositLine.CRMDepositLineId,
                    CRMDepositId = cRMDepositLine.CRMDepositId,
                    ReceiveMethodId = cRMDepositLine.ReceiveMethodId,
                    DepositAmount = cRMDepositLine.DepositAmount,
                    Reference = cRMDepositLine.Reference,
                    Remark = cRMDepositLine.Remark,
                    OrgBranchName = cRMDepositLine.OrgBranchName,
                    DateCreated = cRMDepositLine.DateCreated,
                    LastModified = cRMDepositLine.LastModified,
                    CreatedBy = cRMDepositLine.CreatedBy,
                    ModifiedBy = cRMDepositLine.ModifiedBy
                };

                db.CRMDepositLine.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                var header = GetDepositHeader(entity.CRMDepositId);
                if (header != null)
                {
                    new CRMPaymentDistibutionHelper().UpdateCRMDepositDistribution(header);
                }
            }

            return Json(new[] { cRMDepositLine }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CRMDepositLine_Destroy([DataSourceRequest]DataSourceRequest request, CRMDepositLine cRMDepositLine)
        {
            if (ModelState.IsValid)
            {
                var entity = new CRMDepositLine
                {
                    CRMDepositLineId = cRMDepositLine.CRMDepositLineId,
                    CRMDepositId = cRMDepositLine.CRMDepositId,
                    ReceiveMethodId = cRMDepositLine.ReceiveMethodId,
                    DepositAmount = cRMDepositLine.DepositAmount,
                    Reference = cRMDepositLine.Reference,
                    Remark = cRMDepositLine.Remark,
                    OrgBranchName = cRMDepositLine.OrgBranchName,
                    DateCreated = cRMDepositLine.DateCreated,
                    LastModified = cRMDepositLine.LastModified,
                    CreatedBy = cRMDepositLine.CreatedBy,
                    ModifiedBy = cRMDepositLine.ModifiedBy
                };

                db.CRMDepositLine.Attach(entity);
                db.CRMDepositLine.Remove(entity);
                db.SaveChanges();

                var header = GetDepositHeader(entity.CRMDepositId);
                if (header != null)
                {
                    new CRMPaymentDistibutionHelper().UpdateCRMDepositDistribution(header);
                }
            }

            return Json(new[] { cRMDepositLine }.ToDataSourceResult(request, ModelState));
        }
        private bool DuplicateLineItemExists(CRMDepositLine line, int headerId)
        {
            return 
                db.CRMDepositLine
                .Any(l => l.CRMDepositLineId != line.CRMDepositLineId && 
                    l.CRMDepositId == headerId && 
                    l.ReceiveMethodId == line.ReceiveMethodId &&
                    l.Reference == line.Reference);
        }
        private CRMDeposit GetDepositHeader(int id)
        {
            return db.CRMDeposit.Find(id);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
