namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class PatientDrugOrder
    {
        public int PatientDrugOrderId { get; set; }
        public int DrugListsId { get; set; }
        public int DrugUnitId { get; set; }
        public int Quantity { get; set; }
        public string Prescription { get; set; }
        public int PatientCardDetailId { get; set; }
        public int PatientId { get; set; }
        public bool IsSentToPatientRegiser { get; set; }
    }
}
