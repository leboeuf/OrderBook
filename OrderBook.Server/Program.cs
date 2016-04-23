using OrderBook.Core.Model;
using OrderBook.Core.Model.Enums;

namespace OrderBook.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderBookForSomeStock = new Core.Model.OrderBook();
            orderBookForSomeStock.Orders.Add(new Order(OrderSide.Sell, 100, 32));

            // wait for orders
                // try matching
                    // put in order book if not matched
                    // delete from orders if matched and broadcast the trade to everyone
            // at the end of trading day, close orders depending on "good til x"
        }
    }
}
