# GoFish

An online game of GoFish for multiple players with a server being able to host multiple clients

## About

Language: C#.

This application consisted of a server, a WPF client and a shared library that both use, it used System.ServiceModel to preform two way communication between the server and client. Each client has a user interface to log in, create, join and leave game lobbies as well as start them. 2-6 clients can join the same game.

## Running

The GoFishService.exe launches the server that GoFish.exe clients must connect to. Connection is done by IP which is displayed in the server window when it is launched.
