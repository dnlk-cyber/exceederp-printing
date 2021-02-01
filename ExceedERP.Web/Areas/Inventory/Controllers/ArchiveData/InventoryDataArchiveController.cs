using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Drawing;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.ViewModels;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Inventory.ArchiveModel;
using ExceedERP.Core.Domain.Inventory.ViewModels;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Repository;
using ExceedERP.Web.Areas.Inventory.Controllers;
using ExceedERP.Web.Helpers;
using Hangfire;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers
{
   
    //[AuthorizeRoles(FinanceConstants.FinanceCheker)]
   
    public class InventoryDataArchiveController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        FinanceHelper _financeHelper = new FinanceHelper();
        AccountHelper _accountHelper = new AccountHelper();
        FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();


        // GET: Finance/Description


        public ActionResult Index()
        {
            ViewData["DataTypeList"] = Enum.GetValues(typeof(InventoryDataType)).Cast<InventoryDataType>().Select(v => v.ToString())
                .ToList()
                .Select(e => new
                {
                    Name = e,
                    Code = e
                });

            var years = _db.GLFiscalYears;
            ViewData["Years"] = years;
            var periods = _db.GLPeriods;
            ViewData["Periods"] = periods;

            return View();
        }

        private List<InventoryDataArchiveVm> GetArchiveData(string dataType,string year,string period)
        {
            List<InventoryDataArchiveVm> chekerVms = new List<InventoryDataArchiveVm>();

            if (!String.IsNullOrEmpty(dataType)&& !String.IsNullOrEmpty(year) && !String.IsNullOrEmpty(period))
            {
                #region Unsplitted Journal
                var cheker = new InventoryDataArchiveVm
                {
                    Type = InventoryDataType.BinCard,
                    GlFiscalYearId = int.Parse(year),
                    GLPeriodId = int.Parse(period)
                };
                chekerVms.Add(cheker);

                #endregion

            }

            return chekerVms;
        }

        public ActionResult InventoryDataArchive_Read([DataSourceRequest]DataSourceRequest request, string type,string year,string period)
        {

            IQueryable<InventoryDataArchiveVm> InventoryDataArchiveVms = GetArchiveData(type,year,period).AsQueryable();
            DataSourceResult result = InventoryDataArchiveVms.ToDataSourceResult(request, InventoryDataArchiveVm => new
            {
                Type = InventoryDataArchiveVm.Type,
                GlFiscalYearId = InventoryDataArchiveVm.GlFiscalYearId,
                GLPeriodId = InventoryDataArchiveVm.GLPeriodId
            });

            return Json(result);
        }

        public JsonResult Archive(string type, string year,string period)
        {
            if (!String.IsNullOrEmpty(type) && !String.IsNullOrEmpty(year) && !String.IsNullOrEmpty(period))
            {
                InventoryDataType datatype = (InventoryDataType)(int.Parse(type));
                var yearobj = _db.GLFiscalYears.Find(int.Parse(year));
                var periodobj = _db.GLPeriods.Find(int.Parse(period));

                switch (datatype)
                {                    
                    case InventoryDataType.BinCard:
                        #region archive bincard data                       

                        #region select
                        var bincards = _db.BinCardLines.Where(x => x.Date <= periodobj.DateTo).ToList();
                        #endregion
                        
                        foreach(var card in bincards)
                        {
                            #region copy
                            if ((card.ReferenceDoc.Contains("Count") || card.TransactionType != BinCardTransactionTypes.Count) && card.Date == periodobj.DateTo)
                                continue;
                            var exist = _db.BinCardLineArchive.Where(x => x.BinCardLineID == card.BinCardLineID).FirstOrDefault();
                            if (exist==null)
                            {
                                var binCardLine = new BinCardLineArchive
                                {
                                    StoreItemAssignmentID = card.StoreItemAssignmentID,
                                    BinCardLineID = card.BinCardLineID,
                                    ReferenceDoc = card.ReferenceDoc,
                                    Date = card.Date,
                                    ReceivedQTY = card.ReceivedQTY,
                                    ReceivedUPrice = card.ReceivedUPrice,
                                    ReceivedTPrice = card.ReceivedTPrice,
                                    BalanceQTY = card.BalanceQTY,
                                    BalanceUPrice = card.BalanceUPrice,
                                    BalanceTPrice = card.BalanceTPrice,
                                    StoreCode = card.StoreCode,
                                    GLPeriodId = card.GLPeriodId,
                                    TransactionType = BinCardTransactionTypes.Issue,
                                    StatusType = BinCardTransactionStatus.Void,
                                };
                                _db.BinCardLineArchive.Add(binCardLine);
                            }
                            #endregion
                            #region delete
                            _db.BinCardLines.Attach(card);
                            _db.BinCardLines.Remove(card);
                            _db.SaveChanges();
                            #endregion
                        }

                        #endregion
                        break;
                   
                }
            }

            var formattedData = new
            {
                Message = "Operation completed.",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);

        }


       
    }
}