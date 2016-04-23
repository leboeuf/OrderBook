# OrderBook

A C# price-time order book matching engine.

## Prerequisites

OrderBook.Server and OrderBook.ServerEndpoint projects make use of *Microsoft Message Que (MSMQ) Server*. To enable this Windows feature, run `OptionalFeatures` at a command prompt and look for MSQM.

## Architecture

OrderBook.Server contains the OrderBook (the list of orders). Clients connect to OrderBook.ServerEndpoint, which transmits orders to the OrderBook.Server and broadcast notifications to clients. Clients never connect directly to OrderBook.Server for security and load concerns.

The OrderBook.Server queue expects to recieve messages of type `Order` and sends messages of type `ProcessedOrder`.

