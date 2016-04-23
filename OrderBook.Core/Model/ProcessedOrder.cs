using System;
using OrderBook.Core.Model.Enums;

namespace OrderBook.Core.Model
{
    public class ProcessedOrder
    {
        public string Symbol { get; set; }
        public OrderSide OrderSide { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public DateTime TimestampReceived { get; set; }
        public DateTime TimestampExecuted { get; set; }
        public Guid Reference { get; set; }

        public static ProcessedOrder CreateFromOrder(Order order)
        {
            return new ProcessedOrder
            {
                Symbol = order.Symbol,
                OrderSide = order.OrderSide,
                Price = order.Price,
                Quantity = order.Quantity,
                Reference = order.Reference,
                TimestampReceived = order.Timestamp,
                TimestampExecuted = DateTime.UtcNow
            };
        }
    }
}
