using System;

namespace MyWebApp.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public int GuestId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPriceInINR => TotalPrice * 83; // Conversion rate: 1 USD = 83 INR
        public string Status { get; set; } = "Pending";
        
        // Navigation properties
        public Guest? Guest { get; set; }
        public Room? Room { get; set; }
    }
}