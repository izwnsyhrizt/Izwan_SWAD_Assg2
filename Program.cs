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

        // Create AvailabilitySchedules
        AvailabilitySchedule availabilitySchedule1 = new AvailabilitySchedule(1); // CarId
        AvailabilitySchedule availabilitySchedule2 = new AvailabilitySchedule(2); // Another CarId

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
        Console.Write("Enter Car ID to book: ");
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

    static void promptBookingDetails()
    {
        var (startDateTime, endDateTime, pickupOption) = enterBookingDetails();

        if (startDateTime >= endDateTime)
        {
            Console.WriteLine("Invalid booking period. The start date must be before the end date.");
            return;
        }

        if (startDateTime < DateTime.Now || endDateTime < DateTime.Now)
        {
            Console.WriteLine("Invalid booking period. Both dates must be in the future.");
            return;
        }

        if (isValidBooking(selectedCar, startDateTime, endDateTime))
        {
            double rentalRate = selectedCar.getRentalRate()?.Rate ?? 0;
            double totalCost = calculateTotalCost(startDateTime, endDateTime, rentalRate);

            // Create and add booking with Car and Renter
            Booking booking = new Booking(1, startDateTime, endDateTime, pickupOption, (int)totalCost, selectedCar, renter);
            selectedCar.getAvailabilitySchedule().AddTimePeriod(startDateTime, endDateTime);
            renter.BookList.Add(booking);

            // Display booking details
            Console.WriteLine("\nYour booking is confirmed!");
            Console.WriteLine($"Booking ID: {booking.BookingId}, Start: {booking.StartDateTime}, End: {booking.EndDateTime}, " +
                              $"Pickup Option: {booking.PickupOption}, Total Cost: ${booking.TotalCost}, Car ID: {booking.Car?.CarId}, Renter: {booking.Renter?.Name}");

            // Display pickup or delivery address
            if (pickupOption.ToLower() == "delivery")
            {
                Console.WriteLine($"Delivery Address: {renter.Address}");
            }
            else
            {
                Console.WriteLine($"Pickup Location: {selectedCar.getAvailabilitySchedule().CarId}"); // Adjust according to your requirement
            }
        }
        else
        {
            Console.WriteLine("Invalid booking period. The dates must not overlap with existing bookings.");
        }
    }

    static (DateTime startDateTime, DateTime endDateTime, string pickupOption) enterBookingDetails()
    {
        Console.Write("Enter start date and time (yyyy-MM-dd HH:mm): ");
        DateTime startDateTime = DateTime.Parse(Console.ReadLine());

        Console.Write("Enter end date and time (yyyy-MM-dd HH:mm): ");
        DateTime endDateTime = DateTime.Parse(Console.ReadLine());

        Console.Write("Enter pickup option (pickup/delivery): ");
        string pickupOption = Console.ReadLine();

        return (startDateTime, endDateTime, pickupOption);
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

        // Prompt for booking details
        Console.Write("Enter start date and time (yyyy-MM-dd HH:mm): ");
        DateTime startDateTime = DateTime.Parse(Console.ReadLine());
        Console.Write("Enter end date and time (yyyy-MM-dd HH:mm): ");
        DateTime endDateTime = DateTime.Parse(Console.ReadLine());
        Console.Write("Enter pickup option (pickup/delivery): ");
        string pickupOption = Console.ReadLine();

        if (startDateTime >= endDateTime)
        {
            Console.WriteLine("Invalid booking period. The start date must be before the end date.");
            return;
        }

        if (startDateTime < DateTime.Now || endDateTime < DateTime.Now)
        {
            Console.WriteLine("Invalid booking period. Both dates must be in the future.");
            return;
        }

        if (isValidBooking(selectedCar, startDateTime, endDateTime))
        {
            double rentalRate = selectedCar.getRentalRate()?.Rate ?? 0;
            double totalCost = calculateTotalCost(startDateTime, endDateTime, rentalRate);

            // Create and add booking with Car and Renter
            Booking booking = new Booking(1, startDateTime, endDateTime, pickupOption, (int)totalCost, selectedCar, renter);
            selectedCar.getAvailabilitySchedule().AddTimePeriod(startDateTime, endDateTime);
            renter.BookList.Add(booking);

            // Display booking details
            Console.WriteLine("\nYour booking is confirmed!");
            Console.WriteLine($"Booking ID: {booking.BookingId}, Start: {booking.StartDateTime}, End: {booking.EndDateTime}, " +
                              $"Pickup Option: {booking.PickupOption}, Total Cost: ${booking.TotalCost}, Car ID: {booking.Car?.CarId}, Renter: {booking.Renter?.Name}");

            // Display pickup or delivery address
            if (pickupOption.ToLower() == "delivery")
            {
                Console.WriteLine($"Delivery Address: {renter.Address}");
            }
            else
            {
                var schedule = selectedCar.getAvailabilitySchedule();
                Console.WriteLine($"Pickup Location: {schedule?.CarId}");
            }
        }
        else
        {
            Console.WriteLine("Invalid booking period. The dates must not overlap with existing bookings.");
        }
    }

    private static bool isValidBooking(Car car, DateTime startDateTime, DateTime endDateTime)
    {
        foreach (var period in car.getAvailabilitySchedule().getTimePeriods())
        {
            if (startDateTime < period.EndDateTime && endDateTime > period.StartDateTime)
            {
                return false; // Overlaps with existing bookings
            }
        }
        return true; // Valid if it does not overlap with any existing time periods
    }

    private static double calculateTotalCost(DateTime startDateTime, DateTime endDateTime, double rate)
    {
        TimeSpan duration = endDateTime - startDateTime;
        double totalCost = rate * duration.TotalDays;
        return totalCost;
    }
}
