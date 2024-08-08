public class ICarStation
{
    public int stationId { get; set; }
    public string name { get; set; }
    public string address { get; set; }
    public string contact { get; set; }

    public ICarStation(int stationId, string name, string address, string contactNumber)
    {
        this.stationId = stationId;
        this.name = name;
        this.address = address;
        this.contact = contact;
    }

    public override string ToString()
    {
        return $"Station ID: {stationId}, Name: {name}, Address: {address}, Contact: {contact}";
    }
}
