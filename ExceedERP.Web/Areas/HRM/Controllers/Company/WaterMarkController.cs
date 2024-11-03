using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using System.IO;

namespace ExceedERP.Web.Areas.HRM.Controllers.Company
{
    [AuthorizeRoles(HrmConstants.HRCompanyAdmin, HrmConstants.HRCompanyUser, HrmConstants.HRAdmin)]
    public class WaterMarkController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult WaterMark_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<WaterMark> data = db.WaterMarks.ToList();
            if (!data.Any())
            {
                var company = new WaterMark
                {
                    IsNeened = false,
                    Name = "Name",
                    Abbreviation = "",
                };
                db.WaterMarks.Add(company);
                db.SaveChanges();
            }
            DataSourceResult result = data.AsEnumerable().ToDataSourceResult(request, item => new
            {
                item.WaterMarkId,
                item.Name,
                item.Abbreviation,
                item.IsNeened,
                item.WaterMarkFileName,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WaterMark_Create([DataSourceRequest] DataSourceRequest request, WaterMark waterMark)
        {
            if (ModelState.IsValid)
            {
                var entity = new WaterMark
                {
                   Name = waterMark.Name,
                   Abbreviation = waterMark.Abbreviation,
                   WaterMarkFileName = waterMark.WaterMarkFileName,
                   IsNeened = waterMark.IsNeened
                };

                db.WaterMarks.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { waterMark }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WaterMark_Update([DataSourceRequest] DataSourceRequest request, WaterMark waterMark)
        {
            if (ModelState.IsValid)
            {
                var entity = new WaterMark
                {
                    WaterMarkId = waterMark.WaterMarkId,
                    Name = waterMark.Name,
                    Abbreviation = waterMark.Abbreviation,
                    WaterMarkFileName = waterMark.WaterMarkFileName,
                    IsNeened = waterMark.IsNeened
                };

                db.WaterMarks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { waterMark }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WaterMark_Destroy([DataSourceRequest] DataSourceRequest request, WaterMark waterMark)
        {
            if (ModelState.IsValid)
            {
                var entity = new WaterMark
                {
                    WaterMarkId = waterMark.WaterMarkId,
                    Name = waterMark.Name,
                    Abbreviation = waterMark.Abbreviation,
                    WaterMarkFileName = waterMark.WaterMarkFileName,
                    IsNeened = waterMark.IsNeened
                };

                db.WaterMarks.Attach(entity);
                db.WaterMarks.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { waterMark }.ToDataSourceResult(request, ModelState));
        }
     
        public void WaterMarkImage_Upload(int id, IEnumerable<HttpPostedFileBase> watermarkImage)
        {
            if (ModelState.IsValid)
            {
                if (watermarkImage != null && watermarkImage.Count() > 0)
                {
                    foreach (var file in watermarkImage)
                    {
                        string path = HttpContext.Server.MapPath("~/Content/Image/WaterMark/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var newFileName = "watermark-image-" + id + Path.GetExtension(file.FileName);
                        //var logoFileName = file.FileName;
                        var filePath = path + newFileName;
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        file.SaveAs(filePath);

                        var water = db.WaterMarks.Find(id);
                        if (water != null)
                        {
                            water.WaterMarkFileName = newFileName;

                            db.WaterMarks.Attach(water);
                            db.Entry(water).State = EntityState.Modified;
                            db.SaveChanges();
                        }


                    }
                }


            }

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
