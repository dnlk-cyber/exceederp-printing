using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CMS
{
    public  class CMSProgram : TrackUserSettingOperation
    {
        [Key]
        public int CMSProgramId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }   
    }
}
