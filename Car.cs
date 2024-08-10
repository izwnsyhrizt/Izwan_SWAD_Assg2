using iCarRentalSystem;

public class Car
{
    public int CarId { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public double Mileage { get; set; }
    public string Insurance { get; set; }
    public string Photos { get; set; }
    public RentalRate RentalRate { get; set; }
    public AvailabilitySchedule AvailabilitySchedule { get; set; }

    public Car(int carId, string make, string model, int year, double mileage, string insurance, string photos)
    {
        CarId = carId;
        Make = make;
        Model = model;
        Year = year;
        Mileage = mileage;
        Insurance = insurance;
        Photos = photos;
        RentalRate = null; // Initialize as null
        AvailabilitySchedule = null; // Initialize as null
    }

    public void SetRentalRate(RentalRate rentalRate)
    {
        if (rentalRate.CarId == this.CarId)
        {
            RentalRate = rentalRate;
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
            AvailabilitySchedule = schedule;
        }
        else
        {
            Console.WriteLine("Error: Schedule's CarId does not match this car.");
        }
    }

    public RentalRate getRentalRate()
    {
        return RentalRate;
    }

    public AvailabilitySchedule getAvailabilitySchedule()
    {
        return AvailabilitySchedule;
    }

    public override string ToString()
    {
        return $"Car ID: {CarId}, Make: {Make}, Model: {Model}, Year: {Year}, Mileage: {Mileage}, Insurance: {Insurance}, Photos: {Photos}";
    }
}
