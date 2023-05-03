using System.Collections.Generic;

namespace Airport.UI.Models.VM
{
    public class DistanceMatrixApiResponse
    {
        public List<string> destination_addresses { get; set; }
        public List<string> origin_addresses { get; set; }
        public List<Row> rows { get; set; }
        public string status { get; set; }
        public List<Geometry> Location { get; set; }
    }

    public class Geometry
    {
        public string Lat { get; set; }
        public string Lng { get; set; }
    }

    public class Row
    {
        public List<Element> elements { get; set; }
    }

    public class Element
    {
        public Distance distance { get; set; }
        public Duration duration { get; set; }
        public string status { get; set; }
    }

    public class Distance
    {
        public string text { get; set; }
        public int value { get; set; }
    }

    public class Duration
    {
        public string text { get; set; }
        public int value { get; set; }
    }


    public class AllDatas
    {
        public string Lng { get; set; }
        public string Lat { get; set; }
        public string DisanceValue { get; set; }
        public string DurationeValue { get; set; }
        public string Destinationaddresses { get; set; }
    }
}
