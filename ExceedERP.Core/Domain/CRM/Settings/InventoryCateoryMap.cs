using System;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class InventoryCateoryMap : TrackUserSettingOperation
    {
        public Guid InventoryCateoryMapId { get; set; }
        [Required]
        [DisplayName("Inventory Category")]
        public string InventoryCategoryId { get; set; }
        [Required]
        public string Category { get; set; }
        public string Remark { get; set; }
    }
}
