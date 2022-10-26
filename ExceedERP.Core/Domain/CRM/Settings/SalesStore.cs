using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class SalesStore : TrackUserOperation
    {
        [DisplayName("Id")]
        public int SalesStoreId { get; set; }

        public string StoreCode { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }

       
        public virtual InventoryStore InventoryStore { get; set; }

        public SalesStoreType Type { get; set; }
        /// <summary>
        /// reference to POS id (number they use on a given store or shop)
        /// </summary>
        public string PosId { get; set; }
        [Display(Name = "POS Service Base URL")]
        public string PosServiceBaseUrl { get; set; }

        [NotMapped]
        public bool HasReference { get; set; }
    }

    public class SalesPersonAssignment : TrackUserOperation
    {
        [Key]
        public int SalesPersonAssignmentId { get; set; }
        [Display(Name ="Store/Shop")]
        public int SalesStoreId { get; set; }
        public virtual SalesStore Store { get; set; }
        [Display(Name ="User")]
        public string UserName { get; set; }

        public string FullName { get; set; }

        [Display(Name ="Receive Method ")]
        public int PaymentMethodId { get; set; }

        [Display(Name = "Cash on hand Receive Method ")]
        public int CashOnHandPaymentMethodId { get; set; }
    }
}
