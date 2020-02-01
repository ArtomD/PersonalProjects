# SocketedGrep

This project builds on [an existing project](https://github.com/ArtomD/PersonalProjects/tree/master/MultithreadGrep) and allows for two way communication between a client and server via sockets.

## About

Language: C++.

This project adapts a previous project that allowed for grepping a file system and lets a console client application query a separate server application that will then search itself locally by the supplied parameters and send the results back to the client. The client uses a custom console interface with a separate window for user input and server outputs. The server also accepts different commands allowing clients to connect to it, disconnect and remotely shut the server down.
The server adapts the previous project by introducing an input socket thread as well as an output socket thread so that it can communicate with the client in either direction while running.
The client runs an inbound and outbound socket thread as well as a UI thread. The UI lets users resize it for ease of use as well as scroll the server output up and down.


## Running

This project is made up of cpp and h file and must be complied to an exe to run. It includes a Visual Studio Solution file and can be opened form there. It uses a custom socket library that comes included in the project but can be re-built separately.
