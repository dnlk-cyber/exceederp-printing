using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CMS
{
    public class TraineeIdNumberComponent : TrackUserSettingOperation
    {
        [Key]
        public int TraineeIdNumberComponentId { get; set; }

        [Display(Name ="Prefix")]
        public int RegistrationNumberPrefixId { get; set; }
        //public virtual RegistrationNumberPrefix RegistrationNumberPrefix { get; set; }
        [Display(Name ="ISO Type")]
        public int ISOTypeId { get; set; }

        [Display(Name ="Trainee")]
        public int CmsTraineeId { get; set; }
        public virtual CmsTrainee Trainee { get; set; }

        /// <summary>
        /// WTTC-3030
        /// </summary>
        public string PrefixValue { get; set; }
        /// <summary>
        /// 9606-1
        /// </summary>
        public string ISOType { get; set; }

        /// <summary>
        /// 170526-1742088
        /// </summary>
        public string SystemId { get; set; }

        /// <summary>
        /// 00001
        /// </summary>
        public string SequenceNumber { get; set; }

        /// <summary>
        /// full id eg. WTTC-3030-9606-1-170526-1742088.000001
        /// </summary>
        public string CombinedIdNumber { get; set; }

    }

    public class TraineeIdGenerationStatusViewModel
    {
        public bool IsSuccessful { get; set; }
        public TraineeIdNumberComponent TraineeIdNumberComponent { get; set; } = new TraineeIdNumberComponent();
        public string ErrorMessage { get; set; }
    }
}
