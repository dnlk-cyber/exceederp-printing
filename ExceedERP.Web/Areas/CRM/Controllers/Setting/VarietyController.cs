using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.CRM;
using System.Data.Entity;
using ExceedERP.Web.Areas.CRM.Models;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Web.Areas.CRM
{

    [AuthorizeRoles(CRMRoles.CRMAdmin)] 
    public class VarietyController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Varietys_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Variety> Varietys = db.Varietys;
            DataSourceResult result = Varietys.ToDataSourceResult(request, Variety => new Variety
            {
                VarietyID = Variety.VarietyID,
                VarietyName = Variety.VarietyName,
                ProductName = Variety.ProductName,
                CreatedBy = Variety.CreatedBy,
                DateCreated = Variety.DateCreated,
                LastModified = Variety.LastModified,
                ModifiedBy = Variety.ModifiedBy,
                OrgBranchName = Variety.OrgBranchName,
                Class = Variety.Class


            });

            return Json(result);
        }


        public ActionResult Varietys_Create([DataSourceRequest]DataSourceRequest request, Variety Variety)
        {
            List<Variety> Varietys = db.Varietys.Where(p => p.VarietyName == Variety.VarietyName).ToList();
            if (Varietys.Any())
            {
                ModelState.AddModelError("DuplicateEntrie", "Variety Already exist");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if(String.IsNullOrEmpty(Variety.Class))
                    {
                        Variety.Class = "No Class";
                    }
                    var entity = new Variety
                    {
                        VarietyID = Variety.VarietyID,
                        VarietyName = Variety.VarietyName,
                        ProductName = Variety.ProductName,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                        Class = Variety.Class


                    };

                    db.Varietys.Add(entity);
                    db.SaveChanges();
                    Variety.VarietyID = entity.VarietyID;
                }

            }
            return Json(new[] { Variety }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult Varietys_Update([DataSourceRequest]DataSourceRequest request, Variety Variety)
        {
            List<Variety> Varietys = db.Varietys.Where(p => p.VarietyName == Variety.VarietyName && p.VarietyID != Variety.VarietyID).ToList();
            if (Varietys.Any())
            {
                ModelState.AddModelError("DuplicateEntire", "Variety Already exist");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (String.IsNullOrEmpty(Variety.Class))
                    {
                        Variety.Class = "No Class";
                    }
                    var entity = new Variety
                    {
                        VarietyID = Variety.VarietyID,
                        VarietyName = Variety.VarietyName,
                        ProductName = Variety.ProductName,
                        Class = Variety.Class


                    };

                    db.Varietys.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }
            return Json(new[] { Variety }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult Varietys_Destroy([DataSourceRequest]DataSourceRequest request, Variety Variety)
        {
            if (ModelState.IsValid)
            {
                var entity = new Variety
                {
                    VarietyID = Variety.VarietyID,
                    VarietyName = Variety.VarietyName,
                    ProductName = Variety.ProductName,
                    Class = Variety.Class,


                };

                db.Varietys.Attach(entity);
                db.Varietys.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Variety number " + Variety.VarietyID);
      
            }

            return Json(new[] { Variety }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult VarietyByProductDDBind(string ThingID)
        {

            var varietys = db.Varietys.Where(x => x.ProductName == ThingID).Select(s => new BindingModel
            {
                Name = s.VarietyName,
                Id = s.VarietyID.ToString()
            });
            
            return Json(varietys, JsonRequestBehavior.AllowGet);
        }
        public ActionResult VarietyByProductIDDDBind(string ThingID)
        {
            var productName = db.InventoryItems.Where(x => x.ItemCode == ThingID).FirstOrDefault().Name;
          
            var varietys = db.Varietys.Where(x => x.ProductName == productName).Select(s => new BindingModel
            {
                Name = s.VarietyName,
                Id = s.VarietyID.ToString()
            });

            return Json(varietys, JsonRequestBehavior.AllowGet);
        }
        public ActionResult VarietyByProductNameDDBind(string ThingID)
        {
            var productName = db.InventoryItems.Where(x => x.ItemCode == ThingID.ToString()).FirstOrDefault().Name;

            var varietys = db.Varietys.Where(x => x.ProductName == productName).Select(s => new BindingModel
            {
                Name = s.VarietyName,
                Id = s.VarietyID.ToString()
            });

            return Json(varietys, JsonRequestBehavior.AllowGet);
        }
        public ActionResult VarietyByInventoryDDBind(string ThingID)
        {
            var Product = db.Products.Where(x => x.ProductID.ToString() == ThingID).FirstOrDefault();
            if (Product != null)
            {
                var productName = db.InventoryItems.Where(x => x.ItemCode == Product.ItemCode).FirstOrDefault().Name;

                var varietys = db.Varietys.Where(x => x.ProductName == productName).Select(s => new BindingModel
                {
                    Name = s.VarietyName,
                    Id = s.VarietyID.ToString()
                });

                return Json(varietys, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);

        }
        public ActionResult VarietyDDBind()
        {

            var varietys = db.Varietys.Select(s => new BindingModel
            {
                Name = s.VarietyName,
                Id = s.VarietyID.ToString()
            });

            return Json(varietys, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ClassDDBind()
        {

            var varietys = db.Varietys.Select(s => new BindingModel
            {
                Name = s.Class,
                Id = s.Class
            });

            return Json(varietys.Distinct(), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ClassByProductIDDDBind(string inventoryItemCode)
        {
            var productName = string.Empty;
           var inventoryItem  = db.InventoryItems.Where(x => x.ItemCode == inventoryItemCode).FirstOrDefault();
            if(inventoryItem != null)
            {
                productName = inventoryItem.Name;
            }
            var varietys = db.Varietys.Where(x => x.ProductName == productName).Select(s => new BindingModel
            {
                Name = s.Class,
                Id = s.Class
            });

            return Json(varietys.Distinct(), JsonRequestBehavior.AllowGet);
        }



        public ActionResult ClassByProductNameDDBind(string ProductName)
        {
            var varietys = db.Varietys.Where(x => x.ProductName == ProductName).Select(s => new BindingModel
            {
                Name = s.Class,
                Id = s.Class
            });

            return Json(varietys.Distinct(), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }





    }
}