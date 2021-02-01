using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core;
using ExceedERP.DataAccess;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.CRM.Controllers
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class VoucherTermController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private BinCardLineReserve _mBinCardLineReserve;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VoucherTerms_Read([DataSourceRequest]DataSourceRequest request, int voucherID)
        {
            IQueryable<VoucherTerm> voucherterms = db.VoucherTerms.Where(vt => vt.Voucher == voucherID.ToString());
            DataSourceResult result = voucherterms.ToDataSourceResult(request, voucherTerm => new
            {
                VoucherTermID = voucherTerm.VoucherTermID,
                Voucher = voucherTerm.Voucher,
                TermID = voucherTerm.TermID,
                Value = voucherTerm.Value,
                CreatedBy = voucherTerm.CreatedBy,
                DateCreated = voucherTerm.DateCreated,
                LastModified = voucherTerm.LastModified,
                ModifiedBy = voucherTerm.ModifiedBy,
                OrgBranchName = voucherTerm.OrgBranchName,
                Remark = voucherTerm.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VoucherTerms_Create([DataSourceRequest]DataSourceRequest request, VoucherTerm voucherTerm, int voucherID,int voucherDefinitionID)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherTerm
                {
                    Voucher = voucherID.ToString(),
                    VoucherDefinitionID=voucherDefinitionID,
                    TermID = voucherTerm.TermID,
                    Value = voucherTerm.Value,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = voucherTerm.Remark
                };
                db.VoucherTerms.Add(entity);
                db.SaveChanges();
                voucherTerm.VoucherTermID = entity.VoucherTermID;
                db = new ExceedDbContext();

                List<BinCardLineReserve> binCardReserveList = db.BinCardLineReserves.Where(bcr => bcr.VoucherID == entity.Voucher).ToList();

                string selectedTermTypeCode = db.VoucherTerms.Where(vt=>vt.VoucherTermID==entity.VoucherTermID).FirstOrDefault().Term.LookupId.ToString();

                foreach (BinCardLineReserve bclr in binCardReserveList)
                {
                    //bclr.InActiveAfterDays=

                }

            }

            return Json(new[] { voucherTerm }.ToDataSourceResult(request, ModelState));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="voucherTerm"></param>
        /// <returns></returns>
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VoucherTerms_Update([DataSourceRequest]DataSourceRequest request, VoucherTerm voucherTerm)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherTerm
                {
                    VoucherTermID = voucherTerm.VoucherTermID,
                    Voucher = voucherTerm.Voucher,
                    TermID = voucherTerm.TermID,
                    Value = voucherTerm.Value,
                    Remark = voucherTerm.Remark
                };

                db.VoucherTerms.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { voucherTerm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VoucherTerms_Destroy([DataSourceRequest]DataSourceRequest request, VoucherTerm voucherTerm)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherTerm
                {
                    VoucherTermID = voucherTerm.VoucherTermID,
                    Voucher = voucherTerm.Voucher,
                    TermID = voucherTerm.TermID,
                    Value = voucherTerm.Value,
                    Remark = voucherTerm.Remark
                };

                db.VoucherTerms.Attach(entity);
                db.VoucherTerms.Remove(entity);
                db.SaveChanges();

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Voucher Term number " + voucherTerm.VoucherTermID);
            }

            return Json(new[] { voucherTerm }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult VoucherTermDDBind()
        {

            var voucherTerm = db.Terms.Select(s => new VoucherTermViewModel
            {
                TermID = s.TermID,
                VoucherTermID =  s.LookupId
               
            });

            return Json(voucherTerm, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
