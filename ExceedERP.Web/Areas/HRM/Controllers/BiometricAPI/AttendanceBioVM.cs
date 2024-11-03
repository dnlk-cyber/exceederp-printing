using System.Collections.Generic;

namespace ExceedERP.Web.Areas.HRM.Controllers.BiometricAPI
{
    public class AttendanceBioVM
    {
        public string Count { get; set; }
        public string Next { get; set; }
        public string Previous { get; set; }
        public string Code { get; set; }
        public List<BioDetailVM> BioDetailVM { get; set; }
    }

    public class BioDetailVM
    {
        public string Id { get; set; }
        public string Emp_code { get; set; }
        public string Punch_Time { get; set; }
        public string Punch_State { get; set; }
        public string Verify_Type { get; set; }
        public string Work_Code { get; set; }
        public string Terminal_sn { get; set; }
        public string Terminal_alias { get; set; }
        public string Area_alias { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string gps_location { get; set; }
        public string mobile { get; set; }
        public int source { get; set; }
        public string purpose { get; set; }
        public string crc { get; set; }
        public int is_attendance { get; set; }
        public string reserved { get; set; }
        public string upload_time { get; set; }
        public string sync_status { get; set; }
        public string sync_time { get; set; }
        public string emp { get; set; }
        public string terminal { get; set; }
    }
}