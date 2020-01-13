/*
*Author: Artom Didinchuk
*Date: 2019-11-05
*Version 1.0
*These are the inbound and outbound socket library classes
*/
#pragma once

#define _WINSOCK_DEPRECATED_NO_WARNINGS
#include <WinSock2.h>
#include <WS2tcpip.h>
#include <iostream>
#include <string>
#include <errno.h>

#pragma comment (lib,"ws2_32.lib")

namespace sockets {
	//enums used to classify commands
	enum Command { Standby, Connect, Exit, IncomingBool, IncomingInt, IncomingString, Params, ShutdownServer};
	//command objects to send
	struct CommandData {
		Command command;
		int size;
		char* data;
	};
	//inbound socket class
	//This socket binds on any avaliable address at the supplied port.
	class InboundSocket {
		SOCKET hSocket;
		SOCKET hAccepted;
	public:
		
		InboundSocket(short PORT);
		/*
		starts listening to an incoming connection
		this is a blocking function and will wait for an incoming connection
		*/
		void acceptConnection();
		/*
		returns the address the socket is bound to
		*/
		std::string getLocalAddress();
		/*
		parses incomingdata into a command object
		*/
		CommandData receiveCommand();

		~InboundSocket();
	};

	class OutboundSocket {
	private:
		SOCKET hSocket;
		//helper methods used to send primitives across the socket
		/*
		sends an int across to the listening socket
		*/
		void sendIntToSocket(int payload);
		/*
		sends a bool across to the listening socket
		*/
		void sendBoolToSocket(bool payload);
	public:
		OutboundSocket(short PORT);
		/*
		attempts to connect to the specefied IP address and port
		returns true if connection is accepted, false otherwise
		*/
		bool attemptConnection(std::string IPAddressString, short PORT);
		/*
		returns the address the socket is bound to
		*/
		std::string getLocalAddress();

		//sends a command object containing the requested payload to the listening socket
		/*
		Sens a bool across to the listening socket.
		*/
		void sendPayload(bool payload);
		/*
		Sends an int across to the listening socket.
		*/
		void sendPayload(int payload);
		/*
		Sends a string across to the listening socket.
		There is no max length set but longer strings will degrade preformance
		*/
		void sendPayload(std::string payload);
		/*
		sends a connection request to the listening socket with ip address and port need to connect back to itself
		*/
		void openConnection(std::string, short port);
		/*
		Sends all the params needed to make a grep search
		*/
		void sendParams(bool verbse, std::string path, std::string regex, std::string extensions);
		/*
		sends a command object with a close request
		*/
		void closeConnection();
		/*
		sends a command object with a shutdown server request
		*/
		void shutdownServer();

		~OutboundSocket();
	};
}