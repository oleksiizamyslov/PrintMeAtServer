This is a test assignment project for PrintMeAtServer.

The project can be launched using docker-compose project. It runs two containers:
1. printme_server: exposes API that accepts messages and schedules them.
2. printme_redis: runs redis instance that persists messages that are queued for further execution.

Example of API usage:
https://localhost:6378/api/printmeat?messageText=MyText&dateTime=2020-11-04T12:00:00

The messages will be scheduled for printing by API. The results of printing can be seen in the container console of the printme_server container.
The messages are persisted by redis. In case of container restart, the messages not yet processed will be read and rescheduled by API.

Some assumptions that are made by current implementation: 
- two scheduled messages with same datetime and text are considered different;
- the order of printing for two scheduled messages with same datetime is considered undefined;
- messages for the past datetime are printed immediately;

The project also includes basic unittest and integration test coverage, primitive logging, exception handling and validation infrastructure that would be expanded further if this were a real project :)