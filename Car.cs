using System;
using System.Collections.Generic;

namespace iCarRentalSystem
{
    public class Car
    {
        public int CarId { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public double Mileage { get; set; }
        public string Insurance { get; set; }
        public string PhotoPath { get; set; }
        public ICarStation iCarStation { get; set; }
        public RentalRate rentalRate;
        public AvailabilitySchedule availabilitySchedule;
        public List<Booking> bookings; // Added bookings list

        public Car(int carId, string make, string model, int year, double mileage, string insurance, string photoPath)
        {
            CarId = carId;
            Make = make;
            Model = model;
            Year = year;
            Mileage = mileage;
            Insurance = insurance;
            PhotoPath = photoPath;
            iCarStation = new ICarStation(1, "iCar Jurong", "Jurong Street 5", 1234567890);
            bookings = new List<Booking>(); // Initialize bookings list
        }

        public void SetRentalRate(RentalRate rentalRate)
        {
            if (rentalRate.CarId == this.CarId)
            {
                this.rentalRate = rentalRate;
            }
            else
            {
                Console.WriteLine("Error: RentalRate's CarId does not match this car.");
            }
        }

        public void SetAvailabilitySchedule(AvailabilitySchedule schedule)
        {
            if (schedule.CarId == this.CarId)
            {
                this.availabilitySchedule = schedule;
            }
            else
            {
                Console.WriteLine("Error: Schedule's CarId does not match this car.");
            }
        }

        public RentalRate getRentalRate()
        {
            return rentalRate;
        }

        public AvailabilitySchedule getAvailabilitySchedule()
        {
            return availabilitySchedule;
        }

        public void AddBooking(Booking booking)
        {
            if (booking == null)
            {
                Console.WriteLine("Error: Booking cannot be null.");
                return;
            }

            // Add the booking to the bookings list
            bookings.Add(booking);
        }

        public List<Booking> getBookings()
        {
            return bookings;
        }
        public override string ToString()
        {
            return $"Car ID: {CarId}, Make: {Make}, Model: {Model}, Year: {Year}, Mileage: {Mileage}, Insurance: {Insurance}, Photo: {PhotoPath}";
        }
    }
}
