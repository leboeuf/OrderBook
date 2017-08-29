using Newtonsoft.Json;
using OrderBook.Core.Model;
using OrderBook.Core.Model.Enums;

namespace OrderBook.EndpointServer
{
    public static class JsonToOrderDeserializer
    {
        public static Order GetOrder(string json)
        {
            var typeDefinition = new { o = "", s = "", q = 0, p = 0 };
            var deserializedOrder = JsonConvert.DeserializeAnonymousType(json, typeDefinition);
            return new Order(deserializedOrder.s, deserializedOrder.o == "BUY" ? OrderSide.Buy : OrderSide.Sell, deserializedOrder.q, deserializedOrder.p);
        }
    }
}
