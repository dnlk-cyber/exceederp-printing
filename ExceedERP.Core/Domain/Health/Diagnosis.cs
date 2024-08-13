namespace ExceedERP.Core.Domain.Health
{
    public class Diagnosis
    {
        public int DiagnosisId { get; set; }
        public int PatientResultId { get; set; }
        public string Description { get; set; }
    }
}
