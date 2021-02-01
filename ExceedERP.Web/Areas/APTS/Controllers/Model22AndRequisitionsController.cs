using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.APTS;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.APTS.Controllers
{
    public class Model22AndRequisitionsController : Controller
    {
        // GET: APTS/Model20Requisitions
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            SaveApproved();
            return View();
        }
        public void PopulateViewData()
        {

            var drug = db.RequisionModel20s.ToList();
            ViewData["druglists"] = drug;


        }
        public ActionResult GetDrugLists()
        {
            var drugLists = db.DrugLists.Where(x => x.IsActive && x.IsOur);
            return Json(drugLists.Select(p => new
            {
                DrugListsId = p.DrugListsId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);
        }

        public void SaveApproved()
        {
            var approveRequests = db.RequisionModel20s.Where(x => x.IsApproved && x.IsNew && !x.IsCompleted).ToList();
            if (approveRequests.Any())
            {
                foreach (var approveRequest in approveRequests)
                {
                    var checkExistance = db.Model22s.FirstOrDefault(x => x.RequisitionModel20Id == approveRequest.RequisitionModel20Id && !x.IsOut);
                    if (checkExistance == null)
                    {
                        var model22 = new Model22();
                        model22.RequisitionModel20Id = approveRequest.RequisitionModel20Id;


                        db.Model22s.Add(model22);
                        db.SaveChanges();

                        var request = db.RequisionModel20s.FirstOrDefault(x => x.RequisitionModel20Id == approveRequest.RequisitionModel20Id);
                        if (request != null)
                        {
                            request.IsNew = false;

                            db.Entry(request).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }

                }
            }
        }
        public ActionResult Model22AndRequisitions_Read([DataSourceRequest]DataSourceRequest request)
        {
            var requests = db.Model22s.ToList();
            DataSourceResult result = requests.ToDataSourceResult(request, medice => new
            {
                Model22Id = medice.Model22Id,
                RequisitionModel20Id = medice.RequisitionModel20Id,
                DispensaryEntryDates = medice.DispensaryEntryDate,
                Unit = medice.Unit,
                QuantityIssued = medice.QuantityIssued,
                UnitCost = medice.UnitCost,
                TotalCost = medice.TotalCost,
                UnitPrice = medice.UnitPrice,
                RetailPrice = medice.RetailPrice,
                DateCreated = medice.DateCreated,
                CreatedBy = medice.CreatedBy,
                IsOut = medice.IsOut,
                IsCompleted = medice.IsCompleted

            });

            return Json(result);
        }
        public ActionResult Model22AndRequisitions_Create([DataSourceRequest]DataSourceRequest request, RequisitionModel20 drugRequest)
        {
            if (ModelState.IsValid)
            {
                //var splitDate = drugRequest.RequestDateAm.Split('/');
                //var RequestDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));

                var entity = new RequisitionModel20
                {
                    DrugListsId = drugRequest.DrugListsId,
                    BegStock = drugRequest.BegStock,
                    ReceivedQty = drugRequest.ReceivedQty,
                    Loss = drugRequest.Loss,
                    EndStock = drugRequest.EndStock,
                    CalculatedConsumption = drugRequest.BegStock + drugRequest.ReceivedQty + drugRequest.Loss - drugRequest.EndStock,
                    Max = (drugRequest.BegStock + drugRequest.ReceivedQty + drugRequest.Loss - drugRequest.EndStock) * 2,
                    QtyToMax = ((drugRequest.BegStock + drugRequest.ReceivedQty + drugRequest.Loss - drugRequest.EndStock) * 2) - 2,
                    QtyToBeApproved = drugRequest.QtyToBeApproved,
                    RequestDate = drugRequest.RequestDate,//DateTime.Now,// RequestDate,
                    RequestDateAm = drugRequest.RequestDateAm,
                    RequestedBy = drugRequest.RequestedBy,
                    ApproverName = drugRequest.ApproverName,
                    ApprovedDate = drugRequest.ApprovedDate,
                    DateCreated = DateTime.Now,
                    IsApproved = drugRequest.IsApproved,
                    IsOut = drugRequest.IsOut,
                    IsCompleted = drugRequest.IsCompleted,
                    CreatedBy = drugRequest.CreatedBy,
                    Remark = drugRequest.Remark
                };
                db.RequisionModel20s.Add(entity);
                db.SaveChanges();
                drugRequest.RequisitionModel20Id = entity.RequisitionModel20Id;
            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Model22AndRequisitions_Update([DataSourceRequest]DataSourceRequest request, Model22 model22)
        {
            //var total = model22.UnitCost * model22.QuantityIssued;
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new Model22
                {
                    Model22Id = model22.Model22Id,
                    RequisitionModel20Id = model22.RequisitionModel20Id,
                    Unit = model22.Unit,
                    QuantityIssued = model22.QuantityIssued,
                    UnitCost = model22.UnitCost,
                    TotalCost = model22.UnitCost * model22.QuantityIssued,
                    UnitPrice = model22.UnitPrice,
                    RetailPrice = model22.RetailPrice,
                    DateCreated = model22.DateCreated,
                    CreatedBy = model22.CreatedBy,
                };

                db.Model22s.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { model22 }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Model22AndRequisitions_Destroy([DataSourceRequest]DataSourceRequest request, RequisitionModel20 drugRequest)
        {
            if (ModelState.IsValid)
            {

                var entity = new RequisitionModel20
                {
                    RequisitionModel20Id = drugRequest.RequisitionModel20Id,
                    DrugListsId = drugRequest.DrugListsId,
                    BegStock = drugRequest.BegStock,
                    ReceivedQty = drugRequest.ReceivedQty,
                    Loss = drugRequest.Loss,
                    EndStock = drugRequest.EndStock,
                    CalculatedConsumption = drugRequest.CalculatedConsumption,
                    Max = drugRequest.Max,
                    QtyToMax = drugRequest.QtyToMax,
                    QtyToBeApproved = drugRequest.QtyToBeApproved,
                    RequestDate = drugRequest.RequestDate,
                    RequestDateAm = drugRequest.RequestDateAm,
                    RequestedBy = drugRequest.RequestedBy,
                    ApproverName = drugRequest.ApproverName,
                    IsApproved = drugRequest.IsApproved,
                    ApprovedDate = drugRequest.ApprovedDate,
                    DateCreated = drugRequest.DateCreated,
                    IsOut = drugRequest.IsOut,
                    IsCompleted = drugRequest.IsCompleted,
                    CreatedBy = drugRequest.CreatedBy,
                    Remark = drugRequest.Remark
                };
                db.RequisionModel20s.Attach(entity);
                db.RequisionModel20s.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { drugRequest }.ToDataSourceResult(request, ModelState));
        }
        public string Out(int id)
        {
            var status = db.Model22s.FirstOrDefault(x => x.Model22Id == id);
            //var stat = db.RequisionModel20s.FirstOrDefault(x=>x.id);
            if (status != null)
            {
                status.IsOut = true;
                status.IsNew = false;

                db.Model22s.Attach(status);
                db.Entry(status).State = EntityState.Modified;
                db.SaveChanges();
            }
            return status?.ToString();
        }
        public string Completed(int id)
        {
            var status = db.Model22s.FirstOrDefault(x => x.Model22Id == id);
            if (status != null)
            {
                if (status.IsOut == false)
                {
                    ModelState.AddModelError("", "This Request is Not Out From Store ");
                }
                else
                {
                    status.IsCompleted = true;
                    status.IsNew = false;
                    status.DispensaryEntryDate = DateTime.Now.Date;

                    db.Model22s.Attach(status);
                    db.Entry(status).State = EntityState.Modified;
                    db.SaveChanges();

                    var completed = db.Model22s.FirstOrDefault(x=>x.Model22Id == id && x.IsCompleted);
                    if (completed != null)
                    {
                        var drugList = db.RequisionModel20s.FirstOrDefault(x => x.RequisitionModel20Id == completed.RequisitionModel20Id);
                        if (drugList != null)
                        {
                            var checkDespensary = db.DespensaryStores.FirstOrDefault(x => x.DrugListsId == drugList.DrugListsId);
                            if (checkDespensary != null)
                            {
                                checkDespensary.BegStock = checkDespensary.BegStock + checkDespensary.NewReceived;
                                checkDespensary.NewReceived = completed.QuantityIssued;
                                checkDespensary.Balance = checkDespensary.BegStock + checkDespensary.NewReceived + completed.QuantityIssued;

                                db.DespensaryStores.Attach(checkDespensary);
                                db.Entry(checkDespensary).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                       
                            //var checkStore = db.DrugStores.FirstOrDefault(x => x.DrugListsId == drugList.DrugListsId);
                            //if (checkStore != null)
                            //{
                            //    checkStore.BegStock = checkStore.BegStock + checkStore.NewReceived;
                            //    checkStore.NewReceived = completed.QuantityIssued;
                            //    checkStore.Balance = checkStore.BegStock + checkStore.NewReceived - completed.QuantityIssued;

                            //    db.DrugStores.Attach(checkStore);
                            //    db.Entry(checkStore).State = EntityState.Modified;
                            //    db.SaveChanges();
                            //}
                        
                            //var requestedDrug = db.RequisionModel20s.FirstOrDefault
                            //    (x => x.DrugListsId == drugList.DrugListsId && x.RequisitionModel20Id == drugList.RequisitionModel20Id);
                            //if (requestedDrug != null)
                            //{
                                var despensaryStore = db.DespensaryStores.FirstOrDefault(x => x.DrugListsId == drugList.DrugListsId);
                                var model = db.Model22s.FirstOrDefault(x => x.Model22Id == id && x.RequisitionModel20Id == drugList.RequisitionModel20Id);
                                if (despensaryStore != null)
                                {
                                    if (model != null)
                                    {
                                        var current = despensaryStore.BegStock;
                                        despensaryStore.BegStock = current;// + despensaryStore.NewReceived;
                                        despensaryStore.NewReceived = model.QuantityIssued;
                                        despensaryStore.Balance = despensaryStore.BegStock + model.QuantityIssued;

                                        db.Entry(despensaryStore).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    var entity = new DespensaryStore
                                    {
                                        DrugListsId = drugList.DrugListsId,
                                        BegStock = 0,
                                        NewReceived = model.QuantityIssued,
                                        Balance = model.QuantityIssued
                                    };
                                    db.DespensaryStores.Add(entity);
                                    db.SaveChanges();
                                    despensaryStore.DespensaryStoreId = entity.DespensaryStoreId;
                                }
                            //}
                        }
                    }

                }

            }
            return status?.ToString();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}