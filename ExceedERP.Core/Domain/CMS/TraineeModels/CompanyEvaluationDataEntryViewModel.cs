using System.Collections.Generic;

namespace ExceedERP.Core.Domain.CMS
{
    public class CompanyEvaluationDataEntryViewModel
    {
        public int CmsTraineeId { get; set; }
        public CmsTrainee Trainee { get; set; } = new CmsTrainee();

        public List<CompanyEvaluationDataEntryISOViwModel> ISOTypesList { get; set; } = new List<CompanyEvaluationDataEntryISOViwModel>();

    }
    public class CompanyEvaluationDataEntryISOViwModel
    {
        public string Name { get; set; }
        public int ISOTypeId { get; set; }
        public List<CompanyEvaluationDataEntryChecklistViewModel> ISOChecklists { get; set; } = new List<CompanyEvaluationDataEntryChecklistViewModel>();
    }
    public class CompanyEvaluationDataEntryChecklistViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int ISOChecklistId { get; set; }

        public bool IsSelected { get; set; }
        public decimal  NumericValue { get; set; }
        public string Remark { get; set; }
        public ISOChecklistValueType ValueType { get; set; }

    }
}
