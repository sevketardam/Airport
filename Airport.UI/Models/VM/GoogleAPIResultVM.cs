namespace Airport.UI.Models.VM
{
    public class GoogleAPIResultVM
    {
        public string Place_id { get; set; }
        public string LocationName{ get; set; }
        public string LocationRadius{ get; set; }
        public string formatted_address { get; set; }
        public GoogleMapAPIGeometryVM Geometry { get; set; }
    }
}
