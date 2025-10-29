using System;

namespace MyWebApp.Models
{
    public class Room
    {
        public int RoomNumber { get; set; }
        public required string RoomType { get; set; }
        public decimal PricePerNight { get; set; }
        public decimal PriceInINR => PricePerNight * 83; // Conversion rate: 1 USD = 83 INR
        public bool IsAvailable { get; set; }
        public required string Description { get; set; }
    }
}