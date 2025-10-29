# Hotel Management System

A web-based hotel management system built with ASP.NET Core that helps manage hotel operations including room bookings, guest management, check-in, and check-out processes.

## Features

- Room Management
  - View all rooms and their details
  - Track room availability status

- Guest Management
  - Add new guests
  - View guest list
  - Track guest bookings

- Booking System
  - Create new bookings
  - View all bookings
  - Manage check-in and check-out processes
  - View guest-specific booking history

## Technology Stack

- ASP.NET Core
- C#
- MVC Architecture
- Bootstrap for frontend styling
- jQuery for client-side functionality

## Project Structure

- `Controllers/`: Contains all the MVC controllers
  - `HomeController.cs`: Handles main page routing
  - `HotelController.cs`: Manages hotel-related operations

- `Models/`: Data models
  - `Booking.cs`: Booking information
  - `Guest.cs`: Guest details
  - `Room.cs`: Room information

- `Views/`: MVC views for the user interface
  - `Hotel/`: Hotel management related views
  - `Shared/`: Shared layouts and partial views

- `Services/`: Business logic layer
  - `HotelService.cs`: Core hotel management functionality

## Setup and Installation

1. Ensure you have .NET 8.0 SDK installed
2. Clone the repository
3. Open the solution in Visual Studio
4. Build the solution
5. Run the application

## Usage

After running the application, you can:
- Navigate to the Rooms page to view available rooms
- Add new guests through the Add Guest page
- Create bookings for guests
- Manage check-ins and check-outs
- View booking history for specific guests

## Contributing

Feel free to submit issues and enhancement requests.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
