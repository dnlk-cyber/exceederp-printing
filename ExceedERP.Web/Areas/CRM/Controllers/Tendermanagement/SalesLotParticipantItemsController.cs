using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.CRM.TenderManagement;
using System.Collections.Generic;

namespace ExceedERP.Web.Areas.CRM.Controllers.TenderManagement
{
	public class SalesLotParticipantItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            
            return View();
        }
        
        public ActionResult SalesLotParticipantItems_Read([DataSourceRequest]DataSourceRequest request,int saleLotParticipantId)
        {
            IQueryable<SalesLotParticipantItem> salesLots= db.SalesLotParticipantItems.Where(x=>x.SalesLotParticipantId==saleLotParticipantId);
            DataSourceResult result = salesLots.ToDataSourceResult(request, salesLot => new {
                SalesLotParticipantItemId = salesLot.SalesLotParticipantItemId,
                SalesLotParticipantId = salesLot.SalesLotParticipantId,
                ProductCategoryId = salesLot.ProductCategoryId,
                ProductID = salesLot.ProductID,                
                EvaluationDate=salesLot.EvaluationDate,
                Quantity = salesLot.Quantity,     
                UnitPrice = salesLot.UnitPrice, 
                TotalPrice = salesLot.TotalPrice,   
                BasePrice=salesLot.BasePrice,
                DateCreated = salesLot.DateCreated,
                CreatedBy = salesLot.CreatedBy,
                ModifiedBy = salesLot.ModifiedBy,
                LastModified = salesLot.LastModified

            });

            return Json(result);
        }

        public ActionResult SalesLotParticipantItems_Create([DataSourceRequest]DataSourceRequest request, SalesLotParticipantItem saleLot, int saleLotParticipantId)
        {               
                saleLot.SalesLotParticipantId = saleLotParticipantId;
                var registeredProductName = db.SalesLotParticipantItems.SingleOrDefault(r => r.SalesLotParticipantId == saleLotParticipantId && saleLot.ProductID == r.ProductID);
            if (registeredProductName != null)
            {
                ModelState.AddModelError("Registerd","Item already Registered");
            }
                var participant = db.SalesLotParticipants.SingleOrDefault(p => p.SalesLotParticipantId == saleLotParticipantId);
                    var purchase = db.SalesLotPurchases.SingleOrDefault(p => p.TinNumber == participant.TinNumber && p.SalesLotId == participant.SalesLotId);
                var record = db.SalesLots.Find(purchase.SalesLotId);

            if (record.IsOnlineApproved)
            {
                ModelState.AddModelError("Registered", "You can not add Participant after Lot is Approved");
            }
           
                var saleslot = db.SalesLots.SingleOrDefault(p => p.SalesLotId == purchase.SalesLotId);
                var tender = db.SaleTenders.SingleOrDefault(p => p.SaleTenderId == saleslot.SaleTenderId);
                saleLot.EvaluationDate = tender.EvaluationDate;
                saleLot.DateCreated = DateTime.Now;
                saleLot.CreatedBy = User.Identity.Name;
                var quantity = db.SalesLotItems.SingleOrDefault(p => p.ProductID == saleLot.ProductID && p.ProductCategoryId == saleLot.ProductCategoryId && p.SalesLotId == saleslot.SalesLotId);
                saleLot.Quantity = quantity.Quantity;
                saleLot.TotalPrice = saleLot.Quantity * saleLot.UnitPrice;
                var winnerCustomer = new Winner();
                winnerCustomer.SalesLotId = saleslot.SalesLotId;
                winnerCustomer.SalesLotName = saleslot.LotName;
                winnerCustomer.TinNumber = purchase.TinNumber;
                winnerCustomer.CustomerName = purchase.CustomerName;
                winnerCustomer.CustormerAddress = purchase.Address;
                winnerCustomer.SalesLotItemId = quantity.SalesLotItemId;
                winnerCustomer.ProducCategoryId = quantity.ProductCategoryId;
                winnerCustomer.ProductCategoryDesc = quantity.ProductCategoryDesc;
                winnerCustomer.ProductID = quantity.ProductID ?? 0;
                winnerCustomer.ProductName = quantity.ProductName;
                winnerCustomer.UnitPrice = saleLot.UnitPrice;
                winnerCustomer.WinningPrice = saleLot.TotalPrice;
                winnerCustomer.CreatedBy = User.Identity.Name; ;
                winnerCustomer.DateCreated = DateTime.Now;
                var checkWinner = db.Winners.SingleOrDefault(w => w.SalesLotId == winnerCustomer.SalesLotId && w.SalesLotItemId == winnerCustomer.SalesLotItemId);
                if (checkWinner == null)
                {
                    var entity = (Winner)GetMappedWinner(winnerCustomer);
                    db.Winners.Add(entity);
                    db.SaveChanges();
                }
                else
                {
                    if (winnerCustomer.WinningPrice > checkWinner.WinningPrice)
                    {
                        //var entity = (Winner)GetMappedWinner(winnerCustomer);
                        checkWinner.TinNumber = purchase.TinNumber;
                        checkWinner.CustormerAddress = purchase.Address;
                        checkWinner.CustomerName = purchase.CustomerName;
                        checkWinner.WinningPrice = saleLot.TotalPrice;
                        checkWinner.ModifiedBy = User.Identity.Name;
                        checkWinner.LastModified = DateTime.Now;
                        db.Winners.Attach(checkWinner);
                        db.Entry(checkWinner).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }


                if (ModelState.IsValid)
                {
                    var avp = db.StoreItemAssignments.SingleOrDefault(bp => bp.ItemCode == saleLot.ProductID.ToString() && bp.ItemCategoryCode == saleLot.ProductCategoryId.ToString());
                    if (avp != null)
                    {
                        saleLot.BasePrice = avp.AvgPrice;
                    }
                    else
                    {
                        saleLot.BasePrice = 0;
                    }
                    var entity = GetMappedObject(saleLot);
                    db.SalesLotParticipantItems.Add(entity);
                    db.SaveChanges();
                    var participantItems = db.SalesLotItems.Where(sli => sli.SalesLotId == saleslot.SalesLotId).ToList();
                
                    foreach (var item in participantItems)
                    {
                        if (item.ProductID != saleLot.ProductID && item.ProductCategoryId == item.ProductCategoryId)
                        {
                        var ParticipantDefaultRegisterItems = new SalesLotParticipantItem();
                        ParticipantDefaultRegisterItems.Quantity = item.Quantity;
                            ParticipantDefaultRegisterItems.ProductCategoryId = item.ProductCategoryId;
                            ParticipantDefaultRegisterItems.ProductID = item.ProductID ?? 0;
                            ParticipantDefaultRegisterItems.SalesLotParticipantItemId = ParticipantDefaultRegisterItems.SalesLotParticipantItemId;
                            ParticipantDefaultRegisterItems.SalesLotParticipantId = saleLot.SalesLotParticipantId;
                            ParticipantDefaultRegisterItems.CreatedBy = saleLot.CreatedBy;
                            ParticipantDefaultRegisterItems.DateCreated = saleLot.DateCreated;
                            ParticipantDefaultRegisterItems.EvaluationDate = saleLot.EvaluationDate;
                            ParticipantDefaultRegisterItems.TotalPrice = 0;
                            // ParticipantDefaultRegisterItems.BasePrice = db.StoreItemAssignments.SingleOrDefault(bp=>bp.ItemCode==item.ProductID.ToString()&&bp.ItemCategoryCode==item.ProductCategoryId.ToString()).AvgPrice;
                            var avpr = db.StoreItemAssignments.SingleOrDefault(bp => bp.ItemCode == saleLot.ProductID.ToString() && bp.ItemCategoryCode == saleLot.ProductCategoryId.ToString());
                            if (avpr != null)
                            {
                                ParticipantDefaultRegisterItems.BasePrice = avpr.AvgPrice;
                            }
                            else
                            {
                                ParticipantDefaultRegisterItems.BasePrice = 0;
                            }
                            ParticipantDefaultRegisterItems.UnitPrice = 0;
                            db.SalesLotParticipantItems.Add(ParticipantDefaultRegisterItems);
                        }
                    }
                    db.SaveChanges();
                    saleLot.SalesLotParticipantItemId = entity.SalesLotParticipantItemId;

                }
            
            
            return Json(new[] { saleLot }.ToDataSourceResult(request, ModelState));
        }
        public object GetMappedWinner(Winner winner)
        {
            var entity = new Winner {
                WinnerId = winner.WinnerId,
                WinningPrice = winner.WinningPrice,
                UnitPrice = winner.UnitPrice,
                TinNumber = winner.TinNumber,
                SalesLotId = winner.SalesLotId,
                SalesLotItemId = winner.SalesLotItemId,
                SalesLotName = winner.SalesLotName,
                ProducCategoryId = winner.ProducCategoryId,
                ProductCategoryDesc = winner.ProductCategoryDesc,
                CustomerName = winner.CustomerName,
                ProductID = winner.ProductID,
                CustormerAddress = winner.CustormerAddress,
                ProductName = winner.ProductName
            };
            return entity;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesLotParticipantItems_Update([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<SalesLotParticipantItem> saleLotItems)
        {
            if (ModelState.IsValid)
            {
                foreach (var saleLot in saleLotItems)
                {
                    var participant = db.SalesLotParticipants.SingleOrDefault(p => p.SalesLotParticipantId == saleLot.SalesLotParticipantId);
                    //var purchase = db.SalesLotPurchases.SingleOrDefault(p => p.TinNumber == participant.TinNumber && p.SalesLotId == participant.SalesLotId);
                    var record = db.SalesLots.Find(participant.SalesLotId);
                    if (record.IsOnlineApproved)
                    {
                        ModelState.AddModelError("Approved", "You can not Update Participant after Lot is Approved");
                    }
                    var saleslot = db.SalesLots.SingleOrDefault(p => p.SalesLotId == participant.SalesLotId);

                    var tender = db.SaleTenders.SingleOrDefault(p => p.SaleTenderId == saleslot.SaleTenderId);
                    saleLot.EvaluationDate = tender.EvaluationDate;
                    var avp = db.StoreItemAssignments.SingleOrDefault(bp => bp.ItemCode == saleLot.ProductID.ToString() && bp.ItemCategoryCode == saleLot.ProductCategoryId.ToString());
                    if (avp != null)
                    {
                        saleLot.BasePrice = avp.AvgPrice;
                    }
                    else
                    {
                        saleLot.BasePrice = 0;
                    }
                    var quantity = db.SalesLotItems.SingleOrDefault(p => p.ProductID == saleLot.ProductID && p.ProductCategoryId == saleLot.ProductCategoryId && p.SalesLotId == saleslot.SalesLotId);
                    saleLot.Quantity = quantity.Quantity;
                    saleLot.TotalPrice = saleLot.Quantity * saleLot.UnitPrice;

                    var checkWinner = db.Winners.SingleOrDefault(w => w.SalesLotId == saleslot.SalesLotId && w.SalesLotItemId == quantity.SalesLotItemId);

                    if (checkWinner == null)
                    {
                        var winnerCustomer = new Winner();
                        winnerCustomer.SalesLotId = saleslot.SalesLotId;
                        winnerCustomer.SalesLotName = saleslot.LotName;
                        winnerCustomer.TinNumber = participant.TinNumber;
                        winnerCustomer.CustomerName = participant.CustomerName;
                        winnerCustomer.CustormerAddress = participant.Address;
                        winnerCustomer.SalesLotItemId = quantity.SalesLotItemId;
                        winnerCustomer.ProducCategoryId = quantity.ProductCategoryId;
                        winnerCustomer.ProductCategoryDesc = quantity.ProductCategoryDesc;
                        winnerCustomer.ProductID = quantity.ProductID ?? 0;
                        winnerCustomer.ProductName = quantity.ProductName;
                        winnerCustomer.WinningPrice = saleLot.TotalPrice;
                        winnerCustomer.UnitPrice = saleLot.UnitPrice;
                        winnerCustomer.CreatedBy = User.Identity.Name; ;
                        winnerCustomer.DateCreated = DateTime.Now;

                        var entity = (Winner)GetMappedWinner(winnerCustomer);
                        db.Winners.Add(entity);
                        db.SaveChanges();

                    }
                    else
                    {
                        if (saleLot.TotalPrice > checkWinner.WinningPrice)
                        {
                            //var entity = (Winner)GetMappedWinner(winnerCustomer);
                            checkWinner.TinNumber = participant.TinNumber;
                            checkWinner.CustormerAddress = participant.Address;
                            checkWinner.CustomerName = participant.CustomerName;
                            checkWinner.WinningPrice = saleLot.TotalPrice;
                            checkWinner.ModifiedBy = User.Identity.Name;
                            checkWinner.LastModified = DateTime.Now;
                            db.Winners.Attach(checkWinner);
                            db.Entry(checkWinner).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                    if (ModelState.IsValid)
                    {
                        saleLot.LastModified = DateTime.Now;
                        saleLot.ModifiedBy = User.Identity.Name;
                        var updateSlote = db.SalesLotParticipantItems.Find(saleLot.SalesLotParticipantItemId);
                        if (updateSlote != null)
                        {
                            updateSlote.ProductCategoryId = saleLot.ProductCategoryId;
                            updateSlote.ProductID = saleLot.ProductID;
                            updateSlote.Quantity = saleLot.Quantity;
                            updateSlote.BasePrice = saleLot.BasePrice;
                            updateSlote.UnitPrice = saleLot.UnitPrice;
                            updateSlote.TotalPrice = saleLot.TotalPrice;
                            updateSlote.EvaluationDate = saleLot.EvaluationDate;
                            db.Entry(updateSlote).State = EntityState.Modified;
                            db.SaveChanges();

                        }
                        if (checkWinner != null)
                        {
                            if (saleLot.TotalPrice < checkWinner.WinningPrice && checkWinner.TinNumber == participant.TinNumber && checkWinner.SalesLotId == participant.SalesLotId)
                            {
                                decimal checkWinningPrice = saleLot.TotalPrice;
                                decimal UnitPricee = saleLot.UnitPrice;
                                var changeWinner = db.SalesLotParticipantItems.Where(pro => pro.ProductID == checkWinner.ProductID).ToList();
                                foreach (var changeWin in changeWinner)
                                {
                                    var checkItemLot = db.SalesLotParticipants.SingleOrDefault(CIL => CIL.SalesLotParticipantId == changeWin.SalesLotParticipantId);
                                    if (checkItemLot.SalesLotId == saleslot.SalesLotId)
                                    {
                                        if (changeWin.TotalPrice > checkWinningPrice)
                                        {
                                            checkWinningPrice = changeWin.TotalPrice;

                                            var WinnerParticipant = db.SalesLotParticipants.SingleOrDefault(p => p.SalesLotParticipantId == changeWin.SalesLotParticipantId);
                                            checkWinner.TinNumber = WinnerParticipant.TinNumber;
                                            checkWinner.CustormerAddress = WinnerParticipant.Address;
                                            checkWinner.CustomerName = WinnerParticipant.CustomerName;
                                            checkWinner.WinningPrice = checkWinningPrice;
                                            checkWinner.UnitPrice = UnitPricee;
                                            checkWinner.UnitPrice = checkWinningPrice;
                                            checkWinner.ModifiedBy = User.Identity.Name;
                                            checkWinner.LastModified = DateTime.Now;

                                        }
                                    }

                                }
                                db.Winners.Attach(checkWinner);
                                db.Entry(checkWinner).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                    }
                }

                db.SaveChanges();

            }



            return Json(new[] { saleLotItems }.ToDataSourceResult(request, ModelState));
        }
        //public ActionResult SalesLotParticipantItems_Update([DataSourceRequest]DataSourceRequest request, SalesLotParticipantItem saleLot)
        //{

        //    var participant = db.SalesLotParticipants.SingleOrDefault(p => p.SalesLotParticipantId == saleLot.SalesLotParticipantId);
        //    //var purchase = db.SalesLotPurchases.SingleOrDefault(p => p.TinNumber == participant.TinNumber && p.SalesLotId == participant.SalesLotId);
        //    var record = db.SalesLots.Find(participant.SalesLotId);


        //    if (record.IsOnlineApproved)
        //    {
        //        ModelState.AddModelError("Approved", "You can not Update Participant after Lot is Approved");
        //    }
        //    var saleslot = db.SalesLots.SingleOrDefault(p => p.SalesLotId == participant.SalesLotId);

        //    var tender = db.SaleTenders.SingleOrDefault(p => p.SaleTenderId == saleslot.SaleTenderId);
        //    saleLot.EvaluationDate = tender.EvaluationDate;
        //    var avp=db.StoreItemAssignments.SingleOrDefault(bp => bp.ItemCode == saleLot.ProductID.ToString() && bp.ItemCategoryCode == saleLot.ProductCategoryId.ToString());
        //    if(avp!=null)
        //    {
        //        saleLot.BasePrice = avp.AvgPrice;
        //    }
        //    else
        //    {
        //        saleLot.BasePrice = 0;
        //    }
        //   var quantity = db.SalesLotItems.SingleOrDefault(p => p.ProductID == saleLot.ProductID && p.ProductCategoryId == saleLot.ProductCategoryId && p.SalesLotId == saleslot.SalesLotId);
        //    saleLot.Quantity = quantity.Quantity;
        //    saleLot.TotalPrice = saleLot.Quantity * saleLot.UnitPrice;

        //    var checkWinner = db.Winners.SingleOrDefault(w => w.SalesLotId == saleslot.SalesLotId && w.SalesLotItemId == quantity.SalesLotItemId);
           
        //    if(checkWinner==null)
        //    {
        //        var winnerCustomer = new Winner();
        //        winnerCustomer.SalesLotId = saleslot.SalesLotId;
        //        winnerCustomer.SalesLotName = saleslot.LotName;
        //        winnerCustomer.TinNumber = participant.TinNumber;
        //        winnerCustomer.CustomerName = participant.CustomerName;
        //        winnerCustomer.CustormerAddress = participant.Address;
        //        winnerCustomer.SalesLotItemId = quantity.SalesLotItemId;
        //        winnerCustomer.ProducCategoryId = quantity.ProductCategoryId;
        //        winnerCustomer.ProductCategoryDesc = quantity.ProductCategoryDesc;
        //        winnerCustomer.ProductID = quantity.ProductID ?? 0;
        //        winnerCustomer.ProductName = quantity.ProductName;
        //        winnerCustomer.WinningPrice = saleLot.TotalPrice;
        //        winnerCustomer.UnitPrice = saleLot.UnitPrice;
        //        winnerCustomer.CreatedBy = User.Identity.Name; ;
        //        winnerCustomer.DateCreated = DateTime.Now;                
               
        //            var entity = (Winner)GetMappedWinner(winnerCustomer);
        //            db.Winners.Add(entity);
        //            db.SaveChanges();
                
        //    }
        //   else
        //    {
        //        if (saleLot.TotalPrice > checkWinner.WinningPrice)
        //        {
        //            //var entity = (Winner)GetMappedWinner(winnerCustomer);
        //            checkWinner.TinNumber = participant.TinNumber;
        //            checkWinner.CustormerAddress = participant.Address;
        //            checkWinner.CustomerName = participant.CustomerName;
        //            checkWinner.WinningPrice = saleLot.TotalPrice;
        //            checkWinner.ModifiedBy = User.Identity.Name;
        //            checkWinner.LastModified = DateTime.Now;
        //            db.Winners.Attach(checkWinner);
        //            db.Entry(checkWinner).State = EntityState.Modified;
        //            db.SaveChanges();
        //        }

        //    }
        //    if (ModelState.IsValid)
        //        {
        //            saleLot.LastModified = DateTime.Now;
        //            saleLot.ModifiedBy = User.Identity.Name;
        //            var updateSlote = db.SalesLotParticipantItems.Find(saleLot.SalesLotParticipantItemId);
        //            if (updateSlote != null)
        //            {
        //                updateSlote.ProductCategoryId = saleLot.ProductCategoryId;
        //                updateSlote.ProductID = saleLot.ProductID;
        //                updateSlote.Quantity = saleLot.Quantity;
        //                updateSlote.BasePrice = saleLot.BasePrice;
        //                updateSlote.UnitPrice = saleLot.UnitPrice;
        //                updateSlote.TotalPrice = saleLot.TotalPrice;
        //                updateSlote.EvaluationDate = saleLot.EvaluationDate;
        //                db.Entry(updateSlote).State = EntityState.Modified;
        //                db.SaveChanges();

        //            }
        //        if (checkWinner != null)
        //        {
        //            if (saleLot.TotalPrice < checkWinner.WinningPrice && checkWinner.TinNumber == participant.TinNumber && checkWinner.SalesLotId == participant.SalesLotId)
        //            {
        //                decimal checkWinningPrice = saleLot.TotalPrice;
        //                decimal UnitPricee = saleLot.UnitPrice;
        //                var changeWinner = db.SalesLotParticipantItems.Where(pro => pro.ProductID == checkWinner.ProductID).ToList();
        //                foreach (var changeWin in changeWinner)
        //                {
        //                    var checkItemLot = db.SalesLotParticipants.SingleOrDefault(CIL => CIL.SalesLotParticipantId == changeWin.SalesLotParticipantId);
        //                    if (checkItemLot.SalesLotId == saleslot.SalesLotId)
        //                    {
        //                        if (changeWin.TotalPrice > checkWinningPrice)
        //                        {
        //                            checkWinningPrice = changeWin.TotalPrice;

        //                            var WinnerParticipant = db.SalesLotParticipants.SingleOrDefault(p => p.SalesLotParticipantId == changeWin.SalesLotParticipantId);
        //                            checkWinner.TinNumber = WinnerParticipant.TinNumber;
        //                            checkWinner.CustormerAddress = WinnerParticipant.Address;
        //                            checkWinner.CustomerName = WinnerParticipant.CustomerName;
        //                            checkWinner.WinningPrice = checkWinningPrice;
        //                            checkWinner.UnitPrice = UnitPricee;
        //                            checkWinner.UnitPrice = checkWinningPrice;
        //                            checkWinner.ModifiedBy = User.Identity.Name;
        //                            checkWinner.LastModified = DateTime.Now;

        //                        }
        //                    }

        //                }
        //                db.Winners.Attach(checkWinner);
        //                db.Entry(checkWinner).State = EntityState.Modified;
        //                db.SaveChanges();
        //            }
        //        }
        //        }
            

        //    return Json(new[] { saleLot}.ToDataSourceResult(request, ModelState));
        //}
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SalesLotParticipantItems_Destroy([DataSourceRequest]DataSourceRequest request, SalesLotParticipantItem saleLot)
        {
            var participant = db.SalesLotParticipants.SingleOrDefault(p => p.SalesLotParticipantId == saleLot.SalesLotParticipantId);
            //var purchase = db.SalesLotPurchases.SingleOrDefault(p => p.TinNumber == participant.TinNumber && p.SalesLotId == participant.SalesLotId);
            var record = db.SalesLots.Find(participant.SalesLotId);
           // var checkRecord=db.SalesLotParticipantItems.Where(par=>par.SalesLotParticipantItemId==saleLot.SalesLotParticipantItemId)
           
                if (record.IsOnlineApproved)
                {
                    ModelState.AddModelError("Registered", "You can not delete Participant after Lot is Approved");
                }
                var saleslot = db.SalesLots.SingleOrDefault(p => p.SalesLotId == participant.SalesLotId);
                var tender = db.SaleTenders.SingleOrDefault(p => p.SaleTenderId == saleslot.SaleTenderId);
                saleLot.EvaluationDate = tender.EvaluationDate;

                var quantity = db.SalesLotItems.SingleOrDefault(p => p.ProductID == saleLot.ProductID &&p.SalesLotId==saleslot.SalesLotId&& p.ProductCategoryId == saleLot.ProductCategoryId);
                saleLot.Quantity = quantity.Quantity;
                saleLot.TotalPrice = saleLot.Quantity * saleLot.UnitPrice;

                var checkWinner = db.Winners.SingleOrDefault(w => w.SalesLotId == saleslot.SalesLotId && w.SalesLotItemId == quantity.SalesLotItemId);

                if (ModelState.IsValid)
                {
                    if (checkWinner.TinNumber == participant.TinNumber && checkWinner.SalesLotId == participant.SalesLotId)
                    {
                        decimal checkWinningPrice = saleLot.TotalPrice;
                    decimal UnitPricee = saleLot.UnitPrice;
                        var changeWinner = db.SalesLotParticipantItems.Where(pro => pro.ProductID == checkWinner.ProductID).ToList();
                        foreach (var changeWin in changeWinner)
                        {

                            if (changeWin.TotalPrice > checkWinningPrice)
                            {
                                checkWinningPrice = changeWin.TotalPrice;
                                 
                                var WinnerParticipant = db.SalesLotParticipants.SingleOrDefault(p => p.SalesLotParticipantId == changeWin.SalesLotParticipantId);
                                checkWinner.TinNumber = WinnerParticipant.TinNumber;
                                checkWinner.CustormerAddress = WinnerParticipant.Address;
                                checkWinner.CustomerName = WinnerParticipant.CustomerName;
                                checkWinner.WinningPrice = checkWinningPrice;
                                checkWinner.UnitPrice = UnitPricee;
                                checkWinner.ModifiedBy = User.Identity.Name;
                                checkWinner.LastModified = DateTime.Now;

                            }

                        }
                        db.Winners.Attach(checkWinner);
                        db.Entry(checkWinner).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                var CurrentParticipants = db.SalesLotParticipants.SingleOrDefault(allP => allP.SalesLotParticipantId == saleLot.SalesLotParticipantId);
                if (CurrentParticipants != null)
                {
                    var allParticipants = db.SalesLotParticipants.Where(Ap => Ap.SalesLotId == CurrentParticipants.SalesLotId).ToList();
                    int count = 0;
                    foreach (var allp in allParticipants)
                    {
                        var checkItemParticipantAvailablility = db.SalesLotParticipantItems.SingleOrDefault(cip => cip.SalesLotParticipantId == allp.SalesLotParticipantId && cip.ProductCategoryId == saleLot.ProductCategoryId && cip.ProductID == saleLot.ProductID);
                        if (checkItemParticipantAvailablility != null)
                        {
                            count += 1;
                        }
                    }
                    if (count <= 1)
                    {
                        var tobeDeletedWinner = db.Winners.SingleOrDefault(tdw => tdw.SalesLotId == CurrentParticipants.SalesLotId && tdw.ProducCategoryId == saleLot.ProductCategoryId && tdw.ProductID == saleLot.ProductID);
                        db.Winners.Remove(tobeDeletedWinner);
                        db.SaveChanges();
                    }
                }  var entity = GetMappedObject(saleLot);
                var tobeDeletedObject = db.SalesLotParticipantItems.SingleOrDefault(itd => itd.SalesLotParticipantItemId == saleLot.SalesLotParticipantItemId);
                    db.SalesLotParticipantItems.Remove(tobeDeletedObject);
                    db.SaveChanges();
                
                }
            
            return Json(new[] { saleLot }.ToDataSourceResult(request, ModelState));
        }

        private SalesLotParticipantItem GetMappedObject(SalesLotParticipantItem salesLot)
        {
            var entity = new SalesLotParticipantItem
            {
                SalesLotParticipantItemId = salesLot.SalesLotParticipantItemId,
                SalesLotParticipantId = salesLot.SalesLotParticipantId,
                ProductCategoryId = salesLot.ProductCategoryId,
                ProductID = salesLot.ProductID,
                Quantity = salesLot.Quantity,
                EvaluationDate=salesLot.EvaluationDate,
                UnitPrice = salesLot.UnitPrice,//SubTotal = salesLot.SubTotal,
                TotalPrice = salesLot.TotalPrice,                
                DateCreated = salesLot.DateCreated,
                CreatedBy = salesLot.CreatedBy,
                ModifiedBy = salesLot.ModifiedBy,
                LastModified = salesLot.LastModified

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
