using System;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.DataAccess.Context;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common.ViewModel;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Microsoft.Ajax.Utilities;
using System.Data.Entity;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;

namespace ExceedERP.Web.Areas.Procurement.Controllers
{
    public class ProcurementDataProviderController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();


        public JsonResult CheckGuaranteeStatus(string id)
        {
            int typed = int.Parse(id);
            var guarantee = db.Guarantees.Find(typed);
            DateTime today = DateTime.Now;
            decimal days = 100000000;
            int alarmday = 0;

           
            if (guarantee != null)
            {
                #region get rule

                var rule = db.GlobalRuleValues.Where(x => x.Module == Modules.Procurement).FirstOrDefault(x => x.Key == KeyType.GuaranteeExpiryAlarmDays);
                if (rule != null)
                    try
                    {
                        alarmday = int.Parse(rule.Value);
                    }
                    catch (Exception e)
                    {
                        Logger.Debug(e.Message);
                    }

                #endregion
                var renewal = guarantee.GuaranteeRenewals;
                if (renewal.Count > 0)
                {
                    int maxid = renewal.Max(x => x.GuaranteeRenewalId);
                    var lastRecord = renewal.SingleOrDefault(x => x.GuaranteeRenewalId == maxid);
                    if (lastRecord != null)
                    {
                        var newExpireDate = guarantee.ExpireDate.AddDays(lastRecord.ValidityPeriod);
                        days = ((newExpireDate - today)).Days;
                    }
                    if (days <= alarmday)
                    {
                        var formattedData = new
                        {
                            Expired = "Yes"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    days = (guarantee.ExpireDate - today).Days;
                    if (days <= alarmday)
                    {
                        var formattedData = new
                        {
                            Expired = "Yes"
                        };
                        return Json(formattedData, JsonRequestBehavior.AllowGet);
                    }
                }

            }

            var formattedData1 = new
            {
                Type = "Manual"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPOforOrderChange(string text)
        {
            var purchaseOrders = db.PurchaseOrders.Where(x => x.IsOnlineApproved && x.IsVoid==false).AsQueryable();

            if (!string.IsNullOrEmpty(text))
                purchaseOrders = purchaseOrders.Where(x => x.PurchaseOrderID.ToString().Contains(text));
            return Json(purchaseOrders.Select(p => new
            {
                Name = p.PurchaseOrderID + "-" + p.SupplierName,
                Code = p.PurchaseOrderID
            }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSuppliersList()
        {
            var suppliers = db.ProcurementSuppliers.Where(x => x.IsActive).AsQueryable();
            return Json(suppliers.Select(p => new
                {
                    Name = p.SupplierId + "-" + p.Name,
                    SupplierId = p.SupplierId
                }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetServiceRequisitionId()
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var storeReq = db.PurchaseRequisitions.AsQueryable().Where(x => x.ItemType == PurchaseItemType.Service)
                .Where(x => x.IsSecondOnlineApproved)
                .Where(x => x.IsVoid == false);//.Where(x => mybraches.Contains(x.OrgBranchName)).Where(sto => sto.IsFuel == false).Where(sto => sto.IsSecondOnlineApproved).AsQueryable();
            return Json(storeReq.Select(i => new
            {
                PurchaseRequisitionID = i.PurchaseRequisitionID,
                Remark = i.PurchaseRequisitionID + "-" + i.Remark,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPRforPoId()
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<PurchaseRequisitionAssignment> prlist = new List<PurchaseRequisitionAssignment>();

            var empid = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);

            var purchaseRequisitionAssignments = db.PurchaseRequisitionAssignments
                .Where(x => x.ProcurementMethod == ProcurementMethod.DirectPurchase)
                .Where(x=>x.ProcurementOfficer==empid)
                .ToList().DistinctBy(x => x.PurchaseRequisitionID);

            foreach (var assignment in purchaseRequisitionAssignments)
            {
                var pr = db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);
                if (pr != null && (pr.IsSecondOnlineApproved && pr.IsVoid == false))
                    prlist.Add(assignment);
            }

            //.Where(x => mybraches.Contains(x.OrgBranchName)).Where(sto => sto.IsFuel == false).Where(sto => sto.IsSecondOnlineApproved).AsQueryable();
            return Json(prlist.Select(i => new
            {
                PurchaseRequisitionID = i.PurchaseRequisitionID,
                Remark = i.PurchaseRequisitionID + "-" + i.PurchaseRequisition.Remark,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseRequisitonForPo(string id, string text)//for header not possible to have id
        {
            List<PurchaseRequisitionAssignment> prlist = new List<PurchaseRequisitionAssignment>();
            List<int> prids = new List<int>();
            var empid = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
            //int did = int.Parse(id);
            // var proformaPurchase = db.ProformaPurchases.Find(did);
            var purchaseRequisitionAssignments = db.PurchaseRequisitionAssignments
                .Where(x => x.ProcurementOfficer == empid)
                .Where(x => x.ProcurementMethod == ProcurementMethod.DirectPurchase).ToList();//.DistinctBy(x => x.PurchaseRequisitionID);

            prids.AddRange(purchaseRequisitionAssignments.DistinctBy(x => x.PurchaseRequisitionID).Select(x => x.PurchaseRequisitionID));

            foreach (var prid in prids)
            {
                int maxid = purchaseRequisitionAssignments.Where(x => x.PurchaseRequisitionID == prid)
                    .Max(x => x.PurchaseRequisitionAssignmentId);
                var maxrecord = purchaseRequisitionAssignments
                    .Where(x => x.PurchaseRequisitionID == prid)
                    .FirstOrDefault(x => x.PurchaseRequisitionAssignmentId == maxid);

                var pr = db.PurchaseRequisitions.Find(prid);
                if (pr != null && (pr.IsSecondOnlineApproved && pr.IsVoid == false))
                    prlist.Add(maxrecord);
            }

            //foreach (var assignment in purchaseRequisitionAssignments)
            //{
            //    var pr = db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);
            //    if (pr != null && (pr.IsSecondOnlineApproved && pr.IsVoid == false))
            //      prlist.Add(assignment);
            //}

            if (!string.IsNullOrEmpty(text))
            {
                prlist = prlist.Where(p => p.PurchaseRequisitionID.ToString().Contains(text.ToUpper())).ToList();
            }


            //if (proformaPurchase != null)
            //{
            //    purcahseRequisition = purcahseRequisition.Where(x => x.PurchaseCategory == proformaPurchase.PurchaseCategory).AsQueryable();
            //}
            return Json(prlist.Select(i => new
            {
                PurchaseRequisitionAssignmentId = i.PurchaseRequisitionAssignmentId,
                Description = i.PurchaseRequisitionID + "-" + i.PurchaseRequisitionAssignmentId
            }),
            JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPurchaseRequisitonForProforma(string id, string text)//for header not possible to have id
        {
            List<PurchaseRequisitionAssignment> prlist = new List<PurchaseRequisitionAssignment>();
            List<int> prids = new List<int>();
            var empid = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
            //int did = int.Parse(id);
           // var proformaPurchase = db.ProformaPurchases.Find(did);
            var purchaseRequisitionAssignments = db.PurchaseRequisitionAssignments
                .Where(x => x.ProcurementOfficer == empid)
                .Where(x => x.ProcurementMethod == ProcurementMethod.Proforma).ToList();//.DistinctBy(x => x.PurchaseRequisitionID);

                  prids.AddRange(purchaseRequisitionAssignments.DistinctBy(x => x.PurchaseRequisitionID).Select(x=>x.PurchaseRequisitionID));

                foreach (var prid in prids)
                {
                    int maxid = purchaseRequisitionAssignments.Where(x => x.PurchaseRequisitionID == prid)
                        .Max(x => x.PurchaseRequisitionAssignmentId);
                    var maxrecord = purchaseRequisitionAssignments
                        .Where(x => x.PurchaseRequisitionID == prid)
                        .FirstOrDefault(x => x.PurchaseRequisitionAssignmentId == maxid);

                    var pr = db.PurchaseRequisitions.Find(prid);
                    if (pr != null && (pr.IsSecondOnlineApproved && pr.IsVoid == false))
                        prlist.Add(maxrecord);
                }

                    //foreach (var assignment in purchaseRequisitionAssignments)
                    //{
                    //    var pr = db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);
                    //    if (pr != null && (pr.IsSecondOnlineApproved && pr.IsVoid == false))
                    //      prlist.Add(assignment);
                    //}

                    if (!string.IsNullOrEmpty(text))
                    {
                        prlist = prlist.Where(p => p.PurchaseRequisitionID.ToString().Contains(text.ToUpper())).ToList();
                    }

            
                //if (proformaPurchase != null)
                //{
                //    purcahseRequisition = purcahseRequisition.Where(x => x.PurchaseCategory == proformaPurchase.PurchaseCategory).AsQueryable();
                //}
                return Json(prlist.Select(i => new
                {
                    PurchaseRequisitionAssignmentId =i.PurchaseRequisitionAssignmentId,
                    Description= i.PurchaseRequisitionID + "-" + i.PurchaseRequisitionAssignmentId
               }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseRequisitonForBid(string id, string text)
        {
            List<PurchaseRequisitionAssignment> prlist = new List<PurchaseRequisitionAssignment>();
            List<int> prids = new List<int>();
            var empid = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
            int did = int.Parse(id);
            var tender = db.Tenders.Find(did);
            var purchaseRequisitionAssignments = db.PurchaseRequisitionAssignments
                .Where(x => x.ProcurementOfficer == empid)
                .Where(x => x.ProcurementMethod == ProcurementMethod.Tender)
                .Where(x=>x.IsOnlineApproved)
                .Where(x => x.IsVoid==false)
                .ToList();//.DistinctBy(x => x.PurchaseRequisitionID);

            prids.AddRange(purchaseRequisitionAssignments.DistinctBy(x => x.PurchaseRequisitionID).Select(x => x.PurchaseRequisitionID));

            foreach (var prid in prids)
            {
                int maxid = purchaseRequisitionAssignments.Where(x => x.PurchaseRequisitionID == prid)
                    .Max(x => x.PurchaseRequisitionAssignmentId);
                var maxrecord = purchaseRequisitionAssignments
                    .Where(x => x.PurchaseRequisitionID == prid)
                    .FirstOrDefault(x => x.PurchaseRequisitionAssignmentId == maxid);

                var pr = db.PurchaseRequisitions.Find(prid);
                if (pr != null && (pr.IsSecondOnlineApproved && pr.IsVoid == false))
                    prlist.Add(maxrecord);
            }

            //foreach (var assignment in purchaseRequisitionAssignments)
            //{
            //    var pr = db.PurchaseRequisitions.Find(assignment.PurchaseRequisitionID);
            //    if (pr != null && (pr.IsSecondOnlineApproved && pr.IsVoid == false))
            //        prlist.Add(assignment);
            //}

            if (!string.IsNullOrEmpty(text))
            {
                prlist = prlist.Where(p => p.PurchaseRequisitionID.ToString().Contains(text.ToUpper())).ToList();
            }

            //if (tender != null)
            //{
            //    purcahseRequisition = purcahseRequisition.Where(x => x.PurchaseCategory == tender.PurchaseCategory)
            //        .ToList();
            //}



            return Json(prlist.Select(i => new
                {
                    PurchaseRequisitionAssignmentId = i.PurchaseRequisitionAssignmentId,
                    Description = "PR-"+i.PurchaseRequisitionID + "-Assignment" + i.PurchaseRequisitionAssignmentId
                 }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetProformaId()
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var empid = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
            var proformaPurchases = db.ProformaPurchases
                .Where(x=>x.ProcurementOfficer==empid)
                .Where(x => x.IsOnlineApproved)
                .Where(x => x.IsVoid == false)
                .ToList();

            var selectedUnique = proformaPurchases.DistinctBy(x => x.PurchaseRequisitionID);
            //.Where(x => mybraches.Contains(x.OrgBranchName)).Where(sto => sto.IsFuel == false).Where(sto => sto.IsSecondOnlineApproved).AsQueryable();
            return Json(proformaPurchases.Select(i => new
            {
                ProformaPurchaseID = i.ProformaPurchaseID,
                Remark = i.ProformaPurchaseID + "-" + i.Remark,
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SupplierShortListSelector()
        {
            return View();
        }

        public JsonResult GetPurchasers()
        {
            var purchasers = db.Purchasers.Select(x => x.EmployeeId).ToList();
            var employees = db.Person_Employee.Where(x => purchasers.Contains(x.EmployeeId)).ToList();
            return Json(employees.Select(p => new
                {
                    Name = p.EmployeeCode + " " + p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
                    Code = p.EmployeeId
                }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSuboardinatePurchasers()
        {
            var userEmployeeId = UserInformationHelper.GetUserEmployeeId(User.Identity.Name);
            var subordinates = EmployeeHelper.GetSubordinates(userEmployeeId);
            var purchasers = db.Purchasers.Select(x => x.EmployeeId).ToList();
           // var subordinatepurchasers = subordinates.Where(x => purchasers.Contains(x));
            var employees = db.Person_Employee
                .Where(x => purchasers.Contains(x.EmployeeId))
                .AsQueryable();
            return Json(employees.Select(p => new
                {
                    Name = p.FirstNameEnglish + " " + p.MiddleNameEnglish + " " + p.LastNameEnglish,
                    Code = p.EmployeeId
                }),
                JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetServiceCategory()
        {
            var itemCategory = db.ServiceCategories.AsQueryable();
            return Json(itemCategory.Select(p => new
            {
                Name = p.ServiceCategoryCode + "-" + p.Name,
                ServiceCategoryCode = p.ServiceCategoryCode
            }),
                JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetServiceItems(string itemCategoryCode, string text)
        {
            var items = db.ServiceItems.Where(x => x.Enable).Where(i => i.ServiceCategoryCode == itemCategoryCode).OrderBy(x => x.Name).AsQueryable();
            if (!string.IsNullOrEmpty(text))
            {
                items = items.Where(p => p.Name.ToUpper().Contains(text.ToUpper()) || p.ServiceItemCode.Contains(text.ToUpper()));
            }
            return Json(items.Select(i => new
            {
                Name = i.ServiceItemCode + "-" + i.Name,
                ServiceItemCode = i.ServiceItemCode

            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetForeignCurrencyProcess()
        {
            var foreignCurrencyProcesses = db.ForeignCurrencyProcesses.AsQueryable();

            return Json(foreignCurrencyProcesses.ToList().DistinctBy(x => x.Description).Select(i => new
            {
                Description = i.Description,
                ForeignCurrencyProcessId = i.ForeignCurrencyProcessId
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetProformaParticipant(int id)
        {
            var participatingSuppliers = db.ParticipatingSuppliers.Where(x => x.ProformaPurchaseID == id).AsQueryable();

            return Json(participatingSuppliers.ToList().DistinctBy(x=>x.SupplierName).Select(i => new
            {
                Name = i.SupplierName,
                Code = i.SupplierName
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetProformaParticipatingSuppliers(string id)
        {
            int pid = int.Parse(id);
            var persons = db.ParticipatingSuppliers.Where(x => x.ProformaPurchaseID == pid).AsQueryable();
            return Json(persons.Select(i => new
            {
                Name = i.SupplierName,
                Code = i.SupplierName
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GeProformaWinningSuppliers(string itemCode, string id)
        {
            int pid = int.Parse(id);
            var persons = db.ParticipatingSupplierItems
                .Where(x => x.ItmCode == itemCode && x.ParticipatingSupplier.ProformaPurchaseID == pid).AsQueryable();
            return Json(persons.Select(i => new
            {
                Name = i.ParticipatingSupplier.SupplierName,
                Code = i.ParticipatingSupplier.SupplierName
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetProformaItems(string id)
        {
            int profid = int.Parse(id);
            var proformaItem = db.ProformaItems
                .Where(x=>x.ProformaPurchaseID==profid);

            return Json(proformaItem.Select(p => new
            {
                Name = p.ItmCode + "-" + p.ItemName + "-" + p.Name,
                ItemCode = p.ItmCode + "_" + p.Name
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetGuaranteeRuleModes(string category, string type)
        {
            int catid = int.Parse(category);
            int typeid = int.Parse(type);
            GuaranteeTypes typee = (GuaranteeTypes)typeid;
            ProcurementCategory cate = (ProcurementCategory)catid;
            var guaranteeModes = db.GuaranteeRules.Where(x => x.GuaranteeType == typee)
                                                   .Where(x => x.ProcurementCategory == cate).AsQueryable();
            return Json(guaranteeModes.Select(i => new
            {
                GuaranteeModeId = i.GuaranteeRuleId,
                GuaranteeMode = i.GuaranteeMode,
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGuaranteeModes()
        {
            var guaranteeModes = db.GuaranteeModes.AsQueryable();
            return Json(guaranteeModes.Select(i => new
            {
                GuaranteeModeId = i.GuaranteeModeId,
                Mode = i.Mode,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProformaPurchaseId()
        {
            var proformaPurchases = db.ProformaPurchases.Where(sto => sto.IsOnlineApproved).AsQueryable();
            return Json(proformaPurchases.Select(i => new
            {
                ProformaPurchaseID = i.ProformaPurchaseID,
                Remark = i.ProformaPurchaseID + "-" + i.ProformaReference,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetForeignPurchaseCostTypes()
        {
            var proformaPurchases = db.ForeignPurchaseCostTypes.AsQueryable();
            return Json(proformaPurchases.Select(i => new
            {
                Name = i.CostName
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPurchaseRequisitionId()
        {
            var purchaseRequisitions = db.PurchaseRequisitions.Where(sto => sto.IsOnlineApproved).AsQueryable();
            return Json(purchaseRequisitions.Select(i => new
            {
                PurchaseRequisitionID = i.PurchaseRequisitionID,
                Remark = i.PurchaseRequisitionID + "-" + i.ReferenceNo + "-" + i.Remark,
            }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSuppliers()
        {

            var suppliers = db.ProcurementSuppliers.Where(x => x.IsActive).Where(x => x.IsForShortList).Where(x => x.IsBlackListed == false).ToList();


            return Json(suppliers.Select(c => new { value = c.SupplierId + "_" + c.Name, desc = c.SupplierId + "_" + c.Name }), JsonRequestBehavior.AllowGet);
        }

       
        public JsonResult GetPurchaseReferencesForFollowUp(string purReference, string text)//for follow up
        {
            List<SelectorVm> list = new List<SelectorVm>();

            if (purReference == "Purchase Order")
            {
                var purchaseOrders = db.PurchaseOrders.Where(x => x.IsOnlineApproved && x.IsVoid == false && x.IsManual).ToList();

                if (!String.IsNullOrWhiteSpace(text))
                    purchaseOrders = purchaseOrders.Where(x => x.PurchaseOrderID.ToString() == text).ToList();



                foreach (var po in purchaseOrders)
                {
                    var record = new SelectorVm
                    {
                        Name = po.PurchaseOrderID + "_" + po.SupplierName,
                        Code = po.PurchaseOrderID.ToString()
                    };

                    list.Add(record);

                }
            }

           
            else if (purReference == "Tender Award")
            {
                var tenders = db.Tenders.Where(x => x.IsOnlineApproved && x.IsVoid == false && x.IsCancelled == false).ToList();


                foreach (var tender in tenders)
                {
                    var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();

                    foreach (var lot in lots)
                    {
                        var awards = db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID).ToList();//change to contract...add not transferred since awards can be transffered separately
                        if (!String.IsNullOrWhiteSpace(text))
                            awards = awards.Where(x => x.BidAwardedSupplierID.ToString() == text).ToList();
                        foreach (var award in awards)
                        {

                            var record = new SelectorVm
                            {
                                Name = tender.TenderReference + "_" + lot.Description + "_" + award.SupplierName,
                                Code = award.BidAwardedSupplierID.ToString()
                            };

                            list.Add(record);
                        }
                    }
                }
            }

            return Json(list.Select(c => new
            {
                Name = c.Name,
                Code = c.Code
            }), JsonRequestBehavior.AllowGet);

        }

       
        
        //public JsonResult GetPurchaseReferences_old1(string purReference,string text)//new optimized,for appayment advance
        //{
        //    List<SelectorVm> list = new List<SelectorVm>();

        //    if (purReference == "Purchase Order")
        //    {
        //        var purchaseOrders = db.PurchaseOrders                   
        //                            .Where(x => x.IsOnlineApproved && x.IsVoid == false)
        //                            .Where(x => x.IsPaid == false)                                    
        //                            .Select(x => new
        //                            {   x.PurchaseOrderID,
        //                                x.SupplierName,
        //                                PurchaseOrderItem=x.PurchaseOrderItem,
        //                                PoPaymentSchedule=x.PoPaymentSchedule
        //                            })
        //                            .AsNoTracking()
        //                            .AsQueryable();

        //        if (!String.IsNullOrWhiteSpace(text))
        //            purchaseOrders = purchaseOrders.Where(x => x.PurchaseOrderID.ToString() == text
        //                                                       ||x.SupplierName.ToUpper().Contains(text.ToUpper())).AsQueryable();

        //        foreach (var po in purchaseOrders)
        //        {
        //            #region check if po has advance payment schedule
        //            var advanceAllowed = po.PoPaymentSchedule
        //                .Where(x => x.PaymentType == PaymentType.Advance)
        //                .Where(x => x.Percentage > 0).ToList();

        //            if (!advanceAllowed.Any())
        //                continue;
                        
        //            #endregion
        //            #region check over advance taken

        //            var advanceTaken = db.APPayments
        //                .Where(x => x.PurchaseReference == po.PurchaseOrderID.ToString())
        //                .Where(x => x.PoType == "Purchase Order")
        //                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
        //                .Select(x => new { x.PrepaymentAmount })
        //                .AsQueryable();
        //            if (advanceTaken.Any())
        //            {                        
        //                decimal amount = po.PurchaseOrderItem.Where(x => x.PurchaseOrderID == po.PurchaseOrderID)
        //                    .Select(x => new { x.UnitPrice, x.Quantity,x.Total }).ToList()
        //                    .Sum(x => x.Total);

        //                decimal totalAadvanceAmount = (amount*advanceAllowed.Sum(x=>x.Percentage))/100;
        //                decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount);
        //                if (advanceTakenAmount >= totalAadvanceAmount)
        //                    continue;
        //            }

        //            #endregion

        //            var record = new SelectorVm
        //            {
        //                Name = po.PurchaseOrderID + "_" + po.SupplierName,
        //                Code = po.PurchaseOrderID.ToString()
        //            };

        //            list.Add(record);

        //        }
        //    }

        //    else if (purReference == "Tender Award")
        //    {
        //        var contracts = db.ContractManagements.Where(x => x.IsOnlineApproved && x.IsVoid == false)
        //                    .Where(x => x.IsPaid == false)
        //                    //.Where(x=>x.ProcurementRefNo==420)//test
        //                    .Select(x => new
        //                    { x.ContractManagementID,
        //                        x.SupplierName,
        //                        x.ProcurementRefNo,
        //                        x.TenderRefNo,
        //                        ContractManagementItems=x.ContractManagementItem,
        //                        ContractPaymentSchedules=x.ContractPaymentSchedules
        //                    })
        //                        .AsNoTracking()
        //                        .AsQueryable();

        //        if (!String.IsNullOrWhiteSpace(text))
        //            contracts = contracts.Where(x => x.ProcurementRefNo.ToString() == text
        //                                       || x.SupplierName.ToUpper().Contains(text.ToUpper())
        //                                       || x.TenderRefNo.ToUpper().Contains(text.ToUpper())).AsQueryable();

        //        foreach (var contract in contracts)
        //        {
        //            #region check if contract has advance payment schedule
        //            var advanceAllowed = contract.ContractPaymentSchedules
        //                .Where(x => x.PaymentType == PaymentType.Advance)
        //                .Where(x => x.Percentage > 0).ToList();

        //            if (!advanceAllowed.Any())
        //                continue;
        //            #endregion
        //            #region check over advance taken

        //            var advanceTaken = db.APPayments
        //                 .Where(x => x.PurchaseReference == contract.ProcurementRefNo.ToString())
        //                .Where(x => x.PoType == "Tender Award")
        //                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
        //                .Select(x => new { x.PrepaymentAmount })
        //                .AsQueryable();
        //            if (advanceTaken.Any())
        //            {
        //                decimal amount = contract.ContractManagementItems.Sum(x => x.Total);
        //                decimal totalAadvanceAmount = (amount * advanceAllowed.Sum(x => x.Percentage)) / 100;
        //                decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount);
        //                if (advanceTakenAmount >= totalAadvanceAmount)
        //                    continue;
        //            }

        //            #endregion

        //            var record = new SelectorVm
        //            {
        //                Name = contract.TenderRefNo + "_" + contract.SupplierName,
        //                Code = contract.ProcurementRefNo.ToString()
        //            };

        //            list.Add(record);
        //        }
        //    }

        //    return Json(list.Select(c => new
        //    {
        //        Name = c.Name,
        //        Code = c.Code
        //    }), JsonRequestBehavior.AllowGet);

        //}

        //public JsonResult GetPurchaseReferences_old(string purReference, string text)//for appayment advance
        //{
        //    List<SelectorVm> list = new List<SelectorVm>();

        //    if (purReference == "Purchase Order")
        //    {
        //        var purchaseOrders = db.PurchaseOrders
        //            .Where(x => x.IsOnlineApproved && x.IsVoid == false).ToList();

        //        if (!String.IsNullOrWhiteSpace(text))
        //            purchaseOrders = purchaseOrders.Where(x => x.PurchaseOrderID.ToString() == text
        //                                                       || x.SupplierName.ToUpper().Contains(text.ToUpper())).ToList();

        //        foreach (var po in purchaseOrders)
        //        {
        //            #region check over advance taken

        //            var advanceTaken = db.APPayments
        //                .Where(x => x.PurchaseReference == po.PurchaseOrderID.ToString())
        //                .Where(x => x.PoType == "Purchase Order")
        //                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
        //                .ToList();
        //            if (advanceTaken.Any())
        //            {
        //                decimal advanceAmount = db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == po.PurchaseOrderID).ToList()
        //                    .Sum(x => x.UnitPrice * x.Quantity);
        //                decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount);
        //                if (advanceTakenAmount > advanceAmount)
        //                    continue;
        //            }

        //            #endregion

        //            var record = new SelectorVm
        //            {
        //                Name = po.PurchaseOrderID + "_" + po.SupplierName,
        //                Code = po.PurchaseOrderID.ToString()
        //            };

        //            list.Add(record);

        //        }
        //    }

        //    else if (purReference == "Tender Award")
        //    {
        //        var tenders = db.Tenders.Where(x => x.IsOnlineApproved && x.IsVoid == false && x.IsCancelled == false).ToList();


        //        foreach (var tender in tenders)
        //        {
        //            var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();

        //            foreach (var lot in lots)
        //            {

        //                var awards = db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID).ToList();//change to contract...add not transferred since awards can be transffered separately
        //                if (!String.IsNullOrWhiteSpace(text))
        //                    awards = awards.Where(x => x.BidAwardedSupplierID.ToString() == text
        //                                               || x.SupplierName.ToUpper().Contains(text.ToUpper())
        //                                               || tender.TenderReference.ToUpper().Contains(text.ToUpper())).ToList();
        //                foreach (var award in awards)
        //                {
        //                    #region check over advance taken

        //                    var advanceTaken = db.APPayments
        //                        .Where(x => x.PurchaseReference == award.BidAwardedSupplierID.ToString())
        //                        .Where(x => x.PoType == "Tender Award")
        //                        .Where(x => x.IsOnlineApproved && x.IsVoid == false)
        //                        .ToList();
        //                    if (advanceTaken.Any())
        //                    {
        //                        decimal advanceAmount = 0;
        //                        var contract = db.ContractManagements.FirstOrDefault(x => x.ProcurementRefNo == award.BidAwardedSupplierID);

        //                        if (contract != null)
        //                        {
        //                            var items = db.ContractManagementItems.Where(x => x.ContractManagementID == contract.ContractManagementID).ToList();

        //                            var schedule = db.ContractPaymentSchedules
        //                                .Where(x => x.ContractManagementID == contract.ContractManagementID)
        //                                .FirstOrDefault(x => x.PaymentType == PaymentType.Advance);

        //                            foreach (var item in items)
        //                            {
        //                                advanceAmount = advanceAmount + (item.Quantity + item.Addition + item.Omission) * item.UnitPrice;
        //                            }

        //                            if (schedule != null && schedule.Percentage > 0)
        //                                advanceAmount = advanceAmount * schedule.Percentage / 100;
        //                        }
        //                        decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount);
        //                        if (advanceTakenAmount > advanceAmount)
        //                            continue;
        //                    }

        //                    #endregion

        //                    var record = new SelectorVm
        //                    {
        //                        Name = tender.TenderReference + "_" + lot.Description + "_" + award.SupplierName,
        //                        Code = award.BidAwardedSupplierID.ToString()
        //                    };

        //                    list.Add(record);
        //                }
        //            }
        //        }
        //    }

        //    return Json(list.Select(c => new
        //    {
        //        Name = c.Name,
        //        Code = c.Code
        //    }), JsonRequestBehavior.AllowGet);

        //}

        public JsonResult GetPreviousPurchaseReference(int procurementMethod)

        {
            int validDays = 180;//less than six month only
            DateTime minimumDate = DateTime.Today.AddDays(-validDays);

            List<SelectorVm> list = new List<SelectorVm>();

             if (procurementMethod == 0)//tender
            {
                var purchaseOrders = db.PurchaseOrders
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Where(x=>x.BidAwardedSupplierID!=null)
                    .Where(x => x.OrderDate >= minimumDate)
                    .ToList();

                foreach (var po in purchaseOrders)
                {
                    var record = new SelectorVm
                    {
                        Name = "PO_" + po.PurchaseOrderID + "_" + po.SupplierName,
                        Code = po.PurchaseOrderID.ToString()
                    };

                    list.Add(record);

                }
                //var tenders = db.Tenders.Where(x => x.IsOnlineApproved && x.IsVoid==false && x.IsCancelled == false).ToList();


                //foreach (var tender in tenders)
                //{
                //    var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();

                //    foreach (var lot in lots)
                //    {
                //        var awards = db.BidAwardedSuppliers
                //            .Where(x => x.LotID == lot.LotID)
                //            .Where(x => x.AwardDate >= minimumDate)
                //            .ToList();
                //        foreach (var award in awards)
                //        {

                //            var record = new SelectorVm
                //            {
                //                Name = "Tender_" + tender.TenderReference + "_" + lot.Description + "_" + award.SupplierName,
                //                Code = award.BidAwardedSupplierID.ToString()
                //            };

                //            list.Add(record);
                //        }
                //    }
                //}
            }
            else if (procurementMethod == 1)//proforma
            {
                var proformaPurchases = db.ProformaPurchases.Where(x => x.IsOnlineApproved && x.IsVoid == false).ToList();
                

                    foreach (var proformaPurchase in proformaPurchases)
                    {
                        var awards = db.WinningSuppliers
                            .Where(x => x.ProformaPurchaseID == proformaPurchase.ProformaPurchaseID)
                            .ToList();
                        foreach (var award in awards)
                        {

                            var record = new SelectorVm
                            {
                                Name = "Proforma_" + proformaPurchase.ProformaPurchaseID + "_" + award.SupplierName + "_" + proformaPurchase.Description,
                                Code = award.WinningSupplierID.ToString()
                            };

                            list.Add(record);
                        }
                    }
                
            }
            else if (procurementMethod == 2)//po
            {
                var purchaseOrders = db.PurchaseOrders
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Where(x => x.OrderDate >= minimumDate)
                    .ToList();

                foreach (var po in purchaseOrders)
                {
                    var record = new SelectorVm
                    {
                        Name = "PO_" + po.PurchaseOrderID + "_" + po.SupplierName,
                        Code = po.PurchaseOrderID.ToString()
                    };

                    list.Add(record);

                }
            }

            return Json(list.Select(c => new
            {
                Name = c.Name,
                Code = c.Code
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetTenderAwards()

        {
            List<SelectorVm> list = new List<SelectorVm>();

            var tenders = db.Tenders.Where(x => x.IsOnlineApproved && x.IsVoid == false && x.IsCancelled == false).ToList();

            foreach (var tender in tenders)
            {
                var lots = db.Lots.Where(x => x.TenderID == tender.TenderID).ToList();

                foreach (var lot in lots)
                {
                    var awards = db.BidAwardedSuppliers.Where(x => x.LotID == lot.LotID).ToList();
                    foreach (var award in awards)
                    {

                        var record = new SelectorVm
                        {
                            Name = "Tender_" + tender.TenderReference + "_" + lot.Description + "_" + award.SupplierName,
                            Code = award.BidAwardedSupplierID.ToString()
                        };

                        list.Add(record);
                    }
                }
            }


            return Json(list.Select(c => new
            {
                Name = c.Name,
                Code = c.Code
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPOandContractGrns(string id,string type)

        {
            List<SelectorVm> list = new List<SelectorVm>();

            int did = int.Parse(id);
            var purchasePayment = db.PurchasePayments.Find(did);
            if(purchasePayment!=null)
            {
                if(purchasePayment.Type=="Tender Award")
                {
                    var receiveType = db.ReceiveTypes.FirstOrDefault(x => x.Type == GoodReceiveType.Tender);
                    if (type == "0")
                    {
                        #region grn stock 

                        var stockReceives = db.GoodReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                            .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                            .Where(x => x.ReceiveRef == purchasePayment.PoOrContractNumber);

                        foreach (var goodReceive in stockReceives)
                        {
                            var record = new SelectorVm
                            {
                                Name = PurchaseItemType.Stock + "_" + goodReceive.GoodReceiveId,
                                Code = PurchaseItemType.Stock + "_" + goodReceive.GoodReceiveId
                            };

                            list.Add(record);
                        }

                        #endregion
                    }
                    else if (type == "2")
                    {
                        #region grn service 

                        var serviceReceives = db.ServiceReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                            .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                            .Where(x => x.ReceiveRef == purchasePayment.PoOrContractNumber);

                        foreach (var serviceReceive in serviceReceives)
                        {
                            var record = new SelectorVm
                            {
                                Name = PurchaseItemType.Service + "_" + serviceReceive.ServiceReceiveId,
                                Code = PurchaseItemType.Service + "_" + serviceReceive.ServiceReceiveId
                            };

                            list.Add(record);
                        }

                        #endregion
                    }
                    else if (type == "1")
                    {
                        #region grn fixed asset 

                        var fixedAssetReceives = db.FixedAssetReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                            .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                            .Where(x => x.ReceiveRef == purchasePayment.PoOrContractNumber);

                        foreach (var fixedAssetReceive in fixedAssetReceives)
                        {
                            var record = new SelectorVm
                            {
                                Name = PurchaseItemType.FixedAsset + "_" + fixedAssetReceive.FixedAssetReceiveId,
                                Code = PurchaseItemType.FixedAsset + "_" + fixedAssetReceive.FixedAssetReceiveId
                            };

                            list.Add(record);
                        }

                        #endregion
                    }
                }

                else if(purchasePayment.Type == "Purchase Order")
                {
                    var receiveType = db.ReceiveTypes.FirstOrDefault(x => x.Type == GoodReceiveType.PurchaseOrder);
                    if (type == "0")
                    {
                        #region grn stock 

                        var stockReceives = db.GoodReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                            .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                            .Where(x => x.ReceiveRef == purchasePayment.PoOrContractNumber);

                        foreach (var goodReceive in stockReceives)
                        {
                            var record = new SelectorVm
                            {
                                Name = PurchaseItemType.Stock + "_" + goodReceive.GoodReceiveId,
                                Code =  goodReceive.GoodReceiveId.ToString()
                            };

                            list.Add(record);
                        }

                        #endregion
                    }
                    else if (type == "2")
                    {
                        #region grn service 

                        var serviceReceives = db.ServiceReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                            .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                            .Where(x => x.ReceiveRef == purchasePayment.PoOrContractNumber);

                        foreach (var serviceReceive in serviceReceives)
                        {
                            var record = new SelectorVm
                            {
                                Name = PurchaseItemType.Service + "_" + serviceReceive.ServiceReceiveId,
                                Code =  serviceReceive.ServiceReceiveId.ToString()
                            };

                            list.Add(record);
                        }

                        #endregion
                    }
                    else if (type == "1")
                    {
                        #region grn fixed asset 

                        var fixedAssetReceives = db.FixedAssetReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                            .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                            .Where(x => x.ReceiveRef == purchasePayment.PoOrContractNumber);

                        foreach (var fixedAssetReceive in fixedAssetReceives)
                        {
                            var record = new SelectorVm
                            {
                                Name = PurchaseItemType.FixedAsset + "_" + fixedAssetReceive.FixedAssetReceiveId,
                                Code = fixedAssetReceive.FixedAssetReceiveId.ToString()
                            };

                            list.Add(record);
                        }

                        #endregion
                    }
                }
            }
            

            return Json(list.Select(c => new
            {
                Name = c.Name,
                Code = c.Code
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPOandContractGrnsForSettlement(string id, string type,string text)
        {
            List<SelectorVm> list = new List<SelectorVm>();
          
            if (type == "0")
            {
                #region grn stock 

                var stockReceives = db.GoodReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false).AsQueryable();
                if (!string.IsNullOrEmpty(text))
                    stockReceives = stockReceives.Where(x=>x.GoodReceiveId.ToString().Contains(text));

                foreach (var goodReceive in stockReceives)
                {
                    var record = new SelectorVm
                    {
                        Name = PurchaseItemType.Stock + "_" + goodReceive.GoodReceiveId,
                        Code = goodReceive.GoodReceiveId.ToString()
                    };

                    list.Add(record);
                }

                #endregion
            }
            else if (type == "2")
            {
                #region grn service 

                var serviceReceives = db.ServiceReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false).AsQueryable();
                if (!string.IsNullOrEmpty(text))
                    serviceReceives = serviceReceives.Where(x => x.ServiceReceiveId.ToString().Contains(text));

                foreach (var serviceReceive in serviceReceives)
                {
                    var record = new SelectorVm
                    {
                        Name = PurchaseItemType.Service + "_" + serviceReceive.ServiceReceiveId,
                        Code = serviceReceive.ServiceReceiveId.ToString()
                    };

                    list.Add(record);
                }

                #endregion
            }
            else if (type == "1")
            {
                #region grn fixed asset 

                var fixedAssetReceives = db.FixedAssetReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false).AsQueryable();
                if (!string.IsNullOrEmpty(text))
                    fixedAssetReceives = fixedAssetReceives.Where(x => x.FixedAssetReceiveId.ToString().Contains(text));

                foreach (var fixedAssetReceive in fixedAssetReceives)
                {
                    var record = new SelectorVm
                    {
                        Name = PurchaseItemType.FixedAsset + "_" + fixedAssetReceive.FixedAssetReceiveId,
                        Code = fixedAssetReceive.FixedAssetReceiveId.ToString()
                    };

                    list.Add(record);
                }

                #endregion
            }
            return Json(list.Select(c => new
            {
                Name = c.Name,
                Code = c.Code
            }), JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetPOandContractGrnsForSettlement_1(string id, string type)

        {
            List<SelectorVm> list = new List<SelectorVm>();

            int did = int.Parse(id);
            var settlement = db.ApPaymentSettlements.Find(did);
            var settlementLine = db.ApPaymentSettlementLines.Where(x => x.ApPaymentSettlementId == settlement.ApPaymentSettlementId).FirstOrDefault();

            if (settlementLine != null && settlementLine.Source == SettlemetSource.Prepayment.ToString())
            {
                var prepayment = db.APPayments.Find(settlementLine.SourceId);
                int purchasePayId = int.Parse(prepayment.PurchaseReference);
                var purchasePayment = db.PurchasePayments.Find(purchasePayId);
                if (purchasePayment != null)
                {
                    if (purchasePayment.Type == "Tender Award")
                    {
                        var receiveType = db.ReceiveTypes.FirstOrDefault(x => x.Type == GoodReceiveType.Tender);
                        if (type == "0")
                        {
                            #region grn stock 

                            var stockReceives = db.GoodReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                                .Where(x => x.ReceiveRef == purchasePayment.PoOrContractNumber);

                            foreach (var goodReceive in stockReceives)
                            {
                                var record = new SelectorVm
                                {
                                    Name = PurchaseItemType.Stock + "_" + goodReceive.GoodReceiveId,
                                    Code = PurchaseItemType.Stock + "_" + goodReceive.GoodReceiveId
                                };

                                list.Add(record);
                            }

                            #endregion
                        }
                        else if (type == "2")
                        {
                            #region grn service 

                            var serviceReceives = db.ServiceReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                                .Where(x => x.ReceiveRef == purchasePayment.PoOrContractNumber);

                            foreach (var serviceReceive in serviceReceives)
                            {
                                var record = new SelectorVm
                                {
                                    Name = PurchaseItemType.Service + "_" + serviceReceive.ServiceReceiveId,
                                    Code = PurchaseItemType.Service + "_" + serviceReceive.ServiceReceiveId
                                };

                                list.Add(record);
                            }

                            #endregion
                        }
                        else if (type == "1")
                        {
                            #region grn fixed asset 

                            var fixedAssetReceives = db.FixedAssetReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                                .Where(x => x.ReceiveRef == purchasePayment.PoOrContractNumber);

                            foreach (var fixedAssetReceive in fixedAssetReceives)
                            {
                                var record = new SelectorVm
                                {
                                    Name = PurchaseItemType.FixedAsset + "_" + fixedAssetReceive.FixedAssetReceiveId,
                                    Code = PurchaseItemType.FixedAsset + "_" + fixedAssetReceive.FixedAssetReceiveId
                                };

                                list.Add(record);
                            }

                            #endregion
                        }
                    }

                    else if (purchasePayment.Type == "Purchase Order")
                    {
                        var receiveType = db.ReceiveTypes.FirstOrDefault(x => x.Type == GoodReceiveType.PurchaseOrder);
                        if (type == "0")
                        {
                            #region grn stock 

                            var stockReceives = db.GoodReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                                .Where(x => x.ReceiveRef == purchasePayment.PoOrContractNumber);

                            foreach (var goodReceive in stockReceives)
                            {
                                var record = new SelectorVm
                                {
                                    Name = PurchaseItemType.Stock + "_" + goodReceive.GoodReceiveId,
                                    Code = goodReceive.GoodReceiveId.ToString()
                                };

                                list.Add(record);
                            }

                            #endregion
                        }
                        else if (type == "2")
                        {
                            #region grn service 

                            var serviceReceives = db.ServiceReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                                .Where(x => x.ReceiveRef == purchasePayment.PoOrContractNumber);

                            foreach (var serviceReceive in serviceReceives)
                            {
                                var record = new SelectorVm
                                {
                                    Name = PurchaseItemType.Service + "_" + serviceReceive.ServiceReceiveId,
                                    Code = serviceReceive.ServiceReceiveId.ToString()
                                };

                                list.Add(record);
                            }

                            #endregion
                        }
                        else if (type == "1")
                        {
                            #region grn fixed asset 

                            var fixedAssetReceives = db.FixedAssetReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                                .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                                .Where(x => x.ReceiveRef == purchasePayment.PoOrContractNumber);

                            foreach (var fixedAssetReceive in fixedAssetReceives)
                            {
                                var record = new SelectorVm
                                {
                                    Name = PurchaseItemType.FixedAsset + "_" + fixedAssetReceive.FixedAssetReceiveId,
                                    Code = fixedAssetReceive.FixedAssetReceiveId.ToString()
                                };

                                list.Add(record);
                            }

                            #endregion
                        }
                    }
                }

            }
            return Json(list.Select(c => new
            {
                Name = c.Name,
                Code = c.Code
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetContractGrns(string id, string type)

        {
            List<SelectorVm> list = new List<SelectorVm>();

            int did = int.Parse(id);
            var contract = db.ContractManagements.Find(did);
            var receiveType = db.ReceiveTypes.FirstOrDefault(x => x.Type == GoodReceiveType.Tender);
            if (type == "0")
            {
                #region grn stock 

                var stockReceives = db.GoodReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                    .Where(x => x.ReceiveRef == contract.ProcurementRefNo.ToString());

                foreach (var goodReceive in stockReceives)
                {
                    var record = new SelectorVm
                    {
                        Name = PurchaseItemType.Stock + "_" + goodReceive.GoodReceiveId,
                        Code = PurchaseItemType.Stock + "_" + goodReceive.GoodReceiveId
                    };

                    list.Add(record);
                }

                #endregion
            }
            else if (type == "2")
            {
                #region grn service 

                var serviceReceives = db.ServiceReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                    .Where(x => x.ReceiveRef == contract.ProcurementRefNo.ToString());

                foreach (var serviceReceive in serviceReceives)
                {
                    var record = new SelectorVm
                    {
                        Name = PurchaseItemType.Service + "_" + serviceReceive.ServiceReceiveId,
                        Code = PurchaseItemType.Service + "_" + serviceReceive.ServiceReceiveId
                    };

                    list.Add(record);
                }

                #endregion
            }
            else if (type == "1")
            {
                #region grn fixed asset 

                var fixedAssetReceives = db.FixedAssetReceives.Where(x => x.IsOnlineApproved && x.IsVoid == false)
                    .Where(x => x.ReceiveTypeId == receiveType.ReceiveTypeId)
                    .Where(x => x.ReceiveRef == contract.ProcurementRefNo.ToString());

                foreach (var fixedAssetReceive in fixedAssetReceives)
                {
                    var record = new SelectorVm
                    {
                        Name = PurchaseItemType.FixedAsset + "_" + fixedAssetReceive.FixedAssetReceiveId,
                        Code = PurchaseItemType.FixedAsset + "_" + fixedAssetReceive.FixedAssetReceiveId
                    };

                    list.Add(record);
                }

                #endregion
            }

            return Json(list.Select(c => new
            {
                Name = c.Name,
                Code = c.Code
            }), JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetPoContractData(string id,string type, string paymenttype)
        {
           
            decimal totalAmount = 0;
            decimal totalServiceCharge = 0;
            decimal amount = 0;
            string supplierId = "";
            string supplierName = "";
            int did = int.Parse(id);

            if(type=="Purchase Order")
            {
                var po = db.PurchaseOrders.Find(did);
                var paymentSchedule = db.PoPaymentSchedules.Where(x => x.PurchaseOrderID == did);
                if (po != null)
                {
                    supplierId = po.SupplierId;
                    supplierName = po.SupplierName;
                }

                if (paymentSchedule.Any())
                {                 
                                     
                    var advances = paymentSchedule.Where(x => x.PaymentType == PaymentType.Advance).Where(x => x.PurchaseOrderID == did).ToList();
                    var payments = paymentSchedule.Where(x => x.PaymentType == PaymentType.Payment).Where(x => x.PurchaseOrderID == did).ToList();
                    var credits = paymentSchedule.Where(x => x.PaymentType == PaymentType.Credit).Where(x => x.PurchaseOrderID == did).ToList();

                    var items = db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == did).ToList();                   

                    if (items.Any())
                        totalAmount = items.Sum(x => x.SubTotal);
                    #region additional charge
                    var additionalCharge = db.PoAdditionalCharges.Where(x => x.PurchaseOrderID == did).ToList();
                    foreach ( var item in additionalCharge )
                    {
                        if(item.AmountType== AmountType.Percent)
                        {
                            totalServiceCharge=totalAmount*(item.Amount/100);
                        }
                        else
                        {
                            totalServiceCharge = totalServiceCharge + item.Amount;
                        }
                    }
                    #endregion
                    totalAmount = totalAmount + totalServiceCharge;

                    if (advances.Any() && paymenttype == "0")
                    {
                        decimal sumPercent = 0;
                        decimal byAmount = 0;
                        
                        var listAmount = advances.Where(x => x.AmountType == AmountType.Amount);
                        var listPercentage = advances.Where(x => x.AmountType == AmountType.Percent);

                        if (listPercentage.Any())
                            sumPercent = listPercentage.Sum(x => x.Percentage);

                        if (listAmount.Any())
                            byAmount = listAmount.Sum(x => x.Percentage);

                        amount = (totalAmount * (sumPercent / 100)) + byAmount;
                    }

                    else if (payments.Any() && paymenttype == "1")
                    {
                        decimal sumPercent = 0;
                        decimal byAmount = 0;

                        var listAmount = payments.Where(x => x.AmountType == AmountType.Amount);
                        var listPercentage = payments.Where(x => x.AmountType == AmountType.Percent);

                        if (listPercentage.Any())
                            sumPercent = listPercentage.Sum(x => x.Percentage);

                        if (listAmount.Any())
                            byAmount = listAmount.Sum(x => x.Percentage);

                        amount = (totalAmount * (sumPercent / 100)) + byAmount;
                    }
                    else if (credits.Any() && paymenttype == "2")
                    {

                        //credit should come from GRN

                        //decimal sumPercent = 0;
                        //decimal byAmount = 0;

                        //var listAmount = credits.Where(x => x.AmountType == AmountType.Amount);
                        //var listPercentage = credits.Where(x => x.AmountType == AmountType.Percent);

                        //if (listPercentage.Any())
                        //    sumPercent = listPercentage.Sum(x => x.Percentage);

                        //if (listAmount.Any())
                        //    byAmount = listAmount.Sum(x => x.Percentage);

                        //amount = (totalAmount * (sumPercent / 100)) + byAmount;
                    }
                    else
                        amount = 0;

                }
            }

            else if (type == "Tender Award")
            {
                var contract = db.ContractManagements.Find(did);
                if (contract != null)
                {
                    supplierId = contract.SupplierId;
                    supplierName = contract.SupplierName;
                }

                var items = db.ContractManagementItems.Where(x => x.ContractManagementID == did).ToList();
                if (items.Any())
                    amount = items.Sum(x => x.Total)- items.Sum(x => x.TaxAmount);
            }

            var formattedData = new
            {               
                PaymentAmount = amount,
                SupplierId=supplierId,
                SupplierName=supplierName
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPoDataWithServiceCharge(string id, string type, string paymenttype)
        {

            decimal amount = 0;
            string supplierId = "";
            string supplierName = "";
            int did = int.Parse(id);

            if (type == "Purchase Order")
            {
                var po = db.PurchaseOrders.Find(did);
                var paymentSchedule = db.PoPaymentSchedules.Where(x => x.PurchaseOrderID == did);
                if (po != null)
                {
                    supplierId = po.SupplierId;
                    supplierName = po.SupplierName;
                }

                if (paymentSchedule.Any())
                {
                    decimal sumAdvancePercent = 0;
                    decimal sumPaymentPercent = 0;
                    decimal sumCreditPercent = 0;
                    var advances = paymentSchedule.Where(x => x.PaymentType == PaymentType.Advance).Where(x => x.PurchaseOrderID == did).ToList();
                    var payments = paymentSchedule.Where(x => x.PaymentType == PaymentType.Payment).Where(x => x.PurchaseOrderID == did).ToList();
                    var credits = paymentSchedule.Where(x => x.PaymentType == PaymentType.Credit).Where(x => x.PurchaseOrderID == did).ToList();

                    var items = db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == did).ToList();
                    if (items.Any())
                        amount = items.Sum(x => x.SubTotal);

                    if (advances.Any() && paymenttype == "0")
                    {
                        sumAdvancePercent = advances.Sum(x => x.Percentage);
                        amount = amount * (sumAdvancePercent / 100);
                    }

                    else if (payments.Any() && paymenttype == "1")
                    {
                        sumPaymentPercent = payments.Sum(x => x.Percentage);
                        amount = amount * (sumPaymentPercent / 100);
                    }
                    else if (payments.Any() && paymenttype == "2")
                    {
                        sumCreditPercent = credits.Sum(x => x.Percentage);
                        amount = amount * (sumCreditPercent / 100);
                    }
                    else
                        amount = 0;

                }
            }

            else if (type == "Tender Award")
            {
                var contract = db.ContractManagements.Find(did);
                if (contract != null)
                {
                    supplierId = contract.SupplierId;
                    supplierName = contract.SupplierName;
                }

                var items = db.ContractManagementItems.Where(x => x.ContractManagementID == did).ToList();
                if (items.Any())
                    amount = items.Sum(x => x.Total) - items.Sum(x => x.TaxAmount);
            }

            var formattedData = new
            {
                PaymentAmount = amount,
                SupplierId = supplierId,
                SupplierName = supplierName
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
    }
}