using Airport.DBEntities.Entities;

namespace Airport.UI.Models.VM
{
    public class UpdateServiceVM
    {
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public int[] ServiceItems { get; set; }
    }
}
