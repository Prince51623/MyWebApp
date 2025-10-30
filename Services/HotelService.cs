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

        public (bool success, string message) CheckInGuest(int bookingId)
        {
            if (!_bookings.TryGetValue(bookingId, out var booking))
            {
                return (false, "Booking not found.");
            }

            if (booking.Status == "Cancelled")
            {
                return (false, "Cannot check in a cancelled booking.");
            }

            if (booking.IsCheckedIn)
            {
                return (false, "Guest is already checked in.");
            }

            if (booking.IsCheckedOut)
            {
                return (false, "Guest has already checked out.");
            }

            // Verify check-in date
            if (DateTime.Now.Date < booking.CheckInDate.Date)
            {
                return (false, "Cannot check in before the scheduled check-in date.");
            }

            booking.IsCheckedIn = true;
            booking.Status = "CheckedIn";
            booking.ActualCheckInDate = DateTime.Now;
            
            return (true, "Guest checked in successfully.");
        }

        public (bool success, string message, decimal finalAmount) CheckOutGuest(int bookingId)
        {
            if (!_bookings.TryGetValue(bookingId, out var booking))
            {
                return (false, "Booking not found.", 0);
            }

            if (booking.Status == "Cancelled")
            {
                return (false, "Cannot check out a cancelled booking.", 0);
            }

            if (!booking.IsCheckedIn)
            {
                return (false, "Guest has not checked in yet.", 0);
            }

            if (booking.IsCheckedOut)
            {
                return (false, "Guest has already checked out.", 0);
            }

            booking.IsCheckedOut = true;
            booking.Status = "CheckedOut";
            booking.ActualCheckOutDate = DateTime.Now;
            
            var room = GetRoom(booking.RoomNumber);
            if (room != null)
            {
                room.IsAvailable = true;
            }

            // Calculate any late checkout fees
            if (DateTime.Now.Date > booking.CheckOutDate.Date)
            {
                int extraDays = (DateTime.Now.Date - booking.CheckOutDate.Date).Days;
                booking.AdditionalCharges += extraDays * (room?.PricePerNight ?? 0);
            }

            return (true, "Guest checked out successfully.", booking.FinalTotal);
        }

        public IEnumerable<Booking> GetBookingsForCheckIn()
        {
            var today = DateTime.Now.Date;
            return _bookings.Values.Where(b => 
                b.Status == "Reserved" && 
                !b.IsCheckedIn && 
                !b.IsCheckedOut && 
                b.CheckInDate.Date <= today);
        }

        public IEnumerable<Booking> GetCurrentlyStayingGuests()
        {
            return _bookings.Values.Where(b => 
                b.IsCheckedIn && 
                !b.IsCheckedOut && 
                b.Status == "CheckedIn");
        }

        public void AddAdditionalCharges(int bookingId, decimal amount, string description)
        {
            if (_bookings.TryGetValue(bookingId, out var booking))
            {
                booking.AdditionalCharges += amount;
            }
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
