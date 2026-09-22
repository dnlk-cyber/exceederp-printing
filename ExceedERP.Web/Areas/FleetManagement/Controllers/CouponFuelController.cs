using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;

using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions; 
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
		[AuthorizeRoles(FleetManagementRoles.ResourceApprover)]  
	public class CouponFuelController : Controller
	{
		ExceedDbContext db = new ExceedDbContext();

		
		//
		// GET: /EquipmentIdentityManagement/EquipmentIdentity/
		public ActionResult Index()
		{

            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber + " " + s.SerialNo
            });
            ViewData["Employee"] = db.Person_Employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).ToList();
            return View();
		}

		public ActionResult _Read([DataSourceRequest] DataSourceRequest request)
		{
            IQueryable<CouponFuel>couponFuel = db.CouponFuel;
            DataSourceResult result = couponFuel.ToDataSourceResult(request, CF => new
            {
                ConsumptionPerKM=CF.ConsumptionPerKM,
                CouponFuelId=CF.CouponFuelId,
                CouponNumber=CF.CouponNumber,
                CreatedBy=CF.CreatedBy,
                CurrentKM=CF.CurrentKM,
                DateCreated=CF.DateCreated,
                DiffKM=CF.DiffKM,
                FleetId=CF.FleetId,
                FuelStation=CF.FuelStation,
                IsOnlineApproved=CF.IsOnlineApproved,
                IsOnlineTransferred=CF.IsOnlineTransferred,
                IsSendForApproval=CF.IsSendForApproval,
                IssueItem=CF.IssueItem,
                IsVoid=CF.IsVoid,
                LastModified=CF.LastModified,
                ModifiedBy=CF.ModifiedBy,
                PreKM=CF.PreKM,
                Remark=CF.Remark,
                RequestDate=CF.RequestDate,
                RequestedQuantity=CF.RequestedQuantity,
                Requestor=CF.Requestor,
                Status=CF.Status,
                TotalPrice=CF.TotalPrice,
                UnitPrice=CF.UnitPrice,
                UOM= CF.UOM,
                IsApproved = CF.IsApproved
                
            });
            return Json(result,JsonRequestBehavior.AllowGet);
		

		}
		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult _Create([DataSourceRequest] DataSourceRequest request, CouponFuel couponFuel)
			{
            
            couponFuel.TotalPrice = couponFuel.UnitPrice * couponFuel.RequestedQuantity;
            var checkKM = db.CouponFuel
                            .Where(x => x.FleetId == couponFuel.FleetId)
                            .OrderByDescending(x => x.CouponFuelId)   // or Date/KM column
                            .FirstOrDefault();

            if(checkKM!=null&&checkKM.CurrentKM>couponFuel.CurrentKM)
            {
                ModelState.AddModelError("not allowed", "Current KM reading can not be less than previous KM reading");
            }
            if (checkKM!=null&&checkKM.RequestDate>couponFuel.RequestDate)
            {
                ModelState.AddModelError("not allowed", "Current request date can not be less than previous request date");

            }
            
            if (checkKM!=null)
            {
                couponFuel.PreKM = checkKM.CurrentKM;
                checkKM.IsApproved = true;
            }
            else
            {
                couponFuel.PreKM = 0;                
            }
            
            couponFuel.DiffKM = couponFuel.CurrentKM - couponFuel.PreKM;
            couponFuel.ConsumptionPerKM = couponFuel.DiffKM / couponFuel.RequestedQuantity;
            couponFuel.CreatedBy = User.Identity.Name;
            couponFuel.DateCreated = DateTime.Now;
            if (couponFuel.CurrentKM<0)
            {
                ModelState.AddModelError("can not be less than zero", "Current KM can not be less than zero");
            }
            if (couponFuel.RequestedQuantity<=0)
            {
                ModelState.AddModelError("can not be less than zero", "Requested quantity can not be less than zero");
            }
            if (couponFuel.UnitPrice<=0)
            {
                ModelState.AddModelError("can not be less than zero", "Unit price can not be less than zero");
            }
            var chuckCoupon = db.CouponFuel.FirstOrDefault(x => x.CouponNumber == couponFuel.CouponNumber);
            if(chuckCoupon!=null)
            {
                ModelState.AddModelError("duplicate", "Cupon number exists ");
            }
            if(ModelState.IsValid)
            {
                db.CouponFuel.Add(couponFuel);
                db.SaveChanges();
                db.Entry(checkKM).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
			return Json(new[] { couponFuel }.ToDataSourceResult(request));
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public JsonResult _Update([DataSourceRequest] DataSourceRequest request, CouponFuel cFuel)
		{
            var checkKM = db.CouponFuel
            .Where(x => x.FleetId == cFuel.FleetId
             && x.CouponFuelId != cFuel.CouponFuelId)
    .OrderByDescending(x => x.CouponFuelId)
    .FirstOrDefault();
            if (checkKM != null && checkKM.RequestDate > cFuel.RequestDate)
            {
                ModelState.AddModelError("not allowed", "Current request date can not be less than previous request date");
            }

            var couponFuel = db.CouponFuel.FirstOrDefault(x => x.CouponFuelId == cFuel.CouponFuelId);
            couponFuel.IssueItem = "Fuel";
            couponFuel.TotalPrice = cFuel.UnitPrice * cFuel.RequestedQuantity;
            
            if (couponFuel.PreKM > cFuel.CurrentKM)
            {
                ModelState.AddModelError("not allowed", "Current KM reading can not be less than previous KM reading");
            }
            couponFuel.DiffKM = cFuel.CurrentKM - couponFuel.PreKM;
            couponFuel.RequestedQuantity = cFuel.RequestedQuantity;
            couponFuel.ConsumptionPerKM = couponFuel.DiffKM / couponFuel.RequestedQuantity;
            if (cFuel.CurrentKM < 0)
            {
                ModelState.AddModelError("can not be less than zero", "Current KM can not be less than zero");
            }
            if (cFuel.RequestedQuantity <= 0)
            {
                ModelState.AddModelError("can not be less than zero", "Requested quantity can not be less than zero");
            }
            if (cFuel.UnitPrice <= 0)
            {
                ModelState.AddModelError("can not be less than zero", "Unit price can not be less than zero");
            }
            
            couponFuel.CouponNumber = cFuel.CouponNumber;
            var checkCoupon = db.CouponFuel.FirstOrDefault(x => x.CouponNumber == cFuel.CouponNumber);
            if (checkCoupon != null)
            {
                ModelState.AddModelError("duplicate", "Cupon number exists ");
            }
            couponFuel.RequestDate = cFuel.RequestDate;
            couponFuel.LastModified = DateTime.Now;
            couponFuel.ModifiedBy = User.Identity.Name;
            if (ModelState.IsValid)
            {
                db.Entry(couponFuel).State=System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

            }
            return Json(new[] { cFuel }.ToDataSourceResult(request));
        }

		public JsonResult _Destroy([DataSourceRequest] DataSourceRequest request, CouponFuel couponFuel)
		{
            var cFuel = db.CouponFuel.FirstOrDefault(x => x.CouponFuelId == couponFuel.CouponFuelId);
            db.CouponFuel.Remove(cFuel);
            db.SaveChanges();
            var checkKM = db.CouponFuel
                            .Where(x => x.FleetId == couponFuel.FleetId)
                            .OrderByDescending(x => x.CouponFuelId)   // or Date/KM column
                            .FirstOrDefault();           
            if (checkKM != null)
            {
                checkKM.IsApproved = false;
            }
            db.Entry(checkKM).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return Json(new[] { couponFuel }.ToDataSourceResult(request));
		}

        public JsonResult GetEmployees(string Text)
        {
            
            var allEmployees=db.Person_Employee.ToList();
            if(!string.IsNullOrWhiteSpace(Text))
            {
                allEmployees = allEmployees.Where(x => Text.ToLower().Contains(x.FirstNameEnglish.ToLower()) || Text.ToLower().Contains(x.MiddleNameEnglish.ToLower()) || Text.ToLower().Contains(x.LastNameEnglish.ToLower())).ToList();
            }
            var result = allEmployees.Select(x => new
            {
                Value=x.EmployeeId,
                Text=x.FirstNameEnglish+" "+x.MiddleNameEnglish+" "+x.LastNameEnglish
            }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFleets(string Text)
        {
            var allFleets = db.Fleets.ToList();
            if (!string.IsNullOrWhiteSpace(Text))
            {
                allFleets = allFleets.Where(x => Text.Contains(x.PlateNumber) || Text.Contains(x.SerialNo)).ToList();
            }
            var result=allFleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber + " " + s.SerialNo
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}

