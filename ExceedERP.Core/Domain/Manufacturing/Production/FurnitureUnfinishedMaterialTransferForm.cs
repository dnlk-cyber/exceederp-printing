using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Manufacturing.Production
{
    public class FurnitureUnfinishedMaterialTransferForm : TrackUserSettingOperation
    {
        [Key]
        public int FurnitureUnfinishedMaterialTransferFormId { get; set; }
        [Display(Name ="Job Type")]
        public int JobTypeId { get; set; }
        public int Quantity { get; set; }
        [Display(Name ="Transfer Date"), DataType(DataType.Date)]
        public DateTime TransferDate { get; set; }
        [Display(Name ="Transferer Name")]
        public int TransfererId { get; set; }
        [Display(Name ="Receiver Name")]
        public int ReceiverId { get; set; }
        [Display(Name ="Quality")]
        public QualityGrade QualityGrade { get; set; }
        [Display(Name ="Progress(%)")]
        public double WorkProgressInPercentage { get; set; }
        public bool IsSent { get; set; }
        public string SentBy { get; set; }
        public DateTime? SentDate { get; set; } 
        public bool IsReceived { get; set; }
        public string ReceivedBy { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public int FurnitureJobOrderProductionId { get; set; }
    }
    public enum QualityGrade
    {
        Excellent,
        Very_Good,
        Good
    }
}
