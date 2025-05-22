using System;
namespace food_ordering_system.v2.Data.Models
{
    public class Payment
    {
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal AmountPaid { get; set; }
        public string PaymentMethod { get; set; }
        public DateTime PaymentDate { get; set; }
        // Default value changed to accommodate cash payments
        public string PaymentStatus { get; set; } = "Pending";
        // Navigation property (optional)
        public Order Order { get; set; }
    }
}