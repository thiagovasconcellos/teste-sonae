using System;

namespace TestMcSonae.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        public string ProductDescription { get; set; }
        public float Value { get; set; }
        public float InStock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}