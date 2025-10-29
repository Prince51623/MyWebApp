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

            _hotelService.AddGuest(guest);
            return RedirectToAction(nameof(Guests));
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
    }
}