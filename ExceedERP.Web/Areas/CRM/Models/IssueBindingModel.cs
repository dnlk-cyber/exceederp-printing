using System.Collections.Generic;

namespace ExceedERP.Web.Areas.CRM.Models
{
    public class IssueBindingModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProductName { get; set; }

    }


    public class GenralIssueBindingModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ProductName { get; set; }

        public List<IssueVewModel> Property { get; set; }
    }
}