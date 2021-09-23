using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CMS
{
    public class ISOChecklist : TrackUserSettingOperation
    {
        [Key]
        public int ISOChecklistId { get; set; }
        public int ISOTypeId { get; set; }
        public virtual ISOType   ISOType { get; set; }

        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ISOChecklistValueType ValueType { get; set; }
    }
}
