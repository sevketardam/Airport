namespace Airport.UI.Models.VM
{
    public class GoogleAPIResultVM
    {
        public string Place_id { get; set; }
        public string LocationName{ get; set; }
        public string LocationRadius{ get; set; }
        public GoogleMapAPIGeometryVM Geometry { get; set; }
    }
}
