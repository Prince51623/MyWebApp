using Microsoft.AspNetCore.Mvc;
using MyWebApp.Models;
using MyWebApp.Services;

namespace MyWebApp.Controllers
{
    public class HotelController : Controller
    {
        private readonly HotelService _hotelService;

        public HotelController(HotelService hotelService)
        {
            _hotelService = hotelService;
        }

        // Room related actions
        public IActionResult Rooms()
        {
            var rooms = _hotelService.GetAllRooms();
            return View(rooms);
        }

        public IActionResult RoomDetails(int id)
        {
            var room = _hotelService.GetRoom(id);
            if (room == null)
                return NotFound();
            return View(room);
        }

        // Guest related actions
        public IActionResult Guests()
        {
            var guests = _hotelService.GetAllGuests();
            return View(guests);
        }

        public IActionResult AddGuest()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddGuest(Guest guest)
        {
            if (!ModelState.IsValid)
                return View(guest);

            try
            {
                _hotelService.AddGuest(guest);
                TempData["SuccessMessage"] = "Guest added successfully!";
                return RedirectToAction(nameof(Guests));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error adding guest. Please try again.");
                return View(guest);
            }
        }

        public IActionResult GuestBookings(int id)
        {
            var guest = _hotelService.GetGuest(id);
            if (guest == null)
                return NotFound();

            var bookings = _hotelService.GetGuestBookings(id);
            ViewBag.Guest = guest;
            return View(bookings);
        }

        // Booking related actions
        public IActionResult Bookings()
        {
            var bookings = _hotelService.GetAllBookings();
            return View(bookings);
        }

        public IActionResult CreateBooking(int? roomNumber = null)
        {
            ViewBag.AvailableRooms = roomNumber.HasValue
                ? _hotelService.GetAvailableRooms().Where(r => r.RoomNumber == roomNumber)
                : _hotelService.GetAvailableRooms();
            ViewBag.Guests = _hotelService.GetAllGuests();
            return View();
        }

        [HttpPost]
        public IActionResult CreateBooking(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AvailableRooms = _hotelService.GetAvailableRooms();
                ViewBag.Guests = _hotelService.GetAllGuests();
                return View(booking);
            }

            var newBooking = _hotelService.CreateBooking(booking);
            if (newBooking == null)
            {
                ModelState.AddModelError("", "Unable to create booking. Room might not be available.");
                ViewBag.AvailableRooms = _hotelService.GetAvailableRooms();
                ViewBag.Guests = _hotelService.GetAllGuests();
                return View(booking);
            }

            return RedirectToAction(nameof(Bookings));
        }

        [HttpPost]
        public IActionResult CancelBooking(int id)
        {
            var result = _hotelService.CancelBooking(id);
            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Bookings));
        }

        // Check-in related actions
        public IActionResult CheckIn()
        {
            try
            {
                var todayBookings = _hotelService.GetBookingsForCheckIn();
                return View(todayBookings);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while loading check-in data.";
                return View(Enumerable.Empty<Booking>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessCheckIn(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid booking ID.";
                    return RedirectToAction(nameof(CheckIn));
                }

                var booking = _hotelService.GetBooking(id);
                if (booking == null)
                {
                    TempData["ErrorMessage"] = "Booking not found.";
                    return RedirectToAction(nameof(CheckIn));
                }

                var (success, message) = _hotelService.CheckInGuest(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = message;
                }
                else
                {
                    TempData["SuccessMessage"] = message;
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred during check-in process.";
            }

            return RedirectToAction(nameof(CheckIn));
        }

        // Check-out related actions
        public IActionResult CheckOut()
        {
            try
            {
                var currentGuests = _hotelService.GetCurrentlyStayingGuests();
                return View(currentGuests);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while loading check-out data.";
                return View(Enumerable.Empty<Booking>());
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProcessCheckOut(int id)
        {
            try
            {
                if (id <= 0)
                {
                    TempData["ErrorMessage"] = "Invalid booking ID.";
                    return RedirectToAction(nameof(CheckOut));
                }

                var booking = _hotelService.GetBooking(id);
                if (booking == null)
                {
                    TempData["ErrorMessage"] = "Booking not found.";
                    return RedirectToAction(nameof(CheckOut));
                }

                var (success, message, finalAmount) = _hotelService.CheckOutGuest(id);
                if (!success)
                {
                    TempData["ErrorMessage"] = message;
                }
                else
                {
                    TempData["SuccessMessage"] = $"Check-out successful. Final amount: ${finalAmount:F2} (₹{finalAmount * 83:F2})";
                }
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred during check-out process.";
            }

            return RedirectToAction(nameof(CheckOut));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddCharges(int bookingId, decimal amount)
        {
            try
            {
                if (amount <= 0)
                {
                    TempData["ErrorMessage"] = "Amount must be greater than zero.";
                    return RedirectToAction(nameof(CheckOut));
                }

                _hotelService.AddAdditionalCharges(bookingId, amount, "Additional services");
                TempData["SuccessMessage"] = $"Additional charges of ${amount:F2} added successfully.";
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while adding charges.";
            }

            return RedirectToAction(nameof(CheckOut));
        }
    }
}
