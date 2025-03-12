using System;

namespace TestMcSonae.Models
{
    public class OrderResult
    {
        public Order Order { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        
        public static OrderResult Successful(Order order)
        {
            return new OrderResult
            {
                Order = order,
                Success = true
            };
        }
        
        public static OrderResult Failed(string errorMessage)
        {
            return new OrderResult
            {
                Success = false,
                ErrorMessage = errorMessage
            };
        }
    }
}