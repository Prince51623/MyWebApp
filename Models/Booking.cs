using System;
using System.ComponentModel.DataAnnotations;

namespace MyWebApp.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public int GuestId { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Check-in Date")]
        public DateTime CheckInDate { get; set; } = DateTime.Now.Date;
        
        [DataType(DataType.Date)]
        [Display(Name = "Check-out Date")]
        public DateTime CheckOutDate { get; set; } = DateTime.Now.Date.AddDays(1);
        
        public decimal TotalPrice { get; set; }
        public decimal TotalPriceInINR => TotalPrice * 83;
        
        // Status: Reserved, CheckedIn, CheckedOut, Cancelled
        public string Status { get; set; } = "Reserved";
        public bool IsCheckedIn { get; set; } = false;
        public bool IsCheckedOut { get; set; } = false;
        public DateTime? ActualCheckInDate { get; set; }
        public DateTime? ActualCheckOutDate { get; set; }
        public decimal AdditionalCharges { get; set; } = 0;
        public decimal FinalTotal => TotalPrice + AdditionalCharges;
        public decimal FinalTotalInINR => FinalTotal * 83;

        // Navigation properties
        public Guest? Guest { get; set; }
        public Room? Room { get; set; }
        
        public string DisplayStatus 
        {
            get 
            {
                if (IsCheckedOut) return "Checked Out";
                if (IsCheckedIn) return "Checked In";
                if (Status == "Cancelled") return "Cancelled";
                return "Reserved";
            }
        }
    }
}