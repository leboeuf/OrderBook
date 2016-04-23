using OrderBook.Core.Model.Enums;
using System;

namespace OrderBook.Core.Model
{
    public class Order
    {
        public string Symbol { get; set; }
        public OrderSide OrderSide { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; } // scaled by 10000 (i.e. $34.25 => 342500)
        public DateTime Timestamp { get; set; }
        public Guid Reference { get; set; } // Used to uniquely identify an order, e.g. to notify when an order is executed

        public Order(string symbol, OrderSide orderSide, int quantity, int price)
        {
            Symbol = symbol;
            OrderSide = orderSide;
            Quantity = quantity;
            Price = price;
            Reference = new Guid();
            Timestamp = DateTime.UtcNow;
        }
    }
}
