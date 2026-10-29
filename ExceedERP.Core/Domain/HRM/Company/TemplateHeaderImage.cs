using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.Company
{
    public class TemplateHeaderImage
    {
        public int TemplateHeaderImageId { get; set; }
        public bool IsNeened { get; set; }
        public string Name { get; set; }
        public string HeaderImageFileName { get; set; }
    }
}
