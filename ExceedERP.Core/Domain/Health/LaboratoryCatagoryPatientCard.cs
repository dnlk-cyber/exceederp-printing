namespace ExceedERP.Core.Domain.Health
{

    public class LaboratoryCatagoryPatientCard
    {
        public int LaboratoryCatagoryPatientCardId { get; set; }
        public int PatientId { get; set; }
        public int PatientCardDetailId { get; set; }
        public string Catagory { get; set; }
    }
}
