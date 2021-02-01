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
        Global=0,
        Finance=1,
        Budget=2,
        HRM=3,
        CRM=4,
        Payroll=5,
        Inventory=6,
        FixedAsset=7,
        Procurement=8,
        FleetAndDispatch=9,
        FleetMaintenance=10,
        Construction=11,
        ArchiveAndLetters=12,
        HealthAndInsurance=13,
        APTS=14,
        SelfCare=15,
        Administration=16
        
    }
    public class GlobalRuleValue
    {
        public int GlobalRuleValueId { get; set; }
        public Modules Module { get; set; }
        public KeyType Key { get; set; }
        public string Value { get; set; }
    }
}
