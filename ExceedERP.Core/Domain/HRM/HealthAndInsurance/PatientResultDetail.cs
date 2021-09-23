namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class PatientResultDetail
    {
        public int PatientResultDetailId { get; set; }
        public int PatientResultId { get; set; }    
        public string Description { get; set; }
        public string TestType { get; set; }

    }
}
