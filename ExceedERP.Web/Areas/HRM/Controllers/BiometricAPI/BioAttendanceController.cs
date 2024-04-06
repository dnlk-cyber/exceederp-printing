using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Attendance;
using ExceedERP.DataAccess.Context;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.BiometricAPI
{
    public class BioAttendanceController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET api/<controller>
        public ActionResult Index()
        {
            TestApi();
            return View();
        }
        public async void TestApi()
        {
            var attendances = await Api_GetAttendance("1000", "3", "3");

            foreach (var att in attendances)
            {
                foreach (var bio in att.BioDetailVM)
                {
                    DateTime.TryParse(bio.Punch_Time, out DateTime punchTime);
                    DateTime date = punchTime.Date;
                    var attendance = db.AttendanceBios.FirstOrDefault(x => x.EmpCode == bio.Emp_code && x.Date == date);
                    if(attendance != null)
                    {
                        attendance.Date = date;
                        if(attendance.PunchTime1 == null)
                        {
                            attendance.PunchTime1 = punchTime;
                        }
                        else if (attendance.PunchTime2 == null)
                        {
                            attendance.PunchTime2 = punchTime;
                        }
                        else if (attendance.PunchTime3 == null)
                        {
                            attendance.PunchTime3 = punchTime;
                        }
                        else if (attendance.PunchTime4 == null)
                        {
                            attendance.PunchTime4 = punchTime;
                        }
                        else if (attendance.PunchTime5 == null)
                        {
                            attendance.PunchTime5 = punchTime;
                        }
                        else if (attendance.PunchTime6 == null)
                        {
                            attendance.PunchTime6 = punchTime;
                        }

                        db.Entry(attendance).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        var entity = new AttendanceBio
                        {
                            EmpCode = bio.Emp_code,
                            Date = date.Date,
                            PunchTime1 = punchTime,
                        };

                        db.AttendanceBios.Add(entity);
                        db.SaveChanges();
                    }                   
                }
            }
        }
        public async Task<List<AttendanceBioVM>> Api_GetAttendance(string empCode, string startTime, string endTime)
        {
            AttendanceBioVM tinInfo = new AttendanceBioVM();
            //var lng = new LanguageHelper().GetLanguage(Request);

            // string baseUrl = "http://192.168.1.17:5050";

            string baseUrl = "http://localhost:8087/api/AttendanceApi";
            var baseUrlSetting = await db.BioAttendanceIntegrationSettings.FirstOrDefaultAsync(d => d.Type == ApiIntegrationSettingAttribute.BaseUrl);
            if (baseUrlSetting != null)
            {
                baseUrl = baseUrlSetting.Value;
            }
            if (baseUrl != null && !string.IsNullOrEmpty(empCode) && !string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(startTime))
            {
                //var fullPath = $"{baseUrl}iclock/api/transactions?emp_code={empCode}&start_time = {startTime} & end_time = {endTime}";
                var fullPath = $"{baseUrl}?emp_code={empCode}&start_time={startTime}&end_time={endTime}";

                #region fetch data from api
                using (HttpClient client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(completeUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    //AuthenticationHeaderValue authHeader = new AuthenticationHeaderValue("bearer", userToken.AccessToken);
                    //client.DefaultRequestHeaders.Authorization = authHeader;
                    //try
                    //{
                    HttpResponseMessage response = await client.GetAsync(fullPath);

                    if (response.IsSuccessStatusCode)
                    {
                        if (response.Content != null)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();

                            var tinInfos = JsonConvert.DeserializeObject<List<AttendanceBioVM>>(responseContent);

                            return tinInfos;
                            //if (tinInfos.Any())
                            //{
                            //    tinInfo = tinInfos.FirstOrDefault();
                            //}
                        }
                    }
                    //}
                    //catch (Exception exception)
                    //{
                    //    var msg = exception.Message;
                    //}

                }
                #endregion
            }

            return new List<AttendanceBioVM>();
        }

        public async Task<JsonResult> GetTinInformation(string empCode, string startTime, string endTime)
        {
            if (!string.IsNullOrEmpty(empCode) && !string.IsNullOrEmpty(startTime) && !string.IsNullOrEmpty(endTime))
            {
                return Json(new
                {
                    FirstNameAmh = "",
                    MiddleNameAmh = "",
                    LastNameAmh = "",
                    FirstNameEng = "",
                    MiddleNameEng = "",
                    LastNameEng = "",
                    TinNo = "",
                    HouseNo = "",
                    PhoneNo = "",
                    Message = "",
                    Error = "",
                    IsExist = true
                });

            }
            else
            {
                var attendance = await Api_GetAttendance(empCode, startTime, endTime);

                if (attendance != null)
                {
                    return Json(new
                    {
                        //FirstNameAmh = attendance.FIRSTNAME_F,
                        //MiddleNameAmh = attendance.MIDDLENAME_F,
                        //LastNameAmh = attendance.LASTNAME_F,
                        //FirstNameEng = attendance.FIRSTNAME,
                        //MiddleNameEng = attendance.MIDDLENAME,
                        //LastNameEng = attendance.LASTNAME,
                        //TinNo = attendance.CMP_TIN,
                        //HouseNo = attendance.HOUSE_NO,
                        //PhoneNo = attendance.MOBILE_PHONE,
                        Message = "",
                        Error = "",
                        IsExist = false,
                        IsMerchant = true
                    });
                }
                else
                {
                    return Json(new
                    {
                        FirstNameAmh = "",
                        MiddleNameAmh = "",
                        LastNameAmh = "",
                        FirstNameEng = "",
                        MiddleNameEng = "",
                        LastNameEng = "",
                        TinNo = "",
                        HouseNo = "",
                        PhoneNo = "",
                        Message = "",
                        Error = "",
                        IsExist = false,
                        IsMerchant = false
                    });
                }
            }
        }
    }
}