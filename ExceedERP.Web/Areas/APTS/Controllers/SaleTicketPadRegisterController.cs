//using ExceedERP.Core.Domain.APTS;
//using ExceedERP.DataAccess.Context;
//using Kendo.Mvc.Extensions;
//using Kendo.Mvc.UI;
//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace ExceedERP.Web.Areas.APTS.Controllers
//{
//    public class SaleTicketPadRegisterController : Controller
//    {
//        // GET: APTS/SaleTicketPadRegister
//        ExceedDbContext db = new ExceedDbContext();
//        public ActionResult Index()
//        {
//            PopulateViewData();
//            return View();
//        }
//        public void PopulateViewData()
//        {
//            var drug = db.DrugLists.Where(x => x.IsActive);
//            ViewData["druglists"] = drug;
//        }
//        public ActionResult GetDrugLists()
//        {
//            var drugLists = db.DrugLists.Where(x => x.IsActive && x.IsOur);
//            return Json(drugLists.Select(p => new
//            {
//                DrugListsId = p.DrugListsId,
//                Description = p.Description
//            }), JsonRequestBehavior.AllowGet);
//        }
//        public ActionResult SaleTicketPadRegister_Read([DataSourceRequest]DataSourceRequest request)
//        {
//            var requests = db.SaleTicketPadRegisters.ToList();
//            DataSourceResult result = requests.ToDataSourceResult(request, medice => new
//            {
//                NameOfReceiver = medice.NameOfReceiver,
//                SerialNoIssuedFrom = medice.SerialNoIssuedFrom,
//                SerialNoIssuedTo = medice.SerialNoIssuedTo,
//                SignatureOfReceiver = medice.SignatureOfReceiver,
//                SerialNoOfUsedPadReturnedFrom = medice.SerialNoOfUsedPadReturnedFrom,
//                SerialNoOfUsedPadReturnedTo = medice.SerialNoOfUsedPadReturnedTo,
//                SignOfAccountant = medice.SignOfAccountant,
//                SetialNoOfLost = medice.SetialNoOfLost,
//                //DateCreated = medice.DateCreated

//            });

//            return Json(result);
//        }
//        public ActionResult SaleTicketPadRegister_Create([DataSourceRequest]DataSourceRequest request, SaleTicketPadRegister saleTicket)
//        {
//            if (ModelState.IsValid)
//            {
                
//                var entity = new SaleTicketPadRegister
//                {
//                    NameOfReceiver = saleTicket.NameOfReceiver,
//                    SerialNoIssuedFrom = saleTicket.SerialNoIssuedFrom,
//                    SerialNoIssuedTo = saleTicket.SerialNoIssuedTo,
//                    SignatureOfReceiver = saleTicket.SignatureOfReceiver,
//                    SerialNoOfUsedPadReturnedFrom = saleTicket.SerialNoOfUsedPadReturnedFrom,
//                    SerialNoOfUsedPadReturnedTo = saleTicket.SerialNoOfUsedPadReturnedTo,
//                    SignOfAccountant = saleTicket.SignOfAccountant,
//                    SetialNoOfLost = saleTicket.SetialNoOfLost,
//                    //DateCreated = DateTime.Now
//                };
//                db.SaleTicketPadRegisters.Add(entity);
//                db.SaveChanges();
//                saleTicket.SaleTicketPadRegisterId = entity.SaleTicketPadRegisterId;
//            }

//            return Json(new[] { saleTicket }.ToDataSourceResult(request, ModelState));
//        }
//        public ActionResult SaleTicketPadRegister_Update([DataSourceRequest]DataSourceRequest request, SaleTicketPadRegister saleTicket)
//        {
//            if (ModelState.IsValid)
//            {
//                var entity = new SaleTicketPadRegister
//                {
//                    SaleTicketPadRegisterId = saleTicket.SaleTicketPadRegisterId,
//                    NameOfReceiver = saleTicket.NameOfReceiver,
//                    SerialNoIssuedFrom = saleTicket.SerialNoIssuedFrom,
//                    SerialNoIssuedTo = saleTicket.SerialNoIssuedTo,
//                    SignatureOfReceiver = saleTicket.SignatureOfReceiver,
//                    SerialNoOfUsedPadReturnedFrom = saleTicket.SerialNoOfUsedPadReturnedFrom,
//                    SerialNoOfUsedPadReturnedTo = saleTicket.SerialNoOfUsedPadReturnedTo,
//                    SignOfAccountant = saleTicket.SignOfAccountant,
//                    SetialNoOfLost = saleTicket.SetialNoOfLost,
//                    //DateCreated = saleTicket.DateCreated
//                };

//                db.SaleTicketPadRegisters.Attach(entity);
//                db.Entry(entity).State = EntityState.Modified;
//                db.SaveChanges();
//            }

//            return Json(new[] { saleTicket }.ToDataSourceResult(request, ModelState));
//        }
//        public ActionResult SaleTicketPadRegister_Destroy([DataSourceRequest]DataSourceRequest request, SaleTicketPadRegister saleTicket)
//        {
//            if (ModelState.IsValid)
//            {

//                var entity = new SaleTicketPadRegister
//                {
//                    SaleTicketPadRegisterId = saleTicket.SaleTicketPadRegisterId,
//                    NameOfReceiver = saleTicket.NameOfReceiver,
//                    SerialNoIssuedFrom = saleTicket.SerialNoIssuedFrom,
//                    SerialNoIssuedTo = saleTicket.SerialNoIssuedTo,
//                    SignatureOfReceiver = saleTicket.SignatureOfReceiver,
//                    SerialNoOfUsedPadReturnedFrom = saleTicket.SerialNoOfUsedPadReturnedFrom,
//                    SerialNoOfUsedPadReturnedTo = saleTicket.SerialNoOfUsedPadReturnedTo,
//                    SignOfAccountant = saleTicket.SignOfAccountant,
//                    SetialNoOfLost = saleTicket.SetialNoOfLost,
//                    //DateCreated = saleTicket.DateCreated
//                };
//                db.SaleTicketPadRegisters.Attach(entity);
//                db.SaleTicketPadRegisters.Remove(entity);
//                db.SaveChanges();
//            }

//            return Json(new[] { saleTicket }.ToDataSourceResult(request, ModelState));
//        }
//        public string Approve(int id)
//        {
//            var status = db.RequisionModel20s.FirstOrDefault(x => x.RequisitionModel20Id == id);
//            if (status != null)
//            {
//                status.IsApproved = true;

//                db.RequisionModel20s.Attach(status);
//                db.Entry(status).State = EntityState.Modified;
//                db.SaveChanges();
//            }
//            return status?.ToString();
//        }
//        protected override void Dispose(bool disposing)
//        {
//            db.Dispose();
//            base.Dispose(disposing);
//        }
//    }
//}