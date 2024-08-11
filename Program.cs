using iCarRentalSystem;
using System;
using System.Collections.Generic;
using System.Linq;

public class Program
{
    private static List<Car> cars = new List<Car>();
    private static Renter renter;
    private static Car selectedCar; // Store the selected car

    static void Main()
    {
        Console.WriteLine("Welcome to the iCar Rental System");

        // Create RentalRates
        RentalRate rentalRate1 = new RentalRate(1, 50.0); // CarId and Rate
        RentalRate rentalRate2 = new RentalRate(2, 60.0); // Another CarId and Rate

        // Create AvailabilitySchedules with hardcoded time periods
        AvailabilitySchedule availabilitySchedule1 = new AvailabilitySchedule(1); // CarId
        availabilitySchedule1.AddTimePeriod(new DateTime(2024, 09, 01, 0, 0, 0), new DateTime(2024, 09, 30, 23, 59, 59));
        AvailabilitySchedule availabilitySchedule2 = new AvailabilitySchedule(2); // Another CarId
        availabilitySchedule2.AddTimePeriod(new DateTime(2024, 09, 01, 0, 0, 0), new DateTime(2024, 09, 30, 23, 59, 59));

        // Create Cars
        Car car1 = new Car(1, "Honda", "Civic", 2023, 10000, "Full Coverage", "path/to/photo1.jpg");
        Car car2 = new Car(2, "Toyota", "Corolla", 2024, 5000, "Basic Coverage", "path/to/photo2.jpg");

        // Set Rental Rates and Availability Schedules
        car1.SetRentalRate(rentalRate1);
        car1.SetAvailabilitySchedule(availabilitySchedule1);

        car2.SetRentalRate(rentalRate2);
        car2.SetAvailabilitySchedule(availabilitySchedule2);

        // Add Cars to List
        cars.Add(car1);
        cars.Add(car2);

        // Create Renter
        renter = new Renter("D1234567", 1, "John Doe", 1234567890, new DateTime(1985, 5, 20), "561 Choa Chu Kang North 6");

        // Main loop
        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Show Cars");
            Console.WriteLine("2. Book Car");
            Console.WriteLine("3. Exit");
            Console.Write("Choose an option: ");

            int option;
            while (!int.TryParse(Console.ReadLine(), out option) || option < 1 || option > 3)
            {
                Console.WriteLine("Invalid option. Please enter a number between 1 and 3.");
                Console.Write("Choose an option: ");
            }

            switch (option)
            {
                case 1:
                    displayCars();
                    break;
                case 2:
                    bookCar();
                    break;
                case 3:
                    Console.WriteLine("Exiting system. Goodbye!");
                    return;
            }
        }
    }

    static void displayCars()
    {
        Console.WriteLine("\nAvailable Cars:");
        getCarDetails();
    }

    static void getCarDetails()
    {
        foreach (var car in cars)
        {
            var rentalRate = car.getRentalRate();
            Console.WriteLine($"Car ID: {car.CarId}, Make: {car.Make}, Model: {car.Model}, Year: {car.Year}, Rate: ${rentalRate?.Rate ?? 0}/day");
        }
    }

    static void selectCar()
    {
        Console.Write("\nEnter Car ID to book: ");
        int carId;
        while (!int.TryParse(Console.ReadLine(), out carId) || !findCar(carId))
        {
            Console.WriteLine("Invalid Car ID. Please enter a valid Car ID.");
            Console.Write("Enter Car ID to book: ");
        }
    }

    static bool findCar(int carId)
    {
        selectedCar = cars.Find(c => c.CarId == carId);
        return selectedCar != null;
    }

    static void displayTimePeriods()
    {
        var availabilitySchedule = selectedCar.getAvailabilitySchedule();
        Console.WriteLine($"\nAvailability Schedule for Car ID {selectedCar.CarId}:");
        foreach (var period in availabilitySchedule.getTimePeriods())
        {
            Console.WriteLine($"From: {period.StartDateTime} To: {period.EndDateTime}");
        }
    }

    static void bookCar()
    {
        // Show available cars before booking
        displayCars();

        selectCar();

        if (selectedCar == null)
        {
            Console.WriteLine("Car selection failed. No car found with the given ID.");
            return;
        }

        // Show availability schedule for the selected car
        displayTimePeriods();

        // Show existing bookings for the selected car
        displayCarBookings();

        // Prompt for booking details and get the entered details
        promptStartDateTime();
        DateTime startDateTime = enterStartDateTime();

        promptEndDateTime();
        DateTime endDateTime = enterEndDateTime();

        promptPickupOption();
        string pickupOption = enterPickupOption();

        if (isValidBooking(selectedCar, startDateTime, endDateTime, pickupOption))
        {
            double rentalRate = selectedCar.getRentalRate()?.Rate ?? 0;
            double totalCost = calculateTotalCost(startDateTime, endDateTime, rentalRate);
            totalCost = additionalCost(totalCost, pickupOption); // Add delivery cost if applicable

            // Create the booking
            Booking booking = createBooking(startDateTime, endDateTime, pickupOption, totalCost);

            // Add booking to car and renter
            addBooking(booking);

            // Display booking details
            Console.WriteLine("\nYour booking is confirmed!");
            Console.WriteLine($"Renter: {booking.Renter?.Name}\nCar ID: {booking.Car?.CarId}\n{booking.StartDateTime} - {booking.EndDateTime}" +
                              $"\nPickup Option: {booking.PickupOption}\nTotal Cost: ${booking.TotalCost}");

            // Display pickup or delivery address
            if (pickupOption.ToLower() == "delivery")
            {
                Console.WriteLine($"Delivery Address: {renter.Address}");
            }
            else
            {
                Console.WriteLine($"Pickup Location: {selectedCar.iCarStation.name}");
            }
        }
    }

    private static bool isValidBooking(Car car, DateTime startDateTime, DateTime endDateTime, string pickupOption)
    {
        var timePeriods = car.getAvailabilitySchedule().getTimePeriods();
        var bookings = car.getBookings();

        // Check if startDateTime and endDateTime are in the future and valid
        if (startDateTime <= DateTime.Now || endDateTime <= DateTime.Now || startDateTime >= endDateTime)
        {
            Console.WriteLine("Error: Start date must be before end date, and both must be in the future.");
            return false;
        }

        // Check if pickupOption is valid
        if (pickupOption.ToLower() != "pickup" && pickupOption.ToLower() != "delivery")
        {
            Console.WriteLine("Error: Pickup option must be either 'pickup' or 'delivery'.");
            return false;
        }

        // Check if the booking is within the availability schedule
        bool isWithinSchedule = timePeriods.Any(period => startDateTime >= period.StartDateTime && endDateTime <= period.EndDateTime);
        if (!isWithinSchedule)
        {
            Console.WriteLine("Error: Booking period must be within the available time periods.");
            return false;
        }

        // Check for overlapping bookings
        foreach (var booking in bookings)
        {
            if (startDateTime < booking.EndDateTime && endDateTime > booking.StartDateTime)
            {
                Console.WriteLine("Error: The dates overlap with an existing booking.");
                return false; // Overlaps with existing bookings
            }
        }

        return true; // Valid if all checks pass
    }

    private static double calculateTotalCost(DateTime startDateTime, DateTime endDateTime, double rate)
    {
        TimeSpan duration = endDateTime - startDateTime;
        double totalCost = rate * duration.TotalDays;
        return totalCost;
    }

    private static double additionalCost(double totalCost, string pickupOption)
    {
        if (pickupOption.ToLower() == "delivery")
        {
            totalCost += 50; // Add $50 for delivery
        }
        return totalCost;
    }

    private static Booking createBooking(DateTime startDateTime, DateTime endDateTime, string pickupOption, double totalCost)
    {
        return new Booking(1, startDateTime, endDateTime, pickupOption, (int)totalCost, selectedCar, renter);
    }

    private static void addBooking(Booking booking)
    {
        // Add booking to car's bookings list and update availability schedule
        selectedCar.bookings.Add(booking);

        // Add booking to renter's bookings list
        renter.BookList.Add(booking);
    }

    // Prompt methods
    static void promptStartDateTime()
    {
        Console.Write("\nEnter start date and time (yyyy-MM-dd HH:mm): ");
    }

    static void promptEndDateTime()
    {
        Console.Write("Enter end date and time (yyyy-MM-dd HH:mm): ");
    }

    static void promptPickupOption()
    {
        Console.Write("Enter pickup option (pickup/delivery): ");
    }

    // Enter methods
    static DateTime enterStartDateTime()
    {
        return DateTime.Parse(Console.ReadLine());
    }

    static DateTime enterEndDateTime()
    {
        return DateTime.Parse(Console.ReadLine());
    }

    static string enterPickupOption()
    {
        return Console.ReadLine();
    }

    static void displayCarBookings()
    {
        var bookings = selectedCar.getBookings();
        if (bookings.Count == 0)
        {
            Console.WriteLine($"\nNo existing bookings for Car ID {selectedCar.CarId}.");
        }
        else
        {
            Console.WriteLine($"\nExisting bookings for Car ID {selectedCar.CarId}:");
            foreach (var booking in bookings)
            {
                Console.WriteLine($"Booking ID: {booking.BookingId}, Start: {booking.StartDateTime}, End: {booking.EndDateTime}");
            }
        }
    }

}

