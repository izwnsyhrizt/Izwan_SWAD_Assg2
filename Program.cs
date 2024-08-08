using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Runtime.ConstrainedExecution;
using iCarRentalSystem;

class Program
{
    static void Main()
    {
        Console.WriteLine("Welcome to the iCar Rental System");

        // Create Renter
        Renter renter = new Renter(driversLicenseNo: "D1234567", userId: 1, name: "John Doe",
            contact: 1234567890, dob: new DateTime(1985, 5, 20), address: "561 Choa Chu Kang North 6");

        // Create iCarStation
        ICarStation station1 = new ICarStation(1, "iCar Station Jurong", "123 Jurong West", "66666666");

        // Create Car
        Car car1 = new Car(1, "Honda", "Civic", 2023, station1, 50.0);

        // Initialize AvailabilitySchedule and add time periods
        var availabilitySchedule = new AvailabilitySchedule(car1.CarId);
        //availabilitySchedule.AddTimePeriod(new DateTime(2024, 8, 1, 10, 0, 0), new DateTime(2024, 8, 7, 18, 0, 0));

        // Assign the schedule to the car
        car1.AvailabilitySchedule = availabilitySchedule;

        // Main loop
        while (true)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1. Book Car");
            Console.WriteLine("2. Exit");
            Console.Write("Choose an option: ");


            int option;
            while (!int.TryParse(Console.ReadLine(), out option) || option < 1 || option > 2)
            {
                Console.WriteLine("Invalid option. Please enter a number between 1 and 2.");
                Console.Write("Choose an option: ");
            }

            switch (option)
            {   
                case 1:
                    BookCar(car1, renter);
                    break;
                case 2:
                    Console.WriteLine("Exiting the system. Goodbye!");
                    return;
            }
        }
    }

    // Method to handle the car booking process
    static void BookCar(Car car, Renter renter)
    {
        Console.WriteLine("\nTaken timeslots:\n");
        foreach (var period in car.AvailabilitySchedule.GetTimePeriods())
        {
            Console.WriteLine($"{period.StartDateTime:yyyy-MM-dd HH:mm} - {period.EndDateTime:yyyy-MM-dd HH:mm}");
        }

        DateTime startDateTime;
        DateTime endDateTime;
        string pickupOption;

        EnterBookingDetails(out startDateTime, out endDateTime, out pickupOption);

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
        
        if (IsValidBooking(car, startDateTime, endDateTime))
        {
            double rentalRate = car.CurrentRate;
            double totalCost = CalculateTotalCost(startDateTime, endDateTime, rentalRate);

            Booking booking = new Booking(1, startDateTime, endDateTime, pickupOption, (int)totalCost);

            car.AvailabilitySchedule.AddTimePeriod(startDateTime, endDateTime);
            car.Bookings.Add(booking);

            Console.WriteLine("\nYour booking is confirmed!");
            Console.WriteLine("\n--- Booking Details ---");
            Console.WriteLine($"\nRenter Details:\n Name: {renter.Name}\n Contact: {renter.Contact}");
            Console.WriteLine($"\nCar Details:\n Make: {car.Make}\n Model: {car.Model}\n Year: {car.Year}\n Rate: ${car.CurrentRate}/day");
            Console.WriteLine($"\nBooking Details:\n Start Date and Time: {booking.StartDateTime:yyyy-MM-dd HH:mm}\n End Date and Time: {booking.EndDateTime:yyyy-MM-dd HH:mm}\n Pickup Option: {booking.PickupOption}\n Total Cost: ${booking.TotalCost}");

            // Display address or pickup location based on the pickup option
            if (pickupOption.ToLower() == "delivery")
            {
                Console.WriteLine($"Delivery Address: {renter.Address}");
            }
            else
            {
                Console.WriteLine($"Pickup Location: {car.ICarStation.name}, {car.ICarStation.address}");
            }
        }
        else
        {
            Console.WriteLine("Invalid booking period. The dates must not overlap with existing bookings.");
        }
    }




    private static void EnterBookingDetails(out DateTime startDateTime, out DateTime endDateTime, out string pickupOption)
    {
        string dateTimeFormat = "yyyy-MM-dd HH:mm";

        // Prompt user to enter start date and time
        Console.Write($"\nEnter start date and time ({dateTimeFormat}): ");
        while (!DateTime.TryParseExact(Console.ReadLine(), dateTimeFormat,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out startDateTime))
        {
            Console.WriteLine("Invalid start date and time format. Please try again.");
            Console.Write($"Enter start date and time ({dateTimeFormat}): ");
        }

        // Prompt user to enter end date and time
        Console.Write($"Enter end date and time ({dateTimeFormat}): ");
        while (!DateTime.TryParseExact(Console.ReadLine(), dateTimeFormat,
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out endDateTime))
        {
            Console.WriteLine("Invalid end date and time format. Please try again.");
            Console.Write($"Enter end date and time ({dateTimeFormat}): ");
        }

        // Prompt user to enter pickup option
        Console.Write("Enter pickup option (e.g. pickup/delivery): ");
        pickupOption = Console.ReadLine();
    }

    // Method to check if the booking period is valid
    private static bool IsValidBooking(Car car, DateTime startDateTime, DateTime endDateTime)
    {
        foreach (var period in car.AvailabilitySchedule.GetTimePeriods())
        {
            // Check if the proposed booking falls within an existing time period
            if (startDateTime < period.EndDateTime && endDateTime > period.StartDateTime)
            {
                return false; // Booking is invalid if it overlaps with any existing time period
            }
        }
        return true; // Booking is valid if it does not overlap with any existing time periods
    }

    // Method to calculate the total cost of the booking
    private static double CalculateTotalCost(DateTime startDateTime, DateTime endDateTime, double rate)
    {
        // Calculate duration of booking
        TimeSpan duration = endDateTime - startDateTime;
        // Calculate total cost
        double totalCost = rate * duration.TotalDays;
        return totalCost;
    }
}