using System;

namespace MyWebApp.Models
{
    public class CheckInInfo
    {
        public DateTime CheckInTime { get; set; }
        public string StaffMember { get; set; }
        public string Notes { get; set; }
    }
}