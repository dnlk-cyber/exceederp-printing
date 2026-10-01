using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ExceedERP.Core.Domain.APTS;
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Core.Domain.Budget.GAAP;
using ExceedERP.Core.Domain.Budget.Task;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.InfoDesk;

using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Finance.AP.PaymentRequest;
using ExceedERP.Core.Domain.Finance.AP.PettyCash;
using ExceedERP.Core.Domain.Finance.AP.Travel;

using ExceedERP.Core.Domain.Finance.AR;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Finance.Report;
using ExceedERP.Core.Domain.Finance.Report.GAAP;

using ExceedERP.Core.Domain.Finance.Report.IFRS;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.ForeignPurchase;

using ExceedERP.Core.Domain.Procurement.Supplier;

using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.Core.Domain.Finance.Budget;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;

using ExceedERP.Core.Domain.FixedAsset;
using ExceedERP.Core.Domain.FixedAsset.Counts;
using ExceedERP.Core.Domain.FixedAsset.Depreciation;
using ExceedERP.Core.Domain.FixedAsset.Issues;
using ExceedERP.Core.Domain.FixedAsset.Receives;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FixedAsset.Requisitions;
using ExceedERP.Core.Domain.FixedAsset.Returns;
using ExceedERP.Core.Domain.FixedAsset.Setting;
using ExceedERP.Core.Domain.FixedAsset.Transfers;
using ExceedERP.Core.Domain.FixedAsset.UserCard;
using ExceedERP.Core.Domain.FleetManagement.BatteryManagement;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;

using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;

using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.Core.Domain.HRM.HRMPlan;
using ExceedERP.Core.Domain.HRM.Letters;
using ExceedERP.Core.Domain.HRM.Payroll.Setting;
using ExceedERP.Core.Domain.HRM.PerformanceManagment;
using ExceedERP.Core.Domain.HRM.Prohibition;
using ExceedERP.Core.Domain.HRM.SalaryStructure;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.Core.Domain.Procurement;
using ExceedERP.Core.Domain.Procurement.Contract;
using ExceedERP.Core.Domain.Procurement.CPOManagement;
using ExceedERP.Core.Domain.Procurement.Guarantee;
using ExceedERP.Core.Domain.Procurement.LC;
using ExceedERP.Core.Domain.Procurement.Need;
using ExceedERP.Core.Domain.Procurement.Plan;
using ExceedERP.Core.Domain.Procurement.Proforma;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.Core.Domain.Procurement.Tender;
using Configuration = ExceedERP.Core.Domain.Common.HRM.Configuration;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.HRM.Templates;
using ExceedERP.Core.Domain.Inventory.ArchiveModel;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Finance.Bank;
using ExceedERP.Core.Domain.Procurement.PaymentRequest;
using ExceedERP.Core.Domain.Finance.AP.FundTransfer;
using ExceedERP.Core.Domain.printing.PrintingEstimation;
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.Core.Domain.Printing.PrintingEstimation;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using ExceedERP.Core.Domain.Manufacturing;
using ExceedERP.Core.Domain.Manufacturing.Production;
using ExceedERP.Core.Domain.HRM.Severance;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.Core.Domain.ArchiveManagement.DepartmentToStaff;
using ExceedERP.Core.Domain.ArchiveManagement.DepartmentToDepartment;
using ExceedERP.Core.Domain.ArchiveManagement.Institution;
using ExceedERP.Core.Domain.ArchiveManagement.Employee;
using ExceedERP.Core.Domain.ArchiveManagement.Setting;
using ExceedERP.Core.Domain.Archive;
using ExceedERP.Core.Domain.Printing;
using ExceedERP.Core.Domain.FleetManagement.RentalPayment;
using ExceedERP.Core.Domain.FleetManagement.FleetInsurances;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.Maintenance.Settings;
using ExceedERP.Core.Domain.Planning.Project;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.Core.Domain.Planning.BusinessPlanDevelopment;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Insurance.InsuranceNew;
using ExceedERP.Core.Domain.Health;
using ExceedERP.Core.Domain.Insurance;
using ExceedERP.Core.Domain.Insurance.Setting;
using ExceedERP.Core.Domain.InfoDesk.License;
using ExceedERP.Core.Domain.InfoDesk.Support;
using ExceedERP.Core.Domain.JobCosting;
using ExceedERP.Core.Domain.JobCosting.Setting;
using static ExceedERP.DataAccess.Context.JobCostingEntityConfiguration;
using ExceedERP.Core.Domain.Manufacturing.JobCosting.Setting;
using ExceedERP.Core.Domain.Manufacturing.JobCosting;
using ExceedERP.Core.Domain.Printing.PrintingProduction;
using ExceedERP.Core.Domain.InfoDesk.ChangeManagement;
using ExceedERP.Core.Domain.CRM.TenderManagement;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Core.Domain.HRM.Payroll.BackPays;

namespace ExceedERP.DataAccess.Context
{
    public class ExceedDbContext : DbContext
    {
        public ExceedDbContext()
            : base("ExceedDbContext")
        {
        }


