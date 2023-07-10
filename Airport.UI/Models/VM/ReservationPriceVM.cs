namespace Airport.UI.Models.VM
{
    public class ReservationPriceVM
    {
        public double LastPrice { get; set; } = 0;
        public double OfferPrice { get; set; } = 0;
        public double ServiceFee { get; set; } = 0;
        public double ExtraServiceFee { get; set; } = 0;
        public double SalesFee { get; set; } = 0;
        public double DiscountPrice { get; set; } = 0;
        public double SpecialPrice { get; set; } = 0;
        public double DiscountRate { get; set; } = 0;
        public double PartnerFee { get; set; } = 0;
        public double GlobalPartnerFee { get; set; } = 0;
        public double DiscountServiceFee { get; set; } = 0;
        public double DiscountOfferPrice { get; set; } = 0;
        public double DiscountExtraService { get; set; } = 0;
    }
}
