This is a test assignment project for PrintMeAtServer.

The project can be launched using docker-compose project. It runs two containers:
1. printme_server: exposes API that accepts messages and schedules them.
2. printme_redis: runs redis instance that persists messages that are queued for further execution.

Example of API usage:
https://localhost:32786/api/printme?messageText=MyText&dateTime=2020-11-04T12:00:00

The messages are being printed at the scheduled time in the container console of the printme_server container.

The project also includes basic unittest and integration test coverage, primitive logging and exception handling infrastructure that would be expanded further if this were a real project :)