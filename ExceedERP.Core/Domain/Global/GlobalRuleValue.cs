using System.ComponentModel;

namespace ExceedERP.Core.Domain.Global
{
    

    public enum KeyType
    {
        [Category("Procurement")]
        OpenBidMinimumNumberOfSupplier,
        RestrictedBidMinimumNumberOfSupplier,
        ProformaMinimumNumberOfSupplier,
        ProformaPurchaseMaxAmount,
        GuaranteeExpiryAlarmDays,
        ContractExpiryAlarmDays,
        TenderMinimumNumberOfSupplier,
    }
    public enum Modules
    {
        Global = 0,
        Finance = 1,
        Budget = 2,
        HRM = 3,
        CRM = 4,
        Payroll = 5,
        Inventory = 6,
        FixedAsset = 7,
        Procurement = 8,
        FleetAndDispatch = 9,
        FleetMaintenance = 10,
        Construction = 11,
        ArchiveAndLetters = 12,
        HealthAndInsurance = 13,
        APTS = 14,
        SelfCare = 15,
        Administration = 16,
        Maintenance = 17,
        ImportExport = 18,
        GeneralService = 19,
        TDS = 20,
        DesignWork = 21,
        REM = 22,
        Planning = 23,
        FleetManagement = 24,
        ConsProduction = 25,
        LegalCase = 26,
        InternalAudit = 27,
        Health = 28,
        Insurance = 29,
        CPM = 30,
        InfoDesk = 31,
        LabManagement = 32,
        Common = 33,
    }
    public class GlobalRuleValue
    {
        public int GlobalRuleValueId { get; set; }
        public Modules Module { get; set; }
        public KeyType Key { get; set; }
        public string Value { get; set; }
    }
}
