namespace ExceedERP.Core.Domain.Global
{
    public class UserBusinessUnitMap
    {
        public int UserBusinessUnitMapId { get; set; }
        public string UserId { get; set; }
        public int BusinessUnitId { get; set; }
        public bool IsDefault { get; set; }
    }
  
}
