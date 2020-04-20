# CQRS Service Example

This is a product microservice to experiment CQRS design. Provides CRUD http api as an interface.

The Command and Query is split in `ProductService` (see `ProductServiceCQRSLib/Services/ProductService.cs` file).

Command part of the service basically stores all events in sqlite database and publishes events in message queue.

Query part builds and update dictionary based on received events and does not do caching.

If Query part of the service goes out of sync, it simple asks from command service (by means of message queue of course), to re-play the events.
