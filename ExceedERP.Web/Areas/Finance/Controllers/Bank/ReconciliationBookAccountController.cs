using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Finance.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Finance;
using ExceedERP.Helpers.Inventory;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Finance.Controllers.Bank
{
    public class ReconciliationBookAccountController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();
        readonly AccountHelper _accountHelper = new AccountHelper();
        private readonly FiscalYearHelper _fiscalYearHelper = new FiscalYearHelper();

        public ActionResult Index()
        {
            var bank = _db.BankAccounts.ToList();
            ViewData["BankList"] = bank
                  .Select(e => new BankAccount
                  {
                      AccountNumber = e.AccountNumber,
                      AccountName = e.AccountNumber + "-" + e.AccountName
                  })
                  .OrderBy(e => e.AccountName);
            return View();
        }

        
        public ActionResult ReconcilationBookAccounts_Read([DataSourceRequest]DataSourceRequest request,string accountnumber)
        {
            AccountHelper _accountHelper = new AccountHelper();
            List<ReconciliationBookAccount> reconlist = new List<ReconciliationBookAccount>();

            IQueryable<ReconciliationBookAccount> reconcilationbookaccounts = _db.ReconcilationBookAccounts;
            if (!String.IsNullOrWhiteSpace(accountnumber))
            {
                var bank = _db.BankAccounts.FirstOrDefault(x => x.AccountNumber == accountnumber);
                if (bank != null)
                {
                    //string banksubaccount = _accountHelper.GetSubAccountSegment(bank.AccountCode);
                    //reconcilationbookaccounts =_db.ReconcilationBookAccounts.Where(x =>x.AccountCode.Contains(banksubaccount));

                    //new 
                    string account = _accountHelper.GetNaturalAccountSegment(bank.AccountCode);
                    string subaccount = _accountHelper.GetSubAccountSegment(bank.AccountCode);
                    reconcilationbookaccounts = reconcilationbookaccounts.Where(x => x.AccountSegment == account && x.SubAccountSegment == subaccount).AsQueryable();
                }
            }

            DataSourceResult result = reconcilationbookaccounts.ToDataSourceResult(request, reconciliationBookAccount => new {
                ReconciliationBookAccountId = reconciliationBookAccount.ReconciliationBookAccountId,
                AccountCode = reconciliationBookAccount.AccountCode,
                Type = reconciliationBookAccount.Type,
                Reference = reconciliationBookAccount.Reference,
                ReferenceDoc = reconciliationBookAccount.ReferenceDoc,
                Debit = reconciliationBookAccount.Debit,
                Credit = reconciliationBookAccount.Credit,
                ExRate = reconciliationBookAccount.ExRate,
                DebitNonFun = reconciliationBookAccount.DebitNonFun,
                CreditNonFun = reconciliationBookAccount.CreditNonFun,
                Date = reconciliationBookAccount.Date,
                Reconciled = reconciliationBookAccount.Reconciled,
                ReconciledDate = reconciliationBookAccount.ReconciledDate,
                RecordType = reconciliationBookAccount.RecordType,
                Check = reconciliationBookAccount.Check,
                Source = reconciliationBookAccount.Source,
                SourceID = reconciliationBookAccount.SourceID,
                AccountSegment = reconciliationBookAccount.AccountSegment,
                BranchSegment = reconciliationBookAccount.BranchSegment,
                CostCenterSegment = reconciliationBookAccount.CostCenterSegment,
                SubAccountSegment = reconciliationBookAccount.SubAccountSegment,
                ChannelSegment = reconciliationBookAccount.ChannelSegment,
                BusinessUnitSegment = reconciliationBookAccount.BusinessUnitSegment,
                OrgBranchName = reconciliationBookAccount.OrgBranchName,
                DateCreated = reconciliationBookAccount.DateCreated,
                LastModified = reconciliationBookAccount.LastModified,
                CreatedBy = reconciliationBookAccount.CreatedBy,
                ModifiedBy = reconciliationBookAccount.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult UnreconciledBookAccounts_Read([DataSourceRequest]DataSourceRequest request, int id, decimal? amount)
        {
            ReconciliationSchedule reconciliationSchedule = _db.ReconciliationSchedules.Find(id);
            if (reconciliationSchedule != null)
            {
                //DateTime dt2 = _fiscalYearHelper.GetEndOfMonth(reconciliationSchedule.Date).AddDays(1); //old
                DateTime dt2 = _fiscalYearHelper.GetEndOfPeriod(reconciliationSchedule.Date).AddDays(1);//new
                var bankaccount =_db.BankAccounts.FirstOrDefault(x => x.AccountNumber == reconciliationSchedule.BankAccount);
                string account = _accountHelper.GetNaturalAccountSegment(bankaccount.AccountCode);
                string subaccount = _accountHelper.GetSubAccountSegment(bankaccount.AccountCode);

                if (bankaccount != null)
                {                   

                    var reconcilationbook = _db.ReconcilationBookAccounts
                                                                  //.Where(x => x.AccountCode == bankaccount.AccountCode)//old    
                                                                  .Where(x => x.AccountSegment == account && x.SubAccountSegment == subaccount)//neew
                                                                  .Where(x => x.Date < dt2)
                                                                  .Where(x => x.RecordType != RecordType.Beginning)
                                                                  .Where(x => x.Reconciled == false);
                   

                    IQueryable<ReconciliationBookAccount> reconcilationbookaccounts = reconcilationbook.AsQueryable();

                    //filter

                    DataSourceResult result = reconcilationbookaccounts.ToDataSourceResult(request,
                        reconciliationBookAccount => new
                        {
                           
                            ReconciliationBookAccountId = reconciliationBookAccount.ReconciliationBookAccountId,
                            AccountCode = reconciliationBookAccount.AccountCode,
                            Type = reconciliationBookAccount.Type,
                            Reference = reconciliationBookAccount.Reference,
                            ReferenceDoc = reconciliationBookAccount.ReferenceDoc,
                            Debit = reconciliationBookAccount.Debit,
                            Credit = reconciliationBookAccount.Credit,
                            ExRate = reconciliationBookAccount.ExRate,
                            DebitNonFun = reconciliationBookAccount.DebitNonFun,
                            CreditNonFun = reconciliationBookAccount.CreditNonFun,
                            Date = reconciliationBookAccount.Date,
                            Reconciled = reconciliationBookAccount.Reconciled,
                            ReconciledDate = reconciliationBookAccount.ReconciledDate,
                            RecordType = reconciliationBookAccount.RecordType,
                            Check = reconciliationBookAccount.Check,
                            Source = reconciliationBookAccount.Source,
                            SourceID = reconciliationBookAccount.SourceID,
                            AccountSegment = reconciliationBookAccount.AccountSegment,
                            BranchSegment = reconciliationBookAccount.BranchSegment,
                            CostCenterSegment = reconciliationBookAccount.CostCenterSegment,
                            SubAccountSegment = reconciliationBookAccount.SubAccountSegment,
                            ChannelSegment = reconciliationBookAccount.ChannelSegment,
                            BusinessUnitSegment = reconciliationBookAccount.BusinessUnitSegment,
                            OrgBranchName = reconciliationBookAccount.OrgBranchName,
                            DateCreated = reconciliationBookAccount.DateCreated,
                            LastModified = reconciliationBookAccount.LastModified,
                            CreatedBy = reconciliationBookAccount.CreatedBy,
                            ModifiedBy = reconciliationBookAccount.ModifiedBy
                        });
                    return Json(result);
                }
            }
            return null;

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UnreconciledBookAccounts_Create_Edit([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ReconciliationBookAccount> reconciliationBookAccounts , int id)
        {
            var reconciliationSchedule = _db.ReconciliationSchedules.Find(id);
            if (reconciliationSchedule == null)
            {
                ModelState.AddModelError("s", "Schdule not found.");
            }
            
            DateTime reconcileDate = _fiscalYearHelper.GetEndOfPeriod(reconciliationSchedule.Date).Date;

            var bookList = reconciliationBookAccounts.ToList();
            if (ModelState.IsValid)
            {
                
                foreach (var record in bookList)
                {
                    var entity = new ReconciliationBookAccount
                    {
                        ReconciliationBookAccountId = record.ReconciliationBookAccountId,
                        AccountCode = record.AccountCode,
                        Type = record.Type,
                        ReferenceDoc = record.ReferenceDoc,
                        Reference = record.Reference,
                        Debit = record.Debit,
                        Credit = record.Credit,
                        ExRate = record.ExRate,
                        DebitNonFun = record.DebitNonFun,
                        CreditNonFun = record.CreditNonFun,
                        Date = record.Date.Date,
                        RecordType = record.RecordType,
                        Reconciled = record.Reconciled,
                        ReconciledDate = record.Reconciled==true? reconcileDate : (DateTime?)null,
                        Source = record.Source,
                        SourceID = record.SourceID,
                        Check = record.Check,
                        AccountSegment = record.AccountSegment,
                        BranchSegment = record.BranchSegment,
                        CostCenterSegment = record.CostCenterSegment,
                        SubAccountSegment = record.SubAccountSegment,
                        ChannelSegment = record.ChannelSegment,
                        BusinessUnitSegment = record.BusinessUnitSegment,
                        OrgBranchName = record.OrgBranchName,
                        DateCreated = record.DateCreated,
                        LastModified = record.LastModified,
                        CreatedBy = record.CreatedBy,
                        ModifiedBy = record.ModifiedBy
                    };

                    _db.ReconcilationBookAccounts.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Bank reconciliation book record", "Success", User.Identity.Name, $"created book record {entity.AccountCode}  {entity.AdditionalDescription}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { bookList }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UnreconciledBookAccounts_Create([DataSourceRequest]DataSourceRequest request, ReconciliationBookAccount reconciliationBookAccount,int id)
        {
            reconciliationBookAccount.Type = "Manual";
            var record = _db.ReconciliationSchedules.Find(id);
            if (record != null)
            {
                var accountcode = _db.BankAccounts
                    .FirstOrDefault(x => x.AccountNumber == record.BankAccount);

               
                    if (accountcode != null)
                    {
                        var entity = new ReconciliationBookAccount
                        {
                           
                            AccountCode =accountcode.AccountCode,
                            Type = reconciliationBookAccount.Type,
                            Reference = reconciliationBookAccount.Reference,
                            Debit = reconciliationBookAccount.Debit,
                            Credit = reconciliationBookAccount.Credit,
                            ExRate = reconciliationBookAccount.ExRate,
                            DebitNonFun = reconciliationBookAccount.DebitNonFun,
                            CreditNonFun = reconciliationBookAccount.CreditNonFun,
                            Date = reconciliationBookAccount.Date.Date,
                            RecordType = RecordType.Beginning,
                            Reconciled = false,
                            ReconciledDate = reconciliationBookAccount.ReconciledDate,
                            Check = false,
                            AccountSegment = reconciliationBookAccount.AccountSegment,
                            BranchSegment = reconciliationBookAccount.BranchSegment,
                            CostCenterSegment = reconciliationBookAccount.CostCenterSegment,
                            SubAccountSegment = reconciliationBookAccount.SubAccountSegment,
                            ChannelSegment = reconciliationBookAccount.ChannelSegment,
                            BusinessUnitSegment = reconciliationBookAccount.BusinessUnitSegment,
                            OrgBranchName = reconciliationBookAccount.OrgBranchName,
                            DateCreated = reconciliationBookAccount.DateCreated,
                            LastModified = reconciliationBookAccount.LastModified,
                            CreatedBy = reconciliationBookAccount.CreatedBy,
                            ModifiedBy = reconciliationBookAccount.ModifiedBy
                        };

                        _db.ReconcilationBookAccounts.Add(entity);
                        _db.SaveChanges();
                        reconciliationBookAccount.ReconciliationBookAccountId = entity.ReconciliationBookAccountId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Bank reconciliation book record", "Success", User.Identity.Name, $"updated book record {entity.AccountCode}  {entity.AdditionalDescription}", Modules.Finance);
                    #endregion

                }
            }

            return Json(new[] { reconciliationBookAccount }.ToDataSourceResult(request, ModelState));
        }
        #region reconciled

        public ActionResult ReconciledBookAccounts_Read([DataSourceRequest]DataSourceRequest request, int id, decimal? amount)
        {
            ReconciliationSchedule reconciliationSchedule = _db.ReconciliationSchedules.Find(id);
            if (reconciliationSchedule != null)
            {
                DateTime reconcileDate = _fiscalYearHelper.GetEndOfPeriod(reconciliationSchedule.Date).Date;
                var bankaccount = _db.BankAccounts.FirstOrDefault(x => x.AccountNumber == reconciliationSchedule.BankAccount);
                string account = _accountHelper.GetNaturalAccountSegment(bankaccount.AccountCode);
                string subaccount = _accountHelper.GetSubAccountSegment(bankaccount.AccountCode);

                if (bankaccount != null)
                {

                    var reconcilationbook = _db.ReconcilationBookAccounts
                                                                  
                                                                  .Where(x => x.AccountSegment == account && x.SubAccountSegment == subaccount)//neew
                                                                  .Where(x => x.ReconciledDate ==reconcileDate)
                                                                  .Where(x => x.RecordType != RecordType.Beginning)
                                                                  .Where(x => x.Reconciled == true);


                    IQueryable<ReconciliationBookAccount> reconcilationbookaccounts = reconcilationbook.AsQueryable();

                    //filter

                    DataSourceResult result = reconcilationbookaccounts.ToDataSourceResult(request,
                        reconciliationBookAccount => new
                        {

                            ReconciliationBookAccountId = reconciliationBookAccount.ReconciliationBookAccountId,
                            AccountCode = reconciliationBookAccount.AccountCode,
                            Type = reconciliationBookAccount.Type,
                            Reference = reconciliationBookAccount.Reference,
                            ReferenceDoc = reconciliationBookAccount.ReferenceDoc,
                            Debit = reconciliationBookAccount.Debit,
                            Credit = reconciliationBookAccount.Credit,
                            ExRate = reconciliationBookAccount.ExRate,
                            DebitNonFun = reconciliationBookAccount.DebitNonFun,
                            CreditNonFun = reconciliationBookAccount.CreditNonFun,
                            Date = reconciliationBookAccount.Date,
                            Reconciled = reconciliationBookAccount.Reconciled,
                            ReconciledDate = reconciliationBookAccount.ReconciledDate,
                            RecordType = reconciliationBookAccount.RecordType,
                            Check = reconciliationBookAccount.Check,
                            Source = reconciliationBookAccount.Source,
                            SourceID = reconciliationBookAccount.SourceID,
                            AccountSegment = reconciliationBookAccount.AccountSegment,
                            BranchSegment = reconciliationBookAccount.BranchSegment,
                            CostCenterSegment = reconciliationBookAccount.CostCenterSegment,
                            SubAccountSegment = reconciliationBookAccount.SubAccountSegment,
                            ChannelSegment = reconciliationBookAccount.ChannelSegment,
                            BusinessUnitSegment = reconciliationBookAccount.BusinessUnitSegment,
                            OrgBranchName = reconciliationBookAccount.OrgBranchName,
                            DateCreated = reconciliationBookAccount.DateCreated,
                            LastModified = reconciliationBookAccount.LastModified,
                            CreatedBy = reconciliationBookAccount.CreatedBy,
                            ModifiedBy = reconciliationBookAccount.ModifiedBy
                        });
                    return Json(result);
                }
            }
            return null;

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciledBookAccounts_Create_Edit([DataSourceRequest]DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ReconciliationBookAccount> reconciliationBookAccounts, int id)
        {
            var reconciliationSchedule = _db.ReconciliationSchedules.Find(id);
            if (reconciliationSchedule == null)
            {
                ModelState.AddModelError("s", "Schdule not found.");
            }

            DateTime reconcileDate = _fiscalYearHelper.GetEndOfPeriod(reconciliationSchedule.Date).Date;

            var bookList = reconciliationBookAccounts.ToList();
            if (ModelState.IsValid)
            {

                foreach (var record in bookList)
                {
                    var entity = new ReconciliationBookAccount
                    {
                        ReconciliationBookAccountId = record.ReconciliationBookAccountId,
                        AccountCode = record.AccountCode,
                        Type = record.Type,
                        ReferenceDoc = record.ReferenceDoc,
                        Reference = record.Reference,
                        Debit = record.Debit,
                        Credit = record.Credit,
                        ExRate = record.ExRate,
                        DebitNonFun = record.DebitNonFun,
                        CreditNonFun = record.CreditNonFun,
                        Date = record.Date.Date,
                        RecordType = record.RecordType,
                        Reconciled = false,
                        ReconciledDate = null,
                        Source = record.Source,
                        SourceID = record.SourceID,
                        Check = record.Check,
                        AccountSegment = record.AccountSegment,
                        BranchSegment = record.BranchSegment,
                        CostCenterSegment = record.CostCenterSegment,
                        SubAccountSegment = record.SubAccountSegment,
                        ChannelSegment = record.ChannelSegment,
                        BusinessUnitSegment = record.BusinessUnitSegment,
                        OrgBranchName = record.OrgBranchName,
                        LastModified = DateTime.Now,
                        ModifiedBy = User.Identity.Name
                    };

                    _db.ReconcilationBookAccounts.Attach(entity);
                    _db.Entry(entity).State = EntityState.Modified;
                    _db.SaveChanges();

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Bank reconciliation book record", "Success", User.Identity.Name, $"unreconciled book record {entity.AccountCode}  {entity.AdditionalDescription}", Modules.Finance);
                    #endregion
                }
            }

            return Json(new[] { bookList }.ToDataSourceResult(request, ModelState));
        }

        #endregion

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconcilationBookAccounts_Update([DataSourceRequest]DataSourceRequest request, ReconciliationBookAccount reconciliationBookAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBookAccount
                {
                  
                    ReconciliationBookAccountId = reconciliationBookAccount.ReconciliationBookAccountId,
                    AccountCode = reconciliationBookAccount.AccountCode,
                    Type = reconciliationBookAccount.Type,
                    Reference = reconciliationBookAccount.Reference,
                    ReferenceDoc = reconciliationBookAccount.ReferenceDoc,
                    Debit = reconciliationBookAccount.Debit,
                    Credit = reconciliationBookAccount.Credit,
                    ExRate = reconciliationBookAccount.ExRate,
                    DebitNonFun = reconciliationBookAccount.DebitNonFun,
                    CreditNonFun = reconciliationBookAccount.CreditNonFun,
                    Date = reconciliationBookAccount.Date.Date,
                    RecordType = reconciliationBookAccount.RecordType,
                    Reconciled = reconciliationBookAccount.Reconciled,
                    ReconciledDate = reconciliationBookAccount.ReconciledDate,
                    Check = reconciliationBookAccount.Check,
                    AccountSegment = reconciliationBookAccount.AccountSegment,
                    BranchSegment = reconciliationBookAccount.BranchSegment,
                    CostCenterSegment = reconciliationBookAccount.CostCenterSegment,
                    SubAccountSegment = reconciliationBookAccount.SubAccountSegment,
                    ChannelSegment = reconciliationBookAccount.ChannelSegment,
                    BusinessUnitSegment = reconciliationBookAccount.BusinessUnitSegment,
                    OrgBranchName = reconciliationBookAccount.OrgBranchName,
                    DateCreated = reconciliationBookAccount.DateCreated,
                    LastModified = reconciliationBookAccount.LastModified,
                    CreatedBy = reconciliationBookAccount.CreatedBy,
                    ModifiedBy = reconciliationBookAccount.ModifiedBy
                };

                _db.ReconcilationBookAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { reconciliationBookAccount }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReconciliationBookAccounts_Destroy([DataSourceRequest]DataSourceRequest request, ReconciliationBookAccount reconciliationBookAccount)
        {
            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBankAccount
                {
                   
                    Type = reconciliationBookAccount.Type,
                    Reference = reconciliationBookAccount.Reference,
                    Debit = reconciliationBookAccount.Debit,
                    Credit = reconciliationBookAccount.Credit,
                    Date = reconciliationBookAccount.Date,
                    Reconciled = reconciliationBookAccount.Reconciled,
                    ReconciledDate = reconciliationBookAccount.ReconciledDate,
                    Check = reconciliationBookAccount.Check,
                    OrgBranchName = reconciliationBookAccount.OrgBranchName,
                    DateCreated = reconciliationBookAccount.DateCreated,
                    LastModified = reconciliationBookAccount.LastModified,
                    CreatedBy = reconciliationBookAccount.CreatedBy,
                    ModifiedBy = reconciliationBookAccount.ModifiedBy
                };

                _db.ReconcilationBankAccounts.Attach(entity);
                _db.ReconcilationBankAccounts.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted reconciliation bank " + reconciliationBookAccount.ReconciliationBookAccountId + " " + reconciliationBookAccount.AccountCode, Modules.Finance);
                #endregion
            }

            return Json(new[] { reconciliationBookAccount }.ToDataSourceResult(request, ModelState));
        }



        //[Authorize(Roles = Bank + "," + Bank_Admin)]
        public JsonResult Change(int id)
        {
            ReconciliationBookAccount reconciliationBookAccount = _db.ReconcilationBookAccounts.Find(id);
            if (reconciliationBookAccount != null && reconciliationBookAccount.Reconciled)
            {
                reconciliationBookAccount.Reconciled = false;
                _db.Entry(reconciliationBookAccount).State = EntityState.Modified;
                _db.SaveChanges();

                var formattedData = new
                {
                    Message = "Operation complete.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

          
            var formattedData1 = new
            {
                Message = "It is already unreconciled.",
                Status = "OK"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);
        }

        //beginning only

        public ActionResult BeginningBookAccounts_Read([DataSourceRequest]DataSourceRequest request, int id, decimal? amount)
        {
            ReconciliationSchedule reconciliationSchedule = _db.ReconciliationSchedules.Find(id);
            if (reconciliationSchedule != null)
            {
                //DateTime dt2 = _fiscalYearHelper.GetEndOfMonth(reconciliationSchedule.Date).AddDays(1);//old
                DateTime dt2 = _fiscalYearHelper.GetEndOfPeriod(reconciliationSchedule.Date).AddDays(1);//new
                var bankaccount = _db.BankAccounts.FirstOrDefault(x => x.AccountNumber == reconciliationSchedule.BankAccount);
                string account = _accountHelper.GetNaturalAccountSegment(bankaccount.AccountCode);
                string subaccount = _accountHelper.GetSubAccountSegment(bankaccount.AccountCode);

                if (bankaccount != null)
                {
                    IQueryable<ReconciliationBookAccount> reconcilationbookaccounts = _db.ReconcilationBookAccounts
                                                                   //.Where(x => x.AccountCode == bankaccount.AccountCode)
                                                                  .Where(x => x.AccountSegment == account && x.SubAccountSegment == subaccount)//neew
                                                                  .Where(x => x.Date < dt2)
                                                                  .Where(x => x.RecordType ==RecordType.Beginning)
                                                                  .Where(x => x.Reconciled == false);

                    DataSourceResult result = reconcilationbookaccounts.ToDataSourceResult(request,
                        reconciliationBookAccount => new
                        {
                            
                            ReconciliationBookAccountId = reconciliationBookAccount.ReconciliationBookAccountId,
                            AccountCode = reconciliationBookAccount.AccountCode,
                            Type = reconciliationBookAccount.Type,
                            Reference = reconciliationBookAccount.Reference,
                            ReferenceDoc = reconciliationBookAccount.ReferenceDoc,
                            Debit = reconciliationBookAccount.Debit,
                            Credit = reconciliationBookAccount.Credit,
                            ExRate = reconciliationBookAccount.ExRate,
                            DebitNonFun = reconciliationBookAccount.DebitNonFun,
                            CreditNonFun = reconciliationBookAccount.CreditNonFun,
                            Date = reconciliationBookAccount.Date,
                            Reconciled = reconciliationBookAccount.Reconciled,
                            ReconciledDate = reconciliationBookAccount.ReconciledDate,
                            RecordType = reconciliationBookAccount.RecordType,
                            Check = reconciliationBookAccount.Check,
                            Source = reconciliationBookAccount.Source,
                            SourceID = reconciliationBookAccount.SourceID,
                            AccountSegment = reconciliationBookAccount.AccountSegment,
                            BranchSegment = reconciliationBookAccount.BranchSegment,
                            CostCenterSegment = reconciliationBookAccount.CostCenterSegment,
                            SubAccountSegment = reconciliationBookAccount.SubAccountSegment,
                            ChannelSegment = reconciliationBookAccount.ChannelSegment,
                            BusinessUnitSegment = reconciliationBookAccount.BusinessUnitSegment,
                            OrgBranchName = reconciliationBookAccount.OrgBranchName,
                            DateCreated = reconciliationBookAccount.DateCreated,
                            LastModified = reconciliationBookAccount.LastModified,
                            CreatedBy = reconciliationBookAccount.CreatedBy,
                            ModifiedBy = reconciliationBookAccount.ModifiedBy
                        });
                    return Json(result);
                }
            }
            return null;

        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BeginningBookAccounts_Create([DataSourceRequest]DataSourceRequest request, ReconciliationBookAccount reconciliationBookAccount, int id)
        {
            ModelState.Clear();

            if(reconciliationBookAccount.Debit!=0&& reconciliationBookAccount.Credit != 0)
                ModelState.AddModelError("both","Both debit and credit can not have value at same time.");
            if (reconciliationBookAccount.Debit == 0 && reconciliationBookAccount.Credit == 0)
                ModelState.AddModelError("both", "Both debit and credit can not have zero value at same time.");
            if (reconciliationBookAccount.Debit < 0 && reconciliationBookAccount.Credit < 0)
                ModelState.AddModelError("both", "Debit and credit can not have negative value.");

            if (String.IsNullOrWhiteSpace(reconciliationBookAccount.Reference))
                ModelState.AddModelError("ref", "Please add reference.");

            reconciliationBookAccount.Type = "Manual";
            var record = _db.ReconciliationSchedules.Find(id);
            if (record != null)
            {
                var accountcode = _db.BankAccounts
                    .FirstOrDefault(x => x.AccountNumber == record.BankAccount);


                if (accountcode != null)
                {
                    var entity = new ReconciliationBookAccount
                    {
                        
                        AccountCode = accountcode.AccountCode,
                        Type = reconciliationBookAccount.Type,
                        Reference = reconciliationBookAccount.Reference,
                        ReferenceDoc = reconciliationBookAccount.ReferenceDoc,
                        Debit = reconciliationBookAccount.Debit,
                        Credit = reconciliationBookAccount.Credit,
                        ExRate = reconciliationBookAccount.ExRate,
                        DebitNonFun = reconciliationBookAccount.DebitNonFun,
                        CreditNonFun = reconciliationBookAccount.CreditNonFun,
                        Date = reconciliationBookAccount.Date,
                        RecordType = RecordType.Beginning,
                        Reconciled = false,
                        Check = false,
                        AccountSegment = _accountHelper.GetNaturalAccountSegment(accountcode.AccountCode),
                        SubAccountSegment = _accountHelper.GetSubAccountSegment(accountcode.AccountCode),
                        BusinessUnitSegment = _accountHelper.GetBusinessUnitSegment(accountcode.AccountCode),
                        OrgBranchName = reconciliationBookAccount.OrgBranchName,
                        DateCreated = reconciliationBookAccount.DateCreated,
                        LastModified = reconciliationBookAccount.LastModified,
                        CreatedBy = reconciliationBookAccount.CreatedBy,
                        ModifiedBy = reconciliationBookAccount.ModifiedBy
                    };

                    _db.ReconcilationBookAccounts.Add(entity);
                    _db.SaveChanges();
                    reconciliationBookAccount.ReconciliationBookAccountId = entity.ReconciliationBookAccountId;
                    AccountHelper helper = new AccountHelper();
                    helper.SplitBookRecord(entity.ReconciliationBookAccountId);

                }
            }

            return Json(new[] { reconciliationBookAccount }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult BeginningBookAccounts_Update([DataSourceRequest]DataSourceRequest request, ReconciliationBookAccount reconciliationBookAccount)
        {
           

            if (reconciliationBookAccount.Debit != 0 && reconciliationBookAccount.Credit != 0)
                ModelState.AddModelError("both", "Both debit and credit can not have value at same time.");
            if (reconciliationBookAccount.Debit == 0 && reconciliationBookAccount.Credit == 0)
                ModelState.AddModelError("both", "Both debit and credit can not have zero value at same time.");
            if (reconciliationBookAccount.Debit < 0 && reconciliationBookAccount.Credit < 0)
                ModelState.AddModelError("both", "Debit and credit can not have negative value.");

            if (String.IsNullOrWhiteSpace(reconciliationBookAccount.Reference))
                ModelState.AddModelError("ref", "Please add reference.");

            if (ModelState.IsValid)
            {
                var entity = new ReconciliationBookAccount
                {
                    
                    ReconciliationBookAccountId = reconciliationBookAccount.ReconciliationBookAccountId,
                    AccountCode = reconciliationBookAccount.AccountCode,
                    Type = reconciliationBookAccount.Type,
                    Reference = reconciliationBookAccount.Reference,
                    ReferenceDoc = reconciliationBookAccount.ReferenceDoc,
                    Debit = reconciliationBookAccount.Debit,
                    Credit = reconciliationBookAccount.Credit,
                    ExRate = reconciliationBookAccount.ExRate,
                    DebitNonFun = reconciliationBookAccount.DebitNonFun,
                    CreditNonFun = reconciliationBookAccount.CreditNonFun,
                    Date = reconciliationBookAccount.Date,
                    RecordType = reconciliationBookAccount.RecordType,
                    Reconciled = reconciliationBookAccount.Reconciled,
                    ReconciledDate = reconciliationBookAccount.ReconciledDate,
                    Check = reconciliationBookAccount.Check,
                    AccountSegment = reconciliationBookAccount.AccountSegment,
                    BranchSegment = reconciliationBookAccount.BranchSegment,
                    CostCenterSegment = reconciliationBookAccount.CostCenterSegment,
                    SubAccountSegment = reconciliationBookAccount.SubAccountSegment,
                    ChannelSegment = reconciliationBookAccount.ChannelSegment,
                    OrgBranchName = reconciliationBookAccount.OrgBranchName,
                    DateCreated = reconciliationBookAccount.DateCreated,
                    LastModified = reconciliationBookAccount.LastModified,
                    CreatedBy = reconciliationBookAccount.CreatedBy,
                    ModifiedBy = reconciliationBookAccount.ModifiedBy
                };

                _db.ReconcilationBookAccounts.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { reconciliationBookAccount }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
