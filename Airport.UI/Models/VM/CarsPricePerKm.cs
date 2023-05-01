namespace Airport.UI.Models.VM
{
    public class CarsPricePerKm
    {
        public int StartKm { get; set; }
        public int UpToKm { get; set; }
        public double Price { get; set; }
        public byte PriceType { get; set; }
    }
}
