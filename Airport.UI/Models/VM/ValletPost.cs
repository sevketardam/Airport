using System;

namespace Airport.UI.Models.VM
{
    public class ValletPost
    {
        public string Status { get; set; }
        public string PaymentStatus { get; set; }
        public string Hash { get; set; }
        public string PaymentCurrency { get; set; }
        public decimal PaymentAmount { get; set; }
        public string PaymentType { get; set; }
        public DateTime PaymentTime { get; set; }
        public string ConversationId { get; set; }
        public string OrderId { get; set; }
        public string ShopCode { get; set; }
        public decimal OrderPrice { get; set; }
        public decimal ProductsTotalPrice { get; set; }
        public string ProductType { get; set; }
        public string CallbackOkUrl { get; set; }
        public string CallbackFailUrl { get; set; }
    }
}
