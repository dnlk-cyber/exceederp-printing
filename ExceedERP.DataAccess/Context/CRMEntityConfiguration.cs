using ExceedERP.Core.Domain.CRM;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace ExceedERP.DataAccess.Context
{
    public class CRMEntityConfiguration
    {

        public class CashSalesPartialLineItemEntityTypeConfiguration : EntityTypeConfiguration<CashSalesPartialLineItem>
        {

            public CashSalesPartialLineItemEntityTypeConfiguration()
            {
                this.ToTable("CashSalesPartialLineItem", "CRM");
                this.HasKey<int>(t => t.CashSalesPartialLineItemId);

            }
        }

        public class AdvancePaymentLogEntityTypeConfiguration : EntityTypeConfiguration<AdvancePaymentLog>
        {

            public AdvancePaymentLogEntityTypeConfiguration()
            {
                this.ToTable("AdvancePaymentLog", "CRM");
                this.HasKey<int>(t => t.AdvancePaymentLogId);

            }
        }
        public class CashSalesPartialItemEntityTypeConfiguration : EntityTypeConfiguration<CashSalesPartialItem>
        {

            public CashSalesPartialItemEntityTypeConfiguration()
            {
                this.ToTable("CashSalesPartialItem", "CRM");
                this.HasKey<int>(t => t.CashSalesPartialItemId);

            }
        }
        public class SupportGroupEntityTypeConfiguration : EntityTypeConfiguration<SupportGroup>
        {

            public SupportGroupEntityTypeConfiguration()
            {
                this.ToTable("SupportGroup", "CRM");
                this.HasKey<int>(t => t.SupportGroupId);

            }
        }



        public class AdvancePaymentDistibutionEntityTypeConfiguration : EntityTypeConfiguration<AdvancePaymentDistibution>
        {

            public AdvancePaymentDistibutionEntityTypeConfiguration()
            {
                this.ToTable("AdvancePaymentDistibution", "CRM");
                this.HasKey<int>(t => t.AdvancePaymentDistibutionId);

                this.Property(p => p.Debit).HasPrecision(18, 4);
                this.Property(p => p.Credit).HasPrecision(18, 4);
            }
        }
        public class SalesInvoicePaymentDistibutionEntityTypeConfiguration : EntityTypeConfiguration<SalesInvoicePaymentDistibution>
        {

            public SalesInvoicePaymentDistibutionEntityTypeConfiguration()
            {
                this.ToTable("SalesInvoicePaymentDistibution", "CRM");
                this.HasKey<int>(t => t.SalesInvoicePaymentDistibutionId);

                this.Property(p => p.Debit).HasPrecision(18, 4);
                this.Property(p => p.Credit).HasPrecision(18, 4);
            }
        }
        public class TicketStatusLogEntityTypeConfiguration : EntityTypeConfiguration<TicketStatusLog>
        {

            public TicketStatusLogEntityTypeConfiguration()
            {
                this.ToTable("TicketStatusLog", "CRM");
                this.HasKey<int>(t => t.TicketStatusLogId);

            }
        }
        public class SupportGroupDetailEntityTypeConfiguration : EntityTypeConfiguration<SupportGroupDetail>
        {

            public SupportGroupDetailEntityTypeConfiguration()
            {
                this.ToTable("SupportGroupDetail", "CRM");
                this.HasKey<int>(t => t.SupportGroupDetailId);

            }
        }
        public class StatusTypeEntityTypeConfiguration : EntityTypeConfiguration<StatusType>
        {

            public StatusTypeEntityTypeConfiguration()
            {
                this.ToTable("StatusType", "CRM");
                this.HasKey<int>(t => t.StatusTypeId);

            }
        }
        public class PriorityTypeEntityTypeConfiguration : EntityTypeConfiguration<PriorityType>
        {

            public PriorityTypeEntityTypeConfiguration()
            {
                this.ToTable("PriorityType", "CRM");
                this.HasKey<int>(t => t.PriorityTypeId);

            }
        }
        public class AssignTicketEntityTypeConfiguration : EntityTypeConfiguration<AssignTicket>
        {

            public AssignTicketEntityTypeConfiguration()
            {
                this.ToTable("AssignTicket", "CRM");
                this.HasKey<int>(t => t.AssignTicketId);

            }
        }
        public class VarietyEntityTypeConfiguration : EntityTypeConfiguration<Variety>
        {

            public VarietyEntityTypeConfiguration()
            {
                this.ToTable("Variety", "CRM");
                this.HasKey<int>(t => t.VarietyID);

            }
        }


        public class AdvancePaymentLineEntityTypeConfiguration : EntityTypeConfiguration<AdvancePaymentLine>
        {

            public AdvancePaymentLineEntityTypeConfiguration()
            {
                this.ToTable("AdvancePaymentLine", "CRM");
                this.HasKey<int>(t => t.AdvancePaymentLineId);

            }
        }


        public class AdvancePaymentEntityTypeConfiguration : EntityTypeConfiguration<AdvancePayment>
        {

            public AdvancePaymentEntityTypeConfiguration()
            {
                this.ToTable("AdvancePayment", "CRM");
                this.HasKey<int>(t => t.AdvancePaymentId);

            }
        }


        public class CampaignProductEntityTypeConfiguration : EntityTypeConfiguration<CampaignProduct>
        {

            public CampaignProductEntityTypeConfiguration()
            {
                this.ToTable("CampaignProduct", "CRM");
                this.HasKey<int>(t => t.CampaignProductID);

            }
        }
        public class CreditSettingEntityTypeConfiguration : EntityTypeConfiguration<CreditSetting>
        {

            public CreditSettingEntityTypeConfiguration()
            {
                this.ToTable("CreditSetting", "CRM");
                this.HasKey<int>(t => t.CreditSettingID);

            }
        }
        public class CreditSettlementEntityTypeConfiguration : EntityTypeConfiguration<CreditSettlement>
        {

            public CreditSettlementEntityTypeConfiguration()
            {
                this.ToTable("CreditSettlement", "CRM");
                this.HasKey<int>(t => t.CreditSettlementId);

            }
        }
        public class CreditSettlementValidationEntityTypeConfiguration : EntityTypeConfiguration<CreditSettlementValidation>
        {

            public CreditSettlementValidationEntityTypeConfiguration()
            {
                this.ToTable("CreditSettlementValidation", "CRM");
                this.HasKey<int>(t => t.CreditSettlementValidationId);

            }
        }
        public class CreditSettlementDistributionEntityTypeConfiguration : EntityTypeConfiguration<CreditSettlementDistribution>
        {

            public CreditSettlementDistributionEntityTypeConfiguration()
            {
                this.ToTable("CreditSettlementDistribution", "CRM");
                this.HasKey<int>(t => t.CreditSettlementDistributionId);

                this.Property(p => p.Debit).HasPrecision(18, 4);
                this.Property(p => p.Credit).HasPrecision(18, 4);

            }
        }
        public class LineCreditSettlementEntityTypeConfiguration : EntityTypeConfiguration<LineCreditSettlement>
        {

            public LineCreditSettlementEntityTypeConfiguration()
            {
                this.ToTable("LineCreditSettlement", "CRM");
                this.HasKey<int>(t => t.LineCreditSettlementId);

                this.Property(p => p.SettelmentAmount).HasPrecision(18, 4);
                this.Property(p => p.NetBalance).HasPrecision(18, 4);
                this.Property(p => p.RemainingAmount).HasPrecision(18, 4);
                this.Property(p => p.WithholdingAmount).HasPrecision(18, 4);
            }
        }
        public class ServiceLineItemEntityTypeConfiguration : EntityTypeConfiguration<ServiceLineItem>
        {

            public ServiceLineItemEntityTypeConfiguration()
            {
                this.ToTable("ServiceLineItem", "CRM");
                this.HasKey<int>(t => t.ServiceLineItemId);

                this.Property(p => p.UnitAmount).HasPrecision(18, 4);
                this.Property(p => p.SubTotal).HasPrecision(18, 4);
                this.Property(p => p.TotalAmount).HasPrecision(18, 4);
                this.Property(p => p.TaxAmount).HasPrecision(18, 4);
                this.Property(p => p.WithholdingAmount).HasPrecision(18, 4);
                this.Property(p => p.Quantity).HasPrecision(18, 4);
                this.Property(p => p.WithholdTaxAmount).HasPrecision(18, 4);
            }
        }

        public class ServiceUsageEntityTypeConfiguration : EntityTypeConfiguration<ServiceUsage>
        {

            public ServiceUsageEntityTypeConfiguration()
            {
                this.ToTable("ServiceUsage", "CRM");
                this.HasKey<int>(t => t.ServiceUsageId);

            }
        }

        public class ServiceUsageApprovalEntityTypeConfiguration : EntityTypeConfiguration<ServiceUsageApproval>
        {

            public ServiceUsageApprovalEntityTypeConfiguration()
            {
                this.ToTable("ServiceUsageApproval", "CRM");
                this.HasKey<int>(t => t.ServiceUsageApprovalId);

            }
        }
        public class CRMHolidayEntityTypeConfiguration : EntityTypeConfiguration<CRMHoliday>
        {

            public CRMHolidayEntityTypeConfiguration()
            {
                this.ToTable("CRMHoliday", "CRM");
                this.HasKey<int>(t => t.CRMHolidayID);

            }
        }

        public class CustomerServiceEntityTypeConfiguration : EntityTypeConfiguration<CustomerService>
        {

            public CustomerServiceEntityTypeConfiguration()
            {
                this.ToTable("CustomerService", "CRM");
                this.HasKey<int>(t => t.CustomerServiceId);

            }
        }

        public class HolidayDefinitionEntityTypeConfiguration : EntityTypeConfiguration<HolidayDefinition>
        {

            public HolidayDefinitionEntityTypeConfiguration()
            {
                this.ToTable("HolidayDefinition", "CRM");
                this.HasKey<int>(t => t.HolidayDefinitionID);

            }
        }

        public class ReceiveMethodEntityTypeConfiguration : EntityTypeConfiguration<ReceiveMethod>
        {

            public ReceiveMethodEntityTypeConfiguration()
            {
                this.ToTable("ReceiveMethod", "CRM");
                this.HasKey<int>(t => t.ReceiveMethodID);

            }
        }

        public class TermEntityTypeConfiguration : EntityTypeConfiguration<Term>
        {

            public TermEntityTypeConfiguration()
            {
                this.ToTable("Term", "CRM");
                this.HasKey<int>(t => t.TermID);

            }
        }

        public class InventoryCateoryMapEntityTypeConfiguration : EntityTypeConfiguration<InventoryCateoryMap>
        {

            public InventoryCateoryMapEntityTypeConfiguration()
            {
                this.ToTable("InventoryCateoryMap", "CRM");
                this.HasKey<Guid>(t => t.InventoryCateoryMapId);

            }
        }
        public class VoucherDefinitionEntityTypeConfiguration : EntityTypeConfiguration<VoucherDefinition>
        {

            public VoucherDefinitionEntityTypeConfiguration()
            {
                this.ToTable("VoucherDefinition", "CRM");
                this.HasKey<int>(vd => vd.VoucherDefinitionID);

            }
        }


        public class VoucherTermEntityTypeConfiguration : EntityTypeConfiguration<VoucherTerm>
        {

            public VoucherTermEntityTypeConfiguration()
            {
                this.ToTable("VoucherTerm", "CRM");
                this.HasKey<int>(t => t.VoucherTermID);

            }
        }

        public class TaxEntityTypeConfiguration : EntityTypeConfiguration<Tax>
        {

            public TaxEntityTypeConfiguration()
            {
                this.ToTable("Tax", "CRM");
                this.HasKey<int>(t => t.TaxID);

            }
        }

        public class TaxTransactionEntityTypeConfiguration : EntityTypeConfiguration<TaxTransaction>
        {

            public TaxTransactionEntityTypeConfiguration()
            {
                this.ToTable("TaxTransaction", "CRM");
                this.HasKey<int>(t => t.TaxTransactionID);

            }
        }
        public class PriceEntityTypeConfiguration : EntityTypeConfiguration<Price>
        {

            public PriceEntityTypeConfiguration()
            {
                this.ToTable("Price", "CRM");
                this.Property(p => p.PriceID).HasColumnOrder(0);
                this.Property(p => p.Thing)
                 .HasColumnAnnotation(
            IndexAnnotation.AnnotationName,
            new IndexAnnotation(
                new IndexAttribute("IX_Priority", 1) { IsUnique = true })
                )
                .HasColumnOrder(1)
                .HasMaxLength(50)
                .IsRequired();
                this.Property(p => p.ReferenceType).HasColumnOrder(3);
                this.Property(p => p.Reference).HasColumnOrder(4);
                this.Property(p => p.Value).HasColumnOrder(5);
                this.Property(p => p.IsDefault).HasColumnOrder(6);
                this.Property(p => p.Value).HasPrecision(18, 4);
                this.Property(p => p.Priority)
                 .HasColumnAnnotation(
            IndexAnnotation.AnnotationName,
            new IndexAnnotation(
                new IndexAttribute("IX_Priority", 2) { IsUnique = true }))
                 .HasColumnOrder(7)
                 .IsRequired();

                this.Property(p => p.Remark).HasColumnOrder(8);
                this.HasKey<int>(t => t.PriceID);


            }
        }
        public class LineItemEntityTypeConfiguration : EntityTypeConfiguration<LineItem>
        {

            public LineItemEntityTypeConfiguration()
            {
                this.ToTable("LineItem", "CRM");
                this.Property(p => p.ThingID);

                this.Property(p => p.StockBalance).HasPrecision(18, 4);

                this.Property(p => p.StockBalanceReserved).HasPrecision(18, 4);
                this.Property(p => p.UnitAmount).HasPrecision(18, 4);
                this.Property(p => p.Quantity).HasPrecision(18, 4);
                this.Property(p => p.TotalAmount).HasPrecision(18, 4);
                this.Property(p => p.CalculatedCost).HasPrecision(18, 4);
                this.Property(p => p.TaxAmount).HasPrecision(18, 4);
                this.Property(p => p.WithholdingAmount).HasPrecision(18, 4);
                this.Property(p => p.WithholdTaxAmount).HasPrecision(18, 4);
                this.Property(p => p.Vat).HasPrecision(18, 4);
                this.Property(p => p.Discount).HasPrecision(18, 4);

            }
        }

        public class UserEntityTypeConfiguration : EntityTypeConfiguration<User>
        {

            public UserEntityTypeConfiguration()
            {
                this.ToTable("User", "CRM");
                this.HasKey<int>(t => t.UserID);

            }
        }

        public class LifeTimeEntityTypeConfiguration : EntityTypeConfiguration<LifeTime>
        {

            public LifeTimeEntityTypeConfiguration()
            {
                this.ToTable("LifeTime", "CRM");
                this.HasKey<int>(t => t.LifeTimeID);

            }
        }




        public class ActivityDefinitionEntityTypeConfiguration : EntityTypeConfiguration<ActivityDefinition>
        {

            public ActivityDefinitionEntityTypeConfiguration()
            {
                this.ToTable("ActivityDefinition", "CRM");
                this.HasKey<int>(t => t.ActivityDefinitionID);
            }
        }
        public class ActivityEntityTypeConfiguration : EntityTypeConfiguration<Activity>
        {

            public ActivityEntityTypeConfiguration()
            {
                this.ToTable("Activity", "CRM");
                this.HasKey<int>(t => t.ActivityID);
            }
        }
        public class GlobalTypeListEntityTypeConfiguration : EntityTypeConfiguration<GlobalTypeList>
        {

            public GlobalTypeListEntityTypeConfiguration()
            {
                this.ToTable("GlobalTypeList", "CRM");
                this.HasKey<int>(t => t.GlobalTypeListID);
            }
        }



        public class VoucherExtensionEntityTypeConfiguration : EntityTypeConfiguration<VoucherExtension>
        {

            public VoucherExtensionEntityTypeConfiguration()
            {
                this.ToTable("VoucherExtension", "CRM");
                this.HasKey<int>(t => t.VoucherExtensionID);
            }
        }
        public class VoucherExtensionTransactionEntityTypeConfiguration : EntityTypeConfiguration<VoucherExtensionTransaction>
        {

            public VoucherExtensionTransactionEntityTypeConfiguration()
            {
                this.ToTable("VoucherExtensionTransaction", "CRM");
                this.HasKey<int>(t => t.VoucherExtensionTransactionID);
            }
        }

        public class SerialDefinitionEntityTypeConfiguration : EntityTypeConfiguration<SerialDefinition>
        {

            public SerialDefinitionEntityTypeConfiguration()
            {
                this.ToTable("SerialDefinition", "CRM");
                this.HasKey<int>(t => t.SerialDefinitionID);
            }
        }
        public class SerialCodeEntityTypeConfiguration : EntityTypeConfiguration<SerialCode>
        {
            public SerialCodeEntityTypeConfiguration()
            {
                this.ToTable("SerialCode", "CRM");
                this.HasKey<int>(t => t.SerialCodeID);
            }
        }

        public class TransactionReferenceEntityTypeConfiguration : EntityTypeConfiguration<TransactionReference>
        {
            public TransactionReferenceEntityTypeConfiguration()
            {
                this.ToTable("TransactionReference", "CRM");
                this.HasKey<int>(t => t.TransactionReferenceID);
            }
        }

        public class StoreTransactionEntityTypeConfiguration : EntityTypeConfiguration<StoreTransaction>
        {
            public StoreTransactionEntityTypeConfiguration()
            {
                this.ToTable("StoreTransaction", "CRM");
                this.HasKey<int>(t => t.StoreTransactionID);
            }
        }

        public class StockBalanceEntityTypeConfiguration : EntityTypeConfiguration<StockBalance>
        {
            public StockBalanceEntityTypeConfiguration()
            {
                this.ToTable("StockBalance", "CRM");
                this.HasKey<int>(t => t.StockBalanceID);
            }
        }

        public class StockLevelEntityTypeConfiguration : EntityTypeConfiguration<StockLevel>
        {
            public StockLevelEntityTypeConfiguration()
            {
                this.ToTable("StockLevel", "CRM");
                this.HasKey<int>(t => t.StockLevelID);
            }
        }
        public class VoucherValueEntityTypeConfiguration : EntityTypeConfiguration<VoucherValue>
        {
            public VoucherValueEntityTypeConfiguration()
            {
                this.ToTable("VoucherValue", "CRM");
                this.HasKey<int>(t => t.VoucherValueID);
            }
        }
        public class GlobalTypeTaxEntityTypeConfiguration : EntityTypeConfiguration<GlobalTypeTax>
        {
            public GlobalTypeTaxEntityTypeConfiguration()
            {
                this.ToTable("GlobalTypeTax", "CRM");
                this.HasKey<int>(t => t.GlobalTypeTaxID);
            }
        }
        public class ControlAccountEntityTypeConfiguration : EntityTypeConfiguration<ControlAccount>
        {
            public ControlAccountEntityTypeConfiguration()
            {
                this.ToTable("ControlAccount", "CRM");
                this.HasKey<int>(t => t.ControlAccountId);
            }
        }

        public class AccountRequirementEntityTypeConfiguration : EntityTypeConfiguration<AccountRequirement>
        {
            public AccountRequirementEntityTypeConfiguration()
            {
                this.ToTable("AccountRequirement", "CRM");
                this.HasKey<int>(t => t.AccountRequirementID);
            }
        }
        public class AccountEntityTypeConfiguration : EntityTypeConfiguration<Accounts>
        {
            public AccountEntityTypeConfiguration()
            {
                this.ToTable("Accounts", "CRM");
                this.HasKey<int>(t => t.AccountsID);
            }
        }
        public class AccountMapEntityTypeConfiguration : EntityTypeConfiguration<AccountMap>
        {
            public AccountMapEntityTypeConfiguration()
            {
                this.ToTable("AccountMap", "CRM");
                this.HasKey<int>(t => t.AccountMapID);
            }
        }


        public class JournalTemplateEntityTypeConfiguration : EntityTypeConfiguration<JournalTemplate>
        {
            public JournalTemplateEntityTypeConfiguration()
            {
                this.ToTable("JournalTemplate", "CRM");
                this.HasKey<int>(t => t.JournalTemplateID);
            }
        }
        public class JournalDetailEntityTypeConfiguration : EntityTypeConfiguration<JournalDetail>
        {
            public JournalDetailEntityTypeConfiguration()
            {
                this.ToTable("JournalDetail", "CRM");
                this.HasKey<int>(t => t.JournalDetailID);
            }
        }

        public class BeginningEntityTypeConfiguration : EntityTypeConfiguration<Beginning>
        {
            public BeginningEntityTypeConfiguration()
            {
                this.ToTable("Beginning", "CRM");
                this.HasKey<int>(t => t.BeginningID);
            }
        }

        public class CommunicationSourceEntityTypeConfiguration : EntityTypeConfiguration<CommunicationSource>
        {
            public CommunicationSourceEntityTypeConfiguration()
            {
                this.ToTable("CommunicationSource", "CRM");
                this.HasKey<int>(t => t.CommunicationSourceID);
            }
        }
        public class CurrencyEntityTypeConfiguration : EntityTypeConfiguration<Currency>
        {
            public CurrencyEntityTypeConfiguration()
            {
                this.ToTable("Currency", "CRM");
                this.HasKey<int>(t => t.CurrencyID);
            }
        }
        public class CurrencyPreferenceEntityTypeConfiguration : EntityTypeConfiguration<CurrencyPreference>
        {
            public CurrencyPreferenceEntityTypeConfiguration()
            {
                this.ToTable("CurrencyPreference", "CRM");
                this.HasKey<int>(t => t.CurrencyPreferenceID);
            }
        }




        public class ObjectStateEntityTypeConfiguration : EntityTypeConfiguration<ObjectState>
        {
            public ObjectStateEntityTypeConfiguration()
            {
                this.ToTable("ObjectState", "CRM");
                this.HasKey<int>(t => t.ObjectStateID);
            }
        }
        class ReligionEntityTypeConfiguration : EntityTypeConfiguration<Religion>
        {
            public ReligionEntityTypeConfiguration()
            {
                this.ToTable("Religion", "CRM");
                this.HasKey<int>(t => t.ReligionID);
            }
        }
        class ValueFactorDefinitionEntityTypeConfiguration : EntityTypeConfiguration<ValueFactorDefinition>
        {
            public ValueFactorDefinitionEntityTypeConfiguration()
            {
                this.ToTable("ValueFactorDefinition", "CRM");
                this.HasKey<int>(t => t.ValueFactorDefinitionID);
            }
        }
        public class ValueFactorEntityTypeConfiguration : EntityTypeConfiguration<ValueFactor>
        {
            public ValueFactorEntityTypeConfiguration()
            {
                this.ToTable("ValueFactor", "CRM");
                this.HasKey<int>(t => t.ValueFactorID);
            }
        }
        public class SalesQuoteVoucherEntityTypeConfiguration : EntityTypeConfiguration<SalesQuoteVoucher>
        {
            public SalesQuoteVoucherEntityTypeConfiguration()
            {
                this.ToTable("SalesQuoteVoucher", "CRM");
                this.HasKey<int>(t => t.SalesQuoteVoucherID);
            }
        }

        public class CartTransactionEntityTypeConfiguration : EntityTypeConfiguration<CartTransaction>
        {
            public CartTransactionEntityTypeConfiguration()
            {
                this.ToTable("CartTransaction", "CRM");
                this.HasKey<int>(t => t.CartTransactionID);
            }
        }
        public class BankAccountDetailEntityTypeConfiguration : EntityTypeConfiguration<BankAccountDetail>
        {
            public BankAccountDetailEntityTypeConfiguration()
            {
                this.ToTable("BankAccountDetail", "CRM");
                this.HasKey<int>(t => t.BankAccountDetailID);
            }
        }
        #region Cmmon
        public class StoreEntityTypeConfiguration : EntityTypeConfiguration<Store>
        {

            public StoreEntityTypeConfiguration()
            {
                this.ToTable("Store", "CRM");
                this.HasKey<int>(o => o.StoreID);
            }
        }

        public class CartEntityTypeConfiguration : EntityTypeConfiguration<Cart>
        {

            public CartEntityTypeConfiguration()
            {
                this.ToTable("Cart", "CRM");
                this.HasKey<int>(o => o.CartID);
            }
        }
        public class OrganizationCustomerEntityTypeConfiguration : EntityTypeConfiguration<OrganizationCustomer>
        {

            public OrganizationCustomerEntityTypeConfiguration()
            {
                this.ToTable("OrganizationCustomer", "CRM");
                this.HasKey<int>(o => o.OrganizationCustomerID);
            }
        }
        public class SalesStoreEntityTypeConfiguration : EntityTypeConfiguration<SalesStore>
        {

            public SalesStoreEntityTypeConfiguration()
            {
                this.ToTable("SalesStore", "CRM");
                this.HasKey<int>(o => o.SalesStoreId);
            }
        }
        public class SalesPersonAssignmentEntityTypeConfiguration : EntityTypeConfiguration<SalesPersonAssignment>
        {

            public SalesPersonAssignmentEntityTypeConfiguration()
            {
                this.ToTable("SalesPersonAssignment", "CRM");
                this.HasKey<int>(o => o.SalesPersonAssignmentId);
            }
        }
        public class AdditionalCostLogEntityTypeConfiguration : EntityTypeConfiguration<AdditionalCostLog>
        {

            public AdditionalCostLogEntityTypeConfiguration()
            {
                this.ToTable("AdditionalCostLog", "CRM");
                this.HasKey<int>(o => o.AdditionalCostLogId);
            }
        }
        public class CommissionLineItemEntityTypeConfiguration : EntityTypeConfiguration<CommissionLineItem>
        {

            public CommissionLineItemEntityTypeConfiguration()
            {
                this.ToTable("CommissionLineItem", "CRM");
                this.HasKey<int>(o => o.CommissionLineItemId);
            }
        }
        public class AdditionalCostEntityTypeConfiguration : EntityTypeConfiguration<AdditionalCost>
        {

            public AdditionalCostEntityTypeConfiguration()
            {
                this.ToTable("AdditionalCost", "CRM");
                this.HasKey<int>(o => o.AdditionalCostId);
            }
        }
        public class OrganizationCategoryEntityTypeConfiguration : EntityTypeConfiguration<OrganizationCategory>
        {

            public OrganizationCategoryEntityTypeConfiguration()
            {
                this.ToTable("OrganizationCategory", "CRM");
                this.HasKey<int>(o => o.OrganizationCategoryID);
            }
        }
        public class ThingCategoryEntityTypeConfiguration : EntityTypeConfiguration<ProductCategory>
        {

            public ThingCategoryEntityTypeConfiguration()
            {

                this.Property(p => p.Index)
                .HasColumnAnnotation(
           IndexAnnotation.AnnotationName,
           new IndexAnnotation(
               new IndexAttribute("IX_ThingCategory", 1) { IsUnique = true })
               )
               .HasColumnOrder(1)
               .IsRequired();
                this.HasKey<int>(o => o.ProductCategoryId);
            }
        }

        public class PersonCategoryEntityTypeConfiguration : EntityTypeConfiguration<PersonCategory>
        {

            public PersonCategoryEntityTypeConfiguration()
            {
                this.ToTable("PersonCategory", "CRM");
                this.HasKey<int>(o => o.PersonCategoryID);
            }
        }
        public class ProductEntityTypeConfiguration : EntityTypeConfiguration<Product>
        {

            public ProductEntityTypeConfiguration()
            {
                this.ToTable("Product", "CRM");
                this.HasKey<int>(o => o.ProductID);
            }

        }
        public class ProductPriceLogEntityTypeConfiguration : EntityTypeConfiguration<ProductPriceLog>
        {

            public ProductPriceLogEntityTypeConfiguration()
            {
                this.ToTable("ProductPriceLog", "CRM");
                this.HasKey<int>(o => o.ProductPriceLogId);

                this.Property(p => p.InitialPrice).HasPrecision(18, 4);
                this.Property(p => p.ProposedNewPrice).HasPrecision(18, 4);
                this.Property(p => p.ApprovedNewPrice).HasPrecision(18, 4);
                this.Property(p => p.NewExchangeRate).HasPrecision(18, 4);
            }

        }
        public class ProductUnitConvertorSetupEntityTypeConfiguration : EntityTypeConfiguration<ProductUnitConvertorSetup>
        {

            public ProductUnitConvertorSetupEntityTypeConfiguration()
            {
                this.ToTable("ProductUnitConvertorSetup", "CRM");
                this.HasKey<int>(o => o.ProductUnitConvertorSetupId);
                this.Property(p => p.MultiplicationFactor).HasPrecision(18, 4);
            }

        }
        public class ObjectStateDefinitionEntityTypeConfiguration : EntityTypeConfiguration<ObjectStateDefinition>
        {

            public ObjectStateDefinitionEntityTypeConfiguration()
            {
                this.ToTable("ObjectStateDefinition", "CRM");
                this.HasKey<int>(o => o.ObjectStateDefinitionID);
            }
        }




        public class LifeSpanEntityTypeConfiguration : EntityTypeConfiguration<LifeSpan>
        {

            public LifeSpanEntityTypeConfiguration()
            {
                this.ToTable("LifeSpan", "CRM");
                this.HasKey<int>(o => o.LifeSpanID);
            }
        }
        public class NonCashTransactionEntityTypeConfiguration : EntityTypeConfiguration<NonCashTransaction>
        {

            public NonCashTransactionEntityTypeConfiguration()
            {
                this.ToTable("NonCashTransaction", "CRM");
                this.HasKey<int>(o => o.NonCashTransactionID);
            }
        }
        public class CampaignEntityTypeConfiguration : EntityTypeConfiguration<Campaign>
        {

            public CampaignEntityTypeConfiguration()
            {
                this.ToTable("Campaign", "CRM");
                this.HasKey<int>(o => o.CampaignID);
            }
        }
        public class CashSalesInvoiceEntityTypeConfiguration : EntityTypeConfiguration<CashSalesInvoice>
        {

            public CashSalesInvoiceEntityTypeConfiguration()
            {
                this.ToTable("CashSalesInvoice", "CRM");
                this.HasKey<int>(o => o.CashSalesInvoiceID);
            }
        }
        public class CRMDepositEntityTypeConfiguration : EntityTypeConfiguration<CRMDeposit>
        {

            public CRMDepositEntityTypeConfiguration()
            {
                this.ToTable("CRMDeposit", "CRM");
                this.HasKey<int>(o => o.CRMDepositId);

            }
        }
        public class CRMDepositLineEntityTypeConfiguration : EntityTypeConfiguration<CRMDepositLine>
        {

            public CRMDepositLineEntityTypeConfiguration()
            {
                this.ToTable("CRMDepositLine", "CRM");
                this.HasKey<int>(o => o.CRMDepositLineId);

                this.Property(p => p.DepositAmount).HasPrecision(18, 4);
            }
        }
        public class CRMDepositValidationEntityTypeConfiguration : EntityTypeConfiguration<CRMDepositValidation>
        {

            public CRMDepositValidationEntityTypeConfiguration()
            {
                this.ToTable("CRMDepositValidation", "CRM");
                this.HasKey<int>(o => o.CRMDepositValidationId);
            }
        }
        public class CRMDepositDistributionEntityTypeConfiguration : EntityTypeConfiguration<CRMDepositDistribution>
        {

            public CRMDepositDistributionEntityTypeConfiguration()
            {
                this.ToTable("CRMDepositDistribution", "CRM");
                this.HasKey<int>(o => o.CRMDepositDistributionId);

                this.Property(p => p.Debit).HasPrecision(18, 4);
                this.Property(p => p.Credit).HasPrecision(18, 4);
            }
        }
        public class CustomerTicketEntityTypeConfiguration : EntityTypeConfiguration<CustomerTicket>
        {

            public CustomerTicketEntityTypeConfiguration()
            {
                this.ToTable("CustomerTicket", "CRM");
                this.HasKey<int>(o => o.CustomerTicketID);
            }
        }
        public class TicketActivityDefinitionEntityTypeConfiguration : EntityTypeConfiguration<TicketActivityDefinition>
        {

            public TicketActivityDefinitionEntityTypeConfiguration()
            {
                this.ToTable("TicketActivityDefinition", "CRM");
                this.HasKey<int>(o => o.TicketActivityDefinitionID);
            }
        }

        public class TicketAssignmentEntityTypeConfiguration : EntityTypeConfiguration<TicketAssignment>
        {

            public TicketAssignmentEntityTypeConfiguration()
            {
                this.ToTable("TicketAssignment", "CRM");
                this.HasKey<int>(o => o.TicketAssignmentID);
            }
        }
        public class TicketRoleEntityTypeConfiguration : EntityTypeConfiguration<TicketRole>
        {
            public TicketRoleEntityTypeConfiguration()
            {
                this.ToTable("TicketRole", "CRM");
                this.HasKey<int>(o => o.TicketRoleID);
            }
        }
        public class RoleEntityTypeConfiguration : EntityTypeConfiguration<Role>
        {
            public RoleEntityTypeConfiguration()
            {
                this.ToTable("Role", "CRM");
                this.HasKey<int>(o => o.RoleID);
            }
        }


        #endregion

        public class SalesOrderVoucherEntityTypeConfiguration : EntityTypeConfiguration<SalesOrderVoucher>
        {

            public SalesOrderVoucherEntityTypeConfiguration()
            {
                this.ToTable("SalesOrderVoucher", "CRM");
                this.HasKey<int>(o => o.SalesOrderVoucherID);
            }
        }



        public class DefaultAccountEntityTypeConfiguration : EntityTypeConfiguration<DefaultLedgerAccount>
        {
            public DefaultAccountEntityTypeConfiguration()
            {
                this.ToTable("DefaultLedgerAccount", "CRM");
                this.HasKey<int>(o => o.DefaultLedgerAccountID);
            }
        }

        public class RangeEntityTypeConfiguration : EntityTypeConfiguration<Range>
        {
            public RangeEntityTypeConfiguration()
            {
                this.ToTable("Range", "CRM");
                this.HasKey<int>(o => o.RangeID);
            }
        }
        public class SpecificationEntityTypeConfiguration : EntityTypeConfiguration<Specification>
        {
            public SpecificationEntityTypeConfiguration()
            {
                this.ToTable("Specification", "CRM");

                this.Property(p => p.ThingID)
                 .HasColumnAnnotation(
            IndexAnnotation.AnnotationName,
            new IndexAnnotation(
                new IndexAttribute("IX_SpecificationIndex", 1) { IsUnique = true }))
                .IsRequired()
                   .HasMaxLength(50);

                this.HasKey<int>(o => o.SpecificationID);

                this.Property(p => p.Index)
                .HasColumnAnnotation(
           IndexAnnotation.AnnotationName,
           new IndexAnnotation(
               new IndexAttribute("IX_SpecificationIndex", 2) { IsUnique = true }))
               .IsRequired();

            }
        }
        public class CreditSalesInvoiceEntityTypeConfiguration : EntityTypeConfiguration<CreditSalesInvoice>
        {
            public CreditSalesInvoiceEntityTypeConfiguration()
            {
                this.ToTable("CreditSalesInvoice", "CRM");
                this.HasKey<int>(o => o.CreditSalesInvoiceID);
            }
        }
        public class ObjectAccountRequirementEntityTypeConfiguration : EntityTypeConfiguration<ObjectAccountRequirement>
        {
            public ObjectAccountRequirementEntityTypeConfiguration()
            {
                this.ToTable("ObjectAccountRequirement", "CRM");
                this.HasKey<int>(o => o.ObjectAccountRequirementID);
            }
        }
        public class ThingCategoryDefaultAccountEntityTypeConfiguration : EntityTypeConfiguration<ThingCategoryDefaultAccount>
        {
            public ThingCategoryDefaultAccountEntityTypeConfiguration()
            {
                this.ToTable("ThingCategoryDefaultAccount", "CRM");
                this.HasKey<int>(t => t.ThingCategoryDefaultAccountID);
            }
        }
        public class CustomerHandlingTeamEntityTypeConfiguration : EntityTypeConfiguration<CustomerHandlingTeam>
        {
            public CustomerHandlingTeamEntityTypeConfiguration()
            {
                this.ToTable("CustomerHandlingTeam", "CRM");
                this.HasKey<int>(c => c.CustomerHandlingTeamID);
            }
        }
        public class SubCountryEntityTypeConfiguration : EntityTypeConfiguration<SubCountry>
        {
            public SubCountryEntityTypeConfiguration()
            {
                this.ToTable("SubCountry", "CRM");
                this.HasKey<int>(c => c.SubCountryID);
            }
        }


        public class IssueHistoryEntityTypeConfiguration : EntityTypeConfiguration<IssueHistory>
        {
            public IssueHistoryEntityTypeConfiguration()
            {
                this.ToTable("IssueHistory", "CRM");
                this.HasKey<int>(c => c.IssueHistoryId);
            }
        }
        public class MailModelEntityTypeConfiguration : EntityTypeConfiguration<MailModel>
        {
            public MailModelEntityTypeConfiguration()
            {
                this.ToTable("MailModel", "CRM");
                this.HasKey<int>(c => c.MailModelID);
            }
        }
        public class PreCustomerScheduleEntityTypeConfiguration : EntityTypeConfiguration<PreCustomerSchedule>
        {
            public PreCustomerScheduleEntityTypeConfiguration()
            {
                this.ToTable("PreCustomerSchedule", "CRM");
            }
        }
        public class PreCustomerEntityTypeConfiguration : EntityTypeConfiguration<PreCustomer>
        {
            public PreCustomerEntityTypeConfiguration()
            {
                this.ToTable("PreCustomer", "CRM");
            }
        }
        public class CRMPeriodEntityTypeConfiguration : EntityTypeConfiguration<CRMPeriod>
        {
            public CRMPeriodEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.CRMPeriodId);
                this.ToTable("CRMPeriod", "CRM");
            }
        }
        public class SalesShipmentEntityTypeConfiguration : EntityTypeConfiguration<SalesShipment>
        {
            public SalesShipmentEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.SalesShipmentId);
                this.ToTable("SalesShipments", "CRM");
            }
        }


        public class SalesShipmentLineItemEntityTypeConfiguration : EntityTypeConfiguration<SalesShipmentLineItem>
        {
            public SalesShipmentLineItemEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.SalesShipmentLineItemId);
                this.ToTable("SalesShipmentLineItem", "CRM");

                this.Property(p => p.SubTotal).HasPrecision(18, 4);

                this.Property(p => p.TotalQuantity).HasPrecision(18, 4);
                this.Property(p => p.TotalAmount).HasPrecision(18, 4);
                this.Property(p => p.Price).HasPrecision(18, 4);
                this.Property(p => p.TaxAmount).HasPrecision(18, 4);
                this.Property(p => p.ShipedQuantityTilPreShipment).HasPrecision(18, 4);
                this.Property(p => p.ShipedQuantity).HasPrecision(18, 4);
                this.Property(p => p.RemainingQuantity).HasPrecision(18, 4);
            }
        }
        public class SalesShipmentValidationEntityTypeConfiguration : EntityTypeConfiguration<SalesShipmentValidation>
        {
            public SalesShipmentValidationEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.SalesShipmentValidationId);
                this.ToTable("SalesShipmentValidation", "CRM");
            }
        }
        public class SalesShipmentDistributionEntityTypeConfiguration : EntityTypeConfiguration<SalesShipmentDistribution>
        {
            public SalesShipmentDistributionEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.SalesShipmentDistributionId);
                this.ToTable("SalesShipmentDistribution", "CRM");

                this.Property(p => p.Debit).HasPrecision(18, 4);
                this.Property(p => p.Credit).HasPrecision(18, 4);
            }
        }

        public class QuotationTermEntityTypeConfiguration : EntityTypeConfiguration<QuotationTerm>
        {
            public QuotationTermEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.QuotationTermId);
                this.ToTable("QuotationTerm", "CRM");
            }
        }
        public class SalesQuoteVoucherValidationEntityTypeConfiguration : EntityTypeConfiguration<SalesQuoteVoucherValidation>
        {
            public SalesQuoteVoucherValidationEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.SalesQuoteVoucherValidationId);
                this.ToTable("SalesQuoteVoucherValidation", "CRM");
            }
        }
        public class SalesQuoteVoucherFirstValidationEntityTypeConfiguration : EntityTypeConfiguration<SalesQuoteVoucherFirstValidation>
        {
            public SalesQuoteVoucherFirstValidationEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.SalesQuoteVoucherFirstValidationId);
                this.ToTable("SalesQuoteVoucherFirstValidation", "CRM");
            }
        }
        public class SalesQuoteVoucherSecondValidationEntityTypeConfiguration : EntityTypeConfiguration<SalesQuoteVoucherSecondValidation>
        {
            public SalesQuoteVoucherSecondValidationEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.SalesQuoteVoucherSecondValidationId);
                this.ToTable("SalesQuoteVoucherSecondValidation", "CRM");
            }
        }
        public class SalesOrderVoucherValidationEntityTypeConfiguration : EntityTypeConfiguration<SalesOrderVoucherValidation>
        {
            public SalesOrderVoucherValidationEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.SalesOrderVoucherValidationId);
                this.ToTable("SalesOrderVoucherValidation", "CRM");
            }
        }

        public class VoucherContractDocumentEntityTypeConfiguration : EntityTypeConfiguration<VoucherContractDocument>
        {
            public VoucherContractDocumentEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.VoucherContractDocumentId);
                this.ToTable("VoucherContractDocument", "CRM");
            }
        }
        public class PaymentScheduleEntityTypeConfiguration : EntityTypeConfiguration<PaymentSchedule>
        {
            public PaymentScheduleEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.PaymentScheduleId);
                this.ToTable("PaymentSchedule", "CRM");
            }
        }
        public class CashSalesInvoiceValidationEntityTypeConfiguration : EntityTypeConfiguration<CashSalesInvoiceValidation>
        {
            public CashSalesInvoiceValidationEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.CashSalesInvoiceValidationId);
                this.ToTable("CashSalesInvoiceValidation", "CRM");
            }
        }
        public class PosSettingEntityTypeConfiguration : EntityTypeConfiguration<PosSetting>
        {
            public PosSettingEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.PosSettingId);
                this.ToTable("PosSetting", "CRM");
            }
        }
        public class AnnualSalesPlanEntityTypeConfiguration : EntityTypeConfiguration<AnnualSalesPlan>
        {
            public AnnualSalesPlanEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.AnnualSalesPlanId);
                this.ToTable("AnnualSalesPlan", "CRM");
            }
        }
        public class BranchAnnualSalesPlanEntityTypeConfiguration : EntityTypeConfiguration<BranchAnnualSalesPlan>
        {
            public BranchAnnualSalesPlanEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.BranchAnnualSalesPlanId);
                this.ToTable("BranchAnnualSalesPlan", "CRM");
            }
        }
        public class StoreAnnualSalesPlanEntityTypeConfiguration : EntityTypeConfiguration<StoreAnnualSalesPlan>
        {
            public StoreAnnualSalesPlanEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.StoreAnnualSalesPlanId);
                this.ToTable("StoreAnnualSalesPlan", "CRM");
            }
        }
        public class StoreMonthlySalesPlanEntityTypeConfiguration : EntityTypeConfiguration<StoreMonthlySalesPlan>
        {
            public StoreMonthlySalesPlanEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.StoreMonthlySalesPlanId);
                this.ToTable("StoreMonthlySalesPlan", "CRM");
            }
        }
        public class AnnualSalesPlanValidationEntityTypeConfiguration : EntityTypeConfiguration<AnnualSalesPlanValidation>
        {
            public AnnualSalesPlanValidationEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.AnnualSalesPlanValidationId);
                this.ToTable("AnnualSalesPlanValidation", "CRM");
            }
        }
        public class CRMDashboardTodateSalesDataEntityTypeConfiguration : EntityTypeConfiguration<CRMDashboardTodateSalesData>
        {
            public CRMDashboardTodateSalesDataEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.CRMDashboardTodateSalesDataId);
                this.ToTable("CRMDashboardTodateSalesData", "CRM");
                this.Property(p => p.Amount).HasPrecision(20, 4);
            }
        }
        public class CRMDashboardTodateSalesBranchDataEntityTypeConfiguration : EntityTypeConfiguration<CRMDashboardTodateSalesBranchData>
        {
            public CRMDashboardTodateSalesBranchDataEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.CRMDashboardTodateSalesBranchDataId);
                this.ToTable("CRMDashboardTodateSalesBranchData", "CRM");
                this.Property(p => p.Amount).HasPrecision(20, 4);
            }
        }
        #region Customer Queue
        public class QueueTypeEntityTypeConfiguration : EntityTypeConfiguration<QueueType>
        {
            public QueueTypeEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.QueueTypeId);
                this.ToTable("QueueType", "CRM");
            }
        }

        public class InvestmentTypeEntityTypeConfiguration : EntityTypeConfiguration<InvestmentType>
        {
            public InvestmentTypeEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.InvestmentTypeId);
                this.ToTable("InvestmentType", "CRM");
            }
        }
        public class LocationMapEntityTypeConfiguration : EntityTypeConfiguration<LocationMap>
        {
            public LocationMapEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.LocationMapId);
                this.ToTable("LocationMap", "CRM");
            }
        }
        public class LocationUserAssignmentEntityTypeConfiguration : EntityTypeConfiguration<LocationUserAssignment>
        {
            public LocationUserAssignmentEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.LocationUserAssignmentId);
                this.ToTable("LocationUserAssignment", "CRM");
            }
        }
        public class QueueCustomerEntityTypeConfiguration : EntityTypeConfiguration<QueueCustomer>
        {
            public QueueCustomerEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.QueueCustomerId);
                this.ToTable("QueueCustomer", "CRM");
            }
        }
        public class QueueEntityTypeConfiguration : EntityTypeConfiguration<Queue>
        {
            public QueueEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.QueueId);
                this.ToTable("Queue", "CRM");
            }
        }
        public class QueueStatusEntityTypeConfiguration : EntityTypeConfiguration<QueueStatus>
        {
            public QueueStatusEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.QueueStatusId);
                this.ToTable("QueueStatus", "CRM");
            }
        }
        public class QueueValidationEntityTypeConfiguration : EntityTypeConfiguration<QueueValidation>
        {
            public QueueValidationEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.QueueValidationId);
                this.ToTable("QueueValidation", "CRM");
            }
        }

        public class QueueStatusValidationEntityTypeConfiguration : EntityTypeConfiguration<QueueStatusValidation>
        {
            public QueueStatusValidationEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.QueueStatusValidationId);
                this.ToTable("QueueStatusValidation", "CRM");
            }
        }
        #endregion

        #region price estimation quote
        public class PriceEstimationQuoteVoucherEntityTypeConfiguration : EntityTypeConfiguration<PriceEstimationQuoteVoucher>
        {
            public PriceEstimationQuoteVoucherEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.PriceEstimationQuoteVoucherId);
                this.ToTable("PriceEstimationQuoteVouchers", "CRM");
                this.Property(p => p.GrandTotal).HasPrecision(20, 4);
            }
        }

        public class PriceEstimationQuoteVoucherLineEntityTypeConfiguration : EntityTypeConfiguration<PriceEstimationQuoteVoucherLine>
        {

            public PriceEstimationQuoteVoucherLineEntityTypeConfiguration()
            {
                this.ToTable("PriceEstimationQuoteVoucherLines", "CRM");
                this.HasKey<int>(t => t.PriceEstimationQuoteVoucherLineId);

                this.Property(p => p.UnitAmount).HasPrecision(18, 4);
                this.Property(p => p.SubTotal).HasPrecision(18, 4);
                this.Property(p => p.TotalAmount).HasPrecision(18, 4);
                this.Property(p => p.TaxAmount).HasPrecision(18, 4);
                this.Property(p => p.WithholdingAmount).HasPrecision(18, 4);
                this.Property(p => p.WithholdTaxAmount).HasPrecision(18, 4);
                this.Property(p => p.Quantity).HasPrecision(18, 4);
            }
        }
        public class PriceEstimationQuoteVoucherServiceEntityTypeConfiguration : EntityTypeConfiguration<PriceEstimationQuoteVoucherService>
        {

            public PriceEstimationQuoteVoucherServiceEntityTypeConfiguration()
            {
                this.ToTable("PriceEstimationQuoteVoucherServices", "CRM");
                this.HasKey<int>(t => t.PriceEstimationQuoteVoucherServiceId);

                this.Property(p => p.UnitAmount).HasPrecision(18, 4);
                this.Property(p => p.SubTotal).HasPrecision(18, 4);
                this.Property(p => p.TotalAmount).HasPrecision(18, 4);
                this.Property(p => p.TaxAmount).HasPrecision(18, 4);
                this.Property(p => p.WithholdingAmount).HasPrecision(18, 4);
                this.Property(p => p.WithholdTaxAmount).HasPrecision(18, 4);
                this.Property(p => p.Quantity).HasPrecision(18, 4);
            }
        }
        public class PriceEstimationQuoteVoucherFirstValidationEntityTypeConfiguration : EntityTypeConfiguration<PriceEstimationQuoteVoucherFirstValidation>
        {
            public PriceEstimationQuoteVoucherFirstValidationEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.PriceEstimationQuoteVoucherFirstValidationId);
                this.ToTable("PriceEstimationQuoteVoucherFirstValidation", "CRM");
            }
        }
        public class PriceEstimationQuoteVoucherSecondValidationEntityTypeConfiguration : EntityTypeConfiguration<PriceEstimationQuoteVoucherSecondValidation>
        {
            public PriceEstimationQuoteVoucherSecondValidationEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.PriceEstimationQuoteVoucherSecondValidationId);
                this.ToTable("PriceEstimationQuoteVoucherSecondValidation", "CRM");
            }
        }
        public class PriceEstimationQuoteVoucherValidationEntityTypeConfiguration : EntityTypeConfiguration<PriceEstimationQuoteVoucherValidation>
        {
            public PriceEstimationQuoteVoucherValidationEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.PriceEstimationQuoteVoucherValidationId);
                this.ToTable("PriceEstimationQuoteVoucherValidation", "CRM");
            }
        }
        #endregion

        #region tender followup
        public class TenderFollowupEntityTypeConfiguration : EntityTypeConfiguration<TenderFollowup>
        {
            public TenderFollowupEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.TenderFollowupId);
                this.ToTable("TenderFollowup", "CRM");
                this.Property(p => p.ServiceCharge).HasPrecision(20, 4);
                this.Property(p => p.BidBondAmount).HasPrecision(20, 4);
                this.Property(p => p.BidAmount).HasPrecision(20, 4);
                this.Property(p => p.BidSecurityCharge).HasPrecision(20, 4);
                this.Property(p => p.BidValidty).HasPrecision(20, 4);
            }
        }
        public class TenderFollowupLineItemEntityTypeConfiguration : EntityTypeConfiguration<TenderFollowupLineItem>
        {
            public TenderFollowupLineItemEntityTypeConfiguration()
            {
                this.HasKey<int>(f => f.TenderFollowupLineItemId);
                this.ToTable("TenderFollowupLineItem", "CRM");
            }
        }
        #endregion
    }
}