        #region global
        //public DbSet<Core.Domain.Global.OrgBranchUserMapping> OrgBranchUserMappings { get; set; }
        public DbSet<Core.Domain.Global.UserBusinessUnitMap> UserBusinessUnitMaps { get; set; }
        public DbSet<Core.Domain.Global.GlobalUserAccessLog> GlobalUserAccessLog { get; set; }
        public DbSet<Core.Domain.Global.GlobalDeleteLog> GlobalDeleteLogs { get; set; }
        public DbSet<Core.Domain.Global.GlobalOperationLog> GlobalOperationLogs { get; set; }
        public DbSet<Core.Domain.Global.GlobalExportLog> GlobalExportLog { get; set; }
        public DbSet<Core.Domain.Global.GlobalBranchsetup> GlobalBranchsetups { get; set; }
        public DbSet<Core.Domain.Global.GlobalNotification> GlobalNotifications { get; set; }
        public DbSet<DefaultBranch> DefaultBranches { get; set; }
        //public System.Data.Entity.DbSet<Branch> Branches { get; set; }
        //public DbSet<OrgBranch> OrgBranches { get; set; }
        public DbSet<OrgBranchUserMapping> OrgBranchUserMappings { get; set; }
        #endregion




        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {


            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //Configure default schema
            modelBuilder.HasDefaultSchema("EXCEED");
            //decimal place
            //modelBuilder.Conventions.Remove<DecimalPropertyConvention>();
            //modelBuilder.Conventions.Add(new DecimalPropertyConvention(18, 9));
            // other code 
            #region inventory

            // unique con
            //modelBuilder.Entity<StoreItemAssignment>().HasIndex(u => new { u.StoreCode, u.ItemCode }).IsUnique(true);
            //modelBuilder.Entity<PropertyCountItem>().HasIndex(u => new { u.ItemCode, u.PropertyCountId }).IsUnique(true);

            modelBuilder.Entity<StoreRequisitionItem>().Property(e => e.RequestedQuantity).HasPrecision(18, 4);

            modelBuilder.Entity<StoreRequisitionItem>().Property(e => e.ApprovedQuantity).HasPrecision(18, 4);

            modelBuilder.Entity<StoreRequisitionItem>().Property(e => e.IssuedQuantity).HasPrecision(18, 4);
            modelBuilder.Entity<StoreRequisitionItem>().Property(e => e.PurchaseQuantity).HasPrecision(18, 4);




            modelBuilder.Entity<GoodReceiveItem>().Property(e => e.Quantity).HasPrecision(18, 4);

            modelBuilder.Entity<GoodReceiveItem>().Property(e => e.UnitPrice).HasPrecision(18, 6);

            modelBuilder.Entity<GoodReceiveItem>().Property(e => e.SubTotal).HasPrecision(18, 6);

            modelBuilder.Entity<GoodReceiveItem>().Property(e => e.TaxAmount).HasPrecision(18, 6);

            modelBuilder.Entity<GoodReceiveItem>().Property(e => e.Total).HasPrecision(18, 6);



            modelBuilder.Entity<IssueItem>().Property(e => e.RequestedQuantity).HasPrecision(18, 4);

            modelBuilder.Entity<IssueItem>().Property(e => e.ApprovedQuantity).HasPrecision(18, 4);

            modelBuilder.Entity<IssueItem>().Property(e => e.IssuedQuantity).HasPrecision(18, 4);

            modelBuilder.Entity<IssueItem>().Property(e => e.UnitPrice).HasPrecision(18, 6);

            modelBuilder.Entity<IssueItem>().Property(e => e.AdditionalCost).HasPrecision(18, 4);

            modelBuilder.Entity<IssueItem>().Property(e => e.Total).HasPrecision(18, 6);



            modelBuilder.Entity<ItemTransferItem>().Property(e => e.RequestedQuantity).HasPrecision(18, 4);

            modelBuilder.Entity<ItemTransferItem>().Property(e => e.ShippedQuantity).HasPrecision(18, 4);

            modelBuilder.Entity<ItemTransferItem>().Property(e => e.ReceivedQuantity).HasPrecision(18, 4);

            modelBuilder.Entity<ItemTransferItem>().Property(e => e.UPrice).HasPrecision(18, 4);

            modelBuilder.Entity<ItemTransferItem>().Property(e => e.TPrice).HasPrecision(18, 4);


            modelBuilder.Entity<ItemTransferItemReceive>().Property(e => e.RequestedQuantity).HasPrecision(18, 4);

            modelBuilder.Entity<ItemTransferItemReceive>().Property(e => e.ShippedQuantity).HasPrecision(18, 4);

            modelBuilder.Entity<ItemTransferItemReceive>().Property(e => e.ReceivedQuantity).HasPrecision(18, 4);

            modelBuilder.Entity<ItemTransferItemReceive>().Property(e => e.UPrice).HasPrecision(18, 4);

            modelBuilder.Entity<ItemTransferItemReceive>().Property(e => e.TPrice).HasPrecision(18, 4);



            modelBuilder.Entity<StoreReturnItems>().Property(e => e.Quantity).HasPrecision(18, 4);

            modelBuilder.Entity<StoreReturnItems>().Property(e => e.UnitPrice).HasPrecision(18, 4);



            modelBuilder.Entity<PropertyCountItem>().Property(e => e.Count).HasPrecision(18, 4);

            modelBuilder.Entity<PropertyCountItem>().Property(e => e.Price).HasPrecision(18, 4);

            modelBuilder.Entity<PropertyCountItem>().Property(e => e.PreviousBalance).HasPrecision(18, 4);



            modelBuilder.Entity<BinCardLine>().Property(e => e.IssuedQTY).HasPrecision(18, 4);

            modelBuilder.Entity<BinCardLine>().Property(e => e.IssuedUPrice).HasPrecision(18, 4);

            modelBuilder.Entity<BinCardLine>().Property(e => e.IssuedTPrice).HasPrecision(18, 4);

            modelBuilder.Entity<BinCardLine>().Property(e => e.ReceivedQTY).HasPrecision(18, 4);

            modelBuilder.Entity<BinCardLine>().Property(e => e.ReceivedUPrice).HasPrecision(18, 4);

            modelBuilder.Entity<BinCardLine>().Property(e => e.ReceivedTPrice).HasPrecision(18, 4);

            modelBuilder.Entity<BinCardLine>().Property(e => e.BalanceUPrice).HasPrecision(18, 4);

            modelBuilder.Entity<BinCardLine>().Property(e => e.BalanceTPrice).HasPrecision(18, 4);

            modelBuilder.Entity<BinCardLine>().Property(e => e.BalanceQTY).HasPrecision(18, 4);



            modelBuilder.Entity<StoreItemAssignment>().Property(e => e.AvgPrice).HasPrecision(18, 4);

            modelBuilder.Entity<StoreItemAssignment>().Property(e => e.Balance).HasPrecision(18, 4);

            modelBuilder.Entity<StoreItemAssignment>().Property(e => e.BegAvgPrice).HasPrecision(18, 4);

            modelBuilder.Entity<StoreItemAssignment>().Property(e => e.BegBalance).HasPrecision(18, 4);



            modelBuilder.Entity<BranchItemAssignment>().Property(e => e.AvgPrice).HasPrecision(18, 4);

            modelBuilder.Entity<BranchItemAssignment>().Property(e => e.Balance).HasPrecision(18, 4);

            modelBuilder.Entity<InventoryItem>().Property(e => e.AvgPrice).HasPrecision(18, 4);

            modelBuilder.Entity<InventoryItem>().Property(e => e.Balance).HasPrecision(18, 4);



            #endregion
            #region finance
            //modelBuilder.Entity<GLLedgerPosting>().HasIndex(g => g.AccountCode).IsUnique();
            //modelBuilder.Entity<GLLedgerPosting>().Property(e => e.AccountCode).HasMaxLength(50);
            //modelBuilder.Entity<GLLedgerPostingLines>().Property(e => e.JournalNumber).HasMaxLength(50);
            //modelBuilder.Entity<GLLedgerPostingLines>().HasIndex(u => new { u.GLLedgerPostingID, u.JournalNumber }).IsUnique(true);

            modelBuilder.Entity<GLTaxRate>().Property(e => e.CalculatedRate).HasPrecision(18, 4);
            modelBuilder.Entity<GLTaxRate>().Property(e => e.Rate).HasPrecision(18, 4);

            modelBuilder.Entity<GLRecordJournalDetail>().Property(e => e.Debit).HasPrecision(18, 9);

            modelBuilder.Entity<GLRecordJournalDetail>().Property(e => e.Credit).HasPrecision(18, 9);

            modelBuilder.Entity<GLRecordJournalDetail>().Property(e => e.ExRate).HasPrecision(18, 4);
            modelBuilder.Entity<GLRecordJournalDetail>().Property(e => e.DebitNonFun).HasPrecision(18, 4);

            modelBuilder.Entity<GLRecordJournalDetail>().Property(e => e.CreditNonFun).HasPrecision(18, 4);

            modelBuilder.Entity<ReconciliationBankAccount>().Property(e => e.ExRate).HasPrecision(18, 4);
            modelBuilder.Entity<ReconciliationBankAccount>().Property(e => e.DebitNonFun).HasPrecision(18, 4);
            modelBuilder.Entity<ReconciliationBankAccount>().Property(e => e.CreditNonFun).HasPrecision(18, 4);

            modelBuilder.Entity<ReconciliationBookAccount>().Property(e => e.ExRate).HasPrecision(18, 4);
            modelBuilder.Entity<ReconciliationBookAccount>().Property(e => e.DebitNonFun).HasPrecision(18, 4);
            modelBuilder.Entity<ReconciliationBookAccount>().Property(e => e.CreditNonFun).HasPrecision(18, 4);

            modelBuilder.Entity<ReconciliationBankServiceCharge>().Property(e => e.ExRate).HasPrecision(18, 4);
            modelBuilder.Entity<ReconciliationBankServiceCharge>().Property(e => e.AmountNonFun).HasPrecision(18, 4);



            modelBuilder.Entity<GLLedgerPostingLines>().Property(e => e.Debit).HasPrecision(18, 9);

            modelBuilder.Entity<GLLedgerPostingLines>().Property(e => e.Credit).HasPrecision(18, 9);

            modelBuilder.Entity<GLLedgerPostingLines>().Property(e => e.Balance).HasPrecision(18, 4);

            modelBuilder.Entity<GLLedgerPostingLines>().Property(e => e.ExRate).HasPrecision(18, 4);
            modelBuilder.Entity<GLLedgerPostingLines>().Property(e => e.DebitNonFun).HasPrecision(18, 4);

            modelBuilder.Entity<GLLedgerPostingLines>().Property(e => e.CreditNonFun).HasPrecision(18, 4);

            modelBuilder.Entity<GLLedgerPostingLines>().Property(e => e.BalanceNonFun).HasPrecision(18, 4);





            modelBuilder.Entity<GlEarningLedgerLine>().Property(e => e.MonthlyAmount).HasPrecision(18, 4);

            modelBuilder.Entity<GlEarningLedgerLine>().Property(e => e.YearlyAmount).HasPrecision(18, 4);



            modelBuilder.Entity<ArSalesOrderLine>().Property(e => e.SalesPrice).HasPrecision(18, 4);

            modelBuilder.Entity<ArSalesOrderLine>().Property(e => e.Discount).HasPrecision(18, 4);

            modelBuilder.Entity<ArSalesOrderLine>().Property(e => e.Commission).HasPrecision(18, 4);

            modelBuilder.Entity<ArSalesOrderLine>().Property(e => e.SubTotal).HasPrecision(18, 4);

            modelBuilder.Entity<ArSalesOrderLine>().Property(e => e.TaxAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArSalesOrderLine>().Property(e => e.TotalAmount).HasPrecision(18, 4);



            modelBuilder.Entity<ArSalesOrderPartial>().Property(e => e.SalesPrice).HasPrecision(18, 4);

            modelBuilder.Entity<ArSalesOrderPartial>().Property(e => e.Discount).HasPrecision(18, 4);

            modelBuilder.Entity<ArSalesOrderPartial>().Property(e => e.Commission).HasPrecision(18, 4);

            modelBuilder.Entity<ArSalesOrderPartial>().Property(e => e.SubTotal).HasPrecision(18, 4);

            modelBuilder.Entity<ArSalesOrderPartial>().Property(e => e.TaxAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArSalesOrderPartial>().Property(e => e.TotalAmount).HasPrecision(18, 4);



            modelBuilder.Entity<ArStandardCollection>().Property(e => e.TotalAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArStandardCollection>().Property(e => e.PrepaymentBalance).HasPrecision(18, 4);

            modelBuilder.Entity<ArStandardCollection>().Property(e => e.AppliedBalance).HasPrecision(18, 4);

            modelBuilder.Entity<ArStandardCollection>().Property(e => e.UnidentifiedBalance).HasPrecision(18, 4);



            modelBuilder.Entity<ArStandardCollectionLines>().Property(e => e.ApplyAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArStandardCollectionLines>().Property(e => e.PrepaymentAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArStandardCollectionLines>().Property(e => e.NetAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArStandardCollectionLines>().Property(e => e.BalanceDue).HasPrecision(18, 4);

            modelBuilder.Entity<ArStandardCollectionLines>().Property(e => e.TaxAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArStandardCollectionLines>().Property(e => e.WithholdTaxAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArStandardCollectionLines>().Property(e => e.WithholdingAmount).HasPrecision(18, 4);



            modelBuilder.Entity<ArMiscelaneousCollectionLine>().Property(e => e.Amount).HasPrecision(18, 4);

            modelBuilder.Entity<ArMiscelaneousCollectionLine>().Property(e => e.TaxAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArMiscelaneousCollectionLine>().Property(e => e.NetReceipt).HasPrecision(18, 4);

            modelBuilder.Entity<ArMiscelaneousCollectionLine>().Property(e => e.TotalAmount).HasPrecision(18, 4);



            modelBuilder.Entity<ArInvoice>().Property(e => e.TotalAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArInvoice>().Property(e => e.AmountDue).HasPrecision(18, 4);

            modelBuilder.Entity<ArInvoice>().Property(e => e.LineAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArInvoice>().Property(e => e.TaxAmount).HasPrecision(18, 4);



            modelBuilder.Entity<ArInvoiceLines>().Property(e => e.TotalAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArInvoiceLines>().Property(e => e.TaxAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ArInvoiceLines>().Property(e => e.SubTotal).HasPrecision(18, 4);

            modelBuilder.Entity<ArInvoiceLines>().Property(e => e.UnitPrice).HasPrecision(18, 4);

            modelBuilder.Entity<ApTPayment>().Property(e => e.Amount).HasPrecision(18, 4);


            #endregion

            #region procurement
            modelBuilder.Entity<ParticipantsItem>().Property(e => e.Quantity).HasPrecision(18, 4);

            modelBuilder.Entity<ParticipantsItem>().Property(e => e.UnitPrice).HasPrecision(18, 6);

            modelBuilder.Entity<ParticipantsItem>().Property(e => e.SubTotal).HasPrecision(18, 6);

            modelBuilder.Entity<ParticipantsItem>().Property(e => e.DiscountAmount).HasPrecision(18, 4);

            modelBuilder.Entity<ParticipantsItem>().Property(e => e.TaxAmount).HasPrecision(18, 6);

            modelBuilder.Entity<ParticipantsItem>().Property(e => e.Total).HasPrecision(18, 6);


            modelBuilder.Entity<BidAwardedSupplierItem>().Property(e => e.Quanity).HasPrecision(18, 4);

            modelBuilder.Entity<BidAwardedSupplierItem>().Property(e => e.UPrice).HasPrecision(18, 6);

            modelBuilder.Entity<BidAwardedSupplierItem>().Property(e => e.Discount).HasPrecision(18, 4);

            modelBuilder.Entity<BidAwardedSupplierItem>().Property(e => e.FinalUPrice).HasPrecision(18, 6);

            modelBuilder.Entity<BidAwardedSupplierItem>().Property(e => e.TaxAmount).HasPrecision(18, 6);

            modelBuilder.Entity<BidAwardedSupplierItem>().Property(e => e.TPrice).HasPrecision(18, 6);


            modelBuilder.Entity<ContractManagementItem>().Property(e => e.Quantity).HasPrecision(18, 4);

            modelBuilder.Entity<ContractManagementItem>().Property(e => e.UnitPrice).HasPrecision(18, 6);

            modelBuilder.Entity<ContractManagementItem>().Property(e => e.Addition).HasPrecision(18, 4);

            modelBuilder.Entity<ContractManagementItem>().Property(e => e.TaxAmount).HasPrecision(18, 6);

            modelBuilder.Entity<ContractManagementItem>().Property(e => e.Total).HasPrecision(18, 6);


            modelBuilder.Entity<ParticipatingSupplier>().Property(e => e.Quantity).HasPrecision(18, 4);

            modelBuilder.Entity<ParticipatingSupplier>().Property(e => e.UPrice).HasPrecision(18, 6);

            modelBuilder.Entity<ParticipatingSupplier>().Property(e => e.TaxAmount).HasPrecision(18, 6);

            modelBuilder.Entity<ParticipatingSupplier>().Property(e => e.TPrice).HasPrecision(18, 6);


            modelBuilder.Entity<ParticipatingSupplierItem>().Property(e => e.Quantity).HasPrecision(18, 4);

            modelBuilder.Entity<ParticipatingSupplierItem>().Property(e => e.UPrice).HasPrecision(18, 6);

            modelBuilder.Entity<ParticipatingSupplierItem>().Property(e => e.TaxAmount).HasPrecision(18, 6);

            modelBuilder.Entity<ParticipatingSupplierItem>().Property(e => e.TPrice).HasPrecision(18, 6);


            modelBuilder.Entity<WinningSupplierItem>().Property(e => e.Quanity).HasPrecision(18, 4);

            modelBuilder.Entity<WinningSupplierItem>().Property(e => e.UPrice).HasPrecision(18, 6);
            modelBuilder.Entity<WinningSupplierItem>().Property(e => e.TaxAmount).HasPrecision(18, 6);

            modelBuilder.Entity<WinningSupplierItem>().Property(e => e.TPrice).HasPrecision(18, 6);


            modelBuilder.Entity<PurchaseOrderItem>().Property(e => e.Quantity).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseOrderItem>().Property(e => e.UnitPrice).HasPrecision(18, 6);
            modelBuilder.Entity<PurchaseOrderItem>().Property(e => e.SubTotal).HasPrecision(18, 6);
            modelBuilder.Entity<PurchaseOrderItem>().Property(e => e.TaxAmount).HasPrecision(18, 6);
            modelBuilder.Entity<PurchaseOrderItem>().Property(e => e.Total).HasPrecision(18, 6);

            modelBuilder.Entity<PurchaseRequisitionItem>().Property(e => e.RequestedQuantity).HasPrecision(18, 4);
            modelBuilder.Entity<PurchaseRequisitionItem>().Property(e => e.Quantity).HasPrecision(18, 4);

            modelBuilder.Entity<ParticipatingSupplierItem>().Property(e => e.UPrice).HasPrecision(18, 6);
            modelBuilder.Entity<ParticipatingSupplierItem>().Property(e => e.TaxAmount).HasPrecision(18, 6);
            modelBuilder.Entity<ParticipatingSupplierItem>().Property(e => e.TPrice).HasPrecision(18, 6);
            #endregion


            Database.SetInitializer<ExceedDbContext>(null);

            #region globals
            modelBuilder.Configurations.Add(new GlobalEntityConfiguration.DocumentApprovalRuleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GlobalEntityConfiguration.ApprovalGroupEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GlobalEntityConfiguration.ApprovalGroupRuleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GlobalEntityConfiguration.AssignApprovalGroupEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GlobalEntityConfiguration.AssignApprovalGroupAssignmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GlobalEntityConfiguration.GlobalRuleValueEntityTypeConfiguration());
            #endregion

            #region common
            modelBuilder.Configurations.Add(new ConfigurationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AddressEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LookupEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PeriodEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new IndustryInvolvedEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AttachmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new IdentificationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LanguageEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LanguageProficiencyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RelationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CountryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BankEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BankReferenceEntityTypeConfiguration());
            #endregion

            #region inventory
            #region inventory_archive
            modelBuilder.Configurations.Add(new StoreRequisitionArchiveEntityConfiguration());
            modelBuilder.Configurations.Add(new StoreRequisitionItemArchiveEntityConfiguration());
            modelBuilder.Configurations.Add(new StoreRequisitionValidationArchiveEntityConfiguration());
            modelBuilder.Configurations.Add(new StoreRequisitionPropertyValidationArchiveEntityConfiguration());
            modelBuilder.Configurations.Add(new BinCardLineArchiveEntityConfiguration());
            #endregion
            #endregion

            #region Archive Management
            modelBuilder.Configurations.Add(new InstitutionArchiveNoEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PersonalArchiveNoEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeIncommingLetterEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InstitutionOutgoingLetterEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InstitutionTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InstitutionIncomingAnswerEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LetterStatusEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LetterSenderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeLetterSenderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ArchiveAttachementEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InstitutionIncomingLetterEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeFolderAttachmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeIncomingAnswerEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeOutgoingLetterEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeOutgoingAnswerEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CompanyDocumentTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CompanyDocumentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LetterReceiverEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AttachedLettersForStaffEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LetterForMultipleStaffEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new OutGoing_DepartmentToDepartmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OutGoing_DepartmentToDepartment_AnswerEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Attachment_DepartmentToDepartmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Receiver_DepartmentToDepartmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DepartmentFolderEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new OutGoing_DepartmentToStaffEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Attachment_DepartmentToStaffEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Receiver_DepartmentToStaffEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Answer_DepartmentToStaffEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new Attachment_Answer_DepartmentToDepartmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Attachment_Answer_DepartmentToStaffEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Attachment_Answer_InstitutionIncomingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Attachment_Answer_LetterSentForEmployeeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SubjectTypeForDepartmentLevelEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DepartmentOrSubjectTypeSelectionForFolderCreationEntityTypeConfiguration());
            #endregion

            #region HRM
            modelBuilder.Configurations.Add(new EmployeePerformanceSummaryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SalarySettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WaterMarkEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EvaluationCriteriaActionTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PlacementAfterSystemtEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeaveEntitlementSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeaveEntitleStartingDateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyLeavePlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyLeavePlanDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeavePlanValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeavePlanFinalValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeePerformanceActionsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProbationPerformanceSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SubjectTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeCodeSequenceSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeCodeSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DeligationCCEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DelegationReasonEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ManPowerPlanningRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BusinessUnitEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SickLeaveSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ComplainDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ComplainCCEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProbationPerformanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProbationPerformanceDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeaveReturnEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RetirementSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TemplateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TemplateTypeSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ComplainResponseEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AppealEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CompanyEmailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ComplainEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OverTimeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OverTimeTransferEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OverTimeTrasferDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OverTimePerDayEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LoanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RegularContributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FixedContributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AssociationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AdditionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AllowanceDefinationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AllowanceMappingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CourtAllowanceMappingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AllowancePerEmployeeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AttendanceNewEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AttendanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WeeklyAttendanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExperienceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CommitmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CompetencyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new JobTitleCodeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TerminationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PlacementEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PlacementHistoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeaveEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeaveEntitleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeaveEntitlementHistoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeaveTransferEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new IncommingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OutgoingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new IncommingStatusEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OutgoingStatusEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProhibitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RankEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RecruitmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new StepEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SalaryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmpTrainingEducationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new NeedAssessmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeesInAssessmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DisabilityEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CourseTrackingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TrainingEvaluationCriteriaCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TrainingEvaluationCriteriaEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TrainingEvaluationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MainCompanyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MainCompanyStructureEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CosignEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ApplicantionFormEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ClearanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AdvanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeavePlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TrainingPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RecruitmentPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new VacancyPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExitInterviewEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeavePaidEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SalaryJobTitleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OtherBenefitEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DisciplineMeasuresEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DisciplineRecordEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CommitteeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CommitteeMemberEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CommitteMinuteEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DisciplineMeasuresDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmpDisciplinaryHearingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmpDisciplineMeasureEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DisciplinaryCaseEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ContractEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmergencyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DisciplineMeasuresTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AbsenceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LateCommingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LateComingSummaryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TrainingStatusEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeAssociationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PerformanceRankEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new DepartmentDefineMeasurmentEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new DepartmentMeasurmentEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new DepartmentPerformanceActionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeCourtAssociationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DisciplineMeasuerByMoneyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new QuarterLengthEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new QuarterEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeaveNewAwajStartYearEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeePeerToPeerAndInternalCustomerPerformanceSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeavePayableSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeavePayableEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeavePayableDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeavePayableWithDetailFiscalYearEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LeaveTypeSettingEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new SeveranceCalculationSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SeverancePayableSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SeverancePayableEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SeverancePayableDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SeveranceWithProbabilityEntityTypeConfiguration());
            #region Payroll Process
            modelBuilder.Configurations.Add(new BonusSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollProcessLongTermDeductionsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProvidentFundEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmpPayrollServiceChargeViewModelEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PeriodStatusEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmpPayrollNoDaysWorkedEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollEmployeeCostCenterEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AllowanceTypeRuleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollProcessAllowancesEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollProcessAdditionsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmpPayrollDeductionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollProcessDeductionsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmpPayrollLongTermDeductionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollProcessLoansEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollProcessAdvancesEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollProcessFixedContributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollProcessRegularContributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollProcessViewModelEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollDistributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollEmployeeAccountEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollAccountsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OverTimeJobResultEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PerdimeSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DelegationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ContributionTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BonusEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BackPaymentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BackPaymentLineEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BackPaymentDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BackPaymentDistributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PayrollPeriodEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeTypeEntityTypeConfiguration());

            #endregion


            #region Performance

            modelBuilder.Configurations.Add(new EmployeePerformanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MeasurementDefinitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MeasurementEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PerformanceActionEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new EmployeeMeasurementDefinitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeMeasurementEntityTypeConfiguration());
            // modelBuilder.Configurations.Add(new EmployeePerformanceActionEntityTypeConfiguration());
            #endregion
            #region Recruitment
            modelBuilder.Configurations.Add(new ApplicantRegistrationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ApplicantExamResultEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ApplicantScreeningResultEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ClearanceProviderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RecruitmentVacancyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ApplicantEducationEntityTypeConfiguration());
            #endregion
            #region Company
            modelBuilder.Configurations.Add(new JobTitleMappingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new JobTitleMappingDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new JobTitleMappingMianEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new JobTitleMappingTempEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new JobTitleMappingDetailTempEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new JobTitleMappingMainTempEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RegionNationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AnnouncmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AnnouncmentDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BranchCostCenterEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new HolidayEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WeekendEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WeekendExceptionEntityTypeConfiguration());
            #endregion

            #region SelfCare

            modelBuilder.Configurations.Add(new LeaveRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExperienceRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TrainingApplicantEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EducationRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RecruitmentRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ApplicantTrainingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ApplicantExperienceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RequestesStatusEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new PromotionRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OverTimeRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OverTimeRequestEmployeeEntityTypeConfiguration());
            #endregion
            modelBuilder.Configurations.Add(new EducationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BranchEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new StructureEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TraineeEntityTypeConfiguration());

            #endregion

            #region payroll

            modelBuilder.Entity<Association>().Property(e => e.Amount).HasPrecision(18, 4);

            modelBuilder.Entity<Association>().Property(e => e.MonthlyAmount).HasPrecision(18, 4);

            modelBuilder.Entity<Association>().Property(e => e.AdditionalAmount).HasPrecision(18, 4);

            modelBuilder.Entity<FixedContribution>().Property(e => e.Amount).HasPrecision(18, 4);

            modelBuilder.Entity<FixedContribution>().Property(e => e.MonthlyAmount).HasPrecision(18, 4);

            modelBuilder.Entity<FixedContribution>().Property(e => e.AmountPercent).HasPrecision(18, 4);

            modelBuilder.Entity<FixedContribution>().Property(e => e.Balance).HasPrecision(18, 4);

            modelBuilder.Entity<RegularContribution>().Property(e => e.Amount).HasPrecision(18, 4);

            modelBuilder.Entity<RegularContribution>().Property(e => e.AdditionalAmount).HasPrecision(18, 4);



            #endregion

            #region APTS
            modelBuilder.Configurations.Add(new DrugToTransferTemporaryListsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DrugStoreTransferEntityTypeConfiguration());

            //modelBuilder.Configurations.Add(new LaboratoryCatagoryEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new LaboratoryTestsEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new LaboratoryCatagoryPatientCardEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new LaboratoryTestPatientCardEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new LaboratoryDrugRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LaboratoryDrugRequestItemEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new EmergencyDrugRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmergencyDrugRequestItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DrugListsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DrugReceivalModel19EntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Model22EntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PatientRegisterEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RequisitionModel20EntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DrugStoreEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DrugStoreItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DespensaryStoreEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DrugUnitEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DrugRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DrugRequestItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new APTSRequestStatusEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PatientRegisterDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DrugPurchaseRequisitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DrugReceivalDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new APTSValidationEntityTypeConfiguration());
            #endregion

            #region Budget GAAP

            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetSheetCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BalanceSheetEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.CostOfServiceSoldEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.ExpenditureEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.ForcastedIncomeStatementEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.InvestmentPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.ProfitLossStatementCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.ProfitLossStatementEntityTypeConfiguration());

            #endregion

            #region Budget

            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetRequestTransferDistributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetRequestTransferLogEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.TaskPerformanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetRequestLineEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.TaskPlanMonthlyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.TaskCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.TaskPerformaceApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.TaskPlanApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.TaskPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.TaskUnitEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetControlEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.PlanCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.RequestedMonthEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetAllocationApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetAllocationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetCurrencyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetPlanApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetPlanMonthlyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetRequestApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetTransferApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetTransferEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetUsageApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetUsageEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BudgetEntityConfiguration.BudgetBankAccountMapEntityTypeConfiguration());




            #endregion

            #region FleetManagement
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.MaintenanceCostDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetAvailabilityTresholdEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetAvailabilityTresholdValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentOperationPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentOperationPlanValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.CouponFuelEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.DriverTransferDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.InternalRentalRateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.TimeSheetExceptionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.TimeSheetSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.InspectionStatusKeyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RequestToInspectionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.HeavyORDumpTruckEntitytypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RoadORLoadEntitytypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RateOfRoadORLoadEntitytypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentedEquipmentMonthlyPaymentSummaryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.WagonDrillDailyOperationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalEquipmentPaymentSummaryDocumentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentedEquipmentMonthlyPaymentSummaryValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentedEquipmentDailyOperationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentFromExternalEquipmentRateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalEquipmentAdvancePaymentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetInsuranceDocumentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetInsuranceValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetInsurancePaymentRequestValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetInsurancePaymentRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetRentalRateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentRateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetAnnualPlanEntitytypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetAnnualPlanDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetAnnualPlanDailyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetBatteryAndTireEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetBoloRegistrationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentFromExternalEquipmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentFromExternalCustomerChangeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.LightVehicleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.DumpTruckTimeSheetDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.TimeSheetConsumptionItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.AssignedEquipmentAdditionalInfoEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.WagonDrillTimesheetEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.DryLoadingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.DumpTruckTimeSheetEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalInspectionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalAdvancePaymentRequestValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalAdvancePaymentRequestDepartmentValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalAdvancePaymentRequestBudgetValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalAdvancePaymentRequestEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalPaymentRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalPaymentRequestValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalPaymentRequestDepartmentValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalPaymentRequestBudgetValidationEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalPaymentRequestFinanceValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalPaymentRequestLineEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalInspectionValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalInspectionCriteriaEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalInspectionCriteriaDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentalMVInspectionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.MachineryWorkingTimeSheetEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetReturnApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetReturnEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetReturnItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.ExternalRentedEquipmentCheckListEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetConditionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.OperatorLicenseEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.OwningandOperatingCostConstantEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.LicenseLevelEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.SiteVechicleTransferTyreDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.ExternalRentedEquipmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetPettyCashTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.OwningandOperatingCostEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetPettyCashEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetIFuelConsumptionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetInsuranceTypeDetailEntityTypeConfiguration());


            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.SiteVechicleTransferDetailEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentArrivalApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.BatteryIssueEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.BatteryReturnEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.BatteryApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.HRFleetMappingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentTyreLogEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentArrivalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentBatteryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentBatteryLogEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.CannibalizationDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FuelConsumptionAlertLevelEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentTyreEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.TyreTransferEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.TyreTransferApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.DriverAssignmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FuelInventoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FuelTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.LocationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentPlanEvaluationApprovalApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.TimeSheetApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.AccessoriesEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RequestedEquipmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.AccidentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.AnnualEquipmentInventoryInspectionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.AnnualInventoryInspectionApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.AssignedEquipmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.AssignEquipmentApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.CannibalizationApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.CannibalizationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.CheckListEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetCheckListEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetLineCheckListListEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.CheckListLogEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.DisposalandReplacementRecommendationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.DriverTransferApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.DriverVehicleTransferEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EducationallevelEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentAttachmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentIdentityEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentNameEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentPlanandEvaluationsEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetHistoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetInsuranceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FleetsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FuelEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.FuelIssueApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.GoodRecivingNoteApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.InspectionApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.InspectionLogEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.LineCheckListEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.EquipmentCheckListEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.ManufacturerEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.OperatorEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.OperatorPositionEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.PlanAndEvaluationApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentForInternalApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentFromExternalApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentFromExternalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentFromExternalDocumentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentInternalProjectEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentOtherSupplierApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentOtherSuppliersEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentRequestApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.RentRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.SalvageStoreApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.SalvageStoreEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.SalvageStoreGoodReceivingEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.SitesVehicleTransferEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.SiteVehicleApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.TimeSheetEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.TireIssueApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.TireIssueEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.TireIssueDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.TireReturnApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.TireReturnEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.SelfCareSatisfactionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.VehicleDemandApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.VehicleDemandEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.VehicleDistributionApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.VehicleDistributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.VehicleReturnApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FleetManagementEntityConfiguration.VehicleReturnEntityTypeConfiguration());
            #endregion
            #region Maintenance
            modelBuilder.Configurations.Add(new DescriptionOfAchievementEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new MaintenanceStageEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new TyreSerialInfoEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new AccesseryInfoEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new ExternalMaintenancePaymentRequestDepValidationEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new ExternalMaintenancePaymentRequestValidationEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new ExternalPaymentRequestJobEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new InternalJobToExternalValidationEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new InternalJobToExternalEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new VehicleReceptionApprovalEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new ServiceGroupSettingEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new AdministrativeJobDepartmentApprovalEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new PmSettingEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new PmSchedulerEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new SOPDocumentEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new CanibalizationEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new CanibalizedPartsEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new CanibalizationDepartmentApprovalEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new CanibalizationApprovalEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new CloseWorkShopWithRemarkEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new MaintenanceCompanyDocumentTypeEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new MaintenanceCompanyDocumentEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new ExternalMaintenancePaymentRequestEntityTypeConfiguration()); // 
            modelBuilder.Configurations.Add(new ExternalMaintenancePaymentRequestDetailEntityTypeConfiguration()); //
            modelBuilder.Configurations.Add(new MaintenancePlanEntityTypeConfiguration()); //
            modelBuilder.Configurations.Add(new MaintenanceDailyPlanEntityTypeConfiguration()); //
            modelBuilder.Configurations.Add(new MaintenancePlanDetailEntityTypeConfiguration()); //
            modelBuilder.Configurations.Add(new AdditionalWorkShopDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExternalServiceCenterPaymentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PMScheduleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PMScheduleDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AdvanceMeasureEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DailyCareCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LogisticPlanDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LogisticPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DailyCareDetailItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DailyCareDetailLogEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DailyCareEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DailyCareLogEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FailureReasonEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ServiceCrewWorkLogEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExternalComponentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExternalComponentDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InventorySparePartMappingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AdministrativeJobApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FollowUpEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MaintenaceInspectionLogEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MaintenanceTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MechanicOnHandEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new JobStoreRequisitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WorkUnitSectionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new JobCausesEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new JobRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MechanicCrewEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MaintenanceCenterEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MaintenanceToolEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new JobOrderDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new JobOrderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ServiceCenterJobEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WorkshopFieldWorkToolEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExternalServiceAttachementEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SatisfactionNoteEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExternalServiceCenterDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new JobOrderServiceCenterDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExternalCrewEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExternalMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OtherCompanyMaintenaceApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OtherCompanyMaintenanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OutsourceMaintenanceApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OutsourceMaintenanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PlannerEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RepairTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ServiceCrewEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ServiceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ServiceSpareEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ServiceToolEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SparePartEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TechnicalJobApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MaintenanceToolIssueAndReturnEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SpareMaterialCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EquipmentInformationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MaintenanceIntervalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new VehicleReceptionFormEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new VehicleReceptionDocumentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new VehiclePartDamageTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new VehiclePartManagementEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new VehiclePartEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ServiceCenterEntityTypeConfiguration());

            #endregion

            #region CRM

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.AdvancePaymentLogEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.AdvancePaymentDistibutionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CashSalesPartialItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CashSalesPartialLineItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.TicketStatusLogEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesStoreEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.AdditionalCostEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.AdditionalCostLogEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CommissionLineItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesInvoicePaymentDistibutionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SupportGroupDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.InventoryCateoryMapEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SupportGroupEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.StatusTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PriorityTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.AssignTicketEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CRMHolidayEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.AdvancePaymentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.AdvancePaymentLineEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ReceiveMethodEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ServiceLineItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ServiceUsageApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ServiceUsageEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.LineItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.LineCreditSettlementEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CreditSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CreditSettlementEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CreditSettlementDistributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CreditSettlementValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CustomerHandlingTeamEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.VoucherDefinitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CustomerServiceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.HolidayDefinitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.MailModelEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PreCustomerEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PreCustomerScheduleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.VarietyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.IssueHistoryEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.TermEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.VoucherTermEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.TaxEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.TaxTransactionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PriceEntityTypeConfiguration());
            //  modelBuilder.Configurations.Add(new LineItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.UserEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.LifeTimeEntityTypeConfiguration());


            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ActivityDefinitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ActivityEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.GlobalTypeListEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.VoucherExtensionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.VoucherExtensionTransactionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SerialDefinitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SerialCodeEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.TransactionReferenceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.StoreTransactionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.StockBalanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.StockLevelEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.VoucherValueEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.GlobalTypeTaxEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ControlAccountEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.AccountRequirementEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.AccountEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.AccountMapEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.JournalTemplateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.JournalDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.BeginningEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CurrencyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CurrencyPreferenceEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ObjectStateEntityTypeConfiguration());
            //  modelBuilder.Configurations.Add(new ReligionEntityTypeConfiguration());
            //   modelBuilder.Configurations.Add(new ValueFactorDefinitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ValueFactorEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesQuoteVoucherEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CartTransactionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.StoreEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CartEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.OrganizationCustomerEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.OrganizationCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ThingCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PersonCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ProductEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ObjectStateDefinitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ProductUnitConvertorSetupEntityTypeConfiguration());


            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.NonCashTransactionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.LifeSpanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CommunicationSourceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesOrderVoucherEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CampaignEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CashSalesInvoiceEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CRMDepositEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CRMDepositLineEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CRMDepositValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CRMDepositDistributionEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CustomerTicketEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.TicketActivityDefinitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.TicketAssignmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.TicketRoleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.RoleEntityTypeConfiguration());


            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.BankAccountDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SpecificationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CreditSalesInvoiceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ObjectAccountRequirementEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ThingCategoryDefaultAccountEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CRMPeriodEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesShipmentEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesShipmentLineItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesShipmentDistributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesShipmentValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesPersonAssignmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.QuotationTermEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesQuoteVoucherValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesQuoteVoucherFirstValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesQuoteVoucherSecondValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesOrderVoucherValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PaymentScheduleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.VoucherContractDocumentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CashSalesInvoiceValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PosSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.AnnualSalesPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.BranchAnnualSalesPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.StoreAnnualSalesPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.StoreMonthlySalesPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.AnnualSalesPlanValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CRMDashboardTodateSalesDataEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.CRMDashboardTodateSalesBranchDataEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PriceEstimationQuoteVoucherEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PriceEstimationQuoteVoucherLineEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PriceEstimationQuoteVoucherServiceEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PriceEstimationQuoteVoucherFirstValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PriceEstimationQuoteVoucherSecondValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.PriceEstimationQuoteVoucherValidationEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.QueueTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.InvestmentTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.LocationMapEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.LocationUserAssignmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.QueueCustomerEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.QueueEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.QueueStatusEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.QueueValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.QueueStatusValidationEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.TenderFollowupEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.TenderFollowupLineItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.ProductPriceLogEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesReturnEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesReturnLineEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CRMEntityConfiguration.SalesReturnValidationEntityTypeConfiguration());

            #region sales tender
            modelBuilder.Configurations.Add(new SalesLotEvaluationByDocumentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SaleTenderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SalesLotEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SalesLotItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SalesLotParticipantEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SalesLotParticipantItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SalesLotPurchaseEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WinnerEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TenderWinnerValidationEntityTypeConfiguration());
            #endregion
            #endregion

            #region Construction
            modelBuilder.Configurations.Add(new ProjectDefinationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectCalanderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectConstraintEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ConstructionProjectUserAssignmentEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new MainContractEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ContractVariationOrderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ContractSuspensionOrderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SupplementalAgreementOrderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ContractClaimEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ContractClaimValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ContractClaimTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExtensionOfTimeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExecutedWorkEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CertifiedPaymentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new VariationOrderTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ContractRevisionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SubContratEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectPaymentTermEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ProjectTakeOffEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectTakeOffLineEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PaymentCertificateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectPaymentCertificateValidationEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new LineAmountEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectSpecificSettingAttributeConfiguration());
            modelBuilder.Configurations.Add(new RetentionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectDefineBudgetEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new BoQOfExecutedWorkEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BoQOfExecutedWorkContentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BoQOfExecutedWorkContentDetailEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ProjectBudgetRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectBudgetTransferEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectBudgetUtilizationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectMaterialRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectMaterialIssuedEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectMaterialDeliveryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEquipmentUtilizationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEquipmentHourEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEquipmentPerformanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEquipmentFuelconsumptionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEquipmentMobilizationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectHumanResourceUtilizationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectWorkBreakDownEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EquipmentMobilizationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationNameEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationBoqEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationBoqBreakdownEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationWorkCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationWorkItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationDirectLabourEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationEquipmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new UnitRateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationUnitFactoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationMobilizationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationCoefficientsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationRawDataEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationIndexLabourEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectMachineryRentHeaderEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ProjectEstimationTransportCostEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationIndirectLabourEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MapWorkItemCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LineDateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MaterialUtilizationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ScheduleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GanttDependencyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GanttResourceAssignmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GanttResourceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectTakeOffLineForKgEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WorkCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MachinaryRentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InternalMachinaryRentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstmnBoqSummaryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationBoqDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WorkItemPerProjectEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEquipmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectMaterialEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new GenericProjectMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GenericProjectEquipmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GenericProjectDirectLabourEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ProjectDirectLabourEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TakeOffPerProjectEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TakeOffLinePerProjectEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TakeOffLineForKgPerProjectEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SettingCalendarEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectJobTitleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new UnitOfMeasurementEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ResourceTypeEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ProjectTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectTypeMappingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MaterialPriceIndexEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MaterialTransportationCostEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new AreaWorkItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectEstimationAreaWorkItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SubContractWorkItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EstimationSubContractWorkItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BlockEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BlockMappingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SubProjectTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SubProjectStructureTypeMappingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ConstructionItemCategoryEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new GenericWorkItemPerProjectEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GenericAreaWorkItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GenericSubContractWorkItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GenericProjectCustomWorkItemEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new DeformedBarEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectCrewEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CrewHumanResourceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CrewEquipmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CrewMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new BarDiameterEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WorkItemDurationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ResourceEstimationLabourEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ResourceEstimationEquipmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ResourceEstimationMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyFuelCostEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LaborRateIndexFactorAttributeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LaborRateIndexFactorAttributeNameEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MethodOfWorkEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ResourceUtilizationPeriodEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ResourceUtilizationDayEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ResourceUtilizationWorkItemkEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DetailedWorkProgressEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ResourceUtilizationMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ResourceUtilizationDirectLabourEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ResourceUtilizationEquipmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RoadEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RoadMappingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EarnedValueManagementTermsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EarnedValuePerProjectEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EarnedValuePerItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EarnedValueManagementTermDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EVMTermOperandComponentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EarnedValueManagementTermFormulaConstantEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EarnedValueManagementPeriodEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ConstructionPeriodEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ConstructionSubPeriodEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new MonthlyProjectProgressEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectQualityEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new QualityPlanningEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new QualityAssuranceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new QualityControlEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectQualityComplaintEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PeriodMappingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PerformanceEvaluationComponentWeightEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PEVCostSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectCostEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectPaymentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectPlanningEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RScheduleSubmissionAcToOperationManualEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RScheduleSubmissionAcToContractEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectPnanningCompletenessEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectPlanningQualityAndAcceptanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectClaimEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ContempraryRecordEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RequirementForMonthlyClaimsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AnalysisTechniqueApplicationForMonthlyClaimsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ApprovalStatusForClaimsRespondedEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectReportEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectReportCompletnessComponentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectReportNarrativeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectCommunicationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectCommunicationWithinProjectEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectCommunicationWithHeadOfficeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectCommunicationWithConsultantEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MonthlyProjectPerformanceModifierEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PerformanceModifierHealthAndSafetyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PerformanceModifierBudgetRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PerformanceModifierBudgetUtilizationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PerformanceModifierMaterialUtilizationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PerformanceModifierSubContractAdministrationEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new DelayTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WorkDelayEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DelayAnalysisTaskEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DelayTaskDependencyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TaskDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TaskDetailBreakDownEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ScheduleRevisionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ScheduleRevisionValidationEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ProjectMaterialExpenseEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectDirectMaterialExpenseEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectIndirectMaterialExpenseEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ProjectEstimationCustomWorkItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EstimationCustomWorkItemMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EstimationCustomWorkItemEquipmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EstimationCustomWorkItemDirectLaborEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EstimationCustomWorkItemContentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EstimationTemplateCustomWorkItemDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectCustomWorkItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GenericProjectCustomWorkItemDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectCustomWorkItemDetailEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ProjectRepresentativeElementEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PriceAdjustmentWeightEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectPriceAdjustmentFormulaTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PriceAdjustmentWeightItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RepresentativeElementCostInBillEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RepresentativeElementWeightRangeEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ProjectPaymentAttributeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectAdvancePaymentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WorkInProgressEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ConsPaymentAgingPeriodEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ConstructionCustomerAccountEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new WorkInProgressValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WorkInProgressDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new WorkInProgressDistributionEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ReceivableInvoiceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ReceivableInvoiceValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ReceivableInvoiceDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ConsReceivableInvoiceDistributionEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ARStandardCollectionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ARStandardCollectionDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ARStandardCollectionValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ConsStandardCollectionDistributionEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new PurchasePriceComparisonEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PurchaseSupplierEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new CashFlowYearEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CashFlowContractDataEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CashFlowContractDataDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CashFlowFinanceSourceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CashFlowFinanceSourceDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CashFlowDeductionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CashFlowDeductionDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new NetOperationCashFlowEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new NetOperationCashFlowDetailEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ConstructionHolidayEntityTypeConfiguration());
            #endregion

            #region Production
            modelBuilder.Configurations.Add(new ProductionProjectEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PlotEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionSiteEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionYearEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionPeriodEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionAndProcessingCostCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionActivityEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CropEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new NaturalGumEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InputMaterialApplicationRateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InputMaterialApplicationRateItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CostTrackingPeriodEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectCostByMonthEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectCostByMonthDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectCostByMonthMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SeedProductionPlanYearEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionOrderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SeedProductionOrderItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GumProductionOrderItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionOrderActivitiyEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MachineryMaintenanceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionRawProductPurchaseEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionOrderInputMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionItemCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProcessingPlantEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new SeedProcessingOrderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GumProcessingOrderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new SeedProcessingOrderItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new GumProcessingOrderItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProcessingOrderActivityEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProcessingInputMaterialEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ProductionStoreReceiveEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionStoreReceiveItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionStoreReceiveValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionStoreReceiveDistributionEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ProductionFinishedGoodsStoreReceiveEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionFinishedGoodsStoreReceiveItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionFinishedGoodsStoreReceiveValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionFinishedGoodsStoreReceiveDistributionEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new CleanedSeedSellingPriceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CleanedSeedSellingPriceItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CleanedNaturalGumSellingPriceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CleanedNaturalGumSellingPriceItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RawProductFinishedGoodMappingEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ProjectedMonthlyCashFlowEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProjectedMonthlyCashFlowItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CleanedProductSaleEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CleanedProductSaleItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CleanedProductSaleItemPerMonthEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RawProductPurchaseEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RawProductPurchaseItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RawProductPurchaseItemPerMonthEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CustomCleaningAndStorageEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CustomCleaningAndStorageItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CustomCleaningAndStorageItemPerMonthEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ProductionIssueEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionIssueItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionIssueValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ProductionIssueDistributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new IssueAndProcessingOrderItemMappingEntityTypeConfiguration());
            #endregion
            #region CMS
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CMSCourseEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CMSProgramEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CityEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.RegistrationNumberPrefixEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.ISOTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.ISOChecklistEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CMSTraineeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.TraineeIdNumberComponentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.TraineeProgramEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CompanyEvaluationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CmsTraineeIdentificationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.WelderPerformanceQualificationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.CMSTraineeCourseEntityTypeConfiguration());
            #endregion


            #region printing 
            modelBuilder.Configurations.Add(new PrintingQualityInspectionEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new PrintingProductionAllottedRawMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingTaskIssueEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new PrintingProductionDailyFollowUpDetailEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new PrintingProductionFollowUpTaskEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new PrintingProductionFollowUpEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingSummaryCommentEntityTypeCOnfiguration());
            modelBuilder.Configurations.Add(new PressDailyFollowUpWastageEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingDailyFollowUpLogBookEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingDailyFollowUpMachineMaintenanceEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new PrintingSummaryCommentEntityTypeCOnfiguration());
            modelBuilder.Configurations.Add(new PrintingProductionAllottedRawMaterialEntityTypeConfiguration());
            // modelBuilder.Configurations.Add(new PrintingDailyFollowUpDelayReasonEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingProductionDailyFollowUpDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingProductionFollowUpTaskEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingProductionFollowUpEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingMachineDetailEntityTypeConfiguration());
            // modelBuilder.Configurations.Add(new PrintingJobOrderProductionJobDesignEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new PrintingJobOrderJobDesignEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingProductionLaborCostEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingProductionMaterialCostEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingJobOrderProductionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingEstimationMarginEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingJobOrderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingJobOrderItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingProformaInvoiceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingProformaInvoiceItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingEstimationSummaryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingEstimationDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingEstimationCostSummaryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingOverAllCostEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingEstimationLaborCostEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingEstimationMaterialCostsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingMaterialCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingMaterialCategoryItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingBindingStyleEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new PrintingJobTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintCostCenterEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingMachineTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintItemCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintLabourRateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintMachinePaperListEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintWorkOrderTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingCostEstimationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingProductProcessEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrintingPaperSizeEntityTypeConfiguration());


            modelBuilder.Configurations.Add(new PrintingProcessEntityTypeConfiguration());
            #region  Estimation
            #region Printing Estimation
            modelBuilder.Configurations.Add(new Context.PrintingStepEntityConfiguration());
            modelBuilder.Configurations.Add(new Context.EstimationFormEntityConfiguration());
            modelBuilder.Configurations.Add(new Context.MaterialCostEntityConfiguration());
            modelBuilder.Configurations.Add(new Context.LaborCostEntityConfiguration());
            modelBuilder.Configurations.Add(new Context.OverallCostEntityConfiguration());
            #endregion
            //#region Furniture
            //modelBuilder.Configurations.Add(new Context.FurnitureCategoryEntityConfiguration());
            //modelBuilder.Configurations.Add(new Context.FurnitureCostEstimationEntityConfiguration());
            //modelBuilder.Configurations.Add(new Context.FurnitureMaterialCostEntityConfiguration());
            //modelBuilder.Configurations.Add(new Context.DirectLaborCostEntityConfiguration());
            //modelBuilder.Configurations.Add(new Context.FurnitureOverallCostEntityConfiguration());
            ////modelBuilder.Configurations.Add(new Context.OverallCostEntityConfiguration());

            //#endregion
            #endregion

            #region Production Follow UP Entity Configuration
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.YearlyProductionPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.HalfYearlyProductionPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.QuarterlyProductionPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.MonthlyProductionPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.WeeklyProductionPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.ProductionJobTypePlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.ProductionCustomerTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.ProductionMonthlyJobTypePlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.ProductionMonthlyPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.ProductionYearlyPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.ProformaEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.GraphicEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobStatusEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.SettingsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobOrderMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.PrintingProductionFinishedGoodsStoreReceiveEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.PrintingProductionFinishedGoodsStoreReceiveItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.PrintingProductionFinishedGoodsStoreReceiveValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.PrintingProductionFinishedGoodsStoreReceiveDistributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FinishedGoodsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.ProductDeliveryEntityTypeConfiguration());
            // modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.ProductDeliveryItemsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.PFProformaItemsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobStatusLineItemEntityTypeConfiguration());
            #endregion
            //#region Job Costing Entity ConfiguraExceedERP.DataAccess.Contexttion
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JcPensionEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.IdleTimeCategoryEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobOrderTimeSheetEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobOrderTimeRegistrationEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobOrderSummaryEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.YearlyOverHeadRateEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.MachineEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.IdleTimeEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.IdleTimeProductiveEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.IdleTimeOTEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.IdleTimeNonProductiveControllableEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new IdleTimeNonProductiveNonControllableEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobChangeOfOrderEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobMaterialCostEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobMaterialreturnEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobLaborCostEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FinishedJobEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.DeliveredJobEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.DailyLaborRateEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.DLRSalaryEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.DLRHourEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobOrderSummaryCostCenterDetailEntityTypeConfiguration());
            //#endregion
            #endregion
            #region Manufucturing 
            #region  Estimation
            modelBuilder.Configurations.Add(new Context.FurnitureBoqDocumentCommentEntityTypeConfiguration());
            #region Furniture Estimation
            modelBuilder.Configurations.Add(new FurnitureProformaInvoiceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureProformaInvoiceItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureStepEntityConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureLaborCostEntityConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureOverallCostEntityConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureCategoryEntityConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureCostEstimationEntityConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureMaterialCostEntityConfiguration());
            modelBuilder.Configurations.Add(new Context.DirectLaborCostEntityConfiguration());

            #endregion
            #endregion

            #region Production Follow UP
            modelBuilder.Configurations.Add(new FurnitureJobDesignEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureBoqJobDesignEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureBoqDocumentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureBoqRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EstimationCostSummaryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EstimationDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EstimationSummaryEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new FurnitureJobOrderProductionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureProductionBillOfMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureProductionBillOfLaborEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureJobAssignmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureDailyProductionFollowUpEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureProductionDailyFollowUpDelayReasonEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureUnfinishedMaterialTransferFormEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new StandardBillOfLaborEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new StandardBillOfMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureStandardJobTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureJobOrderItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureBillOfQuantityEntityConfiguration());
            modelBuilder.Configurations.Add(new FurnitureBOQMaterialEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FurnitureBOQLaborEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ManifucturingTaskCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ManufacturingMaterialCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ManufacturingMaterialCategoryItemEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureYearlyProductionPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureHalfYearlyProductionPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureQuarterlyProductionPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureMonthlyProductionPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureWeeklyProductionPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureProductionJobTypePlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureProductionCustomerTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureProductionMonthlyJobTypePlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureProductionMonthlyPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureProductionYearlyPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureJobCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureProformaEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureGraphicEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureJobEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureJobStatusEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureSettingsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureJobOrderMaterialEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new Context.FurniturePrintingProductionFinishedGoodsStoreReceiveEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new Context.FurniturePrintingProductionFinishedGoodsStoreReceiveItemEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new Context.FurniturePrintingProductionFinishedGoodsStoreReceiveValidationEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new Context.FurniturePrintingProductionFinishedGoodsStoreReceiveDistributionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureFinishedGoodsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureProductDeliveryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureProductDeliveryItemsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurniturePFProformaItemsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.FurnitureJobStatusLineItemEntityTypeConfiguration());
            #endregion                                  Furniture

            #region Job Costing 
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureJcPensionEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureIdleTimeCategoryEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureJobOrderTimeSheetEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureJobOrderTimeRegistrationEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureJobOrderSummaryEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureYearlyOverHeadRateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureMachineEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureIdleTimeEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureIdleTimeProductiveEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureIdleTimeOTEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureIdleTimeNonProductiveControllableEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureIdleTimeNonProductiveNonControllableEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureJobCostEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureJobChangeOfOrderEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureJobMaterialCostEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureJobMaterialreturnEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureJobLaborCostEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureFinishedJobEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureDeliveredJobEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureDailyLaborRateEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureDLRSalaryEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureDLRHourEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.FurnitureJobOrderSummaryCostCenterDetailEntityTypeConfiguration());
            #endregion
            #endregion

            #region Job Costing Entity Configuration
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.JcPensionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.IdleTimeCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.JobOrderTimeSheetEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.JobOrderTimeRegistrationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.JobOrderSummaryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.YearlyOverHeadRateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.MachineEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.IdleTimeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.IdleTimeProductiveEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.IdleTimeOTEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.IdleTimeNonProductiveControllableEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.IdleTimeNonProductiveNonControllableEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.JobCostEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.JobChangeOfOrderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.JobMaterialCostEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.JobMaterialreturnEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.JobLaborCostEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.FinishedJobEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.DeliveredJobEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.DailyLaborRateEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.PrintingJobSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.DLRSalaryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.DLRHourEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ExceedERP.DataAccess.Context.JobCostingEntityConfiguration.JobOrderSummaryCostCenterDetailEntityTypeConfiguration());
            #endregion

            #region Planning
            modelBuilder.Configurations.Add(new Context.GlobalIndicatorEntityTypeConfiguration());
            #region Bussiness Plan
            modelBuilder.Configurations.Add(new Context.BusinessPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.BusinessPlanValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ShareAndDividendEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ShareAndDividendFiscalYearDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.LandManagementEntityTypeConfiguration());
            #endregion
            modelBuilder.Configurations.Add(new Context.ObjectiveEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.IndicatorEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ExcutivePlanKPIEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.PlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ActualEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.BSCPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.BSCActualEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.PlanningDetailEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new Context.PlanningWithSubIndicatorEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.MonthlyPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ActualDetailEntityTypeConfiguration());
            //modelBuilder.Configurations.Add(new Context.ActualDetailWithSubIndicatorEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.MonthlyActualEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.RiskMitigationPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.RiskFollowUpEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.WeeklyPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.WeeklyActualEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ProjectWeeklyPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ProjectWeeklyActualEntityTypeConfiguration());
            #region project Plan
            modelBuilder.Configurations.Add(new Context.ProjectObjectiveEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ProjectKPIEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ProjectSubKPIEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ProjectPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ProjectActualEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ProjectMonthlyPlanEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ProjectMonthlyActualEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ProjectExcutiveSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new Context.ProjectExcutiveSummarySettingEntityTypeConfiguration());


            #endregion
            #endregion

            #region Health 
            modelBuilder.Configurations.Add(new MedicalRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MedicalAttachmentEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new HealthRankAndValueSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new HealthResponsiblityLevelValueSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PaymentCoverageSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new HealthRankAndValueEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new HealthRankAndValueValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new HealthRankAndValueFinalValidationEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new HealthBenefitSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new HealthApprovalSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new HealthExpenseAndTaxSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new HealthAttachmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ReferralFormEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new ReferralSheetEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PatientsLeaveEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PrescriptionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new TreatmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PatientCardEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PatientCardDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PatientResultEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PatientResultDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PatientDrugOrderEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new DiagnosisEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LaboratoryCatagoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LaboratoryTestsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LaboratoryTestPatientCardEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new LaboratoryCatagoryPatientCardEntityTypeConfiguration());
            #endregion
            #region Insurance


            modelBuilder.Configurations.Add(new InsuranceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InsurancePremiumSettingEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InsuranceGroupEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AccidentRecordEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AccidentMedicalRecordEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new RestitutionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new CourtInsuranceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InsuranceTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InsuranceCategoryEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new OtherInsuranceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InsuranceCoverTypeEntityTypeConfiguration());
            #region New
            modelBuilder.Configurations.Add(new EmployeeMedicalExpenseRefundEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MedicalExpenseRefundAttachmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MedicalExpenseRefundDepartmentApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new MedicalExpenseRefundApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeAccidentInsuranceClaimEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AccidentInsuranceClaimAttachmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AccidentInsuranceClaimDepartmentApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AccidentInsuranceClaimApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EmployeeAccidentCreditServiceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AccidentCreditServiceAttachmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AccidentCreditServiceDepartmentApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AccidentCreditServiceApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InsurancePolicyRenewalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PolicyRenewalAttachmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PolicyRenewalDepartmentApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new PolicyRenewalApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EquipmentAccidentClaimEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new EquipmentAccidentClaimAttachmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AccidentClaimDepartmentApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new AccidentClaimApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FireAndLightningInsuranceEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FireAndLightningInsuranceDetailEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FireAndLightningInsuranceApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FireAndLightningInsuranceDepartmentApprovalEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new FireAndLightningInsuranceAttachmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InsurancePaymentSettingEntityTypeConfiguration());

            #endregion
            #endregion

            #region Info Desk
            #region change mgt
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.ChangeRequestEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.ChangeTaskEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.ChangeApprovalEntityTypeConfiguration());
            #endregion
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.ChatSessionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.MessageEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.MaterialItemsEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.MaterialsUsedEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.SoftwareLicenseEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.AssignedUserEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.InfoDeskKnowledgeBaseEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.KnowledgeBaseAttachmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.CustomerTicketAttachmentEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.CustomerTicketEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.TicketActivityDefinitionEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.TicketStatusLogEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.SupportGroupEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.StatusTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.PriorityTypeEntityTypeConfiguration());
            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.AssignTicketEntityTypeConfiguration());

            modelBuilder.Configurations.Add(new InfoDeskEntityConfiguration.TicketTypeEntityTypeConfiguration());


            #endregion
        }
        public DbSet<ConsProjectType> ConsProjectTypes { get; set; }
        #region Planning
        public DbSet<GlobalIndicator> GlobalIndicators { get; set; }
        #region Bussiness Plan 
        public DbSet<BusinessPlan> BusinessPlans { get; set; }
        public DbSet<BusinessPlanValidation> BusinessPlanValidations { get; set; }
        public DbSet<ShareAndDividend> ShareAndDividends { get; set; }
        public DbSet<ShareAndDividendFiscalYearDetail> ShareAndDividendFiscalYearDetails { get; set; }
        public DbSet<LandManagement> LandManagements { get; set; }

        #endregion
        public DbSet<Objective> Objectives { get; set; }
        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<ExcutivePlanKPI> ExcutivePlanKPIs { get; set; }
        public DbSet<Plan> Plans { get; set; }
        public DbSet<BSCPlan> BSCPlans { get; set; }
        public DbSet<PlanningDetail> PlanningDetails { get; set; }
        ////public DbSet<PlanningWithSubIndicator> PlanningWithSubIndicators { get; set; }
        public DbSet<MonthlyPlan> MonthlyPlans { get; set; }
        public DbSet<WeeklyPlan> WeeklyPlans { get; set; }
        public DbSet<Actual> Actuals { get; set; }
        public DbSet<BSCActual> BSCActuals { get; set; }
        public DbSet<ActualDetail> ActualDetails { get; set; }
        //public DbSet<ActualDetailWithSubIndicator> ActualDetailWithSubIndicators { get; set; }
        public DbSet<MonthlyActual> MonthlyActuals { get; set; }
        public DbSet<WeeklyActual> WeeklyActuals { get; set; }
        public DbSet<RiskMitigationPlan> RiskMitigationPlans { get; set; }
        public DbSet<RiskFollowUp> RiskFollowUps { get; set; }
        public DbSet<ProjectObjective> ProjectObjectives { get; set; }
        public DbSet<ProjectKPI> ProjectKPIs { get; set; }
        public DbSet<ProjectSubKPI> ProjectSubKPIs { get; set; }
        public DbSet<ProjectPlan> ProjectPlans { get; set; }
        public DbSet<ProjectActual> ProjectActuals { get; set; }
        public DbSet<ProjectMonthlyPlan> ProjectMonthlyPlans { get; set; }
        public DbSet<ProjectMonthlyActual> ProjectMonthlyActuals { get; set; }
        public DbSet<ProjectWeeklyPlan> ProjectWeeklyPlans { get; set; }
        public DbSet<ProjectWeeklyActual> ProjectWeeklyActuals { get; set; }
        public DbSet<ProjectExcutiveSetting> ProjectExcutiveSettings { get; set; }
        public DbSet<ProjectExcutiveSummarySetting> ProjectExcutiveSummarySettings { get; set; }


        #endregion
        #region Common
        public DbSet<IsoDocument> IsoDocuments { get; set; }

        public DbSet<Configuration> Configurations { get; set; }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Lookup> Lookups { get; set; }


        public DbSet<Period> Periods { get; set; }



        public DbSet<IndustryInvolved> IndustryInvolveds { get; set; }



        public DbSet<Attachment> Attachments { get; set; }

        public DbSet<Identification> Identifications { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<LanguageProficiency> LanguageOfProficiencies { get; set; }

        public DbSet<Country> Countries { get; set; }
        public DbSet<Bank> Banks { get; set; }


        public DbSet<Relation> Relations { get; set; }










        public DbSet<BankReference> BankReferences { get; set; }



        #endregion

        #region Construction
        public System.Data.Entity.DbSet<ProjectInformation> ProjectInformation { get; set; }
        public System.Data.Entity.DbSet<ConstructionProjectUserAssignment> ConstructionProjectUserAssignments { get; set; }
        public System.Data.Entity.DbSet<ProjectCalendar> ProjectCalander { get; set; }
        public System.Data.Entity.DbSet<SettingCalendar> SettingCalendar { get; set; }
        public System.Data.Entity.DbSet<ProjectConstraintsAndRisk> ProjectConstraint { get; set; }

        public System.Data.Entity.DbSet<ProjectWorkBreakDown> ProjectWorkBreakDown { get; set; }
        public System.Data.Entity.DbSet<ProjectEquipmentMobilization> ProjectEquipmentMobilization { get; set; }
        public System.Data.Entity.DbSet<ProjectHumanResourceUtilization> ProjectHumanResourceUtilization { get; set; }
        public System.Data.Entity.DbSet<ProjectContract> ProjectContract { get; set; }
        public System.Data.Entity.DbSet<ProjectSubContract> SubContrat { get; set; }
        public DbSet<ContractVariationOrder> ContractVariationOrder { get; set; }
        public DbSet<ContractSuspensionOrder> ContractSuspensionOrder { get; set; }
        public DbSet<SupplementalAgreement> SupplementalAgreement { get; set; }
        public DbSet<ContractClaim> ContractClaim { get; set; }
        public DbSet<ContractClaimValidation> ContractClaimValidations { get; set; }
        public DbSet<ContractClaimType> ContractClaimType { get; set; }
        public DbSet<ExtensionOfTime> ExtensionOfTime { get; set; }
        public DbSet<ExecutedWork> ExecutedWork { get; set; }
        public DbSet<CertifiedPayment> CertifiedPayment { get; set; }
        public DbSet<VariationOrderType> VariationOrderType { get; set; }
        public DbSet<ContractRevision> ContractRevision { get; set; }
        public DbSet<ProjectPaymentTerm> ProjectPaymentTerms { get; set; }

        public System.Data.Entity.DbSet<ProjectDefineBudget> ProjectDefineBudgets { get; set; }
        public System.Data.Entity.DbSet<ProjectBudgetRequest> ProjectBudgetRequest { get; set; }
        public System.Data.Entity.DbSet<ProjectBudgetTransfer> ProjectBudgetTransfer { get; set; }
        public System.Data.Entity.DbSet<ProjectBudgetUtilization> ProjectBudgetUtilization { get; set; }
        public System.Data.Entity.DbSet<ProjectMaterialRequest> ProjectMaterialRequest { get; set; }
        public System.Data.Entity.DbSet<ProjectMaterialIssued> ProjectMaterialIssued { get; set; }
        public System.Data.Entity.DbSet<ProjectMaterialDelivery> ProjectMaterialDelivery { get; set; }
        public System.Data.Entity.DbSet<ProjectEquipmentUtilization> EquipmentUtilization { get; set; }
        public System.Data.Entity.DbSet<ProjectEquipmentHour> EquipmentHour { get; set; }
        public System.Data.Entity.DbSet<ProjectEquipmentPerformance> EquipmentPerformance { get; set; }
        public System.Data.Entity.DbSet<ProjectEquipmentFuelconsumption> EquipmentFuelconsumption { get; set; }
        public System.Data.Entity.DbSet<EquipmentMobilization> EquipmentMobilization { get; set; }
        public System.Data.Entity.DbSet<ProjectTakeOff> ProjectTakeOff { get; set; }
        public System.Data.Entity.DbSet<ProjectTakeOffLine> ProjectTakeOffLine { get; set; }
        public System.Data.Entity.DbSet<ProjectJobTitle> ProjectJobTitles { get; set; }
        public System.Data.Entity.DbSet<ProjectPaymentCertificate> PaymentCertificates { get; set; }
        public System.Data.Entity.DbSet<ProjectPaymentCertificateValidation> PaymentCertificateValidations { get; set; }
        public System.Data.Entity.DbSet<BoQOfExecutedWork> BoQOfExcutedWork { get; set; }
        public System.Data.Entity.DbSet<BoQOfExecutedWorkContent> BoQOfExcutedWorkContent { get; set; }
        public System.Data.Entity.DbSet<BoQOfExecutedWorkContentDetail> BoQOfExcutedWorkContentDetail { get; set; }

        public System.Data.Entity.DbSet<ProjectPaymenCertificatetLine> LineAmounts { get; set; }

        //public System.Data.Entity.DbSet<ProjectPaymenAdvance> AdvancePayments { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.Construction.ProjectSpecificSettingAttribute> ProjectSpecificSettingAttributes { get; set; }

        public System.Data.Entity.DbSet<ProjectPaymenRetention> Retentions { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationName> ProjectEstimationName { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationBoq> ProjectEstimationBoq { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationBoqBreakdown> ProjectEstimationBoqBreakdown { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationWorkCategory> ProjectEstimationWorkCategory { get; set; }
        public System.Data.Entity.DbSet<WorkCategory> WorkCategory { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationWorkItem> ProjectEstimationWorkItem { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationMaterial> ProjectEstimationMaterial { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationDirectLabour> ProjectEstimationDirectLabour { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationIndirectLabour> ProjectEstimationIndirectLabour { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationEquipment> ProjectEstimationEquipment { get; set; }
        public System.Data.Entity.DbSet<UnitRate> UnitRate { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationUnitFactory> ProjectEstimationUnitFactory { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationMobilization> ProjectEstimationMobilization { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationCoefficients> ProjectEstimationCoefficients { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationRawData> ProjectEstimationRawData { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationIndexLabour> ProjectEstimationIndexLabour { get; set; }
        public System.Data.Entity.DbSet<ProjectMachineryRentHeader> ProjectMachineryRentHeader { get; set; }
        public System.Data.Entity.DbSet<MachinaryRent> MachinaryRent { get; set; }
        public System.Data.Entity.DbSet<InternalMachinaryRent> InternalMachinaryRent { get; set; }
        public System.Data.Entity.DbSet<ProjectEstimationTransportCost> ProjectEstimationTransportCost { get; set; }
        public System.Data.Entity.DbSet<MapWorkItemCategory> MapWorkItemCategory { get; set; }
        public System.Data.Entity.DbSet<LineDate> LineDate { get; set; }
        public System.Data.Entity.DbSet<MaterialUtilization> MaterialUtilization { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.Construction.ProjectTaskSchedule> ProjectTaskSchedules { get; set; }
        //public System.Data.Entity.DbSet<ExceedERP.Core.Domain.Construction.Task> Schedules { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ResourceAssignment> ResourceAssignments { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.Task> Tasks { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.TaskDetail> TaskDetail { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.TaskDetailBreakDown> TaskDetailBreakDown { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.Dependency> Dependencies { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.Resource> Resources { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ResourceType> ResourceTypes { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectTakeOffLineForKg> ProjectTakeOffLineForKg { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectEstmnBoqSummary> ProjectEstmnBoqSummary { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectEstimationBoqDetail> ProjectEstimationBoqDetail { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.WorkItemPerProject> WorkItemPerProject { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.GenericWorkItemPerProject> GenericWorkItemPerProject { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.GenericAreaWorkItem> GenericAreaWorkItem { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.GenericSubContractWorkItem> GenericSubContractWorkItem { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.GenericProjectCustomWorkItem> GenericProjectCustomWorkItem { get; set; }

        public DbSet<ProjectDirectLabour> ProjectDirectLabour { get; set; }
        public DbSet<TakeOffPerProject> TakeOffPerProject { get; set; }
        public DbSet<ProjectMaterial> ProjectMaterial { get; set; }
        public DbSet<ProjectEquipment> ProjectEquipment { get; set; }
        public DbSet<GenericProjectMaterial> GenericProjectMaterial { get; set; }
        public DbSet<GenericProjectEquipment> GenericProjectEquipment { get; set; }
        public DbSet<GenericProjectDirectLabour> GenericProjectDirectLabour { get; set; }

        public DbSet<TakeOffLinePerProject> TakeOffLinePerProject { get; set; }
        public DbSet<TakeOffLineForKgPerProject> TakeOffLineForKgPerProject { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.UnitOfMeasurement> UnitOfMeasurement { get; set; }
        //public DbSet<ExceedERP.Core.Domain.Construction.ProjectType> ProjectTypes { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectTypeMapping> ProjectTypeMappings { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MaterialPriceIndex> MaterialPriceIndexIndices { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MaterialTransportationCost> MaterialTransportationCost { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.AreaWorkItem> AreaWorkItemes { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectEstimationAreaWorkItem> ProjectEstimationAreaWorkItem { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.SubContractWorkItem> SubContractWorkItem { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EstimationSubContractWorkItem> EstimationSubContractWorkItem { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.Block> Blocks { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.BlockMapping> BlockMappings { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.SubProjectType> SubProjectType { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.SubProjectStructureTypeMapping> SubProjectStructureTypeMapping { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.ConstructionItemCategory> ConstructionItemCategories { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.DeformedBar> DeformedBars { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectCrew> ProjectCrew { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.CrewHumanResource> CrewHumanResources { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.CrewMaterial> CrewMaterials { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.CrewEquipment> CrewEquipments { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.BarDiameter> BarDiameter { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.WorkItemDuration> WorkItemDuration { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ResourceEstimationLabour> ResourceEstimationLabour { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ResourceEstimationMaterial> ResourceEstimationMaterials { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ResourceEstimationEquipment> ResourceEstimationEquipments { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyFuelCost> MonthlyFuelCost { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.LaborRateIndexFactorAttribute> LaborRateIndexFactorAttributes { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.LaborRateIndexFactorAttributeName> LaborRateIndexFactorAttributeNames { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MethodOfWork> MethodOfWork { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ResourceUtilizationPeriod> ResourceUtilizationPeriods { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ResourceUtilizationDay> ResourceUtilizationDays { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ResourceUtilizationWorkItem> ResourceUtilizationWorkItem { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.DetailedWorkProgress> DetailedWorkProgress { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ResourceUtilizationMaterial> ResourceUtilizationMaterials { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ResourceUtilizationDirectLabour> ResourceUtilizationDirectLabours { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ResourceUtilizationEquipment> ResourceUtilizationEquipment { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.Road> Roads { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.RoadMapping> RoadMapping { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EarnedValueManagementTerm> EarnedValueManagementTerms { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EarnedValuePerProject> EarnedValuePerProject { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EarnedValuePerItem> EarnedValuePerItem { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EarnedValueManagementTermDetail> EarnedValueManagementTermDetail { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EVMTermOperandComponent> EVMTermOperandComponents { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EarnedValueManagementTermFormulaConstant> EarnedValueManagementTermFormulaConstants { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EarnedValueManagementPeriod> EarnedValueManagementPeriods { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.ConstructionPeriod> ConstructionPeriod { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ConstructionSubPeriod> ConstructionSubPeriod { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectProgress> MonthlyProjectProgress { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.PerformanceEvaluationComponentWeight> PerformanceEvaluationComponentWeight { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.PEVCostSetting> PEVCostSetting { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectQuality> MonthlyProjectQuality { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.QualityPlanning> QualityPlanning { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.QualityAssurance> QualityAssurance { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.QualityControl> QualityControl { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectQualityComplaint> MonthlyProjectQualityComplaints { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.PeriodMapping> PeriodMapping { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectCost> MonthlyProjectCost { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectPayment> MonthlyProjectPayment { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectPlanning> MonthlyProjectPlanning { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.RScheduleSubmissionAcToOperationManual> ScheduleSubmissionAcToOperationManual { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.RScheduleSubmissionAcToContract> RScheduleSubmissionAcToContract { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectPnanningCompleteness> ProjectPnanningCompleteness { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectPlanningQualityAndAcceptance> ProjectPlanningQualityAndAcceptance { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectClaim> MonthlyProjectClaim { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ContempraryRecord> ContempraryRecord { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.RequirementForMonthlyClaims> RequirementForMonthlyClaims { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.AnalysisTechniqueApplicationForMonthlyClaims> AnalysisTechniqueApplicationForMonthlyClaims { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ApprovalStatusForClaimsResponded> ApprovalStatusForClaimsResponded { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectReport> MonthlyProjectReport { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectReportCompletnessComponent> MonthlyProjectReportCompletnessComponent { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectReportNarrative> MonthlyProjectReportNarrative { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectCommunication> MonthlyProjectCommunication { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectCommunicationWithinProject> MonthlyProjectCommunicationWithinProject { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectCommunicationWithHeadOffice> MonthlyProjectCommunicationWithHeadOffice { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectCommunicationWithConsultant> MonthlyProjectCommunicationWithConsultant { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.MonthlyProjectPerformanceModifier> MonthlyProjectPerformanceModifier { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.PerformanceModifierHealthAndSafety> PerformanceModifierHealthAndSafety { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.PerformanceModifierBudgetRequest> PerformanceModifierBudgetRequest { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.PerformanceModifierBudgetUtilization> PerformanceModifierBudgetUtilization { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.PerformanceModifierMaterialUtilization> PerformanceModifierMaterialUtilization { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.PerformanceModifierSubContractAdministration> PerformanceModifierSubContractAdministration { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.DelayType> DelayTypes { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.WorkDelay> WorkDelay { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.DelayAnalysisTask> DelayAnalysisTasks { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.DelayTaskDependency> DelayTaskDependencies { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectPaymentAttribute> ProjectPaymentAttributes { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectAdvancePayment> ProjectAdvancePayments { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.WorkInProgress> WorkInProgress { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ConsPaymentAgingPeriod> ConsPaymentAgingPeriods { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ConstructionCustomerAccount> ConstructionCustomerAccounts { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.WorkInProgressValidation> WorkInProgressValidations { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.WorkInProgressDetail> WorkInProgressDetails { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.WorkInProgressDistribution> WorkInProgressDistribution { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.ConsReceivableInvoice> ConsReceivableInvoices { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ConsReceivableInvoiceValidation> ConsReceivableInvoiceValidation
        {
            get;
            set;
        }
        public DbSet<ExceedERP.Core.Domain.Construction.ConsReceivableInvoiceDetail> ConsReceivableInvoiceDetails { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ConsReceivableInvoiceDistribution> ConsReceivableInvoiceDistribution { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.ConsARStandardCollection> ConsARStandardCollections { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ConsARStandardCollectionDetail> ConsARStandardCollectionDetail
        {
            get;
            set;
        }
        public DbSet<ExceedERP.Core.Domain.Construction.ConsStandardCollectionDistribution> ConsStandardCollectionDistribution
        {
            get;
            set;
        }
        public DbSet<ExceedERP.Core.Domain.Construction.ConsARStandardCollectionValidation> ConsARStandardCollectionValidations
        {
            get;
            set;
        }

        public DbSet<ExceedERP.Core.Domain.Construction.ScheduleRevision> ScheduleRevisions { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ScheduleRevisionValidation> ScheduleRevisionValidations { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectMaterialExpense> ProjectMaterialExpenses { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectDirectMaterialExpense> ProjectDirectMaterialExpenses { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectIndirectMaterialExpense> ProjectIndirectMaterialExpenses { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.ProjectEstimationCustomWorkItem> ProjectEstimationCustomWorkItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EstimationCustomWorkItemMaterial> EstimationCustomWorkItemMaterial { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EstimationCustomWorkItemEquipment> EstimationCustomWorkItemEquipment { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EstimationCustomWorkItemDirectLabor> EstimationCustomWorkItemDirectLabor { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EstimationCustomWorkItemContent> EstimationCustomWorkItemContents { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.EstimationTemplateCustomWorkItemDetail> EstimationTemplateCustomWorkItemDetails { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectCustomWorkItem> ProjectCustomWorkItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.GenericProjectCustomWorkItemDetail> GenericProjectCustomWorkItemDetails { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectCustomWorkItemDetail> ProjectCustomWorkItemDetails { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectRepresentativeElement> ProjectRepresentativeElements { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.RepresentativeElementCostInBill> RepresentativeElementCostInBill { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.RepresentativeElementWeightRange> RepresentativeElementWeightRange { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.PriceAdjustmentWeight> PriceAdjustmentWeights { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.ProjectPriceAdjustmentFormulaType> ProjectPriceAdjustmentFormulaTypes { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.PriceAdjustmentWeightItem> PriceAdjustmentWeightItems { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.MaterialUtilizationDetail> MaterialUtilizationDetails { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.MaterialDemand> MaterialDemands { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.PurchaseSupplier> PurchaseSuppliers { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.PurchasePriceComparison> PurchasePriceComparison { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.CashFlowYear> CashFlowYear { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.CashFlowContractData> CashFlowContractData { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.CashFlowContractDataDetail> CashFlowContractDataDetail { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.CashFlowFinanceSource> CashFlowFinanceSources { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.CashFlowFinanceSourceDetail> CashFlowFinanceSourceDetail { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.CashFlowDeduction> CashFlowDeduction { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.CashFlowDeductionDetail> CashFlowDeductionDetail { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.NetOperationCashFlow> NetOperationCashFlow { get; set; }
        public DbSet<ExceedERP.Core.Domain.Construction.NetOperationCashFlowDetail> NetOperationCashFlowDetail { get; set; }

        public DbSet<ExceedERP.Core.Domain.Construction.ConstructionHoliday> ConstructionHolidays { get; set; }
        #endregion

        #region Production
        public DbSet<ExceedERP.Core.Domain.Production.ProductionProject> ProductionProjects { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.Plot> Plots { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionSite> ProductionSites { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionYear> ProductionYears { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionPeriod> ProductionPeriods { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionSetting> ProductionSettings { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionAndProcessingCostCategory> ProductionAndProcessingCostCategories { get; set; }
        // all the activities in the entire module
        public DbSet<ExceedERP.Core.Domain.Production.ProductionActivity> ProductionActivities { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.Crop> Crops { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.NaturalGum> NaturalGums { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.SeedProductionPlan> SeedProductionPlans { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.CostTrackingPeriod> CostTrackingPeriods { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProjectCostByMonth> ProjectCostByMonth { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProjectCostByMonthDetail> ProjectCostByMonthDetails { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProjectCostByMonthMaterial> ProjectCostByMonthMaterials { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.SeedProductionPlanYear> SeedProductionPlanYear { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.InputMaterialApplicationRate> InputMaterialApplicationRates { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.InputMaterialApplicationRateItem> InputMaterialApplicationRateItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionOrder> ProductionOrders { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.SeedProductionOrderItem> SeedProductionOrderItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.GumProductionOrderItem> GumProductionOrderItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionOrderActivitiy> ProductionOrderActivitiy { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.MachineryMaintenance> MachineryMaintenance { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionRawProductPurchase> ProductionRawProductPurchase { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionOrderInputMaterial> ProductionOrderInputMaterial { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionItemCategory> ProductionItemCategories { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProcessingPlant> ProcessingPlants { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.SeedProcessingOrder> SeedProcessingOrders { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.GumProcessingOrder> GumProcessingOrders { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.SeedProcessingOrderItem> SeedProcessingOrderItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.GumProcessingOrderItem> GumProcessingOrderItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProcessingOrderActivity> ProcessingOrderActivities { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProcessingInputMaterial> ProcessingInputMaterials { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionStoreReceive> ProductionStoreReceives { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionStoreReceiveItem> ProductionStoreReceiveItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionStoreReceiveValidation> ProductionStoreReceiveValidations { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionStoreReceiveDistribution> ProductionStoreReceiveDistributions { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionFinishedGoodsStoreReceive> ProductionFinishedGoodsStoreReceives { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionFinishedGoodsStoreReceiveItem> ProductionFinishedGoodsStoreReceiveItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionFinishedGoodsStoreReceiveValidation> ProductionFinishedGoodsStoreReceiveValidations { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionFinishedGoodsStoreReceiveDistribution> ProductionFinishedGoodsStoreReceiveDistributions { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.CleanedSeedSellingPrice> CleanedSeedSellingPrices { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.CleanedSeedSellingPriceItem> CleanedSeedSellingPriceItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.CleanedNaturalGumSellingPrice> CleanedNaturalGumSellingPrices { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.CleanedNaturalGumSellingPriceItem> CleanedNaturalGumSellingPriceItem { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.RawProductFinishedGoodMapping> RawProductFinishedGoodMapping { get; set; }

        public DbSet<ExceedERP.Core.Domain.Production.ProjectedMonthlyCashFlow> ProjectedMonthlyCashFlow { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProjectedMonthlyCashFlowItem> ProjectedMonthlyCashFlowItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.CleanedProductSale> CleanedProductSales { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.CleanedProductSaleItem> CleanedProductSaleItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.CleanedProductSaleItemPerMonth> CleanedProductSaleItemPerMonth { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.RawProductPurchase> RawProductPurchases { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.RawProductPurchaseItem> RawProductPurchaseItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.RawProductPurchaseItemPerMonth> RawProductPurchaseItemPerMonth { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.CustomCleaningAndStorage> CustomCleaningAndStorage { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.CustomCleaningAndStorageItem> CustomCleaningAndStorageItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.CustomCleaningAndStorageItemPerMonth> CustomCleaningAndStorageItemPerMonth { get; set; }

        public DbSet<ExceedERP.Core.Domain.Production.ProductionIssue> ProductionIssues { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionIssueItem> ProductionIssueItems { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionIssueValidation> ProductionIssueValidations { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.ProductionIssueDistribution> ProductionIssueDistributions { get; set; }
        public DbSet<ExceedERP.Core.Domain.Production.IssueAndProcessingOrderItemMapping> IssueAndProcessingOrderItemMappings { get; set; }

        #endregion

        #region Printing
        public DbSet<PrintingQualityInspection> PrintingQualityInspections { get; set; }
        public DbSet<PrintingTaskIssue> PrintingTaskIssues { get; set; }
        public DbSet<PrintingProductionAllottedRawMaterial> PrintingProductionAllottedRawMaterials { get; set; }
        public DbSet<PrintingDailyFollowUpLogBook> PrintingDailyFollowUpLogBooks { get; set; }
        public DbSet<PressDailyFollowUpWastage> PressDailyFollowUpWastages { get; set; }
        public DbSet<PrintingDailyFollowUpMachineMaintenance> PrintingDailyFollowUpMachineMaintenances { get; set; }
        public DbSet<PrintingSummaryComment> PrintingSummaryComments { get; set; }
        public DbSet<PrintingMachineDetail> PrintingMachineDetails { get; set; }
        public DbSet<PrintingJobOrderJobDesign> PrintingJobOrderJobDesigns { get; set; }
        public DbSet<PrintingJobOrderProductionJobDesign> PrintingJobOrderProductionJobDesigns { get; set; }
        public DbSet<PrintingJobOrderProduction> PrintingJobOrderProductions { get; set; }
        public DbSet<PrintingProductionMaterialCost> PrintingProductionMaterialCosts { get; set; }
        public DbSet<PrintingProductionLaborCost> PrintingProductionLaborCosts { get; set; }
        public DbSet<PrintingEstimationMargin> PrintingEstimationMargins { get; set; }
        public DbSet<PrintingJobOrder> PrintingJobOrders { get; set; }
        public DbSet<PrintingJobOrderItem> PrintingJobOrderItems { get; set; }
        public DbSet<PrintingProformaInvoice> PrintingProformaInvoices { get; set; }
        public DbSet<PrintingProformaInvoiceItem> PrintingProformaInvoiceItems { get; set; }
        public DbSet<PrintingEstimationSummary> PrintingEstimationSummaries { get; set; }
        public DbSet<PrintingEstimationDetail> PrintingEstimationDetails { get; set; }
        public DbSet<PrintingEstimationCostSummary> PrintingEstimationCostSummaries { get; set; }
        public DbSet<PrintingEstimationMaterialCost> PrintingEstimationMaterialCosts { get; set; }
        public DbSet<PrintingEstimationLaborCost> PrintingEstimationLaborCosts { get; set; }
        public DbSet<PrintingOverAllCost> PrintingOverAllCosts { get; set; }
        public DbSet<PrintingMaterialCategory> PrintingMaterialCategories { get; set; }
        public DbSet<PrintingMaterialCategoryItem> PrintingMaterialCategoryItems { get; set; }
        public DbSet<PrintingBindingStyle> PrintingBindingStyles { get; set; }
        public DbSet<PrintingJobType> PrintingJobTypes { get; set; }
        public DbSet<PrintCostCenter> PrintCostCenters { get; set; }
        public DbSet<PrintingMachineType> PrintingMachineTypes { get; set; }
        public DbSet<PrintingProcess> PrintingProcesses { get; set; }
        public DbSet<PrintItemCategory> PrintItemCategorys { get; set; }
        public DbSet<PrintLabourRate> PrintLabourRates { get; set; }
        public DbSet<PrintMachinePaperList> PrintMachinePaperLists { get; set; }
        public DbSet<PrintWorkOrderType> PrintWorkOrderTypes { get; set; }
        public DbSet<PrintingCostEstimation> PrintingCostEstimations { get; set; }
        public DbSet<PrintingProductProcess> PrintingProductProcesses { get; set; }
        public DbSet<PrintingPaperSize> PrintingPaperSizes { get; set; }
        public DbSet<PrintingMachineList> PrintingMachineLists { get; set; }
        #region  Estimation
        #region Printimg Estimation
        public DbSet<PrintingStep> PrintingSteps { get; set; }
        public DbSet<EstimationForm> EstimationForms { get; set; }
        public DbSet<MaterialCost> MaterialCosts { get; set; }
        public DbSet<LaborCost> LaborCosts { get; set; }
        public DbSet<OverallCost> OverallCosts { get; set; }
        public DbSet<PrintingCriteriaCategory> PrintingCriteriaCatagorys { get; set; }
        #endregion
        #endregion
        #region Production Follow UP
        public DbSet<YearlyProductionPlan> YearlyProductionPlans { get; set; }
        public DbSet<HalfYearlyProductionPlan> HalfYearlyProductionPlans { get; set; }
        public DbSet<QuarterlyProductionPlan> QuarterlyProductionPlans { get; set; }
        public DbSet<MonthlyProductionPlan> MonthlyProductionPlans { get; set; }
        public DbSet<WeeklyProductionPlan> WeeklyProductionPlans { get; set; }
        public DbSet<ProductionJobTypePlan> ProductionJobTypePlans { get; set; }
        public DbSet<ProductionCustomerType> ProductionCustomerTypes { get; set; }
        public DbSet<Proforma> Proformas { get; set; }
        public DbSet<ProductionYearlyPlan> ProductionYearlyPlans { get; set; }
        public DbSet<ProductionMonthlyPlan> ProductionMonthlyPlans { get; set; }
        public DbSet<ProductionMonthlyJobTypePlan> ProductionMonthlyJobTypePlans { get; set; }
        public DbSet<PFProformaItems> PFProformaItems { get; set; }
        public DbSet<JobCategory> JobCategories { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobStatus> JobStatuses { get; set; }
        public DbSet<ChangeOrder> ChangeOrders { get; set; }
        public DbSet<JobOrderMaterial> JobOrderMaterials { get; set; }
        public DbSet<PrintingProductionFinishedGoodsStoreReceive> PrintingProductionFinishedGoodsStoreReceives { get; set; }
        public DbSet<PrintingProductionFinishedGoodsStoreReceiveItem> PrintingProductionFinishedGoodsStoreReceiveItems { get; set; }
        public DbSet<PrintingProductionFinishedGoodsStoreReceiveValidation> PrintingProductionFinishedGoodsStoreReceiveValidations { get; set; }
        public DbSet<PrintingProductionFinishedGoodsStoreReceiveDistribution> PrintingProductionFinishedGoodsStoreReceiveDistributions { get; set; }
        public DbSet<FinishedGoods> FinishedGoods { get; set; }
        public DbSet<ProductDelivery> ProductDeliveries { get; set; }
        public DbSet<ProductDeliveryItems> ProductDeliveryItems { get; set; }
        public DbSet<JobStatusLineItem> JobStatusLineItems { get; set; }
        #endregion
        #region Job Costing
        //public DbSet<IdleTimeCategory> IdleTimeCategories { get; set; }
        //public DbSet<JcPension> JcPensions { get; set; }
        //public DbSet<YearlyOverHeadRate> YearlyOverHeadRates { get; set; }
        //public DbSet<Machine> Machines { get; set; }
        //public DbSet<Unit> Units { get; set; }
        //public DbSet<IdleTime> IdleTimes { get; set; }
        //public DbSet<IdleTimeOT> IdleTimeOTs { get; set; }
        //public DbSet<IdleTimeProductive> IdleTimeProductives { get; set; }
        //public DbSet<IdleTimeNonProductiveControllable> IdleTimeNonProductiveControllables { get; set; }
        //public DbSet<PrintingIdleTimeNonProductiveNonControllable> PrintingIdleTimeNonProductiveNonControllables { get; set; }
        //public DbSet<JobCost> JobCosts { get; set; }
        //public DbSet<JobChangeOfOrder> JobChangeOfOrders { get; set; }
        //public DbSet<JobLaborCost> JobLaborCosts { get; set; }
        //public DbSet<JobMaterialCost> JobMaterialCosts { get; set; }
        //public DbSet<JobMaterialReturn> JobMaterialReturns { get; set; }
        //public DbSet<FinishedJob> FinishedJobs { get; set; }
        //public DbSet<DeliveredJob> DeliveredJobs { get; set; }
        //public DbSet<DailyLaborRate> DailyLaborRates { get; set; }
        //public DbSet<DLRSalary> DLRSalaries { get; set; }
        //public DbSet<DLRHour> DLRHours { get; set; }
        //public DbSet<JobOrderTimeSheet> JobOrderTimeSheets { get; set; }
        //public DbSet<JobOrderTimeRegistration> JobOrderTimeRegistrations { get; set; }
        //public DbSet<JobOrderSummary> JobOrderSummarys { get; set; }
        //public DbSet<JobOrderSummaryCostCenter> JobOrderSummaryCostCenters { get; set; }
        //public DbSet<JobOrderSummaryCostCenterDetail> JobOrderSummaryCostCenterDetails { get; set; }
        ////public DbSet<PrintingIdleTimeNonProductiveNonControllable> PrintingIdleTimeNonProductiveNonControllable { get; set; }


        #endregion
        #endregion
        #region Manufucturing
        public DbSet<FurnitureBoqDocumentComment> FurnitureBoqDocumentComments { get; set; }

        #region  Estimation
        #region Furniture Estimation
        public DbSet<FurnitureProformaInvoice> FurnitureProformaInvoices { get; set; }
        public DbSet<FurnitureProformaInvoiceItem> FurnitureProformaInvoiceItems { get; set; }
        public DbSet<FurnitureJobOrderForm> FurnitureJobOrderForms { get; set; }
        public DbSet<FurnitureStep> FurnitureSteps { get; set; }
        public DbSet<FurnitureMaterialCost> FurnitureMaterialCosts { get; set; }
        public DbSet<FurnitureEstimationForm> FurnitureEstimationForms { get; set; }
        public DbSet<FurnitureLaborCost> FurnitureLaborCosts { get; set; }
        public DbSet<FurnitureOverallCost> FurnitureOverallCosts { get; set; }
        public DbSet<FurnitureCategory> FurnitureCategories { get; set; }
        public DbSet<DirectLaborCost> DirectLaborCosts { get; set; }

        #endregion
        #endregion
        #region Print Followup
        public DbSet<PrintingProductionFollowUp> PrintingProductionFollowUps { get; set; }
        public DbSet<PrintingProductionFollowUpTask> PrintingProductionFollowUpTasks { get; set; }
        public DbSet<PrintingProductionDailyFollowUpDetail> PrintingProductionDailyFollowUpDetails { get; set; }
        //public DbSet<PrintingDailyFollowUpDelayReason> PrintingDailyFollowUpDelayReasons { get; set; }
        //public DbSet<PrintingProductionAllottedRawMaterial> PrintingProductionAllottedRawMaterials { get; set; }
        #endregion
        #region Production Follow UP
        public DbSet<FurniturStandardJobDesign> FurniturStandardJobDesigns { get; set; }
        public DbSet<FurnitureJobDesign> FurnitureJobDesigns { get; set; }
        public DbSet<FurnitureBoqJobDesign> FurnitureBoqJobDesigns { get; set; }
        public DbSet<FurnitureBoqRequest> FurnitureBoqRequests { get; set; }
        public DbSet<FurnitureBoqDocument> FurnitureBoqDocuments { get; set; }
        public DbSet<EstimationSummary> EstimationSummaries { get; set; }
        public DbSet<EstimationDetail> EstimationDetails { get; set; }
        public DbSet<EstimationCostSummary> EstimationCostSummaries { get; set; }
        public DbSet<FurnitureJobOrderProduction> FurnitureJobOrderProductions { get; set; }
        public DbSet<FurnitureProductionBillOfMaterial> FurnitureProductionBillOfMaterials { get; set; }
        public DbSet<FurnitureProductionBillOfLabor> FurnitureProductionBillOfLabors { get; set; }
        public DbSet<FurnitureJobAssignment> FurnitureJobAssignments { get; set; }
        public DbSet<FurnitureDailyProductionFollowUp> FurnitureDailyProductionFollowUps { get; set; }
        public DbSet<FurnitureProductionDailyFollowUpDelayReason> FurnitureProductionDailyFollowUpDelayReasons { get; set; }
        public DbSet<FurnitureUnfinishedMaterialTransferForm> FurnitureUnfinishedMaterialTransferForms { get; set; }
        public DbSet<FurnitureStandardJobType> FurnitureStandardJobTypes { get; set; }
        public DbSet<StandardBillOfMaterial> StandardBillOfMaterials { get; set; }
        public DbSet<StandardBillOfLabor> StandardBillOfLabors { get; set; }
        public DbSet<FurnitureJobOrderItem> FurnitureJobOrderItems { get; set; }
        public DbSet<FurnitureBillOfQuantity> FurnitureBillOfQuantities { get; set; }
        public DbSet<FurnitureBOQMaterial> FurnitureBOQMaterials { get; set; }
        public DbSet<FurnitureBOQLabor> FurnitureBOQLabors { get; set; }
        public DbSet<ManifucturingTaskCategory> ManifucturingTaskCategories { get; set; }
        public DbSet<ManufacturingMaterialCategory> ManufacturingMaterialCategories { get; set; }
        public DbSet<ManufacturingMaterialCategoryItem> ManufacturingMaterialCategoryItems { get; set; }
        public DbSet<FurnitureYearlyProductionPlan> FurnitureYearlyProductionPlans { get; set; }

        public DbSet<FurnitureJobProcuctionPlan> FurnitureJobProcuctionPlans { get; set; }
        public DbSet<FurnitureHalfYearlyProductionPlan> FurnitureHalfYearlyProductionPlans { get; set; }
        public DbSet<FurnitureQuarterlyProductionPlan> FurnitureQuarterlyProductionPlans { get; set; }
        public DbSet<FurnitureMonthlyProductionPlan> FurnitureMonthlyProductionPlans { get; set; }
        public DbSet<FurnitureWeeklyProductionPlan> FurnitureWeeklyProductionPlans { get; set; }
        public DbSet<FurnitureProductionJobTypePlan> FurnitureProductionJobTypePlans { get; set; }
        public DbSet<FurnitureProductionCustomerType> FurnitureProductionCustomerTypes { get; set; }
        public DbSet<FurnitureProforma> FurnitureProformas { get; set; }
        public DbSet<FurnitureProductionYearlyPlan> FurnitureProductionYearlyPlans { get; set; }
        public DbSet<FurnitureProductionMonthlyPlan> FurnitureProductionMonthlyPlans { get; set; }
        public DbSet<FurnitureProductionMonthlyJobTypePlan> FurnitureProductionMonthlyJobTypePlans { get; set; }
        public DbSet<FurniturePFProformaItems> FurniturePFProformaItems { get; set; }
        public DbSet<FurnitureJobCategory> FurnitureJobCategories { get; set; }
        public DbSet<FurnitureJob> FurnitureJobs { get; set; }
        public DbSet<FurnitureJobStatus> FurnitureJobStatuses { get; set; }
        public DbSet<FurnitureChangeOrder> FurnitureChangeOrders { get; set; }
        public DbSet<FurnitureJobOrderMaterial> FurnitureJobOrderMaterials { get; set; }
        public DbSet<FurnitureProductionFinishedGoodsStoreReceive> FurnitureProductionFinishedGoodsStoreReceives { get; set; }
        public DbSet<FurnitureProductionFinishedGoodsStoreReceiveItem> FurnitureProductionFinishedGoodsStoreReceiveItems { get; set; }
        public DbSet<FurnitureProductionFinishedGoodsStoreReceiveValidation> FurnitureProductionFinishedGoodsStoreReceiveValidations { get; set; }
        public DbSet<FurnitureProductionFinishedGoodsStoreReceiveDistribution> FurnitureProductionFinishedGoodsStoreReceiveDistributions { get; set; }
        public DbSet<FurnitureFinishedGoods> FurnitureFinishedGoods { get; set; }
        public DbSet<FurnitureProductDelivery> FurnitureProductDeliveries { get; set; }
        public DbSet<FurnitureProductDeliveryItems> FurnitureProductDeliveryItems { get; set; }
        public DbSet<FurnitureJobStatusLineItem> FurnitureJobStatusLineItems { get; set; }
        #endregion    


        #region Job Costing
        public DbSet<FurnitureIdleTimeCategory> FurnitureIdleTimeCategories { get; set; }
        public DbSet<FurnitureJcPension> FurnitureJcPensions { get; set; }
        public DbSet<FurnitureYearlyOverHeadRate> FurnitureYearlyOverHeadRates { get; set; }
        public DbSet<FurnitureMachine> FurnitureMachines { get; set; }
        public DbSet<FurnitureUnit> FurnitureUnits { get; set; }
        public DbSet<FurnitureIdleTime> FurnitureIdleTimes { get; set; }
        public DbSet<FurnitureIdleTimeOT> FurnitureIdleTimeOTs { get; set; }
        public DbSet<FurnitureIdleTimeProductive> FurnitureIdleTimeProductives { get; set; }
        public DbSet<FurnitureIdleTimeNonProductiveControllable> FurnitureIdleTimeNonProductiveControllables { get; set; }
        public DbSet<FurnitureIdleTimeNonProductiveNonControllable> FurnitureIdleTimeNonProductiveNonControllables { get; set; }
        public DbSet<FurnitureJobCost> FurnitureJobCosts { get; set; }
        public DbSet<FurnitureJobChangeOfOrder> FurnitureJobChangeOfOrders { get; set; }
        public DbSet<FurnitureJobLaborCost> FurnitureJobLaborCosts { get; set; }
        public DbSet<FurnitureJobMaterialCost> FurnitureJobMaterialCosts { get; set; }
        public DbSet<FurnitureJobMaterialReturn> FurnitureJobMaterialReturns { get; set; }
        public DbSet<FurnitureFinishedJob> FurnitureFinishedJobs { get; set; }
        public DbSet<FurnitureDeliveredJob> FurnitureDeliveredJobs { get; set; }
        public DbSet<FurnitureDailyLaborRate> FurnitureDailyLaborRates { get; set; }
        public DbSet<FurnitureDLRSalary> FurnitureDLRSalaries { get; set; }
        public DbSet<FurnitureDLRHour> FurnitureDLRHours { get; set; }
        public DbSet<FurnitureJobOrderTimeSheet> FurnitureJobOrderTimeSheets { get; set; }
        public DbSet<FurnitureJobOrderTimeRegistration> FurnitureJobOrderTimeRegistrations { get; set; }
        public DbSet<FurnitureJobOrderSummary> FurnitureJobOrderSummarys { get; set; }
        public DbSet<FurnitureJobOrderSummaryCostCenter> FurnitureJobOrderSummaryCostCenters { get; set; }
        public DbSet<FurnitureJobOrderSummaryCostCenterDetail> FurnitureJobOrderSummaryCostCenterDetails { get; set; }

        #endregion
        #endregion
        #region Job Costing
        public DbSet<IdleTimeCategory> IdleTimeCategories { get; set; }
        public DbSet<JcPension> JcPensions { get; set; }
        public DbSet<YearlyOverHeadRate> YearlyOverHeadRates { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<IdleTime> IdleTimes { get; set; }
        public DbSet<IdleTimeOT> IdleTimeOTs { get; set; }
        public DbSet<IdleTimeProductive> IdleTimeProductives { get; set; }
        public DbSet<IdleTimeNonProductiveControllable> IdleTimeNonProductiveControllables { get; set; }
        public DbSet<IdleTimeNonProductiveNonControllable> IdleTimeNonProductiveNonControllables { get; set; }
        public DbSet<JobCost> JobCosts { get; set; }
        public DbSet<PrintingJobSetting> PrintingJobSetting { get; set; }
        public DbSet<JobChangeOfOrder> JobChangeOfOrders { get; set; }
        public DbSet<JobLaborCost> JobLaborCosts { get; set; }
        public DbSet<JobMaterialCost> JobMaterialCosts { get; set; }
        public DbSet<JobMaterialReturn> JobMaterialReturns { get; set; }
        public DbSet<FinishedJob> FinishedJobs { get; set; }
        public DbSet<DeliveredJob> DeliveredJobs { get; set; }
        public DbSet<DailyLaborRate> DailyLaborRates { get; set; }
        public DbSet<DLRSalary> DLRSalaries { get; set; }
        public DbSet<DLRHour> DLRHours { get; set; }
        public DbSet<JobOrderTimeSheet> JobOrderTimeSheets { get; set; }
        public DbSet<JobOrderTimeRegistration> JobOrderTimeRegistrations { get; set; }
        public DbSet<JobOrderSummary> JobOrderSummarys { get; set; }
        public DbSet<JobOrderSummaryCostCenter> JobOrderSummaryCostCenters { get; set; }
        public DbSet<JobOrderSummaryCostCenterDetail> JobOrderSummaryCostCenterDetails { get; set; }

        #endregion

        #region HRM
        public System.Data.Entity.DbSet<WeekendException> WeekendExceptions { get; set; }
        public System.Data.Entity.DbSet<EmployeePerformanceSummary> EmployeePerformanceSummary { get; set; }
        public System.Data.Entity.DbSet<LeavePayableSetting> LeavePayableSettings { get; set; }
        public System.Data.Entity.DbSet<LeavePayable> LeavePayables { get; set; }
        public System.Data.Entity.DbSet<LeavePayableDetail> LeavePayableDetails { get; set; }
        public System.Data.Entity.DbSet<SeveranceWithProbability> SeveranceWithProbabilities { get; set; }
        public System.Data.Entity.DbSet<SeverancePayableDetail> SeverancePayableDetails { get; set; }
        public System.Data.Entity.DbSet<SeverancePayable> SeverancePayables { get; set; }
        public System.Data.Entity.DbSet<SeverancePayableSetting> SeverancePayableSettings { get; set; }
        public System.Data.Entity.DbSet<SeveranceCalculationSetting> SeveranceCalculationSettings { get; set; }
        public System.Data.Entity.DbSet<LeaveTypeSetting> LeaveTypeSettings { get; set; }


        #region Company
        public System.Data.Entity.DbSet<ClearanceProviderDetail> ClearanceProviderDetails { get; set; }
        public System.Data.Entity.DbSet<ClearanceRequest> ClearanceRequests { get; set; }
        public System.Data.Entity.DbSet<ClearanceRequestApprover> ClearanceRequestApprovers { get; set; }
        public System.Data.Entity.DbSet<ClearanceRequestStatus> ClearanceRequestStatus { get; set; }
        public System.Data.Entity.DbSet<SalarySetting> SalarySettings { get; set; }
        public System.Data.Entity.DbSet<WaterMark> WaterMarks { get; set; }
        public System.Data.Entity.DbSet<ManPowerPlanningRequest> ManPowerPlanningRequests { get; set; }
        public System.Data.Entity.DbSet<BusinessUnit> BusinessUnits { get; set; }
        public System.Data.Entity.DbSet<MainCompany> Companies { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.Common.HRM.CompanyEmail> CompanyEmails { get; set; }
        public System.Data.Entity.DbSet<MainCompanyStructure> MainCompanyStructures { get; set; }
        public System.Data.Entity.DbSet<Branch> Branch { get; set; }
        public System.Data.Entity.DbSet<JobTitleMappingDetail> JobTitleMappingDetail { get; set; }
        public System.Data.Entity.DbSet<JobTitleMappingMain> JobTitleMappingMian { get; set; }
        public System.Data.Entity.DbSet<JobTitleMappingDetailTemp> JobTitleMappingDetailTemp { get; set; }
        public System.Data.Entity.DbSet<JobTitleMappingMainTemp> JobTitleMappingMainTemp { get; set; }
        public System.Data.Entity.DbSet<RegionNation> RegionNation { get; set; }
        public System.Data.Entity.DbSet<BranchCostCenter> BranchCostCenter { get; set; }
        public System.Data.Entity.DbSet<Announcment> Announcmentes { get; set; }
        public System.Data.Entity.DbSet<AnnouncmentDetail> AnnouncmentDetails { get; set; }
        public System.Data.Entity.DbSet<Holiday> Holidays { get; set; }
        public System.Data.Entity.DbSet<Weekend> Weekends { get; set; }
        public System.Data.Entity.DbSet<QuarterLength> QuarterLengths { get; set; }
        public System.Data.Entity.DbSet<Quarter> Quarters { get; set; }
        public System.Data.Entity.DbSet<DatePreferenceSetting> DatePreferenceSettings { get; set; }
        #endregion
        #region Payroll

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the over time. </summary>
        ///
        /// <value> The over time. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.BonusSetting> BonusSettings { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.OverTime> OverTimes { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.OverTimePerDay> OverTimePerDays { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.OverTimeTransfer> OverTimeTransfers { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.OverTimeTrasferDetail> OverTimeTrasferDetails { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.Setting.PerdimeSetting> PerdimeSetting { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.Setting.ContributionType> ContributionTypes { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.Setting.PayrollCostAllocationSetting> PayrollCostAllocationSetting { get; set; }

        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.PayrollPeriod> PayrollPeriods { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.ProvidentFund> ProvidentFunds { get; set; }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the loan. </summary>
        ///
        /// <value> The loan. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.Loan> Loans { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the regular contributions. </summary>
        ///
        /// <value> The regular contributions. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.RegularContribution> RegularContributions { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the fixed contributions. </summary>
        ///
        /// <value> The fixed contributions. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.FixedContribution> FixedContributions { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the associations. </summary>
        ///
        /// <value> The associations. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.Association> Associations { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the additions. </summary>
        ///
        /// <value> The additions. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public System.Data.Entity.DbSet<Addition> Additions { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.Bonus> Bonuses { get; set; }
        public DbSet<BackPay> BackPays { get; set; }
        public DbSet<BackPaymentLine> BackPaymentLines { get; set; }
        public DbSet<BackPaymentDetail> BackPaymentDetails { get; set; }
        public DbSet<BackPaymentDistribution> BackPaymentDistributions { get; set; }
        public DbSet<Advance> Advances { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.PayrollProcessLongTermDeductions> PayrollProcessLongTermDeductions { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.EmpPayrollServiceChargeViewModel> EmpPayrollServiceChargeViewModel { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.PeriodStatus> PeriodStatuses { get; set; }
        public System.Data.Entity.DbSet<EmpPayrollNoDaysWorked> EmpPayrollNoDaysWorkeds { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.Setting.PayrollEmployeeCostCenter> PayrollEmployeeCostCenter { get; set; }
        public System.Data.Entity.DbSet<AllowanceTypeRule> AllowanceTypeRule { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.PayrollProcessAllowances> PayrollProcessAllowances { get; set; }

        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.PayrollProcessAdditions> PayrollProcessAdditions { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.EmpPayrollDeduction> EmpPayrollDeductions { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.PayrollProcessDeductions> PayrollProcessDeductions { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.EmpPayrollLongTermDeduction> EmpPayrollLongTermDeductions { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.PayrollProcessLoans> PayrollProcessLoans { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.PayrollProcessAdvances> PayrollProcessAdvances { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.PayrollProcessFixedContribution> PayrollProcessFixedContribution { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.PayrollProcessRegularContribution> PayrollProcessRegularContribution { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.PayrollProcessViewModel> PayrollProcessViewModel { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.PayrollDistribution> PayrollDistributions { get; set; }


        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.Setting.PayrollEmployeeAccount> PayrollEmployeeAccounts { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Payroll.Setting.PayrollAccounts> PayrollAccounts { get; set; }
        public System.Data.Entity.DbSet<Delegation> Delegations { get; set; }

        #endregion
        #region Allowance
        public System.Data.Entity.DbSet<AllowanceDefination> AllowanceDefinations { get; set; }
        public System.Data.Entity.DbSet<AllowanceMapping> AllowanceMappings { get; set; }
        public System.Data.Entity.DbSet<CourtAllowanceMapping> CourtAllowanceMappings { get; set; }
        public System.Data.Entity.DbSet<AllowancePerEmployee> AllowancePerEmployees
        {
            get;
            set;
        }
        public System.Data.Entity.DbSet<OtherBenefit> OtherBenefits { get; set; }
        #endregion

        #region Attendance
        public System.Data.Entity.DbSet<Attendance> Attendances { get; set; }
        public System.Data.Entity.DbSet<WeeklyAttendance> WeeklyAttendances { get; set; }
        public System.Data.Entity.DbSet<LateComming> LateCommings { get; set; }
        public System.Data.Entity.DbSet<LateComingSummary> LateComingSummaries { get; set; }
        public System.Data.Entity.DbSet<Absence> Absence { get; set; }
        #endregion

        #region Employee Detail
        public System.Data.Entity.DbSet<EmployeeType> EmployeeTypes { get; set; }
        public System.Data.Entity.DbSet<EmployeeCodeSequenceSetting> EmployeeCodeSequenceSettings { get; set; }
        public System.Data.Entity.DbSet<EmployeeCodeSetting> EmployeeCodeSettings { get; set; }
        public System.Data.Entity.DbSet<ComplainResponse> ComplainResponses { get; set; }
        public System.Data.Entity.DbSet<Person_Employee> Person_Employee { get; set; }
        public System.Data.Entity.DbSet<Complain> Complains { get; set; }

        public System.Data.Entity.DbSet<Experience> Experiences { get; set; }
        public System.Data.Entity.DbSet<Commitment> Commitments { get; set; }
        public System.Data.Entity.DbSet<Competency> Competencies { get; set; }
        public System.Data.Entity.DbSet<JobTitleCode> JobTitleCodes { get; set; }
        public System.Data.Entity.DbSet<Termination> Terminations { get; set; }
        public System.Data.Entity.DbSet<ExitInterview> ExitInterviews { get; set; }
        public System.Data.Entity.DbSet<Disability> Disabilities { get; set; }
        public System.Data.Entity.DbSet<Placement> Placements { get; set; }
        public System.Data.Entity.DbSet<PlacementHistory> PlacementHistories { get; set; }
        public System.Data.Entity.DbSet<PlacementAfterSystem> PlacementAfterSystems { get; set; }
        public System.Data.Entity.DbSet<Cosign> Cosigns { get; set; }
        public System.Data.Entity.DbSet<Committee> Committees { get; set; }
        public System.Data.Entity.DbSet<CommitteeMember> CommitteeMembers { get; set; }
        public System.Data.Entity.DbSet<CommitteMinute> CommitteMinutes { get; set; }
        public System.Data.Entity.DbSet<Clearance> Clearances { get; set; }
        public System.Data.Entity.DbSet<ApplicantionForm> ApplicantionFormes { get; set; }

        public System.Data.Entity.DbSet<Emergency> Emergencies { get; set; }
        public System.Data.Entity.DbSet<Contract> Contract { get; set; }
        public System.Data.Entity.DbSet<EmployeeAssociation> EmployeeAssociation { get; set; }
        public System.Data.Entity.DbSet<EmployeeCourtAssociation> EmployeeCourtAssociation { get; set; }
        #endregion

        #region Leave Management
        public System.Data.Entity.DbSet<LeaveEntitlementSetting> LeaveEntitlementSettings { get; set; }
        public System.Data.Entity.DbSet<LeaveEntitleStartingDate> LeaveEntitleStartingDates { get; set; }
        public System.Data.Entity.DbSet<LeaveNewAwajStartYear> LeaveNewAwajStartYears { get; set; }
        public System.Data.Entity.DbSet<MonthlyLeavePlan> MonthlyLeavePlans { get; set; }
        public System.Data.Entity.DbSet<MonthlyLeavePlanDetail> MonthlyLeavePlanDetails { get; set; }
        public System.Data.Entity.DbSet<Leave> Leaves { get; set; }
        public System.Data.Entity.DbSet<LeaveEntitle> LeavesEntitle { get; set; }
        public DbSet<LeaveEntitlementHistory> LeaveEntitlementHistories { get; set; }

        public System.Data.Entity.DbSet<LeaveTransfer> LeaveTransfer { get; set; }
        public System.Data.Entity.DbSet<LeavePaid> LeavePaid { get; set; }
        public System.Data.Entity.DbSet<LeaveUsageTrackingForVoid> LeaveUsageTrackingForVoids { get; set; }
        #endregion

        #region Lettres
        public System.Data.Entity.DbSet<Incomming> Incomming { get; set; }
        public System.Data.Entity.DbSet<Outgoing> Outgoing { get; set; }
        public System.Data.Entity.DbSet<OutgoingStatus> OutgoingStatus { get; set; }
        public System.Data.Entity.DbSet<IncommingStatus> IncommingStatus { get; set; }
        #endregion

        #region PerformanceManagment
        public DbSet<MainCompanyEvalutionSetting> MainCompanyEvalutionSettings { get; set; }
        public System.Data.Entity.DbSet<PerformanceSetting> PerformanceSettings { get; set; }
        public System.Data.Entity.DbSet<CriteriaCategory> CriteriaCategories { get; set; }
        public System.Data.Entity.DbSet<InternalCustomerDetail> InternalCustomerDetails { get; set; }
        public DbSet<PerformanceRequestPeriod> PerformanceRequestPeriods { get; set; }
        public DbSet<PeerToPeerPerformanceEvaluation> PeerToPeerPerformanceEvaluations { get; set; }
        public DbSet<InternalCustomerPerformanceEvaluation> InternalCustomerPerformanceEvaluations { get; set; }
        public DbSet<InternalCustomer> InternalCustomers { get; set; }
        public DbSet<InternalCustomerEmployeePerformance> InternalCustomerEmployeePerformances { get; set; }
        public DbSet<InternalCustomerEmployeeMeasurement> InternalCustomerEmployeeMeasurements { get; set; }
        public DbSet<InternalCustomerEmployeePerformanceScore> InternalCustomerEmployeePerformanceScores { get; set; }
        public DbSet<PeerToPeerEmployeePerformance> PeerToPeerEmployeePerformances { get; set; }
        public DbSet<PeerEmployeeMeasurement> PeerEmployeeMeasurements { get; set; }
        public DbSet<PeerToPeerEmployeePerformanceScore> PeerToPeerEmployeePerformanceScores { get; set; }
        public DbSet<EvaluationCriteriaActionType> EvaluationCriteriaActionTypes { get; set; }
        public DbSet<EmployeePerformanceScore> EmployeePerformanceScores { get; set; }
        public DbSet<ProbationPerformanceSetting> ProbationPerformanceSettings { get; set; }
        public DbSet<EmployeeProbationSetting> EmployeeProbationSettings { get; set; }
        public DbSet<MeasurementDefinition> MeasurementDefinitions { get; set; }
        public DbSet<Measurement> Measurements { get; set; }
        public DbSet<PerformanceAction> PerformanceActions { get; set; }
        public DbSet<EmployeePerformance> EmployeePerformances { get; set; }
        public DbSet<EmployeeMeasurementDefinition> EmployeeMeasurementDefinition { get; set; }
        public DbSet<EmployeeMeasurement> EmployeeMeasurement { get; set; }
        public DbSet<EmployeePerformanceAction> EmployeePerformanceAction { get; set; }
        public DbSet<PerformanceRank> PerformanceRank { get; set; }
        //public DbSet<DepartmentDefineMeasurment> DepartmentDefineMeasurments { get; set; }
        //public DbSet<DepartmentMeasurment> DepartmentMeasurments { get; set; }
        //public DbSet<DepartmentPerformanceAction> DepartmentPerformanceActions { get; set; }
        public System.Data.Entity.DbSet<EmployeePeerToPeerAndInternalCustomerPerformanceSetting> EmployeePeerToPeerAndInternalCustomerPerformanceSettings { get; set; }

        #endregion

        #region Plan
        public DbSet<TrainingNeedRequest> TrainingNeedRequests { get; set; }

        public System.Data.Entity.DbSet<LeavePlan> LeavePlans { get; set; }
        public System.Data.Entity.DbSet<LeavePlanValidation> LeavePlanValidation { get; set; }
        public System.Data.Entity.DbSet<LeavePlanFinalValidation> LeavePlanFinalValidation { get; set; }

        public System.Data.Entity.DbSet<RecruitmentPlan> RecruitmentPlans { get; set; }
        public System.Data.Entity.DbSet<TrainingPlan> TrainingPlans { get; set; }
        public System.Data.Entity.DbSet<Vacancy> VaccancyPlans { get; set; }

        public System.Data.Entity.DbSet<ApplicantDetail> ApplicantDetails { get; set; }
        public System.Data.Entity.DbSet<ExamTypeSetting> ExamTypeSettings { get; set; }
        public System.Data.Entity.DbSet<CheckUp> CheckUps { get; set; }
        public System.Data.Entity.DbSet<ApplicantRegistration> ApplicantRegistration { get; set; }
        public System.Data.Entity.DbSet<ApplicantExamResult> ApplicantExamResult { get; set; }
        //public System.Data.Entity.DbSet<ApplicantScreeningResult> ApplicantScreeningResult { get; set; }
        public System.Data.Entity.DbSet<ApplicantScreenResult> ApplicantScreenResults { get; set; }
        public System.Data.Entity.DbSet<InternalApplicantScreening> InternalApplicantScreenings { get; set; }
        public System.Data.Entity.DbSet<InternalApplicantScreeningResult> InternalApplicantScreeningResults { get; set; }
        public System.Data.Entity.DbSet<InternalApplicantExamResult> InternalApplicantExamResults { get; set; }

        public System.Data.Entity.DbSet<ApplicantScreening> ApplicantScreenings { get; set; }
        #endregion

        #region Prohibition
        public System.Data.Entity.DbSet<Prohibition> Prohibitions { get; set; }
        #endregion

        #region Range

        #endregion

        #region RecruitmentManagment
        public System.Data.Entity.DbSet<EmployeeTransferRequest> EmployeeTransferRequests { get; set; }
        public System.Data.Entity.DbSet<TransferRequestDetail> TransferRequestDetails { get; set; }
        public System.Data.Entity.DbSet<TransferRequestValidation> TransferRequestValidations { get; set; }
        public System.Data.Entity.DbSet<TransferRequestDepartmentValidation> TransferRequestDepartmentValidations { get; set; }
        public DbSet<Recruitment> Recruitments { get; set; }
        public DbSet<RecruitmentVacancy> RecruitmentVacancy { get; set; }
        public DbSet<ClearanceProvider> ClearanceProvideres { get; set; }
        public DbSet<ApplicantEducation> ApplicantEducation { get; set; }


        public DbSet<RecruitmetnInterviewerPerseon> RecruitmetnInterviewerPerseons { get; set; }

        public DbSet<ExperienceOrganizationTypeSetting> ExperienceOrganizationTypeSettings { get; set; }
        public DbSet<ScreeningSetting> ScreeningSettings { get; set; }
        public DbSet<ExperienceJobTitle> ExperienceJobTitles { get; set; }

        public DbSet<ScreeningEducationLevel> ScreeningEducationLevels { get; set; }
        public DbSet<ScreeningEducationField> ScreeningEducationFields { get; set; }
        public DbSet<ScreeningExperienceJobtitle> ScreeningExperienceJobtitles { get; set; }
        public DbSet<ScreeningExperienceJobtitleForInternal> ScreeningExperienceJobtitleForInternals { get; set; }
        public DbSet<ApplicantCertefication> ApplicantCertefications { get; set; }
        public DbSet<RecruitmentApplicantPassSetting> RecruitmentApplicantPassSettings { get; set; }

        public DbSet<InternalApplicants> InternalApplicants { get; set; }
        //public DbSet<InternalApplicantEducation> InternalApplicantEducations { get; set; }
        //public DbSet<InternalApplicantExperience> InternalApplicantExperiences { get; set; }
        #endregion
        #region Salary Structure
        public System.Data.Entity.DbSet<Rank> Ranks { get; set; }
        public System.Data.Entity.DbSet<Step> Steps { get; set; }

        public System.Data.Entity.DbSet<Salary> Salaries { get; set; }
        public System.Data.Entity.DbSet<SalaryJobTitle> SalariesJobTitles { get; set; }

        #endregion

        #region Training and Education
        public DbSet<TrainingEvaluationBefore> TrainingEvaluationBefores { get; set; }
        public DbSet<TrainingEvaluationAfter> TrainingEvaluationAfters { get; set; }
        public System.Data.Entity.DbSet<NeedAssessmentSummary> NeedAssessmentSummary { get; set; }
        public System.Data.Entity.DbSet<NeedAssessmentSummaryDetail> NeedAssessmentSummaryDetails { get; set; }
        public DbSet<Training> EmpTrainingEducations { get; set; }
        public DbSet<NeedAssessment> NeedAssessments { get; set; }
        public DbSet<EmployeesInAssessment> EmployeesInAssessments { get; set; }
        public DbSet<CourseTracking> CourseTrackings { get; set; }
        public DbSet<Education> Education { get; set; }
        public DbSet<Trainee> Trainee { get; set; }
        public DbSet<TrainingStatus> TrainingStatuses { get; set; }
        public DbSet<TrainingFeedback> TrainingFeedbacks { get; set; }
        public DbSet<TrainingEvaluationCriteriaCategory> TrainingEvaluationCriteriaCategories { get; set; }
        public DbSet<TrainingEvaluationCriteria> TrainingEvaluationCriterias { get; set; }
        public DbSet<TrainingEvaluation> TrainingEvaluations { get; set; }
        #endregion

        #region Discipline

        public System.Data.Entity.DbSet<DisciplineMeasures> DisciplineMeasures { get; set; }
        public System.Data.Entity.DbSet<Appeal> Appeal { get; set; }
        public System.Data.Entity.DbSet<DisciplineMeasuresDetail> DisciplineMeasuresDetail { get; set; }
        public System.Data.Entity.DbSet<DisciplineRecord> DisciplineRecord { get; set; }
        public System.Data.Entity.DbSet<DisciplineMeasuresType> DisciplineMeasuresType { get; set; }
        public System.Data.Entity.DbSet<EmpDisciplinaryHearing> EmpDisciplinaryHearing { get; set; }
        public System.Data.Entity.DbSet<EmpDisciplineMeasure> EmpDisciplineMeasure { get; set; }
        public System.Data.Entity.DbSet<DisciplinaryCase> DisciplinaryCases { get; set; }
        public System.Data.Entity.DbSet<DisciplineMeasuerByMoney> DisciplineMeasuerByMoney { get; set; }
        #endregion


        #region Self Care
        public DbSet<TrainingApplicant> TrainingApplicants { get; set; }
        public DbSet<EducationRequest> EducationRequests { get; set; }
        public DbSet<MedicalRequest> MedicalRequest { get; set; }
        public DbSet<LeaveRequest> LeaveRequest { get; set; }
        public DbSet<LeaveRequestAttachement> LeaveRequestAttachements { get; set; }
        public DbSet<RecruitmentRequest> RecruitmentRequest { get; set; }
        public DbSet<RequestesStatus> RequestesStatus { get; set; }
        public DbSet<ApplicantExperience> ApplicantExperience { get; set; }
        public DbSet<ApplicantTraining> ApplicantTraining { get; set; }
        ///publty.DbSet<PromotionRequest> PromotionRequest { get; set; }
        public DbSet<OverTimeRequest> OverTimeRequest { get; set; }
        public DbSet<OverTimeRequestEmployee> OverTimeRequestEmployee { get; set; }
        #endregion

        #region Company
        public System.Data.Entity.DbSet<JobTitleMapping> JobTitleMapping { get; set; }
        public System.Data.Entity.DbSet<JobTitleMappingTemp> JobTitleMappingTemp { get; set; }
        public System.Data.Entity.DbSet<Structure> Structure { get; set; }
        #endregion

        public System.Data.Entity.DbSet<DelegationReason> DelegationReasons { get; set; }
        public System.Data.Entity.DbSet<DeligationCC> DelegationCCs { get; set; }
        public System.Data.Entity.DbSet<SickLeaveSetting> SickLeaveSettings { get; set; }
        public System.Data.Entity.DbSet<ComplainDetail> ComplainDetails { get; set; }
        public System.Data.Entity.DbSet<ComplainCC> ComplainCCs { get; set; }
        public System.Data.Entity.DbSet<ProbationPerformance> ProbationPerformances { get; set; }
        public System.Data.Entity.DbSet<ProbationPerformanceDetail> ProbationPerformanceDetails { get; set; }
        public System.Data.Entity.DbSet<LeaveReturn> LeaveReturns { get; set; }
        public DbSet<RetirementSetting> RetirementSettings { get; set; }
        public System.Data.Entity.DbSet<Template> Templates { get; set; }
        public System.Data.Entity.DbSet<TemplateTypeSetting> TemplateTypeSettings { get; set; }
        public System.Data.Entity.DbSet<ExperienceRequest> ExprianceRequests { get; set; }

        public System.Data.Entity.DbSet<OverTimeJobResult> OverTimeJobResults { get; set; }
        #endregion

        #region Archive Management
        public DbSet<SubjectType> SubjectTypes { get; set; }
        public DbSet<InstitutionIncomingLetter> InstitutionIncomingLetters { get; set; }
        public DbSet<EmployeeIncomingLetter> EmployeeIncommingLetter { get; set; }
        public DbSet<InstitutionOutgoingLetter> InstitutionOutgoingLetters { get; set; }
        public DbSet<PersonalArchiveNo> PersonalArchiveNo { get; set; }
        public DbSet<InstitutionArchiveNo> InstitutionArchiveNo { get; set; }
        public DbSet<InstitutionType> InstitutionType { get; set; }
        public DbSet<LetterStatus> LetterStatus { get; set; }
        public DbSet<LetterSender> LetterSender { get; set; }
        public DbSet<EmployeeLetterSender> EmployeeLetterSender { get; set; }
        public DbSet<ArchiveAttachement> ArchiveAttachements { get; set; }
        public DbSet<InstitutionIncomingAnswer> InstitutionIncomingAnswers { get; set; }
        public DbSet<EmployeeFolderAttachment> EmployeeFolderAttachments { get; set; }
        public DbSet<EmployeeIncomingAnswer> EmployeeIncomingAnswers { get; set; }
        public DbSet<EmployeeOutgoingLetter> EmployeeOutgoingLetters { get; set; }
        public DbSet<EmployeeOutgoingAnswer> EmployeeOutgoingAnswers { get; set; }
        public DbSet<CompanyDocumentType> CompanyDocumentTypes { get; set; }
        public DbSet<CompanyDocument> CompanyDocuments { get; set; }
        public DbSet<LetterReceiver> LetterReceivers { get; set; }
        public DbSet<LetterForMultipleStaff> LetterFolMultipleStaffs { get; set; }
        public DbSet<AttachedLettersForStaff> AttachedLettersForStaffs { get; set; }

        public DbSet<OutGoing_DepartmentToDepartment> OutGoing_DepartmentToDepartments { get; set; }
        public DbSet<OutGoing_DepartmentToDepartment_Answer> OutGoing_DepartmentToDepartment_Answers { get; set; }
        public DbSet<Attachment_DepartmentToDepartment> Attachment_DepartmentToDepartments { get; set; }
        public DbSet<Receiver_DepartmentToDepartment> Receiver_DepartmentToDepartments { get; set; }
        public DbSet<DepartmentFolder> DepartmentFolders { get; set; }
        public DbSet<OutGoing_DepartmentToStaff> OutGoing_DepartmentToStaffs { get; set; }
        public DbSet<Attachment_DepartmentToStaff> Attachment_DepartmentToStaffs { get; set; }
        public DbSet<Receiver_DepartmentToStaff> Receiver_DepartmentToStaffs { get; set; }
        public DbSet<Answer_DepartmentToStaff> Answer_DepartmentToStaffs { get; set; }
        public DbSet<Attachment_Answer_DepartmentToDepartment> Attachment_Answer_DepartmentToDepartments { get; set; }
        public DbSet<Attachment_Answer_DepartmentToStaff> Attachment_Answer_DepartmentToStaffs { get; set; }
        public DbSet<Attachment_Answer_InstitutionIncoming> Attachment_Answer_InstitutionIncomings { get; set; }
        public DbSet<Attachment_Answer_LetterSentForEmployee> Attachment_Answer_LetterSentForEmployees { get; set; }
        public DbSet<SubjectTypeForDepartmentLevel> SubjectTypeForDepartmentLevels { get; set; }
        public DbSet<DepartmentOrSubjectTypeSelectionForFolderCreation> DepartmentOrSubjectTypeSelectionForFolderCreations { get; set; }
        #endregion

        #region APTS
        public System.Data.Entity.DbSet<DrugToTransferTemporaryLists> DrugToTransferTemporaryLists { get; set; }
        public System.Data.Entity.DbSet<DrugStoreTransfer> DrugStoreTransfers { get; set; }
        // public DbSet<LaboratoryCatagory> LaboratoryCatagories { get; set; }
        //public DbSet<LaboratoryTests> LaboratoryTests { get; set; }
        // public DbSet<LaboratoryCatagoryPatientCard> LaboratoryCatagoryPatientCards { get; set; }
        //public DbSet<LaboratoryTestPatientCard> LaboratoryTestPatientCards { get; set; }
        public DbSet<LaboratoryDrugRequest> LaboratoryDrugRequests { get; set; }
        public DbSet<LaboratoryDrugRequestItem> LaboratoryDrugRequestItems { get; set; }
        public DbSet<EmergencyDrugRequest> EmergencyDrugRequests { get; set; }
        public DbSet<EmergencyDrugRequestItem> EmergencyDrugRequestItems { get; set; }
        public DbSet<DrugLists> DrugLists { get; set; }
        public DbSet<DrugUnit> DrugUnits { get; set; }
        public DbSet<DrugReceivalModel19> DrugReceivalModel19s { get; set; }
        public DbSet<Model22> Model22s { get; set; }
        public DbSet<PatientRegister> PatientRegisters { get; set; }
        public DbSet<PatientRegisterDetail> PatientRegisterDetails { get; set; }
        public DbSet<RequisitionModel20> RequisionModel20s { get; set; }
        public DbSet<DrugStore> DrugStores { get; set; }
        public DbSet<DrugStoreItem> DrugStoreItems { get; set; }
        public DbSet<DespensaryStore> DespensaryStores { get; set; }
        public DbSet<DrugRequest> DrugRequests { get; set; }
        public DbSet<DrugRequestItem> DrugRequestItems { get; set; }
        public DbSet<APTSRequestStatus> APTSRequestStatus { get; set; }
        public DbSet<DrugPurchaseRequisition> DrugPurchaseRequisitions { get; set; }
        public DbSet<DrugReceivalDetail> DrugReceivalDetails { get; set; }
        public DbSet<APTSValidation> APTSValidations { get; set; }
        #endregion

        #region FleetManagement


        #region 1 - Fleet Module
        public DbSet<MaintenanceCostDetail> MaintenanceCostDetail { get; set; }
        public DbSet<FleetAvailabilityTreshold> FleetAvailabilityTreshold { get; set; }
        public DbSet<FleetAvailabilityTresholdValidation> FleetAvailabilityTresholdValidation { get; set; }
        public DbSet<EquipmentOperationPlan> EquipmentOperationPlan { get; set; }
        public DbSet<EquipmentOperationPlanValidation> EquipmentOperationPlanValidation { get; set; }
        public DbSet<InternalRentalRate> InternalRentalRate { get; set; }
        public DbSet<TimeSheetException> TimeSheetException { get; set; }
        public DbSet<HeavyORDumpTruck> HaeavyOrDumpTruckSelectors { get; set; }
        public DbSet<RoadORLoad> RoadORLoads { get; set; }
        public DbSet<RateOfRoadORLoad> RateOfRoadORLoads { get; set; }
        public DbSet<OwningandOperatingCost> OwningandOperatingCosts { get; set; }
        public DbSet<FleetPettyCashType> FleetPettyCashTypes { get; set; }
        public DbSet<OwningandOperatingCostConstant> OwningandOperatingCostConstants { get; set; }
        public DbSet<LicenseLevel> LicenseLevels { get; set; }
        public DbSet<FleetPettyCash> FleetPettyCashes { get; set; }
        public DbSet<FleetIFuelConsumption> FleetIFuelConsumptions { get; set; }
        public DbSet<Accident> Accident { get; set; }
        public DbSet<FleetInsuranceTypeDetail> FleetInsuranceTypeDetails { get; set; }
        public DbSet<Accessories> FleetAccessories { get; set; }
        public DbSet<EquipmentIdentity> EquipmentIdentity { get; set; }
        public DbSet<EquipmentType> EquipmentType { get; set; }
        public DbSet<EquipmentApproval> EquipmentApproval { get; set; }
        public DbSet<Fleets> Fleets { get; set; }

        public DbSet<EquipmentBattery> EquipmentBattery { get; set; }
        public DbSet<EquipmentArrival> EquipmentArrival { get; set; }
        public DbSet<EquipmentArrivalApproval> EquipmentArrivalApproval { get; set; }
        public DbSet<EquipmentBatteryLog> EquipmentBatteryLog { get; set; }
        public DbSet<EquipmentTyreLog> EquipmentTyreLog { get; set; }
        public DbSet<FuelConsumptionAlertLevel> FuelConsumptionAlertLevel { get; set; }
        public DbSet<FleetHistory> FleetHistorys { get; set; }
        public DbSet<FleetInsurance> FleetInsurance { get; set; }
        public DbSet<FleetInsuranceValidation> FleetInsuranceValidation { get; set; }
        public DbSet<FleetInsurancePaymentRequestValidation> FleetInsurancePaymentRequestValidation { get; set; }
        public DbSet<FleetInsurancePaymentRequest> FleetInsurancePaymentRequest { get; set; }
        public DbSet<InsuranceCompany> InsuranceCompany { get; set; }
        public DbSet<RentalCompany> RentalCompany { get; set; }
        public DbSet<RentalInspectionCriteria> RentalInspectionCriteria { get; set; }
        public DbSet<RentalInspectionCriteriaDetail> RentalInspectionCriteriaDetails { get; set; }
        public DbSet<RentalMVInspection> RentalMVInspections { get; set; }
        public DbSet<RentalInspection> RentalInspections { get; set; }
        public DbSet<RentalAdvancePaymentRequest> RentalAdvancePaymentRequest { get; set; }
        public DbSet<RentalPaymentRequest> RentalPaymentRequests { get; set; }
        public DbSet<RentalAdvancePaymentRequestValidation> RentalAdvancePaymentRequestValidation { get; set; }
        public DbSet<RentalAdvancePaymentRequestDepartmentValidation> RentalAdvancePaymentRequestDepartmentValidation { get; set; }
        public DbSet<RentalAdvancePaymentRequestBudgetValidation> RentalAdvancePaymentRequestBudgetValidation { get; set; }
        public DbSet<RentalPaymentRequestLine> RentalPaymentRequestLines { get; set; }

        public DbSet<RentalPaymentRequestValidation> RentalPaymentRequestValidations { get; set; }
        public DbSet<RentalPaymentRequestDepartmentValidation> RentalPaymentRequestDepartmentValidations { get; set; }
        public DbSet<RentalPaymentRequestBudgetValidation> RentalPaymentRequestBudgetValidations { get; set; }
        // public DbSet<RentalPaymentRequestFinanceValidation> RentalPaymentRequestFinanceValidations { get; set; }



        public DbSet<RentalInspectionValidation> RentalInspectionValidations { get; set; }
        //public DbSet<FleetInsurances> FleetInsurances { get; set; }
        public DbSet<Operator> Operator { get; set; }
        public DbSet<TimeSheet> TimeSheet { get; set; }
        public DbSet<EquipmentTyre> EquipmentTyres { get; set; }
        public DbSet<EquipmentLocation> EquipmentLocations { get; set; }
        public DbSet<TimeSheetApproval> TimeSheetApprovals { get; set; }
        public DbSet<CouponFuel> CouponFuel { get; set; }
        public DbSet<DriverAssignment> DriverAssignments { get; set; }
        public DbSet<EquipmentName> EquipmentName { get; set; }
        public DbSet<FuelInventory> FuelInventorys { get; set; }
        public DbSet<OperatorPosition> OperatorPosition { get; set; }
        public DbSet<Educationallevel> Educationallevel { get; set; }
        public DbSet<Manufacturer> Manufacturer { get; set; }
        public DbSet<AssignedEquipment> AssignedEquipment { get; set; }
        public DbSet<EquipmentAttachment> EquipmentAttachment { get; set; }
        public DbSet<HRFleetMapping> HRFleetMapping { get; set; }
        public DbSet<FleetCondition> FleetConditions { get; set; }
        public DbSet<OperatorLicense> OperatorLicense { get; set; }
        public DbSet<LicenseType> LicenseTypes { get; set; }
        public DbSet<FleetReturn> FleetReturns { get; set; }
        public DbSet<FleetReturnItem> FleetReturnItems { get; set; }
        public DbSet<FleetReturnApproval> FleetReturnApprovals { get; set; }
        public DbSet<DryLoadingTimeSheet> DryLoadingTimeSheets { get; set; }
        public DbSet<DumpTruckTimeSheet> DumpTruckTimeSheets { get; set; }
        public DbSet<TimeSheetConsumptionItem> TimeSheetConsumptionItem { get; set; }
        public DbSet<MachineryWorkingTimeSheet> MachineryWorkingTimeSheets { get; set; }
        public DbSet<WagonDrillTimesheet> WagonDrillTimesheets { get; set; }

        #endregion


        #region 2 - Fuel Module

        public DbSet<Fuel> Fuel { get; set; }
        public DbSet<FuelIssueApproval> FuelIssueApproval { get; set; }
        public DbSet<FuelType> FuelTypes { get; set; }

        #endregion


        #region 3 - Inspection And Recommendation Module
        public DbSet<CheckList> CheckLists { get; set; }
        public DbSet<FleetCheckList> FleetCheckLists { get; set; }
        public DbSet<EquipmentCheckList> EquipmentCheckLists { get; set; }
        public DbSet<FleetLineCheckList> FleetLineCheckLists { get; set; }
        public DbSet<LineCheckList> LineCheckLists { get; set; }
        public DbSet<CheckListLog> CheckListLogs { get; set; }
        public DbSet<InspectionLog> InspectionLogs { get; set; }
        public DbSet<InspectionApproval> InspectionApprovals { get; set; }

        public DbSet<AnnualEquipmentInventoryInspection> AnnualEquipmentInventoryInspection { get; set; }
        public DbSet<InspectionStatusKey> InspectionStatusKeys { get; set; }
        public DbSet<EquipmentPlanandEvaluations> EquipmentPlanandEvaluation { get; set; }
        public DbSet<EquipmentPlanEvaluationApproval> EquipmentPlanEvaluationApprovals { get; set; }

        public DbSet<AnnualInventoryInspectionApproval> AnnualInventoryInspectionApproval { get; set; }

        #endregion

        #region 5 - Recycle Module

        public DbSet<Cannibalization> Cannibalization { get; set; }
        public DbSet<SalvageStore> SalvageStore { get; set; }
        public DbSet<SalvageStoreGoodReceiving> SalvageStoreGoodReceiving { get; set; }
        public DbSet<CannibalizationDetail> CannibalizationDetails { get; set; }
        public DbSet<CannibalizationApproval> CannibalizationApproval { get; set; }
        public DbSet<GoodRecivingNoteApproval> GoodRecivingNoteApproval { get; set; }
        public DbSet<SalvageStoreApproval> SalvageStoreApproval { get; set; }

        public DbSet<AdvanceMeasure> AdvanceMeasures { get; set; }
        public DbSet<DailyCare> DailyCares { get; set; }
        public DbSet<DailyCareCategory> DailyCareCategories { get; set; }
        public DbSet<DailyCareDetailLog> DailyCareDetailLogs { get; set; }
        public DbSet<DailyCareLog> DailyCareLogs { get; set; }
        public DbSet<FailureReason> FailureReasons { get; set; }
        #endregion



        #region 6 - Rent Management

        public DbSet<RentFromExternalCustomerChange> RentFromExternalCustomerChange { get; set; }
        public DbSet<RentFromExternalEquipment> RentFromExternalEquipments { get; set; }
        public DbSet<ExternalRentedEquipmentCheckList> ExternalRentedEquipmentCheckList { get; set; }
        public DbSet<RentInternalProject> RentInternalProject { get; set; }
        public DbSet<ExternalRentedEquipment> ExternalRentedEquipments { get; set; }
        public DbSet<RentToExternal> RentToExternals { get; set; }
        public DbSet<RentRequest> RentRequest { get; set; }
        public DbSet<RentFromExternal> RentFromExternal { get; set; }
        public DbSet<RentalEquipmentAdvancePayment> RentalEquipmentAdvancePayment { get; set; }
        public DbSet<RentFromExternalDocument> RentFromExternalDocument { get; set; }
        public DbSet<RentForInternalApproval> RentForInternalApproval { get; set; }
        public DbSet<RentFromExternalApproval> RentFromExternalApproval { get; set; }
        public DbSet<RentRequestApproval> RentRequestApproval { get; set; }
        public DbSet<RentToExternalApproval> RentOtherSupplierApproval { get; set; }

        #endregion


        #region 7 Fleet Resource
        public DbSet<SiteVechicleTransferTyreDetail> SiteVechicleTransferTyreDetailes { get; set; }
        public DbSet<SelfCareSatisfaction> SelfCareSatisfaction { get; set; }
        public DbSet<RequestedEquipment> RequestedEquipments { get; set; }
        public DbSet<VehicleDemand> VehicleDemand { get; set; }
        public DbSet<VehicleDistribution> VehicleDistribution { get; set; }
        public DbSet<VehicleReturn> VehicleReturn { get; set; }
        public DbSet<SitesVehicleTransfer> SitesVehicleTransfer { get; set; }
        public DbSet<SiteVechicleTransferDetail> SiteVechicleTransferDetails { get; set; }
        public DbSet<DriverTransferDetail> DriverTransferDetail { get; set; }
        public DbSet<DriverVehicleTransfer> DriverVehicleTransfer { get; set; }
        public DbSet<AssignEquipmentApproval> AssignEquipmentApproval { get; set; }
        public DbSet<DriverTransferApproval> DriverVehicleTransferApproval { get; set; }
        public DbSet<SiteVehicleApproval> SiteVehicleApproval { get; set; }
        public DbSet<VehicleDemandApproval> VehicleDemandApproval { get; set; }
        public DbSet<VehicleDistributionApproval> VehicleDistributionApproval { get; set; }
        public DbSet<VehicleReturnApproval> VehicleReturnApproval { get; set; }

        #endregion


        #region 8 Tire
        public DbSet<TireIssue> TireIssue { get; set; }
        public DbSet<TireIssueDetail> TireIssueDetails { get; set; }
        public DbSet<TireReturn> TireReturn { get; set; }
        public DbSet<TireIssueApproval> TireIssueApproval { get; set; }
        public DbSet<TireReturnApproval> TireReturnApproval { get; set; }
        public DbSet<TyreTransfer> TyreTransfers { get; set; }
        public DbSet<TyreTransferApproval> TyreTransferApprovals { get; set; }

        #endregion


        #region 9 Battery
        public DbSet<BatteryIssue> BatteryIssue { get; set; }
        public DbSet<BatteryReturn> BatteryReturn { get; set; }
        public DbSet<BatteryApproval> BatteryApproval { get; set; }

        #endregion
        public DbSet<RentFromExternalEquipmentRate> RentFromExternalEquipmentRates { get; set; }
        public DbSet<FleetRentalRate> FleetRentalRates { get; set; }
        public DbSet<EquipmentRate> EquipmentRates { get; set; }
        public DbSet<FleetAnnualPlan> FleetAnnualPlans { get; set; }
        public DbSet<FleetAnnualPlanDetail> FleetAnnualPlanDetails { get; set; }
        public DbSet<FleetAnnualPlanDaily> FleetAnnualPlanDaily { get; set; }
        public DbSet<FleetBatteryAndTire> FleetBatteryAndTires { get; set; }
        public DbSet<FleetBoloRegistration> FleetBoloRegistrations { get; set; }
        public DbSet<FleetBoloRegistrationHistory> FleetBoloRegistrationsHistorys { get; set; }
        //  public DbSet<DryLoadingTimeSheetViewModel> DryLoadingTimeSheetViewModels { get; set; }
        public DbSet<TimeSheetSetting> TimeSheetSetting { get; set; }
        public DbSet<LightVehicleTimeSheet> LightVehicleTimeSheets { get; set; }
        public DbSet<RentedEquipmentMonthlyPaymentSummary> RentedEquipmentMonthlyPaymentSummary { get; set; }
        public DbSet<WagonDrillDailyOperation> WagonDrillDailyOperation { get; set; }
        public DbSet<RentalEquipmentPaymentSummaryDocument> RentalEquipmentPaymentSummaryDocument { get; set; }
        public DbSet<RentedEquipmentMonthlyPaymentSummaryValidation> RentedEquipmentMonthlyPaymentSummaryValidation { get; set; }
        public DbSet<RentedEquipmentDailyOperation> RentedEquipmentDailyOperation { get; set; }
        public DbSet<DumpTruckTimeSheetDetail> DumpTruckTimeSheetDetails { get; set; }
        public DbSet<TimeSheetApprovalNew> TimeSheetApprovalNew { get; set; }
        public DbSet<CostSheetApproval> CostSheetApprovals { get; set; }
        public DbSet<AssignedEquipmentAdditionalInfo> AssignedEquipmentAdditionalInfos { get; set; }
        public DbSet<FleetInsuranceDocument> FleetInsuranceDocuments { get; set; }
        public DbSet<FleetInsuranceHistory> FleetInsuranceHistorys { get; set; }
        public DbSet<BoloRegistrationDocument> BoloRegistrationDocuments { get; set; }
        #endregion
        #region Maintenance
        public DbSet<DescriptionOfAchievement> DescriptionOfAchievement { get; set; }
        public DbSet<ExternalMaintenancePaymentRequestDepValidation> ExternalMaintenancePaymentRequestDepValidations { get; set; }
        public DbSet<ExternalMaintenancePaymentRequestValidation> ExternalMaintenancePaymentRequestValidations { get; set; }
        public DbSet<ExternalPaymentRequestJob> ExternalPaymentRequestJobs { get; set; }
        public DbSet<MaintenanceCompanyDocumentType> MaintenanceCompanyDocumentTypes { get; set; }
        public DbSet<MaintenanceCompanyDocument> MaintenanceCompanyDocuments { get; set; }
        public DbSet<ExternalMaintenancePaymentRequest> ExternalMaintenancePaymentRequests { get; set; }
        public DbSet<ExternalMaintenancePaymentRequestDetail> ExternalMaintenancePaymentRequestDetails { get; set; }
        public DbSet<TyreSerialInfo> TyreSerialInfo { get; set; }
        public DbSet<AccesseryInfo> AccesseryInfo { get; set; }
        public DbSet<MaintenancePlan> MaintenancePlans { get; set; }
        public DbSet<MaintenancePlanDetail> MaintenancePlanDetails { get; set; }
        public DbSet<MaintenanceDailyPlan> MaintenanceDailyPlan { get; set; }
        public DbSet<AdditionalWorkShopDetail> AdditionalWorkShopDetails { get; set; }
        public DbSet<SatisfactionNote> SatisfactionNote { get; set; }
        public DbSet<ExternalServiceCenterPayment> ExternalServiceCenterPayments { get; set; }
        public DbSet<PeriodicEquipmentInspection> PeriodicEquipmentInspection { get; set; }
        public DbSet<MechanicTypeClass> MechanicTypeClasses { get; set; }
        public DbSet<PMSchedule> PMSchedules { get; set; }
        public DbSet<DailyCareDetailItem> DailyCareDetailItems { get; set; }
        public DbSet<PMScheduleDetail> PMScheduleDetails { get; set; }
        public DbSet<ServiceCrewWorkLog> ServiceCrewWorkLogs { get; set; }
        public DbSet<JobRequest> JobRequest { get; set; }
        public DbSet<MaintenanceStage> MaintenanceStage { get; set; }
        public DbSet<RequestToInspection> RequestToInspections { get; set; }
        public DbSet<Canibalization> Canibalizations { get; set; }
        public DbSet<CanibalizationDepartmentApproval> CanibalizationDepartmentApproval { get; set; }
        public DbSet<CanibalizedParts> CanibalizedParts { get; set; }
        public DbSet<CanibalizationApproval> CanibalizationApprovals { get; set; }
        public DbSet<LogisticPlan> LogisticPlans { get; set; }
        public DbSet<LogisticPlanDetail> LogisticPlanDetails { get; set; }
        public DbSet<MaintenaceInspectionLog> MaintenaceInspectionLog { get; set; }
        public DbSet<InventorySparePartMapping> InventorySparePartMapping { get; set; }
        public DbSet<ServiceCenter> ServiceCenters { get; set; }

        public DbSet<ExternalComponent> ExternalComponent { get; set; }
        public DbSet<ExternalComponentDetail> ExternalComponentDetail { get; set; }
        public DbSet<VehiclePartDamageType> VehiclePartDamageType { get; set; }
        public DbSet<VehiclePart> VehiclePart { get; set; }
        public DbSet<VehiclePartManagement> VehiclePartManagement { get; set; }
        public DbSet<PmSetting> PmSettings { get; set; }
        public DbSet<SOPDocument> SOPDocuments { get; set; }
        public DbSet<PmScheduler> PmSchedulers { get; set; }
        public DbSet<VehicleReceptionForm> VehicleReceptionForm { get; set; }
        public DbSet<VehicleReceptionDocument> VehicleReceptionDocuments { get; set; }
        public DbSet<VehicleReceptionApproval> VehicleReceptionApprovals { get; set; }
        public DbSet<JobOrder> JobOrder { get; set; }
        public DbSet<CloseWorkShopWithRemark> CloseWorkShopWithRemarks { get; set; }
        public DbSet<ExternalServiceCenterDetail> ExternalServiceCenterDetail { get; set; }
        public DbSet<ServiceCenterJob> ServiceCenterJob { get; set; }
        public DbSet<WorkshopFieldWorkTool> WorkshopFieldWorkTool { get; set; }
        public DbSet<ExternalServiceAttachement> ExternalServiceAttachement { get; set; }
        public DbSet<InternalJobToExternal> InternalJobToExternal { get; set; }
        public DbSet<InternalJobToExternalValidation> InternalJobToExternalValidation { get; set; }
        public DbSet<JobOrderServiceCenterDetail> JobOrderServiceCenterDetails { get; set; }
        public DbSet<MechanicOnHand> MechanicOnHand { get; set; }
        public DbSet<MaintenanceType> MaintenanceType { get; set; }
        public DbSet<JobCauses> JobCauses { get; set; }
        public DbSet<JobStoreRequisition> JobStoreRequisition { get; set; }
        //public DbSet<StoreRequisition> StoreRequisitions { get; set; }
        public DbSet<JobOrderDetail> JobOrderDetail { get; set; }
        public DbSet<ExternalMaterial> ExternalMaterial { get; set; }
        public DbSet<WorkUnitSection> WorkUnitSection { get; set; }
        public DbSet<ExternalCrew> ExternalCrew { get; set; }
        public DbSet<MaintenanceTool> MaintenanceTool { get; set; }
        public DbSet<MechanicCrew> MechanicCrew { get; set; }
        public DbSet<OutsourceMaintenance> OutsourceMaintenance { get; set; }
        public DbSet<OtherCompanyMaintenance> OtherCompanyMaintenance { get; set; }
        public DbSet<Planner> Planner { get; set; }

        public DbSet<Service> Service { get; set; }

        public DbSet<SparePart> SparePart { get; set; }
        public DbSet<ServiceTool> ServiceTool { get; set; }
        public DbSet<ServiceCrew> ServiceCrew { get; set; }
        public DbSet<ServiceSpare> ServiceSpare { get; set; }
        public DbSet<RepairType> RepairType { get; set; }
        public DbSet<ServiceGroupSetting> ServiceGroupSettings { get; set; }
        public DbSet<MaintenanceCenter> MaintenanceCenter { get; set; }

        public DbSet<OutsourceMaintenanceApproval> OutsourceMaintenanceApproval { get; set; }
        public DbSet<OtherCompanyMaintenaceApproval> OtherCompanyMaintenaceApproval { get; set; }
        public DbSet<AdministrativeJobApproval> AdministrativeJobApproval { get; set; }
        public DbSet<AdministrativeJobDepartmentApproval> AdministrativeJobDepartmentApproval { get; set; }
        public DbSet<TechnicalJobApproval> TechnicalJobApproval { get; set; }
        public DbSet<FollowUp> FollowUp { get; set; }
        public DbSet<MaintenanceToolIssueAndReturn> MaintenanceToolIssueAndReturn { get; set; }
        public DbSet<SpareMaterialCategory> SpareMaterialCategory { get; set; }
        public DbSet<EquipmentInformation> EquipmentInformation { get; set; }
        public DbSet<MaintenanceInterval> MaintenanceIntervals { get; set; }

        #endregion

        #region CMS
        public DbSet<ExceedERP.Core.Domain.CMS.CMSProgram> CMSPrograms { get; set; }
        public DbSet<ExceedERP.Core.Domain.CMS.CMSCourse> CMSCourses { get; set; }
        public DbSet<ExceedERP.Core.Domain.CMS.City> Cities { get; set; }
        public DbSet<ExceedERP.Core.Domain.CMS.RegistrationNumberPrefix> RegistrationNumberPrefixes { get; set; }
        public DbSet<ExceedERP.Core.Domain.CMS.ISOType> ISOTypes { get; set; }
        public DbSet<ExceedERP.Core.Domain.CMS.ISOChecklist> ISOChecklists { get; set; }

        public DbSet<ExceedERP.Core.Domain.CMS.CmsTrainee> CmsTrainees { get; set; }
        public DbSet<ExceedERP.Core.Domain.CMS.TraineeIdNumberComponent> TraineeIdNumberComponents { get; set; }
        public DbSet<ExceedERP.Core.Domain.CMS.TraineeProgram> TraineePrograms { get; set; }
        public DbSet<ExceedERP.Core.Domain.CMS.CompanyEvaluation> CompanyEvaluations { get; set; }
        public DbSet<ExceedERP.Core.Domain.CMS.CmsTraineeIdentification> CmsTraineeIdentifications { get; set; }
        public DbSet<ExceedERP.Core.Domain.CMS.WelderPerformanceQualification> WelderPerformanceQualification { get; set; }
        public DbSet<ExceedERP.Core.Domain.CMS.CMSTraineeCourse> CMSTraineeCourses { get; set; }

        #endregion

        #region CRM

        public DbSet<AdvancePaymentDistibution> AdvancePaymentDistibutions { get; set; }

        public DbSet<AdvancePaymentLog> AdvancePaymentLogs { get; set; }
        public DbSet<CashSalesPartialItem> CashSalesPartialItems { get; set; }

        public DbSet<CashSalesPartialLineItem> CashSalesPartialLineItems { get; set; }
        public DbSet<SalesInvoicePaymentDistibution> SalesInvoicePaymentDistibutions { get; set; }

        public DbSet<DefaultLedgerAccount> DefaultLedgerAccounts { get; set; }
        public DbSet<TicketStatusLog> TicketStatusLogs { get; set; }
        public DbSet<SubCountry> SubCountries { get; set; }
        public DbSet<SupportGroupDetail> SupportGroupDetails { get; set; }
        public DbSet<VoucherDefinition> VoucherDefinitions { get; set; }
        public DbSet<Term> Terms { get; set; }
        public DbSet<AdvancePayment> AdvancePayments { get; set; }
        public DbSet<AdvancePaymentLine> AdvancePaymentLines { get; set; }

        public DbSet<VoucherTerm> VoucherTerms { get; set; }
        public DbSet<Core.Domain.CRM.Tax> Taxs { get; set; }
        public DbSet<TaxTransaction> TaxTransactions { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<LineItem> LineItems { get; set; }
        public DbSet<CreditSetting> CreditSettings { get; set; }
        public DbSet<CreditSettlement> CreditSettlement { get; set; }
        public DbSet<CreditSettlementValidation> CreditSettlementValidation { get; set; }
        public DbSet<CreditSettlementDistribution> CreditSettlementDistribution { get; set; }
        public DbSet<LineCreditSettlement> LineCreditSettlements { get; set; }
        public DbSet<IssueHistory> IssueHistorys { get; set; }
        public DbSet<AdditionalCostLog> AdditionalCostLogs { get; set; }
        public DbSet<CommissionLineItem> CommissionLineItems { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<LifeTime> LifeTimes { get; set; }
        public DbSet<ActivityDefinition> ActivityDefinitions { get; set; }
        public DbSet<Activity> Activitys { get; set; }
        public DbSet<SalesStore> SalesStores { get; set; }
        public DbSet<AdditionalCost> AdditionalCosts { get; set; }
        public DbSet<GlobalTypeList> GlobalTypeList { get; set; }
        public DbSet<ReceiveMethod> ReceiveMethod { get; set; }
        public DbSet<VoucherExtension> VoucherExtensions { get; set; }
        public DbSet<VoucherExtensionTransaction> VoucherExtensionTransactions { get; set; }
        public DbSet<SerialDefinition> SerialDefinitions { get; set; }
        public DbSet<SerialCode> SerialCodes { get; set; }
        public DbSet<TransactionReference> TransactionReferences { get; set; }
        public DbSet<StoreTransaction> StoreTransactions { get; set; }
        public DbSet<StockBalance> StockBalances { get; set; }
        public DbSet<ServiceUsageApproval> ServiceUsageApprovals { get; set; }
        public DbSet<StockLevel> StockLevels { get; set; }
        public DbSet<CustomerService> CustomerServices { get; set; }
        public DbSet<ServiceUsage> ServiceUsages { get; set; }
        public DbSet<ServiceLineItem> ServiceLineItems { get; set; }
        public DbSet<InventoryCateoryMap> InventoryCateoryMaps { get; set; }

        public DbSet<VoucherValue> VoucherValues { get; set; }
        public DbSet<GlobalTypeTax> GlobalTypeTaxs { get; set; }
        public DbSet<ControlAccount> ControlAccounts { get; set; }
        public DbSet<AccountRequirement> AccountRequirements { get; set; }
        public DbSet<Accounts> Accounts { get; set; }
        public DbSet<AccountMap> AccountMaps { get; set; }
        public DbSet<JournalTemplate> JournalTemplates { get; set; }
        public DbSet<JournalDetail> JournalDetails { get; set; }
        public DbSet<Beginning> Beginnings { get; set; }
        public DbSet<CommunicationSource> CommunicationSources { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<CurrencyPreference> CurrencyPreferences { get; set; }
        public DbSet<ObjectState> ObjectStates { get; set; }
        public DbSet<Religion> Religions { get; set; }
        public DbSet<ValueFactorDefinition> ValueFactorDefinitions { get; set; }
        public DbSet<ValueFactor> ValueFactors { get; set; }

        public DbSet<SalesQuoteVoucher> SalesQuoteVouchers { get; set; }

        public DbSet<SalesOrderVoucher> SalesOrderVouchers { get; set; }

        public DbSet<CartTransaction> CartTransactions { get; set; }

        public DbSet<NonCashTransaction> NonCashTransactions { get; set; }

        public DbSet<LifeSpan> LifeSpans { get; set; }

        public DbSet<ObjectStateDefinition> ObjectStateDefinitions { get; set; }

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductUnitConvertorSetup> ProductUnitConvertorSetup { get; set; }

        public DbSet<PersonCategory> PersonCategories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<OrganizationCategory> OrganizationCategories { get; set; }

        public DbSet<OrganizationCustomer> OrganizationCustomers { get; set; }
        public DbSet<Cart> Carts { get; set; }

        public DbSet<Store> Stores { get; set; }

        public DbSet<CustomerTicket> CustomerTickets { get; set; }

        public DbSet<TicketActivityDefinition> TicketActivityDefinitions { get; set; }

        public DbSet<Core.Domain.CRM.TicketAssignment> TicketAssignments { get; set; }

        public DbSet<Core.Domain.CRM.TicketRole> TicketRoles { get; set; }

        public DbSet<BankAccountDetail> BankAccountDetails { get; set; }

        public DbSet<CreditSalesInvoice> CreditSalesInvoices { get; set; }
        public DbSet<CashSalesInvoice> CashSalesInvoices { get; set; }

        public DbSet<CRMDeposit> CRMDeposit { get; set; }
        public DbSet<CRMDepositLine> CRMDepositLine { get; set; }
        public DbSet<CRMDepositValidation> CRMDepositValidation { get; set; }
        public DbSet<CRMDepositDistribution> CRMDepositDistribution { get; set; }

        public DbSet<MailModel> MailModels { get; set; }
        public DbSet<Variety> Varietys { get; set; }

        public DbSet<PreCustomerView> PreCustomerViews { get; set; }


        public DbSet<Campaign> Campaigns { get; set; }

        public DbSet<Role> Roles { get; set; }


        public DbSet<Specification> Specifications { get; set; }

        public DbSet<ObjectAccountRequirement> ObjectAccountRequirements { get; set; }

        public DbSet<ThingCategoryDefaultAccount> ThingCategoryDefaultAccounts { get; set; }


        public DbSet<CustomerHandlingTeam> CustomerHandlingTeams { get; set; }


        public DbSet<CRMHoliday> CRMHolidays { get; set; }


        public DbSet<HolidayDefinition> HolidayDefinitions { get; set; }


        public DbSet<CampaignProduct> CampaignProducts { get; set; }

        public DbSet<PreCustomerSchedule> PreCustomerSchedules { get; set; }

        public DbSet<PreCustomer> PreCustomers { get; set; }

        public DbSet<BinCardLineReserve> BinCardLineReserves { get; set; }

        public DbSet<SupportGroup> SupportGroups { get; set; }

        public DbSet<StatusType> StatusTypes { get; set; }

        public DbSet<PriorityType> PriorityTypes { get; set; }
        public DbSet<AssignTicket> AssignTickets { get; set; }
        public DbSet<CRMPeriod> CRMPeriods { get; set; }
        public DbSet<SalesShipment> SalesShipments { get; set; }
        public DbSet<SalesShipmentLineItem> SalesShipmentLineItem { get; set; }
        public DbSet<SalesShipmentDistribution> SalesShipmentDistribution { get; set; }
        public DbSet<SalesShipmentValidation> SalesShipmentValidation { get; set; }
        public DbSet<SalesPersonAssignment> SalesPersonAssignments { get; set; }
        public DbSet<QuotationTerm> QuotationTerms { get; set; }

        public DbSet<SalesQuoteVoucherValidation> SalesQuoteVoucherValidation { get; set; }
        public DbSet<SalesQuoteVoucherFirstValidation> SalesQuoteVoucherFirstValidation { get; set; }
        public DbSet<SalesQuoteVoucherSecondValidation> SalesQuoteVoucherSecondValidation { get; set; }
        public DbSet<SalesOrderVoucherValidation> SalesOrderVoucherValidation { get; set; }
        public DbSet<PaymentSchedule> PaymentSchedule { get; set; }
        public DbSet<VoucherContractDocument> VoucherContractDocument { get; set; }
        public DbSet<CashSalesInvoiceValidation> CashSalesInvoiceValidation { get; set; }
        public DbSet<PosSetting> PosSetting { get; set; }
        public DbSet<TenderFollowup> TenderFollowup { get; set; }
        public DbSet<TenderFollowupLineItem> TenderFollowupLineItem { get; set; }

        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.AnnualSalesPlan> AnnualSalesPlans { get; set; }

        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.BranchAnnualSalesPlan> BranchAnnualSalesPlans { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.StoreAnnualSalesPlan> StoreAnnualSalesPlans { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.StoreMonthlySalesPlan> StoreMonthlySalesPlans { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.AnnualSalesPlanValidation> AnnualSalesPlanValidation { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.CRMDashboardTodateSalesData> CRMDashboardTodateSalesData { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.CRMDashboardTodateSalesBranchData> CRMDashboardTodateSalesBranchData { get; set; }

        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.PriceEstimationQuoteVoucher> PriceEstimationQuoteVouchers { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.PriceEstimationQuoteVoucherLine> PriceEstimationQuoteVoucherLines { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.PriceEstimationQuoteVoucherService> PriceEstimationQuoteVoucherServices { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.PriceEstimationQuoteVoucherFirstValidation> PriceEstimationQuoteVoucherFirstValidation { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.PriceEstimationQuoteVoucherSecondValidation> PriceEstimationQuoteVoucherSecondValidation { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.PriceEstimationQuoteVoucherValidation> PriceEstimationQuoteVoucherValidation { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.CRM.ProductPriceLog> ProductPriceLog { get; set; }


        public DbSet<QueueType> QueueTypes { get; set; }
        public DbSet<InvestmentType> InvestmentTypes { get; set; }
        public DbSet<LocationMap> LocationMap { get; set; }
        public DbSet<LocationUserAssignment> LocationUserAssignment { get; set; }
        public DbSet<QueueCustomer> QueueCustomer { get; set; }
        public DbSet<Queue> Queue { get; set; }
        public DbSet<QueueStatus> QueueStatus { get; set; }
        public DbSet<QueueValidation> QueueValidation { get; set; }
        public DbSet<QueueStatusValidation> QueueStatusValidation { get; set; }
        public DbSet<SalesReturn> SalesReturn { get; set; }
        public DbSet<SalesReturnLine> SalesReturnLines { get; set; }
        public DbSet<SalesReturnValidation> SalesReturnValidation { get; set; }

        #region sales tender
        public DbSet<SalesLotEvaluationByDocument> SalesLotEvaluationByDocuments { get; set; }
        public DbSet<SaleTender> SaleTenders { get; set; }
        public DbSet<SalesLot> SalesLots { get; set; }
        public DbSet<SalesLotItem> SalesLotItems { get; set; }
        public DbSet<SalesLotParticipant> SalesLotParticipants { get; set; }
        public DbSet<SalesLotParticipantItem> SalesLotParticipantItems { get; set; }
        public DbSet<SalesLotPurchase> SalesLotPurchases { get; set; }
        public DbSet<Winner> Winners { get; set; }
        public DbSet<TenderWinnerValidation> TenderWinnerValidations { get; set; }
        #endregion
        #endregion

        #region Budget

        public DbSet<BudgetType> BudgetTypes { get; set; }
        public DbSet<BudgetRequestTransferDistribution> BudgetRequestTransferDistributions { get; set; }
        public DbSet<BudgetRequestTransferLog> BudgetRequestTransferLogs { get; set; }
        public DbSet<BudgetRequestLine> BudgetRequestLines { get; set; }
        public DbSet<TaskUnit> TaskUnits { get; set; }
        public DbSet<TaskPlan> TaskPlans { get; set; }
        public DbSet<TaskPlanApproval> TaskPlanApprovals { get; set; }
        public DbSet<TaskPlanMonthly> TaskPlanMonthlys { get; set; }
        public DbSet<TaskPerformance> TaskPerformances { get; set; }
        public DbSet<TaskPerformaceApproval> TaskPerformaceApprovals { get; set; }
        public DbSet<TaskCategory> TaskCategorys { get; set; }
        public DbSet<BudgetControl> BudgetControls { get; set; }
        public DbSet<BudgetPlan> BudgetPlans { get; set; }
        public DbSet<BudgetPlanApproval> BudgetPlanApprovals { get; set; }
        public DbSet<RequestedMonth> RequestedMonths { get; set; }
        public DbSet<BudgetPlanMonthly> BudgetPlanMonthlys { get; set; }
        public DbSet<BudgetTransfer> BudgetTransfers { get; set; }
        public DbSet<BudgetUsage> BudgetUsages { get; set; }
        public DbSet<BudgetAllocation> BudgetAllocations { get; set; }
        public DbSet<BudgetCategory> BudgetCategorys { get; set; }
        public DbSet<BudgetRequest> BudgetRequests { get; set; }
        public DbSet<PlanCategory> PlanCategorys { get; set; }
        public DbSet<BudgetCurrency> BudgetCurrencies { get; set; }
        public DbSet<BudgetAllocationApproval> BudgetAllocationApprovals { get; set; }
        public DbSet<BudgetUsageApproval> BudgetUsageApprovals { get; set; }
        public DbSet<BudgetTransferApproval> BudgetTransferApprovals { get; set; }
        public DbSet<BudgetRequestApproval> BudgetRequestApprovals { get; set; }
        public DbSet<BudgetBankAccountMap> BudgetBankAccountMaps { get; set; }

        #endregion

        #region Budget GAAP
        public DbSet<BalanceSheet> BalanceSheets { get; set; }
        public DbSet<BudgetSheetCategory> BalanceSheetCategories { get; set; }
        public DbSet<CostOfServiceSold> CostOfServiceSolds { get; set; }
        public DbSet<Expenditure> Expenditures { get; set; }
        public DbSet<ForcastedIncomeStatement> ForcastedIncomeStatements { get; set; }
        public DbSet<InvestmentPlan> InvestmentPlans { get; set; }
        public DbSet<ProfitLossStatement> ProfitLossStatements { get; set; }
        public DbSet<ProfitLossStatementCategory> ProfitLossStatementCategories { get; set; }

        #endregion

        #region FixedAsset

        #region depreciation
        public DbSet<MonthlyDepreciation> MonthlyDepreciations { get; set; }
        public DbSet<MonthlyDepreciationItem> MonthlyDepreciationItems { get; set; }
        public DbSet<MonthlyDepreciationDistribution> MonthlyDepreciationDistributions { get; set; }
        public DbSet<MonthlyDepreciationValidation> MonthlyDepreciationValidations { get; set; }
        //for tax
        public DbSet<AnnualDepreciation> AnnualDepreciations { get; set; }
        public DbSet<AnnualDepreciationItem> AnnualDepreciationItems { get; set; }
        public DbSet<AnnualDepreciationDistribution> AnnualDepreciationDistributions { get; set; }
        public DbSet<AnnualDepreciationValidation> AnnualDepreciationValidations { get; set; }

        //for ifrs
        public DbSet<IfrsDepreciation> IfrsDepreciations { get; set; }
        public DbSet<IfrsDepreciationItem> IfrsDepreciationItems { get; set; }
        public DbSet<IfrsDepreciationDistribution> IfrsDepreciationDistributions { get; set; }
        public DbSet<IfrsDepreciationValidation> IfrsDepreciationValidations { get; set; }

        public DbSet<CategoryDepreciationValue> CategoryDepreciationValues { get; set; }
        #endregion

        public DbSet<FixedAssetCondition> FixedAssetConditions { get; set; }

        public DbSet<FixedAssetCount> FixedAssetCounts { get; set; }
        public DbSet<FixedAssetCountItem> FixedAssetCountItems { get; set; }
        public DbSet<FixedAssetCountValidation> FixedAssetCountValidations { get; set; }

        #region asset transfer
        public DbSet<AssetTransfer> AssetTransfers { get; set; }
        public DbSet<AssetTransferItem> AssetTransferItems { get; set; }
        public DbSet<AssetTransferValidation> AssetTransferValidations { get; set; }

        public DbSet<AssetTransferReceive> AssetTransferReceives { get; set; }
        public DbSet<AssetTransferReceiveItem> AssetTransferReceiveItems { get; set; }
        public DbSet<AssetTransferReceiveValidation> AssetTransferReceiveValidations { get; set; }
        #endregion

        #region machinery- asset transfer
        public DbSet<MachineryAndVehicleTransfer> MachineryAndVehicleTransfers { get; set; }
        public DbSet<DamagedPart> DamagedParts { get; set; }
        public DbSet<MissedPart> MissedParts { get; set; }
        public DbSet<Accessory> Accessories { get; set; }
        public DbSet<MachineryAndVehicleTransferValidation> MachineryAndVehicleTransferValidations { get; set; }

        public DbSet<MachineryAndVehicleTransferReceive> MachineryAndVehicleTransferReceives { get; set; }
        public DbSet<DamagedPartReceive> DamagedPartReceives { get; set; }
        public DbSet<MissedPartReceive> MissedPartReceives { get; set; }
        public DbSet<AccessoryReceive> AccessoryReceives { get; set; }
        public DbSet<MachineryAndVehicleTransferReceiveValidation> MachineryAndVehicleTransferReceiveValidations { get; set; }
        #endregion
        public DbSet<TagCodeSetting> TagCodeSettings { get; set; }
        public DbSet<FixedAssetPeriod> FixedAssetPeriods { get; set; }
        public DbSet<FixedAssetRequisition> FixedAssetRequisitions { get; set; }
        public DbSet<FixedAssetRequisitionItem> FixedAssetRequisitionItems { get; set; }
        public DbSet<FixedAssetRequisitionAttachment> FixedAssetRequisitionAttachments { get; set; }
        public DbSet<FixedAssetRequisitionValidation> FixedAssetRequisitionValidations { get; set; }
        public DbSet<FixedAssetRequisitionSecondValidation> FixedAssetRequisitionSecondValidations { get; set; }
        public DbSet<FixedAssetRequisitionPropertyValidation> FixedAssetRequisitionPropertyValidations { get; set; }

        public DbSet<GroupAssetBalance> GroupAssetBalances { get; set; }
        public DbSet<LocationTracking> LocationTrackings { get; set; }
        public DbSet<FixedAssetReceive> FixedAssetReceives { get; set; }
        public DbSet<FixedAssetInspection> FixedAssetInspections { get; set; }
        public DbSet<FixedAssetReceiveItem> FixedAssetReceiveItems { get; set; }
        public DbSet<FixedAssetReceiveDistribution> FixedAssetReceiveDistributions { get; set; }
        public DbSet<FixedAssetReceiveValidation> FixedAssetReceiveValidations { get; set; }

        public DbSet<FixedAssetIssue> FixedAssetIssues { get; set; }
        public DbSet<FixedAssetIssueItem> FixedAssetIssueItems { get; set; }

        public DbSet<FixedAssetIssueValidation> FixedAssetIssueValidations { get; set; }

        public DbSet<FixedAssetReturn> FixedAssetReturns { get; set; }
        public DbSet<FixedAssetReturnItem> FixedAssetReturnItems { get; set; }
        public DbSet<FixedAssetReturnValidation> FixedAssetReturnValidations { get; set; }
        public DbSet<FixedAssetReturnDepartmentValidation> FixedAssetReturnDepartmentValidations { get; set; }

        public DbSet<FixedAssetSubCategory> FixedAssetSubCategories { get; set; }
        public DbSet<FixedAssetSubCategoryName> FixedAssetSubCategoryNames { get; set; }
        public DbSet<FixedAssetCategory> FixedAssetCategories { get; set; }
        public DbSet<FixedAssetDepreciationSetup> FixedAssetDepreciationSetups { get; set; }

        public DbSet<FixedAssetClearingAccountSetup> FixedAssetClearingAccountSetups { get; set; }

        public DbSet<FixedAssetGroup> FixedAssetGroups { get; set; }
        public DbSet<TagSetting> TagSettings { get; set; }
        public DbSet<IndexingCount> IndexingCounts { get; set; }

        //  public DbSet<UserCard> UserCards { get; set; }

        public DbSet<UserCardItems> UserCardItems { get; set; }


        public DbSet<DepreciationForCost> DepreciationForCosts { get; set; }
        public DbSet<DepreciationForCostDistribution> DepreciationForCostDistributions { get; set; }
        public DbSet<AssetRegistration> AssetRegistrations { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<FixedAssetRevaluation> FixedAssetRevaluations { get; set; }

        // public DbSet<Disposal> Disposals { get; set; }

        public DbSet<AssetDisposal> AssetDisposals { get; set; }
        public DbSet<DisposalItem> DisposalItems { get; set; }
        public DbSet<AssetDisposalValidation> AssetDisposalValidation { get; set; }

        public DbSet<Sales> Sales { get; set; }
        public DbSet<Appreciation> Appreciations { get; set; }
        public DbSet<DepreciationRule> DepretiationRules { get; set; }


        #endregion

        #region  Inventory

        public DbSet<BranchItemAssignment> BranchItemAssignments { get; set; }
        public DbSet<FuelItemSetting> FuelItemSettings { get; set; }
        public DbSet<NonFuelItemSetting> NonFuelItemSettings { get; set; }
        public DbSet<AveragePriceSetting> AveragePriceSettings { get; set; }
        public DbSet<ServiceReceive> ServiceReceives { get; set; }
        public DbSet<ServiceReceiveItem> ServiceReceiveItems { get; set; }
        public DbSet<ServiceReceiveValidation> ServiceReceiveValidations { get; set; }
        public DbSet<ServiceReceiveDistribution> ServiceReceiveDistributions { get; set; }
        public DbSet<InventoryPeriod> InventoryPeriods { get; set; }
        public DbSet<ReceiveType> ReceiveTypes { get; set; }
        public DbSet<UoM> UoMs { get; set; }
        public DbSet<GoodReceive> GoodReceives { get; set; }
        public DbSet<GoodReceiveItem> GoodReceiveItems { get; set; }
        public DbSet<GoodReceiveDonationItem> GoodReceiveDonationItems { get; set; }
        public DbSet<GoodReceiveValidation> GoodReceiveValidations { get; set; }
        public DbSet<GoodReciveDonationValidation> GoodReciveDonationValidations { get; set; }
        public DbSet<GoodReciveDonationDistribution> GoodReciveDonationDistributions { get; set; }
        //public DbSet<IssueFuel> IssueFuels { get; set; }
        //public DbSet<IssueFuelItem> IssueFuelItems { get; set; }
        public DbSet<GoodReceiveDistribution> GoodReceiveDistributions { get; set; }
        public DbSet<GoodSellingAndLending> GoodSellingAndLendings { get; set; }
        public DbSet<GoodSellingAndLendingItem> GoodSellingAndLendingItems { get; set; }
        public DbSet<GoodSellingAndLendingValidation> GoodSellingAndLendingValidations { get; set; }
        public DbSet<GoodSellingAndLendingDistribution> GoodSellingAndLendingDistributions { get; set; }
        public DbSet<Issue> Issues { get; set; }
        public DbSet<IssueItem> IssueItems { get; set; }
        public DbSet<IssueValidation> IssueValidations { get; set; }
        public DbSet<IssueDistribution> IssueDistributions { get; set; }
        //public DbSet<IssueFuelValidation> IssueFuelValidations { get; set; }
        //public DbSet<IssueFuelDistribution> IssueFuelDistributions { get; set; }
        public DbSet<ItemCategory> ItemCategorys { get; set; }
        public DbSet<ItemCategoryUserRole> ItemCategoryUserRoles { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<ItemPerStoreBalance> ItemPerStoreBalances { get; set; }
        public DbSet<ItemInspection> ItemInspections { get; set; }
        //public DbSet<ItemInspectionItem> ItemInspectionItems { get; set; }
        //public DbSet<ItemInspectionValidation> ItemInspectionValidations { get; set; }
        public DbSet<ItemTransfer> ItemTransfers { get; set; }
        public DbSet<ItemTransferItem> ItemTransferItems { get; set; }
        public DbSet<ItemTransferValidation> ItemTransferValidations { get; set; }
        public DbSet<ItemTransferReciverValidation> ItemTransferReciverValidations { get; set; }

        public DbSet<ItemTransferDistribution> ItemTransferDistributions { get; set; }
        public DbSet<PurchaseRequisition> PurchaseRequisitions { get; set; }
        public DbSet<PurchaseRequisitionItem> PurchaseRequisitionItems { get; set; }
        public DbSet<PurchaseRequisitionItemSpecification> PurchaseRequisitionItemSpecifications { get; set; }
        public DbSet<PurchaseRequisitionAssignment> PurchaseRequisitionAssignments { get; set; }
        public DbSet<PurchaseRequisitionReassignmentValidation> PurchaseRequisitionReassignmentValidations { get; set; }
        public DbSet<PurchaseRequisitionReassignmentItem> PurchaseRequisitionReassignmentItems { get; set; }
        public DbSet<PurchaseRequisitionUserDepartmentValidation> PurchaseRequisitionUserDepartmentValidations { get; set; }
        public DbSet<PurchaseRequisitionPurchaseDepartmentValidation> PurchaseRequisitionPurchaseDepartmentValidations { get; set; }
        // public DbSet<PurchaseRequisitionDistribution> PurchaseRequisitionDistributions { get; set; }
        public DbSet<PurchaseRequisitionAttachment> PurchaseRequisitionAttachments { get; set; }
        public DbSet<PurchaseReturn> PurchaseReturns { get; set; }
        public DbSet<PurchaseReturnItems> PurchaseReturnItemss { get; set; }
        public DbSet<PurchaseReturnValidation> PurchaseReturnValidations { get; set; }
        public DbSet<PurchaseReturnDistribution> PurchaseReturnDistributions { get; set; }
        public DbSet<StockAdjustment> StockAdjustments { get; set; }
        public DbSet<StockAdjustmentItems> StockAdjustmentItemss { get; set; }
        public DbSet<StockAdjustmentValidation> StockAdjustmentValidations { get; set; }
        public DbSet<StockAdjustmentDistribution> StockAdjustmentDistributions { get; set; }
        public DbSet<InventoryStore> InventoryStores { get; set; }
        public DbSet<InventoryStorePeriodStatus> InventoryStorePeriodStatuses { get; set; }
        public DbSet<ProjectStores> ProjectStores { get; set; }
        public DbSet<StoreKeeperAssignment> StoreKeeperAssignments { get; set; }
        public DbSet<StoreItemAssignment> StoreItemAssignments { get; set; }
        public DbSet<BinCardLine> BinCardLines { get; set; }
        public DbSet<StoreRequisition> StoreRequisitions { get; set; }
        public DbSet<StoreRequisitionItem> StoreRequisitionItems { get; set; }
        public DbSet<StoreRequisitionValidation> StoreRequisitionValidations { get; set; }
        public DbSet<StoreRequisitionPropertyValidation> StoreRequisitionPropertyValidations { get; set; }
        public DbSet<StoreRequisitionSecondValidation> StoreRequisitionSecondValidations { get; set; }
        public DbSet<StoreRequisitionAttachment> StoreRequisitionAttachments { get; set; }
        public DbSet<StoreReturn> StoreReturns { get; set; }
        public DbSet<StoreReturnItems> StoreReturnItemss { get; set; }
        public DbSet<StoreReturnValidation> StoreReturnValidations { get; set; }
        public DbSet<StoreReturnDepartmentValidation> StoreReturnDepartmentValidations { get; set; }
        public DbSet<StoreReturnDistribution> StoreReturnDistributions { get; set; }
        public DbSet<StoreReturnAttachment> StoreReturnAttachments { get; set; }
        public DbSet<PropertyCount> PropertyCounts { get; set; }
        public DbSet<PropertyCountItem> PropertyCountItems { get; set; }
        //public DbSet<InventoryPeriod> InventoryPeriods { get; set; }
        public DbSet<TempGoodReceive> TempGoodReceives { get; set; }

        public DbSet<TempGoodReceiveItem> TempGoodReceiveItems { get; set; }

        public DbSet<TempGoodReceiveValidation> TempGoodReceiveValidations { get; set; }
        public DbSet<DepartmentCostCenter> DepartmentCostCenters { get; set; }
        public DbSet<GoodReceiveDonation> GoodReceiveDonations { get; set; }
        public DbSet<EvaluatedPrice> EvaluatedPrices { get; set; }
        public DbSet<RevaluationItem> RevaluationItems { get; set; }
        public DbSet<Revaluation> Revaluations { get; set; }
        public DbSet<RevaluationValidation> RevaluationValidations { get; set; }
        public DbSet<ItemTransferReceive> ItemTransferReceive { get; set; }
        public DbSet<ItemTransferItemReceive> ItemTransferItemReceive { get; set; }
        public DbSet<ItemTransferReceiverValidation> ItemTransferReceiverValidation { get; set; }
        public DbSet<ItemTransferReceiveDistribution> ItemTransferReceiveDistribution { get; set; }

        #region inventory_archive
        public DbSet<StoreRequisitionArchive> StoreRequisitionArchive { get; set; }
        public DbSet<StoreRequisitionItemArchive> StoreRequisitionItemArchive { get; set; }
        public DbSet<StoreRequisitionValidationArchive> StoreRequisitionValidationArchive { get; set; }
        public DbSet<StoreRequisitionPropertyValidationArchive> StoreRequisitionPropertyValidationArchive { get; set; }

        public DbSet<BinCardLineArchive> BinCardLineArchive { get; set; }
        #endregion


        #endregion

        #region Procurement

        public DbSet<TenderExtension> TenderExtensions { get; set; }
        public DbSet<TenderCancellation> TenderCancellations { get; set; }
        public DbSet<TenderCancellationUploadDbModel> TenderCancellationUploadDbModels { get; set; }

        public DbSet<GuaranteeMode> GuaranteeModes { get; set; }
        public DbSet<ContractPaymentSchedule> ContractPaymentSchedules { get; set; }

        public DbSet<ContractManagementValidation> ContractManagementValidations { get; set; }
        public DbSet<Purchaser> Purchasers { get; set; }
        public DbSet<ContractRenewal> ContractRenewals { get; set; }

        public DbSet<ContractRenewalValidation> ContractRenewalValidations { get; set; }
        public DbSet<ContractAttachment> ContractAttachments { get; set; }
        public DbSet<ContractPayment> ContractPayments { get; set; }
        public DbSet<ContractPaymentAttachment> ContractPaymentAttachments { get; set; }
        public DbSet<ContractPaymentGrn> ContractPaymentGrns { get; set; }
        public DbSet<ContractPaymentValidation> ContractPaymentValidations { get; set; }
        //new generic payment request
        public DbSet<PurchasePayment> PurchasePayments { get; set; }
        public DbSet<PurchasePaymentAttachment> PurchasePaymentAttachments { get; set; }
        public DbSet<PurchasePaymentGrn> PurchasePaymentGrns { get; set; }
        public DbSet<PurchasePaymentValidation> PurchasePaymentValidations { get; set; }
        public DbSet<PurchasePaymentBudgetValidation> PurchasePaymentBudgetValidations { get; set; }
        public DbSet<PurchasePaymentFinanceValidation> PurchasePaymentFinanceValidations { get; set; }
        public DbSet<PurchasePaymentAuthorizeValidation> PurchasePaymentAuthorizeValidations { get; set; }

        public DbSet<ContractTermination> ContractTerminations { get; set; }
        public DbSet<ContractTerminationAttachment> ContractTerminationAttachments { get; set; }
        public DbSet<ContractTerminationValidation> ContractTerminationValidations { get; set; }

        public DbSet<ProformTechnical> ProformTechnicals { get; set; }

        public DbSet<ProformPassFail> ProformPassFails { get; set; }
        public DbSet<SupplierListMinuteFile> SupplierListMinuteFiles { get; set; }

        public DbSet<GuaranteeFile> GuaranteeFiles { get; set; }

        public DbSet<PurchaseFollowUpItems> PurchaseFollowUpItems { get; set; }

        public DbSet<LotEvaluationByDocument> LotEvaluationByDocuments { get; set; }
        public DbSet<BidAwardedSupplierItem> BidAwardedSupplierItems { get; set; }

        public DbSet<LotRequiredDocument> LotRequiredDocuments { get; set; }

        public DbSet<ServiceCategory> ServiceCategories { get; set; }

        public DbSet<ServiceItem> ServiceItems { get; set; }


        public DbSet<GuaranteeRule> GuaranteeRules { get; set; }

        public DbSet<Guarantee> Guarantees { get; set; }
        public DbSet<GuaranteeRenewalRequest> GuaranteeRenewalRequests { get; set; }

        public DbSet<GuaranteeRenewal> GuaranteeRenewals { get; set; }

        public DbSet<GuaranteeValidation> GuaranteeValidations { get; set; }

        public DbSet<SupplierBlackFile> SupplierBlackFiles { get; set; }

        public DbSet<ProcurementPeriod> ProcurementPeriods { get; set; }

        public DbSet<PassFailResult> TenderPassFailResult { get; set; }

        public DbSet<TenderTechnical> TenderTechnicals { get; set; }

        public DbSet<TenderMessages> TenderMessages { get; set; }
        public DbSet<ParticipantsItem> ParticipantsItems { get; set; }

        public DbSet<ForeignPurchase> ForeignPurchases { get; set; }
        public DbSet<ForeignPurchaseFollowUpItems> ForeignPurchaseFollowUpItems { get; set; }
        public DbSet<ForeignPurchaseFollowUpStatus> ForeignPurchaseFollowUpStatus { get; set; }
        public DbSet<ForeignPurchaseFollowUpSystemStatus> ForeignPurchaseFollowUpSystemStatus { get; set; }
        public DbSet<ForeignPurchaseCost> ForeignPurchaseCosts { get; set; }
        public DbSet<ForeignPurchaseAccount> ForeignPurchaseAccounts { get; set; }
        public DbSet<ForeignPurchaseCostValidation> ForeignPurchaseCostValidations { get; set; }
        public DbSet<ForeignPurchaseItemCost> ForeignPurchaseItemCosts { get; set; }
        public DbSet<ForeignPurchaseValidation> ForeignPurchaseValidations { get; set; }
        public DbSet<ForeignPurchaseCostType> ForeignPurchaseCostTypes { get; set; }

        public DbSet<ReportTemplate> ReportTemplates { get; set; }

        public DbSet<ProcurementPOType> ProcurementPOTypes { get; set; }

        public DbSet<ItemPriceIndex> ItemPriceIndexes { get; set; }
        public DbSet<PriceIndexValidation> PriceIndexValidations { get; set; }

        public DbSet<ContractManagementItem> ContractManagementItems { get; set; }

        public DbSet<ContractManagement> ContractManagements { get; set; }

        public DbSet<CPOManagement> CPOManagements { get; set; }

        public DbSet<LCManagement> LCManagements { get; set; }

        public DbSet<AnnualNeedAssesmentItem> AnnualNeedAssesmentItems { get; set; }

        public DbSet<AnnualNeedAssesment> AnnualNeedAssesments { get; set; }

        public DbSet<AnnualNeedAssesmentValidation> AnnualNeedAssesmentValidations { get; set; }

        public DbSet<ProformaEvaluationCriteria> ProformaEvaluationCriterias { get; set; }

        public DbSet<ProformaPurchase> ProformaPurchases { get; set; }
        public DbSet<ProformaUploadDbModel> ProformaUploadDbModels { get; set; }

        public DbSet<ProformaEvaluationResult> ProformaEvaluationResults { get; set; }
        public DbSet<ProformaEvaluationItemResult> ProformaEvaluationItemResults { get; set; }

        public DbSet<ParticipatingSupplier> ParticipatingSuppliers { get; set; }
        public DbSet<ParticipatingSupplierItem> ParticipatingSupplierItems { get; set; }

        public DbSet<SupplierRank> SupplierRanks { get; set; }
        public DbSet<ProformaPurchaseRequisition> ProformaPurchaseRequisitions { get; set; }
        public DbSet<ProformaItem> ProformaItems { get; set; }
        public DbSet<ProformaTeam> ProformaTeams { get; set; }
        public DbSet<ProformaTeamMember> ProformaTeamMembers { get; set; }
        public DbSet<ProformaTeamDecision> ProformaTeamDecisions { get; set; }
        public DbSet<WinningSupplierItem> WinningSupplierItems { get; set; }

        public DbSet<PurchaseValidation> PurchaseValidations { get; set; }

        public DbSet<RequestedSupplier> RequestedSuppliers { get; set; }

        public DbSet<WinningSupplier> WinningSuppliers { get; set; }

        public DbSet<ProcurementSupplier> ProcurementSuppliers { get; set; }

        public DbSet<SupplierCategory> SupplierCategories { get; set; }

        public DbSet<ProcurementPlan> ProcurementPlans { get; set; }
        public DbSet<ProcurementPlanItem> ProcurementPlanItems { get; set; }
        public DbSet<ProcurementPlanValidation> ProcurementPlanValidations { get; set; }
        public DbSet<LotItems> LotItems { get; set; }

        public DbSet<LotFinancialOffer> LotFinancialOffers { get; set; }


        public DbSet<PurchaseFollowUp> PurchaseFollowUps { get; set; }
        public DbSet<PurchaseFollowUpStatus> PurchaseStatus { get; set; }
        public DbSet<PurchaseFollowUpSystemStatus> PurchaseFollowUpSystemStatus { get; set; }

        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PoPurchaseRequisition> PoPurchaseRequisitions { get; set; }

        public DbSet<PurchaseOrderDistribution> PurchaseOrderDistributions { get; set; }

        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        public DbSet<PurchaseOrderValidation> PurchaseOrderValidations { get; set; }
        public DbSet<PoPaymentSchedule> PoPaymentSchedules { get; set; }
        public DbSet<PoAdditionalCharge> PoAdditionalCharges { get; set; }
        public DbSet<PoAttachment> PoAttachments { get; set; }

        //change of order

        public DbSet<PoChangeOforder> PoChangeOforders { get; set; }
        public DbSet<PoChangeOfOrderItem> PoChangeOfOrderItems { get; set; }
        public DbSet<PoChangeOforderValidation> PoChangeOforderValidations { get; set; }

        public DbSet<BankGuarantee> BankGuarantees { get; set; }
        public DbSet<Tender> Tenders { get; set; }
        public DbSet<Lot> Lots { get; set; }
        public DbSet<LotTeam> LotTeams { get; set; }
        public DbSet<LotTeamMember> LotTeamMembers { get; set; }
        public DbSet<LotTeamDecision> LotTeamDecisions { get; set; }
        public DbSet<BidAwardedSupplier> BidAwardedSuppliers { get; set; }
        public DbSet<BidPParticipatingSuppliers> BidPParticipatingSupplierses { get; set; }
        public DbSet<BidParticipatingSupplierItem> BidParticipatingSupplierItems { get; set; }
        public DbSet<BidPurchasingSuppliers> BidPurchasingSupplierses { get; set; }
        public DbSet<TenderEvaluationResult> TenderEvaluationResults { get; set; }
        public DbSet<TenderEvaluationItemResult> TenderEvaluationItemResults { get; set; }
        public DbSet<TenderEvaluationCriteria> TenderEvaluationCriterias { get; set; }
        public DbSet<TenderMinuteUploadDBModel> TenderMinuteUploads { get; set; }
        public DbSet<TenderBidDocument> TenderBidDocuments { get; set; }
        public DbSet<TenderTemplateFiles> TenderTemplateFiles { get; set; }
        public DbSet<TenderValidation> TenderValidations { get; set; }


        public DbSet<SupplierBlackList> SupplierBlackLists { get; set; }

        public DbSet<SupplierBlackListValidation> SupplierBlackListValidations { get; set; }

        public DbSet<SupplierPerformance> SupplierPerformances { get; set; }

        public DbSet<SupplierPerformanceValidation> SupplierPerformanceValidations { get; set; }

        public DbSet<SupplierRemoveBlackList> SupplierRemoveBlackLists { get; set; }

        #endregion

        #region finance 
        #region budget
        public DbSet<BudgetAccountSheduleCategory> BudgetAccountSheduleCategories { get; set; }
        public DbSet<BudgetAccountShedule> BudgetAccountShedules { get; set; }
        public DbSet<BudgetAccountSheduleSubCategory> BudgetAccountSheduleSubCategories { get; set; }
        public DbSet<BudgetAccountSheduleLine> BudgetAccountSheduleLines { get; set; }
        public DbSet<BudgetLineSheduleAccounts> BudgetLineSheduleAccounts { get; set; }
        #endregion

        #region AR

        public DbSet<ArSalesOrder> ArSalesOrders { get; set; }
        public DbSet<ArSalesOrderPartial> ArSalesOrderPartials { get; set; }
        public DbSet<ArSalesOrderPartialPrintDistribution> ArSalesOrderPartialPrintDistributions { get; set; }
        public DbSet<ArSalesOrderPartialPrintValidation> ArSalesOrderPartialPrintValidations { get; set; }
        public DbSet<ArSalesOrderLine> ArSalesOrderLines { get; set; }
        public DbSet<ArSalesOrderAdvanceLine> ArSalesOrderAdvanceLines { get; set; }
        public DbSet<ArSalesOrderDistribution> ArSalesOrderDistributions { get; set; }
        public DbSet<ArSalesOrderValidation> ArSalesOrderValidations { get; set; }
        public DbSet<ArSalesOrderStatus> ArSalesOrderStatus { get; set; }
        public DbSet<ArSalesOrderBroadcastTime> ArSalesOrderBroadcastTimes { get; set; }
        public DbSet<ArSalesOrderPaymentSchedule> ArSalesOrderPaymentSchedules { get; set; }
        public DbSet<ArSalesOrderContract> ArSalesOrderContracts { get; set; }
        public DbSet<PartnerType> PartnerTypes { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<ArCustomerProfile> ARCustomerProfiles { get; set; }

        public DbSet<ArCustomerType> ARCustomerTypes { get; set; }

        public DbSet<ArInvoice> ARInvoices { get; set; }

        public DbSet<ArInvoiceDistribution> ARInvoiceDistributions { get; set; }

        public DbSet<ArInvoiceLines> ARInvoiceLines { get; set; }

        public DbSet<ArInvoiceValidation> ARInvoiceValidations { get; set; }

        public DbSet<ArInvoiceViewModel> ARInvoiceViewModels { get; set; }

        public DbSet<ARMiscelaneousCollection> ARMiscelaneousCollections { get; set; }
        public DbSet<ArMiscelaneousCollectionLine> ArMiscelaneousCollectionLines { get; set; }

        public DbSet<ARMiscelaneousCollectionDistribution> ARMiscelaneousCollectionDistributions { get; set; }

        public DbSet<ARMiscelaneousCollectionValidation> ARMiscelaneousCollectionValidations { get; set; }

        public DbSet<ARRemitAddress> ARRemitAddresses { get; set; }

        public DbSet<ARSetupProductServiceAccount> ARSetupProductServiceAccounts { get; set; }
        public DbSet<ArProductService> ArProductService { get; set; }

        public DbSet<ARSetupReceiptBalanceAccount> ARSetupReceiptBalanceAccounts { get; set; }

        public DbSet<ARSetupTaxRate> ARSetupTaxRates { get; set; }

        public DbSet<ARSetupTransactionClass> ARSetupTransactionClasses { get; set; }

        public DbSet<ARSetupTransactionType> ARSetupTransactionTypes { get; set; }

        public DbSet<ArStandardCollection> ARStandardCollections { get; set; }

        public DbSet<ArStandardCollectionDistribution> ARStandardCollectionDistributions { get; set; }

        public DbSet<ArStandardCollectionLines> ARStandardCollectionLines { get; set; }

        public DbSet<ArStandardCollectionValidation> ARStandardCollectionValidations { get; set; }

        public DbSet<ArStandardCollectionViewModel> ARStandardCollectionViewModels { get; set; }


        #endregion
        public DbSet<GLSegmentCrossValidation> GlSegmentCrossValidations { get; set; }
        public DbSet<GLCrossValidationRuleElement> GlCrossValidationRuleElements { get; set; }
        public DbSet<GlRuleValue> GlRuleValues { get; set; }
        public DbSet<GLSegmentSetup> GlSegmentSetups { get; set; }
        public DbSet<GLChartOfAccountSegment> GlChartOfAccountSegments { get; set; }
        public DbSet<GlCostAllocationSetup> GlCostAllocationSetups { get; set; }
        public DbSet<JournalSourceMap> JournalSourceMaps { get; set; }
        public DbSet<GlFiscalYear> GLFiscalYears { get; set; }
        public DbSet<AgingPeriodSetup> AgingPeriodSetups { get; set; }
        public DbSet<GLLedgerPosting> GLLedgerPostings { get; set; }
        public DbSet<GLLedgerPostingLines> GLLedgerPostingLines { get; set; }
        public DbSet<GlRetainedEarningAccount> GlRetainedEarningAccounts { get; set; }
        public DbSet<GlEarningLedger> GlEarningLedgers { get; set; }
        public DbSet<GlEarningLedgerLine> GlEarningLedgerLines { get; set; }
        public DbSet<EmployeeSubAccount> EmployeeSubAccounts { get; set; }
        public DbSet<SubAccountRange> SubAccountRanges { get; set; }
        #region AP
        #region fund transfer
        public DbSet<FundTransfer> FundTransfers { get; set; }
        public DbSet<FundTransferDetail> FundTransferDetails { get; set; }
        public DbSet<FundTransferAttachment> FundTransferAttachments { get; set; }
        public DbSet<FundTransferDepartmentValidation> FundTransferDepartmentValidations { get; set; }
        public DbSet<FundTransferBudgetValidation> FundTransferBudgetValidations { get; set; }
        public DbSet<FundTransferValidation> FundTransferValidations { get; set; }
        //fund receive confirmation
        public DbSet<FundReceive> FundReceives { get; set; }
        public DbSet<FundReceiveDistribution> FundReceiveDistributions { get; set; }
        public DbSet<FundReceiveValidation> FundReceiveValidations { get; set; }
        #endregion
        #region payment request
        public DbSet<PaymentRequest> PaymentRequests { get; set; }
        public DbSet<PaymentRequestLine> PaymentRequestLines { get; set; }
        public DbSet<FundTransferLine> FundTransferLines { get; set; }
        public DbSet<PaymentRequestAttachment> PaymentRequestAttachments { get; set; }
        public DbSet<PaymentRequestDepartmentValidation> PaymentRequestDepartmentValidations { get; set; }
        public DbSet<PaymentRequestBudgetValidation> PaymentRequestBudgetValidations { get; set; }
        public DbSet<PaymentRequestAuthorizeValidation> PaymentRequestAuthorizeValidations { get; set; }
        public DbSet<PaymentRequestValidation> PaymentRequestValidations { get; set; }
        #endregion
        #region travel order
        public DbSet<ApTravelOrder> ApTravelOrders { get; set; }
        public DbSet<ApTravelOrderAdvancePayment> ApTravelOrderAdvancePayments { get; set; }
        public DbSet<ApTravelOrderDeparture> ApTravelOrderDepartures { get; set; }
        public DbSet<ApTravelOrderValidation> ApTravelOrderValidations { get; set; }
        public DbSet<ApTravelOrdertAttachment> ApTravelOrdertAttachments { get; set; }
        public DbSet<ApTravelOrderDepartmentValidation> ApTravelOrderDepartmentValidations { get; set; }
        public DbSet<ApTravelOrderBudgetValidation> ApTravelOrderBudgetValidations { get; set; }
        #endregion
        #region expense claim
        public DbSet<ApTravelExpenseClaim> ApTravelExpenseClaims { get; set; }
        public DbSet<ApTravelExpenseExpense> ApTravelExpenseExpenses { get; set; }
        public DbSet<ApTravelExpensePayment> ApTravelExpensePayments { get; set; }
        public DbSet<ApTravelExpenseRefund> ApTravelExpenseRefunds { get; set; }
        public DbSet<ApTravelExpenseClaimAttachment> ApTravelExpenseClaimAttachments { get; set; }
        public DbSet<ApTravelExpenseValidation> ApTravelExpenseValidations { get; set; }
        public DbSet<ApTravelExpenseDepartmentValidation> ApTravelExpenseDepartmentValidations { get; set; }
        public DbSet<ApTravelExpenseBudgetValidation> ApTravelExpenseBudgetValidations { get; set; }
        public DbSet<ApTravelExpenseDistribution> ApTravelExpenseDistributions { get; set; }
        public DbSet<APTravelClaimDeparture> APTravelClaimDepartures { get; set; }
        public DbSet<ApTravelFuelExpenseClaim> ApTravelFuelExpenseClaims { get; set; }

        #endregion
        public DbSet<ApCashierSetup> CashierSetups { get; set; }
        public DbSet<ApPettyCash> ApPettyCash { get; set; }
        public DbSet<ApPettyCashPayment> ApPettyCashPayments { get; set; }
        public DbSet<ApPettyCashUtilization> ApPettyCashUtilizations { get; set; }
        public DbSet<ApPettyCashValidation> ApPettyCashValidations { get; set; }
        public DbSet<ApPettyCashDistribution> ApPettyCashDistributions { get; set; }

        public DbSet<ApSetupItem> ApSetupItems { get; set; }
        public DbSet<APBusinessType> APBusinessTypes { get; set; }
        public DbSet<APSupplierType> APSupplierTypes { get; set; }

        public DbSet<ApSetupReceiptBalanceAccount> ApSetupReceiptBalanceAccounts { get; set; }

        public DbSet<ApMiscellaneousPaymentDistribution> APMiscellaneousPaymentDistributions { get; set; }

        public DbSet<ApMiscellaneousPayment> APMiscellaneousPayments { get; set; }
        public DbSet<ApMiscellaneousPaymentLine> ApMiscellaneousPaymentLines { get; set; }

        public DbSet<ApMiscellaneousPaymentValidation> APMiscellaneousPaymentValidations { get; set; }

        public DbSet<ApPaymentSettlement> ApPaymentSettlements { get; set; }
        public DbSet<ApPaymentSettlementLine> ApPaymentSettlementLines { get; set; }
        public DbSet<ApPaymentSettlementGrn> ApPaymentSettlementGrns { get; set; }
        public DbSet<ApPaymentSettlementDistribution> ApPaymentSettlementDistributions { get; set; }
        public DbSet<ApPaymentSettlementDepartmentValidation> ApPaymentSettlementDepartmentValidations { get; set; }
        public DbSet<ApPaymentSettlementValidation> ApPaymentSettlementValidations { get; set; }

        public DbSet<ApDirectPayment> ApDirectPayments { get; set; }
        public DbSet<ApDirectPaymentLine> ApDirectPaymentLines { get; set; }
        public DbSet<ApDirectPaymentDistribution> ApDirectPaymentDistributions { get; set; }
        public DbSet<ApDirectPaymentValidation> ApDirectPaymentValidations { get; set; }
        public DbSet<ApPaybleInvoiceDistribution> APPaybleInvoiceDistributions { get; set; }
        public DbSet<APPaybleInvoicePrepayment> APPaybleInvoicePrepayments { get; set; }

        public DbSet<ApPaybleInvoices> APPaybleInvoices { get; set; }

        public DbSet<ApPaybleInvoicesLines> APPaybleInvoicesLines { get; set; }

        public DbSet<ApPaybleInvoiceValidation> APPaybleInvoiceValidations { get; set; }

        public DbSet<ApPaymentDistribution> APPaymentDistributions { get; set; }

        public DbSet<ApPayment> APPayments { get; set; }

        public DbSet<ApPaymentValidation> APPaymentValidations { get; set; }

        public DbSet<APsupplierBlacklist> APsupplierBlacklists { get; set; }

        public DbSet<APSupplierRegistration> APSupplierRegistrations { get; set; }
        public DbSet<ApTPayment> ApTPayments { get; set; }
        public DbSet<ApTPaymentValidation> ApTPaymentValidations { get; set; }
        public DbSet<ApTPaymentRequestResponse> ApTPaymentRequestResponses { get; set; }
        #endregion
        public DbSet<ReconciliationBankAccount> ReconcilationBankAccounts { get; set; }

        public DbSet<ReconciliationBookAccount> ReconcilationBookAccounts { get; set; }


        public DbSet<ReconciliationSchedule> ReconciliationSchedules { get; set; }
        public DbSet<ReconciliationBankServiceCharge> ReconciliationBankServiceCharges { get; set; }
        public DbSet<ReconciliationBankServiceChargeValidation> ReconciliationBankServiceChargeValidations { get; set; }
        public DbSet<ReconciliationBankServiceChargeDistribution> ReconciliationBankServiceChargeDistributions { get; set; }

        public DbSet<Core.Domain.Finance.Common.BankAccount> BankAccounts { get; set; }
        public DbSet<BankUserAssignment> BankUserAssignments { get; set; }

        public DbSet<Core.Domain.Finance.Common.FinInStatementRecord> FinInStatementRecords { get; set; }

        public DbSet<Core.Domain.Finance.Common.PaymentMethod> PaymentMethods { get; set; }

        public DbSet<Core.Domain.Finance.Common.PaymentTerms> PaymentTerms { get; set; }

        public DbSet<GLChartOfAccountAccount> GLChartOfAccountAccounts { get; set; }
        public DbSet<GLAccountConfig> GLAccountConfigs { get; set; }
        public DbSet<GLCountryType> GLCountryTypes { get; set; }

        public DbSet<GLJournalCategoriers> GLJournalCategoriers { get; set; }

        public DbSet<GLJournalSources> GLJournalSources { get; set; }

        public DbSet<GLPeriod> GLPeriods { get; set; }
        public DbSet<GlRecordJournal> GLRecordJournals { get; set; }

        public DbSet<GLRecordJournalDetail> GLRecordJournalDetails { get; set; }

        public DbSet<GLRecordJournalValidation> GLRecordJournalValidations { get; set; }
        public DbSet<GLRecordJournalBeg> GlRecordJournalBegs { get; set; }

        public DbSet<GLRecordJournalBegDistribution> GLRecordJournalBegDistributions { get; set; }

        public DbSet<GLTaxRate> GLTaxRates { get; set; }

        public DbSet<GLVatWithholding> GLVatWithholdings { get; set; }

        public DbSet<FinSetupCurrency> FinSetupCurrencies { get; set; }

        public DbSet<FinSetupCurrencyExchange> FinSetupCurrencyExchanges { get; set; }

        public DbSet<IfrsStatementOfComprehensiveIncome> IfrsStatementOfComprehensiveIncomes { get; set; }

        public DbSet<IfrsScoiContinuingOperation> IfrsScoiContinuingOperations { get; set; }

        public DbSet<IfrsScoiDiscontinuedOperation> IfrsScoiDiscontinuedOperations { get; set; }

        public DbSet<IfrsScoiOtherComprehensiveIncome> IfrsScoiOtherComprehensiveIncomes { get; set; }

        public DbSet<IfrsStatementOfFinancialPosition> IfrsStatementOfFinancialPositions { get; set; }

        public DbSet<IfrsSofpAsset> IfrsSofpAssets { get; set; }

        public DbSet<IfrsSofpEquityAndLiablity> IfrsSofpEquityAndLiablities { get; set; }

        public DbSet<IfrsStatementOfCashFlow> IfrsStatementOfCashFlows { get; set; }

        public DbSet<IfrsSocfOperatingActivity> IfrsSocfOperatingActivities { get; set; }

        public DbSet<IfrsSocfInvestingActivity> IfrsSocfInvestingActivities { get; set; }

        public DbSet<IfrsSocfFinancingActivity> IfrsSocfFinancingActivities { get; set; }

        public DbSet<IfrsCashAndCashEquivalentRange> IfrsCashAndCashEquivalentRanges { get; set; }

        public DbSet<IfrsSocfOperatingActivityRange> IfrsSocfOperatingActivityRanges { get; set; }
        public DbSet<IfrsSocfInvestingActivityRange> IfrsSocfInvestingActivityRanges { get; set; }
        public DbSet<IfrsSocfFinancingActivityRange> IfrsSocfFinancingActivityRanges { get; set; }
        public DbSet<IfrsScoiContinuingOperationRange> IfrsScoiContinuingOperationRanges { get; set; }

        public DbSet<IfrsScoiDiscontinuedOperationRange> IfrsScoiDiscontinuedOperationRanges { get; set; }

        public DbSet<IfrsScoiOtherComprehensiveIncomeRange> IfrsScoiOtherComprehensiveIncomeRanges { get; set; }
        public DbSet<IfrsSofpAssetRange> IfrsSofpAssetRanges { get; set; }

        public DbSet<IfrsSofpEquityAndLiablityRange> IfrsSofpEquityAndLiablityRanges { get; set; }

        public DbSet<IfrsStatementOfChangesInEquity> IfrsStatementOfChangesInEquities { get; set; }

        public DbSet<IfrsStatementOfChangesInEquityRange> IfrsStatementOfChangesInEquityRanges { get; set; }


        public DbSet<RevenueBySalesType> RevenueBySalesTypes { get; set; }
        public DbSet<RevenueList> RevenueLists { get; set; }
        public DbSet<RevenueListRange> RevenueListRanges { get; set; }
        public DbSet<GaapIncomeStatement> GaapIncomeStatements { get; set; }

        public DbSet<GaapIncomeStatementOperationRevenue> GaapIncomeStatementOperationRevenues { get; set; }
        public DbSet<GaapIncomeStatementOperationRevenueRange> GaapIncomeStatementOperationRevenueRanges { get; set; }

        public DbSet<GaapIncomeStatementOperationExpense> GaapIncomeStatementOperationExpenses { get; set; }
        public DbSet<GaapIncomeStatementOperationExpenseRange> GaapIncomeStatementOperationExpenseRanges { get; set; }





        public DbSet<GaapBalanceSheet> GapBalanceSheets { get; set; }

        public DbSet<GaapBalanceSheetAsset> GaapBalanceSheetAssets { get; set; }
        public DbSet<GaapBalanceSheetAssetRange> GaapBalanceSheetAssetRanges { get; set; }
        public DbSet<GaapBalanceSheetLiability> GaapBalanceSheetLiabilities { get; set; }

        public DbSet<GaapBalanceSheetLiabilityRange> GaapBalanceSheetLiabilityRanges { get; set; }
        public DbSet<GaapBalanceSheetCapital> GaapBalanceSheetCapitals { get; set; }
        public DbSet<GaapBalanceSheetCapitalRange> GaapBalanceSheetCapitalRanges { get; set; }

        public DbSet<GaapCashFlow> GaapCashFlows { get; set; }

        public DbSet<GaapCashFlowReceipt> GaapCashFlowReceipts { get; set; }

        public DbSet<GaapCashFlowReceiptRange> GaapCashFlowReceiptRanges { get; set; }

        public DbSet<GaapCashFlowPayOut> GaapCashFlowPayOuts { get; set; }
        public DbSet<GaapCashFlowPayOutRange> GaapCashFlowPayOutRanges { get; set; }

        public DbSet<Core.Domain.Dashboard.FinanceDashboard> FinanceDashboards { get; set; }

        public DbSet<Core.Domain.Dashboard.FinanceDashboardRange> FinanceDashboardRanges { get; set; }
        public DbSet<FinanceSetting> FinanceSettings { get; set; }
        public DbSet<InvoiceArchive> InvoiceArchives { get; set; }

        #endregion

        #region Health
        public System.Data.Entity.DbSet<MedicalAttachment> MedicalAttachments { get; set; }
        public System.Data.Entity.DbSet<HealthRankAndValue> HealthRankAndValues { get; set; }
        public System.Data.Entity.DbSet<HealthRankAndValueValidation> HealthRankAndValueValidation { get; set; }
        public System.Data.Entity.DbSet<HealthRankAndValueFinalValidation> HealthRankAndValueFinalValidations { get; set; }
        public System.Data.Entity.DbSet<HealthRankAndValueSetting> HealthRankAndValueSettings { get; set; }
        public System.Data.Entity.DbSet<HealthResponsiblityLevelValueSetting> HealthResponsiblityLevelValueSettings { get; set; }
        public System.Data.Entity.DbSet<PaymentCoverageSetting> PaymentCoverageSettings { get; set; }
        public System.Data.Entity.DbSet<HealthBenefitSetting> HealthBenefitSettings { get; set; }
        public System.Data.Entity.DbSet<HealthApprovalSetting> HealthApprovalSettings { get; set; }
        public System.Data.Entity.DbSet<HealthExpenseAndTaxSetting> HealthExpenseAndTaxSettings { get; set; }
        public System.Data.Entity.DbSet<HealthAttachment> HealthAttachments { get; set; }
        public System.Data.Entity.DbSet<ReferralForm> ReferralForm { get; set; }
        public System.Data.Entity.DbSet<ReferralSheet> ReferralSheet { get; set; }
        public System.Data.Entity.DbSet<PatientsLeave> PatientsLeave { get; set; }
        public System.Data.Entity.DbSet<Treatment> Treatment { get; set; }
        public System.Data.Entity.DbSet<PatientCard> PatientCard { get; set; }
        public System.Data.Entity.DbSet<PatientCardDetail> PatientCardDetail { get; set; }
        public System.Data.Entity.DbSet<PatientResult> PatientResults { get; set; }
        public System.Data.Entity.DbSet<PatientResultDetail> PatientResultDetails { get; set; }
        public System.Data.Entity.DbSet<PatientDrugOrder> PatientDrugOrders { get; set; }
        public System.Data.Entity.DbSet<Diagnosis> Diagnosis { get; set; }
        public System.Data.Entity.DbSet<Prescription> Prescription { get; set; }
        public DbSet<LaboratoryTests> LaboratoryTests { get; set; }
        public DbSet<LaboratoryCatagory> LaboratoryCatagories { get; set; }
        public DbSet<LaboratoryTestPatientCard> LaboratoryTestPatientCards { get; set; }
        public DbSet<LaboratoryCatagoryPatientCard> LaboratoryCatagoryPatientCards { get; set; }
        #endregion
        #region Insurance
        public System.Data.Entity.DbSet<InsuranceGroup> InsuranceGroup { get; set; }
        public System.Data.Entity.DbSet<InsurancePremiumSetting> InsurancePremiumSettings { get; set; }
        public System.Data.Entity.DbSet<Insurance> Insurance { get; set; }
        public System.Data.Entity.DbSet<AccidentRecord> AccidentRecords { get; set; }
        public System.Data.Entity.DbSet<AccidentMedicalRecord> AccidentMedicalRecords { get; set; }
        public System.Data.Entity.DbSet<Restitution> Restitutions { get; set; }
        public System.Data.Entity.DbSet<CourtInsurance> CourtInsurance { get; set; }
        public System.Data.Entity.DbSet<InsuranceType> InsuranceType { get; set; }
        public System.Data.Entity.DbSet<InsuranceCategory> InsuranceCategory { get; set; }
        public System.Data.Entity.DbSet<EmployeeAccident> EmployeeAccidents { get; set; }
        public System.Data.Entity.DbSet<OtherInsurance> OtherInsurance { get; set; }
        public System.Data.Entity.DbSet<FixedAssetAccidentRecord> FixedAssetAccidentRecords { get; set; }
        public System.Data.Entity.DbSet<VehicleAccidentDetail> VehicleAccidentDetails { get; set; }
        public DbSet<InsuranceCoverType> InsuranceCoverTypes { get; set; }

        #region new Insurance 
        public System.Data.Entity.DbSet<EmployeeMedicalExpenseRefund> EmployeeMedicalExpenseRefunds { get; set; }
        public System.Data.Entity.DbSet<MedicalExpenseRefundAttachment> MedicalExpenseRefundAttachments { get; set; }
        public System.Data.Entity.DbSet<MedicalExpenseRefundDepartmentApproval> MedicalExpenseRefundDepartmentApprovals { get; set; }
        public System.Data.Entity.DbSet<MedicalExpenseRefundApproval> MedicalExpenseRefundApprovals { get; set; }
        public System.Data.Entity.DbSet<EmployeeAccidentInsuranceClaim> EmployeeAccidentInsuranceClaims { get; set; }
        public System.Data.Entity.DbSet<AccidentInsuranceClaimAttachment> AccidentInsuranceClaimAttachments { get; set; }
        public System.Data.Entity.DbSet<AccidentInsuranceClaimDepartmentApproval> AccidentInsuranceClaimDepartmentApprovals { get; set; }
        public System.Data.Entity.DbSet<AccidentInsuranceClaimApproval> AccidentInsuranceClaimApprovals { get; set; }
        public System.Data.Entity.DbSet<EmployeeAccidentCreditService> EmployeeAccidentCreditServices { get; set; }
        public System.Data.Entity.DbSet<AccidentCreditServiceAttachment> AccidentCreditServiceAttachments { get; set; }
        public System.Data.Entity.DbSet<AccidentCreditServiceDepartmentApproval> AccidentCreditServiceDepartmentApprovals { get; set; }
        public System.Data.Entity.DbSet<AccidentCreditServiceApproval> AccidentCreditServiceApprovals { get; set; }
        public System.Data.Entity.DbSet<InsurancePolicyRenewal> InsurancePolicyRenewals { get; set; }
        public System.Data.Entity.DbSet<PolicyRenewalAttachment> PolicyRenewalAttachments { get; set; }
        public System.Data.Entity.DbSet<PolicyRenewalDepartmentApproval> PolicyRenewalDepartmentApprovals { get; set; }
        public System.Data.Entity.DbSet<PolicyRenewalApproval> PolicyRenewalApprovals { get; set; }
        public System.Data.Entity.DbSet<EquipmentAccidentClaim> EquipmentAccidentClaims { get; set; }
        public System.Data.Entity.DbSet<EquipmentAccidentClaimAttachment> EquipmentAccidentClaimAttachment { get; set; }
        public System.Data.Entity.DbSet<AccidentClaimDepartmentApproval> AccidentClaimDepartmentApprovals { get; set; }
        public System.Data.Entity.DbSet<AccidentClaimApproval> AccidentClaimApprovals { get; set; }
        public System.Data.Entity.DbSet<FireAndLightningInsurance> FireAndLightningInsurances { get; set; }
        public System.Data.Entity.DbSet<FireAndLightningInsuranceDetail> FireAndLightningInsuranceDetails { get; set; }
        public System.Data.Entity.DbSet<FireAndLightningInsuranceAttachment> FireAndLightningInsuranceAttachment { get; set; }
        public System.Data.Entity.DbSet<FireAndLightningInsuranceDepartmentApproval> FireAndLightningInsuranceDepartmentApprovals { get; set; }
        public System.Data.Entity.DbSet<FireAndLightningInsuranceApproval> FireAndLightningInsuranceApprovals { get; set; }
        public System.Data.Entity.DbSet<InsurancePaymentSetting> InsurancePaymentSettings { get; set; }



        #endregion
        #endregion

        #region  InfoDesk
        #region Change Mgt
        public DbSet<ChangeRequest> ChangeRequests { get; set; }
        public DbSet<ChangeTask> ChangeTasks { get; set; }
        public DbSet<ChangeApproval> ChangeApprovals { get; set; }
        #endregion
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<SoftwareLicense> SoftwareLicenses { get; set; }
        public DbSet<AssignedUser> AssignedUsers { get; set; }
        public DbSet<InfoDeskKnowledgeBase> InfoDeskKnowledgeBases { get; set; }
        public DbSet<KnowledgeBaseAttachment> KnowledgeBaseAttachments { get; set; }
        public DbSet<CustomerTicketAttachment> CustomerTicketAttachments { get; set; }

        public DbSet<TicketType_HelpDesk> TicketType_HelpDesks { set; get; }
        public DbSet<Core.Domain.InfoDesk.Support.TicketStatusLog_HelpDesk> TicketStatusLog_HelpDesks { get; set; }

        public DbSet<Core.Domain.InfoDesk.Support.SupportGroupDetail_HelpDesk> SupportGroupDetail_HelpDesks { get; set; }
        public DbSet<Core.Domain.InfoDesk.Support.CustomerTicket_HelpDesk> CustomerTicket_HelpDesks { get; set; }

        public DbSet<Core.Domain.InfoDesk.Support.TicketActivityDefinition_HelpDesk> TicketActivityDefinition_HelpDesks { get; set; }

        public DbSet<Core.Domain.InfoDesk.Support.SupportGroup_HelpDesk> SupportGroup_HelpDesks { get; set; }

        public DbSet<Core.Domain.InfoDesk.Support.StatusType_HelpDesk> StatusType_HelpDesks { get; set; }

        public DbSet<Core.Domain.InfoDesk.Support.PriorityType_HelpDesk> PriorityType_HelpDesks { get; set; }
        public DbSet<Core.Domain.InfoDesk.Support.AssignTicket_HelpDesk> AssignTicket_HelpDesks { get; set; }

        //public DbSet<ExceedERP.Core.Domain.InfoDesk.Support.CustomerTicket_HelpDesk> CustomerTickets { get; set; }

        public DbSet<Core.Domain.InfoDesk.Support.MaterialItems> MaterialItems { get; set; }
        public DbSet<Core.Domain.InfoDesk.Support.MaterialsUsed> MaterialsUseds { get; set; }


        #endregion
        //#region  Estimation
        //#region Printimg Estimation
        //public DbSet<PrintingStep> PrintingSteps { get; set; }
        //public DbSet<EstimationForm> EstimationForms { get; set; }
        //public DbSet<MaterialCost> MaterialCosts { get; set; }
        //public DbSet<LaborCost> LaborCosts { get; set; }
        //public DbSet<OverallCost> OverallCosts { get; set; }
        //#endregion
        //#region Furniture Estimation
        //public DbSet<FurnitureCategory> FurnitureCategories { get; set; }
        //public DbSet<FurnitureCostEstimation> FurnitureCostEstimations { get; set; }
        //public DbSet<DirectLaborCost> DirectLaborCosts { get; set; }
        //public DbSet<FurnitureMaterialCost> FurnitureMaterialCosts { get; set; }
        //public DbSet<FurnitureOverallCost> FurnitureOverallCosts { get; set; }

        //#endregion
        //#endregion
        //#region Production Follow UP
        //public DbSet<YearlyProductionPlan> YearlyProductionPlans { get; set; }
        //public DbSet<HalfYearlyProductionPlan> HalfYearlyProductionPlans { get; set; }
        //public DbSet<QuarterlyProductionPlan> QuarterlyProductionPlans { get; set; }
        //public DbSet<MonthlyProductionPlan> MonthlyProductionPlans { get; set; }
        //public DbSet<WeeklyProductionPlan> WeeklyProductionPlans { get; set; }
        //public DbSet<ProductionJobTypePlan> ProductionJobTypePlans { get; set; }
        //public DbSet<ProductionCustomerType> ProductionCustomerTypes { get; set; }
        //public DbSet<Proforma> Proformas { get; set; }
        //public DbSet<ProductionYearlyPlan> ProductionYearlyPlans { get; set; }
        //public DbSet<ProductionMonthlyPlan> ProductionMonthlyPlans { get; set; }
        //public DbSet<ProductionMonthlyJobTypePlan> ProductionMonthlyJobTypePlans { get; set; }
        //public DbSet<PFProformaItems> PFProformaItems { get; set; }
        //public DbSet<JobCategory> JobCategories { get; set; }
        //public DbSet<Job> Jobs { get; set; }
        //public DbSet<JobStatus> JobStatuses { get; set; }
        //public DbSet<ChangeOrder> ChangeOrders { get; set; }
        //public DbSet<JobOrderMaterial> JobOrderMaterials { get; set; }
        //public DbSet<PrintingProductionFinishedGoodsStoreReceive> PrintingProductionFinishedGoodsStoreReceives { get; set; }
        //public DbSet<PrintingProductionFinishedGoodsStoreReceiveItem> PrintingProductionFinishedGoodsStoreReceiveItems { get; set; }
        //public DbSet<PrintingProductionFinishedGoodsStoreReceiveValidation> PrintingProductionFinishedGoodsStoreReceiveValidations { get; set; }
        //public DbSet<PrintingProductionFinishedGoodsStoreReceiveDistribution> PrintingProductionFinishedGoodsStoreReceiveDistributions { get; set; }
        //public DbSet<FinishedGoods> FinishedGoods { get; set; }
        //public DbSet<ProductDelivery> ProductDeliveries { get; set; }
        //public DbSet<ProductDeliveryItems> ProductDeliveryItems { get; set; }
        //public DbSet<JobStatusLineItem> JobStatusLineItems { get; set; }
        //#endregion
        //#region Job Costing
        //public DbSet<IdleTimeCategory> IdleTimeCategories { get; set; }
        //public DbSet<JcPension> JcPensions { get; set; }
        //public DbSet<YearlyOverHeadRate> YearlyOverHeadRates { get; set; }
        //public DbSet<Machine> Machines { get; set; }
        //public DbSet<Unit> Units { get; set; }
        //public DbSet<IdleTime> IdleTimes { get; set; }
        //public DbSet<IdleTimeOT> IdleTimeOTs { get; set; }
        //public DbSet<IdleTimeProductive> IdleTimeProductives { get; set; }
        //public DbSet<IdleTimeNonProductiveControllable> IdleTimeNonProductiveControllables { get; set; }
        //public DbSet<IdleTimeNonProductiveNonControllable> IdleTimeNonProductiveNonControllables { get; set; }
        //public DbSet<JobCost> JobCosts { get; set; }
        //public DbSet<JobChangeOfOrder> JobChangeOfOrders { get; set; }
        //public DbSet<JobLaborCost> JobLaborCosts { get; set; }
        //public DbSet<JobMaterialCost> JobMaterialCosts { get; set; }
        //public DbSet<JobMaterialReturn> JobMaterialReturns { get; set; }
        // public DbSet<FinishedJob> FinishedJobs { get; set; }
        //public DbSet<DeliveredJob> DeliveredJobs { get; set; }
        //public DbSet<DailyLaborRate> DailyLaborRates { get; set; }
        //public DbSet<DLRSalary> DLRSalaries { get; set; }
        //public DbSet<DLRHour> DLRHours { get; set; }
        //public DbSet<JobOrderTimeSheet> JobOrderTimeSheets { get; set; }
        //public DbSet<JobOrderTimeRegistration> JobOrderTimeRegistrations { get; set; }
        //public DbSet<JobOrderSummary> JobOrderSummarys { get; set; }
        //public DbSet<JobOrderSummaryCostCenter> JobOrderSummaryCostCenters { get; set; }
        //public DbSet<JobOrderSummaryCostCenterDetail> JobOrderSummaryCostCenterDetails { get; set; }

        //#endregion

        #region dashboard
        public DbSet<Core.Domain.Dashboard.InventoryItembalance> InventoryItembalances { get; set; }
        public DbSet<Core.Domain.Dashboard.DashboardDictionary> DashboardDictionaries { get; set; }


        #endregion

        #region globals

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Core.Domain.Global.GlobalRuleValue> GlobalRuleValues { get; set; }

        public DbSet<Core.Domain.Common.DocumentApproval.ApprovalDelegation> ApprovalDelegations { get; set; }

        public DbSet<Core.Domain.Common.DocumentApproval.DocumentApprovalRule> DocumentApprovalRules { get; set; }
        public DbSet<Core.Domain.Common.DocumentApproval.ApprovalGroup> ApprovalGroups { get; set; }
        public DbSet<Core.Domain.Common.DocumentApproval.ApprovalGroupRule> ApprovalGroupRules { get; set; }
        public DbSet<Core.Domain.Common.DocumentApproval.AssignApprovalGroup> AssignApprovalGroups { get; set; }
        public DbSet<Core.Domain.Common.DocumentApproval.AssignApprovalGroupAssignment> AssignApprovalGroupAssignments { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.EmployeeDetail.Freelancer> Freelancers { get; set; }
        public System.Data.Entity.DbSet<ExceedERP.Core.Domain.HRM.Attendance.AttendanceNew> AttendanceNews { get; set; }

        #endregion


        //audit

        //public override int SaveChanges()
        //{
        //    var modifiedEntries = ChangeTracker.Entries()
        //        .Where(x => x.Entity is TrackUserSettingOperation
        //                    && (x.State == EntityState.Added || x.State == EntityState.Modified));

        //    foreach (var entry in modifiedEntries)
        //    {
        //        TrackUserSettingOperation entity = entry.Entity as TrackUserSettingOperation;
        //        if (entity != null)
        //        {
        //            string identityName = Thread.CurrentPrincipal.Identity.Name;
        //            DateTime now = DateTime.Now;

        //            if (entry.State == EntityState.Added)
        //            {
        //                entity.CreatedBy = identityName;
        //                entity.DateCreated = now;
        //            }
        //            else
        //            {
        //                base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
        //                base.Entry(entity).Property(x => x.DateCreated).IsModified = false;
        //                entity.ModifiedBy = identityName;
        //                entity.LastModified = now;
        //            }

        //        }
        //    }

        //    return base.SaveChanges();
        //}




        public DbSet<SupplierRemoveBlackListValidation> SupplierRemoveBlackListValidations { get; set; }
        public DbSet<GlChartOfAccountCategory> GlChartOfAccountCategories { get; set; }



        public DbSet<Core.Domain.Finance.AP.ApSetupCommonSupplier> ApSetupCommonSuppliers { get; set; }

        public DbSet<RevenueAccount> RevenueAccounts { get; set; }

        public DbSet<StockClearingAccount> StockClearingAccounts { get; set; }

        public DbSet<LotPurchaseRequisition> LotPurchaseRequisitions { get; set; }

        public DbSet<ForeignCurrencyProcess> ForeignCurrencyProcesses { get; set; }

        public DbSet<GlBeginningPeriod> GlBeginningPeriods { get; set; }

        public DbSet<Core.Domain.Global.GlobalStatus> GlobalStatus { get; set; }

        public DbSet<PurchaseRequisitionSecondValidation> PurchaseRequisitionSecondValidations { get; set; }

        public DbSet<ArSalesOrderPartialPrint> ArSalesOrderPartialPrints { get; set; }
        public System.Data.Entity.DbSet<DelegationTempoRole> DelegationTempoRoles { get; set; }

    }
}