using System;
using TestMcSonae.Models;

namespace TestMcSonae.Events
{
    public class OrderEventArgs : EventArgs
    {
        public Order Order { get; }

        public OrderEventArgs(Order order)
        {
            Order = order;
        }
    }

    public static class OrderEvents
    {
        public static event EventHandler<OrderEventArgs> OrderExpired;

        public static void RaiseOrderExpired(Order order)
        {
            OrderExpired?.Invoke(null, new OrderEventArgs(order));
        }
    }
}