using System.Collections.Generic;

namespace OrderBook.Core.Model
{
    public class OrderBook
    {
        public List<Order> Orders { get; set; }

        public OrderBook()
        {
            Orders = new List<Order>();
        }
    }
}
