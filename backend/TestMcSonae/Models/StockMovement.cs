using System;

namespace TestMcSonae.Models
{
    public enum MovementType
    {
        Reservation,
        ReservationRelease,
        Sale,
        Return,
        Adjustment
    }

    public class StockMovement
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public float Quantity { get; set; }
        public MovementType Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}