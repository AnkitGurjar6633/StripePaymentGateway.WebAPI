namespace StripePaymentGateway.WebAPI.Models
{
    public class PaymentLinkRequestDTO
    {
        public string  PgName { get; set; }
        public string OrderID { get; set;}
        public decimal Amount { get; set; }
        public string Currency {  get; set; }
        public string CustomerName { get; set; }
        public string Mobile {  get; set; }
        public string Email { get; set; }
    }
}
