using MyWebApp.Models;
using System.Collections.Concurrent;

namespace MyWebApp.Services
{
    public class HotelService
    {
        private readonly ConcurrentDictionary<int, Room> _rooms = new();
        private readonly ConcurrentDictionary<int, Guest> _guests = new();
        private readonly ConcurrentDictionary<int, Booking> _bookings = new();
        private int _nextGuestId = 1;
        private int _nextBookingId = 1;

        public HotelService()
        {
            // Initialize some sample rooms
            InitializeRooms();
        }

        private void InitializeRooms()
        {
            var rooms = new[]
            {
                new Room { RoomNumber = 101, RoomType = "Standard", PricePerNight = 100.00m, IsAvailable = true, Description = "Standard Room with Single Bed" },
                new Room { RoomNumber = 102, RoomType = "Standard", PricePerNight = 100.00m, IsAvailable = true, Description = "Standard Room with Single Bed" },
                new Room { RoomNumber = 201, RoomType = "Deluxe", PricePerNight = 200.00m, IsAvailable = true, Description = "Deluxe Room with Double Bed" },
                new Room { RoomNumber = 202, RoomType = "Deluxe", PricePerNight = 200.00m, IsAvailable = true, Description = "Deluxe Room with Double Bed" },
                new Room { RoomNumber = 301, RoomType = "Suite", PricePerNight = 300.00m, IsAvailable = true, Description = "Luxury Suite with King Bed" }
            };

            foreach (var room in rooms)
            {
                _rooms.TryAdd(room.RoomNumber, room);
            }
        }

        // Room operations
        public IEnumerable<Room> GetAllRooms() => _rooms.Values;
        public Room? GetRoom(int roomNumber) => _rooms.GetValueOrDefault(roomNumber);
        public IEnumerable<Room> GetAvailableRooms() => _rooms.Values.Where(r => r.IsAvailable);

        // Guest operations
        public Guest AddGuest(Guest guest)
        {
            guest.Id = _nextGuestId++;
            _guests.TryAdd(guest.Id, guest);
            return guest;
        }
        
        public Guest? GetGuest(int id) => _guests.GetValueOrDefault(id);
        public IEnumerable<Guest> GetAllGuests() => _guests.Values;

        // Booking operations
        public Booking? CreateBooking(Booking booking)
        {
            var room = GetRoom(booking.RoomNumber);
            if (room == null || !room.IsAvailable) return null;

            booking.Id = _nextBookingId++;
            booking.Room = room;
            booking.Guest = GetGuest(booking.GuestId);
            booking.TotalPrice = CalculateTotalPrice(booking.CheckInDate, booking.CheckOutDate, room.PricePerNight);
            
            if (_bookings.TryAdd(booking.Id, booking))
            {
                room.IsAvailable = false;
                return booking;
            }
            
            return null;
        }

        public bool CancelBooking(int bookingId)
        {
            if (_bookings.TryGetValue(bookingId, out var booking))
            {
                booking.Status = "Cancelled";
                var room = GetRoom(booking.RoomNumber);
                if (room != null)
                {
                    room.IsAvailable = true;
                }
                return true;
            }
            return false;
        }

        public IEnumerable<Booking> GetAllBookings() => _bookings.Values;
        public Booking? GetBooking(int id) => _bookings.GetValueOrDefault(id);
        public IEnumerable<Booking> GetGuestBookings(int guestId) => 
            _bookings.Values.Where(b => b.GuestId == guestId);

        private static decimal CalculateTotalPrice(DateTime checkIn, DateTime checkOut, decimal pricePerNight)
        {
            var nights = (checkOut - checkIn).Days;
            return nights * pricePerNight;
        }
    }
}