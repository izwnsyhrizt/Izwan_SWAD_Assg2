using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iCarRentalSystem;

public class Car
{
    public int CarId { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public ICarStation ICarStation { get; set; }

    public AvailabilitySchedule AvailabilitySchedule { get; set; }
    public RentalRate RentalRate { get; set; }
    public List<Booking> Bookings { get; set; }
    public double CurrentRate
    {
        get { return RentalRate?.Rate ?? 0; } // Retrieve the rate from RentalRate or return 0 if RentalRate is null
        set { RentalRate = new RentalRate(value); } // Update the rate by creating a new RentalRate object
    }
    public List<AvailabilitySchedule> AvailabilitySchedules { get; set; } = new List<AvailabilitySchedule>();

    // Static list to hold all car instances
    private static List<Car> cars = new List<Car>();

    public Car(int carId, string make, string model, int year, ICarStation iCarStation, double rentalRate)
    {
        this.CarId = carId;
        this.Make = make;
        this.Model = model;
        this.Year = year;
        this.ICarStation = iCarStation;
        this.AvailabilitySchedule = new AvailabilitySchedule(carId);
        this.RentalRate = new RentalRate(rentalRate); // Initialize RentalRate with the rate
        this.Bookings = new List<Booking>();
        cars.Add(this); // Add new car to static list
    }

    public List<double> GetCurrentRates()
    {
        // Return a list with the current rate
        return new List<double> { CurrentRate };
    }

    public List<AvailabilitySchedule> GetCurrentSchedules()
    {
        // Return the list of availability schedules
        return AvailabilitySchedules;
    }

    public override string ToString()
    {
        return $"Car ID: {CarId}, Make: {Make}, Model: {Model}, Year: {Year}, Rate: {RentalRate?.Rate}, Availability: {AvailabilitySchedule}";
    }
}
