using ExceedERP.Core.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Construction
{
  public  class DocumentManagement
    {
    }

  public class ProjectDocument
  {[Key ]
      public int ProjectdDocumentId { get; set; }
       [DisplayName("Document Type")]
       [StringLength(100)]
      public string DocumentType { get; set; }
      public byte[] File { get; set; }
      public string Description { get; set; }
      public virtual ProjectInformation Project { get; set; }
  }
}
