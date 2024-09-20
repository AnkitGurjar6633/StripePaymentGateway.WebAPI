using System.ComponentModel.DataAnnotations;

namespace StripePaymentGateway.WebAPI.Models
{
    public class OrderDetails
    {
        [Key]
        public string OrderID { get; set; }
        public string CustomerName { get; set; }
        public string Email {  get; set; }
        public string Mobile {  get; set; }
        public decimal Amount { get; set; }
        public string Currency {  get; set; }
        public string SessionID {  get; set; }
    }
}
