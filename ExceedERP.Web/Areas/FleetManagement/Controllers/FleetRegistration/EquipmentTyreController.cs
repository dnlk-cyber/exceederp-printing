using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class EquipmentTyreController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /FleetManagement/EquipmentTyre/
      

        public string GetDuration( DateTime? dt)
        {

            TimeSpan Duration = (DateTime.Now - DateTime.Parse(dt.ToString()));
            return Duration.TotalHours + " Hours";
        }
        public ActionResult TyreByFleet_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            //List<EquipmentTyreViewModel> tires = new List<EquipmentTyreViewModel>();
            var Tyres = db.EquipmentTyres.Where(x => x.fleetId.ToString() == Id && x.Isattached == true).AsQueryable();
           
            
          

            return Json(Tyres.ToDataSourceResult(request));

        }


        public ActionResult EquipmentTyre_Create([DataSourceRequest] DataSourceRequest request, EquipmentTyreViewModel model, string Id)
        {
            
            IFleetService _FleetService = new FleetService();



            var Battery = db.EquipmentTyres.Where(x => x.SerialNo == model.SerialNo).ToList();
            if (Battery.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("DuplicateSerialNo", " Serial Number Already Registered!");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            var Batterys = db.EquipmentTyres.Where(x => x.SerialNo == model.SerialNo).ToList();
            if (Batterys.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("DuplicateSerialNo", " Store Code Already Registered!");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }

            var fleet = _FleetService.GetById(Guid.Parse(Id));

            if (fleet != null)
                {
                    var  Tyre = new EquipmentTyre();
                    Tyre.EquipmentTyreId = Guid.NewGuid();
                    Tyre.OdoMeter = fleet.CurentOdoMeter;
                    Tyre.Condition = model.Condition;
                Tyre.Make = model.Make;
                Tyre.StoreCode = model.StoreCode;
                Tyre.MakeCountry = model.MakeCountry;
                Tyre.Size = model.Size;
                Tyre.SerialNo = model.SerialNo;
                Tyre.Type = model.Type;
                Tyre.UnitPrice = model.UnitPrice;
                Tyre.Tube = model.Tube;
                Tyre.fleetId = Guid.Parse(Id);
                    Tyre.Isattached = true;
                    Tyre.DateCreated = DateTime.Now;
                    Tyre.CreatedBy = User.Identity.Name;

                    
                    db.EquipmentTyres.Add(Tyre);
                    db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "equipment tyre registration", "Success", User.Identity.Name, $"created equipment tyre {Tyre.EquipmentTyreId}", Modules.FleetManagement);
                #endregion


                _FleetService = new FleetService();
                    Fleets fleets = _FleetService.GetAll().Where(x => x.FleetsID.ToString() == Id).FirstOrDefault();
                    fleets.LastModified = DateTime.Now;
                    _FleetService.Update(fleets);
                 }


            return Json(new[] { model }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentTyre_Update([DataSourceRequest] DataSourceRequest request, EquipmentTyreViewModel model, string Id)
        {
            var Battery = db.EquipmentTyres.Where(x => x.SerialNo == model.SerialNo && x.EquipmentTyreId != model.EquipmentTyreId).ToList();
            if (Battery.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("DuplicateSerialNo", " Serial Number Already Registered!");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            var Batterys = db.EquipmentTyres.Where(x => x.SerialNo == model.SerialNo).ToList();
            if (Batterys.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("DuplicateSerialNo", " Store Code Already Registered!");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            IFleetService _FleetService = new FleetService();
            var fleet = _FleetService.GetById(Guid.Parse(Id));
            if (model != null)
            {
                var Tyre = new EquipmentTyre();
                Tyre.EquipmentTyreId = Guid.NewGuid();
                Tyre.OdoMeter = fleet.CurentOdoMeter;
                Tyre.Condition = model.Condition;
                Tyre.Make = model.Make;
                Tyre.MakeCountry = model.MakeCountry;
                Tyre.Size = model.Size;
                Tyre.SerialNo = model.SerialNo;
                Tyre.Type = model.Tube;
                Tyre.UnitPrice = model.UnitPrice;
                Tyre.fleetId = Guid.Parse(Id);
                Tyre.Isattached = true;
                Tyre.DateCreated = DateTime.Now;
                Tyre.CreatedBy = User.Identity.Name; 
                db.EquipmentTyres.Attach(Tyre);
                db.Entry(Tyre).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Tyre registration", "Success", User.Identity.Name, $"updated tyre {Tyre.EquipmentTyreId}", Modules.FleetManagement);
                #endregion

            }

            return Json(new[] { model }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentTyre_Destroy([DataSourceRequest] DataSourceRequest request, EquipmentTyre EquipmentTyre)
        {
            var EquipmentTyreObj = db.EquipmentTyres.Where(x => x.EquipmentTyreId == EquipmentTyre.EquipmentTyreId).FirstOrDefault();
            if (EquipmentTyre != null)
            {

                db.EquipmentTyres.Attach(EquipmentTyreObj);
                db.EquipmentTyres.Remove(EquipmentTyreObj);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment tyre" + EquipmentTyre.EquipmentTyreId);

            }

            return Json(new[] { EquipmentTyre }.ToDataSourceResult(request));
        }


        
        public ActionResult TyreByFleet(string Id)
        {

            var Tyres = db.EquipmentTyres.Where(x => x.fleetId.ToString() == Id && x.Isattached == true).Select(e => new

            {
                Value = e.EquipmentTyreId,
                Text = e.SerialNo + "-" + e.Make,


            }).ToList();

            return Json(Tyres, JsonRequestBehavior.AllowGet);
        }




        
        public ActionResult TyreByFleetByPlate(string Id,string search)
        {
            var Equipment = db.Fleets.Where(x => x.PlateNumber == Id).FirstOrDefault();
            if (Equipment != null)
            {
                var Tyres = db.EquipmentTyres.Where(x => x.fleetId == Equipment.FleetsID && x.Isattached == true).Select(e => new

                {
                    Value = e.EquipmentTyreId,
                    Text = e.SerialNo + "-" + e.Make,


                }).ToList();
                if(!string.IsNullOrWhiteSpace(search))
                {
                    Tyres = Tyres.Where(x => x.Text.ToUpper().Contains(search.ToUpper())).ToList();
                }
                return Json(Tyres, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);
            
        }

        public ActionResult GetTyreInfo(Guid Id)
        {
            string Response = string.Empty;
            decimal OdomenterDiff = 0;
            var Tyre = db.EquipmentTyres.Where(x => x.fleetId == Id && x.Isattached == true).FirstOrDefault();
            //var issueDate = db.TireIssue.Where(x => (x.SerialNo == Serial || x.SerialNoB == Serial||x.SerialNoC==Serial||x.SerialNoD==Serial)&&x.PlateNumber==Id.ToString() ).FirstOrDefault();            
            if (Tyre != null)
            {
                var Fleet = db.Fleets.Where(x => x.FleetsID == Tyre.fleetId).FirstOrDefault();
                if (Tyre != null)
                {
                    OdomenterDiff = decimal.Parse(Fleet.CurentOdoMeter.ToString()) - Tyre.OdoMeter;

                  //  Response = Tyre.Make + "_" + Tyre.Size + "_" + Tyre.Type + "_" + OdomenterDiff + "_" + Tyre.UnitPrice + "_" + Tyre.StoreCode + "_" + DateTime.Parse(Tyre.DateCreated.ToString()).Date.ToString("dd/mm/yyyy") + "_" + Tyre.Tube;
                   Response = Tyre.Make + "_" + Tyre.Size + "_" + Tyre.Type + "_" + OdomenterDiff + "_" + Tyre.UnitPrice + "_" + Tyre.StoreCode + "_" + DateTime.Parse(Tyre.DateCreated.ToString()).Date.ToString("dd/mm/yyyy") + "_" + Tyre.Tube;            
                }
            }
                    return Json(Response, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetIssueDate(Guid Id,string Serial)
        {
            string Response = string.Empty;           
            var issueDate = db.TireIssue.Where(x => (x.SerialNo == Serial || x.SerialNoB == Serial||x.SerialNoC==Serial||x.SerialNoD==Serial)&&x.FleetId==Id ).FirstOrDefault();            
            if (issueDate!=null)
            {
                

                  //  Response = Tyre.Make + "_" + Tyre.Size + "_" + Tyre.Type + "_" + OdomenterDiff + "_" + Tyre.UnitPrice + "_" + Tyre.StoreCode + "_" + DateTime.Parse(Tyre.DateCreated.ToString()).Date.ToString("dd/mm/yyyy") + "_" + Tyre.Tube;
                   Response =  DateTime.Parse(issueDate.IssueDate.ToString()).Date.ToString("dd/mm/yyyy");
            
                
            }
                    return Json(Response, JsonRequestBehavior.AllowGet);
        }
   
    }
}
