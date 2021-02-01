namespace ExceedERP.Core.Domain.HRM.ArchiveManagement
{
    public class EmployeeIncommingUploadDbModel
    {
        public int EmployeeIncommingUploadDbModelId { get; set; }
        public string BookNo { get; set; }
        public virtual EmployeeIncomingLetter EmployeeIncomingLetter { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}