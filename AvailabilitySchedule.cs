public class AvailabilitySchedule
{
    public int CarId { get; set; }
    private List<(DateTime StartDateTime, DateTime EndDateTime)> timePeriods { get; set; }

    public AvailabilitySchedule(int carId)
    {
        CarId = carId;
        timePeriods = new List<(DateTime, DateTime)>();
    }

    public void AddTimePeriod(DateTime startDateTime, DateTime endDateTime)
    {
        if (IsValidDate(startDateTime, endDateTime))
        {
            timePeriods.Add((startDateTime, endDateTime));
        }
        else
        {
            Console.WriteLine("Error: Invalid date. The start date must be before the end date, and both must be in the future.");
        }
    }

    public List<(DateTime StartDateTime, DateTime EndDateTime)> getTimePeriods()
    {
        return timePeriods;
    }

    public bool IsValidDate(DateTime startDateTime, DateTime endDateTime)
    {
        return startDateTime < endDateTime && startDateTime > DateTime.Now && endDateTime > DateTime.Now;
    }
}
