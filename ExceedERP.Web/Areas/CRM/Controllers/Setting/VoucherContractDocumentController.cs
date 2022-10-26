using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Global;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    public class VoucherContractDocumentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult LineItems_Read([DataSourceRequest]DataSourceRequest request, int voucher, VoucherType type)
        {
            IQueryable<VoucherContractDocument> lineItems = db.VoucherContractDocument.Where(c => c.VoucherId == voucher && c.VoucherType == type);
            DataSourceResult result = lineItems.ToDataSourceResult(request, voucherContractDocument => new
            {
                VoucherContractDocumentId = voucherContractDocument.VoucherContractDocumentId,
                FileName = voucherContractDocument.FileName,
                //Content = voucherContractDocument.Content,
                //Remark = voucherContractDocument.Remark,
                VoucherId = voucherContractDocument.VoucherId,
                VoucherType = voucherContractDocument.VoucherType,
                
                CreatedBy = voucherContractDocument.CreatedBy,
                DateCreated = voucherContractDocument.DateCreated,
                LastModified = voucherContractDocument.LastModified,
                ModifiedBy = voucherContractDocument.ModifiedBy,
                OrgBranchName = voucherContractDocument.OrgBranchName,

            });

            return Json(result);
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult LineItem_Create([DataSourceRequest]DataSourceRequest request, VoucherContractDocument voucherContractDocument, int voucher, VoucherType type, HttpPostedFileBase files)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new VoucherContractDocument
        //        {
        //            VoucherContractDocumentId = voucherContractDocument.VoucherContractDocumentId,
        //            FileName = voucherContractDocument.FileName,
        //            Content = voucherContractDocument.Content,
        //            Remark = voucherContractDocument.Remark,
        //            VoucherId = voucher,
        //            VoucherType = type,

        //            CreatedBy = User.Identity.Name,
        //            DateCreated = DateTime.Now,

        //        };

        //        db.VoucherContractDocument.Add(entity);
        //        db.SaveChanges();
        //        voucherContractDocument.VoucherContractDocumentId = entity.VoucherContractDocumentId;
        //        UserHelper.OperationLog(Request.UserHostAddress, "Create", "Success", User.Identity.Name, "Create Contract Document Line Item with id " + voucherContractDocument.VoucherContractDocumentId, Modules.CRM);

        //    }

        //    return Json(new[] { voucherContractDocument }.ToDataSourceResult(request, ModelState));
        //}
        public void ContractDocument_Upload( int voucher, VoucherType type,IEnumerable< HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                if(files != null && files.Count() > 0)
                {
                    foreach(var file in files)
                    {
                        byte[] fileData = null;
                        using (var binaryReader = new BinaryReader(file.InputStream))
                        {
                            fileData = binaryReader.ReadBytes(file.ContentLength);
                        }

                        var entity = new VoucherContractDocument
                        {
                            FileName = file.FileName,
                            Content = fileData,
                            ContentType = file.ContentType,
                            //Remark = voucherContractDocument.Remark,
                            VoucherId = voucher,
                            VoucherType = type,

                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,

                        };

                        db.VoucherContractDocument.Add(entity);
                        db.SaveChanges();
                        UserHelper.OperationLog(Request.UserHostAddress, "Create", "Success", User.Identity.Name, "Create Contract Document Line Item with id " + entity.VoucherContractDocumentId, Modules.CRM);
                    }
                }
               

            }

            //return Json(new[] { voucherContractDocument }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadContractDocument(int documentId) {
            var currentDocument = db.VoucherContractDocument.Find(documentId);
            if(currentDocument == null)
            {
                return null;
            }
            else if (string.IsNullOrEmpty(currentDocument.ContentType))
            {
                ModelState.AddModelError("", "Invalid File");
                return null;
            }
            else
            {
                //var memory = new MemoryStream(currentDocument.Content);
                return File(currentDocument.Content, currentDocument.ContentType, currentDocument.FileName);
            }
        }
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult LineItem_Update([DataSourceRequest]DataSourceRequest request, VoucherContractDocument voucherContractDocument)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new VoucherContractDocument
        //        {
        //            VoucherContractDocumentId = voucherContractDocument.VoucherContractDocumentId,
        //            FileName = voucherContractDocument.FileName,
        //            Content = voucherContractDocument.Content,
        //            Remark = voucherContractDocument.Remark,
        //            VoucherId = voucherContractDocument.VoucherId,
        //            VoucherType = voucherContractDocument.VoucherType,

        //            CreatedBy = voucherContractDocument.CreatedBy,
        //            DateCreated = voucherContractDocument.DateCreated,
        //            LastModified = voucherContractDocument.LastModified,
        //            ModifiedBy = voucherContractDocument.ModifiedBy,
        //            OrgBranchName = voucherContractDocument.OrgBranchName
        //        };

        //        db.VoucherContractDocument.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //        UserHelper.OperationLog(Request.UserHostAddress, "Update", "Success", User.Identity.Name, "Updated Voucher Document Line Item with id " + voucherContractDocument.VoucherContractDocumentId, Modules.CRM);

        //    }

        //    return Json(new[] { voucherContractDocument }.ToDataSourceResult(request, ModelState));
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LineItem_Destroy([DataSourceRequest]DataSourceRequest request, VoucherContractDocument voucherContractDocument)
        {
            if (ModelState.IsValid)
            {
                var entity = new VoucherContractDocument
                {
                    VoucherContractDocumentId = voucherContractDocument.VoucherContractDocumentId,
                    FileName = voucherContractDocument.FileName,
                    Content = voucherContractDocument.Content,
                    Remark = voucherContractDocument.Remark,
                    VoucherId = voucherContractDocument.VoucherId,
                    VoucherType = voucherContractDocument.VoucherType,
                    ContentType = voucherContractDocument.ContentType,

                    CreatedBy = voucherContractDocument.CreatedBy,
                    DateCreated = voucherContractDocument.DateCreated,
                    LastModified = voucherContractDocument.LastModified,
                    ModifiedBy = voucherContractDocument.ModifiedBy,
                    OrgBranchName = voucherContractDocument.OrgBranchName

                };

                db.VoucherContractDocument.Attach(entity);
                db.VoucherContractDocument.Remove(entity);
                db.SaveChanges();
                UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Voucher Contract Line Item with id " + voucherContractDocument.VoucherContractDocumentId, Modules.CRM);

            }

            return Json(new[] { voucherContractDocument }.ToDataSourceResult(request, ModelState));
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}